using OpenTK;
using System;
using System.Collections.Generic;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;

namespace RingRaceLab
{
    public class GameManager
    {
        private readonly List<GameEntity> _entities = new List<GameEntity>();
        private readonly CollisionMask _collisionSystem;
        private readonly InputHandler _inputHandler = new InputHandler();
        private readonly Stopwatch _stopwatch = new Stopwatch();
        public event Action OnGameFinished;
        public Track Track { get; private set; }
        public Car Car1 { get; private set; }
        public Car Car2 { get; private set; }

        public GameManager(string trackTexture, string collisionMap, Vector2[] spawnPositions)
        {
            _stopwatch.Start();
            if (spawnPositions == null || spawnPositions.Length < 2)
                throw new ArgumentException("Необходимо задать хотя бы две стартовые позиции.", nameof(spawnPositions));

            Track = new Track(trackTexture, spawnPositions);
            CarConfig config1 = new CarConfig(); // Можно настроить параметры по умолчанию или передать специфичные
            CarConfig config2 = new CarConfig(); // Аналогично для второго автомобиля
            Car1 = new Car(spawnPositions[0], "sprites/car2.png", config1);
            Car2 = new Car(spawnPositions[1], "sprites/car1.png", config2);

            _collisionSystem = new CollisionMask(collisionMap);

            _entities.Add(Track);
            _entities.Add(Car1);
            _entities.Add(Car2);
        }

        public void Update(GLControl glControl)
        {
            float deltaTime = (float)_stopwatch.Elapsed.TotalSeconds;
            _stopwatch.Restart();

            var input1 = _inputHandler.GetPlayer1Input();
            var input2 = _inputHandler.GetPlayer2Input();

            UpdateCar(Car1, deltaTime, input1);
            UpdateCar(Car2, deltaTime, input2);

            glControl.Invalidate();
        }

        private void UpdateCar(Car car, float deltaTime, (bool forward, bool backward, bool left, bool right) input)
        {
            Vector2 oldPos = car._movement.Position; // Предполагается, что Position доступно через геттер
            float oldAngle = car._movement.Angle;    // Аналогично для Angle

            car.Update(deltaTime, input.forward, input.backward, input.left, input.right);

            if (_collisionSystem.CheckCollision(car))
            {
                car._movement.Position = oldPos;
                car._movement.Angle = oldAngle;
                car._movement.CurrentSpeed *= -0.25f; // Логика отскока
            }
        }

        public void Draw()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            foreach (var entity in _entities)
            {
                entity.Draw();
            }
        }
    }
}