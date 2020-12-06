using OpenTK;
using OpenTK.Audio.OpenAL;
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
using System.Timers;
using System.Threading.Tasks;
using System.Threading;
using Timer = System.Timers.Timer;

namespace PhysicsSim.Scenes
{
    public sealed class DopplerScene : Scene
    {
        public float Radius = 15f, Thickness = 4f, LeafLength = 10f;

        private ROCollection _source, _observer;
        private RenderObject _sArrow, _oArrow;

        private RectangularButton _button;

        private CursorState _state = CursorState.None;

        private Vector2 _sVelocity = default, _oVelocity = default;
        private Vector2 _sInitPos, _oInitPos;

        private bool _working = default;

        private Timer _waveTimer;
        private List<(float Time, Vector3 Position)> _waves;
        private SortedSet<float> _beepTime;
        private float _frequency = 2f, _speed = 100f;

        private ContextHandle _context;
        private IntPtr _device;
        private int _alSource;

        public DopplerScene(MainWindow window) : base(window)
        {

        }

        public override void Dispose()
        {
            var toDispose = new ARenderable[]
            {
                _source, _observer, _sArrow, _oArrow, _button
            };
            foreach (var renderable in toDispose)
                renderable?.Dispose();

            if (_context != ContextHandle.Zero)
            {
                Alc.MakeContextCurrent(ContextHandle.Zero);
                Alc.DestroyContext(_context);
            }
            _context = ContextHandle.Zero;

            if (_device != IntPtr.Zero)
            {
                Alc.CloseDevice(_device);
            }
            _device = IntPtr.Zero;
        }

        public override void Initialize()
        {
            if (_working) _button.Press();
            _source.Position = new Vector3(+0.25f * Window.Width, 0f, 0f);
            _observer.Position = new Vector3(-0.25f * Window.Width, 0f, 0f);
            _sArrow.Dispose();
            _oArrow.Dispose();
            _sArrow = new RenderObject(ObjectFactory.Arrow(0, 0, 0, Color4.White), Window.ColoredProgram);
            _oArrow = new RenderObject(ObjectFactory.Arrow(0, 0, 0, Color4.White), Window.ColoredProgram);
            _sVelocity = _oVelocity = default;
            _waves.Clear();
        }

        protected override void OnResize(object sender, EventArgs e)
        {
            _button.Area = new RectangleF(Window.Width / 2f - 75f, -Window.Height / 2f + 15f, 60f, 60f);
        }

        protected override void OnClosed(object sender, EventArgs e)
        {
            Dispose();
        }

        protected override void OnLoad(object sender, EventArgs e)
        {
            ROCollection CreateParticipant(Color4 color, Vector3 position)
                =>  new ROCollection(
                        new RenderObject[]
                        {
                            new RenderObject(ObjectFactory.FilledCircle(Radius, color), Window.ColoredProgram),
                            new RenderObject(ObjectFactory.HollowCircle(Radius, Thickness, Color4.White, ObjectFactory.BorderType.Inner), Window.ColoredProgram),
                        })
                    {
                        Position = position
                    };

            _waves = new List<(float, Vector3)>();
            _beepTime = new SortedSet<float>();
            _source = CreateParticipant(Color4.Lime, new Vector3(+0.25f * Window.Width, 0f, 0f));
            _observer = CreateParticipant(Color4.Red, new Vector3(-0.25f * Window.Width, 0f, 0f));
            _sArrow = new RenderObject(ObjectFactory.Arrow(0, 0, 0, Color4.White), Window.ColoredProgram);
            _oArrow = new RenderObject(ObjectFactory.Arrow(0, 0, 0, Color4.White), Window.ColoredProgram);
            _button = new RectangularButton(60f, 60f, ARectangularInteraction.DefaultLineWidth, Color4.Gray, Color4.White, Window.ColoredProgram);
            _button.ButtonPressEvent += _button_ButtonPressEvent;

            _waveTimer = new Timer(5000);
            _waveTimer.Elapsed += (o, _) =>
            {
                float time = Time;
                Vector3 sp = _source.Position, os = _observer.Position;
                Vector2 p = (sp - os).Xy;
                float a = _speed * _speed - _oVelocity.LengthSquared;
                float b = 2 * Vector2.Dot(p, _oVelocity);
                float c = -p.LengthSquared;
                if (b*b-4*a*c >= 0)
                {
                    float sqrt = (float)Math.Sqrt(b*b-4*a*c);
                    float x1 = Time + (-b + sqrt)/(2*a);
                    float x2 = Time + (-b - sqrt)/(2*a);
                    if (x1 > Time) _beepTime.Add(x1);
                    if (x2 > Time) _beepTime.Add(x2);
                }
                _waves.Add((time, sp));
            };

            PrepareBeep();
        }

        private void _button_ButtonPressEvent(object sender, EventArgs e)
        {
            _working ^= true;
            if (_working)
            {
                _sInitPos = _source.Position.Xy;
                _oInitPos = _observer.Position.Xy;
                _sArrow.Enabled = false;
                _oArrow.Enabled = false;
                _waveTimer.Interval = 1000 / _frequency;
                _waveTimer.Start();
            }
            else
            {
                _source.Position = new Vector3(_sInitPos.X, _sInitPos.Y, 0f);
                _observer.Position = new Vector3(_oInitPos.X, _oInitPos.Y, 0f);
                _sArrow.Enabled = true;
                _oArrow.Enabled = true;
                _waves.Clear();
                _beepTime.Clear();
                _waveTimer.Stop();
            }
            _button.FillColor = _working ? Color4.Red : Color4.Gray;
        }

        protected override void OnRenderFrame(object sender, FrameEventArgs e)
        {
            if (!Enabled) return;

            GL.Clear(ClearBufferMask.ColorBufferBit);

            Matrix4 projection = MainWindow.GetProjection(Window.Width, Window.Height);

            _sArrow?.Render(ref projection);
            _oArrow?.Render(ref projection);
            _source.Render(ref projection);
            _observer.Render(ref projection);
            _button.Render(ref projection);

            int n = _waves.Count;
            for (int i = 0; i < n; ++i)
            {
                float radius = (Time - _waves[i].Time) * _speed;
                var ro = new RenderObject(ObjectFactory.HollowCircle(radius, Thickness, Color4.White, ObjectFactory.BorderType.Middle, 100), Window.ColoredProgram);
                ro.Position = _waves[i].Position;
                ro.Render(ref projection);
                ro.Dispose();
            }

            Window.SwapBuffers();
        }

        protected override void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!Enabled) return;
            var pos = MainWindow.ScreenToCoord(e.X, e.Y, Window.Width, Window.Height);
            float distSource = (float)Math.Sqrt(Math.Pow(pos.X - _source.Position.X, 2) + Math.Pow(pos.Y - _source.Position.Y, 2));
            float distObserver = (float)Math.Sqrt(Math.Pow(pos.X - _observer.Position.X, 2) + Math.Pow(pos.Y - _observer.Position.Y, 2));
            float minDist = Math.Min(distSource, distObserver);
            if (_state == CursorState.None && _button.PressIfInside(pos)) return;
            if (!_working)
            {
                if (_state == CursorState.None && minDist <= Radius)
                {
                    _state = (minDist == distSource) ? CursorState.SourceSelect : CursorState.ObserverSelect;
                }
                else if (_state == CursorState.SourceSelect || _state == CursorState.ObserverSelect)
                {
                    if (_state == CursorState.SourceSelect)
                        _sVelocity = new Vector2(pos.X - _source.Position.X, pos.Y - _source.Position.Y) / 2f;
                    else
                        _oVelocity = new Vector2(pos.X - _observer.Position.X, pos.Y - _observer.Position.Y) / 2f;
                    _state = CursorState.None;
                }
            }
        }

        protected override void OnMouseMove(object sender, MouseMoveEventArgs e)
        {
            if (!Enabled || _working) return;
            var pos = MainWindow.ScreenToCoord(e.X, e.Y, Window.Width, Window.Height);
            if (e.Mouse.IsButtonDown(MouseButton.Left))
            {
                ROCollection moving = null;
                RenderObject arrow = null;
                if (_state == CursorState.SourceSelect || _state == CursorState.SourceMove)
                {
                    _state = CursorState.SourceMove;
                    moving = _source;
                    arrow = _sArrow;
                }
                else if (_state == CursorState.ObserverSelect || _state == CursorState.ObserverMove)
                {
                    _state = CursorState.ObserverMove;
                    moving = _observer;
                    arrow = _oArrow;
                }
                if (moving != null) moving.Position = new Vector3(pos.X, pos.Y, 0);
                if (arrow != null) arrow.Position = new Vector3(pos.X, pos.Y, 0);
            }
            else
            {
                if (_state == CursorState.SourceSelect || _state == CursorState.ObserverSelect)
                {
                    Vector3 origin;
                    float dist, angle;
                    if (_state == CursorState.SourceSelect)
                    {
                        origin = _source.Position;
                        dist = (float)Math.Sqrt(Math.Pow(pos.X - _source.Position.X, 2) + Math.Pow(pos.Y - _source.Position.Y, 2));
                        angle = (float)Math.Atan2(pos.Y - _source.Position.Y, pos.X - _source.Position.X);
                    }
                    else
                    {
                        origin = _observer.Position;
                        dist = (float)Math.Sqrt(Math.Pow(pos.X - _observer.Position.X, 2) + Math.Pow(pos.Y - _observer.Position.Y, 2));
                        angle = (float)Math.Atan2(pos.Y - _observer.Position.Y, pos.X - _observer.Position.X);
                    }
                    RenderObject arrow = 
                        new RenderObject(
                            ObjectFactory.Arrow(dist, LeafLength, MathHelper.PiOver6, Color4.IndianRed),
                            Window.ColoredProgram)
                        {
                            Position = origin,
                            Rotation = new Vector3(0f, 0f, angle)
                        };
                    if (_state == CursorState.SourceSelect)
                    {
                        _sArrow?.Dispose();
                        _sArrow = arrow;
                    }
                    else
                    {
                        _oArrow?.Dispose();
                        _oArrow = arrow;
                    }
                }
            }
        }

        protected override void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!Enabled || _working) return;
            if (_state == CursorState.SourceMove || _state == CursorState.ObserverMove)
            {
                _state = CursorState.None;
            }
        }

        protected override void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            base.OnMouseWheel(sender, e);
        }

        protected override void OnUpdateFrame(object sender, FrameEventArgs e)
        {
            base.OnUpdateFrame(sender, e);
            if (!Enabled) return;
            float dt = (float)e.Time;
            if (_working)
            {
                _source.Position = new Vector3(_source.Position.X + _sVelocity.X * dt, _source.Position.Y + _sVelocity.Y * dt, 0f);
                _observer.Position = new Vector3(_observer.Position.X + _oVelocity.X * dt, _observer.Position.Y + _oVelocity.Y * dt, 0f);
            }
            if (_beepTime.Count > 0 && Time >= _beepTime.Min)
            {
                Console.WriteLine($"{_beepTime.Min}, {Time}");
                _beepTime.Remove(_beepTime.Min);
                Action action = () =>
                {
                    AL.SourcePlay(_alSource);
                    Thread.Sleep((int)(200 / _frequency));
                    AL.SourceStop(_alSource);
                };
                action.BeginInvoke((o) => action.EndInvoke(o), null);
            }
        }

        private unsafe void PrepareBeep()
        {
            _device = Alc.OpenDevice(null);
            _context = Alc.CreateContext(_device, (int*)null);

            Alc.MakeContextCurrent(_context);

            AL.GenBuffers(1, out int buffers);
            AL.GenSources(1, out _alSource);

            int sampleFreq = 44100;
            double dt = 2 * Math.PI / sampleFreq;
            double amp = 0.5;

            int freq = 440;
            var dataCount = sampleFreq / freq;


            var sinData = new short[dataCount];
            for (int i = 0; i < sinData.Length; ++i)
            {
                sinData[i] = (short)(amp * Int16.MaxValue * Math.Sin(i * dt * freq));
            }
            AL.BufferData(buffers, ALFormat.Mono16, sinData, sinData.Length, sampleFreq);
            AL.Source(_alSource, ALSourcei.Buffer, buffers);
            AL.Source(_alSource, ALSourceb.Looping, true);
        }

        private enum CursorState
        {
            None, SourceSelect, SourceMove, ObserverSelect, ObserverMove
        }
    }
}
