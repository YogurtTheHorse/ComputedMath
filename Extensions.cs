using System.Text;

namespace ComputedMath {
    public static class Extensions {
        public static string ToPascalCase(this string original) {
            var builder = new StringBuilder();

            for (var i = 0; i < original.Length; i++)
                if (i == 0 || original[i - 1] == '_')
                    builder.Append(char.ToUpper(original[i]));
                else if (original[i] != '_') builder.Append(original[i]);

            return builder.ToString();
        }
    }
}