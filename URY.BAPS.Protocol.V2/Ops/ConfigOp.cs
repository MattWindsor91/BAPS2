namespace URY.BAPS.Protocol.V2.Commands
{
    // The numbers assigned to each of these enum values are significant:
    // when shifted left by the appropriate member of CommandShifts,
    // they form BapsNet flags.
    // As such, _do not_ change them without good reason.

    /// <summary>
    ///     Enumeration of config operations.
    /// </summary>
    public enum ConfigOp : byte
    {
        GetOptions = 0,
        GetOptionChoices = 1,
        GetConfigSettings = 2,
        GetConfigSetting = 3,
        GetOption = 4,
        SetConfigValue = 5,
        GetUsers = 6,
        GetPermissions = 7,
        GetUser = 8,
        AddUser = 9,
        RemoveUser = 10,
        SetPassword = 11,
        GrantPermission = 12,
        RevokePermission = 13,
        // 14 - 15: unused
        Option = 16,
        OptionChoice = 17,
        ConfigSetting = 18,
        User = 19,
        Permission = 20,
        UserResult = 21,
        ConfigResult = 22,
        ConfigError = 23,
        GetIpRestrictions = 24,
        IpRestriction = 25,
        AlterIpRestriction = 26,
    }

    public static class ConfigOpExtensions
    {
        /// <summary>
        ///     Gets whether a particular config operation can take an index.
        /// </summary>
        /// <param name="op">The operation to check.</param>
        /// <returns>True if <paramref name="op"/> can take an index (that is, its
        ///  packed representation's lowest 7 bits are a has-index flag followed by
        ///  6 index bits); false if not (and the packed representation's lowest
        ///  7 bits are one don't-care bit followed by 6 value bits).
        /// </returns>
        public static bool CanTakeIndex(this ConfigOp op)
        {
            return op switch {
                ConfigOp.SetConfigValue => true,
                ConfigOp.Option => true,
                ConfigOp.ConfigSetting => true,
                ConfigOp.ConfigResult => true,
                // Technically, IPRestriction doesn't take an index.
                // However, it puts the 'alter/deny' bit in the same place as
                // the 'has/doesn't have index' bit, and it's easier to model
                // this as an indexed command.
                ConfigOp.IpRestriction => true,
                _ => false
            };
        }
    }
}