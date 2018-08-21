using System;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;

namespace ComputedMath.Solvers.SecondLab {
    public sealed class LabGrammar : InterpretedLanguageGrammar {
        public LabGrammar() : base(false) {
            var numberLiteral = new NumberLiteral("number") {
                DefaultIntTypes = new TypeCode[1] {
                    TypeCode.Double
                }
            };
            var identifierTerminal = new IdentifierTerminal("identifier");
            NonGrammarTerminals.Add(new CommentTerminal("comment", "#", "\n", "\r"));
            KeyTerm term = ToTerm(",");
            var stringLiteral =
                new StringLiteral("string", "\"", StringOptions.AllowsAllEscapes | StringOptions.IsTemplate);
            stringLiteral.AddStartEnd("'", StringOptions.AllowsAllEscapes | StringOptions.IsTemplate);
            stringLiteral.AstConfig.NodeType = typeof(StringTemplateNode);
            var nonTerminal1 = new NonTerminal("Expr");
            var templateSettings = new StringTemplateSettings {ExpressionRoot = nonTerminal1};
            SnippetRoots.Add(nonTerminal1);
            stringLiteral.AstConfig.Data = templateSettings;
            var nonTerminal2 = new NonTerminal("Term");
            var nonTerminal3 = new NonTerminal("BinExpr", typeof(BinaryOperationNode));
            var nonTerminal4 = new NonTerminal("ParExpr");
            var nonTerminal5 = new NonTerminal("UnExpr", typeof(UnaryOperationNode));
            var nonTerminal6 = new NonTerminal("TernaryIf", typeof(IfNode));
            var listNonTerminal1 = new NonTerminal("ArgList", typeof(ExpressionListNode));
            var nonTerminal7 = new NonTerminal("FunctionCall", typeof(FunctionCallNode));
            var nonTerminal8 = new NonTerminal("MemberAccess", typeof(MemberAccessNode));
            var nonTerminal9 = new NonTerminal("IndexedAccess", typeof(IndexedAccessNode));
            var nonTerminal10 = new NonTerminal("ObjectRef");
            var nonTerminal11 = new NonTerminal("UnOp");
            var nonTerminal12 = new NonTerminal("BinOp", "operator");
            var nonTerminal13 = new NonTerminal("PrefixIncDec", typeof(IncDecNode));
            var nonTerminal14 = new NonTerminal("PostfixIncDec", typeof(IncDecNode));
            var nonTerminal15 = new NonTerminal("IncDecOp");
            var nonTerminal16 = new NonTerminal("AssignmentStmt", typeof(AssignmentNode));
            var nonTerminal17 = new NonTerminal("AssignmentOp", "assignment operator");
            var nonTerminal18 = new NonTerminal("Statement");
            var listNonTerminal2 = new NonTerminal("Program", typeof(StatementListNode));
            nonTerminal1.Rule =
                nonTerminal2 | nonTerminal5 |
                nonTerminal3 | nonTerminal13 |
                nonTerminal14 | nonTerminal6;
            nonTerminal2.Rule =
                numberLiteral | nonTerminal4 |
                stringLiteral | nonTerminal7 |
                identifierTerminal | nonTerminal8 | nonTerminal9;
            nonTerminal4.Rule = "(" + nonTerminal1 + ")";
            nonTerminal5.Rule = nonTerminal11 + nonTerminal2 +
                                ReduceHere();
            nonTerminal11.Rule = ToTerm("+") | "-" | "!";
            nonTerminal3.Rule = nonTerminal1 + nonTerminal12 + nonTerminal1;
            nonTerminal12.Rule =
                ToTerm("+") |
                "-" | "*" |
                "/" | "**" | "==" | "<" | "<=" |
                ">" | ">=" | "!=" | "&&" | "||" | "&" | "|";
            nonTerminal13.Rule = nonTerminal15 + identifierTerminal;
            nonTerminal14.Rule = identifierTerminal + PreferShiftHere() +
                                 nonTerminal15;
            nonTerminal15.Rule = ToTerm("++") | "--";
            nonTerminal6.Rule =
                nonTerminal1 + "?" + nonTerminal1 + ":" +
                nonTerminal1;
            nonTerminal8.Rule =
                nonTerminal1 + PreferShiftHere() + "." +
                identifierTerminal;
            nonTerminal16.Rule = nonTerminal10 + nonTerminal17 + nonTerminal1;
            nonTerminal17.Rule = ToTerm("=") | "+=" | "-=" | "*=" |
                                 "/=";
            nonTerminal18.Rule = nonTerminal16 | nonTerminal1 | Empty;
            listNonTerminal1.Rule = MakeStarRule(listNonTerminal1, term, nonTerminal1);
            nonTerminal7.Rule =
                nonTerminal1 + PreferShiftHere() + "(" +
                listNonTerminal1 + ")";
            nonTerminal7.NodeCaptionTemplate = "call #{0}(...)";
            nonTerminal10.Rule = identifierTerminal | nonTerminal8 |
                                 nonTerminal9;
            nonTerminal9.Rule =
                nonTerminal1 + PreferShiftHere() + "[" +
                nonTerminal1 + "]";
            listNonTerminal2.Rule =
                MakePlusRule(listNonTerminal2, NewLine, nonTerminal18);
            Root = listNonTerminal2;
            RegisterOperators(10, "?");
            RegisterOperators(15, "&", "&&", "|", "||");
            RegisterOperators(20, "==", "<", "<=", ">", ">=", "!=");
            RegisterOperators(30, "+", "-");
            RegisterOperators(40, "*", "/");
            RegisterOperators(50, Associativity.Right, "**");
            RegisterOperators(60, "!");
            MarkPunctuation("(", ")", "?", ":", "[", "]");
            RegisterBracePair("(", ")");
            RegisterBracePair("[", "]");
            MarkTransient(nonTerminal2, nonTerminal1, nonTerminal18, nonTerminal12, nonTerminal11, nonTerminal15,
                nonTerminal17, nonTerminal4, nonTerminal10);
            MarkNotReported("++", "--");
            AddToNoReportGroup("(", "++", "--");
            AddToNoReportGroup(NewLine);
            AddOperatorReportGroup("operator");
            AddTermsReportGroup("assignment operator", "=", "+=", "-=", "*=", "/=");

            LanguageFlags = LanguageFlags.NewLineBeforeEOF | LanguageFlags.CreateAst | LanguageFlags.SupportsBigInt;
        }
    }
}