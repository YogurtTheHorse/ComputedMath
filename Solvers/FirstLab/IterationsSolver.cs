using System;
using System.Collections.Generic;
using System.Linq;
using ComputedMath.MathExtensions;
using ComputedMath.Models;

namespace ComputedMath.Solvers.FirstLab {
    public static class IterationsSolver {
        public static IEnumerable<BoxModel> Solve(Fraction[,] input, double precision, bool useSeidel) {
            if (!input.MainDiagonalStabilize(out int swapsCount)) {
                yield return new BoxModel("Fail", "Unable to ake main diagonal retreat.");
                yield break;
            }

            if (swapsCount > 0) {
                yield return new ArrayBoxModel<Fraction>("Main diagonal retreation", input);
            }

            int size = input.GetLength(0);
            var betaVector = new double[size];
            var xVector = new double[size];
            var oldXVector = new double[size];

            for (var i = 0; i < betaVector.Length; i++) {
                xVector[i] = betaVector[i] = (input[i, size] / input[i, i]).ToDouble();
                oldXVector[i] = Math.Abs(xVector[i]) + precision * 2;
            }

            var alphaMatrix = new double[size, size];

            for (var i = 0; i < size; i++) {
                for (var j = 0; j < size; j++) {
                    alphaMatrix[i, j] = i == j ? 0 : (-input[i, j] / input[i, i]).ToDouble();
                }
            }

            yield return new LaTeXBox(
                "Coefficients",
                $"\\alpha = {alphaMatrix.ToLaTex()} " +
                $"\\beta = {betaVector.ToLaTeXVector()}"
            );

            bool isConverge = useSeidel || CheckConverge(alphaMatrix);
            if (!isConverge) {
                yield return new BoxModel(
                    "Fail",
                    "Matrix won't converage"
                );
                yield break;
            }

            var iterationsCount = 0;
            while (Enumerable.Range(0, size).Select(i => Math.Abs(xVector[i] - oldXVector[i])).Max() >= precision) {
                xVector.CopyTo(oldXVector, 0);
                iterationsCount++;

                for (var i = 0; i < size; i++) {
                    xVector[i] = betaVector[i]
                                 + Enumerable.Range(useSeidel ?  i : 0, size - (useSeidel ?  i : 0))
                                     .Select(j => oldXVector[j] * alphaMatrix[i, j])
                                     .Sum()
                                 + (useSeidel
                                     ? Enumerable.Range(0, i)
                                         .Select(j => xVector[j] * alphaMatrix[i, j])
                                         .Sum()
                                     : 0);
                }
            }

            var errors = new List<double>();

            for (var i = 0; i < size; i++) {
                var sum = 0.0;
                for (var j = 0; j < size; j++) {
                    sum += input[i, j].ToDouble() * xVector[j];
                }

                errors.Add(Math.Abs(sum - input[i, size].ToDouble()));
            }

            yield return new LaTeXBox(
                "Solution",
                $"x = {xVector.ToLaTeXVector()} \\\\" +
                $"\\delta = {errors.ToLaTeXVector()}\\\\" +
                $"\\delta_{{max}} = {errors.Max()}\\\\" +
                $"Iterations\\ count\\colon {iterationsCount}"
            );
        }

        private static bool CheckConverge(double[,] alphaMatrix) {
            var res = true;
            int size = alphaMatrix.GetLength(0);

            for (var i = 0; i < size; i++) {
                if (Enumerable.Range(0, size).Select(j => Math.Abs(alphaMatrix[i, j])).Sum() < 1) {
                    continue;
                }

                res = false;
                break;
            }

            if (res) {
                return true;
            }

            for (var j = 0; j < size; j++) {
                if (Enumerable.Range(0, size).Select(i => Math.Abs(alphaMatrix[i, j])).Sum() < 1) {
                    continue;
                }

                res = false;
                break;
            }

            return res;
        }
    }
}