using System.Collections.Generic;
using Nancy;

namespace ComputedMath.Models {
    public class LabResultsModel {
        public List<BoxModel> Results;
        public virtual string Name { get; set; }

        public virtual HttpFile File { get; set; }

        public virtual void Solve() {
            Results = new List<BoxModel>();
        }
    }
}