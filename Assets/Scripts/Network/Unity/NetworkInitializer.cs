using Network.Enums;
using Network.Factories;
using Network.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
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

        [SerializeField]
        private bool _initializeOnStart = false;
        [SerializeField]
        private bool _useLocalNetworkIpAddress = false;

        private void Start()
        {
            SetLocalIpAddress();

            if (_initializeOnStart)
            {
                Initialize(ServerIpAddress);
            }
        }

        public void Initialize(string ipAddress)
        {
            if (NetworkManager != null)
                return;

            string targetIpAddress = ipAddress == string.Empty ? ServerIpAddress : ipAddress;

            try
            {
                NetworkManager = NetworkRoleManagerFactory.CreateNetworkManager(NetworkRole, gameObject);
                NetworkManager.Initialize(targetIpAddress, ServerPort);
                OnNetworkManagerInitialized?.Invoke(NetworkManager);
            }
            catch
            {
                Shutdown();
            }
        }

        public void Shutdown()
        {
            if (NetworkManager == null)
                return;

            NetworkManager.Shutdown();
            Destroy(NetworkManager);
        }

        public string GetLocalIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            return string.Empty;
        }

        private void SetLocalIpAddress()
        {
            if (_useLocalNetworkIpAddress)
            {
                ServerIpAddress = GetLocalIpAddress();
            }
        }
    }
}