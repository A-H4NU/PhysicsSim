using System;

namespace Hanu.ElectroLib.Exceptions
{
    internal class PhysicalNonsenseException : Exception
    {
        public PhysicalNonsenseType PhysicalNonsenseType;

        public PhysicalNonsenseException(PhysicalNonsenseType type)
        {
            PhysicalNonsenseType = type;
        }
    }
}