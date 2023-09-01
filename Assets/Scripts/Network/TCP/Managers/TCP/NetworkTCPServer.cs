using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

namespace Network.Managers
{
    public class NetworkTCPServer : BaseTcpProtocol
    {
        public int Port = 3334;

        public List<Client> ClientsOnAuthentification = new List<Client>();
        public List<Client> ConnectedClients = new List<Client>();

        private TcpListener _tcpListener = default;

        private Thread _acceptClientsTask;

        public NetworkTCPServer(string newAuthentificationToken = null) : base(newAuthentificationToken)
        {
            _networkThread = new Thread(DoNetworkStuff);
            _acceptClientsTask = new Thread(AcceptTcpClients);
        }

        ~NetworkTCPServer()
        {
            Shutdown();
        }

        public override void Initialize()
        {
            if (_tcpListener != null)
            {
                return;
            }

            Debug.Log(_firstDeviceIpAddress);

            _tcpListener = new TcpListener(_firstDeviceIpAddress, Port);
            _tcpListener.Start();

            RunThreads();
        }

        public override void Shutdown()
        {
            StopThreads();

            if (_tcpListener != null)
            {
                _tcpListener.Stop();
                _tcpListener = null;
            }
        }

        public void SendDataToClient(byte[] data, TcpClient tcpClient)
        {
            Task.Run(() => WriteDataToStream(data, tcpClient.GetStream()));
        }

        private void RunThreads()
        {
            _networkThread.Start();
            _acceptClientsTask.Start();
        }

        private void StopThreads()
        {
            _networkThread.Abort();
            _acceptClientsTask.Abort();
        }

        private void DoNetworkStuff()
        {
            while (_tcpListener != null)
            {
                Authentification();
                SendData();
                ListenData();
            }
        }

        private async void AcceptTcpClients()
        {
            while (_tcpListener != null)
            {
                try
                {
                    TcpClient tcpClient = await _tcpListener.AcceptTcpClientAsync();
                    ClientsOnAuthentification.Add(new Client(tcpClient));
                    Debug.Log($"Connected client, do authentification: {tcpClient.Client.RemoteEndPoint}");
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
        }

        protected async override void Authentification()
        {
            if (ClientsOnAuthentification.Count > 0)
            {
                foreach (Client client in ClientsOnAuthentification)
                {
                    try
                    {
                        NetworkStream networkStream = client.TcpClient.GetStream();

                        if (networkStream.DataAvailable && networkStream.CanRead)
                        {
                            DataPackage isAuthentifiacationApprovedDataPackage;
                            byte[] buffer = new byte[256];

                            await networkStream.ReadAsync(buffer, 0, buffer.Length);

                            DataPackage clientAuthentificationDataPackage = new DataPackage(buffer);

                            if (clientAuthentificationDataPackage.DataType == DataType.Authentification)
                            {
                                byte[] bytesToSend;
                                bool isAuthentificationApproved = true;

                                if (_hasAuthentification)
                                {
                                    isAuthentificationApproved = Encoding.UTF8.GetString(clientAuthentificationDataPackage.Data) == _authentificationToken;

                                    Debug.Log($"Is tokens equals: {Encoding.UTF8.GetString(clientAuthentificationDataPackage.Data) == _authentificationToken}, received: {Encoding.UTF8.GetString(clientAuthentificationDataPackage.Data)}, server token: {_authentificationToken}");
                                    Debug.Log($"length: {Encoding.UTF8.GetString(clientAuthentificationDataPackage.Data).Length}, received: {Encoding.UTF8.GetString(clientAuthentificationDataPackage.Data)}");
                                    Debug.Log($"server token: {_authentificationToken}, length: {_authentificationToken.Length}");

                                    for (int i = 0; i < clientAuthentificationDataPackage.Data.Length; i++)
                                    {
                                        Debug.Log(clientAuthentificationDataPackage.Data[i].ToString("X2"));
                                    }

                                    if (isAuthentificationApproved)
                                    {
                                        ConnectedClients.Add(client);
                                    }

                                    ClientsOnAuthentification.Remove(client);
                                }
                                else
                                {
                                    ConnectedClients.Add(client);
                                    ClientsOnAuthentification.Remove(client);
                                }

                                isAuthentifiacationApprovedDataPackage = new DataPackage(new byte[1] { Convert.ToByte(isAuthentificationApproved) }, DataType.Authentification);
                                bytesToSend = isAuthentifiacationApprovedDataPackage.DataPackageToBytes();

                                Debug.Log($"Is authentification approved: {isAuthentificationApproved}");

                                SendDataToClient(bytesToSend, client.TcpClient);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message + " " + e.Source);
                    }
                }
            }
        }


        protected override void SendData()
        {

            while (DataToSend.Count > 0 && ConnectedClients.Count > 0)
            {
                try
                {
                    byte[] data = DataToSend.Dequeue();

                    foreach (Client client in ConnectedClients)
                    {
                        Task sendTask = new Task(() => WriteDataToStream(data, client.TcpClient.GetStream()));
                        sendTask.Start();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
        }

        protected override void ListenData()
        {
            if (ConnectedClients.Count > 0)
            {
                foreach (Client client in ConnectedClients)
                {
                    try
                    {
                        NetworkStream clientStream = client.TcpClient.GetStream();

                        if (clientStream.DataAvailable && clientStream.CanRead)
                        {
                            ReadDataFromStream(clientStream);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                    }
                }
            }
        }

        protected override void CheckConnection()
        {
            foreach (Client client in ConnectedClients)
            {
                try
                {
                    DataPackage dataPackage = new DataPackage(new byte[1], DataType.ConnectionCheck);
                    byte[] data = dataPackage.DataPackageToBytes();

                    client.TcpClient.GetStream().Write(data, 0, data.Length);
                }
                catch (Exception e)
                {
                    ConnectedClients.Remove(client);
                    Debug.LogError(e.Message);
                    break;
                }
            }
        }
    }
}