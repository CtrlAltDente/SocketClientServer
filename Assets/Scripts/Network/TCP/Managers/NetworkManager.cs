using System;
using System.Collections.Generic;
using UnityEngine;
using Network.Processors;

namespace Network.Managers
{
    public abstract class NetworkManager : MonoBehaviour
    {
        public bool InitializeOnStart = false;

        [SerializeField] protected BaseTcpProtocol _protocolManager;

        [SerializeField] protected List<NetworkDataProcessor> _networkReceivedDataProcessors = new List<NetworkDataProcessor>();

        protected void Start()
        {
            if (InitializeOnStart)
            {
                Initialize();
            }
        }
        private void Update()
        {
            GiveReceivedDataToDataProcessors();
            UpdateInformation();
        }

        private void OnApplicationQuit()
        {
            Shutdown();
        }

        public abstract void Initialize();

        public virtual void Shutdown()
        {
            if (_protocolManager != null)
            {
                _protocolManager.Shutdown();
                _protocolManager = null;
                GC.Collect();
            }
        }

        public void SendData(byte[] data)
        {
            _protocolManager.DataToSend.Enqueue(data);
        }

        protected void GiveReceivedDataToDataProcessors()
        {
            if (_protocolManager == null)
            {
                return;
            }

            if (_protocolManager.ReceivedData.Count > 0)
            {
                byte[] data = _protocolManager.ReceivedData.Dequeue();

                foreach (NetworkDataProcessor processor in _networkReceivedDataProcessors)
                {
                    processor.ProcessData(data);
                }
            }
        }

        protected abstract void UpdateInformation();       
    }
}