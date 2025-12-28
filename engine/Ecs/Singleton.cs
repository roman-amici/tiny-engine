namespace TinyEngine.Ecs;

public class Singleton<T> : IComponentContainer where T : class
{
    public Singleton()
    {
    }

    public void Spawn(EntityId entityId, T value)
    {
        if (Singlet != null)
        {
            throw new InvalidOperationException("Singleton already spawned");
        }

        EntityId = entityId;
        Singlet = value;
    }

    public EntityId? EntityId {get; private set;}
    public T? Singlet {get; private set;}


    public void RemoveEntity(EntityId entityId)
    {
        if (entityId == EntityId)
        {
            Singlet = null;
            EntityId = null;
        }
    }
}