// This used to be BAPSPresenterMainReactions_config.cpp.

using System;
using BAPSPresenter;

namespace BAPSPresenter2
{
    partial class Main
    {
        /** All these functions firstly retrieve all the data the command needs and then (if possible)
	        update the configDialog... exceptions to the rule exist... read on
        **/

        void processChoice(uint optionid, uint choiceIndex, string choiceDescription)
        {
            /** Cache this info **/
            ConfigCache.addOptionChoice((int)optionid, (int)choiceIndex, choiceDescription);
            /** Ignore if the config dialog is closed **/
            var cd = configDialog;
            if (cd == null) return;
            try
            {
                cd.closeMutex.WaitOne();
                if (cd.Visible)
                {
                    cd.Invoke((Action<object, object, string>)configDialog.addChoice, optionid, choiceIndex, choiceDescription);
                }
            }
            catch (Exception e)
            {
                var error = string.Concat("Failed to process choice:\n", e.Message, "\nStack Trace:\n", e.StackTrace);
                logError(error);
            }
            finally
            {
                cd.closeMutex.ReleaseMutex();
            }
        }

        void processChoiceCount(uint optionid, uint count)
        {
            /** Ignore if the config dialog is closed **/
            var cd = configDialog;
            if (cd == null) return;
            try
            {
                cd.closeMutex.WaitOne();
                if (cd.Visible)
                {
                    cd.Invoke((Action<object, object>)configDialog.setChoiceCount, optionid, count);
                }
            }
            catch (Exception e)
            {
                var error = string.Concat("Failed to process choice count:\n", e.Message, "\nStack Trace:\n", e.StackTrace);
                logError(error);
            }
            finally
            {
                cd.closeMutex.ReleaseMutex();
            }
        }

        void processOption(Command cmdReceived, uint optionid, string description, uint type)
        {
            /** Cache this info **/
            ConfigCache.addOptionDescription((int)optionid, (int)type, description, (cmdReceived & Command.CONFIG_USEVALUEMASK) != 0);
            /** Pass onto the config dialog if available **/
            var cd = configDialog;
            if (cd == null) return;
            try
            {
                cd.closeMutex.WaitOne();
                if (cd.Visible)
                {
                    /** Check for an indexed option **/
                    if (cmdReceived.IsFlagSet(Command.CONFIG_USEVALUEMASK))
                    {
                        /** Indexed option - does not update the form UI just data **/
                        var indexid = (ushort)(cmdReceived & Command.CONFIG_VALUEMASK);
                        cd.addOption(new ConfigOptionInfo((int)optionid, description, (int)type, indexid));
                    }
                    else
                    {
                        /** Non indexed option - does not update the form ui just data **/
                        cd.addOption(new ConfigOptionInfo((int)optionid, description, (int)type));
                    }
                    /** The configDialog form knows how many options it is expecting and
                        will report true when it has them all, at this point it is able to
                        draw itself
                    **/
                    if (cd.isReadyToShow())
                    {
                        /** Tell the form to draw its controls **/
                        cd.Invoke((Action)configDialog.updateUI);
                    }
                }
            }
            catch (Exception e)
            {
                var error = string.Concat("Failed to process option:\n", e.Message, "\nStack Trace:\n", e.StackTrace);
                logError(error);
            }
            finally
            {
                cd.closeMutex.ReleaseMutex();
            }
        }

        void processOptionCount(uint count)
        {
            /** Let the config dialog know how many options to expect **/
            var cd = configDialog;
            if (cd == null) return;
            try
            {
                cd.closeMutex.WaitOne();
                if (cd.Visible)
                {
                    cd.setNumberOfOptions((int)count);
                }
            }
            catch (Exception e)
            {
                var error = string.Concat("Failed to process option count:\n", e.Message, "\nStack Trace:\n", e.StackTrace);
                logError(error);
            }
            finally
            {
                cd.closeMutex.ReleaseMutex();
            }
        }

        void processConfigSetting(Command cmdReceived, uint optionid, ConfigType type)
        {
            uint valueInt = 0;
            string valueStr = null;
            /** Determine what the final argument is going to be and retrieve it **/
            switch (type)
            {
                case ConfigType.INT:
                case ConfigType.CHOICE:
                    valueInt = clientSocket.receiveI();
                    break;
                case ConfigType.STR:
                    valueStr = clientSocket.receiveS();
                    break;
                default:
                    {
                        sendQuit("Invalid type received in processConfigSetting", false);
                    }
                    break;
            }

            /** Cache this setting **/
            /** Use index=-1 to represent a non indexed setting **/
            int index = -1;
            if (cmdReceived.IsFlagSet(Command.CONFIG_USEVALUEMASK))
            {
                index = (int)(cmdReceived & Command.CONFIG_VALUEMASK);
            }
            switch (type)
            {
                case ConfigType.INT:
                case ConfigType.CHOICE:
                    ConfigCache.addOptionValue((int)optionid, index, (int)valueInt);
                    break;
                case ConfigType.STR:
                    ConfigCache.addOptionValue((int)optionid, index, valueStr);
                    break;
            }
            /** 
                In order to support fetching single config options for use in normal
                operation and also to retrieve all options (to change their values)
                We test if the configDialog handle is null, if it is we assume the
                config setting is intended for the cache.
            **/
            var cd = configDialog;
            if (cd != null)
            {
                try
                {
                    cd.closeMutex.WaitOne();
                    if (cd.Visible)
                    {
                        /** If Config Dialog is visible at this point then it cannot close as we hold the closeMutex
                            this means we no longer have to use our copy of the objects handle (cd) **/
                        /** THE ABOVE STATEMENT IS NOT TRUE! We may receive a value after the form has closed
                            but not yet know it is closed. We just catch the exception and continue! nasty
                        **/
                        /** If the value mask is used it means that the setting is for an indexed
                            option and the specified index is in the value
                        **/
                        if (cmdReceived.IsFlagSet(Command.CONFIG_USEVALUEMASK))
                        {
                            switch (type)
                            {
                                case ConfigType.INT:
                                case ConfigType.CHOICE:
                                    {
                                        /** Box it up and send it off, choices can be treated as
                                            just ints because that is the underlying datatype
                                        **/
                                        configDialog.Invoke((Action<object, object, object>)configDialog.setValue, optionid, index, valueInt);
                                    }
                                    break;
                                case ConfigType.STR:
                                    {
                                        configDialog.Invoke((Action<object, object, object>)configDialog.setValue, optionid, index, valueStr);
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            /** Non indexed settings, just box them up and send them off **/
                            switch (type)
                            {
                                case ConfigType.INT:
                                case ConfigType.CHOICE:
                                    {
                                        configDialog.Invoke((Action<object, object>)configDialog.setValue, optionid, valueInt);
                                    }
                                    break;
                                case ConfigType.STR:
                                    {
                                        configDialog.Invoke((Action<object, object>)configDialog.setValue, optionid, valueStr);
                                    }
                                    break;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    var error = string.Concat("Failed to process config setting: ", optionid.ToString(), ":\n", e.Message, "\nStack Trace:\n", e.StackTrace);
                    logError(error);
                }
                finally
                {
                    cd.closeMutex.ReleaseMutex();
                }
            }
        }

        void processConfigResult(Command cmdReceived, uint optionid, uint result)
        {
            /** We receive a result for every config setting we try to update **/
            /** Only report these if the form is still open, it is a serious error, if
                we receive one of these when the form is closing **/
            if (configDialog == null) return;
            // NB: indexes don't seem to be used here.
            try
            {
                configDialog.Invoke((Action<object, object>)configDialog.setResult, optionid, result);
            }
            catch (Exception e)
            {
                var error = string.Concat("Failed to process config result:\n", e.Message, "\nStack Trace:\n", e.StackTrace);
                logError(error);
            }
        }

        void processConfigError(uint errorCode, string description)
        {
            if (securityDialog == null) return;
            securityDialog.Invoke((Action<object, string>)securityDialog.receiveConfigError, errorCode, description);
        }

        void processUserInfo(string username, uint permissions)
        {
            if (securityDialog == null) return;
            securityDialog.Invoke((Action<string, object>)securityDialog.receiveUserInfo, username, permissions);
        }

        void processUserCount(uint userCount)
        {
            if (securityDialog == null) return;
            securityDialog.Invoke((Action<object>)securityDialog.receiveUserCount, userCount);
        }

        void processUserResult(uint resultCode, string description)
        {
            if (securityDialog == null) return;
            securityDialog.Invoke((Action<object, string>)securityDialog.receiveUserResult, resultCode, description);
        }

        void processPermissionInfo(uint permissionCode, string description)
        {
            if (securityDialog == null) return;
            securityDialog.Invoke((Action<object, string>)securityDialog.receivePermissionInfo, permissionCode, description);
        }

        void processPermissionCount(uint permissionCount)
        {
            if (securityDialog == null) return;
            securityDialog.Invoke((Action<object>)securityDialog.receivePermissionCount, permissionCount);
        }

        void processIPRestrictionCount(Command cmd, uint count)
        {
            if (securityDialog == null) return;
            if (cmd.IsFlagSet(Command.CONFIG_USEVALUEMASK))
            {
                securityDialog.Invoke((Action<object>)securityDialog.receiveIPDenyCount, count);
            }
            else
            {
                securityDialog.Invoke((Action<object>)securityDialog.receiveIPAllowCount, count);
            }
        }

        void processIPRestriction(Command cmd, string ipaddress, uint mask)
        {
            if (securityDialog == null) return;
            if (cmd.IsFlagSet(Command.CONFIG_USEVALUEMASK))
            {
                securityDialog.Invoke((Action<string, object>)securityDialog.receiveIPDeny, ipaddress, mask);
            }
            else
            {
                securityDialog.Invoke((Action<string, object>)securityDialog.receiveIPAllow, ipaddress, mask);
            }
        }
    }
}