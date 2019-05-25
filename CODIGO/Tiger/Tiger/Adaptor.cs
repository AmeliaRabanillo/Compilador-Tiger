
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using System;
using Tiger.AST;



namespace Tiger
{
    class Adaptor : CommonTreeAdaptor
    {
        public Adaptor() : base() { }


        public override object Create(IToken fromToken)
        {
            if (fromToken == null)
                return new  EmptyNode(fromToken);


            switch (fromToken.Type)
            {
                #region Nodes
                case tigerParser.FUNCTION_CALL_NODE:
                    return new FuncCallNode(fromToken);
                case tigerParser.WHILE_NODE:
                    return new WhileNode(fromToken);
                case tigerParser.IF_THEN_ELSE_NODE:
                    return new IfThenElseNode(fromToken);
                case tigerParser.IF_THEN_NODE:
                    return new IfThenNode(fromToken);
                case tigerParser.NEGATIVE_NODE:
                    return new NegNode(fromToken);
                case tigerParser.PROGRAM_NODE:
                    return new ProgramNode(fromToken);
                case tigerParser.EXPR_SEQ_NODE:
                    return new ExprSeqNode(fromToken);
                case tigerParser.EXPR_LIST_NODE:
                    return new ExprListNode(fromToken);
                case tigerParser.FOR_NODE:
                    return new ForNode(fromToken);
                case tigerParser.ACCESS_LVALUE:
                    return new Access_Lvalue(fromToken);
                case tigerParser.BREAK:
                    return new BreakNode(fromToken);
                case tigerParser.EXPR_SEQ_LET_NODE:
                    return new ExprSeqNode(fromToken);
                case tigerParser.VAR_DEC_NODE:
                    return new SimpleVariableDecNode(fromToken);
                case tigerParser.VAR_TYPE_ID_DEC_NODE:
                    return new TypeIdVariableDecNode(fromToken);
                case tigerParser.DECLARATION_LIST_NODE:
                    return new DeclarationListNode(fromToken);
                case tigerParser.LET_NODE:
                    return new LetNode(fromToken);
                case tigerParser.LVALUE_STEPS:
                    return new LvalueStepsNode(fromToken);
                case tigerParser.ASSIGN:
                    return new AssignLvalueNode(fromToken);
                case tigerParser.ALIAS_DECL_NODE:
                    return new AliasDecNode(fromToken);
                case tigerParser.VARIABLE_DECLARATION_BLOCK_NODE:
                    return new VariableDecBlockNode(fromToken);
                case tigerParser.TYPE_DECLARATION_BLOCK_NODE:
                    return new TypeDecBlockNode(fromToken);
                case tigerParser.ARRAY_OF_DECL_NODE:
                    return new ArrayDecNode(fromToken);
                case tigerParser.ARRAY_CREATION_NODE:
                    return new ArrayCreationNode(fromToken);
                case tigerParser.INDEXER:
                    return new IndexerNode(fromToken);

                #region records
                case tigerParser.RECORD_DECL_NODE:
                    return new RecordDecNode(fromToken);
                case tigerParser.FIELD_DEF_NODE://Para la declaracion de recods
                    return new FieldDecNode(fromToken);
                case tigerParser.FIELD_DEF_SEQ_NODE://Para la declaracion de recods
                    return new FieldsDecNode(fromToken);

                case tigerParser.RECORD_CREATION_NODE:
                    return new RecordCreationNode(fromToken);
                case tigerParser.FIELD_LIST_NODE://{a=b,c=d...}
                    return new FieldListNode(fromToken);
                case tigerParser.FIELD_NODE://a=b dentrp de un field_list_node
                    return new FieldNode(fromToken);
                #endregion

                case tigerParser.FUNCTION_DECLARATION_BLOCK_NODE:
                    return new FunctionDecBlockNode(fromToken);
                case tigerParser.FUNCTION_DEC_NODE:
                    return new MethodDecNode(fromToken                        );
                case tigerParser.PROCEDURE_DEC_NODE:
                    return new ProcedureDecNode(fromToken);

                #endregion

                #region binary
                case tigerParser.OR:
                    return new OrNode(fromToken);
                case tigerParser.AND:
                    return new AndNode(fromToken);
                case tigerParser.PLUS:
                    return new PlusNode(fromToken);
                case tigerParser.MINUS:
                    return new MinusNode(fromToken);
                case tigerParser.MULT:
                    return new StarNode(fromToken);
                case tigerParser.DIV:
                    return new DivNode(fromToken);
                case tigerParser.LESS_THAN:
                    return new LTNode(fromToken);
                case tigerParser.LESS_EQUAL_THAN:
                    return new LTENode(fromToken);
                case tigerParser.GREATER_THAN:
                    return new GTNode(fromToken);
                case tigerParser.GREATER_EQUAL_THAN:
                    return new GTENode(fromToken);
                case tigerParser.DIFFERENT:
                    return new DifferentNode(fromToken);
                case tigerParser.EQUAL:
                    return new EqualNode(fromToken);

                #endregion

                #region Others
                case tigerParser.ID:
                    return new NameNode(fromToken);
                case tigerParser.INT:
                    return new IntNode(fromToken);
                case tigerParser.STRING:
                    return new StringNode(fromToken);
                case tigerParser.NIL:
                    return new NilNode(fromToken);
                case tigerParser.DOT:
                    return new DotNode(fromToken);
                #endregion

                default:
                    //throw new InvalidOperationException();
                    return new UnknowNode(fromToken);

            }
        }
    }
}
