using OpenTK;
using OpenTK.Graphics;

using PhysicsSim.VBOs;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PhysicsSim.Interactions
{
    public sealed class RectangularCheckBox : ARectangularInteraction, ICheckBox
    {
        public event EventHandler CheckBoxToggleEvent;

        private Color4 _checkColor;
        
        public Color4 CheckColor
        {
            get => _checkColor;
            set
            {
                _checkColor = value;
                LoadObject();
            }
        }

        #region Contructors

        public RectangularCheckBox(float width, float height, float lineWidth, Color4 fillColor, Color4 lineColor, Color4 checkColor, int program)
            : base(width, height, lineWidth, fillColor, lineColor, program)
        {
            _checkColor = checkColor;
            LoadObject();
        }

        public RectangularCheckBox(float width, float height, Color4 fillColor, Color4 checkColor, int program)
            : base(width, height, fillColor, program)
        {
            _checkColor = checkColor;
            LoadObject();
        }

        public RectangularCheckBox(RectangleF rectangle, float lineWidth, Color4 fillColor, Color4 lineColor, Color4 checkColor, int program)
            : base(rectangle, lineWidth, fillColor, lineColor, program)
        {
            _checkColor = checkColor;
            LoadObject();
        }

        public RectangularCheckBox(RectangleF rectangle, Color4 fillColor, Color4 checkColor, int program)
            : base(rectangle, fillColor, program)
        {
            _checkColor = checkColor;
            LoadObject();
        }

        #endregion


        public bool IsChecked { get; private set; } = false;

        public override void Dispose()
        {
            if (!_disposed)
            {
                _render.Dispose();
                _render = null;
                _disposed = true;
            }
        }

        public bool IsInside(System.Numerics.Vector2 coord) => Area.Contains(coord.X, coord.Y);

        public override void Render(ref Matrix4 projection, Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("_render");
            }
            _render.Render(ref projection, translation, rotation, scale);
        }

        public bool ToggleIfInside(System.Numerics.Vector2 coord)
        {
            if (IsInside(coord))
            {
                Toggle();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Toggle()
        {
            IsChecked ^= true;
            _render[_render.Count - 1].Enabled = IsChecked;
            Console.WriteLine(_render[_render.Count - 1].Enabled);
            CheckBoxToggleEvent?.Invoke(this, new EventArgs());
        }

        protected override void LoadObject()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("_render");
            }
            _render?.Dispose();
            var renders = new List<RenderObject>();
            renders.Add(new RenderObject(ObjectFactory.Rectangle(_width, _height, _fillColor), _program));
            if (_lineColor != null)
            {
                renders.Add(new RenderObject(ObjectFactory.RectangleEdge(_width, _height, _lineWidth, _lineColor.Value), _program));
            }
            renders.Add(new RenderObject(ObjectFactory.CheckMark(_width, _height, _checkColor), _program) { Enabled = IsChecked });
            _render = new ROCollection(renders)
            {
                Position = new Vector3(Position.X, Position.Y, 0)
            };
        }
    }
}
