using System;
using System.Timers;

namespace RingRaceLab
{
    public abstract class CarDecorator
    {
        protected readonly Car _car;
        public readonly Timer _timer;
        protected readonly float _duration;
        public DateTime timerStartTime;

        public CarDecorator(Car car, float duration)
        {
            _car = car;
            _duration = duration;
            _timer = new Timer(duration * 1000);
            _timer.Elapsed += OnTimerEnd;
        }

        public virtual void Apply()
        {
            _timer.Start();
            timerStartTime = DateTime.Now;
            ApplyEffect();
        }

        public virtual void Remove()
        {
            _timer.Stop();
            RevertEffect();
        }

        public void Update(float deltaTime)
        {
            // Логика обновления (если требуется)
        }

        protected abstract void ApplyEffect();
        protected abstract void RevertEffect();

        private void OnTimerEnd(object sender, ElapsedEventArgs e)
        {
            _car.RemoveDecorator();
        }
    }
}