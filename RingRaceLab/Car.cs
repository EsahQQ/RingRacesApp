using System;
using System.Drawing;
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

        // Новое свойство для цвета
        public Color CarColor { get; set; }

        public Car(Vector2 startPosition, Color? color = null)
        {
            Position = startPosition;
            Angle = 0f;
            Speed = 200f;
            TurnSpeed = 180f;
            // Если цвет не указан, используем красный
            CarColor = color ?? Color.Red;
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
            GL.Translate(Position.X, Position.Y, 0);
            GL.Rotate(Angle, 0, 0, 1);

            GL.Begin(PrimitiveType.Quads);
            // Преобразуем System.Drawing.Color в OpenTK.Graphics.Color3:
            GL.Color3(CarColor);
            GL.Vertex2(-width / 2, -height / 2);
            GL.Vertex2(width / 2, -height / 2);
            GL.Vertex2(width / 2, height / 2);
            GL.Vertex2(-width / 2, height / 2);
            GL.End();

            GL.PopMatrix();
        }
    }
}
