using System.Collections;
using GXPEngine.Components;

namespace GXPEngine.HUD
{
    public class GameHud : GameObject
    {
        public static GameHud Instance;

        private BaseLevel _level;
        private MCamera _cam;

        private MemoriesHudPanel _memoriesHudPanel;

        public GameHud(BaseLevel pLevel, MCamera pCam) : base(false)
        {
            Instance = this;

            _level = pLevel;
            _cam = pCam;
            Cam = _cam;

            _cam.AddChild(this);
            SetXY(-_cam.Width / 2f, -_cam.Height / 2f);

            _memoriesHudPanel = new MemoriesHudPanel();
            AddChild(_memoriesHudPanel);
            _memoriesHudPanel.SetXY(70, 36);

            CoroutineManager.StartCoroutine(Start(), this);
        }

        private IEnumerator Start()
        {
            yield return new WaitForMilliSeconds(2000);
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
            DrawableTweener.TweenSpriteAlpha(flashHud, 0, alphaValue, 600, Easing.Equation.QuadEaseOut, 0, () =>
            {
                //Disable player after fade in
                _level.Player.InputEnabled = false;
            });
            
            yield return new WaitForMilliSeconds(600);
            
            //Hold execution until ESCAPE pressed
            while (!Input.GetKeyDown(Key.ESCAPE))
            {
                yield return null;
            }

            //Fadeout Panel
            DrawableTweener.TweenSpriteAlpha(flashHud, alphaValue, 0, 600, Easing.Equation.QuadEaseOut, 0, () =>
            {
                flashHud.Destroy();
            });
            
            yield return new WaitForMilliSeconds(800);
            
            _memoriesHudPanel.EnableIndicator(0);

            _level.Player.InputEnabled = true;
        }

        public static MCamera Cam { get; set; }
    }
}