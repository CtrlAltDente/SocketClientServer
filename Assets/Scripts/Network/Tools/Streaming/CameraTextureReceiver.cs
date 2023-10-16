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

        [SerializeField]
        private Vector2 _receivedTextureSize = new Vector2(1024, 1024);

        private Texture2D _receivedTexture;

        protected override void Start()
        {
            base.Start();

            _receivedTexture = new Texture2D((int)_receivedTextureSize.x, (int)_receivedTextureSize.y, GraphicsFormat.R8G8B8A8_UNorm, TextureCreationFlags.None);
        }

        public override void ProcessReceivedDataPackage(DataPackage dataPackage)
        {
            if (dataPackage.DataType == Enums.DataType.Image)
            {
                _dataPackagesQueue.Enqueue(dataPackage);
            }
        }

        protected override void DecodeData()
        {
            while (_dataPackagesQueue.Count > 0)
            {
                DataPackage dataPackage = _dataPackagesQueue.Dequeue();
                ImageConversion.LoadImage(_receivedTexture, dataPackage.Data);
                OnCameraTextureReceived?.Invoke(_receivedTexture);

                CheckBufferOverloading();
            }
        }
    }
}