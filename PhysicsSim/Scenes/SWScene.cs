using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

using PhysicsSim.Interactions;
using PhysicsSim.VBOs;

using System;

namespace PhysicsSim.Scenes
{
    public class SWScene : Scene
    {
        public SWScene(MainWindow window) : base(window)
        {

        }

        protected override void OnClosed(object sender, EventArgs e)
        {
            Dispose();
        }

        protected override void OnLoad(object sender, EventArgs e)
        {
        }

        protected override void OnRenderFrame(object sender, FrameEventArgs e)
        {
            if (!Enabled)
            {
                return;
            }

            GL.Clear(ClearBufferMask.ColorBufferBit);

            _window.SwapBuffers();
        }

        protected override void OnUpdateFrame(object sender, FrameEventArgs e)
        {
            if (!Enabled)
            {
                return;
            }
        }

        protected override void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!Enabled)
            {
                return;
            }
        }

        protected override void OnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (!Enabled)
            {
                return;
            }
        }

        public override void Dispose()
        {
        }
    }
}
