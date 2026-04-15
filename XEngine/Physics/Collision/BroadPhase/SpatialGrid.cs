using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XEngine.Core.Base;
using XEngine.Core.Common;
using XEngine.Core.Physics.Collision.NarrowPhase;
using XEngine.Core.Physics.Components;
using XEngine.Core.Physics.MathStructs;
using XEngine.Core.Scenery;
using XEngine.Core.Utils;

namespace XEngine.Core.Physics.Collision.Detection
{
    public class SpatialGrid : IBroadPhase
    {
        public int CellSize;
        private readonly Dictionary<long, List<CollisionObject>> _grid = [];
        private readonly List<List<CollisionObject>> _listPool = [];
        private int _usedLists = 0;
        private readonly HashSet<long> _foundPairs = [];

        public SpatialGrid(int cellsize)
        {
            CellSize = cellsize;
        }

        public IEnumerable<(CollisionObject, CollisionObject)> GetPotentialPairs(Scene scene)
        {
            _foundPairs.Clear();
            ClearGrid();

            // отбор
            foreach (var (entity, cd, tr, rb) in scene.Query<Collider, TransformComp, Rigidbody>())
            {
                var bounds = cd.GetBounds(tr);

                int minX = (int)MathF.Floor(bounds.Min.X / CellSize);
                int maxX = (int)MathF.Floor(bounds.Max.X / CellSize);
                int minY = (int)MathF.Floor(bounds.Min.Y / CellSize);
                int maxY = (int)MathF.Floor(bounds.Max.Y / CellSize);

                for (int x = minX; x <= maxX; x++)
                    for (int y = minY; y <= maxY; y++)
                    {
                        long key = (long)x << 32 | (uint)y;
                        Insert(key, new CollisionObject() { entityId = entity.Id, rb = rb, col = cd, tr = tr });
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
                        var coA = cell[i];
                        var coB = cell[j];
                        int id1 = coA.entityId;
                        int id2 = coB.entityId;
                        if (id1 > id2) (id1, id2) = (id2, id1);
                        long pairKey = (long)id1 << 32 | (uint)id2; // Bit-packing оптимизация

                        if (_foundPairs.Add(pairKey))
                        {
                            if (coA.col.Layer != coB.col.Layer) continue;
                            if (!MathUtils.Box2Intersect(coA.col.GetBounds(coA.tr), coB.col.GetBounds(coB.tr))) continue;
                            
                            yield return (coA, coB);
                        }
                    }
                }
            }
        }

        private List<CollisionObject> GetListFromPool()
        {
            if (_usedLists < _listPool.Count)
                return _listPool[_usedLists++];

            var newList = new List<CollisionObject>();
            _listPool.Add(newList);
            _usedLists++;
            return newList;
        }

        private void Insert(long key, CollisionObject co)
        {
            if (!_grid.TryGetValue(key, out var list))
            {
                list = GetListFromPool();
                _grid[key] = list;
            }
            list.Add(co);
        }

        private void ClearGrid()
        {
            for (int i = 0; i < _usedLists; i++) _listPool[i].Clear();
            _usedLists = 0;
            _grid.Clear();
        }
    }
}
