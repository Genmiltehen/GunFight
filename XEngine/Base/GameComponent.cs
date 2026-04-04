namespace XEngine.Core.Base
{
    public abstract class GameComponent
    {
        public bool IsDeleted { get; private set; } = false;
        public Entity Owner { get; internal set; }
    }
}
