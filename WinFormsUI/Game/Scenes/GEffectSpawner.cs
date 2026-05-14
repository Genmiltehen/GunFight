using WinFormsUI.Game.Player.Stats;
using WinFormsUI.Game.Player.Stats.Effects;
using XEngine.Core.Base;
using XEngine.Core.Common;
using XEngine.Core.Common.Sprite;

namespace WinFormsUI.Game.Scenes
{
    internal class GEffectSpawner : GameComponent
    {
        private static readonly string[] effects = ["jumpBoost", "speedBoost", "defenceBoost"];
        public GameTimer SpawnTimer = new(20, true);

        public GEffectSpawner Init()
        {
            Owner.Scene.RegisterTimer(SpawnTimer);
            SpawnTimer.OnComplete += SpawnRandom;
            SpawnTimer.Start();
            SpawnTimer.ForceEnd();
            return this;
        }

        private void SpawnRandom()
        {
            var scene = Owner.Scene;
            scene.Schedule(() =>
            {
                var effId = effects[scene.Random.Next(effects.Length)];
                var (effect, tex) = GetEffect(effId);
                effect.Duration(GetDuration());
                var effEntity = LevelElementsFabctory.CreateEffect(scene, Owner.Transform.Position, 1, effect);
                effEntity.AddComponent<GSprite>()
                    .SetSizingPolicy(SizingPolicy.Source)
                    .SetTexture(scene.Assets.LoadTexture(tex));
            });
        }

        private float GetIntesity() => float.Lerp(1.5f, 2.5f, Owner.Scene.GetR01());
        private float GetDuration() => float.Lerp(5f, 10f, Owner.Scene.GetR01());

        private (Effect, string) GetEffect(string type) => type switch
        {
            "jumpBoost" => (new JumpBoostEffect(GetIntesity()), "Effects\\JumpBoost.png"),
            "speedBoost" => (new SpeedBoostEffect(GetIntesity()), "Effects\\SpeedBoost.png"),
            "defenceBoost" => (new ArmorBoostEffect(GetIntesity()), "Effects\\DefenceBoost.png"), // TODO
            _ => (new SpeedBoostEffect(GetIntesity()), "Effects\\SpeedBoost.png")
        };

        public void Dispose()
        {
            SpawnTimer.OnComplete -= SpawnRandom;
            Owner.Scene.UnregisterTimer(SpawnTimer);
        }
    }
}
