using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiger.Semantics;
using Antlr.Runtime;
using System.Reflection.Emit;

namespace Tiger.AST
{
    public class ExprSeqNode : LanguageNode
    {
        public ExprSeqNode() : base() { }
        public ExprSeqNode(IToken n) : base(n) { }
        public ExprSeqNode(ExprSeqNode n) : base(n) { }
        public bool existsBreak;
        public TypeInfo possibleReturn;

        public List<LanguageNode> exprs { get; protected set; }

        public override void CheckSemantics(Scope scope, Report report)
        {
            exprs = new List<LanguageNode>();

            for (int i = 0; i < ChildCount; i++)
                exprs.Add(Children[i] as LanguageNode);

            for (int i = 0; i < exprs.Count; i++)
            {
                exprs[i].CheckSemantics(scope, report);
                if (exprs[i].returnType.isError)
                {
                    returnType = scope.FindType("error");
                    return;
                }
            }

            if (exprs.Count > 0 && returnType == null)
            {
                returnType = exprs[exprs.Count - 1].returnType;
                var info = scope.FindType(returnType.name);
                if (info == null)
                {
                    report.Add(Line, CharPositionInLine, string.Format("Type {0} cant exists in the current context", returnType.name));
                    returnType = scope.FindType("error");
                }
                return;
            }

            if (existsBreak && exprs.Count > 0)
            {
                possibleReturn = exprs[exprs.Count - 1].returnType;
                var info = scope.FindType(possibleReturn.name);
                if (info == null)
                {
                    report.Add(Line, CharPositionInLine, string.Format("Type {0} cant exists in the current context", possibleReturn.name));
                    returnType = scope.FindType("error");
                }
                return;
            }

            returnType = scope.FindType("void");

        }

        public override void Generate(CodeGenerator cg)
        {
            for (int i = 0; i < exprs.Count - 1; i++)
            {
                exprs[i].Generate(cg);
                if (!exprs[i].returnType.isVoid)
                    cg.generator.Emit(OpCodes.Pop);
            }

            if (exprs.Count > 0)
                exprs[exprs.Count - 1].Generate(cg);

        }
    }
}
