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

            _decodeDataCoroutine = StartCoroutine(DecodeData());
        }

        public override void ProcessReceivedDataPackage(DataPackage dataPackage)
        {
            if (dataPackage.DataType == DataType.Text)
            {
                _dataPackagesQueue.Enqueue(dataPackage);
            }
        }

        protected override IEnumerator DecodeData()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f / 25f);
                yield return new WaitForEndOfFrame();

                while (_dataPackagesQueue.Count > 0)
                {
                    DataPackage dataPackage = _dataPackagesQueue.Dequeue();
                    byte[] dataBytes = dataPackage.Data;
                    string receivedString = Encoding.UTF8.GetString(dataBytes);
                    OnStringReceived?.Invoke(receivedString);

                    yield return null;
                }

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