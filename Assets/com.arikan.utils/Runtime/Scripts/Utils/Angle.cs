// //***********************************************************************//
// // Copyright (C) 2017 Bilal Arikan. All Rights Reserved.
// // Author: Bilal Arikan
// // Time  : 04.11.2017   
// //***********************************************************************//
// using System;
// using System.Collections.Generic;
// using UnityEngine;

// namespace Arikan.Tools
// {
//     /// <summary>
//     /// Açı işlemlerini kolaylaştırması için
//     /// </summary>
//     [Serializable]
//     public struct Angle
//     {
//         public const float TwoPI = 360;//6.283185307f;
//         public float PI { get { return UnityEngine.Mathf.PI; } }

//         [SerializeField]
//         private float value;
//         [SerializeField]
//         public readonly float Radian;
//         static float mod = 0;

//         Angle(float value)
//         {
//             this.value = value % TwoPI;
//             if (value < 0)
//                 this.value += TwoPI;
//             this.Radian = value * UnityEngine.Mathf.Deg2Rad;
//         }


//         public static Angle operator +(Angle first, Angle second)
//         {
//             mod = (first.value + second.value) % TwoPI;
//             return new Angle(mod < 0 ? mod + TwoPI : mod);
//         }
//         public static Angle operator -(Angle first, Angle second)
//         {
//             mod = (first.value - second.value) % TwoPI;
//             return new Angle(mod < 0 ? mod + TwoPI : mod);
//         }
//         public static Angle operator *(Angle first, Angle second)
//         {
//             mod = (first.value * second.value) % TwoPI;
//             return new Angle(mod < 0 ? mod + TwoPI : mod);
//         }
//         public static Angle operator /(Angle first, Angle second)
//         {
//             mod = (first.value / second.value) % TwoPI;
//             return new Angle(mod < 0 ? mod + TwoPI : mod);
//         }

//         public static implicit operator float(Angle from)
//         {
//             return from.value;
//         }
//         public static implicit operator Angle(float value)
//         {
//             mod = value % TwoPI;
//             return new Angle(mod < 0 ? mod + TwoPI : mod);
//         }

//         public static implicit operator Vector2(Angle from)
//         {
//             return new Vector2(UnityEngine.Mathf.Cos(from.Radian), UnityEngine.Mathf.Sin(from.Radian));
//         }
//         public static implicit operator Angle(Vector2 value)
//         {
//             //return new Angle(Vector2.Angle(Vector2.right, value)*Mathf.Deg2Rad);
//             return new Angle(UnityEngine.Mathf.Atan2(value.y, value.x) * UnityEngine.Mathf.Rad2Deg);
//         }

//         public override string ToString()
//         {
//             return value.ToString();
//         }

//         public static Angle Lerp(Angle a, Angle b, float alpha, bool shortest = true)
//         {
//             float dist = UnityEngine.Mathf.DeltaAngle(a, b);
//             if (shortest)
//                 return a + (dist * alpha);
//             else
//                 return a + (dist * alpha) + TwoPI / 2;
//         }

//         public static Angle Random()
//         {
//             return new Angle(UnityEngine.Random.Range(0, TwoPI));
//         }

//         public static Angle Random(Angle a, Angle b, bool shortest = true)
//         {
//             return Lerp(a, b, UnityEngine.Random.Range(0f, 1f), shortest);
//         }

//         public static Angle Mean(List<Angle> angles)
//         {
//             Vector2 sum = Vector2.zero;

//             foreach (Angle a in angles)
//                 sum += (Vector2)a;

//             return (Angle)sum;
//         }
// /// <summary>
// /// verilen 2 açıdan targete en yakın olanı 1.si uzak olanı 2. si
// /// olcak şekilde 2 boyutunda array döndürür
// /// </summary>
// /// <param name="target"></param>
// /// <param name="a1"></param>
// /// <param name="a2"></param>
// /// <returns></returns>
// public static Angle[] ClosestAngle(Angle target, Angle a1, Angle a2)
// {
//     float a1Range = Mathf.DeltaAngle(a1, target);
//     float a2Range = Mathf.DeltaAngle(a2, target);

//     a1Range = Mathf.Abs(a1Range);
//     a2Range = Mathf.Abs(a2Range);

//     if (a1Range < a2Range)
//         return new Angle[] { a1, a2 };
//     else
//         return new Angle[] { a2, a1 };
// }

//     }
// }
