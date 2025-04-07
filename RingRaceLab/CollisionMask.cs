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
        public Bitmap collisionBitmap;

        public int Width => collisionBitmap.Width;
        public int Height => collisionBitmap.Height;

        // Загрузить коллизионную карту по указанному пути
        public CollisionMask(string path)
        {
            if (!System.IO.File.Exists(path))
                throw new Exception("Файл коллизионной карты не найден: " + path);
            collisionBitmap = new Bitmap(path);
        }

        // Проверяет, можно ли ехать в точке (x,y).
        // Здесь считаем, что если пиксель «яркий» (например, значение R > 200), то это проходимая область.
        public bool IsDrivable(int x, int y)
        {
            // Если вышли за пределы изображения – считаем область недопустимой
            if (x < 0 || y < 0 || x >= collisionBitmap.Width || y >= collisionBitmap.Height)
                return false;

            Color pixel = collisionBitmap.GetPixel(x, y);
            // Простая проверка: если RGB значения выше порога, то область проходимая.
            return pixel.R < 100 && pixel.G < 100 && pixel.B < 100;
        }

        // Метод для проверки коллизии автомобиля по его углам (corners)
        public bool CheckCollision(Car car)
        {
            List<Vector2> corners = car.GetCorners();

            foreach (var corner in corners)
            {
                int checkX = (int)corner.X;
                int checkY = (int)corner.Y;

                if (!IsDrivable(checkX, checkY))
                    return true;
            }
            return false;
        }
    }
}
