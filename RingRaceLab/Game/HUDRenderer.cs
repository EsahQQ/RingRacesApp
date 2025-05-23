using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace RingRaceLab.Game
{
    /// <summary>
    /// Класс для отрисовки пользовательского интерфейса (HUD) в игре,
    /// включая индикаторы топлива и эффектов призов для автомобилей.
    /// </summary>
    public class HUDRenderer
    {
        /// <summary>
        /// Словарь, содержащий загруженные текстуры для индикаторов по их ключам.
        /// </summary>
        private readonly Dictionary<string, int> _textures;

        /// <summary>
        /// Ширина области отрисовки HUD.
        /// </summary>
        private readonly int Width;

        /// <summary>
        /// Высота области отрисовки HUD.
        /// </summary>
        private readonly int Height;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="HUDRenderer"/>.
        /// </summary>
        /// <param name="textures">Словарь текстур, используемых для отрисовки индикаторов.</param>
        /// <param name="Width">Ширина области отрисовки.</param>
        /// <param name="Height">Высота области отрисовки.</param>
        public HUDRenderer(Dictionary<string, int> textures, int Width, int Height)
        {
            _textures = textures;
            this.Width = Width;
            this.Height = Height;
        }

        /// <summary>
        /// Отрисовывает элементы HUD для двух автомобилей, включая индикаторы топлива, скорости и замедления.
        /// </summary>
        /// <param name="car1">Первый автомобиль, для которого отрисовывается HUD.</param>
        /// <param name="car2">Второй автомобиль, для которого отрисовывается HUD.</param>
        public void DrawHUD(Car car1, Car car2)
        {
            // Получение уровней индикаторов для Car1
            int fuelLevel1 = GetFuelLevel(car1.Fuel);
            int speedLevel1 = GetDecoratorLevel(car1, true); // true для SpeedBoostDecorator
            int slowLevel1 = GetDecoratorLevel(car1, false); // false для SlowDownDecorator

            // Отрисовка индикаторов для Car1 (в левой части экрана)
            DrawIndicator(new Rectangle(Width / 384, Height / 216, Width / 12, Height / 27), $"fuel{fuelLevel1}");
            DrawIndicator(new Rectangle(Width / 384, (int)(Height / 21.6), Width / 12, Height / 27), $"speed{speedLevel1}");
            DrawIndicator(new Rectangle(Width / 384, (int)(Height / 11.36), Width / 12, Height / 27), $"slow{slowLevel1}");

            // Получение уровней индикаторов для Car2
            int fuelLevel2 = GetFuelLevel(car2.Fuel);
            int speedLevel2 = GetDecoratorLevel(car2, true);
            int slowLevel2 = GetDecoratorLevel(car2, false);

            // Отрисовка индикаторов для Car2 (в правой части экрана)
            DrawIndicator(new Rectangle(Width - (int)(Width / 11.63), Height / 216, Width / 12, Height / 27), $"fuel{fuelLevel2}");
            DrawIndicator(new Rectangle(Width - (int)(Width / 11.63), (int)(Height / 21.6), Width / 12, Height / 27), $"speed{speedLevel2}");
            DrawIndicator(new Rectangle(Width - (int)(Width / 11.63), (int)(Height / 11.36), Width / 12, Height / 27), $"slow{slowLevel2}");
        }

        /// <summary>
        /// Определяет уровень индикатора топлива на основе текущего количества топлива.
        /// </summary>
        /// <param name="fuel">Текущее количество топлива.</param>
        /// <returns>Уровень индикатора от 0 до 5.</returns>
        private int GetFuelLevel(float fuel)
        {
            if (fuel > 80) return 5;
            if (fuel > 60) return 4;
            if (fuel > 40) return 3;
            if (fuel > 20) return 2;
            if (fuel > 0) return 1;
            return 0;
        }

        /// <summary>
        /// Определяет уровень индикатора эффекта приза (скорость или замедление)
        /// на основе оставшегося времени действия эффекта.
        /// </summary>
        /// <param name="car">Автомобиль, для которого проверяется эффект.</param>
        /// <param name="isSpeed">Если true, проверяется эффект ускорения; если false, проверяется эффект замедления.</param>
        /// <returns>Уровень индикатора от 0 до 5, основанный на оставшихся секундах.</returns>
        private int GetDecoratorLevel(Car car, bool isSpeed)
        {
            if (car._currentDecorator == null) return 0;
            TimeSpan elapsed = DateTime.Now - car._currentDecorator.timerStartTime;
            int remaining = (int)((car._currentDecorator._timer.Interval - elapsed.TotalMilliseconds) / 1000) + 1; // Оставшиеся секунды

            if ((isSpeed && car._currentDecorator is SpeedBoostDecorator) ||
                (!isSpeed && car._currentDecorator is SlowDownDecorator)) // Предполагается наличие SlowDownDecorator
            {
                // Уровни индикаторов соответствуют оставшимся секундам до 5
                if (remaining >= 5) return 5;
                if (remaining == 4) return 4;
                if (remaining == 3) return 3;
                if (remaining == 2) return 2;
                if (remaining == 1) return 1;
                return 0; // Эффект закончился
            }
            return 0; // Другой тип декоратора или нет нужного эффекта
        }


        /// <summary>
        /// Отрисовывает один индикатор HUD в заданном прямоугольнике с использованием указанной текстуры.
        /// </summary>
        /// <param name="rect">Прямоугольник, в котором будет отрисован индикатор.</param>
        /// <param name="textureKey">Ключ текстуры индикатора в словаре текстур.</param>
        private void DrawIndicator(Rectangle rect, string textureKey)
        {
            // Получаем ID текстуры по ключу
            if (!_textures.TryGetValue(textureKey, out int textureId)) return; // Выходим, если текстура не найдена

            GL.Enable(EnableCap.Texture2D); // Включаем использование текстур
            GL.BindTexture(TextureTarget.Texture2D, textureId); // Привязываем нужную текстуру

            // Начинаем отрисовку квадрата (прямоугольника)
            GL.Begin(PrimitiveType.Quads);
            // Задаем текстурные координаты и координаты вершин для каждого угла прямоугольника
            GL.TexCoord2(0, 0); GL.Vertex2(rect.Left, rect.Top);       // Левый верхний угол текстуры к левому верхнему углу прямоугольника
            GL.TexCoord2(1, 0); GL.Vertex2(rect.Right, rect.Top);      // Правый верхний угол текстуры к правому верхнему углу прямоугольника
            GL.TexCoord2(1, 1); GL.Vertex2(rect.Right, rect.Bottom);   // Правый нижний угол текстуры к правому нижнему углу прямоугольника
            GL.TexCoord2(0, 1); GL.Vertex2(rect.Left, rect.Bottom);    // Левый нижний угол текстуры к левому нижнему углу прямоугольника
            GL.End(); // Заканчиваем отрисовку

            GL.Disable(EnableCap.Texture2D); // Выключаем использование текстур
        }
    }
}