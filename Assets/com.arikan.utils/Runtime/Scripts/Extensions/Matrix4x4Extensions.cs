using System.Collections.Generic;
using UnityEngine;

namespace Arikan
{
    public static class Matrix4x4Extensions
    {
        public static Matrix4x4 RotateX(float aAngleRad)
        {
            Matrix4x4 m = Matrix4x4.identity;     //  1   0   0   0 
            m.m11 = m.m22 = Mathf.Cos(aAngleRad); //  0  cos -sin 0
            m.m21 = Mathf.Sin(aAngleRad);         //  0  sin  cos 0
            m.m12 = -m.m21;                       //  0   0   0   1
            return m;
        }
        public static Matrix4x4 RotateY(float aAngleRad)
        {
            Matrix4x4 m = Matrix4x4.identity;     // cos  0  sin  0
            m.m00 = m.m22 = Mathf.Cos(aAngleRad); //  0   1   0   0
            m.m02 = Mathf.Sin(aAngleRad);         //-sin  0  cos  0
            m.m20 = -m.m02;                       //  0   0   0   1
            return m;
        }
        public static Matrix4x4 RotateZ(float aAngleRad)
        {
            Matrix4x4 m = Matrix4x4.identity;     // cos -sin 0   0
            m.m00 = m.m11 = Mathf.Cos(aAngleRad); // sin  cos 0   0
            m.m10 = Mathf.Sin(aAngleRad);         //  0   0   1   0
            m.m01 = -m.m10;                       //  0   0   0   1
            return m;
        }
        public static Matrix4x4 Rotate(this Matrix4x4 m, Vector3 aEulerAngles)
        {
            var rad = aEulerAngles * Mathf.Deg2Rad;
            return m * RotateY(rad.y) * RotateX(rad.x) * RotateZ(rad.z);
        }
        public static Matrix4x4 Scale(Vector3 aScale)
        {
            var m = Matrix4x4.identity; //  sx   0   0   0
            m.m00 = aScale.x;           //   0  sy   0   0
            m.m11 = aScale.y;           //   0   0  sz   0
            m.m22 = aScale.z;           //   0   0   0   1
            return m;
        }
        public static Matrix4x4 Translation(this Matrix4x4 m, Vector3 aPosition)
        {
            m.m03 = aPosition.x;        // 0   1   0   y
            m.m13 = aPosition.y;        // 0   0   1   z
            m.m23 = aPosition.z;        // 0   0   0   1
            return m;
        }

        public static Quaternion GetRotation(this Matrix4x4 m)
        {
            Vector3 forward;
            forward.x = m.m02;
            forward.y = m.m12;
            forward.z = m.m22;

            Vector3 upwards;
            upwards.x = m.m01;
            upwards.y = m.m11;
            upwards.z = m.m21;

            return Quaternion.LookRotation(forward, upwards);
        }
        public static Vector3 GetPosition(this Matrix4x4 m)
        {
            Vector3 position;
            position.x = m.m03;
            position.y = m.m13;
            position.z = m.m23;
            return position;
            //return m.MultiplyPoint(Vector3.zero);
        }
        public static Vector3 GetScale(this Matrix4x4 m)
        {
            Vector3 scale;
            scale.x = new Vector4(m.m00, m.m10, m.m20, m.m30).magnitude;
            scale.y = new Vector4(m.m01, m.m11, m.m21, m.m31).magnitude;
            scale.z = new Vector4(m.m02, m.m12, m.m22, m.m32).magnitude;
            return scale;
        }

        /// <summary>
        /// Element-wise addition of two Matrix4x4s - extension method
        /// </summary>
        /// <param name="a">matrix</param>
        /// <param name="b">matrix</param>
        /// <returns>element-wise (a+b)</returns>
        public static Matrix4x4 Add(this Matrix4x4 a, Matrix4x4 b)
        {
            Matrix4x4 result = new Matrix4x4();
            result.SetColumn(0, a.GetColumn(0) + b.GetColumn(0));
            result.SetColumn(1, a.GetColumn(1) + b.GetColumn(1));
            result.SetColumn(2, a.GetColumn(2) + b.GetColumn(2));
            result.SetColumn(3, a.GetColumn(3) + b.GetColumn(3));
            return result;
        }

        /// <summary>
        /// Element-wise subtraction of two Matrix4x4s - extension method
        /// </summary>
        /// <param name="a">matrix</param>
        /// <param name="b">matrix</param>
        /// <returns>element-wise (a-b)</returns>
        public static Matrix4x4 Subtract(this Matrix4x4 a, Matrix4x4 b)
        {
            Matrix4x4 result = new Matrix4x4();
            result.SetColumn(0, a.GetColumn(0) - b.GetColumn(0));
            result.SetColumn(1, a.GetColumn(1) - b.GetColumn(1));
            result.SetColumn(2, a.GetColumn(2) - b.GetColumn(2));
            result.SetColumn(3, a.GetColumn(3) - b.GetColumn(3));
            return result;
        }

        public static Matrix4x4 FromArray(this float[] m)
        {
            Matrix4x4 arr = new Matrix4x4();
            arr[0, 0] = m[0];
            arr[1, 0] = m[1];
            arr[2, 0] = m[2];
            arr[3, 0] = m[3];
            arr[0, 1] = m[4];
            arr[1, 1] = m[5];
            arr[2, 1] = m[6];
            arr[3, 1] = m[7];
            arr[0, 2] = m[8];
            arr[1, 2] = m[9];
            arr[2, 2] = m[10];
            arr[3, 2] = m[11];
            arr[0, 3] = m[12];
            arr[1, 3] = m[13];
            arr[2, 3] = m[14];
            arr[3, 3] = m[15];
            return arr;
        }
        public static Matrix4x4 FromArray(this List<float> m)
        {
            Matrix4x4 arr = new Matrix4x4();
            arr[0, 0] = m[0];
            arr[1, 0] = m[1];
            arr[2, 0] = m[2];
            arr[3, 0] = m[3];
            arr[0, 1] = m[4];
            arr[1, 1] = m[5];
            arr[2, 1] = m[6];
            arr[3, 1] = m[7];
            arr[0, 2] = m[8];
            arr[1, 2] = m[9];
            arr[2, 2] = m[10];
            arr[3, 2] = m[11];
            arr[0, 3] = m[12];
            arr[1, 3] = m[13];
            arr[2, 3] = m[14];
            arr[3, 3] = m[15];
            return arr;
        }
        public static float[] MakeArray(this Matrix4x4 m)
        {
            float[] arr = new float[16];
            arr[0] = m[0, 0];
            arr[1] = m[1, 0];
            arr[2] = m[2, 0];
            arr[3] = m[3, 0];
            arr[4] = m[0, 1];
            arr[5] = m[1, 1];
            arr[6] = m[2, 1];
            arr[7] = m[3, 1];
            arr[8] = m[0, 2];
            arr[9] = m[1, 2];
            arr[10] = m[2, 2];
            arr[11] = m[3, 2];
            arr[12] = m[0, 3];
            arr[13] = m[1, 3];
            arr[14] = m[2, 3];
            arr[15] = m[3, 3];
            return arr;
        }
    }
}