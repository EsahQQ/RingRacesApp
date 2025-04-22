using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RingRaceLab
{
    public class FinishLine
    {
        private Vector2 _startPoint;
        private Vector2 _endPoint;
        private bool _hasCrossed; // Для отслеживания пересечения

        public FinishLine(Vector2 start, Vector2 end)
        {
            _startPoint = start;
            _endPoint = end;
        }

        public bool CheckCrossing(Vector2 previousPosition, Vector2 currentPosition)
        {
            // Проверяем пересечение линии с помощью алгоритма пересечения отрезков
            return LineIntersection.CheckLineCrossing(
                previousPosition,
                currentPosition,
                _startPoint,
                _endPoint
            );
        }
    }
}
