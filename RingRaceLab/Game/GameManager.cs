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
using RingRaceLab.Prize;

namespace RingRaceLab
{
    /// <summary>
    /// Управляет основным игровым циклом, сущностями, столкновениями, призами и отображением HUD.
    /// </summary>
    public class GameManager
    {
        /// <summary>
        /// Менеджер сущностей для хранения и управления всеми игровыми объектами.
        /// </summary>
        private readonly EntityManager _entityManager = new EntityManager();

        /// <summary>
        /// Менеджер призов для создания, управления и обработки сбора призов.
        /// </summary>
        private readonly PrizeManager _prizeManager;

        /// <summary>
        /// Рендерер HUD для отображения информации о состоянии игроков.
        /// </summary>
        private readonly HUDRenderer _hudRenderer;

        /// <summary>
        /// Система обнаружения столкновений, основанная на маске коллизий.
        /// </summary>
        private readonly CollisionMask _collisionSystem;

        /// <summary>
        /// Менеджер ввода для обработки действий игроков.
        /// </summary>
        private readonly InputManager _inputManager = new InputManager();

        /// <summary>
        /// Секундомер для измерения времени между кадрами (для дельта времени).
        /// </summary>
        private readonly Stopwatch _stopwatch = new Stopwatch();

        /// <summary>
        /// Делегат для события завершения гонки автомобилем.
        /// </summary>
        /// <param name="finishedCar">Автомобиль, завершивший гонку.</param>
        public delegate void CarFinishedHandler(Car finishedCar);

        /// <summary>
        /// Событие, возникающее при завершении гонки одним из автомобилей.
        /// </summary>
        public event CarFinishedHandler OnCarFinished;

        /// <summary>
        /// Игровой трек.
        /// </summary>
        public Track Track { get; private set; }

        /// <summary>
        /// Первый автомобиль игрока.
        /// </summary>
        public Car Car1 { get; private set; }

        /// <summary>
        /// Второй автомобиль игрока.
        /// </summary>
        public Car Car2 { get; private set; }

        /// <summary>
        /// Словарь для хранения загруженных текстур по имени.
        /// </summary>
        private Dictionary<string, int> _textures = new Dictionary<string, int>();

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="GameManager"/>.
        /// Создает и настраивает все игровые компоненты: трек, автомобили,
        /// систему столкновений, менеджеры сущностей и призов, а также HUD.
        /// </summary>
        /// <param name="trackTexture">Путь к текстуре трека.</param>
        /// <param name="collisionMap">Путь к изображению маски коллизий.</param>
        /// <param name="spawnPositions">Массив стартовых позиций для автомобилей.</param>
        /// <param name="finishPosition">Позиция финишной линии (начало и конец отрезка).</param>
        /// <param name="player1CarTexture">Путь к текстуре автомобиля первого игрока.</param>
        /// <param name="player2CarTexture">Путь к текстуре автомобиля второго игрока.</param>
        /// <param name="Width">Ширина игрового окна.</param>
        /// <param name="Height">Высота игрового окна.</param>
        /// <exception cref="ArgumentException">Выбрасывается, если задано менее двух стартовых позиций.</exception>
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

        /// <summary>
        /// Загружает текстуры, используемые в игре, в словарь.
        /// </summary>
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

        /// <summary>
        /// Обновляет состояние игрового мира за один кадр.
        /// Обрабатывает ввод, обновляет положение автомобилей, проверяет коллизии
        /// и обновляет состояние индикаторов.
        /// </summary>
        /// <param name="glControl">Элемент управления GLControl для запроса перерисовки.</param>
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

            glControl.Invalidate();
        }


        /// <summary>
        /// Обновляет состояние одного автомобиля (движение, вращение, коллизии, финиш).
        /// </summary>
        /// <param name="car">Автомобиль для обновления.</param>
        /// <param name="deltaTime">Время, прошедшее с предыдущего кадра.</param>
        /// <param name="input">Ввод игрока для данного автомобиля.</param>
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
                if (car.lapsComplete == 5)
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

        /// <summary>
        /// Выполняет отрисовку всех игровых сущностей и HUD.
        /// </summary>
        public void Draw()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            foreach (var entity in _entityManager.GetEntities())
            {
                entity.Draw();
            }

            _prizeManager.DrawPrizes();

            _hudRenderer.DrawHUD(Car1, Car2);

        }
    }
}