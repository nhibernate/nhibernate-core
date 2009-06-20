// $ANTLR 3.1.2 /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g 2009-06-20 02:19:15

// The variable 'variable' is assigned but its value is never used.
#pragma warning disable 168, 219
// Unreachable code detected.
#pragma warning disable 162
namespace  NHibernate.Hql.Ast.ANTLR 
{

using NHibernate.Hql.Ast.ANTLR.Tree;


using System;
using Antlr.Runtime;
using Antlr.Runtime.Tree;using IList 		= System.Collections.IList;
using ArrayList 	= System.Collections.ArrayList;
using Stack 		= Antlr.Runtime.Collections.StackList;

using IDictionary	= System.Collections.IDictionary;
using Hashtable 	= System.Collections.Hashtable;

/**
 * SQL Generator Tree Parser, providing SQL rendering of SQL ASTs produced by the previous phase, HqlSqlWalker.  All
 * syntax decoration such as extra spaces, lack of spaces, extra parens, etc. should be added by this class.
 * <br>
 * This grammar processes the HQL/SQL AST and produces an SQL string.  The intent is to move dialect-specific
 * code into a sub-class that will override some of the methods, just like the other two grammars in this system.
 * @author Joshua Davis (joshua@hibernate.org)
 */
public partial class SqlGenerator : TreeParser
{
    public static readonly string[] tokenNames = new string[] 
	{
        "<invalid>", 
		"<EOR>", 
		"<DOWN>", 
		"<UP>", 
		"ALL", 
		"ANY", 
		"AND", 
		"AS", 
		"ASCENDING", 
		"AVG", 
		"BETWEEN", 
		"CLASS", 
		"COUNT", 
		"DELETE", 
		"DESCENDING", 
		"DOT", 
		"DISTINCT", 
		"ELEMENTS", 
		"ESCAPE", 
		"EXISTS", 
		"FALSE", 
		"FETCH", 
		"FROM", 
		"FULL", 
		"GROUP", 
		"HAVING", 
		"IN", 
		"INDICES", 
		"INNER", 
		"INSERT", 
		"INTO", 
		"IS", 
		"JOIN", 
		"LEFT", 
		"LIKE", 
		"MAX", 
		"MIN", 
		"NEW", 
		"NOT", 
		"NULL", 
		"OR", 
		"ORDER", 
		"OUTER", 
		"PROPERTIES", 
		"RIGHT", 
		"SELECT", 
		"SET", 
		"SOME", 
		"SUM", 
		"TRUE", 
		"UNION", 
		"UPDATE", 
		"VERSIONED", 
		"WHERE", 
		"LITERAL_by", 
		"CASE", 
		"END", 
		"ELSE", 
		"THEN", 
		"WHEN", 
		"ON", 
		"WITH", 
		"BOTH", 
		"EMPTY", 
		"LEADING", 
		"MEMBER", 
		"OBJECT", 
		"OF", 
		"TRAILING", 
		"AGGREGATE", 
		"ALIAS", 
		"CONSTRUCTOR", 
		"CASE2", 
		"EXPR_LIST", 
		"FILTER_ENTITY", 
		"IN_LIST", 
		"INDEX_OP", 
		"IS_NOT_NULL", 
		"IS_NULL", 
		"METHOD_CALL", 
		"NOT_BETWEEN", 
		"NOT_IN", 
		"NOT_LIKE", 
		"ORDER_ELEMENT", 
		"QUERY", 
		"RANGE", 
		"ROW_STAR", 
		"SELECT_FROM", 
		"UNARY_MINUS", 
		"UNARY_PLUS", 
		"VECTOR_EXPR", 
		"WEIRD_IDENT", 
		"CONSTANT", 
		"NUM_INT", 
		"NUM_DOUBLE", 
		"NUM_FLOAT", 
		"NUM_LONG", 
		"JAVA_CONSTANT", 
		"COMMA", 
		"EQ", 
		"OPEN", 
		"CLOSE", 
		"NE", 
		"SQL_NE", 
		"LT", 
		"GT", 
		"LE", 
		"GE", 
		"CONCAT", 
		"BNOT", 
		"BOR", 
		"BXOR", 
		"BAND", 
		"PLUS", 
		"MINUS", 
		"STAR", 
		"DIV", 
		"OPEN_BRACKET", 
		"CLOSE_BRACKET", 
		"COLON", 
		"PARAM", 
		"QUOTED_String", 
		"IDENT", 
		"ID_START_LETTER", 
		"ID_LETTER", 
		"ESCqs", 
		"WS", 
		"EXPONENT", 
		"FLOAT_SUFFIX", 
		"HEX_DIGIT", 
		"'ascending'", 
		"'descending'", 
		"FROM_FRAGMENT", 
		"IMPLIED_FROM", 
		"JOIN_FRAGMENT", 
		"SELECT_CLAUSE", 
		"LEFT_OUTER", 
		"RIGHT_OUTER", 
		"ALIAS_REF", 
		"PROPERTY_REF", 
		"SQL_TOKEN", 
		"SELECT_COLUMNS", 
		"SELECT_EXPR", 
		"THETA_JOINS", 
		"FILTERS", 
		"METHOD_NAME", 
		"NAMED_PARAM", 
		"BOGUS"
    };

    public const int COMMA = 98;
    public const int EXISTS = 19;
    public const int EXPR_LIST = 73;
    public const int FETCH = 21;
    public const int MINUS = 114;
    public const int AS = 7;
    public const int END = 56;
    public const int INTO = 30;
    public const int NAMED_PARAM = 146;
    public const int FROM_FRAGMENT = 132;
    public const int FALSE = 20;
    public const int ELEMENTS = 17;
    public const int THEN = 58;
    public const int FILTERS = 144;
    public const int ALIAS = 70;
    public const int BOR = 110;
    public const int ALIAS_REF = 138;
    public const int ON = 60;
    public const int DOT = 15;
    public const int ORDER = 41;
    public const int AND = 6;
    public const int CONSTANT = 92;
    public const int RIGHT = 44;
    public const int METHOD_CALL = 79;
    public const int UNARY_MINUS = 88;
    public const int CONCAT = 108;
    public const int PROPERTIES = 43;
    public const int SELECT = 45;
    public const int LE = 106;
    public const int RIGHT_OUTER = 137;
    public const int BETWEEN = 10;
    public const int NUM_INT = 93;
    public const int SQL_TOKEN = 140;
    public const int LEFT_OUTER = 136;
    public const int BOTH = 62;
    public const int METHOD_NAME = 145;
    public const int PLUS = 113;
    public const int VERSIONED = 52;
    public const int MEMBER = 65;
    public const int UNION = 50;
    public const int DISTINCT = 16;
    public const int RANGE = 85;
    public const int FILTER_ENTITY = 74;
    public const int IDENT = 122;
    public const int WHEN = 59;
    public const int DESCENDING = 14;
    public const int WS = 126;
    public const int EQ = 99;
    public const int NEW = 37;
    public const int LT = 104;
    public const int ESCqs = 125;
    public const int OF = 67;
    public const int T__130 = 130;
    public const int UPDATE = 51;
    public const int SELECT_FROM = 87;
    public const int LITERAL_by = 54;
    public const int FLOAT_SUFFIX = 128;
    public const int ANY = 5;
    public const int UNARY_PLUS = 89;
    public const int NUM_FLOAT = 95;
    public const int GE = 107;
    public const int CASE = 55;
    public const int OPEN_BRACKET = 117;
    public const int ELSE = 57;
    public const int OPEN = 100;
    public const int COUNT = 12;
    public const int NULL = 39;
    public const int THETA_JOINS = 143;
    public const int IMPLIED_FROM = 133;
    public const int COLON = 119;
    public const int DIV = 116;
    public const int HAVING = 25;
    public const int ALL = 4;
    public const int SET = 46;
    public const int T__131 = 131;
    public const int INSERT = 29;
    public const int TRUE = 49;
    public const int CASE2 = 72;
    public const int IS_NOT_NULL = 77;
    public const int WHERE = 53;
    public const int AGGREGATE = 69;
    public const int VECTOR_EXPR = 90;
    public const int BNOT = 109;
    public const int LEADING = 64;
    public const int CLOSE_BRACKET = 118;
    public const int NUM_DOUBLE = 94;
    public const int INNER = 28;
    public const int QUERY = 84;
    public const int ORDER_ELEMENT = 83;
    public const int SELECT_EXPR = 142;
    public const int OR = 40;
    public const int JOIN_FRAGMENT = 134;
    public const int FULL = 23;
    public const int INDICES = 27;
    public const int IS_NULL = 78;
    public const int GROUP = 24;
    public const int ESCAPE = 18;
    public const int PARAM = 120;
    public const int INDEX_OP = 76;
    public const int ID_LETTER = 124;
    public const int HEX_DIGIT = 129;
    public const int LEFT = 33;
    public const int TRAILING = 68;
    public const int JOIN = 32;
    public const int NOT_BETWEEN = 80;
    public const int BAND = 112;
    public const int SUM = 48;
    public const int ROW_STAR = 86;
    public const int OUTER = 42;
    public const int FROM = 22;
    public const int NOT_IN = 81;
    public const int DELETE = 13;
    public const int OBJECT = 66;
    public const int MAX = 35;
    public const int QUOTED_String = 121;
    public const int EMPTY = 63;
    public const int NOT_LIKE = 82;
    public const int ASCENDING = 8;
    public const int NUM_LONG = 96;
    public const int IS = 31;
    public const int SQL_NE = 103;
    public const int IN_LIST = 75;
    public const int WEIRD_IDENT = 91;
    public const int GT = 105;
    public const int NE = 102;
    public const int MIN = 36;
    public const int LIKE = 34;
    public const int WITH = 61;
    public const int IN = 26;
    public const int CONSTRUCTOR = 71;
    public const int PROPERTY_REF = 139;
    public const int CLASS = 11;
    public const int SOME = 47;
    public const int SELECT_COLUMNS = 141;
    public const int EXPONENT = 127;
    public const int ID_START_LETTER = 123;
    public const int BOGUS = 147;
    public const int EOF = -1;
    public const int CLOSE = 101;
    public const int AVG = 9;
    public const int SELECT_CLAUSE = 135;
    public const int STAR = 115;
    public const int BXOR = 111;
    public const int NOT = 38;
    public const int JAVA_CONSTANT = 97;

    // delegates
    // delegators



        public SqlGenerator(ITreeNodeStream input)
    		: this(input, new RecognizerSharedState()) {
        }

        public SqlGenerator(ITreeNodeStream input, RecognizerSharedState state)
    		: base(input, state) {
            InitializeCyclicDFAs();

             
        }
        

    override public string[] TokenNames {
		get { return SqlGenerator.tokenNames; }
    }

    override public string GrammarFileName {
		get { return "/Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g"; }
    }



    // $ANTLR start "statement"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:27:1: statement : ( selectStatement | updateStatement | deleteStatement | insertStatement );
    public void statement() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:28:2: ( selectStatement | updateStatement | deleteStatement | insertStatement )
            int alt1 = 4;
            switch ( input.LA(1) ) 
            {
            case SELECT:
            	{
                alt1 = 1;
                }
                break;
            case UPDATE:
            	{
                alt1 = 2;
                }
                break;
            case DELETE:
            	{
                alt1 = 3;
                }
                break;
            case INSERT:
            	{
                alt1 = 4;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d1s0 =
            	        new NoViableAltException("", 1, 0, input);

            	    throw nvae_d1s0;
            }

            switch (alt1) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:28:4: selectStatement
                    {
                    	PushFollow(FOLLOW_selectStatement_in_statement57);
                    	selectStatement();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:29:4: updateStatement
                    {
                    	PushFollow(FOLLOW_updateStatement_in_statement62);
                    	updateStatement();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:30:4: deleteStatement
                    {
                    	PushFollow(FOLLOW_deleteStatement_in_statement67);
                    	deleteStatement();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 4 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:31:4: insertStatement
                    {
                    	PushFollow(FOLLOW_insertStatement_in_statement72);
                    	insertStatement();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;

            }
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "statement"


    // $ANTLR start "selectStatement"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:34:1: selectStatement : ^( SELECT selectClause from ( ^( WHERE whereExpr ) )? ( ^( GROUP groupExprs ( ^( HAVING booleanExpr[false] ) )? ) )? ( ^( ORDER orderExprs ) )? ) ;
    public void selectStatement() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:35:2: ( ^( SELECT selectClause from ( ^( WHERE whereExpr ) )? ( ^( GROUP groupExprs ( ^( HAVING booleanExpr[false] ) )? ) )? ( ^( ORDER orderExprs ) )? ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:35:4: ^( SELECT selectClause from ( ^( WHERE whereExpr ) )? ( ^( GROUP groupExprs ( ^( HAVING booleanExpr[false] ) )? ) )? ( ^( ORDER orderExprs ) )? )
            {
            	Match(input,SELECT,FOLLOW_SELECT_in_selectStatement84); if (state.failed) return ;

            	if ( (state.backtracking==0) )
            	{
            	   Out("select "); 
            	}

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_selectClause_in_selectStatement90);
            	selectClause();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	PushFollow(FOLLOW_from_in_selectStatement94);
            	from();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:38:3: ( ^( WHERE whereExpr ) )?
            	int alt2 = 2;
            	int LA2_0 = input.LA(1);

            	if ( (LA2_0 == WHERE) )
            	{
            	    alt2 = 1;
            	}
            	switch (alt2) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:38:5: ^( WHERE whereExpr )
            	        {
            	        	Match(input,WHERE,FOLLOW_WHERE_in_selectStatement101); if (state.failed) return ;

            	        	if ( (state.backtracking==0) )
            	        	{
            	        	   Out(" where "); 
            	        	}

            	        	Match(input, Token.DOWN, null); if (state.failed) return ;
            	        	PushFollow(FOLLOW_whereExpr_in_selectStatement105);
            	        	whereExpr();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        	Match(input, Token.UP, null); if (state.failed) return ;

            	        }
            	        break;

            	}

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:39:3: ( ^( GROUP groupExprs ( ^( HAVING booleanExpr[false] ) )? ) )?
            	int alt4 = 2;
            	int LA4_0 = input.LA(1);

            	if ( (LA4_0 == GROUP) )
            	{
            	    alt4 = 1;
            	}
            	switch (alt4) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:39:5: ^( GROUP groupExprs ( ^( HAVING booleanExpr[false] ) )? )
            	        {
            	        	Match(input,GROUP,FOLLOW_GROUP_in_selectStatement117); if (state.failed) return ;

            	        	if ( (state.backtracking==0) )
            	        	{
            	        	   Out(" group by "); 
            	        	}

            	        	Match(input, Token.DOWN, null); if (state.failed) return ;
            	        	PushFollow(FOLLOW_groupExprs_in_selectStatement121);
            	        	groupExprs();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;
            	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:39:47: ( ^( HAVING booleanExpr[false] ) )?
            	        	int alt3 = 2;
            	        	int LA3_0 = input.LA(1);

            	        	if ( (LA3_0 == HAVING) )
            	        	{
            	        	    alt3 = 1;
            	        	}
            	        	switch (alt3) 
            	        	{
            	        	    case 1 :
            	        	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:39:49: ^( HAVING booleanExpr[false] )
            	        	        {
            	        	        	Match(input,HAVING,FOLLOW_HAVING_in_selectStatement126); if (state.failed) return ;

            	        	        	if ( (state.backtracking==0) )
            	        	        	{
            	        	        	   Out(" having "); 
            	        	        	}

            	        	        	Match(input, Token.DOWN, null); if (state.failed) return ;
            	        	        	PushFollow(FOLLOW_booleanExpr_in_selectStatement130);
            	        	        	booleanExpr(false);
            	        	        	state.followingStackPointer--;
            	        	        	if (state.failed) return ;

            	        	        	Match(input, Token.UP, null); if (state.failed) return ;

            	        	        }
            	        	        break;

            	        	}


            	        	Match(input, Token.UP, null); if (state.failed) return ;

            	        }
            	        break;

            	}

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:40:3: ( ^( ORDER orderExprs ) )?
            	int alt5 = 2;
            	int LA5_0 = input.LA(1);

            	if ( (LA5_0 == ORDER) )
            	{
            	    alt5 = 1;
            	}
            	switch (alt5) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:40:5: ^( ORDER orderExprs )
            	        {
            	        	Match(input,ORDER,FOLLOW_ORDER_in_selectStatement147); if (state.failed) return ;

            	        	if ( (state.backtracking==0) )
            	        	{
            	        	   Out(" order by "); 
            	        	}

            	        	Match(input, Token.DOWN, null); if (state.failed) return ;
            	        	PushFollow(FOLLOW_orderExprs_in_selectStatement151);
            	        	orderExprs();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        	Match(input, Token.UP, null); if (state.failed) return ;

            	        }
            	        break;

            	}


            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "selectStatement"


    // $ANTLR start "updateStatement"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:47:1: updateStatement : ^( UPDATE ^( FROM fromTable ) setClause ( whereClause )? ) ;
    public void updateStatement() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:48:2: ( ^( UPDATE ^( FROM fromTable ) setClause ( whereClause )? ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:48:4: ^( UPDATE ^( FROM fromTable ) setClause ( whereClause )? )
            {
            	Match(input,UPDATE,FOLLOW_UPDATE_in_updateStatement174); if (state.failed) return ;

            	if ( (state.backtracking==0) )
            	{
            	   Out("update "); 
            	}

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	Match(input,FROM,FOLLOW_FROM_in_updateStatement182); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_fromTable_in_updateStatement184);
            	fromTable();
            	state.followingStackPointer--;
            	if (state.failed) return ;

            	Match(input, Token.UP, null); if (state.failed) return ;
            	PushFollow(FOLLOW_setClause_in_updateStatement190);
            	setClause();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:51:3: ( whereClause )?
            	int alt6 = 2;
            	int LA6_0 = input.LA(1);

            	if ( (LA6_0 == WHERE) )
            	{
            	    alt6 = 1;
            	}
            	switch (alt6) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:51:4: whereClause
            	        {
            	        	PushFollow(FOLLOW_whereClause_in_updateStatement195);
            	        	whereClause();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}


            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "updateStatement"


    // $ANTLR start "deleteStatement"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:55:1: deleteStatement : ^( DELETE from ( whereClause )? ) ;
    public void deleteStatement() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:57:2: ( ^( DELETE from ( whereClause )? ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:57:4: ^( DELETE from ( whereClause )? )
            {
            	Match(input,DELETE,FOLLOW_DELETE_in_deleteStatement214); if (state.failed) return ;

            	if ( (state.backtracking==0) )
            	{
            	   Out("delete"); 
            	}

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_from_in_deleteStatement220);
            	from();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:59:3: ( whereClause )?
            	int alt7 = 2;
            	int LA7_0 = input.LA(1);

            	if ( (LA7_0 == WHERE) )
            	{
            	    alt7 = 1;
            	}
            	switch (alt7) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:59:4: whereClause
            	        {
            	        	PushFollow(FOLLOW_whereClause_in_deleteStatement225);
            	        	whereClause();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}


            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "deleteStatement"


    // $ANTLR start "insertStatement"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:63:1: insertStatement : ^( INSERT ^(i= INTO ( . )* ) selectStatement ) ;
    public void insertStatement() // throws RecognitionException [1]
    {   
        IASTNode i = null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:64:2: ( ^( INSERT ^(i= INTO ( . )* ) selectStatement ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:64:4: ^( INSERT ^(i= INTO ( . )* ) selectStatement )
            {
            	Match(input,INSERT,FOLLOW_INSERT_in_insertStatement242); if (state.failed) return ;

            	if ( (state.backtracking==0) )
            	{
            	   Out( "insert " ); 
            	}

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	i=(IASTNode)Match(input,INTO,FOLLOW_INTO_in_insertStatement251); if (state.failed) return ;

            	if ( (state.backtracking==0) )
            	{
            	   Out( i ); Out( " " ); 
            	}

            	if ( input.LA(1) == Token.DOWN )
            	{
            	    Match(input, Token.DOWN, null); if (state.failed) return ;
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:65:38: ( . )*
            	    do 
            	    {
            	        int alt8 = 2;
            	        int LA8_0 = input.LA(1);

            	        if ( ((LA8_0 >= ALL && LA8_0 <= BOGUS)) )
            	        {
            	            alt8 = 1;
            	        }
            	        else if ( (LA8_0 == UP) )
            	        {
            	            alt8 = 2;
            	        }


            	        switch (alt8) 
            	    	{
            	    		case 1 :
            	    		    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:65:38: .
            	    		    {
            	    		    	MatchAny(input); if (state.failed) return ;

            	    		    }
            	    		    break;

            	    		default:
            	    		    goto loop8;
            	        }
            	    } while (true);

            	    loop8:
            	    	;	// Stops C# compiler whining that label 'loop8' has no statements


            	    Match(input, Token.UP, null); if (state.failed) return ;
            	}
            	PushFollow(FOLLOW_selectStatement_in_insertStatement261);
            	selectStatement();
            	state.followingStackPointer--;
            	if (state.failed) return ;

            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "insertStatement"


    // $ANTLR start "setClause"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:70:1: setClause : ^( SET comparisonExpr[false] ( comparisonExpr[false] )* ) ;
    public void setClause() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:73:2: ( ^( SET comparisonExpr[false] ( comparisonExpr[false] )* ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:73:4: ^( SET comparisonExpr[false] ( comparisonExpr[false] )* )
            {
            	Match(input,SET,FOLLOW_SET_in_setClause281); if (state.failed) return ;

            	if ( (state.backtracking==0) )
            	{
            	   Out(" set "); 
            	}

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_comparisonExpr_in_setClause285);
            	comparisonExpr(false);
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:73:51: ( comparisonExpr[false] )*
            	do 
            	{
            	    int alt9 = 2;
            	    int LA9_0 = input.LA(1);

            	    if ( (LA9_0 == BETWEEN || LA9_0 == EXISTS || LA9_0 == IN || LA9_0 == LIKE || (LA9_0 >= IS_NOT_NULL && LA9_0 <= IS_NULL) || (LA9_0 >= NOT_BETWEEN && LA9_0 <= NOT_LIKE) || LA9_0 == EQ || LA9_0 == NE || (LA9_0 >= LT && LA9_0 <= GE)) )
            	    {
            	        alt9 = 1;
            	    }


            	    switch (alt9) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:73:53: comparisonExpr[false]
            			    {
            			    	if ( (state.backtracking==0) )
            			    	{
            			    	   Out(", "); 
            			    	}
            			    	PushFollow(FOLLOW_comparisonExpr_in_setClause292);
            			    	comparisonExpr(false);
            			    	state.followingStackPointer--;
            			    	if (state.failed) return ;

            			    }
            			    break;

            			default:
            			    goto loop9;
            	    }
            	} while (true);

            	loop9:
            		;	// Stops C# compiler whining that label 'loop9' has no statements


            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "setClause"


    // $ANTLR start "whereClause"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:76:1: whereClause : ^( WHERE whereClauseExpr ) ;
    public void whereClause() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:77:2: ( ^( WHERE whereClauseExpr ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:77:4: ^( WHERE whereClauseExpr )
            {
            	Match(input,WHERE,FOLLOW_WHERE_in_whereClause310); if (state.failed) return ;

            	if ( (state.backtracking==0) )
            	{
            	   Out(" where "); 
            	}

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_whereClauseExpr_in_whereClause314);
            	whereClauseExpr();
            	state.followingStackPointer--;
            	if (state.failed) return ;

            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "whereClause"


    // $ANTLR start "whereClauseExpr"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:80:1: whereClauseExpr : ( ( SQL_TOKEN )=> conditionList | booleanExpr[ false ] );
    public void whereClauseExpr() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:81:2: ( ( SQL_TOKEN )=> conditionList | booleanExpr[ false ] )
            int alt10 = 2;
            int LA10_0 = input.LA(1);

            if ( (LA10_0 == SQL_TOKEN) )
            {
                int LA10_1 = input.LA(2);

                if ( (LA10_1 == DOWN) && (synpred1_SqlGenerator()) )
                {
                    alt10 = 1;
                }
                else if ( (LA10_1 == UP) )
                {
                    alt10 = 2;
                }
                else 
                {
                    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    NoViableAltException nvae_d10s1 =
                        new NoViableAltException("", 10, 1, input);

                    throw nvae_d10s1;
                }
            }
            else if ( (LA10_0 == AND || LA10_0 == BETWEEN || LA10_0 == EXISTS || LA10_0 == IN || LA10_0 == LIKE || LA10_0 == NOT || LA10_0 == OR || (LA10_0 >= IS_NOT_NULL && LA10_0 <= IS_NULL) || (LA10_0 >= NOT_BETWEEN && LA10_0 <= NOT_LIKE) || LA10_0 == EQ || LA10_0 == NE || (LA10_0 >= LT && LA10_0 <= GE)) )
            {
                alt10 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d10s0 =
                    new NoViableAltException("", 10, 0, input);

                throw nvae_d10s0;
            }
            switch (alt10) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:81:4: ( SQL_TOKEN )=> conditionList
                    {
                    	PushFollow(FOLLOW_conditionList_in_whereClauseExpr333);
                    	conditionList();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:82:4: booleanExpr[ false ]
                    {
                    	PushFollow(FOLLOW_booleanExpr_in_whereClauseExpr338);
                    	booleanExpr(false);
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;

            }
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "whereClauseExpr"


    // $ANTLR start "orderExprs"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:85:1: orderExprs : ( expr ) (dir= orderDirection )? ( orderExprs )? ;
    public void orderExprs() // throws RecognitionException [1]
    {   
        SqlGenerator.orderDirection_return dir = default(SqlGenerator.orderDirection_return);


        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:87:2: ( ( expr ) (dir= orderDirection )? ( orderExprs )? )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:87:4: ( expr ) (dir= orderDirection )? ( orderExprs )?
            {
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:87:4: ( expr )
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:87:6: expr
            	{
            		PushFollow(FOLLOW_expr_in_orderExprs354);
            		expr();
            		state.followingStackPointer--;
            		if (state.failed) return ;

            	}

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:87:13: (dir= orderDirection )?
            	int alt11 = 2;
            	int LA11_0 = input.LA(1);

            	if ( (LA11_0 == ASCENDING || LA11_0 == DESCENDING) )
            	{
            	    alt11 = 1;
            	}
            	switch (alt11) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:87:14: dir= orderDirection
            	        {
            	        	PushFollow(FOLLOW_orderDirection_in_orderExprs361);
            	        	dir = orderDirection();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;
            	        	if ( (state.backtracking==0) )
            	        	{
            	        	   Out(" "); Out(((dir != null) ? ((IASTNode)dir.Start) : null)); 
            	        	}

            	        }
            	        break;

            	}

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:87:66: ( orderExprs )?
            	int alt12 = 2;
            	int LA12_0 = input.LA(1);

            	if ( ((LA12_0 >= ALL && LA12_0 <= ANY) || LA12_0 == COUNT || LA12_0 == DOT || LA12_0 == FALSE || LA12_0 == NULL || LA12_0 == SELECT || LA12_0 == SOME || (LA12_0 >= TRUE && LA12_0 <= UNION) || LA12_0 == CASE || LA12_0 == AGGREGATE || LA12_0 == CASE2 || LA12_0 == INDEX_OP || LA12_0 == METHOD_CALL || LA12_0 == UNARY_MINUS || LA12_0 == VECTOR_EXPR || (LA12_0 >= CONSTANT && LA12_0 <= JAVA_CONSTANT) || (LA12_0 >= BNOT && LA12_0 <= DIV) || (LA12_0 >= PARAM && LA12_0 <= IDENT) || LA12_0 == ALIAS_REF || LA12_0 == SQL_TOKEN || LA12_0 == NAMED_PARAM) )
            	{
            	    alt12 = 1;
            	}
            	switch (alt12) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:87:68: orderExprs
            	        {
            	        	if ( (state.backtracking==0) )
            	        	{
            	        	  Out(", "); 
            	        	}
            	        	PushFollow(FOLLOW_orderExprs_in_orderExprs371);
            	        	orderExprs();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}


            }

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "orderExprs"


    // $ANTLR start "groupExprs"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:90:1: groupExprs : expr ( groupExprs )? ;
    public void groupExprs() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:92:2: ( expr ( groupExprs )? )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:92:4: expr ( groupExprs )?
            {
            	PushFollow(FOLLOW_expr_in_groupExprs386);
            	expr();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:92:9: ( groupExprs )?
            	int alt13 = 2;
            	int LA13_0 = input.LA(1);

            	if ( ((LA13_0 >= ALL && LA13_0 <= ANY) || LA13_0 == COUNT || LA13_0 == DOT || LA13_0 == FALSE || LA13_0 == NULL || LA13_0 == SELECT || LA13_0 == SOME || (LA13_0 >= TRUE && LA13_0 <= UNION) || LA13_0 == CASE || LA13_0 == AGGREGATE || LA13_0 == CASE2 || LA13_0 == INDEX_OP || LA13_0 == METHOD_CALL || LA13_0 == UNARY_MINUS || LA13_0 == VECTOR_EXPR || (LA13_0 >= CONSTANT && LA13_0 <= JAVA_CONSTANT) || (LA13_0 >= BNOT && LA13_0 <= DIV) || (LA13_0 >= PARAM && LA13_0 <= IDENT) || LA13_0 == ALIAS_REF || LA13_0 == SQL_TOKEN || LA13_0 == NAMED_PARAM) )
            	{
            	    alt13 = 1;
            	}
            	switch (alt13) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:92:11: groupExprs
            	        {
            	        	if ( (state.backtracking==0) )
            	        	{
            	        	  Out(" , "); 
            	        	}
            	        	PushFollow(FOLLOW_groupExprs_in_groupExprs392);
            	        	groupExprs();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}


            }

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "groupExprs"

    public class orderDirection_return : TreeRuleReturnScope
    {
    };

    // $ANTLR start "orderDirection"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:95:1: orderDirection : ( ASCENDING | DESCENDING );
    public SqlGenerator.orderDirection_return orderDirection() // throws RecognitionException [1]
    {   
        SqlGenerator.orderDirection_return retval = new SqlGenerator.orderDirection_return();
        retval.Start = input.LT(1);

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:96:2: ( ASCENDING | DESCENDING )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:
            {
            	if ( input.LA(1) == ASCENDING || input.LA(1) == DESCENDING ) 
            	{
            	    input.Consume();
            	    state.errorRecovery = false;state.failed = false;
            	}
            	else 
            	{
            	    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
            	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	    throw mse;
            	}


            }

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "orderDirection"


    // $ANTLR start "whereExpr"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:100:1: whereExpr : ( filters ( thetaJoins )? ( booleanExpr[ true ] )? | thetaJoins ( booleanExpr[ true ] )? | booleanExpr[false] );
    public void whereExpr() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:104:2: ( filters ( thetaJoins )? ( booleanExpr[ true ] )? | thetaJoins ( booleanExpr[ true ] )? | booleanExpr[false] )
            int alt17 = 3;
            switch ( input.LA(1) ) 
            {
            case FILTERS:
            	{
                alt17 = 1;
                }
                break;
            case THETA_JOINS:
            	{
                alt17 = 2;
                }
                break;
            case AND:
            case BETWEEN:
            case EXISTS:
            case IN:
            case LIKE:
            case NOT:
            case OR:
            case IS_NOT_NULL:
            case IS_NULL:
            case NOT_BETWEEN:
            case NOT_IN:
            case NOT_LIKE:
            case EQ:
            case NE:
            case LT:
            case GT:
            case LE:
            case GE:
            case SQL_TOKEN:
            	{
                alt17 = 3;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d17s0 =
            	        new NoViableAltException("", 17, 0, input);

            	    throw nvae_d17s0;
            }

            switch (alt17) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:104:4: filters ( thetaJoins )? ( booleanExpr[ true ] )?
                    {
                    	PushFollow(FOLLOW_filters_in_whereExpr427);
                    	filters();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:105:3: ( thetaJoins )?
                    	int alt14 = 2;
                    	int LA14_0 = input.LA(1);

                    	if ( (LA14_0 == THETA_JOINS) )
                    	{
                    	    alt14 = 1;
                    	}
                    	switch (alt14) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:105:5: thetaJoins
                    	        {
                    	        	if ( (state.backtracking==0) )
                    	        	{
                    	        	   Out(" and "); 
                    	        	}
                    	        	PushFollow(FOLLOW_thetaJoins_in_whereExpr435);
                    	        	thetaJoins();
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;

                    	}

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:106:3: ( booleanExpr[ true ] )?
                    	int alt15 = 2;
                    	int LA15_0 = input.LA(1);

                    	if ( (LA15_0 == AND || LA15_0 == BETWEEN || LA15_0 == EXISTS || LA15_0 == IN || LA15_0 == LIKE || LA15_0 == NOT || LA15_0 == OR || (LA15_0 >= IS_NOT_NULL && LA15_0 <= IS_NULL) || (LA15_0 >= NOT_BETWEEN && LA15_0 <= NOT_LIKE) || LA15_0 == EQ || LA15_0 == NE || (LA15_0 >= LT && LA15_0 <= GE) || LA15_0 == SQL_TOKEN) )
                    	{
                    	    alt15 = 1;
                    	}
                    	switch (alt15) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:106:5: booleanExpr[ true ]
                    	        {
                    	        	if ( (state.backtracking==0) )
                    	        	{
                    	        	   Out(" and "); 
                    	        	}
                    	        	PushFollow(FOLLOW_booleanExpr_in_whereExpr446);
                    	        	booleanExpr(true);
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;

                    	}


                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:107:4: thetaJoins ( booleanExpr[ true ] )?
                    {
                    	PushFollow(FOLLOW_thetaJoins_in_whereExpr456);
                    	thetaJoins();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:108:3: ( booleanExpr[ true ] )?
                    	int alt16 = 2;
                    	int LA16_0 = input.LA(1);

                    	if ( (LA16_0 == AND || LA16_0 == BETWEEN || LA16_0 == EXISTS || LA16_0 == IN || LA16_0 == LIKE || LA16_0 == NOT || LA16_0 == OR || (LA16_0 >= IS_NOT_NULL && LA16_0 <= IS_NULL) || (LA16_0 >= NOT_BETWEEN && LA16_0 <= NOT_LIKE) || LA16_0 == EQ || LA16_0 == NE || (LA16_0 >= LT && LA16_0 <= GE) || LA16_0 == SQL_TOKEN) )
                    	{
                    	    alt16 = 1;
                    	}
                    	switch (alt16) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:108:5: booleanExpr[ true ]
                    	        {
                    	        	if ( (state.backtracking==0) )
                    	        	{
                    	        	   Out(" and "); 
                    	        	}
                    	        	PushFollow(FOLLOW_booleanExpr_in_whereExpr464);
                    	        	booleanExpr(true);
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;

                    	}


                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:109:4: booleanExpr[false]
                    {
                    	PushFollow(FOLLOW_booleanExpr_in_whereExpr475);
                    	booleanExpr(false);
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;

            }
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "whereExpr"


    // $ANTLR start "filters"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:112:1: filters : ^( FILTERS conditionList ) ;
    public void filters() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:113:2: ( ^( FILTERS conditionList ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:113:4: ^( FILTERS conditionList )
            {
            	Match(input,FILTERS,FOLLOW_FILTERS_in_filters488); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_conditionList_in_filters490);
            	conditionList();
            	state.followingStackPointer--;
            	if (state.failed) return ;

            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "filters"


    // $ANTLR start "thetaJoins"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:116:1: thetaJoins : ^( THETA_JOINS conditionList ) ;
    public void thetaJoins() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:117:2: ( ^( THETA_JOINS conditionList ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:117:4: ^( THETA_JOINS conditionList )
            {
            	Match(input,THETA_JOINS,FOLLOW_THETA_JOINS_in_thetaJoins504); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_conditionList_in_thetaJoins506);
            	conditionList();
            	state.followingStackPointer--;
            	if (state.failed) return ;

            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "thetaJoins"


    // $ANTLR start "conditionList"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:120:1: conditionList : sqlToken ( conditionList )? ;
    public void conditionList() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:121:2: ( sqlToken ( conditionList )? )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:121:4: sqlToken ( conditionList )?
            {
            	PushFollow(FOLLOW_sqlToken_in_conditionList519);
            	sqlToken();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:121:13: ( conditionList )?
            	int alt18 = 2;
            	int LA18_0 = input.LA(1);

            	if ( (LA18_0 == SQL_TOKEN) )
            	{
            	    alt18 = 1;
            	}
            	switch (alt18) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:121:15: conditionList
            	        {
            	        	if ( (state.backtracking==0) )
            	        	{
            	        	   Out(" and "); 
            	        	}
            	        	PushFollow(FOLLOW_conditionList_in_conditionList525);
            	        	conditionList();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}


            }

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "conditionList"


    // $ANTLR start "selectClause"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:124:1: selectClause : ^( SELECT_CLAUSE ( distinctOrAll )? ( selectColumn )+ ) ;
    public void selectClause() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:125:2: ( ^( SELECT_CLAUSE ( distinctOrAll )? ( selectColumn )+ ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:125:4: ^( SELECT_CLAUSE ( distinctOrAll )? ( selectColumn )+ )
            {
            	Match(input,SELECT_CLAUSE,FOLLOW_SELECT_CLAUSE_in_selectClause540); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:125:20: ( distinctOrAll )?
            	int alt19 = 2;
            	int LA19_0 = input.LA(1);

            	if ( (LA19_0 == ALL || LA19_0 == DISTINCT) )
            	{
            	    alt19 = 1;
            	}
            	switch (alt19) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:125:21: distinctOrAll
            	        {
            	        	PushFollow(FOLLOW_distinctOrAll_in_selectClause543);
            	        	distinctOrAll();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:125:37: ( selectColumn )+
            	int cnt20 = 0;
            	do 
            	{
            	    int alt20 = 2;
            	    int LA20_0 = input.LA(1);

            	    if ( (LA20_0 == COUNT || LA20_0 == DOT || LA20_0 == FALSE || LA20_0 == SELECT || LA20_0 == TRUE || LA20_0 == CASE || LA20_0 == AGGREGATE || (LA20_0 >= CONSTRUCTOR && LA20_0 <= CASE2) || LA20_0 == METHOD_CALL || LA20_0 == UNARY_MINUS || (LA20_0 >= CONSTANT && LA20_0 <= JAVA_CONSTANT) || (LA20_0 >= BNOT && LA20_0 <= DIV) || (LA20_0 >= PARAM && LA20_0 <= IDENT) || LA20_0 == ALIAS_REF || LA20_0 == SQL_TOKEN || LA20_0 == SELECT_EXPR) )
            	    {
            	        alt20 = 1;
            	    }


            	    switch (alt20) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:125:39: selectColumn
            			    {
            			    	PushFollow(FOLLOW_selectColumn_in_selectClause549);
            			    	selectColumn();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return ;

            			    }
            			    break;

            			default:
            			    if ( cnt20 >= 1 ) goto loop20;
            			    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            		            EarlyExitException eee20 =
            		                new EarlyExitException(20, input);
            		            throw eee20;
            	    }
            	    cnt20++;
            	} while (true);

            	loop20:
            		;	// Stops C# compiler whinging that label 'loop20' has no statements


            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "selectClause"


    // $ANTLR start "selectColumn"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:128:1: selectColumn : p= selectExpr (sc= SELECT_COLUMNS )? ;
    public void selectColumn() // throws RecognitionException [1]
    {   
        IASTNode sc = null;
        SqlGenerator.selectExpr_return p = default(SqlGenerator.selectExpr_return);


        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:129:2: (p= selectExpr (sc= SELECT_COLUMNS )? )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:129:4: p= selectExpr (sc= SELECT_COLUMNS )?
            {
            	PushFollow(FOLLOW_selectExpr_in_selectColumn567);
            	p = selectExpr();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:129:17: (sc= SELECT_COLUMNS )?
            	int alt21 = 2;
            	int LA21_0 = input.LA(1);

            	if ( (LA21_0 == SELECT_COLUMNS) )
            	{
            	    alt21 = 1;
            	}
            	switch (alt21) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:129:18: sc= SELECT_COLUMNS
            	        {
            	        	sc=(IASTNode)Match(input,SELECT_COLUMNS,FOLLOW_SELECT_COLUMNS_in_selectColumn572); if (state.failed) return ;
            	        	if ( (state.backtracking==0) )
            	        	{
            	        	   Out(sc); 
            	        	}

            	        }
            	        break;

            	}

            	if ( (state.backtracking==0) )
            	{
            	   Separator( (sc != null) ? sc : ((p != null) ? ((IASTNode)p.Start) : null) ,", "); 
            	}

            }

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "selectColumn"

    public class selectExpr_return : TreeRuleReturnScope
    {
    };

    // $ANTLR start "selectExpr"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:132:1: selectExpr : (e= selectAtom | count | ^( CONSTRUCTOR ( DOT | IDENT ) ( selectColumn )+ ) | methodCall | aggregate | c= constant | arithmeticExpr | param= PARAM | selectStatement );
    public SqlGenerator.selectExpr_return selectExpr() // throws RecognitionException [1]
    {   
        SqlGenerator.selectExpr_return retval = new SqlGenerator.selectExpr_return();
        retval.Start = input.LT(1);

        IASTNode param = null;
        SqlGenerator.selectAtom_return e = default(SqlGenerator.selectAtom_return);

        SqlGenerator.constant_return c = default(SqlGenerator.constant_return);


        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:133:2: (e= selectAtom | count | ^( CONSTRUCTOR ( DOT | IDENT ) ( selectColumn )+ ) | methodCall | aggregate | c= constant | arithmeticExpr | param= PARAM | selectStatement )
            int alt23 = 9;
            switch ( input.LA(1) ) 
            {
            case DOT:
            case ALIAS_REF:
            case SQL_TOKEN:
            case SELECT_EXPR:
            	{
                alt23 = 1;
                }
                break;
            case COUNT:
            	{
                alt23 = 2;
                }
                break;
            case CONSTRUCTOR:
            	{
                alt23 = 3;
                }
                break;
            case METHOD_CALL:
            	{
                alt23 = 4;
                }
                break;
            case AGGREGATE:
            	{
                alt23 = 5;
                }
                break;
            case FALSE:
            case TRUE:
            case CONSTANT:
            case NUM_INT:
            case NUM_DOUBLE:
            case NUM_FLOAT:
            case NUM_LONG:
            case JAVA_CONSTANT:
            case QUOTED_String:
            case IDENT:
            	{
                alt23 = 6;
                }
                break;
            case CASE:
            case CASE2:
            case UNARY_MINUS:
            case BNOT:
            case BOR:
            case BXOR:
            case BAND:
            case PLUS:
            case MINUS:
            case STAR:
            case DIV:
            	{
                alt23 = 7;
                }
                break;
            case PARAM:
            	{
                alt23 = 8;
                }
                break;
            case SELECT:
            	{
                alt23 = 9;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
            	    NoViableAltException nvae_d23s0 =
            	        new NoViableAltException("", 23, 0, input);

            	    throw nvae_d23s0;
            }

            switch (alt23) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:133:4: e= selectAtom
                    {
                    	PushFollow(FOLLOW_selectAtom_in_selectExpr592);
                    	e = selectAtom();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(((e != null) ? ((IASTNode)e.Start) : null)); 
                    	}

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:134:4: count
                    {
                    	PushFollow(FOLLOW_count_in_selectExpr599);
                    	count();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:135:4: ^( CONSTRUCTOR ( DOT | IDENT ) ( selectColumn )+ )
                    {
                    	Match(input,CONSTRUCTOR,FOLLOW_CONSTRUCTOR_in_selectExpr605); if (state.failed) return retval;

                    	Match(input, Token.DOWN, null); if (state.failed) return retval;
                    	if ( input.LA(1) == DOT || input.LA(1) == IDENT ) 
                    	{
                    	    input.Consume();
                    	    state.errorRecovery = false;state.failed = false;
                    	}
                    	else 
                    	{
                    	    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
                    	    MismatchedSetException mse = new MismatchedSetException(null,input);
                    	    throw mse;
                    	}

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:135:32: ( selectColumn )+
                    	int cnt22 = 0;
                    	do 
                    	{
                    	    int alt22 = 2;
                    	    int LA22_0 = input.LA(1);

                    	    if ( (LA22_0 == COUNT || LA22_0 == DOT || LA22_0 == FALSE || LA22_0 == SELECT || LA22_0 == TRUE || LA22_0 == CASE || LA22_0 == AGGREGATE || (LA22_0 >= CONSTRUCTOR && LA22_0 <= CASE2) || LA22_0 == METHOD_CALL || LA22_0 == UNARY_MINUS || (LA22_0 >= CONSTANT && LA22_0 <= JAVA_CONSTANT) || (LA22_0 >= BNOT && LA22_0 <= DIV) || (LA22_0 >= PARAM && LA22_0 <= IDENT) || LA22_0 == ALIAS_REF || LA22_0 == SQL_TOKEN || LA22_0 == SELECT_EXPR) )
                    	    {
                    	        alt22 = 1;
                    	    }


                    	    switch (alt22) 
                    		{
                    			case 1 :
                    			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:135:34: selectColumn
                    			    {
                    			    	PushFollow(FOLLOW_selectColumn_in_selectExpr617);
                    			    	selectColumn();
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return retval;

                    			    }
                    			    break;

                    			default:
                    			    if ( cnt22 >= 1 ) goto loop22;
                    			    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
                    		            EarlyExitException eee22 =
                    		                new EarlyExitException(22, input);
                    		            throw eee22;
                    	    }
                    	    cnt22++;
                    	} while (true);

                    	loop22:
                    		;	// Stops C# compiler whinging that label 'loop22' has no statements


                    	Match(input, Token.UP, null); if (state.failed) return retval;

                    }
                    break;
                case 4 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:136:4: methodCall
                    {
                    	PushFollow(FOLLOW_methodCall_in_selectExpr627);
                    	methodCall();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 5 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:137:4: aggregate
                    {
                    	PushFollow(FOLLOW_aggregate_in_selectExpr632);
                    	aggregate();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 6 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:138:4: c= constant
                    {
                    	PushFollow(FOLLOW_constant_in_selectExpr639);
                    	c = constant();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(((c != null) ? ((IASTNode)c.Start) : null)); 
                    	}

                    }
                    break;
                case 7 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:139:4: arithmeticExpr
                    {
                    	PushFollow(FOLLOW_arithmeticExpr_in_selectExpr646);
                    	arithmeticExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 8 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:140:4: param= PARAM
                    {
                    	param=(IASTNode)Match(input,PARAM,FOLLOW_PARAM_in_selectExpr653); if (state.failed) return retval;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(param); 
                    	}

                    }
                    break;
                case 9 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:142:4: selectStatement
                    {
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("("); 
                    	}
                    	PushFollow(FOLLOW_selectStatement_in_selectExpr663);
                    	selectStatement();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(")"); 
                    	}

                    }
                    break;

            }
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "selectExpr"


    // $ANTLR start "count"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:145:1: count : ^( COUNT ( distinctOrAll )? countExpr ) ;
    public void count() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:146:2: ( ^( COUNT ( distinctOrAll )? countExpr ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:146:4: ^( COUNT ( distinctOrAll )? countExpr )
            {
            	Match(input,COUNT,FOLLOW_COUNT_in_count677); if (state.failed) return ;

            	if ( (state.backtracking==0) )
            	{
            	   Out("count("); 
            	}

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:146:32: ( distinctOrAll )?
            	int alt24 = 2;
            	int LA24_0 = input.LA(1);

            	if ( (LA24_0 == ALL || LA24_0 == DISTINCT) )
            	{
            	    alt24 = 1;
            	}
            	switch (alt24) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:146:34: distinctOrAll
            	        {
            	        	PushFollow(FOLLOW_distinctOrAll_in_count684);
            	        	distinctOrAll();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_countExpr_in_count690);
            	countExpr();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	if ( (state.backtracking==0) )
            	{
            	   Out(")"); 
            	}

            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "count"


    // $ANTLR start "distinctOrAll"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:149:1: distinctOrAll : ( DISTINCT | ^( ALL ( . )* ) );
    public void distinctOrAll() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:150:2: ( DISTINCT | ^( ALL ( . )* ) )
            int alt26 = 2;
            int LA26_0 = input.LA(1);

            if ( (LA26_0 == DISTINCT) )
            {
                alt26 = 1;
            }
            else if ( (LA26_0 == ALL) )
            {
                alt26 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d26s0 =
                    new NoViableAltException("", 26, 0, input);

                throw nvae_d26s0;
            }
            switch (alt26) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:150:4: DISTINCT
                    {
                    	Match(input,DISTINCT,FOLLOW_DISTINCT_in_distinctOrAll705); if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("distinct "); 
                    	}

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:151:4: ^( ALL ( . )* )
                    {
                    	Match(input,ALL,FOLLOW_ALL_in_distinctOrAll713); if (state.failed) return ;

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return ;
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:151:10: ( . )*
                    	    do 
                    	    {
                    	        int alt25 = 2;
                    	        int LA25_0 = input.LA(1);

                    	        if ( ((LA25_0 >= ALL && LA25_0 <= BOGUS)) )
                    	        {
                    	            alt25 = 1;
                    	        }
                    	        else if ( (LA25_0 == UP) )
                    	        {
                    	            alt25 = 2;
                    	        }


                    	        switch (alt25) 
                    	    	{
                    	    		case 1 :
                    	    		    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:151:10: .
                    	    		    {
                    	    		    	MatchAny(input); if (state.failed) return ;

                    	    		    }
                    	    		    break;

                    	    		default:
                    	    		    goto loop25;
                    	        }
                    	    } while (true);

                    	    loop25:
                    	    	;	// Stops C# compiler whining that label 'loop25' has no statements


                    	    Match(input, Token.UP, null); if (state.failed) return ;
                    	}
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("all "); 
                    	}

                    }
                    break;

            }
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "distinctOrAll"


    // $ANTLR start "countExpr"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:154:1: countExpr : ( ROW_STAR | simpleExpr );
    public void countExpr() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:156:2: ( ROW_STAR | simpleExpr )
            int alt27 = 2;
            int LA27_0 = input.LA(1);

            if ( (LA27_0 == ROW_STAR) )
            {
                alt27 = 1;
            }
            else if ( (LA27_0 == COUNT || LA27_0 == DOT || LA27_0 == FALSE || LA27_0 == NULL || LA27_0 == TRUE || LA27_0 == CASE || LA27_0 == AGGREGATE || LA27_0 == CASE2 || LA27_0 == INDEX_OP || LA27_0 == METHOD_CALL || LA27_0 == UNARY_MINUS || (LA27_0 >= CONSTANT && LA27_0 <= JAVA_CONSTANT) || (LA27_0 >= BNOT && LA27_0 <= DIV) || (LA27_0 >= PARAM && LA27_0 <= IDENT) || LA27_0 == ALIAS_REF || LA27_0 == SQL_TOKEN || LA27_0 == NAMED_PARAM) )
            {
                alt27 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d27s0 =
                    new NoViableAltException("", 27, 0, input);

                throw nvae_d27s0;
            }
            switch (alt27) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:156:4: ROW_STAR
                    {
                    	Match(input,ROW_STAR,FOLLOW_ROW_STAR_in_countExpr732); if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("*"); 
                    	}

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:157:4: simpleExpr
                    {
                    	PushFollow(FOLLOW_simpleExpr_in_countExpr739);
                    	simpleExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;

            }
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "countExpr"

    public class selectAtom_return : TreeRuleReturnScope
    {
    };

    // $ANTLR start "selectAtom"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:160:1: selectAtom : ( ^( DOT ( . )* ) | ^( SQL_TOKEN ( . )* ) | ^( ALIAS_REF ( . )* ) | ^( SELECT_EXPR ( . )* ) );
    public SqlGenerator.selectAtom_return selectAtom() // throws RecognitionException [1]
    {   
        SqlGenerator.selectAtom_return retval = new SqlGenerator.selectAtom_return();
        retval.Start = input.LT(1);

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:161:2: ( ^( DOT ( . )* ) | ^( SQL_TOKEN ( . )* ) | ^( ALIAS_REF ( . )* ) | ^( SELECT_EXPR ( . )* ) )
            int alt32 = 4;
            switch ( input.LA(1) ) 
            {
            case DOT:
            	{
                alt32 = 1;
                }
                break;
            case SQL_TOKEN:
            	{
                alt32 = 2;
                }
                break;
            case ALIAS_REF:
            	{
                alt32 = 3;
                }
                break;
            case SELECT_EXPR:
            	{
                alt32 = 4;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
            	    NoViableAltException nvae_d32s0 =
            	        new NoViableAltException("", 32, 0, input);

            	    throw nvae_d32s0;
            }

            switch (alt32) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:161:4: ^( DOT ( . )* )
                    {
                    	Match(input,DOT,FOLLOW_DOT_in_selectAtom751); if (state.failed) return retval;

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return retval;
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:161:10: ( . )*
                    	    do 
                    	    {
                    	        int alt28 = 2;
                    	        int LA28_0 = input.LA(1);

                    	        if ( ((LA28_0 >= ALL && LA28_0 <= BOGUS)) )
                    	        {
                    	            alt28 = 1;
                    	        }
                    	        else if ( (LA28_0 == UP) )
                    	        {
                    	            alt28 = 2;
                    	        }


                    	        switch (alt28) 
                    	    	{
                    	    		case 1 :
                    	    		    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:161:10: .
                    	    		    {
                    	    		    	MatchAny(input); if (state.failed) return retval;

                    	    		    }
                    	    		    break;

                    	    		default:
                    	    		    goto loop28;
                    	        }
                    	    } while (true);

                    	    loop28:
                    	    	;	// Stops C# compiler whining that label 'loop28' has no statements


                    	    Match(input, Token.UP, null); if (state.failed) return retval;
                    	}

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:162:4: ^( SQL_TOKEN ( . )* )
                    {
                    	Match(input,SQL_TOKEN,FOLLOW_SQL_TOKEN_in_selectAtom761); if (state.failed) return retval;

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return retval;
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:162:16: ( . )*
                    	    do 
                    	    {
                    	        int alt29 = 2;
                    	        int LA29_0 = input.LA(1);

                    	        if ( ((LA29_0 >= ALL && LA29_0 <= BOGUS)) )
                    	        {
                    	            alt29 = 1;
                    	        }
                    	        else if ( (LA29_0 == UP) )
                    	        {
                    	            alt29 = 2;
                    	        }


                    	        switch (alt29) 
                    	    	{
                    	    		case 1 :
                    	    		    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:162:16: .
                    	    		    {
                    	    		    	MatchAny(input); if (state.failed) return retval;

                    	    		    }
                    	    		    break;

                    	    		default:
                    	    		    goto loop29;
                    	        }
                    	    } while (true);

                    	    loop29:
                    	    	;	// Stops C# compiler whining that label 'loop29' has no statements


                    	    Match(input, Token.UP, null); if (state.failed) return retval;
                    	}

                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:163:4: ^( ALIAS_REF ( . )* )
                    {
                    	Match(input,ALIAS_REF,FOLLOW_ALIAS_REF_in_selectAtom771); if (state.failed) return retval;

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return retval;
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:163:16: ( . )*
                    	    do 
                    	    {
                    	        int alt30 = 2;
                    	        int LA30_0 = input.LA(1);

                    	        if ( ((LA30_0 >= ALL && LA30_0 <= BOGUS)) )
                    	        {
                    	            alt30 = 1;
                    	        }
                    	        else if ( (LA30_0 == UP) )
                    	        {
                    	            alt30 = 2;
                    	        }


                    	        switch (alt30) 
                    	    	{
                    	    		case 1 :
                    	    		    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:163:16: .
                    	    		    {
                    	    		    	MatchAny(input); if (state.failed) return retval;

                    	    		    }
                    	    		    break;

                    	    		default:
                    	    		    goto loop30;
                    	        }
                    	    } while (true);

                    	    loop30:
                    	    	;	// Stops C# compiler whining that label 'loop30' has no statements


                    	    Match(input, Token.UP, null); if (state.failed) return retval;
                    	}

                    }
                    break;
                case 4 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:164:4: ^( SELECT_EXPR ( . )* )
                    {
                    	Match(input,SELECT_EXPR,FOLLOW_SELECT_EXPR_in_selectAtom781); if (state.failed) return retval;

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return retval;
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:164:18: ( . )*
                    	    do 
                    	    {
                    	        int alt31 = 2;
                    	        int LA31_0 = input.LA(1);

                    	        if ( ((LA31_0 >= ALL && LA31_0 <= BOGUS)) )
                    	        {
                    	            alt31 = 1;
                    	        }
                    	        else if ( (LA31_0 == UP) )
                    	        {
                    	            alt31 = 2;
                    	        }


                    	        switch (alt31) 
                    	    	{
                    	    		case 1 :
                    	    		    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:164:18: .
                    	    		    {
                    	    		    	MatchAny(input); if (state.failed) return retval;

                    	    		    }
                    	    		    break;

                    	    		default:
                    	    		    goto loop31;
                    	        }
                    	    } while (true);

                    	    loop31:
                    	    	;	// Stops C# compiler whining that label 'loop31' has no statements


                    	    Match(input, Token.UP, null); if (state.failed) return retval;
                    	}

                    }
                    break;

            }
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "selectAtom"


    // $ANTLR start "from"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:172:1: from : ^(f= FROM ( fromTable )* ) ;
    public void from() // throws RecognitionException [1]
    {   
        IASTNode f = null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:173:2: ( ^(f= FROM ( fromTable )* ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:173:4: ^(f= FROM ( fromTable )* )
            {
            	f=(IASTNode)Match(input,FROM,FOLLOW_FROM_in_from804); if (state.failed) return ;

            	if ( (state.backtracking==0) )
            	{
            	   Out(" from "); 
            	}

            	if ( input.LA(1) == Token.DOWN )
            	{
            	    Match(input, Token.DOWN, null); if (state.failed) return ;
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:174:3: ( fromTable )*
            	    do 
            	    {
            	        int alt33 = 2;
            	        int LA33_0 = input.LA(1);

            	        if ( (LA33_0 == FROM_FRAGMENT || LA33_0 == JOIN_FRAGMENT) )
            	        {
            	            alt33 = 1;
            	        }


            	        switch (alt33) 
            	    	{
            	    		case 1 :
            	    		    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:174:4: fromTable
            	    		    {
            	    		    	PushFollow(FOLLOW_fromTable_in_from811);
            	    		    	fromTable();
            	    		    	state.followingStackPointer--;
            	    		    	if (state.failed) return ;

            	    		    }
            	    		    break;

            	    		default:
            	    		    goto loop33;
            	        }
            	    } while (true);

            	    loop33:
            	    	;	// Stops C# compiler whining that label 'loop33' has no statements


            	    Match(input, Token.UP, null); if (state.failed) return ;
            	}

            }

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "from"


    // $ANTLR start "fromTable"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:177:1: fromTable : ( ^(a= FROM_FRAGMENT ( tableJoin[ a ] )* ) | ^(a= JOIN_FRAGMENT ( tableJoin[ a ] )* ) );
    public void fromTable() // throws RecognitionException [1]
    {   
        IASTNode a = null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:182:2: ( ^(a= FROM_FRAGMENT ( tableJoin[ a ] )* ) | ^(a= JOIN_FRAGMENT ( tableJoin[ a ] )* ) )
            int alt36 = 2;
            int LA36_0 = input.LA(1);

            if ( (LA36_0 == FROM_FRAGMENT) )
            {
                alt36 = 1;
            }
            else if ( (LA36_0 == JOIN_FRAGMENT) )
            {
                alt36 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d36s0 =
                    new NoViableAltException("", 36, 0, input);

                throw nvae_d36s0;
            }
            switch (alt36) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:182:4: ^(a= FROM_FRAGMENT ( tableJoin[ a ] )* )
                    {
                    	a=(IASTNode)Match(input,FROM_FRAGMENT,FOLLOW_FROM_FRAGMENT_in_fromTable837); if (state.failed) return ;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(a); 
                    	}

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return ;
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:182:36: ( tableJoin[ a ] )*
                    	    do 
                    	    {
                    	        int alt34 = 2;
                    	        int LA34_0 = input.LA(1);

                    	        if ( (LA34_0 == FROM_FRAGMENT || LA34_0 == JOIN_FRAGMENT) )
                    	        {
                    	            alt34 = 1;
                    	        }


                    	        switch (alt34) 
                    	    	{
                    	    		case 1 :
                    	    		    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:182:37: tableJoin[ a ]
                    	    		    {
                    	    		    	PushFollow(FOLLOW_tableJoin_in_fromTable843);
                    	    		    	tableJoin(a);
                    	    		    	state.followingStackPointer--;
                    	    		    	if (state.failed) return ;

                    	    		    }
                    	    		    break;

                    	    		default:
                    	    		    goto loop34;
                    	        }
                    	    } while (true);

                    	    loop34:
                    	    	;	// Stops C# compiler whining that label 'loop34' has no statements


                    	    Match(input, Token.UP, null); if (state.failed) return ;
                    	}

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:183:4: ^(a= JOIN_FRAGMENT ( tableJoin[ a ] )* )
                    {
                    	a=(IASTNode)Match(input,JOIN_FRAGMENT,FOLLOW_JOIN_FRAGMENT_in_fromTable858); if (state.failed) return ;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(a); 
                    	}

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return ;
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:183:36: ( tableJoin[ a ] )*
                    	    do 
                    	    {
                    	        int alt35 = 2;
                    	        int LA35_0 = input.LA(1);

                    	        if ( (LA35_0 == FROM_FRAGMENT || LA35_0 == JOIN_FRAGMENT) )
                    	        {
                    	            alt35 = 1;
                    	        }


                    	        switch (alt35) 
                    	    	{
                    	    		case 1 :
                    	    		    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:183:37: tableJoin[ a ]
                    	    		    {
                    	    		    	PushFollow(FOLLOW_tableJoin_in_fromTable864);
                    	    		    	tableJoin(a);
                    	    		    	state.followingStackPointer--;
                    	    		    	if (state.failed) return ;

                    	    		    }
                    	    		    break;

                    	    		default:
                    	    		    goto loop35;
                    	        }
                    	    } while (true);

                    	    loop35:
                    	    	;	// Stops C# compiler whining that label 'loop35' has no statements


                    	    Match(input, Token.UP, null); if (state.failed) return ;
                    	}

                    }
                    break;

            }
            if ( (state.backtracking==0) )
            {

                 FromFragmentSeparator(a);

            }
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "fromTable"


    // $ANTLR start "tableJoin"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:186:1: tableJoin[ IASTNode parent ] : ( ^(c= JOIN_FRAGMENT ( tableJoin[ c ] )* ) | ^(d= FROM_FRAGMENT ( tableJoin[ d ] )* ) );
    public void tableJoin(IASTNode parent) // throws RecognitionException [1]
    {   
        IASTNode c = null;
        IASTNode d = null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:187:2: ( ^(c= JOIN_FRAGMENT ( tableJoin[ c ] )* ) | ^(d= FROM_FRAGMENT ( tableJoin[ d ] )* ) )
            int alt39 = 2;
            int LA39_0 = input.LA(1);

            if ( (LA39_0 == JOIN_FRAGMENT) )
            {
                alt39 = 1;
            }
            else if ( (LA39_0 == FROM_FRAGMENT) )
            {
                alt39 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d39s0 =
                    new NoViableAltException("", 39, 0, input);

                throw nvae_d39s0;
            }
            switch (alt39) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:187:4: ^(c= JOIN_FRAGMENT ( tableJoin[ c ] )* )
                    {
                    	c=(IASTNode)Match(input,JOIN_FRAGMENT,FOLLOW_JOIN_FRAGMENT_in_tableJoin887); if (state.failed) return ;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" "); Out(c); 
                    	}

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return ;
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:187:46: ( tableJoin[ c ] )*
                    	    do 
                    	    {
                    	        int alt37 = 2;
                    	        int LA37_0 = input.LA(1);

                    	        if ( (LA37_0 == FROM_FRAGMENT || LA37_0 == JOIN_FRAGMENT) )
                    	        {
                    	            alt37 = 1;
                    	        }


                    	        switch (alt37) 
                    	    	{
                    	    		case 1 :
                    	    		    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:187:47: tableJoin[ c ]
                    	    		    {
                    	    		    	PushFollow(FOLLOW_tableJoin_in_tableJoin892);
                    	    		    	tableJoin(c);
                    	    		    	state.followingStackPointer--;
                    	    		    	if (state.failed) return ;

                    	    		    }
                    	    		    break;

                    	    		default:
                    	    		    goto loop37;
                    	        }
                    	    } while (true);

                    	    loop37:
                    	    	;	// Stops C# compiler whining that label 'loop37' has no statements


                    	    Match(input, Token.UP, null); if (state.failed) return ;
                    	}

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:188:4: ^(d= FROM_FRAGMENT ( tableJoin[ d ] )* )
                    {
                    	d=(IASTNode)Match(input,FROM_FRAGMENT,FOLLOW_FROM_FRAGMENT_in_tableJoin908); if (state.failed) return ;

                    	if ( (state.backtracking==0) )
                    	{
                    	   NestedFromFragment(d,parent); 
                    	}

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return ;
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:188:58: ( tableJoin[ d ] )*
                    	    do 
                    	    {
                    	        int alt38 = 2;
                    	        int LA38_0 = input.LA(1);

                    	        if ( (LA38_0 == FROM_FRAGMENT || LA38_0 == JOIN_FRAGMENT) )
                    	        {
                    	            alt38 = 1;
                    	        }


                    	        switch (alt38) 
                    	    	{
                    	    		case 1 :
                    	    		    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:188:59: tableJoin[ d ]
                    	    		    {
                    	    		    	PushFollow(FOLLOW_tableJoin_in_tableJoin913);
                    	    		    	tableJoin(d);
                    	    		    	state.followingStackPointer--;
                    	    		    	if (state.failed) return ;

                    	    		    }
                    	    		    break;

                    	    		default:
                    	    		    goto loop38;
                    	        }
                    	    } while (true);

                    	    loop38:
                    	    	;	// Stops C# compiler whining that label 'loop38' has no statements


                    	    Match(input, Token.UP, null); if (state.failed) return ;
                    	}

                    }
                    break;

            }
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "tableJoin"


    // $ANTLR start "booleanOp"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:191:1: booleanOp[ bool parens ] : ( ^( AND booleanExpr[true] booleanExpr[true] ) | ^( OR booleanExpr[false] booleanExpr[false] ) | ^( NOT booleanExpr[false] ) );
    public void booleanOp(bool parens) // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:192:2: ( ^( AND booleanExpr[true] booleanExpr[true] ) | ^( OR booleanExpr[false] booleanExpr[false] ) | ^( NOT booleanExpr[false] ) )
            int alt40 = 3;
            switch ( input.LA(1) ) 
            {
            case AND:
            	{
                alt40 = 1;
                }
                break;
            case OR:
            	{
                alt40 = 2;
                }
                break;
            case NOT:
            	{
                alt40 = 3;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d40s0 =
            	        new NoViableAltException("", 40, 0, input);

            	    throw nvae_d40s0;
            }

            switch (alt40) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:192:4: ^( AND booleanExpr[true] booleanExpr[true] )
                    {
                    	Match(input,AND,FOLLOW_AND_in_booleanOp933); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_booleanExpr_in_booleanOp935);
                    	booleanExpr(true);
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" and "); 
                    	}
                    	PushFollow(FOLLOW_booleanExpr_in_booleanOp940);
                    	booleanExpr(true);
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:193:4: ^( OR booleanExpr[false] booleanExpr[false] )
                    {
                    	Match(input,OR,FOLLOW_OR_in_booleanOp948); if (state.failed) return ;

                    	if ( (state.backtracking==0) )
                    	{
                    	   if (parens) Out("("); 
                    	}

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_booleanExpr_in_booleanOp952);
                    	booleanExpr(false);
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" or "); 
                    	}
                    	PushFollow(FOLLOW_booleanExpr_in_booleanOp957);
                    	booleanExpr(false);
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   if (parens) Out(")"); 
                    	}

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:194:4: ^( NOT booleanExpr[false] )
                    {
                    	Match(input,NOT,FOLLOW_NOT_in_booleanOp967); if (state.failed) return ;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" not ("); 
                    	}

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_booleanExpr_in_booleanOp971);
                    	booleanExpr(false);
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(")"); 
                    	}

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;

            }
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "booleanOp"


    // $ANTLR start "booleanExpr"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:197:1: booleanExpr[ bool parens ] : ( booleanOp[ parens ] | comparisonExpr[ parens ] | st= SQL_TOKEN );
    public void booleanExpr(bool parens) // throws RecognitionException [1]
    {   
        IASTNode st = null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:198:2: ( booleanOp[ parens ] | comparisonExpr[ parens ] | st= SQL_TOKEN )
            int alt41 = 3;
            switch ( input.LA(1) ) 
            {
            case AND:
            case NOT:
            case OR:
            	{
                alt41 = 1;
                }
                break;
            case BETWEEN:
            case EXISTS:
            case IN:
            case LIKE:
            case IS_NOT_NULL:
            case IS_NULL:
            case NOT_BETWEEN:
            case NOT_IN:
            case NOT_LIKE:
            case EQ:
            case NE:
            case LT:
            case GT:
            case LE:
            case GE:
            	{
                alt41 = 2;
                }
                break;
            case SQL_TOKEN:
            	{
                alt41 = 3;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d41s0 =
            	        new NoViableAltException("", 41, 0, input);

            	    throw nvae_d41s0;
            }

            switch (alt41) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:198:4: booleanOp[ parens ]
                    {
                    	PushFollow(FOLLOW_booleanOp_in_booleanExpr988);
                    	booleanOp(parens);
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:199:4: comparisonExpr[ parens ]
                    {
                    	PushFollow(FOLLOW_comparisonExpr_in_booleanExpr995);
                    	comparisonExpr(parens);
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:200:4: st= SQL_TOKEN
                    {
                    	st=(IASTNode)Match(input,SQL_TOKEN,FOLLOW_SQL_TOKEN_in_booleanExpr1004); if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(st); 
                    	}

                    }
                    break;

            }
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "booleanExpr"


    // $ANTLR start "comparisonExpr"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:203:1: comparisonExpr[ bool parens ] : ( binaryComparisonExpression | exoticComparisonExpression );
    public void comparisonExpr(bool parens) // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:204:2: ( binaryComparisonExpression | exoticComparisonExpression )
            int alt42 = 2;
            int LA42_0 = input.LA(1);

            if ( (LA42_0 == EQ || LA42_0 == NE || (LA42_0 >= LT && LA42_0 <= GE)) )
            {
                alt42 = 1;
            }
            else if ( (LA42_0 == BETWEEN || LA42_0 == EXISTS || LA42_0 == IN || LA42_0 == LIKE || (LA42_0 >= IS_NOT_NULL && LA42_0 <= IS_NULL) || (LA42_0 >= NOT_BETWEEN && LA42_0 <= NOT_LIKE)) )
            {
                alt42 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d42s0 =
                    new NoViableAltException("", 42, 0, input);

                throw nvae_d42s0;
            }
            switch (alt42) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:204:4: binaryComparisonExpression
                    {
                    	PushFollow(FOLLOW_binaryComparisonExpression_in_comparisonExpr1020);
                    	binaryComparisonExpression();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:205:4: exoticComparisonExpression
                    {
                    	if ( (state.backtracking==0) )
                    	{
                    	   if (parens) Out("("); 
                    	}
                    	PushFollow(FOLLOW_exoticComparisonExpression_in_comparisonExpr1027);
                    	exoticComparisonExpression();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   if (parens) Out(")"); 
                    	}

                    }
                    break;

            }
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "comparisonExpr"


    // $ANTLR start "binaryComparisonExpression"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:208:1: binaryComparisonExpression : ( ^( EQ expr expr ) | ^( NE expr expr ) | ^( GT expr expr ) | ^( GE expr expr ) | ^( LT expr expr ) | ^( LE expr expr ) );
    public void binaryComparisonExpression() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:209:2: ( ^( EQ expr expr ) | ^( NE expr expr ) | ^( GT expr expr ) | ^( GE expr expr ) | ^( LT expr expr ) | ^( LE expr expr ) )
            int alt43 = 6;
            switch ( input.LA(1) ) 
            {
            case EQ:
            	{
                alt43 = 1;
                }
                break;
            case NE:
            	{
                alt43 = 2;
                }
                break;
            case GT:
            	{
                alt43 = 3;
                }
                break;
            case GE:
            	{
                alt43 = 4;
                }
                break;
            case LT:
            	{
                alt43 = 5;
                }
                break;
            case LE:
            	{
                alt43 = 6;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d43s0 =
            	        new NoViableAltException("", 43, 0, input);

            	    throw nvae_d43s0;
            }

            switch (alt43) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:209:4: ^( EQ expr expr )
                    {
                    	Match(input,EQ,FOLLOW_EQ_in_binaryComparisonExpression1042); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1044);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("="); 
                    	}
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1048);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:210:4: ^( NE expr expr )
                    {
                    	Match(input,NE,FOLLOW_NE_in_binaryComparisonExpression1055); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1057);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("<>"); 
                    	}
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1061);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:211:4: ^( GT expr expr )
                    {
                    	Match(input,GT,FOLLOW_GT_in_binaryComparisonExpression1068); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1070);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(">"); 
                    	}
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1074);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 4 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:212:4: ^( GE expr expr )
                    {
                    	Match(input,GE,FOLLOW_GE_in_binaryComparisonExpression1081); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1083);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(">="); 
                    	}
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1087);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 5 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:213:4: ^( LT expr expr )
                    {
                    	Match(input,LT,FOLLOW_LT_in_binaryComparisonExpression1094); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1096);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("<"); 
                    	}
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1100);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 6 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:214:4: ^( LE expr expr )
                    {
                    	Match(input,LE,FOLLOW_LE_in_binaryComparisonExpression1107); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1109);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("<="); 
                    	}
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1113);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;

            }
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "binaryComparisonExpression"


    // $ANTLR start "exoticComparisonExpression"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:217:1: exoticComparisonExpression : ( ^( LIKE expr expr likeEscape ) | ^( NOT_LIKE expr expr likeEscape ) | ^( BETWEEN expr expr expr ) | ^( NOT_BETWEEN expr expr expr ) | ^( IN expr inList ) | ^( NOT_IN expr inList ) | ^( EXISTS quantified ) | ^( IS_NULL expr ) | ^( IS_NOT_NULL expr ) );
    public void exoticComparisonExpression() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:218:2: ( ^( LIKE expr expr likeEscape ) | ^( NOT_LIKE expr expr likeEscape ) | ^( BETWEEN expr expr expr ) | ^( NOT_BETWEEN expr expr expr ) | ^( IN expr inList ) | ^( NOT_IN expr inList ) | ^( EXISTS quantified ) | ^( IS_NULL expr ) | ^( IS_NOT_NULL expr ) )
            int alt44 = 9;
            switch ( input.LA(1) ) 
            {
            case LIKE:
            	{
                alt44 = 1;
                }
                break;
            case NOT_LIKE:
            	{
                alt44 = 2;
                }
                break;
            case BETWEEN:
            	{
                alt44 = 3;
                }
                break;
            case NOT_BETWEEN:
            	{
                alt44 = 4;
                }
                break;
            case IN:
            	{
                alt44 = 5;
                }
                break;
            case NOT_IN:
            	{
                alt44 = 6;
                }
                break;
            case EXISTS:
            	{
                alt44 = 7;
                }
                break;
            case IS_NULL:
            	{
                alt44 = 8;
                }
                break;
            case IS_NOT_NULL:
            	{
                alt44 = 9;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d44s0 =
            	        new NoViableAltException("", 44, 0, input);

            	    throw nvae_d44s0;
            }

            switch (alt44) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:218:4: ^( LIKE expr expr likeEscape )
                    {
                    	Match(input,LIKE,FOLLOW_LIKE_in_exoticComparisonExpression1127); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1129);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" like "); 
                    	}
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1133);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_likeEscape_in_exoticComparisonExpression1135);
                    	likeEscape();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:219:4: ^( NOT_LIKE expr expr likeEscape )
                    {
                    	Match(input,NOT_LIKE,FOLLOW_NOT_LIKE_in_exoticComparisonExpression1143); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1145);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" not like "); 
                    	}
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1149);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_likeEscape_in_exoticComparisonExpression1151);
                    	likeEscape();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:220:4: ^( BETWEEN expr expr expr )
                    {
                    	Match(input,BETWEEN,FOLLOW_BETWEEN_in_exoticComparisonExpression1158); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1160);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" between "); 
                    	}
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1164);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" and "); 
                    	}
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1168);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 4 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:221:4: ^( NOT_BETWEEN expr expr expr )
                    {
                    	Match(input,NOT_BETWEEN,FOLLOW_NOT_BETWEEN_in_exoticComparisonExpression1175); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1177);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" not between "); 
                    	}
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1181);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" and "); 
                    	}
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1185);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 5 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:222:4: ^( IN expr inList )
                    {
                    	Match(input,IN,FOLLOW_IN_in_exoticComparisonExpression1192); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1194);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" in"); 
                    	}
                    	PushFollow(FOLLOW_inList_in_exoticComparisonExpression1198);
                    	inList();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 6 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:223:4: ^( NOT_IN expr inList )
                    {
                    	Match(input,NOT_IN,FOLLOW_NOT_IN_in_exoticComparisonExpression1206); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1208);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" not in "); 
                    	}
                    	PushFollow(FOLLOW_inList_in_exoticComparisonExpression1212);
                    	inList();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 7 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:224:4: ^( EXISTS quantified )
                    {
                    	Match(input,EXISTS,FOLLOW_EXISTS_in_exoticComparisonExpression1220); if (state.failed) return ;

                    	if ( (state.backtracking==0) )
                    	{
                    	   OptionalSpace(); Out("exists "); 
                    	}

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_quantified_in_exoticComparisonExpression1224);
                    	quantified();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 8 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:225:4: ^( IS_NULL expr )
                    {
                    	Match(input,IS_NULL,FOLLOW_IS_NULL_in_exoticComparisonExpression1232); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1234);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" is null"); 
                    	}

                    }
                    break;
                case 9 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:226:4: ^( IS_NOT_NULL expr )
                    {
                    	Match(input,IS_NOT_NULL,FOLLOW_IS_NOT_NULL_in_exoticComparisonExpression1243); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1245);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" is not null"); 
                    	}

                    }
                    break;

            }
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "exoticComparisonExpression"


    // $ANTLR start "likeEscape"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:229:1: likeEscape : ( ^( ESCAPE expr ) )? ;
    public void likeEscape() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:230:2: ( ( ^( ESCAPE expr ) )? )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:230:4: ( ^( ESCAPE expr ) )?
            {
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:230:4: ( ^( ESCAPE expr ) )?
            	int alt45 = 2;
            	int LA45_0 = input.LA(1);

            	if ( (LA45_0 == ESCAPE) )
            	{
            	    alt45 = 1;
            	}
            	switch (alt45) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:230:6: ^( ESCAPE expr )
            	        {
            	        	Match(input,ESCAPE,FOLLOW_ESCAPE_in_likeEscape1262); if (state.failed) return ;

            	        	if ( (state.backtracking==0) )
            	        	{
            	        	   Out(" escape "); 
            	        	}

            	        	Match(input, Token.DOWN, null); if (state.failed) return ;
            	        	PushFollow(FOLLOW_expr_in_likeEscape1266);
            	        	expr();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        	Match(input, Token.UP, null); if (state.failed) return ;

            	        }
            	        break;

            	}


            }

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "likeEscape"


    // $ANTLR start "inList"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:233:1: inList : ^( IN_LIST ( parenSelect | simpleExprList ) ) ;
    public void inList() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:234:2: ( ^( IN_LIST ( parenSelect | simpleExprList ) ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:234:4: ^( IN_LIST ( parenSelect | simpleExprList ) )
            {
            	Match(input,IN_LIST,FOLLOW_IN_LIST_in_inList1282); if (state.failed) return ;

            	if ( (state.backtracking==0) )
            	{
            	   Out(" "); 
            	}

            	if ( input.LA(1) == Token.DOWN )
            	{
            	    Match(input, Token.DOWN, null); if (state.failed) return ;
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:234:28: ( parenSelect | simpleExprList )
            	    int alt46 = 2;
            	    int LA46_0 = input.LA(1);

            	    if ( (LA46_0 == SELECT || LA46_0 == UNION) )
            	    {
            	        alt46 = 1;
            	    }
            	    else if ( (LA46_0 == UP || LA46_0 == COUNT || LA46_0 == DOT || LA46_0 == FALSE || LA46_0 == NULL || LA46_0 == TRUE || LA46_0 == CASE || LA46_0 == AGGREGATE || LA46_0 == CASE2 || LA46_0 == INDEX_OP || LA46_0 == METHOD_CALL || LA46_0 == UNARY_MINUS || (LA46_0 >= CONSTANT && LA46_0 <= JAVA_CONSTANT) || (LA46_0 >= BNOT && LA46_0 <= DIV) || (LA46_0 >= PARAM && LA46_0 <= IDENT) || LA46_0 == ALIAS_REF || LA46_0 == SQL_TOKEN || LA46_0 == NAMED_PARAM) )
            	    {
            	        alt46 = 2;
            	    }
            	    else 
            	    {
            	        if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	        NoViableAltException nvae_d46s0 =
            	            new NoViableAltException("", 46, 0, input);

            	        throw nvae_d46s0;
            	    }
            	    switch (alt46) 
            	    {
            	        case 1 :
            	            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:234:30: parenSelect
            	            {
            	            	PushFollow(FOLLOW_parenSelect_in_inList1288);
            	            	parenSelect();
            	            	state.followingStackPointer--;
            	            	if (state.failed) return ;

            	            }
            	            break;
            	        case 2 :
            	            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:234:44: simpleExprList
            	            {
            	            	PushFollow(FOLLOW_simpleExprList_in_inList1292);
            	            	simpleExprList();
            	            	state.followingStackPointer--;
            	            	if (state.failed) return ;

            	            }
            	            break;

            	    }


            	    Match(input, Token.UP, null); if (state.failed) return ;
            	}

            }

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "inList"


    // $ANTLR start "simpleExprList"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:237:1: simpleExprList : (e= simpleExpr )* ;
    public void simpleExprList() // throws RecognitionException [1]
    {   
        SqlGenerator.simpleExpr_return e = default(SqlGenerator.simpleExpr_return);


        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:238:2: ( (e= simpleExpr )* )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:238:4: (e= simpleExpr )*
            {
            	if ( (state.backtracking==0) )
            	{
            	   Out("("); 
            	}
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:238:18: (e= simpleExpr )*
            	do 
            	{
            	    int alt47 = 2;
            	    int LA47_0 = input.LA(1);

            	    if ( (LA47_0 == COUNT || LA47_0 == DOT || LA47_0 == FALSE || LA47_0 == NULL || LA47_0 == TRUE || LA47_0 == CASE || LA47_0 == AGGREGATE || LA47_0 == CASE2 || LA47_0 == INDEX_OP || LA47_0 == METHOD_CALL || LA47_0 == UNARY_MINUS || (LA47_0 >= CONSTANT && LA47_0 <= JAVA_CONSTANT) || (LA47_0 >= BNOT && LA47_0 <= DIV) || (LA47_0 >= PARAM && LA47_0 <= IDENT) || LA47_0 == ALIAS_REF || LA47_0 == SQL_TOKEN || LA47_0 == NAMED_PARAM) )
            	    {
            	        alt47 = 1;
            	    }


            	    switch (alt47) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:238:19: e= simpleExpr
            			    {
            			    	PushFollow(FOLLOW_simpleExpr_in_simpleExprList1313);
            			    	e = simpleExpr();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return ;
            			    	if ( (state.backtracking==0) )
            			    	{
            			    	   Separator(((e != null) ? ((IASTNode)e.Start) : null)," , "); 
            			    	}

            			    }
            			    break;

            			default:
            			    goto loop47;
            	    }
            	} while (true);

            	loop47:
            		;	// Stops C# compiler whining that label 'loop47' has no statements

            	if ( (state.backtracking==0) )
            	{
            	   Out(")"); 
            	}

            }

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "simpleExprList"

    public class expr_return : TreeRuleReturnScope
    {
    };

    // $ANTLR start "expr"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:242:1: expr : ( simpleExpr | ^( VECTOR_EXPR (e= expr )* ) | parenSelect | ^( ANY quantified ) | ^( ALL quantified ) | ^( SOME quantified ) );
    public SqlGenerator.expr_return expr() // throws RecognitionException [1]
    {   
        SqlGenerator.expr_return retval = new SqlGenerator.expr_return();
        retval.Start = input.LT(1);

        SqlGenerator.expr_return e = default(SqlGenerator.expr_return);


        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:243:2: ( simpleExpr | ^( VECTOR_EXPR (e= expr )* ) | parenSelect | ^( ANY quantified ) | ^( ALL quantified ) | ^( SOME quantified ) )
            int alt49 = 6;
            switch ( input.LA(1) ) 
            {
            case COUNT:
            case DOT:
            case FALSE:
            case NULL:
            case TRUE:
            case CASE:
            case AGGREGATE:
            case CASE2:
            case INDEX_OP:
            case METHOD_CALL:
            case UNARY_MINUS:
            case CONSTANT:
            case NUM_INT:
            case NUM_DOUBLE:
            case NUM_FLOAT:
            case NUM_LONG:
            case JAVA_CONSTANT:
            case BNOT:
            case BOR:
            case BXOR:
            case BAND:
            case PLUS:
            case MINUS:
            case STAR:
            case DIV:
            case PARAM:
            case QUOTED_String:
            case IDENT:
            case ALIAS_REF:
            case SQL_TOKEN:
            case NAMED_PARAM:
            	{
                alt49 = 1;
                }
                break;
            case VECTOR_EXPR:
            	{
                alt49 = 2;
                }
                break;
            case SELECT:
            case UNION:
            	{
                alt49 = 3;
                }
                break;
            case ANY:
            	{
                alt49 = 4;
                }
                break;
            case ALL:
            	{
                alt49 = 5;
                }
                break;
            case SOME:
            	{
                alt49 = 6;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
            	    NoViableAltException nvae_d49s0 =
            	        new NoViableAltException("", 49, 0, input);

            	    throw nvae_d49s0;
            }

            switch (alt49) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:243:4: simpleExpr
                    {
                    	PushFollow(FOLLOW_simpleExpr_in_expr1332);
                    	simpleExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:244:4: ^( VECTOR_EXPR (e= expr )* )
                    {
                    	Match(input,VECTOR_EXPR,FOLLOW_VECTOR_EXPR_in_expr1339); if (state.failed) return retval;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("("); 
                    	}

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return retval;
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:244:33: (e= expr )*
                    	    do 
                    	    {
                    	        int alt48 = 2;
                    	        int LA48_0 = input.LA(1);

                    	        if ( ((LA48_0 >= ALL && LA48_0 <= ANY) || LA48_0 == COUNT || LA48_0 == DOT || LA48_0 == FALSE || LA48_0 == NULL || LA48_0 == SELECT || LA48_0 == SOME || (LA48_0 >= TRUE && LA48_0 <= UNION) || LA48_0 == CASE || LA48_0 == AGGREGATE || LA48_0 == CASE2 || LA48_0 == INDEX_OP || LA48_0 == METHOD_CALL || LA48_0 == UNARY_MINUS || LA48_0 == VECTOR_EXPR || (LA48_0 >= CONSTANT && LA48_0 <= JAVA_CONSTANT) || (LA48_0 >= BNOT && LA48_0 <= DIV) || (LA48_0 >= PARAM && LA48_0 <= IDENT) || LA48_0 == ALIAS_REF || LA48_0 == SQL_TOKEN || LA48_0 == NAMED_PARAM) )
                    	        {
                    	            alt48 = 1;
                    	        }


                    	        switch (alt48) 
                    	    	{
                    	    		case 1 :
                    	    		    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:244:34: e= expr
                    	    		    {
                    	    		    	PushFollow(FOLLOW_expr_in_expr1346);
                    	    		    	e = expr();
                    	    		    	state.followingStackPointer--;
                    	    		    	if (state.failed) return retval;
                    	    		    	if ( (state.backtracking==0) )
                    	    		    	{
                    	    		    	   Separator(((e != null) ? ((IASTNode)e.Start) : null)," , "); 
                    	    		    	}

                    	    		    }
                    	    		    break;

                    	    		default:
                    	    		    goto loop48;
                    	        }
                    	    } while (true);

                    	    loop48:
                    	    	;	// Stops C# compiler whining that label 'loop48' has no statements

                    	    if ( (state.backtracking==0) )
                    	    {
                    	       Out(")"); 
                    	    }

                    	    Match(input, Token.UP, null); if (state.failed) return retval;
                    	}

                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:245:4: parenSelect
                    {
                    	PushFollow(FOLLOW_parenSelect_in_expr1361);
                    	parenSelect();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 4 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:246:4: ^( ANY quantified )
                    {
                    	Match(input,ANY,FOLLOW_ANY_in_expr1367); if (state.failed) return retval;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("any "); 
                    	}

                    	Match(input, Token.DOWN, null); if (state.failed) return retval;
                    	PushFollow(FOLLOW_quantified_in_expr1371);
                    	quantified();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    	Match(input, Token.UP, null); if (state.failed) return retval;

                    }
                    break;
                case 5 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:247:4: ^( ALL quantified )
                    {
                    	Match(input,ALL,FOLLOW_ALL_in_expr1379); if (state.failed) return retval;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("all "); 
                    	}

                    	Match(input, Token.DOWN, null); if (state.failed) return retval;
                    	PushFollow(FOLLOW_quantified_in_expr1383);
                    	quantified();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    	Match(input, Token.UP, null); if (state.failed) return retval;

                    }
                    break;
                case 6 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:248:4: ^( SOME quantified )
                    {
                    	Match(input,SOME,FOLLOW_SOME_in_expr1391); if (state.failed) return retval;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("some "); 
                    	}

                    	Match(input, Token.DOWN, null); if (state.failed) return retval;
                    	PushFollow(FOLLOW_quantified_in_expr1395);
                    	quantified();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    	Match(input, Token.UP, null); if (state.failed) return retval;

                    }
                    break;

            }
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "expr"


    // $ANTLR start "quantified"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:251:1: quantified : ( sqlToken | selectStatement ) ;
    public void quantified() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:252:2: ( ( sqlToken | selectStatement ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:252:4: ( sqlToken | selectStatement )
            {
            	if ( (state.backtracking==0) )
            	{
            	   Out("("); 
            	}
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:252:18: ( sqlToken | selectStatement )
            	int alt50 = 2;
            	int LA50_0 = input.LA(1);

            	if ( (LA50_0 == SQL_TOKEN) )
            	{
            	    alt50 = 1;
            	}
            	else if ( (LA50_0 == SELECT) )
            	{
            	    alt50 = 2;
            	}
            	else 
            	{
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d50s0 =
            	        new NoViableAltException("", 50, 0, input);

            	    throw nvae_d50s0;
            	}
            	switch (alt50) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:252:20: sqlToken
            	        {
            	        	PushFollow(FOLLOW_sqlToken_in_quantified1413);
            	        	sqlToken();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;
            	    case 2 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:252:31: selectStatement
            	        {
            	        	PushFollow(FOLLOW_selectStatement_in_quantified1417);
            	        	selectStatement();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}

            	if ( (state.backtracking==0) )
            	{
            	   Out(")"); 
            	}

            }

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "quantified"


    // $ANTLR start "parenSelect"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:255:1: parenSelect : ( selectStatement | ^( UNION selectStatement parenSelect ) );
    public void parenSelect() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:256:2: ( selectStatement | ^( UNION selectStatement parenSelect ) )
            int alt51 = 2;
            int LA51_0 = input.LA(1);

            if ( (LA51_0 == SELECT) )
            {
                alt51 = 1;
            }
            else if ( (LA51_0 == UNION) )
            {
                alt51 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d51s0 =
                    new NoViableAltException("", 51, 0, input);

                throw nvae_d51s0;
            }
            switch (alt51) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:256:4: selectStatement
                    {
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("("); 
                    	}
                    	PushFollow(FOLLOW_selectStatement_in_parenSelect1436);
                    	selectStatement();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(")"); 
                    	}

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:257:4: ^( UNION selectStatement parenSelect )
                    {
                    	Match(input,UNION,FOLLOW_UNION_in_parenSelect1445); if (state.failed) return ;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("("); 
                    	}

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_selectStatement_in_parenSelect1449);
                    	selectStatement();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(") union "); 
                    	}
                    	PushFollow(FOLLOW_parenSelect_in_parenSelect1453);
                    	parenSelect();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;

            }
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "parenSelect"

    public class simpleExpr_return : TreeRuleReturnScope
    {
    };

    // $ANTLR start "simpleExpr"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:261:1: simpleExpr : (c= constant | NULL | addrExpr | sqlToken | aggregate | methodCall | count | parameter | arithmeticExpr );
    public SqlGenerator.simpleExpr_return simpleExpr() // throws RecognitionException [1]
    {   
        SqlGenerator.simpleExpr_return retval = new SqlGenerator.simpleExpr_return();
        retval.Start = input.LT(1);

        SqlGenerator.constant_return c = default(SqlGenerator.constant_return);


        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:262:2: (c= constant | NULL | addrExpr | sqlToken | aggregate | methodCall | count | parameter | arithmeticExpr )
            int alt52 = 9;
            switch ( input.LA(1) ) 
            {
            case FALSE:
            case TRUE:
            case CONSTANT:
            case NUM_INT:
            case NUM_DOUBLE:
            case NUM_FLOAT:
            case NUM_LONG:
            case JAVA_CONSTANT:
            case QUOTED_String:
            case IDENT:
            	{
                alt52 = 1;
                }
                break;
            case NULL:
            	{
                alt52 = 2;
                }
                break;
            case DOT:
            case INDEX_OP:
            case ALIAS_REF:
            	{
                alt52 = 3;
                }
                break;
            case SQL_TOKEN:
            	{
                alt52 = 4;
                }
                break;
            case AGGREGATE:
            	{
                alt52 = 5;
                }
                break;
            case METHOD_CALL:
            	{
                alt52 = 6;
                }
                break;
            case COUNT:
            	{
                alt52 = 7;
                }
                break;
            case PARAM:
            case NAMED_PARAM:
            	{
                alt52 = 8;
                }
                break;
            case CASE:
            case CASE2:
            case UNARY_MINUS:
            case BNOT:
            case BOR:
            case BXOR:
            case BAND:
            case PLUS:
            case MINUS:
            case STAR:
            case DIV:
            	{
                alt52 = 9;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
            	    NoViableAltException nvae_d52s0 =
            	        new NoViableAltException("", 52, 0, input);

            	    throw nvae_d52s0;
            }

            switch (alt52) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:262:4: c= constant
                    {
                    	PushFollow(FOLLOW_constant_in_simpleExpr1470);
                    	c = constant();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(((c != null) ? ((IASTNode)c.Start) : null)); 
                    	}

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:263:4: NULL
                    {
                    	Match(input,NULL,FOLLOW_NULL_in_simpleExpr1477); if (state.failed) return retval;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("null"); 
                    	}

                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:264:4: addrExpr
                    {
                    	PushFollow(FOLLOW_addrExpr_in_simpleExpr1484);
                    	addrExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 4 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:265:4: sqlToken
                    {
                    	PushFollow(FOLLOW_sqlToken_in_simpleExpr1489);
                    	sqlToken();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 5 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:266:4: aggregate
                    {
                    	PushFollow(FOLLOW_aggregate_in_simpleExpr1494);
                    	aggregate();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 6 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:267:4: methodCall
                    {
                    	PushFollow(FOLLOW_methodCall_in_simpleExpr1499);
                    	methodCall();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 7 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:268:4: count
                    {
                    	PushFollow(FOLLOW_count_in_simpleExpr1504);
                    	count();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 8 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:269:4: parameter
                    {
                    	PushFollow(FOLLOW_parameter_in_simpleExpr1509);
                    	parameter();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 9 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:270:4: arithmeticExpr
                    {
                    	PushFollow(FOLLOW_arithmeticExpr_in_simpleExpr1514);
                    	arithmeticExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;

            }
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "simpleExpr"

    public class constant_return : TreeRuleReturnScope
    {
    };

    // $ANTLR start "constant"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:273:1: constant : ( NUM_DOUBLE | NUM_FLOAT | NUM_INT | NUM_LONG | QUOTED_String | CONSTANT | JAVA_CONSTANT | TRUE | FALSE | IDENT );
    public SqlGenerator.constant_return constant() // throws RecognitionException [1]
    {   
        SqlGenerator.constant_return retval = new SqlGenerator.constant_return();
        retval.Start = input.LT(1);

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:274:2: ( NUM_DOUBLE | NUM_FLOAT | NUM_INT | NUM_LONG | QUOTED_String | CONSTANT | JAVA_CONSTANT | TRUE | FALSE | IDENT )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:
            {
            	if ( input.LA(1) == FALSE || input.LA(1) == TRUE || (input.LA(1) >= CONSTANT && input.LA(1) <= JAVA_CONSTANT) || (input.LA(1) >= QUOTED_String && input.LA(1) <= IDENT) ) 
            	{
            	    input.Consume();
            	    state.errorRecovery = false;state.failed = false;
            	}
            	else 
            	{
            	    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
            	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	    throw mse;
            	}


            }

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "constant"


    // $ANTLR start "arithmeticExpr"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:286:1: arithmeticExpr : ( additiveExpr | bitwiseExpr | multiplicativeExpr | ^( UNARY_MINUS expr ) | caseExpr );
    public void arithmeticExpr() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:287:2: ( additiveExpr | bitwiseExpr | multiplicativeExpr | ^( UNARY_MINUS expr ) | caseExpr )
            int alt53 = 5;
            switch ( input.LA(1) ) 
            {
            case PLUS:
            case MINUS:
            	{
                alt53 = 1;
                }
                break;
            case BNOT:
            case BOR:
            case BXOR:
            case BAND:
            	{
                alt53 = 2;
                }
                break;
            case STAR:
            case DIV:
            	{
                alt53 = 3;
                }
                break;
            case UNARY_MINUS:
            	{
                alt53 = 4;
                }
                break;
            case CASE:
            case CASE2:
            	{
                alt53 = 5;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d53s0 =
            	        new NoViableAltException("", 53, 0, input);

            	    throw nvae_d53s0;
            }

            switch (alt53) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:287:4: additiveExpr
                    {
                    	PushFollow(FOLLOW_additiveExpr_in_arithmeticExpr1583);
                    	additiveExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:288:4: bitwiseExpr
                    {
                    	PushFollow(FOLLOW_bitwiseExpr_in_arithmeticExpr1588);
                    	bitwiseExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:289:4: multiplicativeExpr
                    {
                    	PushFollow(FOLLOW_multiplicativeExpr_in_arithmeticExpr1593);
                    	multiplicativeExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 4 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:291:4: ^( UNARY_MINUS expr )
                    {
                    	Match(input,UNARY_MINUS,FOLLOW_UNARY_MINUS_in_arithmeticExpr1600); if (state.failed) return ;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("-"); 
                    	}

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_arithmeticExpr1604);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 5 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:292:4: caseExpr
                    {
                    	PushFollow(FOLLOW_caseExpr_in_arithmeticExpr1610);
                    	caseExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;

            }
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "arithmeticExpr"


    // $ANTLR start "additiveExpr"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:295:1: additiveExpr : ( ^( PLUS expr expr ) | ^( MINUS expr nestedExprAfterMinusDiv ) );
    public void additiveExpr() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:296:2: ( ^( PLUS expr expr ) | ^( MINUS expr nestedExprAfterMinusDiv ) )
            int alt54 = 2;
            int LA54_0 = input.LA(1);

            if ( (LA54_0 == PLUS) )
            {
                alt54 = 1;
            }
            else if ( (LA54_0 == MINUS) )
            {
                alt54 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d54s0 =
                    new NoViableAltException("", 54, 0, input);

                throw nvae_d54s0;
            }
            switch (alt54) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:296:4: ^( PLUS expr expr )
                    {
                    	Match(input,PLUS,FOLLOW_PLUS_in_additiveExpr1622); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_additiveExpr1624);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("+"); 
                    	}
                    	PushFollow(FOLLOW_expr_in_additiveExpr1628);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:297:4: ^( MINUS expr nestedExprAfterMinusDiv )
                    {
                    	Match(input,MINUS,FOLLOW_MINUS_in_additiveExpr1635); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_additiveExpr1637);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("-"); 
                    	}
                    	PushFollow(FOLLOW_nestedExprAfterMinusDiv_in_additiveExpr1641);
                    	nestedExprAfterMinusDiv();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;

            }
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "additiveExpr"


    // $ANTLR start "bitwiseExpr"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:300:1: bitwiseExpr : ( ^( BAND expr nestedExpr ) | ^( BOR expr nestedExpr ) | ^( BXOR expr nestedExpr ) | ^( BNOT nestedExpr ) );
    public void bitwiseExpr() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:301:2: ( ^( BAND expr nestedExpr ) | ^( BOR expr nestedExpr ) | ^( BXOR expr nestedExpr ) | ^( BNOT nestedExpr ) )
            int alt55 = 4;
            switch ( input.LA(1) ) 
            {
            case BAND:
            	{
                alt55 = 1;
                }
                break;
            case BOR:
            	{
                alt55 = 2;
                }
                break;
            case BXOR:
            	{
                alt55 = 3;
                }
                break;
            case BNOT:
            	{
                alt55 = 4;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d55s0 =
            	        new NoViableAltException("", 55, 0, input);

            	    throw nvae_d55s0;
            }

            switch (alt55) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:301:4: ^( BAND expr nestedExpr )
                    {
                    	Match(input,BAND,FOLLOW_BAND_in_bitwiseExpr1654); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_bitwiseExpr1656);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("&"); 
                    	}
                    	PushFollow(FOLLOW_nestedExpr_in_bitwiseExpr1660);
                    	nestedExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:302:4: ^( BOR expr nestedExpr )
                    {
                    	Match(input,BOR,FOLLOW_BOR_in_bitwiseExpr1667); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_bitwiseExpr1669);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("|"); 
                    	}
                    	PushFollow(FOLLOW_nestedExpr_in_bitwiseExpr1673);
                    	nestedExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:303:4: ^( BXOR expr nestedExpr )
                    {
                    	Match(input,BXOR,FOLLOW_BXOR_in_bitwiseExpr1680); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_bitwiseExpr1682);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("^"); 
                    	}
                    	PushFollow(FOLLOW_nestedExpr_in_bitwiseExpr1686);
                    	nestedExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 4 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:304:4: ^( BNOT nestedExpr )
                    {
                    	Match(input,BNOT,FOLLOW_BNOT_in_bitwiseExpr1693); if (state.failed) return ;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("~"); 
                    	}

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_nestedExpr_in_bitwiseExpr1697);
                    	nestedExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;

            }
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "bitwiseExpr"


    // $ANTLR start "multiplicativeExpr"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:307:1: multiplicativeExpr : ( ^( STAR nestedExpr nestedExpr ) | ^( DIV nestedExpr nestedExprAfterMinusDiv ) );
    public void multiplicativeExpr() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:308:2: ( ^( STAR nestedExpr nestedExpr ) | ^( DIV nestedExpr nestedExprAfterMinusDiv ) )
            int alt56 = 2;
            int LA56_0 = input.LA(1);

            if ( (LA56_0 == STAR) )
            {
                alt56 = 1;
            }
            else if ( (LA56_0 == DIV) )
            {
                alt56 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d56s0 =
                    new NoViableAltException("", 56, 0, input);

                throw nvae_d56s0;
            }
            switch (alt56) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:308:4: ^( STAR nestedExpr nestedExpr )
                    {
                    	Match(input,STAR,FOLLOW_STAR_in_multiplicativeExpr1711); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_nestedExpr_in_multiplicativeExpr1713);
                    	nestedExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("*"); 
                    	}
                    	PushFollow(FOLLOW_nestedExpr_in_multiplicativeExpr1717);
                    	nestedExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:309:4: ^( DIV nestedExpr nestedExprAfterMinusDiv )
                    {
                    	Match(input,DIV,FOLLOW_DIV_in_multiplicativeExpr1724); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_nestedExpr_in_multiplicativeExpr1726);
                    	nestedExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("/"); 
                    	}
                    	PushFollow(FOLLOW_nestedExprAfterMinusDiv_in_multiplicativeExpr1730);
                    	nestedExprAfterMinusDiv();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;

            }
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "multiplicativeExpr"


    // $ANTLR start "nestedExpr"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:312:1: nestedExpr : ( ( additiveExpr )=> additiveExpr | ( bitwiseExpr )=> bitwiseExpr | expr );
    public void nestedExpr() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:314:2: ( ( additiveExpr )=> additiveExpr | ( bitwiseExpr )=> bitwiseExpr | expr )
            int alt57 = 3;
            alt57 = dfa57.Predict(input);
            switch (alt57) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:314:4: ( additiveExpr )=> additiveExpr
                    {
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("("); 
                    	}
                    	PushFollow(FOLLOW_additiveExpr_in_nestedExpr1752);
                    	additiveExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(")"); 
                    	}

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:315:4: ( bitwiseExpr )=> bitwiseExpr
                    {
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("("); 
                    	}
                    	PushFollow(FOLLOW_bitwiseExpr_in_nestedExpr1767);
                    	bitwiseExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(")"); 
                    	}

                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:316:4: expr
                    {
                    	PushFollow(FOLLOW_expr_in_nestedExpr1774);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;

            }
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "nestedExpr"


    // $ANTLR start "nestedExprAfterMinusDiv"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:319:1: nestedExprAfterMinusDiv : ( ( arithmeticExpr )=> arithmeticExpr | expr );
    public void nestedExprAfterMinusDiv() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:321:2: ( ( arithmeticExpr )=> arithmeticExpr | expr )
            int alt58 = 2;
            alt58 = dfa58.Predict(input);
            switch (alt58) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:321:4: ( arithmeticExpr )=> arithmeticExpr
                    {
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("("); 
                    	}
                    	PushFollow(FOLLOW_arithmeticExpr_in_nestedExprAfterMinusDiv1796);
                    	arithmeticExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(")"); 
                    	}

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:322:4: expr
                    {
                    	PushFollow(FOLLOW_expr_in_nestedExprAfterMinusDiv1803);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;

            }
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "nestedExprAfterMinusDiv"


    // $ANTLR start "caseExpr"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:325:1: caseExpr : ( ^( CASE ( ^( WHEN booleanExpr[false] expr ) )+ ( ^( ELSE expr ) )? ) | ^( CASE2 expr ( ^( WHEN expr expr ) )+ ( ^( ELSE expr ) )? ) );
    public void caseExpr() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:326:2: ( ^( CASE ( ^( WHEN booleanExpr[false] expr ) )+ ( ^( ELSE expr ) )? ) | ^( CASE2 expr ( ^( WHEN expr expr ) )+ ( ^( ELSE expr ) )? ) )
            int alt63 = 2;
            int LA63_0 = input.LA(1);

            if ( (LA63_0 == CASE) )
            {
                alt63 = 1;
            }
            else if ( (LA63_0 == CASE2) )
            {
                alt63 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d63s0 =
                    new NoViableAltException("", 63, 0, input);

                throw nvae_d63s0;
            }
            switch (alt63) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:326:4: ^( CASE ( ^( WHEN booleanExpr[false] expr ) )+ ( ^( ELSE expr ) )? )
                    {
                    	Match(input,CASE,FOLLOW_CASE_in_caseExpr1815); if (state.failed) return ;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("case"); 
                    	}

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:327:3: ( ^( WHEN booleanExpr[false] expr ) )+
                    	int cnt59 = 0;
                    	do 
                    	{
                    	    int alt59 = 2;
                    	    int LA59_0 = input.LA(1);

                    	    if ( (LA59_0 == WHEN) )
                    	    {
                    	        alt59 = 1;
                    	    }


                    	    switch (alt59) 
                    		{
                    			case 1 :
                    			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:327:5: ^( WHEN booleanExpr[false] expr )
                    			    {
                    			    	Match(input,WHEN,FOLLOW_WHEN_in_caseExpr1825); if (state.failed) return ;

                    			    	if ( (state.backtracking==0) )
                    			    	{
                    			    	   Out( " when "); 
                    			    	}

                    			    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    			    	PushFollow(FOLLOW_booleanExpr_in_caseExpr1829);
                    			    	booleanExpr(false);
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return ;
                    			    	if ( (state.backtracking==0) )
                    			    	{
                    			    	   Out(" then "); 
                    			    	}
                    			    	PushFollow(FOLLOW_expr_in_caseExpr1834);
                    			    	expr();
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return ;

                    			    	Match(input, Token.UP, null); if (state.failed) return ;

                    			    }
                    			    break;

                    			default:
                    			    if ( cnt59 >= 1 ) goto loop59;
                    			    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    		            EarlyExitException eee59 =
                    		                new EarlyExitException(59, input);
                    		            throw eee59;
                    	    }
                    	    cnt59++;
                    	} while (true);

                    	loop59:
                    		;	// Stops C# compiler whinging that label 'loop59' has no statements

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:328:3: ( ^( ELSE expr ) )?
                    	int alt60 = 2;
                    	int LA60_0 = input.LA(1);

                    	if ( (LA60_0 == ELSE) )
                    	{
                    	    alt60 = 1;
                    	}
                    	switch (alt60) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:328:5: ^( ELSE expr )
                    	        {
                    	        	Match(input,ELSE,FOLLOW_ELSE_in_caseExpr1846); if (state.failed) return ;

                    	        	if ( (state.backtracking==0) )
                    	        	{
                    	        	   Out(" else "); 
                    	        	}

                    	        	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	        	PushFollow(FOLLOW_expr_in_caseExpr1850);
                    	        	expr();
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        	Match(input, Token.UP, null); if (state.failed) return ;

                    	        }
                    	        break;

                    	}

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" end"); 
                    	}

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:330:4: ^( CASE2 expr ( ^( WHEN expr expr ) )+ ( ^( ELSE expr ) )? )
                    {
                    	Match(input,CASE2,FOLLOW_CASE2_in_caseExpr1866); if (state.failed) return ;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("case "); 
                    	}

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_caseExpr1870);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:331:3: ( ^( WHEN expr expr ) )+
                    	int cnt61 = 0;
                    	do 
                    	{
                    	    int alt61 = 2;
                    	    int LA61_0 = input.LA(1);

                    	    if ( (LA61_0 == WHEN) )
                    	    {
                    	        alt61 = 1;
                    	    }


                    	    switch (alt61) 
                    		{
                    			case 1 :
                    			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:331:5: ^( WHEN expr expr )
                    			    {
                    			    	Match(input,WHEN,FOLLOW_WHEN_in_caseExpr1877); if (state.failed) return ;

                    			    	if ( (state.backtracking==0) )
                    			    	{
                    			    	   Out( " when "); 
                    			    	}

                    			    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    			    	PushFollow(FOLLOW_expr_in_caseExpr1881);
                    			    	expr();
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return ;
                    			    	if ( (state.backtracking==0) )
                    			    	{
                    			    	   Out(" then "); 
                    			    	}
                    			    	PushFollow(FOLLOW_expr_in_caseExpr1885);
                    			    	expr();
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return ;

                    			    	Match(input, Token.UP, null); if (state.failed) return ;

                    			    }
                    			    break;

                    			default:
                    			    if ( cnt61 >= 1 ) goto loop61;
                    			    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    		            EarlyExitException eee61 =
                    		                new EarlyExitException(61, input);
                    		            throw eee61;
                    	    }
                    	    cnt61++;
                    	} while (true);

                    	loop61:
                    		;	// Stops C# compiler whinging that label 'loop61' has no statements

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:332:3: ( ^( ELSE expr ) )?
                    	int alt62 = 2;
                    	int LA62_0 = input.LA(1);

                    	if ( (LA62_0 == ELSE) )
                    	{
                    	    alt62 = 1;
                    	}
                    	switch (alt62) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:332:5: ^( ELSE expr )
                    	        {
                    	        	Match(input,ELSE,FOLLOW_ELSE_in_caseExpr1897); if (state.failed) return ;

                    	        	if ( (state.backtracking==0) )
                    	        	{
                    	        	   Out(" else "); 
                    	        	}

                    	        	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	        	PushFollow(FOLLOW_expr_in_caseExpr1901);
                    	        	expr();
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        	Match(input, Token.UP, null); if (state.failed) return ;

                    	        }
                    	        break;

                    	}

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" end"); 
                    	}

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;

            }
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "caseExpr"


    // $ANTLR start "aggregate"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:336:1: aggregate : ^(a= AGGREGATE expr ) ;
    public void aggregate() // throws RecognitionException [1]
    {   
        IASTNode a = null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:337:2: ( ^(a= AGGREGATE expr ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:337:4: ^(a= AGGREGATE expr )
            {
            	a=(IASTNode)Match(input,AGGREGATE,FOLLOW_AGGREGATE_in_aggregate1925); if (state.failed) return ;

            	if ( (state.backtracking==0) )
            	{
            	   Out(a); Out("("); 
            	}

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_expr_in_aggregate1930);
            	expr();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	if ( (state.backtracking==0) )
            	{
            	   Out(")"); 
            	}

            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "aggregate"


    // $ANTLR start "methodCall"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:341:1: methodCall : ^(m= METHOD_CALL i= METHOD_NAME ( ^( EXPR_LIST ( arguments )? ) )? ) ;
    public void methodCall() // throws RecognitionException [1]
    {   
        IASTNode m = null;
        IASTNode i = null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:342:2: ( ^(m= METHOD_CALL i= METHOD_NAME ( ^( EXPR_LIST ( arguments )? ) )? ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:342:4: ^(m= METHOD_CALL i= METHOD_NAME ( ^( EXPR_LIST ( arguments )? ) )? )
            {
            	m=(IASTNode)Match(input,METHOD_CALL,FOLLOW_METHOD_CALL_in_methodCall1949); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	i=(IASTNode)Match(input,METHOD_NAME,FOLLOW_METHOD_NAME_in_methodCall1953); if (state.failed) return ;
            	if ( (state.backtracking==0) )
            	{
            	   BeginFunctionTemplate(m,i); 
            	}
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:343:3: ( ^( EXPR_LIST ( arguments )? ) )?
            	int alt65 = 2;
            	int LA65_0 = input.LA(1);

            	if ( (LA65_0 == EXPR_LIST) )
            	{
            	    alt65 = 1;
            	}
            	switch (alt65) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:343:5: ^( EXPR_LIST ( arguments )? )
            	        {
            	        	Match(input,EXPR_LIST,FOLLOW_EXPR_LIST_in_methodCall1962); if (state.failed) return ;

            	        	if ( input.LA(1) == Token.DOWN )
            	        	{
            	        	    Match(input, Token.DOWN, null); if (state.failed) return ;
            	        	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:343:17: ( arguments )?
            	        	    int alt64 = 2;
            	        	    int LA64_0 = input.LA(1);

            	        	    if ( ((LA64_0 >= ALL && LA64_0 <= ANY) || LA64_0 == BETWEEN || LA64_0 == COUNT || LA64_0 == DOT || (LA64_0 >= EXISTS && LA64_0 <= FALSE) || LA64_0 == IN || LA64_0 == LIKE || LA64_0 == NULL || LA64_0 == SELECT || LA64_0 == SOME || (LA64_0 >= TRUE && LA64_0 <= UNION) || LA64_0 == CASE || LA64_0 == AGGREGATE || LA64_0 == CASE2 || (LA64_0 >= INDEX_OP && LA64_0 <= NOT_LIKE) || LA64_0 == UNARY_MINUS || LA64_0 == VECTOR_EXPR || (LA64_0 >= CONSTANT && LA64_0 <= JAVA_CONSTANT) || LA64_0 == EQ || LA64_0 == NE || (LA64_0 >= LT && LA64_0 <= GE) || (LA64_0 >= BNOT && LA64_0 <= DIV) || (LA64_0 >= PARAM && LA64_0 <= IDENT) || LA64_0 == ALIAS_REF || LA64_0 == SQL_TOKEN || LA64_0 == NAMED_PARAM) )
            	        	    {
            	        	        alt64 = 1;
            	        	    }
            	        	    switch (alt64) 
            	        	    {
            	        	        case 1 :
            	        	            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:343:18: arguments
            	        	            {
            	        	            	PushFollow(FOLLOW_arguments_in_methodCall1965);
            	        	            	arguments();
            	        	            	state.followingStackPointer--;
            	        	            	if (state.failed) return ;

            	        	            }
            	        	            break;

            	        	    }


            	        	    Match(input, Token.UP, null); if (state.failed) return ;
            	        	}

            	        }
            	        break;

            	}

            	if ( (state.backtracking==0) )
            	{
            	   EndFunctionTemplate(m); 
            	}

            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "methodCall"


    // $ANTLR start "arguments"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:347:1: arguments : ( expr | comparisonExpr[true] ) ( ( expr | comparisonExpr[true] ) )* ;
    public void arguments() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:348:2: ( ( expr | comparisonExpr[true] ) ( ( expr | comparisonExpr[true] ) )* )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:348:4: ( expr | comparisonExpr[true] ) ( ( expr | comparisonExpr[true] ) )*
            {
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:348:4: ( expr | comparisonExpr[true] )
            	int alt66 = 2;
            	int LA66_0 = input.LA(1);

            	if ( ((LA66_0 >= ALL && LA66_0 <= ANY) || LA66_0 == COUNT || LA66_0 == DOT || LA66_0 == FALSE || LA66_0 == NULL || LA66_0 == SELECT || LA66_0 == SOME || (LA66_0 >= TRUE && LA66_0 <= UNION) || LA66_0 == CASE || LA66_0 == AGGREGATE || LA66_0 == CASE2 || LA66_0 == INDEX_OP || LA66_0 == METHOD_CALL || LA66_0 == UNARY_MINUS || LA66_0 == VECTOR_EXPR || (LA66_0 >= CONSTANT && LA66_0 <= JAVA_CONSTANT) || (LA66_0 >= BNOT && LA66_0 <= DIV) || (LA66_0 >= PARAM && LA66_0 <= IDENT) || LA66_0 == ALIAS_REF || LA66_0 == SQL_TOKEN || LA66_0 == NAMED_PARAM) )
            	{
            	    alt66 = 1;
            	}
            	else if ( (LA66_0 == BETWEEN || LA66_0 == EXISTS || LA66_0 == IN || LA66_0 == LIKE || (LA66_0 >= IS_NOT_NULL && LA66_0 <= IS_NULL) || (LA66_0 >= NOT_BETWEEN && LA66_0 <= NOT_LIKE) || LA66_0 == EQ || LA66_0 == NE || (LA66_0 >= LT && LA66_0 <= GE)) )
            	{
            	    alt66 = 2;
            	}
            	else 
            	{
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d66s0 =
            	        new NoViableAltException("", 66, 0, input);

            	    throw nvae_d66s0;
            	}
            	switch (alt66) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:348:5: expr
            	        {
            	        	PushFollow(FOLLOW_expr_in_arguments1990);
            	        	expr();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;
            	    case 2 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:348:12: comparisonExpr[true]
            	        {
            	        	PushFollow(FOLLOW_comparisonExpr_in_arguments1994);
            	        	comparisonExpr(true);
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:348:34: ( ( expr | comparisonExpr[true] ) )*
            	do 
            	{
            	    int alt68 = 2;
            	    int LA68_0 = input.LA(1);

            	    if ( ((LA68_0 >= ALL && LA68_0 <= ANY) || LA68_0 == BETWEEN || LA68_0 == COUNT || LA68_0 == DOT || (LA68_0 >= EXISTS && LA68_0 <= FALSE) || LA68_0 == IN || LA68_0 == LIKE || LA68_0 == NULL || LA68_0 == SELECT || LA68_0 == SOME || (LA68_0 >= TRUE && LA68_0 <= UNION) || LA68_0 == CASE || LA68_0 == AGGREGATE || LA68_0 == CASE2 || (LA68_0 >= INDEX_OP && LA68_0 <= NOT_LIKE) || LA68_0 == UNARY_MINUS || LA68_0 == VECTOR_EXPR || (LA68_0 >= CONSTANT && LA68_0 <= JAVA_CONSTANT) || LA68_0 == EQ || LA68_0 == NE || (LA68_0 >= LT && LA68_0 <= GE) || (LA68_0 >= BNOT && LA68_0 <= DIV) || (LA68_0 >= PARAM && LA68_0 <= IDENT) || LA68_0 == ALIAS_REF || LA68_0 == SQL_TOKEN || LA68_0 == NAMED_PARAM) )
            	    {
            	        alt68 = 1;
            	    }


            	    switch (alt68) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:348:36: ( expr | comparisonExpr[true] )
            			    {
            			    	if ( (state.backtracking==0) )
            			    	{
            			    	   CommaBetweenParameters(", "); 
            			    	}
            			    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:348:70: ( expr | comparisonExpr[true] )
            			    	int alt67 = 2;
            			    	int LA67_0 = input.LA(1);

            			    	if ( ((LA67_0 >= ALL && LA67_0 <= ANY) || LA67_0 == COUNT || LA67_0 == DOT || LA67_0 == FALSE || LA67_0 == NULL || LA67_0 == SELECT || LA67_0 == SOME || (LA67_0 >= TRUE && LA67_0 <= UNION) || LA67_0 == CASE || LA67_0 == AGGREGATE || LA67_0 == CASE2 || LA67_0 == INDEX_OP || LA67_0 == METHOD_CALL || LA67_0 == UNARY_MINUS || LA67_0 == VECTOR_EXPR || (LA67_0 >= CONSTANT && LA67_0 <= JAVA_CONSTANT) || (LA67_0 >= BNOT && LA67_0 <= DIV) || (LA67_0 >= PARAM && LA67_0 <= IDENT) || LA67_0 == ALIAS_REF || LA67_0 == SQL_TOKEN || LA67_0 == NAMED_PARAM) )
            			    	{
            			    	    alt67 = 1;
            			    	}
            			    	else if ( (LA67_0 == BETWEEN || LA67_0 == EXISTS || LA67_0 == IN || LA67_0 == LIKE || (LA67_0 >= IS_NOT_NULL && LA67_0 <= IS_NULL) || (LA67_0 >= NOT_BETWEEN && LA67_0 <= NOT_LIKE) || LA67_0 == EQ || LA67_0 == NE || (LA67_0 >= LT && LA67_0 <= GE)) )
            			    	{
            			    	    alt67 = 2;
            			    	}
            			    	else 
            			    	{
            			    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            			    	    NoViableAltException nvae_d67s0 =
            			    	        new NoViableAltException("", 67, 0, input);

            			    	    throw nvae_d67s0;
            			    	}
            			    	switch (alt67) 
            			    	{
            			    	    case 1 :
            			    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:348:71: expr
            			    	        {
            			    	        	PushFollow(FOLLOW_expr_in_arguments2003);
            			    	        	expr();
            			    	        	state.followingStackPointer--;
            			    	        	if (state.failed) return ;

            			    	        }
            			    	        break;
            			    	    case 2 :
            			    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:348:78: comparisonExpr[true]
            			    	        {
            			    	        	PushFollow(FOLLOW_comparisonExpr_in_arguments2007);
            			    	        	comparisonExpr(true);
            			    	        	state.followingStackPointer--;
            			    	        	if (state.failed) return ;

            			    	        }
            			    	        break;

            			    	}


            			    }
            			    break;

            			default:
            			    goto loop68;
            	    }
            	} while (true);

            	loop68:
            		;	// Stops C# compiler whining that label 'loop68' has no statements


            }

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "arguments"


    // $ANTLR start "parameter"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:351:1: parameter : (n= NAMED_PARAM | p= PARAM );
    public void parameter() // throws RecognitionException [1]
    {   
        IASTNode n = null;
        IASTNode p = null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:352:2: (n= NAMED_PARAM | p= PARAM )
            int alt69 = 2;
            int LA69_0 = input.LA(1);

            if ( (LA69_0 == NAMED_PARAM) )
            {
                alt69 = 1;
            }
            else if ( (LA69_0 == PARAM) )
            {
                alt69 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d69s0 =
                    new NoViableAltException("", 69, 0, input);

                throw nvae_d69s0;
            }
            switch (alt69) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:352:4: n= NAMED_PARAM
                    {
                    	n=(IASTNode)Match(input,NAMED_PARAM,FOLLOW_NAMED_PARAM_in_parameter2025); if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(n); 
                    	}

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:353:4: p= PARAM
                    {
                    	p=(IASTNode)Match(input,PARAM,FOLLOW_PARAM_in_parameter2034); if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(p); 
                    	}

                    }
                    break;

            }
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "parameter"


    // $ANTLR start "addrExpr"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:356:1: addrExpr : ( ^(r= DOT . . ) | i= ALIAS_REF | ^(j= INDEX_OP ( . )* ) );
    public void addrExpr() // throws RecognitionException [1]
    {   
        IASTNode r = null;
        IASTNode i = null;
        IASTNode j = null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:357:2: ( ^(r= DOT . . ) | i= ALIAS_REF | ^(j= INDEX_OP ( . )* ) )
            int alt71 = 3;
            switch ( input.LA(1) ) 
            {
            case DOT:
            	{
                alt71 = 1;
                }
                break;
            case ALIAS_REF:
            	{
                alt71 = 2;
                }
                break;
            case INDEX_OP:
            	{
                alt71 = 3;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d71s0 =
            	        new NoViableAltException("", 71, 0, input);

            	    throw nvae_d71s0;
            }

            switch (alt71) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:357:4: ^(r= DOT . . )
                    {
                    	r=(IASTNode)Match(input,DOT,FOLLOW_DOT_in_addrExpr2050); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	MatchAny(input); if (state.failed) return ;
                    	MatchAny(input); if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(r); 
                    	}

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:358:4: i= ALIAS_REF
                    {
                    	i=(IASTNode)Match(input,ALIAS_REF,FOLLOW_ALIAS_REF_in_addrExpr2064); if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(i); 
                    	}

                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:359:4: ^(j= INDEX_OP ( . )* )
                    {
                    	j=(IASTNode)Match(input,INDEX_OP,FOLLOW_INDEX_OP_in_addrExpr2074); if (state.failed) return ;

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return ;
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:359:17: ( . )*
                    	    do 
                    	    {
                    	        int alt70 = 2;
                    	        int LA70_0 = input.LA(1);

                    	        if ( ((LA70_0 >= ALL && LA70_0 <= BOGUS)) )
                    	        {
                    	            alt70 = 1;
                    	        }
                    	        else if ( (LA70_0 == UP) )
                    	        {
                    	            alt70 = 2;
                    	        }


                    	        switch (alt70) 
                    	    	{
                    	    		case 1 :
                    	    		    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:359:17: .
                    	    		    {
                    	    		    	MatchAny(input); if (state.failed) return ;

                    	    		    }
                    	    		    break;

                    	    		default:
                    	    		    goto loop70;
                    	        }
                    	    } while (true);

                    	    loop70:
                    	    	;	// Stops C# compiler whining that label 'loop70' has no statements


                    	    Match(input, Token.UP, null); if (state.failed) return ;
                    	}
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(j); 
                    	}

                    }
                    break;

            }
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "addrExpr"


    // $ANTLR start "sqlToken"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:362:1: sqlToken : ^(t= SQL_TOKEN ( . )* ) ;
    public void sqlToken() // throws RecognitionException [1]
    {   
        IASTNode t = null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:363:2: ( ^(t= SQL_TOKEN ( . )* ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:363:4: ^(t= SQL_TOKEN ( . )* )
            {
            	t=(IASTNode)Match(input,SQL_TOKEN,FOLLOW_SQL_TOKEN_in_sqlToken2094); if (state.failed) return ;

            	if ( (state.backtracking==0) )
            	{
            	   Out(t); 
            	}

            	if ( input.LA(1) == Token.DOWN )
            	{
            	    Match(input, Token.DOWN, null); if (state.failed) return ;
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:363:30: ( . )*
            	    do 
            	    {
            	        int alt72 = 2;
            	        int LA72_0 = input.LA(1);

            	        if ( ((LA72_0 >= ALL && LA72_0 <= BOGUS)) )
            	        {
            	            alt72 = 1;
            	        }
            	        else if ( (LA72_0 == UP) )
            	        {
            	            alt72 = 2;
            	        }


            	        switch (alt72) 
            	    	{
            	    		case 1 :
            	    		    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:363:30: .
            	    		    {
            	    		    	MatchAny(input); if (state.failed) return ;

            	    		    }
            	    		    break;

            	    		default:
            	    		    goto loop72;
            	        }
            	    } while (true);

            	    loop72:
            	    	;	// Stops C# compiler whining that label 'loop72' has no statements


            	    Match(input, Token.UP, null); if (state.failed) return ;
            	}

            }

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "sqlToken"

    // $ANTLR start "synpred1_SqlGenerator"
    public void synpred1_SqlGenerator_fragment() {
        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:81:4: ( SQL_TOKEN )
        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:81:5: SQL_TOKEN
        {
        	Match(input,SQL_TOKEN,FOLLOW_SQL_TOKEN_in_synpred1_SqlGenerator328); if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred1_SqlGenerator"

    // $ANTLR start "synpred2_SqlGenerator"
    public void synpred2_SqlGenerator_fragment() {
        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:314:4: ( additiveExpr )
        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:314:5: additiveExpr
        {
        	PushFollow(FOLLOW_additiveExpr_in_synpred2_SqlGenerator1745);
        	additiveExpr();
        	state.followingStackPointer--;
        	if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred2_SqlGenerator"

    // $ANTLR start "synpred3_SqlGenerator"
    public void synpred3_SqlGenerator_fragment() {
        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:315:4: ( bitwiseExpr )
        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:315:5: bitwiseExpr
        {
        	PushFollow(FOLLOW_bitwiseExpr_in_synpred3_SqlGenerator1760);
        	bitwiseExpr();
        	state.followingStackPointer--;
        	if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred3_SqlGenerator"

    // $ANTLR start "synpred4_SqlGenerator"
    public void synpred4_SqlGenerator_fragment() {
        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:321:4: ( arithmeticExpr )
        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/SqlGenerator.g:321:5: arithmeticExpr
        {
        	PushFollow(FOLLOW_arithmeticExpr_in_synpred4_SqlGenerator1789);
        	arithmeticExpr();
        	state.followingStackPointer--;
        	if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred4_SqlGenerator"

    // Delegated rules

   	public bool synpred2_SqlGenerator() 
   	{
   	    state.backtracking++;
   	    int start = input.Mark();
   	    try 
   	    {
   	        synpred2_SqlGenerator_fragment(); // can never throw exception
   	    }
   	    catch (RecognitionException re) 
   	    {
   	        Console.Error.WriteLine("impossible: "+re);
   	    }
   	    bool success = !state.failed;
   	    input.Rewind(start);
   	    state.backtracking--;
   	    state.failed = false;
   	    return success;
   	}
   	public bool synpred4_SqlGenerator() 
   	{
   	    state.backtracking++;
   	    int start = input.Mark();
   	    try 
   	    {
   	        synpred4_SqlGenerator_fragment(); // can never throw exception
   	    }
   	    catch (RecognitionException re) 
   	    {
   	        Console.Error.WriteLine("impossible: "+re);
   	    }
   	    bool success = !state.failed;
   	    input.Rewind(start);
   	    state.backtracking--;
   	    state.failed = false;
   	    return success;
   	}
   	public bool synpred1_SqlGenerator() 
   	{
   	    state.backtracking++;
   	    int start = input.Mark();
   	    try 
   	    {
   	        synpred1_SqlGenerator_fragment(); // can never throw exception
   	    }
   	    catch (RecognitionException re) 
   	    {
   	        Console.Error.WriteLine("impossible: "+re);
   	    }
   	    bool success = !state.failed;
   	    input.Rewind(start);
   	    state.backtracking--;
   	    state.failed = false;
   	    return success;
   	}
   	public bool synpred3_SqlGenerator() 
   	{
   	    state.backtracking++;
   	    int start = input.Mark();
   	    try 
   	    {
   	        synpred3_SqlGenerator_fragment(); // can never throw exception
   	    }
   	    catch (RecognitionException re) 
   	    {
   	        Console.Error.WriteLine("impossible: "+re);
   	    }
   	    bool success = !state.failed;
   	    input.Rewind(start);
   	    state.backtracking--;
   	    state.failed = false;
   	    return success;
   	}


   	protected DFA57 dfa57;
   	protected DFA58 dfa58;
	private void InitializeCyclicDFAs()
	{
    	this.dfa57 = new DFA57(this);
    	this.dfa58 = new DFA58(this);
	    this.dfa57.specialStateTransitionHandler = new DFA.SpecialStateTransitionHandler(DFA57_SpecialStateTransition);
	    this.dfa58.specialStateTransitionHandler = new DFA.SpecialStateTransitionHandler(DFA58_SpecialStateTransition);
	}

    const string DFA57_eotS =
        "\x1f\uffff";
    const string DFA57_eofS =
        "\x1f\uffff";
    const string DFA57_minS =
        "\x01\x04\x06\x00\x18\uffff";
    const string DFA57_maxS =
        "\x01\u0092\x06\x00\x18\uffff";
    const string DFA57_acceptS =
        "\x07\uffff\x01\x03\x15\uffff\x01\x01\x01\x02";
    const string DFA57_specialS =
        "\x01\uffff\x01\x00\x01\x01\x01\x02\x01\x03\x01\x04\x01\x05\x18\uffff}>";
    static readonly string[] DFA57_transitionS = {
            "\x02\x07\x06\uffff\x01\x07\x02\uffff\x01\x07\x04\uffff\x01\x07"+
            "\x12\uffff\x01\x07\x05\uffff\x01\x07\x01\uffff\x01\x07\x01\uffff"+
            "\x02\x07\x04\uffff\x01\x07\x0d\uffff\x01\x07\x02\uffff\x01\x07"+
            "\x03\uffff\x01\x07\x02\uffff\x01\x07\x08\uffff\x01\x07\x01\uffff"+
            "\x01\x07\x01\uffff\x06\x07\x0b\uffff\x01\x06\x01\x04\x01\x05"+
            "\x01\x03\x01\x01\x01\x02\x02\x07\x03\uffff\x03\x07\x0f\uffff"+
            "\x01\x07\x01\uffff\x01\x07\x05\uffff\x01\x07",
            "\x01\uffff",
            "\x01\uffff",
            "\x01\uffff",
            "\x01\uffff",
            "\x01\uffff",
            "\x01\uffff",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            ""
    };

    static readonly short[] DFA57_eot = DFA.UnpackEncodedString(DFA57_eotS);
    static readonly short[] DFA57_eof = DFA.UnpackEncodedString(DFA57_eofS);
    static readonly char[] DFA57_min = DFA.UnpackEncodedStringToUnsignedChars(DFA57_minS);
    static readonly char[] DFA57_max = DFA.UnpackEncodedStringToUnsignedChars(DFA57_maxS);
    static readonly short[] DFA57_accept = DFA.UnpackEncodedString(DFA57_acceptS);
    static readonly short[] DFA57_special = DFA.UnpackEncodedString(DFA57_specialS);
    static readonly short[][] DFA57_transition = DFA.UnpackEncodedStringArray(DFA57_transitionS);

    protected class DFA57 : DFA
    {
        public DFA57(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 57;
            this.eot = DFA57_eot;
            this.eof = DFA57_eof;
            this.min = DFA57_min;
            this.max = DFA57_max;
            this.accept = DFA57_accept;
            this.special = DFA57_special;
            this.transition = DFA57_transition;

        }

        override public string Description
        {
            get { return "312:1: nestedExpr : ( ( additiveExpr )=> additiveExpr | ( bitwiseExpr )=> bitwiseExpr | expr );"; }
        }

    }


    protected internal int DFA57_SpecialStateTransition(DFA dfa, int s, IIntStream _input) //throws NoViableAltException
    {
            ITreeNodeStream input = (ITreeNodeStream)_input;
    	int _s = s;
        switch ( s )
        {
               	case 0 : 
                   	int LA57_1 = input.LA(1);

                   	 
                   	int index57_1 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred2_SqlGenerator()) ) { s = 29; }

                   	else if ( (true) ) { s = 7; }

                   	 
                   	input.Seek(index57_1);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 1 : 
                   	int LA57_2 = input.LA(1);

                   	 
                   	int index57_2 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred2_SqlGenerator()) ) { s = 29; }

                   	else if ( (true) ) { s = 7; }

                   	 
                   	input.Seek(index57_2);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 2 : 
                   	int LA57_3 = input.LA(1);

                   	 
                   	int index57_3 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred3_SqlGenerator()) ) { s = 30; }

                   	else if ( (true) ) { s = 7; }

                   	 
                   	input.Seek(index57_3);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 3 : 
                   	int LA57_4 = input.LA(1);

                   	 
                   	int index57_4 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred3_SqlGenerator()) ) { s = 30; }

                   	else if ( (true) ) { s = 7; }

                   	 
                   	input.Seek(index57_4);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 4 : 
                   	int LA57_5 = input.LA(1);

                   	 
                   	int index57_5 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred3_SqlGenerator()) ) { s = 30; }

                   	else if ( (true) ) { s = 7; }

                   	 
                   	input.Seek(index57_5);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 5 : 
                   	int LA57_6 = input.LA(1);

                   	 
                   	int index57_6 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred3_SqlGenerator()) ) { s = 30; }

                   	else if ( (true) ) { s = 7; }

                   	 
                   	input.Seek(index57_6);
                   	if ( s >= 0 ) return s;
                   	break;
        }
        if (state.backtracking > 0) {state.failed = true; return -1;}
        NoViableAltException nvae57 =
            new NoViableAltException(dfa.Description, 57, _s, input);
        dfa.Error(nvae57);
        throw nvae57;
    }
    const string DFA58_eotS =
        "\x1e\uffff";
    const string DFA58_eofS =
        "\x1e\uffff";
    const string DFA58_minS =
        "\x01\x04\x0b\x00\x12\uffff";
    const string DFA58_maxS =
        "\x01\u0092\x0b\x00\x12\uffff";
    const string DFA58_acceptS =
        "\x0c\uffff\x01\x02\x10\uffff\x01\x01";
    const string DFA58_specialS =
        "\x01\uffff\x01\x00\x01\x01\x01\x02\x01\x03\x01\x04\x01\x05\x01\x06"+
        "\x01\x07\x01\x08\x01\x09\x01\x0a\x12\uffff}>";
    static readonly string[] DFA58_transitionS = {
            "\x02\x0c\x06\uffff\x01\x0c\x02\uffff\x01\x0c\x04\uffff\x01\x0c"+
            "\x12\uffff\x01\x0c\x05\uffff\x01\x0c\x01\uffff\x01\x0c\x01\uffff"+
            "\x02\x0c\x04\uffff\x01\x0a\x0d\uffff\x01\x0c\x02\uffff\x01\x0b"+
            "\x03\uffff\x01\x0c\x02\uffff\x01\x0c\x08\uffff\x01\x09\x01\uffff"+
            "\x01\x0c\x01\uffff\x06\x0c\x0b\uffff\x01\x06\x01\x04\x01\x05"+
            "\x01\x03\x01\x01\x01\x02\x01\x07\x01\x08\x03\uffff\x03\x0c\x0f"+
            "\uffff\x01\x0c\x01\uffff\x01\x0c\x05\uffff\x01\x0c",
            "\x01\uffff",
            "\x01\uffff",
            "\x01\uffff",
            "\x01\uffff",
            "\x01\uffff",
            "\x01\uffff",
            "\x01\uffff",
            "\x01\uffff",
            "\x01\uffff",
            "\x01\uffff",
            "\x01\uffff",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            ""
    };

    static readonly short[] DFA58_eot = DFA.UnpackEncodedString(DFA58_eotS);
    static readonly short[] DFA58_eof = DFA.UnpackEncodedString(DFA58_eofS);
    static readonly char[] DFA58_min = DFA.UnpackEncodedStringToUnsignedChars(DFA58_minS);
    static readonly char[] DFA58_max = DFA.UnpackEncodedStringToUnsignedChars(DFA58_maxS);
    static readonly short[] DFA58_accept = DFA.UnpackEncodedString(DFA58_acceptS);
    static readonly short[] DFA58_special = DFA.UnpackEncodedString(DFA58_specialS);
    static readonly short[][] DFA58_transition = DFA.UnpackEncodedStringArray(DFA58_transitionS);

    protected class DFA58 : DFA
    {
        public DFA58(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 58;
            this.eot = DFA58_eot;
            this.eof = DFA58_eof;
            this.min = DFA58_min;
            this.max = DFA58_max;
            this.accept = DFA58_accept;
            this.special = DFA58_special;
            this.transition = DFA58_transition;

        }

        override public string Description
        {
            get { return "319:1: nestedExprAfterMinusDiv : ( ( arithmeticExpr )=> arithmeticExpr | expr );"; }
        }

    }


    protected internal int DFA58_SpecialStateTransition(DFA dfa, int s, IIntStream _input) //throws NoViableAltException
    {
            ITreeNodeStream input = (ITreeNodeStream)_input;
    	int _s = s;
        switch ( s )
        {
               	case 0 : 
                   	int LA58_1 = input.LA(1);

                   	 
                   	int index58_1 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred4_SqlGenerator()) ) { s = 29; }

                   	else if ( (true) ) { s = 12; }

                   	 
                   	input.Seek(index58_1);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 1 : 
                   	int LA58_2 = input.LA(1);

                   	 
                   	int index58_2 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred4_SqlGenerator()) ) { s = 29; }

                   	else if ( (true) ) { s = 12; }

                   	 
                   	input.Seek(index58_2);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 2 : 
                   	int LA58_3 = input.LA(1);

                   	 
                   	int index58_3 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred4_SqlGenerator()) ) { s = 29; }

                   	else if ( (true) ) { s = 12; }

                   	 
                   	input.Seek(index58_3);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 3 : 
                   	int LA58_4 = input.LA(1);

                   	 
                   	int index58_4 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred4_SqlGenerator()) ) { s = 29; }

                   	else if ( (true) ) { s = 12; }

                   	 
                   	input.Seek(index58_4);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 4 : 
                   	int LA58_5 = input.LA(1);

                   	 
                   	int index58_5 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred4_SqlGenerator()) ) { s = 29; }

                   	else if ( (true) ) { s = 12; }

                   	 
                   	input.Seek(index58_5);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 5 : 
                   	int LA58_6 = input.LA(1);

                   	 
                   	int index58_6 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred4_SqlGenerator()) ) { s = 29; }

                   	else if ( (true) ) { s = 12; }

                   	 
                   	input.Seek(index58_6);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 6 : 
                   	int LA58_7 = input.LA(1);

                   	 
                   	int index58_7 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred4_SqlGenerator()) ) { s = 29; }

                   	else if ( (true) ) { s = 12; }

                   	 
                   	input.Seek(index58_7);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 7 : 
                   	int LA58_8 = input.LA(1);

                   	 
                   	int index58_8 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred4_SqlGenerator()) ) { s = 29; }

                   	else if ( (true) ) { s = 12; }

                   	 
                   	input.Seek(index58_8);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 8 : 
                   	int LA58_9 = input.LA(1);

                   	 
                   	int index58_9 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred4_SqlGenerator()) ) { s = 29; }

                   	else if ( (true) ) { s = 12; }

                   	 
                   	input.Seek(index58_9);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 9 : 
                   	int LA58_10 = input.LA(1);

                   	 
                   	int index58_10 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred4_SqlGenerator()) ) { s = 29; }

                   	else if ( (true) ) { s = 12; }

                   	 
                   	input.Seek(index58_10);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 10 : 
                   	int LA58_11 = input.LA(1);

                   	 
                   	int index58_11 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred4_SqlGenerator()) ) { s = 29; }

                   	else if ( (true) ) { s = 12; }

                   	 
                   	input.Seek(index58_11);
                   	if ( s >= 0 ) return s;
                   	break;
        }
        if (state.backtracking > 0) {state.failed = true; return -1;}
        NoViableAltException nvae58 =
            new NoViableAltException(dfa.Description, 58, _s, input);
        dfa.Error(nvae58);
        throw nvae58;
    }
 

    public static readonly BitSet FOLLOW_selectStatement_in_statement57 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_updateStatement_in_statement62 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_deleteStatement_in_statement67 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_insertStatement_in_statement72 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_SELECT_in_selectStatement84 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_selectClause_in_selectStatement90 = new BitSet(new ulong[]{0x0000000000400000UL});
    public static readonly BitSet FOLLOW_from_in_selectStatement94 = new BitSet(new ulong[]{0x0020020001000008UL});
    public static readonly BitSet FOLLOW_WHERE_in_selectStatement101 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_whereExpr_in_selectStatement105 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_GROUP_in_selectStatement117 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_groupExprs_in_selectStatement121 = new BitSet(new ulong[]{0x0000000002000008UL});
    public static readonly BitSet FOLLOW_HAVING_in_selectStatement126 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_booleanExpr_in_selectStatement130 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_ORDER_in_selectStatement147 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_orderExprs_in_selectStatement151 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_UPDATE_in_updateStatement174 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_FROM_in_updateStatement182 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_fromTable_in_updateStatement184 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_setClause_in_updateStatement190 = new BitSet(new ulong[]{0x0020000000000008UL});
    public static readonly BitSet FOLLOW_whereClause_in_updateStatement195 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_DELETE_in_deleteStatement214 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_from_in_deleteStatement220 = new BitSet(new ulong[]{0x0020000000000008UL});
    public static readonly BitSet FOLLOW_whereClause_in_deleteStatement225 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_INSERT_in_insertStatement242 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_INTO_in_insertStatement251 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_selectStatement_in_insertStatement261 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_SET_in_setClause281 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_comparisonExpr_in_setClause285 = new BitSet(new ulong[]{0x0000000404080408UL,0x00000F4800076000UL});
    public static readonly BitSet FOLLOW_comparisonExpr_in_setClause292 = new BitSet(new ulong[]{0x0000000404080408UL,0x00000F4800076000UL});
    public static readonly BitSet FOLLOW_WHERE_in_whereClause310 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_whereClauseExpr_in_whereClause314 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_conditionList_in_whereClauseExpr333 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_booleanExpr_in_whereClauseExpr338 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_expr_in_orderExprs354 = new BitSet(new ulong[]{0x0086A0800010D132UL,0x071FE003F5009120UL,0x0000000000041400UL});
    public static readonly BitSet FOLLOW_orderDirection_in_orderExprs361 = new BitSet(new ulong[]{0x0086A08000109032UL,0x071FE003F5009120UL,0x0000000000041400UL});
    public static readonly BitSet FOLLOW_orderExprs_in_orderExprs371 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_expr_in_groupExprs386 = new BitSet(new ulong[]{0x0086A08000109032UL,0x071FE003F5009120UL,0x0000000000041400UL});
    public static readonly BitSet FOLLOW_groupExprs_in_groupExprs392 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_set_in_orderDirection0 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_filters_in_whereExpr427 = new BitSet(new ulong[]{0x0000014404080442UL,0x00000F4800076000UL,0x0000000000009000UL});
    public static readonly BitSet FOLLOW_thetaJoins_in_whereExpr435 = new BitSet(new ulong[]{0x0000014404080442UL,0x00000F4800076000UL,0x0000000000001000UL});
    public static readonly BitSet FOLLOW_booleanExpr_in_whereExpr446 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_thetaJoins_in_whereExpr456 = new BitSet(new ulong[]{0x0000014404080442UL,0x00000F4800076000UL,0x0000000000001000UL});
    public static readonly BitSet FOLLOW_booleanExpr_in_whereExpr464 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_booleanExpr_in_whereExpr475 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_FILTERS_in_filters488 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_conditionList_in_filters490 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_THETA_JOINS_in_thetaJoins504 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_conditionList_in_thetaJoins506 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_sqlToken_in_conditionList519 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000000000000UL,0x0000000000001000UL});
    public static readonly BitSet FOLLOW_conditionList_in_conditionList525 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_SELECT_CLAUSE_in_selectClause540 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_distinctOrAll_in_selectClause543 = new BitSet(new ulong[]{0x0082208000109000UL,0x071FE003F10091A0UL,0x0000000000045400UL});
    public static readonly BitSet FOLLOW_selectColumn_in_selectClause549 = new BitSet(new ulong[]{0x0082208000109008UL,0x071FE003F10091A0UL,0x0000000000045400UL});
    public static readonly BitSet FOLLOW_selectExpr_in_selectColumn567 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000000000000UL,0x0000000000002000UL});
    public static readonly BitSet FOLLOW_SELECT_COLUMNS_in_selectColumn572 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_selectAtom_in_selectExpr592 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_count_in_selectExpr599 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_CONSTRUCTOR_in_selectExpr605 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_set_in_selectExpr607 = new BitSet(new ulong[]{0x0082208000109000UL,0x071FE003F10091A0UL,0x0000000000045400UL});
    public static readonly BitSet FOLLOW_selectColumn_in_selectExpr617 = new BitSet(new ulong[]{0x0082208000109008UL,0x071FE003F10091A0UL,0x0000000000045400UL});
    public static readonly BitSet FOLLOW_methodCall_in_selectExpr627 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_aggregate_in_selectExpr632 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_constant_in_selectExpr639 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_arithmeticExpr_in_selectExpr646 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_PARAM_in_selectExpr653 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_selectStatement_in_selectExpr663 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_COUNT_in_count677 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_distinctOrAll_in_count684 = new BitSet(new ulong[]{0x0082008000109000UL,0x071FE003F1409120UL,0x0000000000041400UL});
    public static readonly BitSet FOLLOW_countExpr_in_count690 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_DISTINCT_in_distinctOrAll705 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ALL_in_distinctOrAll713 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_ROW_STAR_in_countExpr732 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_simpleExpr_in_countExpr739 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_DOT_in_selectAtom751 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_SQL_TOKEN_in_selectAtom761 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_ALIAS_REF_in_selectAtom771 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_SELECT_EXPR_in_selectAtom781 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_FROM_in_from804 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_fromTable_in_from811 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000000000000UL,0x0000000000000050UL});
    public static readonly BitSet FOLLOW_FROM_FRAGMENT_in_fromTable837 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_tableJoin_in_fromTable843 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000000000000UL,0x0000000000000050UL});
    public static readonly BitSet FOLLOW_JOIN_FRAGMENT_in_fromTable858 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_tableJoin_in_fromTable864 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000000000000UL,0x0000000000000050UL});
    public static readonly BitSet FOLLOW_JOIN_FRAGMENT_in_tableJoin887 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_tableJoin_in_tableJoin892 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000000000000UL,0x0000000000000050UL});
    public static readonly BitSet FOLLOW_FROM_FRAGMENT_in_tableJoin908 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_tableJoin_in_tableJoin913 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000000000000UL,0x0000000000000050UL});
    public static readonly BitSet FOLLOW_AND_in_booleanOp933 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_booleanExpr_in_booleanOp935 = new BitSet(new ulong[]{0x0000014404080440UL,0x00000F4800076000UL,0x0000000000001000UL});
    public static readonly BitSet FOLLOW_booleanExpr_in_booleanOp940 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_OR_in_booleanOp948 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_booleanExpr_in_booleanOp952 = new BitSet(new ulong[]{0x0000014404080440UL,0x00000F4800076000UL,0x0000000000001000UL});
    public static readonly BitSet FOLLOW_booleanExpr_in_booleanOp957 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NOT_in_booleanOp967 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_booleanExpr_in_booleanOp971 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_booleanOp_in_booleanExpr988 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_comparisonExpr_in_booleanExpr995 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_SQL_TOKEN_in_booleanExpr1004 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_binaryComparisonExpression_in_comparisonExpr1020 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_exoticComparisonExpression_in_comparisonExpr1027 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_EQ_in_binaryComparisonExpression1042 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1044 = new BitSet(new ulong[]{0x0086A08000109030UL,0x071FE003F5009120UL,0x0000000000041400UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1048 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NE_in_binaryComparisonExpression1055 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1057 = new BitSet(new ulong[]{0x0086A08000109030UL,0x071FE003F5009120UL,0x0000000000041400UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1061 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_GT_in_binaryComparisonExpression1068 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1070 = new BitSet(new ulong[]{0x0086A08000109030UL,0x071FE003F5009120UL,0x0000000000041400UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1074 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_GE_in_binaryComparisonExpression1081 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1083 = new BitSet(new ulong[]{0x0086A08000109030UL,0x071FE003F5009120UL,0x0000000000041400UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1087 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_LT_in_binaryComparisonExpression1094 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1096 = new BitSet(new ulong[]{0x0086A08000109030UL,0x071FE003F5009120UL,0x0000000000041400UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1100 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_LE_in_binaryComparisonExpression1107 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1109 = new BitSet(new ulong[]{0x0086A08000109030UL,0x071FE003F5009120UL,0x0000000000041400UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1113 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_LIKE_in_exoticComparisonExpression1127 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1129 = new BitSet(new ulong[]{0x0086A08000109030UL,0x071FE003F5009120UL,0x0000000000041400UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1133 = new BitSet(new ulong[]{0x0000000000040008UL});
    public static readonly BitSet FOLLOW_likeEscape_in_exoticComparisonExpression1135 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NOT_LIKE_in_exoticComparisonExpression1143 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1145 = new BitSet(new ulong[]{0x0086A08000109030UL,0x071FE003F5009120UL,0x0000000000041400UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1149 = new BitSet(new ulong[]{0x0000000000040008UL});
    public static readonly BitSet FOLLOW_likeEscape_in_exoticComparisonExpression1151 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_BETWEEN_in_exoticComparisonExpression1158 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1160 = new BitSet(new ulong[]{0x0086A08000109030UL,0x071FE003F5009120UL,0x0000000000041400UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1164 = new BitSet(new ulong[]{0x0086A08000109030UL,0x071FE003F5009120UL,0x0000000000041400UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1168 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NOT_BETWEEN_in_exoticComparisonExpression1175 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1177 = new BitSet(new ulong[]{0x0086A08000109030UL,0x071FE003F5009120UL,0x0000000000041400UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1181 = new BitSet(new ulong[]{0x0086A08000109030UL,0x071FE003F5009120UL,0x0000000000041400UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1185 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_IN_in_exoticComparisonExpression1192 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1194 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000800UL});
    public static readonly BitSet FOLLOW_inList_in_exoticComparisonExpression1198 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NOT_IN_in_exoticComparisonExpression1206 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1208 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000800UL});
    public static readonly BitSet FOLLOW_inList_in_exoticComparisonExpression1212 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_EXISTS_in_exoticComparisonExpression1220 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_quantified_in_exoticComparisonExpression1224 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_IS_NULL_in_exoticComparisonExpression1232 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1234 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_IS_NOT_NULL_in_exoticComparisonExpression1243 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1245 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_ESCAPE_in_likeEscape1262 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_likeEscape1266 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_IN_LIST_in_inList1282 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_parenSelect_in_inList1288 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_simpleExprList_in_inList1292 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_simpleExpr_in_simpleExprList1313 = new BitSet(new ulong[]{0x0082008000109002UL,0x071FE003F1009120UL,0x0000000000041400UL});
    public static readonly BitSet FOLLOW_simpleExpr_in_expr1332 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_VECTOR_EXPR_in_expr1339 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_expr1346 = new BitSet(new ulong[]{0x0086A08000109038UL,0x071FE003F5009120UL,0x0000000000041400UL});
    public static readonly BitSet FOLLOW_parenSelect_in_expr1361 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ANY_in_expr1367 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_quantified_in_expr1371 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_ALL_in_expr1379 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_quantified_in_expr1383 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_SOME_in_expr1391 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_quantified_in_expr1395 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_sqlToken_in_quantified1413 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_selectStatement_in_quantified1417 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_selectStatement_in_parenSelect1436 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_UNION_in_parenSelect1445 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_selectStatement_in_parenSelect1449 = new BitSet(new ulong[]{0x0004200000000000UL});
    public static readonly BitSet FOLLOW_parenSelect_in_parenSelect1453 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_constant_in_simpleExpr1470 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_NULL_in_simpleExpr1477 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_addrExpr_in_simpleExpr1484 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_sqlToken_in_simpleExpr1489 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_aggregate_in_simpleExpr1494 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_methodCall_in_simpleExpr1499 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_count_in_simpleExpr1504 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_parameter_in_simpleExpr1509 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_arithmeticExpr_in_simpleExpr1514 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_set_in_constant0 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_additiveExpr_in_arithmeticExpr1583 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_bitwiseExpr_in_arithmeticExpr1588 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_multiplicativeExpr_in_arithmeticExpr1593 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_UNARY_MINUS_in_arithmeticExpr1600 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_arithmeticExpr1604 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_caseExpr_in_arithmeticExpr1610 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_PLUS_in_additiveExpr1622 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_additiveExpr1624 = new BitSet(new ulong[]{0x0086A08000109030UL,0x071FE003F5009120UL,0x0000000000041400UL});
    public static readonly BitSet FOLLOW_expr_in_additiveExpr1628 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_MINUS_in_additiveExpr1635 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_additiveExpr1637 = new BitSet(new ulong[]{0x0086A08000109030UL,0x071FE003F5009120UL,0x0000000000041400UL});
    public static readonly BitSet FOLLOW_nestedExprAfterMinusDiv_in_additiveExpr1641 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_BAND_in_bitwiseExpr1654 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_bitwiseExpr1656 = new BitSet(new ulong[]{0x0086A08000109030UL,0x071FE003F5009120UL,0x0000000000041400UL});
    public static readonly BitSet FOLLOW_nestedExpr_in_bitwiseExpr1660 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_BOR_in_bitwiseExpr1667 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_bitwiseExpr1669 = new BitSet(new ulong[]{0x0086A08000109030UL,0x071FE003F5009120UL,0x0000000000041400UL});
    public static readonly BitSet FOLLOW_nestedExpr_in_bitwiseExpr1673 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_BXOR_in_bitwiseExpr1680 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_bitwiseExpr1682 = new BitSet(new ulong[]{0x0086A08000109030UL,0x071FE003F5009120UL,0x0000000000041400UL});
    public static readonly BitSet FOLLOW_nestedExpr_in_bitwiseExpr1686 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_BNOT_in_bitwiseExpr1693 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_nestedExpr_in_bitwiseExpr1697 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_STAR_in_multiplicativeExpr1711 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_nestedExpr_in_multiplicativeExpr1713 = new BitSet(new ulong[]{0x0086A08000109030UL,0x071FE003F5009120UL,0x0000000000041400UL});
    public static readonly BitSet FOLLOW_nestedExpr_in_multiplicativeExpr1717 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_DIV_in_multiplicativeExpr1724 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_nestedExpr_in_multiplicativeExpr1726 = new BitSet(new ulong[]{0x0086A08000109030UL,0x071FE003F5009120UL,0x0000000000041400UL});
    public static readonly BitSet FOLLOW_nestedExprAfterMinusDiv_in_multiplicativeExpr1730 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_additiveExpr_in_nestedExpr1752 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_bitwiseExpr_in_nestedExpr1767 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_expr_in_nestedExpr1774 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_arithmeticExpr_in_nestedExprAfterMinusDiv1796 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_expr_in_nestedExprAfterMinusDiv1803 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_CASE_in_caseExpr1815 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_WHEN_in_caseExpr1825 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_booleanExpr_in_caseExpr1829 = new BitSet(new ulong[]{0x0086A08000109030UL,0x071FE003F5009120UL,0x0000000000041400UL});
    public static readonly BitSet FOLLOW_expr_in_caseExpr1834 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_ELSE_in_caseExpr1846 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_caseExpr1850 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_CASE2_in_caseExpr1866 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_caseExpr1870 = new BitSet(new ulong[]{0x0800000000000000UL});
    public static readonly BitSet FOLLOW_WHEN_in_caseExpr1877 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_caseExpr1881 = new BitSet(new ulong[]{0x0086A08000109030UL,0x071FE003F5009120UL,0x0000000000041400UL});
    public static readonly BitSet FOLLOW_expr_in_caseExpr1885 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_ELSE_in_caseExpr1897 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_caseExpr1901 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_AGGREGATE_in_aggregate1925 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_aggregate1930 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_METHOD_CALL_in_methodCall1949 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_METHOD_NAME_in_methodCall1953 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000000000200UL});
    public static readonly BitSet FOLLOW_EXPR_LIST_in_methodCall1962 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_arguments_in_methodCall1965 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_expr_in_arguments1990 = new BitSet(new ulong[]{0x0086A08404189432UL,0x071FEF4BF507F120UL,0x0000000000041400UL});
    public static readonly BitSet FOLLOW_comparisonExpr_in_arguments1994 = new BitSet(new ulong[]{0x0086A08404189432UL,0x071FEF4BF507F120UL,0x0000000000041400UL});
    public static readonly BitSet FOLLOW_expr_in_arguments2003 = new BitSet(new ulong[]{0x0086A08404189432UL,0x071FEF4BF507F120UL,0x0000000000041400UL});
    public static readonly BitSet FOLLOW_comparisonExpr_in_arguments2007 = new BitSet(new ulong[]{0x0086A08404189432UL,0x071FEF4BF507F120UL,0x0000000000041400UL});
    public static readonly BitSet FOLLOW_NAMED_PARAM_in_parameter2025 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_PARAM_in_parameter2034 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_DOT_in_addrExpr2050 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_ALIAS_REF_in_addrExpr2064 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_INDEX_OP_in_addrExpr2074 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_SQL_TOKEN_in_sqlToken2094 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_SQL_TOKEN_in_synpred1_SqlGenerator328 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_additiveExpr_in_synpred2_SqlGenerator1745 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_bitwiseExpr_in_synpred3_SqlGenerator1760 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_arithmeticExpr_in_synpred4_SqlGenerator1789 = new BitSet(new ulong[]{0x0000000000000002UL});

}
}