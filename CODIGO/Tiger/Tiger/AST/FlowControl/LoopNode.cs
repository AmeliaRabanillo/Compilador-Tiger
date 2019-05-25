using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiger.Semantics;
using Antlr.Runtime;
using System.Reflection.Emit;

namespace Tiger.AST
{
    public abstract class LoopNode : FlowNode, IBreak
    {
        public LoopNode() : base() { }
        public LoopNode(IToken t) : base(t) { }
        public LoopNode(LoopNode n) : base(n) { }

        public Label loopEnd { get; set; }
    }

    public interface IBreak
    {
        Label loopEnd { get; set; }
    }
}
