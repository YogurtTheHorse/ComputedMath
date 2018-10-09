using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ComputedMath.MathExtensions;

namespace ComputedMath.Solvers.ThirdLab {
    public static class Interpolation {
        private static readonly ConcurrentDictionary<(int, int, int), double> NewtonDiffs =
            new ConcurrentDictionary<(int, int, int), double>();

        public delegate Func<double, double> GenerateApproximatedPolynomial(double[] xData, double[] yData);

        public static Func<double, double> LaGrangePolynomial(double[] xData, double[] yData) {
            if (xData.Length != yData.Length) {
                throw new ArgumentException("xData length must be equable to yData's");
            }

            return x => Enumerable.Range(0, xData.Length)
                .Select(i => yData[i] * P(x, xData, i)).Sum();
        }

        private static double P(double x, IReadOnlyList<double> xData, int i) {
            double denominator = 1, numerator = 1;

            for (var j = 0; j < xData.Count; j++) {
                if (i == j) {
                    continue;
                }

                denominator *= xData[i] - xData[j];
                numerator *= x - xData[j];
            }

            return numerator / denominator;
        }

        public static Func<double, double> NewtonFormula(double[] xData, double[] yData) {
            return x => {
                if (x < xData[0]) {
                    return 0;
                }

                int zeroIndex = xData.Count(x0 => x0 <= x) - 1;
                double q = (x - xData[zeroIndex]) / (xData[1] - xData[0]);
                var skippedYData = new double[yData.Length - zeroIndex];
                Array.Copy(yData, zeroIndex, skippedYData, 0, skippedYData.Length);

                int n = Math.Min(yData.Length - zeroIndex - 1, 5);

                double result = yData[zeroIndex];
                for (var i = 0; i < n; i++) {
                    double qs = 1;
                    for (var j = 0; j <= i; j++) {
                        qs *= q - j;
                    }

                    double fact = 1;
                    for (var j = 2; j <= i + 1; j++) {
                        fact *= j;
                    }

                    result += qs * FiniteDifferences(yData, zeroIndex, i + 1) / fact;
                }

                return result;
            };
        }

        private static double FiniteDifferences(IReadOnlyList<double> yData, int k, int n) {
            if (n <= 0) {
                return yData[k];
            }

            if (!NewtonDiffs.TryGetValue((yData.GetHashCode(), k, n), out double res)) {
                res = FiniteDifferences(yData, k + 1, n - 1) - FiniteDifferences(yData, k, n - 1);
                NewtonDiffs[(yData.GetHashCode(), k, n)] = res;
            }

            return res;
        }

        public static Func<double, double> CubicSpline(double[] xData, double[] yData) {
            var cubicSpline = new CubicSpline(xData, yData);

            return x => cubicSpline.Eval(new[] {x})[0];
        }
    }
}