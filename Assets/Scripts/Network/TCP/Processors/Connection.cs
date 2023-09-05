using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

[Serializable]
public class Connection
{
    public TcpClient TcpClient { get; private set; }
    public string EndIp;

    public Connection(TcpClient tcpClient)
    {
        TcpClient = tcpClient;
        EndIp = tcpClient.Client.RemoteEndPoint.ToString();
    }
}
