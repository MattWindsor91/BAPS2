using System;
using URY.BAPS.Common.Model.MessageEvents;
using URY.BAPS.Common.Model.ServerConfig;
using URY.BAPS.Common.Protocol.V2.Commands;

namespace URY.BAPS.Common.Protocol.V2.Decode
{
    public partial class CommandDecoder
    {
        private static int IndexAsInt(IndexableConfigCommandBase c)
        {
            return c.HasIndex ? c.Index : -1;
        }

        #region Value commands

        public void Visit(ValueConfigCommand command)
        {
            switch (command.Op)
            {
                case ConfigOp.OptionChoice when command.ModeFlag:
                    DecodeOptionChoice();
                    break;
                case ConfigOp.OptionChoice when !command.ModeFlag:
                    DecodeOptionChoiceCount();
                    break;
                case ConfigOp.User when command.ModeFlag:
                    DecodeUser();
                    break;
                case ConfigOp.User when !command.ModeFlag:
                    DecodeUserCount();
                    break;
                case ConfigOp.Permission when command.ModeFlag:
                    DecodePermission();
                    break;
                case ConfigOp.Permission when !command.ModeFlag:
                    DecodePermissionCount();
                    break;
                case ConfigOp.UserResult:
                    DecodeUserResult(command.Value);
                    break;
                case ConfigOp.ConfigError:
                    DecodeConfigError(command.Value);
                    break;
                // ConfigOp.IpRestriction doesn't have an index, but we model it as if it does.
                // See ConfigOpExtensions.CanTakeIndex for explanation.
            }
        }

        private void DecodeOptionChoice()
        {
            var optionId = ReceiveUint();
            var choiceIndex = ReceiveUint();
            var choiceDescription = ReceiveString();
            Dispatch(new ConfigChoiceArgs(optionId, choiceIndex, choiceDescription));
        }

        private void DecodeOptionChoiceCount()
        {
            var optionId = ReceiveUint();
            DecodeCount(CountType.ConfigChoice, optionId);
        }

        private void DecodeUser()
        {
            var username = ReceiveString();
            var permissions = ReceiveUint();
            Dispatch(new UserArgs(username, permissions));
        }

        private void DecodeUserCount()
        {
            DecodeCount(CountType.User);
        }

        private void DecodePermission()
        {
            var permissionCode = ReceiveUint();
            var description = ReceiveString();
            Dispatch(new PermissionArgs(permissionCode, description));
        }

        private void DecodePermissionCount()
        {
            DecodeCount(CountType.Permission);
        }

        private void DecodeUserResult(byte resultCode)
        {
            var description = ReceiveString();
            Dispatch(new UserResultArgs(resultCode, description));
        }

        private void DecodeConfigError(byte value)
        {
            DecodeError(ErrorType.Config, value);
        }

        #endregion Value commands

        #region Indexable commands

        public void Visit(NonIndexedConfigCommand command)
        {
            DecodeIndexableConfigCommand(command);
        }

        public void Visit(IndexedConfigCommand command)
        {
            DecodeIndexableConfigCommand(command);
        }

        private void DecodeIndexableConfigCommand(IndexableConfigCommandBase command)
        {
            var maybeIndex = IndexAsInt(command);

            switch (command.Op)
            {
                case ConfigOp.Option when command.ModeFlag:
                    DecodeOption(maybeIndex);
                    break;
                case ConfigOp.Option when !command.ModeFlag:
                    DecodeOptionCount();
                    break;
                case ConfigOp.ConfigSetting when command.ModeFlag:
                    DecodeConfigSetting(maybeIndex);
                    break;
                case ConfigOp.ConfigSetting when !command.ModeFlag:
                    DecodeConfigSettingCount();
                    break;
                case ConfigOp.ConfigResult:
                    DecodeConfigResult(maybeIndex);
                    break;
                // See above for why this is here.
                case ConfigOp.IpRestriction when command.ModeFlag:
                    DecodeIpRestriction(command.HasIndex);
                    break;
                case ConfigOp.IpRestriction when !command.ModeFlag:
                    DecodeIpRestrictionCount();
                    break;
                default:
                    ReportMalformedCommand(CommandGroup.Config);
                    break;
            }
        }

        private void DecodeIpRestriction(bool indexBitSet)
        {
            var type = DecodeIpRestrictionType(indexBitSet);
            var ipAddress = ReceiveString();
            var mask = ReceiveUint();
            var restriction = new IpRestriction(type, ipAddress, mask);
            Dispatch(new IpRestrictionArgs(restriction));
        }

        private void DecodeConfigResult(int maybeIndex)
        {
            var optionId = ReceiveUint();
            var result = DecodeConfigResult();
            Dispatch(new ConfigResultArgs(optionId, result, maybeIndex));
        }

        private void DecodeOption(int index)
        {
            var optionId = ReceiveUint();
            var description = ReceiveString();
            var type = DecodeConfigType();
            Dispatch(new ConfigOptionArgs(optionId, type, description, index));
        }

        private void DecodeOptionCount()
        {
            DecodeCount(CountType.ConfigOption);
        }

        private void DecodeConfigSetting(int index)
        {
            var optionId = ReceiveUint();
            var type = DecodeConfigType();
            var value = DecodeConfigSettingValue(type);
            Dispatch(new ConfigSettingArgs(optionId, type, value, index));
        }

        private void DecodeConfigSettingCount()
        {
            _ = ReceiveUint();
        }

        private static IpRestrictionType DecodeIpRestrictionType(bool indexBitSet)
        {
            return indexBitSet ? IpRestrictionType.Allow : IpRestrictionType.Deny;
        }

        private void DecodeIpRestrictionCount()
        {
            DecodeCount(CountType.IpRestriction);
        }

        private object DecodeConfigSettingValue(ConfigType type)
        {
            return type switch {
                ConfigType.Int => (object) (int) ReceiveUint(),
                ConfigType.Choice => (int) ReceiveUint(),
                ConfigType.Str => ReceiveString(),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Invalid type received")
                };
        }

        #endregion Indexable commands

        #region Decoding enumerations

        /// <summary>
        ///     Receives and decodes a <see cref="ConfigResult" /> from the protocol source.
        /// </summary>
        /// <returns>A <see cref="ConfigResult" /> received from upstream.</returns>
        private ConfigResult DecodeConfigResult()
        {
            return (ConfigResult) ReceiveUint();
        }

        /// <summary>
        ///     Receives and decodes a <see cref="ConfigType" /> from the protocol source.
        /// </summary>
        /// <returns>A <see cref="ConfigType" /> received from upstream.</returns>
        private ConfigType DecodeConfigType()
        {
            return (ConfigType) ReceiveUint();
        }

        #endregion Decoding enumerations
    }
}