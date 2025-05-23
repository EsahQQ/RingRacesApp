using System;
using System.Timers;

namespace RingRaceLab
{
    /// <summary>
    /// Абстрактный базовый класс для декораторов автомобиля.
    /// </summary>
    public abstract class CarDecorator
    {
        /// <summary>
        /// Декорируемый автомобиль.
        /// </summary>
        protected readonly Car _car;

        /// <summary>
        /// Таймер длительности эффекта.
        /// </summary>
        public readonly Timer _timer;

        /// <summary>
        /// Длительность эффекта в секундах.
        /// </summary>
        protected readonly float _duration;

        /// <summary>
        /// Время начала действия таймера.
        /// </summary>
        public DateTime timerStartTime;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="CarDecorator"/>.
        /// </summary>
        /// <param name="car">Автомобиль для декорирования.</param>
        /// <param name="duration">Длительность эффекта в секундах.</param>
        public CarDecorator(Car car, float duration)
        {
            _car = car;
            _duration = duration;
            _timer = new Timer(duration * 1000);
            _timer.Elapsed += OnTimerEnd;
        }

        /// <summary>
        /// Применяет эффект декоратора и запускает таймер.
        /// </summary>
        public virtual void Apply()
        {
            _timer.Start();
            timerStartTime = DateTime.Now;
            ApplyEffect(); // Абстрактный метод для специфичного эффекта
        }

        /// <summary>
        /// Останавливает таймер и отменяет эффект декоратора.
        /// </summary>
        public virtual void Remove()
        {
            _timer.Stop();
            RevertEffect(); // Абстрактный метод для отмены специфичного эффекта
        }

        /// <summary>
        /// Обновляет состояние декоратора (если необходимо).
        /// </summary>
        /// <param name="deltaTime">Время, прошедшее с последнего обновления.</param>
        public void Update(float deltaTime)
        {
            // Логика обновления (если требуется в наследниках)
        }

        /// <summary>
        /// Применяет специфичный эффект декоратора к автомобилю. Реализуется в наследниках.
        /// </summary>
        protected abstract void ApplyEffect();

        /// <summary>
        /// Отменяет специфичный эффект декоратора. Реализуется в наследниках.
        /// </summary>
        protected abstract void RevertEffect();

        /// <summary>
        /// Обработчик события окончания таймера. Удаляет декоратор из автомобиля.
        /// </summary>
        private void OnTimerEnd(object sender, ElapsedEventArgs e)
        {
            _car.RemoveDecorator(); // Удаление декоратора через метод автомобиля
        }
    }
}