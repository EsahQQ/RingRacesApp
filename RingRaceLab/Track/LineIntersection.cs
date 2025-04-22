using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RingRaceLab
{
    public static class LineIntersection
    {
        public static bool CheckLineCrossing(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
            float denominator = (a1.X - a2.X) * (b1.Y - b2.Y) - (a1.Y - a2.Y) * (b1.X - b2.X);
            if (denominator == 0) return false;

            float t = ((a1.X - b1.X) * (b1.Y - b2.Y) - (a1.Y - b1.Y) * (b1.X - b2.X)) / denominator;
            float u = -((a1.X - a2.X) * (a1.Y - b1.Y) - (a1.Y - a2.Y) * (a1.X - b1.X)) / denominator;

            return t >= 0 && t <= 1 && u >= 0 && u <= 1;
        }
    }
}
