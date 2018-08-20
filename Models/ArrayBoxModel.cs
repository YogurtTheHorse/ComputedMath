using ComputedMath.MathExtensions;

namespace ComputedMath.Models {
    public class ArrayBoxModel<T> : LaTeXBox {
        public ArrayBoxModel(string title, T[,] matrix) {
            Title = title;
            Content = $"$${matrix.ToLaTex()}$$";
        }
    }
}