using OpenTK.Mathematics;
using XEngine.Core.Base;
using XEngine.Core.Scenery;
using XEngine.Core.Utils.JSONConverters;

namespace WinFormsUI.Game.Scenes.LOCs
{
    internal class BoxLOC : BaseLOC
    {
        [JsonVector2] public Vector2 Size { get; set; } = Vector2.One;

        public override Entity Spawn(GScene scene) => LevelElementsFabctory.CreateBox(scene, Pos, Size, Rotation);
    }
}
