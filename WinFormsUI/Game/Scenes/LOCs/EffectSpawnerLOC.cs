using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormsUI.Game.Player.Stats;
using WinFormsUI.Game.Player.Stats.Effects;
using XEngine.Core.Base;
using XEngine.Core.Common.Sprite;
using XEngine.Core.Scenery;

namespace WinFormsUI.Game.Scenes.LOCs
{
    internal class EffectSpawnerLOC : BaseLOC
    {
        public string Effect { get; set; } = "";
        public float Intensity { get; set; } = 0;
        public float Duration { get; set; } = 0;
        public float ColliderSize { get; set; } = 0;
        public string TexturePath { get; set; } = "";

        public override Entity Spawn(GScene scene)
        {
            var effect = GetEffect(Effect).Duration(Duration);
            var effEntity = LevelElementsFabctory.CreateEffect(scene, Pos, ColliderSize, effect);
            effEntity.AddComponent<GSprite>()
                .SetSizingPolicy(SizingPolicy.Source)
                .SetTexture(scene.Assets.LoadTexture(TexturePath));
            return effEntity;
        }

        private Effect GetEffect(string type) => type switch
        {
            "jumpBoost" => new JumpBoostEffect(Intensity),
            "speedBoost" => new SpeedBoostEffect(Intensity),
            "defenceBoost" => new SpeedBoostEffect(1), // TODO
            _ => new SpeedBoostEffect(1)
        };
    }
}
