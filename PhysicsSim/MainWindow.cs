using PhysicsSim.Scenes;
using PhysicsSim.VBOs;
using PhysicsSim.Vertices;

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

namespace PhysicsSim
{
    public sealed class MainWindow : GameWindow
    {
        public static int ModelviewLocation = 10;

        public static int ProjectionLocation = 11;

        public int Program { get; private set; }

        private readonly Timer _timer;

        private ElectroScene _es;

        public MainWindow(int width, int height)
            : base(width, height, new GraphicsMode(32, 24, 0, 8), "PhysicsSim", GameWindowFlags.Default, DisplayDevice.Default)
        {
            // Create timer that perform a specific function
            _timer = new Timer(5000);
            _timer.Elapsed += (o, e) => Console.WriteLine($"total memory using at {e.SignalTime:HH:mm:ss:fff}: {GC.GetTotalMemory(true)} bytes");
            _timer.Start();

            _es = new ElectroScene(this);
            _es.Enabled = true;
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
            Program = CreateProgram();

            // Fill each face no matter it is a front face or not
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.PatchParameter(PatchParameterInt.PatchVertices, 3);

            GL.LineWidth(2f);
        }

        /// <summary>
        /// Called when the window is resized
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            // change the viewport (where rendered images are represented)
            GL.Viewport(ClientRectangle);
        }

        

        public double Time { get; private set; } = 0.0;
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            Time += e.Time;
            HandleKeyboard();
        }

        /// <summary>
        /// Called when the window has closed, dispose variables that are needed to be cleared
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
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
    }
}
