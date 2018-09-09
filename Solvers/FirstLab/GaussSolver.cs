using System;
using System.Collections.Generic;
using System.Linq;
using ComputedMath.MathExtensions;
using ComputedMath.Models;

namespace ComputedMath.Solvers.FirstLab {
    public static class GaussSolver {
        public static IEnumerable<BoxModel> GaussSolution(Fraction[,] matrix, bool useMainComponents) {
            var results = new List<BoxModel>();

            var startMatrix = new Fraction[matrix.GetLength(0), matrix.GetLength(1)];
            Array.Copy(matrix, startMatrix, matrix.Length);

            int swapsCount;
            Fraction detMultiplyBy;

            bool gotTriangle = useMainComponents
                ? ForwardInsertionWithMainElement(ref matrix, results, out swapsCount, out detMultiplyBy)
                : ForwardInsertion(ref matrix, results, out swapsCount, out detMultiplyBy);

            if (!gotTriangle) return results;

            results.Add(new ArrayBoxModel<Fraction>("Triangle matrix", matrix));

            Fraction determinator = (swapsCount % 2 == 0 ? 1 : -1)
                                    * detMultiplyBy
                                    * Enumerable.Range(0, matrix.GetLength(0))
                                        .Select(i => matrix[i, i])
                                        .Aggregate(new Fraction(1), (f, mul) => mul * f);

            results.Add(new LaTeXBox(
                "Determinant",
                $"det {startMatrix.ToLaTex()} = {determinator.ToLaTeX()} \\approx {determinator.ToDouble()}"));

            if (!BackInsertion(ref matrix, results)) return results;

            results.Add(new ArrayBoxModel<Fraction>("Solution matrix", matrix));

            List<Fraction> solutions = GetAndAddSolutions(matrix, results).ToList();
            var errors = "";

            for (var i = 0; i < matrix.GetLength(0); i++) {
                var sum = new Fraction();
                for (var j = 0; j < matrix.GetLength(1) - 1; j++) {
                    sum += matrix[i, j] * solutions[j];
                }

                Fraction error = sum - matrix[i, matrix.GetUpperBound(1)];
                errors += $"\\delta_{{{i + 1}}} = {error} \\\\";
            }

            results.Add(new LaTeXBox("Errors", errors));

            return results;
        }

        private static IEnumerable<Fraction> GetAndAddSolutions(Fraction[,] matrix, ICollection<BoxModel> results) {
            var solutions = new List<string>();
            for (var i = 0; i < matrix.GetLength(0); i++) {
                Fraction element = matrix[i, matrix.GetLength(0)];
                solutions.Add(
                    $"\\\\x_{{ {i + 1} }} = {element.ToLaTeX()} " +
                    $"\\approx{element.ToDouble()}");
                yield return element;
            }

            results.Add(new LaTeXBox("Solutions", string.Join(" ", solutions)));
        }

        private static bool ForwardInsertionWithMainElement(
            ref Fraction[,] matrix,
            ICollection<BoxModel> results,
            out int swapsCount,
            out Fraction multiplcateBy
        ) {
            swapsCount = 0;
            multiplcateBy = 1;

            for (var column = 0; column < matrix.GetLength(0) - 1; column++) {
                var mainIndex = 0;
                for (var i = 1; i < matrix.GetLength(0); i++) {
                    if (matrix[i, column].Absolute() >= matrix[mainIndex, column].Absolute()) {
                        mainIndex = i;
                    }
                }

                if (matrix[mainIndex, column] == 0) {
                    results.Add(new BoxModel("Fail", "Matrix has no unique solutions"));
                    return false;
                }

                if (column != mainIndex) {
                    matrix.Swap(column, mainIndex);
                    swapsCount++;
                }

                multiplcateBy *= matrix[column, column];
                matrix.DivideRow(column, matrix[column, column]);


                for (int i = column + 1; i < matrix.GetLength(0); i++) {
                    Fraction[] row = matrix.GetRow(column);
                    row.Multiply(matrix[i, column]);
                    matrix.Substraction(i, row);
                }
            }

            return true;
        }

        private static bool ForwardInsertion(
            ref Fraction[,] matrix,
            ICollection<BoxModel> results,
            out int swapsCount,
            out Fraction multiplcateBy
        ) {
            swapsCount = 0;
            multiplcateBy = 1;

            if (!matrix.MainDiagonalStabilize(out int stabilizeSwapsCount)) {
                results.Add(new BoxModel("Fail", "Matrix has no unique solution"));
                return false;
            }

            if (stabilizeSwapsCount > 0) {
                results.Add(new ArrayBoxModel<Fraction>("Stable matrix", matrix));
            }


            // elimination
            for (var sourceRow = 0; sourceRow + 1 < matrix.GetLength(0); sourceRow++) {
                for (int destRow = sourceRow + 1; destRow < matrix.GetLength(0); destRow++) {
                    Fraction df = matrix[sourceRow, sourceRow];
                    Fraction sf = matrix[destRow, sourceRow];

                    for (var i = 0; i < matrix.GetLength(0) + 1; i++) {
                        matrix[destRow, i] = matrix[destRow, i] * df - matrix[sourceRow, i] * sf;
                    }

                    multiplcateBy /= df;
                }
            }

            return true;
        }

        private static bool BackInsertion(ref Fraction[,] matrix, ICollection<BoxModel> results) {
            for (int row = matrix.GetUpperBound(0); row >= 0; row--) {
                Fraction f = matrix[row, row];
                if (f == 0) {
                    results.Add(
                        new ArrayBoxModel<Fraction>(
                            "Fail on back insertion. Looks like matrix has no unique solutions",
                            matrix
                        )
                    );
                    return false;
                }

                matrix.DivideRow(row, f);

                for (var destRow = 0; destRow < row; destRow++) {
                    matrix[destRow, matrix.GetLength(0)] -= matrix[destRow, row] * matrix[row, matrix.GetLength(0)];
                    matrix[destRow, row] = 0;
                }
            }

            return true;
        }
    }
}