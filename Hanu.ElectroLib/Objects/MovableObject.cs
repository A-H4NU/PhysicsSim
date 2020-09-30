using MathNet.Numerics.LinearAlgebra;

namespace Hanu.ElectroLib.Objects
{
    public class MovableObject : PhysicalObject
    {
        public MovableObject(Vector<double> position, double charge = 0, double mass = 0)
            : base(position, charge, mass)
        {
            return;
        }

        /// <summary>
        /// unit = m
        /// <para> To modify this, use <see cref="Position"/> </para>
        /// </summary>
        public override Vector<double> Position
        {
            get => _position;
        }

        /// <summary>
        /// unit = m
        /// <para> Modifiable property of <see cref="Position"/> </para>
        /// </summary>
        public Vector<double> PositionM
        {
            get => _position;
            set
            {
                _position = value;
            }
        }

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

        // TODO: implement velocity property
    }
}