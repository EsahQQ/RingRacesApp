using OpenTK;

namespace RingRaceLab
{
    public class SlowDownPrize : IPrize
    {
        public Vector2 Position { get; set; }
        public int TextureId { get; }

        public SlowDownPrize(Vector2 position)
        {
            Position = position;
            TextureId = TextureLoader.LoadFromFile("sprites/slow_prize.png");
        }

        public void ApplyEffect(Car car)
        {
            car.AddDecorator(new SlowDownDecorator(car, 0.5f, 5f));
        }
    }
}