using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiger.Semantics;
using Antlr.Runtime;

namespace Tiger.AST
{
    public class LvalueStepsNode : OthersNode
    {
        public TypeInfo lastType;

        public LvalueStepsNode() : base() { }
        public LvalueStepsNode(IToken t) : base(t) { }
        public LvalueStepsNode(LvalueStepsNode n) : base(n) { }

        public override void CheckSemantics(Scope scope, Report report)
        {
        }

        public void CheckSemantics(Scope scope, Report report, TypeInfo start)
        {
            
            foreach (var item in Children)
            {
                IndexerNode index = item as IndexerNode;
                if (index != null)
                {//...[x]
                    if (start.declaration == null || !(start.declaration is ArrayDecNode))
                    {
                        //Si la declaracion no existe o no es un array
                        report.Add(Line, CharPositionInLine, string.Format("Can't apply an indexer to {0}", start.name));
                        returnType = scope.FindType("error");
                        return;
                    }

                    index.CheckSemantics(scope, report, start.declaration as ArrayDecNode);
                    if (index.returnType != null && !index.returnType.isInt)
                    {
                        report.Add(Line, CharPositionInLine, string.Format("Index value must be int, got {0}", index.returnType));
                        returnType = scope.FindType("error");
                        return;
                    }


                    start = (start.declaration as ArrayDecNode).itemsTypeInfo;                    
                }
                else
                {
                    DotNode dotNode = item as DotNode;
                    if (dotNode != null)
                    {
                        if (start.declaration == null && !(start.declaration is RecordDecNode))
                        {
                            report.Add(Line, CharPositionInLine, string.Format("Only can access to a field of a record"));
                            returnType = scope.FindType("error");
                            return;
                        }

                        dotNode.CheckSemantics(scope, report, start.declaration as RecordDecNode);
                        if (dotNode.returnType != null && dotNode.returnType.isError)
                        {
                            returnType = scope.FindType("error");
                            return;
                        }

                        start = dotNode.fieldType;
                    }
                }

            }

            returnType = start; //TOOD esto es un parche
            lastType = start;
        }

        public override void Generate(CodeGenerator cg)
        {
            foreach (var item in Children)
            {
                AccessNode t = item as AccessNode;
                t.Generate(cg);
            }
        }

        public AccessNode GenerateForAssign(CodeGenerator cg)
        {
            if (ChildCount == 0)
                return null;

            for (int i = 0; i < ChildCount - 1; i++)
                (Children[i] as AccessNode).Generate(cg);

            var t = Children[ChildCount - 1] as AccessNode;
            return t;
        }
    }
}
