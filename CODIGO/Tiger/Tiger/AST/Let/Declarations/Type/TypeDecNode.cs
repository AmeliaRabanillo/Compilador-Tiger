using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;
using Tiger.Semantics;

namespace Tiger.AST
{
    public abstract class TypeDecNode : DeclarationNode
    {
        public NameNode id { get { return Children[0] as NameNode; } }//identificador que se le esta poniendo a lo que se va a declarar
        public TypeInfo declaredType;
        public Type realType;//para guardar el tipo despues de que lo cree y poder hacer referencia en generacion de codigo
        protected string ConvertedName;

        public TypeDecNode() : base() { }
        public TypeDecNode(IToken t) : base(t) { }
        public TypeDecNode(TypeDecNode n) : base(n) { }

        public abstract void SetDeclaredType(Scope scope);
        public abstract void SetTypeAfterCheck(Scope scope, Report report);
        public void SetConvertedName(Scope scope)
        {
            ConvertedName = scope.ConvertTo(id.name);
        }

        public override void CheckSemantics(Scope scope, Report report)
        {
            SetConvertedName(scope);
        }
    }
}
