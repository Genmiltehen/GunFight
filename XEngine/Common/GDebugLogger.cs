using System.Diagnostics;
using XEngine.Core.Base;

namespace XEngine.Core.Common
{
    public sealed class GDebugLogger : GameComponent, IDisposable
    {
        public void Dispose()
        {
            Debug.WriteLine(Owner.Id);
        }
    }
}
