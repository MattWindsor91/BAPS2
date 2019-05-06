namespace URY.BAPS.Client.Common.Events
{
    public class LibraryResultArgs : ArgsBase
    {
        public uint ResultId { get; }
        public byte DirtyStatus { get; }
        public string Description { get; }
        
        public LibraryResultArgs(uint resultId, byte dirtyStatus, string description)
        {
            ResultId = resultId;
            DirtyStatus = dirtyStatus;
            Description = description;
        }
    }
    
    
    public class ListingResultArgs : ArgsBase
    {
        public uint ListingId { get; }
        public uint ChannelId { get; }
        
        public string Description { get; }
        
        public ListingResultArgs(uint listingId, uint channelId, string description)
        {
            ListingId = listingId;
            ChannelId = channelId;
            Description = description;
        }
    }
    
    
    public class PermissionArgs : ArgsBase
    {
        public uint PermissionCode { get; }
        public string Description { get; }
        
        public PermissionArgs(uint permissionCode, string description)
        {
            PermissionCode = permissionCode;
            Description = description;
        }
    }
    
    public class ShowResultArgs : ArgsBase
    {
        public uint ShowId { get; }
        public string Description { get; }
        
        public ShowResultArgs(uint showId, string description)
        {
            ShowId = showId;
            Description = description;
        }
    }
}