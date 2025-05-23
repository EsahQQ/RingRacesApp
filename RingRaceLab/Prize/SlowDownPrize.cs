using OpenTK;
using System.Windows.Forms;
using System;
using RingRaceLab.Game;

namespace RingRaceLab
{
    /// <summary>
    /// Приз, применяющий эффект замедления к автомобилю.
    /// </summary>
    public class SlowDownPrize : IPrize
    {
        /// <summary>
        /// Позиция приза.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// ID текстуры приза.
        /// </summary>
        public int TextureId { get; }

        /// <summary>
        /// Инициализирует приз замедления.
        /// </summary>
        /// <param name="position">Позиция приза.</param>
        public SlowDownPrize(Vector2 position)
        {
            Position = position;
            try
            {
                TextureId = TextureCache.GetTexture("sprites/slow_prize.png");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки текстуры: {ex.Message}");
                TextureId = -1; // Используйте значение по умолчанию
            }
        }

        /// <summary>
        /// Применяет эффект замедления к машине.
        /// </summary>
        /// <param name="car">Машина, к которой применяется эффект.</param>
        public void ApplyEffect(Car car)
        {
            car.ApplyDecorator(new SlowDownDecorator(car, 0.5f, 5f));
        }
    }
}