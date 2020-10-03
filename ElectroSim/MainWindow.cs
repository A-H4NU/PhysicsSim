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
        public static int ModelviewLocation = 10;

        public static int ProjectionLocation = 11;

        public static float Scale = 50f;

        public static float MaxT = 100f;

        public static int LinePerUnitCharge = 6;

        private List<ARenderable> _renderables;

        private List<RPhysicalObject> _pObjs;

        private int _program;

        private readonly Timer _timer;

        public MainWindow(int width, int height)
            : base(width, height, new GraphicsMode(32, 24, 0, 8), "ElectroSim", GameWindowFlags.Default, DisplayDevice.Default)
        {
            _timer = new Timer(5000);
            _timer.Elapsed += (o, e) => Console.WriteLine($"total memory using at {e.SignalTime:HH:mm:ss:fff}: {GC.GetTotalMemory(true)} bytes");
            _timer.Start();
        }

        #region OpenTK Events

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(new Color4(0.1f, 0.1f, 0.4f, 1.0f));

            _program = CreateProgram();
            _renderables = new List<ARenderable>();
            _pObjs = new List<RPhysicalObject>();

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.PatchParameter(PatchParameterInt.PatchVertices, 3);
            
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(ClientRectangle);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            HandleKeyboard();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.UseProgram(_program);
            Matrix4 projection = GetProjection();
            GL.UniformMatrix4(ProjectionLocation, false, ref projection);
            foreach (ARenderable render in _renderables)
            {
                render.Render();
            }
            foreach (RPhysicalObject obj in _pObjs)
            {
                obj.Render();
            }

            SwapBuffers();
        }
        protected override void OnClosed(EventArgs e)
        {
            foreach (ARenderable obj in _renderables)
            {
                obj.Dispose();
            }
            foreach (RPhysicalObject obj in _pObjs)
            {
                obj.Dispose();
            }
            _renderables.Clear();
            _pObjs.Clear();
            _renderables = null;
            _pObjs = null;
            GC.Collect();
        }

        #endregion

        #region Input Handling

        private void HandleKeyboard()
        {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Key.Escape))
            {
                Close();
            }
        }

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
                if (_pObjs.Extracted().Any((p) => p.Charge != 0))
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    _renderables.Clear();
                    int linecount = 0;
                    foreach (var obj in _pObjs)
                    {
                        if (obj.PObject.Charge <= 0) continue;
                        linecount += (int)(obj.PObject.Charge / 1e-6f) * LinePerUnitCharge;
                    }
                    var taskList = new List<Task<List<System.Numerics.Vector2>>>(linecount);
                    foreach (var obj in _pObjs)
                    {
                        if (obj.PObject.Charge < 0) continue;
                        int units = (int)(obj.PObject.Charge / 1e-6f);
                        for (int i = 0; i < units * LinePerUnitCharge; ++i)
                        {
                            double angle = 2*i*Math.PI / (units*LinePerUnitCharge);
                            var delta = new System.Numerics.Vector2(
                                (float)Math.Cos(angle), (float)Math.Sin(angle)) * RPhysicalObject.Radius / Scale;
                            var task = PLine.ElectricFieldLineAsync(
                                system: _pObjs.Extracted(),
                                initPos: obj.PObject.Position + delta,
                                endFunc: (t, v)
                                    => t > MaxT ||
                                    !(-Width / Scale < v.X && v.X < Width / Scale && -Height / Scale < v.Y && v.Y < Height / Scale),
                                startFromNegative: false,
                                delta: 1e-3f);
                            taskList.Add(task);
                        }
                    }
                    while (taskList.Any())
                    {
                        var finished = await Task.WhenAny(taskList);
                        taskList.Remove(finished);
                        var result = await finished;
                        _renderables.Add(new RenderObject(ObjectFactory.Curve(result, Color4.White)) { Scale = new Vector3(Scale, Scale, 1) });
                    }
                    stopwatch.Stop();
                    Console.WriteLine($"Time elapsed: {stopwatch.ElapsedMilliseconds} ms");
                }
            }
        }

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
                    Console.WriteLine($"Electric field at {pos}: {ef}");
                }
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (_pObjs.Count > 0)
            {
                _pObjs.Last().PObject.Charge += e.Delta * 1e-6f;
            }
        }

        #endregion

        #region Shader Handling

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

        private Matrix4 GetProjection()
        {
            return Matrix4.CreateOrthographic(Width, Height, -1f, 1f);
        }

        private double Difference(ColoredVertex[] v1, ColoredVertex[] v2)
        {
            double res = 0f;
            for (int i = 0; i < v1.Length; ++i)
            {
                res += (v1[i].Position - v2[i].Position).LengthSquared;
                Console.WriteLine($"{v1,15}{v2,15}");
            }
            return (double)Math.Sqrt(res);
        }

        private System.Numerics.Vector2 ScreenToCoord(int x, int y)
            => new System.Numerics.Vector2(
                x -Width / 2f,
                -y + Height / 2f
                );
    }

    public static class MyExtension
    {
        public static System.Numerics.Vector2 SetLength(this System.Numerics.Vector2 vector, float length)
            => vector / vector.Length() * length;
    }
}
