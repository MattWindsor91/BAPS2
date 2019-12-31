using System.Net.Sockets;
using URY.BAPS.Client.Common.Login.LoginResult;

namespace URY.BAPS.Client.Common.Login
{
    public interface IHandshakePerformer<out T>
    {
        T Result { get; }
        
        ILoginResult DoHandshake(TcpClient client);
    }
}