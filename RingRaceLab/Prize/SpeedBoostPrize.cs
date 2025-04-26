using OpenTK;

namespace RingRaceLab
{
    public class SpeedBoostPrize : IPrize
    {
        public Vector2 Position { get; set; }
        public int TextureId { get; }

        public SpeedBoostPrize(Vector2 position)
        {
            Position = position;
            TextureId = TextureLoader.LoadFromFile("sprites/speed_prize.png");
        }

        public void ApplyEffect(Car car)
        {
            car.AddDecorator(new SpeedBoostDecorator(car, 1.5f, 5f));
        }
    }
}