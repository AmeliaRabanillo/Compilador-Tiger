using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Tiger.AST;

namespace Tiger.Semantics
{
    public class Scope
    {
        private static string[] standar = { "getline", "printi", "chr", "not", "exit", "printiline", "print", "ord", "size", "printline", "substring", "concat", "void", "string", "int" };

        public StandarScope standarScope;
        private Dictionary<string, VariableInfo> variables;
        private Dictionary<string, FunctionInfo> functions;
        //public static Dictionary<string, FunctionInfo> standarFunctions { get; private set; }
        //public static Dictionary<string, TypeInfo> standarTypes { get; private set; }
        private Dictionary<string, TypeInfo> types;
        public ScopeType type { get; private set; }
        public bool isRoot { get { return parent == null; } }
        public Scope parent { get; private set; }
        public List<Scope> children { get; private set; }
        private int IdNumber;

        public Scope(StandarScope pstandarScope, ScopeType ptype,Scope pparent = null)
        {
            standarScope = pstandarScope;

            variables = new Dictionary<string, VariableInfo>();
            functions = new Dictionary<string, FunctionInfo>();
            types = new Dictionary<string, TypeInfo>();
            children = new List<Scope>();
            parent = pparent;
            type = ptype;


            IdNumber = pstandarScope.GetId();

            //setPublicFunctions();
            //setPublicTypes();
        }

        //void setPublicTypes()
        //{
        //    standarTypes = new Dictionary<string, TypeInfo>();

        //    standarTypes["int"] = new TypeInfo("int", Types.Int);
        //    standarTypes["string"] = new TypeInfo("string", Types.String);
        //    standarTypes["void"] = new TypeInfo("void", Types.Void);
        //    standarTypes["nil"] = new TypeInfo("nil", Types.Nil);
        //    standarTypes["error"] = new TypeInfo("error", Types.Error);
        //}

        //void setPublicFunctions()
        //{
        //    standarFunctions = new Dictionary<string, FunctionInfo>();

        //    standarFunctions["print"] = new FunctionInfo("print", null);
        //    standarFunctions["printi"] = new FunctionInfo("printi", null);
        //    standarFunctions["printline"] = new FunctionInfo("printline", null);
        //    standarFunctions["printiline"] = new FunctionInfo("printiline", null);
        //    standarFunctions["getline"] = new FunctionInfo("getline", null);
        //    standarFunctions["ord"] = new FunctionInfo("ord", null);
        //    standarFunctions["chr"] = new FunctionInfo("chr", null);
        //    standarFunctions["size"] = new FunctionInfo("size", null);
        //    standarFunctions["substring"] = new FunctionInfo("substring", null);
        //    standarFunctions["concat"] = new FunctionInfo("concat", null);
        //    standarFunctions["not"] = new FunctionInfo("not", null);
        //    standarFunctions["exit"] = new FunctionInfo("exit", null);

            
        //}

        #region Find Things
        public TypeInfo FindType(string name)
        {
            string convName = ConvertTo(name);
            if (types.ContainsKey(convName))
                return types[convName];

            if (!isRoot)
                return parent.FindType(name);

            return standarScope.FindStandarType(name);            
        }

        public VariableInfo FindVariable(string name)
        {
            string convName = ConvertTo(name);
            if (variables.ContainsKey(convName))
                return variables[convName];
            
            if (!isRoot)
                return parent.FindVariable(name);
            
            return null;
        }

        public FunctionInfo FindFunction(string name)
        {
            string convName = ConvertTo(name);
            if (functions.ContainsKey(convName))
                return functions[convName];

            if (!isRoot)
                return parent.FindFunction(name);

            return standarScope.FindStandarFunction(name);
        }

        public TypeInfo ShortFindType(string name)
        {
            name = ConvertTo(name);
            if (types.ContainsKey(name))
                return types[name];

            return null;
        }

        public VariableInfo ShortFindVariable(string name)
        {
            name = ConvertTo(name);
            if (variables.ContainsKey(name))
                return variables[name];
            
            return null;
        }
                
        public FunctionInfo ShortFindFunction(string name)
        {
            name = ConvertTo(name);
            if (functions.ContainsKey(name))
                return functions[name];

            return standarScope.FindStandarFunction(name);
        }

        public void AllVariables(List<VariableInfo> list)
        {
            foreach (var item in variables)
                list.Add(item.Value);

            foreach (var item in children)
                item.AllVariables(list);

        }
        #endregion

        #region Add Things
        public TypeInfo AddType(string name, Types type, TypeDecNode typeDeclaration)
        {
            string convNAme = ConvertTo(name);
            TypeInfo newType = new TypeInfo(name, convNAme, type, typeDeclaration);
            types[convNAme] = newType;
            return newType;
        }
        
        public VariableInfo AddVariable(string name, TypeInfo type)
        {
            string convNAme = ConvertTo(name);
            VariableInfo newVar = new VariableInfo(name, type,convNAme);
            variables[convNAme] = newVar;
            return newVar;
        }

        public FunctionInfo AddFunction(string name, FunctionDecNode function)
        {
            string convNAme = ConvertTo(name);
            FunctionInfo newFunc = new FunctionInfo(name, convNAme, function);
            functions[convNAme] = newFunc;
            return newFunc;
        }

        public Scope AddChild(ScopeType t)
        {
            Scope s = new Scope(standarScope, t,this);
            children.Add(s);
            return s;
        }

        #endregion

        public string ConvertTo(string name)
        {
            if (standar.Contains(name))
                return name;

            return string.Concat(name, '_', IdNumber);
        }

        public string ConvertFrom(string name)
        {
            if (standar.Contains(name))
                return name;

            return name.Substring(name.LastIndexOf('_'));
        }
       
    }

    public enum ScopeType { Main, Function, Let, For }
    
}

