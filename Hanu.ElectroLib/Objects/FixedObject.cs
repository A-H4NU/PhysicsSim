using System.ComponentModel;
using System.Numerics;

namespace Hanu.ElectroLib.Objects
{
    /// <summary>
    /// Physical object but its position is constant
    /// </summary>
    public class FixedObject : PhysicalObject
    {
        public FixedObject(Vector2 position, float charge = 0, float mass = 0)
            : base(position, charge, mass)
        {
            return;
        }

        public override Vector2 Position { get => _position; }

        private float _charge;
        public override float Charge
        {
            get => _charge;
            set
            {
                _charge = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Charge"));
            }
        }

        private float _mass;

        public override float Mass
        {
            get => _mass;
            set
            {
                _mass = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Mass"));
            }
        }

        public override event PropertyChangedEventHandler PropertyChanged;
    }
}