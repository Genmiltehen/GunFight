using GameEngineLib.Impl.RenderImpl;

namespace GameEngineLib.Impl.SceneImpl
{
    public class Scene
    {
        protected readonly IAssetLoader AssetsLoader;

        private readonly List<Entity> _entities = [];
        private readonly List<IGameSystem> _systems = [];

        protected Scene(IAssetLoader assetsLoader)
        {
            AssetsLoader = assetsLoader;
        }

        public void AddEntity(Entity entity) => _entities.Add(entity);
        public void AddSystem(IGameSystem system)
        {
            _systems.Add(system);
            _systems.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }

        // TODO: add removers

        public virtual void Load() { }
        public virtual void Unload() { }

        public void Update(float _dt)
        {
            foreach (var _p in _systems) _p.Update(this, _dt);
        }

        // --- Queries ---

        public IEnumerable<Entity> Query(Predicate<Entity> _predicate)
        {
            foreach (var entity in _entities) if (_predicate(entity)) yield return entity;
        }
        public IEnumerable<(Entity, T1)> Query<T1>(Predicate<Entity>? _predicate = null)
            where T1 : GameComponent
        {
            foreach (var entity in _entities)
            {
                if (_predicate != null && !_predicate(entity)) continue;
                if (entity.Has<T1>())
                {
                    yield return (entity, entity.Get<T1>()!);
                }
            }
        }
        public IEnumerable<(Entity, T1, T2)> Query<T1, T2>(Predicate<Entity>? _predicate = null)
            where T1 : GameComponent
            where T2 : GameComponent
        {
            foreach (var entity in _entities)
            {
                if (_predicate != null && !_predicate(entity)) continue;

                if (entity.Has<T1>() && entity.Has<T2>())
                {
                    yield return (entity, entity.Get<T1>()!, entity.Get<T2>()!);
                }
            }
        }
        public IEnumerable<(Entity, T1, T2, T3)> Query<T1, T2, T3>(Predicate<Entity>? _predicate = null)
            where T1 : GameComponent
            where T2 : GameComponent
            where T3 : GameComponent
        {
            foreach (var _e in _entities)
            {
                if (_predicate != null && !_predicate(_e)) continue;

                if (_e.Has<T1>() && _e.Has<T2>() && _e.Has<T3>())
                {
                    yield return (_e, _e.Get<T1>()!, _e.Get<T2>()!, _e.Get<T3>()!);
                }
            }
        }
    }
}
