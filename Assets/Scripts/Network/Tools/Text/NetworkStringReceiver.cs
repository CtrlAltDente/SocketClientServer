using Network.Data;
using Network.Enums;
using Network.UnityComponents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Network.Tools
{
    public class NetworkStringReceiver : MonoBehaviour
    {
        public UnityEvent<string> OnStringReceived;

        [SerializeField]
        private UnityNetworkManager _unityNetworkManager = default;

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

        public void ProcessReceivedDataPackageAsString(DataPackage dataPackage)
        {
            if (dataPackage.DataType == DataType.Text)
            {
                string receivedString = DecodeBytesToString(dataPackage.Data);
                OnStringReceived?.Invoke(receivedString);
                Debug.Log($"Received text: {receivedString}");
            }
        }

        private void TrySubscribeOnUnityReceivePackageEvent()
        {
            try
            {
                _unityNetworkManager.OnDataPackageReceived.AddListener(ProcessReceivedDataPackageAsString);
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
                _unityNetworkManager.OnDataPackageReceived.RemoveListener(ProcessReceivedDataPackageAsString);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
                Debug.LogWarning("Unity network manager doesn`t setted at start");
            }
        }

        private string DecodeBytesToString(byte[] dataBytes)
        {
            return Encoding.UTF8.GetString(dataBytes);
        }
    }
}