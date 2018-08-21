using System;
using System.Linq.Expressions;
using Irony.Interpreter;
using Irony.Parsing;

namespace ComputedMath.Solvers.SecondLab {
    public class LabRuntime : LanguageRuntime {
        public LabRuntime(LanguageData language) : base(language) { }

        public override void Init() {
            base.Init();
            
            BuiltIns.ImportStaticMembers(typeof(Math));
        }

        public override void InitBinaryOperatorImplementationsForMatchedTypes() {
            base.InitBinaryOperatorImplementationsForMatchedTypes();

            AddBinary(ExpressionType.Power, typeof(double), ((x, y) => (object) Math.Pow((double)x, (double)y)));
        }
    }
}