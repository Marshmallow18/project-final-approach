using GXPEngine;

namespace MathfExtensions
{
    public static class MathfExtensions
    {
        public static float DegToRad(this float degree)
        {
            return degree * Mathf.PI / 180f;
        }
        
        public static float RadToDegree(this float radians)
        {
            return radians * 180 / Mathf.PI;
        }
    }
}