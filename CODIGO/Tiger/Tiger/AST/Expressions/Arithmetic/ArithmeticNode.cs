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
    class ArithmeticNode : BinaryNode
    {
        public ArithmeticNode() : base() { }

        public ArithmeticNode(IToken t) : base(t) { }

        public ArithmeticNode(ArithmeticNode n) : base(n) { }

        public override OpCode OperatorOpCode
        {
            get;
        }

        public override void Generate(CodeGenerator cg)
        {
            LeftOperand.Generate(cg);
            RightOperand.Generate(cg);

            cg.generator.Emit(OperatorOpCode);
        }

        public override void CheckSemantics(Scope scope, Report report)
        {
            base.CheckSemantics(scope, report);

            if (returnType.isError)
                return;

            if (!RightOperand.returnType.isInt)
            {
                returnType = scope.FindType("error");
                report.Add(Line, CharPositionInLine, string.Format("Can't apply {0} between expressions of type {1}", Text, RightOperand.returnType.name));
                return;
            }
            returnType = scope.FindType("int");
        }
    }
}
