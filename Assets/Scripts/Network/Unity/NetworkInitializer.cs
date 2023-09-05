using Network.Enums;
using Network.Factories;
using Network.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Network.UnityComponents
{
    public class NetworkInitializer : MonoBehaviour
    {
        public NetworkRole NetworkRole;

        public UnityNetworkManager NetworkManager;

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
            NetworkManager = NetworkRoleManagerFactory.CreateNetworkManager(NetworkRole, gameObject);
            NetworkManager.Initialize();
        }

        public void Shutdown()
        {
            NetworkManager.Shutdown();
            Destroy(NetworkManager);
        }
    }
}