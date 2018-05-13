using System;
using SplashKitSDK;
using System.Collections.Generic;

public class GamePeer
{
    public string Name { get; set; }
    private ServerSocket _server;

    private Dictionary<string, Connection> _peers = new Dictionary<string, Connection>();

    public string GetMsg { get; private set; }

    public ServerSocket Server
    {
        get
        {
            return _server;
        }
    }

    public GamePeer(ushort port)
    {
        _server = new ServerSocket("GameServer", port);
    }

    public void ConnectToGamePeer(string address, ushort port)
    {
        Connection newConnection = new Connection($"{address }:{port}", address, port);
        if (newConnection.IsOpen)
        {
            Console.WriteLine($"Conected to {address}:{port}");
        }
    }

    private void EstablishConnection(Connection con)
    {
        // Send my name...
        con.SendMessage(Name);

        // Wait for a message...
        SplashKit.CheckNetworkActivity();
        for (int i = 0; i < 10 && !con.HasMessages; i++)
        {
            //SplashKit.Delay(200);
            SplashKit.CheckNetworkActivity();
        }

        if (!con.HasMessages)
        {
            con.Close();
            throw new Exception("Timeout waiting for name of peer");
        }

        // Read the name
        string name = con.ReadMessageData();

        // See if we can register this...
        if (_peers.ContainsKey(name))
        {
            con.Close();
            throw new Exception("Unable to connect to multiple peers with the same name");
        }

        // Register
        _peers[name] = con;
        Console.WriteLine($"Connected to {name} at { SplashKit.Ipv4ToStr(con.IP) }:{con.Port}");
    }

    public void CheckNewConnections()
    {
        while (_server.HasNewConnections)
        {
            Connection newConnection = _server.FetchNewConnection();
            try
            {
                EstablishConnection(newConnection);
            }
            catch
            {
                // ignore errors
            }
        }
    }

    public void Broadcast(string message)
    {

        SplashKit.BroadcastMessage(message);
    }

    public void SendMessageTo(string name, string message)
    {
        if (_peers.ContainsKey(Name))
        {
            _peers[Name].SendMessage(message);
        }
    }

    public void PrintNewMessages()
    {
        SplashKit.CheckNetworkActivity();

        while (SplashKit.HasMessages())
        {
            using (Message m = SplashKit.ReadMessage())
            {
                Console.WriteLine($"Got message from {m.Host}:{m.Port}");
                Console.WriteLine(m.Data);
            }
        }
    }

    public string GetNewMessages()
    {
        SplashKit.CheckNetworkActivity();

        while (SplashKit.HasMessages())
        {
            using (Message m = SplashKit.ReadMessage())
            {
                //Console.WriteLine($"Got message from {m.Host}:{m.Port}");
                GetMsg = m.Data;
            }
        }
        return GetMsg;
    }

    public void Close()
    {
        _server.Close();
        SplashKit.CloseAllConnections();
    }
}
