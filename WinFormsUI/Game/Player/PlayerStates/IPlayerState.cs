using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XEngine.Core.Scenery;

namespace WinFormsUI.Game.Player.PlayerStates
{
    public interface IPlayerState
    {
        public string DebugName { get; }
        public void Enter(GPlayer player);
        public void ProcessInput(GPlayer player, GScene scene, float dt);
        public void Exit(GPlayer player);
    }
}
