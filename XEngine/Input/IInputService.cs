using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XEngine.Core.Input
{
    public interface IInputService
    {
        public bool IsActionActive(string actionName);
        public bool IsActionJustActivated(string actionName);
        // IsActionJustDeactivated
        public float GetAxis(string axisName);
    }
}
