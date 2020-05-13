using System;
using System.Collections;
using System.Linq;
using GXPEngine.Components;
using GXPEngine.Core;
using GXPEngine.HUD.FlashBack_Huds;
using TiledMapParserExtended;

namespace GXPEngine.HUD
{
    public class GameHud : GameObject
    {
        public static GameHud Instance;

        private BaseLevel _level;
        private MCamera _cam;

        private MemoriesHudPanel _memoriesHudPanel;

        private FlashbackCounterHudPanel _flashbackCounterHudPanel;

        public GameHud(BaseLevel pLevel, MCamera pCam) : base(false)
        {
            Instance = this;

            _level = pLevel;
            _cam = pCam;
            Cam = _cam;

            _cam.AddChild(this);
            SetXY(-_cam.Width / 2f, -_cam.Height / 2f);

            _flashbackCounterHudPanel = new FlashbackCounterHudPanel();
            AddChild(_flashbackCounterHudPanel);
            _flashbackCounterHudPanel.SetXY(70, 36);

            _memoriesHudPanel = new MemoriesHudPanel();
            _memoriesHudPanel.SetXY(70, 36);

            CoroutineManager.StartCoroutine(Start(), this);
        }

        private IEnumerator Start()
        {
            yield return new WaitForMilliSeconds(2000);
        }

        public void SetFlashbackHudCounterText(string text)
        {
            _flashbackCounterHudPanel.TextVal = text;
        }

        public void ShowHistoricHud(string historyFileName)
        {
            CoroutineManager.StartCoroutine(ShowHistoricImageHudRoutine(historyFileName), this);
        }

        IEnumerator ShowHistoricImageHudRoutine(string historyFileName)
        {
            var flashHud = new HistoricImageHud(historyFileName);
            AddChild(flashHud);
            flashHud.SetXY(0, 0);
            float alphaValue = 1f;

            //Fade In panel
            DrawableTweener.TweenSpriteAlpha(flashHud, 0, alphaValue, MyGame.AlphaTweenDuration,
                Easing.Equation.QuadEaseOut, 0, () =>
                {
                    //Disable player after fade in
                    _level.Player.InputEnabled = false;
                });

            yield return new WaitForMilliSeconds(MyGame.AlphaTweenDuration);

            //Hold execution until ESCAPE pressed
            while (!Input.GetKeyDown(Key.ESCAPE))
            {
                yield return null;
            }

            //Fadeout Panel
            DrawableTweener.TweenSpriteAlpha(flashHud, alphaValue, 0, MyGame.AlphaTweenDuration,
                Easing.Equation.QuadEaseOut, 0,
                () => { flashHud.Destroy(); });

            yield return new WaitForMilliSeconds(800);

            _level.Player.InputEnabled = true;
        }

        /// <summary>
        /// Create and Load the flashback panel using data of the tiledObject
        /// </summary>
        /// <param name="tileData"></param>
        public FlashBackHud01 LoadFlashbackHud(TiledObject tileData)
        {
            if (tileData.propertyList?.properties == null)
            {
                Console.WriteLine($"ERROR: {tileData.Name} Flashback without properties in Tiled");
                return null;
            }

            var allProps = tileData.propertyList.properties;

            var imageFiles = allProps.Where(p => p.Name.ToLower().StartsWith("image_")).ToArray();
            var texts = allProps.Where(p => p.Name.ToLower().StartsWith("text_")).ToArray();

            if (imageFiles.Length == 0 || imageFiles.Length != texts.Length)
            {
                Console.WriteLine(
                    $"ERROR+: {this}: Images count differs from text count, check tmx objects properties");
                return null;
            }

            //Texts: replace multiple linebreaks for one linebreak
            var flashHud = new FlashBackHud01(tileData.Name, game.width, game.height)
            {
                ImagesFiles = imageFiles.Select(im => im.Value).ToArray(),
                Texts = texts.Select(t =>
                    t.Type == "string"
                        ? string.Join(System.Environment.NewLine,
                            t.Value.Split(System.Environment.NewLine.ToCharArray(),
                                StringSplitOptions.RemoveEmptyEntries))
                        : t.Value).ToArray(),
                Level = _level
            };

            HierarchyManager.Instance.LateAdd(this, flashHud);

            return flashHud;
        }

        public IEnumerator ShowFlashbackDetectivePanel()
        {
            yield return null;

            DrawableTweener.TweenSpriteAlpha(_flashbackCounterHudPanel, 1, 0, Settings.Default_AlphaTween_Duration,
                () =>
                {
                    _memoriesHudPanel.visible = true;
                    AddChild(_memoriesHudPanel);

                    DrawableTweener.TweenSpriteAlpha(_memoriesHudPanel, 0, 1, Settings.Default_AlphaTween_Duration);

                    DrawableTweener.TweenScale(_memoriesHudPanel, Vector2.one, Vector2.one * 1.1f,
                        Settings.Default_AlphaTween_Duration / 2,
                        () =>
                        {
                            DrawableTweener.TweenScale(_memoriesHudPanel, Vector2.one * 1.1f, Vector2.one * 1,
                                Settings.Default_AlphaTween_Duration / 2,
                                null);
                        });
                });

            yield return new WaitForMilliSeconds(Settings.Default_AlphaTween_Duration * 2);
        }

        public TextBox ShowTextBox(string pText, int pWidth, int pHeight, int duration = 0, int delay = 0,
            bool lockPlayer = false,
            TextBox.OnFinished onFinished = null, bool fade = true)
        {
            var textBox = new TextBox(pText, pWidth, pHeight);

            if (lockPlayer && _level?.Player != null)
            {
                _level.Player.InputEnabled = false;
            }

            if (duration > 0)
                textBox.ShowPressAnyKeyToNext = false;

            textBox.CenterOnBottom();

            CoroutineManager.StartCoroutine(ShowTextBoxRoutine(textBox, duration, delay, onFinished, fade), this);

            return textBox;
        }

        IEnumerator ShowTextBoxRoutine(TextBox textBox, int duration, int delay, TextBox.OnFinished onFinished, bool fade)
        {
            if (delay > 0)
            {
                yield return new WaitForMilliSeconds(delay);
            }

            AddChild(textBox);

            if (fade)
             DrawableTweener.TweenSpriteAlpha(textBox, 0, 1, Settings.Default_AlphaTween_Duration);

            yield return textBox.TweenTextRoutine(0, Settings.Flashbacks_TextBoxTweenSpeed,
                duration == 0 ? true : false);

            int time = 0;
            while (time < (duration + Settings.Default_AlphaTween_Duration))
            {
                yield return null;
                time += Time.deltaTime;
            }

            if (fade)
            {
                DrawableTweener.TweenSpriteAlpha(textBox, 1, 0, Settings.Default_AlphaTween_Duration, () =>
                {
                    HierarchyManager.Instance.LateDestroy(textBox);

                    if (_level?.Player != null)
                        _level.Player.InputEnabled = true;

                    onFinished?.Invoke();
                });
            }
            else
            {
                HierarchyManager.Instance.LateDestroy(textBox);

                if (_level?.Player != null)
                    _level.Player.InputEnabled = true;

                onFinished?.Invoke();
            }
        }

        public static MCamera Cam { get; set; }

        public MemoriesHudPanel MemoriesHudPanel => _memoriesHudPanel;
    }
}