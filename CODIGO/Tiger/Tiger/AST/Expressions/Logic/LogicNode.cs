using System.Reflection.Emit;
using Antlr.Runtime;
using Tiger.Semantics;

namespace Tiger.AST
{
    abstract class LogicNode : BinaryNode
    {
        public LogicNode() : base() { }

        public LogicNode(IToken t) : base(t) { }

        public LogicNode(LogicNode n) : base(n) { }

        public override OpCode OperatorOpCode
        {
            get;
        }

        public override void CheckSemantics(Scope scope, Report report)
        {
            base.CheckSemantics(scope, report);

            if (returnType.isError)
                return;

            if (!RightOperand.returnType.isInt)
            {
                returnType = scope.FindType("error");
                report.Add(RightOperand.Line, RightOperand.CharPositionInLine, string.Format("Can't apply {0} between expressions of type {1}", Text, RightOperand.returnType.name));
                return;
            }
            returnType = scope.FindType("int");
        }
        
    }
}
