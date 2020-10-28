using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using PhysicsSim.VBOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsSim.Scenes
{
    public sealed class TestScene : Scene
    {
        private RenderObject circle;
        private List<ARenderable> _lines;

        public TestScene(MainWindow window) : base(window)
        {
            circle = new RenderObject(ObjectFactory.FilledCircle(13f, Color4.AliceBlue));
            _lines = new List<ARenderable>();
            var l = new List<System.Numerics.Vector2>(new System.Numerics.Vector2[] { new System.Numerics.Vector2(0, 0), new System.Numerics.Vector2(0, 100) });
            _lines.Add(new RenderObject(ObjectFactory.Curve(l, Color4.White)));
        }

        public override void Dispose()
        {
        }

        protected override void OnClosed(object sender, EventArgs e)
        {
        }

        protected override void OnLoad(object sender, EventArgs e)
        {
        }

        private Matrix4 GetProjection()
        {
            return Matrix4.CreateOrthographic(_window.Width, _window.Height, -1f, 1f);
        }

        protected override void OnRenderFrame(object sender, FrameEventArgs e)
        {

            if (!Enabled) return;
            // Clear the used buffer to paint a new frame onto it
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Declare that we will use this program
            GL.UseProgram(_window.Program);
            // Get projection matrix and make shaders to compute with this matrix
            Matrix4 projection = GetProjection();
            GL.UniformMatrix4(MainWindow.ProjectionLocation, false, ref projection);
            circle.Render();
            _lines[0].Render();
            _window.SwapBuffers();
        }

        protected override void OnUpdateFrame(object sender, FrameEventArgs e)
        {
            if (!Enabled) return;
        }
    }
}
