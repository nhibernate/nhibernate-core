// $ANTLR 3.1.2 /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g 2009-04-07 16:32:59

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
    public const int MINUS = 110;
    public const int AS = 7;
    public const int END = 56;
    public const int INTO = 30;
    public const int NAMED_PARAM = 142;
    public const int FROM_FRAGMENT = 128;
    public const int FALSE = 20;
    public const int ELEMENTS = 17;
    public const int THEN = 58;
    public const int FILTERS = 140;
    public const int ALIAS = 70;
    public const int ALIAS_REF = 134;
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
    public const int RIGHT_OUTER = 133;
    public const int BETWEEN = 10;
    public const int NUM_INT = 93;
    public const int SQL_TOKEN = 136;
    public const int LEFT_OUTER = 132;
    public const int BOTH = 62;
    public const int METHOD_NAME = 141;
    public const int PLUS = 109;
    public const int VERSIONED = 52;
    public const int MEMBER = 65;
    public const int UNION = 50;
    public const int DISTINCT = 16;
    public const int RANGE = 85;
    public const int FILTER_ENTITY = 74;
    public const int IDENT = 118;
    public const int WHEN = 59;
    public const int DESCENDING = 14;
    public const int WS = 122;
    public const int NEW = 37;
    public const int EQ = 99;
    public const int LT = 104;
    public const int ESCqs = 121;
    public const int OF = 67;
    public const int UPDATE = 51;
    public const int SELECT_FROM = 87;
    public const int LITERAL_by = 54;
    public const int FLOAT_SUFFIX = 124;
    public const int ANY = 5;
    public const int UNARY_PLUS = 89;
    public const int NUM_FLOAT = 95;
    public const int GE = 107;
    public const int CASE = 55;
    public const int OPEN_BRACKET = 113;
    public const int ELSE = 57;
    public const int OPEN = 100;
    public const int COUNT = 12;
    public const int NULL = 39;
    public const int THETA_JOINS = 139;
    public const int IMPLIED_FROM = 129;
    public const int COLON = 115;
    public const int DIV = 112;
    public const int HAVING = 25;
    public const int ALL = 4;
    public const int SET = 46;
    public const int INSERT = 29;
    public const int TRUE = 49;
    public const int CASE2 = 72;
    public const int IS_NOT_NULL = 77;
    public const int WHERE = 53;
    public const int AGGREGATE = 69;
    public const int VECTOR_EXPR = 90;
    public const int LEADING = 64;
    public const int CLOSE_BRACKET = 114;
    public const int NUM_DOUBLE = 94;
    public const int T__126 = 126;
    public const int INNER = 28;
    public const int QUERY = 84;
    public const int ORDER_ELEMENT = 83;
    public const int SELECT_EXPR = 138;
    public const int OR = 40;
    public const int JOIN_FRAGMENT = 130;
    public const int FULL = 23;
    public const int INDICES = 27;
    public const int IS_NULL = 78;
    public const int GROUP = 24;
    public const int T__127 = 127;
    public const int ESCAPE = 18;
    public const int PARAM = 116;
    public const int INDEX_OP = 76;
    public const int ID_LETTER = 120;
    public const int HEX_DIGIT = 125;
    public const int LEFT = 33;
    public const int TRAILING = 68;
    public const int JOIN = 32;
    public const int NOT_BETWEEN = 80;
    public const int SUM = 48;
    public const int ROW_STAR = 86;
    public const int OUTER = 42;
    public const int FROM = 22;
    public const int NOT_IN = 81;
    public const int DELETE = 13;
    public const int OBJECT = 66;
    public const int MAX = 35;
    public const int QUOTED_String = 117;
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
    public const int PROPERTY_REF = 135;
    public const int CLASS = 11;
    public const int SOME = 47;
    public const int SELECT_COLUMNS = 137;
    public const int EXPONENT = 123;
    public const int ID_START_LETTER = 119;
    public const int BOGUS = 143;
    public const int EOF = -1;
    public const int CLOSE = 101;
    public const int AVG = 9;
    public const int SELECT_CLAUSE = 131;
    public const int STAR = 111;
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
		get { return "/Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g"; }
    }



    // $ANTLR start "statement"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:27:1: statement : ( selectStatement | updateStatement | deleteStatement | insertStatement );
    public void statement() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:28:2: ( selectStatement | updateStatement | deleteStatement | insertStatement )
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
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:28:4: selectStatement
                    {
                    	PushFollow(FOLLOW_selectStatement_in_statement57);
                    	selectStatement();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:29:4: updateStatement
                    {
                    	PushFollow(FOLLOW_updateStatement_in_statement62);
                    	updateStatement();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:30:4: deleteStatement
                    {
                    	PushFollow(FOLLOW_deleteStatement_in_statement67);
                    	deleteStatement();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 4 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:31:4: insertStatement
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:34:1: selectStatement : ^( SELECT selectClause from ( ^( WHERE whereExpr ) )? ( ^( GROUP groupExprs ( ^( HAVING booleanExpr[false] ) )? ) )? ( ^( ORDER orderExprs ) )? ) ;
    public void selectStatement() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:35:2: ( ^( SELECT selectClause from ( ^( WHERE whereExpr ) )? ( ^( GROUP groupExprs ( ^( HAVING booleanExpr[false] ) )? ) )? ( ^( ORDER orderExprs ) )? ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:35:4: ^( SELECT selectClause from ( ^( WHERE whereExpr ) )? ( ^( GROUP groupExprs ( ^( HAVING booleanExpr[false] ) )? ) )? ( ^( ORDER orderExprs ) )? )
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
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:38:3: ( ^( WHERE whereExpr ) )?
            	int alt2 = 2;
            	int LA2_0 = input.LA(1);

            	if ( (LA2_0 == WHERE) )
            	{
            	    alt2 = 1;
            	}
            	switch (alt2) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:38:5: ^( WHERE whereExpr )
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

            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:39:3: ( ^( GROUP groupExprs ( ^( HAVING booleanExpr[false] ) )? ) )?
            	int alt4 = 2;
            	int LA4_0 = input.LA(1);

            	if ( (LA4_0 == GROUP) )
            	{
            	    alt4 = 1;
            	}
            	switch (alt4) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:39:5: ^( GROUP groupExprs ( ^( HAVING booleanExpr[false] ) )? )
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
            	        	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:39:47: ( ^( HAVING booleanExpr[false] ) )?
            	        	int alt3 = 2;
            	        	int LA3_0 = input.LA(1);

            	        	if ( (LA3_0 == HAVING) )
            	        	{
            	        	    alt3 = 1;
            	        	}
            	        	switch (alt3) 
            	        	{
            	        	    case 1 :
            	        	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:39:49: ^( HAVING booleanExpr[false] )
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

            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:40:3: ( ^( ORDER orderExprs ) )?
            	int alt5 = 2;
            	int LA5_0 = input.LA(1);

            	if ( (LA5_0 == ORDER) )
            	{
            	    alt5 = 1;
            	}
            	switch (alt5) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:40:5: ^( ORDER orderExprs )
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:47:1: updateStatement : ^( UPDATE ^( FROM fromTable ) setClause ( whereClause )? ) ;
    public void updateStatement() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:48:2: ( ^( UPDATE ^( FROM fromTable ) setClause ( whereClause )? ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:48:4: ^( UPDATE ^( FROM fromTable ) setClause ( whereClause )? )
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
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:51:3: ( whereClause )?
            	int alt6 = 2;
            	int LA6_0 = input.LA(1);

            	if ( (LA6_0 == WHERE) )
            	{
            	    alt6 = 1;
            	}
            	switch (alt6) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:51:4: whereClause
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:55:1: deleteStatement : ^( DELETE from ( whereClause )? ) ;
    public void deleteStatement() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:57:2: ( ^( DELETE from ( whereClause )? ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:57:4: ^( DELETE from ( whereClause )? )
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
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:59:3: ( whereClause )?
            	int alt7 = 2;
            	int LA7_0 = input.LA(1);

            	if ( (LA7_0 == WHERE) )
            	{
            	    alt7 = 1;
            	}
            	switch (alt7) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:59:4: whereClause
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:63:1: insertStatement : ^( INSERT i= INTO selectStatement ) ;
    public void insertStatement() // throws RecognitionException [1]
    {   
        IASTNode i = null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:64:2: ( ^( INSERT i= INTO selectStatement ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:64:4: ^( INSERT i= INTO selectStatement )
            {
            	Match(input,INSERT,FOLLOW_INSERT_in_insertStatement242); if (state.failed) return ;

            	if ( (state.backtracking==0) )
            	{
            	   Out( "insert " ); 
            	}

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	i=(IASTNode)Match(input,INTO,FOLLOW_INTO_in_insertStatement250); if (state.failed) return ;
            	if ( (state.backtracking==0) )
            	{
            	   Out( i ); Out( " " ); 
            	}
            	PushFollow(FOLLOW_selectStatement_in_insertStatement256);
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:70:1: setClause : ^( SET comparisonExpr[false] ( comparisonExpr[false] )* ) ;
    public void setClause() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:73:2: ( ^( SET comparisonExpr[false] ( comparisonExpr[false] )* ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:73:4: ^( SET comparisonExpr[false] ( comparisonExpr[false] )* )
            {
            	Match(input,SET,FOLLOW_SET_in_setClause276); if (state.failed) return ;

            	if ( (state.backtracking==0) )
            	{
            	   Out(" set "); 
            	}

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_comparisonExpr_in_setClause280);
            	comparisonExpr(false);
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:73:51: ( comparisonExpr[false] )*
            	do 
            	{
            	    int alt8 = 2;
            	    int LA8_0 = input.LA(1);

            	    if ( (LA8_0 == BETWEEN || LA8_0 == EXISTS || LA8_0 == IN || LA8_0 == LIKE || (LA8_0 >= IS_NOT_NULL && LA8_0 <= IS_NULL) || (LA8_0 >= NOT_BETWEEN && LA8_0 <= NOT_LIKE) || LA8_0 == EQ || LA8_0 == NE || (LA8_0 >= LT && LA8_0 <= GE)) )
            	    {
            	        alt8 = 1;
            	    }


            	    switch (alt8) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:73:53: comparisonExpr[false]
            			    {
            			    	if ( (state.backtracking==0) )
            			    	{
            			    	   Out(", "); 
            			    	}
            			    	PushFollow(FOLLOW_comparisonExpr_in_setClause287);
            			    	comparisonExpr(false);
            			    	state.followingStackPointer--;
            			    	if (state.failed) return ;

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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:76:1: whereClause : ^( WHERE whereClauseExpr ) ;
    public void whereClause() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:77:2: ( ^( WHERE whereClauseExpr ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:77:4: ^( WHERE whereClauseExpr )
            {
            	Match(input,WHERE,FOLLOW_WHERE_in_whereClause305); if (state.failed) return ;

            	if ( (state.backtracking==0) )
            	{
            	   Out(" where "); 
            	}

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_whereClauseExpr_in_whereClause309);
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:80:1: whereClauseExpr : ( ( SQL_TOKEN )=> conditionList | booleanExpr[ false ] );
    public void whereClauseExpr() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:81:2: ( ( SQL_TOKEN )=> conditionList | booleanExpr[ false ] )
            int alt9 = 2;
            int LA9_0 = input.LA(1);

            if ( (LA9_0 == SQL_TOKEN) )
            {
                int LA9_1 = input.LA(2);

                if ( (LA9_1 == DOWN) && (synpred1_SqlGenerator()) )
                {
                    alt9 = 1;
                }
                else if ( (LA9_1 == UP) )
                {
                    alt9 = 2;
                }
                else 
                {
                    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    NoViableAltException nvae_d9s1 =
                        new NoViableAltException("", 9, 1, input);

                    throw nvae_d9s1;
                }
            }
            else if ( (LA9_0 == AND || LA9_0 == BETWEEN || LA9_0 == EXISTS || LA9_0 == IN || LA9_0 == LIKE || LA9_0 == NOT || LA9_0 == OR || (LA9_0 >= IS_NOT_NULL && LA9_0 <= IS_NULL) || (LA9_0 >= NOT_BETWEEN && LA9_0 <= NOT_LIKE) || LA9_0 == EQ || LA9_0 == NE || (LA9_0 >= LT && LA9_0 <= GE)) )
            {
                alt9 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d9s0 =
                    new NoViableAltException("", 9, 0, input);

                throw nvae_d9s0;
            }
            switch (alt9) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:81:4: ( SQL_TOKEN )=> conditionList
                    {
                    	PushFollow(FOLLOW_conditionList_in_whereClauseExpr328);
                    	conditionList();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:82:4: booleanExpr[ false ]
                    {
                    	PushFollow(FOLLOW_booleanExpr_in_whereClauseExpr333);
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:85:1: orderExprs : ( expr ) (dir= orderDirection )? ( orderExprs )? ;
    public void orderExprs() // throws RecognitionException [1]
    {   
        SqlGenerator.orderDirection_return dir = default(SqlGenerator.orderDirection_return);


        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:87:2: ( ( expr ) (dir= orderDirection )? ( orderExprs )? )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:87:4: ( expr ) (dir= orderDirection )? ( orderExprs )?
            {
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:87:4: ( expr )
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:87:6: expr
            	{
            		PushFollow(FOLLOW_expr_in_orderExprs349);
            		expr();
            		state.followingStackPointer--;
            		if (state.failed) return ;

            	}

            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:87:13: (dir= orderDirection )?
            	int alt10 = 2;
            	int LA10_0 = input.LA(1);

            	if ( (LA10_0 == ASCENDING || LA10_0 == DESCENDING) )
            	{
            	    alt10 = 1;
            	}
            	switch (alt10) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:87:14: dir= orderDirection
            	        {
            	        	PushFollow(FOLLOW_orderDirection_in_orderExprs356);
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

            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:87:66: ( orderExprs )?
            	int alt11 = 2;
            	int LA11_0 = input.LA(1);

            	if ( ((LA11_0 >= ALL && LA11_0 <= ANY) || LA11_0 == COUNT || LA11_0 == DOT || LA11_0 == FALSE || LA11_0 == NULL || LA11_0 == SELECT || LA11_0 == SOME || LA11_0 == TRUE || LA11_0 == CASE || LA11_0 == AGGREGATE || LA11_0 == CASE2 || LA11_0 == INDEX_OP || LA11_0 == METHOD_CALL || LA11_0 == UNARY_MINUS || LA11_0 == VECTOR_EXPR || (LA11_0 >= CONSTANT && LA11_0 <= JAVA_CONSTANT) || (LA11_0 >= PLUS && LA11_0 <= DIV) || (LA11_0 >= PARAM && LA11_0 <= IDENT) || LA11_0 == ALIAS_REF || LA11_0 == SQL_TOKEN || LA11_0 == NAMED_PARAM) )
            	{
            	    alt11 = 1;
            	}
            	switch (alt11) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:87:68: orderExprs
            	        {
            	        	if ( (state.backtracking==0) )
            	        	{
            	        	  Out(", "); 
            	        	}
            	        	PushFollow(FOLLOW_orderExprs_in_orderExprs366);
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:90:1: groupExprs : expr ( groupExprs )? ;
    public void groupExprs() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:92:2: ( expr ( groupExprs )? )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:92:4: expr ( groupExprs )?
            {
            	PushFollow(FOLLOW_expr_in_groupExprs381);
            	expr();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:92:9: ( groupExprs )?
            	int alt12 = 2;
            	int LA12_0 = input.LA(1);

            	if ( ((LA12_0 >= ALL && LA12_0 <= ANY) || LA12_0 == COUNT || LA12_0 == DOT || LA12_0 == FALSE || LA12_0 == NULL || LA12_0 == SELECT || LA12_0 == SOME || LA12_0 == TRUE || LA12_0 == CASE || LA12_0 == AGGREGATE || LA12_0 == CASE2 || LA12_0 == INDEX_OP || LA12_0 == METHOD_CALL || LA12_0 == UNARY_MINUS || LA12_0 == VECTOR_EXPR || (LA12_0 >= CONSTANT && LA12_0 <= JAVA_CONSTANT) || (LA12_0 >= PLUS && LA12_0 <= DIV) || (LA12_0 >= PARAM && LA12_0 <= IDENT) || LA12_0 == ALIAS_REF || LA12_0 == SQL_TOKEN || LA12_0 == NAMED_PARAM) )
            	{
            	    alt12 = 1;
            	}
            	switch (alt12) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:92:11: groupExprs
            	        {
            	        	if ( (state.backtracking==0) )
            	        	{
            	        	  Out(" , "); 
            	        	}
            	        	PushFollow(FOLLOW_groupExprs_in_groupExprs387);
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:95:1: orderDirection : ( ASCENDING | DESCENDING );
    public SqlGenerator.orderDirection_return orderDirection() // throws RecognitionException [1]
    {   
        SqlGenerator.orderDirection_return retval = new SqlGenerator.orderDirection_return();
        retval.Start = input.LT(1);

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:96:2: ( ASCENDING | DESCENDING )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:100:1: whereExpr : ( filters ( thetaJoins )? ( booleanExpr[ true ] )? | thetaJoins ( booleanExpr[ true ] )? | booleanExpr[false] );
    public void whereExpr() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:104:2: ( filters ( thetaJoins )? ( booleanExpr[ true ] )? | thetaJoins ( booleanExpr[ true ] )? | booleanExpr[false] )
            int alt16 = 3;
            switch ( input.LA(1) ) 
            {
            case FILTERS:
            	{
                alt16 = 1;
                }
                break;
            case THETA_JOINS:
            	{
                alt16 = 2;
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
                alt16 = 3;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d16s0 =
            	        new NoViableAltException("", 16, 0, input);

            	    throw nvae_d16s0;
            }

            switch (alt16) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:104:4: filters ( thetaJoins )? ( booleanExpr[ true ] )?
                    {
                    	PushFollow(FOLLOW_filters_in_whereExpr422);
                    	filters();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:105:3: ( thetaJoins )?
                    	int alt13 = 2;
                    	int LA13_0 = input.LA(1);

                    	if ( (LA13_0 == THETA_JOINS) )
                    	{
                    	    alt13 = 1;
                    	}
                    	switch (alt13) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:105:5: thetaJoins
                    	        {
                    	        	if ( (state.backtracking==0) )
                    	        	{
                    	        	   Out(" and "); 
                    	        	}
                    	        	PushFollow(FOLLOW_thetaJoins_in_whereExpr430);
                    	        	thetaJoins();
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;

                    	}

                    	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:106:3: ( booleanExpr[ true ] )?
                    	int alt14 = 2;
                    	int LA14_0 = input.LA(1);

                    	if ( (LA14_0 == AND || LA14_0 == BETWEEN || LA14_0 == EXISTS || LA14_0 == IN || LA14_0 == LIKE || LA14_0 == NOT || LA14_0 == OR || (LA14_0 >= IS_NOT_NULL && LA14_0 <= IS_NULL) || (LA14_0 >= NOT_BETWEEN && LA14_0 <= NOT_LIKE) || LA14_0 == EQ || LA14_0 == NE || (LA14_0 >= LT && LA14_0 <= GE) || LA14_0 == SQL_TOKEN) )
                    	{
                    	    alt14 = 1;
                    	}
                    	switch (alt14) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:106:5: booleanExpr[ true ]
                    	        {
                    	        	if ( (state.backtracking==0) )
                    	        	{
                    	        	   Out(" and "); 
                    	        	}
                    	        	PushFollow(FOLLOW_booleanExpr_in_whereExpr441);
                    	        	booleanExpr(true);
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;

                    	}


                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:107:4: thetaJoins ( booleanExpr[ true ] )?
                    {
                    	PushFollow(FOLLOW_thetaJoins_in_whereExpr451);
                    	thetaJoins();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:108:3: ( booleanExpr[ true ] )?
                    	int alt15 = 2;
                    	int LA15_0 = input.LA(1);

                    	if ( (LA15_0 == AND || LA15_0 == BETWEEN || LA15_0 == EXISTS || LA15_0 == IN || LA15_0 == LIKE || LA15_0 == NOT || LA15_0 == OR || (LA15_0 >= IS_NOT_NULL && LA15_0 <= IS_NULL) || (LA15_0 >= NOT_BETWEEN && LA15_0 <= NOT_LIKE) || LA15_0 == EQ || LA15_0 == NE || (LA15_0 >= LT && LA15_0 <= GE) || LA15_0 == SQL_TOKEN) )
                    	{
                    	    alt15 = 1;
                    	}
                    	switch (alt15) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:108:5: booleanExpr[ true ]
                    	        {
                    	        	if ( (state.backtracking==0) )
                    	        	{
                    	        	   Out(" and "); 
                    	        	}
                    	        	PushFollow(FOLLOW_booleanExpr_in_whereExpr459);
                    	        	booleanExpr(true);
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;

                    	}


                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:109:4: booleanExpr[false]
                    {
                    	PushFollow(FOLLOW_booleanExpr_in_whereExpr470);
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:112:1: filters : ^( FILTERS conditionList ) ;
    public void filters() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:113:2: ( ^( FILTERS conditionList ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:113:4: ^( FILTERS conditionList )
            {
            	Match(input,FILTERS,FOLLOW_FILTERS_in_filters483); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_conditionList_in_filters485);
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:116:1: thetaJoins : ^( THETA_JOINS conditionList ) ;
    public void thetaJoins() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:117:2: ( ^( THETA_JOINS conditionList ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:117:4: ^( THETA_JOINS conditionList )
            {
            	Match(input,THETA_JOINS,FOLLOW_THETA_JOINS_in_thetaJoins499); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_conditionList_in_thetaJoins501);
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:120:1: conditionList : sqlToken ( conditionList )? ;
    public void conditionList() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:121:2: ( sqlToken ( conditionList )? )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:121:4: sqlToken ( conditionList )?
            {
            	PushFollow(FOLLOW_sqlToken_in_conditionList514);
            	sqlToken();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:121:13: ( conditionList )?
            	int alt17 = 2;
            	int LA17_0 = input.LA(1);

            	if ( (LA17_0 == SQL_TOKEN) )
            	{
            	    alt17 = 1;
            	}
            	switch (alt17) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:121:15: conditionList
            	        {
            	        	if ( (state.backtracking==0) )
            	        	{
            	        	   Out(" and "); 
            	        	}
            	        	PushFollow(FOLLOW_conditionList_in_conditionList520);
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:124:1: selectClause : ^( SELECT_CLAUSE ( distinctOrAll )? ( selectColumn )+ ) ;
    public void selectClause() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:125:2: ( ^( SELECT_CLAUSE ( distinctOrAll )? ( selectColumn )+ ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:125:4: ^( SELECT_CLAUSE ( distinctOrAll )? ( selectColumn )+ )
            {
            	Match(input,SELECT_CLAUSE,FOLLOW_SELECT_CLAUSE_in_selectClause535); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:125:20: ( distinctOrAll )?
            	int alt18 = 2;
            	int LA18_0 = input.LA(1);

            	if ( (LA18_0 == ALL || LA18_0 == DISTINCT) )
            	{
            	    alt18 = 1;
            	}
            	switch (alt18) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:125:21: distinctOrAll
            	        {
            	        	PushFollow(FOLLOW_distinctOrAll_in_selectClause538);
            	        	distinctOrAll();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}

            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:125:37: ( selectColumn )+
            	int cnt19 = 0;
            	do 
            	{
            	    int alt19 = 2;
            	    int LA19_0 = input.LA(1);

            	    if ( (LA19_0 == COUNT || LA19_0 == DOT || LA19_0 == FALSE || LA19_0 == SELECT || LA19_0 == TRUE || LA19_0 == CASE || LA19_0 == AGGREGATE || (LA19_0 >= CONSTRUCTOR && LA19_0 <= CASE2) || LA19_0 == METHOD_CALL || LA19_0 == UNARY_MINUS || (LA19_0 >= CONSTANT && LA19_0 <= JAVA_CONSTANT) || (LA19_0 >= PLUS && LA19_0 <= DIV) || (LA19_0 >= PARAM && LA19_0 <= IDENT) || LA19_0 == ALIAS_REF || LA19_0 == SQL_TOKEN || LA19_0 == SELECT_EXPR) )
            	    {
            	        alt19 = 1;
            	    }


            	    switch (alt19) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:125:39: selectColumn
            			    {
            			    	PushFollow(FOLLOW_selectColumn_in_selectClause544);
            			    	selectColumn();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return ;

            			    }
            			    break;

            			default:
            			    if ( cnt19 >= 1 ) goto loop19;
            			    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            		            EarlyExitException eee19 =
            		                new EarlyExitException(19, input);
            		            throw eee19;
            	    }
            	    cnt19++;
            	} while (true);

            	loop19:
            		;	// Stops C# compiler whinging that label 'loop19' has no statements


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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:128:1: selectColumn : p= selectExpr (sc= SELECT_COLUMNS )? ;
    public void selectColumn() // throws RecognitionException [1]
    {   
        IASTNode sc = null;
        SqlGenerator.selectExpr_return p = default(SqlGenerator.selectExpr_return);


        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:129:2: (p= selectExpr (sc= SELECT_COLUMNS )? )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:129:4: p= selectExpr (sc= SELECT_COLUMNS )?
            {
            	PushFollow(FOLLOW_selectExpr_in_selectColumn562);
            	p = selectExpr();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:129:17: (sc= SELECT_COLUMNS )?
            	int alt20 = 2;
            	int LA20_0 = input.LA(1);

            	if ( (LA20_0 == SELECT_COLUMNS) )
            	{
            	    alt20 = 1;
            	}
            	switch (alt20) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:129:18: sc= SELECT_COLUMNS
            	        {
            	        	sc=(IASTNode)Match(input,SELECT_COLUMNS,FOLLOW_SELECT_COLUMNS_in_selectColumn567); if (state.failed) return ;
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:132:1: selectExpr : (e= selectAtom | count | ^( CONSTRUCTOR ( DOT | IDENT ) ( selectColumn )+ ) | methodCall | aggregate | c= constant | arithmeticExpr | param= PARAM | selectStatement );
    public SqlGenerator.selectExpr_return selectExpr() // throws RecognitionException [1]
    {   
        SqlGenerator.selectExpr_return retval = new SqlGenerator.selectExpr_return();
        retval.Start = input.LT(1);

        IASTNode param = null;
        SqlGenerator.selectAtom_return e = default(SqlGenerator.selectAtom_return);

        SqlGenerator.constant_return c = default(SqlGenerator.constant_return);


        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:133:2: (e= selectAtom | count | ^( CONSTRUCTOR ( DOT | IDENT ) ( selectColumn )+ ) | methodCall | aggregate | c= constant | arithmeticExpr | param= PARAM | selectStatement )
            int alt22 = 9;
            switch ( input.LA(1) ) 
            {
            case DOT:
            case ALIAS_REF:
            case SQL_TOKEN:
            case SELECT_EXPR:
            	{
                alt22 = 1;
                }
                break;
            case COUNT:
            	{
                alt22 = 2;
                }
                break;
            case CONSTRUCTOR:
            	{
                alt22 = 3;
                }
                break;
            case METHOD_CALL:
            	{
                alt22 = 4;
                }
                break;
            case AGGREGATE:
            	{
                alt22 = 5;
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
                alt22 = 6;
                }
                break;
            case CASE:
            case CASE2:
            case UNARY_MINUS:
            case PLUS:
            case MINUS:
            case STAR:
            case DIV:
            	{
                alt22 = 7;
                }
                break;
            case PARAM:
            	{
                alt22 = 8;
                }
                break;
            case SELECT:
            	{
                alt22 = 9;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
            	    NoViableAltException nvae_d22s0 =
            	        new NoViableAltException("", 22, 0, input);

            	    throw nvae_d22s0;
            }

            switch (alt22) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:133:4: e= selectAtom
                    {
                    	PushFollow(FOLLOW_selectAtom_in_selectExpr587);
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
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:134:4: count
                    {
                    	PushFollow(FOLLOW_count_in_selectExpr594);
                    	count();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:135:4: ^( CONSTRUCTOR ( DOT | IDENT ) ( selectColumn )+ )
                    {
                    	Match(input,CONSTRUCTOR,FOLLOW_CONSTRUCTOR_in_selectExpr600); if (state.failed) return retval;

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

                    	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:135:32: ( selectColumn )+
                    	int cnt21 = 0;
                    	do 
                    	{
                    	    int alt21 = 2;
                    	    int LA21_0 = input.LA(1);

                    	    if ( (LA21_0 == COUNT || LA21_0 == DOT || LA21_0 == FALSE || LA21_0 == SELECT || LA21_0 == TRUE || LA21_0 == CASE || LA21_0 == AGGREGATE || (LA21_0 >= CONSTRUCTOR && LA21_0 <= CASE2) || LA21_0 == METHOD_CALL || LA21_0 == UNARY_MINUS || (LA21_0 >= CONSTANT && LA21_0 <= JAVA_CONSTANT) || (LA21_0 >= PLUS && LA21_0 <= DIV) || (LA21_0 >= PARAM && LA21_0 <= IDENT) || LA21_0 == ALIAS_REF || LA21_0 == SQL_TOKEN || LA21_0 == SELECT_EXPR) )
                    	    {
                    	        alt21 = 1;
                    	    }


                    	    switch (alt21) 
                    		{
                    			case 1 :
                    			    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:135:34: selectColumn
                    			    {
                    			    	PushFollow(FOLLOW_selectColumn_in_selectExpr612);
                    			    	selectColumn();
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return retval;

                    			    }
                    			    break;

                    			default:
                    			    if ( cnt21 >= 1 ) goto loop21;
                    			    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
                    		            EarlyExitException eee21 =
                    		                new EarlyExitException(21, input);
                    		            throw eee21;
                    	    }
                    	    cnt21++;
                    	} while (true);

                    	loop21:
                    		;	// Stops C# compiler whinging that label 'loop21' has no statements


                    	Match(input, Token.UP, null); if (state.failed) return retval;

                    }
                    break;
                case 4 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:136:4: methodCall
                    {
                    	PushFollow(FOLLOW_methodCall_in_selectExpr622);
                    	methodCall();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 5 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:137:4: aggregate
                    {
                    	PushFollow(FOLLOW_aggregate_in_selectExpr627);
                    	aggregate();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 6 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:138:4: c= constant
                    {
                    	PushFollow(FOLLOW_constant_in_selectExpr634);
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
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:139:4: arithmeticExpr
                    {
                    	PushFollow(FOLLOW_arithmeticExpr_in_selectExpr641);
                    	arithmeticExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 8 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:140:4: param= PARAM
                    {
                    	param=(IASTNode)Match(input,PARAM,FOLLOW_PARAM_in_selectExpr648); if (state.failed) return retval;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(param); 
                    	}

                    }
                    break;
                case 9 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:142:4: selectStatement
                    {
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("("); 
                    	}
                    	PushFollow(FOLLOW_selectStatement_in_selectExpr658);
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:145:1: count : ^( COUNT ( distinctOrAll )? countExpr ) ;
    public void count() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:146:2: ( ^( COUNT ( distinctOrAll )? countExpr ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:146:4: ^( COUNT ( distinctOrAll )? countExpr )
            {
            	Match(input,COUNT,FOLLOW_COUNT_in_count672); if (state.failed) return ;

            	if ( (state.backtracking==0) )
            	{
            	   Out("count("); 
            	}

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:146:32: ( distinctOrAll )?
            	int alt23 = 2;
            	int LA23_0 = input.LA(1);

            	if ( (LA23_0 == ALL || LA23_0 == DISTINCT) )
            	{
            	    alt23 = 1;
            	}
            	switch (alt23) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:146:34: distinctOrAll
            	        {
            	        	PushFollow(FOLLOW_distinctOrAll_in_count679);
            	        	distinctOrAll();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_countExpr_in_count685);
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:149:1: distinctOrAll : ( DISTINCT | ^( ALL ( . )* ) );
    public void distinctOrAll() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:150:2: ( DISTINCT | ^( ALL ( . )* ) )
            int alt25 = 2;
            int LA25_0 = input.LA(1);

            if ( (LA25_0 == DISTINCT) )
            {
                alt25 = 1;
            }
            else if ( (LA25_0 == ALL) )
            {
                alt25 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d25s0 =
                    new NoViableAltException("", 25, 0, input);

                throw nvae_d25s0;
            }
            switch (alt25) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:150:4: DISTINCT
                    {
                    	Match(input,DISTINCT,FOLLOW_DISTINCT_in_distinctOrAll700); if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("distinct "); 
                    	}

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:151:4: ^( ALL ( . )* )
                    {
                    	Match(input,ALL,FOLLOW_ALL_in_distinctOrAll708); if (state.failed) return ;

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return ;
                    	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:151:10: ( . )*
                    	    do 
                    	    {
                    	        int alt24 = 2;
                    	        int LA24_0 = input.LA(1);

                    	        if ( ((LA24_0 >= ALL && LA24_0 <= BOGUS)) )
                    	        {
                    	            alt24 = 1;
                    	        }
                    	        else if ( (LA24_0 == UP) )
                    	        {
                    	            alt24 = 2;
                    	        }


                    	        switch (alt24) 
                    	    	{
                    	    		case 1 :
                    	    		    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:151:10: .
                    	    		    {
                    	    		    	MatchAny(input); if (state.failed) return ;

                    	    		    }
                    	    		    break;

                    	    		default:
                    	    		    goto loop24;
                    	        }
                    	    } while (true);

                    	    loop24:
                    	    	;	// Stops C# compiler whining that label 'loop24' has no statements


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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:154:1: countExpr : ( ROW_STAR | simpleExpr );
    public void countExpr() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:156:2: ( ROW_STAR | simpleExpr )
            int alt26 = 2;
            int LA26_0 = input.LA(1);

            if ( (LA26_0 == ROW_STAR) )
            {
                alt26 = 1;
            }
            else if ( (LA26_0 == COUNT || LA26_0 == DOT || LA26_0 == FALSE || LA26_0 == NULL || LA26_0 == TRUE || LA26_0 == CASE || LA26_0 == AGGREGATE || LA26_0 == CASE2 || LA26_0 == INDEX_OP || LA26_0 == METHOD_CALL || LA26_0 == UNARY_MINUS || (LA26_0 >= CONSTANT && LA26_0 <= JAVA_CONSTANT) || (LA26_0 >= PLUS && LA26_0 <= DIV) || (LA26_0 >= PARAM && LA26_0 <= IDENT) || LA26_0 == ALIAS_REF || LA26_0 == SQL_TOKEN || LA26_0 == NAMED_PARAM) )
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
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:156:4: ROW_STAR
                    {
                    	Match(input,ROW_STAR,FOLLOW_ROW_STAR_in_countExpr727); if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("*"); 
                    	}

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:157:4: simpleExpr
                    {
                    	PushFollow(FOLLOW_simpleExpr_in_countExpr734);
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:160:1: selectAtom : ( ^( DOT ( . )* ) | ^( SQL_TOKEN ( . )* ) | ^( ALIAS_REF ( . )* ) | ^( SELECT_EXPR ( . )* ) );
    public SqlGenerator.selectAtom_return selectAtom() // throws RecognitionException [1]
    {   
        SqlGenerator.selectAtom_return retval = new SqlGenerator.selectAtom_return();
        retval.Start = input.LT(1);

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:161:2: ( ^( DOT ( . )* ) | ^( SQL_TOKEN ( . )* ) | ^( ALIAS_REF ( . )* ) | ^( SELECT_EXPR ( . )* ) )
            int alt31 = 4;
            switch ( input.LA(1) ) 
            {
            case DOT:
            	{
                alt31 = 1;
                }
                break;
            case SQL_TOKEN:
            	{
                alt31 = 2;
                }
                break;
            case ALIAS_REF:
            	{
                alt31 = 3;
                }
                break;
            case SELECT_EXPR:
            	{
                alt31 = 4;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
            	    NoViableAltException nvae_d31s0 =
            	        new NoViableAltException("", 31, 0, input);

            	    throw nvae_d31s0;
            }

            switch (alt31) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:161:4: ^( DOT ( . )* )
                    {
                    	Match(input,DOT,FOLLOW_DOT_in_selectAtom746); if (state.failed) return retval;

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return retval;
                    	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:161:10: ( . )*
                    	    do 
                    	    {
                    	        int alt27 = 2;
                    	        int LA27_0 = input.LA(1);

                    	        if ( ((LA27_0 >= ALL && LA27_0 <= BOGUS)) )
                    	        {
                    	            alt27 = 1;
                    	        }
                    	        else if ( (LA27_0 == UP) )
                    	        {
                    	            alt27 = 2;
                    	        }


                    	        switch (alt27) 
                    	    	{
                    	    		case 1 :
                    	    		    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:161:10: .
                    	    		    {
                    	    		    	MatchAny(input); if (state.failed) return retval;

                    	    		    }
                    	    		    break;

                    	    		default:
                    	    		    goto loop27;
                    	        }
                    	    } while (true);

                    	    loop27:
                    	    	;	// Stops C# compiler whining that label 'loop27' has no statements


                    	    Match(input, Token.UP, null); if (state.failed) return retval;
                    	}

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:162:4: ^( SQL_TOKEN ( . )* )
                    {
                    	Match(input,SQL_TOKEN,FOLLOW_SQL_TOKEN_in_selectAtom756); if (state.failed) return retval;

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return retval;
                    	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:162:16: ( . )*
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
                    	    		    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:162:16: .
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
                case 3 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:163:4: ^( ALIAS_REF ( . )* )
                    {
                    	Match(input,ALIAS_REF,FOLLOW_ALIAS_REF_in_selectAtom766); if (state.failed) return retval;

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return retval;
                    	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:163:16: ( . )*
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
                    	    		    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:163:16: .
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
                case 4 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:164:4: ^( SELECT_EXPR ( . )* )
                    {
                    	Match(input,SELECT_EXPR,FOLLOW_SELECT_EXPR_in_selectAtom776); if (state.failed) return retval;

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return retval;
                    	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:164:18: ( . )*
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
                    	    		    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:164:18: .
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:172:1: from : ^(f= FROM ( fromTable )* ) ;
    public void from() // throws RecognitionException [1]
    {   
        IASTNode f = null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:173:2: ( ^(f= FROM ( fromTable )* ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:173:4: ^(f= FROM ( fromTable )* )
            {
            	f=(IASTNode)Match(input,FROM,FOLLOW_FROM_in_from799); if (state.failed) return ;

            	if ( (state.backtracking==0) )
            	{
            	   Out(" from "); 
            	}

            	if ( input.LA(1) == Token.DOWN )
            	{
            	    Match(input, Token.DOWN, null); if (state.failed) return ;
            	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:174:3: ( fromTable )*
            	    do 
            	    {
            	        int alt32 = 2;
            	        int LA32_0 = input.LA(1);

            	        if ( (LA32_0 == FROM_FRAGMENT || LA32_0 == JOIN_FRAGMENT) )
            	        {
            	            alt32 = 1;
            	        }


            	        switch (alt32) 
            	    	{
            	    		case 1 :
            	    		    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:174:4: fromTable
            	    		    {
            	    		    	PushFollow(FOLLOW_fromTable_in_from806);
            	    		    	fromTable();
            	    		    	state.followingStackPointer--;
            	    		    	if (state.failed) return ;

            	    		    }
            	    		    break;

            	    		default:
            	    		    goto loop32;
            	        }
            	    } while (true);

            	    loop32:
            	    	;	// Stops C# compiler whining that label 'loop32' has no statements


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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:177:1: fromTable : ( ^(a= FROM_FRAGMENT ( tableJoin[ a ] )* ) | ^(a= JOIN_FRAGMENT ( tableJoin[ a ] )* ) );
    public void fromTable() // throws RecognitionException [1]
    {   
        IASTNode a = null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:182:2: ( ^(a= FROM_FRAGMENT ( tableJoin[ a ] )* ) | ^(a= JOIN_FRAGMENT ( tableJoin[ a ] )* ) )
            int alt35 = 2;
            int LA35_0 = input.LA(1);

            if ( (LA35_0 == FROM_FRAGMENT) )
            {
                alt35 = 1;
            }
            else if ( (LA35_0 == JOIN_FRAGMENT) )
            {
                alt35 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d35s0 =
                    new NoViableAltException("", 35, 0, input);

                throw nvae_d35s0;
            }
            switch (alt35) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:182:4: ^(a= FROM_FRAGMENT ( tableJoin[ a ] )* )
                    {
                    	a=(IASTNode)Match(input,FROM_FRAGMENT,FOLLOW_FROM_FRAGMENT_in_fromTable832); if (state.failed) return ;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(a); 
                    	}

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return ;
                    	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:182:36: ( tableJoin[ a ] )*
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
                    	    		    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:182:37: tableJoin[ a ]
                    	    		    {
                    	    		    	PushFollow(FOLLOW_tableJoin_in_fromTable838);
                    	    		    	tableJoin(a);
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
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:183:4: ^(a= JOIN_FRAGMENT ( tableJoin[ a ] )* )
                    {
                    	a=(IASTNode)Match(input,JOIN_FRAGMENT,FOLLOW_JOIN_FRAGMENT_in_fromTable853); if (state.failed) return ;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(a); 
                    	}

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return ;
                    	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:183:36: ( tableJoin[ a ] )*
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
                    	    		    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:183:37: tableJoin[ a ]
                    	    		    {
                    	    		    	PushFollow(FOLLOW_tableJoin_in_fromTable859);
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:186:1: tableJoin[ IASTNode parent ] : ( ^(c= JOIN_FRAGMENT ( tableJoin[ c ] )* ) | ^(d= FROM_FRAGMENT ( tableJoin[ d ] )* ) );
    public void tableJoin(IASTNode parent) // throws RecognitionException [1]
    {   
        IASTNode c = null;
        IASTNode d = null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:187:2: ( ^(c= JOIN_FRAGMENT ( tableJoin[ c ] )* ) | ^(d= FROM_FRAGMENT ( tableJoin[ d ] )* ) )
            int alt38 = 2;
            int LA38_0 = input.LA(1);

            if ( (LA38_0 == JOIN_FRAGMENT) )
            {
                alt38 = 1;
            }
            else if ( (LA38_0 == FROM_FRAGMENT) )
            {
                alt38 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d38s0 =
                    new NoViableAltException("", 38, 0, input);

                throw nvae_d38s0;
            }
            switch (alt38) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:187:4: ^(c= JOIN_FRAGMENT ( tableJoin[ c ] )* )
                    {
                    	c=(IASTNode)Match(input,JOIN_FRAGMENT,FOLLOW_JOIN_FRAGMENT_in_tableJoin882); if (state.failed) return ;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" "); Out(c); 
                    	}

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return ;
                    	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:187:46: ( tableJoin[ c ] )*
                    	    do 
                    	    {
                    	        int alt36 = 2;
                    	        int LA36_0 = input.LA(1);

                    	        if ( (LA36_0 == FROM_FRAGMENT || LA36_0 == JOIN_FRAGMENT) )
                    	        {
                    	            alt36 = 1;
                    	        }


                    	        switch (alt36) 
                    	    	{
                    	    		case 1 :
                    	    		    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:187:47: tableJoin[ c ]
                    	    		    {
                    	    		    	PushFollow(FOLLOW_tableJoin_in_tableJoin887);
                    	    		    	tableJoin(c);
                    	    		    	state.followingStackPointer--;
                    	    		    	if (state.failed) return ;

                    	    		    }
                    	    		    break;

                    	    		default:
                    	    		    goto loop36;
                    	        }
                    	    } while (true);

                    	    loop36:
                    	    	;	// Stops C# compiler whining that label 'loop36' has no statements


                    	    Match(input, Token.UP, null); if (state.failed) return ;
                    	}

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:188:4: ^(d= FROM_FRAGMENT ( tableJoin[ d ] )* )
                    {
                    	d=(IASTNode)Match(input,FROM_FRAGMENT,FOLLOW_FROM_FRAGMENT_in_tableJoin903); if (state.failed) return ;

                    	if ( (state.backtracking==0) )
                    	{
                    	   NestedFromFragment(d,parent); 
                    	}

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return ;
                    	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:188:58: ( tableJoin[ d ] )*
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
                    	    		    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:188:59: tableJoin[ d ]
                    	    		    {
                    	    		    	PushFollow(FOLLOW_tableJoin_in_tableJoin908);
                    	    		    	tableJoin(d);
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:191:1: booleanOp[ bool parens ] : ( ^( AND booleanExpr[true] booleanExpr[true] ) | ^( OR booleanExpr[false] booleanExpr[false] ) | ^( NOT booleanExpr[false] ) );
    public void booleanOp(bool parens) // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:192:2: ( ^( AND booleanExpr[true] booleanExpr[true] ) | ^( OR booleanExpr[false] booleanExpr[false] ) | ^( NOT booleanExpr[false] ) )
            int alt39 = 3;
            switch ( input.LA(1) ) 
            {
            case AND:
            	{
                alt39 = 1;
                }
                break;
            case OR:
            	{
                alt39 = 2;
                }
                break;
            case NOT:
            	{
                alt39 = 3;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d39s0 =
            	        new NoViableAltException("", 39, 0, input);

            	    throw nvae_d39s0;
            }

            switch (alt39) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:192:4: ^( AND booleanExpr[true] booleanExpr[true] )
                    {
                    	Match(input,AND,FOLLOW_AND_in_booleanOp928); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_booleanExpr_in_booleanOp930);
                    	booleanExpr(true);
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" and "); 
                    	}
                    	PushFollow(FOLLOW_booleanExpr_in_booleanOp935);
                    	booleanExpr(true);
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:193:4: ^( OR booleanExpr[false] booleanExpr[false] )
                    {
                    	Match(input,OR,FOLLOW_OR_in_booleanOp943); if (state.failed) return ;

                    	if ( (state.backtracking==0) )
                    	{
                    	   if (parens) Out("("); 
                    	}

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_booleanExpr_in_booleanOp947);
                    	booleanExpr(false);
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" or "); 
                    	}
                    	PushFollow(FOLLOW_booleanExpr_in_booleanOp952);
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
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:194:4: ^( NOT booleanExpr[false] )
                    {
                    	Match(input,NOT,FOLLOW_NOT_in_booleanOp962); if (state.failed) return ;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" not ("); 
                    	}

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_booleanExpr_in_booleanOp966);
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:197:1: booleanExpr[ bool parens ] : ( booleanOp[ parens ] | comparisonExpr[ parens ] | st= SQL_TOKEN );
    public void booleanExpr(bool parens) // throws RecognitionException [1]
    {   
        IASTNode st = null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:198:2: ( booleanOp[ parens ] | comparisonExpr[ parens ] | st= SQL_TOKEN )
            int alt40 = 3;
            switch ( input.LA(1) ) 
            {
            case AND:
            case NOT:
            case OR:
            	{
                alt40 = 1;
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
                alt40 = 2;
                }
                break;
            case SQL_TOKEN:
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
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:198:4: booleanOp[ parens ]
                    {
                    	PushFollow(FOLLOW_booleanOp_in_booleanExpr983);
                    	booleanOp(parens);
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:199:4: comparisonExpr[ parens ]
                    {
                    	PushFollow(FOLLOW_comparisonExpr_in_booleanExpr990);
                    	comparisonExpr(parens);
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:200:4: st= SQL_TOKEN
                    {
                    	st=(IASTNode)Match(input,SQL_TOKEN,FOLLOW_SQL_TOKEN_in_booleanExpr999); if (state.failed) return ;
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:203:1: comparisonExpr[ bool parens ] : ( binaryComparisonExpression | exoticComparisonExpression );
    public void comparisonExpr(bool parens) // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:204:2: ( binaryComparisonExpression | exoticComparisonExpression )
            int alt41 = 2;
            int LA41_0 = input.LA(1);

            if ( (LA41_0 == EQ || LA41_0 == NE || (LA41_0 >= LT && LA41_0 <= GE)) )
            {
                alt41 = 1;
            }
            else if ( (LA41_0 == BETWEEN || LA41_0 == EXISTS || LA41_0 == IN || LA41_0 == LIKE || (LA41_0 >= IS_NOT_NULL && LA41_0 <= IS_NULL) || (LA41_0 >= NOT_BETWEEN && LA41_0 <= NOT_LIKE)) )
            {
                alt41 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d41s0 =
                    new NoViableAltException("", 41, 0, input);

                throw nvae_d41s0;
            }
            switch (alt41) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:204:4: binaryComparisonExpression
                    {
                    	PushFollow(FOLLOW_binaryComparisonExpression_in_comparisonExpr1015);
                    	binaryComparisonExpression();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:205:4: exoticComparisonExpression
                    {
                    	if ( (state.backtracking==0) )
                    	{
                    	   if (parens) Out("("); 
                    	}
                    	PushFollow(FOLLOW_exoticComparisonExpression_in_comparisonExpr1022);
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:208:1: binaryComparisonExpression : ( ^( EQ expr expr ) | ^( NE expr expr ) | ^( GT expr expr ) | ^( GE expr expr ) | ^( LT expr expr ) | ^( LE expr expr ) );
    public void binaryComparisonExpression() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:209:2: ( ^( EQ expr expr ) | ^( NE expr expr ) | ^( GT expr expr ) | ^( GE expr expr ) | ^( LT expr expr ) | ^( LE expr expr ) )
            int alt42 = 6;
            switch ( input.LA(1) ) 
            {
            case EQ:
            	{
                alt42 = 1;
                }
                break;
            case NE:
            	{
                alt42 = 2;
                }
                break;
            case GT:
            	{
                alt42 = 3;
                }
                break;
            case GE:
            	{
                alt42 = 4;
                }
                break;
            case LT:
            	{
                alt42 = 5;
                }
                break;
            case LE:
            	{
                alt42 = 6;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d42s0 =
            	        new NoViableAltException("", 42, 0, input);

            	    throw nvae_d42s0;
            }

            switch (alt42) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:209:4: ^( EQ expr expr )
                    {
                    	Match(input,EQ,FOLLOW_EQ_in_binaryComparisonExpression1037); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1039);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("="); 
                    	}
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1043);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:210:4: ^( NE expr expr )
                    {
                    	Match(input,NE,FOLLOW_NE_in_binaryComparisonExpression1050); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1052);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("<>"); 
                    	}
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1056);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:211:4: ^( GT expr expr )
                    {
                    	Match(input,GT,FOLLOW_GT_in_binaryComparisonExpression1063); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1065);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(">"); 
                    	}
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1069);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 4 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:212:4: ^( GE expr expr )
                    {
                    	Match(input,GE,FOLLOW_GE_in_binaryComparisonExpression1076); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1078);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(">="); 
                    	}
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1082);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 5 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:213:4: ^( LT expr expr )
                    {
                    	Match(input,LT,FOLLOW_LT_in_binaryComparisonExpression1089); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1091);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("<"); 
                    	}
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1095);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 6 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:214:4: ^( LE expr expr )
                    {
                    	Match(input,LE,FOLLOW_LE_in_binaryComparisonExpression1102); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1104);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("<="); 
                    	}
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1108);
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:217:1: exoticComparisonExpression : ( ^( LIKE expr expr likeEscape ) | ^( NOT_LIKE expr expr likeEscape ) | ^( BETWEEN expr expr expr ) | ^( NOT_BETWEEN expr expr expr ) | ^( IN expr inList ) | ^( NOT_IN expr inList ) | ^( EXISTS quantified ) | ^( IS_NULL expr ) | ^( IS_NOT_NULL expr ) );
    public void exoticComparisonExpression() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:218:2: ( ^( LIKE expr expr likeEscape ) | ^( NOT_LIKE expr expr likeEscape ) | ^( BETWEEN expr expr expr ) | ^( NOT_BETWEEN expr expr expr ) | ^( IN expr inList ) | ^( NOT_IN expr inList ) | ^( EXISTS quantified ) | ^( IS_NULL expr ) | ^( IS_NOT_NULL expr ) )
            int alt43 = 9;
            switch ( input.LA(1) ) 
            {
            case LIKE:
            	{
                alt43 = 1;
                }
                break;
            case NOT_LIKE:
            	{
                alt43 = 2;
                }
                break;
            case BETWEEN:
            	{
                alt43 = 3;
                }
                break;
            case NOT_BETWEEN:
            	{
                alt43 = 4;
                }
                break;
            case IN:
            	{
                alt43 = 5;
                }
                break;
            case NOT_IN:
            	{
                alt43 = 6;
                }
                break;
            case EXISTS:
            	{
                alt43 = 7;
                }
                break;
            case IS_NULL:
            	{
                alt43 = 8;
                }
                break;
            case IS_NOT_NULL:
            	{
                alt43 = 9;
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
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:218:4: ^( LIKE expr expr likeEscape )
                    {
                    	Match(input,LIKE,FOLLOW_LIKE_in_exoticComparisonExpression1122); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1124);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" like "); 
                    	}
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1128);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_likeEscape_in_exoticComparisonExpression1130);
                    	likeEscape();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:219:4: ^( NOT_LIKE expr expr likeEscape )
                    {
                    	Match(input,NOT_LIKE,FOLLOW_NOT_LIKE_in_exoticComparisonExpression1138); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1140);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" not like "); 
                    	}
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1144);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_likeEscape_in_exoticComparisonExpression1146);
                    	likeEscape();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:220:4: ^( BETWEEN expr expr expr )
                    {
                    	Match(input,BETWEEN,FOLLOW_BETWEEN_in_exoticComparisonExpression1153); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1155);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" between "); 
                    	}
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1159);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" and "); 
                    	}
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1163);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 4 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:221:4: ^( NOT_BETWEEN expr expr expr )
                    {
                    	Match(input,NOT_BETWEEN,FOLLOW_NOT_BETWEEN_in_exoticComparisonExpression1170); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1172);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" not between "); 
                    	}
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1176);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" and "); 
                    	}
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1180);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 5 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:222:4: ^( IN expr inList )
                    {
                    	Match(input,IN,FOLLOW_IN_in_exoticComparisonExpression1187); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1189);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" in"); 
                    	}
                    	PushFollow(FOLLOW_inList_in_exoticComparisonExpression1193);
                    	inList();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 6 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:223:4: ^( NOT_IN expr inList )
                    {
                    	Match(input,NOT_IN,FOLLOW_NOT_IN_in_exoticComparisonExpression1201); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1203);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" not in "); 
                    	}
                    	PushFollow(FOLLOW_inList_in_exoticComparisonExpression1207);
                    	inList();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 7 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:224:4: ^( EXISTS quantified )
                    {
                    	Match(input,EXISTS,FOLLOW_EXISTS_in_exoticComparisonExpression1215); if (state.failed) return ;

                    	if ( (state.backtracking==0) )
                    	{
                    	   OptionalSpace(); Out("exists "); 
                    	}

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_quantified_in_exoticComparisonExpression1219);
                    	quantified();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 8 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:225:4: ^( IS_NULL expr )
                    {
                    	Match(input,IS_NULL,FOLLOW_IS_NULL_in_exoticComparisonExpression1227); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1229);
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
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:226:4: ^( IS_NOT_NULL expr )
                    {
                    	Match(input,IS_NOT_NULL,FOLLOW_IS_NOT_NULL_in_exoticComparisonExpression1238); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1240);
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:229:1: likeEscape : ( ^( ESCAPE expr ) )? ;
    public void likeEscape() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:230:2: ( ( ^( ESCAPE expr ) )? )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:230:4: ( ^( ESCAPE expr ) )?
            {
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:230:4: ( ^( ESCAPE expr ) )?
            	int alt44 = 2;
            	int LA44_0 = input.LA(1);

            	if ( (LA44_0 == ESCAPE) )
            	{
            	    alt44 = 1;
            	}
            	switch (alt44) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:230:6: ^( ESCAPE expr )
            	        {
            	        	Match(input,ESCAPE,FOLLOW_ESCAPE_in_likeEscape1257); if (state.failed) return ;

            	        	if ( (state.backtracking==0) )
            	        	{
            	        	   Out(" escape "); 
            	        	}

            	        	Match(input, Token.DOWN, null); if (state.failed) return ;
            	        	PushFollow(FOLLOW_expr_in_likeEscape1261);
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:233:1: inList : ^( IN_LIST ( parenSelect | simpleExprList ) ) ;
    public void inList() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:234:2: ( ^( IN_LIST ( parenSelect | simpleExprList ) ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:234:4: ^( IN_LIST ( parenSelect | simpleExprList ) )
            {
            	Match(input,IN_LIST,FOLLOW_IN_LIST_in_inList1277); if (state.failed) return ;

            	if ( (state.backtracking==0) )
            	{
            	   Out(" "); 
            	}

            	if ( input.LA(1) == Token.DOWN )
            	{
            	    Match(input, Token.DOWN, null); if (state.failed) return ;
            	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:234:28: ( parenSelect | simpleExprList )
            	    int alt45 = 2;
            	    int LA45_0 = input.LA(1);

            	    if ( (LA45_0 == SELECT) )
            	    {
            	        alt45 = 1;
            	    }
            	    else if ( (LA45_0 == UP || LA45_0 == COUNT || LA45_0 == DOT || LA45_0 == FALSE || LA45_0 == NULL || LA45_0 == TRUE || LA45_0 == CASE || LA45_0 == AGGREGATE || LA45_0 == CASE2 || LA45_0 == INDEX_OP || LA45_0 == METHOD_CALL || LA45_0 == UNARY_MINUS || (LA45_0 >= CONSTANT && LA45_0 <= JAVA_CONSTANT) || (LA45_0 >= PLUS && LA45_0 <= DIV) || (LA45_0 >= PARAM && LA45_0 <= IDENT) || LA45_0 == ALIAS_REF || LA45_0 == SQL_TOKEN || LA45_0 == NAMED_PARAM) )
            	    {
            	        alt45 = 2;
            	    }
            	    else 
            	    {
            	        if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	        NoViableAltException nvae_d45s0 =
            	            new NoViableAltException("", 45, 0, input);

            	        throw nvae_d45s0;
            	    }
            	    switch (alt45) 
            	    {
            	        case 1 :
            	            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:234:30: parenSelect
            	            {
            	            	PushFollow(FOLLOW_parenSelect_in_inList1283);
            	            	parenSelect();
            	            	state.followingStackPointer--;
            	            	if (state.failed) return ;

            	            }
            	            break;
            	        case 2 :
            	            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:234:44: simpleExprList
            	            {
            	            	PushFollow(FOLLOW_simpleExprList_in_inList1287);
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:237:1: simpleExprList : (e= simpleExpr )* ;
    public void simpleExprList() // throws RecognitionException [1]
    {   
        SqlGenerator.simpleExpr_return e = default(SqlGenerator.simpleExpr_return);


        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:238:2: ( (e= simpleExpr )* )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:238:4: (e= simpleExpr )*
            {
            	if ( (state.backtracking==0) )
            	{
            	   Out("("); 
            	}
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:238:18: (e= simpleExpr )*
            	do 
            	{
            	    int alt46 = 2;
            	    int LA46_0 = input.LA(1);

            	    if ( (LA46_0 == COUNT || LA46_0 == DOT || LA46_0 == FALSE || LA46_0 == NULL || LA46_0 == TRUE || LA46_0 == CASE || LA46_0 == AGGREGATE || LA46_0 == CASE2 || LA46_0 == INDEX_OP || LA46_0 == METHOD_CALL || LA46_0 == UNARY_MINUS || (LA46_0 >= CONSTANT && LA46_0 <= JAVA_CONSTANT) || (LA46_0 >= PLUS && LA46_0 <= DIV) || (LA46_0 >= PARAM && LA46_0 <= IDENT) || LA46_0 == ALIAS_REF || LA46_0 == SQL_TOKEN || LA46_0 == NAMED_PARAM) )
            	    {
            	        alt46 = 1;
            	    }


            	    switch (alt46) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:238:19: e= simpleExpr
            			    {
            			    	PushFollow(FOLLOW_simpleExpr_in_simpleExprList1308);
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
            			    goto loop46;
            	    }
            	} while (true);

            	loop46:
            		;	// Stops C# compiler whining that label 'loop46' has no statements

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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:242:1: expr : ( simpleExpr | ^( VECTOR_EXPR (e= expr )* ) | parenSelect | ^( ANY quantified ) | ^( ALL quantified ) | ^( SOME quantified ) );
    public SqlGenerator.expr_return expr() // throws RecognitionException [1]
    {   
        SqlGenerator.expr_return retval = new SqlGenerator.expr_return();
        retval.Start = input.LT(1);

        SqlGenerator.expr_return e = default(SqlGenerator.expr_return);


        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:243:2: ( simpleExpr | ^( VECTOR_EXPR (e= expr )* ) | parenSelect | ^( ANY quantified ) | ^( ALL quantified ) | ^( SOME quantified ) )
            int alt48 = 6;
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
                alt48 = 1;
                }
                break;
            case VECTOR_EXPR:
            	{
                alt48 = 2;
                }
                break;
            case SELECT:
            	{
                alt48 = 3;
                }
                break;
            case ANY:
            	{
                alt48 = 4;
                }
                break;
            case ALL:
            	{
                alt48 = 5;
                }
                break;
            case SOME:
            	{
                alt48 = 6;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
            	    NoViableAltException nvae_d48s0 =
            	        new NoViableAltException("", 48, 0, input);

            	    throw nvae_d48s0;
            }

            switch (alt48) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:243:4: simpleExpr
                    {
                    	PushFollow(FOLLOW_simpleExpr_in_expr1327);
                    	simpleExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:244:4: ^( VECTOR_EXPR (e= expr )* )
                    {
                    	Match(input,VECTOR_EXPR,FOLLOW_VECTOR_EXPR_in_expr1334); if (state.failed) return retval;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("("); 
                    	}

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return retval;
                    	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:244:33: (e= expr )*
                    	    do 
                    	    {
                    	        int alt47 = 2;
                    	        int LA47_0 = input.LA(1);

                    	        if ( ((LA47_0 >= ALL && LA47_0 <= ANY) || LA47_0 == COUNT || LA47_0 == DOT || LA47_0 == FALSE || LA47_0 == NULL || LA47_0 == SELECT || LA47_0 == SOME || LA47_0 == TRUE || LA47_0 == CASE || LA47_0 == AGGREGATE || LA47_0 == CASE2 || LA47_0 == INDEX_OP || LA47_0 == METHOD_CALL || LA47_0 == UNARY_MINUS || LA47_0 == VECTOR_EXPR || (LA47_0 >= CONSTANT && LA47_0 <= JAVA_CONSTANT) || (LA47_0 >= PLUS && LA47_0 <= DIV) || (LA47_0 >= PARAM && LA47_0 <= IDENT) || LA47_0 == ALIAS_REF || LA47_0 == SQL_TOKEN || LA47_0 == NAMED_PARAM) )
                    	        {
                    	            alt47 = 1;
                    	        }


                    	        switch (alt47) 
                    	    	{
                    	    		case 1 :
                    	    		    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:244:34: e= expr
                    	    		    {
                    	    		    	PushFollow(FOLLOW_expr_in_expr1341);
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
                    	    		    goto loop47;
                    	        }
                    	    } while (true);

                    	    loop47:
                    	    	;	// Stops C# compiler whining that label 'loop47' has no statements

                    	    if ( (state.backtracking==0) )
                    	    {
                    	       Out(")"); 
                    	    }

                    	    Match(input, Token.UP, null); if (state.failed) return retval;
                    	}

                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:245:4: parenSelect
                    {
                    	PushFollow(FOLLOW_parenSelect_in_expr1356);
                    	parenSelect();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 4 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:246:4: ^( ANY quantified )
                    {
                    	Match(input,ANY,FOLLOW_ANY_in_expr1362); if (state.failed) return retval;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("any "); 
                    	}

                    	Match(input, Token.DOWN, null); if (state.failed) return retval;
                    	PushFollow(FOLLOW_quantified_in_expr1366);
                    	quantified();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    	Match(input, Token.UP, null); if (state.failed) return retval;

                    }
                    break;
                case 5 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:247:4: ^( ALL quantified )
                    {
                    	Match(input,ALL,FOLLOW_ALL_in_expr1374); if (state.failed) return retval;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("all "); 
                    	}

                    	Match(input, Token.DOWN, null); if (state.failed) return retval;
                    	PushFollow(FOLLOW_quantified_in_expr1378);
                    	quantified();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    	Match(input, Token.UP, null); if (state.failed) return retval;

                    }
                    break;
                case 6 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:248:4: ^( SOME quantified )
                    {
                    	Match(input,SOME,FOLLOW_SOME_in_expr1386); if (state.failed) return retval;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("some "); 
                    	}

                    	Match(input, Token.DOWN, null); if (state.failed) return retval;
                    	PushFollow(FOLLOW_quantified_in_expr1390);
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:251:1: quantified : ( sqlToken | selectStatement ) ;
    public void quantified() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:252:2: ( ( sqlToken | selectStatement ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:252:4: ( sqlToken | selectStatement )
            {
            	if ( (state.backtracking==0) )
            	{
            	   Out("("); 
            	}
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:252:18: ( sqlToken | selectStatement )
            	int alt49 = 2;
            	int LA49_0 = input.LA(1);

            	if ( (LA49_0 == SQL_TOKEN) )
            	{
            	    alt49 = 1;
            	}
            	else if ( (LA49_0 == SELECT) )
            	{
            	    alt49 = 2;
            	}
            	else 
            	{
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d49s0 =
            	        new NoViableAltException("", 49, 0, input);

            	    throw nvae_d49s0;
            	}
            	switch (alt49) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:252:20: sqlToken
            	        {
            	        	PushFollow(FOLLOW_sqlToken_in_quantified1408);
            	        	sqlToken();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;
            	    case 2 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:252:31: selectStatement
            	        {
            	        	PushFollow(FOLLOW_selectStatement_in_quantified1412);
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:255:1: parenSelect : selectStatement ;
    public void parenSelect() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:256:2: ( selectStatement )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:256:4: selectStatement
            {
            	if ( (state.backtracking==0) )
            	{
            	   Out("("); 
            	}
            	PushFollow(FOLLOW_selectStatement_in_parenSelect1431);
            	selectStatement();
            	state.followingStackPointer--;
            	if (state.failed) return ;
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
    // $ANTLR end "parenSelect"

    public class simpleExpr_return : TreeRuleReturnScope
    {
    };

    // $ANTLR start "simpleExpr"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:259:1: simpleExpr : (c= constant | NULL | addrExpr | sqlToken | aggregate | methodCall | count | parameter | arithmeticExpr );
    public SqlGenerator.simpleExpr_return simpleExpr() // throws RecognitionException [1]
    {   
        SqlGenerator.simpleExpr_return retval = new SqlGenerator.simpleExpr_return();
        retval.Start = input.LT(1);

        SqlGenerator.constant_return c = default(SqlGenerator.constant_return);


        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:260:2: (c= constant | NULL | addrExpr | sqlToken | aggregate | methodCall | count | parameter | arithmeticExpr )
            int alt50 = 9;
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
                alt50 = 1;
                }
                break;
            case NULL:
            	{
                alt50 = 2;
                }
                break;
            case DOT:
            case INDEX_OP:
            case ALIAS_REF:
            	{
                alt50 = 3;
                }
                break;
            case SQL_TOKEN:
            	{
                alt50 = 4;
                }
                break;
            case AGGREGATE:
            	{
                alt50 = 5;
                }
                break;
            case METHOD_CALL:
            	{
                alt50 = 6;
                }
                break;
            case COUNT:
            	{
                alt50 = 7;
                }
                break;
            case PARAM:
            case NAMED_PARAM:
            	{
                alt50 = 8;
                }
                break;
            case CASE:
            case CASE2:
            case UNARY_MINUS:
            case PLUS:
            case MINUS:
            case STAR:
            case DIV:
            	{
                alt50 = 9;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
            	    NoViableAltException nvae_d50s0 =
            	        new NoViableAltException("", 50, 0, input);

            	    throw nvae_d50s0;
            }

            switch (alt50) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:260:4: c= constant
                    {
                    	PushFollow(FOLLOW_constant_in_simpleExpr1447);
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
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:261:4: NULL
                    {
                    	Match(input,NULL,FOLLOW_NULL_in_simpleExpr1454); if (state.failed) return retval;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("null"); 
                    	}

                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:262:4: addrExpr
                    {
                    	PushFollow(FOLLOW_addrExpr_in_simpleExpr1461);
                    	addrExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 4 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:263:4: sqlToken
                    {
                    	PushFollow(FOLLOW_sqlToken_in_simpleExpr1466);
                    	sqlToken();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 5 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:264:4: aggregate
                    {
                    	PushFollow(FOLLOW_aggregate_in_simpleExpr1471);
                    	aggregate();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 6 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:265:4: methodCall
                    {
                    	PushFollow(FOLLOW_methodCall_in_simpleExpr1476);
                    	methodCall();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 7 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:266:4: count
                    {
                    	PushFollow(FOLLOW_count_in_simpleExpr1481);
                    	count();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 8 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:267:4: parameter
                    {
                    	PushFollow(FOLLOW_parameter_in_simpleExpr1486);
                    	parameter();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 9 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:268:4: arithmeticExpr
                    {
                    	PushFollow(FOLLOW_arithmeticExpr_in_simpleExpr1491);
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:271:1: constant : ( NUM_DOUBLE | NUM_FLOAT | NUM_INT | NUM_LONG | QUOTED_String | CONSTANT | JAVA_CONSTANT | TRUE | FALSE | IDENT );
    public SqlGenerator.constant_return constant() // throws RecognitionException [1]
    {   
        SqlGenerator.constant_return retval = new SqlGenerator.constant_return();
        retval.Start = input.LT(1);

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:272:2: ( NUM_DOUBLE | NUM_FLOAT | NUM_INT | NUM_LONG | QUOTED_String | CONSTANT | JAVA_CONSTANT | TRUE | FALSE | IDENT )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:284:1: arithmeticExpr : ( additiveExpr | multiplicativeExpr | ^( UNARY_MINUS expr ) | caseExpr );
    public void arithmeticExpr() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:285:2: ( additiveExpr | multiplicativeExpr | ^( UNARY_MINUS expr ) | caseExpr )
            int alt51 = 4;
            switch ( input.LA(1) ) 
            {
            case PLUS:
            case MINUS:
            	{
                alt51 = 1;
                }
                break;
            case STAR:
            case DIV:
            	{
                alt51 = 2;
                }
                break;
            case UNARY_MINUS:
            	{
                alt51 = 3;
                }
                break;
            case CASE:
            case CASE2:
            	{
                alt51 = 4;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d51s0 =
            	        new NoViableAltException("", 51, 0, input);

            	    throw nvae_d51s0;
            }

            switch (alt51) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:285:4: additiveExpr
                    {
                    	PushFollow(FOLLOW_additiveExpr_in_arithmeticExpr1560);
                    	additiveExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:286:4: multiplicativeExpr
                    {
                    	PushFollow(FOLLOW_multiplicativeExpr_in_arithmeticExpr1565);
                    	multiplicativeExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:288:4: ^( UNARY_MINUS expr )
                    {
                    	Match(input,UNARY_MINUS,FOLLOW_UNARY_MINUS_in_arithmeticExpr1572); if (state.failed) return ;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("-"); 
                    	}

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_arithmeticExpr1576);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 4 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:289:4: caseExpr
                    {
                    	PushFollow(FOLLOW_caseExpr_in_arithmeticExpr1582);
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:292:1: additiveExpr : ( ^( PLUS expr expr ) | ^( MINUS expr nestedExprAfterMinusDiv ) );
    public void additiveExpr() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:293:2: ( ^( PLUS expr expr ) | ^( MINUS expr nestedExprAfterMinusDiv ) )
            int alt52 = 2;
            int LA52_0 = input.LA(1);

            if ( (LA52_0 == PLUS) )
            {
                alt52 = 1;
            }
            else if ( (LA52_0 == MINUS) )
            {
                alt52 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d52s0 =
                    new NoViableAltException("", 52, 0, input);

                throw nvae_d52s0;
            }
            switch (alt52) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:293:4: ^( PLUS expr expr )
                    {
                    	Match(input,PLUS,FOLLOW_PLUS_in_additiveExpr1594); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_additiveExpr1596);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("+"); 
                    	}
                    	PushFollow(FOLLOW_expr_in_additiveExpr1600);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:294:4: ^( MINUS expr nestedExprAfterMinusDiv )
                    {
                    	Match(input,MINUS,FOLLOW_MINUS_in_additiveExpr1607); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_additiveExpr1609);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("-"); 
                    	}
                    	PushFollow(FOLLOW_nestedExprAfterMinusDiv_in_additiveExpr1613);
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


    // $ANTLR start "multiplicativeExpr"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:297:1: multiplicativeExpr : ( ^( STAR nestedExpr nestedExpr ) | ^( DIV nestedExpr nestedExprAfterMinusDiv ) );
    public void multiplicativeExpr() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:298:2: ( ^( STAR nestedExpr nestedExpr ) | ^( DIV nestedExpr nestedExprAfterMinusDiv ) )
            int alt53 = 2;
            int LA53_0 = input.LA(1);

            if ( (LA53_0 == STAR) )
            {
                alt53 = 1;
            }
            else if ( (LA53_0 == DIV) )
            {
                alt53 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d53s0 =
                    new NoViableAltException("", 53, 0, input);

                throw nvae_d53s0;
            }
            switch (alt53) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:298:4: ^( STAR nestedExpr nestedExpr )
                    {
                    	Match(input,STAR,FOLLOW_STAR_in_multiplicativeExpr1626); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_nestedExpr_in_multiplicativeExpr1628);
                    	nestedExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("*"); 
                    	}
                    	PushFollow(FOLLOW_nestedExpr_in_multiplicativeExpr1632);
                    	nestedExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:299:4: ^( DIV nestedExpr nestedExprAfterMinusDiv )
                    {
                    	Match(input,DIV,FOLLOW_DIV_in_multiplicativeExpr1639); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_nestedExpr_in_multiplicativeExpr1641);
                    	nestedExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("/"); 
                    	}
                    	PushFollow(FOLLOW_nestedExprAfterMinusDiv_in_multiplicativeExpr1645);
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:302:1: nestedExpr : ( ( additiveExpr )=> additiveExpr | expr );
    public void nestedExpr() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:304:2: ( ( additiveExpr )=> additiveExpr | expr )
            int alt54 = 2;
            alt54 = dfa54.Predict(input);
            switch (alt54) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:304:4: ( additiveExpr )=> additiveExpr
                    {
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("("); 
                    	}
                    	PushFollow(FOLLOW_additiveExpr_in_nestedExpr1667);
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
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:305:4: expr
                    {
                    	PushFollow(FOLLOW_expr_in_nestedExpr1674);
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:308:1: nestedExprAfterMinusDiv : ( ( arithmeticExpr )=> arithmeticExpr | expr );
    public void nestedExprAfterMinusDiv() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:310:2: ( ( arithmeticExpr )=> arithmeticExpr | expr )
            int alt55 = 2;
            alt55 = dfa55.Predict(input);
            switch (alt55) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:310:4: ( arithmeticExpr )=> arithmeticExpr
                    {
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("("); 
                    	}
                    	PushFollow(FOLLOW_arithmeticExpr_in_nestedExprAfterMinusDiv1696);
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
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:311:4: expr
                    {
                    	PushFollow(FOLLOW_expr_in_nestedExprAfterMinusDiv1703);
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:314:1: caseExpr : ( ^( CASE ( ^( WHEN booleanExpr[false] expr ) )+ ( ^( ELSE expr ) )? ) | ^( CASE2 expr ( ^( WHEN expr expr ) )+ ( ^( ELSE expr ) )? ) );
    public void caseExpr() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:315:2: ( ^( CASE ( ^( WHEN booleanExpr[false] expr ) )+ ( ^( ELSE expr ) )? ) | ^( CASE2 expr ( ^( WHEN expr expr ) )+ ( ^( ELSE expr ) )? ) )
            int alt60 = 2;
            int LA60_0 = input.LA(1);

            if ( (LA60_0 == CASE) )
            {
                alt60 = 1;
            }
            else if ( (LA60_0 == CASE2) )
            {
                alt60 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d60s0 =
                    new NoViableAltException("", 60, 0, input);

                throw nvae_d60s0;
            }
            switch (alt60) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:315:4: ^( CASE ( ^( WHEN booleanExpr[false] expr ) )+ ( ^( ELSE expr ) )? )
                    {
                    	Match(input,CASE,FOLLOW_CASE_in_caseExpr1715); if (state.failed) return ;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("case"); 
                    	}

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:316:3: ( ^( WHEN booleanExpr[false] expr ) )+
                    	int cnt56 = 0;
                    	do 
                    	{
                    	    int alt56 = 2;
                    	    int LA56_0 = input.LA(1);

                    	    if ( (LA56_0 == WHEN) )
                    	    {
                    	        alt56 = 1;
                    	    }


                    	    switch (alt56) 
                    		{
                    			case 1 :
                    			    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:316:5: ^( WHEN booleanExpr[false] expr )
                    			    {
                    			    	Match(input,WHEN,FOLLOW_WHEN_in_caseExpr1725); if (state.failed) return ;

                    			    	if ( (state.backtracking==0) )
                    			    	{
                    			    	   Out( " when "); 
                    			    	}

                    			    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    			    	PushFollow(FOLLOW_booleanExpr_in_caseExpr1729);
                    			    	booleanExpr(false);
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return ;
                    			    	if ( (state.backtracking==0) )
                    			    	{
                    			    	   Out(" then "); 
                    			    	}
                    			    	PushFollow(FOLLOW_expr_in_caseExpr1734);
                    			    	expr();
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return ;

                    			    	Match(input, Token.UP, null); if (state.failed) return ;

                    			    }
                    			    break;

                    			default:
                    			    if ( cnt56 >= 1 ) goto loop56;
                    			    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    		            EarlyExitException eee56 =
                    		                new EarlyExitException(56, input);
                    		            throw eee56;
                    	    }
                    	    cnt56++;
                    	} while (true);

                    	loop56:
                    		;	// Stops C# compiler whinging that label 'loop56' has no statements

                    	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:317:3: ( ^( ELSE expr ) )?
                    	int alt57 = 2;
                    	int LA57_0 = input.LA(1);

                    	if ( (LA57_0 == ELSE) )
                    	{
                    	    alt57 = 1;
                    	}
                    	switch (alt57) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:317:5: ^( ELSE expr )
                    	        {
                    	        	Match(input,ELSE,FOLLOW_ELSE_in_caseExpr1746); if (state.failed) return ;

                    	        	if ( (state.backtracking==0) )
                    	        	{
                    	        	   Out(" else "); 
                    	        	}

                    	        	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	        	PushFollow(FOLLOW_expr_in_caseExpr1750);
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
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:319:4: ^( CASE2 expr ( ^( WHEN expr expr ) )+ ( ^( ELSE expr ) )? )
                    {
                    	Match(input,CASE2,FOLLOW_CASE2_in_caseExpr1766); if (state.failed) return ;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("case "); 
                    	}

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_caseExpr1770);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:320:3: ( ^( WHEN expr expr ) )+
                    	int cnt58 = 0;
                    	do 
                    	{
                    	    int alt58 = 2;
                    	    int LA58_0 = input.LA(1);

                    	    if ( (LA58_0 == WHEN) )
                    	    {
                    	        alt58 = 1;
                    	    }


                    	    switch (alt58) 
                    		{
                    			case 1 :
                    			    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:320:5: ^( WHEN expr expr )
                    			    {
                    			    	Match(input,WHEN,FOLLOW_WHEN_in_caseExpr1777); if (state.failed) return ;

                    			    	if ( (state.backtracking==0) )
                    			    	{
                    			    	   Out( " when "); 
                    			    	}

                    			    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    			    	PushFollow(FOLLOW_expr_in_caseExpr1781);
                    			    	expr();
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return ;
                    			    	if ( (state.backtracking==0) )
                    			    	{
                    			    	   Out(" then "); 
                    			    	}
                    			    	PushFollow(FOLLOW_expr_in_caseExpr1785);
                    			    	expr();
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return ;

                    			    	Match(input, Token.UP, null); if (state.failed) return ;

                    			    }
                    			    break;

                    			default:
                    			    if ( cnt58 >= 1 ) goto loop58;
                    			    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    		            EarlyExitException eee58 =
                    		                new EarlyExitException(58, input);
                    		            throw eee58;
                    	    }
                    	    cnt58++;
                    	} while (true);

                    	loop58:
                    		;	// Stops C# compiler whinging that label 'loop58' has no statements

                    	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:321:3: ( ^( ELSE expr ) )?
                    	int alt59 = 2;
                    	int LA59_0 = input.LA(1);

                    	if ( (LA59_0 == ELSE) )
                    	{
                    	    alt59 = 1;
                    	}
                    	switch (alt59) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:321:5: ^( ELSE expr )
                    	        {
                    	        	Match(input,ELSE,FOLLOW_ELSE_in_caseExpr1797); if (state.failed) return ;

                    	        	if ( (state.backtracking==0) )
                    	        	{
                    	        	   Out(" else "); 
                    	        	}

                    	        	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	        	PushFollow(FOLLOW_expr_in_caseExpr1801);
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:325:1: aggregate : ^(a= AGGREGATE expr ) ;
    public void aggregate() // throws RecognitionException [1]
    {   
        IASTNode a = null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:326:2: ( ^(a= AGGREGATE expr ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:326:4: ^(a= AGGREGATE expr )
            {
            	a=(IASTNode)Match(input,AGGREGATE,FOLLOW_AGGREGATE_in_aggregate1825); if (state.failed) return ;

            	if ( (state.backtracking==0) )
            	{
            	   Out(a); Out("("); 
            	}

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_expr_in_aggregate1830);
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:330:1: methodCall : ^(m= METHOD_CALL i= METHOD_NAME ( ^( EXPR_LIST ( arguments )? ) )? ) ;
    public void methodCall() // throws RecognitionException [1]
    {   
        IASTNode m = null;
        IASTNode i = null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:331:2: ( ^(m= METHOD_CALL i= METHOD_NAME ( ^( EXPR_LIST ( arguments )? ) )? ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:331:4: ^(m= METHOD_CALL i= METHOD_NAME ( ^( EXPR_LIST ( arguments )? ) )? )
            {
            	m=(IASTNode)Match(input,METHOD_CALL,FOLLOW_METHOD_CALL_in_methodCall1849); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	i=(IASTNode)Match(input,METHOD_NAME,FOLLOW_METHOD_NAME_in_methodCall1853); if (state.failed) return ;
            	if ( (state.backtracking==0) )
            	{
            	   BeginFunctionTemplate(m,i); 
            	}
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:332:3: ( ^( EXPR_LIST ( arguments )? ) )?
            	int alt62 = 2;
            	int LA62_0 = input.LA(1);

            	if ( (LA62_0 == EXPR_LIST) )
            	{
            	    alt62 = 1;
            	}
            	switch (alt62) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:332:5: ^( EXPR_LIST ( arguments )? )
            	        {
            	        	Match(input,EXPR_LIST,FOLLOW_EXPR_LIST_in_methodCall1862); if (state.failed) return ;

            	        	if ( input.LA(1) == Token.DOWN )
            	        	{
            	        	    Match(input, Token.DOWN, null); if (state.failed) return ;
            	        	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:332:17: ( arguments )?
            	        	    int alt61 = 2;
            	        	    int LA61_0 = input.LA(1);

            	        	    if ( ((LA61_0 >= ALL && LA61_0 <= ANY) || LA61_0 == BETWEEN || LA61_0 == COUNT || LA61_0 == DOT || (LA61_0 >= EXISTS && LA61_0 <= FALSE) || LA61_0 == IN || LA61_0 == LIKE || LA61_0 == NULL || LA61_0 == SELECT || LA61_0 == SOME || LA61_0 == TRUE || LA61_0 == CASE || LA61_0 == AGGREGATE || LA61_0 == CASE2 || (LA61_0 >= INDEX_OP && LA61_0 <= NOT_LIKE) || LA61_0 == UNARY_MINUS || LA61_0 == VECTOR_EXPR || (LA61_0 >= CONSTANT && LA61_0 <= JAVA_CONSTANT) || LA61_0 == EQ || LA61_0 == NE || (LA61_0 >= LT && LA61_0 <= GE) || (LA61_0 >= PLUS && LA61_0 <= DIV) || (LA61_0 >= PARAM && LA61_0 <= IDENT) || LA61_0 == ALIAS_REF || LA61_0 == SQL_TOKEN || LA61_0 == NAMED_PARAM) )
            	        	    {
            	        	        alt61 = 1;
            	        	    }
            	        	    switch (alt61) 
            	        	    {
            	        	        case 1 :
            	        	            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:332:18: arguments
            	        	            {
            	        	            	PushFollow(FOLLOW_arguments_in_methodCall1865);
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:336:1: arguments : ( expr | comparisonExpr[true] ) ( ( expr | comparisonExpr[true] ) )* ;
    public void arguments() // throws RecognitionException [1]
    {   
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:337:2: ( ( expr | comparisonExpr[true] ) ( ( expr | comparisonExpr[true] ) )* )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:337:4: ( expr | comparisonExpr[true] ) ( ( expr | comparisonExpr[true] ) )*
            {
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:337:4: ( expr | comparisonExpr[true] )
            	int alt63 = 2;
            	int LA63_0 = input.LA(1);

            	if ( ((LA63_0 >= ALL && LA63_0 <= ANY) || LA63_0 == COUNT || LA63_0 == DOT || LA63_0 == FALSE || LA63_0 == NULL || LA63_0 == SELECT || LA63_0 == SOME || LA63_0 == TRUE || LA63_0 == CASE || LA63_0 == AGGREGATE || LA63_0 == CASE2 || LA63_0 == INDEX_OP || LA63_0 == METHOD_CALL || LA63_0 == UNARY_MINUS || LA63_0 == VECTOR_EXPR || (LA63_0 >= CONSTANT && LA63_0 <= JAVA_CONSTANT) || (LA63_0 >= PLUS && LA63_0 <= DIV) || (LA63_0 >= PARAM && LA63_0 <= IDENT) || LA63_0 == ALIAS_REF || LA63_0 == SQL_TOKEN || LA63_0 == NAMED_PARAM) )
            	{
            	    alt63 = 1;
            	}
            	else if ( (LA63_0 == BETWEEN || LA63_0 == EXISTS || LA63_0 == IN || LA63_0 == LIKE || (LA63_0 >= IS_NOT_NULL && LA63_0 <= IS_NULL) || (LA63_0 >= NOT_BETWEEN && LA63_0 <= NOT_LIKE) || LA63_0 == EQ || LA63_0 == NE || (LA63_0 >= LT && LA63_0 <= GE)) )
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
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:337:5: expr
            	        {
            	        	PushFollow(FOLLOW_expr_in_arguments1890);
            	        	expr();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;
            	    case 2 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:337:12: comparisonExpr[true]
            	        {
            	        	PushFollow(FOLLOW_comparisonExpr_in_arguments1894);
            	        	comparisonExpr(true);
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}

            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:337:34: ( ( expr | comparisonExpr[true] ) )*
            	do 
            	{
            	    int alt65 = 2;
            	    int LA65_0 = input.LA(1);

            	    if ( ((LA65_0 >= ALL && LA65_0 <= ANY) || LA65_0 == BETWEEN || LA65_0 == COUNT || LA65_0 == DOT || (LA65_0 >= EXISTS && LA65_0 <= FALSE) || LA65_0 == IN || LA65_0 == LIKE || LA65_0 == NULL || LA65_0 == SELECT || LA65_0 == SOME || LA65_0 == TRUE || LA65_0 == CASE || LA65_0 == AGGREGATE || LA65_0 == CASE2 || (LA65_0 >= INDEX_OP && LA65_0 <= NOT_LIKE) || LA65_0 == UNARY_MINUS || LA65_0 == VECTOR_EXPR || (LA65_0 >= CONSTANT && LA65_0 <= JAVA_CONSTANT) || LA65_0 == EQ || LA65_0 == NE || (LA65_0 >= LT && LA65_0 <= GE) || (LA65_0 >= PLUS && LA65_0 <= DIV) || (LA65_0 >= PARAM && LA65_0 <= IDENT) || LA65_0 == ALIAS_REF || LA65_0 == SQL_TOKEN || LA65_0 == NAMED_PARAM) )
            	    {
            	        alt65 = 1;
            	    }


            	    switch (alt65) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:337:36: ( expr | comparisonExpr[true] )
            			    {
            			    	if ( (state.backtracking==0) )
            			    	{
            			    	   CommaBetweenParameters(", "); 
            			    	}
            			    	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:337:70: ( expr | comparisonExpr[true] )
            			    	int alt64 = 2;
            			    	int LA64_0 = input.LA(1);

            			    	if ( ((LA64_0 >= ALL && LA64_0 <= ANY) || LA64_0 == COUNT || LA64_0 == DOT || LA64_0 == FALSE || LA64_0 == NULL || LA64_0 == SELECT || LA64_0 == SOME || LA64_0 == TRUE || LA64_0 == CASE || LA64_0 == AGGREGATE || LA64_0 == CASE2 || LA64_0 == INDEX_OP || LA64_0 == METHOD_CALL || LA64_0 == UNARY_MINUS || LA64_0 == VECTOR_EXPR || (LA64_0 >= CONSTANT && LA64_0 <= JAVA_CONSTANT) || (LA64_0 >= PLUS && LA64_0 <= DIV) || (LA64_0 >= PARAM && LA64_0 <= IDENT) || LA64_0 == ALIAS_REF || LA64_0 == SQL_TOKEN || LA64_0 == NAMED_PARAM) )
            			    	{
            			    	    alt64 = 1;
            			    	}
            			    	else if ( (LA64_0 == BETWEEN || LA64_0 == EXISTS || LA64_0 == IN || LA64_0 == LIKE || (LA64_0 >= IS_NOT_NULL && LA64_0 <= IS_NULL) || (LA64_0 >= NOT_BETWEEN && LA64_0 <= NOT_LIKE) || LA64_0 == EQ || LA64_0 == NE || (LA64_0 >= LT && LA64_0 <= GE)) )
            			    	{
            			    	    alt64 = 2;
            			    	}
            			    	else 
            			    	{
            			    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            			    	    NoViableAltException nvae_d64s0 =
            			    	        new NoViableAltException("", 64, 0, input);

            			    	    throw nvae_d64s0;
            			    	}
            			    	switch (alt64) 
            			    	{
            			    	    case 1 :
            			    	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:337:71: expr
            			    	        {
            			    	        	PushFollow(FOLLOW_expr_in_arguments1903);
            			    	        	expr();
            			    	        	state.followingStackPointer--;
            			    	        	if (state.failed) return ;

            			    	        }
            			    	        break;
            			    	    case 2 :
            			    	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:337:78: comparisonExpr[true]
            			    	        {
            			    	        	PushFollow(FOLLOW_comparisonExpr_in_arguments1907);
            			    	        	comparisonExpr(true);
            			    	        	state.followingStackPointer--;
            			    	        	if (state.failed) return ;

            			    	        }
            			    	        break;

            			    	}


            			    }
            			    break;

            			default:
            			    goto loop65;
            	    }
            	} while (true);

            	loop65:
            		;	// Stops C# compiler whining that label 'loop65' has no statements


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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:340:1: parameter : (n= NAMED_PARAM | p= PARAM );
    public void parameter() // throws RecognitionException [1]
    {   
        IASTNode n = null;
        IASTNode p = null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:341:2: (n= NAMED_PARAM | p= PARAM )
            int alt66 = 2;
            int LA66_0 = input.LA(1);

            if ( (LA66_0 == NAMED_PARAM) )
            {
                alt66 = 1;
            }
            else if ( (LA66_0 == PARAM) )
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
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:341:4: n= NAMED_PARAM
                    {
                    	n=(IASTNode)Match(input,NAMED_PARAM,FOLLOW_NAMED_PARAM_in_parameter1925); if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(n); 
                    	}

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:342:4: p= PARAM
                    {
                    	p=(IASTNode)Match(input,PARAM,FOLLOW_PARAM_in_parameter1934); if (state.failed) return ;
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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:345:1: addrExpr : ( ^(r= DOT . . ) | i= ALIAS_REF | ^(j= INDEX_OP ( . )* ) );
    public void addrExpr() // throws RecognitionException [1]
    {   
        IASTNode r = null;
        IASTNode i = null;
        IASTNode j = null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:346:2: ( ^(r= DOT . . ) | i= ALIAS_REF | ^(j= INDEX_OP ( . )* ) )
            int alt68 = 3;
            switch ( input.LA(1) ) 
            {
            case DOT:
            	{
                alt68 = 1;
                }
                break;
            case ALIAS_REF:
            	{
                alt68 = 2;
                }
                break;
            case INDEX_OP:
            	{
                alt68 = 3;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d68s0 =
            	        new NoViableAltException("", 68, 0, input);

            	    throw nvae_d68s0;
            }

            switch (alt68) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:346:4: ^(r= DOT . . )
                    {
                    	r=(IASTNode)Match(input,DOT,FOLLOW_DOT_in_addrExpr1950); if (state.failed) return ;

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
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:347:4: i= ALIAS_REF
                    {
                    	i=(IASTNode)Match(input,ALIAS_REF,FOLLOW_ALIAS_REF_in_addrExpr1964); if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(i); 
                    	}

                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:348:4: ^(j= INDEX_OP ( . )* )
                    {
                    	j=(IASTNode)Match(input,INDEX_OP,FOLLOW_INDEX_OP_in_addrExpr1974); if (state.failed) return ;

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return ;
                    	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:348:17: ( . )*
                    	    do 
                    	    {
                    	        int alt67 = 2;
                    	        int LA67_0 = input.LA(1);

                    	        if ( ((LA67_0 >= ALL && LA67_0 <= BOGUS)) )
                    	        {
                    	            alt67 = 1;
                    	        }
                    	        else if ( (LA67_0 == UP) )
                    	        {
                    	            alt67 = 2;
                    	        }


                    	        switch (alt67) 
                    	    	{
                    	    		case 1 :
                    	    		    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:348:17: .
                    	    		    {
                    	    		    	MatchAny(input); if (state.failed) return ;

                    	    		    }
                    	    		    break;

                    	    		default:
                    	    		    goto loop67;
                    	        }
                    	    } while (true);

                    	    loop67:
                    	    	;	// Stops C# compiler whining that label 'loop67' has no statements


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
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:351:1: sqlToken : ^(t= SQL_TOKEN ( . )* ) ;
    public void sqlToken() // throws RecognitionException [1]
    {   
        IASTNode t = null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:352:2: ( ^(t= SQL_TOKEN ( . )* ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:352:4: ^(t= SQL_TOKEN ( . )* )
            {
            	t=(IASTNode)Match(input,SQL_TOKEN,FOLLOW_SQL_TOKEN_in_sqlToken1994); if (state.failed) return ;

            	if ( (state.backtracking==0) )
            	{
            	   Out(t); 
            	}

            	if ( input.LA(1) == Token.DOWN )
            	{
            	    Match(input, Token.DOWN, null); if (state.failed) return ;
            	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:352:30: ( . )*
            	    do 
            	    {
            	        int alt69 = 2;
            	        int LA69_0 = input.LA(1);

            	        if ( ((LA69_0 >= ALL && LA69_0 <= BOGUS)) )
            	        {
            	            alt69 = 1;
            	        }
            	        else if ( (LA69_0 == UP) )
            	        {
            	            alt69 = 2;
            	        }


            	        switch (alt69) 
            	    	{
            	    		case 1 :
            	    		    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:352:30: .
            	    		    {
            	    		    	MatchAny(input); if (state.failed) return ;

            	    		    }
            	    		    break;

            	    		default:
            	    		    goto loop69;
            	        }
            	    } while (true);

            	    loop69:
            	    	;	// Stops C# compiler whining that label 'loop69' has no statements


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
        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:81:4: ( SQL_TOKEN )
        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:81:5: SQL_TOKEN
        {
        	Match(input,SQL_TOKEN,FOLLOW_SQL_TOKEN_in_synpred1_SqlGenerator323); if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred1_SqlGenerator"

    // $ANTLR start "synpred2_SqlGenerator"
    public void synpred2_SqlGenerator_fragment() {
        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:304:4: ( additiveExpr )
        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:304:5: additiveExpr
        {
        	PushFollow(FOLLOW_additiveExpr_in_synpred2_SqlGenerator1660);
        	additiveExpr();
        	state.followingStackPointer--;
        	if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred2_SqlGenerator"

    // $ANTLR start "synpred3_SqlGenerator"
    public void synpred3_SqlGenerator_fragment() {
        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:310:4: ( arithmeticExpr )
        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/SqlGenerator.g:310:5: arithmeticExpr
        {
        	PushFollow(FOLLOW_arithmeticExpr_in_synpred3_SqlGenerator1689);
        	arithmeticExpr();
        	state.followingStackPointer--;
        	if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred3_SqlGenerator"

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


   	protected DFA54 dfa54;
   	protected DFA55 dfa55;
	private void InitializeCyclicDFAs()
	{
    	this.dfa54 = new DFA54(this);
    	this.dfa55 = new DFA55(this);
	    this.dfa54.specialStateTransitionHandler = new DFA.SpecialStateTransitionHandler(DFA54_SpecialStateTransition);
	    this.dfa55.specialStateTransitionHandler = new DFA.SpecialStateTransitionHandler(DFA55_SpecialStateTransition);
	}

    const string DFA54_eotS =
        "\x19\uffff";
    const string DFA54_eofS =
        "\x19\uffff";
    const string DFA54_minS =
        "\x01\x04\x02\x00\x16\uffff";
    const string DFA54_maxS =
        "\x01\u008e\x02\x00\x16\uffff";
    const string DFA54_acceptS =
        "\x03\uffff\x01\x02\x14\uffff\x01\x01";
    const string DFA54_specialS =
        "\x01\uffff\x01\x00\x01\x01\x16\uffff}>";
    static readonly string[] DFA54_transitionS = {
            "\x02\x03\x06\uffff\x01\x03\x02\uffff\x01\x03\x04\uffff\x01\x03"+
            "\x12\uffff\x01\x03\x05\uffff\x01\x03\x01\uffff\x01\x03\x01\uffff"+
            "\x01\x03\x05\uffff\x01\x03\x0d\uffff\x01\x03\x02\uffff\x01\x03"+
            "\x03\uffff\x01\x03\x02\uffff\x01\x03\x08\uffff\x01\x03\x01\uffff"+
            "\x01\x03\x01\uffff\x06\x03\x0b\uffff\x01\x01\x01\x02\x02\x03"+
            "\x03\uffff\x03\x03\x0f\uffff\x01\x03\x01\uffff\x01\x03\x05\uffff"+
            "\x01\x03",
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
            ""
    };

    static readonly short[] DFA54_eot = DFA.UnpackEncodedString(DFA54_eotS);
    static readonly short[] DFA54_eof = DFA.UnpackEncodedString(DFA54_eofS);
    static readonly char[] DFA54_min = DFA.UnpackEncodedStringToUnsignedChars(DFA54_minS);
    static readonly char[] DFA54_max = DFA.UnpackEncodedStringToUnsignedChars(DFA54_maxS);
    static readonly short[] DFA54_accept = DFA.UnpackEncodedString(DFA54_acceptS);
    static readonly short[] DFA54_special = DFA.UnpackEncodedString(DFA54_specialS);
    static readonly short[][] DFA54_transition = DFA.UnpackEncodedStringArray(DFA54_transitionS);

    protected class DFA54 : DFA
    {
        public DFA54(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 54;
            this.eot = DFA54_eot;
            this.eof = DFA54_eof;
            this.min = DFA54_min;
            this.max = DFA54_max;
            this.accept = DFA54_accept;
            this.special = DFA54_special;
            this.transition = DFA54_transition;

        }

        override public string Description
        {
            get { return "302:1: nestedExpr : ( ( additiveExpr )=> additiveExpr | expr );"; }
        }

    }


    protected internal int DFA54_SpecialStateTransition(DFA dfa, int s, IIntStream _input) //throws NoViableAltException
    {
            ITreeNodeStream input = (ITreeNodeStream)_input;
    	int _s = s;
        switch ( s )
        {
               	case 0 : 
                   	int LA54_1 = input.LA(1);

                   	 
                   	int index54_1 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred2_SqlGenerator()) ) { s = 24; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index54_1);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 1 : 
                   	int LA54_2 = input.LA(1);

                   	 
                   	int index54_2 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred2_SqlGenerator()) ) { s = 24; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index54_2);
                   	if ( s >= 0 ) return s;
                   	break;
        }
        if (state.backtracking > 0) {state.failed = true; return -1;}
        NoViableAltException nvae54 =
            new NoViableAltException(dfa.Description, 54, _s, input);
        dfa.Error(nvae54);
        throw nvae54;
    }
    const string DFA55_eotS =
        "\x19\uffff";
    const string DFA55_eofS =
        "\x19\uffff";
    const string DFA55_minS =
        "\x01\x04\x07\x00\x11\uffff";
    const string DFA55_maxS =
        "\x01\u008e\x07\x00\x11\uffff";
    const string DFA55_acceptS =
        "\x08\uffff\x01\x02\x0f\uffff\x01\x01";
    const string DFA55_specialS =
        "\x01\uffff\x01\x00\x01\x01\x01\x02\x01\x03\x01\x04\x01\x05\x01\x06"+
        "\x11\uffff}>";
    static readonly string[] DFA55_transitionS = {
            "\x02\x08\x06\uffff\x01\x08\x02\uffff\x01\x08\x04\uffff\x01\x08"+
            "\x12\uffff\x01\x08\x05\uffff\x01\x08\x01\uffff\x01\x08\x01\uffff"+
            "\x01\x08\x05\uffff\x01\x06\x0d\uffff\x01\x08\x02\uffff\x01\x07"+
            "\x03\uffff\x01\x08\x02\uffff\x01\x08\x08\uffff\x01\x05\x01\uffff"+
            "\x01\x08\x01\uffff\x06\x08\x0b\uffff\x01\x01\x01\x02\x01\x03"+
            "\x01\x04\x03\uffff\x03\x08\x0f\uffff\x01\x08\x01\uffff\x01\x08"+
            "\x05\uffff\x01\x08",
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
            ""
    };

    static readonly short[] DFA55_eot = DFA.UnpackEncodedString(DFA55_eotS);
    static readonly short[] DFA55_eof = DFA.UnpackEncodedString(DFA55_eofS);
    static readonly char[] DFA55_min = DFA.UnpackEncodedStringToUnsignedChars(DFA55_minS);
    static readonly char[] DFA55_max = DFA.UnpackEncodedStringToUnsignedChars(DFA55_maxS);
    static readonly short[] DFA55_accept = DFA.UnpackEncodedString(DFA55_acceptS);
    static readonly short[] DFA55_special = DFA.UnpackEncodedString(DFA55_specialS);
    static readonly short[][] DFA55_transition = DFA.UnpackEncodedStringArray(DFA55_transitionS);

    protected class DFA55 : DFA
    {
        public DFA55(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 55;
            this.eot = DFA55_eot;
            this.eof = DFA55_eof;
            this.min = DFA55_min;
            this.max = DFA55_max;
            this.accept = DFA55_accept;
            this.special = DFA55_special;
            this.transition = DFA55_transition;

        }

        override public string Description
        {
            get { return "308:1: nestedExprAfterMinusDiv : ( ( arithmeticExpr )=> arithmeticExpr | expr );"; }
        }

    }


    protected internal int DFA55_SpecialStateTransition(DFA dfa, int s, IIntStream _input) //throws NoViableAltException
    {
            ITreeNodeStream input = (ITreeNodeStream)_input;
    	int _s = s;
        switch ( s )
        {
               	case 0 : 
                   	int LA55_1 = input.LA(1);

                   	 
                   	int index55_1 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred3_SqlGenerator()) ) { s = 24; }

                   	else if ( (true) ) { s = 8; }

                   	 
                   	input.Seek(index55_1);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 1 : 
                   	int LA55_2 = input.LA(1);

                   	 
                   	int index55_2 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred3_SqlGenerator()) ) { s = 24; }

                   	else if ( (true) ) { s = 8; }

                   	 
                   	input.Seek(index55_2);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 2 : 
                   	int LA55_3 = input.LA(1);

                   	 
                   	int index55_3 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred3_SqlGenerator()) ) { s = 24; }

                   	else if ( (true) ) { s = 8; }

                   	 
                   	input.Seek(index55_3);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 3 : 
                   	int LA55_4 = input.LA(1);

                   	 
                   	int index55_4 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred3_SqlGenerator()) ) { s = 24; }

                   	else if ( (true) ) { s = 8; }

                   	 
                   	input.Seek(index55_4);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 4 : 
                   	int LA55_5 = input.LA(1);

                   	 
                   	int index55_5 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred3_SqlGenerator()) ) { s = 24; }

                   	else if ( (true) ) { s = 8; }

                   	 
                   	input.Seek(index55_5);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 5 : 
                   	int LA55_6 = input.LA(1);

                   	 
                   	int index55_6 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred3_SqlGenerator()) ) { s = 24; }

                   	else if ( (true) ) { s = 8; }

                   	 
                   	input.Seek(index55_6);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 6 : 
                   	int LA55_7 = input.LA(1);

                   	 
                   	int index55_7 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred3_SqlGenerator()) ) { s = 24; }

                   	else if ( (true) ) { s = 8; }

                   	 
                   	input.Seek(index55_7);
                   	if ( s >= 0 ) return s;
                   	break;
        }
        if (state.backtracking > 0) {state.failed = true; return -1;}
        NoViableAltException nvae55 =
            new NoViableAltException(dfa.Description, 55, _s, input);
        dfa.Error(nvae55);
        throw nvae55;
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
    public static readonly BitSet FOLLOW_INTO_in_insertStatement250 = new BitSet(new ulong[]{0x0000200000000000UL});
    public static readonly BitSet FOLLOW_selectStatement_in_insertStatement256 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_SET_in_setClause276 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_comparisonExpr_in_setClause280 = new BitSet(new ulong[]{0x0000000404080408UL,0x00000F4800076000UL});
    public static readonly BitSet FOLLOW_comparisonExpr_in_setClause287 = new BitSet(new ulong[]{0x0000000404080408UL,0x00000F4800076000UL});
    public static readonly BitSet FOLLOW_WHERE_in_whereClause305 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_whereClauseExpr_in_whereClause309 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_conditionList_in_whereClauseExpr328 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_booleanExpr_in_whereClauseExpr333 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_expr_in_orderExprs349 = new BitSet(new ulong[]{0x0082A0800010D132UL,0x0071E003F5009120UL,0x0000000000004140UL});
    public static readonly BitSet FOLLOW_orderDirection_in_orderExprs356 = new BitSet(new ulong[]{0x0082A08000109032UL,0x0071E003F5009120UL,0x0000000000004140UL});
    public static readonly BitSet FOLLOW_orderExprs_in_orderExprs366 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_expr_in_groupExprs381 = new BitSet(new ulong[]{0x0082A08000109032UL,0x0071E003F5009120UL,0x0000000000004140UL});
    public static readonly BitSet FOLLOW_groupExprs_in_groupExprs387 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_set_in_orderDirection0 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_filters_in_whereExpr422 = new BitSet(new ulong[]{0x0000014404080442UL,0x00000F4800076000UL,0x0000000000000900UL});
    public static readonly BitSet FOLLOW_thetaJoins_in_whereExpr430 = new BitSet(new ulong[]{0x0000014404080442UL,0x00000F4800076000UL,0x0000000000000100UL});
    public static readonly BitSet FOLLOW_booleanExpr_in_whereExpr441 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_thetaJoins_in_whereExpr451 = new BitSet(new ulong[]{0x0000014404080442UL,0x00000F4800076000UL,0x0000000000000100UL});
    public static readonly BitSet FOLLOW_booleanExpr_in_whereExpr459 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_booleanExpr_in_whereExpr470 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_FILTERS_in_filters483 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_conditionList_in_filters485 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_THETA_JOINS_in_thetaJoins499 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_conditionList_in_thetaJoins501 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_sqlToken_in_conditionList514 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000000000000UL,0x0000000000000100UL});
    public static readonly BitSet FOLLOW_conditionList_in_conditionList520 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_SELECT_CLAUSE_in_selectClause535 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_distinctOrAll_in_selectClause538 = new BitSet(new ulong[]{0x0082208000109000UL,0x0071E003F10091A0UL,0x0000000000004540UL});
    public static readonly BitSet FOLLOW_selectColumn_in_selectClause544 = new BitSet(new ulong[]{0x0082208000109008UL,0x0071E003F10091A0UL,0x0000000000004540UL});
    public static readonly BitSet FOLLOW_selectExpr_in_selectColumn562 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000000000000UL,0x0000000000000200UL});
    public static readonly BitSet FOLLOW_SELECT_COLUMNS_in_selectColumn567 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_selectAtom_in_selectExpr587 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_count_in_selectExpr594 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_CONSTRUCTOR_in_selectExpr600 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_set_in_selectExpr602 = new BitSet(new ulong[]{0x0082208000109000UL,0x0071E003F10091A0UL,0x0000000000004540UL});
    public static readonly BitSet FOLLOW_selectColumn_in_selectExpr612 = new BitSet(new ulong[]{0x0082208000109008UL,0x0071E003F10091A0UL,0x0000000000004540UL});
    public static readonly BitSet FOLLOW_methodCall_in_selectExpr622 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_aggregate_in_selectExpr627 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_constant_in_selectExpr634 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_arithmeticExpr_in_selectExpr641 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_PARAM_in_selectExpr648 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_selectStatement_in_selectExpr658 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_COUNT_in_count672 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_distinctOrAll_in_count679 = new BitSet(new ulong[]{0x0082008000109000UL,0x0071E003F1409120UL,0x0000000000004140UL});
    public static readonly BitSet FOLLOW_countExpr_in_count685 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_DISTINCT_in_distinctOrAll700 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ALL_in_distinctOrAll708 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_ROW_STAR_in_countExpr727 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_simpleExpr_in_countExpr734 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_DOT_in_selectAtom746 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_SQL_TOKEN_in_selectAtom756 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_ALIAS_REF_in_selectAtom766 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_SELECT_EXPR_in_selectAtom776 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_FROM_in_from799 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_fromTable_in_from806 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000000000000UL,0x0000000000000005UL});
    public static readonly BitSet FOLLOW_FROM_FRAGMENT_in_fromTable832 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_tableJoin_in_fromTable838 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000000000000UL,0x0000000000000005UL});
    public static readonly BitSet FOLLOW_JOIN_FRAGMENT_in_fromTable853 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_tableJoin_in_fromTable859 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000000000000UL,0x0000000000000005UL});
    public static readonly BitSet FOLLOW_JOIN_FRAGMENT_in_tableJoin882 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_tableJoin_in_tableJoin887 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000000000000UL,0x0000000000000005UL});
    public static readonly BitSet FOLLOW_FROM_FRAGMENT_in_tableJoin903 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_tableJoin_in_tableJoin908 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000000000000UL,0x0000000000000005UL});
    public static readonly BitSet FOLLOW_AND_in_booleanOp928 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_booleanExpr_in_booleanOp930 = new BitSet(new ulong[]{0x0000014404080440UL,0x00000F4800076000UL,0x0000000000000100UL});
    public static readonly BitSet FOLLOW_booleanExpr_in_booleanOp935 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_OR_in_booleanOp943 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_booleanExpr_in_booleanOp947 = new BitSet(new ulong[]{0x0000014404080440UL,0x00000F4800076000UL,0x0000000000000100UL});
    public static readonly BitSet FOLLOW_booleanExpr_in_booleanOp952 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NOT_in_booleanOp962 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_booleanExpr_in_booleanOp966 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_booleanOp_in_booleanExpr983 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_comparisonExpr_in_booleanExpr990 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_SQL_TOKEN_in_booleanExpr999 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_binaryComparisonExpression_in_comparisonExpr1015 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_exoticComparisonExpression_in_comparisonExpr1022 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_EQ_in_binaryComparisonExpression1037 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1039 = new BitSet(new ulong[]{0x0082A08000109030UL,0x0071E003F5009120UL,0x0000000000004140UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1043 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NE_in_binaryComparisonExpression1050 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1052 = new BitSet(new ulong[]{0x0082A08000109030UL,0x0071E003F5009120UL,0x0000000000004140UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1056 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_GT_in_binaryComparisonExpression1063 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1065 = new BitSet(new ulong[]{0x0082A08000109030UL,0x0071E003F5009120UL,0x0000000000004140UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1069 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_GE_in_binaryComparisonExpression1076 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1078 = new BitSet(new ulong[]{0x0082A08000109030UL,0x0071E003F5009120UL,0x0000000000004140UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1082 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_LT_in_binaryComparisonExpression1089 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1091 = new BitSet(new ulong[]{0x0082A08000109030UL,0x0071E003F5009120UL,0x0000000000004140UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1095 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_LE_in_binaryComparisonExpression1102 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1104 = new BitSet(new ulong[]{0x0082A08000109030UL,0x0071E003F5009120UL,0x0000000000004140UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1108 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_LIKE_in_exoticComparisonExpression1122 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1124 = new BitSet(new ulong[]{0x0082A08000109030UL,0x0071E003F5009120UL,0x0000000000004140UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1128 = new BitSet(new ulong[]{0x0000000000040008UL});
    public static readonly BitSet FOLLOW_likeEscape_in_exoticComparisonExpression1130 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NOT_LIKE_in_exoticComparisonExpression1138 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1140 = new BitSet(new ulong[]{0x0082A08000109030UL,0x0071E003F5009120UL,0x0000000000004140UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1144 = new BitSet(new ulong[]{0x0000000000040008UL});
    public static readonly BitSet FOLLOW_likeEscape_in_exoticComparisonExpression1146 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_BETWEEN_in_exoticComparisonExpression1153 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1155 = new BitSet(new ulong[]{0x0082A08000109030UL,0x0071E003F5009120UL,0x0000000000004140UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1159 = new BitSet(new ulong[]{0x0082A08000109030UL,0x0071E003F5009120UL,0x0000000000004140UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1163 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NOT_BETWEEN_in_exoticComparisonExpression1170 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1172 = new BitSet(new ulong[]{0x0082A08000109030UL,0x0071E003F5009120UL,0x0000000000004140UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1176 = new BitSet(new ulong[]{0x0082A08000109030UL,0x0071E003F5009120UL,0x0000000000004140UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1180 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_IN_in_exoticComparisonExpression1187 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1189 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000800UL});
    public static readonly BitSet FOLLOW_inList_in_exoticComparisonExpression1193 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NOT_IN_in_exoticComparisonExpression1201 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1203 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000800UL});
    public static readonly BitSet FOLLOW_inList_in_exoticComparisonExpression1207 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_EXISTS_in_exoticComparisonExpression1215 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_quantified_in_exoticComparisonExpression1219 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_IS_NULL_in_exoticComparisonExpression1227 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1229 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_IS_NOT_NULL_in_exoticComparisonExpression1238 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1240 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_ESCAPE_in_likeEscape1257 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_likeEscape1261 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_IN_LIST_in_inList1277 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_parenSelect_in_inList1283 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_simpleExprList_in_inList1287 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_simpleExpr_in_simpleExprList1308 = new BitSet(new ulong[]{0x0082008000109002UL,0x0071E003F1009120UL,0x0000000000004140UL});
    public static readonly BitSet FOLLOW_simpleExpr_in_expr1327 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_VECTOR_EXPR_in_expr1334 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_expr1341 = new BitSet(new ulong[]{0x0082A08000109038UL,0x0071E003F5009120UL,0x0000000000004140UL});
    public static readonly BitSet FOLLOW_parenSelect_in_expr1356 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ANY_in_expr1362 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_quantified_in_expr1366 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_ALL_in_expr1374 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_quantified_in_expr1378 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_SOME_in_expr1386 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_quantified_in_expr1390 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_sqlToken_in_quantified1408 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_selectStatement_in_quantified1412 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_selectStatement_in_parenSelect1431 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_constant_in_simpleExpr1447 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_NULL_in_simpleExpr1454 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_addrExpr_in_simpleExpr1461 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_sqlToken_in_simpleExpr1466 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_aggregate_in_simpleExpr1471 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_methodCall_in_simpleExpr1476 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_count_in_simpleExpr1481 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_parameter_in_simpleExpr1486 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_arithmeticExpr_in_simpleExpr1491 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_set_in_constant0 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_additiveExpr_in_arithmeticExpr1560 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_multiplicativeExpr_in_arithmeticExpr1565 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_UNARY_MINUS_in_arithmeticExpr1572 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_arithmeticExpr1576 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_caseExpr_in_arithmeticExpr1582 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_PLUS_in_additiveExpr1594 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_additiveExpr1596 = new BitSet(new ulong[]{0x0082A08000109030UL,0x0071E003F5009120UL,0x0000000000004140UL});
    public static readonly BitSet FOLLOW_expr_in_additiveExpr1600 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_MINUS_in_additiveExpr1607 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_additiveExpr1609 = new BitSet(new ulong[]{0x0082A08000109030UL,0x0071E003F5009120UL,0x0000000000004140UL});
    public static readonly BitSet FOLLOW_nestedExprAfterMinusDiv_in_additiveExpr1613 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_STAR_in_multiplicativeExpr1626 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_nestedExpr_in_multiplicativeExpr1628 = new BitSet(new ulong[]{0x0082A08000109030UL,0x0071E003F5009120UL,0x0000000000004140UL});
    public static readonly BitSet FOLLOW_nestedExpr_in_multiplicativeExpr1632 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_DIV_in_multiplicativeExpr1639 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_nestedExpr_in_multiplicativeExpr1641 = new BitSet(new ulong[]{0x0082A08000109030UL,0x0071E003F5009120UL,0x0000000000004140UL});
    public static readonly BitSet FOLLOW_nestedExprAfterMinusDiv_in_multiplicativeExpr1645 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_additiveExpr_in_nestedExpr1667 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_expr_in_nestedExpr1674 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_arithmeticExpr_in_nestedExprAfterMinusDiv1696 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_expr_in_nestedExprAfterMinusDiv1703 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_CASE_in_caseExpr1715 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_WHEN_in_caseExpr1725 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_booleanExpr_in_caseExpr1729 = new BitSet(new ulong[]{0x0082A08000109030UL,0x0071E003F5009120UL,0x0000000000004140UL});
    public static readonly BitSet FOLLOW_expr_in_caseExpr1734 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_ELSE_in_caseExpr1746 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_caseExpr1750 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_CASE2_in_caseExpr1766 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_caseExpr1770 = new BitSet(new ulong[]{0x0800000000000000UL});
    public static readonly BitSet FOLLOW_WHEN_in_caseExpr1777 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_caseExpr1781 = new BitSet(new ulong[]{0x0082A08000109030UL,0x0071E003F5009120UL,0x0000000000004140UL});
    public static readonly BitSet FOLLOW_expr_in_caseExpr1785 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_ELSE_in_caseExpr1797 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_caseExpr1801 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_AGGREGATE_in_aggregate1825 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_aggregate1830 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_METHOD_CALL_in_methodCall1849 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_METHOD_NAME_in_methodCall1853 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000000000200UL});
    public static readonly BitSet FOLLOW_EXPR_LIST_in_methodCall1862 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_arguments_in_methodCall1865 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_expr_in_arguments1890 = new BitSet(new ulong[]{0x0082A08404189432UL,0x0071EF4BF507F120UL,0x0000000000004140UL});
    public static readonly BitSet FOLLOW_comparisonExpr_in_arguments1894 = new BitSet(new ulong[]{0x0082A08404189432UL,0x0071EF4BF507F120UL,0x0000000000004140UL});
    public static readonly BitSet FOLLOW_expr_in_arguments1903 = new BitSet(new ulong[]{0x0082A08404189432UL,0x0071EF4BF507F120UL,0x0000000000004140UL});
    public static readonly BitSet FOLLOW_comparisonExpr_in_arguments1907 = new BitSet(new ulong[]{0x0082A08404189432UL,0x0071EF4BF507F120UL,0x0000000000004140UL});
    public static readonly BitSet FOLLOW_NAMED_PARAM_in_parameter1925 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_PARAM_in_parameter1934 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_DOT_in_addrExpr1950 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_ALIAS_REF_in_addrExpr1964 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_INDEX_OP_in_addrExpr1974 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_SQL_TOKEN_in_sqlToken1994 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_SQL_TOKEN_in_synpred1_SqlGenerator323 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_additiveExpr_in_synpred2_SqlGenerator1660 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_arithmeticExpr_in_synpred3_SqlGenerator1689 = new BitSet(new ulong[]{0x0000000000000002UL});

}
}