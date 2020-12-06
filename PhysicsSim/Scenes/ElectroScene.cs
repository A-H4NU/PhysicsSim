using Hanu.ElectroLib.Objects;
using Hanu.ElectroLib.Physics;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

using PhysicsSim.VBOs;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PhysicsSim.Scenes
{
    public sealed class ElectroScene : Scene
    {
        public static float SelectRadius = 0.5f;

        /// <summary>
        /// The ratio of system coordinate and the corresponding position in the simulation program
        /// </summary>
        public static float Scale = 50f;

        /// <summary>
        /// Maximum t value to render electric field lines
        /// </summary>
        public static float MaxT = 5f;

        public static int LinePerUnitCharge = 10;

        public static float UnitCharge = 1e-6f;

        private List<List<System.Numerics.Vector2>> _lines;
        private List<RPhysicalObject> _pObjs;
        private RPhysicalObject _selected = null;
        private RenderObject _selectMarker;

        private bool _rendering = false;

        public ElectroScene(MainWindow window)
            : base(window)
        {
            _pObjs = new List<RPhysicalObject>();
            _lines = new List<List<System.Numerics.Vector2>>();
        }

        #region OpenTK Event Handling

        protected override void OnLoad(object sender, EventArgs e)
        {
            _selectMarker = new RenderObject(
                ObjectFactory.HollowCircle(
                    RPhysicalObject.Radius * 1.2f,
                    RPhysicalObject.BorderThickness * 1.2f,
                    Color4.GreenYellow,
                    ObjectFactory.BorderType.Outter),
                Window.ColoredProgram);
        }

        protected override void OnClosed(object sender, EventArgs e)
        {
            Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Called before <see cref="OnRenderFrame(FrameEventArgs)"/>, to update variables by time and input
        /// </summary>
        protected override void OnUpdateFrame(object sender, FrameEventArgs e)
        {
            if (!Enabled)
            {
                return;
            }
            Time += (float)e.Time;
        }

        protected override void OnRenderFrame(object sender, FrameEventArgs e)
        {
            if (!Enabled)
            {
                return;
            }
            _rendering = true;

            // Clear the used buffer to paint a new frame onto it
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Declare that we will use this program
            GL.UseProgram(Window.ColoredProgram);
            // Get projection matrix and make shaders to compute with this matrix
            Matrix4 projection = MainWindow.GetProjection(Window.Width, Window.Height);

            // Render all objects, overlaying other objects rendered before
            foreach (var l in _lines)
            {
                var ro = new RenderObject(
                             ObjectFactory.Curve(l, Color4.White),
                             Window.ColoredProgram)
                         {
                            Scale = new Vector3(Scale, Scale, 1)
                         };
                ro.Render(ref projection);
                ro.Dispose();
            }

            foreach (RPhysicalObject obj in _pObjs)
            {
                obj.Render(ref projection);
            }

            if (_selected != null)
            {
                _selectMarker.Render(ref projection);
            }

            // Swap buffers (currently painting buffer and the buffer that is displayed now)
            Window.SwapBuffers();

            _rendering = false;
        }

        #endregion

        #region Input Handling

        protected override void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!Enabled || Time <= 0.05f)
            {
                return;
            }

            System.Numerics.Vector2 pos = MainWindow.ScreenToCoord(e.X, e.Y, Window.Width, Window.Height) / Scale;
            if (e.Button == MouseButton.Left)
            {
                bool s = false;
                if (_pObjs.Count > 0)
                {
                    var list = (from obj in _pObjs
                                where (obj.PObject.Position - pos).LengthSquared() < SelectRadius * SelectRadius
                                orderby (obj.PObject.Position - pos).LengthSquared()
                                select obj).ToArray();
                    if (list.Length > 0)
                    {
                        s = true;
                        _selected = list[0];
                    }
                }
                if (!s)
                {
                    MovableObject obj = new MovableObject(pos);
                    var rpo = new RPhysicalObject(obj, Window.ColoredProgram);
                    _pObjs.Add(rpo);
                    _selected = rpo;
                    Console.WriteLine($"Added object at {pos}");
                }
                _selectMarker.Position = new Vector3(_selected.PObject.Position.X, _selected.PObject.Position.Y, 0f) * Scale;
            }
        }

        protected override void OnMouseMove(object sender, MouseMoveEventArgs e)
        {
            if (!Enabled) return;
            System.Numerics.Vector2 pos = MainWindow.ScreenToCoord(e.X, e.Y, Window.Width, Window.Height) / Scale;
            if (_selected != null && e.Mouse.IsButtonDown(MouseButton.Left))
            {
                ((MovableObject)_selected.PObject).PositionM = pos;
                DrawElectricLines(true);
                _selectMarker.Position = new Vector3(pos.X, pos.Y, 0) * Scale;
            }
        }

        protected override void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!Enabled)
            {
                return;
            }
            if (_selected != null)
            {
                float charge = ((int)Math.Round(_selected.PObject.Charge / UnitCharge) + e.Delta) * UnitCharge;
                _selected.PObject.Charge = charge;
                DrawElectricLines(false);
            }
        }

        protected override void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!Enabled)
            {
                return;
            }
            if (_selected != null)
            {
                //_selected = null;
                DrawElectricLines(false);
            }
        }

        protected override void OnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (!Enabled)
            {
                return;
            }
            if (e.Key == Key.F)
            {
                DrawElectricLines(false);
            }
        }

        #endregion

        private async void DrawElectricLines(bool quick)
        {
            if (!_pObjs.Any((p) => p.PObject.Charge != 0f))
            {
                return;
            }
            var tasklist = new List<Task<List<System.Numerics.Vector2>>>();
            var extracted = _pObjs.Extracted();
            foreach (var obj in _pObjs)
            {
                int units = (int)Math.Round(obj.PObject.Charge / UnitCharge);
                for (int i = 0; i < units * LinePerUnitCharge; ++i)
                {
                    double angle = 2*i*Math.PI / (units*LinePerUnitCharge);
                    // relative displacement with respect to the position of the charge
                    System.Numerics.Vector2 delta = new System.Numerics.Vector2(
                        (float)Math.Cos(angle), (float)Math.Sin(angle)) * RPhysicalObject.Radius / Scale;
                    // create new task that calculates an electric field line from the specified starting point

                    Task<List<System.Numerics.Vector2>> task = null;
                    if (quick)
                    {
                        task = PLine.ElectricFieldLineFastAsync(
                            system: extracted,                      // list of physical objects
                            initPos: obj.PObject.Position + delta,  // starting point of the line
                            endFunc: (t, v)                         // ending function (calculation stops if true)
                                => t > MaxT ||
                                !(-Window.Width / Scale < v.X && v.X < Window.Width / Scale && -Window.Height / Scale < v.Y && v.Y < Window.Height / Scale),
                            startFromNegative: false,               // is starting from negative charge
                            delta: 1e-3f);
                    }
                    else
                    {
                        task = PLine.ElectricFieldLineAsync(
                            system: extracted,                      // list of physical objects
                            initPos: obj.PObject.Position + delta,  // starting point of the line
                            endFunc: (t, v)                         // ending function (calculation stops if true)
                                => t > MaxT ||
                                !(-Window.Width / Scale < v.X && v.X < Window.Width / Scale && -Window.Height / Scale < v.Y && v.Y < Window.Height / Scale),
                            startFromNegative: false,               // is starting from negative charge
                            delta: 1e-3f);
                    }
                    tasklist.Add(task);
                }
            }

            var newL = new List<List<System.Numerics.Vector2>>();
            await Task.WhenAll(tasklist);
            foreach (var task in tasklist)
            {
                var res = await task;
                newL.Add(res);
            }

            while (_rendering) { }
            _lines = newL;
        }

        public override void Dispose()
        {
            // Clear all disposable objects
            //foreach (ARenderable obj in _lines)
            //{
            //    obj.Dispose();
            //}
            foreach (RPhysicalObject obj in _pObjs)
            {
                obj.Dispose();
            }
            _lines.Clear();
            _pObjs.Clear();
            _lines = null;
            _pObjs = null;
        }

        public override void Initialize()
        {
            Time = 0f;
            foreach (RPhysicalObject obj in _pObjs)
            {
                obj.Dispose();
            }
            _selected = null;
            _pObjs.Clear();
            _lines.Clear();
        }
    }
}
