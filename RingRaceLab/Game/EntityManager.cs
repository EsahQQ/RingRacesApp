using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RingRaceLab.Game
{
    public class EntityManager
    {
        public readonly List<GameEntity> _entities = new List<GameEntity>();

        public void AddEntity(GameEntity entity)
        {
            _entities.Add(entity);
        }

        public void RemoveEntity(GameEntity entity)
        {
            _entities.Remove(entity);
        }

        public IEnumerable<GameEntity> GetEntities()
        {
            return _entities;
        }
    }
}
