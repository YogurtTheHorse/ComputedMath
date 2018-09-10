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
                ("First", typeof(FirstLabModel)),
                ("Second", typeof(SecondLabModel)),
                ("Third", typeof(ThirdLabModel)),
                ("Fourth", typeof(FourthLab))
            };

            foreach ((string name, Type type) in labs) {
                Get($"/labs/{name.ToLower()}", _ => type.CreateInstance());
                Post($"/labs/{name.ToLower()}", _ => {
                    MethodInfo method = typeof(ModuleExtensions)
                        .GetMethod("Bind", new[] {typeof(NancyModule)})
                        .MakeGenericMethod(type);

                    LabResultsModel model = null;
                    try {
                        model = (LabResultsModel) method.Invoke(null, new object[] {this});
                        model.Solve();
                    }
                    catch {
                        model = (LabResultsModel)type.CreateInstance();
                        model.WasErrors = true;
                        }
                    return View[$"{name}Lab.sshtml", model];
                });
            }
        }
    }
}