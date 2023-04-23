using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class BinarySerializationUtils
{
    public static byte[] Serialize(object value)
    {
        byte[] temp = null;
        BinaryFormatter bf = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream())
        {
            bf.Serialize(ms, value);
            temp = ms.ToArray();
        }
        return temp;
    }

    public static T Deserialize<T>(byte[] serializedState)
    {
        T temp = default(T);
        BinaryFormatter formatter = new BinaryFormatter();
        using (var ms = new MemoryStream(serializedState))
        {
            temp = (T)formatter.Deserialize(ms);
        }
        return temp;
    }
}