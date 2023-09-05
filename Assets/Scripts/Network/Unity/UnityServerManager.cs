using Network.Data;
using Network.TCP;
using Network.TCP.SocketLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Network.UnityComponents
{
    public class UnityServerManager : UnityNetworkManager
    {
        private Coroutine _listenConnectionsCoroutine;

        public override void Initialize()
        {
            _protocolLogic = new TcpServerLogic(DoOnConnectionInitializedOperations);
            _protocolLogic.Initialize(ServerIpAddress, ServerPort);
        }

        public override void Shutdown()
        {
            StopCoroutine(NetworkOperationsCoroutine);
            _protocolLogic.Shutdown();
        }
    }
}