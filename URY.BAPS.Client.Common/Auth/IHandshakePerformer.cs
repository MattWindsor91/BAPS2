using System.Net.Sockets;
using URY.BAPS.Client.Common.Auth.LoginResult;

namespace URY.BAPS.Client.Common.Auth
{
    public interface IHandshakePerformer<out T>
    {
        T Result { get; }
        
        ILoginResult DoHandshake(TcpClient client);
    }
}