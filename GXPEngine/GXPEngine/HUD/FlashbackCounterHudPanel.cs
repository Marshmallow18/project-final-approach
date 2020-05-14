using System.Drawing;
using System.Runtime.InteropServices;

namespace GXPEngine.HUD
{
    public class FlashbackCounterHudPanel : EasyDraw
    {
        private Sprite _bgSprite;

        private Color _bgClearColor  = Color.Black;
        private string _textVal;
        
        public FlashbackCounterHudPanel() : base(300, 40, false)
        {
            Clear(_bgClearColor);
            TextAlign(CenterMode.Center, CenterMode.Center);
            TextFont(Settings.Textbox_Font, 24);
            Text("0 of 10 flashbacks", width/2, height/2);
        }


        public string TextVal
        {
            get => _textVal;
            set
            {
                _textVal = value; 
                Clear(_bgClearColor);
                Text(value, width/2, height/2);
            }
        }
    }
}