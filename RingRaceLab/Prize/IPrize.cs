using OpenTK;

namespace RingRaceLab
{
    public interface IPrize
    {
        Vector2 Position { get; set; }
        int TextureId { get; }
        void ApplyEffect(Car car);
    }
}