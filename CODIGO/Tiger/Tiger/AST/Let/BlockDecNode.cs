using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiger.Semantics;
using Antlr.Runtime;

namespace Tiger.AST
{
    public abstract class BlockDecNode : OthersNode
    {
        public BlockDecNode() : base() { }
        public BlockDecNode(IToken t) : base(t) { }
        public BlockDecNode(BlockDecNode n) : base(n) { }

        
        public override void Generate(CodeGenerator cg)
        {
            foreach (var item in Children)
                (item as LanguageNode).Generate(cg);
        }
               
    }
}
