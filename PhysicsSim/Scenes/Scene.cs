using OpenTK;
using OpenTK.Input;

using System;

namespace PhysicsSim.Scenes
{
    public abstract class Scene : IDisposable
    {
        protected readonly MainWindow _window;

        protected MainWindow Window { get => _window; }

        public float Time;

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
            _window.MouseUp += OnMouseUp;
            _window.MouseMove += OnMouseMove;
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
        protected virtual void OnResize(object sender, EventArgs e) { }

        /// <summary>
        /// Called before <see cref="OnRenderFrame(FrameEventArgs)"/>, to update variables by time and input
        /// <para>Do work only when <see cref="Enabled"/> == <see cref="true"/></para>
        /// </summary>
        protected virtual void OnUpdateFrame(object sender, FrameEventArgs e)
        {
            Time += (float)e.Time;
        }

        /// <summary>
        /// Called to do render works
        /// <para>Do work only when <see cref="Enabled"/> == <see cref="true"/></para>
        /// </summary>
        protected abstract void OnRenderFrame(object sender, FrameEventArgs e);

        /// <summary>
        /// Called when the window is closed
        /// </summary>
        protected abstract void OnClosed(object sender, EventArgs e);

        /// <summary>
        /// Called when a click is detedted
        /// <para>Do work only when <see cref="Enabled"/> == <see cref="true"/></para>
        /// </summary>
        protected virtual void OnMouseDown(object sender, MouseButtonEventArgs e) { }

        /// <summary>
        /// Called when a mouse wheel movement is detedted
        /// <para>Do work only when <see cref="Enabled"/> == <see cref="true"/></para>
        /// </summary>
        protected virtual void OnMouseWheel(object sender, MouseWheelEventArgs e) { }

        /// <summary>
        /// Called when a keyboard input is detedted
        /// <para>Do work only when <see cref="Enabled"/> == <see cref="true"/></para>
        /// </summary>
        protected virtual void OnKeyDown(object sender, KeyboardKeyEventArgs e) { }

        /// <summary>
        /// Called when a mouse up is detedted
        /// <para>Do work only when <see cref="Enabled"/> == <see cref="true"/></para>
        /// </summary>
        protected virtual void OnMouseUp(object sender, MouseButtonEventArgs e) { }

        /// <summary>
        /// Called when a mouse movement is detedted
        /// <para>Do work only when <see cref="Enabled"/> == <see cref="true"/></para>
        /// </summary>
        protected virtual void OnMouseMove(object sender, MouseMoveEventArgs e) { }

        public abstract void Initialize();

        public abstract void Dispose();
    }
}
