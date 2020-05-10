using System;
using GXPEngine.HUD;

namespace GXPEngine
{
    /// <summary>
    /// When player goes over it, OnEnterTrigger is excuted, when player goes out OnExitTrigger executed
    /// </summary>
    public class MemoryPickUp : AnimationSprite, IHasTrigger
    {
        private TriggerBehavior _trigger;

        public MemoryPickUp(string filename, int cols, int rows, int frames = -1, bool keepInCache = false,
            bool addCollider = true) : base(filename, cols, rows, frames, keepInCache, addCollider)
        {
            _trigger = new TriggerBehavior(this);
        }

        void OnCollision(GameObject other)
        {
            _trigger.OnTrigger(other);
        }

        void Update()
        {
            _trigger.HitTest();
        }

        void IHasTrigger.OnEnterTrigger(GameObject other)
        {
            Console.WriteLine($"{this}: OnEnterTrigger -> {other}");
            
            DrawableTweener.TweenSpriteAlpha(this, 1, 0, 200, Easing.Equation.QuadEaseOut, 0, () =>
            {
                GameHud.Instance.ShowFlashBackHud();
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