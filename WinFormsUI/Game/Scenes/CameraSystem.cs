using OpenTK.Mathematics;
using System.Diagnostics;
using WinFormsUI.Game.Player;
using XEngine.Core.Base;
using XEngine.Core.Common;
using XEngine.Core.Scenery;

namespace WinFormsUI.Game.Scenes
{
    internal class CameraSystem : IGameSystem
    {
        private float _padding;
        public int Priority => 100;

        public bool IsEnabled { get; set; } = true;

        public CameraSystem(float padding)
        {
            _padding = padding;
        }

        public void Update(GScene _scene, float _dt)
        {
            Vector2 pA = _scene.Query<GTransform, GPlayer>((_, _, plr) => plr.Name == "A").First().Item2.Position2D;
            Vector2 pB = _scene.Query<GTransform, GPlayer>((_, _, plr) => plr.Name == "B").First().Item2.Position2D;

            Vector2 center = (pA + pB) / 2f;
            float distance = Vector2.Distance(pA, pB);

            _scene.Camera.Approach(new(center.X, center.Y, distance + _padding), 2f * _dt);

            //_scene.Camera.Zoom

            //// Adjust orthographic size based on distance
            //camera.orthographicSize = float.Lerp(camera.orthographicSize, targetSize, smoothSpeed);
            //camera.transform.position = new Vector3(center.x, center.y, camera.transform.position.z);
        }
    }
}
