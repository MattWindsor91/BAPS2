// This used to be BAPSPresenterMainReactions_config.cpp.

using System;
using BAPSClientCommon;
using BAPSClientCommon.BapsNet;
using BAPSClientCommon.Events;
using BAPSClientCommon.ServerConfig;

namespace BAPSPresenter2
{
    partial class Main
    {
        private void SetupConfigReactions(Receiver r)
        {
            Config.InstallReceiverEventHandlers(r);

            r.ConfigOption += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((Updates.ConfigOptionHandler)processOption, sender, e);
                }
                else processOption(sender, e);
            };
            r.ConfigChoice += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((Updates.ConfigChoiceHandler)processChoice, sender, e);
                }
                else processChoice(sender, e);
            };
            r.ConfigSetting += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((Updates.ConfigSettingHandler)processConfigSetting, sender, e);
                }
                else processConfigSetting(sender, e);
            };
            r.ConfigResult += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((Action<Command, uint, ConfigResult>)processConfigResult, e.cmdReceived, e.optionID, e.result);
                }
                else processConfigResult(e.cmdReceived, e.optionID, e.result);
            };
            r.User += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((Action<string, uint>)processUserInfo, e.username, e.permissions);
                }
                else processUserInfo(e.username, e.permissions);
            };
            r.Permission += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((Action<uint, string>)processPermissionInfo, e.permissionCode, e.description);
                }
                else processPermissionInfo(e.permissionCode, e.description);
            };
            r.UserResult += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((Action<uint, string>)processUserResult, e.resultCode, e.description);
                }
                else processUserResult(e.resultCode, e.description);
            };
            r.IpRestriction += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((Action<Command, string, uint>)processIPRestriction, e.cmdReceived, e.ipAddress, e.mask);
                }
                else processIPRestriction(e.cmdReceived, e.ipAddress, e.mask);
            };
        }

        /** All these functions firstly retrieve all the data the command needs and then (if possible)
	        update the configDialog... exceptions to the rule exist... read on
        **/

        private void processChoice(object sender, Updates.ConfigChoiceArgs e)
        {
            /** Ignore if the config dialog is closed **/
            var cd = configDialog;
            if (cd == null) return;
            try
            {
                cd.closeMutex.WaitOne();
                if (cd.Visible)
                {
                    cd.Invoke((Action<uint, int, string>)configDialog.addChoice, e.OptionId, (int)e.ChoiceId, e.ChoiceDescription);
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

        private void processOption(object sender, Updates.ConfigOptionArgs e)
        {
            /** Pass onto the config dialog if available **/
            var cd = configDialog;
            if (cd == null) return;
            try
            {
                cd.closeMutex.WaitOne();
                if (cd.Visible)
                {
                    if (e.HasIndex)
                    {
                        /** Indexed option - does not update the form UI just data **/
                        cd.addOption(new ConfigOptionInfo((int)e.OptionId, e.Description, (int)e.Type, e.Index));
                    }
                    else
                    {
                        /** Non indexed option - does not update the form ui just data **/
                        cd.addOption(new ConfigOptionInfo((int)e.OptionId, e.Description, (int)e.Type));
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
#if !DEBUG
            catch (Exception exc)
            {
                var error = string.Concat("Failed to process option:\n", exc.Message, "\nStack Trace:\n", exc.StackTrace);
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

        private void processConfigSetting(object sender, Updates.ConfigSettingArgs e)
        {
            var hasIndex = 0 <= e.Index;
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
                        if (hasIndex)
                        {
                            switch (e.Type)
                            {
                                case ConfigType.Int:
                                case ConfigType.Choice:
                                    {
                                        /** Box it up and send it off, choices can be treated as
                                            just ints because that is the underlying datatype
                                        **/
                                        configDialog.Invoke((Action<uint, int, int>)configDialog.setValue, e.OptionId, e.Index, (int)e.Value);
                                    }
                                    break;
                                case ConfigType.Str:
                                    {
                                        configDialog.Invoke((Action<uint, int, string>)configDialog.setValue, e.OptionId, e.Index, (string)e.Value);
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            /** Non indexed settings, just box them up and send them off **/
                            switch (e.Type)
                            {
                                case ConfigType.Int:
                                case ConfigType.Choice:
                                    {
                                        configDialog.Invoke((Action<uint, int>)configDialog.setValue, e.OptionId, (int)e.Value);
                                    }
                                    break;
                                case ConfigType.Str:
                                    {
                                        configDialog.Invoke((Action<uint, string>)configDialog.setValue, e.OptionId, (string)e.Value);
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
            var s = securityDialog;
            if (s == null) return;
            if (s.IsDisposed || !s.IsHandleCreated) return;
            if (s.InvokeRequired)
            {
                s.Invoke((Action<uint, string>)s.receiveConfigError, errorCode, description);
            }
            else s.receiveConfigError(errorCode, description);
        }

        private void processUserInfo(string username, uint permissions)
        {
            var s = securityDialog;
            if (s == null) return;
            if (s.IsDisposed || !s.IsHandleCreated) return;
            if (s.InvokeRequired)
            {
                s.Invoke((Action<string, uint>)s.receiveUserInfo, username, permissions);
            }
            else s.receiveUserInfo(username, permissions);
        }

        private void processUserCount(uint userCount)
        {
            var s = securityDialog;
            if (s == null) return;
            if (s.IsDisposed || !s.IsHandleCreated) return;
            if (s.InvokeRequired)
            {
                s.Invoke((Action<uint>)s.receiveUserCount, userCount);
            }
            else s.receiveUserCount(userCount);
        }

        private void processUserResult(uint resultCode, string description)
        {
            var s = securityDialog;
            if (s == null) return;
            if (s.IsDisposed || !s.IsHandleCreated) return;
            if (s.InvokeRequired)
            {
                s.Invoke((Action<uint, string>)s.receiveUserResult, resultCode, description);
            }
            else s.receiveUserResult(resultCode, description);
        }

        private void processPermissionInfo(uint permissionCode, string description)
        {
            var s = securityDialog;
            if (s == null) return;
            if (s.IsDisposed || !s.IsHandleCreated) return;
            if (s.InvokeRequired)
            {
                s.Invoke((Action<uint, string>)s.receivePermissionInfo, permissionCode, description);
            }
            else s.receivePermissionInfo(permissionCode, description);
        }

        private void processPermissionCount(uint permissionCount)
        {
            var s = securityDialog;
            if (s == null) return;
            if (s.IsDisposed || !s.IsHandleCreated) return;
            if (s.InvokeRequired)
            {
                s.Invoke((Action<uint>)s.receivePermissionCount, permissionCount);
            }
            else s.receivePermissionCount(permissionCount);
        }

        private void processIPRestrictionCount(Command cmd, uint count)
        {
            var s = securityDialog;
            if (s == null) return;
            if (s.IsDisposed || !s.IsHandleCreated) return;
            var target = cmd.HasFlag(Command.ConfigUseValueMask)
                ? (Action<uint>)s.receiveIPDenyCount
                : s.receiveIPAllowCount;
            if (s.InvokeRequired)
            {
                s.Invoke(target, count);
            }
            else target(count);
        }

        private void processIPRestriction(Command cmd, string ipaddress, uint mask)
        {
            var s = securityDialog;
            if (s == null) return;
            if (s.IsDisposed || !s.IsHandleCreated) return;
            var target = cmd.HasFlag(Command.ConfigUseValueMask)
                ? (Action<string, uint>)s.receiveIPDeny
                : s.receiveIPAllow;
            if (s.InvokeRequired)
            {
                s.Invoke(target, ipaddress, mask);
            }
            else target(ipaddress, mask);
        }
    }
}