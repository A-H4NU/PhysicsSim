using MathNet.Numerics.LinearAlgebra;

namespace Hanu.ElectroLib.Objects
{
    /// <summary>
    /// Physical object but its position is constant
    /// </summary>
    public class FixedObject : PhysicalObject
    {
        public FixedObject(Vector<double> position, double charge = 0, double mass = 0)
            : base(position, charge, mass)
        {
            return;
        }

        public override Vector<double> Position { get => _position; }

        private double _charge;
        public override double Charge
        {
            get => _charge;
            set
            {
                _charge = value;
            }
        }

        private double _mass;
        public override double Mass
        {
            get => _mass;
            set
            {
                _mass = value;
            }
        }
    }
}