namespace URY.BAPS.Common.Model.ServerConfig
{
    /// <summary>
    ///     An IP restriction entry.
    /// </summary>
    public class IpRestriction
    {
        public IpRestriction(IpRestrictionType type, string ipAddress, uint mask)
        {
            Type = type;
            IpAddress = ipAddress;
            Mask = mask;
        }

        public IpRestrictionType Type { get; }

        public string IpAddress { get; }

        public uint Mask { get; }
    }

    /// <summary>
    ///     Type of IP restriction entries.
    /// </summary>
    public enum IpRestrictionType
    {
        // TODO(@MattWindsor91): replace with polymorphism
        Allow = 0,
        Deny = 1
    }
}