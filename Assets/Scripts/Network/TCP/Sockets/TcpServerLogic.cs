using Network.Interfaces;
using Network.TCP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace Network.TCP.SocketLogic
{
    public class TcpServerLogic : TcpBaseLogic, IProtocolLogic
    {
        private TcpListener _tcpListener;

        private Task ListenConnectionsTask;

        public TcpServerLogic(Action<Connection> OnConnectionAction) : base(OnConnectionAction)
        {

        }

        public void Initialize(string serverIpAddress, int serverPort)
        {
            try
            {
                _tcpListener = new TcpListener(IPAddress.Parse(serverIpAddress), serverPort);
                _tcpListener.Start();

                ListenConnectionsTask = new Task(ListenNewConnections);
                ListenConnectionsTask.Start();
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
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        public async void ListenNewConnections()
        {
            try
            {
                while (_tcpListener != null)
                {
                    Debug.Log("Listen");
                    TcpClient newClient = await _tcpListener.AcceptTcpClientAsync();
                    Debug.Log("Listen1");
                    Connection newConnection = new Connection(newClient);
                    Debug.Log("Listen2");
                    OnConnectionInitialized?.Invoke(newConnection);
                    Debug.Log("New connection");
                }
            }
            catch (Exception e)
            {
                Shutdown();
                Debug.LogError(e.Message);
            }
        }
    }
}