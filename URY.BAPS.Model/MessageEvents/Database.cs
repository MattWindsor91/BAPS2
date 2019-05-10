namespace URY.BAPS.Model.MessageEvents
{
    public class LibraryResultArgs : MessageArgsBase
    {
        public LibraryResultArgs(uint resultId, byte dirtyStatus, string description)
        {
            ResultId = resultId;
            DirtyStatus = dirtyStatus;
            Description = description;
        }

        public uint ResultId { get; }
        public byte DirtyStatus { get; }
        public string Description { get; }
    }


    public class ListingResultArgs : MessageArgsBase
    {
        public ListingResultArgs(uint listingId, uint channelId, string description)
        {
            ListingId = listingId;
            ChannelId = channelId;
            Description = description;
        }

        public uint ListingId { get; }
        public uint ChannelId { get; }

        public string Description { get; }
    }


    public class PermissionArgs : MessageArgsBase
    {
        public PermissionArgs(uint permissionCode, string description)
        {
            PermissionCode = permissionCode;
            Description = description;
        }

        public uint PermissionCode { get; }
        public string Description { get; }
    }

    public class ShowResultArgs : MessageArgsBase
    {
        public ShowResultArgs(uint showId, string description)
        {
            ShowId = showId;
            Description = description;
        }

        public uint ShowId { get; }
        public string Description { get; }
    }
}