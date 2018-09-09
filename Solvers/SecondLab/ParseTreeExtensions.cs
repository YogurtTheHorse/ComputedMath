using System;
using System.Linq;
using System.Linq.Expressions;
using ComputedMath.Models.Labs;
using Irony.Interpreter.Ast;
using Irony.Parsing;
using Nancy.Bootstrapper;

namespace ComputedMath.Solvers.SecondLab {
    public static class ParseTreeExtensions {
        public static string ToLaTeX(this ParseTree tree) {
            return ToLaTeX(tree.Root.AstNode as AstNode);
        }

        private static string ToLaTeX(this AstNode node, int parentOperationPriority = 0) {
            switch (node) {
                case StatementListNode listNode:
                    return string.Join("\\\\", listNode.ChildNodes.Select(childNode => childNode.ToLaTeX()));
                case BinaryOperationNode binNode:
                    string opSymbol = binNode.OpSymbol;

                    KeyTerm operationTerm = Grammar.CurrentGrammar.ToTerm(opSymbol);
                    bool ignorePriorirty = binNode.Op == ExpressionType.Divide || binNode.Op == ExpressionType.Power;

                    string
                        left = binNode.Left.ToLaTeX(ignorePriorirty ? 0 : operationTerm.Precedence),
                        right = binNode.Right.ToLaTeX(ignorePriorirty ? 0 : operationTerm.Precedence);


                    switch (binNode.Op) {
                        case ExpressionType.Divide:
                            return $"\\frac{{{left}}}{{{right}}}";
                        case ExpressionType.Power:
                            return $"{{{left}}}^{{{right}}}";
                        default:
                            if (operationTerm.Precedence < parentOperationPriority) {
                                left = "\\left(" + left;
                                right = right + "\\right)";
                            }

                            if (binNode.Op == ExpressionType.Multiply &&
                                !(char.IsDigit(left[left.Length - 1]) && char.IsDigit(right[0]))) {
                                opSymbol = "";
                            }

                            return $"{left} {opSymbol} {right}";
                    }

                case UnaryOperationNode unaryNode:
                    return $"{unaryNode.OpSymbol}{{{unaryNode.Argument.ToLaTeX()}}}";

                case FunctionCallNode callNode:
                    string args = string.Join(
                        ", ",
                        ((ExpressionListNode) callNode.ChildNodes[1]).ChildNodes.Select(childNode => ToLaTeX(childNode))
                    );
                    return $"{callNode.ChildNodes[0].AsString.ToLower()}({args})";
                default:
                    return node.AsString;
            }
        }
    }
}