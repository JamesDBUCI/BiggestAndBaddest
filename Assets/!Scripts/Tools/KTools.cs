using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KTools
{
    public static bool SameString(string a, string b)
    {
        return a.ToUpper() == b.ToUpper();
    }
    public static Color ColorFromRGBA(int r, int g, int b, int a = 255)
    {
        return new Color(r / 255, g / 255, b / 255, a / 255);
    }
    public static bool TryGetColorFromRGBA(out Color outColor, string rHex, string gHex, string bHex, string aHex = "FF")
    {
        outColor = Color.magenta;

        string[] strings = new string[] { rHex, gHex, bHex, aHex };
        int[] components = new int[4];

        for (int i = 0; i < components.Length; i++)
        {
            if (!int.TryParse(strings[i], System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out components[i]))
            {
                return false;
            }
        }

        outColor = ColorFromRGBA(components[0], components[1], components[2], components[3]);
        return true;
    }
    public static float RandRollIncremented(float minRoll, float maxRollInclusive, float minIncrement)
    {
        return Mathf.Round(Random.Range((int)(minRoll / minIncrement), (int)(maxRollInclusive / minIncrement))) * minIncrement;
    }
}
