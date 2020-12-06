using OpenTK;

using PhysicsSim.ComponentModel;
using PhysicsSim.Events;
using PhysicsSim.Exceptions;

using System;

namespace PhysicsSim.VBOs
{
    public abstract class ARenderable : IDisposable, INotifyRenderPropertyChanged
    {
        public bool Enabled { get; set; } = true;

        private Vector3 _position = Vector3.Zero;
        public virtual Vector3 Position
        {
            get => _position;
            set
            {
                var old = _position;
                _position = value;
                PositionChangedEvent?.Invoke(this, new RenderChangedEventArgs(old, value));
            }
        }

        private Vector3 _rotation = Vector3.Zero;
        public virtual Vector3 Rotation
        {
            get => _rotation;
            set
            {
                var old = _rotation;
                _rotation = value;
                RotationChangedEvent?.Invoke(this, new RenderChangedEventArgs(old, value));
            }
        }

        private Vector3 _scale = Vector3.One;
        public virtual Vector3 Scale
        {
            get => _scale;
            set
            {
                var old = _scale;
                _scale = value;
                ScaleChangedEvent?.Invoke(this, new RenderChangedEventArgs(old, value));
            }
        }
        
        public event EventHandler<RenderChangedEventArgs> PositionChangedEvent;
        public event EventHandler<RenderChangedEventArgs> RotationChangedEvent;
        public event EventHandler<RenderChangedEventArgs> ScaleChangedEvent;

        protected BindType _boundFlag = BindType.None;

        public abstract void Dispose();

        /// <summary>
        /// Render with no additional transition, rotation, or scaling
        /// </summary>
        public virtual void Render(ref Matrix4 projection) => Render(ref projection, Vector3.Zero, Vector3.Zero, Vector3.One);

        /// <summary>
        /// Render with additional transition, rotation, or scaling
        /// </summary>
        /// <param name="translation">additional transition</param>
        /// <param name="rotation">additional rotation</param>
        /// <param name="scale">additional scaling</param>
        public abstract void Render(ref Matrix4 projection, Vector3 translation, Vector3 rotation, Vector3 scale);

        /// <summary>
        /// Return new modelview matrix with no additional transition, rotation, or scaling
        /// </summary>
        public virtual Matrix4 GetModelView() => GetModelView(Vector3.Zero, Vector3.Zero, Vector3.One);

        /// <summary>
        /// Return new modelview matrix with additional transition, rotation, or scaling
        /// </summary>
        /// <param name="translation">additional transition</param>
        /// <param name="rotation">additional rotation</param>
        /// <param name="scale">additional scaling</param>
        /// <returns><see cref="Matrix4.Zero"/> when not enabled</returns>
        public virtual Matrix4 GetModelView(Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            if (!Enabled) return Matrix4.Zero;
            Matrix4 t = Matrix4.CreateTranslation(Position + translation);
            Matrix4 r = Matrix4.CreateRotationX(Rotation.X + rotation.X)
                * Matrix4.CreateRotationY(Rotation.Y + rotation.Y)
                * Matrix4.CreateRotationZ(Rotation.Z + rotation.Z);
            Matrix4 s = Matrix4.CreateScale(Scale * scale);
            return s * r * t;
        }

        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="AlreadyBoundException"></exception>
        public virtual void BindProperty(INotifyRenderPropertyChanged renderObject, BindType bindType)
        {
            if (ReferenceEquals(this, renderObject))
            {
                throw new ArgumentException();
            }
            switch (bindType)
            {
                case BindType.None:
                    return;
                case BindType.Position:
                    if ((_boundFlag & BindType.Position) != BindType.None)
                    {
                        throw new AlreadyBoundException();
                    }
                    Position = renderObject.Position;
                    renderObject.PositionChangedEvent += (o, e) => Position = e.NewValue;
                    _boundFlag |= BindType.Position;
                    break;
                case BindType.Rotation:
                    if ((_boundFlag & BindType.Rotation) != BindType.None)
                    {
                        throw new AlreadyBoundException();
                    }
                    Rotation = renderObject.Rotation;
                    renderObject.RotationChangedEvent += (o, e) => Rotation = e.NewValue;
                    _boundFlag |= BindType.Rotation;
                    break;
                case BindType.Scale:
                    if ((_boundFlag & BindType.Scale) != BindType.None)
                    {
                        throw new AlreadyBoundException();
                    }
                    Scale = renderObject.Scale;
                    renderObject.ScaleChangedEvent += (o, e) => Scale = e.NewValue;
                    _boundFlag |= BindType.Scale;
                    break;
                default:
                    throw new ArgumentException("bindType");
            }
        }
    }

    [Flags]
    public enum BindType
    {
        None = 0, Position = 1, Rotation = 2, Scale = 4
    }
}
