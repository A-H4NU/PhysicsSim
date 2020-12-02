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
        /// <summary>
        /// The ratio of system coordinate and the corresponding position in the simulation program
        /// </summary>
        public static float Scale = 50f;

        /// <summary>
        /// Maximum t value to render electric field lines
        /// </summary>
        public static float MaxT = 5f;

        public static int LinePerUnitCharge = 12;

        public static float UnitCharge = 1e-6f;

        private List<ARenderable> _lines;
        private List<RPhysicalObject> _pObjs;

        public ElectroScene(MainWindow window)
            : base(window)
        {
            _pObjs = new List<RPhysicalObject>();
            _lines = new List<ARenderable>();
        }

        #region OpenTK Event Handling

        protected override void OnLoad(object sender, EventArgs e)
        {
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
            // Clear the used buffer to paint a new frame onto it
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Declare that we will use this program
            GL.UseProgram(_window.ColoredProgram);
            // Get projection matrix and make shaders to compute with this matrix
            Matrix4 projection = MainWindow.GetProjection(_window.Width, _window.Height);

            // Render all objects, overlaying other objects rendered before
            foreach (ARenderable render in _lines)
            {
                render.Render(ref projection);
            }
            foreach (RPhysicalObject obj in _pObjs)
            {
                obj.Render(ref projection);
            }

            // Swap buffers (currently painting buffer and the buffer that is displayed now)
            _window.SwapBuffers();
        }

        #endregion

        #region Input Handling

        protected override void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!Enabled || Time <= 0.05f)
            {
                return;
            }

            System.Numerics.Vector2 pos = MainWindow.ScreenToCoord(e.X, e.Y, _window.Width, _window.Height) / Scale;
            if (e.Button == MouseButton.Left)
            {
                FixedObject obj = new FixedObject(pos);
                _pObjs.Add(new RPhysicalObject(obj, _window.ColoredProgram));
                Console.WriteLine($"Added object at {pos}");
            }
        }

        protected override void OnMouseMove(object sender, MouseMoveEventArgs e)
        {
            if (!Enabled)
            {
                return;
            }

            System.Numerics.Vector2 pos = MainWindow.ScreenToCoord(e.X, e.Y, _window.Width, _window.Height) / Scale;
            if (e.Mouse.IsButtonDown(MouseButton.Left))
            {
                Console.WriteLine("drag");
            }
        }

        protected override void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!Enabled)
            {
                return;
            }

            if (_pObjs.Count > 0)
            {
                _pObjs.Last().PObject.Charge += e.Delta * UnitCharge;
            }
        }

        protected override void OnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (!Enabled)
            {
                return;
            }

            if (e.Key == Key.F11)
            {
                if (_window.WindowState != WindowState.Fullscreen)
                {
                    _window.WindowState = WindowState.Fullscreen;
                }
                else
                {
                    _window.WindowState = WindowState.Normal;
                }
            }
            if (e.Key == Key.F && true || !e.IsRepeat)
            {
                DrawElectricLines();
            }
        }

        #endregion

        private async void DrawElectricLines()
        {
            // If _pObjs contains any object whose charge is not zero
            if (_pObjs.Any((p) => p.PObject.Charge != 0f))
            {
                // Initialize stopwatch to measure time comsumed
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                // Dispose elements in _lines and clear it
                foreach (ARenderable line in _lines)
                {
                    line.Dispose();
                }
                _lines.Clear();

                // get the number of lines needed
                int linecount = 0;
                foreach (RPhysicalObject obj in _pObjs)
                {
                    if (obj.PObject.Charge <= 0)
                    {
                        continue;
                    }

                    linecount += (int)(obj.PObject.Charge / UnitCharge) * LinePerUnitCharge;
                }

                List<Task<List<System.Numerics.Vector2>>> taskList = new List<Task<List<System.Numerics.Vector2>>>(linecount);
                foreach (RPhysicalObject obj in _pObjs)
                {
                    if (obj.PObject.Charge < 0)
                    {
                        continue;
                    }

                    int units = (int)(obj.PObject.Charge / UnitCharge);
                    for (int i = 0; i < units * LinePerUnitCharge; ++i)
                    {
                        double angle = 2*i*Math.PI / (units*LinePerUnitCharge);
                        // relative displacement with respect to the position of the charge
                        System.Numerics.Vector2 delta = new System.Numerics.Vector2(
                            (float)Math.Cos(angle), (float)Math.Sin(angle)) * RPhysicalObject.Radius / Scale;
                        // create new task that calculates an electric field line from the specified starting point
                        Task<List<System.Numerics.Vector2>> task = PLine.ElectricFieldLineAsync(
                            system: _pObjs.Extracted(),             // list of physical objects
                            initPos: obj.PObject.Position + delta,  // starting point of the line
                            endFunc: (t, v)                         // ending function (calculation stops if true)
                                => t > MaxT ||
                                !(-_window.Width / Scale < v.X && v.X < _window.Width / Scale && -_window.Height / Scale < v.Y && v.Y < _window.Height / Scale),
                            startFromNegative: false,               // is starting from negative charge
                            delta: 2e-3f);
                        taskList.Add(task);
                    }
                }
                while (taskList.Any())
                {
                    // await the next task which is finished
                    Task<List<System.Numerics.Vector2>> finished = await Task.WhenAny(taskList);
                    // remove the finished task from the list
                    taskList.Remove(finished);
                    // get the result from the finished task
                    List<System.Numerics.Vector2> result = await finished;
                    // add the line to _lines list
                    _lines.Add(new RenderObject(ObjectFactory.Curve(result, Color4.White), _window.ColoredProgram) { Scale = new Vector3(Scale, Scale, 1) });
                }
                // stop the stopwatch and write how much time is elapsed
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed: {stopwatch.Elapsed.TotalMilliseconds:F2} ms; {_lines.Count} lines");
            }
        }

        public override void Dispose()
        {
            // Clear all disposable objects
            foreach (ARenderable obj in _lines)
            {
                obj.Dispose();
            }
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
            foreach (RenderObject obj in _lines)
            {
                obj.Dispose();
            }
            _pObjs.Clear();
            _lines.Clear();
        }
    }
}
