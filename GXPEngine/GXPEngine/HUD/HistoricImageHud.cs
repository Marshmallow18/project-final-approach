using System.Collections;
using System.Drawing;
using GXPEngine.Components;

namespace GXPEngine.HUD
{
    public class FlashBackHud : HudPanel
    {
        private Sprite _mainImage;

        public FlashBackHud(string flashImage) : base("data/White Texture.png", true, false)
        {
            var bg = new EasyDraw(game.width, game.height, false);
            bg.Clear(Color.FromArgb(245, Color.Black));
            AddChild(bg);

            _mainImage = new Sprite(flashImage, true, false);
            AddChild(_mainImage);
            DrawableTweener.TweenSpriteAlpha(_mainImage, 0, 1, 400, Easing.Equation.CubicEaseOut);
            
            var pressToContinueText = new TextBox("Press Esc to continue", game.width, 30, 0xffffff, 0x00010101,
                CenterMode.Center, CenterMode.Center);
            AddChild(pressToContinueText);
            pressToContinueText.SetXY(0, game.height - 30);

            BlinkOut(pressToContinueText);
        }

        void BlinkIn(Sprite s)
        {
            DrawableTweener.TweenSpriteAlpha(s, 0, 1, 400, Easing.Equation.QuadEaseIn, 0, () => { BlinkOut(s); });
        }

        void BlinkOut(Sprite s)
        {
            DrawableTweener.TweenSpriteAlpha(s, 1, 0, 400, Easing.Equation.QuadEaseIn, 0, () => { BlinkIn(s); });
        }
    }
}