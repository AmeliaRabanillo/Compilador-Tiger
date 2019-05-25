using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiger.Semantics;

namespace Tiger.AST
{
    public class TypeNode : OthersNode
    {
        public NameNode name { get { return Children[0] as NameNode; } }
        

        public override void CheckSemantics(Scope scope, Report report)
        {
        }

        public override void Generate(CodeGenerator cg)
        {
        }
    }
}
