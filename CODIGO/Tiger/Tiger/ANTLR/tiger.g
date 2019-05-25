grammar tiger;

options{
	language = CSharp3;
	output = AST;
	k=2;
}

tokens
{
	/*Keywords*/
	ARRAY='array';
	IF='if';
	THEN='then';
	ELSE='else';
	WHILE='while';
	FOR='for';
	TO='to';
	DO='do';
	LET='let';
	IN='in';
	END='end';
	OF='of';
	BREAK='break';
	NIL='nil';
	FUNCTION='function';
	VAR='var';
	TYPE='type';
	
	/*Symbols*/
	COMMA=',';
	COLON=':';
	SEMICOLON=';';
	LEFT_PARENTHESIS='(';
	RIGHT_PARENTHESIS=')';
	LEFT_BRACKETS='[';
	RIGHT_BRACKETS=']';
	LEFT_CURLY_BRACKETS='{';
	RIGHT_CURLY_BRACKETS='}';
	DOT='.';
	PLUS='+';
	MINUS='-';
	MULT='*';
	DIV='/';
	EQUAL='=';
	DIFFERENT='<>';
	LESS_THAN='<';
	LESS_EQUAL_THAN='<=';
	GREATER_THAN='>';
	GREATER_EQUAL_THAN='>=';
	AND='&';
	OR='|';
	ASSIGN=':=';
	QUOTES='"';
	TEST = ']of';
	
	//Nodes
	//Los operadores binarios se crean nodos con los tokens ya definidos anteriormente
	PROGRAM_NODE;	
	WHILE_NODE;	
	FOR_NODE;	
	//BREAK_NODE;	
	IF_THEN_NODE;	
	IF_THEN_ELSE_NODE;	
	LET_NODE;	
	EXPR_SEQ_LET_NODE;//let ... in expr_seq
	NEGATIVE_NODE;	
	INDEXER;//para el indexado de los lvalues
	//para los .id de los lvalue se usa el DOT ya predefinido
	
	EXPR_SEQ_NODE;//(expr;...)
	EXPR_LIST_NODE;//expr, ...
	FIELD_LIST_NODE;//id = expr, ...	
	FIELD_NODE;//id=expr; es cada uno de los fields de la lista de arriba
	
	DECLARATION_LIST_NODE;
	TYPE_DECLARATION_BLOCK_NODE;		//
	VARIABLE_DECLARATION_BLOCK_NODE;		//declaration_list (para las dec del let)
	FUNCTION_DECLARATION_BLOCK_NODE;		//
	
	//para todo lo de los types
	FIELD_DEF_SEQ_NODE;//type_field, ...
	FIELD_DEF_NODE;//id:type_id
	ALIAS_DECL_NODE;//type type_id = type_id
	RECORD_DECL_NODE;//type type_id = {type_field}
	ARRAY_OF_DECL_NODE;//type type_id = array of type_id
	
	
	//
	VAR_DEC_NODE;//var id := expr	
	VAR_TYPE_ID_DEC_NODE;//var id type_id := expr	
	
	FUNCTION_DEC_NODE;//function id (type_fields) = expr		
	PROCEDURE_DEC_NODE;//function id (type_fields): type_id = expr	
	FUNCTION_CALL_NODE;//if(expr_list)
	
	RECORD_CREATION_NODE;//para el que sale directamente de una expresion
	ARRAY_CREATION_NODE;//para el que sale directamente de una expresion
	
	//para el acceso a los lvalue TODO faltan mas
	ACCESS_LVALUE;
	LVALUE_STEPS;
}

@lexer::header{using System;}
@lexer::namespace{Tiger}
@lexer::ctorModifier{public}
@lexer::members{
	public override void ReportError(RecognitionException exc)
	{
		/* Abort on first error. */
      		throw new ParsingException(GetErrorMessage(exc, TokenNames), exc);
	}
}


@parser::header{using System;}
@parser::namespace{Tiger}
@parser::ctorModifier{public}
@parser::member{
	public override void ReportError(RecognitionException exc) 
    	{ 
        	/* Abort on first error. */
       		throw new ParsingException(GetErrorMessage(exc, TokenNames), exc);
    	} 
}


fragment 
DIGIT	:	'0'..'9';

fragment
ASCII_CODE
	:'0' DIGIT DIGIT
	|'1'(('0'|'1')DIGIT|'2' '0'..'7');	

fragment
SCAPE	:'\\'('n'|'t'|'"'|'\\'|ASCII_CODE|WS+'\\');

fragment
LETTER		:	'a'..'z'|'A'..'Z';

fragment
ASCII_CHAR	:	' '|'!'|'#'..'['|']'..'~';

fragment
WS		:	(' '|'\t'|'\r'|'\n');

//COMMENT		:	'/*'.*(COMMENT.*)*'*/'{$channel=Hidden;};
COMMENT:	'/*' (options {greedy=false; } : COMMENT | .)* '*/' {$channel=Hidden;};

ID		:	(LETTER)(DIGIT|LETTER|'_')*;

INT		:	(DIGIT)+;

STRING		:	QUOTES(SCAPE|ASCII_CHAR)*QUOTES;



WHITE_SPACES 	:	WS+ {$channel=Hidden;};



public program	:	prog=statement EOF -> ^(PROGRAM_NODE $prog);


comparision_op
	:	LESS_THAN
	|	LESS_EQUAL_THAN
	|	GREATER_THAN
	|	GREATER_EQUAL_THAN
	|	DIFFERENT
	|	EQUAL;
	

statement
	:	(left=disjunction->$left)(OR right=disjunction-> ^(OR $statement $right))*;

disjunction
	:	(left=comparision_expr->$left)(AND right=comparision_expr->^(AND $disjunction $right))*;

comparision_expr
	:	(left=expr -> $left )
		(
			(
				(
					LESS_THAN right=expr -> ^(LESS_THAN $left $right)
					|LESS_EQUAL_THAN right=expr -> ^(LESS_EQUAL_THAN $left $right)
					|GREATER_THAN right=expr -> ^(GREATER_THAN $left $right)
					|GREATER_EQUAL_THAN right=expr -> ^(GREATER_EQUAL_THAN $left $right)
					|DIFFERENT right=expr -> ^(DIFFERENT $left $right)
					|EQUAL right=expr -> ^(EQUAL $left $right)
				)
				|(-> $left)
			)
		);
	
expr    :(left=term->$left)
          ( (PLUS right1=term -> ^(PLUS $expr $right1))
          | (MINUS right2=term -> ^(MINUS $expr $right2))
          )* 
        ;

term    : (left=factor -> $left)
          ( MULT right=factor -> ^(MULT $term $right)
          | DIV right=factor -> ^(DIV $term $right)
          )* 
        ;
factor	:	atom
	|	MINUS (atom-> ^(NEGATIVE_NODE atom)|factor-> ^(NEGATIVE_NODE factor));
		
        
atom
	:	INT
	|	STRING
	|	NIL
	|	function_call
	|	flow_control
	|	BREAK
	|	let_
	|	parenthesis_expr_seq
	|	lvalue;
	//|	lvalue_assign;

	//|	atom_type_id;
	
parenthesis_expr_seq
	:	LEFT_PARENTHESIS (expr_seq)? RIGHT_PARENTHESIS -> ^(EXPR_SEQ_NODE (expr_seq)?);

if_	
	:	(IF cond=statement THEN block1=statement)
			(ELSE block2=statement-> ^(IF_THEN_ELSE_NODE $cond $block1 $block2)
			|-> ^(IF_THEN_NODE $cond $block1));

 
function_call
	:	func_id=ID LEFT_PARENTHESIS (expr_list)? RIGHT_PARENTHESIS -> ^(FUNCTION_CALL_NODE $func_id  ^(EXPR_LIST_NODE expr_list?));

expr_seq	
	:	statement(SEMICOLON statement)*->statement+;
	
expr_list
	:	statement(COMMA statement)*->statement+;
	
field_list
	:	(field)(COMMA field)* ->^(FIELD_LIST_NODE field+);
field
	:	ID EQUAL statement-> ^(FIELD_NODE ID statement);
	
	
lvalue	:id=ID (
		//(LEFT_BRACKETS expr1=statement RIGHT_BRACKETS OF expr2=statement -> ^(ARRAY_CREATION_NODE $id $expr1 $expr2))//para crear array, NO para declararlo
		
		(
			LEFT_BRACKETS expr1=statement RIGHT_BRACKETS
				(
					OF expr2=statement -> ^(ARRAY_CREATION_NODE $id $expr1 $expr2)//para crear array, NO para declararlo
					|steps?(	
						(ASSIGN sta=statement -> ^(ASSIGN ^(ACCESS_LVALUE $id ^(LVALUE_STEPS ^(INDEXER $expr1) steps?)) $sta))
						|-> ^(ACCESS_LVALUE $id ^(LVALUE_STEPS ^(INDEXER $expr1) steps?))
				
				)		
			)
			|steps?(	
						(ASSIGN sta=statement -> ^(ASSIGN ^(ACCESS_LVALUE $id ^(LVALUE_STEPS steps?)) $sta))
						|-> ^(ACCESS_LVALUE $id ^(LVALUE_STEPS steps?))
				)
		)	
		|(LEFT_CURLY_BRACKETS fields=field_list? RIGHT_CURLY_BRACKETS -> ^(RECORD_CREATION_NODE $id  $fields?))//para crear records		 				
		);	


steps	:(indexer -> indexer|lvalue_dot -> lvalue_dot)(lvalue_dot -> $steps lvalue_dot|indexer-> $steps indexer)*;
	
lvalue_dot:DOT ID->^(DOT ID);	
indexer	:LEFT_BRACKETS statement RIGHT_BRACKETS -> ^(INDEXER statement);

//lvalue_assign
//	:	s=lvalue ASSIGN sta=statement -> ^(ASSIGN $s $sta);

//atom_type_id
//	:	id=type_id
//		(
//			LEFT_BRACKETS expr1=statement RIGHT_BRACKETS OF expr2=statement -> ^(ARRAY_CREATION_NODE $id $expr1 $expr2)
//			|LEFT_CURLY_BRACKETS fields=field_list? RIGHT_CURLY_BRACKETS -> ^(RECORD_CREATION_NODE $id $fields)
//		);

flow_control
	:	if_
	|	while_
	|	for_;

for_	
	:	FOR id=ID ASSIGN from=statement TO to=statement DO block=statement -> ^(FOR_NODE $id $from $to $block);


while_	
	:	WHILE cond=statement DO block=statement -> ^(WHILE_NODE $cond $block);


let_	:	LET dec_list=declaration_list IN (expr_seq)? END -> ^(LET_NODE $dec_list ^(EXPR_SEQ_NODE expr_seq?));//^(LET_NODE[$LET] ^(EXPR_SEQ_LET_NODE $dec_list) expr_seq?);//para que es el elt entre []?


declaration_list	
	:	declaration+ -> ^(DECLARATION_LIST_NODE declaration+);//que significa el + del nodo


declaration
	:	(type_declaration)+ -> ^(TYPE_DECLARATION_BLOCK_NODE type_declaration+) 
	|	(variable_declaration)+ -> ^(VARIABLE_DECLARATION_BLOCK_NODE variable_declaration+)//^(VARIABLE_DECLARATION_LIST_NODE variable_declaration+)
	|	(function_declaration)+ -> ^(FUNCTION_DECLARATION_BLOCK_NODE function_declaration+);


type_declaration
	:	(TYPE id1=type_id EQUAL)
	(
		id2=ID -> ^(ALIAS_DECL_NODE $id1 $id2)//alias
	|	LEFT_CURLY_BRACKETS fields=type_fields? RIGHT_CURLY_BRACKETS->^(RECORD_DECL_NODE $id1 ^(FIELD_DEF_SEQ_NODE $fields?))//record
	|	ARRAY OF elem_id=type_id -> ^(ARRAY_OF_DECL_NODE $id1 $elem_id)//array of
	);


type_fields
	:	type_field(COMMA type_field)* -> type_field*;//como funciona esto OJO
	
	
type_field
	:	 id=ID COLON type=type_id -> ^(FIELD_DEF_NODE $id $type);


type_id
	:	ID;//ver si el type_id este es un ID normal 
	

variable_declaration
	: 	VAR id=ID (
	|	(ASSIGN sta=statement ->^(VAR_DEC_NODE $id $sta))
	|	(COLON type=type_id ASSIGN sta=statement->^(VAR_TYPE_ID_DEC_NODE $id $sta $type)));


function_declaration
	:	FUNCTION f_id=ID LEFT_PARENTHESIS (type_fields)? RIGHT_PARENTHESIS
		(COLON type=type_id EQUAL sta=statement -> ^(FUNCTION_DEC_NODE $f_id $type $sta ^(FIELD_DEF_SEQ_NODE type_fields?))
		| EQUAL sta=statement -> ^(PROCEDURE_DEC_NODE $f_id $sta ^(FIELD_DEF_SEQ_NODE type_fields?)));








