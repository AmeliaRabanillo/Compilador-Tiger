using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;

namespace Tiger.AST
{
    class PlusNode : ArithmeticNode
    {
        public PlusNode() : base() { }

        public PlusNode(IToken t) : base(t) { }

        public PlusNode(PlusNode n) : base(n) { }

        public override OpCode OperatorOpCode
        {
            get
            {
                return OpCodes.Add;
            }
        }
    }
}
