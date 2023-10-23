using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Network.TCP.Core
{
    public class TcpBaseLogic
    {
        public Action<Connection> OnConnectionInitialized;

        public TcpBaseLogic(Action<Connection> OnConnectionAction)
        {
            OnConnectionInitialized += OnConnectionAction;
        }
    }
}