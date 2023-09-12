using Network.Data;
using Network.UnityComponents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Network.Tools
{
    public abstract class NetworkPackageReceiver : MonoBehaviour
    {
        [SerializeField]
        protected UnityNetworkManager _unityNetworkManager = default;

        public abstract void ProcessReceivedDataPackage(DataPackage dataPackage);

        private void Start()
        {
            TrySubscribeOnUnityReceivePackageEvent();
        }

        private void OnDestroy()
        {
            TryUnsubscribeOnUnityReceivePackageEvent();
        }

        public void SetUnityNetworkManager(UnityNetworkManager unityNetworkManager)
        {
            _unityNetworkManager = unityNetworkManager;
            TrySubscribeOnUnityReceivePackageEvent();
        }

        private void TrySubscribeOnUnityReceivePackageEvent()
        {
            try
            {
                _unityNetworkManager.OnDataPackageReceived.AddListener(ProcessReceivedDataPackage);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
                Debug.LogWarning("Unity network manager doesn`t setted at start");
            }
        }

        private void TryUnsubscribeOnUnityReceivePackageEvent()
        {
            try
            {
                _unityNetworkManager.OnDataPackageReceived.RemoveListener(ProcessReceivedDataPackage);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
                Debug.LogWarning("Unity network manager doesn`t setted at start");
            }
        }
    }
}