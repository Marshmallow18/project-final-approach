using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Security.Policy;

namespace GXPEngine.HUD.FlashBack_Huds
{
    public class FlashBackHud01 : FlashBackHud
    {
        private string[] _imagesFiles = new string[0];
        private string[] _texts = new string[0];

        private Sprite[] _sprites;
        private TextBox _textBox;
        private int _spriteIndex;

        private BaseLevel _level;

        public bool toDestroy;

        //Accelerate tweens to make it faster
        private int _alphaTweenDuration = 1;
        private int _textSpeed = 1;

        private bool _allowSkipByKey = true;
        
        /// <summary>
        /// Loaded in Game Hud, all logic run as a sequence/routine in Start()
        /// </summary>
        /// <param name="flashBackName"></param>
        /// <param name="pWidth"></param>
        /// <param name="pHeight"></param>
        public FlashBackHud01(string flashBackName, int pWidth, int pHeight, bool speedUp, bool pAllowSkipByKey = true) : base(flashBackName,
            pWidth, pHeight)
        {
            alpha = 0;

            //Used in Gamehud to check if this flashback can be skipped by pressing ESCAPE
            _allowSkipByKey = pAllowSkipByKey;

            _alphaTweenDuration =
                (speedUp) ? Settings.Default_AlphaTween_Duration / 6 : Settings.Default_AlphaTween_Duration;
            _textSpeed = (speedUp) ? Settings.Flashbacks_TextBoxTweenSpeed * 10 : Settings.Flashbacks_TextBoxTweenSpeed;

            CoroutineManager.StartCoroutine(Start(), this);
        }

        /// <summary>
        /// Sequence of events:
        /// -Disable Players Input
        /// -Load Image Sprites
        /// -Add First Sprite/Textbox
        /// -Navigate through all texts of the first sprite (when player presses AnyKey)
        /// -Go to next image (loop - when player presses AnyKey)
        /// -Enables Player input
        /// -Destroy itself
        /// </summary>
        /// <returns></returns>
        private IEnumerator Start()
        {
            //Wait for images and text, this is not supposed to happen but...
            while (_imagesFiles.Length == 0 || _texts.Length == 0 || _level == null)
            {
                yield return null;
            }

            //Check for design error (images needs texts)
            if (_imagesFiles.Length != _texts.Length)
            {
                Console.WriteLine($"{this}: Images count differs from text count, check tmx objects properties");
                yield break;
            }

            _level.Player.InputEnabled = false;

            yield return new WaitForMilliSeconds(_alphaTweenDuration);

            //Create Sprites with the images
            _sprites = new Sprite[_imagesFiles.Length];
            for (int i = 0; i < _imagesFiles.Length; i++)
            {
                string fileName = _imagesFiles[i];
                if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName)))
                {
                    Console.WriteLine($"{this}: File '{_imagesFiles[i]}' not exists");
                    fileName = "data/No Image.png";
                }

                var s = new Sprite(fileName, true, false);
                s.alpha = 0;
                _sprites[i] = s;
            }

            //Load FirstImage
            AddChild(_sprites[0]);

            //Load textbox
            _textBox = new TextBox("", game.width - 120, 90);
            AddChild(_textBox);

            //Fade in panel and its children
            DrawableTweener.TweenSpriteAlpha(this, 0, 1, _alphaTweenDuration, Easing.Equation.QuadEaseOut);

            for (int i = 0; i < _sprites.Length; i++)
            {
                //Load first texts, split by NewLine
                string[] texts = _texts[i].Split(new string[] {Environment.NewLine},
                    StringSplitOptions.RemoveEmptyEntries);

                _textBox.Text = texts.Length == 0 ? "" : texts[0];
                _textBox.SetXY(120 / 2f, game.height - _textBox.Height - 30);

                //Tween text, can be skipped by AnyKey
                yield return _textBox.TweenTextRoutine(0, _textSpeed);

                yield return null;

                //Wait AnyKey to go to the next text
                while (!Input.GetAnyKeyDown(Key.ESCAPE))
                {
                    yield return null;
                }

                yield return null;

                //Loop through next texts
                for (int t = 1; t < texts.Length; t++)
                {
                    _textBox.Text = texts[t];
                    _textBox.SetXY(120 / 2f, game.height - _textBox.Height - 30);
                    yield return _textBox.TweenTextRoutine(0, _textSpeed);

                    yield return null;

                    //Wait AnyKey to go to the next text, if it is the last ont, goes to next image
                    while (!Input.GetAnyKeyDown(Key.ESCAPE))
                    {
                        yield return null;
                    }

                    yield return null;
                }

                //Loop to next Image/Texts
                if ((i + 1) < _sprites.Length)
                {
                    //Fadeout current image
                    var currentIndex = i;
                    DrawableTweener.TweenSpriteAlpha(_sprites[i], 1, 0, _alphaTweenDuration,
                        Easing.Equation.QuadEaseOut, 0,
                        () => { _sprites[currentIndex].Destroy(); });


                    //Fadein next images
                    AddChildAt(_sprites[i + 1], _textBox.Index);
                    DrawableTweener.TweenSpriteAlpha(_sprites[i + 1], 0, 1, _alphaTweenDuration,
                        Easing.Equation.QuadEaseOut);

                    yield return new WaitForMilliSeconds(_alphaTweenDuration);
                }
            }

            //Fadeout panel, destroy itself, enables Player input
            DrawableTweener.TweenSpriteAlpha(this, 1, 0, MyGame.AlphaTweenDuration, Easing.Equation.QuadEaseOut, 0,
                () =>
                {
                    HierarchyManager.Instance.LateDestroy(this);
                    toDestroy = true;
                    _level.Player.InputEnabled = true;
                });
        }

        public string[] ImagesFiles
        {
            get => _imagesFiles;
            set => _imagesFiles = value;
        }

        public string[] Texts
        {
            get => _texts;
            set => _texts = value;
        }

        public BaseLevel Level
        {
            get => _level;
            set => _level = value;
        }

        public bool AllowSkipByKey => _allowSkipByKey;
    }
}