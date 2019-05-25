using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;

namespace Tiger.AST
{
    class EqualNode:OrderNode
    {
        public EqualNode() : base() { }

        public EqualNode(IToken t) : base(t) { }

        public EqualNode(GTNode n) : base(n) { }

        public override OpCode OperatorOpCode
        {
            get
            {
                return OpCodes.Beq;
            }
        }
    }
}
