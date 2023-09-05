using Network.Data;
using Network.TCP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Network.Processors
{
    [Serializable]
    public class ConnectionDataReceiver
    {
        public Action<DataPackage> OnDataPackageReceived;

        [SerializeField]
        private List<Connection> Connections = new List<Connection>();

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

                        Debug.Log($"Received data {receivedDataPackage.DataType}");

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
    }
}