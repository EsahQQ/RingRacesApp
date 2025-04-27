using OpenTK;
using System.Windows.Forms;
using System;
using RingRaceLab.Game;

namespace RingRaceLab
{
    public class FuelPrize : IPrize
    {
        public Vector2 Position { get; set; }
        public int TextureId { get; }

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
                TextureId = -1; // Используйте значение по умолчанию
            }
        }

        public void ApplyEffect(Car car)
        {
            car.Fuel += 50;
        }
    }
}