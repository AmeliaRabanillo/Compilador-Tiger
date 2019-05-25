using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;

namespace Tiger.AST
{
    class AndNode:LogicNode
    {
        public AndNode() : base() { }

        public AndNode(IToken t) : base(t) { }

        public AndNode(AndNode n) : base(n) { }

        public override void Generate(CodeGenerator cg)
        {
            Label push_false = cg.generator.DefineLabel();           
            Label end = cg.generator.DefineLabel();

            LeftOperand.Generate(cg);
            cg.generator.Emit(OpCodes.Ldc_I4_0);
            cg.generator.Emit(OpCodes.Beq, push_false);

            //el primer operando era verdadero
            RightOperand.Generate(cg);
            cg.generator.Emit(OpCodes.Ldc_I4_0);
            cg.generator.Emit(OpCodes.Beq, push_false);

            //el segundo operando era verdadero
            cg.generator.Emit(OpCodes.Ldc_I4_1);
            cg.generator.Emit(OpCodes.Br, end);

            //el primer operando es falso
            cg.generator.MarkLabel(push_false);
            cg.generator.Emit(OpCodes.Ldc_I4_0);

            cg.generator.MarkLabel(end);
        }
    }
}
