using Network.Managers;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Network.TCP.Sender
{
    public class DataSender
    {
        private BaseTcpProtocol TcpProtocol;

        public DataSender(BaseTcpProtocol tcpProtocol)
        {
            TcpProtocol = tcpProtocol;
        }


    }
}
