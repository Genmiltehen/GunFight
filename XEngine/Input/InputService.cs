using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XEngine.Core.Input
{
    public class InputService
    {
        private readonly Dictionary<Keys, string> _actionMap = [];
        private readonly HashSet<string> _activeActions = [];

        public InputService(Control control)
        {
            control.KeyDown += OnKeyDown;
            control.KeyUp += OnKeyUp;
        }

        public void BindAction(Keys key, string actionName)
        {
            _actionMap[key] = actionName;
        }

        public bool IsActionActive(string actionName)
        {
            return _activeActions.Contains(actionName);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (_actionMap.TryGetValue(e.KeyCode, out var actionName)) _activeActions.Add(actionName);
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (_actionMap.TryGetValue(e.KeyCode, out var actionName)) _activeActions.Remove(actionName);
        }
    }
}
