using XEngine.Core.Base;
using XEngine.Core.Scenery;

namespace WinFormsUI.Game.Scenes.LOCs
{
    internal class WeaponSpawnerLOC : BaseLOC
    {
        public override Entity Spawn(GScene scene)
        {
            return LevelElementsFabctory.CreateWeaponSpawner(scene, Pos);
        }
    }
}
