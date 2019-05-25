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
    class IndexerNode : AccessNode
    {
        public LanguageNode index { get { return Children[0] as LanguageNode; } }
        public ArrayDecNode declaration;

        public IndexerNode() : base() { }
        public IndexerNode(IToken t) : base(t) { }
        public IndexerNode(IndexerNode n) : base(n) { }

        public override void CheckSemantics(Scope scope, Report report)
        {
        }

        public void CheckSemantics(Scope scope, Report report, ArrayDecNode node)
        {
            index.CheckSemantics(scope, report);
            if (index.returnType.isError || !index.returnType.Alike(scope.FindType("int")))
            {
                report.Add(Line, CharPositionInLine, string.Format("An array indexer must be int"));
                returnType = scope.FindType("error");
                return;
            }
            
            declaration = node;
        }


        public override void Generate(CodeGenerator cg)
        {
            index.Generate(cg);
            cg.generator.Emit(OpCodes.Ldelem, declaration.itemsTypeInfo.GetType(cg));//poner en la pila el valor del array en el indice que la generacion de index dejo en la pila
        }


        public override void GenerateForAssign(CodeGenerator cg)
        {
            //Para asignar tiene que tener en la pila el valor asignar y despues el index
            LocalBuilder toAssign = cg.generator.DeclareLocal(declaration.declaredType.GetType(cg));//valor a asignar
            cg.generator.Emit(OpCodes.Stloc, toAssign);

            LocalBuilder ind = cg.generator.DeclareLocal(typeof(int));
            cg.generator.Emit(OpCodes.Stloc, ind);

            cg.generator.Emit(OpCodes.Ldloc, ind);

            cg.generator.Emit(OpCodes.Ldloc, toAssign);//poner la parte derecha de la asignacion en la pila
            cg.generator.Emit(OpCodes.Stelem, declaration.itemsTypeInfo.GetType(cg));
            //para el opcode anterior el index tiene que estar en el tope, mas bajo el valor y despues la referencia
        }
    }
}
