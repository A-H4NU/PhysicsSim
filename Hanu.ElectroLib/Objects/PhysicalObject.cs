using MathNet.Numerics.LinearAlgebra;

namespace Hanu.ElectroLib.Objects
{
    public abstract class PhysicalObject
    {
        /// <summary> unit = m </summary>
        protected Vector<double> _position;

        /// <summary> unit = m </summary>
        public abstract Vector<double> Position { get; }

        /// <summary> unit = C </summary>
        public abstract double Charge { get; set; }

        /// <summary> unit = kg </summary>
        public abstract double Mass { get; set; }

        /// <summary> unit = m </summary>
        public double X => _position[0];
        /// <summary> unit = m </summary>
        public double Y => _position[1];

        public PhysicalObject(Vector<double> position, double charge = 0, double mass = 0)
        {
            _position = position;
            Charge = charge;
            Mass = mass;
        }

        public override string ToString() => $"( {_position}; {Mass} kg; {Charge} C; )";
    }
}