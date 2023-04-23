using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Enums
{
    public static bool Next<T>(this T src, out T value) where T : struct
    {
        if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

        T[] Arr = (T[])Enum.GetValues(src.GetType());
        int j = Array.IndexOf<T>(Arr, src) + 1;
        if (Arr.Length == j)
        {
            value = Arr[0];
            return false;
        }
        else
        {
            value = Arr[j];
            return true;
        }
    }

    public static bool Previous<T>(this T src, out T value) where T : struct
    {
        if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

        T[] Arr = (T[])Enum.GetValues(src.GetType());
        int j = Array.IndexOf<T>(Arr, src) - 1;

        if (j < 0)
        {
            value = Arr[Arr.Length - 1];
            return false;
        }
        else
        {
            value = Arr[j];
            return true;
        }
    }

    public static T ToEnum<T>(string val, T defaultValue) where T : struct, System.IConvertible
    {
        if (!typeof(T).IsEnum) throw new System.ArgumentException("T must be an enumerated type");

        try
        {
            T result = (T)System.Enum.Parse(typeof(T), val, true);
            return result;
        }
        catch
        {
            return defaultValue;
        }
    }

    public static T ToEnum<T>(int val, T defaultValue) where T : struct, System.IConvertible
    {
        if (!typeof(T).IsEnum) throw new System.ArgumentException("T must be an enumerated type");

        try
        {
            return (T)System.Enum.ToObject(typeof(T), val);
        }
        catch
        {
            return defaultValue;
        }
    }

    public static System.Enum ToEnumOfType(System.Type enumType, object value)
    {
        return System.Enum.Parse(enumType, System.Convert.ToString(value), true) as System.Enum;
    }

    public static bool TryToEnum<T>(object val, out T result) where T : struct, System.IConvertible
    {
        if (!typeof(T).IsEnum) throw new System.ArgumentException("T must be an enumerated type");

        try
        {
            result = (T)System.Enum.Parse(typeof(T), System.Convert.ToString(val), true);
            return true;
        }
        catch
        {
            result = default(T);
            return false;
        }
    }

}
