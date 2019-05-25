using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Antlr.Runtime;
using Antlr.Runtime.Tree;
using Tiger.Semantics;
using System.Reflection.Emit;

namespace Tiger.AST
{
    public abstract class Node : CommonTree
    {
        protected Node():base() { }

        protected Node(Node node) : base(node) { }

        protected Node(IToken tokens):base(tokens){ }

        public override bool IsNil { get { return (base.IsNil && (this is EmptyNode)); } }

        public abstract void CheckSemantics(Scope scope, Report report);
        public abstract void Generate(CodeGenerator cg);
    }
}
