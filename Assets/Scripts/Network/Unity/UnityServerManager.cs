using Network.Data;
using Network.TCP;
using Network.TCP.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Network.UnityComponents
{
    public class UnityServerManager : UnityNetworkManager
    {
        public override void Initialize(string serverIpAddress, int serverPort)
        {
            if (_protocolLogic != null)
                return;

            _protocolLogic = new TcpServerLogic(DoOnConnectionInitializedOperations);

            ServerIpAddress = serverIpAddress;
            ServerPort = serverPort;

            _protocolLogic.Initialize(serverIpAddress, serverPort);

            NetworkRole = Enums.NetworkRole.Server;
            DoOnSuccessfullInitializationOperations();
        }
    }
}