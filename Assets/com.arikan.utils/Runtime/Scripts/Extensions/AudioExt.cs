using Arikan;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioInterruptMode
{
    StopIfPlaying,
    DoNotPlayIfPlaying,
    PlayOverExisting
}

public static partial class ArikanExtensions
{
    public static IEnumerator Fade(this AudioSource src, float target, float duration)
    {
        float targetTime = Time.unscaledTime + duration;
        var def = src.volume;

        while (Time.unscaledTime < targetTime)
        {
            src.volume = Mathf.Lerp(target, def, (targetTime - Time.unscaledTime) / duration);
            yield return null;
        }
        src.volume = target;
    }

    public static void Play(this AudioSource src, AudioClip clip, AudioInterruptMode mode)
    {
        if (clip == null) throw new System.ArgumentNullException("clip");

        switch (mode)
        {
            case AudioInterruptMode.StopIfPlaying:
                if (src.isPlaying) src.Stop();
                break;
            case AudioInterruptMode.DoNotPlayIfPlaying:
                if (src.isPlaying) return;
                break;
            case AudioInterruptMode.PlayOverExisting:
                break;
        }

        src.PlayOneShot(clip);
    }

    public static void Play(this AudioSource src, AudioClip clip, float volumeScale, AudioInterruptMode mode)
    {
        if (clip == null) throw new System.ArgumentNullException("clip");

        switch (mode)
        {
            case AudioInterruptMode.StopIfPlaying:
                if (src.isPlaying) src.Stop();
                break;
            case AudioInterruptMode.DoNotPlayIfPlaying:
                if (src.isPlaying) return;
                break;
            case AudioInterruptMode.PlayOverExisting:
                break;
        }

        src.PlayOneShot(clip, volumeScale);
    }

}
