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
using XEngine.Core.Utils;

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

            float r = 50;
            float W = 1000;
            float H = 600;

            GenerateBoxEnc(r, W, H);

            var e = SpawnCapsule(new(0, 0), 75, 25, MathF.PI / 2);
            e.AddComponent<PlayerTag>();
            e.Get<Rigidbody>()!.InvInertia = 0;

            //foreach (var i in Enumerable.Range(0, 2))
            //{
            //    float y = 50f * i - 400;
            //    SpawnCapsule(new Vector2(0, y), 100, 20);
            //}
            //Vector2 size = new(100, 100);
            //Vector2 broad = new(225, 100);
            //SpawnBox(new Vector2(-125, -500), broad);
            //SpawnBox(new Vector2(125, -500), broad);
            //SpawnBox(new Vector2(0, 0), size, 0.5f);
            //var e = SpawnCapsule(new Vector2(0, -200), 100, 20, MathF.PI / 2);
            //e.Get<Rigidbody>()!.SetStatic();
            //e.Get<Rigidbody>()!.InvInertia = 0;
            //SpawnBox(new Vector2(0, -300), size, 0.5f);
            //SpawnBox(new Vector2(0, -200), size);
            //SpawnBox(new Vector2(0, -100), size);
            //SpawnBox(new Vector2(0, 0), size);
            //e = SpawnBox(new Vector2(0, 200), broad, 0);
            //e.AddComponent<SpriteComp>()
            //.Init(_texture)
            //.SetSize(broad);
            //var erb = e.Get<Rigidbody>()!;
            //erb.InvMass = 0;
            //erb.AddForceAtPoint(new Vector2(0, -200), new Vector2(1000, 0));
        }

        private Entity SpawnBox(Vector2 pos, Vector2 size, float angle = 0)
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

        private Entity SpawnCapsule(Vector2 pos, float l, float r, float angle = 0)
        {
            var mass = 1f;
            var col = new CapsuleCollider(l, r);
            var moi = col.UnitMassMoI * mass;

            var e = GenerateRigidbody(out TransformComp tr, out Collider cc, out Rigidbody rb);
            tr.Init(new Vector3(pos.X, pos.Y, 0), angle, Vector2.One);
            cc.Init(col, Vector2.Zero);
            rb.Init(mass, moi).GravityScale = 1;
            //rb.InvInertia = 0;
            return e;
        }

        private void GenerateBoxEnc(float r, float W, float H)
        {
            TransformComp tr;
            Collider cc;
            Rigidbody rb;

            Vector2 ver = new Vector2(0, H * 2) + new Vector2(2 * r, 2 * r);
            Vector2 hor = new Vector2(W * 2, 0) + new Vector2(2 * r, 2 * r);

            GenerateRigidbody(out tr, out cc, out rb);
            tr.Init(new Vector3(0, -H, 0), 0f, Vector2.One);
            cc.Init(new BoxCollider(hor), Vector2.Zero);
            rb.SetStatic().GravityScale = 0;

            GenerateRigidbody(out tr, out cc, out rb);
            tr.Init(new Vector3(-W, 0, 0), 0, Vector2.One);
            cc.Init(new BoxCollider(ver), Vector2.Zero);
            rb.SetStatic().GravityScale = 0;

            GenerateRigidbody(out tr, out cc, out rb);
            tr.Init(new Vector3(W, 0, 0), 0, Vector2.One);
            cc.Init(new BoxCollider(ver), Vector2.Zero);
            rb.SetStatic().GravityScale = 0;
        }

        private void GenerateCapsuleEnc(float r, float W, float H)
        {
            TransformComp tr;
            Collider cc;
            Rigidbody rb;

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
