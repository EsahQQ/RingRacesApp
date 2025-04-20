using OpenTK;
using System;
using System.Collections.Generic;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;   

namespace RingRaceLab
{
    public class GameManager
    {
        private readonly List<GameEntity> _entities = new List<GameEntity>();
        private readonly CollisionSystem _collisionSystem;
        private readonly InputHandler _inputHandler = new InputHandler();
        private DateTime _lastFrameTime = DateTime.Now;

        public Track Track { get; private set; }
        public Car Car1 { get; private set; }
        public Car Car2 { get; private set; }

        public GameManager(string trackTexture, string collisionMap, Vector2[] spawnPositions)
        {
            // Инициализация объектов
            Track = new Track(trackTexture, spawnPositions);
            Car1 = new Car(spawnPositions[0], "sprites/car2.png");
            Car2 = new Car(spawnPositions[1], "sprites/car1.png");

            _collisionSystem = new CollisionSystem(collisionMap);

            _entities.Add(Track);
            _entities.Add(Car1);
            _entities.Add(Car2);
        }

        public void Update(GLControl glControl)
        {
            // Расчет времени
            float deltaTime = (float)(DateTime.Now - _lastFrameTime).TotalSeconds;
            _lastFrameTime = DateTime.Now;

            // Обработка ввода
            var input1 = _inputHandler.GetPlayer1Input();
            var input2 = _inputHandler.GetPlayer2Input();

            // Обновление состояния
            UpdateCar(Car1, deltaTime, input1);
            UpdateCar(Car2, deltaTime, input2);

            // Обновление отображения
            glControl.Invalidate();
        }

        private void UpdateCar(Car car, float deltaTime, (bool forward, bool backward, bool left, bool right) input)
        {
            Vector2 oldPos = car.Position;
            float oldAngle = car.Angle;

            car.Update(deltaTime, input.forward, input.backward, input.left, input.right);

            if (_collisionSystem.CheckCollision(car))
            {
                car.Position = oldPos;
                car.Angle = oldAngle;
                car.CurrentSpeed *= -0.3f;
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