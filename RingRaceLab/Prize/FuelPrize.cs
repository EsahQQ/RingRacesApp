using OpenTK;

namespace RingRaceLab
{
    public class FuelPrize : IPrize
    {
        public Vector2 Position { get; set; }
        public int TextureId { get; }

        public FuelPrize(Vector2 position)
        {
            Position = position;
            TextureId = TextureLoader.LoadFromFile("sprites/fuel_prize.png");
        }

        public void ApplyEffect(Car car)
        {
            car.Fuel += 50;
        }
    }
}