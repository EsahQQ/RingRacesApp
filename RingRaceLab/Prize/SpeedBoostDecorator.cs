using System.Timers;

namespace RingRaceLab
{
    /// <summary>
    /// Декоратор, применяющий эффект ускорения к машине.
    /// </summary>
    public class SpeedBoostDecorator : CarDecorator
    {
        /// <summary>
        /// Множитель, применяемый к максимальной скорости.
        /// </summary>
        private readonly float _multiplier;

        /// <summary>
        /// Исходная максимальная скорость машины до применения эффекта.
        /// </summary>
        private float _originalSpeed;

        /// <summary>
        /// Инициализирует новый экземпляр декоратора ускорения.
        /// </summary>
        /// <param name="car">Машина для декорирования.</param>
        /// <param name="multiplier">Множитель для ускорения.</param>
        /// <param name="duration">Длительность эффекта в миллисекундах.</param>
        public SpeedBoostDecorator(Car car, float multiplier, float duration)
            : base(car, duration)
        {
            _multiplier = multiplier;
        }

        /// <summary>
        /// Применяет эффект ускорения, увеличивая максимальную скорость машины.
        /// </summary>
        protected override void ApplyEffect()
        {
            _originalSpeed = _car._movement._config.ForwardMaxSpeed;
            _car._movement._config.ForwardMaxSpeed *= _multiplier;
        }

        /// <summary>
        /// Отменяет эффект ускорения, восстанавливая исходную максимальную скорость.
        /// </summary>
        protected override void RevertEffect()
        {
            _car._movement._config.ForwardMaxSpeed = _originalSpeed;
        }
    }
}