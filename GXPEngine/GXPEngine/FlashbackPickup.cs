using System;
using GXPEngine.HUD;
using TiledMapParserExtended;

namespace GXPEngine
{
    public class FlashbackPickup : AnimationSprite, IHasTrigger
    {
        private TriggerBehavior _trigger;
        private TiledObject _flashbackData;

        public FlashbackPickup(string filename, TiledObject pFlashbackData, int cols, int rows, int frames = -1,
            bool keepInCache = false, bool addCollider = true) : base(filename, cols, rows, frames, keepInCache,
            addCollider)
        {
            _flashbackData = pFlashbackData;
            _trigger = new TriggerBehavior(this);
        }

        void OnCollision(GameObject other)
        {
            if (!Enabled)
                return;

            if (other is Player)
                _trigger.OnTrigger(other);
        }

        void Update()
        {
            _trigger.HitTest();
        }

        void IHasTrigger.OnEnterTrigger(GameObject other)
        {
            FlashbackManager.Instance.PlayerPickedupFlashblack(this, true);
            Console.WriteLine($"{this}: OnEnterTrigger -> {other}");
        }

        void IHasTrigger.OnExitTrigger(GameObject other)
        {
            Console.WriteLine($"{this}: OnExitTrigger -> {other}");
        }

        public void Blink()
        {
            DrawableTweener.Blink(this, 1, 0.5f, Settings.Default_AlphaTween_Duration);
        }

        GameObject IHasTrigger.gameObject => this;

        public TiledObject FlashbackData => _flashbackData;

        public TriggerBehavior Trigger => _trigger;
    }
}