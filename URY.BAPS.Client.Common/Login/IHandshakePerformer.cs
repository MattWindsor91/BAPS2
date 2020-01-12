using System.Net.Sockets;
using URY.BAPS.Client.Common.Login.LoginResult;

namespace URY.BAPS.Client.Common.Login
{
    public interface IHandshakePerformer<out T>
    {
        T DoHandshake(TcpClient client);
    }
}