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
        public Track Track { get; private set; }
        public Car Car1 { get; private set; }
        public Car Car2 { get; private set; }

        private List<IPrize> _activePrizes = new List<IPrize>();
        private PrizeFactory[] _prizeFactories = {
            new FuelPrizeFactory(),
            new SpeedBoostPrizeFactory(),
            new SlowDownPrizeFactory()
        };

        private int _currentFuelLevel1 = 5;
        private int _currentSpeedLevel1 = 0;
        private int _currentSlowLevel1 = 0;

        private int _currentFuelLevel2 = 5;
        private int _currentSpeedLevel2 = 0;
        private int _currentSlowLevel2 = 0;

        private System.Timers.Timer _prizeRespawnTimer;
        private const int MIN_PRIZES = 5;
        private const int MAX_PRIZES = 10;
        private const int RESPAWN_INTERVAL = 3000; // 3 секунд
        private Dictionary<string, int> _textures = new Dictionary<string, int>();

        public GameManager(string trackTexture, string collisionMap, Vector2[] spawnPositions, Vector2[] finishPosition, string player1CarTexture, string player2CarTexture, List<FlowLayoutPanel> playerPanels)
        {
            _stopwatch.Start();
            if (spawnPositions == null || spawnPositions.Length < 2)
                throw new ArgumentException("Необходимо задать хотя бы две стартовые позиции.", nameof(spawnPositions));
            Track = new Track(trackTexture, spawnPositions, finishPosition[0], finishPosition[1]);
            CarConfig config1 = new CarConfig(); // Можно настроить параметры по умолчанию или передать специфичные
            CarConfig config2 = new CarConfig(); 
            Car1 = new Car(spawnPositions[0], player1CarTexture, config1);
            Car2 = new Car(spawnPositions[1], player2CarTexture, config2);
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
            LoadTextures();
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

            var input1 = _inputHandler.GetPlayer1Input();
            var input2 = _inputHandler.GetPlayer2Input();

            UpdateCar(Car1, deltaTime, input1);
            UpdateCar(Car2, deltaTime, input2);

            List<IPrize> toRemove = new List<IPrize>();
            lock (_activePrizes)
            {
                foreach (var prize in _activePrizes.ToList())
                {
                    if (CheckCarPrizeCollision(Car1, prize))
                    {
                        prize.ApplyEffect(Car1);
                        _activePrizes.Remove(prize);
                    }
                    else if (CheckCarPrizeCollision(Car2, prize))
                    {
                        prize.ApplyEffect(Car2);
                        _activePrizes.Remove(prize);
                    }
                }
                // Удаляем после перебора
                foreach (var prize in toRemove)
                {
                    _activePrizes.Remove(prize);
                }
            }
            UpdateIndicators(Car1, ref _currentFuelLevel1, ref _currentSpeedLevel1, ref _currentSlowLevel1);
            UpdateIndicators(Car2, ref _currentFuelLevel2, ref _currentSpeedLevel2, ref _currentSlowLevel2);
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
                car.lapsComplete++;
                // Триггерим событие финиша
                if (car.lapsComplete == 5)
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
            DrawPlayerHUD();
            lock (_activePrizes)
            {
                foreach (var prize in _activePrizes)
                {
                    GL.PushMatrix();
                    GL.Enable(EnableCap.Texture2D);
                    GL.BindTexture(TextureTarget.Texture2D, prize.TextureId);

                    // Для текстур 16x32 пикселей
                    GL.Begin(PrimitiveType.Quads);
                    GL.TexCoord2(0, 0); GL.Vertex2(prize.Position.X - 8, prize.Position.Y - 16); // X: -8/+8, Y: -16/+16
                    GL.TexCoord2(1, 0); GL.Vertex2(prize.Position.X + 8, prize.Position.Y - 16);
                    GL.TexCoord2(1, 1); GL.Vertex2(prize.Position.X + 8, prize.Position.Y + 16);
                    GL.TexCoord2(0, 1); GL.Vertex2(prize.Position.X - 8, prize.Position.Y + 16);
                    GL.End();

                    GL.PopMatrix();
                }
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

        // метод для респауна
        private void RespawnPrizes()
        {
            if (_activePrizes.Count < MIN_PRIZES)
            {
                int needed = MAX_PRIZES - _activePrizes.Count;
                SpawnPrizes(needed);
            }
        }
        private void DrawPlayerHUD()
        {
            // Панель игрока 1
            DrawIndicator(new Rectangle(5, 5, 160, 40), $"fuel{_currentFuelLevel1}");
            DrawIndicator(new Rectangle(5, 50, 160, 40), $"speed{_currentSpeedLevel1}");
            DrawIndicator(new Rectangle(5, 95, 160, 40), $"slow{_currentSlowLevel1}");

            // Панель игрока 2
            DrawIndicator(new Rectangle(1920 - 165, 5, 160, 40), $"fuel{_currentFuelLevel2}");
            DrawIndicator(new Rectangle(1920 - 165, 50, 160, 40), $"speed{_currentSpeedLevel2}");
            DrawIndicator(new Rectangle(1920 - 165, 95, 160, 40), $"slow{_currentSlowLevel2}");
        }

        private void DrawIndicator(Rectangle rect, string textureKey)
        {
            if (!_textures.TryGetValue(textureKey, out int textureId)) return;

            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, textureId);

            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 0); GL.Vertex2(rect.Left, rect.Top);
            GL.TexCoord2(1, 0); GL.Vertex2(rect.Right, rect.Top);
            GL.TexCoord2(1, 1); GL.Vertex2(rect.Right, rect.Bottom);
            GL.TexCoord2(0, 1); GL.Vertex2(rect.Left, rect.Bottom);
            GL.End();

            GL.Disable(EnableCap.Texture2D);
        }
    }
}