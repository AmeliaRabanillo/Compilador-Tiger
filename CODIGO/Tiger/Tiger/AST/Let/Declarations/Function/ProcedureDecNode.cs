using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;
using Tiger.Semantics;

namespace Tiger.AST
{
    public class ProcedureDecNode : FunctionDecNode
    {//function id(type-fields_opt)=expr
        public ProcedureDecNode() : base() { }
        public ProcedureDecNode(IToken t) : base(t) { }
        public ProcedureDecNode(ProcedureDecNode n) : base(n) { }

        public override void PutReturnType(Scope scope)
        {
            functionReturnType = scope.FindType("void");
            returnType = scope.FindType("void");
        }

        public override void CheckSemantics(Scope scope, Report report)
        {
            returnType = scope.FindType("void");
            functionReturnType = returnType;

            base.CheckSemantics(scope, report);
            if (returnType.isError)//quiere decir que hubo error
                return;

            expr.CheckSemantics(scope, report);
            if (expr.returnType != null && expr.returnType.isError)
            {
                returnType = scope.FindType("error");
                return;
            }

            if (!expr.returnType.isVoid)
            {
                report.Add(Line, CharPositionInLine, string.Format("Procedure {0} must return no value", id.name));
                returnType = scope.FindType("error");
                return;
            }
        }
    }
}
