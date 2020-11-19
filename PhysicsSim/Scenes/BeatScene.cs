using Hanu.ElectroLib.Physics;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

using PhysicsSim.Interactions;
using PhysicsSim.VBOs;
using System;
using System.Collections.Generic;
using System.Drawing;
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

        public static float time = 0f, speed = 0.00001f;

        private ARenderable _line;

        private readonly Dictionary<string, RectangularButton> _buttons;

        private static bool _working1 = false;
        private static float _time1 = 100000f, _time2 = 100000f;
        private static bool _working2 = false;

        public static List<System.Numerics.Vector2> WaveLine(float Amp,
                                             float speed,
                                             float frequency_1,
                                             float frequency_2,
                                             float windowwidth,
                                             float time = 0,
                                             float delta = 1)
        {
            List<System.Numerics.Vector2> vectorArrays = new List<System.Numerics.Vector2>();
            System.Numerics.Vector2 i = new System.Numerics.Vector2(-windowwidth / 2, 0);
            float count = 0;
            float flag;
            while (vectorArrays.Count == 0 || vectorArrays.Last().X <= windowwidth / 2)
            {
                double temp = 0f;
                if (BeatScene._working1)
                {
                    if (_time1 * speed * 500 + 800 < count)
                    {
                        flag = 0;
                    }
                    else
                    {
                        flag = 1;
                    }
                    temp += flag * Amp * Math.Sin(-2 * Math.PI * frequency_1 / speed * count + 2 * Math.PI * frequency_1 * time);
                }
                if (BeatScene._working2)
                {
                    if (_time2 * speed * 500 + 800 < count)
                    {
                        flag = 0;
                    }
                    else
                    {
                        flag = 1;
                    }
                    temp -= flag * (Amp) * Math.Sin(-2 * Math.PI * frequency_2 / speed * count + 2 * Math.PI * frequency_2 * time);
                }
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
            _buttons = new Dictionary<string, RectangularButton>();
        }

        public override void Dispose()
        {
            _line.Dispose();
            foreach (var button in _buttons.Values)
            {
                button.Dispose();
            }
            _buttons.Clear();
            GC.SuppressFinalize(this);
        }

        protected override void OnResize(object sender, EventArgs e)
        {
            _buttons["start1"].Area = new RectangleF(_window.Width / 2f - 75f, -_window.Height / 2f + 15f, 60f, 60f);
            _buttons["start2"].Area = new RectangleF(_window.Width / 2f - 145f, -_window.Height / 2f + 15f, 60f, 60f);
        }

        protected override void OnClosed(object sender, EventArgs e)
        {
            Dispose();
            GC.SuppressFinalize(this);
        }

        public override void Initialize()
        {
            Time = 0f;
            _working1 = false;
            _working2 = false;
        }

        protected override void OnLoad(object sender, EventArgs e)
        {
            ///// BUTTONS /////
            _buttons.Add(
                "start1",
                new RectangularButton(
                    new RectangleF(_window.Width / 2f - 75f, -_window.Height / 2f + 15f, 60f, 60f),
                    ARectangularInteraction.DefaultLineWidth,
                    Color4.Gray,
                    Color4.White,
                    _window.ColoredProgram));
            _buttons["start1"].ButtonPressEvent += (o, a) =>
            {
                _working1 ^= true;
                _time1 = 0f;
                _buttons["start1"].FillColor = _working1 ? Color4.Red : Color4.Gray;
            };

            _buttons.Add(
                "start2",
                new RectangularButton(
                    new RectangleF(_window.Width / 2f - 145f, -_window.Height / 2f + 15f, 60f, 60f),
                    ARectangularInteraction.DefaultLineWidth,
                    Color4.Gray,
                    Color4.White,
                    _window.ColoredProgram));
            _buttons["start2"].ButtonPressEvent += (o, a) =>
            {
                _working2 ^= true;
                _time2 = 0f;
                _buttons["start2"].FillColor = _working2 ? Color4.Red : Color4.Gray;
            };
            ///////////////////
        }

        protected override void OnRenderFrame(object sender, FrameEventArgs e)
        {
            if (!Enabled) return;
            // Clear the used buffer to paint a new frame onto it
            GL.Clear(ClearBufferMask.ColorBufferBit);
            Console.WriteLine(_window.Width);
            // Declare that we will use this program
            //GL.UseProgram(_window.ColoredProgram);
            // Get projection matrix and make shaders to compute with this matrix
            Matrix4 projection = GetProjection();

            _line?.Render(ref projection);

            foreach (var button in _buttons.Values)
            {
                button.Render(ref projection);
            }

            _window.SwapBuffers();
        }

        protected override void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!Enabled)
            {
                return;
            }
            var pos = MainWindow.ScreenToCoord(e.X, e.Y, _window.Width, _window.Height);
            _buttons["start1"].PressIfInside(pos);
            _buttons["start2"].PressIfInside(pos);
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
            if (_working1)
            {
                _time1 += 1000f;
            }
            if (_working2)
            {
                _time2 += 1000f;
            }
            _line = new RenderObject(ObjectFactory.Curve(WaveLine(100f, speed, 6f, 7f , _window.Width, time), Color4.WhiteSmoke), _window.ColoredProgram);
        }

        private System.Numerics.Vector2 ScreenToCoord(int x, int y)
            => new System.Numerics.Vector2(
                x - _window.Width / 2f,
                -y + _window.Height / 2f
                );
    }
}
