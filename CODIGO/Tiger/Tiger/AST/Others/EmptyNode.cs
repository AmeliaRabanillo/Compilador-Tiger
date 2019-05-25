using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;
using Tiger.Semantics;

namespace Tiger.AST
{
    public class EmptyNode:OthersNode
    {
        public EmptyNode() : base() { }

        public EmptyNode(IToken t) : base(t) { }

        public EmptyNode(EmptyNode n) : base(n) { }

        public override void CheckSemantics(Scope scope, Report report)
        {
        }

        public override void Generate(CodeGenerator cg)
        {
        }
    }
}
