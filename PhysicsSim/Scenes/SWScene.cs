using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

using PhysicsSim.Interactions;
using PhysicsSim.VBOs;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Permissions;

namespace PhysicsSim.Scenes
{
    public sealed class SWScene : Scene
    {
        public const string WaveVertexShader = @"Shaders\wave_vertex_shader.vert";

        public const float DefaultSpeed = 100f;

        public const float DefaultAmplitude = 10f;

        public const float DefaultLength = 100f;

        public const float DefaultFrequency = 0f;

        private bool _working = false;

        public static int WaveProgram;

        /// <summary> unit = m/s </summary>
        private float _speed = DefaultSpeed;

        /// <summary> unit = m </summary>
        private float _amplitude = DefaultAmplitude;

        /// <summary> unit = m </summary>
        private float _length = DefaultLength;

        /// <summary> unit = Hz </summary>
        private float _frequency = DefaultFrequency;

        #region Renderables

        private RenderObject _wave;
        private RenderObject _circleL, _circleR;
        private ROCollection _ampLines;
        private RectangularButton _startButton;
        private StandardSlider _freqSlider, _speedSlider;
        private RenderText _freqText, _lengthText, _speedText;
        private RectangularCheckBox _timeSlipCheck;
        private RenderText _timeSlipLabel, _startLabel;

        #endregion

        public SWScene(MainWindow window) : base(window)
        {
        }

        protected override void OnResize(object sender, EventArgs e)
        {
            _wave.Scale = new Vector3(_window.Width / _length, 1, 1);
            _startButton.Area = new RectangleF(_window.Width / 2f - 75f, -_window.Height / 2f + 15f, 60f, 60f);
            _timeSlipCheck.Area = new RectangleF(_window.Width / 2f - _startButton.Width - 105f, -_window.Height / 2f + 15f, 60f, 60f);
            _ampLines.Scale = new Vector3(_window.Width / _length, 1, 1);
            _freqSlider.Position = new Vector3(
                -_window.Width / 2f + _freqSlider.Width / 2f + 15f,
                -_window.Height / 2f + _freqSlider.Height / 2f + 15f, 0f);
            _speedSlider.Position = new Vector3(
                -_window.Width / 2f + _speedSlider.Width / 2f + 15f,
                -_window.Height / 2f + _freqSlider.Height + _speedSlider.Height / 2f + 30f, 0f);
            _freqText.Position = new Vector3(
                -_window.Width / 2f + _freqSlider.Width + _freqText.Width / 2f + 30f,
                -_window.Height / 2f + _freqSlider.Height / 2f + 15f, 0f);
            _lengthText.Position = new Vector3(
                -_window.Width / 2f + _lengthText.Width / 2f + 15f,
                _window.Height / 2f - _lengthText.Height / 2f - 15f, 0f);
            _speedText.Position = new Vector3(
                -_window.Width / 2f + _speedSlider.Width + _speedText.Width / 2f + 30f,
                -_window.Height / 2f + _freqText.Height + _speedText.Height / 2f + 45f, 0f);
            _timeSlipLabel.Position = new Vector3(
                _window.Width / 2f - _startButton.Width + _timeSlipCheck.Width / 2f - 105f,
                -_window.Height / 2f + _timeSlipCheck.Height + 30f, 0f);
            _startLabel.Position = new Vector3(
                _window.Width / 2f + _startButton.Width / 2f - 75f,
                -_window.Height / 2f + _startButton.Height + 30f, 0f);
        }

        protected override void OnClosed(object sender, EventArgs e)
        {
            Dispose();
        }

        protected override void OnLoad(object sender, EventArgs e)
        {
            WaveProgram = MainWindow.CreateProgram(WaveVertexShader, MainWindow.ColoredFragmentShaderPath);

            _wave = new RenderObject(
                ObjectFactory.Curve(
                    FunctionToCurve((x) => 0f, -_length * 1f, _length * 1f),
                    Color4.White),
                WaveProgram)
            {
                Scale = new Vector3(_window.Width / _length, 1, 1)
            };

            ///// BUTTONS /////
            _startButton = new RectangularButton(
                                new RectangleF(_window.Width / 2f - 75f, -_window.Height / 2f + 15f, 60f, 60f),
                                ARectangularInteraction.DefaultLineWidth,
                                Color4.Gray,
                                Color4.White,
                                _window.ColoredProgram);
            _startButton.ButtonPressEvent += (o, a) =>
            {
                _working ^= true;
                _startButton.FillColor = _working ? Color4.Red : Color4.Gray;
            };
            ///////////////////

            ///// CHECKBOX /////
            _timeSlipCheck = new RectangularCheckBox(60f, 60f, 5f, Color4.Black, Color4.White, Color4.Red, _window.ColoredProgram);
            ////////////////////

            ///// CIRCLES /////
            Color4 colorC = new Color4(0xCA, 0xC0, 0x3E, 128);
            _circleL = new RenderObject(ObjectFactory.FilledCircle(20f, colorC), _window.ColoredProgram);
            _circleR = new RenderObject(ObjectFactory.FilledCircle(20f, colorC), _window.ColoredProgram);
            ///////////////////

            Color4 colorL = new Color4(0.5f, 0.5f, 0.2f, 1.0f);
            _ampLines = new ROCollection(new RenderObject[]
            {
                new RenderObject(
                    ObjectFactory.Curve(
                        colorL,
                        new System.Numerics.Vector2(-0.5f * _length, +_amplitude / 0.1f),
                        new System.Numerics.Vector2(+0.5f * _length, +_amplitude / 0.1f)),
                    _window.ColoredProgram),
                new RenderObject(
                    ObjectFactory.Curve(
                        colorL,
                        new System.Numerics.Vector2(-0.5f * _length, -_amplitude / 0.1f),
                        new System.Numerics.Vector2(+0.5f * _length, -_amplitude / 0.1f)),
                    _window.ColoredProgram),
                new RenderObject(
                    ObjectFactory.Curve(
                        new Color4(1f, 1f, 1f, 0.3f),
                        new System.Numerics.Vector2(-0.5f * _length, 0f),
                        new System.Numerics.Vector2(+0.5f * _length, 0f)),
                    _window.ColoredProgram)
            })
            {
                Scale = new Vector3(_window.Width / _length, 1, 1)
            };

            ///// SLIDERS /////
            _freqSlider = new StandardSlider(400, 50, 20, 0, 5f, Color4.LightBlue, Color4.White, _window.ColoredProgram);
            _freqSlider.ValueChangedEvent += (o, ev) =>
            {
                _frequency = ev.NewValue;
                _freqText.Text = $"f={ev.NewValue:0.000} Hz";
                UniformComponents();
            };
            _speedSlider = new StandardSlider(400, 50, 20, 0, 200, Color4.LightBlue, Color.White, _window.ColoredProgram) { Value = 100f };
            _speedSlider.ValueChangedEvent += (o, ev) =>
            {
                _speed = ev.NewValue;
                _speedText.Text = $"v={ev.NewValue:000.00} m/s";
                UniformComponents();
            };
            ///////////////////

            ////// TEXTS ////// 
            string fontName = "Time New Roman";
            _freqText = new RenderText(25, fontName, "f=0.000 Hz", Color.Transparent, Color.White, _window.TexturedProgram);
            _speedText = new RenderText(25, fontName, "v=100.00 m/s", Color.Transparent, Color.White, _window.TexturedProgram);
            _lengthText = new RenderText(25, fontName, "L=100.00 m", Color.Transparent, Color.White, _window.TexturedProgram);
            _timeSlipLabel = new RenderText(10, fontName, "timeslip", Color.Transparent, Color.White, _window.TexturedProgram);
            _startLabel = new RenderText(10, fontName, "start", Color.Transparent, Color.White, _window.TexturedProgram);
            //////////////////

            UniformComponents();
        }

        protected override void OnRenderFrame(object sender, FrameEventArgs e)
        {
            if (!Enabled)
            {
                return;
            }
            GL.Clear(ClearBufferMask.ColorBufferBit);

            Matrix4 projection = MainWindow.GetProjection(_window.Width, _window.Height);
            _ampLines.Render(ref projection);
            GL.UseProgram(WaveProgram);
            GL.Uniform1(24, Time);
            _wave.Render(ref projection);
            _startButton.Render(ref projection);
            _circleL.Render(ref projection);
            _circleR.Render(ref projection);
            _freqSlider.Render(ref projection);
            _speedSlider.Render(ref projection);
            _freqText.Render(ref projection);
            _lengthText.Render(ref projection);
            _speedText.Render(ref projection);
            _timeSlipCheck.Render(ref projection);
            _timeSlipLabel.Render(ref projection);
            _startLabel.Render(ref projection);

            _window.SwapBuffers();
        }

        protected override void OnUpdateFrame(object sender, FrameEventArgs e)
        {
            if (!Enabled)
            {
                return;
            }
            if (_working)
            {
                Time += (_timeSlipCheck.IsChecked ? 3 : 1) * (float)e.Time;
            }
            else
            {
                Time = 0f;
            }
            _circleL.Position = new Vector3(-.5f * _window.Width, WaveFunc(-.5f * _length), 0);
            _circleR.Position = new Vector3(+.5f * _window.Width, WaveFunc(+.5f * _length), 0);
        }

        protected override void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!Enabled)
            {
                return;
            }
            var pos = MainWindow.ScreenToCoord(e.X, e.Y, _window.Width, _window.Height);
            _startButton.PressIfInside(pos);
            if (_freqSlider.SelectIfInside(pos))
            {
                _freqSlider.SlideIfSelected(pos);
            }
            if (_speedSlider.SelectIfInside(pos))
            {
                _speedSlider.SlideIfSelected(pos);
            }
            _timeSlipCheck.ToggleIfInside(pos);
        }

        protected override void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!Enabled)
            {
                return;
            }
            var pos = MainWindow.ScreenToCoord(e.X, e.Y, _window.Width, _window.Height);
            _freqSlider.Unselect();
            _speedSlider.Unselect();
        }

        protected override void OnMouseMove(object sender, MouseMoveEventArgs e)
        {
            if (!Enabled)
            {
                return;
            }
            var pos = MainWindow.ScreenToCoord(e.X, e.Y, _window.Width, _window.Height);
            _freqSlider.SlideIfSelected(pos);
            _speedSlider.SlideIfSelected(pos);
        }

        protected override void OnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (!Enabled)
            {
                return;
            }
            if (e.Key == Key.F11 && !e.IsRepeat)
            {
                if (_window.WindowState == WindowState.Fullscreen)
                {
                    _window.WindowState = WindowState.Normal;
                }
                else
                {
                    _window.WindowState = WindowState.Fullscreen;
                }
            }
        }

        public override void Dispose()
        {
            _wave.Dispose();
            _circleL.Dispose();
            _circleR.Dispose();
            _ampLines.Dispose();
            _startButton.Dispose();
            _timeSlipCheck.Dispose();
            _freqSlider.Dispose();
            _speedSlider.Dispose();
            _freqText.Dispose();
            _speedText.Dispose();
            _lengthText.Dispose();
            GL.DeleteProgram(WaveProgram);
            GC.SuppressFinalize(this);
        }

        private void UniformComponents()
        {
            GL.UseProgram(WaveProgram);
            GL.Uniform1(20, _amplitude);
            GL.Uniform1(21, _length);
            GL.Uniform1(22, _frequency);
            GL.Uniform1(23, _speed);
        }

        private static float Sooth(float x)
        {
            x = Math.Abs(x);
            double a = 2 - 2 / (1 + Math.Pow(Math.E, -1/Math.Sqrt(x)));
            return (float)(a*a);
        }

        private float WaveFunc(float x)
        {
            x += _length / 2;
            float res = 0;
            float wavenumber = 2 * MathHelper.Pi / _speed * _frequency;
            float angularFrequency = 2 * MathHelper.Pi * _frequency;
            int n = 0;
            while (Time * _speed > 2 * n * _length + x && n <= 10)
            {
                float para = angularFrequency * (Time - _length / _speed * n) - wavenumber * (x + _length * n);
                res += (float)(Math.Pow(0.8, n) * _amplitude * Sooth(para) * Math.Sin(para));
                n += 1;
            }
            n = 1;
            while (Time * _speed > _length * 2 * n - x && n <= 11)
            {
                float para = angularFrequency * (Time - _length / _speed * n) + wavenumber * (x - _length * n);
                res -= (float)(Math.Pow(0.8, n - 1) * _amplitude * Sooth(para) * Math.Sin(para));
                n += 1;
            }
            return res;
        }

        public override void Initialize()
        {
            Time = 0f;
            if (_working == true)
            {
                _working = false;
                _startButton.FillColor = Color4.Gray;
            }
            if (_timeSlipCheck.IsChecked)
            {
                _timeSlipCheck.Toggle();
            }
            _frequency = DefaultFrequency;
            _speed = DefaultSpeed;
            UniformComponents();
        }

        private List<System.Numerics.Vector2> FunctionToCurve(Func<float, float> function, float start, float end, int precision = 500)
        {
            var result = new List<System.Numerics.Vector2>(precision + 1);
            float delta = (end - start) / precision;
            for (int i = 0; i <= precision; ++i)
            {
                float current = start + i * delta;
                result.Add(new System.Numerics.Vector2(current, function(current)));
            }
            return result;
        }
    }
}
