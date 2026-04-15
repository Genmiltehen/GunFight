using OpenTK.Mathematics;
using System.Security.Cryptography.Xml;
using XEngine.Core.Base;
using XEngine.Core.Common;
using XEngine.Core.Graphics;
using XEngine.Core.Input;
using XEngine.Core.Physics;
using XEngine.Core.Scenery;

namespace WinFormsUI.Game
{
    public class TestSystem(InputService input) : InputSystem(input)
    {
        public override void Update(Scene scene, float dt)
        {
            //Func2(scene, dt);
        }

        private void Func1(Scene scene, float dt)
        {
            float h = input.GetAxis("Horzontal");
            float v = input.GetAxis("Vertical");
            float r = input.GetAxis("Rotato");

            var (_, cam, tr) = scene.Query<CameraComp, TransformComp>().FirstOrDefault();

            tr.Position2D += new Vector2(h, v) * dt * 500;
            cam.Zoom += r * dt;
            //rb.AddTorque(r * dt * 100);
        }

        private void Func2(Scene scene, float dt)
        {
            float h = input.GetAxis("Horzontal");
            float v = input.GetAxis("Vertical");
            float r = input.GetAxis("Rotato");

            var (_, tr) = scene.Query<TransformComp>(e => e.Has<PlayerTag>()).FirstOrDefault();

            tr.Position2D += new Vector2(h, v) * dt * 100;
            tr.Rotation += r * dt;
            //rb.AddTorque(r * dt * 100);
        }
    }
}
