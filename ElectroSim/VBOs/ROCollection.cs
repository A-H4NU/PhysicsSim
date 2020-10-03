using OpenTK;

using System.Collections.Generic;

namespace ElectroSim.VBOs
{
    public class ROCollection : ARenderable
    {
        private List<ARenderable> _rObj;

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

        public override void Render(Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            foreach (ARenderable ro in _rObj)
            {
                ro.Render(
                    translation + Position,
                    rotation + Rotation,
                    scale * Scale);
            }
        }
    }
}
