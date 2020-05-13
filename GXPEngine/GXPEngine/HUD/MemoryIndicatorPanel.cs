using System;
using System.Collections;
using System.Threading;
using GXPEngine.Core;

namespace GXPEngine.HUD
{
    public class MemoryIndicatorPanel : HudPanel
    {
        private Sprite _onIndicatorSprite;

        public MemoryIndicatorPanel(bool keepInCache = false, bool addCollider = true) : base(
            "data/Hud Off Memory Indicator Panel.png", keepInCache, addCollider)
        {
            _onIndicatorSprite = new Sprite("data/Hud On Memory Indicator Panel.png", false, false);
            AddChild(_onIndicatorSprite);
            _onIndicatorSprite.SetXY(0, 0);
            _onIndicatorSprite.visible = false;

            CoroutineManager.StartCoroutine(Start(), this);

            var mouseHandler = new MMouseHandler(this);
            mouseHandler.OnMouseClick += (target, type) => { Console.WriteLine($"{this}: clicked"); };
            mouseHandler.OnMouseOverTarget += (target, type) => { Console.WriteLine($"{this}: over"); };
            mouseHandler.OnMouseOffTarget += (target, type) => { Console.WriteLine($"{this}: off"); };
        }

        private IEnumerator Start()
        {
            yield break;
        }

        public void EnableIndicator()
        {
            _onIndicatorSprite.visible = true;
            DrawableTweener.TweenSpriteAlpha(_onIndicatorSprite, 0, 1, MyGame.AlphaTweenDuration, Easing.Equation.ElasticEaseOut, 0,
                () => { });
        }

        public void DisableIndicator()
        {
            DrawableTweener.TweenSpriteAlpha(_onIndicatorSprite, 1, 0, MyGame.AlphaTweenDuration, Easing.Equation.QuadEaseOut, 0,
                () => { _onIndicatorSprite.visible = false; });
        }
    }
}