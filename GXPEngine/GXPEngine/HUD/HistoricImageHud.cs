using System.Collections;
using System.Drawing;
using GXPEngine.Components;

namespace GXPEngine.HUD
{
    public class HistoricImageHud : HudPanel
    {
        private Sprite _mainImage;

        public HistoricImageHud(string historyFileName) : base("data/White Texture.png", true, false)
        {
            var bg = new EasyDraw(game.width, game.height, false);
            bg.Clear(Color.FromArgb(0, Color.Black));
            AddChild(bg);

            _mainImage = new Sprite(historyFileName, true, false);
            AddChild(_mainImage);

            _mainImage.scale = 0.5f;
            _mainImage.SetOriginToCenter();
            _mainImage.SetXY(MyGame.HALF_SCREEN_WIDTH, MyGame.HALF_SCREEN_HEIGHT);
            
            DrawableTweener.TweenSpriteAlpha(_mainImage, 0, 1, 400, Easing.Equation.CubicEaseOut);
            
            var textBg = new EasyDraw(game.width, 30, false);
            textBg.Clear(Color.Black);
            AddChild(textBg);
            textBg.SetXY(0, game.height - 30);
            
            var pressToContinueText = new TextBox("Press Esc to continue", game.width, 30, 0xffffff, 0x00010101,
                CenterMode.Center, CenterMode.Center);
            AddChild(pressToContinueText);
            pressToContinueText.SetXY(0, game.height - 30);

            DrawableTweener.Blink(pressToContinueText, 1, 0, 400);
        }
    }
}