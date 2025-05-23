using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RingRaceLab.Prize
{
    /// <summary>
    /// Фабрика для создания призов ускорения.
    /// </summary>
    public class SpeedBoostPrizeFactory : PrizeFactory
    {
        /// <summary>
        /// Создает приз ускорения.
        /// </summary>
        /// <param name="position">Позиция приза.</param>
        /// <returns>Приз ускорения.</returns>
        public override IPrize CreatePrize(Vector2 position) => new SpeedBoostPrize(position);
    }
}
