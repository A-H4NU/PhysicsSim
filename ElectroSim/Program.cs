using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.OdeSolvers;

using System;
using System.Collections.Generic;
using System.Linq;
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

            var y0 = CreateVector.Dense<double>(2);
            y0[0] = 1;
            var result = SecondOrder(y0, 0, f, endFunc);
            Console.WriteLine(result.Last());
        }

        public static Vector<double> f(double t, Vector<double> y)
            => CreateVector.Dense(new double[] { t + Math.Cos(t), t - Math.Sin(t) });

        public static bool endFunc(Vector<double> y) => y.L2Norm() >= 10;

        public static List<Vector<double>> SecondOrder(Vector<double> y0, double start, Func<double, Vector<double>, Vector<double>> f, Func<Vector<double>, bool> endFunc, double delta = 1e-4)
        {
            double num = delta;
            List<Vector<double>> vectorArrays = new List<Vector<double>>();
            double num1 = start;
            vectorArrays.Add(y0);
            while (endFunc(vectorArrays.Last()) == false)
            {
                Vector<double> nums = f(num1, y0);
                Vector<double> nums1 = f(num1, y0 + (nums * num));
                vectorArrays.Add(y0 + (num * 0.5 * (nums + nums1)));
                num1 += num;
                y0 = vectorArrays.Last();
            }
            return vectorArrays;
        }
    }
}
