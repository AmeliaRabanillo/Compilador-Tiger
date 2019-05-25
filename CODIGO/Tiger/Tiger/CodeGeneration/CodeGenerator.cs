using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;

namespace Tiger
{
    public class CodeGenerator
    {
        public ILGenerator generator;
        public ModuleBuilder moduleBuilder;
        public TypeBuilder typeBuilder;

        public CodeGenerator(ILGenerator g, ModuleBuilder mb, TypeBuilder tb)
        {
            generator = g;
            moduleBuilder = mb;
            typeBuilder = tb;
        }
    }
}
