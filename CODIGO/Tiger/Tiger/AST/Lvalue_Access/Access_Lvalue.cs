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
    public class Access_Lvalue : OthersNode
    {
        public NameNode id { get { return Children[0] as NameNode; } }
        public VariableInfo varInfo { get; private set; }
        public LvalueStepsNode steps
        {
            get
            {
                try
                {
                    return Children[1] as LvalueStepsNode;
                }
                catch
                {
                    return null;
                }
            }
        }

        public Access_Lvalue() : base() { }
        public Access_Lvalue(IToken t) : base(t) { }
        public Access_Lvalue(Access_Lvalue n) : base(n) { }

        public override void CheckSemantics(Scope scope, Report report)
        {
            varInfo = scope.FindVariable(id.name);

            if (varInfo == null)
            {
                report.Add(Line, CharPositionInLine, string.Format("Variable {0} doesn't exists in the current context", id.name));
                returnType = scope.FindType("error");
                return;
            }

            if (steps.ChildCount == 0)
            {
                returnType = varInfo.type;
                return;
            }


            steps.CheckSemantics(scope, report,varInfo.type);

            if (steps.returnType!=null && steps.returnType.isError)
            {
                returnType = scope.FindType("error");
                return;
            }

            returnType = steps.lastType;        
        }


        public override void Generate(CodeGenerator cg)
        {
            //solo para obtener el valor            
            cg.generator.Emit(OpCodes.Ldsfld, varInfo.fieldBuilder);
            if (steps.ChildCount != 0)
                steps.Generate(cg);
        }

        public AccessNode GenerateForAssign(CodeGenerator cg)
        {
            if (steps.ChildCount > 0)
                cg.generator.Emit(OpCodes.Ldsfld, varInfo.fieldBuilder);//poner en el tope de la pila el valor de varInfo

            return steps.GenerateForAssign(cg);
        }
    }
}
