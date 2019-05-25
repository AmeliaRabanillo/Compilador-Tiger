using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;

namespace Tiger.AST
{
    class OrNode:LogicNode
    {
        public OrNode() : base() { }

        public OrNode(IToken t) : base(t) { }

        public OrNode(OrNode n) : base(n) { }

        public override void Generate(CodeGenerator cg)
        {
            Label op1false = cg.generator.DefineLabel();
            Label op2false = cg.generator.DefineLabel();
            Label end = cg.generator.DefineLabel();

            LeftOperand.Generate(cg);
            cg.generator.Emit(OpCodes.Ldc_I4_0);            
            cg.generator.Emit(OpCodes.Beq,op1false);

            //el primer operando era verdadero
            cg.generator.Emit(OpCodes.Ldc_I4_1);
            cg.generator.Emit(OpCodes.Br, end);

            //el primer operando era falso
            cg.generator.MarkLabel(op1false);
            RightOperand.Generate(cg);
            cg.generator.Emit(OpCodes.Ldc_I4_0);
            cg.generator.Emit(OpCodes.Beq, op2false);

            //el segundo operando era verdadero (el primer op es falso)
            cg.generator.Emit(OpCodes.Ldc_I4_1);
            cg.generator.Emit(OpCodes.Br, end);

            //el segundo operando es falso (el segundo op es falso)
            cg.generator.MarkLabel(op2false);
            cg.generator.Emit(OpCodes.Ldc_I4_0);

            cg.generator.MarkLabel(end);                       
        }
    }
}
