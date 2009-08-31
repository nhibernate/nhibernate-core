/**
 * SQL Generator Tree Parser, providing SQL rendering of SQL ASTs produced by the previous phase, HqlSqlWalker.  All
 * syntax decoration such as extra spaces, lack of spaces, extra parens, etc. should be added by this class.
 * <br>
 * This grammar processes the HQL/SQL AST and produces an SQL string.  The intent is to move dialect-specific
 * code into a sub-class that will override some of the methods, just like the other two grammars in this system.
 * @author Joshua Davis (joshua@hibernate.org)
 */

tree grammar SqlGenerator;

options 
{
	language=CSharp2;
	tokenVocab=HqlSqlWalker;
	ASTLabelType=IASTNode;
	output=None;
}

@namespace { NHibernate.Hql.Ast.ANTLR }

@header
{
using NHibernate.Hql.Ast.ANTLR.Tree;
}

statement
	: selectStatement
	| updateStatement
	| deleteStatement
	| insertStatement
	;

selectStatement
	: ^(SELECT { Out("select "); }
		selectClause
		from
		( ^(WHERE { Out(" where "); } whereExpr ) )?
		( ^(GROUP { Out(" group by "); } groupExprs ( ^(HAVING { Out(" having "); } booleanExpr[false]) )? ) )?
		( ^(ORDER { Out(" order by "); } orderExprs ) )?
	)
	;

// Note: eats the FROM token node, as it is not valid in an update statement.
// It's outlived its usefulness after analysis phase :)
// TODO : needed to use conditionList directly here and deleteStatement, as whereExprs no longer works for this stuff
updateStatement
	: ^(UPDATE { Out("update "); }
		^( FROM fromTable )
		setClause
		(whereClause)?
	)
	;

deleteStatement
	// Note: not space needed at end of "delete" because the from rule included one before the "from" it outputs
	: ^(DELETE { Out("delete"); }
		from
		(whereClause)?
	)
	;

insertStatement
	: ^(INSERT { Out( "insert " ); }
		^(i=INTO { Out( i ); Out( " " ); } .*)
		selectStatement
	)
	;

setClause
	// Simply re-use comparisionExpr, because it already correctly defines the EQ rule the
	// way it is needed here; not the most aptly named, but ah
	: ^( SET { Out(" set "); } comparisonExpr[false] ( { Out(", "); } comparisonExpr[false] )* )
	;

whereClause
	: ^(WHERE { Out(" where "); } whereClauseExpr )
	;

whereClauseExpr
	: (SQL_TOKEN) => conditionList
	| booleanExpr[ false ]
	;

orderExprs
	// TODO: remove goofy space before the comma when we don't have to regression test anymore.
	: ( expr ) (dir=orderDirection { Out(" "); Out($dir.start); })? ( {Out(", "); } orderExprs)?
	;

groupExprs
	// TODO: remove goofy space before the comma when we don't have to regression test anymore.
	: expr ( {Out(" , "); } groupExprs)?
	;

orderDirection
	: ASCENDING
	| DESCENDING
	;

whereExpr
	// Expect the filter subtree, followed by the theta join subtree, followed by the HQL condition subtree.
	// Might need parens around the HQL condition if there is more than one subtree.
	// Put 'and' between each subtree.
	: filters
		( { Out(" and "); } thetaJoins )?
		( { Out(" and "); } booleanExpr [ true ] )?
	| thetaJoins
		( { Out(" and "); } booleanExpr [ true ] )? 
	| booleanExpr[false]
	;

filters
	: ^(FILTERS conditionList )
	;

thetaJoins
	: ^(THETA_JOINS conditionList )
	;

conditionList
	: sqlToken ( { Out(" and "); } conditionList )?
	;

selectClause
	: ^(SELECT_CLAUSE (distinctOrAll)? ( selectColumn )+ )
	;

selectColumn
	: p=selectExpr (sc=SELECT_COLUMNS { Out($sc); } )? { Separator( ($sc != null) ? $sc : $p.start ,", "); }
	;

selectExpr
	: e=selectAtom { Out($e.start); }
	| count
	| ^(CONSTRUCTOR (DOT | IDENT) ( selectColumn )+ )
	| methodCall
	| aggregate
	| c=constant { Out($c.start); }
	| arithmeticExpr
	| param=PARAM { Out($param); }
//	| sn=SQL_NODE { Out(sn); }
	| { Out("("); } selectStatement { Out(")"); }
	;

count
	: ^(COUNT { Out("count("); }  ( distinctOrAll ) ? countExpr { Out(")"); } )
	;

distinctOrAll
	: DISTINCT { Out("distinct "); }
	| ^(ALL .*) { Out("all "); }
	;

countExpr
	// Syntacitic predicate resolves star all by itself, avoiding a conflict with STAR in expr.
	: ROW_STAR { Out("*"); }
	| simpleExpr
	;

selectAtom
	: ^(DOT .*)
	| ^(SQL_TOKEN .*)
	| ^(ALIAS_REF .*)
	| ^(SELECT_EXPR .*)
	;

// The from-clause piece is all goofed up.  Currently, nodes of type FROM_FRAGMENT
// and JOIN_FRAGMENT can occur at any level in the FromClause sub-tree. We really
// should come back and clean this up at some point; which I think will require
// a post-HqlSqlWalker phase to "re-align" the FromElements in a more sensible
// manner.
from
	: ^(f=FROM { Out(" from "); }
		(fromTable)* )
	;

fromTable
@after {
   FromFragmentSeparator($a);
}
	// Write the table node (from fragment) and all the join fragments associated with it.
	: ^( a=FROM_FRAGMENT  { Out(a); } (tableJoin [ a ])* )
	| ^( a=JOIN_FRAGMENT  { Out(a); } (tableJoin [ a ])* )
	;

tableJoin [ IASTNode parent ]
	: ^( c=JOIN_FRAGMENT { Out(" "); Out($c); } (tableJoin [ c ] )* )
	| ^( d=FROM_FRAGMENT { NestedFromFragment($d,parent); } (tableJoin [ d ] )* )
	;

booleanOp[ bool parens ]
	: ^(AND booleanExpr[true] { Out(" and "); } booleanExpr[true])
	| ^(OR { if (parens) Out("("); } booleanExpr[false] { Out(" or "); } booleanExpr[false] { if (parens) Out(")"); })
	| ^(NOT { Out(" not ("); } booleanExpr[false] { Out(")"); } )
	;

booleanExpr[ bool parens ]
	: booleanOp [ parens ]
	| comparisonExpr [ parens ]
	| methodCall
	| st=SQL_TOKEN { Out(st); } // solely for the purpose of mapping-defined where-fragments
	;
	
comparisonExpr[ bool parens ]
	: binaryComparisonExpression
	| { if (parens) Out("("); } exoticComparisonExpression { if (parens) Out(")"); }
	;
	
binaryComparisonExpression
	: ^(EQ expr { Out("="); } expr)
	| ^(NE expr { Out("<>"); } expr)
	| ^(GT expr { Out(">"); } expr)
	| ^(GE expr { Out(">="); } expr)
	| ^(LT expr { Out("<"); } expr)
	| ^(LE expr { Out("<="); } expr)
	;
	
exoticComparisonExpression
	: ^(LIKE expr { Out(" like "); } expr likeEscape )
	| ^(NOT_LIKE expr { Out(" not like "); } expr likeEscape)
	| ^(BETWEEN expr { Out(" between "); } expr { Out(" and "); } expr)
	| ^(NOT_BETWEEN expr { Out(" not between "); } expr { Out(" and "); } expr)
	| ^(IN expr { Out(" in"); } inList )
	| ^(NOT_IN expr { Out(" not in "); } inList )
	| ^(EXISTS { OptionalSpace(); Out("exists "); } quantified )
	| ^(IS_NULL expr) { Out(" is null"); }
	| ^(IS_NOT_NULL expr) { Out(" is not null"); }
	;

likeEscape
	: ( ^(ESCAPE { Out(" escape "); } expr) )?
	;

inList
	: ^(IN_LIST { Out(" "); } ( parenSelect | simpleExprList ) )
	;
	
simpleExprList
	: { Out("("); } (e=simpleExpr { Separator($e.start," , "); } )* { Out(")"); }
	;

// A simple expression, or a sub-select with parens around it.
expr
	: simpleExpr
	| ^( VECTOR_EXPR { Out("("); } (e=expr { Separator($e.start," , "); } )*  { Out(")"); } )
	| parenSelect
	| ^(ANY { Out("any "); } quantified )
	| ^(ALL { Out("all "); } quantified )
	| ^(SOME { Out("some "); } quantified )
	;
	
quantified
	: { Out("("); } ( sqlToken | selectStatement ) { Out(")"); } 
	;
	
parenSelect
	: { Out("("); } selectStatement { Out(")"); } 
	| ^(UNION { Out("("); } selectStatement { Out(") union "); } parenSelect )
	;

	
simpleExpr
	: c=constant { Out($c.start); }
	| NULL { Out("null"); }
	| addrExpr
	| sqlToken
	| aggregate
	| methodCall
	| count
	| parameter
	| arithmeticExpr
	;
	
constant
	: NUM_DOUBLE
	| NUM_DECIMAL
	| NUM_FLOAT
	| NUM_INT
	| NUM_LONG
	| QUOTED_String
	| CONSTANT
	| JAVA_CONSTANT
	| TRUE
	| FALSE
	| IDENT
	;
	
arithmeticExpr
	: additiveExpr
	| bitwiseExpr
	| multiplicativeExpr
//	| ^(CONCAT { Out("("); } expr ( { Out("||"); } expr )+ { Out(")"); } )
	| ^(UNARY_MINUS { Out("-"); } expr)
	| caseExpr
	;

additiveExpr
	: ^(PLUS expr { Out("+"); } expr)
	| ^(MINUS expr { Out("-"); } nestedExprAfterMinusDiv)
	;

bitwiseExpr
	: ^(BAND expr { Out("&"); } nestedExpr)
	| ^(BOR expr { Out("|"); } nestedExpr)
	| ^(BXOR expr { Out("^"); } nestedExpr)
	| ^(BNOT { Out("~"); } nestedExpr)	
	;

multiplicativeExpr
	: ^(STAR nestedExpr { Out("*"); } nestedExpr)
	| ^(DIV nestedExpr { Out("/"); } nestedExprAfterMinusDiv)
	;

nestedExpr
	// Generate parens around nested additive expressions, use a syntactic predicate to avoid conflicts with 'expr'.
	: (additiveExpr) => { Out("("); } additiveExpr { Out(")"); }
	| (bitwiseExpr) => { Out("("); } bitwiseExpr { Out(")"); }
	| expr
	;
	
nestedExprAfterMinusDiv
	// Generate parens around nested arithmetic expressions, use a syntactic predicate to avoid conflicts with 'expr'.
	: (arithmeticExpr) => { Out("("); } arithmeticExpr { Out(")"); }
	| expr
	;

caseExpr
	: ^(CASE { Out("case"); } 
		( ^(WHEN { Out( " when "); } booleanExpr[false] { Out(" then "); } expr) )+ 
		( ^(ELSE { Out(" else "); } expr) )?
		{ Out(" end"); } )
	| ^(CASE2 { Out("case "); } expr
		( ^(WHEN { Out( " when "); } expr { Out(" then "); } expr) )+ 
		( ^(ELSE { Out(" else "); } expr) )?
		{ Out(" end"); } )
	;

aggregate
	: ^(a=AGGREGATE { Out(a); Out("("); }  expr { Out(")"); } )
	;


methodCall
	: ^(m=METHOD_CALL i=METHOD_NAME { BeginFunctionTemplate(m,i); }
	 ( ^(EXPR_LIST (arguments)? ) )?
	 { EndFunctionTemplate(m); } )
	;

arguments
	: (expr | comparisonExpr[true]) ( { CommaBetweenParameters(", "); } (expr | comparisonExpr[true]) )*
	;

parameter
	: n=NAMED_PARAM { Out(n); }
	| p=PARAM { Out(p); }
	;

addrExpr
	: ^(r=DOT . .) { Out(r); }
	| i=ALIAS_REF { Out(i); }
	| ^(j=INDEX_OP .*) { Out(j); }
	;

sqlToken
	: ^(t=SQL_TOKEN { Out(t); } .*)
	;

