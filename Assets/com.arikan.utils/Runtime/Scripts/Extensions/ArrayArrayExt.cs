using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Arikan
{
    public static partial class ArikanExtensions
    {

        public static Vector2Int Next<T>(this T[][] lst, Vector2 coord) => Next(lst, new Vector2Int((int)coord.x, (int)coord.y));
        public static Vector2Int Next<T>(this T[][] lst, float x, float y) => Next(lst, new Vector2Int((int)x, (int)y));
        public static Vector2Int Next<T>(this T[][] lst, int x, int y) => Next(lst, new Vector2Int(x, y));
        public static Vector2Int Next<T>(this T[][] lst, Vector2Int coord)
        {
            if (lst[coord.x].Length - 1 == coord.y)
            {
                if (lst.Length - 1 == coord.x)
                    return Vector2Int.zero;
                else
                    return coord.AddX(1).SetY(0);
            }
            else
            {
                return coord.AddY(1);
            }
        }

        public static Vector2Int Prev<T>(this T[][] lst, Vector2 coord) => Prev(lst, new Vector2Int((int)coord.x, (int)coord.y));
        public static Vector2Int Prev<T>(this T[][] lst, float x, float y) => Prev(lst, new Vector2Int((int)x, (int)y));
        public static Vector2Int Prev<T>(this T[][] lst, int x, int y) => Prev(lst, new Vector2Int(x, y));
        public static Vector2Int Prev<T>(this T[][] lst, Vector2Int coord)
        {
            if (coord.y == 0)
            {
                if (coord.x == 0)
                    return new Vector2Int(lst.Length - 1, lst[lst.Length - 1].Length - 1);
                else
                    return coord.AddX(-1).SetY(lst[lst.Length - 1].Length - 1);
            }
            else
            {
                return coord.AddY(-1);
            }
        }
        public static T TryGetValue<T>(this T[][] lst, Vector2 coord) => TryGetValue(lst, new Vector2Int((int)coord.x, (int)coord.y));
        public static T TryGetValue<T>(this T[][] lst, Vector2Int coord)
        {
            var l = lst.TryGet(coord.x);
            if (l != null)
                return l.TryGet(coord.y);
            else
                return default(T);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        public static void Randomize<T>(this T[,] values)
        {
            // Get the dimensions.
            int num_rows = values.GetUpperBound(0) + 1;
            int num_cols = values.GetUpperBound(1) + 1;
            int num_cells = num_rows * num_cols;

            // Randomize the array.
            System.Random rand = new System.Random();
            for (int i = 0; i < num_cells - 1; i++)
            {
                // Pick a random cell between i and the end of the array.
                int j = rand.Next(i, num_cells);

                // Convert to row/column indexes.
                int row_i = i / num_cols;
                int col_i = i % num_cols;
                int row_j = j / num_cols;
                int col_j = j % num_cols;

                // Swap cells i and j.
                T temp = values[row_i, col_i];
                values[row_i, col_i] = values[row_j, col_j];
                values[row_j, col_j] = temp;
            }
        }

        public static IEnumerable<T> Neighbours4Side<T>(this T[,] array2d, int x, int y)
        {
            int xSize = array2d.GetLength(0);
            int ySize = array2d.GetLength(1);

            for (int i = -1; i <= 1; i++)
            {
                // yatay index boyutun dışındaysa
                if (!(0 <= x + i && x + i < xSize))
                    continue;

                for (int j = -1; j <= 1; j++)
                {
                    // kendisini es geç
                    if (i == 0 && j == 0)
                        continue;
                    // köşeleri es geç
                    if (i != 0 && j != 0)
                        continue;

                    // dikey index boyutun dışındaysa
                    if (!(0 <= y + j && y + j < ySize))
                        continue;

                    yield return array2d[x + i, y + j];
                }
            }
        }

        public static IEnumerable<T> Neighbours8Side<T>(this T[,] array2d, int x, int y)
        {
            int xSize = array2d.GetLength(0);
            int ySize = array2d.GetLength(1);

            for (int i = -1; i <= 1; i++)
            {
                // yatay index boyutun dışındaysa
                if (!(0 <= x + i && x + i < xSize))
                    continue;

                for (int j = -1; j <= 1; j++)
                {
                    // kendisini es geç
                    if (i == 0 && j == 0)
                        continue;
                    // dikey index boyutun dışındaysa
                    if (!(0 <= y + j && y + j < ySize))
                        continue;

                    yield return array2d[x + i, y + j];
                }
            }
        }
    }

}

