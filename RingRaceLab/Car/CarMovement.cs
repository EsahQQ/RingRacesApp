using OpenTK;
using System;

namespace RingRaceLab
{
    /// <summary>
    /// Управляет движением и ориентацией автомобиля.
    /// </summary>
    public class CarMovement
    {
        /// <summary>
        /// Текущая позиция автомобиля.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Текущий угол ориентации автомобиля (в градусах).
        /// </summary>
        public float Angle { get; set; }

        /// <summary>
        /// Текущая скорость автомобиля.
        /// </summary>
        public float CurrentSpeed { get; set; }

        /// <summary>
        /// Конфигурация параметров движения автомобиля.
        /// </summary>
        public CarConfig _config; // Убрал internal, так как используется из Car

        /// <summary>
        /// Инициализирует управление движением автомобиля.
        /// </summary>
        /// <param name="startPosition">Начальная позиция.</param>
        /// <param name="config">Конфигурация движения.</param>
        public CarMovement(Vector2 startPosition, CarConfig config)
        {
            Position = startPosition;
            _config = config;
        }

        /// <summary>
        /// Обновляет позицию, угол и скорость автомобиля.
        /// </summary>
        /// <param name="deltaTime">Время, прошедшее с последнего обновления.</param>
        /// <param name="moveForward">Флаг движения вперед.</param>
        /// <param name="moveBackward">Флаг движения назад.</param>
        /// <param name="turnLeft">Флаг поворота влево.</param>
        /// <param name="turnRight">Флаг поворота вправо.</param>
        public void Update(float deltaTime, bool moveForward, bool moveBackward, bool turnLeft, bool turnRight)
        {
            // Переносим всю логику движения из старого класса Car
            if (moveForward && !moveBackward)
            {
                if (CurrentSpeed > 0)
                {
                    CurrentSpeed += _config.ForwardAcceleration * deltaTime;
                    CurrentSpeed = Math.Min(CurrentSpeed, _config.ForwardMaxSpeed);
                }
                else
                {
                    CurrentSpeed += _config.ForwardAcceleration * deltaTime * 2;
                    CurrentSpeed = Math.Min(CurrentSpeed, _config.ForwardMaxSpeed);
                }
            }
            else if (moveBackward && !moveForward)
            {
                if (CurrentSpeed < 0)
                {
                    CurrentSpeed -= _config.ReverseAcceleration * deltaTime;
                    CurrentSpeed = Math.Max(CurrentSpeed, -_config.ReverseMaxSpeed);
                }
                else
                {
                    CurrentSpeed -= _config.ReverseAcceleration * deltaTime * 3;
                    CurrentSpeed = Math.Max(CurrentSpeed, -_config.ReverseMaxSpeed);
                }
            }
            else
            {
                ApplyDeceleration(deltaTime);
            }

            UpdateRotation(deltaTime, turnLeft, turnRight);
            UpdatePosition(deltaTime);
        }


        private void ApplyDeceleration(float deltaTime)
        {
            if (CurrentSpeed > 0)
            {
                CurrentSpeed = Math.Max(0, CurrentSpeed - _config.Deceleration * deltaTime);
            }
            else if (CurrentSpeed < 0)
            {
                CurrentSpeed = Math.Min(0, CurrentSpeed + _config.Deceleration * deltaTime);
            }
        }

        private void UpdateRotation(float deltaTime, bool turnLeft, bool turnRight)
        {
            if (Math.Abs(CurrentSpeed) > 0.1f)
            {
                float speedFactor = Math.Max(
                    (_config.ForwardMaxSpeed - Math.Abs(CurrentSpeed)) / _config.ForwardMaxSpeed,
                    0.6f
                );

                float effectiveTurnSpeed = _config.TurnSpeed * speedFactor;

                if (turnLeft) Angle -= effectiveTurnSpeed * deltaTime;
                if (turnRight) Angle += effectiveTurnSpeed * deltaTime;
            }
        }

        private void UpdatePosition(float deltaTime)
        {
            float rad = MathHelper.DegreesToRadians(Angle);
            Vector2 direction = new Vector2((float)Math.Cos(rad), (float)Math.Sin(rad));
            Position += direction * CurrentSpeed * deltaTime;
        }
    }
}