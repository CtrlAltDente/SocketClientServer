using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Network.Managers
{
    public abstract class BaseTcpProtocol
    {
        public Queue<byte[]> ReceivedData = new Queue<byte[]>();
        public Queue<byte[]> DataToSend = new Queue<byte[]>();

        protected bool _hasAuthentification = false;
        protected string _authentificationToken = "default";

        protected Thread _networkThread;
        protected Thread _sendDataThread;
        protected Thread _listenDataThread;
        protected Thread _checkConnectionThread;

        protected IPAddress[] _availableIPAddresses => GetLocalIPv4Address();

        protected IPAddress _firstDeviceIpAddress => _availableIPAddresses[0];

        public BaseTcpProtocol(string newAuthentificationToken = null)
        {
            _hasAuthentification = newAuthentificationToken != null;
            _authentificationToken = newAuthentificationToken == null ? _authentificationToken : newAuthentificationToken;
        }

        public abstract void Initialize();
        public abstract void Shutdown();
        protected abstract void ListenData();
        protected abstract void SendData();
        protected abstract void CheckConnection();
        protected abstract void Authentification();

        protected async void WriteDataToStream(byte[] data, NetworkStream stream)
        {
            try
            {
                if (stream.CanWrite)
                {
                    await stream.WriteAsync(data, 0, data.Length);

                    Debug.Log("Data writed");
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        protected async void ReadDataFromStream(NetworkStream stream)
        {
            if (stream.CanRead && stream.DataAvailable)
            {
                byte[] buffer = new byte[128 * 1024];
                int bytes;

                using (MemoryStream ms = new MemoryStream())
                {
                    do 
                    {
                        bytes = stream.Read(buffer, 0, buffer.Length);
                        await ms.WriteAsync(buffer, 0, buffer.Length);
                    }
                    while (stream.DataAvailable);

                    ReceivedData.Enqueue(ms.ToArray());
                }
            }
        }

        public static IPAddress[] GetLocalIPv4Address()
        {
            List<IPAddress> ipAddresses = new List<IPAddress>();

            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    if(ip.GetAddressBytes()[0] == 192 && ip.GetAddressBytes()[1] == 168) //IP address in local network
                        ipAddresses.Add(ip);
                }
            }

            if (ipAddresses.Count > 0)
            {
                return ipAddresses.ToArray();
            }
            else
            {
                throw new Exception("No network adapters with an IPv4 address in the system!");
            }
        }
    }
}