using Box2D.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormsUI.Game.Player;
using XEngine.Core.Base;

namespace WinFormsUI.Game.Scenes.PlayerSpawner
{
    public class GPlayerSpawner : GameComponent
    {
        public string Name { get; private set; } = "";

        public GPlayerSpawner Init(string name)
        {
            Name = name;
            return this;
        }

        public Entity Spawn(string playerId)
        {
            Owner.MarkDelete();
            B2Vec2 pos = new(Owner.Transform.Position2D.X, Owner.Transform.Position2D.Y);
            return PlayerFactory.Instance.CreatePlayer(Owner.Scene, pos, Name, playerId);
        }
    }
}
