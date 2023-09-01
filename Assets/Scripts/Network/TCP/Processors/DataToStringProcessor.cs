using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Network.Processors
{
    public class DataToStringProcessor : NetworkDataProcessor
    {
        public UnityEvent<string> OnStringReceivedEvent;

        public DataToStringProcessor()
        {
            PROCESSOR_DATA_TYPE = DataType.Text;
        }

        public override void ProcessData(byte[] data)
        {
            DataPackage dataPackage = new DataPackage(data);

            if (dataPackage.DataType == PROCESSOR_DATA_TYPE)
            {
                string text = Encoding.UTF8.GetString(dataPackage.Data);
                OnStringReceivedEvent.Invoke(text);
            }
        }
    }
}