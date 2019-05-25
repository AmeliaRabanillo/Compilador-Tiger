using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiger.Semantics;
using Antlr.Runtime;

namespace Tiger.AST
{
    public class FieldNode : OthersNode
    {
        //NOTA: este nodo es para la creacion de records, no para la declaracion
        public NameNode id { get { return Children[0] as NameNode; } }
        public LanguageNode expr { get { return Children[1] as LanguageNode; } }

        public FieldNode() : base() { }
        public FieldNode(IToken t) : base(t) { }
        public FieldNode(FieldNode n) : base(n) { }

        public override void CheckSemantics(Scope scope, Report report)
        {
        }

        public override void Generate(CodeGenerator cg)
        {
        }

        public void CheckSemantics(Scope scope, Report report, FieldDecNode node)//se le pasa el nodo que estaba en la declaracion del record
        {
            if (id.name != node.id.name)
            {
                report.Add(Line, CharPositionInLine, string.Format("Record's field's names must be identical"));
                returnType = scope.FindType("error");
                return;
            }

            expr.CheckSemantics(scope,report);
            if (expr.returnType.isError)
            {
                returnType = scope.FindType("error");
                return;
            }

            TypeInfo info = scope.FindType(node.type.name);//Buscar el tipo de retorno que se designo en la declaracion del record
            if (info == null)
            {
                report.Add(node.Line, node.CharPositionInLine, string.Format("Cant find type {0}", node.type.name));
                returnType = scope.FindType("error");
                return;
            }

            if (!info.Alike(expr.returnType))
            {
                report.Add(Line, CharPositionInLine, string.Format("Cant assign {0} to {1}", info.name, expr.returnType.name));
                returnType = scope.FindType("error");
            }

            returnType = scope.FindType("void");
        }

    }
}
