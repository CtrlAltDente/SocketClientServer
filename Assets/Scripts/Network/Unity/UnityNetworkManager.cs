using Network.Data;
using Network.Enums;
using Network.Interfaces;
using Network.Processors;
using Network.TCP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Network.UnityComponents
{
    public abstract class UnityNetworkManager : MonoBehaviour
    {
        public string ServerIpAddress = "192.168.88.29";
        public int ServerPort = 3334;

        public Action<DataPackage> OnDataPackageReceived;

        public ConnectionDataSender DataSender = new ConnectionDataSender();
        public ConnectionDataReceiver DataReceiver = new ConnectionDataReceiver();

        protected IProtocolLogic _protocolLogic;

        private void Start()
        {
            DataReceiver.OnDataPackageReceived += RaiseDataPackageReceiveEvent;
            StartCoroutine(DoNetworkLogic());
        }

        public void RaiseDataPackageReceiveEvent(DataPackage dataPackage)
        {
            OnDataPackageReceived?.Invoke(dataPackage);
        }

        public abstract void Initialize();

        public abstract void Shutdown();

        public abstract void SendDataPackage(DataPackage dataPackage);

        protected void OnConnectionToHandlers(Connection connection)
        {
            DataSender.AddConnection(connection);
            DataReceiver.AddConnection(connection);
        }

        private IEnumerator DoNetworkLogic()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.2f);
                DataSender.DataToSend.Enqueue(new DataPackage(new byte[1] { 1 }, DataType.ConnectionCheck));

                DataReceiver.ReceiveDataFromAll();
                DataSender.SendDataToAll();
            }
        }
    }
}