using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;
using Tiger.Semantics;

namespace Tiger.AST
{
    //Para los parametros de los records y las funciones
    public class FieldsDecNode : OthersNode
    {
        public FieldsDecNode() : base() { }
        public FieldsDecNode(IToken t) : base(t) { }
        public FieldsDecNode(FieldsDecNode n) : base(n) { }

        public List<FieldDecNode> parameters;

        public void Create()
        {
            parameters = new List<FieldDecNode>();
            if (Children != null)
                foreach (var item in Children)
                    parameters.Add(item as FieldDecNode);
        }

        public void CheckSemantics(Scope scope, Report report, List<Semantics.TypeInfo> infos)
        {
            Create();

            List<string> names = new List<string>();

            foreach (var item in parameters)
            {
                item.CheckSemantics(scope, report, names, infos);
                if (item.returnType != null && item.returnType.isError)
                {
                    returnType = scope.FindType("error");
                    return;
                }
            }
        }

        public override void CheckSemantics(Scope scope, Report report)
        {
        }

        public override void Generate(CodeGenerator cg)
        {
        }
    }
}
