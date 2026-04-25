using XEngine.Core.Common;

namespace XEngine.Core.Base
{
    public sealed class Entity : IDisposable
    {
        public int Id { get; private set; }
        public GTransform Transform { get; private set; }
        private readonly Dictionary<Type, GameComponent> _components = [];
        internal bool _isDeleted = false;

        internal Entity(int id)
        {
            Id = id;
            Transform = AddComponent<GTransform>();
        }

        public T AddComponent<T>() where T : GameComponent, new()
        {
            T component = new() { Owner = this };
            _components[typeof(T)] = component;
            return component;
        }

        public bool Has<T>() where T : GameComponent => _components.ContainsKey(typeof(T));

        public T? Get<T>() where T : GameComponent => _components.TryGetValue(typeof(T), out var c) ? (T)c : default;

        public bool TryGet<T>(out T comp) where T : GameComponent
        {
            if (!Has<T>())
            {
                comp = null!;
                return false;
            }
            comp = (T)_components[typeof(T)];
            return true;
        }

        public void Remove()
        {
            _isDeleted = true;

            Entity? current = Transform.FirstChild?.Owner;
            while (current != null)
            {
                current.Remove();
                current = current.Transform.NextSibling?.Owner;
            }
        }

        public Entity? GetChild(int index)
        {
            return Transform.GetChild(index)?.Owner;
        }

        public void Dispose()
        {
            foreach (var pair in _components) (pair.Value as IDisposable)?.Dispose();
        }
    }
}
