using Nancy;
using Nancy.Bootstrapper;
using Nancy.Configuration;
using Nancy.TinyIoc;

namespace ComputedMath {
    public sealed class ComputedMathBootstrapper : DefaultNancyBootstrapper {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines) {
            Conventions.ViewLocationConventions.Add(
                (viewName, model, context) => string.Concat("Views/Labs/", viewName)
            );
        }


#if DEBUG
        public override void Configure(INancyEnvironment environment) {
            environment.Tracing(true, true);
            base.Configure(environment);
        }
#endif
    }
}