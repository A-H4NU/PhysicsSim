using ElectroSim.Vertices;

using OpenTK;
using OpenTK.Graphics.OpenGL4;

using System;

namespace ElectroSim.VBOs
{
    public class RenderObject : ARenderable
    {
        private bool _initialized;
        private readonly int _vertexArray;
        private readonly int _buffer;
        private readonly int _verticeCount;
        private readonly PrimitiveType _renderType;

        public RenderObject((ColoredVertex[] vertices, PrimitiveType renderType) tuple)
        {
            ColoredVertex[] vertices = tuple.vertices;
            _renderType = tuple.renderType;
            _verticeCount = vertices.Length;

            _vertexArray = GL.GenVertexArray();
            _buffer = GL.GenBuffer();

            GL.BindVertexArray(_vertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _buffer);

            GL.NamedBufferStorage(
                _buffer,
                ColoredVertex.SIZE * _verticeCount,
                vertices,
                BufferStorageFlags.MapWriteBit);

            GL.VertexArrayAttribBinding(_vertexArray, 0, 0);
            GL.EnableVertexArrayAttrib(_vertexArray, 0);
            GL.VertexArrayAttribFormat(
                _vertexArray,
                0,
                4,
                VertexAttribType.Float,
                false,
                0);

            GL.VertexArrayAttribBinding(_vertexArray, 1, 0);
            GL.EnableVertexArrayAttrib(_vertexArray, 1);
            GL.VertexArrayAttribFormat(
                _vertexArray,
                1,
                4,
                VertexAttribType.Float,
                false,
                16);

            GL.VertexArrayVertexBuffer(_vertexArray, 0, _buffer, IntPtr.Zero, ColoredVertex.SIZE);

            _initialized = true;
        }

        public override void Render(Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            Matrix4 modelview = GetModelView(translation, rotation, scale);
            GL.UniformMatrix4(MainWindow.ModelviewLocation, false, ref modelview);
            GL.BindVertexArray(_vertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _buffer);
            GL.DrawArrays(_renderType, 0, _verticeCount);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _initialized)
            {
                GL.DeleteVertexArray(_vertexArray);
                GL.DeleteBuffer(_buffer);
                _initialized = false;
            }
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
