using Network.Enums;
using Network.UnityComponents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Network.Factories
{
    public static class NetworkRoleManagerFactory
    {
        public static UnityNetworkManager CreateNetworkManager(NetworkRole networkRole, GameObject gameObject)
        {
            UnityNetworkManager networkManager;

            switch (networkRole)
            {
                case NetworkRole.Server:
                    {
                        networkManager = gameObject.AddComponent<UnityServerManager>();
                        return networkManager;
                    }
                case NetworkRole.Client:
                    {
                        networkManager = gameObject.AddComponent<UnityClientManager>();
                        return networkManager;
                    }

                default: throw new Exception("No role created");
            }
        }
    }
}