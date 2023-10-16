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

        protected override void Start()
        {
            base.Start();
        }

        public override void ProcessReceivedDataPackage(DataPackage dataPackage)
        {
            if (dataPackage.DataType == DataType.Text)
            {
                _dataPackagesQueue.Enqueue(dataPackage);

                if (_decodeDataCoroutine == null)
                {
                    _decodeDataCoroutine = StartCoroutine(DecodeData());
                }
            }
        }

        protected override IEnumerator DecodeData()
        {
            while (_dataPackagesQueue.Count > 0)
            {
                DataPackage dataPackage = _dataPackagesQueue.Dequeue();
                byte[] dataBytes = dataPackage.Data;
                string receivedString = Encoding.UTF8.GetString(dataBytes);
                OnStringReceived?.Invoke(receivedString);

                CheckBufferOverloading();

                yield return null;
            }
        }

        private List<byte> RemoveEmptyBytesInArrayAndReturnByteList(byte[] byteArray)
        {
            List<byte> fixedBytes = new List<byte>();

            foreach (byte byteInArray in byteArray)
            {
                if (byteInArray.ToString("X2") != "00")
                {
                    fixedBytes.Add(byteInArray);
                }
            }

            return fixedBytes;
        }
    }
}