using System;
using System.Collections.Generic;
using System.Linq;
using ComputedMath.Solvers.SecondLab;
using Irony.Interpreter;
using Irony.Parsing;
using static ComputedMath.Solvers.ThirdLab.Interpolation;

namespace ComputedMath.Models.Labs {
    public class ThirdLabModel : LabResultsModel {
        private readonly ScriptApp _scriptApp;
        private readonly Random _random;
        private readonly LanguageData _languageData;
        private ParseTree _parseTree;

        public override string Name => "Function approximation";

        public string Function { get; set; } = "x ** 2";
        public double A { get; set; } = 0;
        public double B { get; set; } = 10;
        public int Count { get; set; } = 10;
        public int Seed { get; set; } = 1000;
        public double Precision { get; set; } = 0.1;

        public ThirdLabModel() {
            _random = new Random(Seed);

            var expressionGrammar = new LabGrammar();
            _languageData = new LanguageData(expressionGrammar);
            var labRuntime = new LabRuntime(_languageData);
            _scriptApp = new ScriptApp(labRuntime);
        }

        public override void Solve() {
            base.Solve();

            var parser = new Parser(_languageData);
            _parseTree = parser.Parse(Function);

            if (_parseTree.HasErrors()) {
                Results.Add(new BoxModel("Error at parsing", "Syntax errors found. Abrting."));
                return;
            }

            double delta = (B - A) / Count;
            double[] xDataOriginal = Enumerable.Range(0, Count + 1).Select(i => A + i * delta).ToArray();
            double[] yDataOriginal = xDataOriginal.Select(CalculateFunction).ToArray();

            double[] yDataForApproximation = yDataOriginal
                .Select(y => y + (_random.NextDouble() - 0.5) * Precision).ToArray();


            double[] visiblesXs = Enumerable.Range(0, Count * 10 + 1).Select(i => A + i * delta / 10).ToArray();
            (double, double)[] calculatedOriginalFunction = visiblesXs.Select(x => (x, CalculateFunction(x))).ToArray();

            var polynoms = new (string, GenerateApproximatedPolynom)[] {
                ("LaGrange polynom", LaGrangePolynom),
                ("Newton formulas", NewtonFomula),
                ("Cubic spline", CubicSpline)
            };

            Results.Add(new ChartBoxModel("Input", new List<(double, double)[]> {calculatedOriginalFunction}));

            foreach ((string name, GenerateApproximatedPolynom polynomMaker) in polynoms) {
                Func<double, double> approximated = polynomMaker(xDataOriginal, yDataForApproximation);

                var data = new List<(double, double)[]> {
                    calculatedOriginalFunction,
                    visiblesXs.Select(x => (x, approximated(x))).ToArray()
                };

                data.AddRange(xDataOriginal.Select((x, i) => new[] {(x, yDataForApproximation[i])}.ToArray()));

                Results.Add(new ChartBoxModel(name, data));
            }
        }

        private double CalculateFunction(double x) {
            _scriptApp.Globals["x"] = x;

            var res = (double) _scriptApp.Evaluate(_parseTree);

            if (double.IsNaN(res) || double.IsInfinity(res)) {
                double epsilon = (A - B) / (Count * 20);
                return CalculateFunction(x + epsilon);
            }

            return res;
        }
    }
}