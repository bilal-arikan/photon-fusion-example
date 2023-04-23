using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public interface IInitWithCrashCheck
{
    string UniqeKey { get; }
}

public static class CrashCheck
{
    private static Dictionary<string, bool> crashDict = new Dictionary<string, bool>();
    public static ReadOnlyDictionary<string, bool> CrashDictionary
    {
        get
        {
            return new ReadOnlyDictionary<string, bool>(crashDict);
        }
    }

    private static string applicationVersion = Application.version;
    public static string ApplicationVersion
    {
        get
        {
            return applicationVersion;
        }
    }

    private static event Action<string> onCrashed;

    public static int ResetAfterCrashCount = 1;

    public static void RegisterForCrash(Action<string> callback)
    {
        onCrashed += callback;
        foreach (var kv in crashDict)
        {
            if (kv.Value)
            {
                callback.Invoke(kv.Key);
            }
        }
    }
    public static void UnregisterForCrash(Action<string> callback)
    {
        onCrashed -= callback;
    }

    public static bool IsCrashed(this IInitWithCrashCheck self, bool showLogIfTrue = true)
    {
        if (PlayerPrefs.GetInt($"{GetCrashCheckPrefKey(self)}_Start") == ResetAfterCrashCount)
        {
            PlayerPrefs.SetInt($"{GetCrashCheckPrefKey(self)}_Start", 0);
            Debug.Log($"{self.UniqeKey} CrashCheck has been reset");
        }
        var started = PlayerPrefs.GetInt($"{GetCrashCheckPrefKey(self)}_Start", 0) > 0;
        if (!started)
        {
            return false;
        }
        var completed = PlayerPrefs.GetInt($"{GetCrashCheckPrefKey(self)}_Completed", 0) > 0;
        bool crashed = started && !completed;
        if (showLogIfTrue && crashed)
        {
            Debug.LogError($"CrashCheck: {self.UniqeKey}_{applicationVersion} is crashed on start, disabling initialization!");
        }
        if (crashed)
        {
            crashDict[self.UniqeKey] = true;
            onCrashed?.Invoke(self.UniqeKey);
        }
        return crashed;
    }
    public static void ResetCrashInfo(this IInitWithCrashCheck self)
    {
        PlayerPrefs.DeleteKey($"{GetCrashCheckPrefKey(self)}_Start");
        PlayerPrefs.DeleteKey($"{GetCrashCheckPrefKey(self)}_Completed");
        crashDict.Remove(self.UniqeKey);
    }

    public static void CrashCheckStart(this IInitWithCrashCheck self)
    {
        PlayerPrefs.SetInt($"{GetCrashCheckPrefKey(self)}_Start", PlayerPrefs.GetInt($"{GetCrashCheckPrefKey(self)}_Start") + 1);
    }

    public static void CrashCheckCompleted(this IInitWithCrashCheck self)
    {
        PlayerPrefs.SetInt($"{GetCrashCheckPrefKey(self)}_Completed", PlayerPrefs.GetInt($"{GetCrashCheckPrefKey(self)}_Completed") + 1);
        PlayerPrefs.SetInt($"{GetCrashCheckPrefKey(self)}_Start", 1);
    }

    private static string GetCrashCheckPrefKey(IInitWithCrashCheck self)
    {
        return $"CrashCheck_{self.UniqeKey}_{applicationVersion}";
    }
}