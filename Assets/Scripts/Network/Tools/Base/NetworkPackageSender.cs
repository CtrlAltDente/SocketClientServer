using Network.UnityComponents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Network.Tools
{
    public class NetworkPackageSender : MonoBehaviour
    {
        [SerializeField]
        protected UnityNetworkManager _unityNetworkManager = default;

        public void SetUnityNetworkManager(UnityNetworkManager unityNetworkManager)
        {
            _unityNetworkManager = unityNetworkManager;
        }
    }
}