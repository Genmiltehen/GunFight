using OpenTK.Mathematics;
using XEngine.Core.Base;
using XEngine.Core.Scenery;
using XEngine.Core.Utils.JSONConverters;

namespace WinFormsUI.Game.Scenes.LOCs
{
    public abstract class BaseLOC
    {
        [JsonVector3]
        public Vector3 Pos { get; set; } = Vector3.Zero;
        public float Rotation { get; set; } = 0;

        public abstract Entity Spawn(GScene scene);
    }
}
