using System;
using System.Collections.Generic;
using System.Linq;
using ComputedMath.Models;
using Irony.Interpreter;
using Irony.Parsing;

namespace ComputedMath.Solvers.SecondLab {
    public static class IntegralCalculator {
        public static IEnumerable<BoxModel> Calculate(
            ParseTree tree,
            LabRuntime runtime,
            double a,
            double b,
            double precision,
            string method,
            string subtype
        ) {
            yield return new LaTeXBox("Input", $"f\\left(x\\right) = {tree.ToLaTeX()}");

            var app = new ScriptApp(runtime);

            double Function(double x) {
                app.Globals["x"] = x;
                return (double) app.Evaluate(tree);
            }

            var count = 4;
            var integralSing = 1;

            if (a > b) {
                (a, b) = (b, a);
                integralSing = -1;
            }

            double oldIntegral = integralSing * Approximate(method, subtype, Function, a, b, count / 2);
            double integral = integralSing * Approximate(method, subtype, Function, a, b, count);

            while (Math.Abs(integral - oldIntegral) >= precision * (method == "simpson" ? 15 : 3)) {
                oldIntegral = integral;
                count *= 2;
                integral = integralSing * Approximate(method, subtype, Function, a, b, count);
            }

            yield return new LaTeXBox($"Result",
                $"N = {count}\\\\" +
                $"\\int_{{{a}}}^{{{b}}} f\\left(x\\right) dx \\approx {integral}"
            );
        }

        private static double Approximate(string method, string subtype, Func<double, double> function, double a,
            double b, int count) {
            double delta = Math.Abs(b - a) / count;

            switch (method) {
                case "rectangle":
                    return delta * (Enumerable.Range(subtype == "left" ? 1 : 0, count - (subtype == "center" ? 2 : 1))
                                        .Select(i => function(a + delta * i))
                                        .Sum() +
                                    (subtype == "center"
                                        ? 0.5 * (function(a) + function(b))
                                        : 0)
                           );

                case "trapezoidal":
                    return (function(a) + function(b)) * delta / 2 +
                           delta * Enumerable.Range(0, count).Select(i => function(a + i * delta)).Sum();

                case "simpson":
                    return delta * Enumerable.Range(0, count / 2).Select(i => {
                        double x = (1 + 2 * i) * delta + a;
                        return function(x - delta) + 4 * function(x) + function(x + delta);
                    }).Sum() / 3;

                default:
                    return 0;
            }
        }
    }
}