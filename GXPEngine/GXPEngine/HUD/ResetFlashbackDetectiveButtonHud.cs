using System;
using GXPEngine.Core;

namespace GXPEngine.HUD
{
    public class ResetFlashbackDetectiveButtonHud : Sprite
    {
        private Sprite _fg;
        private Sprite _onHoverSprite;

        private Rectangle _customColliderBounds;

        public ResetFlashbackDetectiveButtonHud() : base("data/Reset Memory Button Hud.png", false, true)
        {
            _onHoverSprite = new Sprite("data/Reset Memory Button Hud OnHover.png", false, false);
            _onHoverSprite.SetOriginToCenter();
            _onHoverSprite.SetActive(false);
            AddChild(_onHoverSprite);

            _customColliderBounds = new Rectangle(-149 * 0.5f, -47 * 0.5f, 149, 47);

            var mouseHandler = new MMouseHandler(this);
            mouseHandler.OnMouseClick += OnMouseClick;
            mouseHandler.OnMouseOverTarget += OnMouseOver;
            mouseHandler.OnMouseOffTarget += OnMouseOff;
        }

        void OnMouseOver(GameObject target, MouseEventType eventType)
        {
            _onHoverSprite.SetActive(true);
            DrawableTweener.TweenSpriteAlpha(_onHoverSprite, _onHoverSprite.alpha, 1,
                Settings.Default_AlphaTween_Duration);
            Console.WriteLine($"{this}: over");
        }

        void OnMouseOff(GameObject target, MouseEventType eventType)
        {
            DrawableTweener.TweenSpriteAlpha(_onHoverSprite, _onHoverSprite.alpha, 0,
                Settings.Default_AlphaTween_Duration);
            Console.WriteLine($"{this}: off");
        }

        void OnMouseClick(GameObject target, MouseEventType eventType)
        {
            DrawableTweener.TweenSpriteAlpha(_onHoverSprite, _onHoverSprite.alpha, 0,
                Settings.Default_AlphaTween_Duration / 3, () =>
                {
                    DrawableTweener.TweenSpriteAlpha(_onHoverSprite, _onHoverSprite.alpha, 1,
                        Settings.Default_AlphaTween_Duration / 3);
                });

            FlashbackManager.Instance.ResetMemorySequence();
            
            Console.WriteLine($"{this}: clicked");
        }

        public override Vector2[] GetExtents()
        {
            Vector2[] ret = new Vector2[4];
            ret[0] = TransformPoint(_customColliderBounds.left, _customColliderBounds.top);
            ret[1] = TransformPoint(_customColliderBounds.right, _customColliderBounds.top);
            ret[2] = TransformPoint(_customColliderBounds.right, _customColliderBounds.bottom);
            ret[3] = TransformPoint(_customColliderBounds.left, _customColliderBounds.bottom);
            return ret;
        }

        public override void SetActive(bool active)
        {
            collider.Enabled = active;
            base.SetActive(active);
        }
    }
}