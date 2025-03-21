using System;
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
            ForwardMaxSpeed = 400f;        // Максимальная скорость вперёд

            // Инициализация параметров для движения назад
            ReverseAcceleration = 150f;    // Задний ход может разгоняться медленнее
            ReverseMaxSpeed = 200f;        // Максимальная скорость заднего хода (по модулю)

            // Замедление (торможение) при отпускании клавиш
            Deceleration = 200f;

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
            // Обработка поворота: применяем TurnSpeed (поворот в градусах в секунду)
            if (turnLeft)
                Angle -= TurnSpeed * deltaTime;
            if (turnRight)
                Angle += TurnSpeed * deltaTime;

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
            float width = 50f;
            float height = 25f;

            GL.PushMatrix();
            GL.Translate(Position.X, Position.Y, 0);
            GL.Rotate(Angle, 0, 0, 1);

            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, textureId);

            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 0); GL.Vertex2(-width / 2, -height / 2);
            GL.TexCoord2(1, 0); GL.Vertex2(width / 2, -height / 2);
            GL.TexCoord2(1, 1); GL.Vertex2(width / 2, height / 2);
            GL.TexCoord2(0, 1); GL.Vertex2(-width / 2, height / 2);
            GL.End();

            GL.Disable(EnableCap.Texture2D);
            GL.PopMatrix();
        }
    }
}
