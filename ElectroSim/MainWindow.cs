using ElectroSim.VBOs;
using ElectroSim.Vertices;

using Hanu.ElectroLib.Objects;
using Hanu.ElectroLib.Physics;

using MathNet.Numerics.LinearAlgebra;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ElectroSim
{
    public sealed class MainWindow : GameWindow
    {
        private List<ARenderable> _renderables;

        private List<RPhysicalObject> _pObjs;

        private int _program;

        private Timer _timer;

        public MainWindow(int width, int height)
            : base(width, height, new GraphicsMode(32, 24, 0, 8), "ElectroSim", GameWindowFlags.Default, DisplayDevice.Default)
        {
            _timer = new Timer(1000);
            _timer.Elapsed += (o, e) =>
            {
                Console.WriteLine($"total memory using at {e.SignalTime:HH:mm:ss:fff}: {GC.GetTotalMemory(true)} bytes");
            };
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
            HandleInput();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            Matrix4 projection = GetProjection();
            GL.UseProgram(_program);
            foreach (var render in _renderables)
            {
                render.Render(ref projection);
            }
            foreach (var obj in _pObjs)
            {
                obj.Render(ref projection);
            }

            SwapBuffers();
        }

        #endregion

        #region Input Handling

        private void HandleInput()
        {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Key.Escape))
            {
                Close();
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            var pos = this.ScreenToCoord(e.X, e.Y);
            if (e.Button == MouseButton.Left)
            {
                var obj = new FixedObject(pos);
                _pObjs.Add(new RPhysicalObject(obj));
                Console.WriteLine($"Added object at {pos}");
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            _pObjs.Last().PObject.Charge += e.Delta * 1e-6f;
        }

        #endregion

        #region Shader Handling

        private int CompileShader(ShaderType type, string filepath)
        {
            var shader = GL.CreateShader(type);
            var source = File.ReadAllText(filepath);
            GL.ShaderSource(shader, source);
            GL.CompileShader(shader);
            var info = GL.GetShaderInfoLog(shader);
            if (!string.IsNullOrWhiteSpace(info))
                Console.WriteLine($"GL.CompileShader [{type}] had info log: {info}");
            return shader;
        }

        private int CreateProgram()
        {
            var program = GL.CreateProgram();
            var shaders = new List<int>();
            shaders.Add(CompileShader(ShaderType.VertexShader, @"Shaders\vertex_shader.vert"));
            shaders.Add(CompileShader(ShaderType.FragmentShader, @"Shaders\fragment_shader.frag"));

            foreach (var shader in shaders)
                GL.AttachShader(program, shader);
            GL.LinkProgram(program);
            var info = GL.GetProgramInfoLog(program);
            if (!string.IsNullOrWhiteSpace(info))
                Console.WriteLine($"GL.LinkProgram had info log: {info}");
            foreach (var shader in shaders)
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

        protected override void OnClosed(EventArgs e)
        {
            foreach (var obj in _renderables)
            {
                obj.Dispose();
            }
            foreach (var obj in _pObjs)
            {
                obj.Dispose();
            }
            _renderables.Clear();
            _pObjs.Clear();
            _renderables = null;
            _pObjs = null;
            GC.Collect();
        }
    }
}
