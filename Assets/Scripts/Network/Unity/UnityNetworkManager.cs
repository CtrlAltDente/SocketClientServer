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
        protected bool InitializeOnStart = false;

        [Range(20, 60)] public int UpdatesPerSecond = 40;

        public string ServerIpAddress = "192.168.88.29";
        public int ServerPort = 3334;

        public UnityEvent<DataPackage> OnDataPackageReceived = new UnityEvent<DataPackage>();

        [SerializeField]
        private ConnectionDataManager _connectionDataManager = new ConnectionDataManager();
        private Queue<DataPackage> _dataPackagesToSendBuffer = new Queue<DataPackage>();

        protected Coroutine _networkOperationsCoroutine;

        protected IProtocolLogic _protocolLogic;

        private void Start()
        {
            if(InitializeOnStart)
            {
                Initialize();
            }

            _connectionDataManager.OnDataPackageReceived += RaiseDataPackageReceiveEvent;
            _networkOperationsCoroutine = StartCoroutine(ReceiveAndSendDataPackages());
        }

        public void RaiseDataPackageReceiveEvent(DataPackage dataPackage)
        {
            OnDataPackageReceived?.Invoke(dataPackage);
        }

        public void SendDataPackage(DataPackage dataPackage)
        {
            if (_connectionDataManager.ConnectionsCount > 0)
            {
                _dataPackagesToSendBuffer.Enqueue(dataPackage);
            }
        }

        public void Initialize()
        {
            Initialize(ServerIpAddress, ServerPort);
        }

        public abstract void Initialize(string serverIpAddress, int serverPort);

        public abstract void Shutdown();

        protected void DoOnConnectionInitializedOperations(Connection connection)
        {
            _connectionDataManager.AddConnection(connection);
        }

        private IEnumerator ReceiveAndSendDataPackages()
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
                    if(_dataPackagesToSendBuffer.Count > 0)
                    {
                        _dataPackagesToSendBuffer.Clear();
                    }
                }
            }
        }

        private void MoveDataPackageFromBufferToManager()
        {
            if (_dataPackagesToSendBuffer.Count > 0)
            {
                while(_dataPackagesToSendBuffer.Count>0)
                {
                    _connectionDataManager.DataToSend.Enqueue(_dataPackagesToSendBuffer.Dequeue());
                }

                _dataPackagesToSendBuffer.Clear();
            }
        }
    }
}