using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RingRaceLab.Prize
{
    /// <summary>
    /// Фабрика для создания призов замедления.
    /// </summary>
    public class SlowDownPrizeFactory : PrizeFactory
    {
        /// <summary>
        /// Создает приз замедления.
        /// </summary>
        /// <param name="position">Позиция приза.</param>
        /// <returns>Приз замедления.</returns>
        public override IPrize CreatePrize(Vector2 position) => new SlowDownPrize(position);
    }
}
