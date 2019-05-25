using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;
using Tiger.Semantics;
using System.Reflection;
using System.Reflection.Emit;

namespace Tiger.AST
{
    class AssignLvalueNode : LanguageNode
    {
        public Access_Lvalue left { get { return Children[0] as Access_Lvalue; } }
        public LanguageNode right { get { return Children[1] as LanguageNode; } }

        public AssignLvalueNode() : base() { }
        public AssignLvalueNode(IToken t) : base(t) { }
        public AssignLvalueNode(AssignLvalueNode n) : base(n) { }


        public override void CheckSemantics(Scope scope, Report report)
        {
            left.CheckSemantics(scope, report);

            if (left.returnType != null && left.returnType.isError)
            {
                returnType = scope.FindType("error");
                return;
            }

            right.CheckSemantics(scope, report);

            if (right.returnType.isError)
            {
                returnType = scope.FindType("error");
                return;
            }

            if (!right.returnType.Alike(left.returnType))
            {
                returnType = scope.FindType("error");
                report.Add(Line, CharPositionInLine, string.Format("Cant assign type {0} to a variable of type {1}", right.returnType.name, left.returnType.name));
                return;
            }

            if (left.varInfo.readOnly)
            {
                report.Add(Line, CharPositionInLine, string.Format("The variable {0} is read only", right));
                returnType = scope.FindType("error");
                return;
            }

            returnType = scope.FindType("void");
        }

        public override void Generate(CodeGenerator cg)
        {
            AccessNode variable = left.GenerateForAssign(cg);

            if (variable == null)
            {
                FieldInfo fieldInfo = left.varInfo.fieldBuilder;
                right.Generate(cg);
                cg.generator.Emit(OpCodes.Stsfld, fieldInfo);
            }
            else
            {
                #region TODO superparche                
                if (variable as IndexerNode != null)
                {
                    (variable as IndexerNode).index.Generate(cg);
                    right.Generate(cg);
                    variable.GenerateForAssign(cg);
                }
                #endregion TODO end superparche
                else
                {
                    right.Generate(cg);
                    variable.GenerateForAssign(cg);
                }


            }
        }
    }
}
