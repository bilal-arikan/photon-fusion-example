using UnityEngine;

namespace Arikan
{
    public static class ColorUtils
    {
        public static Color Random()
        {
            Color c = new Color();
            c[0] = UnityEngine.Random.value;
            c[1] = UnityEngine.Random.value;
            c[2] = UnityEngine.Random.value;
            c[3] = 1;
            return c;
        }
        public static Color Random(Color c1, Color c2)
        {
            Color c = new Color();
            c[0] = UnityEngine.Random.Range(c1[0], c2[0]);
            c[1] = UnityEngine.Random.Range(c1[1], c2[1]);
            c[2] = UnityEngine.Random.Range(c1[2], c2[2]);
            c[3] = 1;
            return c;
        }
        public static Color Lerp(float t, params Color[] colors)
        {
            if (colors == null || colors.Length == 0) return Color.black;
            if (colors.Length == 1) return colors[0];

            int i = Mathf.FloorToInt(colors.Length * t);
            if (i < 0) i = 0;
            if (i >= colors.Length - 1) return colors[colors.Length - 1];

            t %= 1f / (float)(colors.Length - 1);
            return Color.Lerp(colors[i], colors[i + 1], t);
        }
    }

}
