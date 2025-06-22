namespace MegaMan.Engine.Core.Entities
{
    public interface IEntitySource
    {
        GameEntity GetOriginalEntity(string name);
    }
}
