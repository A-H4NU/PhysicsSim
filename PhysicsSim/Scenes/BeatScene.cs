using Hanu.ElectroLib.Physics;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using PhysicsSim.VBOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsSim.Scenes
{
    public class BeatScene : Scene
    {

        public float Frequency1,Frequency2;

        public static float Amplitude;

        public static float Wavelength;

        public static float time = 0f;

        private ARenderable _line;

        RenderObject button1,button2;

        public static List<System.Numerics.Vector2> WaveLine(float Amp,
                                             float wavelength,
                                             float period,
                                             float windowwidth,
                                             float time = 0,
                                             float delta = 1)
        {
            List<System.Numerics.Vector2> vectorArrays = new List<System.Numerics.Vector2>();
            System.Numerics.Vector2 i = new System.Numerics.Vector2(-windowwidth / 2, 0);
            float count = 0;

            while (vectorArrays.Count == 0 || vectorArrays.Last().X <= windowwidth / 2)
            {
                double temp = Amp * Math.Sin((-2 * Math.PI / wavelength) * count + 2 * Math.PI / period * time);
                i.X = count - windowwidth;
                i.Y = (float)temp;
                vectorArrays.Add(i);
                count += delta;
            }
            return vectorArrays;
        }

        public BeatScene(MainWindow window)
            : base(window)
        {
        }

        public override void Dispose()
        {
        }

        protected override void OnClosed(object sender, EventArgs e)
        {
            Dispose();
            GC.SuppressFinalize(this);
        }

        protected override void OnLoad(object sender, EventArgs e)
        {
            _line = new RenderObject(ObjectFactory.Curve(WaveLine(10f, 100f, 0.1f, _window.Width, time), Color4.White), _window.ColoredProgram);
        }

        protected override void OnRenderFrame(object sender, FrameEventArgs e)
        {
            if (!Enabled) return;
            // Clear the used buffer to paint a new frame onto it
            GL.Clear(ClearBufferMask.ColorBufferBit);
            Console.WriteLine(_window.RenderFrequency);
            // Declare that we will use this program
            //GL.UseProgram(_window.ColoredProgram);
            // Get projection matrix and make shaders to compute with this matrix
            Matrix4 projection = GetProjection();

            _line?.Render(ref projection);

            _window.SwapBuffers();
        }

        protected override void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!Enabled) return;

            System.Numerics.Vector2 pos = ScreenToCoord(e.X, e.Y);
            if (e.Button == MouseButton.Left)
            {
            }
            if (e.Button == MouseButton.Right)
            {
            
            }
        }

        private Matrix4 GetProjection()
        {
            return Matrix4.CreateOrthographic(_window.Width, _window.Height, -1f, 1f);
        }

        protected override void OnUpdateFrame(object sender, FrameEventArgs e)
        {
            if (!Enabled) return;

            time += (float)e.Time;
            time %= 10f;
            _line = new RenderObject(ObjectFactory.Curve(WaveLine(10f, 100f, 0.1f, _window.Width, time), Color4.Yellow), _window.ColoredProgram);
        }

        private System.Numerics.Vector2 ScreenToCoord(int x, int y)
            => new System.Numerics.Vector2(
                x - _window.Width / 2f,
                -y + _window.Height / 2f
                );
    }
}
