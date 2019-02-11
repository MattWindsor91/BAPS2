namespace URY.BAPS.Client.Common.ServerConfig
{
    /// <summary>
    ///     Enumeration of configuration types as understood by BapsNet.
    /// </summary>
    public enum ConfigType
    {
        // The values of this enum are the ones used in BapsNet;
        // please don't change unless necessary.

        /// <summary>
        ///     Integral configuration type.
        /// </summary>
        Int = 0,

        /// <summary>
        ///     String configuration type.
        /// </summary>
        Str = 1,

        /// <summary>
        ///     Choice configuration type (including booleans).
        /// </summary>
        Choice = 2
    }
}