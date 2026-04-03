using GameEngineLib.Impl.SceneImpl;

namespace GameEngineLib.Impl
{
    public interface IGameSystem
    {
        public int Priority { get; }
        void Update (Scene _scene, float _dt);
    }
}
