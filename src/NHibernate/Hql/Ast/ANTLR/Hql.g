grammar Hql;

options
{
	language=CSharp3;
	output=AST;
	ASTLabelType=IASTNode;
}

tokens
{
	// -- HQL Keyword tokens --
	ALL='all';
	ANY='any';
	AND='and';
	AS='as';
	ASCENDING='asc';
	AVG='avg';
	BETWEEN='between';
	CLASS='class';
	COUNT='count';
	DELETE='delete';
	DESCENDING='desc';
	DOT;
	DISTINCT='distinct';
	ELEMENTS='elements';
	ESCAPE='escape';
	EXISTS='exists';
	FALSE='false';
	FETCH='fetch';
	FROM='from';
	FULL='full';
	GROUP='group';
	HAVING='having';
	IN='in';
	INDICES='indices';
	INNER='inner';
	INSERT='insert';
	INTO='into';
	IS='is';
	JOIN='join';
	LEFT='left';
	LIKE='like';
	MAX='max';
	MIN='min';
	NEW='new';
	NOT='not';
	NULL='null';
	OR='or';
	ORDER='order';
	OUTER='outer';
	PROPERTIES='properties';
	RIGHT='right';
	SELECT='select';
	SET='set';
	SKIP='skip';
	SOME='some';
	SUM='sum';
	TAKE='take';
	TRUE='true';
	UNION='union';
	UPDATE='update';
	VERSIONED='versioned';
	WHERE='where';
	LITERAL_by='by';

	// -- SQL tokens --
	// These aren't part of HQL, but the SQL fragment parser uses the HQL lexer, so they need to be declared here.
	CASE='case';
	END='end';
	ELSE='else';
	THEN='then';
	WHEN='when';
	ON='on';
	WITH='with';

	// -- EJBQL tokens --
	BOTH='both';
	EMPTY='empty';
	LEADING='leading';
	MEMBER='member';
	OBJECT='object';
	OF='of';
	TRAILING='trailing';

	// -- Synthetic token types --
	AGGREGATE;		// One of the aggregate functions (e.g. min, max, avg)
	ALIAS;
	CONSTRUCTOR;
	CASE2;
	EXPR_LIST;
	FILTER_ENTITY;		// FROM element injected because of a filter expression (happens during compilation phase 2)
	IN_LIST;
	INDEX_OP;
	IS_NOT_NULL;
	IS_NULL;			// Unary 'is null' operator.
	METHOD_CALL;
	NOT_BETWEEN;
	NOT_IN;
	NOT_LIKE;
	ORDER_ELEMENT;
	QUERY;
	RANGE;
	ROW_STAR;
	SELECT_FROM;
	UNARY_MINUS;
	UNARY_PLUS;
	VECTOR_EXPR;		// ( x, y, z )
	WEIRD_IDENT;		// Identifiers that were keywords when they came in.

	// Literal tokens.
	CONSTANT;
	NUM_INT;
	NUM_DOUBLE;
	NUM_DECIMAL;
	NUM_FLOAT;
	NUM_LONG;
	JAVA_CONSTANT;
}

@parser::namespace { NHibernate.Hql.Ast.ANTLR }
@lexer::namespace { NHibernate.Hql.Ast.ANTLR }

@header
{
using NHibernate.Hql.Ast.ANTLR.Tree;
}

public statement
	: ( updateStatement | deleteStatement | selectStatement | insertStatement ) EOF!
	;

updateStatement
	: UPDATE^ (VERSIONED)?
		optionalFromTokenFromClause
		setClause
		(whereClause)?
	;

setClause
	: (SET^ assignment (COMMA! assignment)*)
	;

assignment
	: stateField EQ^ newValue
	;

// "state_field" is the term used in the EJB3 sample grammar; used here for easy reference.
// it is basically a property ref
stateField
	: path
	;

// this still needs to be defined in the ejb3 spec; additiveExpression is currently just a best guess,
// although it is highly likely I would think that the spec may limit this even more tightly.
newValue
	: concatenation
	;

deleteStatement
	: DELETE^
		(optionalFromTokenFromClause)
		(whereClause)?
	;

// Note the use of optionalFromTokenFromClause2 - without using this subrule,
// the tree-rewrite rule does not work if the (optional) FROM token is missing
optionalFromTokenFromClause
	: optionalFromTokenFromClause2 path (asAlias)? 
		-> ^(FROM ^(RANGE path asAlias?))
	;

optionalFromTokenFromClause2
	: FROM?
	;

selectStatement
	: q=queryRule 
	-> ^(QUERY["query"] $q)
	;

insertStatement
	// Would be nice if we could abstract the FromClause/FromElement logic
	// out such that it could be reused here; something analogous to
	// a "table" rule in sql-grammars
	: INSERT^ intoClause selectStatement
	;

intoClause
	: INTO^ path { WeakKeywords(); } insertablePropertySpec
	;

insertablePropertySpec
	: OPEN primaryExpression ( COMMA primaryExpression )* CLOSE
		-> ^(RANGE["column-spec"] primaryExpression*)
	;

//## query:
//##     [selectClause] fromClause [whereClause] [groupByClause] [havingClause] [orderByClause] [skipClause] [takeClause];

queryRule
	: selectFrom
		(whereClause)?
		(groupByClause)?
		(havingClause)?
		(orderByClause)?
		(skipClause)?
		(takeClause)?
		;

selectFrom
	:  (s=selectClause)? (f=fromClause)? 
		{
			if ($f.tree == null && !filter) 
				throw new RecognitionException("FROM expected (non-filter queries must contain a FROM clause)");
		}
		-> {$f.tree == null && filter}? ^(SELECT_FROM FROM["{filter-implied FROM}"] selectClause?)
		-> ^(SELECT_FROM fromClause? selectClause?)
	;


selectClause
	: SELECT^	// NOTE: The '^' after a token causes the corresponding AST node to be the root of the sub-tree.
		{ WeakKeywords(); }	// Weak keywords can appear immediately after a SELECT token.
		(DISTINCT)? ( selectedPropertiesList | newExpression | selectObject )
	;

newExpression
	: (NEW path) op=OPEN selectedPropertiesList CLOSE
		-> ^(CONSTRUCTOR[$op] path selectedPropertiesList)
	;

selectObject
   : OBJECT^ OPEN! identifier CLOSE!
   ;

// NOTE: This *must* begin with the "FROM" token, otherwise the sub-query rule will be ambiguous
// with the expression rule.
// Also note: after a comma weak keywords are allowed and should be treated as identifiers.

fromClause
	: FROM^ { WeakKeywords(); } fromRange ( fromJoin | COMMA! { WeakKeywords(); } fromRange )*
	;

fromJoin
	: ( ( ( LEFT | RIGHT ) (OUTER)? ) | FULL | INNER )? JOIN^ (FETCH)? path (asAlias)? (propertyFetch)? (withClause)?
	| ( ( ( LEFT | RIGHT ) (OUTER)? ) | FULL | INNER )? JOIN^ (FETCH)? ELEMENTS! OPEN! path CLOSE! (asAlias)? (propertyFetch)? (withClause)?
	;

withClause
	: WITH^ logicalExpression
	;

fromRange
	: fromClassOrOuterQueryPath
	| inClassDeclaration
	| inCollectionDeclaration
	| inCollectionElementsDeclaration
	; 

fromClassOrOuterQueryPath
	: path { WeakKeywords(); } (asAlias)? (propertyFetch)? 
		-> ^(RANGE path asAlias? propertyFetch?)
	;

inClassDeclaration
	: alias IN CLASS? path 
		-> ^(RANGE path alias)
	;

inCollectionDeclaration!
    : IN OPEN path CLOSE alias 
    	-> ^(JOIN["join"] INNER["inner"] path alias)
    ;

inCollectionElementsDeclaration
	: alias IN ELEMENTS OPEN path CLOSE 
		-> ^(JOIN["join"] INNER["inner"] path alias)
	| ELEMENTS OPEN path CLOSE AS alias
		-> ^(JOIN["join"] INNER["inner"] path alias)
    ;

// Alias rule - Parses the optional 'as' token and forces an AST identifier node.
asAlias
	: (AS!)? alias
	;
alias
	: i=identifier
	-> ^(ALIAS[$i.start])
	;

propertyFetch
	: FETCH ALL! PROPERTIES!
	;

groupByClause
	: GROUP^ 
		'by'! expression ( COMMA! expression )*
	;

orderByClause
	: ORDER^ 'by'! orderElement ( COMMA! orderElement )*
	;

skipClause
	: SKIP^ (NUM_INT | parameter)
	;

takeClause
	: TAKE^ (NUM_INT | parameter)
	;

parameter
	: COLON^ identifier
	| PARAM^ (NUM_INT)?
	;

orderElement
	: expression ( ascendingOrDescending )?
	;

ascendingOrDescending
	: ( a='asc' | a='ascending' )
		-> ^(ASCENDING[$a.Text])
	| ( d='desc' | d='descending')
		-> ^(DESCENDING[$d.Text])
	;

havingClause
	: HAVING^ logicalExpression
	;

whereClause
	: WHERE^ logicalExpression
	;

selectedPropertiesList
	: aliasedExpression ( COMMA! aliasedExpression )*
	;
	
aliasedExpression
	: expression ( AS^ identifier )?
	;

// expressions
// Note that most of these expressions follow the pattern
//   thisLevelExpression :
//       nextHigherPrecedenceExpression
//           (OPERATOR nextHigherPrecedenceExpression)*
// which is a standard recursive definition for a parsing an expression.
//
// Operator precedence in HQL
// lowest  --> ( 8)  OR
//             ( 7)  AND, NOT
//             ( 6)  equality: ==, <>, !=, is
//             ( 5)  relational: <, <=, >, >=,
//                   LIKE, NOT LIKE, BETWEEN, NOT BETWEEN, IN, NOT IN
//             ( 4)  bitwise: |,  &	 
//             ( 3)  addition and subtraction: +(binary) -(binary)
//             ( 2)  multiplication: * / %, concatenate: ||
// highest --> ( 1)  +(unary) -(unary)
//                   []   () (method call)  . (dot -- identifier qualification)
//                   aggregate function
//                   ()  (explicit parenthesis)
//
// Note that the above precedence levels map to the rules below...
// Once you have a precedence chart, writing the appropriate rules as below
// is usually very straightfoward

logicalExpression
	: expression
	;

// Main expression rule
expression
	: logicalOrExpression
	;

// level 8 - OR
logicalOrExpression
	: logicalAndExpression ( OR^ logicalAndExpression )*
	;

// level 7 - AND, NOT
logicalAndExpression
	: negatedExpression ( AND^ negatedExpression )*
	;

// NOT nodes aren't generated.  Instead, the operator in the sub-tree will be
// negated, if possible.   Expressions without a NOT parent are passed through.
negatedExpression
@init{ WeakKeywords(); } // Weak keywords can appear in an expression, so look ahead.
	: NOT x=negatedExpression
		-> ^({NegateNode($x.tree)})
	| equalityExpression
	;

//## OP: EQ | LT | GT | LE | GE | NE | SQL_NE | LIKE;

// level 6 - EQ, NE
equalityExpression
		@after{
			// Post process the equality expression to clean up 'is null', etc.
			$equalityExpression.tree = ProcessEqualityExpression($equalityExpression.tree);
		}
	: x=relationalExpression (
		( EQ^
		| isx=IS^	{ $isx.Type = EQ; } (NOT! { $isx.Type =NE; } )?
		| NE^
		| ne=SQL_NE^	{ $ne.Type = NE; }
		) y=relationalExpression)*
	;

// level 5 - LT, GT, LE, GE, LIKE, NOT LIKE, BETWEEN, NOT BETWEEN
// NOTE: The NOT prefix for LIKE and BETWEEN will be represented in the
// token type.  When traversing the AST, use the token type, and not the
// token text to interpret the semantics of these nodes.
relationalExpression
	: concatenation (
		( ( ( LT^ | GT^ | LE^ | GE^ ) bitwiseNotExpression )* )
		// Disable node production for the optional 'not'.
		| (n=NOT!)? (
			// Represent the optional NOT prefix using the token type by
			// testing 'n' and setting the token type accordingly.
			(i=IN^ {
					$i.Type = (n == null) ? IN : NOT_IN;
					$i.Text = (n == null) ? "in" : "not in";
				}
				inList)
			| (b=BETWEEN^ {
					$b.Type = (n == null) ? BETWEEN : NOT_BETWEEN;
					$b.Text = (n == null) ? "between" : "not between";
				}
				betweenList )
			| (l=LIKE^ {
					$l.Type = (n == null) ? LIKE : NOT_LIKE;
					$l.Text = (n == null) ? "like" : "not like";
				}
				concatenation likeEscape)
			| (MEMBER! (OF!)? p=path! {
				root_0 = ProcessMemberOf($n,$p.tree, root_0);
			  } ) 
			)
		)
	;

likeEscape
	: (ESCAPE^ concatenation)?
	;

inList
	: compoundExpr
	-> ^(IN_LIST["inList"] compoundExpr)
	;

betweenList
	: concatenation AND! concatenation
	;

//level 5 - string concatenation
concatenation
@after {
   if (c != null)
   {
      IASTNode mc = (IASTNode) adaptor.Create(METHOD_CALL, "||");
      IASTNode concat = (IASTNode) adaptor.Create(IDENT, "concat");
      mc.AddChild(concat);
      mc.AddChild((IASTNode) retval.Tree);
      retval.Tree = mc;
   }
}
	: a=bitwiseNotExpression 
	( c=CONCAT^ { $c.Type = EXPR_LIST; $c.Text = "concatList"; } 
	  bitwiseNotExpression
	  ( CONCAT! bitwiseNotExpression )* 
	  )?
	;

// level 4 - bitwise
bitwiseNotExpression 
	: (BNOT^ bitwiseOrExpression)
	| bitwiseOrExpression
	;

bitwiseOrExpression 
	: bitwiseXOrExpression (BOR^ bitwiseXOrExpression)*
	;

bitwiseXOrExpression 
	: bitwiseAndExpression (BXOR^ bitwiseAndExpression)*
	;

bitwiseAndExpression 
	: additiveExpression (BAND^ additiveExpression)*
	;

// level 3 - binary plus and minus
additiveExpression
	: multiplyExpression ( ( PLUS^ | MINUS^ ) multiplyExpression )*
	;

// level 2 - binary multiply and divide
multiplyExpression
	: unaryExpression ( ( STAR^ | DIV^ ) unaryExpression )*
	;
	
// level 1 - unary minus, unary plus, not
unaryExpression
	: m=MINUS mu=unaryExpression -> ^(UNARY_MINUS[$m] $mu)
	| p=PLUS pu=unaryExpression -> ^(UNARY_PLUS[$p] $pu)
	| caseExpression
	| quantifiedExpression
	| atom
	;
	
caseExpression
	: CASE (whenClause)+ (elseClause)? END
		-> ^(CASE whenClause+ elseClause?) 
	| CASE unaryExpression (altWhenClause)+ (elseClause)? END
		-> ^(CASE2 unaryExpression altWhenClause+ elseClause?)
	;
	
whenClause
	: (WHEN^ logicalExpression THEN! expression)
	;
	
altWhenClause
	: (WHEN^ unaryExpression THEN! expression)
	;
	
elseClause
	: (ELSE^ expression)
	;
	
quantifiedExpression
	: ( SOME^ | EXISTS^ | ALL^ | ANY^ ) 
	( identifier | collectionExpr | (OPEN! ( subQuery ) CLOSE!) )
	;

// level 0 - expression atom
// ident qualifier ('.' ident ), array index ( [ expr ] ),
// method call ( '.' ident '(' exprList ') )
atom
	 : primaryExpression
		(
			DOT^ identifier
				( options { greedy=true; } :
					( op=OPEN^ {$op.Type = METHOD_CALL; } exprList CLOSE! ) )?
		|	lb=OPEN_BRACKET^ {$lb.Type = INDEX_OP; } expression CLOSE_BRACKET!
		)*
	;

// level 0 - the basic element of an expression
primaryExpression
	:   identPrimary ( options {greedy=true;} : DOT^ 'class' )?
	|   constant
	|   COLON^ identifier
	// TODO: Add parens to the tree so the user can control the operator evaluation order.
	|   OPEN! (expressionOrVector | subQuery) CLOSE!
	|   PARAM^ (NUM_INT)?
	;

// This parses normal expression and a list of expressions separated by commas.  If a comma is encountered
// a parent VECTOR_EXPR node will be created for the list.
expressionOrVector!
	: e=expression ( v=vectorExpr )? 
	-> {v != null}? ^(VECTOR_EXPR["{vector}"] $e $v)
	-> $e
	;

vectorExpr
	: COMMA! expression (COMMA! expression)*
	;

// identifier, followed by member refs (dot ident), or method calls.
// NOTE: handleDotIdent() is called immediately after the first IDENT is recognized because
// the method looks a head to find keywords after DOT and turns them into identifiers.
identPrimary
	: identifier { HandleDotIdent(); }
			( options {greedy=true;} : DOT^ ( identifier | o=OBJECT { $o.Type = IDENT; } ) )*
			( ( op=OPEN^ { $op.Type = METHOD_CALL;} exprList CLOSE! )
			)?
	// Also allow special 'aggregate functions' such as count(), avg(), etc.
	| aggregate
	;
	
//## aggregate:
//##     ( aggregateFunction OPEN path CLOSE ) | ( COUNT OPEN STAR CLOSE ) | ( COUNT OPEN (DISTINCT | ALL) path CLOSE );

//## aggregateFunction:
//##     COUNT | 'sum' | 'avg' | 'max' | 'min';
aggregate
	: ( op=SUM | op=AVG | op=MAX | op=MIN ) OPEN additiveExpression CLOSE
		-> ^(AGGREGATE[$op] additiveExpression)
	// Special case for count - It's 'parameters' can be keywords.
	|  COUNT OPEN ( s=STAR | p=aggregateDistinctAll ) CLOSE
		-> {s == null}? ^(COUNT $p)
		-> ^(COUNT ^(ROW_STAR["*"]))
	|  collectionExpr
	;

aggregateDistinctAll
	: ( ( DISTINCT | ALL )? ( path | collectionExpr ) )
	;
	
//## collection: ( OPEN query CLOSE ) | ( 'elements'|'indices' OPEN path CLOSE );

collectionExpr
	: (ELEMENTS^ | INDICES^) OPEN! path CLOSE!
	;
                                           
compoundExpr
	: collectionExpr
	| path
	| (OPEN! ( subQuery | (expression (COMMA! expression)*) ) CLOSE!)
	;

exprList
@after {
   IASTNode root = (IASTNode) adaptor.Create(EXPR_LIST, "exprList");
   root.AddChild((IASTNode)retval.Tree);
   retval.Tree = root;
}
	: (TRAILING {$TRAILING.Type = IDENT;}
	      | LEADING {$LEADING.Type = IDENT;}
	      | BOTH {$BOTH.Type = IDENT;}
	      )?
	  ( 
	  	expression ( (COMMA! expression)+ 
	  			| f=FROM expression {$f.Type = IDENT;}
	  			| AS! identifier )? 
	  	| f2=FROM expression {$f2.Type = IDENT;}
	  )?
	;
	
subQuery
	: innerSubQuery (UNION^ innerSubQuery)*
	;
	
innerSubQuery
	: queryRule
	-> ^(QUERY["query"] queryRule)
	;

constant
	: NUM_INT
	| NUM_FLOAT
	| NUM_LONG
	| NUM_DOUBLE
	| NUM_DECIMAL
	| QUOTED_String
	| NULL
	| TRUE
	| FALSE
	| EMPTY
	;

//## quantifiedExpression: 'exists' | ( expression 'in' ) | ( expression OP 'any' | 'some' ) collection;

//## compoundPath: path ( OPEN_BRACKET expression CLOSE_BRACKET ( '.' path )? )*;

//## path: identifier ( '.' identifier )*;

path
@init {
// TODO - need to clean up DotIdent - suspect that DotIdent2 supersedes the other one, but need to do the analysis
//HandleDotIdent2();
}
	: identifier ( DOT^ { WeakKeywords(); } identifier )*
	;

// Wraps the IDENT token from the lexer, in order to provide
// 'keyword as identifier' trickery.
identifier
	: IDENT
	;
	catch [RecognitionException ex]
	{
		retval.Tree = HandleIdentifierError(input.LT(1),ex);
	}
	
	

// **** LEXER ******************************************************************

// -- Keywords --

EQ: '=';
LT: '<';
GT: '>';
SQL_NE: '<>';
NE: '!=' | '^=';
LE: '<=';
GE: '>=';

BOR	:	 '|';
BXOR	:	'^';
BAND	:	'&';
BNOT	:	'!';

COMMA: ',';

OPEN: '(';
CLOSE: ')';
OPEN_BRACKET: '[';
CLOSE_BRACKET: ']';

CONCAT: '||';
PLUS: '+';
MINUS: '-';
STAR: '*';
DIV: '/';
COLON: ':';
PARAM: '?';

IDENT 
	: ID_START_LETTER ( ID_LETTER )*
	;

fragment
ID_START_LETTER
    :    '_'
    |    '$'
    |    'a'..'z'
    |    'A'..'Z'
    |    '\u0080'..'\ufffe'       // HHH-558 : Allow unicode chars in identifiers
    ;

fragment
ID_LETTER
    :    ID_START_LETTER
    |    '0'..'9'
    ;

QUOTED_String
	  : '\'' ( (ESCqs)=> ESCqs | ~'\'' )* '\''
	;

fragment
ESCqs
	:
		'\'' '\''
	;

WS  :   (   ' '
		|   '\t'
		|   '\r' '\n'
		|   '\n'
		|   '\r'
		)
		{Skip();} //ignore this token
	;

//--- From the Java example grammar ---
// a numeric literal
NUM_INT
	@init {bool isDecimal=false; IToken t=null;}
	:   '.' {_type = DOT;}
			(	('0'..'9')+ (EXPONENT)? (f1=FLOAT_SUFFIX {t=f1;})?
				{
					if (t != null && t.Text.ToUpperInvariant().IndexOf('F')>=0)
					{
						_type = NUM_FLOAT;
					}
					else if (t != null && t.Text.ToUpperInvariant().IndexOf('M')>=0)
					{
						_type = NUM_DECIMAL;
					}
					else
					{
						_type = NUM_DOUBLE; // assume double
					}
				}
			)?
	|	(	'0' {isDecimal = true;} // special case for just '0'
			(	('x')
				(											// hex
					// the 'e'|'E' and float suffix stuff look
					// like hex digits, hence the (...)+ doesn't
					// know when to stop: ambig.  ANTLR resolves
					// it correctly by matching immediately.  It
					// is therefore ok to hush warning.
					// TODO options { warnWhenFollowAmbig=false; }
				:	HEX_DIGIT
				)+
			|	('0'..'7')+									// octal
			)?
		|	('1'..'9') ('0'..'9')*  {isDecimal=true;}		// non-zero decimal
		)
		(	('l') { _type = NUM_LONG; }

		// only check to see if it's a float if looks like decimal so far
		|	{isDecimal}?
			(   '.' ('0'..'9')* (EXPONENT)? (f2=FLOAT_SUFFIX {t=f2;})?
			|   EXPONENT (f3=FLOAT_SUFFIX {t=f3;})?
			|   f4=FLOAT_SUFFIX {t=f4;}
			)
			{
				if (t != null && t.Text.ToUpperInvariant().IndexOf('F') >= 0)
				{
					_type = NUM_FLOAT;
				}
				else if (t != null && t.Text.ToUpperInvariant().IndexOf('M')>=0)
				{
					_type = NUM_DECIMAL;
				}
				else
				{
					_type = NUM_DOUBLE; // assume double
				}
			}
		)?
	;

// hexadecimal digit (again, note it's protected!)
fragment
HEX_DIGIT
	:	('0'..'9'|'a'..'f')
	;

// a couple protected methods to assist in matching floating point numbers
fragment
EXPONENT
	:	('e') ('+'|'-')? ('0'..'9')+
	;

fragment
FLOAT_SUFFIX
	:	'f'|'d'|'m'
	;

