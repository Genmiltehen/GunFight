using OpenTK.Mathematics;
using System.Diagnostics;
using XEngine.Core;
using XEngine.Core.Base;
using XEngine.Core.Common;
using XEngine.Core.Common.Sprite;
using XEngine.Core.Physics;
using XEngine.Core.Physics.ColliderShapes;
using XEngine.Core.Physics.CollisionDetector;
using XEngine.Core.Physics.Systems;
using XEngine.Core.Physics.Utils;
using XEngine.Core.Scenery;

namespace WinFormsUI.Game
{
    public class TestScene(GameEngine _engine) : Scene(_engine)
    {
        public override void Load()
        {
            AddSystem(new TestSystem(Input));
            AddSystem(new IntegrationSystem(new Vector2(0, -1000)));
            AddSystem(new CollisionSystem(new SpatialGridCollisionDetector(200)));

            Camera.Zoom = 0.3f;

            TransformComp tr;
            ColliderComp cc;
            RigidbodyComp rb;

            float r = 50;
            float W = 500;
            float H = 600;

            GenerateRigidbody(out tr, out cc, out rb).AddComponent<PlayerTag>();
            tr.Init(new Vector3(0, -H, 0), 0f, Vector2.One);
            cc.Init(new CapsuleCollider(2 * W, r), Vector2.Zero);
            rb.SetStatic().GravityScale = 0;

            GenerateRigidbody(out tr, out cc, out rb);
            tr.Init(new Vector3(0, H, 0), 0f, Vector2.One);
            cc.Init(new CapsuleCollider(2 * W, r), Vector2.Zero);
            rb.SetStatic().GravityScale = 0;

            GenerateRigidbody(out tr, out cc, out rb);
            tr.Init(new Vector3(-W, 0, 0), MathF.PI / 2, Vector2.One);
            cc.Init(new CapsuleCollider(2 * H, r), Vector2.Zero);
            rb.SetStatic().GravityScale = 0;

            GenerateRigidbody(out tr, out cc, out rb);
            tr.Init(new Vector3(W, 0, 0), MathF.PI / 2, Vector2.One);
            cc.Init(new CapsuleCollider(2 * H, r), Vector2.Zero);
            rb.SetStatic().GravityScale = 0;

            SpawnTests(20);

            //Entity entity2 = CreateEntity();
            //entity2.AddComponent<TransformComp>()
            //    .Init(new Vector3(0, 0, 0), 0f, Vector2.One);
            //entity2.AddComponent<ColliderComp>()
            //    .Init(new CapsuleCollider(l, r), Vector2.Zero);
            //RigidbodyComp rb2 = entity2.AddComponent<RigidbodyComp>().Init(1, 1);
            //rb2.GravityScale = 0;



            //rb.AddForce(new Vector2(1000, 5000));

            //Entity entity2 = CreateEntity("Attach");
            //entity2.AddComponent<TransformComp>()
            //    .Init(new Vector3(100, 100, 0.5f), 0.5f, Vector2.One)
            //    .SetParent(entity1.Get<TransformComp>());
            //entity2.AddComponent<SpriteComp>().Init(Assets.LoadTexture("Test/test.png"));
        }

        private void SpawnTests(int n)
        {
            TransformComp tr;
            ColliderComp cc;
            RigidbodyComp rb;

            for (int i = 0; i < n; i++)
            {
                float y = (float)i * 50;

                //var ang = 0.5f;
                var ang = MathF.PI / 2;
                var mass = 10f;
                var col = new CapsuleCollider(100, 20);
                var moi = col.UnitMassMoI * mass;

                GenerateRigidbody(out tr, out cc, out rb);
                tr.Init(new Vector3(0, y - 200, 0), ang, Vector2.One);
                cc.Init(col, Vector2.Zero);
                rb.Init(mass, moi).GravityScale = 1;
                //rb.InvInertia = 0;
            }
        }

        private Entity GenerateRigidbody(out TransformComp tr, out ColliderComp cc, out RigidbodyComp rb)
        {
            Entity entity = CreateEntity();
            tr = entity.AddComponent<TransformComp>();
            cc = entity.AddComponent<ColliderComp>();
            rb = entity.AddComponent<RigidbodyComp>();
            return entity;
        }
    }
}
