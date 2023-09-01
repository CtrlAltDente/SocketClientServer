using UnityEngine;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Network.Managers
{
    public class NetworkTCPClient : BaseTcpProtocol
    {
        public int ClientPort = 3333;
        public int ServerPort = 3334;

        private TcpClient _tcpClient;

        public NetworkTCPClient(string newAuthentificationToken = null) : base(newAuthentificationToken)
        {
            _sendDataThread = new Thread(SendData);
            _listenDataThread = new Thread(ListenData);
            _checkConnectionThread = new Thread(CheckConnection);
        }

        ~NetworkTCPClient()
        {
            Shutdown();
        }

        public override void Initialize()
        {
            if (_tcpClient != null)
                return;

            _tcpClient = new TcpClient();
        }

        public override void Shutdown()
        {
            StopThreads();

            try
            {
                _tcpClient.Dispose();
                _tcpClient = null;
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        public async void RunConnectionToIPAddress(string ipAddress)
        {
            try
            {
                await _tcpClient.ConnectAsync(ipAddress, ServerPort);
                Debug.Log(_tcpClient.Connected);

                if (_tcpClient.Connected)
                {
                    await Task.Run(Authentification);
                }
            }
            catch
            {
                Shutdown();
            }
        }

        protected async override void Authentification()
        {
            try
            {
                DataPackage authentificationRequestPackage = new DataPackage(Encoding.UTF8.GetBytes(_authentificationToken), DataType.Authentification);
                byte[] authentificationRequesBytes = authentificationRequestPackage.DataPackageToBytes();

                await _tcpClient.GetStream().WriteAsync(authentificationRequesBytes, 0, authentificationRequesBytes.Length);

                bool hasAnswer = false;
                byte[] receivedBytes = new byte[1];

                while (!hasAnswer)
                {
                    if (_tcpClient.GetStream().DataAvailable && _tcpClient.GetStream().CanRead)
                    {
                        receivedBytes = new byte[256];
                        await _tcpClient.GetStream().ReadAsync(receivedBytes, 0, receivedBytes.Length);
                        hasAnswer = true;
                    }
                }

                DataPackage receivedDataPackage = new DataPackage(receivedBytes);

                if (receivedDataPackage.DataType == DataType.Authentification)
                {
                    if (receivedDataPackage.Data[0] == 1)
                    {
                        RunThreads();
                        Debug.Log("Client connected");
                    }
                    else
                    {
                        Shutdown();
                        Debug.Log("Client doesn`t connected");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        private void RunThreads()
        {
            _sendDataThread.Start();
            _listenDataThread.Start();
            _checkConnectionThread.Start();
        }

        private void StopThreads()
        {
            _sendDataThread.Abort();
            _listenDataThread.Abort();
            _checkConnectionThread.Abort();
        }

        protected override void SendData()
        {
            try
            {
                while (_tcpClient.Connected && _tcpClient != null)
                {
                    if (DataToSend.Count > 0 && _tcpClient.GetStream().CanWrite)
                    {
                        WriteDataToStream(DataToSend.Dequeue(), _tcpClient.GetStream());
                    }
                }
            }
            catch (Exception e)
            {
                Shutdown();
            }
        }

        protected override void ListenData()
        {
            try
            {
                NetworkStream stream = _tcpClient.GetStream();

                while (_tcpClient.Connected && _tcpClient != null)
                {
                    ReadDataFromStream(stream);
                }
            }
            catch (Exception e)
            {
                Shutdown();
            }
        }

        protected override void CheckConnection()
        {
            try
            {
                while (_tcpClient.Connected)
                {
                    Thread.Sleep(100);

                    DataPackage dataPackage = new DataPackage(new byte[1], DataType.ConnectionCheck);
                    byte[] data = dataPackage.DataPackageToBytes();

                    _tcpClient.GetStream().Write(data, 0, data.Length);
                    Debug.Log("CLIENT IS CONNECTED");
                }
            }
            catch (Exception e)
            {
                Shutdown();
                Debug.Log("Connection to server ended");
            }
        }
    }
}