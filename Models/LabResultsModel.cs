using System.Collections.Generic;
using Nancy;

namespace ComputedMath.Models {
    public class LabResultsModel {
        public List<BoxModel> Results;
        public virtual string Name { get; set; }
        public bool WasErrors { get; set; }

        public virtual void Solve() {
            WasErrors = false;
            Results = new List<BoxModel>();
        }
    }
}