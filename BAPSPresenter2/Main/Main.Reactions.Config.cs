// This used to be BAPSPresenterMainReactions_config.cpp.

using System;
using BAPSCommon;
using BAPSPresenter;

namespace BAPSPresenter2
{
    partial class Main
    {
        /** All these functions firstly retrieve all the data the command needs and then (if possible)
	        update the configDialog... exceptions to the rule exist... read on
        **/

        private void processChoice(uint optionid, uint choiceIndex, string choiceDescription)
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
                    cd.Invoke((Action<uint, int, string>)configDialog.addChoice, optionid, (int)choiceIndex, choiceDescription);
                }
            }
#if !DEBUG
            catch (Exception e)
            {
                var error = string.Concat("Failed to process choice:\n", e.Message, "\nStack Trace:\n", e.StackTrace);
                logError(error);
            }
#endif
            finally
            {
                cd.closeMutex.ReleaseMutex();
            }
        }

        private void processChoiceCount(uint optionid, int count)
        {
            /** Ignore if the config dialog is closed **/
            var cd = configDialog;
            if (cd == null) return;
            try
            {
                cd.closeMutex.WaitOne();
                if (cd.Visible)
                {
                    cd.Invoke((Action<uint, int>)configDialog.setChoiceCount, optionid, count);
                }
            }
#if !DEBUG
            catch (Exception e)
            {
                var error = string.Concat("Failed to process choice count:\n", e.Message, "\nStack Trace:\n", e.StackTrace);
                logError(error);
            }
#endif
            finally
            {
                cd.closeMutex.ReleaseMutex();
            }
        }

        private void processOption(Command cmdReceived, uint optionid, string description, uint type)
        {
            /** Cache this info **/
            ConfigCache.addOptionDescription((int)optionid, (int)type, description, cmdReceived.HasFlag(Command.CONFIG_USEVALUEMASK));
            /** Pass onto the config dialog if available **/
            var cd = configDialog;
            if (cd == null) return;
            try
            {
                cd.closeMutex.WaitOne();
                if (cd.Visible)
                {
                    /** Check for an indexed option **/
                    if (cmdReceived.HasFlag(Command.CONFIG_USEVALUEMASK))
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
#if DEBUG
            catch (Exception e)
            {
                var error = string.Concat("Failed to process option:\n", e.Message, "\nStack Trace:\n", e.StackTrace);
                logError(error);
            }
#endif
            finally
            {
                cd.closeMutex.ReleaseMutex();
            }
        }

        private void processOptionCount(uint count)
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
#if !DEBUG
            catch (Exception e)
            {
                var error = string.Concat("Failed to process option count:\n", e.Message, "\nStack Trace:\n", e.StackTrace);
                logError(error);
            }
#endif
            finally
            {
                cd.closeMutex.ReleaseMutex();
            }
        }

        private void processConfigSetting(Command cmdReceived, uint optionid, ConfigType type)
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
                        SendQuit("Invalid type received in processConfigSetting", false);
                    }
                    break;
            }

            /** Cache this setting **/
            /** Use index=-1 to represent a non indexed setting **/
            int index = -1;
            if (cmdReceived.HasFlag(Command.CONFIG_USEVALUEMASK))
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
                        if (cmdReceived.HasFlag(Command.CONFIG_USEVALUEMASK))
                        {
                            switch (type)
                            {
                                case ConfigType.INT:
                                case ConfigType.CHOICE:
                                    {
                                        /** Box it up and send it off, choices can be treated as
                                            just ints because that is the underlying datatype
                                        **/
                                        configDialog.Invoke((Action<uint, int, int>)configDialog.setValue, optionid, index, (int)valueInt);
                                    }
                                    break;
                                case ConfigType.STR:
                                    {
                                        configDialog.Invoke((Action<uint, int, string>)configDialog.setValue, optionid, index, valueStr);
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
                                        configDialog.Invoke((Action<uint, int>)configDialog.setValue, optionid, (int)valueInt);
                                    }
                                    break;
                                case ConfigType.STR:
                                    {
                                        configDialog.Invoke((Action<uint, string>)configDialog.setValue, optionid, valueStr);
                                    }
                                    break;
                            }
                        }
                    }
                }
#if !DEBUG
                catch (Exception e)
                {
                    var error = string.Concat("Failed to process config setting: ", optionid.ToString(), ":\n", e.Message, "\nStack Trace:\n", e.StackTrace);
                    logError(error);
                }
#endif
                finally
                {
                    cd.closeMutex.ReleaseMutex();
                }
            }
        }

        private void processConfigResult(Command cmdReceived, uint optionid, ConfigResult result)
        {
            /** We receive a result for every config setting we try to update **/
            /** Only report these if the form is still open, it is a serious error, if
                we receive one of these when the form is closing **/
            if (configDialog == null) return;
            // NB: indexes don't seem to be used here.
#if !DEBUG
            try
            {
#endif
                configDialog.Invoke((Action<uint, ConfigResult>)configDialog.setResult, optionid, result);
#if !DEBUG

            }
            catch (Exception e)
            {
                var error = string.Concat("Failed to process config result:\n", e.Message, "\nStack Trace:\n", e.StackTrace);
                logError(error);
            }
#endif
            }

        private void processConfigError(uint errorCode, string description)
        {
            if (securityDialog == null) return;
            securityDialog.Invoke((Action<object, string>)securityDialog.receiveConfigError, errorCode, description);
        }

        private void processUserInfo(string username, uint permissions)
        {
            if (securityDialog == null) return;
            securityDialog.Invoke((Action<string, object>)securityDialog.receiveUserInfo, username, permissions);
        }

        private void processUserCount(uint userCount)
        {
            if (securityDialog == null) return;
            securityDialog.Invoke((Action<object>)securityDialog.receiveUserCount, userCount);
        }

        private void processUserResult(uint resultCode, string description)
        {
            if (securityDialog == null) return;
            securityDialog.Invoke((Action<object, string>)securityDialog.receiveUserResult, resultCode, description);
        }

        private void processPermissionInfo(uint permissionCode, string description)
        {
            if (securityDialog == null) return;
            securityDialog.Invoke((Action<object, string>)securityDialog.receivePermissionInfo, permissionCode, description);
        }

        private void processPermissionCount(uint permissionCount)
        {
            if (securityDialog == null) return;
            securityDialog.Invoke((Action<object>)securityDialog.receivePermissionCount, permissionCount);
        }

        private void processIPRestrictionCount(Command cmd, uint count)
        {
            if (securityDialog == null) return;
            var target = cmd.HasFlag(Command.CONFIG_USEVALUEMASK)
                ? (Action<object>)securityDialog.receiveIPDenyCount
                : securityDialog.receiveIPAllowCount;
            securityDialog.Invoke(target, count);
        }

        private void processIPRestriction(Command cmd, string ipaddress, uint mask)
        {
            if (securityDialog == null) return;
            var target = cmd.HasFlag(Command.CONFIG_USEVALUEMASK)
                ? (Action<string, object>)securityDialog.receiveIPDeny
                : securityDialog.receiveIPAllow;
            securityDialog.Invoke(target, ipaddress, mask);
        }
    }
}