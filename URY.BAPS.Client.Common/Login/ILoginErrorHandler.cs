using URY.BAPS.Client.Common.Login.LoginResult;

namespace URY.BAPS.Client.Common.Login
{
    /// <summary>
    ///     Interface for authenticator components that handle login
    ///     errors.
    ///     <para>
    ///         For example, an implementation might pop up message boxes,
    ///         or write to the console.
    ///     </para>
    /// </summary>
    public interface ILoginErrorHandler
    {
        void Handle(ILoginResult result);
    }
}