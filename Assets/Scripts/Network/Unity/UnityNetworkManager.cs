using Network.Data;
using Network.Enums;
using Network.Interfaces;
using Network.Processors;
using Network.TCP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Network.UnityComponents
{
    public abstract class UnityNetworkManager : MonoBehaviour
    {
        public bool InitializeOnStart = false;

        [Range(20, 60)] public int UpdatesPerSecond = 60;

        public string ServerIpAddress = "127.0.0.1";
        public int ServerPort = 3334;

        public UnityEvent<DataPackage> OnDataPackageReceived = new UnityEvent<DataPackage>();

        [SerializeField]
        private ConnectionDataManager _connectionDataManager = new ConnectionDataManager();

        private Queue<DataPackage> _receivedDataPackages = new Queue<DataPackage>();
        private Queue<DataPackage> _dataPackagesToSendBuffer = new Queue<DataPackage>();

        [SerializeField]
        private int _receivedCount = 0;

        protected Thread _sendPackagesThread;
        protected Thread _receivePackagesThread;

        protected IProtocolLogic _protocolLogic;

        [SerializeField]
        protected bool _isStarted = false;

        private void Start()
        {
            if (InitializeOnStart)
            {
                Initialize();
            }
        }

        private void OnDestroy()
        {
            if (_isStarted)
            {
                Shutdown();
            }
        }

        public void RaiseDataPackageReceiveEvent(DataPackage dataPackage)
        {
            _receivedDataPackages.Enqueue(dataPackage);
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

        public void Shutdown()
        {
            _isStarted = false;
            _connectionDataManager.OnDataPackageReceived -= RaiseDataPackageReceiveEvent;
            _connectionDataManager.ShutdownConnectionsThreads();
            _protocolLogic.Shutdown();
        }

        public abstract void Initialize(string serverIpAddress, int serverPort);

        protected void DoOnConnectionInitializedOperations(Connection connection)
        {
            _connectionDataManager.AddConnection(connection);
        }

        protected void DoOnSuccessfullInitializationOperations()
        {
            _isStarted = true;
            StartCoroutine(ApplyDataToProcessors());

            _connectionDataManager.OnDataPackageReceived += RaiseDataPackageReceiveEvent;

            _sendPackagesThread = new Thread(SendDataPackages);
            _sendPackagesThread.Start();

            _receivePackagesThread = new Thread(ReceiveDataPackages);
            _receivePackagesThread.Start();
        }

        private IEnumerator ApplyDataToProcessors()
        {
            while (_isStarted)
            {
                while (_receivedDataPackages.Count > 0)
                {
                    DataPackage dataPackage = _receivedDataPackages.Dequeue();

                    OnDataPackageReceived?.Invoke(dataPackage);

                    yield return null;
                }

                yield return null;
            }

            Debug.LogWarning("Data processing ended");
        }

        private void SendDataPackages()
        {
            while (_isStarted)
            {
                if (_connectionDataManager.ConnectionsCount > 0)
                {
                    MoveDataPackageFromBufferToManager();
                    _connectionDataManager.SendDataToAll();
                }
                else
                {
                    if (_dataPackagesToSendBuffer.Count > 0)
                    {
                        _dataPackagesToSendBuffer.Clear();
                    }
                }
            }

            Debug.LogWarning("Data sending ended");
        }

        private void ReceiveDataPackages()
        {
            while (_isStarted)
            {
                if (_connectionDataManager.ConnectionsCount > 0)
                {
                    _connectionDataManager.ReceiveDataFromAll();
                    _receivedCount = _receivedDataPackages.Count;
                }
            }

            Debug.LogWarning("Data receiving ended");
        }

        private void MoveDataPackageFromBufferToManager()
        {
            while (_dataPackagesToSendBuffer.Count > 0)
            {
                DataPackage dataPackage = _dataPackagesToSendBuffer.Dequeue();
                _connectionDataManager.DataToSend.Enqueue(dataPackage);
            }
        }
    }
}