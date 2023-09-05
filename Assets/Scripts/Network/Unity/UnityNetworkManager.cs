using Network.Data;
using Network.Enums;
using Network.Interfaces;
using Network.Processors;
using Network.TCP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

namespace Network.UnityComponents
{
    public abstract class UnityNetworkManager : MonoBehaviour
    {
        [Range(20, 60)] public int UpdatesPerSecond = 40;

        public string ServerIpAddress = "192.168.88.29";
        public int ServerPort = 3334;

        public UnityEvent<DataPackage> OnDataPackageReceived;

        [SerializeField]
        private ConnectionDataManager _connectionDataManager = new ConnectionDataManager();
        private Queue<DataPackage> DataPackagesToSendBuffer = new Queue<DataPackage>();

        protected Coroutine NetworkOperationsCoroutine;

        protected IProtocolLogic _protocolLogic;

        private void Start()
        {
            _connectionDataManager.OnDataPackageReceived += RaiseDataPackageReceiveEvent;
            NetworkOperationsCoroutine = StartCoroutine(DoNetworkLogic());
        }

        public void RaiseDataPackageReceiveEvent(DataPackage dataPackage)
        {
            OnDataPackageReceived?.Invoke(dataPackage);
        }

        public void SendDataPackage(DataPackage dataPackage)
        {
            if (_connectionDataManager.ConnectionsCount > 0)
            {
                DataPackagesToSendBuffer.Enqueue(dataPackage);
            }
        }

        public abstract void Initialize();

        public abstract void Shutdown();

        protected void DoOnConnectionInitializedOperations(Connection connection)
        {
            _connectionDataManager.AddConnection(connection);
        }

        private IEnumerator DoNetworkLogic()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f / (float)UpdatesPerSecond);

                if (_connectionDataManager.ConnectionsCount > 0)
                {
                    _connectionDataManager.DataToSend.Enqueue(new DataPackage(new byte[1] { 1 }, DataType.ConnectionCheck));

                    MoveDataPackageFromBufferToManager();

                    _connectionDataManager.ReceiveDataFromAll();
                    _connectionDataManager.SendDataToAll();
                }
                else
                {
                    if(DataPackagesToSendBuffer.Count > 0)
                    {
                        DataPackagesToSendBuffer.Clear();
                    }
                }
            }
        }

        private void MoveDataPackageFromBufferToManager()
        {
            foreach(DataPackage dataPackage in DataPackagesToSendBuffer)
            {
                _connectionDataManager.DataToSend.Enqueue(DataPackagesToSendBuffer.Dequeue());
            }

            DataPackagesToSendBuffer.Clear();
        }
    }
}