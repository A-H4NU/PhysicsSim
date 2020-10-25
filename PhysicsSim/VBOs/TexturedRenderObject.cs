using OpenTK;
using OpenTK.Graphics.OpenGL4;

using PhysicsSim.Vertices;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Matrix4 modelview = GetModelView(translation, rotation, scale);
            GL.UniformMatrix4(MainWindow.ModelviewLocation, false, ref modelview);
            GL.UniformMatrix4(MainWindow.ProjectionLocation, false, ref projection);
            GL.BindVertexArray(_vertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _buffer);
            GL.BindTexture(TextureTarget.Texture2D, _texture);
            GL.DrawArrays(_renderType, 0, _verticeCount);
        }

        private int InitTexture(string filename)
        {
            var data = LoadTexture(filename, out int width, out int height);
            int texture = GL.GenTexture();
            GL.TextureStorage2D(
                texture:        texture,
                levels:         1,
                internalformat: SizedInternalFormat.Rgba16f,
                width:          width,
                height:         height);

            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TextureSubImage2D(
                texture:        texture,
                level:          0,
                xoffset:        0,
                yoffset:        0,
                width:          width,
                height:         height,
                format:         OpenTK.Graphics.OpenGL4.PixelFormat.Rgba,
                type:           PixelType.Float,
                pixels:         data);
            return texture;
        }

        private float[] LoadTexture(string filename, out int width, out int height)
        {
            float[] res;
            using (Bitmap bmp = (Bitmap)Image.FromFile(filename))
            {
                width = bmp.Width;
                height = bmp.Height;
                res = new float[width * height * 4];
                BitmapData data = null;
                try
                {
                    data = bmp.LockBits(
                        new Rectangle(0, 0, bmp.Width, bmp.Height),
                        ImageLockMode.ReadOnly,
                        System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    unsafe
                    {
                        int index = 0;
                        var ptr = (byte*)data.Scan0;
                        int remain = data.Stride - data.Width * 3;
                        for (int i = 0; i < data.Height; ++i)
                        {
                            for (int j = 0; j < data.Width; ++j)
                            {
                                res[index++] = ptr[2] / 255f;
                                res[index++] = ptr[1] / 255f;
                                res[index++] = ptr[1] / 255f;
                                res[index++] = 1f;
                                ptr += 3;
                            }
                            ptr += remain;
                        }
                    }
                }
                finally
                {
                    bmp.UnlockBits(data);
                }
            }
            return res;
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
