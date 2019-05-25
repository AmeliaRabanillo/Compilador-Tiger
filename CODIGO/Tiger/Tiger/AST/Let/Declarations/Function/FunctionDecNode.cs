using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiger.Semantics;
using Antlr.Runtime;
using System.Reflection.Emit;
using System.Reflection;

namespace Tiger.AST
{
    public abstract class FunctionDecNode : DeclarationNode
    {
        public FunctionDecNode() : base() { }
        public FunctionDecNode(IToken t) : base(t) { }
        public FunctionDecNode(FunctionDecNode n) : base(n) { }

        public NameNode id { get { return Children[0] as NameNode; } }
        public FieldsDecNode paramList
        {
            get
            {
                if (this as MethodDecNode != null)
                    return Children[3] as FieldsDecNode;

                return Children[2] as FieldsDecNode;
            }
        }
        public LanguageNode expr
        {
            get
            {
                if (this as MethodDecNode != null)
                    return Children[2] as LanguageNode;

                return Children[1] as LanguageNode;
            }
        }
        public List<Semantics.TypeInfo> paramsTypes { get; set; }
        public Semantics.TypeInfo functionReturnType { get; set; }
        public MethodBuilder builder { get; set; }

        public Scope funcScope;
        //////////////////////
        //el scope de la funcion
        //lo pone el bloque porque en la especificacion 
        //dice que el scope de una funcion empieza en la
        //secuencia de declaraciones a la que pertenece 
        //y termina al final de dicha secuencia de delcaraciones
        //////////////////////

        public override void CheckSemantics(Scope scope, Report report)
        {
            //Chequear los parametros
            paramList.CheckSemantics(scope, report);
            if (paramList.returnType != null && paramList.returnType.isError)
                returnType = scope.FindType("error");
        }
        public override void Generate(CodeGenerator cg)
        {
            //para poder generar codigo en el builder de la funcion
            CodeGenerator funcCG = new CodeGenerator(builder.GetILGenerator(), cg.moduleBuilder, cg.typeBuilder);
            
            List<VariableInfo> infos = new List<VariableInfo>();
            funcScope.AllVariables(infos);

            funcCG.generator.BeginScope();

            List<LocalBuilder> builders = new List<LocalBuilder>();

            for (int i = 0; i < infos.Count; i++)
            {
                infos[i].Create(funcCG);
                builders.Add(funcCG.generator.DeclareLocal(infos[i].type.GetType(cg)));
                funcCG.generator.Emit(OpCodes.Ldsfld, infos[i].fieldBuilder);//Poner el valor de la variable exitente en la pila
                funcCG.generator.Emit(OpCodes.Stloc, builders[i]);//Guardar ese valor anterior en una variable local
            }

            //Cargar los parametros en las variables que cree en la definicion de la funcion
            for (int i = 0; i < paramList.parameters.Count; i++)
            {
                string name = paramList.parameters[i].id.name;
                VariableInfo current = funcScope.FindVariable(name);
                funcCG.generator.Emit(OpCodes.Ldarg, i);
                funcCG.generator.Emit(OpCodes.Stsfld, current.fieldBuilder);
            }
                       

            expr.Generate(funcCG);
            for (int i = 0; i < builders.Count; i++)
            {
                funcCG.generator.Emit(OpCodes.Ldloc, builders[i]);
                funcCG.generator.Emit(OpCodes.Stsfld, infos[i].fieldBuilder);
            }

            funcCG.generator.EndScope();
            funcCG.generator.Emit(OpCodes.Ret);
        }

        public void GenerateFunction(CodeGenerator cg)
        {
            var params_ = new List<Type>();
            foreach (var item in paramsTypes)
                params_.Add(item.GetType(cg));

            FunctionInfo info = funcScope.FindFunction(id.name);
            builder = cg.typeBuilder.DefineMethod(info.ConvertedName, System.Reflection.MethodAttributes.Public | System.Reflection.MethodAttributes.Static,CallingConventions.Standard, functionReturnType.GetType(cg), params_.ToArray());
            
            //buscar las variables para los parametros que recibira y crearlas si no existian
            for(int i=0; i< paramList.parameters.Count;i++)
            {
                FieldDecNode currentField = paramList.parameters[i];
                VariableInfo current = funcScope.ShortFindVariable(currentField.id.name);
                current.Create(cg);
            }
        }

        public void UpdateTypes(Scope scope)
        {
            paramsTypes = new List<Semantics.TypeInfo>();

            foreach (var item in paramList.parameters)
                paramsTypes.Add(item.GetTypeInfo(scope));
        }

        public abstract void PutReturnType(Scope scope);
    }
}
