using WinFormsUI.Game.Combat.Weapons;
using WinFormsUI.Game.Drop;
using XEngine.Core.Base;
using XEngine.Core.Common;

namespace WinFormsUI.Game.Scenes
{
    internal class GWeaponSpawner : GameComponent, IDisposable
    {
        public GameTimer SpawnTimer = new(20, true);
        public float ExpirationTime = 10;

        public GWeaponSpawner Init()
        {
            Owner.Scene.RegisterTimer(SpawnTimer);
            SpawnTimer.OnComplete += SpawnRandom;
            SpawnTimer.Start();
            SpawnTimer.ForceEnd();
            return this;
        }

        private void SpawnRandom()
        {
            Owner.Scene.Schedule(() =>
            {
                var l = WeaponFactory.Instance.GetIds().ToList();
                var id = l[Owner.Scene.Random.Next(l.Count)];
                if (WeaponFactory.Instance.TryCreateWeapon(id, out var w))
                {
                    w.Init(Owner.Scene);
                    DropBuilder.Init(w)
                        .SetExpirationTime(ExpirationTime)
                        .CanPickup(true)
                        .SetVelocity(new(0, 3), 0)
                        .Spawn(Owner.Scene, Owner.Transform.Position);
                }
            });
        }

        public void Dispose()
        {
            SpawnTimer.OnComplete -= SpawnRandom;
            Owner.Scene.UnregisterTimer(SpawnTimer);
        }
    }
}
