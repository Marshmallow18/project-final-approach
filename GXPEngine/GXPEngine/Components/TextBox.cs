using System.Drawing;

namespace GXPEngine.Components
{
    public class TextBox : EasyDraw
    {
        private string _textValue;
        private Color _bgColor;
        private Color _color;

        public TextBox(string pText, int pWidth, int pHeight) : base(pWidth, pHeight, false)
        {
            _bgColor = Color.Black;
            _color = Color.White;
            _textValue = pText;
        }

        void Update()
        {
            Clear(_bgColor);
            Fill(_color);
            Stroke(_color);
            TextAlign(CenterMode.Center, CenterMode.Center);
            Text(_textValue, width / 2, height / 2);
        }


        public Color BgColor
        {
            get => _bgColor;
            set => _bgColor = value;
        }

        public Color Color
        {
            get => _color;
            set => _color = value;
        }

        public string TextValue
        {
            get => _textValue;
            set => _textValue = value;
        }
    }
}