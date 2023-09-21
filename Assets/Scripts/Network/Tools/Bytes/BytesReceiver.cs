using Network.Data;
using Network.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Network.Tools
{
    public class BytesReceiver : NetworkPackageReceiver
    {
        public UnityEvent<byte[]> OnBytesReceived;

        public override void ProcessReceivedDataPackage(DataPackage dataPackage)
        {
            if(dataPackage.DataType == DataType.Bytes)
            {
                _dataPackagesQueue.Enqueue(dataPackage);
            }
        }

        protected override IEnumerator DecodeData()
        {
            while (true)
            {
                while(_dataPackagesQueue.Count > 0)
                {
                    DataPackage dataPackage = _dataPackagesQueue.Dequeue();

                    OnBytesReceived?.Invoke(dataPackage.Data);

                    CheckBufferOverloading();
                }

                yield return null;
            }
        }
    }
}