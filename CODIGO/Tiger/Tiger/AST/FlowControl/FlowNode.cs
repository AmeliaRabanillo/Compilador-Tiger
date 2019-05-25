using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;

namespace Tiger.AST
{
   public abstract class FlowNode:LanguageNode
    {
        public FlowNode() : base() { }

        public FlowNode(IToken t):base(t){ }

        public FlowNode(FlowNode n) : base(n) { }
    }
}
