using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using Antlr.Runtime;

namespace Tiger.AST
{
    class DifferentNode:OrderNode
    {
        public DifferentNode() : base() { }

        public DifferentNode(IToken t) : base(t) { }

        public DifferentNode(DifferentNode n) : base(n) { }
        

        public override void Generate(CodeGenerator cg)
        {
            Label ret0 = cg.generator.DefineLabel();
            Label end = cg.generator.DefineLabel();

            LeftOperand.Generate(cg);
            RightOperand.Generate(cg);

            if (LeftOperand.returnType.isString)
            {
                cg.generator.Emit(OpCodes.Call, typeof(string).GetMethod("Compare", new Type[] { typeof(string), typeof(string) }));
                cg.generator.Emit(OpCodes.Ldc_I4_0);
            }

            cg.generator.Emit(OpCodes.Beq,ret0);
            cg.generator.Emit(OpCodes.Ldc_I4_1);
            cg.generator.Emit(OpCodes.Br, end);

            cg.generator.MarkLabel(ret0);
            cg.generator.Emit(OpCodes.Ldc_I4_0);

            cg.generator.MarkLabel(end);




        }
    }
}
