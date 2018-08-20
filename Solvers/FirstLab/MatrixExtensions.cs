using ComputedMath.MathExtensions;

namespace ComputedMath.Solvers.FirstLab {
    public static class MatrixExtensions {
        public static bool MainDiagonalStabilize(this Fraction[,] matrix) {
            return matrix.MainDiagonalStabilize(out int tmp);
        }
        
        public static bool MainDiagonalStabilize(this Fraction[,] matrix, out int swapsCount) {
            swapsCount = 0;
            
            for (var col = 0; col + 1 < matrix.GetLength(0); col++) {
                if (matrix[col, col] != 0) continue;

                // check for zero coefficients
                // find non-zero coefficient
                int swapRow = col + 1;
                for (; swapRow < matrix.GetLength(0); swapRow++) {
                    if (matrix[swapRow, col] != 0) {
                        break;
                    }
                }

                if (swapRow < matrix.GetLength(0) && matrix[swapRow, col] != 0) {
                    matrix.Swap(swapRow, col);
                    swapsCount++;
                }
                else {
                    // no, then the matrix has no unique solution
                    return false;
                }
            }

            return true;
        }
    }
}