using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace RingRaceLab
{
    public class Car : GameEntity
    {

        public Vector2 Position { get; set; } // Положение
        public float Angle { get; set; } // Угол машины
        public float ForwardAcceleration { get; set; } // Ускорение при движении вперёд (положительное)
        public float ForwardMaxSpeed { get; set; } // Максимальная скорость вперёд
        public float ReverseAcceleration { get; set; } // Ускорение при движении назад (увеличивает отрицательную скорость)
        public float ReverseMaxSpeed { get; set; } // Максимальная скорость назад (по модулю)
        public float Deceleration { get; set; } // Общая скорость замедления при отсутствии ввода (торможение)
        public float TurnSpeed { get; set; } // Скорость поворота (в градусах в секунду)
        public float CurrentSpeed { get; set; } // Текущая скорость машины (может быть отрицательной для заднего хода)
        private int textureId; // Id текстуры

        public Car(Vector2 startPosition, string texturePath)
        {
            Position = startPosition;
            Angle = 0f;
            ForwardAcceleration = 150f; // Машина быстрее разгоняется вперёд
            ForwardMaxSpeed = 350f; // Максимальная скорость вперёд
            ReverseAcceleration = 120f; // Задний ход может разгоняться медленнее
            ReverseMaxSpeed = 200f; // Максимальная скорость заднего хода (по модулю)
            Deceleration = 75f; // Замедление (торможение) при отпускании клавиш
            TurnSpeed = 180f; // Скорость поворота (поворот на 180 градусов в секунду)
            CurrentSpeed = 0f; // Изначально машина неподвижна
            textureId = TextureLoader.LoadFromFile(texturePath); // Загружаем текстуру машины
        }


        public void Update(float deltaTime, bool moveForward, bool moveBackward, bool turnLeft, bool turnRight)
        {
            if (moveForward && !moveBackward) // Если нажата клавиша для движения вперед, увеличиваем CurrentSpeed
            {
                CurrentSpeed += ForwardAcceleration * deltaTime;
                if (CurrentSpeed > ForwardMaxSpeed)
                    CurrentSpeed = ForwardMaxSpeed;
            }
            else if (moveBackward && !moveForward) // Если нажата клавиша для движения назад, уменьшаем CurrentSpeed (делая его отрицательным)
            {
                CurrentSpeed -= ReverseAcceleration * deltaTime;
                if (CurrentSpeed < -ReverseMaxSpeed)
                    CurrentSpeed = -ReverseMaxSpeed;
            }
            else // Если ни одна команда не нажата, применяем замедление по направлению к 0
            {
                if (CurrentSpeed > 0)
                {
                    CurrentSpeed -= Deceleration * deltaTime;
                    if (CurrentSpeed < 0)
                        CurrentSpeed = 0;
                }
                else if (CurrentSpeed < 0)
                {
                    CurrentSpeed += Deceleration * deltaTime;
                    if (CurrentSpeed > 0)
                        CurrentSpeed = 0;
                }
            }
            float turningThreshold = 0.1f; // Порог, ниже которого поворот не выполняется (например, 0.1f или любая другая подходящая величина)
            // -------------------------
            // Здесь вычислим коэффициент поворота:
            // Чем быстрее скорость (по модулю), тем меньше поворот.
            // Если она нулевая, то умножаем TurnSpeed на 1; если скорость равна максимальной, то например можем ограничить до minTurnFactor.
            float max = ForwardMaxSpeed; // можно использовать ForwardMaxSpeed для вычислений,
                                         // либо, если скорость может быть отрицательной, задать отдельное значение.
            float speedAbs = Math.Abs(CurrentSpeed);
            float minTurnFactor = 0.8f; // минимальный коэффициент (20% от TurnSpeed)
                                        // Вычисляем линейно: когда скорость 0, коэффициент = 1; когда скорость = max, коэффициент = minTurnFactor.
            float turningFactor = Math.Max((max - speedAbs) / max, minTurnFactor);
            float effectiveTurnSpeed = TurnSpeed * turningFactor;

            // **Поворот выполняется только тогда, когда автомобиль уже движется достаточно**
            if (speedAbs > turningThreshold)
            {
                if (turnLeft)
                    Angle -= effectiveTurnSpeed * deltaTime;
                if (turnRight)
                    Angle += effectiveTurnSpeed * deltaTime;
            }

            // Вычисляем направление движения на основе текущего угла (переводим градусы в радианы)
            float rad = MathHelper.DegreesToRadians(Angle);
            Vector2 direction = new Vector2((float)Math.Cos(rad), (float)Math.Sin(rad));

            // Обновляем позицию машины с учетом текущей скорости
            Position += direction * CurrentSpeed * deltaTime;
        }

        public override void Draw()
        {
            float width = 32f;
            float height = 16f;

            GL.PushMatrix();
            GL.Translate(Position.X, Position.Y, 0);
            GL.Rotate(Angle, 0, 0, 1);

            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, textureId);

            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 0); GL.Vertex2(-width, -height);
            GL.TexCoord2(1, 0); GL.Vertex2(width, -height);
            GL.TexCoord2(1, 1); GL.Vertex2(width, height);
            GL.TexCoord2(0, 1); GL.Vertex2(-width, height);
            GL.End();

            GL.Disable(EnableCap.Texture2D);
            GL.PopMatrix();
        }

        public List<Vector2> GetCorners()
        {
            // Преобразуем угол в радианы для вычислений
            float rad = MathHelper.DegreesToRadians(Angle);
            float cos = (float)Math.Cos(rad);
            float sin = (float)Math.Sin(rad);

            // Вычисляем половины ширины и высоты
            float halfWidth = 32f;
            float halfHeight = 16f;

            // Локальные координаты углов (относительно центра автомобиля)
            Vector2 topLeft = new Vector2(-halfWidth, -halfHeight);
            Vector2 topRight = new Vector2(halfWidth, -halfHeight);
            Vector2 bottomRight = new Vector2(halfWidth, halfHeight);
            Vector2 bottomLeft = new Vector2(-halfWidth, halfHeight);

            List<Vector2> localCorners = new List<Vector2> { topLeft, topRight, bottomRight, bottomLeft };
            List<Vector2> globalCorners = new List<Vector2>();

            foreach (var local in localCorners)
            {
                // Применяем поворот и смещаем на позицию центра
                float rotatedX = local.X * cos - local.Y * sin;
                float rotatedY = local.X * sin + local.Y * cos;
                globalCorners.Add(new Vector2(Position.X + rotatedX, Position.Y + rotatedY));
            }

            return globalCorners;
        }
    }
}
