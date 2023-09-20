using Network.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering;

namespace Network.Tools
{
    public class CameraTextureReceiver : NetworkPackageReceiver
    {
        public UnityEvent<Texture2D> OnCameraTextureReceived;

        private Texture2D _receivedTexture;

        protected override void Start()
        {
            base.Start();

            _receivedTexture = new Texture2D(512, 512, GraphicsFormat.R8G8B8A8_UNorm, TextureCreationFlags.None);

            _decodeDataCoroutine = StartCoroutine(DecodeData());
        }

        public override void ProcessReceivedDataPackage(DataPackage dataPackage)
        {
            if (dataPackage.DataType == Enums.DataType.Image)
            {
                _dataPackagesQueue.Enqueue(dataPackage);
            }
        }

        protected override IEnumerator DecodeData()
        {
            while (true)
            {
                if (_dataPackagesQueue.Count > 0)
                {
                    DataPackage dataPackage = _dataPackagesQueue.Dequeue();

                    ImageConversion.LoadImage(_receivedTexture, dataPackage.Data);

                    OnCameraTextureReceived?.Invoke(_receivedTexture);

                    CheckBufferOverloading();
                }

                yield return null;
            }
        }
    }
}