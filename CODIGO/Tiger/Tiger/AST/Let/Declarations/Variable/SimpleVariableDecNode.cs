using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;
using Tiger.Semantics;

namespace Tiger.AST
{
    class SimpleVariableDecNode : VariableDecNode
    {
        public SimpleVariableDecNode() : base() { }
        public SimpleVariableDecNode(IToken t) : base(t) { }
        public SimpleVariableDecNode(SimpleVariableDecNode n) : base(n) { }

        public override void CheckSemantics(Scope scope, Report report)
        {
            base.CheckSemantics(scope, report);
            if (returnType==null || !returnType.isError)
            {
                expr.CheckSemantics(scope, report);
                if (expr.returnType.isError)
                {
                    returnType = scope.FindType("error");
                    report.Add(Line, CharPositionInLine, string.Format("Invalid expresion assigned to {0}", id.name));
                    return;
                }

                if (expr.returnType.isNull)
                {//Nota: a las variables de las que se infiere el tipo por la parte derecha no se les puede asignar null, por eso este error
                    returnType = scope.FindType("error");
                    report.Add(Line, CharPositionInLine, string.Format("Can't assign a nil expression to {0}", id.name));
                    return;
                }

                if (expr.returnType.isVoid)
                {
                    returnType = scope.FindType("error");
                    report.Add(Line, CharPositionInLine, string.Format("Can't assign a void expression to {0}", id.name));
                    return;
                }
                returnType = expr.returnType;
                varInfo = scope.AddVariable(id.name, expr.returnType.GetBase());//Nota: a los array se les pone como tipo de retorno el tipo de sus elementos
                varInfo.type = expr.returnType;
            }
        }
    }
}
