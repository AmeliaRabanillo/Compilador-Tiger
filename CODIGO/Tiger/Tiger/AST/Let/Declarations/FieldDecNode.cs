using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiger.Semantics;
using Antlr.Runtime;

namespace Tiger.AST
{
    public class FieldDecNode : OthersNode
    {
        public FieldDecNode() : base() { }
        public FieldDecNode(IToken t) : base(t) { }
        public FieldDecNode(FieldDecNode n) : base(n) { }

        public NameNode id { get { return Children[0] as NameNode; } }
        public NameNode type { get { return Children[1] as NameNode; } }

        public override void CheckSemantics(Scope scope, Report report)
        {
        }

        public void CheckSemantics(Scope scope, Report report, List<string> names, List<Semantics.TypeInfo> infos)
        {
            id.CheckSemantics(scope, report);

            if ((id.returnType != null && id.returnType.isError) || names.Contains(id.name))
            {
                report.Add(id.Line, id.CharPositionInLine, string.Format("Can't exists more than one field with the same identifier {0}", id.name));
                returnType = scope.FindType("error");
                return;
            }

            TypeInfo info = scope.FindType(type.name);
            if (info == null)
            {
                report.Add(id.Line, id.CharPositionInLine, string.Format("Wrong type {0} ", type.name));
                returnType = scope.FindType("error");
                return;
            }

            infos.Add(info);
            names.Add(id.name);
        }

        public override void Generate(CodeGenerator cg)
        {
        }

        public TypeInfo GetTypeInfo(Scope scope)
        {
            return scope.FindType(type.name);
        }
    }
}
