using System.Drawing;

namespace GXPEngine.Core
{
    public partial struct Vector2
    {
        public static readonly Vector2 zero = new Vector2(0, 0);
        public static readonly Vector2 right = new Vector2(1, 0);
        public static readonly Vector2 up = new Vector2(0, 1);
        public static readonly Vector2 one = new Vector2(1, 1);

        public Point ToPoint()
        {
            return new Point(Mathf.Round(x), Mathf.Round(y));
        }

        public static Vector2 operator *(Vector2 v0, float scalar)
        {
            return new Vector2(v0.x * scalar, v0.y * scalar);
        }

        public static Vector2 operator +(Vector2 v0, Vector2 v1)
        {
            return new Vector2(v0.x + v1.x, v0.y + v1.y);
        }

        public static Vector2 operator -(Vector2 v0, Vector2 v1)
        {
            return new Vector2(v0.x - v1.x, v0.y - v1.y);
        }

        public float Magnitude => Mathf.Sqrt(this.x * this.x + this.y * this.y);

        public Vector2 Normalized
        {
            get
            {
                var mag = this.Magnitude;
                return new Vector2(this.x / mag, this.y / mag);
            }
        }

        public static float Dot(Vector2 v0, Vector2 v1)
        {
            return v0.x * v1.x + v0.y * v1.y;
        }

        public static Vector2 Projection(Vector2 v0, Vector2 v1)
        {
//            float dot = Vector2.Dot(v0, v1);
//            float v1Mag = v1.Magnitude;
//            float scalarProjection = dot / v1Mag;
            float dotV0V1 = Vector2.Dot(v0, v1);
            float dotV1V1 = Vector2.Dot(v1, v1);

            return v1 * (dotV0V1 / dotV1V1);
        }

        public static float Cross(Vector2 v0, Vector2 v1)
        {
            return v0.x * v1.y - v0.y * v1.x;
        }

        public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            t = Mathf.Clamp(t, 0f, 1f);
            return new Vector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
        }
        
        public static float AngleBetween(Vector2 vector1, Vector2 vector2)
        {
            float sin = vector1.x * vector2.y - vector2.x * vector1.y;  
            float cos = vector1.x * vector2.x + vector1.y * vector2.y;

            return Mathf.Atan2(sin, cos);
        }
    }
}