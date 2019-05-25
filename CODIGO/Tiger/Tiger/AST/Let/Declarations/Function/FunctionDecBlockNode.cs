using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiger.Semantics;
using Antlr.Runtime;

namespace Tiger.AST
{
    public class FunctionDecBlockNode : BlockDecNode
    {
        public List<FunctionDecNode> functions;

        public FunctionDecBlockNode() : base() { }
        public FunctionDecBlockNode(IToken t) : base(t) { }
        public FunctionDecBlockNode(FunctionDecBlockNode n) : base(n) { }

        public void Create()
        {
            functions = new List<FunctionDecNode>();

            foreach (var item in Children)
                functions.Add(item as FunctionDecNode);
        }

        public override void CheckSemantics(Scope scope, Report report)
        {
            Create();
            foreach (var func in functions)
            {//agregar las funciones primero porque en el cuerpo se pueden usar funciones que esten declaradas debajo

                func.funcScope = scope.AddChild(ScopeType.Function);
                List<string> names = new List<string>();

                func.paramList.Create();
                foreach (var param in func.paramList.parameters)
                {
                    if (names.Contains(param.id.name))
                    {
                        report.Add(Line, CharPositionInLine, string.Format("Two parameters of function {0} cant have the name {1}", func.id.name, param.id.name));
                        returnType = scope.FindType("error");
                        return;
                    }

                    names.Add(param.id.name);
                    func.funcScope.AddVariable(param.id.name, param.GetTypeInfo(func.funcScope));
                }

                #region id check
                //esto no lo puedo hacer en el nodo de la funcion porque necesito agregar la funcion en el scope del let
                func.id.CheckSemantics(scope, report);

                if (func.id.returnType.isError || scope.ShortFindFunction(func.id.name) != null || scope.ShortFindVariable(func.id.name) != null)
                {//si entra aqui es que el id esta mal, o ya existe una variable y una funcion con el mismo id
                    report.Add(Line, CharPositionInLine, string.Format("Can't create function {0}", func.id.name));
                    returnType = scope.FindType("error");
                }
                #endregion
                
                scope.AddFunction(func.id.name, func);
                func.PutReturnType(func.funcScope);
                func.UpdateTypes(func.funcScope);
            }
            foreach (var func in functions)
            {         
                //No puedo chequear la funcion al principio porque no estarian agregadas las variables, ni los tipos de los parametros
                func.CheckSemantics(func.funcScope, report);

                if (func.returnType != null && func.returnType.isError)
                {
                    returnType = scope.FindType("error");
                    return;
                }
            }
        }

        public override void Generate(CodeGenerator cg)
        {
            foreach (var item in functions)
                item.GenerateFunction(cg);

            foreach (var item in functions)
                item.Generate(cg);
        }
    }
}
