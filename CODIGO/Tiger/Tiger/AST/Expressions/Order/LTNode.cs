using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;

namespace Tiger.AST
{
    class LTNode:OrderNode
    {
        public LTNode() : base() { }

        public LTNode(IToken t) : base(t) { }

        public LTNode(LTNode n) : base(n) { }

        public override OpCode OperatorOpCode
        {
            get
            {
                return OpCodes.Blt;
            }
        }
    }
}
