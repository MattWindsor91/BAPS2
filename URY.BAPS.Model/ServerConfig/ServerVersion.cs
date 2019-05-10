namespace URY.BAPS.Model.ServerConfig
{
    /// <summary>
    ///     Information about the server's version.
    /// </summary>
    public class ServerVersion
    {
        public ServerVersion(string version, string date, string time, string author)
        {
            Version = version;
            Date = date;
            Time = time;
            Author = author;
        }

        public string Version { get; }
        public string Date { get; }
        public string Time { get; }
        public string Author { get; }
    }
}