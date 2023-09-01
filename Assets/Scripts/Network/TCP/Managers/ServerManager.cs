using System.Collections.Generic;

namespace Network.Managers
{
    public class ServerManager : NetworkManager
    {
        public NetworkTCPServer NetworkTCPServer => (NetworkTCPServer)_protocolManager;

        public List<Client> ConnectedClients;

        public override void Initialize()
        {
            _protocolManager = new NetworkTCPServer("default");
            NetworkTCPServer.Initialize();
        }

        protected override void UpdateInformation()
        {

        }
    }
}