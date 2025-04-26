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

        public CollisionMask(string path)
        {
            if (!System.IO.File.Exists(path))
                throw new Exception("Файл коллизионной карты не найден: " + path);
            _collisionBitmap = new Bitmap(path);
            Width = _collisionBitmap.Width;
            Height = _collisionBitmap.Height;
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

        public bool IsDrivable(int x, int y)
        {
            if (x < 0 || y < 0 || x >= _collisionBitmap.Width || y >= _collisionBitmap.Height)
                return false;
            Color pixel = _collisionBitmap.GetPixel(x, y);
            return pixel.R < 100 && pixel.G < 100 && pixel.B < 100;
        }
    }
}
