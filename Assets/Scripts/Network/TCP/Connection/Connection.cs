using Network.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Network.TCP
{
    [Serializable]
    public class Connection
    {
        public TcpClient TcpClient { get; private set; }
        public string EndIp;

        public Action<Connection> OnConnectionClosed;

        public bool IsDataSending => _dataToSend.Count > 0;
        public bool IsDataReceivingInProcess => _networkStream.DataAvailable;

        private NetworkStream _networkStream;

        public Queue<byte[]> _receivedData = new Queue<byte[]>();
        public Queue<byte[]> _dataToSend = new Queue<byte[]>();

        private Thread _dataSendingThread;
        private Thread _dataReceivingThread;

        public Connection(TcpClient tcpClient)
        {
            TcpClient = tcpClient;
            EndIp = tcpClient.Client.RemoteEndPoint.ToString();

            _networkStream = TcpClient.GetStream();

            _dataSendingThread = new Thread(SendData);
            _dataReceivingThread = new Thread(ReceiveData);

            _dataSendingThread.Start();
            _dataReceivingThread.Start();
        }

        public void ShutdownThreads()
        {
            TcpClient = null;
        }

        public void AddDataToSend(byte[] data)
        {
            _dataToSend.Enqueue(data);
        }

        public Queue<byte[]> GetAllReceivedData()
        {
            return _receivedData;
        }

        private async void ReceiveData()
        {
            while (TcpClient != null)
            {
                try
                {
                    if (_networkStream.DataAvailable && _networkStream.CanRead)
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            do
                            {
                                byte[] buffer = new byte[32 * 1024];
                                int bytes = await _networkStream.ReadAsync(buffer, 0, buffer.Length);
                                memoryStream.Write(buffer, 0, bytes);
                            }

                            while (_networkStream.DataAvailable);

                            byte[] data = memoryStream.ToArray();
                            _receivedData.Enqueue(data);
                        }
                    }
                }
                catch
                {
                    Debug.LogWarning("Connection closed");
                    TcpClient = null;
                }
            }
        }

        private async void SendData()
        {
            while (TcpClient != null)
            {
                try
                {
                    if (_dataToSend.Count > 0)
                    {
                        if (_networkStream.CanWrite)
                        {
                            byte[] data = _dataToSend.Dequeue();
                            await _networkStream.WriteAsync(data, 0, data.Length);
                        }
                    }
                }
                catch
                {
                    Debug.LogWarning("Connection closed");
                    TcpClient = null;
                }
            }
        }
    }
}