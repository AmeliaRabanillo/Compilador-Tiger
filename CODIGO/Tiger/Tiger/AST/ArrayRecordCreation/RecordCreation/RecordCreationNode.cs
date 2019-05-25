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
    //Usa FieldListNode y FieldNode, porque FieldDecNode y FieldsDecNode son para cuando declaro los records y las funciones
    public class RecordCreationNode : LanguageNode
    {
        public NameNode id { get { return Children[0] as NameNode; } }
        public FieldListNode fieldList
        {
            get
            {
                return ChildCount > 1 ? Children[1] as FieldListNode : null;
            }
        }
      
        public RecordCreationNode() : base() { }
        public RecordCreationNode(IToken t) : base(t) { }
        public RecordCreationNode(RecordCreationNode n) : base(n) { }

        public override void CheckSemantics(Scope scope, Report report)
        {
            TypeInfo info = scope.FindType(id.name);
            if (info == null)
            {
                report.Add(Line, CharPositionInLine, string.Format("Can't find type {0}", id.name));
                returnType = scope.FindType("error");
                return;
            }

            if (!info.isRecord)//El tipo del que se inteta crear el record debe haber sido declarado anteriormente como un record o un alias a un record
            {
                report.Add(Line, CharPositionInLine, string.Format("Cant create a record of {0}", info.name));
                returnType = scope.FindType("error");
                return;
            }

            RecordDecNode record = info.declaration.declaredType.declaration as RecordDecNode;
            if (fieldList != null)
            {
                fieldList.CheckSemantics(scope, report, record);
                if (fieldList.returnType.isError)
                {
                    returnType = scope.FindType("error");
                    return;
                }
            }

            returnType = info.declaration.declaredType;           
        }

        public override void Generate(CodeGenerator cg)
        {
            Type type = returnType.GetType(cg);

            cg.generator.Emit(OpCodes.Newobj, type.GetConstructor(System.Type.EmptyTypes));

            foreach (var field in fieldList.fields)
            {
                cg.generator.Emit(OpCodes.Dup);//Duplicar la referencia al objeto
                field.expr.Generate(cg);//poner en el tope de la pila el valor a asignar
                cg.generator.Emit(OpCodes.Stfld, type.GetField(field.id.name));// poner en el campo designado el valor en el tope de la pila
            }
        }
    }
}
