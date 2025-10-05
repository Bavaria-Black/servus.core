using System.Net;
using System.Net.Sockets;

namespace Servus.Core.Network;

public class PortFinder
{
    /// <summary>
    /// Returns a unused local port on the current host
    /// </summary>
    /// <returns>Port number or 0 if no free port was found.</returns>
    public static int FindFreeLocalPort()
    {
        int port;
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            var endpoint = new IPEndPoint(IPAddress.Any, 0);
            socket.Bind(endpoint);
            endpoint = (IPEndPoint)socket.LocalEndPoint!;
            port = endpoint.Port;
        }
        finally
        {
            socket.Close();
        }

        return port;
    }
}