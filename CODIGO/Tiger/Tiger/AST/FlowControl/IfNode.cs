using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiger.Semantics;
using Antlr.Runtime;

namespace Tiger.AST
{
    public abstract class IfNode : FlowNode
    {
        public IfNode() : base() { }
        public IfNode(IToken t) : base(t) { }
        public IfNode(IfNode n) : base(n) { }


        public LanguageNode condition { get { return Children[0] as LanguageNode; } }
        public LanguageNode thenBlock { get { return Children[1] as LanguageNode; } }


        public override void CheckSemantics(Scope scope, Report report)
        {
            condition.CheckSemantics(scope, report);

            if (!condition.returnType.isInt)
            {
                report.Add(condition.Line, condition.CharPositionInLine, string.Format("If's Condition's return type must be int not {0}.", condition.returnType.name));
                returnType = scope.FindType("error");                
                return;
            }

            thenBlock.CheckSemantics(scope, report);

            if (thenBlock.returnType.isError)
                returnType = scope.FindType("error");
            
            returnType = thenBlock.returnType;
           
        }
        
    }
}
