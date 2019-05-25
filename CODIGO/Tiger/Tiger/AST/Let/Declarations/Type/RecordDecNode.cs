using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;
using Tiger.Semantics;
using System.Reflection.Emit;
using System.Reflection;

namespace Tiger.AST
{
    public class RecordDecNode : TypeDecNode
    {
        //type name = {name1:type1,...}
        public FieldsDecNode paramsNode { get { return Children[1] as FieldsDecNode; } }
        public List<Semantics.TypeInfo> infos;//typeinfo de los campos que se estan intentando crear

        public RecordDecNode() : base() { }
        public RecordDecNode(IToken t) : base(t) { }
        public RecordDecNode(RecordDecNode n) : base(n) { }

        public override void CheckSemantics(Scope scope, Report report)
        {
            base.CheckSemantics(scope, report);

            infos = new List<Semantics.TypeInfo>();
            id.CheckSemantics(scope, report);

            paramsNode.CheckSemantics(scope, report, infos);

            if ((id.returnType != null && id.returnType.isError) || (paramsNode.returnType != null && paramsNode.returnType.isError))
            {
                returnType = scope.FindType("error");
                return;
            }
        }

        public override void Generate(CodeGenerator cg)
        {
            if (realType == null)
            {
                TypeBuilder typeBuilder = cg.moduleBuilder.DefineType(ConvertedName, TypeAttributes.Public);

                realType = typeBuilder;
                var parametersList = paramsNode.parameters;

                for (int i = 0; i < parametersList.Count; i++)
                {
                    Type type = null;
                    if (infos[i].name == id.name)
                        type = realType;
                    else
                        type = infos[i].GetType(cg);

                    //a esto no tengo que convertirle el nombre porque en un mismo record no puedo tener dos fields con el mismo nombre
                    typeBuilder.DefineField(parametersList[i].id.name, type, FieldAttributes.Public);
                }

                ConstructorBuilder constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, System.Type.EmptyTypes);
                ILGenerator gen = constructor.GetILGenerator();
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Call, typeof(object).GetConstructor(System.Type.EmptyTypes));
                gen.Emit(OpCodes.Ret);

                typeBuilder.CreateType();
            }
        }

        public override void SetDeclaredType(Scope scope)
        {
            if (this.declaredType == null)
            {
                declaredType = scope.FindType(id.name);

                for (int i = 0; i < infos.Count; i++)
                {
                    if (infos[i].declaration != null && infos[i].declaration.declaredType == null)//TOOD ver esto que significa exactamente
                        infos[i].declaration.SetDeclaredType(scope);
                    infos[i] = infos[i].GetBase();
                }
            }
        }

        public override void SetTypeAfterCheck(Scope scope, Report report)
        {
            foreach (var item in infos)
            {
                if (item.declaration.returnType != null && item.declaration.returnType.isError)
                {
                    returnType = scope.FindType("error");
                    return;
                }
            }
        }
    }
}
