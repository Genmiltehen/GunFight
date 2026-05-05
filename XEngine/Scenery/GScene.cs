using Box2D.NET;
using System.Diagnostics;
using XEngine.Core.Base;
using XEngine.Core.Box2DCompat.Components;
using XEngine.Core.Common;
using XEngine.Core.Graphics;
using XEngine.Core.Input;
using XEngine.Core.Utils;

namespace XEngine.Core.Scenery
{
    public class GScene
    {
        protected int _id = 0;

        private readonly Dictionary<int, Entity> _entities = [];
        private readonly List<IGameSystem> _systems = [];
        private readonly List<Action> actions = [];
        private readonly List<GameTimer> _timers = [];

        public readonly IInputService Input;
        public readonly IAssetLoader Assets;
        public readonly Random Random;
        public float GetRandomFloat() => (float)Random.NextDouble();
        public GCamera Camera { get; }
        public GBox2DWorld World { get; }

        public GScene(GameEngine _engine)
        {
            Assets = _engine.Assets;
            Input = _engine.Input;
            Random = new Random();

            var _cam = SpawnEntity();
            Camera = _cam.AddComponent<GCamera>();
            _cam.Transform.Init(new(0, 0, 30), 0);

            var _wld = SpawnEntity();
            World = _wld.AddComponent<GBox2DWorld>().Init(pixelPerMetre: 16, gravity: new B2Vec2(0, -18));
        }

        public Entity SpawnEntity()
        {
            int id = _id++;
            var _e = new Entity(id, this);
            _entities[id] = _e;
            return _e;
        }

        #region System
        public void AddSystem(IGameSystem system)
        {
            _systems.Add(system);
            _systems.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }
        public void RemoveSystem(IGameSystem system)
        {
            _systems.Remove(system);
        }
        #endregion

        #region Context
        public virtual void Load() { }
        public void Unload()
        {
            World.Dispose();
        }
        #endregion

        #region Timer
        public void RegisterTimer(GameTimer timer)
        {
            if (!_timers.Contains(timer)) _timers.Add(timer);
        }
        public void RemoveTimer(GameTimer timer)
        {
            _timers.Remove(timer);
        }
        #endregion

        #region Update
        public void Update(float _dt)
        {
            UpdateTimers(_dt);
            InvokeSystems(_dt);
            InvokeEvents();
            RemoveDeleted();
        }
        private void UpdateTimers(float _dt)
        {
            foreach (var timer in _timers) timer.Tick(_dt);
        }
        private void InvokeSystems(float _dt)
        {
            foreach (var _s in _systems) if (_s.IsEnabled) _s.Update(this, _dt);
        }
        public void Schedule(Action action)
        {
            actions.Add(action);
        }
        private void InvokeEvents()
        {
            foreach (var action in actions) action.Invoke();
            actions.Clear();
        }
        private void RemoveDeleted()
        {
            var _removedEntityId = _entities
                .Where(kvp => kvp.Value._isDeleted)
                .Select(kvp => kvp.Key);
            foreach (var key in _removedEntityId)
            {
                if (_entities.TryGetValue(key, out var value))
                {
                    value.Transform.SetParent(null);
                    value.Dispose();
                    _entities.Remove(key);
                }
            }
        }
        #endregion

        #region Query
        public IEnumerable<Entity> IterateByIds(IEnumerable<int> ids)
        {
            foreach (var id in ids) yield return _entities[id];
        }
        public IEnumerable<Entity> Query(Predicate<Entity> _predicate)
        {
            foreach (var _e in _entities.Values) if (_predicate(_e)) yield return _e;
        }
        public IEnumerable<(Entity, T1)> Query<T1>(Func<Entity, T1, bool>? _predicate = null)
            where T1 : GameComponent
        {
            foreach (var _e in _entities.Values)
            {
                if (_e.TryGet<T1>(out T1 comp1))
                {
                    if (_predicate != null && !_predicate(_e, comp1)) continue;
                    yield return (_e, comp1);
                }
            }
        }
        public IEnumerable<(Entity, T1, T2)> Query<T1, T2>(Func<Entity, T1, T2, bool>? _predicate = null)
            where T1 : GameComponent
            where T2 : GameComponent
        {
            foreach (var _e in _entities.Values)
            {
                if (_e.TryGet<T1>(out T1 comp1) && _e.TryGet<T2>(out T2 comp2))
                {
                    if (_predicate != null && !_predicate(_e, comp1, comp2)) continue;
                    yield return (_e, comp1, comp2);
                }
            }
        }
        public IEnumerable<(Entity, T1, T2, T3)> Query<T1, T2, T3>(Func<Entity, T1, T2, T3, bool>? _predicate = null)
            where T1 : GameComponent
            where T2 : GameComponent
            where T3 : GameComponent
        {
            foreach (var _e in _entities.Values)
            {
                if (_e.TryGet<T1>(out T1 comp1) && _e.TryGet<T2>(out T2 comp2) && _e.TryGet<T3>(out T3 comp3))
                {
                    if (_predicate != null && !_predicate(_e, comp1, comp2, comp3)) continue;
                    yield return (_e, comp1, comp2, comp3);
                }
            }
        }
        #endregion
    }
}
