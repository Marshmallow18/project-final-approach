using System.Drawing;

namespace GXPEngine.Screens
{
    public class StartScreen : Sprite
    {
        private EasyDraw _fader;

        private bool _lockStart = true;
        
        public StartScreen() : base(Settings.StartScreen_Bg_Image, false, false)
        {
           GameSoundManager.Instance.PlayMusic(Settings.StartScreen_Music);
           
           _fader = new EasyDraw(game.width, game.height, false);
           _fader.Clear(Color.Black);
           AddChild(_fader);

           DrawableTweener.TweenSpriteAlpha(_fader, 1, 0, Settings.Default_AlphaTween_Duration, () =>
               {
                   _lockStart = false;
               });
        }

        void Update()
        {
            if (!_lockStart && Input.GetKeyDown(Key.ENTER))
            {
                StartGame();
            }
        }
        
        public void StartGame()
        {
            GameSoundManager.Instance.FadeOutCurrentMusic(Settings.Default_AlphaTween_Duration);
            
            DrawableTweener.TweenSpriteAlpha(_fader, 0, 1, Settings.Default_AlphaTween_Duration,() =>
            {
                ((MyGame)game).LoadLevel();
                this.Destroy();
            });
        }
    }
}