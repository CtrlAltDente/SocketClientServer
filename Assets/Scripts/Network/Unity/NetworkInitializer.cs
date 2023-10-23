using Network.Enums;
using Network.Factories;
using Network.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Network.UnityComponents
{
    public class NetworkInitializer : MonoBehaviour
    {
        public string ServerIpAddress = "127.0.0.1";
        public int ServerPort = 3334;

        public NetworkRole NetworkRole;

        public UnityNetworkManager NetworkManager;

        public UnityEvent<UnityNetworkManager> OnNetworkManagerInitialized;

        [SerializeField] private bool _initializeOnStart = false;

        private void Start()
        {
            if (_initializeOnStart)
            {
                Initialize();
            }
        }

        public void Initialize()
        {
            if (NetworkManager != null)
                return;

            NetworkManager = NetworkRoleManagerFactory.CreateNetworkManager(NetworkRole, gameObject);
            NetworkManager.Initialize(ServerIpAddress, ServerPort);
            OnNetworkManagerInitialized?.Invoke(NetworkManager);
        }

        public void Shutdown()
        {
            if (NetworkManager == null)
                return;

            NetworkManager.Shutdown();
            Destroy(NetworkManager);
        }
    }
}