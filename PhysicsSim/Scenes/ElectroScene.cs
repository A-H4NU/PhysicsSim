using PhysicsSim.VBOs;

using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL4;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hanu.ElectroLib.Physics;
using Hanu.ElectroLib.Objects;
using System.Diagnostics;
using OpenTK.Graphics;
using System.Runtime.InteropServices;
using System.Security.Permissions;

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

        public static int LinePerUnitCharge = 32;

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
            if (!Enabled) return;
        }

        protected override void OnRenderFrame(object sender, FrameEventArgs e)
        {
            if (!Enabled) return;
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
            if (!Enabled) return;

            System.Numerics.Vector2 pos = MainWindow.ScreenToCoord(e.X, e.Y, _window.Width, _window.Height) / Scale;
            if (e.Button == MouseButton.Left)
            {
                FixedObject obj = new FixedObject(pos);
                _pObjs.Add(new RPhysicalObject(obj, _window.ColoredProgram));
                Console.WriteLine($"Added object at {pos}");
            }
            if (e.Button == MouseButton.Right)
            {
                if (_pObjs.Count != 0)
                {
                    var ef = PSystem.GetElectricFieldAt(_pObjs.Extracted(), pos);
                    Console.WriteLine($"Electric field at {pos,15}: {ef.Length(),9:F2} N/C, {Math.Atan2(ef.Y, ef.X) * 180 / Math.PI,7:F2}°");
                }
            }
        }

        protected override void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!Enabled) return;

            if (_pObjs.Count > 0)
            {
                _pObjs.Last().PObject.Charge += e.Delta * UnitCharge;
            }
        }

        protected override async void OnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (!Enabled) return;

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
                // If _pObjs contains any object whose charge is not zero
                if (_pObjs.Any((p) => p.PObject.Charge != 0f))
                {
                    // Initialize stopwatch to measure time comsumed
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    // Dispose elements in _lines and clear it
                    foreach (var line in _lines)
                    {
                        line.Dispose();
                    }
                    _lines.Clear();

                    // get the number of lines needed
                    int linecount = 0;
                    foreach (var obj in _pObjs)
                    {
                        if (obj.PObject.Charge <= 0) continue;
                        linecount += (int)(obj.PObject.Charge / UnitCharge) * LinePerUnitCharge;
                    }

                    var taskList = new List<Task<List<System.Numerics.Vector2>>>(linecount);
                    foreach (var obj in _pObjs)
                    {
                        if (obj.PObject.Charge < 0) continue;
                        int units = (int)(obj.PObject.Charge / UnitCharge);
                        for (int i = 0; i < units * LinePerUnitCharge; ++i)
                        {
                            double angle = 2*i*Math.PI / (units*LinePerUnitCharge);
                            // relative displacement with respect to the position of the charge
                            var delta = new System.Numerics.Vector2(
                                (float)Math.Cos(angle), (float)Math.Sin(angle)) * RPhysicalObject.Radius / Scale;
                            // create new task that calculates an electric field line from the specified starting point
                            var task = PLine.ElectricFieldLineAsync(
                                system: _pObjs.Extracted(),             // list of physical objects
                                initPos: obj.PObject.Position + delta,  // starting point of the line
                                endFunc: (t, v)                         // ending function (calculation stops if true)
                                    => t > MaxT ||
                                    !(-_window.Width / Scale < v.X && v.X < _window.Width / Scale && -_window.Height / Scale < v.Y && v.Y < _window.Height / Scale),
                                startFromNegative: false);              // is starting from negative charge
                            taskList.Add(task);
                        }
                    }
                    while (taskList.Any())
                    {
                        // await the next task which is finished
                        var finished = await Task.WhenAny(taskList);
                        // remove the finished task from the list
                        taskList.Remove(finished);
                        // get the result from the finished task
                        var result = await finished;
                        // add the line to _lines list
                        _lines.Add(new RenderObject(ObjectFactory.Curve(result, Color4.White), _window.ColoredProgram) { Scale = new Vector3(Scale, Scale, 1) });
                    }
                    // stop the stopwatch and write how much time is elapsed
                    stopwatch.Stop();
                    Console.WriteLine($"Time elapsed: {stopwatch.Elapsed.TotalMilliseconds:F2} ms; {_lines.Count} lines");
                }
            }
        }

        #endregion

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
    }
}
