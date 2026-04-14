using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XEngine.Core.Base;
using XEngine.Core.Common;
using XEngine.Core.Physics.Utils;
using XEngine.Core.Scenery;
using XEngine.Core.Utils;

namespace XEngine.Core.Physics.CollisionDetector
{
    public class SpatialGridCollisionDetector : ICollisionDetector
    {
        public int CellSize = 128;
        private readonly Dictionary<long, List<PhysicsEntityData>> _grid = [];
        private readonly List<List<PhysicsEntityData>> _listPool = [];
        private int _usedLists = 0;
        private readonly HashSet<long> _foundPairs = [];

        public SpatialGridCollisionDetector(int cellsize)
        {
            CellSize = cellsize;
        }

        public IEnumerable<CollisionManifold> GetManifolds(Scene scene, CollisionChecker onPairFound)
        {
            _foundPairs.Clear();
            ClearGrid();

            // отбор
            foreach (var (entity, cd, tr, rb) in scene.Query<ColliderComp, TransformComp, RigidbodyComp>())
            {
                var bounds = cd.GetBounds(tr);

                int minX = (int)MathF.Floor(bounds.Min.X / CellSize);
                int maxX = (int)MathF.Floor(bounds.Max.X / CellSize);
                int minY = (int)MathF.Floor(bounds.Min.Y / CellSize);
                int maxY = (int)MathF.Floor(bounds.Max.Y / CellSize);

                for (int x = minX; x <= maxX; x++)
                    for (int y = minY; y <= maxY; y++)
                    {
                        long key = ((long)x << 32) | (uint)y;
                        Insert(key, new PhysicsEntityData() { entityId=entity.Id, rb = rb, col = cd, tr = tr });
                    }
            }

            // пересечение
            foreach (var cell in _grid.Values)
            {
                if (cell.Count < 2) continue;

                for (int i = 0; i < cell.Count; i++)
                {
                    for (int j = i + 1; j < cell.Count; j++)
                    {
                        var cd1 = cell[i];
                        var cd2 = cell[j];
                        int id1 = cd1.entityId;
                        int id2 = cd2.entityId;
                        if (id1 > id2) (id1, id2) = (id2, id1);
                        long pairKey = ((long)id1 << 32) | (uint)id2; // Bit-packing оптимизация

                        if (_foundPairs.Add(pairKey))
                        {
                            if (cd1.col.Layer != cd2.col.Layer) continue;
                            if (!MathUtils.Box2Intersect(cd1.col.GetBounds(cd1.tr), cd2.col.GetBounds(cd2.tr))) continue;

                            var manifold = onPairFound(cd1, cd2);
                            if (manifold.HasValue) yield return manifold.Value;
                        }
                    }
                }
            }
        }

        private List<PhysicsEntityData> GetListFromPool()
        {
            if (_usedLists < _listPool.Count)
                return _listPool[_usedLists++];

            var newList = new List<PhysicsEntityData>();
            _listPool.Add(newList);
            _usedLists++;
            return newList;
        }

        private void Insert(long key, PhysicsEntityData cd)
        {
            if (!_grid.TryGetValue(key, out var list))
            {
                list = GetListFromPool();
                _grid[key] = list;
            }
            list.Add(cd);
        }

        private void ClearGrid()
        {
            for (int i = 0; i < _usedLists; i++) _listPool[i].Clear();
            _usedLists = 0;
            _grid.Clear();
        }
    }
}
