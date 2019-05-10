namespace URY.BAPS.Protocol.V2.Ops
{
    // The numbers assigned to each of these enum values are significant:
    // when shifted left by the appropriate member of CommandShifts,
    // they form BapsNet flags.
    // As such, _do not_ change them without good reason.

    /// <summary>
    ///     Enumeration of database operations.
    /// </summary>
    public enum DatabaseOp : byte
    {
        LibrarySearch = 0,
        LibraryOrdering = 1,
        LibraryResult = 2,
        LibraryError = 3,
        GetShows = 4,
        Show = 5,
        GetListings = 6,
        Listing = 7,
        AssignListing = 8,
        BapsDbError = 9
    }
}