using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static partial class ArikanExtensions
{

    public static IEnumerable<T> Except<T>(this IEnumerable<T> lst, T element)
    {
        foreach (var e in lst)
            if (!object.Equals(e, element)) yield return e;
    }

    public static IEnumerable<T> Except<T>(this IEnumerable<T> lst, T element, IEqualityComparer<T> comparer)
    {
        foreach (var e in lst)
            if (!comparer.Equals(e, element)) yield return e;
    }



    public static int IndexOf(this System.Array lst, object obj)
    {
        return System.Array.IndexOf(lst, obj);
    }
    public static int IndexOf<T>(this T[] lst, T obj)
    {
        return System.Array.IndexOf(lst, obj);
    }
    public static int IndexOf<T>(this IEnumerable<T> items, T item)
    {
        var index = 0;
        foreach (var i in items)
        {
            if (Equals(i, item))
            {
                return index;
            }
            ++index;
        }
        return -1;
    }
    public static int IndexOf<T>(this IEnumerable<T> items, Func<T, bool> predicate)
    {
        var index = 0;
        foreach (var i in items)
        {
            if (predicate(i))
            {
                return index;
            }
            ++index;
        }
        return -1;
    }


    public static IEnumerable<TResult> ConvertAll<T, TResult>(this IEnumerable<T> source, Func<T, TResult> converter)
    {
        foreach (var s in source)
            yield return converter(s);
    }

    public static T TryGet<T>(this T[] a, int index, bool isCircular = false)
    {
        if (a.Length != 0 && index > -1 && index < a.Length)
            return a[index];
        else if (isCircular)
            return a[MathUtil.ModePositive(index, a.Length)];
        else
            return default(T);
    }
    public static bool TrySet<T>(this T[] a, int index, T val, bool isCircular = false)
    {
        if (a.Length != 0 && index > -1 && index < a.Length)
        {
            a[index] = val;
            return true;
        }
        else if (isCircular)
        {
            a[MathUtil.ModePositive(index, a.Length)] = val;
            return true;
        }
        else
            return false;
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ts"></param>
    public static IList<T> Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
        return ts;
    }

    public static T Find<T>(this T[] sequence, Predicate<T> match)
    {
        return Array.Find(sequence, match);
    }
    public static T[] FindAll<T>(this T[] sequence, Predicate<T> match)
    {
        return Array.FindAll(sequence, match);
    }
    public static T FindMax<T>(this IEnumerable<T> e, Func<T, int> func)
    {
        int index = 0;
        int max = int.MinValue;
        T currentMaxItem = default(T);

        foreach (var item in e)
        {
            var value = func(item);
            if (value > max)
            {
                currentMaxItem = item;
                max = value;
            }

            index++;
        }
        return currentMaxItem;
    }
    public static TOut[] ConvertAll<T, TOut>(this T[] sequence, Converter<T, TOut> match)
    {
        return Array.ConvertAll(sequence, match);
    }
    public static void ForEach<T>(this T[] sequence, Action<T> func)
    {
        for (int i = 0; i < sequence.Length; i++)
            func(sequence[i]);
    }
    public static void ForEach<T>(this T[] sequence, Action<int, T> func)
    {
        for (int i = 0; i < sequence.Length; i++)
            func(i, sequence[i]);
    }

    public static void RandomForEach<T>(this List<T> sequence, Action<T> func)
    {
        int[] randomIndexes = new int[sequence.Count];
        for (int i = 0; i < randomIndexes.Length; i++)
            randomIndexes[i] = i;
        ArikanExtensions.Shuffle(randomIndexes);
        for (int i = 0; i < randomIndexes.Length; i++)
            func.Invoke(sequence[randomIndexes[i]]);
    }

    /// <summary>
    ///   Determines whether a sequence is null or doesn't contain any elements.
    /// </summary>
    /// <typeparam name="T">Type of the elements of the sequence to check.</typeparam>
    /// <param name="sequence">Sequence to check. </param>
    /// <returns>
    ///   <c>true</c> if the sequence is null or empty, and
    ///   <c>false</c> otherwise.
    /// </returns>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> sequence)
    {
        if (sequence == null)
        {
            return true;
        }

        return !sequence.Any();
    }


    /// <summary>
    /// 
    /// BitArray
    /// 
    /// </summary>
    /// <param name="bits"></param>
    /// <returns></returns>
    public static byte[] ToByteArray(this System.Collections.BitArray bits)
    {
        int numBytes = (int)System.Math.Ceiling(bits.Count / 8.0f);
        byte[] bytes = new byte[numBytes];

        for (int i = 0; i < bits.Count; i++)
        {
            if (bits[i])
            {
                int j = i / 8;
                int m = i % 8;
                bytes[j] |= (byte)(1 << m);
            }
        }

        return bytes;
    }

    public static string ArrayToString(this IEnumerable ary) => ArrayToString(ary, ',');
    public static string ArrayToString(this IEnumerable ary, char join)
    {
        if (ary == null) return null;

        StringBuilder b = new StringBuilder();
        b.Append('[');
        foreach (var item in ary)
        {
            b.Append(item?.ToString());
            b.Append(join);
        }
        b.Append(']');
        return b.ToString();
    }

    public static T GetRandom<T>(this T[] ts)
    {
        if (ts.Length == 0)
            return default(T);
        else if (ts.Length == 1)
            return ts[0];
        else
            return ts[UnityEngine.Random.Range(0, ts.Length)];
    }
    public static T[] GetRandom<T>(this T[] ts, int amount)
    {
        if (ts.Length == 0)
            return new T[0];
        else if (ts.Length <= amount)
        {
            var indexes = MathUtil.MixedNumbers(0, ts.Length);
            T[] arr = new T[ts.Length];
            for (int i = 0; i < ts.Length; i++)
                arr[i] = ts[indexes[i]];
            ArikanExtensions.Shuffle(arr);
            return arr;
        }
        else
        {
            var indexes = MathUtil.MixedNumbers(0, ts.Length);
            T[] arr = new T[amount];
            for (int i = 0; i < amount; i++)
                arr[i] = ts[indexes[i]];
            return arr;
        }
    }

    public static T GetRandom<T>(this IList<T> ts)
    {
        if (ts.Count == 0)
            return default(T);
        else if (ts.Count == 1)
            return ts[0];
        else
            return ts[UnityEngine.Random.Range(0, ts.Count)];
    }
    public static T[] GetRandom<T>(this IList<T> ts, int amount)
    {
        if (ts.Count == 0)
            return new T[0];
        else if (ts.Count <= amount)
        {
            var indexes = MathUtil.MixedNumbers(0, ts.Count);
            T[] arr = new T[ts.Count];
            for (int i = 0; i < ts.Count; i++)
                arr[i] = ts[indexes[i]];
            ArikanExtensions.Shuffle(arr);
            return arr;
        }
        else
        {
            var indexes = MathUtil.MixedNumbers(0, ts.Count);
            T[] arr = new T[amount];
            for (int i = 0; i < amount; i++)
                arr[i] = ts[indexes[i]];
            return arr;
        }
    }

    public static T GetRandom<T>(this HashSet<T> ts)
    {
        if (ts.Count == 0)
            return default(T);
        else if (ts.Count == 1)
            return ts.First();
        else
            return ts.ElementAt(UnityEngine.Random.Range(0, ts.Count));
    }
    public static KeyValuePair<T1, T2> GetRandom<T1, T2>(this IDictionary<T1, T2> ts)
    {
        if (ts.Count == 0)
            return default(KeyValuePair<T1, T2>);
        else if (ts.Count == 1)
            return ts.First();
        else
            return ts.ElementAt(UnityEngine.Random.Range(0, ts.Count));
    }

    public static IEnumerable<T> Yield<T>(this T t)
    {
        yield return t;
    }
}
