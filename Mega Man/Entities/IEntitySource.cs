namespace MegaMan.Engine.Entities
{
    public interface IEntitySource
    {
        GameEntity GetOriginalEntity(string name);
    }
}
