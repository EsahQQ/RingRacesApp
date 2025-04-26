using OpenTK;
using System;

namespace RingRaceLab
{
    public class CarMovement
    {
        public Vector2 Position { get; set; }
        public float Angle { get;  set; }
        public float CurrentSpeed { get; set; }

        public CarConfig _config;

        public CarMovement(Vector2 startPosition, CarConfig config)
        {
            Position = startPosition;
            _config = config;
        }

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
                    0.8f
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