using OpenTK;
using System.Windows.Forms;
using System;
using RingRaceLab.Game;

namespace RingRaceLab
{
    /// <summary>
    /// Приз, применяющий эффект ускорения к автомобилю.
    /// </summary>
    public class SpeedBoostPrize : IPrize
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
        /// Инициализирует приз ускорения.
        /// </summary>
        /// <param name="position">Позиция приза.</param>
        public SpeedBoostPrize(Vector2 position)
        {
            Position = position;
            try
            {
                TextureId = TextureCache.GetTexture("sprites/speed_prize.png");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки текстуры: {ex.Message}");
                TextureId = -1; // Используйте значение по умолчанию
            }
        }

        /// <summary>
        /// Применяет эффект ускорения к машине.
        /// </summary>
        /// <param name="car">Машина, к которой применяется эффект.</param>
        public void ApplyEffect(Car car)
        {
            car.ApplyDecorator(new SpeedBoostDecorator(car, 1.5f, 5f));
        }
    }
}