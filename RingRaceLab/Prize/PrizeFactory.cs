using OpenTK;

namespace RingRaceLab
{
    public abstract class PrizeFactory
    {
        public abstract IPrize CreatePrize(Vector2 position);
    }

    public class FuelPrizeFactory : PrizeFactory
    {
        public override IPrize CreatePrize(Vector2 position) => new FuelPrize(position);
    }

    public class SpeedBoostPrizeFactory : PrizeFactory
    {
        public override IPrize CreatePrize(Vector2 position) => new SpeedBoostPrize(position);
    }
    public class SlowDownPrizeFactory : PrizeFactory
    {
        public override IPrize CreatePrize(Vector2 position) => new SlowDownPrize(position);
    }
}