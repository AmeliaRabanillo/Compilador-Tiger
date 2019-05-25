using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;
using Tiger.Semantics;

namespace Tiger.AST
{
    public abstract class BinaryNode : ExpressionNode
    {
        protected BinaryNode() : base() { }

        protected BinaryNode(IToken t) : base(t) { }

        protected BinaryNode(BinaryNode n) : base(n) { }

        public LanguageNode LeftOperand
        {
            get { return this.Children[0] as LanguageNode; }
        }

        public LanguageNode RightOperand
        {
            get
            {
                return this.Children[1] as LanguageNode;
            }
        }

        public override void CheckSemantics(Scope scope, Report report)
        {
            LeftOperand.CheckSemantics(scope, report);
            RightOperand.CheckSemantics(scope, report);
            
            //No tener errores en los operandos
            if (LeftOperand.returnType.isError || RightOperand.returnType.isError)
            {
                returnType = scope.FindType("error");
                return;
            }

            //Verificar que sean del mismo tipo
            if (!LeftOperand.returnType.Alike(RightOperand.returnType))
            {
                report.Add(LeftOperand.Line, LeftOperand.CharPositionInLine, string.Format("Can't apply operator {0} between types {1} and {2}", Text, LeftOperand.returnType.name, RightOperand.returnType.name));
                returnType = scope.FindType("error");
                return;
            }

            returnType = LeftOperand.returnType;           
        }
        
        public abstract OpCode OperatorOpCode { get; }
    }
}
