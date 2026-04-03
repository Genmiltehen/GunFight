namespace GameEngineLib.Impl
{
    public abstract class GameComponent
    {
        public Entity Owner { get; internal set; }
    }
}
