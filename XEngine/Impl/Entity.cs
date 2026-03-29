using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngineLib.Impl
{
    public class Entity
    {
        private readonly Dictionary<Type, IGameComponent> _components = [];
        public Entity(params IGameComponent[] Components)
        {
            foreach (var component in Components)
            {
                _components.Add(component.GetType(), component);
            }
        }

        public void AddComponent<T>(T _comp) where T : IGameComponent => _components.Add(typeof(T), _comp);

        public bool Has<T>() where T : IGameComponent => _components.ContainsKey(typeof(T));

        public T? Get<T>() where T : IGameComponent => _components.TryGetValue(typeof(T), out var c) ? (T)c : default;
    }
}
