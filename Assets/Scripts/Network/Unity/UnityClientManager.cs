using Network.Data;
using Network.TCP;
using Network.TCP.SocketLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Network.UnityComponents
{
    public class UnityClientManager : UnityNetworkManager
    {
        public override void Initialize(string serverIpAddress, int serverPort)
        {
            _protocolLogic = new TcpClientLogic(DoOnConnectionInitializedOperations);
            _protocolLogic.Initialize(serverIpAddress, serverPort);
        }

        public override void Shutdown()
        {
            StopCoroutine(_networkOperationsCoroutine);
            _protocolLogic.Shutdown();
        }
    }
}