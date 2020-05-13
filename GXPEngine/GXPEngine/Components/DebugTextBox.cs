using System;
using System.Drawing;

namespace GXPEngine.Components
{
    public class DebugTextBox : EasyDraw
    {
        private string _textValue;
        private Color _bgColor;
        private Color _color;

        private CenterMode _horAlign;
        private CenterMode _verAlign;

        private float _textSize;
        
        public DebugTextBox(string pText, int pWidth, int pHeight, uint textColor = 0xffffffff, uint bgColor = 0xff000000, CenterMode hor = CenterMode.Min, CenterMode ver = CenterMode.Center, bool addCollider = false) : base(pWidth, pHeight, addCollider)
        {
            _bgColor = Color.FromArgb((int)bgColor);
            _color = Color.FromArgb((int)textColor);
            _textValue = pText;

            _horAlign = hor;
            _verAlign = ver;
        }

        void Update()
        {
            if (!Enabled)
                return;

            Clear(_bgColor);
            Fill(_color);
            Stroke(_color);
            TextAlign(_horAlign, _verAlign);
            _textSize = TextWidth(_textValue);
            Text(_textValue, _horAlign == CenterMode.Center ? width / 2f: 0, height / 2f);
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