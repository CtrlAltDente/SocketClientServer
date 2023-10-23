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

        public void CheckThatConnectionsAreActive()
        {
            for (int i = 0; i < Connections.Count; i++)
            {
                if (!IsConnectionActive(Connections[i]))
                {
                    KillConnection(Connections[i]);
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

        public void ReceiveDataFromAll()
        {
            Connection[] connections = Connections.ToArray();

            foreach (Connection connection in connections)
            {
                ReadDataFromConnection(connection);
            }
        }

        public void SendDataToAll()
        {
            DataToSendCount = DataToSend.Count;     //this is for debug

            while (DataToSend.Count > 0)
            {
                Connection[] connections = Connections.ToArray();

                DataPackage dataPackage = DataToSend.Dequeue();

                byte[] data = dataPackage.DataPackageToBytes();

                foreach (Connection connection in connections)
                {
                    SendDataToConnection(connection, data);
                }
            }
        }

        private void ReadDataFromConnection(Connection connection)
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

        private void SendDataToConnection(Connection connection, byte[] data)
        {
            if (IsConnectionActive(connection))
            {
                connection.AddDataToSend(data);
            }
        }

        private bool IsConnectionActive(Connection connection)
        {
            return connection.TcpClient != null;
        }

        private void KillConnection(Connection connection)
        {
            if (Connections.Contains(connection))
            {
                Debug.Log("Connection is removed");
                Connections.Remove(connection);
            }
        }
    }
}