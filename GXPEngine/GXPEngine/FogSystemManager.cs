using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using GXPEngine.Components;
using GXPEngine.Core;
using Rectangle = System.Drawing.Rectangle;

namespace GXPEngine
{
    /// <summary>
    /// NOT USED, will be deleted
    /// </summary>
    //[Obsolete("Not used, only testing", true)]
    public class FogSystemManager : GameObject
    {
        private Image _lightAlpha;
        private Sprite _lightSprite;
        private EasyDraw _bg;

        private MCamera _cam;
        private GameObject _mainTarget;

        private Pen _pen;
        private Brush[] _brushes;
        private HatchStyle _hatchStyle;

        private HatchStyle[] _hatchStyles;

        public int hatchIndex;

        [Obsolete("Only for test", true)]
        public FogSystemManager(MCamera pCam, GameObject pTarget) : base(false)
        {
            _lightAlpha = new Bitmap("data/player_light_grid_map.png");
            _lightSprite = new Sprite("data/player_light_grid_map.png", false, false);

            _brushes = new Brush[4];
            SetBrushes(HatchStyle.Cross);

            _bg = new EasyDraw(game.width, game.height);
            pCam.AddChild(_bg);
            _bg.SetOriginToCenter();
            _bg.Clear(Color.FromArgb(100, Color.Black));

            _cam = pCam;
            _mainTarget = pTarget;

            string[] hatchesStrs = new string[51];
            for (int i = 0; i < 51; i++)
            {
                var hatchStyle = (HatchStyle) i;
                hatchesStrs[i] = hatchStyle.ToString();
            }

            hatchesStrs = hatchesStrs.OrderBy(h => h).ToArray();

            _hatchStyles = new HatchStyle[51];
            for (int i = 0; i < _hatchStyles.Length; i++)
            {
                var hatchStyle = Enum.Parse(typeof(HatchStyle), hatchesStrs[i]);
                _hatchStyles[i] = (HatchStyle) hatchStyle;
            }

            Console.WriteLine($"{this}: {string.Join("\r\n", _hatchStyles)}");
        }

        void Update()
        {
            //_bg.graphics.CompositingMode = CompositingMode.SourceOver;
            _bg.Clear(Color.FromArgb(255, Color.Black));

            var localPos = _bg.InverseTransformPoint(_mainTarget.x, _mainTarget.y);

            float localX = localPos.x + MyGame.HALF_SCREEN_WIDTH;
            float localY = localPos.y + MyGame.HALF_SCREEN_HEIGHT;

            _bg.graphics.CompositingMode = CompositingMode.SourceCopy;
            _lightSprite.SetXY(localX, localY);
            _bg.DrawSprite(_lightSprite);
            
            DrawLight(Input.mouseX, Input.mouseY);
            //DrawLight(localX, localY);

            localPos = _bg.InverseTransformPoint(2792, 3995);
            DrawLight(localPos.x + MyGame.HALF_SCREEN_WIDTH, localPos.y + MyGame.HALF_SCREEN_HEIGHT);

            //_bg.graphics.DrawImage(_lightAlpha, Input.mouseX, Input.mouseY);

            if (Input.GetKeyDown(Key.V))
            {
                hatchIndex = GeneralTools.GetCircularArrayIndex(hatchIndex - 1, _hatchStyles.Length);
                SetBrushes(_hatchStyles[hatchIndex]);
                Console.WriteLine(_hatchStyles[hatchIndex]);
            }
            else if (Input.GetKeyDown(Key.B))
            {
                hatchIndex = GeneralTools.GetCircularArrayIndex(hatchIndex + 1, _hatchStyles.Length);
                SetBrushes(_hatchStyles[hatchIndex]);
                Console.WriteLine(_hatchStyles[hatchIndex]);
            }
        }

        void DrawLight(float pX, float pY)
        {
            int size = 512;
            int pSize = size;
            float posX = pX - size / 2;
            float posY = pY - size / 2;


            var lastComposition = _bg.graphics.CompositingMode;
            _bg.graphics.CompositingMode = CompositingMode.SourceCopy;

            for (int i = 3; i < 4; i++)
            {
                pSize = size - 60 * (i + 1);
                posX = pX - pSize / 2;
                posY = pY - pSize / 2;

                _bg.graphics.FillEllipse(_brushes[i], posX, posY, pSize, pSize);
            }

            _bg.graphics.CompositingMode = lastComposition;
        }

        void SetBrushes(HatchStyle hatchStyle)
        {
            var c = Color.Black;

            //_brushes[0] = new HatchBrush(hatchStyle, Color.FromArgb(200, c));
            //_brushes[1] = new HatchBrush(hatchStyle, Color.FromArgb(100, c));
            //_brushes[2] = new HatchBrush(hatchStyle, Color.FromArgb(50, c));
            _brushes[3] = new SolidBrush(Color.FromArgb(1, c));
        }
    }
}