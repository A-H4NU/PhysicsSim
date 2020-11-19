using OpenTK;

using System;
using System.Collections.Generic;

namespace PhysicsSim.VBOs
{
    public class ROCollection : ARenderable
    {
        private List<ARenderable> _rObj;

        public ARenderable this[int index]
        {
            get => _rObj[index];
        }

        public int Count { get => _rObj.Count; }

        private ROCollection()
        {
            _rObj = new List<ARenderable>();
        }

        public ROCollection(IEnumerable<ARenderable> renderObjects) : this()
        {
            _rObj.AddRange(renderObjects);
        }

        public override void Dispose()
        {
            foreach (ARenderable ro in _rObj)
            {
                ro.Dispose();
            }
            _rObj.Clear();
            _rObj = null;
        }

        public override void Render(ref Matrix4 projection, Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            foreach (ARenderable ro in _rObj)
            {
                ro.Render(
                    ref projection,
                    translation + Position,
                    rotation + Rotation,
                    scale * Scale);
            }
        }
    }
}
