using OpenTK;
using OpenTK.Graphics.OpenGL4;

using PhysicsSim.Vertices;

using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace PhysicsSim.VBOs
{
    public class TexturedRenderObject : ARenderable
    {
        private bool _initialized;
        private readonly int _verticeCount;
        private readonly int _program;

        private readonly int _vertexArray, _buffer;
        private readonly PrimitiveType _renderType;
        private readonly int _texture;

        public TexturedRenderObject((TexturedVertex[] vertices, PrimitiveType renderType) tuple, string filename, int program)
        {
            TexturedVertex[] vertices = tuple.vertices;
            _renderType = tuple.renderType;
            _verticeCount = vertices.Length;
            _program = program;

            _vertexArray = GL.GenVertexArray();
            _buffer = GL.GenBuffer();

            GL.BindVertexArray(_vertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _buffer);

            GL.NamedBufferStorage(
                _buffer,
                TexturedVertex.SIZE * _verticeCount,
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
                2,
                VertexAttribType.Float,
                false,
                16);

            GL.VertexArrayVertexBuffer(_vertexArray, 0, _buffer, IntPtr.Zero, TexturedVertex.SIZE);

            _texture = InitTexture(filename);
        }

        public override void Render(ref Matrix4 projection, Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            GL.UseProgram(_program);
            GL.BindVertexArray(_vertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _buffer);
            GL.BindTexture(TextureTarget.Texture2D, _texture);
            Matrix4 modelview = GetModelView(translation, rotation, scale);
            GL.UniformMatrix4(MainWindow.ModelviewLocation, false, ref modelview);
            GL.UniformMatrix4(MainWindow.ProjectionLocation, false, ref projection);
            GL.DrawArrays(_renderType, 0, _verticeCount);
        }

        private int InitTexture(string filename)
        {
            GL.CreateTextures(TextureTarget.Texture2D, 1, out int texture);
            Bitmap bitmap = (Bitmap)Image.FromFile(filename);
            BitmapData data = bitmap.LockBits(
               new Rectangle(0, 0, bitmap.Width, bitmap.Height),
               ImageLockMode.ReadOnly,
               System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgba,
                data.Width,
                data.Height,
                0,
                OpenTK.Graphics.OpenGL4.PixelFormat.Bgra,
                PixelType.UnsignedByte,
                data.Scan0);
            bitmap.UnlockBits(data);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            return texture;
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
