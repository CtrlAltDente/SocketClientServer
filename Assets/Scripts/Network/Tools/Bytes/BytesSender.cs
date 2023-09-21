using Network.Data;
using Network.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Network.Tools
{
    public class BytesSender : NetworkPackageSender
    {
        public void SendBytes(byte[] data)
        {
            DataPackage dataPackage = new DataPackage(data, DataType.Bytes);
            _unityNetworkManager.SendDataPackage(dataPackage);
        }
    }
}