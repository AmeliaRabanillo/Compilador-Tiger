using Antlr.Runtime;
using System;
using Tiger.Semantics;
using System.Reflection.Emit;
using Tiger;

namespace Tiger.AST
{
    public class UnknowNode : OthersNode
    {
        public UnknowNode() : base() { }

        public UnknowNode(IToken token) : base(token) { }

        public override void CheckSemantics(Scope scope, Report report)
        {
            throw new InvalidOperationException();
        }

        public override void Generate(CodeGenerator cg)
        {
            throw new InvalidOperationException();
        }
    }
}
