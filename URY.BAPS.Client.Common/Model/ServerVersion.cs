namespace URY.BAPS.Client.Common.Model
{
    /// <summary>
    ///     Information about the server's version.
    /// </summary>
    public class ServerVersion
    {
        public string Version { get; }
        public string Date { get; }
        public string Time { get; }
        public string Author { get; }

        public ServerVersion(string version, string date, string time, string author)
        {
            Version = version;
            Date = date;
            Time = time;
            Author = author;
        }
    }
}