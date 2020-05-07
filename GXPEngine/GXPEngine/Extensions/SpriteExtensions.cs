namespace GXPEngine.Extensions
{
    public static class SpriteExtensions
    {
        public static void CentralizeOrigin(this Sprite sprite)
        {
            sprite.SetOrigin(sprite.width / 2, sprite.height / 2);
        }
    }
}