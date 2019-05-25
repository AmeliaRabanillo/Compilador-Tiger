using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;

namespace Tiger.AST
{
    public abstract class ExpressionNode : LanguageNode
    {
        protected ExpressionNode() : base() { }

        protected ExpressionNode(IToken t) : base(t) { }

        protected ExpressionNode(ExpressionNode n) : base(n) { }
        
    }
}
