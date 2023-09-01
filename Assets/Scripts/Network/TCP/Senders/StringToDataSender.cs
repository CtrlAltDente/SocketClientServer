using System.Collections;
using System.Collections.Generic;
using System.Text;
using Network.Managers;
using UnityEngine;

namespace Network.Senders
{
    public class StringToDataSender : NetworkDataSender
    {
        public void SendString(string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);

            DataPackage dataPackage = new DataPackage(data, DataType.Text);

            NetworkManager.SendData(dataPackage.DataPackageToBytes());
        }
    }
}