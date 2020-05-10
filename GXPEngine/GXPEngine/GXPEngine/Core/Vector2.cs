using System;

namespace GXPEngine.Core
{
	public partial struct Vector2
	{
		public float x;
		public float y;
		
		public Vector2 (float x, float y)
		{
			this.x = x;
			this.y = y;
		}
		
		override public string ToString() {
			return $"[Vector2 {x:0.00} | {y:0.00}]";
		}
	}
}

