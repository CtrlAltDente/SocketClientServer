using Network.Data;
using Network.Enums;
using Network.UnityComponents;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Network.Tools
{
    public class NetworkStringSender : NetworkPackageSender
    {
        public void SendString(string text)
        {
            byte[] textInBytes = Encoding.UTF8.GetBytes(text);
            DataPackage dataPackage = new DataPackage(textInBytes, DataType.Text);
            _unityNetworkManager.SendDataPackage(dataPackage);
        }
    }
}