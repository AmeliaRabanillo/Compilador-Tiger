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
    class FuncCallNode : OthersNode
    {
        public string name { get { return Children[0].ToString(); } }

        public ExprListNode Params { get { return Children[1] as ExprListNode; } }

        public FunctionInfo function { get; private set; }

        public FuncCallNode() : base() { }
        public FuncCallNode(IToken t) : base(t) { }
        public FuncCallNode(FuncCallNode n) : base(n) { }


        public override void CheckSemantics(Scope scope, Report report)
        {
            FunctionInfo func = scope.FindFunction(name);
            if (func == null)
            {
                report.Add(Line, CharPositionInLine, string.Format("Function {0} isn't defined", name));
                returnType = scope.FindType("error");
                return;
            }
            
            if (Params.Check(func,scope,report))
            {
                returnType = func.returnType;
                function = func;
                return;
            }

            returnType = scope.FindType("error");
        }

        public override void Generate(CodeGenerator cg)
        {
            Params.Generate(cg);
            if (function.isStandar)
            {
                cg.generator.Emit(OpCodes.Call, function.method);
                return;
            }

            cg.generator.Emit(OpCodes.Call, function.declaration.builder);
        }
    }
}
