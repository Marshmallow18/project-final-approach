namespace GXPEngine
{
    public interface IHasSpeed
    {
        float Speed { get; }
        float MaxSpeed { get; }
        GameObject gameObject { get; }
    }
}