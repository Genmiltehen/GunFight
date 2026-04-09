using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;
using System.Text;
using XEngine.Core.Config;
using XEngine.Core.Input.InputAxis;

namespace XEngine.Core.Input
{
    public class InputService
    {
        private readonly Dictionary<string, List<Keys>> _actionBindings = [];
        private readonly HashSet<Keys> _downKeys = [];
        private readonly Dictionary<string, Axis> _axes = [];

        public InputService() { }

        public void LoadBindingsFromConfig(GameConfig config)
        {
            foreach (var keyBinding in config.KeyMap) BindAction(keyBinding.Key, keyBinding.Value);
            foreach (var axisBinding in config.Axes) _axes[axisBinding.Key] = new Axis(axisBinding.Value);
        }

        public void Update(float dt)
        {
            foreach (var axis in _axes.Values) axis.Update(this, dt);
        }

        public void BindAction(string actionName, Keys key)
        {
            if (!_actionBindings.ContainsKey(actionName))
                _actionBindings[actionName] = [];

            _actionBindings[actionName].Add(key);
        }

        public void SetKeyDown(Keys key) => _downKeys.Add(key);
        public void SetKeyUp(Keys key) => _downKeys.Remove(key);
        public void ClearStates() => _downKeys.Clear();
        public float GetAxis(string axisName) => _axes.TryGetValue(axisName, out var a) ? a.Value : 0f;
        public bool IsActionActive(string actionName)
        {
            if (!_actionBindings.TryGetValue(actionName, out var keys))
                return false;
            foreach (var key in keys) if (_downKeys.Contains(key)) return true;
            return false;
        }
    }
}