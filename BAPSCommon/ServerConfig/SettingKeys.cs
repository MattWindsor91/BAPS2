namespace BAPSClientCommon.ServerConfig
{
    /// <summary>
    /// String constants corresponding to descriptions used in the bapsnet
    /// config system.
    /// </summary>
    public static class SettingKeys
    {
        // NB: All capitalisation here is deliberate, to conform with the BAPS server.
        
        public const string AutoAdvance = "Auto Advance";
        public const string PlayOnLoad = "Play on load";
        public const string Repeat = "Repeat";

        public const string NumChannels = "Number of Channels";
        public const string NumDirectories = "Number of directories";  // [sic]
    }
}