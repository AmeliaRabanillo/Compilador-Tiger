using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Antlr.Runtime;
using Tiger.Semantics;
using System.Reflection.Emit;

namespace Tiger.AST
{
    public abstract class LanguageNode : Node
    {
        
        protected LanguageNode() : base() { }
        protected LanguageNode(IToken tokens) : base(tokens) { }
        protected LanguageNode(LanguageNode node) : base(node) { }

        public TypeInfo returnType;
        
    }
}
