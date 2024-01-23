using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Network.Samples.GroupChat
{
    [Serializable]
    public struct ChatMessage
    {
        public string OwnerEndIP;
        public string Text;
    }
}