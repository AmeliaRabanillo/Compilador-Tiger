using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiger.Semantics;
using Antlr.Runtime;
using System.Reflection.Emit;

namespace Tiger.AST
{
    public abstract class VariableDecNode : DeclarationNode
    {
        public VariableDecNode() : base() { }
        public VariableDecNode(IToken t) : base(t) { }
        public VariableDecNode(VariableDecNode n) : base(n) { }


        public NameNode id { get { return Children[0] as NameNode; } }
        public LanguageNode expr { get { return Children[1] as LanguageNode; } }
        public VariableInfo varInfo { get; set; }
        

        public override void Generate(CodeGenerator cg)
        {
            varInfo.Create(cg);
            expr.Generate(cg);
            cg.generator.Emit(OpCodes.Stsfld, varInfo.fieldBuilder);
        }

        public override void CheckSemantics(Scope scope, Report report)
        {
            id.CheckSemantics(scope, report);

            if (!id.returnType.isOK)//Chequear que el nombre no sea una palabra reservada
            {
                returnType = scope.FindType("error");
                return;
            }


            if (scope.ShortFindFunction(id.name) != null || scope.ShortFindVariable(id.name) != null)//Chequear que no exista ya una variable o funcion con ese nombre
            {
                report.Add(id.Line, id.CharPositionInLine, string.Format("Identifier {0} already in use.", id.name));
                returnType = scope.FindType("error");
                return;
            }

            returnType = scope.FindType("void");
        }
    }
}
