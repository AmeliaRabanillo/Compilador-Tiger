using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Tiger.Semantics;
using Antlr.Runtime;

namespace Tiger.AST
{
    class IntNode : OthersNode
    {
        public IntNode() : base()
        {
        }

        public IntNode(IToken t) : base(t)
        {
        }

        public int value
        {
            get
            {
                return int.Parse(Text);
            }
        }

        public override void CheckSemantics(Scope scope, Report report)
        {
            int i;
            if (!int.TryParse(Text, out i))
            {
                report.Add(Line, CharPositionInLine, "Invalid int");
                returnType = scope.FindType("error");
                return;
            }

            returnType = scope.FindType("int");
        }

        public override void Generate(CodeGenerator cg)
        {
            cg.generator.Emit(OpCodes.Ldc_I4, value);
        }
    }
}
