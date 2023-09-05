using Network.Enums;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
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
        
        return RemoveEmptyBytesInArrayAndReturnByteList(byteWithOffset).ToArray();
    }

    private List<byte> RemoveEmptyBytesInArrayAndReturnByteList(byte[] byteArray)
    {
        List<byte> fixedBytes = new List<byte>();

        foreach (byte byteInArray in byteArray)
        {
            if (byteInArray.ToString("X2") != "00")
            {
                fixedBytes.Add(byteInArray);
            }
        }

        return fixedBytes;
    }
}
