namespace Network.Managers
{
    public class ClientManager : NetworkManager
    {
        public bool DiscoverServerInLocalNetwork = false;

        public NetworkTCPClient NetworkTCPClient => (NetworkTCPClient)_protocolManager;

        public override void Initialize()
        {
            _protocolManager = new NetworkTCPClient();
            NetworkTCPClient.Initialize();
        }

        public void ConnectClientToIPAddress(string address)
        {
            NetworkTCPClient.RunConnectionToIPAddress(address);
        }

        protected override void UpdateInformation()
        {

        }
    }
}