using OpenTK;
using System;
using System.Collections.Generic;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;
using System.Timers;
using System.Threading;
using System.Drawing;
using RingRaceLab.Game;

namespace RingRaceLab
{
    public class GameManager
    {
        private readonly EntityManager _entityManager = new EntityManager();
        private readonly PrizeManager _prizeManager;
        private readonly HUDRenderer _hudRenderer;
        private readonly CollisionMask _collisionSystem;
        private readonly InputManager _inputManager = new InputManager();
        private readonly Stopwatch _stopwatch = new Stopwatch();
        public delegate void CarFinishedHandler(Car finishedCar);
        public event CarFinishedHandler OnCarFinished;
        public Track Track { get; private set; }
        public Car Car1 { get; private set; }
        public Car Car2 { get; private set; }

        private int _currentFuelLevel1 = 5;
        private int _currentSpeedLevel1 = 0;
        private int _currentSlowLevel1 = 0;

        private int _currentFuelLevel2 = 5;
        private int _currentSpeedLevel2 = 0;
        private int _currentSlowLevel2 = 0;

        private Dictionary<string, int> _textures = new Dictionary<string, int>();

        public GameManager(string trackTexture, string collisionMap, Vector2[] spawnPositions, Vector2[] finishPosition, string player1CarTexture, string player2CarTexture, int Width, int Height)
        {
            _stopwatch.Start();
            if (spawnPositions == null || spawnPositions.Length < 2)
                throw new ArgumentException("Необходимо задать хотя бы две стартовые позиции.", nameof(spawnPositions));
            Track = new Track(trackTexture, spawnPositions, finishPosition[0], finishPosition[1], Width, Height);
            CarConfig config1 = new CarConfig
            {
                Size = new Vector2(Width / 60, (int)(Height / 67.5)),
            }; 
            CarConfig config2 = new CarConfig
            {
                Size = new Vector2(Width / 60, (int)(Height / 67.5)),
            };
            Car1 = new Car(spawnPositions[0], player1CarTexture, config1);
            Car2 = new Car(spawnPositions[1], player2CarTexture, config2);
            _collisionSystem = new CollisionMask(collisionMap);
            _entityManager.AddEntity(Track);
            _entityManager.AddEntity(Car1);
            _entityManager.AddEntity(Car2);

            PrizeFactory[] prizeFactories = {
                new FuelPrizeFactory(),
                new SpeedBoostPrizeFactory(),
                new SlowDownPrizeFactory()
            };
            _prizeManager = new PrizeManager(prizeFactories, _collisionSystem, Width, Height);
            _prizeManager.SpawnPrizes(10);
            
            LoadTextures();
            _hudRenderer = new HUDRenderer(_textures, Width, Height);
        }

        private void LoadTextures()
        {
            // Индикаторы
            _textures["fuel0"] = TextureLoader.LoadFromFile("sprites/fuel_indicator_0.png");
            _textures["fuel1"] = TextureLoader.LoadFromFile("sprites/fuel_indicator_1.png");
            _textures["fuel2"] = TextureLoader.LoadFromFile("sprites/fuel_indicator_2.png");
            _textures["fuel3"] = TextureLoader.LoadFromFile("sprites/fuel_indicator_3.png");
            _textures["fuel4"] = TextureLoader.LoadFromFile("sprites/fuel_indicator_4.png");
            _textures["fuel5"] = TextureLoader.LoadFromFile("sprites/fuel_indicator_5.png");

            _textures["speed0"] = TextureLoader.LoadFromFile("sprites/speed_indicator_0.png");
            _textures["speed1"] = TextureLoader.LoadFromFile("sprites/speed_indicator_1.png");
            _textures["speed2"] = TextureLoader.LoadFromFile("sprites/speed_indicator_2.png");
            _textures["speed3"] = TextureLoader.LoadFromFile("sprites/speed_indicator_3.png");
            _textures["speed4"] = TextureLoader.LoadFromFile("sprites/speed_indicator_4.png");
            _textures["speed5"] = TextureLoader.LoadFromFile("sprites/speed_indicator_5.png");

            _textures["slow0"] = TextureLoader.LoadFromFile("sprites/slow_indicator_0.png");
            _textures["slow1"] = TextureLoader.LoadFromFile("sprites/slow_indicator_1.png");
            _textures["slow2"] = TextureLoader.LoadFromFile("sprites/slow_indicator_2.png");
            _textures["slow3"] = TextureLoader.LoadFromFile("sprites/slow_indicator_3.png");
            _textures["slow4"] = TextureLoader.LoadFromFile("sprites/slow_indicator_4.png");
            _textures["slow5"] = TextureLoader.LoadFromFile("sprites/slow_indicator_5.png");

        }

        private void UpdateIndicators(Car car, ref int fuel, ref int speed, ref int slow)
        {
            if (car.Fuel > 80) fuel = 5;
            else if (car.Fuel > 60) fuel = 4;
            else if (car.Fuel > 40) fuel = 3;
            else if (car.Fuel > 20) fuel = 2;
            else if (car.Fuel > 0) fuel = 1;
            else fuel = 0;
            if (car._currentDecorator != null)
            {
                TimeSpan elapsed = DateTime.Now - car._currentDecorator.timerStartTime;
                int remaining = (int)((car._currentDecorator._timer.Interval - elapsed.TotalMilliseconds) / 1000) + 1;

                if (car._currentDecorator is SpeedBoostDecorator)
                {
                    if (remaining == 5)
                        speed = 5;
                    else if (remaining == 4)
                        speed = 4;
                    else if (remaining == 3)
                        speed = 3;
                    else if (remaining == 2)
                        speed = 2;
                    else if (remaining == 1)
                        speed = 1;
                    else
                        speed = 0;
                    slow = 0;
                }
                else
                {
                    if (remaining == 5)
                        slow = 5;
                    else if (remaining == 4)
                        slow = 4;
                    else if (remaining == 3)
                        slow = 3;
                    else if (remaining == 2)
                        slow = 2;
                    else if (remaining == 1)
                        slow = 1;
                    else
                        slow = 0;
                    speed = 0;
                }
            }
            else
            {
                speed = 0;
                slow = 0;
            }
        }

        public void Update(GLControl glControl)
        {
            float deltaTime = (float)_stopwatch.Elapsed.TotalSeconds;
            _stopwatch.Restart(); 

            var input1 = _inputManager.GetPlayer1Input();
            var input2 = _inputManager.GetPlayer2Input();

            UpdateCar(Car1, deltaTime, input1);
            UpdateCar(Car2, deltaTime, input2);

            _prizeManager.CheckPrizeCollisions(Car1);
            _prizeManager.CheckPrizeCollisions(Car2);

            UpdateIndicators(Car1, ref _currentFuelLevel1, ref _currentSpeedLevel1, ref _currentSlowLevel1);
            UpdateIndicators(Car2, ref _currentFuelLevel2, ref _currentSpeedLevel2, ref _currentSlowLevel2);
            glControl.Invalidate();
        }

        

        private void UpdateCar(Car car, float deltaTime, (bool forward, bool backward, bool left, bool right) input)
        {
            Vector2 oldPos = car._movement.Position; 
            float oldAngle = car._movement.Angle;    

            car.Update(deltaTime, input.forward, input.backward, input.left, input.right);

            int hasCrossed = Track.FinishLine.CheckCrossing(oldPos, car._movement.Position);
            // Проверка пересечения финиша
            if (hasCrossed == 1)
            {
                car.lapsComplete++;
                // Триггерим событие финиша
                if (car.lapsComplete == 0)
                {
                    OnCarFinished?.Invoke(car);
                }
            }
            else if (hasCrossed == -1)
            {
                car.lapsComplete--;
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
            foreach (var entity in _entityManager._entities)
            {
                entity.Draw();
            }

            _prizeManager.DrawPrizes();

            _hudRenderer.DrawHUD(Car1, Car2);

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