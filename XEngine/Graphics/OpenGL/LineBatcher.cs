using Box2D.NET;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XEngine.Core.Graphics.OpenGL
{
    public sealed class LineBatcher : IDisposable
    {
        private readonly List<float> _vertices = [];
        private readonly List<uint> _indices = [];
        private int _vertexCount = 0;

        private int _vboCapacity = 1024;
        private int _eboCapacity = 512;

        private int _vao, _vbo, _ebo;
        private bool _disposed = false;

        private int _loopStartIndex = -1;

        public readonly struct LineLoopScope : IDisposable
        {
            private readonly LineBatcher _batcher;
            private readonly bool _close;

            public LineLoopScope(LineBatcher batcher, bool close)
            {
                _batcher = batcher;
                _close = close;
                _batcher.BeginLoop();
            }

            public void Dispose() => _batcher.EndLoop(_close);
        }

        private void BeginLoop()
        {
            _loopStartIndex = _vertexCount;
        }

        private void EndLoop(bool closeLoop = true)
        {
            if (closeLoop && _vertexCount > _loopStartIndex + 2)
            {
                _indices.Add((uint)_vertexCount - 1);
                _indices.Add((uint)_loopStartIndex);
            }
        }

        public void Init()
        {
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();
            _ebo = GL.GenBuffer();

            GL.BindVertexArray(_vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _vboCapacity * sizeof(float), IntPtr.Zero, BufferUsageHint.DynamicDraw);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _eboCapacity * sizeof(uint), IntPtr.Zero, BufferUsageHint.DynamicDraw);

            GL.BindVertexArray(0);
        }

        public LineLoopScope TraceLine(bool closed = true)
        {
            return new LineLoopScope(this, closed);
        }

        public void AddPoint(B2Vec2 point)
        {
            if (_loopStartIndex == -1)
                throw new InvalidOperationException("Unable to add point to line without context");

            _vertices.Add(point.X);
            _vertices.Add(point.Y);
            _vertexCount++;

            int current = _vertexCount - 1;
            if (current > _loopStartIndex)
            {
                _indices.Add((uint)current - 1u);
                _indices.Add((uint)current);
            }
        }

        public void AddPolygon(Span<B2Vec2> points)
        {
            using (TraceLine(true)) foreach (var point in points) AddPoint(point);
        }

        public void Clear()
        {
            _vertices.Clear();
            _indices.Clear();
            _vertexCount = 0;
        }

        public void Draw()
        {
            if (_indices.Count == 0) return;

            GL.BindVertexArray(_vao);
            UpdateVBO();
            UpdateEBO();

            GL.DrawElements(PrimitiveType.Lines, _indices.Count, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            GL.DeleteBuffer(_vbo);
            GL.DeleteBuffer(_ebo);
            GL.DeleteVertexArray(_vao);

            GC.SuppressFinalize(this);
        }

        private void UpdateVBO()
        {
            float[] data = [.. _vertices];

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            if (data.Length > _vboCapacity)
            {
                _vboCapacity = data.Length * 2;
                GL.BufferData(BufferTarget.ArrayBuffer, _vboCapacity * sizeof(float), data, BufferUsageHint.DynamicDraw);
            }
            else
            {
                GL.BufferData(BufferTarget.ArrayBuffer, _vboCapacity * sizeof(float), IntPtr.Zero, BufferUsageHint.DynamicDraw);
                GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, data.Length * sizeof(float), data);
            }
        }

        private void UpdateEBO()
        {
            uint[] data = [.. _indices];
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);

            if (data.Length > _eboCapacity)
            {
                _eboCapacity = data.Length * 2;
                GL.BufferData(BufferTarget.ElementArrayBuffer, _eboCapacity * sizeof(uint), data, BufferUsageHint.DynamicDraw);
            }
            else
            {
                GL.BufferData(BufferTarget.ElementArrayBuffer, _eboCapacity * sizeof(uint), IntPtr.Zero, BufferUsageHint.DynamicDraw);
                GL.BufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, data.Length * sizeof(uint), data);
            }
        }
    }
}
