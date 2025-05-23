using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RingRaceLab
{
    /// <summary>
    /// Представляет игровой автомобиль.
    /// </summary>
    public class Car : GameEntity // Убедитесь, что GameEntity доступен
    {
        /// <summary>
        /// Управление движением автомобиля.
        /// </summary>
        internal CarMovement _movement;

        /// <summary>
        /// Рендерер автомобиля.
        /// </summary>
        internal CarRenderer _renderer;

        /// <summary>
        /// Физика автомобиля.
        /// </summary>
        internal CarPhysics _physics;

        /// <summary>
        /// Количество завершенных кругов.
        /// </summary>
        public int lapsComplete = -1;

        /// <summary>
        /// Текущий уровень топлива.
        /// </summary>
        public float Fuel { get; set; } = 100;

        /// <summary>
        /// Активный декоратор (эффект приза).
        /// </summary>
        public CarDecorator _currentDecorator;

        /// <summary>
        /// Инициализирует новый автомобиль.
        /// </summary>
        /// <param name="startPosition">Стартовая позиция.</param>
        /// <param name="texturePath">Путь к текстуре.</param>
        /// <param name="config">Конфигурация автомобиля.</param>
        public Car(Vector2 startPosition, string texturePath, CarConfig config)
        {
            _movement = new CarMovement(startPosition, config);
            _renderer = new CarRenderer(texturePath, config.Size);
            _physics = new CarPhysics(config.Size);
        }

        /// <summary>
        /// Обновляет состояние автомобиля (движение, топливо, декоратор).
        /// </summary>
        /// <param name="deltaTime">Время с последнего обновления.</param>
        /// <param name="moveForward">Двигаться вперед?</param>
        /// <param name="moveBackward">Двигаться назад?</param>
        /// <param name="turnLeft">Повернуть влево?</param>
        /// <param name="turnRight">Повернуть вправо?</param>
        public void Update(float deltaTime, bool moveForward, bool moveBackward, bool turnLeft, bool turnRight)
        {
            Fuel -= Math.Abs(_movement.CurrentSpeed) * deltaTime * _movement._config.FuelConsumptionRate;
            Fuel = Math.Max(0, Fuel);

            if (Fuel > 0)
            {
                _movement.Update(deltaTime, moveForward, moveBackward, turnLeft, turnRight);
            }

            // Обновление декоратора
            _currentDecorator?.Update(deltaTime);
        }

        /// <summary>
        /// Применяет декоратор к автомобилю.
        /// </summary>
        /// <param name="newDecorator">Декоратор для применения.</param>
        public void ApplyDecorator(CarDecorator newDecorator)
        {
            _currentDecorator?.Remove();
            _currentDecorator = newDecorator;
            _currentDecorator.Apply();
        }

        /// <summary>
        /// Удаляет текущий декоратор с автомобиля.
        /// </summary>
        public void RemoveDecorator()
        {
            _currentDecorator?.Remove();
            _currentDecorator = null;
        }

        /// <summary>
        /// Отрисовывает автомобиль.
        /// </summary>
        public override void Draw()
        {
            _renderer.Draw(_movement.Position, _movement.Angle);
        }

        /// <summary>
        /// Получает координаты углов автомобиля.
        /// </summary>
        /// <returns>Список координат углов.</returns>
        public List<Vector2> GetCorners()
        {
            return _physics.GetCorners(_movement.Position, _movement.Angle);
        }
    }
}