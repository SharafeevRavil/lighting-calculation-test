using UnityEngine;
using static UnityEngine.Random;

namespace Utils
{
    public static class ColorHelper
    {
        public static Color Random() => new(Range(0f, 1f), Range(0f, 1f), Range(0f, 1f));
    }
}