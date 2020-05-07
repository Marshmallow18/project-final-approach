using System;

namespace GXPEngine
{
    public static partial class Mathf
    {
        public const float E = (float)Math.E;
        
        public static float Map(float value, float start1, float stop1, float start2, float stop2)
        {
            float outgoing =
                start2 + (stop2 - start2) * ((value - start1) / (stop1 - start1));

            return outgoing;
        }
        
        /// <summary>
        ///   <para>Compares two floating point values if they are similar. Inspired in Unity code</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static bool Approximately(float a, float b)
        {
            return Mathf.Abs(b - a) < (float) Mathf.Max(1E-06f * Mathf.Max(Mathf.Abs(a), Mathf.Abs(b)), Mathf.E * 8f);
        }
        
        public static bool AlmostEquals(float float1, float float2, float precision)
        {
            return (Mathf.Abs(float1 - float2) <= precision);
        }


    }
}