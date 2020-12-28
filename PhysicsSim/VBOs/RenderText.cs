using OpenTK;
using OpenTK.Graphics.OpenGL4;

using PhysicsSim.Vertices;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsSim.VBOs
{
    public class RenderText : ARenderable
    {
        public const string Characters = @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 !@#$%^&*()_+-=[];',./{}:""<>?";
        private static readonly Dictionary<char, int> s_lookup;

        private readonly int[] _widths, _additiveWidths;
        private int _totalWidth;

        private readonly Color _bgColor, _color;
        private readonly int _texture, _program;

        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                var listRenderable = new TexturedRenderObject[value.Length];
                int[] idxs = new int[value.Length];
                float sumWidth = 0f;
                for (int i = 0; i < value.Length; ++i)
                {
                    if (s_lookup.TryGetValue(value[i], out idxs[i]))
                    {
                        int idx = idxs[i];
                        var tro = new TexturedRenderObject(
                            ObjectFactory.TexRectangle(
                                _widths[idx],
                                TotalSize.Height,
                                new RectangleF((float)_additiveWidths[idx] / _totalWidth, 0f, (float)_widths[idx] / _totalWidth, 1f)),
                            _texture,
                            _program);
                        listRenderable[i] = tro;
                        sumWidth += _widths[idx];
                    }
                }
                sumWidth *= 0.8f;
                float currentSum = 0;
                for (int i = 0; i < value.Length; ++i)
                {
                    listRenderable[i].Position = new Vector3(currentSum - sumWidth / 2f + _widths[idxs[i]] / 2f, 0, 0);
                    currentSum += _widths[idxs[i]] * 0.8f;
                }
                Size = new SizeF(currentSum, TotalSize.Height);
                _roc?.Dispose();
                _roc = new ROCollection(listRenderable);
                _roc.BindProperty(this, BindType.Position);
                _text = value;
            }
        }

        public SizeF TotalSize { get; private set; }

        public SizeF Size { get; private set; }

        public float Width => Size.Width;

        public float Height => Size.Height;

        private ROCollection _roc;

        static RenderText()
        {
            s_lookup = new Dictionary<char, int>();
            for (int i = 0; i < Characters.Length; ++i)
            {
                if (!s_lookup.ContainsKey(Characters[i]))
                {
                    s_lookup.Add(Characters[i], i);
                }
            }
        }

        public RenderText(int fontSize, string fontName, string text, Color bgColor, Color color, int program)
        {
            _color = color;
            _bgColor = bgColor;
            _widths = new int[Characters.Length];
            _additiveWidths = new int[Characters.Length];
            Bitmap bitmap = GenerateCharacters(fontSize, fontName, out Size totalSize);
            TotalSize = totalSize;
            _texture = TexturedRenderObject.InitTexture(bitmap);
            _program = program;
            Text = text;
        }

        private Bitmap GenerateCharacters(int fontSize, string fontName, out Size totalSize)
        {
            var characters = new List<Bitmap>();
            using (var font = new Font(fontName, fontSize))
            {
                for (int i = 0; i < Characters.Length; ++i)
                {
                    var charBmp = GenerateCharacter(font, Characters[i]);
                    characters.Add(charBmp);
                    _widths[i] = charBmp.Width;
                    _additiveWidths[(i+1) % Characters.Length] = charBmp.Width + _additiveWidths[i];
                }
                _totalWidth = _additiveWidths[0];
                _additiveWidths[0] = 0;
                totalSize = new Size(_totalWidth, characters.Max(x => x.Height));
                var charMap = new Bitmap(totalSize.Width, totalSize.Height);
                using (var gfx = Graphics.FromImage(charMap))
                {
                    gfx.FillRectangle(new SolidBrush(_bgColor), 0, 0, charMap.Width, charMap.Height);
                    for (int i = 0; i < characters.Count; ++i)
                    {
                        var c = characters[i];
                        gfx.DrawImageUnscaled(c, _additiveWidths[i], 0);
                        c.Dispose();
                    }
                }
                return charMap;
            }
        }

        private Bitmap GenerateCharacter(Font font, char c)
        {
            var size = GetSize(font, c);
            var bmp = new Bitmap((int)size.Width, (int)size.Height);
            using (var gfx = Graphics.FromImage(bmp))
            {
                gfx.FillRectangle(new SolidBrush(_bgColor), 0, 0, bmp.Width, bmp.Height);
                gfx.DrawString(c.ToString(), font, new SolidBrush(_color), 0, 0);
            }
            return bmp;
        }

        private SizeF GetSize(Font font, char c)
        {
            using (var bmp = new Bitmap(512, 512))
            {
                using (var gfx = Graphics.FromImage(bmp))
                {
                    return gfx.MeasureString(c.ToString(), font);
                }
            }
        }

        public override void Dispose()
        {
            _roc.Dispose();
            GL.DeleteTexture(_texture);
        }

        public override void Render(ref Matrix4 projection, Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            _roc.Render(ref projection, translation, rotation, scale);
        }
    }
}
