using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;
using Tiger.Semantics;

namespace Tiger.AST
{
    class NegNode : ExpressionNode
    {
        public NegNode() : base() { }

        public NegNode(IToken t) : base(t) { }

        public NegNode(NegNode n) : base(n) { }

        public LanguageNode operand { get { return Children[0] as LanguageNode; } }
                
        public override void CheckSemantics(Scope scope, Report report)
        {
            operand.CheckSemantics(scope, report);
            TypeInfo intType = scope.FindType("int");
            if (operand.returnType.Alike(intType))
            {
                returnType = intType;

            }
            else {
                report.Add(operand.Line, operand.CharPositionInLine, string.Format("Can't apply - operator to {0}", operand.returnType.name));
                returnType = scope.FindType("error");
            }
        }

        public override void Generate(CodeGenerator cg)
        {
            operand.Generate(cg);
            cg.generator.Emit(OpCodes.Neg);
        }
    }
}
