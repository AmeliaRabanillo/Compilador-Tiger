using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;
using Tiger.Semantics;

namespace Tiger.AST
{
    public class VariableDecBlockNode : BlockDecNode
    {
        public List<VariableDecNode> simpleVariables;

        public VariableDecBlockNode() : base() { }
        public VariableDecBlockNode(IToken t) : base(t) { }
        public VariableDecBlockNode(VariableDecBlockNode n) : base(n) { }

        public void Create()
        {
            simpleVariables = new List<VariableDecNode>();
            foreach (var item in Children)
                simpleVariables.Add((item as VariableDecNode));
        }

        public override void CheckSemantics(Scope scope, Report report)
        {
            Create();

            foreach (var item in simpleVariables)
            {
                item.CheckSemantics(scope, report);
                if (item.returnType.isError)
                {
                    returnType = scope.FindType("error");
                    return;
                }
            }

            returnType = scope.FindType("void");
        }
    }
}
