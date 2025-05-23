using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RingRaceLab
{
    /// <summary>
    /// Представляет маску коллизий, основанную на изображении.
    /// </summary>
    public class CollisionMask
    {
        /// <summary>
        /// Изображение коллизионной маски.
        /// </summary>
        private readonly Bitmap _collisionBitmap;

        /// <summary>
        /// Ширина маски.
        /// </summary>
        public int Width;

        /// <summary>
        /// Высота маски.
        /// </summary>
        public int Height;

        /// <summary>
        /// Массив булевых значений, указывающий, проходим ли каждый пиксель.
        /// </summary>
        private bool[,] _drivablePixels;

        /// <summary>
        /// Инициализирует новую маску коллизий из файла изображения.
        /// </summary>
        /// <param name="path">Путь к файлу изображения коллизионной маски.</param>
        /// <exception cref="Exception">Выбрасывается, если файл не найден.</exception>
        public CollisionMask(string path)
        {
            if (!System.IO.File.Exists(path))
                throw new Exception("Файл коллизионной карты не найден: " + path);
            _collisionBitmap = new Bitmap(path);
            Width = _collisionBitmap.Width;
            Height = _collisionBitmap.Height;
            _drivablePixels = new bool[Width, Height];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Color pixel = _collisionBitmap.GetPixel(x, y);
                    // Считаем пиксель проходимым, если он темный (R, G, B < 100)
                    _drivablePixels[x, y] = pixel.R < 100 && pixel.G < 100 && pixel.B < 100;
                }
            }
        }

        /// <summary>
        /// Проверяет, сталкивается ли автомобиль с непроходимой областью маски.
        /// </summary>
        /// <param name="car">Автомобиль для проверки.</param>
        /// <returns>True, если обнаружена коллизия, иначе false.</returns>
        public bool CheckCollision(Car car)
        {
            List<Vector2> corners = car.GetCorners();
            foreach (var corner in corners)
            {
                int x = (int)corner.X;
                int y = (int)corner.Y;
                if (!IsDrivable(x, y))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Проверяет, является ли пиксель с заданными координатами проходимым.
        /// </summary>
        /// <param name="x">Координата X пикселя.</param>
        /// <param name="y">Координата Y пикселя.</param>
        /// <returns>True, если пиксель проходим и находится в пределах маски, иначе false.</returns>
        public virtual bool IsDrivable(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return false;
            return _drivablePixels[x, y];
        }
    }
}