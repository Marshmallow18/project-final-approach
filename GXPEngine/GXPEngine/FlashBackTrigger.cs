using System;
using System.Collections;
using GXPEngine.HUD;
using GXPEngine.HUD.FlashBack_Huds;
using TiledMapParserExtended;

namespace GXPEngine
{
    public class FlashBackTrigger : Sprite, IHasTrigger
    {
        private TiledObject _flashbackData;
        private TriggerBehavior _trigger;

        public FlashBackTrigger(string filename, TiledObject pFlashbackData) : base(filename, false, true)
        {
            _trigger = new TriggerBehavior(this);
            _flashbackData = pFlashbackData;
        }

        void OnCollision(GameObject other)
        {
            if (other is Player)
                _trigger.OnTrigger(other);
        }

        void Update()
        {
            visible = MyGame.Debug;
            _trigger.HitTest();
        }

        void IHasTrigger.OnEnterTrigger(GameObject other)
        {
            FlashbackManager.Instance.PlayerPickedupFlashblackTrigger(_flashbackData);

            Console.WriteLine($"{this}: OnEnterTrigger => {other}");
        }

        void IHasTrigger.OnExitTrigger(GameObject other)
        {
            Console.WriteLine($"{this}: OnExitTrigger => {other}");
        }

        GameObject IHasTrigger.gameObject => this;

        public TiledObject FlashbackTriggerData => _flashbackData;
    }
}