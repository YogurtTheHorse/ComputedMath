using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ComputedMath.Solvers.SecondLab;
using ComputedMath.Solvers.ThirdLab;
using Irony.Interpreter;
using Irony.Parsing;

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

            double[] yDataApproximated = yDataOriginal
                .Select(y => y + (_random.NextDouble() - 0.5) * Precision).ToArray();
            Func<double, double> approximated = Interpolation.LaGrangePolynom(xDataOriginal, yDataApproximated);

            double[] visiblesXs = Enumerable.Range(0, Count * 10 + 1).Select(i => A + i * delta / 10).ToArray();


            var data = new List<(double, double)[]> {
                visiblesXs.Select(x => (x, CalculateFunction(x))).ToArray(),
                visiblesXs.Select(x => (x, approximated(x))).ToArray()
            };

            data.AddRange(xDataOriginal.Select((x, i) => new[] {(x, yDataApproximated[i])}.ToArray()));

            Results.Add(new ChartBoxModel("LaGrange polynom", data));
            Results.Add(new ChartBoxModel("LaGrange polynom 1", data));
            Results.Add(new ChartBoxModel("LaGrange polynom 3", data));
        }

        private double CalculateFunction(double x) {
            _scriptApp.Globals["x"] = x;
            return (double) _scriptApp.Evaluate(_parseTree);
        }
    }
}