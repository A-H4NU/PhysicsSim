using OpenTK;
using OpenTK.Graphics.ES11;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroSim.VBOs
{
    public class ROCollection : ARenderable
    {
        private List<RenderObject> _rObj;

        public ROCollection()
        {
            _rObj = new List<RenderObject>();
        }

        public ROCollection(IEnumerable<RenderObject> renderObjects) : this()
        {
            _rObj.AddRange(renderObjects);
        }

        public override void Dispose()
        {
            foreach (var ro in _rObj)
            {
                ro.Dispose();
            }
            _rObj.Clear();
            _rObj = null;
        }

        public override void Render(ref Matrix4 projection, Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            foreach (var ro in _rObj)
            {
                ro.Render(ref projection, translation, rotation, scale);
            }
        }
    }
}
