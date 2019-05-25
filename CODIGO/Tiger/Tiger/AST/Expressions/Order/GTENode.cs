using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;

namespace Tiger.AST
{
    class GTENode:OrderNode
    {
        public GTENode():base(){ }

        public GTENode(IToken t) : base(t) { }

        public GTENode(GTENode n) : base(n) { }

        public override OpCode OperatorOpCode
        {
            get
            {
                return OpCodes.Bge;
            }
        }
    }
}
