using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RingRaceLab
{
    public class CollisionSystem
    {
        private readonly CollisionMask _collisionMask;

        public CollisionSystem(string collisionMapPath)
        {
            _collisionMask = new CollisionMask(collisionMapPath);
        }

        public bool CheckCollision(Car car)
        {
            return _collisionMask.CheckCollision(car);
        }
    }
}
