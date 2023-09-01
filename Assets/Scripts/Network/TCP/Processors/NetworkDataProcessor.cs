using UnityEngine;

namespace Network.Processors
{
    public abstract class NetworkDataProcessor : MonoBehaviour
    {
        public DataType PROCESSOR_DATA_TYPE { get; protected set; }

        public abstract void ProcessData(byte[] data);
    }
}