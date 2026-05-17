using XEngine.Core.Base;
using XEngine.Core.Scenery;

namespace WinFormsUI.Game.Scenes.LOCs
{
    internal class EffectSpawnerLOC : BaseLOC
    {
        public override Entity Spawn(GScene scene) => LevelElementsFabctory.CreateEffectSpawner(scene, Pos);
    }
}
