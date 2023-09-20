using Network.Enums;
using System;
using System.Collections.Generic;

namespace Network.Data
{
    [Serializable]
    public class DataPackage
    {
        public byte[] Data;
        public DataType DataType;

        public DataPackage(byte[] data, DataType dataType)
        {
            Data = data;
            DataType = dataType;
        }

        public DataPackage(byte[] bytes)
        {
            DataType = (DataType)bytes[0];
            Data = GetByteArrayByOffset(bytes, 1);
        }

        public byte[] DataPackageToBytes()
        {
            byte[] bytes = new byte[Data.Length + 1];
            bytes[0] = (byte)DataType;
            Array.Copy(Data, 0, bytes, 1, Data.Length);
            return bytes;
        }

        private byte[] GetByteArrayByOffset(byte[] byteArray, int offset)
        {
            byte[] byteWithOffset = new byte[byteArray.Length - offset];
            Array.Copy(byteArray, offset, byteWithOffset, 0, byteArray.Length - offset);

            return byteWithOffset;
        }
    }
}