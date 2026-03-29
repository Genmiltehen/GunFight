using GameEngineLib.Impl.SceneImpl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngineLib.Impl
{
    public interface IGameSystem
    {
        public int Priority { get; }
        void Update (Scene _scene, float _dt);
    }
}
