using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;
using Tiger.Semantics;

namespace Tiger.AST
{
    public class TypeDecBlockNode : BlockDecNode
    {
        public List<TypeDecNode> simpleTypes;

        public TypeDecBlockNode() : base() { }
        public TypeDecBlockNode(IToken t) : base(t) { }
        public TypeDecBlockNode(TypeDecBlockNode n) : base(n) { }

        public void Create()
        {
            simpleTypes = new List<TypeDecNode>();
            foreach (var item in Children)
                simpleTypes.Add((item as TypeDecNode));
        }

        public override void CheckSemantics(Scope scope, Report report)
        {
            Create();

            #region Eliminar las definiciones ya existentes
            List<string> types_names = new List<string>();

            for (int i = 0; i < simpleTypes.Count; i++)
            {
                if (simpleTypes[i].id.name == "string" || simpleTypes[i].id.name == "int" || scope.ShortFindType(simpleTypes[i].id.name) != null || types_names.Contains(simpleTypes[i].id.name))
                {
                    report.Add(simpleTypes[i].Line, simpleTypes[i].CharPositionInLine, string.Format("Cant define type {0}", simpleTypes[i].id.name));
                    returnType = scope.FindType("error");
                    return;
                }

                types_names.Add(simpleTypes[i].id.name);
            }

            #endregion

            Graph graph = new Graph(simpleTypes);
            var toAnalize = graph.pending;
            var circularity = graph.circularity;

            foreach (var item in circularity)
            {
                report.Add(item[0].Line, item[0].CharPositionInLine,string.Format("Type {0} is circular", item[0].id.name) );
                returnType = scope.FindType("error");
                return;
            }
            foreach (var item in toAnalize)
                scope.AddType(item.id.name, Types.User, item);

            foreach (var item in toAnalize)
            {
                item.CheckSemantics(scope, report);
                if (item.returnType != null && item.returnType.isError)
                {
                    returnType = scope.FindType("error");
                    return;
                }
                
                item.returnType = scope.FindType("void");
            }
            
            for (int i = 0; i < toAnalize.Count; i++)
            {
                scope.AddType(toAnalize[i].id.name, Types.User, toAnalize[i]);
                if (!toAnalize[i].returnType.isError)
                    toAnalize[i].SetDeclaredType(scope);
            } 
        }
    }
}
