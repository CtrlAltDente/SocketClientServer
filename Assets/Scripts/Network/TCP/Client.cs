using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace Network
{
    public class Client
    {
        [SerializeField] public TcpClient TcpClient = default;
        [SerializeField] private string clientInfo = default;

        public Client(TcpClient client)
        {
            this.TcpClient = client;
            clientInfo = client.Client.RemoteEndPoint.ToString();
        }
    }
}