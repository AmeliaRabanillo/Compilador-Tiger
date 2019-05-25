using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;
using Tiger.Semantics;
using Antlr.Runtime;

namespace Tiger.AST
{
    class TypeIdVariableDecNode : VariableDecNode
    {
        public NameNode type { get { return Children[2] as NameNode; } }

        public TypeIdVariableDecNode() : base() { }
        public TypeIdVariableDecNode(IToken t) : base(t) { }
        public TypeIdVariableDecNode(TypeIdVariableDecNode n) : base(n) { }

        public override void CheckSemantics(Scope scope, Report report)
        {
            TypeInfo info = scope.FindType(type.name);

            if (info == null)//Chequear que el tipo del que se esta intentando crear la variable exista
            {
                report.Add(Line, CharPositionInLine, string.Format("Does not exists a type {0}", type.name));
                returnType = scope.FindType("error");
                return;
            }

            base.CheckSemantics(scope, report);

            if (returnType==null || !returnType.isError)
            {
                expr.CheckSemantics(scope, report);
                if (!expr.returnType.Alike(info.GetBase()))
                {
                    report.Add(Line, CharPositionInLine, string.Format("Can't assing a value {0} to a variable {1}", expr.returnType.name, info.name));
                    returnType = scope.FindType("error");
                    return;
                }
                varInfo = scope.AddVariable(id.name, info.GetBase());
            }            
        }
    }
}
