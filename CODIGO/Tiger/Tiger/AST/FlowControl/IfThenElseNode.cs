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
    public class IfThenElseNode:IfNode
    {
        public IfThenElseNode() : base() { }
        public IfThenElseNode(IToken t) : base(t) { }
        public IfThenElseNode(IfThenElseNode n) : base(n) { }

       
        public LanguageNode elseBlock { get { return Children[2] as LanguageNode; } }

        public override void CheckSemantics(Scope scope, Report report)
        {
            base.CheckSemantics(scope, report);
            elseBlock.CheckSemantics(scope, report);

            if (returnType.isError || elseBlock.returnType.isError)
            {
                returnType = scope.FindType("error");
                return;
            }

            if (!thenBlock.returnType.Alike(elseBlock.returnType))
            {
                report.Add(thenBlock.Line, thenBlock.CharPositionInLine, string.Format("then else can't respectively return types {0} and {1}", thenBlock.returnType.name, elseBlock.returnType.name));
                returnType = scope.FindType("error");
                return;
            }

            
             returnType = thenBlock.returnType;
        }

        public override void Generate(CodeGenerator cg)
        {
            Label evaluateElse = cg.generator.DefineLabel();
            Label end = cg.generator.DefineLabel();

            condition.Generate(cg);
            cg.generator.Emit(OpCodes.Ldc_I4_0);
            cg.generator.Emit(OpCodes.Beq, evaluateElse);

            thenBlock.Generate(cg);
            cg.generator.Emit(OpCodes.Br, end);
            
            cg.generator.MarkLabel(evaluateElse);
            elseBlock.Generate(cg);
            
            cg.generator.MarkLabel(end);
        }
    }
}
