using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.OdeSolvers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ElectroSim
{
    class Program
    {
        static void Main(string[] args)
        {
            MainWindow mw = new MainWindow(1600, 900);
            mw.Run(60);
            mw.Close();
        }
    }
}
