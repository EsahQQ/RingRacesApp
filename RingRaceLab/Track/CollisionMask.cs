using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RingRaceLab
{
    public class CollisionMask
    {
        private readonly Bitmap _collisionBitmap;
        public int Width;
        public int Height;
        private bool[,] _drivablePixels;

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
                    _drivablePixels[x, y] = pixel.R < 100 && pixel.G < 100 && pixel.B < 100;
                }
            }
        }

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

        public virtual bool IsDrivable(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return false;
            return _drivablePixels[x, y];
        }
    }
}
