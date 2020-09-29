using Hanu.ElectroLib.Objects;
using Hanu.ElectroLib.Physics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            PSystem system = new PSystem()
            {
                new FixedObject(new Vector2(5, 0), 1e+6f),
                new FixedObject(new Vector2(-5, 0), 1e+6f)
            };
            var f = PLine.ElectricFieldLine(system, new Vector2(5, 6), 6 * (int)6e3 + 1).ToArray();
            for (int i = 0; i <= 100; ++i)
            {
                Console.WriteLine(f[i * 50]);
            }
        }
    }
}
