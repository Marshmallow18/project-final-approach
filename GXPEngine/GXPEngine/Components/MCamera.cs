using GXPEngine.Core;

namespace GXPEngine.Components
{
    public class MCamera : Camera
    {
        private int _width;
        private int _height;

        public MCamera(int windowX, int windowY, int windowWidth, int windowHeight) : base(windowX, windowY, windowWidth, windowHeight)
        {
            _width = windowWidth;
            _height = windowHeight;
        }

        public Vector2 WorldPositionToScreenPosition(Vector2 wordPoint)
        {
            return wordPoint - this.Position + new Vector2(this._width/2, this._height / 2);
        }

        public int Width => _width;
        public int Height => _height;
    }
}