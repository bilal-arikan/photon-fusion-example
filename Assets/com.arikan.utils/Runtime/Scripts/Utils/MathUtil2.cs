//***********************************************************************//
// Copyright (C) 2017 Bilal Arikan. All Rights Reserved.
// Author: Bilal Arikan
// Time  : 04.11.2017   
//***********************************************************************//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Arikan
{
    public static partial class MathUtil
    {
        /// <summary>
		/// Returns the result of rolling a dice of the specified number of sides
		/// </summary>
		public static int RollADice(int numberOfSides = 6)
        {
            return (UnityEngine.Random.Range(1, numberOfSides));
        }


        /// <summary>
		/// Returns a random vector3 from 2 defined vector3.
		/// </summary>
		/// <returns>The vector3.</returns>
		/// <param name="min">Minimum.</param>
		/// <param name="max">Maximum.</param>
		public static Vector3 RandomVector3(Vector3 minimum, Vector3 maximum)
        {
            return new Vector3(UnityEngine.Random.Range(minimum.x, maximum.x),
                                             UnityEngine.Random.Range(minimum.y, maximum.y),
                                             UnityEngine.Random.Range(minimum.z, maximum.z));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static double Next(this System.Random r, double min, double max)
        {
            return r.NextDouble() * (max - min) + min;
        }
        public static float Next(this System.Random r, float min, float max)
        {
            return (float)(r.NextDouble() * (double)(max - min) + (double)min);
        }

        public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
        {
            return Mathf.Atan2(Vector3.Dot(n, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
        }

        /// <summary>
		/// Rotates a point around the given pivot.
		/// </summary>
		/// <returns>The new point position.</returns>
		/// <param name="point">The point to rotate.</param>
		/// <param name="pivot">The pivot's position.</param>
		/// <param name="angle">The angle we want to rotate our point.</param>
		public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, float angle)
        {
            angle = angle * (Mathf.Deg2Rad);
            var rotatedX = Mathf.Cos(angle) * (point.x - pivot.x) - Mathf.Sin(angle) * (point.y - pivot.y) + pivot.x;
            var rotatedY = Mathf.Sin(angle) * (point.x - pivot.x) + Mathf.Cos(angle) * (point.y - pivot.y) + pivot.y;
            return new Vector3(rotatedX, rotatedY, 0);
        }

        /// <summary>
        /// Rotates a point around the given pivot.
        /// </summary>
        /// <returns>The new point position.</returns>
        /// <param name="point">The point to rotate.</param>
        /// <param name="pivot">The pivot's position.</param>
        /// <param name="angles">The angle as a Vector3.</param>
        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angle)
        {
            // we get point direction from the point to the pivot
            Vector3 direction = point - pivot;
            // we rotate the direction
            direction = Quaternion.Euler(angle) * direction;
            // we determine the rotated point's position
            point = direction + pivot;
            return point;
        }

    }
}
