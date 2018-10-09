using System;
using Irony.Interpreter.Ast;

namespace ComputedMath.Solvers.FourthLab {
    public static class DiferensialSolver {
        public static (double[], double[]) Eiler(
            (double, double) startPoint,
            double segmentLength,
            double precision,
            Func<double, double, double> function,
            int enchantmentLevel = 0
        ) {
            var calculatedXs = new double[(int) (segmentLength / precision) + 1];
            var calculatedYs = new double[calculatedXs.Length];

            (calculatedXs[0], calculatedYs[0]) = startPoint;

            for (var i = 1; i < calculatedXs.Length; i++) {
                double fi = function(calculatedXs[i - 1], calculatedYs[i - 1]);
                calculatedXs[i] = calculatedXs[0] + precision * i;
                calculatedYs[i] = calculatedYs[i - 1] +
                                  precision * fi;

                for (var j = 0; j < enchantmentLevel; j++) {
                    calculatedYs[i] = calculatedYs[i - 1] +
                                      precision * (fi + function(calculatedXs[i], calculatedYs[i])) / 2;
                }
            }

            return (calculatedXs, calculatedYs);
        }

        public static double[] Milne(
            Func<double, double, double> func,
            double y0,
            double t0,
            int count,
            double h,
            double precision = 0.0001
        ) {
            if (count < 4) {
                throw new ArgumentException("Count must be greater than 3", nameof(count));
            }

            var res = new double[count + 1];
            double[] rungeKuttaPrecalculated = RungeKutta(func, y0, t0, 3, h);

            Array.Copy(rungeKuttaPrecalculated, 0, res, 0, rungeKuttaPrecalculated.Length);

            for (var i = 4; i <= count; i++) {
                double fs = 2 * func(t0 + (i - 3) * h, res[i - 3])
                            - func(t0 + (i - 2) * h, res[i - 2])
                            + 2 * func(t0 + (i - 1) * h, res[i - 1]);
                double preY = res[i - 4] + 4 * h * fs / 3;
                double b = res[i - 2],
                       a = b;

                for (var j = 0; j < 1000; j++) {
                    fs = func(t0 + (i - 2) * h, b)
                         + 4 * func(t0 + (i - 1) * h, res[i - 1])
                         + func(t0 + h * i, preY);

                    b = res[i] = res[i - 2] + h * fs / 3;
                } 
            }

            return res;
        }

        public static double[] RungeKutta(
            Func<double, double, double> func,
            double y0,
            double t0,
            int count,
            double h
        ) {
            var res = new double[count + 1];
            res[0] = y0;

            for (var i = 1; i <= count; i++) {
                double prevT = t0 + i * (h - 1);

                double
                    k1 = func(prevT, res[i - 1]),
                    k2 = func(prevT + 0.5 * h, res[i - 1] + h * 0.5 * k1),
                    k3 = func(prevT + 0.5 * h, res[i - 1] + h * 0.5 * k2),
                    k4 = func(prevT + h, res[i - 1] + h * k3);

                res[i] = res[i - 1] + h * (k1 + 2 * k2 + 2 * k3 + k4) / 6;
            }

            return res;
        }
    }
}