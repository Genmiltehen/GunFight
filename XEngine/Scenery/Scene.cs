using XEngine.Core.Base;
using XEngine.Core.Common;
using XEngine.Core.Graphics;
using XEngine.Core.Input;

namespace XEngine.Core.Scenery
{
    public class Scene
    {
        protected int _id = 0;
        protected readonly IAssetLoader Assets;
        protected InputService Input;

        private readonly Dictionary<int, Entity> _entities = [];
        private readonly List<IGameSystem> _systems = [];
        public CameraComp Camera { get; private set; }

        public Scene(GameEngine _engine)
        {
            Assets = _engine.Assets;
            Input = _engine.Input;

            var _cam = CreateEntity();
            Camera = _cam.AddComponent<CameraComp>();
            _cam.AddComponent<TransformComp>();
        }

        public Entity CreateEntity()
        {
            int id = _id++;
            var _e = new Entity(id);
            _entities[id] = _e;
            return _e;
        }

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
            foreach (var entity in _entities.Values) if (_predicate(entity)) yield return entity;
        }
        public IEnumerable<(Entity, T1)> Query<T1>(Predicate<Entity>? _predicate = null)
            where T1 : GameComponent
        {
            foreach (var entity in _entities.Values)
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
            foreach (var entity in _entities.Values)
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
            foreach (var _e in _entities.Values)
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
