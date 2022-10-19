using System.Collections.Generic;

namespace Gravity
{
    public sealed class EntityManager
    {
        private static EntityManager? instance = null;

        private static readonly List<Entity> entities;
        private static readonly List<Entity> pendingEntities;

        private static bool updatingEntities = false;

        private EntityManager()
        {
        }

        public static EntityManager Instance
        {
            get
            {
                instance ??= new EntityManager();
                return instance;
            }
        }

        public IReadOnlyCollection<Entity> AllEntities
        {
            get { return entities; }
        }

        public void AddEntity(Entity entity)
        {
            if (updatingEntities)
                AddOrderedEntity(entity, pendingEntities);
            else
                AddOrderedEntity(entity, entities);
        }

        public void RemoveEntity(Entity entity)
        {
            for (var i = pendingEntities.Count - 1; i >= 0; i--)
            {
                if (pendingEntities[i] == entity)
                    pendingEntities.RemoveAt(i);
            }

            for (var i = entities.Count - 1; i >= 0; i--)
            {
                if (entities[i] == entity)
                    entities.RemoveAt(i);
            }
        }

        private void AddOrderedEntity(Entity entity, List<Entity> collection)
        {
            var order = entity.UpdateOrder;
            var i = 0;
            while (i < collection.Count && order > collection[i].UpdateOrder)
            {
                i++;
            }
            collection.Insert(i, entity);
        }
    }
}
