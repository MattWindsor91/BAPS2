namespace URY.BAPS.Client.Common.Auth
{
    public interface IAuthedConnectionBuilder<TConn> where TConn : class
    {
        TConn? Connection { get; }

        ILoginResult Attempt(ILoginPromptResponse promptResponse);
    }
}