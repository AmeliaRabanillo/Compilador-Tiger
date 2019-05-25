using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;
using Tiger.Semantics;

namespace Tiger.AST
{
    class AliasDecNode : TypeDecNode
    {
        public NameNode aliasOf { get { return Children[1] as NameNode; } }//identificador de lo que se haciendo el alias
        
        public AliasDecNode() : base() { }
        public AliasDecNode(IToken t) : base(t) { }
        public AliasDecNode(AliasDecNode n) : base(n) { }

        public override void CheckSemantics(Scope scope, Report report)
        {
            base.CheckSemantics(scope, report);


            id.CheckSemantics(scope, report);
            if (!id.returnType.isOK )
            {
                returnType = scope.FindType("error");
                return;
            }

            if (id.name == "int" || id.name == "string")
            {
                report.Add(Line,CharPositionInLine,"Cant create alias with identifier int or string");
                returnType = scope.FindType("error");
                return;
            }

            //TypeInfo type_id = scope.ShortFindType(id.name);
            //if (type_id != null)
            //{
            //    returnType = scope.FindType("error");
            //    report.Add(Line, CharPositionInLine, string.Format("The type {0} is already defined", type_id.name));
            //    return;
            //}

            aliasOf.CheckSemantics(scope, report);
            if (!aliasOf.returnType.isOK)
            {
                returnType = scope.FindType("error");
                return;
            }

            TypeInfo aliasOfTypeInfo = scope.FindType(aliasOf.name);

            if (aliasOfTypeInfo == null)
            {
                returnType = scope.FindType("error");
                report.Add(Line, CharPositionInLine, string.Format("The type {0} doesn't exists", aliasOf.name));
                return;
            }

            if (aliasOfTypeInfo.name == id.name)
            {
                returnType = scope.FindType("error");
                report.Add(Line, CharPositionInLine, string.Format("Can't define recursive alias of {0}", id.name));
                return;
            }

            returnType = scope.FindType("void");
        }
        
        public override void Generate(CodeGenerator cg)
        {
            if (realType == null)
                realType = declaredType.GetType(cg);
        }

        public override void SetDeclaredType(Scope scope)
        {
            if (declaredType == null)
            {
                TypeInfo info = scope.FindType(aliasOf.name);
                if (info.declaration != null && info.declaration.declaredType == null)
                    info.declaration.SetDeclaredType(scope);

                declaredType = info.GetBase();
            }
        }

        public override void SetTypeAfterCheck(Scope scope, Report report)
        {
            var info = scope.ShortFindType(aliasOf.name);
            if (info.declaration.returnType!=null && info.declaration.returnType.isError)
            {
                returnType = scope.FindType("error");
                return;
            }
        }
    }
}
