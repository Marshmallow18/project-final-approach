using System.Drawing;

namespace GXPEngine
{
    public class CircleGameObject : EasyDraw
    {
        public CircleGameObject(int pWidth, int pHeight, uint pColor = 0xffffffff) : base(pWidth, pHeight, false)
        {
            SetOrigin(pWidth / 2, pHeight / 2);
            Clear(Color.Transparent);
            this.NoFill();
            this.Stroke(Color.FromArgb((int) pColor));
            this.StrokeWeight(1);
            Ellipse(pWidth / 2, pHeight / 2, pWidth - 2, pHeight - 2);
        }
    }
}