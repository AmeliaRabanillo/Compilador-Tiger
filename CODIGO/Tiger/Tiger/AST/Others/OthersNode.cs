using Antlr.Runtime;

namespace Tiger.AST
{
    public abstract class OthersNode:ExpressionNode
    {
        protected OthersNode() : base() { }

        protected OthersNode(IToken token) : base(token) { }

        protected OthersNode(OthersNode n) : base(n) { }
        
    }
}
