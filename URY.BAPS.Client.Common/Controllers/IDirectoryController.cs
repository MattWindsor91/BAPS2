namespace URY.BAPS.Client.Common.Controllers
{
    public interface IDirectoryController
    {
        /// <summary>
        ///     Asks the server to refresh this directory's listing.
        /// </summary>
        void Refresh();
    }
}