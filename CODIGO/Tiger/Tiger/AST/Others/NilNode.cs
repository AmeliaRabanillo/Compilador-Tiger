using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;
using Tiger.Semantics;

namespace Tiger.AST
{
    public class NilNode : OthersNode
    {
        public NilNode() : base() { }

        public NilNode(IToken t) : base(t) { }

        public NilNode(NilNode n) : base(n) { }

        public override void CheckSemantics(Scope scope, Report report)
        {
            returnType = scope.FindType("nil");
        }

        public override void Generate(CodeGenerator cg)
        {
            cg.generator.Emit(OpCodes.Ldnull);
        }
    }
}
