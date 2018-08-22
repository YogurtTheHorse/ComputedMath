using System;
using System.Reflection;
using ComputedMath.Models;
using ComputedMath.Models.Labs;

using Nancy;
using Nancy.Extensions;

using ModuleExtensions = Nancy.ModelBinding.ModuleExtensions;

namespace ComputedMath {
    public sealed class MainModule : NancyModule {
        public MainModule() {
            Get("/", _ => View["Index"]);
            Get("/labs", _ => View["Index"]);

            (string, Type)[] labs = {
                ("first", typeof(FirstLabModel)),
                ("second", typeof(SecondLabModel)),
                ("third", typeof(ThirdLabModel))
            };

            foreach ((string name, Type type) in labs) {
                Get("/labs/" + name, _ => type.CreateInstance());
                Post("/labs/" + name, _ => {
                    MethodInfo method = typeof(ModuleExtensions)
                        .GetMethod("Bind", new[] {typeof(NancyModule)})
                        .MakeGenericMethod(type);
                    
                    var model = (LabResultsModel) method.Invoke(null, new object[]{this});
                    model.Solve();
                    return model;
                });
            }
        }
    }
}