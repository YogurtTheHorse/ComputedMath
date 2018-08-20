namespace ComputedMath.Models {
    public class LaTeXBox : BoxModel {
        protected LaTeXBox() { }

        public LaTeXBox(string title, string content) {
            Title = title;
            Content = content.Contains("\n") ? $"${content}$" : $"$${content}$$";
        }
    }
}