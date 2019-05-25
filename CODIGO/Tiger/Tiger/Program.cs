using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Antlr.Runtime;
using Tiger.AST;
using Tiger;
using Tiger.Semantics;
using System.Reflection;
using System.Reflection.Emit;

namespace Tiger
{
    class Program
    {
        enum ErrorCodes { NoError, WrongParameters, FileError, SyntaxError, SemanticError, CodeGenerationError, UnexpectedError }
        
        static void Main(string[] args)
        {
            Console.WriteLine("Tiger Compiler");
            Console.WriteLine("2016-2017 Amelia Rabanillo Echaniz & Aryan Curiel Pardo");
            Console.WriteLine();
            
            if (args.Length != 1)
            {
                Console.WriteLine("There is no file path to process");
                Environment.ExitCode = (int)ErrorCodes.WrongParameters;
                return;
            }
            string path = args[0];


            if (!File.Exists(path))
            {
                Console.WriteLine("File {0} does not exists", args[0]);
                Environment.ExitCode = (int)ErrorCodes.FileError;
                return;
            }

            if (Path.GetExtension(path) != ".tig")
            {
                Console.WriteLine("File extension must be .tig");
                Environment.ExitCode = (int)ErrorCodes.FileError;
                return;
            }

            try
            {
                ProcessFile(path, Path.ChangeExtension(path, "exe"));
            }
            catch (Exception exc)
            {
                Console.Error.WriteLine("Unexpected error: {0}", exc.Message);
                Environment.ExitCode = 1;
            }

        }

        static void ProcessFile(string inputPath, string outputPath)
        {
            Console.WriteLine("Building: {0}", Path.GetFullPath(inputPath));
            ProgramNode root = ParseInput(inputPath);

            if (root == null)
            {
                Environment.ExitCode = 1;
                return;
            }

            try
            {
                if (!CheckSemantics(root))
                {
                    Environment.ExitCode = 1;
                    return;
                }
            }
            catch
            {
                Console.WriteLine("Syntax ERROR");
                Environment.ExitCode = 1;
                return;
            }
            Console.WriteLine("No Semantic error found");


            GenerateCode(root, outputPath);
            Console.WriteLine("Success");
            Environment.ExitCode = (int)ErrorCodes.NoError;
        }

        static ProgramNode ParseInput(string inputPath)
        {
            try
            {
                ANTLRFileStream input = new ANTLRFileStream(inputPath);
                tigerLexer lexer = new tigerLexer(input);
                CommonTokenStream tokens = new CommonTokenStream(lexer);
                tigerParser parser = new tigerParser(tokens);

                parser.TreeAdaptor = new Adaptor();

                return (ProgramNode)parser.program().Tree;

            }
            catch (ParsingException exc)
            {
                Console.WriteLine("Error parsing input file: {0} [line:{1}, column:{2}]", exc.Message, exc.RecognitionError.Line, exc.RecognitionError.CharPositionInLine);
                Environment.ExitCode = 1;
                return null;
            }
        }

        static bool CheckSemantics(ProgramNode root)
        {
            StandarScope standarScope = new StandarScope();
            Scope scope = new Scope(standarScope, ScopeType.Main);
            Report report = new Report();

            root.CheckSemantics(scope, report);

            Console.WriteLine("No Syntax error found");

            if (report.errors.Count == 0)
                return true;
            
            foreach (var error in report.errors)
                Console.WriteLine("Semantic Error: {0} [line: {1}, column: {2}]", error.message, error.line, error.column);

            return false;
        }

        static void GenerateCode(ProgramNode root, string outputPath)
        {
            string name = Path.GetFileNameWithoutExtension(outputPath);
            string filename = Path.GetFileName(outputPath);

            AssemblyName assemblyName = new AssemblyName(name);
            AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave, Path.GetDirectoryName(Path.GetFullPath(outputPath)));

            ModuleBuilder moduleBuilder = assembly.DefineDynamicModule(name, filename);
            TypeBuilder programType = moduleBuilder.DefineType("Program", TypeAttributes.Public);
            MethodBuilder mainMethod = programType.DefineMethod("Main", MethodAttributes.Static|MethodAttributes.Public, typeof(void), System.Type.EmptyTypes);
            assembly.SetEntryPoint(mainMethod);
            ILGenerator generator = mainMethod.GetILGenerator();

            generator.BeginExceptionBlock();

            CodeGenerator cg = new CodeGenerator(generator, moduleBuilder, programType);

            root.Generate(cg);

            generator.BeginCatchBlock(typeof(Exception));
            generator.Emit(OpCodes.Pop);
            generator.Emit(OpCodes.Ldc_I4_1);
            generator.Emit(OpCodes.Call, typeof(Environment).GetMethod("Exit"));
            generator.EndExceptionBlock();
            generator.Emit(OpCodes.Ret);
            
            programType.CreateType();
            moduleBuilder.CreateGlobalFunctions();
            assembly.Save(filename);

        }
    }
}
