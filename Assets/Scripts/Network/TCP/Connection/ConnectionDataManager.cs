using Network.Data;
using Network.Processors;
using Network.TCP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

namespace Network.Processors
{
    [Serializable]
    public class ConnectionDataManager
    {
        public Queue<DataPackage> DataToSend = new Queue<DataPackage>();

        public Action<DataPackage> OnDataPackageReceived;

        public bool IsConnected => Connections.Count > 0;
        public int ConnectionsCount => Connections.Count;

        [SerializeField]
        private List<Connection> Connections = new List<Connection>();

        [SerializeField]
        private int DataToSendCount;

        public void AddConnection(Connection connection)
        {
            Connections.Add(connection);
        }

        public void ReceiveDataFromAll()
        {
            try
            {
                foreach (Connection connection in Connections)
                {
                    ReadDataFromConnectionStream(connection);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        public void SendDataToAll()
        {
            try
            {
                DataToSendCount = DataToSend.Count;

                while (DataToSend.Count > 0)
                {
                    DataPackage dataPackage = DataToSend.Dequeue();

                    byte[] data = dataPackage.DataPackageToBytes();

                    foreach (Connection connection in Connections)
                    {
                        SendDataToConnection(connection, data);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        private async void ReadDataFromConnectionStream(Connection connection)
        {
            try
            {
                NetworkStream networkStream = connection.TcpClient.GetStream();
                if (networkStream.DataAvailable && networkStream.CanRead)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        do
                        {
                            byte[] buffer = new byte[32 * 1024];
                            int bytes = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                            memoryStream.Write(buffer, 0, bytes);
                        }
                        while (networkStream.DataAvailable);

                        byte[] data = memoryStream.ToArray();

                        DataPackage receivedDataPackage = new DataPackage(data);

                        OnDataPackageReceived?.Invoke(receivedDataPackage);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);

                if (Connections.Contains(connection))
                {
                    Connections.Remove(connection);
                    Debug.Log("Removed");
                }
            }
        }

        private async void SendDataToConnection(Connection connection, byte[] data)
        {
            try
            {
                NetworkStream networkStream = connection.TcpClient.GetStream();

                if (networkStream.CanWrite)
                {
                    await networkStream.WriteAsync(data, 0, data.Length);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);

                if (Connections.Contains(connection))
                {
                    Debug.Log("Removed");
                    Connections.Remove(connection);
                }
            }
        }
    }
}