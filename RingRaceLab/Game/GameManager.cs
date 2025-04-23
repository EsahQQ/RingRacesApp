using OpenTK;
using System;
using System.Collections.Generic;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;
using System.Windows.Forms;

namespace RingRaceLab
{
    public class GameManager
    {
        private readonly List<GameEntity> _entities = new List<GameEntity>();
        private readonly CollisionMask _collisionSystem;
        private readonly InputHandler _inputHandler = new InputHandler();
        private readonly Stopwatch _stopwatch = new Stopwatch();
        public delegate void CarFinishedHandler(Car finishedCar);
        public event CarFinishedHandler OnCarFinished;
        private Dictionary<Car, int> _lapsPassed = new Dictionary<Car, int>();
        public Track Track { get; private set; }
        public Car Car1 { get; private set; }
        public Car Car2 { get; private set; }

        public GameManager(string trackTexture, string collisionMap, Vector2[] spawnPositions, Vector2[] finishPosition, string player1CarTexture, string player2CarTexture)
        {
            _stopwatch.Start();
            if (spawnPositions == null || spawnPositions.Length < 2)
                throw new ArgumentException("Необходимо задать хотя бы две стартовые позиции.", nameof(spawnPositions));
            Track = new Track(trackTexture, spawnPositions, finishPosition[0], finishPosition[1]);
            CarConfig config1 = new CarConfig(); // Можно настроить параметры по умолчанию или передать специфичные
            CarConfig config2 = new CarConfig(); 
            Car1 = new Car(spawnPositions[0], player1CarTexture, config1);
            Car2 = new Car(spawnPositions[1], player2CarTexture, config2);
            _lapsPassed.Add(Car1, -1);
            _lapsPassed.Add(Car2, -1);
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
            Vector2 oldPos = car._movement.Position; 
            float oldAngle = car._movement.Angle;    

            car.Update(deltaTime, input.forward, input.backward, input.left, input.right);

            // Проверка пересечения финиша
            if (Track.FinishLine.CheckCrossing(oldPos, car._movement.Position))
            {
                _lapsPassed[car]++;
                // Триггерим событие финиша
                if (_lapsPassed[car] == 5)
                {
                    OnCarFinished?.Invoke(car);
                }
            }

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
        public void Reset(Vector2[] spawnPositions)
        {
            // Переинициализируем машины
            Car1 = new Car(spawnPositions[0], "sprites/car2.png", new CarConfig());
            Car2 = new Car(spawnPositions[1], "sprites/car1.png", new CarConfig());

            // Сбросим внутреннее состояние
            _stopwatch.Restart();
        }
    }
}