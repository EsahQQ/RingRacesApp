using System.Timers;

namespace RingRaceLab
{
    public class SlowDownDecorator : CarDecorator
    {
        private float _originalSpeed;
        private float _multiplier;

        public SlowDownDecorator(Car car, float multiplier, float duration) : base(car)
        {
            _multiplier = multiplier;
            _originalSpeed = car._movement._config.ForwardMaxSpeed;
            car._movement._config.ForwardMaxSpeed *= multiplier;

            _timer.Interval = duration * 1000;
            _timer.Start();
        }

        protected override void OnTimerEnd(object sender, ElapsedEventArgs e)
        {
            _decoratedCar._movement._config.ForwardMaxSpeed = _originalSpeed;
            base.OnTimerEnd(sender, e);
        }
    }
}