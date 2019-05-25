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
    public class ArrayCreationNode : LanguageNode
    {
        public NameNode id { get { return Children[0] as NameNode; } }
        public LanguageNode lenghtExpr { get { return Children[1] as LanguageNode; } }
        public LanguageNode typeExpr { get { return Children[2] as LanguageNode; } }

        public TypeInfo itemsTypeinfo;

        public ArrayCreationNode() : base() { }
        public ArrayCreationNode(IToken t) : base(t) { }
        public ArrayCreationNode(ArrayCreationNode n) : base(n) { }

        //TOOD ver si tengo que verificar que este en el in de un let
        public override void CheckSemantics(Scope scope, Report report)
        {   //Verificar que exista el tipo del que se esta creando el array
            TypeInfo idInfo = scope.FindType(id.name);
            if (idInfo == null)
            {
                report.Add(id.Line, id.CharPositionInLine, string.Format("Type {0} doesn't exists in the current context", id.name));
                returnType = scope.FindType("error");
                return;
            }

            if (!idInfo.isArray)
            {
                report.Add(id.Line, id.CharPositionInLine, string.Format("Can't create array of {0}", id.name));
                returnType = scope.FindType("error");
                return;
            }

            //Verificar que el lenght del array sea entero
            lenghtExpr.CheckSemantics(scope, report);
            if (!lenghtExpr.returnType.Alike(scope.FindType("int")))
            {
                report.Add(lenghtExpr.Line, lenghtExpr.CharPositionInLine, string.Format("Array's lenght must be int not {0}", lenghtExpr.returnType.name));
                returnType = scope.FindType("error");
                return;
            }

            typeExpr.CheckSemantics(scope, report);
            if (typeExpr.returnType.isError)
            {
                returnType = scope.FindType("error");
                return;
            }

            
            itemsTypeinfo = (idInfo.declaration.declaredType.declaration as ArrayDecNode).itemsTypeInfo;

            if (!typeExpr.returnType.Alike(itemsTypeinfo))
            {
                report.Add(typeExpr.Line, typeExpr.CharPositionInLine, string.Format("Can't create array of {0}", typeExpr.returnType.name));
                returnType = scope.FindType("error");
                return;
            }

            returnType = idInfo.declaration.declaredType;
            
        }

        public override void Generate(CodeGenerator cg)
        {
            //Etiquetas para el ciclo que inicializa el array
            Label init = cg.generator.DefineLabel();
            Label end = cg.generator.DefineLabel();

            //Variables
            //Nota no poner la expresion a inicializar como una variable porque no se el tipo
            LocalBuilder lenght = cg.generator.DeclareLocal(typeof(int));
            LocalBuilder i = cg.generator.DeclareLocal(typeof(int));

            Type systemItemsType = itemsTypeinfo.GetType(cg);

            lenghtExpr.Generate(cg);//pone en tope de la pila la cant de elementos
            cg.generator.Emit(OpCodes.Dup);//uno para almacenar en la variable y otro para crear el array
            cg.generator.Emit(OpCodes.Stloc, lenght);
            cg.generator.Emit(OpCodes.Newarr, systemItemsType);//crear el array

            //Aqui empieza el proceso de inicializar el array            
            cg.generator.Emit(OpCodes.Ldc_I4_0);
            cg.generator.Emit(OpCodes.Stloc, i);//hacer i=0

            //ciclo de inicializacion
            cg.generator.MarkLabel(init);
            cg.generator.Emit(OpCodes.Ldloc, lenght);
            cg.generator.Emit(OpCodes.Ldloc, i);

            cg.generator.Emit(OpCodes.Ble, end);//ver si i<lenght

            cg.generator.Emit(OpCodes.Dup);//duplicar la referencia al array porque una se va a sacar al inicializar el actual elemento

            cg.generator.Emit(OpCodes.Ldloc, i);
            typeExpr.Generate(cg);//poner el valor a inicializar en el tope de la pila
            cg.generator.Emit(OpCodes.Stelem, systemItemsType);//poner el valor en el indice del array, los parametros en la pila son: ref al array, indice, item, (item en el tope)

            //incrementar i
            cg.generator.Emit(OpCodes.Ldloc, i);
            cg.generator.Emit(OpCodes.Ldc_I4_1);
            cg.generator.Emit(OpCodes.Add);
            cg.generator.Emit(OpCodes.Stloc, i);


            cg.generator.Emit(OpCodes.Br, init);
            cg.generator.MarkLabel(end);
            //La generacion deja en el tope de la pila la referencia al array inicializado
        }
    }
}
