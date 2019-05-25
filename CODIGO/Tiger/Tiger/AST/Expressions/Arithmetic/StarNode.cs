using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;

namespace Tiger.AST
{
    class StarNode : ArithmeticNode
    {
        public StarNode() : base() { }

        public StarNode(IToken t) : base(t) { }

        public StarNode(StarNode n) : base(n) { }
        
        public override OpCode OperatorOpCode
        {
            get
            {
                return OpCodes.Mul;
            }
        }
    }
}
