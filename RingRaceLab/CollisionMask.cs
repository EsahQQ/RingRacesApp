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
        public CollisionMask(string path) // Загрузить коллизионную карту по указанному пути
        {
            if (!System.IO.File.Exists(path))
                throw new Exception("Файл коллизионной карты не найден: " + path);
            collisionBitmap = new Bitmap(path);
        }
        
        public bool IsDrivable(int x, int y) // Проверяет, можно ли ехать в точке (x,y).
        {
            if (x < 0 || y < 0 || x >= collisionBitmap.Width || y >= collisionBitmap.Height) // Если вышли за пределы изображения – считаем область недопустимой
                return false;
            Color pixel = collisionBitmap.GetPixel(x, y);
            return pixel.R < 100 && pixel.G < 100 && pixel.B < 100; // Здесь считаем, что если пиксель «яркий» (например, значение R > 200), то это проходимая область.
        }

        public bool CheckCollision(Car car) // Метод для проверки коллизии автомобиля по его углам (corners)
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
