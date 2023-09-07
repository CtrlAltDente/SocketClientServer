using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Network.Interfaces
{
    public interface IProtocolLogic
    {
        public void Initialize(string serverIpAddress, int serverPort);
        public void Shutdown();
    }
}