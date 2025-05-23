using OpenTK;
using System.Windows.Forms;
using System;
using RingRaceLab.Game;

namespace RingRaceLab
{
    /// <summary>
    /// Приз, добавляющий топливо автомобилю.
    /// </summary>
    public class FuelPrize : IPrize
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
        /// Инициализирует приз топлива.
        /// </summary>
        /// <param name="position">Позиция приза.</param>
        public FuelPrize(Vector2 position)
        {
            Position = position;
            try
            {
                TextureId = TextureCache.GetTexture("sprites/fuel_prize.png");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки текстуры: {ex.Message}");
                TextureId = -1;
            }
        }

        /// <summary>
        /// Применяет эффект приза (добавляет топливо) к машине.
        /// </summary>
        /// <param name="car">Машина, к которой применяется эффект.</param>
        public void ApplyEffect(Car car)
        {
            car.Fuel = Math.Min(car.Fuel + 25, 100);
        }
    }
}