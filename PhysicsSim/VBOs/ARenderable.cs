using OpenTK;

using System;

namespace PhysicsSim.VBOs
{
    public abstract class ARenderable : IDisposable
    {
        public Vector3 Position { get; set; } = Vector3.Zero;
        public Vector3 Rotation { get; set; } = Vector3.Zero;
        public Vector3 Scale { get; set; } = Vector3.One;

        public abstract void Dispose();

        /// <summary>
        /// Render with no additional transition, rotation, or scaling
        /// </summary>
        public virtual void Render() => Render(Vector3.Zero, Vector3.Zero, Vector3.One);

        /// <summary>
        /// Render with additional transition, rotation, or scaling
        /// </summary>
        /// <param name="translation">additional transition</param>
        /// <param name="rotation">additional rotation</param>
        /// <param name="scale">additional scaling</param>
        public abstract void Render(Vector3 translation, Vector3 rotation, Vector3 scale);

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
        /// <returns></returns>
        public virtual Matrix4 GetModelView(Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            Matrix4 t = Matrix4.CreateTranslation(Position + translation);
            Matrix4 r = Matrix4.CreateRotationX(Rotation.X + rotation.X)
                * Matrix4.CreateRotationY(Rotation.Y + rotation.Y)
                * Matrix4.CreateRotationZ(Rotation.Z + rotation.Z);
            Matrix4 s = Matrix4.CreateScale(Scale * scale);
            return t * r * s;
        }
    }
}
