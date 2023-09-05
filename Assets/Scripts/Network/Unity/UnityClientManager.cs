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
        public override void Initialize()
        {
            _protocolLogic = new TcpClientLogic(OnConnectionToHandlers);
            _protocolLogic.Initialize(ServerIpAddress, ServerPort);
        }

        public override void Shutdown()
        {
            _protocolLogic.Shutdown();
        }

        public override void SendDataPackage(DataPackage dataPackage)
        {

        }
    }
}