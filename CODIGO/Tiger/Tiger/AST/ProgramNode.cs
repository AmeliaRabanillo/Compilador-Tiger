using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tiger.Semantics;
using System.Reflection.Emit;
using Antlr.Runtime;


namespace Tiger.AST
{
    public class ProgramNode : LanguageNode
    {
        public ProgramNode() : base() { }
        public ProgramNode(IToken t) : base(t) { }
        public ProgramNode(ProgramNode n) : base(n) { }

        public Scope programNodeScope;

        public LanguageNode program { get { return Children[0] as LanguageNode; } }

        public override void CheckSemantics(Scope scope, Report report)
        {
            programNodeScope = scope;
            program.CheckSemantics(scope, report);
            if (program.returnType.isError)
                returnType = scope.FindType("error");

            var info = scope.FindType(program.returnType.name);
            if (info == null)
                report.Add(Line, CharPositionInLine, string.Format("Cant fint type {0}", program.returnType.name));
        }

        public override void Generate(CodeGenerator cg)
        {
            programNodeScope.standarScope.GenerateStandarLibrary(cg);
            program.Generate(cg);

            if (!program.returnType.isVoid)
                cg.generator.Emit(OpCodes.Pop);

            cg.generator.Emit(OpCodes.Ldc_I4_0);
            cg.generator.Emit(OpCodes.Call, typeof(Environment).GetMethod("Exit"));
        }
    }
}
