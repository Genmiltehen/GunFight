using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormsUI.Game.Scenes.PlayerSpawner;
using XEngine.Core.Base;
using XEngine.Core.Scenery;

namespace WinFormsUI.Game.Scenes.LOCs
{
    internal class PlayerLOC : BaseLOC
    {
        public string Name { get; set; } = "NoneChar";

        public override Entity Spawn(GScene scene)
        {
            var spawner = scene.SpawnEntity();
            spawner.Transform.Init(Pos, 0);
            spawner.AddComponent<GPlayerSpawner>().Init(Name);
            return spawner;
        }
    }
}
