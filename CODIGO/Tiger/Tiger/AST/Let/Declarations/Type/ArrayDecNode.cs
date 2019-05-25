using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;
using Tiger.Semantics;

namespace Tiger.AST
{
    class ArrayDecNode : TypeDecNode
    {
        public NameNode itemsType { get { return Children[1] as NameNode; } }
        public TypeInfo itemsTypeInfo;//el tipo de los items (sin especificar array)
        //el real type es el itemsTypeInfo especificando que es array

        public ArrayDecNode() : base() { }
        public ArrayDecNode(IToken t) : base(t) { }
        public ArrayDecNode(ArrayDecNode n) : base(n) { }

        public override void CheckSemantics(Scope scope, Report report)
        {
            base.CheckSemantics(scope, report);

            id.CheckSemantics(scope, report);
            if (id.returnType.isError)
            {
                returnType = scope.FindType("error");
                return;
            }
            
            //el tipo de los elementos ya existe
            TypeInfo itemsInfo = scope.FindType(itemsType.name);
            if (itemsInfo == null)
            {
                report.Add(itemsType.Line, itemsType.CharPositionInLine, string.Format("Type {0} doesn't exists", itemsType.name));
                returnType = scope.FindType("error");
                return;
            }
            else if (itemsInfo.name == id.name)
            {
                report.Add(itemsType.Line, itemsType.CharPositionInLine, string.Format("Can't define recursive arrays of {0} ", itemsType.name));
                returnType = scope.FindType("error");
                return;
            }

            itemsTypeInfo = itemsInfo;
            returnType = scope.FindType("void");
        }

        public override void Generate(CodeGenerator cg)
        {
            if (realType == null)
                realType = itemsTypeInfo.GetType(cg).MakeArrayType();
        }

        public override void SetDeclaredType(Scope scope)
        {
            if (declaredType == null)
            {
                declaredType = scope.FindType(id.name);

                TypeInfo info = scope.FindType(itemsTypeInfo.name);
                if (info.declaration != null && info.declaration.declaredType == null)
                    info.declaration.SetDeclaredType(scope);

                itemsTypeInfo = info.GetBase();
            }
        }

        public override void SetTypeAfterCheck(Scope scope, Report report)
        {
            var info = scope.ShortFindType(itemsType.name);
            if (info.declaration.returnType != null && info.declaration.returnType.isError)
            {
                returnType = scope.FindType("error");
                return;
            }
        }
    }
}
