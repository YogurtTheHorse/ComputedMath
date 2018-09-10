using System;
using System.Runtime.CompilerServices;
using ComputedMath.MathExtensions;
using ComputedMath.Models.Enums;
using ComputedMath.Solvers.FirstLab;

namespace ComputedMath.Models.Labs {
    public class FirstLabModel : LabResultsModel {
        private Fraction[,] _matrix = new Fraction[0, 0];

        private LinearSolverMethod _method;
        public override string Name => "Linear system solver";

        public override void Solve() {
            base.Solve();

            Results.Add(new ArrayBoxModel<Fraction>($"Input (Using #{Method})", _matrix));

            switch (_method) {
                case LinearSolverMethod.Gauss:
                case LinearSolverMethod.GaussMain:
                    Results.AddRange(
                        GaussSolver.GaussSolution(_matrix, _method == LinearSolverMethod.GaussMain)
                    );
                    break;

                case LinearSolverMethod.SimpleIterations:
                case LinearSolverMethod.Seidel:
                    Results.AddRange(
                        IterationsSolver.Solve(_matrix, Precision, _method == LinearSolverMethod.Seidel)
                    );
                    break;

                default:
                    Results.Add(new BoxModel("Not implemented method", $"Method {_method} is not implemented."));
                    break;
            }
        }

        #region Public API

        public string Method {
            get => _method.ToString();
            set => _method = Enum.Parse<LinearSolverMethod>(value.ToPascalCase());
        }

        public string Coefficients {
            get => string.Join(",", _matrix ?? new Fraction[,] { });
            set {
                string[] strCoef = value.Split(',');

                var rowsCount = 0;
                for (var i = 1; i <= 21; i++) {
                    if (i * (i + 1) != strCoef.Length) continue;

                    rowsCount = i;
                    break;
                }

                if (rowsCount == 0) throw new ArgumentException();

                _matrix = new Fraction[rowsCount, rowsCount + 1];
                for (var i = 0; i < rowsCount; i++) {
                    for (var j = 0; j < rowsCount + 1; j++) {
                        _matrix[i, j] = Fraction.ToFraction(strCoef[i * (rowsCount + 1) + j].Trim());
                    }
                }
            }
        }

        public double Precision { get; set; } = 0.01;

        #endregion
    }
}