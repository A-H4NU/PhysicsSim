using OpenTK;
using OpenTK.Graphics;

using System;
using System.Drawing;

namespace PhysicsSim.Interactions
{
    public class RectangularButton : ARectangularInteraction, IButton
    {
        public event EventHandler ButtonPressEvent;

        #region Contructors

        public RectangularButton(float width, float height, float lineWidth, Color4 fillColor, Color4 lineColor, int program)
            : base(width, height, lineWidth, fillColor, lineColor, program)
        {
            LoadObject();
        }

        public RectangularButton(float width, float height, Color4 fillColor, int program)
            : base(width, height, fillColor, program)
        {
            LoadObject();
        }

        public RectangularButton(RectangleF rectangle, float lineWidth, Color4 fillColor, Color4 lineColor, int program)
            : base(rectangle, lineWidth, fillColor, lineColor, program)
        {
            LoadObject();
        }

        public RectangularButton(RectangleF rectangle, Color4 fillColor, int program)
            : base(rectangle, fillColor, program)
        {
            LoadObject();
        }

        #endregion

        public bool IsInside(System.Numerics.Vector2 coord) => Area.Contains(coord.X, coord.Y);

        /// <exception cref="ObjectDisposedException"/>
        public override void Render(ref Matrix4 projection, Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("_render");
            }

            _render.Render(ref projection, translation, rotation, scale);
        }

        public override void Dispose()
        {
            if (!_disposed)
            {
                _render.Dispose();
                _render = null;
                _disposed = true;
            }
        }

        public bool PressIfInside(System.Numerics.Vector2 coord)
        {
            if (IsInside(coord))
            {
                Press();
                return true;
            }
            return false;
        }

        public void Press()
        {
            ButtonPressEvent?.Invoke(this, new EventArgs());
        }
    }
}
