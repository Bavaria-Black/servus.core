using System;
using System.Net;
using System.Net.Sockets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Servus.Core.Network;

namespace Servus.Core.Tests.Network;

[TestClass]
public class PortFinderTests
{
    [TestMethod]
    public void FindFreeLocalPort_Should_Return_Valid_Port()
    {
        // Act
        var port = PortFinder.FindFreeLocalPort();

        // Assert
        Assert.IsTrue(port is >= 1024 and <= 65535,
            $"Expected a valid port between 1024 and 65535, but got {port}");
    }

    [TestMethod]
    public void FindFreeLocalPort_Should_Return_Usable_Port()
    {
        // Arrange
        var port = PortFinder.FindFreeLocalPort();

        // Act
        Exception? ex = null;
        try
        {
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var endpoint = new IPEndPoint(IPAddress.Loopback, port);
            socket.Bind(endpoint);
        }
        catch (Exception e)
        {
            ex = e;
        }

        // Assert
        Assert.IsNull(ex, $"Port {port} should be usable, but binding failed: {ex?.Message}");
    }

    [TestMethod]
    public void FindFreeLocalPort_Should_Return_Different_Ports_On_Subsequent_Calls()
    {
        // Act
        var port1 = PortFinder.FindFreeLocalPort();
        var port2 = PortFinder.FindFreeLocalPort();

        // Assert
        Assert.AreNotEqual(port1, port2, "Subsequent calls returned the same port");
    }

    [TestMethod]
    public void Found_Port_Should_Be_Reused_When_Released()
    {
        var port = PortFinder.FindFreeLocalPort();

        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(new IPEndPoint(IPAddress.Loopback, port));
        socket.Close();

        Exception? ex = null;
        try
        {
            using var testSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            testSocket.Bind(new IPEndPoint(IPAddress.Loopback, port));
        }
        catch (Exception e)
        {
            ex = e;
        }

        // Assert
        Assert.IsNull(ex, $"Port {port} should be reusable after release, but failed: {ex?.Message}");
    }
}