using MathNet.Numerics.LinearAlgebra;

using System.ComponentModel;
using System.Numerics;

namespace Hanu.ElectroLib.Objects
{
    public abstract class PhysicalObject : INotifyPropertyChanged
    {
        /// <summary> unit = m </summary>
        protected Vector2 _position;

        public abstract event PropertyChangedEventHandler PropertyChanged;

        /// <summary> unit = m </summary>
        public abstract Vector2 Position { get; }

        /// <summary> unit = C </summary>
        public abstract float Charge { get; set; }

        /// <summary> unit = kg </summary>
        public abstract float Mass { get; set; }

        /// <summary> unit = m </summary>
        public float X => _position.X;
        /// <summary> unit = m </summary>
        public float Y => _position.Y;

        public PhysicalObject(Vector2 position, float charge = 0, float mass = 0)
        {
            _position = position;
            Charge = charge;
            Mass = mass;
        }

        public override string ToString() => $"( {_position}; {Mass} kg; {Charge} C; )";
    }
}