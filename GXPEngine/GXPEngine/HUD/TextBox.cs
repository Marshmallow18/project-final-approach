using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using GXPEngine.Core;
using GXPEngine.OpenGL;

namespace GXPEngine.HUD
{
    public class TextBox : EasyDraw
    {
        private Color _bgColor;
        private Color _textColor;
        private Color _borderColor;
        private float _borderWeight = 1f;

        private bool _autoWrap = true;
        private bool _autoHeight = true;

        private string _text;
        private string _textToShow;
        private float _paddingX = 8;
        private float _paddingY = 8;

        private EasyDraw _easy;

        private bool _showPressAnyKeyToNext = true;
        private EasyDraw _pressAnyKeyMsg;

        public delegate void OnFinished();

        public TextBox(string pText, int pWidth, int pHeight, uint pTextColor = 0xffffffff, uint pBgColor = 0xff050505,
            uint pBorderColor = 0xffffffff, float pBorderWeight = 1, bool addCollider = false) : base(pWidth, pHeight,
            addCollider)
        {
            _bgColor = Color.FromArgb((int) pBgColor);
            _textColor = Color.FromArgb((int) pTextColor);
            _borderColor = Color.FromArgb((int) pBorderColor);
            _borderWeight = pBorderWeight;

            _easy = new EasyDraw(pWidth, pHeight);
            AddChild(_easy);

            _easy.TextFont(Settings.Textbox_Font, 12);

            this.Text = pText;
        }

        void Update()
        {
            _easy.Clear(Color.Transparent);
            _easy.Fill(_bgColor);
            _easy.Stroke(_borderColor);
            _easy.StrokeWeight(_borderWeight);
            _easy.ShapeAlign(CenterMode.Min, CenterMode.Min);
            _easy.Rect(0, 0, _easy.width - _borderWeight, _easy.height - _borderWeight);

            _easy.Fill(_textColor);
            _easy.TextAlign(CenterMode.Min, CenterMode.Min);
            _easy.Text(_textToShow, _paddingX, _paddingY);

            if (_pressAnyKeyMsg != null)
                _pressAnyKeyMsg.visible = _showPressAnyKeyToNext;
        }

        string WrappedText(out int numOfLines)
        {
            float w = _easy.TextWidth(_text);
            numOfLines = Mathf.Ceiling(w / (_easy.width - 2 * _paddingX));

            string[] words = _text.Split(' ');
            List<string> lines = new List<string>();

            string line = "";

            for (int i = 0; i < words.Length; i++)
            {
                string tempLine = line + words[i];
                w = _easy.TextWidth(tempLine);

                if (w > (_easy.width - 2 * _paddingX))
                {
                    lines.Add(line.Trim());
                    line = words[i] + " ";
                }
                else
                {
                    line += words[i] + " ";
                }
            }

            if (!string.IsNullOrWhiteSpace(line))
                lines.Add(line);

            return string.Join(Environment.NewLine, lines);
        }

        public void CentralizeHorizontal()
        {
            this.x = game.width / 2f - this.Width / 2f;
        }

        public void CenterOnBottom()
        {
            CentralizeHorizontal();
            this.y = game.height - this.Height - Settings.Textbox_Margin_Bottom;
        }

        public bool AutoHeight
        {
            get => _autoHeight;
            set => _autoHeight = value;
        }

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                _textToShow = "";

                _text = (_autoWrap) ? WrappedText(out var numOfLines) : _text;

                if (_autoHeight)
                {
                    ResizeHeight();
                }
            }
        }

        public void ResizeHeight()
        {
            int h = Mathf.Ceiling(_easy.TextHeight(_text) + _paddingY * 2 + 22);

            if (h > _easy.height)
            {
                int lastWidth = _easy.width;
                _easy.Destroy();
                _easy = null;
                _easy = new EasyDraw(lastWidth, h);
                _easy.TextFont(Settings.Textbox_Font, 12);
                AddChild(_easy);
                LoadPressAnyKeyMsg();
            }
        }

        public IEnumerator TweenTextRoutine(int duration, int speed, bool waitAnyKetToClose = false)
        {
            int time = 0;

            string text = _text;
            int len = text.Length;

            _text = "";

            if (speed > 0)
            {
                duration = Mathf.Ceiling((float) len / speed) * 1000;
            }

            while (time < duration && !Input.GetAnyKeyDown())
            {
                float easing = Easing.Ease(Easing.Equation.Linear, time, 0, 1, duration);
                int mapIndex = Mathf.Round(Mathf.Map(easing, 0, 1, 0, len - 1));

                //skip spaces
                if (speed > 0 && text[mapIndex] == ' ')
                {
                    duration -= speed;
                    continue;
                }

                _text = text.Substring(0, mapIndex + 1);
                _textToShow = _text;

                yield return null;
                time += Time.deltaTime;
            }

            _text = text;
            _textToShow = _text;

            yield return null;

            while (waitAnyKetToClose && !Input.GetAnyKeyDown())
            {
                yield return null;
            }
        }

        void LoadPressAnyKeyMsg()
        {
            _pressAnyKeyMsg = new EasyDraw(150, 22);
            _pressAnyKeyMsg.Clear(Color.Transparent);
            AddChild(_pressAnyKeyMsg);
            _pressAnyKeyMsg.TextAlign(CenterMode.Min, CenterMode.Center);
            _pressAnyKeyMsg.TextSize(10);
            _pressAnyKeyMsg.Text("Press any key to next", 0, 15);
            _pressAnyKeyMsg.SetXY(Width - _pressAnyKeyMsg.width, Height - _pressAnyKeyMsg.height - _paddingY);
        }

        public int Height => _easy.height;
        public float Width => _easy.width;

        public bool ShowPressAnyKeyToNext
        {
            get => _showPressAnyKeyToNext;
            set => _showPressAnyKeyToNext = value;
        }
    }
}