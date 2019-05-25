using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;
using Tiger.Semantics;

namespace Tiger.AST
{
    public abstract class AccessNode : LanguageNode
    {
        public AccessNode() : base() { }
        public AccessNode(IToken t) : base(t) { }
        public AccessNode(AccessNode n) : base(n) { }

        public abstract void GenerateForAssign(CodeGenerator cg);
        
    }
}
