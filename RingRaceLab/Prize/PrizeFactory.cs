using OpenTK;

namespace RingRaceLab
{
    /// <summary>
    /// Абстрактная фабрика для создания различных типов призов.
    /// </summary>
    public abstract class PrizeFactory
    {
        /// <summary>
        /// Создает новый экземпляр приза.
        /// </summary>
        /// <param name="position">Позиция приза.</param>
        /// <returns>Созданный приз.</returns>
        public abstract IPrize CreatePrize(Vector2 position);
    }

    /// <summary>
    /// Фабрика для создания призов топлива.
    /// </summary>
    public class FuelPrizeFactory : PrizeFactory
    {
        /// <summary>
        /// Создает приз топлива.
        /// </summary>
        /// <param name="position">Позиция приза.</param>
        /// <returns>Приз топлива.</returns>
        public override IPrize CreatePrize(Vector2 position) => new FuelPrize(position);
    }

    /// <summary>
    /// Фабрика для создания призов ускорения.
    /// </summary>
    public class SpeedBoostPrizeFactory : PrizeFactory
    {
        /// <summary>
        /// Создает приз ускорения.
        /// </summary>
        /// <param name="position">Позиция приза.</param>
        /// <returns>Приз ускорения.</returns>
        public override IPrize CreatePrize(Vector2 position) => new SpeedBoostPrize(position);
    }

    /// <summary>
    /// Фабрика для создания призов замедления.
    /// </summary>
    public class SlowDownPrizeFactory : PrizeFactory
    {
        /// <summary>
        /// Создает приз замедления.
        /// </summary>
        /// <param name="position">Позиция приза.</param>
        /// <returns>Приз замедления.</returns>
        public override IPrize CreatePrize(Vector2 position) => new SlowDownPrize(position);
    }
}