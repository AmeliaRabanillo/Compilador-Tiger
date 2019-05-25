using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Tiger.Semantics;
using Antlr.Runtime;

namespace Tiger.AST
{
    public class ExprListNode : OthersNode
    {
        public List<LanguageNode> expr_list;

        public ExprListNode() : base()
        {
            Create();
        }

        public ExprListNode(IToken t) : base(t)
        {
            Create();
        }

        public ExprListNode(ExprListNode n) : base(n)
        {
            Create();
        }

        void Create()
        {
            expr_list = new List<LanguageNode>();
            if (Children != null)
                foreach (var item in Children)
                    expr_list.Add(item as LanguageNode);
        }

        public override void CheckSemantics(Scope scope, Report report)
        {
        }

        public bool Check(FunctionInfo func, Scope scope, Report report)
        {
            Create();
            if (expr_list.Count != func.parameters.Count)
            {
                report.Add(Line, CharPositionInLine, string.Format("Wrong parameter's number, expected {0}, got {1}", func.parameters.Count, expr_list.Count));
                returnType = scope.FindType("error");
                return false;
            }
            
            for (int i = 0; i < expr_list.Count; i++)
            {
                expr_list[i].CheckSemantics(scope, report);
                if (expr_list[i].returnType.isError)
                {
                    report.Add(Line, CharPositionInLine, string.Format("Error in parameter {0}", i));
                    returnType = scope.FindType("error");
                    return false;
                }

                if (!expr_list[i].returnType.Alike(func.parameters[i]))
                {
                    report.Add(Line, CharPositionInLine, string.Format("Parameter {0} has a wrong type, expected {1}, got {2}", i, func.parameters[i].name, expr_list[i].returnType.name));
                    returnType = scope.FindType("error");
                    return false;
                }
            }

            return true;
        }

        public override void Generate(CodeGenerator cg)
        {
            //Poner en la pila los parametros, porque lo necesita la llamada a la funcion
            foreach (var item in expr_list)
                item.Generate(cg);
        }
    }
}
