using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RingRaceLab.Game
{
    /// <summary>
    /// Управляет коллекцией игровых сущностей (<see cref="GameEntity"/>) в игре.
    /// Предоставляет методы для добавления, удаления и получения сущностей.
    /// </summary>
    public class EntityManager
    {
        /// <summary>
        /// Коллекция всех игровых сущностей, управляемых менеджером.
        /// </summary>
        public readonly List<GameEntity> _entities = new List<GameEntity>();

        /// <summary>
        /// Добавляет указанную игровую сущность в коллекцию.
        /// </summary>
        /// <param name="entity">Сущность для добавления.</param>
        public void AddEntity(GameEntity entity)
        {
            _entities.Add(entity);
        }

        /// <summary>
        /// Удаляет указанную игровую сущность из коллекции.
        /// </summary>
        /// <param name="entity">Сущность для удаления.</param>
        public void RemoveEntity(GameEntity entity)
        {
            _entities.Remove(entity);
        }

        /// <summary>
        /// Возвращает перечислимую коллекцию всех игровых сущностей, управляемых менеджером.
        /// </summary>
        /// <returns>Коллекция сущностей.</returns>
        public IEnumerable<GameEntity> GetEntities()
        {
            return _entities;
        }
    }
}