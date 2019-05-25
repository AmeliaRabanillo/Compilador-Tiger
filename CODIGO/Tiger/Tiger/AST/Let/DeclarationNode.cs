using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;

namespace Tiger.AST
{
    public abstract class DeclarationNode:LanguageNode
    {
        public DeclarationNode() : base() { }

        public DeclarationNode(IToken n) : base(n) { }

        public DeclarationNode(DeclarationNode n) : base(n) { }
    }
}
