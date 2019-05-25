using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;

namespace Tiger.AST
{
    class LTENode:OrderNode
    {
        public LTENode() : base() { }

        public LTENode(IToken t) : base(t) { }

        public LTENode(LTENode n) : base(n) { }

        public override OpCode OperatorOpCode
        {
            get
            {
                return OpCodes.Ble;
            }
        }
    }
}
