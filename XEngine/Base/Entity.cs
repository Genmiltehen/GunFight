using System.Diagnostics;
using XEngine.Core.Common;
using XEngine.Core.Scenery;

namespace XEngine.Core.Base
{
    public sealed class Entity : System.IDisposable
    {
        public int Id { get; private set; }
        public GTransform Transform { get; private set; }
        public GScene Scene { get; private set; }

        internal bool _isDeleted = false;
        private readonly Dictionary<Type, GameComponent> _components = [];

        internal Entity(int id, GScene scene)
        {
            Id = id;
            Scene = scene;
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

        public void MarkDelete(bool RemoveChildren = false)
        {
            _isDeleted = true;

            List<Entity> children = [];
            Entity? current = Transform.FirstChild?.Owner;
            while (current != null)
            {
                children.Add(current);
                current = current.Transform.NextSibling?.Owner;
            }

            foreach (var child in children)
            {
                if (RemoveChildren) child.MarkDelete(true);
                else
                {
                    child.Transform.Position2D = child.Transform.RelativePosition2D;
                    child.Transform.SetParent(null);
                }
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
