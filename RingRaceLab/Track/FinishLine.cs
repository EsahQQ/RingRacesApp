using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RingRaceLab
{
    /// <summary>
    /// Представляет финишную линию на трассе.
    /// </summary>
    public class FinishLine
    {
        private Vector2 _startPoint;
        private Vector2 _endPoint;

        /// <summary>
        /// Отслеживает пересечение линии.
        /// </summary>
        private bool _hasCrossed; // Для отслеживания пересечения

        /// <summary>
        /// Инициализирует финишную линию.
        /// </summary>
        /// <param name="start">Начальная точка линии.</param>
        /// <param name="end">Конечная точка линии.</param>
        public FinishLine(Vector2 start, Vector2 end)
        {
            _startPoint = start;
            _endPoint = end;
        }

        /// <summary>
        /// Проверяет, пересек ли объект финишную линию.
        /// </summary>
        /// <param name="previousPosition">Предыдущая позиция объекта.</param>
        /// <param name="currentPosition">Текущая позиция объекта.</param>
        /// <returns>1, если пересечено в прямом направлении; -1, если в обратном; 0, если не пересечено.</returns>
        public int CheckCrossing(Vector2 previousPosition, Vector2 currentPosition)
        {
            // Проверяем пересечение линии с помощью алгоритма пересечения отрезков
            if (LineIntersection.CheckLineCrossing( 
                    previousPosition,
                    currentPosition,
                    _startPoint,
                    _endPoint
                    ))
            {
                // Проверяем направление пересечения (например, по оси X)
                if (previousPosition.X < currentPosition.X) // Пример проверки направления
                {
                    return 1; // Пересечено в прямом направлении
                }
                else
                {
                    return -1; // Пересечено в обратном направлении
                }
            }
            return 0; // Не пересечено
        }
    }
}