using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Antlr.Runtime;
using Tiger.Semantics;
using System.Reflection.Emit;

namespace Tiger.AST
{
    public class ForNode : LoopNode
    {
        public NameNode id { get { return Children[0] as NameNode; } }
        public LanguageNode from { get { return Children[1] as LanguageNode; } }
        public LanguageNode to { get { return Children[2] as LanguageNode; } }
        public LanguageNode block { get { return Children[3] as LanguageNode; } }
        public VariableInfo iterVar { get; set; }

        public ForNode() : base() { }
        public ForNode(IToken t) : base(t) { }
        public ForNode(ForNode n) : base(n) { }

        public override void CheckSemantics(Scope scope, Report report)
        {
            id.CheckSemantics(scope, report);

            Scope forScope = scope.AddChild(ScopeType.For);

            from.CheckSemantics(scope, report);
            to.CheckSemantics(scope, report);            

            if (id.returnType.isError || from.returnType.isError || to.returnType.isError)
            {
                returnType = scope.FindType("error");
                return;
            }

            if (!from.returnType.isInt || !to.returnType.isInt)
            {
                report.Add(Line, CharPositionInLine, "For expressions must be integer");
                returnType = scope.FindType("error");
                return;
            }

            iterVar = forScope.AddVariable(id.name, scope.FindType("int"));
            iterVar.readOnly = true;

            block.CheckSemantics(forScope, report);
            if (block.returnType.isError)
            {
                returnType = scope.FindType("error");
                return;
            }
                        
            returnType = scope.FindType("void");
        }

        public override void Generate(CodeGenerator cg)
        {
            Label forStart = cg.generator.DefineLabel();
            loopEnd = cg.generator.DefineLabel();

            iterVar.Create(cg);

            from.Generate(cg);//generar codigo del from
            cg.generator.Emit(OpCodes.Stsfld, iterVar.fieldBuilder);//ponerselo a la variable para iteracion

            cg.generator.BeginScope();

            LocalBuilder upper = cg.generator.DeclareLocal(typeof(int));//para guardar el valor que retorno 'to'
            to.Generate(cg);
            cg.generator.Emit(OpCodes.Stloc, upper);//Guardar lo que devolvio 'to' en upper        

            cg.generator.MarkLabel(forStart);
            cg.generator.Emit(OpCodes.Ldsfld, iterVar.fieldBuilder);
            cg.generator.Emit(OpCodes.Ldloc, upper);            
            cg.generator.Emit(OpCodes.Bgt, loopEnd);

            block.Generate(cg);

            if (!block.returnType.isVoid)
                cg.generator.Emit(OpCodes.Pop);
                        
            //Adicionar 1
            cg.generator.Emit(OpCodes.Ldc_I4_1);
            cg.generator.Emit(OpCodes.Ldsfld, iterVar.fieldBuilder);
            cg.generator.Emit(OpCodes.Add);
            cg.generator.Emit(OpCodes.Stsfld, iterVar.fieldBuilder);
            cg.generator.Emit(OpCodes.Br, forStart);

            cg.generator.MarkLabel(loopEnd);

            cg.generator.EndScope();
        }
    }
}

