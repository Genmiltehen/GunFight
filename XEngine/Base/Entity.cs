namespace XEngine.Core.Base
{
    public sealed class Entity : IDisposable
    {
        public bool IsDeleted { get; set; } = false;
        private readonly Dictionary<Type, GameComponent> _components = [];
        public int Id { get; private set; }

        internal Entity(int id)
        {
            Id = id;
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

        public void Dispose()
        {
            foreach (var pair in _components) (pair.Value as IDisposable)?.Dispose();
        }
    }
}
