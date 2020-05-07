using System;
using GXPEngine.Core;

namespace GXPEngine
{
    public static class MRandom
    {
        static Random rand = new Random(Time.now);

        public static float Range(float min, float max)
        {
            return min + (float) rand.NextDouble() * max;
        }

        public static int Range(int min, int max)
        {
            return rand.Next(min, max);
        }

        public static Vector2 InsideUnitCircle()
        {
            float angle = Range(Mathf.PI / 180, Mathf.PI * 2);
            float mag = Range(0f, 1f);
            
            float x = Mathf.Cos(angle) - mag;
            float y = Mathf.Sin(angle) - mag;
            
            return new Vector2(x, y);
        }
        
        public static Vector2 OnUnitCircle()
        {
            float angle = Range(Mathf.PI / 180, Mathf.PI * 2);
            
            float x = Mathf.Cos(angle);
            float y = Mathf.Sin(angle);
            
            return new Vector2(x, y);
        }
    }
}