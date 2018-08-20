using ComputedMath.Models.Labs;
using Nancy;
using Nancy.ModelBinding;

namespace ComputedMath {
    public sealed class MainModule : NancyModule {
        public MainModule() {
            Get("/", _ => View["Index"]);
            Get("/labs/first", _ => new FirstLabModel());
            Post("/labs/first", _ => {
                var model = this.Bind<FirstLabModel>();
                model.Solve();
                return model;
            });
        }
    }
}