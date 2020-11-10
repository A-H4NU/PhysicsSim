using OpenTK;
using OpenTK.Graphics;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsSim.Interactions
{
    public class TexturedButton : ATexturedInteraction, IButton
    {
        public event EventHandler ButtonPressEvent;

        public bool IsNull()
        {
            return ButtonPressEvent == null;
        }

        #region Constructors

        public TexturedButton(float width, float height, float lineWidth, Color4 lineColor, string filepath, int coloredProgram, int textureProgram)
            : base(width, height, lineWidth, lineColor, filepath, coloredProgram, textureProgram)
        {
            LoadObject();
        }

        public TexturedButton(float width, float height, string filepath, int coloredProgram, int textureProgram)
            : base(width, height, filepath, coloredProgram, textureProgram)
        {
            LoadObject();
        }

        public TexturedButton(RectangleF rec, float lineWidth, Color4 lineColor, string filepath, int coloredProgram, int textureProgram)
            : base(rec, lineWidth, lineColor, filepath, coloredProgram, textureProgram)
        {
            LoadObject();
        }

        public TexturedButton(RectangleF rec, string filepath, int coloredProgram, int textureProgram)
            : base(rec, filepath, coloredProgram, textureProgram)
        {
            LoadObject();
        }

        #endregion

        public bool IsInside(System.Numerics.Vector2 coord) => Area.Contains(coord.X, coord.Y);

        public void Press()
        {
            ButtonPressEvent?.Invoke(this, new EventArgs());
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

        public override void Render(ref Matrix4 projection, Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("_render");
            }
            _render.Render(ref projection, translation, rotation, scale);
        }
    }
}
