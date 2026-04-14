namespace XEngine.Core.Base
{
    public class Entity
    {
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
    }
}
