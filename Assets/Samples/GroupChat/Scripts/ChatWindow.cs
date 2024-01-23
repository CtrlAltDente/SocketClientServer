using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Network.Samples.GroupChat
{
    public class ChatWindow : MonoBehaviour
    {
        [SerializeField]
        private ChatInfo _chatInfo;

        [SerializeField]
        private ChatSystem _chatSystem;

        [SerializeField]
        private TMP_InputField _messageField;

        [SerializeField]
        public TextMeshProUGUI _messages;

        public void SetChatInfo(ChatInfo chatInfo)
        {
            SetMessages(chatInfo);
            _chatInfo = chatInfo;
        }

        public void SendChatMessage()
        {
            _chatSystem.SendChatMessage(_messageField.text);
            _messageField.text = string.Empty;
        }

        private void SetMessages(ChatInfo chatInfo)
        {
            _messages.text = string.Empty;
            
            foreach (var message in chatInfo.Messages)
            {
                _messages.text += $"{message.OwnerEndIP}: {message.Text}\n\n";
            }
        }
    }
}