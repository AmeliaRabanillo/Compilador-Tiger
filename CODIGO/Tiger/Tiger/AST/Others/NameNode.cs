using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;
using Tiger.Semantics;

namespace Tiger.AST
{
    public class NameNode:OthersNode
    {
        public string name { get { return Text; } }

        public NameNode() : base() { }
        public NameNode(IToken t) : base(t) { }
        public NameNode(NameNode n) : base(n) { }
        
        public override void CheckSemantics(Scope scope, Report report)
        {
            List<string> reservedWords =new List<string>() { "array", "break", "do", "else", "end", "for", "function", "if", "in", "let", "nil", "of", "then", "to", "type", "var", "while"};
            
            if (reservedWords.Contains(name))
            {
                report.Add(Line, CharPositionInLine, string.Format("Name {0} is invalid because is a reserved word", name));
                returnType = scope.FindType("error");
            }

            returnType = new TypeInfo("Ok", Types.Ok);
        }

        public override void Generate(CodeGenerator cg)
        {
        }
    }
}
