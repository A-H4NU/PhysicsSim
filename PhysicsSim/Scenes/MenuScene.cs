using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

using PhysicsSim.Interactions;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsSim.Scenes
{
    public sealed class MenuScene : Scene
    {
        private readonly List<TexturedButton> _buttons;

        private readonly List<Scene> _scenes;
        
        public MenuScene(MainWindow window) : base(window)
        {
            _buttons = new List<TexturedButton>();
            _scenes = new List<Scene>()
            {
                this,
                new ElectroScene(_window),
                new SWScene(_window),
                new BeatScene(_window)
            };
        }

        protected override void OnResize(object sender, EventArgs e)
        {
            base.OnResize(sender, e);

            RearrangeButtons();
        }

        protected override void OnLoad(object sender, EventArgs e)
        {
            Color4 border = new Color4(0x50, 0xB0, 0xB2, 0xFF);
            _buttons.AddRange(new TexturedButton[]
            {
                new TexturedButton(500f, 500f, 5f, border, @"Textures\electro.jpg", _window.ColoredProgram, _window.TexturedProgram),
                new TexturedButton(500f, 500f, 5f, border, @"Textures\standing_wave.jpg", _window.ColoredProgram, _window.TexturedProgram),
                new TexturedButton(500f, 500f, 5f, border, @"Textures\beat_wave.jpg", _window.ColoredProgram, _window.TexturedProgram)
            });
            for (int i = 0; i < _buttons.Count; ++i)
            {;
                int idx = i;
                _buttons[i].ButtonPressEvent += (o, _) => ActivateScene(idx + 1);
            }
            RearrangeButtons();
        }

        private void ActivateScene(int index)
        {
            for (int i = 0; i < _scenes.Count; ++i)
            {
                _scenes[i].Enabled = false;
            }
            _scenes[index].Enabled = true;
            _scenes[index].Initialize();
        }

        private void RearrangeButtons()
        {
            int gridW = 2, gridH = 2;
            float tileside = (float)_window.Width / _window.Height < 1.5f ?
                _window.Width / gridW :
                _window.Height / gridH;
            tileside *= 0.95f;
            for (int i = 0; i < Math.Min(_buttons.Count, gridW * gridH); ++i)
            {
                int j = gridH - i / gridW - 1, k = i % gridW;
                RectangleF rectangle = new RectangleF(
                    0.5f * tileside * gridW * (2f * k / gridW - 1),
                    0.5f * tileside * gridH * (2f * j / gridH - 1),
                    tileside, tileside);
                _buttons[i].Area = rectangle;
            }
        }

        protected override void OnUpdateFrame(object sender, FrameEventArgs e)
        {
            if (Enabled)
            {
            }
        }

        protected override void OnRenderFrame(object sender, FrameEventArgs e)
        {
            if (Enabled)
            {
                GL.Clear(ClearBufferMask.ColorBufferBit);

                Matrix4 projection = MainWindow.GetProjection(_window.Width, _window.Height);
                foreach (var button in _buttons)
                {
                    button.Render(ref projection);
                }

                _window.SwapBuffers();
            }
        }

        protected override void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Enabled)
            {
                var pos = MainWindow.ScreenToCoord(e.X, e.Y, _window.Width, _window.Height);
                foreach (var button in _buttons)
                {
                    button.PressIfInside(pos);
                }
            }
        }

        protected override void OnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.Escape && !e.IsRepeat)
            {
                if (_scenes[0].Enabled)
                {
                    _window.Close();
                }
                else
                {
                    ActivateScene(0);
                }
            }
        }

        protected override void OnClosed(object sender, EventArgs e)
        {
            Dispose();
        }

        public override void Dispose()
        {
            foreach (var button in _buttons)
            {
                button.Dispose();
            }
            _buttons.Clear();
        }

        public override void Initialize()
        {
            
        }
    }
}
