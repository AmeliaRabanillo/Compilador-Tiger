using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;

namespace Tiger.AST
{
    class MinusNode : ArithmeticNode
    {
        public MinusNode() : base() { }

        public MinusNode(IToken t) : base(t) { }

        public MinusNode(MinusNode n) : base(n) { }

        public override OpCode OperatorOpCode
        {
            get
            {
                return OpCodes.Sub;
            }
        }
    }
}
