using System;
using System.Linq;

namespace ComputedMath.Solvers.ThirdLab {
    public static class Interpolation {
        public static Func<double, double> LaGrangePolynom(double[] xData, double[] yData) {
            if (xData.Length != yData.Length) {
                throw new ArgumentException("xData length must be equable to yData's");
            }

            return x => Enumerable.Range(0, xData.Length)
                .Select(i => yData[i] * P(x, xData, i)).Sum();
        }

        private static double P(double x, double[] xData, int i) {
            double denominator = 1, numerator = 1;

            for (var j = 0; j < xData.Length; j++) {
                if (i == j) {
                    continue;
                }
                    
                denominator *= xData[i] - xData[j];
                numerator *= x - xData[j];
            }

            return numerator / denominator;
        }
    }
}