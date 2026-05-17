using OpenTK.Mathematics;
using XEngine.Core.Base;
using XEngine.Core.Scenery;
using XEngine.Core.Utils.JSONConverters;

namespace WinFormsUI.Game.Scenes.LOCs
{
    public class PlatformLOC : BaseLOC
    {
        [JsonVector2] public Vector2 Size { get; set; } = Vector2.One;
        public override Entity Spawn(GScene scene) => LevelElementsFabctory.CreatePlatform(scene, Pos, Size, Rotation);
    }
}
