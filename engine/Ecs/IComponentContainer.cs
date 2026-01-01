namespace TinyEngine.Ecs;

public interface IComponentContainer
{
    void RemoveEntity(EntityId entityId);
}

public interface IComponentJoin
{
    void EntityRemoved(EntityId entityId);
}
