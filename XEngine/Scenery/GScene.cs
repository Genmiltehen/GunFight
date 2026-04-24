using Box2D.NET;
using XEngine.Core.Base;
using XEngine.Core.Box2DCompat.Components;
using XEngine.Core.Common;
using XEngine.Core.Graphics;
using XEngine.Core.Input;

namespace XEngine.Core.Scenery
{
    public class GScene
    {
        protected int _id = 0;

        private readonly Dictionary<int, Entity> _entities = [];
        private readonly List<IGameSystem> _systems = [];
        private readonly List<Action> actions = [];

        public readonly IInputService Input;
        public readonly IAssetLoader Assets;
        public GCamera Camera { get; private set; }
        public GBox2DWorld World { get; private set; }

        public GScene(GameEngine _engine)
        {
            Assets = _engine.Assets;
            Input = _engine.Input;

            var _cam = CreateEntity();
            Camera = _cam.AddComponent<GCamera>();
            _cam.AddComponent<GTransform>();

            var _wld = CreateEntity();
            World = _wld.AddComponent<GBox2DWorld>().Init(pixelPerMetre: 16, gravity: new B2Vec2(0, -18));
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

        public void RemoveSystem(IGameSystem system)
        {
            _systems.Remove(system);
        }

        public void Schedule(Action action)
        {
            actions.Add(action);
        }



        public virtual void Load() { }
        public void Unload()
        {
            World.Dispose();
        }

        // -- Update Cycle ---

        public void Update(float _dt)
        {
            InvokeSystems(_dt);
            InvokeEvents();
            RemoveDeleted();
        }

        private void InvokeSystems(float _dt)
        {
            foreach (var _s in _systems) if (_s.IsEnabled) _s.Update(this, _dt);
        }

        private void InvokeEvents()
        {
            foreach (var action in actions) action.Invoke();
        }

        private void RemoveDeleted()
        {
            var _removedEntityId = _entities
                .Where(kvp => kvp.Value.IsDeleted)
                .Select(kvp => kvp.Key);
            foreach (var key in _removedEntityId)
            {
                if (_entities.TryGetValue(key, out var value))
                {
                    value.Dispose();
                    _entities.Remove(key);
                }
            }
        }

        // --- Queries ---

        public IEnumerable<Entity> IterateByIds(IEnumerable<int> ids)
        {
            foreach (var id in ids) yield return _entities[id];
        }

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
