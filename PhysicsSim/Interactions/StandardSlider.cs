using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

using PhysicsSim.Events;
using PhysicsSim.VBOs;
using PhysicsSim.Vertices;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsSim.Interactions
{
    public class StandardSlider : ARenderable, ISlider
    {
        private readonly int _program;

        public bool Selected { get; set; }

        protected float _value;
        /// <summary>This must be between <see cref="MaxValue"/> and <see cref="MinValue"/></summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public float Value
        {
            get => _value;
            set
            {
                if (value > _max || value < _min)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                float old = _value;
                _value = value;
                ValueChangedEvent?.Invoke(this, new ValueChangedEventArgs<float>(old, value));
                MoveRender();
            }
        }

        protected float _max;
        public float MaxValue
        {
            get => _max;
            set
            {
                _max = value;
                MoveRender();
            }
        }

        protected float _min;
        public float MinValue
        {
            get => _min;
            set
            {
                _min = value;
                MoveRender();
            }
        }

        public readonly Color4 LineColor;

        public readonly Color4 SliderColor;

        protected float _width;
        public float Width
        {
            get => _width;
            set
            {
                _width = value;
            }
        }

        protected float _height;
        public float Height
        {
            get => _height;
            set
            {
                _height = value;
            }
        }

        protected float _sliderW;
        public float SliderWidth
        {
            get => _sliderW;
            set
            {
                _sliderW = value;
            }
        }

        public RectangleF Area
        {
            get
            {
                float t = (_value - _min) / (_max - _min);
                return new RectangleF(Position.X - _width / 2f + (_width - _sliderW) * t, Position.Y -_height / 2f, _sliderW, _height);
            }
        }

        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public StandardSlider(float width, float height, float sliderWidth, float min, float max, Color4 slider, Color4 line, int program)
        {
            if (sliderWidth >= width)
                throw new ArgumentOutOfRangeException("sliderWidth", "sliderWidth cannot be larger than or equal to width");
            _value = _min = min;
            _sliderW = sliderWidth;
            _max = max;
            LineColor = line;
            SliderColor = slider;
            _width = width;
            _height = height;
            _program = program;
            LoadObject();
        }

        private ROCollection _render;

        public event EventHandler<ValueChangedEventArgs<float>> ValueChangedEvent;

        public override void Dispose()
        {
            _render?.Dispose();
        }

        public bool IsInside(System.Numerics.Vector2 coord) => Area.Contains(coord.X, coord.Y);

        public override void Render(ref Matrix4 projection, Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            _render.Render(ref projection, translation, rotation, scale);
        }

        public bool SelectIfInside(System.Numerics.Vector2 pos)
        {
            if (IsInside(pos))
            {
                Selected = true;
                return true;
            }
            return false;
        }

        public void Unselect()
        {
            Selected = false;
        }

        public bool SlideIfSelected(System.Numerics.Vector2 pos)
        {
            if (IsInside(pos) && Selected)
            {
                float t = (pos.X - (Position.X - _width / 2f + _sliderW / 2f)) / (_width - _sliderW);
                t = t < 0 ? 0 : (t > 1 ? 1 : t);
                Value = _min + (_max - _min) * t;
                return true;
            }
            return false;
        }

        private void LoadObject()
        {
            var render = new ARenderable[]
            {
                new RenderObject(
                    (new ColoredVertex[]
                    {
                        new ColoredVertex(new Vector4(-_width / 2f, 0f, 0f, 1f), LineColor),
                        new ColoredVertex(new Vector4(+_width / 2f, 0f, 0f, 1f), LineColor)
                    },
                    PrimitiveType.Lines), _program),
                new RenderObject(ObjectFactory.Rectangle(_sliderW, _height, SliderColor), _program),
                new RenderObject(
                    (new ColoredVertex[]
                    {
                        new ColoredVertex(new Vector4(-_width / 2f, -_height / 2f, 0f, 1f), LineColor),
                        new ColoredVertex(new Vector4(-_width / 2f, +_height / 2f, 0f, 1f), LineColor),
                        new ColoredVertex(new Vector4(+_width / 2f, -_height / 2f, 0f, 1f), LineColor),
                        new ColoredVertex(new Vector4(+_width / 2f, +_height / 2f, 0f, 1f), LineColor),
                    },
                    PrimitiveType.Lines), _program)
            };
            _render = new ROCollection(render);
            _render.BindProperty(this, BindType.Position);
            MoveRender();
        }

        private void MoveRender()
        {
            float t = (_value - _min) / (_max - _min);
            _render[1].Position = new Vector3((_width - _sliderW) * (t - 0.5f), 0f, 0f);
        }
    }
}
