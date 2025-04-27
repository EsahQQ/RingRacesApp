using System.Timers;

namespace RingRaceLab
{
    public class SlowDownDecorator : CarDecorator
    {
        private readonly float _multiplier;
        private float _originalSpeed;

        public SlowDownDecorator(Car car, float multiplier, float duration)
            : base(car, duration)
        {
            _multiplier = multiplier;
        }

        protected override void ApplyEffect()
        {
            _originalSpeed = _car._movement._config.ForwardMaxSpeed;
            _car._movement._config.ForwardMaxSpeed *= _multiplier;
        }

        protected override void RevertEffect()
        {
            _car._movement._config.ForwardMaxSpeed = _originalSpeed;
        }
    }
}