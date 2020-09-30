﻿using OpenTK;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroSim.VBOs
{
    public abstract class ARenderable : IDisposable
    {
        public Vector3 Position { get; set; } = Vector3.Zero;
        public Vector3 Rotation { get; set; } = Vector3.Zero;
        public Vector3 Scale { get; set; } = Vector3.One;

        public abstract void Dispose();

        public virtual void Render(ref Matrix4 projection) => Render(ref projection, Vector3.Zero, Vector3.Zero, Vector3.One);

        public abstract void Render(ref Matrix4 projection, Vector3 translation, Vector3 rotation, Vector3 scale);

        protected virtual Matrix4 GetModelView() => GetModelView(Vector3.Zero, Vector3.Zero, Vector3.One);

        protected virtual Matrix4 GetModelView(Vector3 translation, Vector3 rotation, Vector3 scale)
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
