using System.Drawing;

namespace GXPEngine.HUD
{
    public abstract class FlashBackHud : EasyDraw
    {
        public FlashBackHud(string flashBackName, int pWidth, int pHeight) : base(pWidth, pHeight, false)
        {
            Clear(Color.Black);
        }
    }
}