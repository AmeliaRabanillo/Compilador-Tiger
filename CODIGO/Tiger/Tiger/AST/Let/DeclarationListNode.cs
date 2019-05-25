using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiger.Semantics;
using Antlr.Runtime;

namespace Tiger.AST
{
    class DeclarationListNode : LanguageNode
    {
        public List<BlockDecNode> declarations;

        public DeclarationListNode() : base() { }
        public DeclarationListNode(IToken t) : base(t) { }
        public DeclarationListNode(BlockDecNode n) : base(n) { }

        void CreateListOfDeclarations()
        {
            declarations = new List<BlockDecNode>();

            foreach (var item in Children)
                declarations.Add(item as BlockDecNode);
        }

        public override void CheckSemantics(Scope scope, Report report)
        {
            CreateListOfDeclarations();

            foreach (var item in declarations)
            {
                item.CheckSemantics(scope, report);
                if (item.returnType!=null && item.returnType.isError)
                {
                    returnType = scope.FindType("error");
                    return;
                }
            }

            returnType = scope.FindType("void");
        }

        public override void Generate(CodeGenerator cg)
        {
            CreateListOfDeclarations();

            foreach (var item in declarations)
                item.Generate(cg);                        
        }
    }
}
