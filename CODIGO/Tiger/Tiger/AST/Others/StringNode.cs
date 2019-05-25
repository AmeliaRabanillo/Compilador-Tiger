using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Tiger.Semantics;
using Antlr.Runtime;

namespace Tiger.AST
{
    class StringNode : OthersNode
    {
        public StringNode() : base() { }

        public StringNode(IToken t) : base(t) { }

        public StringNode(StringNode n) : base(n) { }

        public string value { get; private set; }

        public override void CheckSemantics(Scope scope, Report report)
        {

            returnType = scope.FindType("string");
            StringBuilder builder = new StringBuilder();

            for (int i = 1; i < Text.Length-1; i++)
            {
                if (Text[i] == '\\')
                {
                    switch (Text[i+1])
                    {
                        case 'n':
                            builder.Append('\n');
                            i++;
                            break;
                        case 't':
                            builder.Append('\t');
                            i++;
                            break;
                        case '"':
                            builder.Append('\"');
                            i++;
                            break;
                        case '\\':
                            builder.Append('\\');
                            i++;
                            break;
                        case 'r':
                            builder.Append('\r');
                            i++;
                            break;
                        default:
                            //todo ver si pongo >= y <= o no
                            if (Text[i + 1] >= 48 && Text[i + 1] <= 57)
                            {
                                builder.Append((char)(int.Parse(Text.Substring(i + 1, 3))));
                                i += 3;
                            }
                            else
                                i = Text.IndexOf('\\', i + 2);
                            break;

                            //TODO falta case '^c' que no se que hacer con el                                                
                    }
                    
                }
                else
                    builder.Append(Text[i]);
            }

            value = builder.ToString();
        }

        public override void Generate(CodeGenerator cg)
        {
            cg.generator.Emit(OpCodes.Ldstr, value);
        }
    }
}
