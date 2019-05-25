using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Antlr.Runtime;
using Tiger.Semantics;
using System.Reflection.Emit;

namespace Tiger.AST
{
    public class WhileNode : LoopNode
    {
        public WhileNode() : base() { }
        public WhileNode(IToken t) : base(t) { }
        public WhileNode(WhileNode n) : base(n) { }

        public LanguageNode condition { get { return Children[0] as LanguageNode; } }
        public LanguageNode doBlock { get { return Children[1] as LanguageNode; } }


        public override void CheckSemantics(Scope scope, Report report)
        {
            condition.CheckSemantics(scope, report);

            if (!condition.returnType.isInt)
            {
                report.Add(Line, CharPositionInLine, string.Format("while's condition must return an integer"));
                returnType = scope.FindType("error");
                return;
            }

            doBlock.CheckSemantics(scope, report);
            if (!doBlock.returnType.isVoid)
            {
                if (!doBlock.returnType.isError)
                    report.Add(Line, CharPositionInLine, "While's Block must return no value");

                returnType = scope.FindType("error");
                return;
            }

            returnType = scope.FindType("void");
        }

        public override void Generate(CodeGenerator cg)
        {
            Label start = cg.generator.DefineLabel();
            loopEnd = cg.generator.DefineLabel();

            cg.generator.MarkLabel(start);

            condition.Generate(cg);
            cg.generator.Emit(OpCodes.Ldc_I4_0);
            cg.generator.Emit(OpCodes.Beq, loopEnd);

            doBlock.Generate(cg);

            cg.generator.Emit(OpCodes.Br, start);

            cg.generator.MarkLabel(loopEnd);
        }
    }
}
