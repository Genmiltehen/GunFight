using OpenTK.Mathematics;
using XEngine.Core.Base;
using XEngine.Core.Scenery;
using XEngine.Core.Utils.JSONConverters;

namespace WinFormsUI.Game.Scenes.LOCs
{
    internal class BoxLOC : BaseLOC
    {
        [JsonVector2] public Vector2 Size { get; set; }

        public override Entity Spawn(GScene scene)
        {
            return LevelElementsFabctory.CreateBox(scene, Pos, Size, Rotation);
        }
    }
}
