using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Arikan
{
    public static class Directions
    {
        const float SquareOfTwo = 0.707105f;

        public static readonly Dictionary<Direction, Vector2> Vectors = new Dictionary<Direction, Vector2>()
        {
            { Direction.Right,new Vector2(1,0) },
            { Direction.TopRight,new Vector2(SquareOfTwo,SquareOfTwo) },
            { Direction.Top,new Vector2(0,1) },
            { Direction.TopLeft,new Vector2(-SquareOfTwo,SquareOfTwo) },
            { Direction.Left,new Vector2(-1,0) },
            { Direction.BottomLeft,new Vector2(-SquareOfTwo,-SquareOfTwo) },
            { Direction.Bottom,new Vector2(0,-1) },
            { Direction.BottomRight,new Vector2(SquareOfTwo,-SquareOfTwo) },
        };
        /// <summary>
        /// Degree (not radian)
        /// </summary>
        public static readonly Dictionary<Direction, float> Angles = new Dictionary<Direction, float>()
        {
            { Direction.Right,     0f },
            { Direction.TopRight,  45f },
            { Direction.Top,       90f },
            { Direction.TopLeft,   135f },
            { Direction.Left,      180f },
            { Direction.BottomLeft,225f },
            { Direction.Bottom,    270f},
            { Direction.BottomRight,315f },
        };

        public static Vector2 Vector(this Direction r)
        {
            return Vectors[r];
        }
        public static float Angle(this Direction r)
        {
            return Angles[r];
        }
        public static bool IsDiagonal(this Direction r)
        {
            if ((int)r % 2 == 0)
                return false;
            else
                return true;
        }
        public static Direction Reverse(this Direction d)
        {
            if ((int)d < 4)
                return (Direction)(d + 4);
            else
                return (Direction)(d - 4);
        }
    }

    public enum Direction
    {
        Right = 0,
        TopRight = 1,
        Top = 2,
        TopLeft = 3,
        Left = 4,
        BottomLeft = 5,
        Bottom = 6,
        BottomRight = 7
    }

}
