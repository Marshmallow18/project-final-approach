using GXPEngine.Core;

namespace GXPEngine.Components
{
    public struct Vector2Int
    {
        public int x;
        public int y;
        public static readonly Vector2Int up = new Vector2Int(0, 1);
        public static readonly Vector2Int right = new Vector2Int(1, 0);
        public static readonly Vector2Int zero = new Vector2Int(0, 0);
        public static readonly Vector2Int one = new Vector2Int(1, 1);
        
        public Vector2Int(int pX = 0, int pY = 0)
        {
            this.x = pX;
            this.y = pY;
        }

        public static Vector2Int RountToInt(Vector2 v)
        {
            return new Vector2Int(Mathf.Round(v.x), Mathf.Round(v.y));
        }

        public static Vector2Int operator *(Vector2Int v0, int scalar)
        {
            return new Vector2Int(v0.x * scalar, v0.y * scalar);
        }

        public static Vector2Int operator +(Vector2Int v0, Vector2Int v1)
        {
            return new Vector2Int(v0.x + v1.x, v0.y + v1.y);
        }

        public static Vector2Int operator -(Vector2Int v0, Vector2Int v1)
        {
            return new Vector2Int(v0.x - v1.x, v0.y - v1.y);
        }
        
        public override string ToString()
        {
            return $"({x}, {y})";
        }
    }
}