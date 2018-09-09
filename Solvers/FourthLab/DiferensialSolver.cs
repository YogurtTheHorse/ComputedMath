using System;

namespace ComputedMath.Solvers.FourthLab {
    public static class DiferensialSolver {
        public delegate (double[], double[]) CauchySolver(
            (double, double) startPoint,
            double segmentLength,
            double precision,
            Func<double, double, double> function);

        public static (double[], double[]) Eiler(
            (double, double) startPoint,
            double segmentLength,
            double precision,
            Func<double, double, double> function,
            uint enchantmentLevel = 0
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
    }
}