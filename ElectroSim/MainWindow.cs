using ElectroSim.VBOs;
using ElectroSim.Vertices;

using MathNet.Numerics.LinearAlgebra;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroSim
{
    public sealed class MainWindow : GameWindow
    {
        private List<ARenderable> _renderables;

        private int _program;

        public MainWindow(int width, int height)
            : base(width, height, new GraphicsMode(32, 24, 0, 8), "ElectroSim", GameWindowFlags.Default, DisplayDevice.Default)
        {
            return;
        }

        #region OpenTK Events

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(Color4.Aqua);

            _program = CreateProgram();
            _renderables = new List<ARenderable>(100);
            //Random random = new Random();
            //for (int i = 0; i < 50; ++i)
            //{
            //    var r = new RenderObject(ObjectFactory.FilledCircle(10f, Color4.Blue))
            //    {
            //        Position = new Vector3(random.Next(-Width / 2, Width / 2), random.Next(-Height / 2, Height / 2), 0)
            //    };
            //    _renderables.Add(r);
            //}
            //for (int i = 0; i < 50; ++i)
            //{
            //    var r = new RenderObject(ObjectFactory.FilledCircle(10f, Color4.Red))
            //    {
            //        Position = new Vector3(random.Next(-Width / 2, Width / 2), random.Next(-Height / 2, Height / 2), 0)
            //    };
            //    _renderables.Add(r);
            //}

            var y0 = CreateVector.Dense<double>(2);
            y0[0] = 0.01;
            var result = SecondOrder(y0, 0, f, endFunc);

            var newr = new List<Vector<double>>(result.Count);
            for (int i = 0; i < result.Count; ++i)
            {
                var add = CreateVector.Dense(
                    new double[] { (double)i / result.Count, result[i][0] }
                    );
                newr.Add(add);
            }

            _renderables.Add(new RenderObject(ObjectFactory.Curve(newr, Color4.Black))
            {
                Scale = new Vector3(100)
            }) ;

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.PatchParameter(PatchParameterInt.PatchVertices, 3);
        }

        /***************************   DELETE THIS **************************************************/

        public static Vector<double> f(double t, Vector<double> y)
            //=> CreateVector.Dense(new double[] { t + Math.Cos(t), t - Math.Sin(t) });
            => y;

        public static bool endFunc(Vector<double> y) => y.L2Norm() >= 5;

        public static List<Vector<double>> SecondOrder(Vector<double> y0, double start, Func<double, Vector<double>, Vector<double>> f, Func<Vector<double>, bool> endFunc, double delta = 1e-4)
        {
            double num = delta;
            List<Vector<double>> vectorArrays = new List<Vector<double>>();
            double num1 = start;
            vectorArrays.Add(y0);
            while (endFunc(vectorArrays.Last()) == false)
            {
                Vector<double> nums = f(num1, y0);
                Vector<double> nums1 = f(num1, y0 + (nums * num));
                vectorArrays.Add(y0 + (num * 0.5 * (nums + nums1)));
                num1 += num;
                y0 = vectorArrays.Last();
            }
            return vectorArrays;
        }

        /********************************************************************************************/

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

            Matrix4 projection = GetProjection();
            GL.UseProgram(_program);
            foreach (var render in _renderables)
            {
                render.Render(ref projection);
            }

            SwapBuffers();
        }

        #endregion

        private void HandleKeyboard()
        {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Key.Escape))
            {
                Close();
            }
        }

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
    }
}
