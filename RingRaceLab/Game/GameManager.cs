using OpenTK;
using System;
using System.Collections.Generic;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;
using System.Timers;

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

        private List<IPrize> _activePrizes = new List<IPrize>();
        private PrizeFactory[] _prizeFactories = {
            new FuelPrizeFactory(),
            new SpeedBoostPrizeFactory(),
            new SlowDownPrizeFactory()
        };

        private System.Timers.Timer _prizeRespawnTimer;
        private const int MIN_PRIZES = 3;
        private const int MAX_PRIZES = 5;
        private const int RESPAWN_INTERVAL = 3000; // 3 секунд

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

            _prizeRespawnTimer = new System.Timers.Timer
            {
                Interval = RESPAWN_INTERVAL
            };
            _prizeRespawnTimer.Elapsed += (s, e) => RespawnPrizes();
            _prizeRespawnTimer.AutoReset = true;
            _prizeRespawnTimer.Start();

            SpawnPrizes(MAX_PRIZES);
        }

        public void Update(GLControl glControl)
        {
            float deltaTime = (float)_stopwatch.Elapsed.TotalSeconds;
            _stopwatch.Restart(); 

            var input1 = _inputHandler.GetPlayer1Input();
            var input2 = _inputHandler.GetPlayer2Input();

            UpdateCar(Car1, deltaTime, input1);
            UpdateCar(Car2, deltaTime, input2);

            foreach (var prize in _activePrizes.ToList())
            {
                if (CheckCarPrizeCollision(Car1, prize) || CheckCarPrizeCollision(Car2, prize))
                {
                    prize.ApplyEffect(Car1);
                    _activePrizes.Remove(prize);
                }
            }

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
            foreach (var prize in _activePrizes)
            {
                GL.PushMatrix();
                GL.Enable(EnableCap.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, prize.TextureId);

                // Для текстур 8x16 пикселей
                GL.Begin(PrimitiveType.Quads);
                GL.TexCoord2(0, 0); GL.Vertex2(prize.Position.X - 4, prize.Position.Y - 8); // X: -4/+4, Y: -8/+8
                GL.TexCoord2(1, 0); GL.Vertex2(prize.Position.X + 4, prize.Position.Y - 8);
                GL.TexCoord2(1, 1); GL.Vertex2(prize.Position.X + 4, prize.Position.Y + 8);
                GL.TexCoord2(0, 1); GL.Vertex2(prize.Position.X - 4, prize.Position.Y + 8);
                GL.End();

                GL.PopMatrix();
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

        private void SpawnPrizes(int count)
        {
            Random rand = new Random();
            int maxAttempts = 100;
            int spawned = 0;

            while (spawned < count && maxAttempts-- > 0)
            {
                Vector2 position = new Vector2(
                    rand.Next(50, _collisionSystem.Width - 50),
                    rand.Next(50, _collisionSystem.Height - 50)
                );

                if (IsValidPosition(position))
                {
                    var factory = _prizeFactories[rand.Next(_prizeFactories.Length)];
                    _activePrizes.Add(factory.CreatePrize(position));
                    spawned++;
                }
            }


        }

        private bool CheckCarPrizeCollision(Car car, IPrize prize)
        {
            return (car._movement.Position - prize.Position).Length < 50;
        }

        private bool IsValidPosition(Vector2 position)
        {
            // 1. Проверка коллизий с дорогой
            if (!_collisionSystem.IsDrivable((int)position.X, (int)position.Y))
                return false;

            // 2. Проверка расстояния до других призов
            foreach (var existingPrize in _activePrizes)
            {
                if (Vector2.Distance(position, existingPrize.Position) < 50)
                    return false;
            }

            // 3. Проверка расстояния до стартовой позиции
            if (Vector2.Distance(position, Track.SpawnPositions[0]) < 100 ||
                Vector2.Distance(position, Track.SpawnPositions[1]) < 100)
                return false;

            return true;
        }

        // Новый метод для респауна
        private void RespawnPrizes()
        {
            if (_activePrizes.Count < MIN_PRIZES)
            {
                int needed = MAX_PRIZES - _activePrizes.Count;
                SpawnPrizes(needed);
            }
        }
    }
}