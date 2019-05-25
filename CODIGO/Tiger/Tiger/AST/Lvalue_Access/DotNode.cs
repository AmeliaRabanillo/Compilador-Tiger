using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiger.Semantics;
using Antlr.Runtime;
using System.Reflection;
using System.Reflection.Emit;

namespace Tiger.AST
{
    public class DotNode : AccessNode
    {
        public NameNode id { get { return Children[0] as NameNode; } }
        public RecordDecNode recordNode;
        public Semantics.TypeInfo fieldType;//El field al que se quiere acceder

        public DotNode() : base() { }
        public DotNode(IToken t) : base(t) { }
        public DotNode(DotNode n) : base(n) { }

        public void CheckSemantics(Scope scope, Report report, RecordDecNode _record)
        {
            recordNode = _record;
            var fieldList = _record.paramsNode.parameters;
            for (int i = 0; i < fieldList.Count; i++)
                if (fieldList[i].id.name == id.name)
                {
                    fieldType = recordNode.infos[i];
                    return;
                }

            returnType = scope.FindType("error");
        }

        public override void Generate(CodeGenerator cg)
        {
            FieldInfo fieldInfo = recordNode.declaredType.declaration.realType.GetField(id.name);
            cg.generator.Emit(OpCodes.Ldfld, fieldInfo);//sustituir la referencia al objeto que esta en la pila por lo especificado en fieldinfo
        }

        public override void GenerateForAssign(CodeGenerator cg)
        {
            //A esto le tiene que llegar lo que va asignar en el tope de la pila
            FieldInfo fieldInfo = recordNode.declaredType.declaration.realType.GetField(id.name);
            cg.generator.Emit(OpCodes.Stfld, fieldInfo);
        }

        public override void CheckSemantics(Scope scope, Report report)
        {
        }
    }
}
