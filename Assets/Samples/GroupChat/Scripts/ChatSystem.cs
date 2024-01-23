using Network.Enums;
using Network.TCP;
using Network.Tools;
using Network.UnityComponents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Network.Samples.GroupChat
{
    public class ChatSystem : MonoBehaviour
    {
        public UnityEvent<ChatInfo> OnChatInfoChanged;

        [SerializeField]
        private NetworkStringSender _networkStringSender;

        [SerializeField]
        private NetworkRole _networkRole;

        [SerializeField]
        private ChatInfo _chatInfo = new ChatInfo();

        [SerializeField]
        private string _localEndIp;

        public ChatInfo ChatInfo => _chatInfo;

        public void UnityNetworkManagerInitialized(UnityNetworkManager unityNetworkManager)
        {
            if (unityNetworkManager.NetworkRole == NetworkRole.Server)
            {
                unityNetworkManager.ConnectionDataManager.OnConnected += AddConnection;
                unityNetworkManager.ConnectionDataManager.OnDisconnected += RemoveConnection;

                _chatInfo.ConnectedIp.Add(unityNetworkManager.LocalEndPoint);
            }

            _networkRole = unityNetworkManager.NetworkRole;
            _localEndIp = unityNetworkManager.LocalEndPoint;
        }

        public void ReceiveChatInfo(string data)
        {
            if (_networkRole != NetworkRole.Client)
                return;

            try
            {
                ChatInfo chatInfo = JsonUtility.FromJson<ChatInfo>(data);
                _chatInfo = chatInfo;

                OnChatInfoChanged?.Invoke(chatInfo);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }

        public void ReceiveChatMessage(string data)
        {
            if (_networkRole != NetworkRole.Server)
                return;

            try
            {
                ChatMessage message = JsonUtility.FromJson<ChatMessage>(data);

                _chatInfo.Messages.Add(message);
                OnChatInfoChanged?.Invoke(_chatInfo);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }

        public void SendChatMessage(string message)
        {
            ChatMessage chatMessage = new ChatMessage();
            chatMessage.Text = message;
            chatMessage.OwnerEndIP = _localEndIp;

            string jsonChatMessage = JsonUtility.ToJson(chatMessage);

            switch (_networkRole)
            {
                case NetworkRole.Server:
                    {
                        _chatInfo.Messages.Add(chatMessage);
                        OnChatInfoChanged?.Invoke(_chatInfo);
                        break;
                    }
                case NetworkRole.Client:
                    {
                        _networkStringSender.SendString(jsonChatMessage);
                        break;
                    }
            }
        }

        private void AddConnection(Connection connection)
        {
            _chatInfo.ConnectedIp.Add(connection.EndIp);
        }

        private void RemoveConnection(Connection connection)
        {
            _chatInfo.ConnectedIp.Remove(connection.EndIp);
        }
    }
}