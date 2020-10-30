using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

using PhysicsSim.Interactions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsSim.Scenes
{
    public class SWScene : Scene
    {
        private RectangularButton _button;

        public SWScene(MainWindow window) : base(window)
        {
            
        }

        protected override void OnClosed(object sender, EventArgs e)
        {
            Dispose();
        }

        protected override void OnLoad(object sender, EventArgs e)
        {
            _button = new RectangularButton(0f, 0f, 100f, 100f, 5f, Color4.Green, Color4.White, _window.ColoredProgram);
        }

        protected override void OnRenderFrame(object sender, FrameEventArgs e)
        {
            if (!Enabled) return;

            GL.Clear(ClearBufferMask.ColorBufferBit);

            Matrix4 projection = MainWindow.GetProjection(_window.Width, _window.Height);
            _button.Render(ref projection);

            _window.SwapBuffers();
        }

        protected override void OnUpdateFrame(object sender, FrameEventArgs e)
        {
            if (!Enabled) return;
        }

        protected override void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!Enabled) return;
            var pos = MainWindow.ScreenToCoord(e.X, e.Y, _window.Width, _window.Height);
            if (_button.IsInside(pos))
            {
                Console.WriteLine("Click!");
            }
        }

        public override void Dispose()
        {
            _button.Dispose();
        }
    }
}
