using PhysicsSim.Scenes;

using Hanu.ElectroLib.Objects;

using OpenTK;
using OpenTK.Graphics;

using System.Collections.Generic;
using System.Linq;

namespace PhysicsSim.VBOs
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

        public override void Render(Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            _renderCollection.Position = new Vector3((float)PObject.X * ElectroScene.Scale, (float)PObject.Y * ElectroScene.Scale, 0);
            _renderCollection.Render(translation, rotation, scale);
        }
    }

    public static class PhysicalObjectExtractExtension
    {
        public static IEnumerable<PhysicalObject> Extracted(this IEnumerable<RPhysicalObject> objects)
        {
            return from obj in objects select obj.PObject;
        }
    }
}
