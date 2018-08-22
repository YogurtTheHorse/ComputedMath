using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ComputedMath.Models {
    public class ChartBoxModel : BoxModel {
        public ChartBoxModel(string title, IEnumerable<(double, double)[]> points) {
            Title = title;

            string placeholderName = $"placeholder_{title.GetHashCode()}";

            string data = "[" +
                          string.Join(
                              ",",
                              points.Select(plot =>
                                  "["
                                  + string.Join(",", plot.Select(t => $"[{t.Item1},{t.Item2}]"))
                                  + "]"
                              )
                          )
                          + "]";

            string script = File.ReadAllText("Content/js/chart.js");
            script = script.Replace("\"!placeholderName!\"", "\"#" + placeholderName + "\"").Replace("\"!data!\"", data);
            

            Content = $"<div id=\"{placeholderName}\" class=\"plot\"></div>" +
                      $"<script type=\"text/javascript\">{script}</script>";
        }
    }
}