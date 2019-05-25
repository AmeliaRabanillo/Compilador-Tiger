using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Tiger.AST;
using System.Reflection.Emit;

namespace Tiger.Semantics
{
    public abstract class ItemInfo
    {
        public string ConvertedName;
        public ItemInfo(string name) { ConvertedName = name; }
    }

    public class VariableInfo : ItemInfo
    {
        public string name;
        public TypeInfo type;
        public FieldBuilder fieldBuilder;//lo que devuelve reflection al crear la variable
        public bool readOnly { get; set; }

        public VariableInfo(string pname, TypeInfo ptype, string pscopeIdName) : base(pscopeIdName)
        {
            name = pname;
            type = ptype;
        }

        public VariableInfo(string pname, TypeInfo ptype) : base(pname)
        {
            name = pname;
            type = ptype;
        }

        public void Create(CodeGenerator cg)
        {
            if (fieldBuilder == null)
                fieldBuilder = cg.typeBuilder.DefineField(ConvertedName, type.GetType(cg), FieldAttributes.Static | FieldAttributes.Public);
        }
        
    }

    public class FunctionInfo : ItemInfo
    {
        public string name { get; private set; }
        public MethodInfo method;
        public bool isStandar { get { return StandarScope.standarFunctions.ContainsKey(name); } }

        public FunctionInfo(string pname, string scopeName, FunctionDecNode pdeclaration = null) : base(scopeName)
        {
            name = pname;
            declaration = pdeclaration;
        }

        public FunctionInfo(string pname, FunctionDecNode pdeclaration = null) : base(pname)
        {
            name = pname;
            declaration = pdeclaration;
        }

        public FunctionDecNode declaration { get; private set; }

        public List<TypeInfo> parameters
        {
            get
            {
                switch (name)
                {
                    case "getline":
                        return new List<TypeInfo>() {};

                    case "printi":
                    case "chr":
                    case "not":
                    case "exit":
                    case "printiline":
                        return new List<TypeInfo>() { new TypeInfo("int", Types.Int) };

                    case "print":
                    case "ord":
                    case "size":
                    case "printline":
                        return new List<TypeInfo>() { new TypeInfo("string", Types.String) };

                    case "substring":
                        return new List<TypeInfo>() { new TypeInfo("string", Types.String), new TypeInfo("int", Types.Int), new TypeInfo("int", Types.Int) };

                    case "concat":
                        return new List<TypeInfo>() { new TypeInfo("string", Types.String), new TypeInfo("string", Types.String) };
                }
                return declaration.paramsTypes;
            }
        }

        public TypeInfo returnType
        {
            get
            {
                switch (name)
                {
                    case "print":
                    case "printi":
                    case "flush":
                    case "exit":
                    case "printiline":
                    case "printline":
                        return new TypeInfo("void", Types.Void);

                    case "getchar":
                    case "chr":
                    case "substring":
                    case "concat":
                    case "getline":
                        return new TypeInfo("string", Types.String);


                    case "ord":
                    case "not":
                    case "size":
                        return new TypeInfo("int", Types.Int);

                    default:
                        return declaration.returnType;

                }
            }
        }
    }

    public class TypeInfo : ItemInfo
    {
        public Types type { get; private set; }
        public TypeDecNode declaration;
        public string name { get; private set; }

        public bool isInt { get { return GetBase().type == Types.Int; } }
        public bool isString { get { return GetBase().type == Types.String; } }
        public bool isError { get { return type == Types.Error; } }
        public bool isVoid { get { return type == Types.Void; } }
        public bool isOK { get { return type == Types.Ok; } }
        public bool isNull { get { return type == Types.Nil; } }
        public bool isUserDefine { get { return type == Types.User; } }
        public bool isArray
        {
            get
            {
                try
                {
                    return GetBase().declaration is ArrayDecNode;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool isRecord
        {
            get
            {
                //ver que sea definido por el usuario y que su declaracion sea un alias a un record o un record
                return (declaration != null && (GetBase().declaration as RecordDecNode != null));
            }
        }


        public TypeInfo(string pname, string scopeName, Types ptype, TypeDecNode pdeclaration = null):base(scopeName)
        {
            name = pname;
            type = ptype;
            declaration = pdeclaration;
        }

        public TypeInfo(string pname, Types ptype, TypeDecNode pdeclaration = null) : base(pname)
        {
            name = pname;
            type = ptype;
            declaration = pdeclaration;
        }

        public bool Alike(TypeInfo type)
        {
            //    if (type.type == this.type && !type.isUserDefine)
            //        return true;

            if ((type.isNull && !isInt) || (!type.isInt && isNull))
                return true;

            var a = type.GetBase();
            var b = GetBase();
            return type.GetBase().name == GetBase().name;
        }

        public bool Alike(Types type)
        {
            return this.type == type;
        }

        public Type GetType(CodeGenerator cg)
        {
            if (isUserDefine)
            {
                if (declaration.realType == null)
                    declaration.Generate(cg);

                return declaration.realType;
            }

            if (isInt)
                return typeof(int);

            if (isVoid)
                return typeof(void);

            if (isString)
                return typeof(string);

            return null;
        }

        public TypeInfo GetBase()
        {
            if (!(declaration is AliasDecNode))
                return this;

            return (declaration as AliasDecNode).declaredType;
        }
    }

    public enum Types { Int, String, Void, Nil, User, Error, Ok }
}
