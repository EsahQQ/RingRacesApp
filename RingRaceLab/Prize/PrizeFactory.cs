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
}