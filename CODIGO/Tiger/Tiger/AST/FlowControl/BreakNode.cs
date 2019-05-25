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
    class BreakNode : FlowNode
    {
        public BreakNode() : base() { }
        public BreakNode(IToken t) : base(t) { }
        public BreakNode(BreakNode n) : base(n) { }

        public IBreak loop { get; private set; }


        public override void CheckSemantics(Scope scope, Report report)
        {

            LanguageNode node = Parent as LanguageNode;

            while (node != null && !(node is IBreak))
            {
                if (node is ExprSeqNode)
                {
                    node.returnType = scope.FindType("void");
                    (node as ExprSeqNode).existsBreak = true;
                }

                if (node is FunctionDecNode)
                {
                    report.Add(Line, CharPositionInLine, "Ilegal Break");
                    returnType = scope.FindType("error");
                    return;
                }

                node = node.Parent as LanguageNode;
            }

            if (node == null)
            {
                report.Add(Line, CharPositionInLine, "Ilegal Break");
                returnType = scope.FindType("error");
                return;
            }

            loop = node as IBreak;
            returnType = scope.FindType("void");
        }

        public override void Generate(CodeGenerator cg)
        {
            cg.generator.Emit(OpCodes.Br, loop.loopEnd);
        }
    }
}
