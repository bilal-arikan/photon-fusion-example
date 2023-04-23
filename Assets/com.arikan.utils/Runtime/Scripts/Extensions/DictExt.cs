using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static partial class ArikanExtensions
{
    public static TValue GetValueOrDefault<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key)
    {
        TValue value;
        return dictionary.TryGetValue(key, out value) ? value : default(TValue);
    }
    public static TValue GetValueOrDefault<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        TValue defaultValue)
    {
        TValue value;
        return dictionary.TryGetValue(key, out value) ? value : defaultValue;
    }

    /// <summary>
    /// returns true if added new key-value pair
    /// </summary>
    public static bool GetValueOrDefaultAndSet<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        TValue defaultValue,
        Func<TValue, TValue> setter)
    {
        TValue value = dictionary.GetValueOrDefault(key, defaultValue);
        return dictionary.AddOrChange(key, setter(value));
    }

    /// <summary>
    /// returns true if added
    /// </summary>
    public static bool AddOrChange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
        if (dictionary.ContainsKey(key))
        {
            dictionary[key] = value;
            return false;
        }
        else
        {
            dictionary.Add(key, value);
            return true;
        }
    }
    /// <summary>
    /// returns true if added
    /// </summary>
    public static bool AddOrDontChange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
        if (dictionary.ContainsKey(key))
            return false;
        else
            dictionary.Add(key, value);
        return true;
    }

    public static TValue GetOrAddNew<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) where TValue : new() => dict.GetOrAdd(key, () => new TValue());
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue)
    {
        TValue value;
        if (!dict.TryGetValue(key, out value))
        {
            value = defaultValue;
            dict.Add(key, value);
        }
        return value;
    }
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> defaultValueFunc)
    {
        TValue value;
        if (!dict.TryGetValue(key, out value))
        {
            value = defaultValueFunc();
            dict.Add(key, value);
        }
        return value;
    }

    public static TKey GetKeyOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TValue value, TKey def)
    {
        foreach (var kv in dict)
            if (kv.Value.Equals(value))
                return kv.Key;
        return def;
    }
    public static TKey GetKeyOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TValue value)
    {
        foreach (var kv in dict)
            if (kv.Value.Equals(value))
                return kv.Key;
        return default(TKey);
    }
    public static int RemoveAll<TKey, TValue>(this Dictionary<TKey, TValue> dict, Predicate<KeyValuePair<TKey, TValue>> match)
    {
        int amount = 0;
        foreach (var kv in dict.ToArray().Where(kv => match(kv)))
        {
            dict.Remove(kv.Key);
            amount++;
        }
        return amount;
    }
}
