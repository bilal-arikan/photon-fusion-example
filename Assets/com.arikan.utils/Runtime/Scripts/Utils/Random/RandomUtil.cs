// using System;
// using UnityEngine;

// namespace Arikan
// {
//     public static class RandomUtil
//     {
//         public const float TWO_PI = 6.28318530717959f;

//         private static UnityRNG _unityRNG = new UnityRNG();

//         public static IRandom Standard { get { return _unityRNG; } }

//         #region Static Properties

//         public static float Angle(this IRandom rng)
//         {
//             return rng.Next() * 360f;
//         }

//         public static float Radian(this IRandom rng)
//         {
//             return rng.Next() * MathUtil.TWO_PI;
//         }

//         /// <summary>
//         /// Return 0 or 1. Numeric version of Bool.
//         /// </summary>
//         /// <returns></returns>
//         public static int Pop(this IRandom rng)
//         {
//             return rng.Next(1000) % 2;
//         }

//         public static int Sign(this IRandom rng)
//         {
//             int n = rng.Next(1000) % 2;
//             return n + n - 1;
//         }

//         /// <summary>
//         /// Return a true randomly.
//         /// </summary>
//         /// <returns></returns>
//         public static bool Bool(this IRandom rng)
//         {
//             return (rng.Next(1000) % 2 != 0);
//         }

//         public static bool Bool(this IRandom rng, float oddsOfTrue)
//         {
//             int i = rng.Next(100000);
//             int m = (int)(oddsOfTrue * 100000);
//             return i < m;
//         }

//         /// <summary>
//         /// Return -1, 0, 1 randomly. This can be used for bizarre things like randomizing an array.
//         /// </summary>
//         /// <returns></returns>
//         public static int Shift(this IRandom rng)
//         {
//             return (rng.Next(999) % 3) - 1;
//         }

//         public static UnityEngine.Vector3 OnUnitSphere(this IRandom rng)
//         {
//             //uniform, using angles
//             var a = rng.Next() * MathUtil.TWO_PI;
//             var b = rng.Next() * MathUtil.TWO_PI;
//             var sa = Mathf.Sin(a);
//             return new Vector3(sa * Mathf.Cos(b), sa * Mathf.Sin(b), Mathf.Cos(a));

//             //non-uniform, needs to test for 0 vector
//             /*
//             var v = new UnityEngine.Vector3(Value, Value, Value);
//             return (v == UnityEngine.Vector3.zero) ? UnityEngine.Vector3.right : v.normalized;
//                 */
//         }

//         public static UnityEngine.Vector2 OnUnitCircle(this IRandom rng)
//         {
//             //uniform, using angles
//             var a = rng.Next() * MathUtil.TWO_PI;
//             return new Vector2(Mathf.Sin(a), Mathf.Cos(a));
//         }

//         public static UnityEngine.Vector3 InsideUnitSphere(this IRandom rng)
//         {
//             return rng.OnUnitSphere() * rng.Next();
//         }

//         public static UnityEngine.Vector2 InsideUnitCircle(this IRandom rng)
//         {
//             return rng.OnUnitCircle() * rng.Next();
//         }

//         public static UnityEngine.Quaternion Rotation(this IRandom rng)
//         {
//             return UnityEngine.Quaternion.AngleAxis(rng.Angle(), rng.OnUnitSphere());
//         }

//         #endregion

//         #region Methods

//         /// <summary>
//         /// Select between min and max, exclussive of max.
//         /// </summary>
//         /// <param name="rng"></param>
//         /// <param name="max"></param>
//         /// <param name="min"></param>
//         /// <returns></returns>
//         public static float Range(this IRandom rng, float max, float min = 0.0f)
//         {
//             return (float)(rng.NextDouble() * (max - min)) + min;
//         }

//         /// <summary>
//         /// Select between min and max, exclussive of max.
//         /// </summary>
//         /// <param name="rng"></param>
//         /// <param name="max"></param>
//         /// <param name="min"></param>
//         /// <returns></returns>
//         public static int Range(this IRandom rng, int max, int min = 0)
//         {
//             return rng.Next(min, max);
//         }

//         /// <summary>
//         /// Select between min and max, exclussive of max.
//         /// </summary>
//         /// <param name="rng"></param>
//         /// <param name="max"></param>
//         /// <param name="min"></param>
//         /// <returns></returns>
//         public static Vector2 Range(this IRandom rng, Vector2 max, Vector2 min)
//         {
//             return new Vector2((float)(rng.NextDouble() * (max.x - min.x)) + min.x, (float)(rng.NextDouble() * (max.y - min.y)) + min.y);
//         }
//         public static Vector3 Range(this IRandom rng, Vector3 max, Vector3 min)
//         {
//             return new Vector3(
//                 (float)(rng.NextDouble() * (max.x - min.x)) + min.x, 
//                 (float)(rng.NextDouble() * (max.y - min.y)) + min.y,
//                 (float)(rng.NextDouble() * (max.z - min.z)) + min.z);
//         }

//         /// <summary>
//         /// Select an weighted index from 0 to length of weights.
//         /// </summary>
//         /// <param name="rng"></param>
//         /// <param name="weights"></param>
//         /// <returns></returns>
//         public static int Range(this IRandom rng, params float[] weights)
//         {
//             int i;
//             float w;
//             float total = 0f;
//             for (i = 0; i < weights.Length; i++)
//             {
//                 w = weights[i];
//                 if (float.IsPositiveInfinity(w)) return i;
//                 else if (w >= 0f && !float.IsNaN(w)) total += w;
//             }

//             if (rng == null) rng = RandomUtil.Standard;
//             float r = rng.Next();
//             float s = 0f;

//             for (i = 0; i < weights.Length; i++)
//             {
//                 w = weights[i];
//                 if (float.IsNaN(w) || w <= 0f) continue;

//                 s += w / total;
//                 if (s > r)
//                 {
//                     return i;
//                 }
//             }

//             //should only get here if last element had a zero weight, and the r was large
//             i = weights.Length - 1;
//             while (i > 0 || weights[i] <= 0f) i--;
//             return i;
//         }

//         /// <summary>
//         /// Select an weighted index from 0 to length of weights.
//         /// </summary>
//         /// <param name="rng"></param>
//         /// <param name="weights"></param>
//         /// <returns></returns>
//         public static int Range(this IRandom rng, float[] weights, int startIndex, int count = -1)
//         {
//             int i;
//             float w;
//             float total = 0f;
//             int last = count < 0 ? weights.Length : System.Math.Min(startIndex + count, weights.Length);
//             for (i = startIndex; i < last; i++)
//             {
//                 w = weights[i];
//                 if (float.IsPositiveInfinity(w)) return i;
//                 else if (w >= 0f && !float.IsNaN(w)) total += w;
//             }

//             if (rng == null) rng = RandomUtil.Standard;
//             float r = rng.Next();
//             float s = 0f;

//             for (i = startIndex; i < last; i++)
//             {
//                 w = weights[i];
//                 if (float.IsNaN(w) || w <= 0f) continue;

//                 s += w / total;
//                 if (s > r)
//                 {
//                     return i;
//                 }
//             }

//             //should only get here if last element had a zero weight, and the r was large
//             i = last - 1;
//             while (i > 0 || weights[i] <= 0f) i--;
//             return i;
//         }


//         #endregion

//     }
// }
