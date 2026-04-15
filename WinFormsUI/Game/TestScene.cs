using OpenTK.Mathematics;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using XEngine.Core;
using XEngine.Core.Base;
using XEngine.Core.Common;
using XEngine.Core.Common.Sprite;
using XEngine.Core.Graphics.OpenGL;
using XEngine.Core.Physics.Collision.Detection;
using XEngine.Core.Physics.Collision.Shapes;
using XEngine.Core.Physics.Components;
using XEngine.Core.Physics.Dynamics;
using XEngine.Core.Scenery;

namespace WinFormsUI.Game
{
    public class TestScene(GameEngine _engine) : Scene(_engine)
    {
        public Texture2D _texture;

        public override void Load()
        {
            _texture = Assets.LoadTexture("Test/test.png");

            AddSystem(new TestSystem(Input));
            AddSystem(new IntegrationSystem(new Vector2(0, -500)));
            AddSystem(new CollisionSystem(new SpatialGrid(25)));

            Camera.Zoom = 0.3f;

            TransformComp tr;
            Collider cc;
            Rigidbody rb;

            float r = 50;
            float W = 300;
            float H = 600;
            Vector2 ver = new Vector2(0, H * 2) + new Vector2(2 * r, 2 * r);
            Vector2 hor = new Vector2(W * 2, 0) + new Vector2(2 * r, 2 * r);

            GenerateRigidbody(out tr, out cc, out rb);
            //.AddComponent<SpriteComp>()
            //.Init(_texture)
            //.SetSize(hor);
            tr.Init(new Vector3(0, -H, 0), 0f, Vector2.One);
            cc.Init(new BoxCollider(hor), Vector2.Zero);
            rb.SetStatic().GravityScale = 0;

            //GenerateRigidbody(out tr, out cc, out rb);
            //tr.Init(new Vector3(0, H, 0), 0f, Vector2.One);
            //cc.Init(new CapsuleCollider(2 * W, r), Vector2.Zero);
            //rb.SetStatic().GravityScale = 0;

            GenerateRigidbody(out tr, out cc, out rb);
            //.AddComponent<SpriteComp>()
            //.Init(_texture)
            //.SetSize(ver);
            tr.Init(new Vector3(-W, 0, 0), 0, Vector2.One);
            cc.Init(new BoxCollider(ver), Vector2.Zero);
            rb.SetStatic().GravityScale = 0;

            GenerateRigidbody(out tr, out cc, out rb);
            //.AddComponent<SpriteComp>()
            //.Init(_texture)
            //.SetSize(ver);
            tr.Init(new Vector3(W, 0, 0), 0, Vector2.One);
            cc.Init(new BoxCollider(ver), Vector2.Zero);
            rb.SetStatic().GravityScale = 0;

            Vector2 size = new(100, 100);
            Vector2 broad = new(225, 100);
            SpawnTest(new Vector2(-125, -500), broad);
            SpawnTest(new Vector2(125, -500), broad);
            var e = SpawnTest(new Vector2(0, -400), size, MathF.PI / 4);
            e.Get<Rigidbody>()!.InvInertia = 0;
            SpawnTest(new Vector2(0, -300), size);
            SpawnTest(new Vector2(0, -200), size);
            SpawnTest(new Vector2(0, -100), size);
            SpawnTest(new Vector2(0, 0), size);
            e = SpawnTest(new Vector2(0, 200), broad, 0);
            //e.AddComponent<SpriteComp>()
            //.Init(_texture)
            //.SetSize(broad);
            var erb = e.Get<Rigidbody>()!;
            erb.InvMass = 0;
            erb.AddForceAtPoint(new Vector2(0, -200), new Vector2(1000, 0));



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

        private Entity SpawnTest(Vector2 pos, Vector2 size, float angle = 0)
        {
            //var ang = 0.0f;
            //var ang = MathF.PI / 2;
            var mass = 1f;
            var col = new BoxCollider(size);
            var moi = col.UnitMassMoI * mass;

            var e = GenerateRigidbody(out TransformComp tr, out Collider cc, out Rigidbody rb);
            //.AddComponent<SpriteComp>()
            //.Init(_texture)
            //.SetSize(size);
            tr.Init(new Vector3(pos.X, pos.Y, 0), angle, Vector2.One);
            cc.Init(col, Vector2.Zero);
            rb.Init(mass, moi).GravityScale = 1;
            //rb.InvInertia = 0;
            return e;
        }

        private Entity GenerateRigidbody(out TransformComp tr, out Collider cc, out Rigidbody rb)
        {
            Entity entity = CreateEntity();
            tr = entity.AddComponent<TransformComp>();
            cc = entity.AddComponent<Collider>();
            rb = entity.AddComponent<Rigidbody>();
            return entity;
        }
    }
}
