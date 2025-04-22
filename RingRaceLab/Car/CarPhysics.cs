using OpenTK;
using System;
using System.Collections.Generic;

namespace RingRaceLab
{
    public class CarPhysics
    {
        private readonly Vector2 _size;

        public CarPhysics(Vector2 size)
        {
            _size = size;
        }

        public List<Vector2> GetCorners(Vector2 position, float angle)
        {
            float rad = MathHelper.DegreesToRadians(angle);
            float cos = (float)Math.Cos(rad);
            float sin = (float)Math.Sin(rad);

            Vector2[] localCorners = {
                new Vector2(-_size.X, -_size.Y),
                new Vector2(_size.X, -_size.Y),
                new Vector2(_size.X, _size.Y),
                new Vector2(-_size.X, _size.Y)
            };

            List<Vector2> globalCorners = new List<Vector2>();
            foreach (var local in localCorners)
            {
                float rotatedX = local.X * cos - local.Y * sin;
                float rotatedY = local.X * sin + local.Y * cos;
                globalCorners.Add(new Vector2(
                    position.X + rotatedX,
                    position.Y + rotatedY
                ));
            }

            return globalCorners;
        }
    }
}