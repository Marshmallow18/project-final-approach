using System.Drawing;
using GXPEngine.HUD;

namespace GXPEngine.Screens
{
    public class PreGameStartScreen : Sprite
    {
        private EasyDraw _fader;

        private bool _lockKey = true;

        public delegate void OnFinished();

        private OnFinished _onFinished;

        public PreGameStartScreen(string fileName, string musicFilename, OnFinished onFinished = null) : base(fileName,
            false, false)
        {
            if (!string.IsNullOrWhiteSpace(musicFilename))
            {
                GameSoundManager.Instance.FadeOutCurrentMusic();
                GameSoundManager.Instance.PlayMusic(musicFilename);
            }

            _fader = new EasyDraw(game.width, game.height, false);
            _fader.Clear(Color.Black);
            AddChild(_fader);

            _onFinished = onFinished;

            _lockKey = false;
            DrawableTweener.TweenSpriteAlpha(_fader, 1, 0, Settings.Default_AlphaTween_Duration, () =>
            {
                //_lockKey = false;
            });
        }

        void Update()
        {
            if (!_lockKey && Input.GetKeyDown(Key.ENTER))
            {
                _lockKey = true;
                FadeInAndFinish();
            }
        }

        void FadeInAndFinish()
        {
            DrawableTweener.TweenSpriteAlpha(_fader, 0, 1, Settings.Default_AlphaTween_Duration, () =>
            {
                Destroy();
                _onFinished?.Invoke();
            });
        }
    }
}