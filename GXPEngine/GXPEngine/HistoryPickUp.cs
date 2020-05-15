using System;
using GXPEngine.HUD;

namespace GXPEngine
{
    /// <summary>
    /// When player goes over it, OnEnterTrigger is excuted, when player goes out OnExitTrigger executed
    /// </summary>
    public class HistoryPickUp : AnimationSprite, IHasTrigger
    {
        private TriggerBehavior _trigger;
        private string _historyImageFileName = "";

        public HistoryPickUp(string pHistoryImageFileName, string filename, int cols, int rows, int frames = -1, bool keepInCache = false,
            bool addCollider = true) : base(filename, cols, rows, frames, keepInCache, addCollider)
        {
            _historyImageFileName = pHistoryImageFileName;
            _trigger = new TriggerBehavior(this);
            
            DrawableTweener.Blink(this, 1, 0.5f, 600);
        }

        void OnCollision(GameObject other)
        {
            if (other is Player)
                _trigger.OnTrigger(other);
        }

        void Update()
        {
            _trigger.HitTest();
        }

        void IHasTrigger.OnEnterTrigger(GameObject other)
        {
            Console.WriteLine($"{this}: OnEnterTrigger -> {other}");

            GameSoundManager.Instance.PlayFx(Settings.History_Pickedup_SFX, Settings.History_Pickedup_SFX_Volume);
            
            DrawableTweener.TweenSpriteAlpha(this, 1, 0, 200, Easing.Equation.QuadEaseOut, 0, () =>
            {
                GameHud.Instance.ShowHistoricHud(_historyImageFileName);
                this.Destroy();
            });
        }

        void IHasTrigger.OnExitTrigger(GameObject other)
        {
            Console.WriteLine($"{this}: OnExitTrigger -> {other}");
        }

        GameObject IHasTrigger.gameObject => this;
    }

    public interface IHasTrigger
    {
        void OnEnterTrigger(GameObject other);
        void OnExitTrigger(GameObject other);
        GameObject gameObject { get; }
    }
}