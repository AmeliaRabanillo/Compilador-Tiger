using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;

namespace Tiger.AST
{
    class DivNode : ArithmeticNode
    {
        public DivNode() : base() { }

        public DivNode(IToken t) : base(t) { }

        public DivNode(DivNode n) : base(n) { }

        public override OpCode OperatorOpCode
        {
            get
            {
                return OpCodes.Div;
            }
        }
    }
}
