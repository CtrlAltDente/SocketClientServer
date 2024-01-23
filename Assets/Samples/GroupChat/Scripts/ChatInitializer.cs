using Network.Enums;
using Network.UnityComponents;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Network.Samples.GroupChat
{
    public class ChatInitializer : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _localIp;
        [SerializeField]
        private TMP_InputField _targetIpField;

        [SerializeField]
        private NetworkInitializer _networkInitializer;

        public void StartServer()
        {
            _networkInitializer.NetworkRole = NetworkRole.Server;

            string localIP = _networkInitializer.GetLocalIpAddress();
            _localIp.text = localIP;

            _networkInitializer.Initialize(localIP);
        }

        public void StartClient()
        {
            _networkInitializer.NetworkRole = NetworkRole.Client;
            _networkInitializer.Initialize(_targetIpField.text);
        }
    }
}