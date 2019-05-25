using System;
using System.Reflection.Emit;
using Antlr.Runtime;
using Tiger.Semantics;

namespace Tiger.AST
{
    class OrderNode : BinaryNode
    {
        public OrderNode() : base() { }

        public OrderNode(IToken t) : base(t) { }

        public OrderNode(OrderNode n) : base(n) { }


        public override OpCode OperatorOpCode
        {
            get;
        }

        public override void CheckSemantics(Scope scope, Report report)
        {
            base.CheckSemantics(scope, report);

            if (returnType.isError)
            {
                report.Add(LeftOperand.Line, LeftOperand.CharPositionInLine, string.Format("Can't apply {0} between expressions of type {1} and {2}", Text, LeftOperand.returnType.name, RightOperand.returnType.name));
                return;
            }           
            returnType = scope.FindType("int");
        }

        public override void Generate(CodeGenerator cg)
        {
            Label ret1 = cg.generator.DefineLabel();
            Label end = cg.generator.DefineLabel();

            LeftOperand.Generate(cg);
            RightOperand.Generate(cg);

            if (LeftOperand.returnType.isString)
            {
                cg.generator.Emit(OpCodes.Call, typeof(string).GetMethod("Compare", new Type[] { typeof(string), typeof(string) }));
                cg.generator.Emit(OpCodes.Ldc_I4_0);
            }
            cg.generator.Emit(OperatorOpCode, ret1);

            cg.generator.Emit(OpCodes.Ldc_I4_0);
            cg.generator.Emit(OpCodes.Br, end);

            cg.generator.MarkLabel(ret1);
            cg.generator.Emit(OpCodes.Ldc_I4_1);

            cg.generator.MarkLabel(end);

        }


    }
}
