using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComputedMath.MathExtensions {
    public static class MatrixExtensions {
        public static void Swap<T>(this T[,] matrix, int firstRow, int secondRow) {
            if (firstRow == secondRow) return;

            var tmp = new T[matrix.GetLength(1)];
            for (var i = 0; i < matrix.GetLength(1); i++) {
                tmp[i] = matrix[firstRow, i];
                matrix[firstRow, i] = matrix[secondRow, i];
                matrix[secondRow, i] = tmp[i];
            }
        }

        public static void DivideRow<T>(this T[,] matrix, int rowIndex, Fraction divider) where T : class {
            for (var i = 0; i < matrix.GetLength(1); i++)
                matrix[rowIndex, i] = matrix[rowIndex, i] as Fraction / divider as T;
        }


        public static void Substraction<T>(this T[,] matrix, int rowIndex, T[] sub) where T : class {
            if (matrix.GetLength(1) != sub.Length) throw new ArgumentException("Not valid size");

            for (var i = 0; i < matrix.GetLength(1); i++)
                matrix[rowIndex, i] = (matrix[rowIndex, i] as Fraction) - (sub[i] as Fraction) as T;
        }

        public static T[] GetRow<T>(this T[,] matrix, int rowIndex) {
            var row = new T[matrix.GetLength(1)];

            for (var i = 0; i < matrix.GetLength(1); i++) row[i] = matrix[rowIndex, i];

            return row;
        }

        public static void Multiply<T>(this T[] row, Fraction mul) where T : class {
            for (var i = 0; i < row.Length; i++) {
                row[i] = (row[i] as Fraction) * mul as T;
            }
        }

        public static string ToLaTex<T>(this T[,] matrix) {
            var style = "{}";
            if (matrix.GetLength(0) != matrix.GetLength(1)) {
                style = "{" + string.Concat(Enumerable.Repeat("r", matrix.GetLength(0))) + "|r}";
            }

            var contentBuilder = new StringBuilder($"\\left({{\\begin{{array}}{style} ");
            var rows = new List<string>();

            for (var i = 0; i < matrix.GetLength(0); i++) {
                var rowItems = new List<string>();
                for (var j = 0; j < matrix.GetLength(1); j++) {
                    T element = matrix[i, j];
                    rowItems.Add((element as ILaTexConvertable)?.ToLaTeX() ?? $"{{{element.ToString()}}}");
                }

                rows.Add(string.Join(" & ", rowItems));
            }

            contentBuilder.Append(string.Join(" \\\\ ", rows));
            contentBuilder.Append("\\end{array}}\\right)");

            return contentBuilder.ToString();
        }

        public static string ToLaTeXVector<T>(this IEnumerable<T> vector) {
            return
                $"\\begin{{pmatrix}}" +
                $"{string.Join("\\\\", vector.Select(v => (v as ILaTexConvertable)?.ToLaTeX() ?? v.ToString()))}" +
                $"\\end{{pmatrix}}";
        }
    }
}