namespace GXPEngine.HUD
{
    public class HudPanel : Sprite
    {
        public HudPanel(string filename, bool keepInCache = false, bool addCollider = true) : base(filename,
            keepInCache, addCollider)
        {
        }

        public void SetHudXY(float pX, float pY)
        {
            float camHalfW = GameHud.Cam.Width / 2f;
            float camHalfH = GameHud.Cam.Height / 2f;
            SetXY(pX - camHalfW, pY - camHalfH);
        }
    }
}