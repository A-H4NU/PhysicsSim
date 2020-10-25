using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsSim.Scenes
{
    public abstract class Scene : IDisposable
    {
        protected readonly MainWindow _window;

        public double Time { get => _window.Time; }

        public bool Enabled { get; set; } = false;

        public Scene(MainWindow window)
        {
            _window = window;

            // Add local methods as events
            _window.Load += OnLoad;
            _window.Resize += OnResize;
            _window.UpdateFrame += OnUpdateFrame;
            _window.RenderFrame += OnRenderFrame;
            _window.Closed += OnClosed;
            _window.MouseDown += OnMouseDown;
            _window.MouseWheel += OnMouseWheel;
            _window.KeyDown += OnKeyDown;
        }

        /// <summary>
        /// Called when <see cref="GameWindow"/> is loading, initialize and load materials needed for the application to run
        /// </summary>
        protected abstract void OnLoad(object sender, EventArgs e);

        /// <summary>
        /// Called when the window is resized
        /// </summary>
        protected virtual void OnResize(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Called before <see cref="OnRenderFrame(FrameEventArgs)"/>, to update variables by time and input
        /// </summary>
        protected abstract void OnUpdateFrame(object sender, FrameEventArgs e);

        /// <summary>
        /// Called to do render works
        /// </summary>
        protected abstract void OnRenderFrame(object sender, FrameEventArgs e);

        /// <summary>
        /// Called when the window is closed
        /// </summary>
        protected abstract void OnClosed(object sender, EventArgs e);

        protected virtual void OnMouseDown(object sender, MouseButtonEventArgs e) { }

        protected virtual void OnMouseWheel(object sender, MouseWheelEventArgs e) { }



        protected virtual void OnKeyDown(object sender, KeyboardKeyEventArgs e) { }

        public abstract void Dispose();
    }
}
