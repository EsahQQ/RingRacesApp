using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RingRaceLab
{
    public class Car : GameEntity
    {
        internal CarMovement _movement;
        internal CarRenderer _renderer;
        internal CarPhysics _physics;
        public int lapsComplete = -1;

        public float Fuel { get; set; } = 30;
        public List<CarDecorator> ActiveDecorators = new List<CarDecorator>();

        public Car(Vector2 startPosition, string texturePath, CarConfig config)
        {
            _movement = new CarMovement(startPosition, config);
            _renderer = new CarRenderer(texturePath, config.Size);
            _physics = new CarPhysics(config.Size);
        }

        public void Update(float deltaTime, bool moveForward, bool moveBackward, bool turnLeft, bool turnRight)
        {
            Fuel -= Math.Abs(_movement.CurrentSpeed) * deltaTime * _movement._config.FuelConsumptionRate;
            Fuel = Math.Max(0, Fuel);

            if (Fuel > 0)
            {
                _movement.Update(deltaTime, moveForward, moveBackward, turnLeft, turnRight);
            }

            // Обновление декораторов
            foreach (var decorator in ActiveDecorators.ToList())
            {
                decorator.Update(deltaTime, moveForward, moveBackward, turnLeft, turnRight);
            }
        }

        public void AddDecorator(CarDecorator decorator)
        {
            ActiveDecorators.Add(decorator);
        }

        public void RemoveDecorator(CarDecorator decorator)
        {
            ActiveDecorators.Remove(decorator);
        }

        public override void Draw()
        {
            _renderer.Draw(_movement.Position, _movement.Angle);
        }

        public List<Vector2> GetCorners()
        {
            return _physics.GetCorners(_movement.Position, _movement.Angle);
        }
    }
}