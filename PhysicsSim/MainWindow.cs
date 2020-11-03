using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

using PhysicsSim.Scenes;
using PhysicsSim.VBOs;

using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;

namespace PhysicsSim
{
    public sealed class MainWindow : GameWindow
    {
        public static readonly int ModelviewLocation = 10;

        public static readonly int ProjectionLocation = 11;

        public static readonly string ColoredVertexShaderPath = @"Shaders\colored_vertex_shader.vert";
        public static readonly string ColoredFragmentShaderPath = @"Shaders\colored_fragment_shader.frag";
        public static readonly string TexturedVertexShaderPath = @"Shaders\textured_vertex_shader.vert";
        public static readonly string TexturedFragmentShaderPath = @"Shaders\textured_fragment_shader.frag";

        public int ColoredProgram { get; private set; }
        public int TexturedProgram { get; private set; }

        private readonly Timer _timer;

        private Scene _es;

        private TexturedRenderObject _tro;

        private string _title;

        public MainWindow(int width, int height)
            : base(width, height, new GraphicsMode(32, 24, 0, 8), "PhysicsSim", GameWindowFlags.Default, DisplayDevice.Default)
        {
            // Create timer that perform a specific function
            _timer = new Timer(5000);
            _timer.Elapsed += (o, e) => Console.WriteLine($"total memory using at {e.SignalTime:HH:mm:ss:fff}: {GC.GetTotalMemory(true)} bytes");
            _timer.Start();

            _es = new SWScene(this) { Enabled = true };

            _title = "PhysicsSim";
        }

        // Contains overrided methods from OpenTK to render
        #region OpenTK Events

        /// <summary>
        /// Called when <see cref="GameWindow"/> is loading, initialize and load materials needed for the application to run
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            // Initialize the default background color
            GL.ClearColor(new Color4(0.1f, 0.1f, 0.4f, 1.0f));

            // Create a new program that is used when rendering
            ColoredProgram = CreateProgram(ColoredVertexShaderPath, ColoredFragmentShaderPath);
            TexturedProgram = CreateProgram(TexturedVertexShaderPath, TexturedFragmentShaderPath);

            _tro = new TexturedRenderObject(ObjectFactory.TexRectangle(900f, 900f), @"Textures\unnamed3.jpeg", TexturedProgram);

            // Fill each face no matter it is a front face or not
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.PatchParameter(PatchParameterInt.PatchVertices, 3);

            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.LineSmooth);

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.LineWidth(2f);

            base.OnLoad(e);
        }

        /// <summary>
        /// Called when the window is resized
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            // change the viewport (where rendered images are represented)
            GL.Viewport(ClientRectangle);

            base.OnResize(e);
        }



        public double Time { get; private set; } = 0.0;
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            Time += e.Time;
            HandleKeyboard();

            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Title = $"{_title} ({RenderFrequency:F0} FPS)";

            base.OnRenderFrame(e);

            //GL.Clear(ClearBufferMask.ColorBufferBit);

            //Matrix4 projection = Matrix4.CreateOrthographic(Width, Height, -1f, 1f);

            //_tro.Render(ref projection);

            //SwapBuffers();
        }

        /// <summary>
        /// Called when the window has closed, dispose variables that are needed to be cleared
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            // Forces an immediate garbage collection of all generations
            GC.Collect();

            base.OnClosed(e);
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

        #endregion

        // Contains methods to create a program and compile shaders
        #region Shader Handling

        /// <summary>
        /// Compiling a shader from the filepath
        /// </summary>
        /// <returns>The ID of the compiled shader</returns>
        private static int CompileShader(ShaderType type, string filepath)
        {
            int shader = GL.CreateShader(type);
            string source = File.ReadAllText(filepath);
            GL.ShaderSource(shader, source);
            GL.CompileShader(shader);
            string info = GL.GetShaderInfoLog(shader);
            if (!String.IsNullOrWhiteSpace(info))
            {
                Console.WriteLine($"GL.CompileShader [{type}] had info log: {info}");
            }

            return shader;
        }

        /// <summary>
        /// Create a new program that contains <see cref="ShaderType.VertexShader"/> and <see cref="ShaderType.FragmentShader"/>
        /// </summary>
        /// <returns>The ID of the created program</returns>
        public static int CreateProgram(string vertexPath, string fragmentPath)
        {
            int program = GL.CreateProgram();
            List<int> shaders = new List<int>
            {
                CompileShader(ShaderType.VertexShader, vertexPath),
                CompileShader(ShaderType.FragmentShader, fragmentPath)
            };

            foreach (int shader in shaders)
            {
                GL.AttachShader(program, shader);
            }

            GL.LinkProgram(program);
            string info = GL.GetProgramInfoLog(program);
            if (!String.IsNullOrWhiteSpace(info))
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
        public static Matrix4 GetProjection(float width, float height)
            => Matrix4.CreateOrthographic(width, height, -1f, 1f);

        /// <summary>
        /// Convert input coordinate to system coordinate
        /// </summary>
        /// <returns>System coordinate</returns>
        public static System.Numerics.Vector2 ScreenToCoord(int x, int y, float width, float height)
            => new System.Numerics.Vector2(x - width / 2f, -y + height / 2f);
    }
}
