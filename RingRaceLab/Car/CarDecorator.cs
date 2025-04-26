using System.Timers;

namespace RingRaceLab
{
    public abstract class CarDecorator : Car
    {
        protected Car _decoratedCar;
        protected Timer _timer;

        public CarDecorator(Car car) : base(car._movement.Position, car._renderer._texturePath, car._movement._config)
        {
            _decoratedCar = car;
            _timer = new Timer();
            _timer.Elapsed += OnTimerEnd;
        }

        protected virtual void OnTimerEnd(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();
            _decoratedCar.RemoveDecorator(this);
        }
    }
}