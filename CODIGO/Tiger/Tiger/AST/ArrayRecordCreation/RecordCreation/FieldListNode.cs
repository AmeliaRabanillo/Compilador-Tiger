using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;
using Tiger.Semantics;

namespace Tiger.AST
{
    public class FieldListNode : OthersNode
    {
        //NOTA: este nodo es para la creacion de records, no para la declaracion
        public List<FieldNode> fields;

        public FieldListNode() : base() { }
        public FieldListNode(IToken t) : base(t) { }
        public FieldListNode(FieldListNode n) : base(n) { }

        public void Create()
        {
            fields = new List<FieldNode>();
            foreach (var item in Children)
                fields.Add(item as FieldNode);
        }

        public void CheckSemantics(Scope scope, Report report,  RecordDecNode node)
        {
            Create();

            if (fields.Count != node.paramsNode.ChildCount)
            {//La cantidad de parametros a inicializar tiene que ser igual que la cantidad de parametros declarados en el record
                report.Add(node.id.Line, node.id.CharPositionInLine, string.Format("Can't create record {0}, have {1} parameters, got {2}", node.id.name, node.paramsNode.ChildCount, fields.Count));
                returnType = scope.FindType("error");
                return;
            }

            for (int i = 0; i < fields.Count; i++)
            {
                fields[i].CheckSemantics(scope, report, node.paramsNode.parameters[i]);
                if (fields[i].returnType != null && fields[i].returnType.isError)
                {
                    returnType = scope.FindType("error");
                    return;
                }
            }

            returnType = scope.FindType("void");

        }

        public override void CheckSemantics(Scope scope, Report report)
        {
        }

        public override void Generate(CodeGenerator cg)
        {
        }
    }
}
