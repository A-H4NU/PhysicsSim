using Hanu.ElectroLib.Objects;

using OpenTK;
using OpenTK.Graphics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroSim.VBOs
{
    public class RPhysicalObject : ARenderable
    {
        public static float Radius = 10f;

        public static float BorderThickness = 2f;

        public PhysicalObject PObject { get; private set; }

        private ROCollection _renderCollection;

        public RPhysicalObject(PhysicalObject pObject)
        {
            PObject = pObject;
            Position = new Vector3(pObject.X, PObject.Y, 0);
            PObject.PropertyChanged += PObject_PropertyChanged;

            _renderCollection = GetROCollection();
        }

        private void PObject_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Charge")
            {
                _renderCollection.Dispose();
                _renderCollection = GetROCollection();
            }
        }

        private ROCollection GetROCollection()
            => new ROCollection(
                    new RenderObject[]
                    {
                        new RenderObject(ObjectFactory.FilledCircle(
                            radius: Radius,
                            color: PObject.Charge == 0f ? Color4.Gray : (PObject.Charge > 0f ? Color4.Red : Color4.Blue))),
                        new RenderObject(ObjectFactory.HollowCircle(
                            radius: Radius, thickness: BorderThickness, color: Color4.White))
                    });

        public override void Dispose()
        {
            _renderCollection.Dispose();
        }

        public override void Render(ref Matrix4 projection, Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            _renderCollection.Position = new Vector3((float)PObject.X, (float)PObject.Y, 0);
            _renderCollection.Render(ref projection, translation, rotation, scale);
        }
    }
}
