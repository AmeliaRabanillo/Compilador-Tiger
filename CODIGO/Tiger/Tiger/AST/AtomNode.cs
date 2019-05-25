using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Tiger.Semantics;

namespace Tiger.AST
{
    class AtomNode:ExpressionNode
    {
        protected AtomNode() : base() { }

        public override void CheckSemantics(Scope scope, Report report)
        {
            throw new NotImplementedException();
        }

        public override void Generate(CodeGenerator cg)
        {
            throw new NotImplementedException();
        }
    }
}
