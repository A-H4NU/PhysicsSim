﻿using MathNet.Numerics.OdeSolvers;
using MathNet.Numerics.LinearAlgebra;

using System.Collections.Generic;
using System;
using System.Linq;

namespace Hanu.ElectroLib.Physics
{
    public static class PLine
    {
        //public static IEnumerable<Vector<double>> ElectricFieldLine(PSystem system, Vector<double> initPos, double end, int n, double step = 1e-16f)
        //{
            
        //}

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
