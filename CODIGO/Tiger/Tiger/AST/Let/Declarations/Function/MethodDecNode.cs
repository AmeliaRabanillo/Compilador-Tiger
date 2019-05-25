using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;
using Tiger.Semantics;

namespace Tiger.AST
{
    public class MethodDecNode : FunctionDecNode
    {
        public MethodDecNode() : base() { }
        public MethodDecNode(IToken t) : base(t) { }
        public MethodDecNode(MethodDecNode n) : base(n) { }

        public NameNode funcReturnType { get { return Children[1] as NameNode; } }

        public override void PutReturnType(Scope scope)
        {
            functionReturnType = scope.FindType(funcReturnType.name);
            returnType = scope.FindType(funcReturnType.name); ;
        }

        public override void CheckSemantics(Scope scope, Report report)
        {
            base.CheckSemantics(scope, report);//aqui chequea el id y que ni exista otra var o funcion con el mismo id
            if (returnType != null && returnType.isError)
                return;

            var info = scope.FindType(funcReturnType.name);
            if (info == null)
            {
                report.Add(id.Line, id.CharPositionInLine, string.Format("Cant find return type {0} of function {1}", info.name, id.name));
                returnType = scope.FindType("error");
                return;

            }
            returnType = info;
            functionReturnType = info;

            expr.CheckSemantics(scope, report);
            if (expr.returnType != null && expr.returnType.isError)
            {
                returnType = scope.FindType("error");
                return;
            }

            if (!expr.returnType.Alike(info))
            {
                report.Add(id.Line, id.CharPositionInLine, string.Format("Declared return type of function {0} isn't equal to body return type ", id.name));
                returnType = scope.FindType("error");
                return;
            }
        }
    }
}
