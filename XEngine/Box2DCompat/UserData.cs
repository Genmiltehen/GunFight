using XEngine.Core.Box2DCompat.Components;

namespace XEngine.Core.Box2DCompat
{
    public class UserData(GBox2DBody hostBoty)
    {
        public GBox2DBody HostBody = hostBoty;
    }
}
