using ElectroSim.VBOs;
using ElectroSim.Vertices;

using Hanu.ElectroLib.Objects;
using Hanu.ElectroLib.Physics;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace ElectroSim
{
    public sealed class MainWindow : GameWindow
    {
        /// <summary>
        /// The location of the modelview matrix in the vertex shader
        /// </summary>
        public static int ModelviewLocation = 10;

        /// <summary>
        /// The location of the projection matrix in the vertex shader
        /// </summary>
        public static int ProjectionLocation = 11;

        /// <summary>
        /// The ratio of system coordinate and the corresponding position in the simulation program
        /// </summary>
        public static float Scale = 50f;

        /// <summary>
        /// Maximum t value to render electric field lines
        /// </summary>
        public static float MaxT = 100f;

        public static int LinePerUnitCharge = 50;

        public static float UnitCharge = 1e-6f;

        /// <summary>
        /// List of electric field lines
        /// </summary>
        private List<ARenderable> _lines;

        /// <summary>
        /// List of renderable physcial objects
        /// </summary>
        private List<RPhysicalObject> _pObjs;

        /// <summary>
        /// Program for rendering. Contains various shaders
        /// </summary>
        private int _program;

        private readonly Timer _timer;

        public MainWindow(int width, int height)
            : base(width, height, new GraphicsMode(32, 24, 0, 8), "ElectroSim", GameWindowFlags.Default, DisplayDevice.Default)
        {
            // Create timer that perform a specific function
            _timer = new Timer(5000);
            _timer.Elapsed += (o, e) => Console.WriteLine($"total memory using at {e.SignalTime:HH:mm:ss:fff}: {GC.GetTotalMemory(true)} bytes");
            _timer.Start();
        }

        // Contains overrided methods from OpenTK to render
        #region OpenTK Events

        /// <summary>
        /// Called when <see cref="GameWindow"/> is loading, initialize and load materials needed for the application to run
        /// </summary>W
        protected override void OnLoad(EventArgs e)
        {
            // Initialize the default background color
            GL.ClearColor(new Color4(0.1f, 0.1f, 0.4f, 1.0f));

            // Create a new program that is used when rendering
            _program = CreateProgram();

            // Initialize these two lists
            _lines = new List<ARenderable>();
            _pObjs = new List<RPhysicalObject>();

            // Fill each face no matter it is a front face or not
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.PatchParameter(PatchParameterInt.PatchVertices, 3);
            
        }

        /// <summary>
        /// Called when the window is resized
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            // change the viewport (where rendered images are represented)
            GL.Viewport(ClientRectangle);
        }

        /// <summary>
        /// Called before <see cref="OnRenderFrame(FrameEventArgs)"/>, to update variables by time and input
        /// </summary>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            HandleKeyboard();
        }

        /// <summary>
        /// Called when rendering has begun, call other rendering methods in here
        /// </summary>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            // Clear the used buffer to paint a new frame onto it
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Declare that we will use this program
            GL.UseProgram(_program);
            // Get projection matrix and make shaders to compute with this matrix
            Matrix4 projection = GetProjection();
            GL.UniformMatrix4(ProjectionLocation, false, ref projection);

            // Render all objects, overlaying other objects rendered before
            foreach (ARenderable render in _lines)
            {
                render.Render();
            }
            foreach (RPhysicalObject obj in _pObjs)
            {
                obj.Render();
            }

            // Swap buffers (currently painting buffer and the buffer that is displayed now)
            SwapBuffers();
        }

        /// <summary>
        /// Called when the window has closed, dispose variables that are needed to be cleared
        /// </summary>
        protected override void OnClosed(EventArgs e)
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
            // Forces an immediate garbage collection of all generations
            GC.Collect();
        }

        #endregion

        // Contains overrided methods from OpenTK and others to handling inputs
        #region Input Handling

        /// <summary>
        /// Handling keyboard input, called in every frame
        /// </summary>
        private void HandleKeyboard()
        {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Key.Escape))
            {
                Close();
            }
        }

        /// <summary>
        /// Called when keyboard input is detected from OS
        /// </summary>
        protected async override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.F11)
            {
                if (WindowState != WindowState.Fullscreen)
                {
                    WindowState = WindowState.Fullscreen;
                }
                else
                {
                    WindowState = WindowState.Normal;
                }
            }
            if (e.Key == Key.F && !e.IsRepeat)
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
                                    !(-Width / Scale < v.X && v.X < Width / Scale && -Height / Scale < v.Y && v.Y < Height / Scale),
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
                        _lines.Add(new RenderObject(ObjectFactory.Curve(result, Color4.White)) { Scale = new Vector3(Scale, Scale, 1) });
                    }
                    // stop the stopwatch and write how much time is elapsed
                    stopwatch.Stop();
                    Console.WriteLine($"Time elapsed: {stopwatch.Elapsed.TotalMilliseconds:F2} ms; {_lines.Count} lines");
                }
            }
        }

        /// <summary>
        /// Called when mouse click is detected
        /// </summary>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            System.Numerics.Vector2 pos = ScreenToCoord(e.X, e.Y);
            if (e.Button == MouseButton.Left)
            {
                pos /= Scale;
                FixedObject obj = new FixedObject(pos);
                _pObjs.Add(new RPhysicalObject(obj));
                Console.WriteLine($"Added object at {pos}");
            }
            if (e.Button == MouseButton.Right)
            {
                if (_pObjs.Count != 0)
                {
                    var ef = PSystem.GetElectricFieldAt(_pObjs.Extracted(), pos);
                    Console.WriteLine($"Electric field at {pos,15}: {ef.Length(),7:F2} N/C, {Math.Atan2(ef.Y, ef.X) * 180 / Math.PI,7:F2}°");
                }
            }
        }

        /// <summary>
        /// Called when mouse wheel moving is detected
        /// </summary>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (_pObjs.Count > 0)
            {
                _pObjs.Last().PObject.Charge += e.Delta * UnitCharge;
            }
        }

        #endregion

        // Contains methods to create a program and compile shaders
        #region Shader Handling

        /// <summary>
        /// Compiling a shader from the filepath
        /// </summary>
        /// <returns>The ID of the compiled shader</returns>
        private int CompileShader(ShaderType type, string filepath)
        {
            int shader = GL.CreateShader(type);
            string source = File.ReadAllText(filepath);
            GL.ShaderSource(shader, source);
            GL.CompileShader(shader);
            string info = GL.GetShaderInfoLog(shader);
            if (!string.IsNullOrWhiteSpace(info))
            {
                Console.WriteLine($"GL.CompileShader [{type}] had info log: {info}");
            }

            return shader;
        }

        /// <summary>
        /// Create a new program that contains <see cref="ShaderType.VertexShader"/> and <see cref="ShaderType.FragmentShader"/>
        /// </summary>
        /// <returns>The ID of the created program</returns>
        private int CreateProgram()
        {
            int program = GL.CreateProgram();
            List<int> shaders = new List<int>();
            shaders.Add(CompileShader(ShaderType.VertexShader, @"Shaders\vertex_shader.vert"));
            shaders.Add(CompileShader(ShaderType.FragmentShader, @"Shaders\fragment_shader.frag"));

            foreach (int shader in shaders)
            {
                GL.AttachShader(program, shader);
            }

            GL.LinkProgram(program);
            string info = GL.GetProgramInfoLog(program);
            if (!string.IsNullOrWhiteSpace(info))
            {
                Console.WriteLine($"GL.LinkProgram had info log: {info}");
            }

            foreach (int shader in shaders)
            {
                GL.DetachShader(program, shader);
                GL.DeleteShader(shader);
            }
            return program;
        }

        #endregion

        /// <summary>
        /// Get projection matrix for rendering, which is a orthographic projection matrix
        /// </summary>
        /// <returns>The orthographic matrix</returns>
        private Matrix4 GetProjection()
        {
            return Matrix4.CreateOrthographic(Width, Height, -1f, 1f);
        }

        /// <summary>
        /// Convert input coordinate to system coordinate
        /// </summary>
        /// <returns>System coordinate</returns>
        private System.Numerics.Vector2 ScreenToCoord(int x, int y)
            => new System.Numerics.Vector2(
                x -Width / 2f,
                -y + Height / 2f
                );
    }
}
