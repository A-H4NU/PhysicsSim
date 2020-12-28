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

        public float freq1,freq2;

        public static float Amplitude;
        public static float screentime = 0f;
        public static float Wavelength;

        public static float time = 0f, speed = 500f;

        private ARenderable _line;

        private readonly Dictionary<string, RectangularButton> _buttons;
        private StandardSlider _freq1Slider, _freq2Slider, _timeSlider;
        private RenderText freq1Text, freq2Text, ScreenTimeText;
        private static bool _working1 = false;
        public static List<float> timelist1 = new List<float>();
        public static List<float> timelist2 = new List<float>();

        private static bool _working2 = false;
        private string fontName = "Arial";

        private static float Sooth(float x)
        {
            x = Math.Abs(x);
            double a = 2 - 2 / (1 + Math.Pow(Math.E, -1/Math.Sqrt(x)));
            return (float)(a*a);
        }

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
            float x = 0;

            float wavenumber1 = 2 * MathHelper.Pi / speed * frequency_1;
            float angularFrequency1 = 2 * MathHelper.Pi * frequency_1;
            float wavenumber2 = 2 * MathHelper.Pi / speed * frequency_2;
            float angularFrequency2 = 2 * MathHelper.Pi * frequency_2;

            while (vectorArrays.Count == 0 || vectorArrays.Last().X <= windowwidth / 2)
            {
                double temp = 0f;

                for (int n = 0; n< (int)(timelist1.Count / 2); n++)
                {
                    if (x >= (time - timelist1[2*n+1]) * speed && x <= (time - timelist1[2*n]) * speed)
                    {
                        float starttime1 = timelist1[2*n];
                        float para1 = angularFrequency1 * (time - starttime1) - wavenumber1 * x;
                        float endtime1 = timelist1[2*n+1];
                        float para1rev = angularFrequency1 * (endtime1-time) + wavenumber1 * x;
                        temp += Amp * Sooth(para1rev) * Sooth(para1) * Math.Sin(para1);
                        break;
                    }
                }
                if (timelist1.Count % 2 == 1 && x <= (time - timelist1.Last()) * speed)
                {
                    float starttime1 = timelist1.Last();
                    float para1 = angularFrequency1 * (time - starttime1) - wavenumber1 * x;
                    temp += Amp * Sooth(para1) * Math.Sin(para1);
                }

                for (int n = 0; n< (int)(timelist2.Count / 2); n++)
                {
                    if (x >= (time - timelist2[2*n+1]) * speed && x <= (time - timelist2[2*n]) * speed)
                    {
                        float starttime2 = timelist2[2*n];
                        float para2 = angularFrequency2 * (time - starttime2) - wavenumber2 * x;
                        float endtime2 = timelist2[2*n+1];
                        float para2rev = angularFrequency2 * (endtime2-time) + wavenumber2 * x;
                        temp += Amp * Sooth(para2rev) * Sooth(para2) * Math.Sin(para2);
                        break;
                    }
                }
                if (timelist2.Count % 2 == 1 && x <= (time - timelist2.Last()) * speed)
                {
                    float starttime2 = timelist2.Last();
                    float para2 = angularFrequency2 * (time - starttime2) - wavenumber2 * x;
                    
                    temp += Amp * Sooth(para2) * Math.Sin(para2);
                }

                i.X = x - windowwidth/2 ;
                i.Y = (float)temp;
                vectorArrays.Add(i);
                x += delta;
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
            _line?.Dispose();
            foreach (var button in _buttons.Values)
            {
                button.Dispose();
            }
            _buttons.Clear();
            _freq1Slider.Dispose();
            _freq2Slider.Dispose();
            _timeSlider.Dispose();
            GC.SuppressFinalize(this);
        }

        protected override void OnResize(object sender, EventArgs e)
        {
            _buttons["start1"].Area = new RectangleF(_window.Width / 2f - 75f, -_window.Height / 2f + 15f, 60f, 60f);
            _buttons["start2"].Area = new RectangleF(_window.Width / 2f - 145f, -_window.Height / 2f + 15f, 60f, 60f);
            _buttons["startboth"].Area = new RectangleF(_window.Width / 2f - 215f, -_window.Height / 2f + 15f, 60f, 60f);
            _freq1Slider.Position = new Vector3(
                -Window.Width / 2f + _freq1Slider.Width / 2f + 15f,
                -Window.Height / 2f + _freq1Slider.Height / 2f + 15f, 0f);
            _freq2Slider.Position = new Vector3(
                -Window.Width / 2f + _freq2Slider.Width / 2f + 15f,
                -Window.Height / 2f + _freq1Slider.Height + _freq2Slider.Height / 2f + 30f, 0f);
            _timeSlider.Position = new Vector3(
                -Window.Width / 2f + _timeSlider.Width / 2f + 15f,
                +Window.Height / 2f - _freq2Slider.Height / 2f - 15f, 0f);
            freq1Text.Position = new Vector3(
                -Window.Width / 2f + _freq1Slider.Width + freq1Text.Width / 2f + 45f,
                -Window.Height / 2f + _freq1Slider.Height / 2f + 15f, 0f);
            freq2Text.Position = new Vector3(
                -Window.Width / 2f + _freq2Slider.Width + freq2Text.Width / 2f + 45f,
                -Window.Height / 2f + _freq1Slider.Height + _freq2Slider.Height / 2f + 30f, 0f);
            ScreenTimeText.Position = new Vector3(
                -Window.Width / 2f + _timeSlider.Width + 45f + ScreenTimeText.Width / 2f,
                +Window.Height / 2f - _freq2Slider.Height / 2f - 15f, 0f);
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
            _freq1Slider.Value = 0f;
            _freq2Slider.Value = 0f;
            _timeSlider.Value = 0f;
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
                if (timelist1.Count() == 0)
                {
                    timelist1.Add(time);
                    timelist1.Add(time + screentime);
                }
                if (time > timelist1.Last())
                {
                    timelist1.Add(time);
                    timelist1.Add(time + screentime);
                }
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
                if(timelist2.Count() == 0)
                {
                    timelist2.Add(time);
                    timelist2.Add(time + screentime);
                }
                if(time > timelist2.Last())
                {
                    timelist2.Add(time);
                    timelist2.Add(time + screentime);
                }
                
                _buttons["start2"].FillColor = _working2 ? Color4.Red : Color4.Gray;
            };

            _buttons.Add(
                "startboth",
                new RectangularButton(
                    new RectangleF(_window.Width / 2f - 215f, -_window.Height / 2f + 15f, 60f, 60f),
                    ARectangularInteraction.DefaultLineWidth,
                    Color4.BlueViolet,
                    Color4.White,
                    _window.ColoredProgram));
            _buttons["startboth"].ButtonPressEvent += (o, a) =>
            {
                if (timelist1.Count() == 0 || timelist2.Count() == 0)
                {
                    _buttons["start1"].Press();
                    _buttons["start2"].Press();
                }
                if (time > timelist1.Last() && time > timelist2.Last())
                {
                    _buttons["start1"].Press();
                    _buttons["start2"].Press();
                }
                
            };
            ///////////////////
            ///
            _freq1Slider = new StandardSlider(400, 50, 20, 0, 15f, Color4.LightBlue, Color4.White, _window.ColoredProgram);
            _freq1Slider.ValueChangedEvent += (o, ev) =>
            {
                freq1 = ev.NewValue;
                freq1Text.Text = $"f1={ev.NewValue:0.00} Hz";
            };

            _freq2Slider = new StandardSlider(400, 50, 20, 0, 15f, Color4.LightBlue, Color.White, _window.ColoredProgram);
            _freq2Slider.ValueChangedEvent += (o, ev) =>
            {
                freq2 = ev.NewValue;
                freq2Text.Text = $"f2={ev.NewValue:0.00} Hz";
            };

            _timeSlider = new StandardSlider(400, 50, 20, 0, 20f, Color4.LightBlue, Color.White, _window.ColoredProgram);
            _timeSlider.ValueChangedEvent += (o, ev) =>
            {
                screentime = ev.NewValue;
                ScreenTimeText.Text = $"ScreenTime={ev.NewValue:0} ";
            };

            freq1Text = new RenderText(25, fontName, "f1=0.00 Hz", Color.Transparent, Color.White, _window.TexturedProgram);
            freq2Text = new RenderText(25, fontName, "f2=0.00 Hz", Color.Transparent, Color.White, _window.TexturedProgram);
            ScreenTimeText = new RenderText(25, fontName, "ScreenTime=0 ", Color.Transparent, Color.White, _window.TexturedProgram);
        }

        protected override void OnRenderFrame(object sender, FrameEventArgs e)
        {
            if (!Enabled) return;
            // Clear the used buffer to paint a new frame onto it
            GL.Clear(ClearBufferMask.ColorBufferBit);
            // Declare that we will use this program
            //GL.UseProgram(_window.ColoredProgram);
            // Get projection matrix and make shaders to compute with this matrix
            Matrix4 projection = GetProjection();

            _line?.Render(ref projection);
            _freq1Slider.Render(ref projection);
            _freq2Slider.Render(ref projection);
            _timeSlider.Render(ref projection);
            freq1Text.Render(ref projection);
            freq2Text.Render(ref projection);
            ScreenTimeText.Render(ref projection);
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
            _buttons["startboth"].PressIfInside(pos);
            if (_freq1Slider.SelectIfInside(pos))
            {
                _freq1Slider.SlideIfSelected(pos);
            }
            if (_freq2Slider.SelectIfInside(pos))
            {
                _freq2Slider.SlideIfSelected(pos);
            }
            if (_timeSlider.SelectIfInside(pos))
            {
                _timeSlider.SlideIfSelected(pos);
            }
        }

        protected override void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!Enabled)
            {
                return;
            }
            var pos = MainWindow.ScreenToCoord(e.X, e.Y, Window.Width, Window.Height);
            _freq1Slider.Unselect();
            _freq2Slider.Unselect();
            _timeSlider.Unselect();
        }

        protected override void OnMouseMove(object sender, MouseMoveEventArgs e)
        {
            if (!Enabled)
            {
                return;
            }
            var pos = MainWindow.ScreenToCoord(e.X, e.Y, Window.Width, Window.Height);
            _freq1Slider.SlideIfSelected(pos);
            _freq2Slider.SlideIfSelected(pos);
            _timeSlider.SlideIfSelected(pos);
        }

        protected override void OnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (!Enabled)
            {
                return;
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
            if (timelist2.Count()!=0)
            {
                if (time <= timelist2.Last())
                {
                    _working2 = true;
                }
                else
                {
                    _working2 = false;
                }
                _buttons["start2"].FillColor = _working2 ? Color4.Red : Color4.Gray;
            }

            if (timelist1.Count()!=0)
            {
                if (time <= timelist1.Last())
                {
                    _working1 = true;
                }
                else
                {
                    _working1 = false;
                }
                _buttons["start1"].FillColor = _working1 ? Color4.Red : Color4.Gray;
            }
            
            if(_working1 || _working2)
            {
                _buttons["startboth"].FillColor = Color4.SkyBlue;
            }
            else
            {
                _buttons["startboth"].FillColor = Color4.BlueViolet;
            }
            _line = new RenderObject(ObjectFactory.Curve(WaveLine(100f, speed, freq1, freq2 , _window.Width, time), Color4.WhiteSmoke), _window.ColoredProgram);
        }

        private System.Numerics.Vector2 ScreenToCoord(int x, int y)
            => new System.Numerics.Vector2(
                x - _window.Width / 2f,
                -y + _window.Height / 2f
                );
    }
}
