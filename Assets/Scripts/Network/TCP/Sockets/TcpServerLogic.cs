using Network.Interfaces;
using Network.TCP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Network.TCP.SocketLogic
{
    public class TcpServerLogic : TcpBaseLogic, IProtocolLogic
    {
        private TcpListener _tcpListener;

        private Thread ListenConnectionsThread;

        public TcpServerLogic(Action<Connection> OnConnectionAction) : base(OnConnectionAction)
        {

        }

        public void Initialize(string serverIpAddress, int serverPort)
        {
            try
            {
                _tcpListener = new TcpListener(IPAddress.Parse(serverIpAddress), serverPort);
                _tcpListener.Start();

                ListenConnectionsThread = new Thread(ListenNewConnections);
                ListenConnectionsThread.Start();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        public void Shutdown()
        {
            try
            {
                _tcpListener.Stop();
                ListenConnectionsThread.Abort();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        private async void ListenNewConnections()
        {
            try
            {
                while (true)
                {
                    TcpClient newClient = await _tcpListener.AcceptTcpClientAsync();
                    Connection newConnection = new Connection(newClient);
                    OnConnectionInitialized?.Invoke(newConnection);
                }
            }
            catch
            {
                Debug.LogWarning("Tcp Listener is closed, new connection listening is stopped");
                Shutdown();
            }
        }
    }
}