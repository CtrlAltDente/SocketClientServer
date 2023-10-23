using Network.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace Network.TCP.SocketLogic
{
    public class TcpClientLogic : TcpBaseLogic, IProtocolLogic
    {
        private TcpClient _tcpClient;

        public TcpClientLogic(Action<Connection> OnConnection) : base(OnConnection)
        {

        }

        public async void Initialize(string serverIpAddress, int serverPort)
        {
            try
            {
                if (_tcpClient != null)
                    return;

                _tcpClient = new TcpClient();

                await _tcpClient.ConnectAsync(serverIpAddress, serverPort);

                Connection newConnection = new Connection(_tcpClient);
                OnConnectionInitialized?.Invoke(newConnection);

                Debug.Log("CONNECTED!");
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
                Shutdown();
            }
        }

        public void Shutdown()
        {
            try
            {
                if (_tcpClient == null)
                    return;

                _tcpClient.Close();
                _tcpClient = null;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
}