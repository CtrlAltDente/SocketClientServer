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
    public class NetworkStringReceiver : NetworkPackageReceiver
    {
        public UnityEvent<string> OnStringReceived;

        public override void ProcessReceivedDataPackage(DataPackage dataPackage)
        {
            if (dataPackage.DataType == DataType.Text)
            {
                string receivedString = DecodeBytesToString(dataPackage.Data);
                OnStringReceived?.Invoke(receivedString);
                Debug.Log($"Received text: {receivedString}");
            }
        }

        private string DecodeBytesToString(byte[] dataBytes)
        {
            return Encoding.UTF8.GetString(dataBytes);
        }
    }
}