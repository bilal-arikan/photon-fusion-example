// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Colors.cs" company="Nick Prühs">
//   Copyright (c) Nick Prühs. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;

public static partial class ArikanExtensions
{
    public static Color WithAlpha(this Color c, float newAlpha)
    {
        return new Color(c.r, c.g, c.b, newAlpha);
    }

    public static Color32 WithAlpha(this Color32 c, byte newAlpha)
    {
        Color32 color = c;
        return new Color32(color.r, color.g, color.b, newAlpha);
    }

    public static Color WithBlue(this Color c, float newBlue)
    {
        return new Color(c.r, c.g, newBlue, c.a);
    }

    public static Color32 WithBlue(this Color32 c, byte newBlue)
    {
        Color32 color = c;
        return new Color32(color.r, color.g, newBlue, color.a);
    }

    public static Color WithGreen(this Color c, float newGreen)
    {
        return new Color(c.r, newGreen, c.b, c.a);
    }

    public static Color32 WithGreen(this Color32 c, byte newGreen)
    {
        Color32 color = c;
        return new Color32(color.r, newGreen, color.b, color.a);
    }

    public static Color WithRed(this Color c, float newRed)
    {
        return new Color(newRed, c.g, c.b, c.a);
    }

    public static Color32 WithRed(this Color32 c, byte newRed)
    {
        Color32 color = c;
        return new Color32(newRed, color.g, color.b, color.a);
    }

    public static int ToInt(this Color color)
    {
        unchecked
        {
            return (Mathf.RoundToInt(color.a * 255) << 24) +
                (Mathf.RoundToInt(color.r * 255) << 16) +
                (Mathf.RoundToInt(color.g * 255) << 8) +
                Mathf.RoundToInt(color.b * 255);
        }
    }

    public static Color ToColor(this int value)
    {
        unchecked
        {
            var a = (float)(value >> 24 & 0xFF) / 255f;
            var r = (float)(value >> 16 & 0xFF) / 255f;
            var g = (float)(value >> 8 & 0xFF) / 255f;
            var b = (float)(value & 0xFF) / 255f;
            return new Color(r, g, b, a);
        }
    }

    public static Color ToColor(this Color32 value)
    {
        return new Color((float)value.r / 255f,
                         (float)value.g / 255f,
                         (float)value.b / 255f,
                         (float)value.a / 255f);
    }

    public static Color ToColor(this Vector3 value)
    {

        return new Color((float)value.x,
                         (float)value.y,
                         (float)value.z);
    }

    public static Color ToColor(this Vector4 value)
    {
        return new Color((float)value.x,
                         (float)value.y,
                         (float)value.z,
                         (float)value.w);
    }

    public static int ToInt(Color32 color)
    {
        return (color.a << 24) +
               (color.r << 16) +
               (color.g << 8) +
               color.b;
    }

    public static Color32 ToColor32(int value)
    {
        byte a = (byte)(value >> 24 & 0xFF);
        byte r = (byte)(value >> 16 & 0xFF);
        byte g = (byte)(value >> 8 & 0xFF);
        byte b = (byte)(value & 0xFF);
        return new Color32(r, g, b, a);
    }

    public static Color32 ToColor32(this Color value)
    {
        return new Color32((byte)(value.r * 255f),
                           (byte)(value.g * 255f),
                           (byte)(value.b * 255f),
                           (byte)(value.a * 255f));
    }

    public static Color32 ToColor32(this Vector3 value)
    {

        return new Color32((byte)(value.x * 255f),
                           (byte)(value.y * 255f),
                           (byte)(value.z * 255f), 255);
    }

    public static Color32 ToColor32(this Vector4 value)
    {
        return new Color32((byte)(value.x * 255f),
                           (byte)(value.y * 255f),
                           (byte)(value.z * 255f),
                           (byte)(value.w * 255f));
    }

}
