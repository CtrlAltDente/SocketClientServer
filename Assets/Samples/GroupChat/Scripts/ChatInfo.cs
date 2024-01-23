using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Network.Samples.GroupChat
{
    [Serializable]
    public struct ChatInfo
    {
        public List<string> ConnectedIp;
        public List<ChatMessage> Messages;
    }
}