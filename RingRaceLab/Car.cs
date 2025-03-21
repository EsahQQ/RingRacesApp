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
        public Vector2 Position { get; set; }
        public float Angle { get; set; }
        public float Speed { get; set; }
        public float TurnSpeed { get; set; }

        private int textureId;


        // Новое свойство для цвета
        public Color CarColor { get; set; }

        public Car(Vector2 startPosition, string texturePath)
        {
            Position = startPosition;
            Angle = 0f;
            Speed = 200f;
            TurnSpeed = 180f;

            // Загружаем текстуру по указанному пути
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

                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                        data.Width, data.Height, 0,
                        OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                        PixelType.UnsignedByte, data.Scan0);

                    bitmap.UnlockBits(data);
                }
                return id;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки текстуры: " + ex.Message);
                throw; // Или верните значение по умолчанию, если хотите продолжить работу
            }
        }


        public void Update(float deltaTime, bool moveForward, bool moveBackward, bool turnLeft, bool turnRight)
        {
            if (turnLeft)
                Angle -= TurnSpeed * deltaTime;
            if (turnRight)
                Angle += TurnSpeed * deltaTime;

            float rad = MathHelper.DegreesToRadians(Angle);
            Vector2 direction = new Vector2((float)Math.Cos(rad), (float)Math.Sin(rad));

            if (moveForward)
                Position += direction * Speed * deltaTime;
            if (moveBackward)
                Position -= direction * Speed * deltaTime;
        }

        public void Draw()
        {
            float width = 50f;
            float height = 25f;

            GL.PushMatrix();
            // Помещаем систему координат туда, где находится машинка
            GL.Translate(Position.X, Position.Y, 0);
            // Поворачиваем в соответствии с углом машинки
            GL.Rotate(Angle, 0, 0, 1);

            // Включаем применение текстур
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, textureId);

            // Рисуем прямоугольник с наложенной текстурой, задавая текстурные координаты
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
