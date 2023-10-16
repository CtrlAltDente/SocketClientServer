using Network.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering;

namespace Network.Tools
{
    public class CameraTextureSender : NetworkPackageSender
    {
        public UnityEvent<Texture2D> OnImageReady;

        public bool StartStreamAtStart;

        [SerializeField]
        private Camera _camera = default;

        [SerializeField][Range(5,120)]
        private int _fps = 40;
        [SerializeField]
        private Vector2 _imageSize = default;

        RenderTexture _renderTexture;
        [SerializeField]
        Texture2D _cameraTexture;

        private Coroutine CameraImageEncodingCoroutine;

        private void Start()
        {
            InitializeComponents();

            if (StartStreamAtStart)
            {
                StartStream();
            }
        }

        private void OnDestroy()
        {
            StopStream();
        }

        public void StartStream()
        {
            CameraImageEncodingCoroutine = StartCoroutine(EncodeCameraImage());
        }

        public void StopStream()
        {
            StopCoroutine(CameraImageEncodingCoroutine);
        }

        private void InitializeComponents()
        {
            _renderTexture = new RenderTexture((int)_imageSize.x, (int)_imageSize.y, 0, GraphicsFormat.R8G8B8A8_UNorm, 0);
            _cameraTexture = new Texture2D((int)_imageSize.x, (int)_imageSize.y, GraphicsFormat.R8G8B8A8_UNorm, TextureCreationFlags.None);
        }

        private IEnumerator EncodeCameraImage()
        {
            while (_camera != null)
            {
                if (_unityNetworkManager != null)
                {
                    yield return new WaitForSeconds(1f / (float)_fps);
                    yield return new WaitForEndOfFrame();

                    ConvertCameraRenderTextureToTexture2D();

                    byte[] cameraTextureBytes = ImageConversion.EncodeToJPG(_cameraTexture, 75);

                    DataPackage dataPackage = new DataPackage(cameraTextureBytes, Enums.DataType.Image);
                    _unityNetworkManager.SendDataPackage(dataPackage);
                }
                else
                {
                    yield return null;
                }
            }
        }

        private void ConvertCameraRenderTextureToTexture2D()
        {
            RenderTexture.active = RenderCameraViewToRenderTexture();

            _cameraTexture.ReadPixels(new Rect(0, 0, RenderTexture.active.width, RenderTexture.active.height), 0, 0);
            _cameraTexture.Apply();
        }

        private RenderTexture RenderCameraViewToRenderTexture()
        {
            _camera.targetTexture = _renderTexture;
            _camera.Render();
            _camera.targetTexture = null;
            return _renderTexture;
        }
    }
}