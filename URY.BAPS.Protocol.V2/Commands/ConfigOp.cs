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
}