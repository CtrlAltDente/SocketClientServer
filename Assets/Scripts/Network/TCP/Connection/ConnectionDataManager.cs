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
            foreach (Connection connection in Connections)
            {
                ReadDataFromConnection(connection);
            }
        }

        public void SendDataToAll()
        {
            DataToSendCount = DataToSend.Count;     //this is for debug

            while (DataToSend.Count > 0)
            {
                DataPackage dataPackage = DataToSend.Dequeue();

                byte[] data = dataPackage.DataPackageToBytes();

                foreach (Connection connection in Connections)
                {
                    AddDataToConnection(connection, data);
                }
            }
        }

        public void ShutdownConnectionsThreads()
        {
            foreach (Connection connection in Connections)
            {
                connection.ShutdownThreads();
            }
        }

        private void ReadDataFromConnection(Connection connection)
        {
            if (CheckNullConnection(connection))
            {
                if (!connection.IsDataReceivingInProcess)
                {
                    Queue<byte[]> connectionReceivedData = connection.GetAllReceivedData();

                    while (connectionReceivedData.Count > 0)
                    {
                        DataPackage receivedDataPackage = new DataPackage(connectionReceivedData.Dequeue());

                        OnDataPackageReceived?.Invoke(receivedDataPackage);
                    }
                }
            }
        }

        private void AddDataToConnection(Connection connection, byte[] data)
        {
            if (CheckNullConnection(connection))
            {
                connection.AddDataToSend(data);
            }
        }

        private bool CheckNullConnection(Connection connection)
        {
            if (connection.TcpClient == null)
            {
                if (Connections.Contains(connection))
                {
                    Debug.Log("Connection is removed");
                    Connections.Remove(connection);
                }
            }

            return connection.TcpClient != null;
        }
    }
}