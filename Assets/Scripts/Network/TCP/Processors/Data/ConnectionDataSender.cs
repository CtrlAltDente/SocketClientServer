using Network.Data;
using Network.TCP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace Network.Processors
{
    [Serializable]
    public class ConnectionDataSender
    {
        public Queue<DataPackage> DataToSend = new Queue<DataPackage>();

        [SerializeField]
        private List<Connection> Connections = new List<Connection>();

        public void AddConnection(Connection connection)
        {
            Connections.Add(connection);
        }

        public void SendDataToAll()
        {
            try
            {
                if (DataToSend.Count > 0)
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