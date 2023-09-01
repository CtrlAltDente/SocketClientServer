using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace Network.TCP
{
    public class Connection
    {
        public Queue<byte[]> ReceivedSocketData = new Queue<byte[]>();
        public Queue<byte[]> ToSendSocketData = new Queue<byte[]>();

        private Socket ConnectionSocket;
    }
}