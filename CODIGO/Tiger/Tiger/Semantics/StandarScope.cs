using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Tiger.Semantics
{
    public class StandarScope
    {
        public static Dictionary<string, FunctionInfo> standarFunctions { get; private set; }
        public static Dictionary<string, TypeInfo> standarTypes { get; private set; }

        public StandarScope()
        {
            setPublicFunctions();
            setPublicTypes();
        }

        void setPublicTypes()
        {
            standarTypes = new Dictionary<string, TypeInfo>();

            standarTypes["int"] = new TypeInfo("int", Types.Int);
            standarTypes["string"] = new TypeInfo("string", Types.String);
            standarTypes["void"] = new TypeInfo("void", Types.Void);
            standarTypes["nil"] = new TypeInfo("nil", Types.Nil);
            standarTypes["error"] = new TypeInfo("error", Types.Error);
        }

        void setPublicFunctions()
        {
            standarFunctions = new Dictionary<string, FunctionInfo>();

            standarFunctions["print"] = new FunctionInfo("print", null);
            standarFunctions["printi"] = new FunctionInfo("printi", null);
            standarFunctions["printline"] = new FunctionInfo("printline", null);
            standarFunctions["printiline"] = new FunctionInfo("printiline", null);
            standarFunctions["getline"] = new FunctionInfo("getline", null);
            standarFunctions["ord"] = new FunctionInfo("ord", null);
            standarFunctions["chr"] = new FunctionInfo("chr", null);
            standarFunctions["size"] = new FunctionInfo("size", null);
            standarFunctions["substring"] = new FunctionInfo("substring", null);
            standarFunctions["concat"] = new FunctionInfo("concat", null);
            standarFunctions["not"] = new FunctionInfo("not", null);
            standarFunctions["exit"] = new FunctionInfo("exit", null);
        }

        public FunctionInfo FindStandarFunction(string name)
        {
            if (standarFunctions.ContainsKey(name))
                return standarFunctions[name];

            return null;
        }

        public TypeInfo FindStandarType(string name)
        {
            if (standarTypes.ContainsKey(name))
                return standarTypes[name];

            return null;
        }

        private int nextScopeId;

        #region Predefined Functions
        public void GenerateStandarLibrary(CodeGenerator cg)
        {
            PrintGenerate(cg);
            PrintiGenerate(cg);
            OrdGenerate(cg);
            ChrGenerate(cg);
            SizeGenerate(cg);
            SubstringGenerate(cg);
            ConcatGenerate(cg);
            NotGenerate(cg);
            ExitGenerate(cg);
            GetLineGenerate(cg);
            PrintLineGenerate(cg);
            PrintiLineGenerate(cg);
        }

        public void PrintGenerate(CodeGenerator cg)
        {
            standarFunctions["print"].method = typeof(Console).GetMethod("Write", new Type[] { typeof(string) });
        }

        public void PrintiGenerate(CodeGenerator cg)
        {
            standarFunctions["printi"].method = typeof(Console).GetMethod("Write", new Type[] { typeof(int) });
        }

        public void OrdGenerate(CodeGenerator cg)
        {
            MethodBuilder methodB = cg.typeBuilder.DefineMethod("ord", System.Reflection.MethodAttributes.Public | System.Reflection.MethodAttributes.Static, typeof(int), new Type[] { typeof(string) });
            ILGenerator generator = methodB.GetILGenerator();

            Label emptyStr = generator.DefineLabel();
            Label end = generator.DefineLabel();

            generator.Emit(OpCodes.Ldarg_0);
            MethodInfo length = typeof(string).GetProperty("Length").GetGetMethod();
            generator.Emit(OpCodes.Call, length);
            generator.Emit(OpCodes.Ldc_I4_0);
            generator.Emit(OpCodes.Beq, emptyStr);

            //not empty string
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldc_I4_0);
            MethodInfo char0 = typeof(string).GetProperty("Chars").GetGetMethod();
            generator.Emit(OpCodes.Call, char0);
            generator.Emit(OpCodes.Conv_I);
            generator.Emit(OpCodes.Br, end);

            //empty string
            generator.MarkLabel(emptyStr);
            generator.Emit(OpCodes.Ldc_I4_M1);

            generator.MarkLabel(end);
            generator.Emit(OpCodes.Ret);

            standarFunctions["ord"].method = methodB as MethodInfo;
        }

        public void ChrGenerate(CodeGenerator cg)
        {
            MethodBuilder methodB = cg.typeBuilder.DefineMethod("chr", MethodAttributes.Static | MethodAttributes.Public, typeof(string), new Type[] { typeof(int) });
            ILGenerator generator = methodB.GetILGenerator();

            Label exception = generator.DefineLabel();
            Label end = generator.DefineLabel();

            //TODO en que rango estan los ASCII hasta 0-256???
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldc_I4_0);
            generator.Emit(OpCodes.Ble, exception);//Si el parametro es menor que 0 ir lanzar excepcion

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldc_I4, 256);
            generator.Emit(OpCodes.Bge, exception);//Si el parametro es mayor de 256 is a lanzar excepcion
            
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Conv_U2);

            generator.Emit(OpCodes.Box, typeof(char));//Casteando el numero a char

            MethodInfo toChar = typeof(object).GetMethod("ToString", System.Type.EmptyTypes);
            
            generator.Emit(OpCodes.Callvirt, toChar);
            generator.Emit(OpCodes.Br, end);

            generator.MarkLabel(exception);
            generator.Emit(OpCodes.Newobj, typeof(ArgumentOutOfRangeException).GetConstructor(System.Type.EmptyTypes));
            generator.Emit(OpCodes.Throw);

            generator.MarkLabel(end);
            generator.Emit(OpCodes.Ret);

            standarFunctions["chr"].method = methodB as MethodInfo;
        }

        public void SizeGenerate(CodeGenerator cg)
        {
            MethodBuilder methodB = cg.typeBuilder.DefineMethod("size", MethodAttributes.Public | MethodAttributes.Static, typeof(int), new Type[] { typeof(string) });
            ILGenerator generator = methodB.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Callvirt, typeof(string).GetProperty("Length").GetGetMethod());
            generator.Emit(OpCodes.Ret);

            standarFunctions["size"].method = methodB as MethodInfo;
        }

        public void SubstringGenerate(CodeGenerator cg)
        {
            MethodBuilder methodB = cg.typeBuilder.DefineMethod("substring", MethodAttributes.Public | MethodAttributes.Static, typeof(string), new Type[] { typeof(string), typeof(int), typeof(int) });
            ILGenerator generator = methodB.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Ldarg_2);
            generator.Emit(OpCodes.Callvirt, typeof(string).GetMethod("Substring", new Type[] { typeof(int), typeof(int) }));
            generator.Emit(OpCodes.Ret);

            standarFunctions["substring"].method = methodB as MethodInfo;
        }

        public void ConcatGenerate(CodeGenerator cg)
        {
            standarFunctions["concat"].method = typeof(string).GetMethod("Concat", new Type[] { typeof(string), typeof(string) });
        }

        public void NotGenerate(CodeGenerator cg)
        {
            MethodBuilder methodB = cg.typeBuilder.DefineMethod("not", MethodAttributes.Static | MethodAttributes.Public, typeof(int), new Type[] { typeof(int) });
            ILGenerator generator = methodB.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldc_I4_0);
            generator.Emit(OpCodes.Ceq);
            generator.Emit(OpCodes.Ret);

            standarFunctions["not"].method = methodB as MethodInfo;

        }

        public void ExitGenerate(CodeGenerator cg)
        {
            standarFunctions["exit"].method = typeof(Environment).GetMethod("Exit");
        }

        public void GetLineGenerate(CodeGenerator cg)
        {
            standarFunctions["getline"].method = typeof(Console).GetMethod("ReadLine");
        }

        public void PrintLineGenerate(CodeGenerator cg)
        {
            MethodBuilder methodB = cg.typeBuilder.DefineMethod("printline", MethodAttributes.Public | MethodAttributes.Static, typeof(void), new Type[] { typeof(string) });
            ILGenerator generator = methodB.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0);

            MethodInfo printLine = typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) });
            
            generator.Emit(OpCodes.Call, printLine);
            generator.Emit(OpCodes.Ret);

            standarFunctions["printline"].method = methodB as MethodInfo;

        }

        public void PrintiLineGenerate(CodeGenerator cg)
        {
            MethodBuilder methodB = cg.typeBuilder.DefineMethod("printiline", MethodAttributes.Public | MethodAttributes.Static, typeof(void), new Type[] { typeof(string) });
            ILGenerator generator = methodB.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0);

            MethodInfo printLine = typeof(Console).GetMethod("WriteLine", new Type[] { typeof(int) });
            
            generator.Emit(OpCodes.Call, printLine);
            generator.Emit(OpCodes.Ret);

            standarFunctions["printiline"].method = methodB as MethodInfo;
        }

        #endregion

        public int GetId()
        {
            return nextScopeId++;
        }
    }
}
