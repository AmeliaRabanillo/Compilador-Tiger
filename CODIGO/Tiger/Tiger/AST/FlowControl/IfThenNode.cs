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
    public class IfThenNode : IfNode
    {
        public IfThenNode() : base() { }
        public IfThenNode(IToken t) : base(t) { }
        public IfThenNode(IfThenNode n) : base(n) { }
        
        public override void CheckSemantics(Scope scope, Report report)
        {
            base.CheckSemantics(scope, report);

            if (thenBlock.returnType != null && thenBlock.returnType.isError)
            {
                returnType = scope.FindType("error");
                return;
            }


            if (thenBlock.returnType != null && !thenBlock.returnType.isVoid)
            {
                report.Add(Line, CharPositionInLine, string.Format("If's thenCode must return no value."));
                returnType = scope.FindType("error");
                return;
            }
            
        }

        public override void Generate(CodeGenerator cg)
        {
            Label end = cg.generator.DefineLabel();

            condition.Generate(cg);
            cg.generator.Emit(OpCodes.Ldc_I4_0);
            cg.generator.Emit(OpCodes.Beq, end);

            thenBlock.Generate(cg);

            cg.generator.MarkLabel(end);
        }
    }
}
