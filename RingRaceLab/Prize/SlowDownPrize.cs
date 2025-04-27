using OpenTK;
using System.Windows.Forms;
using System;
using RingRaceLab.Game;

namespace RingRaceLab
{
    public class SlowDownPrize : IPrize
    {
        public Vector2 Position { get; set; }
        public int TextureId { get; }

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

        public void ApplyEffect(Car car)
        {
            car.ApplyDecorator(new SpeedBoostDecorator(car, 0.5f, 5f));
        }
    }
}