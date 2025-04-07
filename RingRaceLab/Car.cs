using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace RingRaceLab
{
    public class Car
    {
        // Положение и угол машины
        public Vector2 Position { get; set; }
        public float Angle { get; set; }

        // Параметры для движения вперёд:
        public float ForwardAcceleration { get; set; }  // Ускорение при движении вперёд (положительное)
        public float ForwardMaxSpeed { get; set; }       // Максимальная скорость вперёд

        // Параметры для движения назад:
        public float ReverseAcceleration { get; set; }   // Ускорение при движении назад (увеличивает отрицательную скорость)
        public float ReverseMaxSpeed { get; set; }        // Максимальная скорость назад (по модулю)

        // Общая скорость замедления при отсутствии ввода (торможение)
        public float Deceleration { get; set; }

        // Скорость поворота (в градусах в секунду)
        public float TurnSpeed { get; set; }

        // Текущая скорость машины (может быть отрицательной для заднего хода)
        public float CurrentSpeed { get; set; }

        private int textureId;
        public Color CarColor { get; set; } // Если нужно задавать цвет, здесь можно использовать

        public Car(Vector2 startPosition, string texturePath)
        {
            Position = startPosition;
            Angle = 0f;

            // Инициализация параметров для движения вперёд
            ForwardAcceleration = 150f;  // Машина быстрее разгоняется вперёд
            ForwardMaxSpeed = 350f;        // Максимальная скорость вперёд

            // Инициализация параметров для движения назад
            ReverseAcceleration = 120f;    // Задний ход может разгоняться медленнее
            ReverseMaxSpeed = 200f;        // Максимальная скорость заднего хода (по модулю)

            // Замедление (торможение) при отпускании клавиш
            Deceleration = 75f;

            // Скорость поворота (поворот на 180 градусов в секунду)
            TurnSpeed = 180f;

            // Изначально машина неподвижна
            CurrentSpeed = 0f;

            // Загружаем текстуру машины
            textureId = LoadTexture(texturePath);
        }

        private int LoadTexture(string path)
        {
            try
            {
                int id;
                using (Bitmap bitmap = new Bitmap(path))
                {
                    GL.GenTextures(1, out id);
                    GL.BindTexture(TextureTarget.Texture2D, id);

                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                    BitmapData data = bitmap.LockBits(
                        new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                        ImageLockMode.ReadOnly,
                        System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    GL.TexImage2D(TextureTarget.Texture2D,
                        0,
                        PixelInternalFormat.Rgba,
                        data.Width,
                        data.Height,
                        0,
                        OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                        PixelType.UnsignedByte,
                        data.Scan0);

                    bitmap.UnlockBits(data);
                }
                return id;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки текстуры: " + ex.Message);
                throw; // Если ошибка, выбрасываем исключение
            }
        }

        /// <summary>
        /// Метод Update осуществляет обновление состояния машины.
        /// Здесь задается логика ускорения для движения вперёд и заднего хода с разными параметрами,
        /// а также плавное торможение по умолчанию. Поворот осуществляется с помощью TurnSpeed.
        /// </summary>
        public void Update(float deltaTime, bool moveForward, bool moveBackward, bool turnLeft, bool turnRight)
        {

            // Обработка ускорения и ускорения назад:
            if (moveForward && !moveBackward)
            {
                // Если нажата клавиша для движения вперед, увеличиваем CurrentSpeed
                CurrentSpeed += ForwardAcceleration * deltaTime;
                if (CurrentSpeed > ForwardMaxSpeed)
                    CurrentSpeed = ForwardMaxSpeed;
            }
            else if (moveBackward && !moveForward)
            {
                // Если нажата клавиша для движения назад, уменьшаем CurrentSpeed (делая его отрицательным)
                CurrentSpeed -= ReverseAcceleration * deltaTime;
                if (CurrentSpeed < -ReverseMaxSpeed)
                    CurrentSpeed = -ReverseMaxSpeed;
            }
            else
            {
                // Если ни одна команда не нажата, применяем замедление по направлению к 0
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

            // Порог, ниже которого поворот не выполняется (например, 0.1f или любая другая подходящая величина)
            float turningThreshold = 0.1f;
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

        /// <summary>
        /// Метод Draw отрисовывает машину с использованием текстурированного прямоугольника фиксированного размера.
        /// Здесь размер машины задается фиксированно (50x25 пикселей для базового разрешения).
        /// </summary>
        public void Draw()
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
