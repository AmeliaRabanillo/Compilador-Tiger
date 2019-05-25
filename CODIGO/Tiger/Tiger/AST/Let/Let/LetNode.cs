using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiger.Semantics;
using Antlr.Runtime;

namespace Tiger.AST
{
    class LetNode : LanguageNode
    {
        public DeclarationListNode declarations { get { return Children[0] as DeclarationListNode; } }
        public ExprSeqNode block { get { return Children[1] as ExprSeqNode; } }

        public LetNode() : base() { }
        public LetNode(IToken t) : base(t) { }
        public LetNode(LetNode n) : base(n) { }

        public override void CheckSemantics(Scope scope, Report report)
        {
            Scope letScope = scope.AddChild(ScopeType.Let);
            if (declarations == null)
            {
                returnType = scope.FindType("error");
                return;
            }

            declarations.CheckSemantics(letScope, report);
            if (declarations.returnType.isError)
            {
                returnType = scope.FindType("error");
                return;
            }

            
            block.CheckSemantics(letScope, report);
            if (block.returnType.isError)
            {
                returnType = scope.FindType("error");
                return;
            }

            foreach (var item in block.exprs)
            {
                if (item.returnType.isUserDefine && letScope.FindType(item.returnType.name) == null)
                {
                    report.Add(Line, CharPositionInLine, "Cant return an expression that doesn't exists in the context");
                    returnType = scope.FindType("error");
                    return;
                }
            }

            //El tipo de retorno va a ser el de la secuencia de expresiones, o sea el tipo de la ultima expresion, o si no hay expresiones, void
            returnType = block.returnType;

            if (block.existsBreak)//Si existe un break tiene mas importancia el tipo de la ultima expresion que el void que pone el break
                returnType = block.possibleReturn;

            //El tipo de retorno de su bloque debe existir en este scope
            var info = scope.FindType(block.returnType.name);
            if (info == null)
            {
                report.Add(block.Line, block.CharPositionInLine, string.Format("Cant return type {0}, because doesn't exists", block.returnType.name));
                returnType = scope.FindType("error");
                return;
            }

           
        }

        public override void Generate(CodeGenerator cg)
        {
            declarations.Generate(cg);
            block.Generate(cg);
        }
    }
}
