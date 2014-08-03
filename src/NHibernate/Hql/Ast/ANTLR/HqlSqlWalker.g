tree grammar HqlSqlWalker;

options
{
	language=CSharp3;
	output=AST;
	tokenVocab=Hql;
	ASTLabelType=IASTNode;
}

tokens
{
	FROM_FRAGMENT;	// A fragment of SQL that represents a table reference in a FROM clause.
	IMPLIED_FROM;	// An implied FROM element.
	JOIN_FRAGMENT;	// A JOIN fragment.
	SELECT_CLAUSE;
	LEFT_OUTER;
	RIGHT_OUTER;
	ALIAS_REF;      // An IDENT that is a reference to an entity via it's alias.
	PROPERTY_REF;   // A DOT that is a reference to a property in an entity.
	SQL_TOKEN;      // A chunk of SQL that is 'rendered' already.
	SELECT_COLUMNS; // A chunk of SQL representing a bunch of select columns.
	SELECT_EXPR;    // A select expression, generated from a FROM element.
	THETA_JOINS;	// Root of theta join condition subtree.
	FILTERS;		// Root of the filters condition subtree.
	METHOD_NAME;    // An IDENT that is a method name.
	NAMED_PARAM;    // A named parameter (:foo).
	BOGUS;          // Used for error state detection, etc.
}

@namespace { NHibernate.Hql.Ast.ANTLR }

@header
{
using System;
using System.Text;
using NHibernate.Hql.Ast.ANTLR.Tree;
}

// The main statement rule.
public statement
	: selectStatement | updateStatement | deleteStatement | insertStatement
	;

selectStatement
	: query
	;

// Cannot use just the fromElement rule here in the update and delete queries
// because fromElement essentially relies on a FromClause already having been
// built :(
updateStatement!
	@after{
		BeforeStatementCompletion( "update" );
		PrepareVersioned( $updateStatement.tree, $v );
		PostProcessUpdate( $updateStatement.tree );
		AfterStatementCompletion( "update" );
	}
	: ^( u=UPDATE { BeforeStatement( "update", UPDATE ); } (v=VERSIONED)? f=fromClause s=setClause (w=whereClause)? ) 
		-> ^($u $f $s $w?)
	;

deleteStatement
	@after {
		BeforeStatementCompletion( "delete" );
		PostProcessDelete( $deleteStatement.tree );
		AfterStatementCompletion( "delete" );
	}
	: ^( DELETE { BeforeStatement( "delete", DELETE ); } fromClause (whereClause)? ) 
	;

insertStatement
	// currently only "INSERT ... SELECT ..." statements supported;
	// do we also need support for "INSERT ... VALUES ..."?
	//
	@after {
		BeforeStatementCompletion( "insert" );
		PostProcessInsert( $insertStatement.tree );
		AfterStatementCompletion( "insert" );
	}
	: ^( INSERT { BeforeStatement( "insert", INSERT ); } intoClause query ) 
	;

intoClause! 
	@after {
		$intoClause.tree = CreateIntoClause($p.p, $ps.tree);
	}
	: ^( INTO { HandleClauseStart( INTO ); } (p=path) ps=insertablePropertySpec ) 
	;

insertablePropertySpec
	: ^( RANGE (IDENT)+ )
	;

setClause
	: ^( SET { HandleClauseStart( SET ); } (assignment)* )
	;

assignment
	@after {
		EvaluateAssignment( $assignment.tree );
	}
	// Note: the propertyRef here needs to be resolved
	// *before* we evaluate the newValue rule...
	: ^( EQ (p=propertyRef) { Resolve($p.tree); } (newValue) ) 
	;

// For now, just use expr.  Revisit after ejb3 solidifies this.
newValue
	: expr | query
	;

query
	: unionedQuery 
	| ^(UNION unionedQuery query)
	;

// The query / subquery rule. Pops the current 'from node' context 
// (list of aliases).
unionedQuery!
	@after {
		// Antlr note: #x_in refers to the input AST, #x refers to the output AST
		BeforeStatementCompletion( "select" );
		ProcessQuery( $s.tree, $unionedQuery.tree );
		AfterStatementCompletion( "select" );
	}
	: ^( QUERY { BeforeStatement( "select", SELECT ); }
			// The first phase places the FROM first to make processing the SELECT simpler.
			^(SELECT_FROM
				f=fromClause
				(s=selectClause)?
			)
			(w=whereClause)?
			(g=groupClause)?
			(h=havingClause)?
			(o=orderClause)?
			(sk=skipClause)?
			(tk=takeClause)?
		) 
	-> ^(SELECT $s? $f $w? $g? $h? $o? $sk? $tk?)
	;

orderClause
	: ^(ORDER { HandleClauseStart( ORDER ); } (orderExprs | query (ASCENDING | DESCENDING)? ))
	;

orderExprs
	: expr ( ASCENDING | DESCENDING )? (orderExprs)?
	;

skipClause
	: ^(SKIP (NUM_INT | parameter))
	;

takeClause
	: ^(TAKE (NUM_INT | parameter))
	;

groupClause
	: ^(GROUP { HandleClauseStart( GROUP ); } (expr)+ )
	;

havingClause
	: ^(HAVING logicalExpr)
	;

selectClause!
	: ^(SELECT { HandleClauseStart( SELECT ); BeforeSelectClause(); } (d=DISTINCT)? x=selectExprList ) 
	-> ^(SELECT_CLAUSE["{select clause}"] $d? $x)
	;

selectExprList @init{
		bool oldInSelect = _inSelect;
		_inSelect = true;
	}
	: ( selectExpr | aliasedSelectExpr )+ {
		_inSelect = oldInSelect;
	}
	;

aliasedSelectExpr!
	@after {
	    SetAlias($se.tree,$i.tree);
	    $aliasedSelectExpr.tree = $se.tree;
	}
	: ^(AS se=selectExpr i=identifier) 
	;

selectExpr
	: p=propertyRef					{ ResolveSelectExpression($p.tree); }
	| ^(ALL ar2=aliasRef) 			{ ResolveSelectExpression($ar2.tree); $selectExpr.tree = $ar2.tree; }
	| ^(OBJECT ar3=aliasRef)		{ ResolveSelectExpression($ar3.tree); $selectExpr.tree = $ar3.tree; }
	| con=constructor 				{ ProcessConstructor($con.tree); }
	| functionCall
	| parameter
	| count
	| collectionFunction			// elements() or indices()
	| literal
	| arithmeticExpr
	| query
	;

count
	: ^(COUNT ( DISTINCT | ALL )? ( aggregateExpr | ROW_STAR ) )
	;

constructor
	: ^(CONSTRUCTOR path ( selectExpr | aliasedSelectExpr )* )
	;

aggregateExpr
	: expr //p:propertyRef { resolve(#p); }
	| collectionFunction
	;

// Establishes the list of aliases being used by this query.
fromClause 
@init{
		// NOTE: This references the INPUT AST! (see http://www.antlr.org/doc/trees.html#Action Translation)
		// the ouput AST (#fromClause) has not been built yet.
		PrepareFromClauseInputTree((IASTNode) input.LT(1), input);
	}
	: ^(f=FROM { PushFromClause($f.tree); HandleClauseStart( FROM ); } fromElementList )
	;

fromElementList @init{
		bool oldInFrom = _inFrom;
		_inFrom = true;
		}
	: (fromElement)+ {
		_inFrom = oldInFrom;
		}
	;

fromElement! 
@init {
   IASTNode fromElement = null;
}
	// A simple class name, alias element.
	: ^(RANGE p=path (a=ALIAS)? (pf=FETCH)? ) { fromElement = CreateFromElement($p.p, $p.tree, $a, $pf); }
		-> {fromElement != null}? ^({fromElement})
		->
	| je=joinElement 
		-> //$je
	// A from element created due to filter compilation
	| fe=FILTER_ENTITY a3=ALIAS 
		-> ^({CreateFromFilterElement($fe,$a3)})
	;

joinElement! 
	// A from element with a join.  This time, the 'path' should be treated as an AST
	// and resolved (like any path in a WHERE clause).   Make sure all implied joins
	// generated by the property ref use the join type, if it was specified.
	: ^(JOIN (j=joinType { SetImpliedJoinType($j.j); } )? (f=FETCH)? pRef=propertyRef (a=ALIAS)? (pf=FETCH)? (^((with=WITH) .*))? ) 
	{
		CreateFromJoinElement($pRef.tree,$a,$j.j,$f, $pf, $with);
		SetImpliedJoinType(INNER);	// Reset the implied join type.
	}
	;

// Returns an node type integer that represents the join type
// tokens.
joinType returns [int j] 
@init {
   $j = INNER;
}
	: ( (left=LEFT | right=RIGHT) (outer=OUTER)? 
	{
		if (left != null)       $j = LEFT_OUTER;
		else if (right != null) $j = RIGHT_OUTER;
		else if (outer != null) $j = RIGHT_OUTER;
	} ) 
	| FULL {
		$j = FULL;
	}
	| INNER {
		$j = INNER;
	}
	;

// Matches a path and returns the normalized string for the path (usually
// fully qualified a class name).
path returns [String p] 
	: a=identifier { $p = $a.start.ToString();}
	| ^(DOT x=path y=identifier) {
			StringBuilder buf = new StringBuilder();
			buf.Append($x.p).Append('.').Append($y.start.ToString());
			$p = buf.ToString();
		}
	;

// Returns a path as a single identifier node.
pathAsIdent 
    : path 
    -> ^(IDENT[$path.p])
    ;

withClause
	// Note : this is used internally from the HqlSqlWalker to
	// parse the node recognized with the with keyword earlier.
	// Done this way because it relies on the join it "qualifies"
	// already having been processed, which would not be the case
	// if withClause was simply referenced from the joinElement
	// rule during recognition...
	: ^(w=WITH { HandleClauseStart( WITH ); } b=logicalExpr ) 
	-> ^($w $b)
	;

whereClause
	: ^(w=WHERE { HandleClauseStart( WHERE ); } b=logicalExpr ) 
	-> ^($w $b)
	;

logicalExpr
	: ^(AND logicalExpr logicalExpr)
	| ^(OR logicalExpr logicalExpr)
	| ^(NOT logicalExpr)
	| comparisonExpr
	| functionCall
	| logicalPath
	;

logicalPath
	@after {
	    PrepareLogicOperator( $logicalPath.tree );
	}
	: p=addrExpr [ true ] {Resolve($p.tree);}  -> ^(EQ $p TRUE)
	;

// TODO: Add any other comparison operators here.
comparisonExpr
	@after {
	    PrepareLogicOperator( $comparisonExpr.tree );
	}
	:
	( ^(EQ exprOrSubquery exprOrSubquery)
	| ^(NE exprOrSubquery exprOrSubquery)
	| ^(LT exprOrSubquery exprOrSubquery)
	| ^(GT exprOrSubquery exprOrSubquery)
	| ^(LE exprOrSubquery exprOrSubquery)
	| ^(GE exprOrSubquery exprOrSubquery)
	| ^(LIKE exprOrSubquery expr ( ^(ESCAPE expr) )? )
	| ^(NOT_LIKE exprOrSubquery expr ( ^(ESCAPE expr) )? )
	| ^(BETWEEN exprOrSubquery exprOrSubquery exprOrSubquery)
	| ^(NOT_BETWEEN exprOrSubquery exprOrSubquery exprOrSubquery)
	| ^(IN exprOrSubquery inRhs )
	| ^(NOT_IN exprOrSubquery inRhs )
	| ^(IS_NULL exprOrSubquery)
	| ^(IS_NOT_NULL exprOrSubquery)
//	| ^(IS_TRUE expr)
//	| ^(IS_FALSE expr)
	| ^(EXISTS ( expr | collectionFunctionOrSubselect ) )
	)
	;

inRhs @init {	int UP = 99999;		// TODO - added this to get compile working.  It's bogus & should be removed
	}
	: ^(IN_LIST ( collectionFunctionOrSubselect | expr* ) )
	;

exprOrSubquery
	: expr
	| query
	| ^(ANY collectionFunctionOrSubselect)
	| ^(ALL collectionFunctionOrSubselect)
	| ^(SOME collectionFunctionOrSubselect)
	;
	
collectionFunctionOrSubselect
	: collectionFunction
	| query
	;
	
expr
	: ae=addrExpr [ true ] { Resolve($ae.tree); }	// Resolve the top level 'address expression'
	| ^( VECTOR_EXPR (expr)* )
	| constant
	| arithmeticExpr
	| functionCall							// Function call, not in the SELECT clause.
	| parameter
	| count										// Count, not in the SELECT clause.
	;

arithmeticExpr
	@after {
		if ($c.tree == null)
		{
			PrepareArithmeticOperator( $arithmeticExpr.tree );
		}
	}
	: ^(PLUS exprOrSubquery exprOrSubquery)
	| ^(MINUS exprOrSubquery exprOrSubquery)
	| ^(DIV exprOrSubquery exprOrSubquery)
	| ^(STAR exprOrSubquery exprOrSubquery)
	| ^(BNOT exprOrSubquery)
	| ^(BAND exprOrSubquery exprOrSubquery)
	| ^(BOR exprOrSubquery exprOrSubquery)
	| ^(BXOR exprOrSubquery exprOrSubquery)
//	| ^(CONCAT exprOrSubquery (exprOrSubquery)+ )
	| ^(UNARY_MINUS exprOrSubquery)
	| c=caseExpr
	;

caseExpr
	: ^(CASE { _inCase = true; } (^(WHEN logicalExpr expr))+ (^(ELSE expr))?) { _inCase = false; }
	| ^(CASE2 { _inCase = true; } expr (^(WHEN expr expr))+ (^(ELSE expr))?) { _inCase = false; }
	;

//TODO: I don't think we need this anymore .. how is it different to 
//      maxelements, etc, which are handled by functionCall
collectionFunction
	: ^(e=ELEMENTS {_inFunctionCall=true;} p1=propertyRef { Resolve($p1.tree); } ) 
		{ ProcessFunction($e.tree,_inSelect); } {_inFunctionCall=false;}
	| ^(i=INDICES {_inFunctionCall=true;} p2=propertyRef { Resolve($p2.tree); } ) 
		{ ProcessFunction($i.tree,_inSelect); } {_inFunctionCall=false;}
	;

functionCall
	: ^(m=METHOD_CALL  {_inFunctionCall=true;} pathAsIdent ( ^(EXPR_LIST (expr | query | comparisonExpr)* ) )? )
		{ ProcessFunction($m.tree,_inSelect); _inFunctionCall=false; }
	| ^(AGGREGATE aggregateExpr )
	;

constant
	: literal
	| NULL
	| t=TRUE { ProcessBool($t); } 
	| f=FALSE { ProcessBool($f); }
	| JAVA_CONSTANT
	;

literal
	: numericLiteral
	| stringLiteral
	;

numericLiteral
@after
{
	ProcessNumericLiteral( $numericLiteral.tree );
}
	: NUM_INT
	| NUM_LONG
	| NUM_FLOAT
	| NUM_DOUBLE
	| NUM_DECIMAL
	;

stringLiteral
	: QUOTED_String
	;

identifier
	: (IDENT | WEIRD_IDENT)
	;

addrExpr [ bool root ]
	: addrExprDot [root]
	| addrExprIndex [root]
	| addrExprIdent [root]
 	;

addrExprDot [ bool root ]
@after
{
	LookupProperty($addrExprDot.tree,root,false);
}
	:	^(d=DOT lhs=addrExprLhs rhs=propertyName )
		-> ^($d $lhs $rhs)
	;

addrExprIndex [ bool root ]
@after
{
	ProcessIndex($addrExprIndex.tree);
}

	:	^(i=INDEX_OP lhs2=addrExprLhs rhs2=expr)	
		-> ^($i $lhs2 $rhs2)
	;

addrExprIdent [ bool root ]
	:	p=identifier 
	-> {IsNonQualifiedPropertyRef($p.tree)}? ^({LookupNonQualifiedProperty($p.tree)})
	-> ^({Resolve($p.tree)})
	;

addrExprLhs
	: addrExpr [ false ]
	;

propertyName
	: identifier
	| CLASS
	| ELEMENTS
	| INDICES
	;

propertyRef!
	: propertyRefPath
	| propertyRefIdent
	;
	
propertyRefPath
@after {
	// This gives lookupProperty() a chance to transform the tree to process collection properties (.elements, etc).
	retval.Tree = LookupProperty((IASTNode) retval.Tree,false,true);
}
	: ^(d=DOT lhs=propertyRefLhs rhs=propertyName )	
		-> ^($d $lhs $rhs)
	;
	
propertyRefIdent
@after {
	// In many cases, things other than property-refs are recognized
	// by this propertyRef rule.  Some of those I have seen:
	//  1) select-clause from-aliases
	//  2) sql-functions
	if ( IsNonQualifiedPropertyRef($p.tree) ) {
		retval.Tree = LookupNonQualifiedProperty($p.tree);
	}
	else {
		Resolve($p.tree);
		retval.Tree = $p.tree;
	}
}
	: p=identifier 
	;

propertyRefLhs
	: propertyRef
	;

aliasRef!
	@after
	{
		LookupAlias($aliasRef.tree);
	}
	: i=identifier 
	// TODO -> ^(ALIAS_REF[$i.start, $i.text])
	;

parameter!
	: ^(c=COLON a=identifier) 
		// Create a NAMED_PARAM node instead of (COLON IDENT).
		-> ^({GenerateNamedParameter( $c, $a.tree )})
	| ^(p=PARAM (n=NUM_INT)?) 
		-> {n != null}? ^({GenerateNamedParameter( $p, $n )})
		-> ^({GeneratePositionalParameter( $p )})
	;

numericInteger
	: NUM_INT
	;
