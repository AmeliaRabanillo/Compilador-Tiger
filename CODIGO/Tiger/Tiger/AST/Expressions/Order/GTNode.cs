using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;

namespace Tiger.AST
{
    class GTNode:OrderNode
    {
        public GTNode():base(){ }

        public GTNode(IToken t) : base(t) { }

        public GTNode(GTNode n) : base(n) { }

        public override OpCode OperatorOpCode
        {
            get
            {
                return OpCodes.Bgt;
            }
        }

    }
}
