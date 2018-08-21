using System;
using ComputedMath.Solvers.SecondLab;
using Irony.Parsing;

namespace ComputedMath.Models.Labs {
    public class SecondLabModel : LabResultsModel {
        public override string Name => "Integral approximation";

        public string Function { get; set; } = "x ** 2";
        public double A { get; set; } = 0;
        public double B { get; set; } = 10;
        public double Precision { get; set; } = 0.001;
        public string Method { get; set; }
        public string Subtype { get; set; }

        public override void Solve() {
            base.Solve();

            var expressionGrammar = new LabGrammar();
            var language = new LanguageData(expressionGrammar);
            var parser = new Parser(language);
            var runtime = new LabRuntime(language);

            ParseTree tree = parser.Parse(Function);

            if (tree.HasErrors()) {
                Results.Add(new BoxModel("Error at parsing", "Syntax errors found. Abrting."));
                return;
            }

            Results.AddRange(IntegralCalculator.Calculate(
                tree,
                runtime,
                A,
                B,
                Precision,
                Method,
                Subtype
            ));
        }
    }
}