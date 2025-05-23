using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RingRaceLab.Prize
{
    /// <summary>
    /// Фабрика для создания призов топлива.
    /// </summary>
    public class FuelPrizeFactory : PrizeFactory
    {
        /// <summary>
        /// Создает приз топлива.
        /// </summary>
        /// <param name="position">Позиция приза.</param>
        /// <returns>Приз топлива.</returns>
        public override IPrize CreatePrize(Vector2 position) => new FuelPrize(position);
    }
}
