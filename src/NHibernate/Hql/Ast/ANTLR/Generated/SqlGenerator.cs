// $ANTLR 3.2 Sep 23, 2009 12:02:23 SqlGenerator.g 2011-05-22 07:45:52

// The variable 'variable' is assigned but its value is never used.
#pragma warning disable 168, 219
// Unreachable code detected.
#pragma warning disable 162


using NHibernate.Hql.Ast.ANTLR.Tree;


using System;
using Antlr.Runtime;
using Antlr.Runtime.Tree;using IList 		= System.Collections.IList;
using ArrayList 	= System.Collections.ArrayList;
using Stack 		= Antlr.Runtime.Collections.StackList;

using IDictionary	= System.Collections.IDictionary;
using Hashtable 	= System.Collections.Hashtable;
namespace  NHibernate.Hql.Ast.ANTLR 
{
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
		"SKIP", 
		"SOME", 
		"SUM", 
		"TAKE", 
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
		"NUM_DECIMAL", 
		"NUM_FLOAT", 
		"NUM_LONG", 
		"JAVA_CONSTANT", 
		"COMMA", 
		"EQ", 
		"OPEN", 
		"CLOSE", 
		"COLON", 
		"PARAM", 
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

    public const int SELECT_COLUMNS = 144;
    public const int EXPONENT = 130;
    public const int LT = 109;
    public const int STAR = 120;
    public const int FLOAT_SUFFIX = 131;
    public const int FILTERS = 147;
    public const int LITERAL_by = 56;
    public const int PROPERTY_REF = 142;
    public const int THETA_JOINS = 146;
    public const int CASE = 57;
    public const int NEW = 37;
    public const int FILTER_ENTITY = 76;
    public const int PARAM = 106;
    public const int COUNT = 12;
    public const int NOT = 38;
    public const int EOF = -1;
    public const int UNARY_PLUS = 91;
    public const int QUOTED_String = 124;
    public const int WEIRD_IDENT = 93;
    public const int ESCqs = 128;
    public const int OPEN_BRACKET = 122;
    public const int FULL = 23;
    public const int ORDER_ELEMENT = 85;
    public const int INSERT = 29;
    public const int ESCAPE = 18;
    public const int IS_NULL = 80;
    public const int FROM_FRAGMENT = 135;
    public const int NAMED_PARAM = 149;
    public const int BOTH = 64;
    public const int SELECT_CLAUSE = 138;
    public const int NUM_DECIMAL = 97;
    public const int EQ = 102;
    public const int VERSIONED = 54;
    public const int SELECT = 45;
    public const int INTO = 30;
    public const int NE = 107;
    public const int GE = 112;
    public const int TAKE = 50;
    public const int ID_LETTER = 127;
    public const int CONCAT = 113;
    public const int NULL = 39;
    public const int ELSE = 59;
    public const int SELECT_FROM = 89;
    public const int TRAILING = 70;
    public const int ON = 62;
    public const int NUM_LONG = 99;
    public const int NUM_DOUBLE = 96;
    public const int UNARY_MINUS = 90;
    public const int DELETE = 13;
    public const int INDICES = 27;
    public const int OF = 69;
    public const int METHOD_CALL = 81;
    public const int LEADING = 66;
    public const int METHOD_NAME = 148;
    public const int SKIP = 47;
    public const int EMPTY = 65;
    public const int GROUP = 24;
    public const int WS = 129;
    public const int FETCH = 21;
    public const int VECTOR_EXPR = 92;
    public const int NOT_IN = 83;
    public const int SELECT_EXPR = 145;
    public const int NUM_INT = 95;
    public const int OR = 40;
    public const int ALIAS = 72;
    public const int JAVA_CONSTANT = 100;
    public const int CONSTANT = 94;
    public const int GT = 110;
    public const int QUERY = 86;
    public const int BNOT = 114;
    public const int INDEX_OP = 78;
    public const int NUM_FLOAT = 98;
    public const int FROM = 22;
    public const int END = 58;
    public const int FALSE = 20;
    public const int DISTINCT = 16;
    public const int CONSTRUCTOR = 73;
    public const int T__133 = 133;
    public const int T__134 = 134;
    public const int CLOSE_BRACKET = 123;
    public const int WHERE = 55;
    public const int CLASS = 11;
    public const int MEMBER = 67;
    public const int INNER = 28;
    public const int PROPERTIES = 43;
    public const int BOGUS = 150;
    public const int ORDER = 41;
    public const int MAX = 35;
    public const int UPDATE = 53;
    public const int JOIN_FRAGMENT = 137;
    public const int SUM = 49;
    public const int AND = 6;
    public const int SQL_NE = 108;
    public const int ASCENDING = 8;
    public const int EXPR_LIST = 75;
    public const int AS = 7;
    public const int THEN = 60;
    public const int IN = 26;
    public const int OBJECT = 68;
    public const int COMMA = 101;
    public const int IS = 31;
    public const int SQL_TOKEN = 143;
    public const int LEFT = 33;
    public const int AVG = 9;
    public const int SOME = 48;
    public const int BOR = 115;
    public const int ALL = 4;
    public const int IMPLIED_FROM = 136;
    public const int IDENT = 125;
    public const int PLUS = 118;
    public const int BXOR = 116;
    public const int CASE2 = 74;
    public const int EXISTS = 19;
    public const int DOT = 15;
    public const int LIKE = 34;
    public const int WITH = 63;
    public const int OUTER = 42;
    public const int ID_START_LETTER = 126;
    public const int LEFT_OUTER = 139;
    public const int ROW_STAR = 88;
    public const int NOT_LIKE = 84;
    public const int HEX_DIGIT = 132;
    public const int NOT_BETWEEN = 82;
    public const int RANGE = 87;
    public const int RIGHT_OUTER = 140;
    public const int RIGHT = 44;
    public const int SET = 46;
    public const int HAVING = 25;
    public const int MIN = 36;
    public const int MINUS = 119;
    public const int IS_NOT_NULL = 79;
    public const int BAND = 117;
    public const int ELEMENTS = 17;
    public const int TRUE = 51;
    public const int JOIN = 32;
    public const int UNION = 52;
    public const int IN_LIST = 77;
    public const int COLON = 105;
    public const int OPEN = 103;
    public const int ANY = 5;
    public const int CLOSE = 104;
    public const int WHEN = 61;
    public const int ALIAS_REF = 141;
    public const int DIV = 121;
    public const int DESCENDING = 14;
    public const int BETWEEN = 10;
    public const int AGGREGATE = 71;
    public const int LE = 111;

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
		get { return "SqlGenerator.g"; }
    }



    // $ANTLR start "statement"
    // SqlGenerator.g:27:1: statement : ( selectStatement | updateStatement | deleteStatement | insertStatement );
    public void statement() // throws RecognitionException [1]
    {   
        try 
    	{
            // SqlGenerator.g:28:2: ( selectStatement | updateStatement | deleteStatement | insertStatement )
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
                    // SqlGenerator.g:28:4: selectStatement
                    {
                    	PushFollow(FOLLOW_selectStatement_in_statement57);
                    	selectStatement();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // SqlGenerator.g:29:4: updateStatement
                    {
                    	PushFollow(FOLLOW_updateStatement_in_statement62);
                    	updateStatement();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 3 :
                    // SqlGenerator.g:30:4: deleteStatement
                    {
                    	PushFollow(FOLLOW_deleteStatement_in_statement67);
                    	deleteStatement();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 4 :
                    // SqlGenerator.g:31:4: insertStatement
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
    // SqlGenerator.g:34:1: selectStatement : ^( SELECT selectClause from ( ^( WHERE whereExpr ) )? ( ^( GROUP groupExprs ) )? ( ^( HAVING booleanExpr[false] ) )? ( ^( ORDER orderExprs ) )? ( ^( SKIP si= limitValue ) )? ( ^( TAKE ti= limitValue ) )? ) ;
    public void selectStatement() // throws RecognitionException [1]
    {   
        SqlGenerator.limitValue_return si = default(SqlGenerator.limitValue_return);

        SqlGenerator.limitValue_return ti = default(SqlGenerator.limitValue_return);


        try 
    	{
            // SqlGenerator.g:35:2: ( ^( SELECT selectClause from ( ^( WHERE whereExpr ) )? ( ^( GROUP groupExprs ) )? ( ^( HAVING booleanExpr[false] ) )? ( ^( ORDER orderExprs ) )? ( ^( SKIP si= limitValue ) )? ( ^( TAKE ti= limitValue ) )? ) )
            // SqlGenerator.g:35:4: ^( SELECT selectClause from ( ^( WHERE whereExpr ) )? ( ^( GROUP groupExprs ) )? ( ^( HAVING booleanExpr[false] ) )? ( ^( ORDER orderExprs ) )? ( ^( SKIP si= limitValue ) )? ( ^( TAKE ti= limitValue ) )? )
            {
            	Match(input,SELECT,FOLLOW_SELECT_in_selectStatement84); if (state.failed) return ;

            	if ( (state.backtracking==0) )
            	{
            	   StartQuery(); Out("select "); 
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
            	// SqlGenerator.g:38:3: ( ^( WHERE whereExpr ) )?
            	int alt2 = 2;
            	int LA2_0 = input.LA(1);

            	if ( (LA2_0 == WHERE) )
            	{
            	    alt2 = 1;
            	}
            	switch (alt2) 
            	{
            	    case 1 :
            	        // SqlGenerator.g:38:5: ^( WHERE whereExpr )
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

            	// SqlGenerator.g:39:3: ( ^( GROUP groupExprs ) )?
            	int alt3 = 2;
            	int LA3_0 = input.LA(1);

            	if ( (LA3_0 == GROUP) )
            	{
            	    alt3 = 1;
            	}
            	switch (alt3) 
            	{
            	    case 1 :
            	        // SqlGenerator.g:39:5: ^( GROUP groupExprs )
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

            	        	Match(input, Token.UP, null); if (state.failed) return ;

            	        }
            	        break;

            	}

            	// SqlGenerator.g:40:3: ( ^( HAVING booleanExpr[false] ) )?
            	int alt4 = 2;
            	int LA4_0 = input.LA(1);

            	if ( (LA4_0 == HAVING) )
            	{
            	    alt4 = 1;
            	}
            	switch (alt4) 
            	{
            	    case 1 :
            	        // SqlGenerator.g:40:5: ^( HAVING booleanExpr[false] )
            	        {
            	        	Match(input,HAVING,FOLLOW_HAVING_in_selectStatement133); if (state.failed) return ;

            	        	if ( (state.backtracking==0) )
            	        	{
            	        	   Out(" having "); 
            	        	}

            	        	Match(input, Token.DOWN, null); if (state.failed) return ;
            	        	PushFollow(FOLLOW_booleanExpr_in_selectStatement137);
            	        	booleanExpr(false);
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        	Match(input, Token.UP, null); if (state.failed) return ;

            	        }
            	        break;

            	}

            	// SqlGenerator.g:41:3: ( ^( ORDER orderExprs ) )?
            	int alt5 = 2;
            	int LA5_0 = input.LA(1);

            	if ( (LA5_0 == ORDER) )
            	{
            	    alt5 = 1;
            	}
            	switch (alt5) 
            	{
            	    case 1 :
            	        // SqlGenerator.g:41:5: ^( ORDER orderExprs )
            	        {
            	        	Match(input,ORDER,FOLLOW_ORDER_in_selectStatement149); if (state.failed) return ;

            	        	if ( (state.backtracking==0) )
            	        	{
            	        	   Out(" order by "); 
            	        	}

            	        	Match(input, Token.DOWN, null); if (state.failed) return ;
            	        	PushFollow(FOLLOW_orderExprs_in_selectStatement153);
            	        	orderExprs();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        	Match(input, Token.UP, null); if (state.failed) return ;

            	        }
            	        break;

            	}

            	// SqlGenerator.g:42:3: ( ^( SKIP si= limitValue ) )?
            	int alt6 = 2;
            	int LA6_0 = input.LA(1);

            	if ( (LA6_0 == SKIP) )
            	{
            	    alt6 = 1;
            	}
            	switch (alt6) 
            	{
            	    case 1 :
            	        // SqlGenerator.g:42:5: ^( SKIP si= limitValue )
            	        {
            	        	Match(input,SKIP,FOLLOW_SKIP_in_selectStatement165); if (state.failed) return ;

            	        	Match(input, Token.DOWN, null); if (state.failed) return ;
            	        	PushFollow(FOLLOW_limitValue_in_selectStatement169);
            	        	si = limitValue();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        	Match(input, Token.UP, null); if (state.failed) return ;
            	        	if ( (state.backtracking==0) )
            	        	{
            	        	   Skip(((si != null) ? ((IASTNode)si.Start) : null)); 
            	        	}

            	        }
            	        break;

            	}

            	// SqlGenerator.g:43:3: ( ^( TAKE ti= limitValue ) )?
            	int alt7 = 2;
            	int LA7_0 = input.LA(1);

            	if ( (LA7_0 == TAKE) )
            	{
            	    alt7 = 1;
            	}
            	switch (alt7) 
            	{
            	    case 1 :
            	        // SqlGenerator.g:43:5: ^( TAKE ti= limitValue )
            	        {
            	        	Match(input,TAKE,FOLLOW_TAKE_in_selectStatement181); if (state.failed) return ;

            	        	Match(input, Token.DOWN, null); if (state.failed) return ;
            	        	PushFollow(FOLLOW_limitValue_in_selectStatement185);
            	        	ti = limitValue();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        	Match(input, Token.UP, null); if (state.failed) return ;
            	        	if ( (state.backtracking==0) )
            	        	{
            	        	   Take(((ti != null) ? ((IASTNode)ti.Start) : null)); 
            	        	}

            	        }
            	        break;

            	}

            	if ( (state.backtracking==0) )
            	{
            	   EndQuery(); 
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
    // SqlGenerator.g:51:1: updateStatement : ^( UPDATE ^( FROM fromTable ) setClause ( whereClause )? ) ;
    public void updateStatement() // throws RecognitionException [1]
    {   
        try 
    	{
            // SqlGenerator.g:52:2: ( ^( UPDATE ^( FROM fromTable ) setClause ( whereClause )? ) )
            // SqlGenerator.g:52:4: ^( UPDATE ^( FROM fromTable ) setClause ( whereClause )? )
            {
            	Match(input,UPDATE,FOLLOW_UPDATE_in_updateStatement212); if (state.failed) return ;

            	if ( (state.backtracking==0) )
            	{
            	   Out("update "); 
            	}

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	Match(input,FROM,FOLLOW_FROM_in_updateStatement220); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_fromTable_in_updateStatement222);
            	fromTable();
            	state.followingStackPointer--;
            	if (state.failed) return ;

            	Match(input, Token.UP, null); if (state.failed) return ;
            	PushFollow(FOLLOW_setClause_in_updateStatement228);
            	setClause();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	// SqlGenerator.g:55:3: ( whereClause )?
            	int alt8 = 2;
            	int LA8_0 = input.LA(1);

            	if ( (LA8_0 == WHERE) )
            	{
            	    alt8 = 1;
            	}
            	switch (alt8) 
            	{
            	    case 1 :
            	        // SqlGenerator.g:55:4: whereClause
            	        {
            	        	PushFollow(FOLLOW_whereClause_in_updateStatement233);
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
    // SqlGenerator.g:59:1: deleteStatement : ^( DELETE from ( whereClause )? ) ;
    public void deleteStatement() // throws RecognitionException [1]
    {   
        try 
    	{
            // SqlGenerator.g:61:2: ( ^( DELETE from ( whereClause )? ) )
            // SqlGenerator.g:61:4: ^( DELETE from ( whereClause )? )
            {
            	Match(input,DELETE,FOLLOW_DELETE_in_deleteStatement252); if (state.failed) return ;

            	if ( (state.backtracking==0) )
            	{
            	   Out("delete"); 
            	}

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_from_in_deleteStatement258);
            	from();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	// SqlGenerator.g:63:3: ( whereClause )?
            	int alt9 = 2;
            	int LA9_0 = input.LA(1);

            	if ( (LA9_0 == WHERE) )
            	{
            	    alt9 = 1;
            	}
            	switch (alt9) 
            	{
            	    case 1 :
            	        // SqlGenerator.g:63:4: whereClause
            	        {
            	        	PushFollow(FOLLOW_whereClause_in_deleteStatement263);
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
    // SqlGenerator.g:67:1: insertStatement : ^( INSERT ^(i= INTO ( . )* ) selectStatement ) ;
    public void insertStatement() // throws RecognitionException [1]
    {   
        IASTNode i = null;

        try 
    	{
            // SqlGenerator.g:68:2: ( ^( INSERT ^(i= INTO ( . )* ) selectStatement ) )
            // SqlGenerator.g:68:4: ^( INSERT ^(i= INTO ( . )* ) selectStatement )
            {
            	Match(input,INSERT,FOLLOW_INSERT_in_insertStatement280); if (state.failed) return ;

            	if ( (state.backtracking==0) )
            	{
            	   Out( "insert " ); 
            	}

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	i=(IASTNode)Match(input,INTO,FOLLOW_INTO_in_insertStatement289); if (state.failed) return ;

            	if ( (state.backtracking==0) )
            	{
            	   Out( i ); Out( " " ); 
            	}

            	if ( input.LA(1) == Token.DOWN )
            	{
            	    Match(input, Token.DOWN, null); if (state.failed) return ;
            	    // SqlGenerator.g:69:38: ( . )*
            	    do 
            	    {
            	        int alt10 = 2;
            	        int LA10_0 = input.LA(1);

            	        if ( ((LA10_0 >= ALL && LA10_0 <= BOGUS)) )
            	        {
            	            alt10 = 1;
            	        }
            	        else if ( (LA10_0 == UP) )
            	        {
            	            alt10 = 2;
            	        }


            	        switch (alt10) 
            	    	{
            	    		case 1 :
            	    		    // SqlGenerator.g:69:38: .
            	    		    {
            	    		    	MatchAny(input); if (state.failed) return ;

            	    		    }
            	    		    break;

            	    		default:
            	    		    goto loop10;
            	        }
            	    } while (true);

            	    loop10:
            	    	;	// Stops C# compiler whining that label 'loop10' has no statements


            	    Match(input, Token.UP, null); if (state.failed) return ;
            	}
            	PushFollow(FOLLOW_selectStatement_in_insertStatement299);
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
    // SqlGenerator.g:74:1: setClause : ^( SET comparisonExpr[false] ( comparisonExpr[false] )* ) ;
    public void setClause() // throws RecognitionException [1]
    {   
        try 
    	{
            // SqlGenerator.g:77:2: ( ^( SET comparisonExpr[false] ( comparisonExpr[false] )* ) )
            // SqlGenerator.g:77:4: ^( SET comparisonExpr[false] ( comparisonExpr[false] )* )
            {
            	Match(input,SET,FOLLOW_SET_in_setClause319); if (state.failed) return ;

            	if ( (state.backtracking==0) )
            	{
            	   Out(" set "); 
            	}

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_comparisonExpr_in_setClause323);
            	comparisonExpr(false);
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	// SqlGenerator.g:77:51: ( comparisonExpr[false] )*
            	do 
            	{
            	    int alt11 = 2;
            	    int LA11_0 = input.LA(1);

            	    if ( (LA11_0 == BETWEEN || LA11_0 == EXISTS || LA11_0 == IN || LA11_0 == LIKE || (LA11_0 >= IS_NOT_NULL && LA11_0 <= IS_NULL) || (LA11_0 >= NOT_BETWEEN && LA11_0 <= NOT_LIKE) || LA11_0 == EQ || LA11_0 == NE || (LA11_0 >= LT && LA11_0 <= GE)) )
            	    {
            	        alt11 = 1;
            	    }


            	    switch (alt11) 
            		{
            			case 1 :
            			    // SqlGenerator.g:77:53: comparisonExpr[false]
            			    {
            			    	if ( (state.backtracking==0) )
            			    	{
            			    	   Out(", "); 
            			    	}
            			    	PushFollow(FOLLOW_comparisonExpr_in_setClause330);
            			    	comparisonExpr(false);
            			    	state.followingStackPointer--;
            			    	if (state.failed) return ;

            			    }
            			    break;

            			default:
            			    goto loop11;
            	    }
            	} while (true);

            	loop11:
            		;	// Stops C# compiler whining that label 'loop11' has no statements


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
    // SqlGenerator.g:80:1: whereClause : ^( WHERE whereClauseExpr ) ;
    public void whereClause() // throws RecognitionException [1]
    {   
        try 
    	{
            // SqlGenerator.g:81:2: ( ^( WHERE whereClauseExpr ) )
            // SqlGenerator.g:81:4: ^( WHERE whereClauseExpr )
            {
            	Match(input,WHERE,FOLLOW_WHERE_in_whereClause348); if (state.failed) return ;

            	if ( (state.backtracking==0) )
            	{
            	   Out(" where "); 
            	}

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_whereClauseExpr_in_whereClause352);
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
    // SqlGenerator.g:84:1: whereClauseExpr : ( ( SQL_TOKEN )=> conditionList | booleanExpr[ false ] );
    public void whereClauseExpr() // throws RecognitionException [1]
    {   
        try 
    	{
            // SqlGenerator.g:85:2: ( ( SQL_TOKEN )=> conditionList | booleanExpr[ false ] )
            int alt12 = 2;
            int LA12_0 = input.LA(1);

            if ( (LA12_0 == SQL_TOKEN) )
            {
                int LA12_1 = input.LA(2);

                if ( (LA12_1 == DOWN) && (synpred1_SqlGenerator()) )
                {
                    alt12 = 1;
                }
                else if ( (LA12_1 == UP) )
                {
                    alt12 = 2;
                }
                else 
                {
                    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    NoViableAltException nvae_d12s1 =
                        new NoViableAltException("", 12, 1, input);

                    throw nvae_d12s1;
                }
            }
            else if ( (LA12_0 == AND || LA12_0 == BETWEEN || LA12_0 == EXISTS || LA12_0 == IN || LA12_0 == LIKE || LA12_0 == NOT || LA12_0 == OR || (LA12_0 >= IS_NOT_NULL && LA12_0 <= NOT_LIKE) || LA12_0 == EQ || LA12_0 == NE || (LA12_0 >= LT && LA12_0 <= GE)) )
            {
                alt12 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d12s0 =
                    new NoViableAltException("", 12, 0, input);

                throw nvae_d12s0;
            }
            switch (alt12) 
            {
                case 1 :
                    // SqlGenerator.g:85:4: ( SQL_TOKEN )=> conditionList
                    {
                    	PushFollow(FOLLOW_conditionList_in_whereClauseExpr371);
                    	conditionList();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // SqlGenerator.g:86:4: booleanExpr[ false ]
                    {
                    	PushFollow(FOLLOW_booleanExpr_in_whereClauseExpr376);
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
    // SqlGenerator.g:89:1: orderExprs : ( expr ) (dir= orderDirection )? ( orderExprs )? ;
    public void orderExprs() // throws RecognitionException [1]
    {   
        SqlGenerator.orderDirection_return dir = default(SqlGenerator.orderDirection_return);


        try 
    	{
            // SqlGenerator.g:91:2: ( ( expr ) (dir= orderDirection )? ( orderExprs )? )
            // SqlGenerator.g:91:4: ( expr ) (dir= orderDirection )? ( orderExprs )?
            {
            	// SqlGenerator.g:91:4: ( expr )
            	// SqlGenerator.g:91:6: expr
            	{
            		PushFollow(FOLLOW_expr_in_orderExprs392);
            		expr();
            		state.followingStackPointer--;
            		if (state.failed) return ;

            	}

            	// SqlGenerator.g:91:13: (dir= orderDirection )?
            	int alt13 = 2;
            	int LA13_0 = input.LA(1);

            	if ( (LA13_0 == ASCENDING || LA13_0 == DESCENDING) )
            	{
            	    alt13 = 1;
            	}
            	switch (alt13) 
            	{
            	    case 1 :
            	        // SqlGenerator.g:91:14: dir= orderDirection
            	        {
            	        	PushFollow(FOLLOW_orderDirection_in_orderExprs399);
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

            	// SqlGenerator.g:91:66: ( orderExprs )?
            	int alt14 = 2;
            	int LA14_0 = input.LA(1);

            	if ( ((LA14_0 >= ALL && LA14_0 <= ANY) || LA14_0 == COUNT || LA14_0 == DOT || LA14_0 == FALSE || LA14_0 == NULL || LA14_0 == SELECT || LA14_0 == SOME || (LA14_0 >= TRUE && LA14_0 <= UNION) || LA14_0 == CASE || LA14_0 == AGGREGATE || LA14_0 == CASE2 || LA14_0 == INDEX_OP || LA14_0 == METHOD_CALL || LA14_0 == UNARY_MINUS || LA14_0 == VECTOR_EXPR || (LA14_0 >= CONSTANT && LA14_0 <= JAVA_CONSTANT) || LA14_0 == PARAM || (LA14_0 >= BNOT && LA14_0 <= DIV) || (LA14_0 >= QUOTED_String && LA14_0 <= IDENT) || LA14_0 == ALIAS_REF || LA14_0 == SQL_TOKEN || LA14_0 == NAMED_PARAM) )
            	{
            	    alt14 = 1;
            	}
            	switch (alt14) 
            	{
            	    case 1 :
            	        // SqlGenerator.g:91:68: orderExprs
            	        {
            	        	if ( (state.backtracking==0) )
            	        	{
            	        	  Out(", "); 
            	        	}
            	        	PushFollow(FOLLOW_orderExprs_in_orderExprs409);
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
    // SqlGenerator.g:94:1: groupExprs : expr ( groupExprs )? ;
    public void groupExprs() // throws RecognitionException [1]
    {   
        try 
    	{
            // SqlGenerator.g:96:2: ( expr ( groupExprs )? )
            // SqlGenerator.g:96:4: expr ( groupExprs )?
            {
            	PushFollow(FOLLOW_expr_in_groupExprs424);
            	expr();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	// SqlGenerator.g:96:9: ( groupExprs )?
            	int alt15 = 2;
            	int LA15_0 = input.LA(1);

            	if ( ((LA15_0 >= ALL && LA15_0 <= ANY) || LA15_0 == COUNT || LA15_0 == DOT || LA15_0 == FALSE || LA15_0 == NULL || LA15_0 == SELECT || LA15_0 == SOME || (LA15_0 >= TRUE && LA15_0 <= UNION) || LA15_0 == CASE || LA15_0 == AGGREGATE || LA15_0 == CASE2 || LA15_0 == INDEX_OP || LA15_0 == METHOD_CALL || LA15_0 == UNARY_MINUS || LA15_0 == VECTOR_EXPR || (LA15_0 >= CONSTANT && LA15_0 <= JAVA_CONSTANT) || LA15_0 == PARAM || (LA15_0 >= BNOT && LA15_0 <= DIV) || (LA15_0 >= QUOTED_String && LA15_0 <= IDENT) || LA15_0 == ALIAS_REF || LA15_0 == SQL_TOKEN || LA15_0 == NAMED_PARAM) )
            	{
            	    alt15 = 1;
            	}
            	switch (alt15) 
            	{
            	    case 1 :
            	        // SqlGenerator.g:96:11: groupExprs
            	        {
            	        	if ( (state.backtracking==0) )
            	        	{
            	        	  Out(" , "); 
            	        	}
            	        	PushFollow(FOLLOW_groupExprs_in_groupExprs430);
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
    // SqlGenerator.g:99:1: orderDirection : ( ASCENDING | DESCENDING );
    public SqlGenerator.orderDirection_return orderDirection() // throws RecognitionException [1]
    {   
        SqlGenerator.orderDirection_return retval = new SqlGenerator.orderDirection_return();
        retval.Start = input.LT(1);

        try 
    	{
            // SqlGenerator.g:100:2: ( ASCENDING | DESCENDING )
            // SqlGenerator.g:
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
    // SqlGenerator.g:104:1: whereExpr : ( filters ( thetaJoins )? ( booleanExpr[ true ] )? | thetaJoins ( booleanExpr[ true ] )? | booleanExpr[false] );
    public void whereExpr() // throws RecognitionException [1]
    {   
        try 
    	{
            // SqlGenerator.g:108:2: ( filters ( thetaJoins )? ( booleanExpr[ true ] )? | thetaJoins ( booleanExpr[ true ] )? | booleanExpr[false] )
            int alt19 = 3;
            switch ( input.LA(1) ) 
            {
            case FILTERS:
            	{
                alt19 = 1;
                }
                break;
            case THETA_JOINS:
            	{
                alt19 = 2;
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
            case METHOD_CALL:
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
                alt19 = 3;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d19s0 =
            	        new NoViableAltException("", 19, 0, input);

            	    throw nvae_d19s0;
            }

            switch (alt19) 
            {
                case 1 :
                    // SqlGenerator.g:108:4: filters ( thetaJoins )? ( booleanExpr[ true ] )?
                    {
                    	PushFollow(FOLLOW_filters_in_whereExpr465);
                    	filters();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	// SqlGenerator.g:109:3: ( thetaJoins )?
                    	int alt16 = 2;
                    	int LA16_0 = input.LA(1);

                    	if ( (LA16_0 == THETA_JOINS) )
                    	{
                    	    alt16 = 1;
                    	}
                    	switch (alt16) 
                    	{
                    	    case 1 :
                    	        // SqlGenerator.g:109:5: thetaJoins
                    	        {
                    	        	if ( (state.backtracking==0) )
                    	        	{
                    	        	   Out(" and "); 
                    	        	}
                    	        	PushFollow(FOLLOW_thetaJoins_in_whereExpr473);
                    	        	thetaJoins();
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;

                    	}

                    	// SqlGenerator.g:110:3: ( booleanExpr[ true ] )?
                    	int alt17 = 2;
                    	int LA17_0 = input.LA(1);

                    	if ( (LA17_0 == AND || LA17_0 == BETWEEN || LA17_0 == EXISTS || LA17_0 == IN || LA17_0 == LIKE || LA17_0 == NOT || LA17_0 == OR || (LA17_0 >= IS_NOT_NULL && LA17_0 <= NOT_LIKE) || LA17_0 == EQ || LA17_0 == NE || (LA17_0 >= LT && LA17_0 <= GE) || LA17_0 == SQL_TOKEN) )
                    	{
                    	    alt17 = 1;
                    	}
                    	switch (alt17) 
                    	{
                    	    case 1 :
                    	        // SqlGenerator.g:110:5: booleanExpr[ true ]
                    	        {
                    	        	if ( (state.backtracking==0) )
                    	        	{
                    	        	   Out(" and "); 
                    	        	}
                    	        	PushFollow(FOLLOW_booleanExpr_in_whereExpr484);
                    	        	booleanExpr(true);
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;

                    	}


                    }
                    break;
                case 2 :
                    // SqlGenerator.g:111:4: thetaJoins ( booleanExpr[ true ] )?
                    {
                    	PushFollow(FOLLOW_thetaJoins_in_whereExpr494);
                    	thetaJoins();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	// SqlGenerator.g:112:3: ( booleanExpr[ true ] )?
                    	int alt18 = 2;
                    	int LA18_0 = input.LA(1);

                    	if ( (LA18_0 == AND || LA18_0 == BETWEEN || LA18_0 == EXISTS || LA18_0 == IN || LA18_0 == LIKE || LA18_0 == NOT || LA18_0 == OR || (LA18_0 >= IS_NOT_NULL && LA18_0 <= NOT_LIKE) || LA18_0 == EQ || LA18_0 == NE || (LA18_0 >= LT && LA18_0 <= GE) || LA18_0 == SQL_TOKEN) )
                    	{
                    	    alt18 = 1;
                    	}
                    	switch (alt18) 
                    	{
                    	    case 1 :
                    	        // SqlGenerator.g:112:5: booleanExpr[ true ]
                    	        {
                    	        	if ( (state.backtracking==0) )
                    	        	{
                    	        	   Out(" and "); 
                    	        	}
                    	        	PushFollow(FOLLOW_booleanExpr_in_whereExpr502);
                    	        	booleanExpr(true);
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;

                    	}


                    }
                    break;
                case 3 :
                    // SqlGenerator.g:113:4: booleanExpr[false]
                    {
                    	PushFollow(FOLLOW_booleanExpr_in_whereExpr513);
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
    // SqlGenerator.g:116:1: filters : ^( FILTERS conditionList ) ;
    public void filters() // throws RecognitionException [1]
    {   
        try 
    	{
            // SqlGenerator.g:117:2: ( ^( FILTERS conditionList ) )
            // SqlGenerator.g:117:4: ^( FILTERS conditionList )
            {
            	Match(input,FILTERS,FOLLOW_FILTERS_in_filters526); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_conditionList_in_filters528);
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
    // SqlGenerator.g:120:1: thetaJoins : ^( THETA_JOINS conditionList ) ;
    public void thetaJoins() // throws RecognitionException [1]
    {   
        try 
    	{
            // SqlGenerator.g:121:2: ( ^( THETA_JOINS conditionList ) )
            // SqlGenerator.g:121:4: ^( THETA_JOINS conditionList )
            {
            	Match(input,THETA_JOINS,FOLLOW_THETA_JOINS_in_thetaJoins542); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_conditionList_in_thetaJoins544);
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
    // SqlGenerator.g:124:1: conditionList : sqlToken ( conditionList )? ;
    public void conditionList() // throws RecognitionException [1]
    {   
        try 
    	{
            // SqlGenerator.g:125:2: ( sqlToken ( conditionList )? )
            // SqlGenerator.g:125:4: sqlToken ( conditionList )?
            {
            	PushFollow(FOLLOW_sqlToken_in_conditionList557);
            	sqlToken();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	// SqlGenerator.g:125:13: ( conditionList )?
            	int alt20 = 2;
            	int LA20_0 = input.LA(1);

            	if ( (LA20_0 == SQL_TOKEN) )
            	{
            	    alt20 = 1;
            	}
            	switch (alt20) 
            	{
            	    case 1 :
            	        // SqlGenerator.g:125:15: conditionList
            	        {
            	        	if ( (state.backtracking==0) )
            	        	{
            	        	   Out(" and "); 
            	        	}
            	        	PushFollow(FOLLOW_conditionList_in_conditionList563);
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
    // SqlGenerator.g:128:1: selectClause : ^( SELECT_CLAUSE ( distinctOrAll )? ( selectColumn )+ ) ;
    public void selectClause() // throws RecognitionException [1]
    {   
        try 
    	{
            // SqlGenerator.g:129:2: ( ^( SELECT_CLAUSE ( distinctOrAll )? ( selectColumn )+ ) )
            // SqlGenerator.g:129:4: ^( SELECT_CLAUSE ( distinctOrAll )? ( selectColumn )+ )
            {
            	Match(input,SELECT_CLAUSE,FOLLOW_SELECT_CLAUSE_in_selectClause578); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	// SqlGenerator.g:129:20: ( distinctOrAll )?
            	int alt21 = 2;
            	int LA21_0 = input.LA(1);

            	if ( (LA21_0 == ALL || LA21_0 == DISTINCT) )
            	{
            	    alt21 = 1;
            	}
            	switch (alt21) 
            	{
            	    case 1 :
            	        // SqlGenerator.g:129:21: distinctOrAll
            	        {
            	        	PushFollow(FOLLOW_distinctOrAll_in_selectClause581);
            	        	distinctOrAll();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}

            	// SqlGenerator.g:129:37: ( selectColumn )+
            	int cnt22 = 0;
            	do 
            	{
            	    int alt22 = 2;
            	    int LA22_0 = input.LA(1);

            	    if ( (LA22_0 == COUNT || LA22_0 == DOT || LA22_0 == FALSE || LA22_0 == SELECT || LA22_0 == TRUE || LA22_0 == CASE || LA22_0 == AGGREGATE || (LA22_0 >= CONSTRUCTOR && LA22_0 <= CASE2) || LA22_0 == METHOD_CALL || LA22_0 == UNARY_MINUS || (LA22_0 >= CONSTANT && LA22_0 <= JAVA_CONSTANT) || LA22_0 == PARAM || (LA22_0 >= BNOT && LA22_0 <= DIV) || (LA22_0 >= QUOTED_String && LA22_0 <= IDENT) || LA22_0 == ALIAS_REF || LA22_0 == SQL_TOKEN || LA22_0 == SELECT_EXPR || LA22_0 == NAMED_PARAM) )
            	    {
            	        alt22 = 1;
            	    }


            	    switch (alt22) 
            		{
            			case 1 :
            			    // SqlGenerator.g:129:39: selectColumn
            			    {
            			    	PushFollow(FOLLOW_selectColumn_in_selectClause587);
            			    	selectColumn();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return ;

            			    }
            			    break;

            			default:
            			    if ( cnt22 >= 1 ) goto loop22;
            			    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            		            EarlyExitException eee22 =
            		                new EarlyExitException(22, input);
            		            throw eee22;
            	    }
            	    cnt22++;
            	} while (true);

            	loop22:
            		;	// Stops C# compiler whining that label 'loop22' has no statements


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
    // SqlGenerator.g:132:1: selectColumn : p= selectExpr (sc= SELECT_COLUMNS )? ;
    public void selectColumn() // throws RecognitionException [1]
    {   
        IASTNode sc = null;
        SqlGenerator.selectExpr_return p = default(SqlGenerator.selectExpr_return);


        try 
    	{
            // SqlGenerator.g:133:2: (p= selectExpr (sc= SELECT_COLUMNS )? )
            // SqlGenerator.g:133:4: p= selectExpr (sc= SELECT_COLUMNS )?
            {
            	PushFollow(FOLLOW_selectExpr_in_selectColumn605);
            	p = selectExpr();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	// SqlGenerator.g:133:17: (sc= SELECT_COLUMNS )?
            	int alt23 = 2;
            	int LA23_0 = input.LA(1);

            	if ( (LA23_0 == SELECT_COLUMNS) )
            	{
            	    alt23 = 1;
            	}
            	switch (alt23) 
            	{
            	    case 1 :
            	        // SqlGenerator.g:133:18: sc= SELECT_COLUMNS
            	        {
            	        	sc=(IASTNode)Match(input,SELECT_COLUMNS,FOLLOW_SELECT_COLUMNS_in_selectColumn610); if (state.failed) return ;
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
    // SqlGenerator.g:136:1: selectExpr : (e= selectAtom | count | ^( CONSTRUCTOR ( DOT | IDENT ) ( selectColumn )+ ) | methodCall | aggregate | c= constant | arithmeticExpr | parameter | selectStatement );
    public SqlGenerator.selectExpr_return selectExpr() // throws RecognitionException [1]
    {   
        SqlGenerator.selectExpr_return retval = new SqlGenerator.selectExpr_return();
        retval.Start = input.LT(1);

        SqlGenerator.selectAtom_return e = default(SqlGenerator.selectAtom_return);

        SqlGenerator.constant_return c = default(SqlGenerator.constant_return);


        try 
    	{
            // SqlGenerator.g:137:2: (e= selectAtom | count | ^( CONSTRUCTOR ( DOT | IDENT ) ( selectColumn )+ ) | methodCall | aggregate | c= constant | arithmeticExpr | parameter | selectStatement )
            int alt25 = 9;
            switch ( input.LA(1) ) 
            {
            case DOT:
            case ALIAS_REF:
            case SQL_TOKEN:
            case SELECT_EXPR:
            	{
                alt25 = 1;
                }
                break;
            case COUNT:
            	{
                alt25 = 2;
                }
                break;
            case CONSTRUCTOR:
            	{
                alt25 = 3;
                }
                break;
            case METHOD_CALL:
            	{
                alt25 = 4;
                }
                break;
            case AGGREGATE:
            	{
                alt25 = 5;
                }
                break;
            case FALSE:
            case TRUE:
            case CONSTANT:
            case NUM_INT:
            case NUM_DOUBLE:
            case NUM_DECIMAL:
            case NUM_FLOAT:
            case NUM_LONG:
            case JAVA_CONSTANT:
            case QUOTED_String:
            case IDENT:
            	{
                alt25 = 6;
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
                alt25 = 7;
                }
                break;
            case PARAM:
            case NAMED_PARAM:
            	{
                alt25 = 8;
                }
                break;
            case SELECT:
            	{
                alt25 = 9;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
            	    NoViableAltException nvae_d25s0 =
            	        new NoViableAltException("", 25, 0, input);

            	    throw nvae_d25s0;
            }

            switch (alt25) 
            {
                case 1 :
                    // SqlGenerator.g:137:4: e= selectAtom
                    {
                    	PushFollow(FOLLOW_selectAtom_in_selectExpr630);
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
                    // SqlGenerator.g:138:4: count
                    {
                    	PushFollow(FOLLOW_count_in_selectExpr637);
                    	count();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 3 :
                    // SqlGenerator.g:139:4: ^( CONSTRUCTOR ( DOT | IDENT ) ( selectColumn )+ )
                    {
                    	Match(input,CONSTRUCTOR,FOLLOW_CONSTRUCTOR_in_selectExpr643); if (state.failed) return retval;

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

                    	// SqlGenerator.g:139:32: ( selectColumn )+
                    	int cnt24 = 0;
                    	do 
                    	{
                    	    int alt24 = 2;
                    	    int LA24_0 = input.LA(1);

                    	    if ( (LA24_0 == COUNT || LA24_0 == DOT || LA24_0 == FALSE || LA24_0 == SELECT || LA24_0 == TRUE || LA24_0 == CASE || LA24_0 == AGGREGATE || (LA24_0 >= CONSTRUCTOR && LA24_0 <= CASE2) || LA24_0 == METHOD_CALL || LA24_0 == UNARY_MINUS || (LA24_0 >= CONSTANT && LA24_0 <= JAVA_CONSTANT) || LA24_0 == PARAM || (LA24_0 >= BNOT && LA24_0 <= DIV) || (LA24_0 >= QUOTED_String && LA24_0 <= IDENT) || LA24_0 == ALIAS_REF || LA24_0 == SQL_TOKEN || LA24_0 == SELECT_EXPR || LA24_0 == NAMED_PARAM) )
                    	    {
                    	        alt24 = 1;
                    	    }


                    	    switch (alt24) 
                    		{
                    			case 1 :
                    			    // SqlGenerator.g:139:34: selectColumn
                    			    {
                    			    	PushFollow(FOLLOW_selectColumn_in_selectExpr655);
                    			    	selectColumn();
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return retval;

                    			    }
                    			    break;

                    			default:
                    			    if ( cnt24 >= 1 ) goto loop24;
                    			    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
                    		            EarlyExitException eee24 =
                    		                new EarlyExitException(24, input);
                    		            throw eee24;
                    	    }
                    	    cnt24++;
                    	} while (true);

                    	loop24:
                    		;	// Stops C# compiler whining that label 'loop24' has no statements


                    	Match(input, Token.UP, null); if (state.failed) return retval;

                    }
                    break;
                case 4 :
                    // SqlGenerator.g:140:4: methodCall
                    {
                    	PushFollow(FOLLOW_methodCall_in_selectExpr665);
                    	methodCall();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 5 :
                    // SqlGenerator.g:141:4: aggregate
                    {
                    	PushFollow(FOLLOW_aggregate_in_selectExpr670);
                    	aggregate();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 6 :
                    // SqlGenerator.g:142:4: c= constant
                    {
                    	PushFollow(FOLLOW_constant_in_selectExpr677);
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
                    // SqlGenerator.g:143:4: arithmeticExpr
                    {
                    	PushFollow(FOLLOW_arithmeticExpr_in_selectExpr684);
                    	arithmeticExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 8 :
                    // SqlGenerator.g:144:4: parameter
                    {
                    	PushFollow(FOLLOW_parameter_in_selectExpr689);
                    	parameter();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 9 :
                    // SqlGenerator.g:147:4: selectStatement
                    {
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("("); 
                    	}
                    	PushFollow(FOLLOW_selectStatement_in_selectExpr698);
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
    // SqlGenerator.g:150:1: count : ^( COUNT ( distinctOrAll )? countExpr ) ;
    public void count() // throws RecognitionException [1]
    {   
        try 
    	{
            // SqlGenerator.g:151:2: ( ^( COUNT ( distinctOrAll )? countExpr ) )
            // SqlGenerator.g:151:4: ^( COUNT ( distinctOrAll )? countExpr )
            {
            	Match(input,COUNT,FOLLOW_COUNT_in_count712); if (state.failed) return ;

            	if ( (state.backtracking==0) )
            	{
            	   Out("count("); 
            	}

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	// SqlGenerator.g:151:32: ( distinctOrAll )?
            	int alt26 = 2;
            	int LA26_0 = input.LA(1);

            	if ( (LA26_0 == ALL || LA26_0 == DISTINCT) )
            	{
            	    alt26 = 1;
            	}
            	switch (alt26) 
            	{
            	    case 1 :
            	        // SqlGenerator.g:151:34: distinctOrAll
            	        {
            	        	PushFollow(FOLLOW_distinctOrAll_in_count719);
            	        	distinctOrAll();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_countExpr_in_count725);
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
    // SqlGenerator.g:154:1: distinctOrAll : ( DISTINCT | ^( ALL ( . )* ) );
    public void distinctOrAll() // throws RecognitionException [1]
    {   
        try 
    	{
            // SqlGenerator.g:155:2: ( DISTINCT | ^( ALL ( . )* ) )
            int alt28 = 2;
            int LA28_0 = input.LA(1);

            if ( (LA28_0 == DISTINCT) )
            {
                alt28 = 1;
            }
            else if ( (LA28_0 == ALL) )
            {
                alt28 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d28s0 =
                    new NoViableAltException("", 28, 0, input);

                throw nvae_d28s0;
            }
            switch (alt28) 
            {
                case 1 :
                    // SqlGenerator.g:155:4: DISTINCT
                    {
                    	Match(input,DISTINCT,FOLLOW_DISTINCT_in_distinctOrAll740); if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("distinct "); 
                    	}

                    }
                    break;
                case 2 :
                    // SqlGenerator.g:156:4: ^( ALL ( . )* )
                    {
                    	Match(input,ALL,FOLLOW_ALL_in_distinctOrAll748); if (state.failed) return ;

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return ;
                    	    // SqlGenerator.g:156:10: ( . )*
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
                    	    		    // SqlGenerator.g:156:10: .
                    	    		    {
                    	    		    	MatchAny(input); if (state.failed) return ;

                    	    		    }
                    	    		    break;

                    	    		default:
                    	    		    goto loop27;
                    	        }
                    	    } while (true);

                    	    loop27:
                    	    	;	// Stops C# compiler whining that label 'loop27' has no statements


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
    // SqlGenerator.g:159:1: countExpr : ( ROW_STAR | simpleExpr );
    public void countExpr() // throws RecognitionException [1]
    {   
        try 
    	{
            // SqlGenerator.g:161:2: ( ROW_STAR | simpleExpr )
            int alt29 = 2;
            int LA29_0 = input.LA(1);

            if ( (LA29_0 == ROW_STAR) )
            {
                alt29 = 1;
            }
            else if ( (LA29_0 == COUNT || LA29_0 == DOT || LA29_0 == FALSE || LA29_0 == NULL || LA29_0 == TRUE || LA29_0 == CASE || LA29_0 == AGGREGATE || LA29_0 == CASE2 || LA29_0 == INDEX_OP || LA29_0 == METHOD_CALL || LA29_0 == UNARY_MINUS || (LA29_0 >= CONSTANT && LA29_0 <= JAVA_CONSTANT) || LA29_0 == PARAM || (LA29_0 >= BNOT && LA29_0 <= DIV) || (LA29_0 >= QUOTED_String && LA29_0 <= IDENT) || LA29_0 == ALIAS_REF || LA29_0 == SQL_TOKEN || LA29_0 == NAMED_PARAM) )
            {
                alt29 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d29s0 =
                    new NoViableAltException("", 29, 0, input);

                throw nvae_d29s0;
            }
            switch (alt29) 
            {
                case 1 :
                    // SqlGenerator.g:161:4: ROW_STAR
                    {
                    	Match(input,ROW_STAR,FOLLOW_ROW_STAR_in_countExpr767); if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("*"); 
                    	}

                    }
                    break;
                case 2 :
                    // SqlGenerator.g:162:4: simpleExpr
                    {
                    	PushFollow(FOLLOW_simpleExpr_in_countExpr774);
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
    // SqlGenerator.g:165:1: selectAtom : ( ^( DOT ( . )* ) | ^( SQL_TOKEN ( . )* ) | ^( ALIAS_REF ( . )* ) | ^( SELECT_EXPR ( . )* ) );
    public SqlGenerator.selectAtom_return selectAtom() // throws RecognitionException [1]
    {   
        SqlGenerator.selectAtom_return retval = new SqlGenerator.selectAtom_return();
        retval.Start = input.LT(1);

        try 
    	{
            // SqlGenerator.g:166:2: ( ^( DOT ( . )* ) | ^( SQL_TOKEN ( . )* ) | ^( ALIAS_REF ( . )* ) | ^( SELECT_EXPR ( . )* ) )
            int alt34 = 4;
            switch ( input.LA(1) ) 
            {
            case DOT:
            	{
                alt34 = 1;
                }
                break;
            case SQL_TOKEN:
            	{
                alt34 = 2;
                }
                break;
            case ALIAS_REF:
            	{
                alt34 = 3;
                }
                break;
            case SELECT_EXPR:
            	{
                alt34 = 4;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
            	    NoViableAltException nvae_d34s0 =
            	        new NoViableAltException("", 34, 0, input);

            	    throw nvae_d34s0;
            }

            switch (alt34) 
            {
                case 1 :
                    // SqlGenerator.g:166:4: ^( DOT ( . )* )
                    {
                    	Match(input,DOT,FOLLOW_DOT_in_selectAtom786); if (state.failed) return retval;

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return retval;
                    	    // SqlGenerator.g:166:10: ( . )*
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
                    	    		    // SqlGenerator.g:166:10: .
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
                case 2 :
                    // SqlGenerator.g:167:4: ^( SQL_TOKEN ( . )* )
                    {
                    	Match(input,SQL_TOKEN,FOLLOW_SQL_TOKEN_in_selectAtom796); if (state.failed) return retval;

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return retval;
                    	    // SqlGenerator.g:167:16: ( . )*
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
                    	    		    // SqlGenerator.g:167:16: .
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
                case 3 :
                    // SqlGenerator.g:168:4: ^( ALIAS_REF ( . )* )
                    {
                    	Match(input,ALIAS_REF,FOLLOW_ALIAS_REF_in_selectAtom806); if (state.failed) return retval;

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return retval;
                    	    // SqlGenerator.g:168:16: ( . )*
                    	    do 
                    	    {
                    	        int alt32 = 2;
                    	        int LA32_0 = input.LA(1);

                    	        if ( ((LA32_0 >= ALL && LA32_0 <= BOGUS)) )
                    	        {
                    	            alt32 = 1;
                    	        }
                    	        else if ( (LA32_0 == UP) )
                    	        {
                    	            alt32 = 2;
                    	        }


                    	        switch (alt32) 
                    	    	{
                    	    		case 1 :
                    	    		    // SqlGenerator.g:168:16: .
                    	    		    {
                    	    		    	MatchAny(input); if (state.failed) return retval;

                    	    		    }
                    	    		    break;

                    	    		default:
                    	    		    goto loop32;
                    	        }
                    	    } while (true);

                    	    loop32:
                    	    	;	// Stops C# compiler whining that label 'loop32' has no statements


                    	    Match(input, Token.UP, null); if (state.failed) return retval;
                    	}

                    }
                    break;
                case 4 :
                    // SqlGenerator.g:169:4: ^( SELECT_EXPR ( . )* )
                    {
                    	Match(input,SELECT_EXPR,FOLLOW_SELECT_EXPR_in_selectAtom816); if (state.failed) return retval;

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return retval;
                    	    // SqlGenerator.g:169:18: ( . )*
                    	    do 
                    	    {
                    	        int alt33 = 2;
                    	        int LA33_0 = input.LA(1);

                    	        if ( ((LA33_0 >= ALL && LA33_0 <= BOGUS)) )
                    	        {
                    	            alt33 = 1;
                    	        }
                    	        else if ( (LA33_0 == UP) )
                    	        {
                    	            alt33 = 2;
                    	        }


                    	        switch (alt33) 
                    	    	{
                    	    		case 1 :
                    	    		    // SqlGenerator.g:169:18: .
                    	    		    {
                    	    		    	MatchAny(input); if (state.failed) return retval;

                    	    		    }
                    	    		    break;

                    	    		default:
                    	    		    goto loop33;
                    	        }
                    	    } while (true);

                    	    loop33:
                    	    	;	// Stops C# compiler whining that label 'loop33' has no statements


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
    // SqlGenerator.g:177:1: from : ^(f= FROM ( fromTable )* ) ;
    public void from() // throws RecognitionException [1]
    {   
        IASTNode f = null;

        try 
    	{
            // SqlGenerator.g:178:2: ( ^(f= FROM ( fromTable )* ) )
            // SqlGenerator.g:178:4: ^(f= FROM ( fromTable )* )
            {
            	f=(IASTNode)Match(input,FROM,FOLLOW_FROM_in_from839); if (state.failed) return ;

            	if ( (state.backtracking==0) )
            	{
            	   Out(" from "); 
            	}

            	if ( input.LA(1) == Token.DOWN )
            	{
            	    Match(input, Token.DOWN, null); if (state.failed) return ;
            	    // SqlGenerator.g:179:3: ( fromTable )*
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
            	    		    // SqlGenerator.g:179:4: fromTable
            	    		    {
            	    		    	PushFollow(FOLLOW_fromTable_in_from846);
            	    		    	fromTable();
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
    // SqlGenerator.g:182:1: fromTable : ( ^(a= FROM_FRAGMENT ( tableJoin[ a ] )* ) | ^(a= JOIN_FRAGMENT ( tableJoin[ a ] )* ) );
    public void fromTable() // throws RecognitionException [1]
    {   
        IASTNode a = null;

        try 
    	{
            // SqlGenerator.g:187:2: ( ^(a= FROM_FRAGMENT ( tableJoin[ a ] )* ) | ^(a= JOIN_FRAGMENT ( tableJoin[ a ] )* ) )
            int alt38 = 2;
            int LA38_0 = input.LA(1);

            if ( (LA38_0 == FROM_FRAGMENT) )
            {
                alt38 = 1;
            }
            else if ( (LA38_0 == JOIN_FRAGMENT) )
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
                    // SqlGenerator.g:187:4: ^(a= FROM_FRAGMENT ( tableJoin[ a ] )* )
                    {
                    	a=(IASTNode)Match(input,FROM_FRAGMENT,FOLLOW_FROM_FRAGMENT_in_fromTable872); if (state.failed) return ;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(a); 
                    	}

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return ;
                    	    // SqlGenerator.g:187:36: ( tableJoin[ a ] )*
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
                    	    		    // SqlGenerator.g:187:37: tableJoin[ a ]
                    	    		    {
                    	    		    	PushFollow(FOLLOW_tableJoin_in_fromTable878);
                    	    		    	tableJoin(a);
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
                    // SqlGenerator.g:188:4: ^(a= JOIN_FRAGMENT ( tableJoin[ a ] )* )
                    {
                    	a=(IASTNode)Match(input,JOIN_FRAGMENT,FOLLOW_JOIN_FRAGMENT_in_fromTable893); if (state.failed) return ;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(a); 
                    	}

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return ;
                    	    // SqlGenerator.g:188:36: ( tableJoin[ a ] )*
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
                    	    		    // SqlGenerator.g:188:37: tableJoin[ a ]
                    	    		    {
                    	    		    	PushFollow(FOLLOW_tableJoin_in_fromTable899);
                    	    		    	tableJoin(a);
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
    // SqlGenerator.g:191:1: tableJoin[ IASTNode parent ] : ( ^(c= JOIN_FRAGMENT ( tableJoin[ c ] )* ) | ^(d= FROM_FRAGMENT ( tableJoin[ d ] )* ) );
    public void tableJoin(IASTNode parent) // throws RecognitionException [1]
    {   
        IASTNode c = null;
        IASTNode d = null;

        try 
    	{
            // SqlGenerator.g:192:2: ( ^(c= JOIN_FRAGMENT ( tableJoin[ c ] )* ) | ^(d= FROM_FRAGMENT ( tableJoin[ d ] )* ) )
            int alt41 = 2;
            int LA41_0 = input.LA(1);

            if ( (LA41_0 == JOIN_FRAGMENT) )
            {
                alt41 = 1;
            }
            else if ( (LA41_0 == FROM_FRAGMENT) )
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
                    // SqlGenerator.g:192:4: ^(c= JOIN_FRAGMENT ( tableJoin[ c ] )* )
                    {
                    	c=(IASTNode)Match(input,JOIN_FRAGMENT,FOLLOW_JOIN_FRAGMENT_in_tableJoin922); if (state.failed) return ;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" "); Out(c); 
                    	}

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return ;
                    	    // SqlGenerator.g:192:46: ( tableJoin[ c ] )*
                    	    do 
                    	    {
                    	        int alt39 = 2;
                    	        int LA39_0 = input.LA(1);

                    	        if ( (LA39_0 == FROM_FRAGMENT || LA39_0 == JOIN_FRAGMENT) )
                    	        {
                    	            alt39 = 1;
                    	        }


                    	        switch (alt39) 
                    	    	{
                    	    		case 1 :
                    	    		    // SqlGenerator.g:192:47: tableJoin[ c ]
                    	    		    {
                    	    		    	PushFollow(FOLLOW_tableJoin_in_tableJoin927);
                    	    		    	tableJoin(c);
                    	    		    	state.followingStackPointer--;
                    	    		    	if (state.failed) return ;

                    	    		    }
                    	    		    break;

                    	    		default:
                    	    		    goto loop39;
                    	        }
                    	    } while (true);

                    	    loop39:
                    	    	;	// Stops C# compiler whining that label 'loop39' has no statements


                    	    Match(input, Token.UP, null); if (state.failed) return ;
                    	}

                    }
                    break;
                case 2 :
                    // SqlGenerator.g:193:4: ^(d= FROM_FRAGMENT ( tableJoin[ d ] )* )
                    {
                    	d=(IASTNode)Match(input,FROM_FRAGMENT,FOLLOW_FROM_FRAGMENT_in_tableJoin943); if (state.failed) return ;

                    	if ( (state.backtracking==0) )
                    	{
                    	   NestedFromFragment(d,parent); 
                    	}

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return ;
                    	    // SqlGenerator.g:193:58: ( tableJoin[ d ] )*
                    	    do 
                    	    {
                    	        int alt40 = 2;
                    	        int LA40_0 = input.LA(1);

                    	        if ( (LA40_0 == FROM_FRAGMENT || LA40_0 == JOIN_FRAGMENT) )
                    	        {
                    	            alt40 = 1;
                    	        }


                    	        switch (alt40) 
                    	    	{
                    	    		case 1 :
                    	    		    // SqlGenerator.g:193:59: tableJoin[ d ]
                    	    		    {
                    	    		    	PushFollow(FOLLOW_tableJoin_in_tableJoin948);
                    	    		    	tableJoin(d);
                    	    		    	state.followingStackPointer--;
                    	    		    	if (state.failed) return ;

                    	    		    }
                    	    		    break;

                    	    		default:
                    	    		    goto loop40;
                    	        }
                    	    } while (true);

                    	    loop40:
                    	    	;	// Stops C# compiler whining that label 'loop40' has no statements


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
    // SqlGenerator.g:196:1: booleanOp[ bool parens ] : ( ^( AND booleanExpr[true] booleanExpr[true] ) | ^( OR booleanExpr[false] booleanExpr[false] ) | ^( NOT booleanExpr[false] ) );
    public void booleanOp(bool parens) // throws RecognitionException [1]
    {   
        try 
    	{
            // SqlGenerator.g:197:2: ( ^( AND booleanExpr[true] booleanExpr[true] ) | ^( OR booleanExpr[false] booleanExpr[false] ) | ^( NOT booleanExpr[false] ) )
            int alt42 = 3;
            switch ( input.LA(1) ) 
            {
            case AND:
            	{
                alt42 = 1;
                }
                break;
            case OR:
            	{
                alt42 = 2;
                }
                break;
            case NOT:
            	{
                alt42 = 3;
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
                    // SqlGenerator.g:197:4: ^( AND booleanExpr[true] booleanExpr[true] )
                    {
                    	Match(input,AND,FOLLOW_AND_in_booleanOp968); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_booleanExpr_in_booleanOp970);
                    	booleanExpr(true);
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" and "); 
                    	}
                    	PushFollow(FOLLOW_booleanExpr_in_booleanOp975);
                    	booleanExpr(true);
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // SqlGenerator.g:198:4: ^( OR booleanExpr[false] booleanExpr[false] )
                    {
                    	Match(input,OR,FOLLOW_OR_in_booleanOp983); if (state.failed) return ;

                    	if ( (state.backtracking==0) )
                    	{
                    	   if (parens) Out("("); 
                    	}

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_booleanExpr_in_booleanOp987);
                    	booleanExpr(false);
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" or "); 
                    	}
                    	PushFollow(FOLLOW_booleanExpr_in_booleanOp992);
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
                    // SqlGenerator.g:199:4: ^( NOT booleanExpr[false] )
                    {
                    	Match(input,NOT,FOLLOW_NOT_in_booleanOp1002); if (state.failed) return ;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" not ("); 
                    	}

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_booleanExpr_in_booleanOp1006);
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
    // SqlGenerator.g:202:1: booleanExpr[ bool parens ] : ( booleanOp[ parens ] | comparisonExpr[ parens ] | methodCall | st= SQL_TOKEN );
    public void booleanExpr(bool parens) // throws RecognitionException [1]
    {   
        IASTNode st = null;

        try 
    	{
            // SqlGenerator.g:203:2: ( booleanOp[ parens ] | comparisonExpr[ parens ] | methodCall | st= SQL_TOKEN )
            int alt43 = 4;
            switch ( input.LA(1) ) 
            {
            case AND:
            case NOT:
            case OR:
            	{
                alt43 = 1;
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
                alt43 = 2;
                }
                break;
            case METHOD_CALL:
            	{
                alt43 = 3;
                }
                break;
            case SQL_TOKEN:
            	{
                alt43 = 4;
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
                    // SqlGenerator.g:203:4: booleanOp[ parens ]
                    {
                    	PushFollow(FOLLOW_booleanOp_in_booleanExpr1023);
                    	booleanOp(parens);
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // SqlGenerator.g:204:4: comparisonExpr[ parens ]
                    {
                    	PushFollow(FOLLOW_comparisonExpr_in_booleanExpr1030);
                    	comparisonExpr(parens);
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 3 :
                    // SqlGenerator.g:205:4: methodCall
                    {
                    	PushFollow(FOLLOW_methodCall_in_booleanExpr1037);
                    	methodCall();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 4 :
                    // SqlGenerator.g:206:4: st= SQL_TOKEN
                    {
                    	st=(IASTNode)Match(input,SQL_TOKEN,FOLLOW_SQL_TOKEN_in_booleanExpr1044); if (state.failed) return ;
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
    // SqlGenerator.g:209:1: comparisonExpr[ bool parens ] : ( binaryComparisonExpression | exoticComparisonExpression );
    public void comparisonExpr(bool parens) // throws RecognitionException [1]
    {   
        try 
    	{
            // SqlGenerator.g:210:2: ( binaryComparisonExpression | exoticComparisonExpression )
            int alt44 = 2;
            int LA44_0 = input.LA(1);

            if ( (LA44_0 == EQ || LA44_0 == NE || (LA44_0 >= LT && LA44_0 <= GE)) )
            {
                alt44 = 1;
            }
            else if ( (LA44_0 == BETWEEN || LA44_0 == EXISTS || LA44_0 == IN || LA44_0 == LIKE || (LA44_0 >= IS_NOT_NULL && LA44_0 <= IS_NULL) || (LA44_0 >= NOT_BETWEEN && LA44_0 <= NOT_LIKE)) )
            {
                alt44 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d44s0 =
                    new NoViableAltException("", 44, 0, input);

                throw nvae_d44s0;
            }
            switch (alt44) 
            {
                case 1 :
                    // SqlGenerator.g:210:4: binaryComparisonExpression
                    {
                    	PushFollow(FOLLOW_binaryComparisonExpression_in_comparisonExpr1060);
                    	binaryComparisonExpression();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // SqlGenerator.g:211:4: exoticComparisonExpression
                    {
                    	if ( (state.backtracking==0) )
                    	{
                    	   if (parens) Out("("); 
                    	}
                    	PushFollow(FOLLOW_exoticComparisonExpression_in_comparisonExpr1067);
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
    // SqlGenerator.g:214:1: binaryComparisonExpression : ( ^( EQ expr expr ) | ^( NE expr expr ) | ^( GT expr expr ) | ^( GE expr expr ) | ^( LT expr expr ) | ^( LE expr expr ) );
    public void binaryComparisonExpression() // throws RecognitionException [1]
    {   
        try 
    	{
            // SqlGenerator.g:215:2: ( ^( EQ expr expr ) | ^( NE expr expr ) | ^( GT expr expr ) | ^( GE expr expr ) | ^( LT expr expr ) | ^( LE expr expr ) )
            int alt45 = 6;
            switch ( input.LA(1) ) 
            {
            case EQ:
            	{
                alt45 = 1;
                }
                break;
            case NE:
            	{
                alt45 = 2;
                }
                break;
            case GT:
            	{
                alt45 = 3;
                }
                break;
            case GE:
            	{
                alt45 = 4;
                }
                break;
            case LT:
            	{
                alt45 = 5;
                }
                break;
            case LE:
            	{
                alt45 = 6;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d45s0 =
            	        new NoViableAltException("", 45, 0, input);

            	    throw nvae_d45s0;
            }

            switch (alt45) 
            {
                case 1 :
                    // SqlGenerator.g:215:4: ^( EQ expr expr )
                    {
                    	Match(input,EQ,FOLLOW_EQ_in_binaryComparisonExpression1082); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1084);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("="); 
                    	}
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1088);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // SqlGenerator.g:216:4: ^( NE expr expr )
                    {
                    	Match(input,NE,FOLLOW_NE_in_binaryComparisonExpression1095); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1097);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("<>"); 
                    	}
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1101);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 3 :
                    // SqlGenerator.g:217:4: ^( GT expr expr )
                    {
                    	Match(input,GT,FOLLOW_GT_in_binaryComparisonExpression1108); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1110);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(">"); 
                    	}
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1114);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 4 :
                    // SqlGenerator.g:218:4: ^( GE expr expr )
                    {
                    	Match(input,GE,FOLLOW_GE_in_binaryComparisonExpression1121); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1123);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(">="); 
                    	}
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1127);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 5 :
                    // SqlGenerator.g:219:4: ^( LT expr expr )
                    {
                    	Match(input,LT,FOLLOW_LT_in_binaryComparisonExpression1134); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1136);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("<"); 
                    	}
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1140);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 6 :
                    // SqlGenerator.g:220:4: ^( LE expr expr )
                    {
                    	Match(input,LE,FOLLOW_LE_in_binaryComparisonExpression1147); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1149);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("<="); 
                    	}
                    	PushFollow(FOLLOW_expr_in_binaryComparisonExpression1153);
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
    // SqlGenerator.g:223:1: exoticComparisonExpression : ( ^( LIKE expr expr likeEscape ) | ^( NOT_LIKE expr expr likeEscape ) | ^( BETWEEN expr expr expr ) | ^( NOT_BETWEEN expr expr expr ) | ^( IN expr inList ) | ^( NOT_IN expr inList ) | ^( EXISTS quantified ) | ^( IS_NULL expr ) | ^( IS_NOT_NULL expr ) );
    public void exoticComparisonExpression() // throws RecognitionException [1]
    {   
        try 
    	{
            // SqlGenerator.g:224:2: ( ^( LIKE expr expr likeEscape ) | ^( NOT_LIKE expr expr likeEscape ) | ^( BETWEEN expr expr expr ) | ^( NOT_BETWEEN expr expr expr ) | ^( IN expr inList ) | ^( NOT_IN expr inList ) | ^( EXISTS quantified ) | ^( IS_NULL expr ) | ^( IS_NOT_NULL expr ) )
            int alt46 = 9;
            switch ( input.LA(1) ) 
            {
            case LIKE:
            	{
                alt46 = 1;
                }
                break;
            case NOT_LIKE:
            	{
                alt46 = 2;
                }
                break;
            case BETWEEN:
            	{
                alt46 = 3;
                }
                break;
            case NOT_BETWEEN:
            	{
                alt46 = 4;
                }
                break;
            case IN:
            	{
                alt46 = 5;
                }
                break;
            case NOT_IN:
            	{
                alt46 = 6;
                }
                break;
            case EXISTS:
            	{
                alt46 = 7;
                }
                break;
            case IS_NULL:
            	{
                alt46 = 8;
                }
                break;
            case IS_NOT_NULL:
            	{
                alt46 = 9;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d46s0 =
            	        new NoViableAltException("", 46, 0, input);

            	    throw nvae_d46s0;
            }

            switch (alt46) 
            {
                case 1 :
                    // SqlGenerator.g:224:4: ^( LIKE expr expr likeEscape )
                    {
                    	Match(input,LIKE,FOLLOW_LIKE_in_exoticComparisonExpression1167); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1169);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" like "); 
                    	}
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1173);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_likeEscape_in_exoticComparisonExpression1175);
                    	likeEscape();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // SqlGenerator.g:225:4: ^( NOT_LIKE expr expr likeEscape )
                    {
                    	Match(input,NOT_LIKE,FOLLOW_NOT_LIKE_in_exoticComparisonExpression1183); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1185);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" not like "); 
                    	}
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1189);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_likeEscape_in_exoticComparisonExpression1191);
                    	likeEscape();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 3 :
                    // SqlGenerator.g:226:4: ^( BETWEEN expr expr expr )
                    {
                    	Match(input,BETWEEN,FOLLOW_BETWEEN_in_exoticComparisonExpression1198); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1200);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" between "); 
                    	}
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1204);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" and "); 
                    	}
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1208);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 4 :
                    // SqlGenerator.g:227:4: ^( NOT_BETWEEN expr expr expr )
                    {
                    	Match(input,NOT_BETWEEN,FOLLOW_NOT_BETWEEN_in_exoticComparisonExpression1215); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1217);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" not between "); 
                    	}
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1221);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" and "); 
                    	}
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1225);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 5 :
                    // SqlGenerator.g:228:4: ^( IN expr inList )
                    {
                    	Match(input,IN,FOLLOW_IN_in_exoticComparisonExpression1232); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1234);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" in"); 
                    	}
                    	PushFollow(FOLLOW_inList_in_exoticComparisonExpression1238);
                    	inList();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 6 :
                    // SqlGenerator.g:229:4: ^( NOT_IN expr inList )
                    {
                    	Match(input,NOT_IN,FOLLOW_NOT_IN_in_exoticComparisonExpression1246); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1248);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(" not in "); 
                    	}
                    	PushFollow(FOLLOW_inList_in_exoticComparisonExpression1252);
                    	inList();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 7 :
                    // SqlGenerator.g:230:4: ^( EXISTS quantified )
                    {
                    	Match(input,EXISTS,FOLLOW_EXISTS_in_exoticComparisonExpression1260); if (state.failed) return ;

                    	if ( (state.backtracking==0) )
                    	{
                    	   OptionalSpace(); Out("exists "); 
                    	}

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_quantified_in_exoticComparisonExpression1264);
                    	quantified();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 8 :
                    // SqlGenerator.g:231:4: ^( IS_NULL expr )
                    {
                    	Match(input,IS_NULL,FOLLOW_IS_NULL_in_exoticComparisonExpression1272); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1274);
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
                    // SqlGenerator.g:232:4: ^( IS_NOT_NULL expr )
                    {
                    	Match(input,IS_NOT_NULL,FOLLOW_IS_NOT_NULL_in_exoticComparisonExpression1283); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_exoticComparisonExpression1285);
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
    // SqlGenerator.g:235:1: likeEscape : ( ^( ESCAPE expr ) )? ;
    public void likeEscape() // throws RecognitionException [1]
    {   
        try 
    	{
            // SqlGenerator.g:236:2: ( ( ^( ESCAPE expr ) )? )
            // SqlGenerator.g:236:4: ( ^( ESCAPE expr ) )?
            {
            	// SqlGenerator.g:236:4: ( ^( ESCAPE expr ) )?
            	int alt47 = 2;
            	int LA47_0 = input.LA(1);

            	if ( (LA47_0 == ESCAPE) )
            	{
            	    alt47 = 1;
            	}
            	switch (alt47) 
            	{
            	    case 1 :
            	        // SqlGenerator.g:236:6: ^( ESCAPE expr )
            	        {
            	        	Match(input,ESCAPE,FOLLOW_ESCAPE_in_likeEscape1302); if (state.failed) return ;

            	        	if ( (state.backtracking==0) )
            	        	{
            	        	   Out(" escape "); 
            	        	}

            	        	Match(input, Token.DOWN, null); if (state.failed) return ;
            	        	PushFollow(FOLLOW_expr_in_likeEscape1306);
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
    // SqlGenerator.g:239:1: inList : ^( IN_LIST ( parenSelect | simpleExprList ) ) ;
    public void inList() // throws RecognitionException [1]
    {   
        try 
    	{
            // SqlGenerator.g:240:2: ( ^( IN_LIST ( parenSelect | simpleExprList ) ) )
            // SqlGenerator.g:240:4: ^( IN_LIST ( parenSelect | simpleExprList ) )
            {
            	Match(input,IN_LIST,FOLLOW_IN_LIST_in_inList1322); if (state.failed) return ;

            	if ( (state.backtracking==0) )
            	{
            	   Out(" "); 
            	}

            	if ( input.LA(1) == Token.DOWN )
            	{
            	    Match(input, Token.DOWN, null); if (state.failed) return ;
            	    // SqlGenerator.g:240:28: ( parenSelect | simpleExprList )
            	    int alt48 = 2;
            	    int LA48_0 = input.LA(1);

            	    if ( (LA48_0 == SELECT || LA48_0 == UNION) )
            	    {
            	        alt48 = 1;
            	    }
            	    else if ( (LA48_0 == UP || LA48_0 == COUNT || LA48_0 == DOT || LA48_0 == FALSE || LA48_0 == NULL || LA48_0 == TRUE || LA48_0 == CASE || LA48_0 == AGGREGATE || LA48_0 == CASE2 || LA48_0 == INDEX_OP || LA48_0 == METHOD_CALL || LA48_0 == UNARY_MINUS || (LA48_0 >= CONSTANT && LA48_0 <= JAVA_CONSTANT) || LA48_0 == PARAM || (LA48_0 >= BNOT && LA48_0 <= DIV) || (LA48_0 >= QUOTED_String && LA48_0 <= IDENT) || LA48_0 == ALIAS_REF || LA48_0 == SQL_TOKEN || LA48_0 == NAMED_PARAM) )
            	    {
            	        alt48 = 2;
            	    }
            	    else 
            	    {
            	        if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	        NoViableAltException nvae_d48s0 =
            	            new NoViableAltException("", 48, 0, input);

            	        throw nvae_d48s0;
            	    }
            	    switch (alt48) 
            	    {
            	        case 1 :
            	            // SqlGenerator.g:240:30: parenSelect
            	            {
            	            	PushFollow(FOLLOW_parenSelect_in_inList1328);
            	            	parenSelect();
            	            	state.followingStackPointer--;
            	            	if (state.failed) return ;

            	            }
            	            break;
            	        case 2 :
            	            // SqlGenerator.g:240:44: simpleExprList
            	            {
            	            	PushFollow(FOLLOW_simpleExprList_in_inList1332);
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
    // SqlGenerator.g:243:1: simpleExprList : (e= simpleExpr )* ;
    public void simpleExprList() // throws RecognitionException [1]
    {   
        SqlGenerator.simpleExpr_return e = default(SqlGenerator.simpleExpr_return);


        try 
    	{
            // SqlGenerator.g:244:2: ( (e= simpleExpr )* )
            // SqlGenerator.g:244:4: (e= simpleExpr )*
            {
            	if ( (state.backtracking==0) )
            	{
            	   Out("("); 
            	}
            	// SqlGenerator.g:244:18: (e= simpleExpr )*
            	do 
            	{
            	    int alt49 = 2;
            	    int LA49_0 = input.LA(1);

            	    if ( (LA49_0 == COUNT || LA49_0 == DOT || LA49_0 == FALSE || LA49_0 == NULL || LA49_0 == TRUE || LA49_0 == CASE || LA49_0 == AGGREGATE || LA49_0 == CASE2 || LA49_0 == INDEX_OP || LA49_0 == METHOD_CALL || LA49_0 == UNARY_MINUS || (LA49_0 >= CONSTANT && LA49_0 <= JAVA_CONSTANT) || LA49_0 == PARAM || (LA49_0 >= BNOT && LA49_0 <= DIV) || (LA49_0 >= QUOTED_String && LA49_0 <= IDENT) || LA49_0 == ALIAS_REF || LA49_0 == SQL_TOKEN || LA49_0 == NAMED_PARAM) )
            	    {
            	        alt49 = 1;
            	    }


            	    switch (alt49) 
            		{
            			case 1 :
            			    // SqlGenerator.g:244:19: e= simpleExpr
            			    {
            			    	PushFollow(FOLLOW_simpleExpr_in_simpleExprList1353);
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
            			    goto loop49;
            	    }
            	} while (true);

            	loop49:
            		;	// Stops C# compiler whining that label 'loop49' has no statements

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
    // SqlGenerator.g:248:1: expr : ( simpleExpr | ^( VECTOR_EXPR (e= expr )* ) | parenSelect | ^( ANY quantified ) | ^( ALL quantified ) | ^( SOME quantified ) );
    public SqlGenerator.expr_return expr() // throws RecognitionException [1]
    {   
        SqlGenerator.expr_return retval = new SqlGenerator.expr_return();
        retval.Start = input.LT(1);

        SqlGenerator.expr_return e = default(SqlGenerator.expr_return);


        try 
    	{
            // SqlGenerator.g:249:2: ( simpleExpr | ^( VECTOR_EXPR (e= expr )* ) | parenSelect | ^( ANY quantified ) | ^( ALL quantified ) | ^( SOME quantified ) )
            int alt51 = 6;
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
            case NUM_DECIMAL:
            case NUM_FLOAT:
            case NUM_LONG:
            case JAVA_CONSTANT:
            case PARAM:
            case BNOT:
            case BOR:
            case BXOR:
            case BAND:
            case PLUS:
            case MINUS:
            case STAR:
            case DIV:
            case QUOTED_String:
            case IDENT:
            case ALIAS_REF:
            case SQL_TOKEN:
            case NAMED_PARAM:
            	{
                alt51 = 1;
                }
                break;
            case VECTOR_EXPR:
            	{
                alt51 = 2;
                }
                break;
            case SELECT:
            case UNION:
            	{
                alt51 = 3;
                }
                break;
            case ANY:
            	{
                alt51 = 4;
                }
                break;
            case ALL:
            	{
                alt51 = 5;
                }
                break;
            case SOME:
            	{
                alt51 = 6;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
            	    NoViableAltException nvae_d51s0 =
            	        new NoViableAltException("", 51, 0, input);

            	    throw nvae_d51s0;
            }

            switch (alt51) 
            {
                case 1 :
                    // SqlGenerator.g:249:4: simpleExpr
                    {
                    	PushFollow(FOLLOW_simpleExpr_in_expr1372);
                    	simpleExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 2 :
                    // SqlGenerator.g:250:4: ^( VECTOR_EXPR (e= expr )* )
                    {
                    	Match(input,VECTOR_EXPR,FOLLOW_VECTOR_EXPR_in_expr1379); if (state.failed) return retval;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("("); 
                    	}

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return retval;
                    	    // SqlGenerator.g:250:33: (e= expr )*
                    	    do 
                    	    {
                    	        int alt50 = 2;
                    	        int LA50_0 = input.LA(1);

                    	        if ( ((LA50_0 >= ALL && LA50_0 <= ANY) || LA50_0 == COUNT || LA50_0 == DOT || LA50_0 == FALSE || LA50_0 == NULL || LA50_0 == SELECT || LA50_0 == SOME || (LA50_0 >= TRUE && LA50_0 <= UNION) || LA50_0 == CASE || LA50_0 == AGGREGATE || LA50_0 == CASE2 || LA50_0 == INDEX_OP || LA50_0 == METHOD_CALL || LA50_0 == UNARY_MINUS || LA50_0 == VECTOR_EXPR || (LA50_0 >= CONSTANT && LA50_0 <= JAVA_CONSTANT) || LA50_0 == PARAM || (LA50_0 >= BNOT && LA50_0 <= DIV) || (LA50_0 >= QUOTED_String && LA50_0 <= IDENT) || LA50_0 == ALIAS_REF || LA50_0 == SQL_TOKEN || LA50_0 == NAMED_PARAM) )
                    	        {
                    	            alt50 = 1;
                    	        }


                    	        switch (alt50) 
                    	    	{
                    	    		case 1 :
                    	    		    // SqlGenerator.g:250:34: e= expr
                    	    		    {
                    	    		    	PushFollow(FOLLOW_expr_in_expr1386);
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
                    	    		    goto loop50;
                    	        }
                    	    } while (true);

                    	    loop50:
                    	    	;	// Stops C# compiler whining that label 'loop50' has no statements

                    	    if ( (state.backtracking==0) )
                    	    {
                    	       Out(")"); 
                    	    }

                    	    Match(input, Token.UP, null); if (state.failed) return retval;
                    	}

                    }
                    break;
                case 3 :
                    // SqlGenerator.g:251:4: parenSelect
                    {
                    	PushFollow(FOLLOW_parenSelect_in_expr1401);
                    	parenSelect();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 4 :
                    // SqlGenerator.g:252:4: ^( ANY quantified )
                    {
                    	Match(input,ANY,FOLLOW_ANY_in_expr1407); if (state.failed) return retval;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("any "); 
                    	}

                    	Match(input, Token.DOWN, null); if (state.failed) return retval;
                    	PushFollow(FOLLOW_quantified_in_expr1411);
                    	quantified();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    	Match(input, Token.UP, null); if (state.failed) return retval;

                    }
                    break;
                case 5 :
                    // SqlGenerator.g:253:4: ^( ALL quantified )
                    {
                    	Match(input,ALL,FOLLOW_ALL_in_expr1419); if (state.failed) return retval;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("all "); 
                    	}

                    	Match(input, Token.DOWN, null); if (state.failed) return retval;
                    	PushFollow(FOLLOW_quantified_in_expr1423);
                    	quantified();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    	Match(input, Token.UP, null); if (state.failed) return retval;

                    }
                    break;
                case 6 :
                    // SqlGenerator.g:254:4: ^( SOME quantified )
                    {
                    	Match(input,SOME,FOLLOW_SOME_in_expr1431); if (state.failed) return retval;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("some "); 
                    	}

                    	Match(input, Token.DOWN, null); if (state.failed) return retval;
                    	PushFollow(FOLLOW_quantified_in_expr1435);
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
    // SqlGenerator.g:257:1: quantified : ( sqlToken | selectStatement ) ;
    public void quantified() // throws RecognitionException [1]
    {   
        try 
    	{
            // SqlGenerator.g:258:2: ( ( sqlToken | selectStatement ) )
            // SqlGenerator.g:258:4: ( sqlToken | selectStatement )
            {
            	if ( (state.backtracking==0) )
            	{
            	   Out("("); 
            	}
            	// SqlGenerator.g:258:18: ( sqlToken | selectStatement )
            	int alt52 = 2;
            	int LA52_0 = input.LA(1);

            	if ( (LA52_0 == SQL_TOKEN) )
            	{
            	    alt52 = 1;
            	}
            	else if ( (LA52_0 == SELECT) )
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
            	        // SqlGenerator.g:258:20: sqlToken
            	        {
            	        	PushFollow(FOLLOW_sqlToken_in_quantified1453);
            	        	sqlToken();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;
            	    case 2 :
            	        // SqlGenerator.g:258:31: selectStatement
            	        {
            	        	PushFollow(FOLLOW_selectStatement_in_quantified1457);
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
    // SqlGenerator.g:261:1: parenSelect : ( selectStatement | ^( UNION selectStatement parenSelect ) );
    public void parenSelect() // throws RecognitionException [1]
    {   
        try 
    	{
            // SqlGenerator.g:262:2: ( selectStatement | ^( UNION selectStatement parenSelect ) )
            int alt53 = 2;
            int LA53_0 = input.LA(1);

            if ( (LA53_0 == SELECT) )
            {
                alt53 = 1;
            }
            else if ( (LA53_0 == UNION) )
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
                    // SqlGenerator.g:262:4: selectStatement
                    {
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("("); 
                    	}
                    	PushFollow(FOLLOW_selectStatement_in_parenSelect1476);
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
                    // SqlGenerator.g:263:4: ^( UNION selectStatement parenSelect )
                    {
                    	Match(input,UNION,FOLLOW_UNION_in_parenSelect1485); if (state.failed) return ;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("("); 
                    	}

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_selectStatement_in_parenSelect1489);
                    	selectStatement();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(") union "); 
                    	}
                    	PushFollow(FOLLOW_parenSelect_in_parenSelect1493);
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
    // SqlGenerator.g:267:1: simpleExpr : (c= constant | NULL | addrExpr | sqlToken | aggregate | methodCall | count | parameter | arithmeticExpr );
    public SqlGenerator.simpleExpr_return simpleExpr() // throws RecognitionException [1]
    {   
        SqlGenerator.simpleExpr_return retval = new SqlGenerator.simpleExpr_return();
        retval.Start = input.LT(1);

        SqlGenerator.constant_return c = default(SqlGenerator.constant_return);


        try 
    	{
            // SqlGenerator.g:268:2: (c= constant | NULL | addrExpr | sqlToken | aggregate | methodCall | count | parameter | arithmeticExpr )
            int alt54 = 9;
            switch ( input.LA(1) ) 
            {
            case FALSE:
            case TRUE:
            case CONSTANT:
            case NUM_INT:
            case NUM_DOUBLE:
            case NUM_DECIMAL:
            case NUM_FLOAT:
            case NUM_LONG:
            case JAVA_CONSTANT:
            case QUOTED_String:
            case IDENT:
            	{
                alt54 = 1;
                }
                break;
            case NULL:
            	{
                alt54 = 2;
                }
                break;
            case DOT:
            case INDEX_OP:
            case ALIAS_REF:
            	{
                alt54 = 3;
                }
                break;
            case SQL_TOKEN:
            	{
                alt54 = 4;
                }
                break;
            case AGGREGATE:
            	{
                alt54 = 5;
                }
                break;
            case METHOD_CALL:
            	{
                alt54 = 6;
                }
                break;
            case COUNT:
            	{
                alt54 = 7;
                }
                break;
            case PARAM:
            case NAMED_PARAM:
            	{
                alt54 = 8;
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
                alt54 = 9;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
            	    NoViableAltException nvae_d54s0 =
            	        new NoViableAltException("", 54, 0, input);

            	    throw nvae_d54s0;
            }

            switch (alt54) 
            {
                case 1 :
                    // SqlGenerator.g:268:4: c= constant
                    {
                    	PushFollow(FOLLOW_constant_in_simpleExpr1510);
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
                    // SqlGenerator.g:269:4: NULL
                    {
                    	Match(input,NULL,FOLLOW_NULL_in_simpleExpr1517); if (state.failed) return retval;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("null"); 
                    	}

                    }
                    break;
                case 3 :
                    // SqlGenerator.g:270:4: addrExpr
                    {
                    	PushFollow(FOLLOW_addrExpr_in_simpleExpr1524);
                    	addrExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 4 :
                    // SqlGenerator.g:271:4: sqlToken
                    {
                    	PushFollow(FOLLOW_sqlToken_in_simpleExpr1529);
                    	sqlToken();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 5 :
                    // SqlGenerator.g:272:4: aggregate
                    {
                    	PushFollow(FOLLOW_aggregate_in_simpleExpr1534);
                    	aggregate();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 6 :
                    // SqlGenerator.g:273:4: methodCall
                    {
                    	PushFollow(FOLLOW_methodCall_in_simpleExpr1539);
                    	methodCall();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 7 :
                    // SqlGenerator.g:274:4: count
                    {
                    	PushFollow(FOLLOW_count_in_simpleExpr1544);
                    	count();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 8 :
                    // SqlGenerator.g:275:4: parameter
                    {
                    	PushFollow(FOLLOW_parameter_in_simpleExpr1549);
                    	parameter();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;

                    }
                    break;
                case 9 :
                    // SqlGenerator.g:276:4: arithmeticExpr
                    {
                    	PushFollow(FOLLOW_arithmeticExpr_in_simpleExpr1554);
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
    // SqlGenerator.g:279:1: constant : ( NUM_DOUBLE | NUM_DECIMAL | NUM_FLOAT | NUM_INT | NUM_LONG | QUOTED_String | CONSTANT | JAVA_CONSTANT | TRUE | FALSE | IDENT );
    public SqlGenerator.constant_return constant() // throws RecognitionException [1]
    {   
        SqlGenerator.constant_return retval = new SqlGenerator.constant_return();
        retval.Start = input.LT(1);

        try 
    	{
            // SqlGenerator.g:280:2: ( NUM_DOUBLE | NUM_DECIMAL | NUM_FLOAT | NUM_INT | NUM_LONG | QUOTED_String | CONSTANT | JAVA_CONSTANT | TRUE | FALSE | IDENT )
            // SqlGenerator.g:
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
    // SqlGenerator.g:293:1: arithmeticExpr : ( additiveExpr | bitwiseExpr | multiplicativeExpr | ^( UNARY_MINUS expr ) | caseExpr );
    public void arithmeticExpr() // throws RecognitionException [1]
    {   
        try 
    	{
            // SqlGenerator.g:294:2: ( additiveExpr | bitwiseExpr | multiplicativeExpr | ^( UNARY_MINUS expr ) | caseExpr )
            int alt55 = 5;
            switch ( input.LA(1) ) 
            {
            case PLUS:
            case MINUS:
            	{
                alt55 = 1;
                }
                break;
            case BNOT:
            case BOR:
            case BXOR:
            case BAND:
            	{
                alt55 = 2;
                }
                break;
            case STAR:
            case DIV:
            	{
                alt55 = 3;
                }
                break;
            case UNARY_MINUS:
            	{
                alt55 = 4;
                }
                break;
            case CASE:
            case CASE2:
            	{
                alt55 = 5;
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
                    // SqlGenerator.g:294:4: additiveExpr
                    {
                    	PushFollow(FOLLOW_additiveExpr_in_arithmeticExpr1628);
                    	additiveExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // SqlGenerator.g:295:4: bitwiseExpr
                    {
                    	PushFollow(FOLLOW_bitwiseExpr_in_arithmeticExpr1633);
                    	bitwiseExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 3 :
                    // SqlGenerator.g:296:4: multiplicativeExpr
                    {
                    	PushFollow(FOLLOW_multiplicativeExpr_in_arithmeticExpr1638);
                    	multiplicativeExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 4 :
                    // SqlGenerator.g:298:4: ^( UNARY_MINUS expr )
                    {
                    	Match(input,UNARY_MINUS,FOLLOW_UNARY_MINUS_in_arithmeticExpr1645); if (state.failed) return ;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("-"); 
                    	}

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_arithmeticExpr1649);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 5 :
                    // SqlGenerator.g:299:4: caseExpr
                    {
                    	PushFollow(FOLLOW_caseExpr_in_arithmeticExpr1655);
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
    // SqlGenerator.g:302:1: additiveExpr : ( ^( PLUS expr expr ) | ^( MINUS expr nestedExprAfterMinusDiv ) );
    public void additiveExpr() // throws RecognitionException [1]
    {   
        try 
    	{
            // SqlGenerator.g:303:2: ( ^( PLUS expr expr ) | ^( MINUS expr nestedExprAfterMinusDiv ) )
            int alt56 = 2;
            int LA56_0 = input.LA(1);

            if ( (LA56_0 == PLUS) )
            {
                alt56 = 1;
            }
            else if ( (LA56_0 == MINUS) )
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
                    // SqlGenerator.g:303:4: ^( PLUS expr expr )
                    {
                    	Match(input,PLUS,FOLLOW_PLUS_in_additiveExpr1667); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_additiveExpr1669);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("+"); 
                    	}
                    	PushFollow(FOLLOW_expr_in_additiveExpr1673);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // SqlGenerator.g:304:4: ^( MINUS expr nestedExprAfterMinusDiv )
                    {
                    	Match(input,MINUS,FOLLOW_MINUS_in_additiveExpr1680); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_additiveExpr1682);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("-"); 
                    	}
                    	PushFollow(FOLLOW_nestedExprAfterMinusDiv_in_additiveExpr1686);
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
    // SqlGenerator.g:307:1: bitwiseExpr : ( ^( BAND expr nestedExpr ) | ^( BOR expr nestedExpr ) | ^( BXOR expr nestedExpr ) | ^( BNOT nestedExpr ) );
    public void bitwiseExpr() // throws RecognitionException [1]
    {   
        try 
    	{
            // SqlGenerator.g:308:2: ( ^( BAND expr nestedExpr ) | ^( BOR expr nestedExpr ) | ^( BXOR expr nestedExpr ) | ^( BNOT nestedExpr ) )
            int alt57 = 4;
            switch ( input.LA(1) ) 
            {
            case BAND:
            	{
                alt57 = 1;
                }
                break;
            case BOR:
            	{
                alt57 = 2;
                }
                break;
            case BXOR:
            	{
                alt57 = 3;
                }
                break;
            case BNOT:
            	{
                alt57 = 4;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d57s0 =
            	        new NoViableAltException("", 57, 0, input);

            	    throw nvae_d57s0;
            }

            switch (alt57) 
            {
                case 1 :
                    // SqlGenerator.g:308:4: ^( BAND expr nestedExpr )
                    {
                    	Match(input,BAND,FOLLOW_BAND_in_bitwiseExpr1699); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_bitwiseExpr1701);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("&"); 
                    	}
                    	PushFollow(FOLLOW_nestedExpr_in_bitwiseExpr1705);
                    	nestedExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // SqlGenerator.g:309:4: ^( BOR expr nestedExpr )
                    {
                    	Match(input,BOR,FOLLOW_BOR_in_bitwiseExpr1712); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_bitwiseExpr1714);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("|"); 
                    	}
                    	PushFollow(FOLLOW_nestedExpr_in_bitwiseExpr1718);
                    	nestedExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 3 :
                    // SqlGenerator.g:310:4: ^( BXOR expr nestedExpr )
                    {
                    	Match(input,BXOR,FOLLOW_BXOR_in_bitwiseExpr1725); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_bitwiseExpr1727);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("^"); 
                    	}
                    	PushFollow(FOLLOW_nestedExpr_in_bitwiseExpr1731);
                    	nestedExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 4 :
                    // SqlGenerator.g:311:4: ^( BNOT nestedExpr )
                    {
                    	Match(input,BNOT,FOLLOW_BNOT_in_bitwiseExpr1738); if (state.failed) return ;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("~"); 
                    	}

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_nestedExpr_in_bitwiseExpr1742);
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
    // SqlGenerator.g:314:1: multiplicativeExpr : ( ^( STAR nestedExpr nestedExpr ) | ^( DIV nestedExpr nestedExprAfterMinusDiv ) );
    public void multiplicativeExpr() // throws RecognitionException [1]
    {   
        try 
    	{
            // SqlGenerator.g:315:2: ( ^( STAR nestedExpr nestedExpr ) | ^( DIV nestedExpr nestedExprAfterMinusDiv ) )
            int alt58 = 2;
            int LA58_0 = input.LA(1);

            if ( (LA58_0 == STAR) )
            {
                alt58 = 1;
            }
            else if ( (LA58_0 == DIV) )
            {
                alt58 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d58s0 =
                    new NoViableAltException("", 58, 0, input);

                throw nvae_d58s0;
            }
            switch (alt58) 
            {
                case 1 :
                    // SqlGenerator.g:315:4: ^( STAR nestedExpr nestedExpr )
                    {
                    	Match(input,STAR,FOLLOW_STAR_in_multiplicativeExpr1756); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_nestedExpr_in_multiplicativeExpr1758);
                    	nestedExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("*"); 
                    	}
                    	PushFollow(FOLLOW_nestedExpr_in_multiplicativeExpr1762);
                    	nestedExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // SqlGenerator.g:316:4: ^( DIV nestedExpr nestedExprAfterMinusDiv )
                    {
                    	Match(input,DIV,FOLLOW_DIV_in_multiplicativeExpr1769); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_nestedExpr_in_multiplicativeExpr1771);
                    	nestedExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("/"); 
                    	}
                    	PushFollow(FOLLOW_nestedExprAfterMinusDiv_in_multiplicativeExpr1775);
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
    // SqlGenerator.g:319:1: nestedExpr : ( ( additiveExpr )=> additiveExpr | ( bitwiseExpr )=> bitwiseExpr | expr );
    public void nestedExpr() // throws RecognitionException [1]
    {   
        try 
    	{
            // SqlGenerator.g:321:2: ( ( additiveExpr )=> additiveExpr | ( bitwiseExpr )=> bitwiseExpr | expr )
            int alt59 = 3;
            alt59 = dfa59.Predict(input);
            switch (alt59) 
            {
                case 1 :
                    // SqlGenerator.g:321:4: ( additiveExpr )=> additiveExpr
                    {
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("("); 
                    	}
                    	PushFollow(FOLLOW_additiveExpr_in_nestedExpr1797);
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
                    // SqlGenerator.g:322:4: ( bitwiseExpr )=> bitwiseExpr
                    {
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("("); 
                    	}
                    	PushFollow(FOLLOW_bitwiseExpr_in_nestedExpr1812);
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
                    // SqlGenerator.g:323:4: expr
                    {
                    	PushFollow(FOLLOW_expr_in_nestedExpr1819);
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
    // SqlGenerator.g:326:1: nestedExprAfterMinusDiv : ( ( arithmeticExpr )=> arithmeticExpr | expr );
    public void nestedExprAfterMinusDiv() // throws RecognitionException [1]
    {   
        try 
    	{
            // SqlGenerator.g:328:2: ( ( arithmeticExpr )=> arithmeticExpr | expr )
            int alt60 = 2;
            alt60 = dfa60.Predict(input);
            switch (alt60) 
            {
                case 1 :
                    // SqlGenerator.g:328:4: ( arithmeticExpr )=> arithmeticExpr
                    {
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("("); 
                    	}
                    	PushFollow(FOLLOW_arithmeticExpr_in_nestedExprAfterMinusDiv1841);
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
                    // SqlGenerator.g:329:4: expr
                    {
                    	PushFollow(FOLLOW_expr_in_nestedExprAfterMinusDiv1848);
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
    // SqlGenerator.g:332:1: caseExpr : ( ^( CASE ( ^( WHEN booleanExpr[false] expr ) )+ ( ^( ELSE expr ) )? ) | ^( CASE2 expr ( ^( WHEN expr expr ) )+ ( ^( ELSE expr ) )? ) );
    public void caseExpr() // throws RecognitionException [1]
    {   
        try 
    	{
            // SqlGenerator.g:333:2: ( ^( CASE ( ^( WHEN booleanExpr[false] expr ) )+ ( ^( ELSE expr ) )? ) | ^( CASE2 expr ( ^( WHEN expr expr ) )+ ( ^( ELSE expr ) )? ) )
            int alt65 = 2;
            int LA65_0 = input.LA(1);

            if ( (LA65_0 == CASE) )
            {
                alt65 = 1;
            }
            else if ( (LA65_0 == CASE2) )
            {
                alt65 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d65s0 =
                    new NoViableAltException("", 65, 0, input);

                throw nvae_d65s0;
            }
            switch (alt65) 
            {
                case 1 :
                    // SqlGenerator.g:333:4: ^( CASE ( ^( WHEN booleanExpr[false] expr ) )+ ( ^( ELSE expr ) )? )
                    {
                    	Match(input,CASE,FOLLOW_CASE_in_caseExpr1860); if (state.failed) return ;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("case"); 
                    	}

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	// SqlGenerator.g:334:3: ( ^( WHEN booleanExpr[false] expr ) )+
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
                    			    // SqlGenerator.g:334:5: ^( WHEN booleanExpr[false] expr )
                    			    {
                    			    	Match(input,WHEN,FOLLOW_WHEN_in_caseExpr1870); if (state.failed) return ;

                    			    	if ( (state.backtracking==0) )
                    			    	{
                    			    	   Out( " when "); 
                    			    	}

                    			    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    			    	PushFollow(FOLLOW_booleanExpr_in_caseExpr1874);
                    			    	booleanExpr(false);
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return ;
                    			    	if ( (state.backtracking==0) )
                    			    	{
                    			    	   Out(" then "); 
                    			    	}
                    			    	PushFollow(FOLLOW_expr_in_caseExpr1879);
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
                    		;	// Stops C# compiler whining that label 'loop61' has no statements

                    	// SqlGenerator.g:335:3: ( ^( ELSE expr ) )?
                    	int alt62 = 2;
                    	int LA62_0 = input.LA(1);

                    	if ( (LA62_0 == ELSE) )
                    	{
                    	    alt62 = 1;
                    	}
                    	switch (alt62) 
                    	{
                    	    case 1 :
                    	        // SqlGenerator.g:335:5: ^( ELSE expr )
                    	        {
                    	        	Match(input,ELSE,FOLLOW_ELSE_in_caseExpr1891); if (state.failed) return ;

                    	        	if ( (state.backtracking==0) )
                    	        	{
                    	        	   Out(" else "); 
                    	        	}

                    	        	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	        	PushFollow(FOLLOW_expr_in_caseExpr1895);
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
                    // SqlGenerator.g:337:4: ^( CASE2 expr ( ^( WHEN expr expr ) )+ ( ^( ELSE expr ) )? )
                    {
                    	Match(input,CASE2,FOLLOW_CASE2_in_caseExpr1911); if (state.failed) return ;

                    	if ( (state.backtracking==0) )
                    	{
                    	   Out("case "); 
                    	}

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_expr_in_caseExpr1915);
                    	expr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	// SqlGenerator.g:338:3: ( ^( WHEN expr expr ) )+
                    	int cnt63 = 0;
                    	do 
                    	{
                    	    int alt63 = 2;
                    	    int LA63_0 = input.LA(1);

                    	    if ( (LA63_0 == WHEN) )
                    	    {
                    	        alt63 = 1;
                    	    }


                    	    switch (alt63) 
                    		{
                    			case 1 :
                    			    // SqlGenerator.g:338:5: ^( WHEN expr expr )
                    			    {
                    			    	Match(input,WHEN,FOLLOW_WHEN_in_caseExpr1922); if (state.failed) return ;

                    			    	if ( (state.backtracking==0) )
                    			    	{
                    			    	   Out( " when "); 
                    			    	}

                    			    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    			    	PushFollow(FOLLOW_expr_in_caseExpr1926);
                    			    	expr();
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return ;
                    			    	if ( (state.backtracking==0) )
                    			    	{
                    			    	   Out(" then "); 
                    			    	}
                    			    	PushFollow(FOLLOW_expr_in_caseExpr1930);
                    			    	expr();
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return ;

                    			    	Match(input, Token.UP, null); if (state.failed) return ;

                    			    }
                    			    break;

                    			default:
                    			    if ( cnt63 >= 1 ) goto loop63;
                    			    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    		            EarlyExitException eee63 =
                    		                new EarlyExitException(63, input);
                    		            throw eee63;
                    	    }
                    	    cnt63++;
                    	} while (true);

                    	loop63:
                    		;	// Stops C# compiler whining that label 'loop63' has no statements

                    	// SqlGenerator.g:339:3: ( ^( ELSE expr ) )?
                    	int alt64 = 2;
                    	int LA64_0 = input.LA(1);

                    	if ( (LA64_0 == ELSE) )
                    	{
                    	    alt64 = 1;
                    	}
                    	switch (alt64) 
                    	{
                    	    case 1 :
                    	        // SqlGenerator.g:339:5: ^( ELSE expr )
                    	        {
                    	        	Match(input,ELSE,FOLLOW_ELSE_in_caseExpr1942); if (state.failed) return ;

                    	        	if ( (state.backtracking==0) )
                    	        	{
                    	        	   Out(" else "); 
                    	        	}

                    	        	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	        	PushFollow(FOLLOW_expr_in_caseExpr1946);
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
    // SqlGenerator.g:343:1: aggregate : ^(a= AGGREGATE expr ) ;
    public void aggregate() // throws RecognitionException [1]
    {   
        IASTNode a = null;

        try 
    	{
            // SqlGenerator.g:344:2: ( ^(a= AGGREGATE expr ) )
            // SqlGenerator.g:344:4: ^(a= AGGREGATE expr )
            {
            	a=(IASTNode)Match(input,AGGREGATE,FOLLOW_AGGREGATE_in_aggregate1970); if (state.failed) return ;

            	if ( (state.backtracking==0) )
            	{
            	   Out(a); Out("("); 
            	}

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_expr_in_aggregate1975);
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
    // SqlGenerator.g:348:1: methodCall : ^(m= METHOD_CALL i= METHOD_NAME ( ^( EXPR_LIST ( arguments )? ) )? ) ;
    public void methodCall() // throws RecognitionException [1]
    {   
        IASTNode m = null;
        IASTNode i = null;

        try 
    	{
            // SqlGenerator.g:349:2: ( ^(m= METHOD_CALL i= METHOD_NAME ( ^( EXPR_LIST ( arguments )? ) )? ) )
            // SqlGenerator.g:349:4: ^(m= METHOD_CALL i= METHOD_NAME ( ^( EXPR_LIST ( arguments )? ) )? )
            {
            	m=(IASTNode)Match(input,METHOD_CALL,FOLLOW_METHOD_CALL_in_methodCall1994); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	i=(IASTNode)Match(input,METHOD_NAME,FOLLOW_METHOD_NAME_in_methodCall1998); if (state.failed) return ;
            	if ( (state.backtracking==0) )
            	{
            	   BeginFunctionTemplate(m,i); 
            	}
            	// SqlGenerator.g:350:3: ( ^( EXPR_LIST ( arguments )? ) )?
            	int alt67 = 2;
            	int LA67_0 = input.LA(1);

            	if ( (LA67_0 == EXPR_LIST) )
            	{
            	    alt67 = 1;
            	}
            	switch (alt67) 
            	{
            	    case 1 :
            	        // SqlGenerator.g:350:5: ^( EXPR_LIST ( arguments )? )
            	        {
            	        	Match(input,EXPR_LIST,FOLLOW_EXPR_LIST_in_methodCall2007); if (state.failed) return ;

            	        	if ( input.LA(1) == Token.DOWN )
            	        	{
            	        	    Match(input, Token.DOWN, null); if (state.failed) return ;
            	        	    // SqlGenerator.g:350:17: ( arguments )?
            	        	    int alt66 = 2;
            	        	    int LA66_0 = input.LA(1);

            	        	    if ( ((LA66_0 >= ALL && LA66_0 <= ANY) || LA66_0 == BETWEEN || LA66_0 == COUNT || LA66_0 == DOT || (LA66_0 >= EXISTS && LA66_0 <= FALSE) || LA66_0 == IN || LA66_0 == LIKE || LA66_0 == NULL || LA66_0 == SELECT || LA66_0 == SOME || (LA66_0 >= TRUE && LA66_0 <= UNION) || LA66_0 == CASE || LA66_0 == AGGREGATE || LA66_0 == CASE2 || (LA66_0 >= INDEX_OP && LA66_0 <= NOT_LIKE) || LA66_0 == UNARY_MINUS || LA66_0 == VECTOR_EXPR || (LA66_0 >= CONSTANT && LA66_0 <= JAVA_CONSTANT) || LA66_0 == EQ || (LA66_0 >= PARAM && LA66_0 <= NE) || (LA66_0 >= LT && LA66_0 <= GE) || (LA66_0 >= BNOT && LA66_0 <= DIV) || (LA66_0 >= QUOTED_String && LA66_0 <= IDENT) || LA66_0 == ALIAS_REF || LA66_0 == SQL_TOKEN || LA66_0 == NAMED_PARAM) )
            	        	    {
            	        	        alt66 = 1;
            	        	    }
            	        	    switch (alt66) 
            	        	    {
            	        	        case 1 :
            	        	            // SqlGenerator.g:350:18: arguments
            	        	            {
            	        	            	PushFollow(FOLLOW_arguments_in_methodCall2010);
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
    // SqlGenerator.g:354:1: arguments : ( expr | comparisonExpr[true] ) ( ( expr | comparisonExpr[true] ) )* ;
    public void arguments() // throws RecognitionException [1]
    {   
        try 
    	{
            // SqlGenerator.g:355:2: ( ( expr | comparisonExpr[true] ) ( ( expr | comparisonExpr[true] ) )* )
            // SqlGenerator.g:355:4: ( expr | comparisonExpr[true] ) ( ( expr | comparisonExpr[true] ) )*
            {
            	// SqlGenerator.g:355:4: ( expr | comparisonExpr[true] )
            	int alt68 = 2;
            	int LA68_0 = input.LA(1);

            	if ( ((LA68_0 >= ALL && LA68_0 <= ANY) || LA68_0 == COUNT || LA68_0 == DOT || LA68_0 == FALSE || LA68_0 == NULL || LA68_0 == SELECT || LA68_0 == SOME || (LA68_0 >= TRUE && LA68_0 <= UNION) || LA68_0 == CASE || LA68_0 == AGGREGATE || LA68_0 == CASE2 || LA68_0 == INDEX_OP || LA68_0 == METHOD_CALL || LA68_0 == UNARY_MINUS || LA68_0 == VECTOR_EXPR || (LA68_0 >= CONSTANT && LA68_0 <= JAVA_CONSTANT) || LA68_0 == PARAM || (LA68_0 >= BNOT && LA68_0 <= DIV) || (LA68_0 >= QUOTED_String && LA68_0 <= IDENT) || LA68_0 == ALIAS_REF || LA68_0 == SQL_TOKEN || LA68_0 == NAMED_PARAM) )
            	{
            	    alt68 = 1;
            	}
            	else if ( (LA68_0 == BETWEEN || LA68_0 == EXISTS || LA68_0 == IN || LA68_0 == LIKE || (LA68_0 >= IS_NOT_NULL && LA68_0 <= IS_NULL) || (LA68_0 >= NOT_BETWEEN && LA68_0 <= NOT_LIKE) || LA68_0 == EQ || LA68_0 == NE || (LA68_0 >= LT && LA68_0 <= GE)) )
            	{
            	    alt68 = 2;
            	}
            	else 
            	{
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d68s0 =
            	        new NoViableAltException("", 68, 0, input);

            	    throw nvae_d68s0;
            	}
            	switch (alt68) 
            	{
            	    case 1 :
            	        // SqlGenerator.g:355:5: expr
            	        {
            	        	PushFollow(FOLLOW_expr_in_arguments2035);
            	        	expr();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;
            	    case 2 :
            	        // SqlGenerator.g:355:12: comparisonExpr[true]
            	        {
            	        	PushFollow(FOLLOW_comparisonExpr_in_arguments2039);
            	        	comparisonExpr(true);
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}

            	// SqlGenerator.g:355:34: ( ( expr | comparisonExpr[true] ) )*
            	do 
            	{
            	    int alt70 = 2;
            	    int LA70_0 = input.LA(1);

            	    if ( ((LA70_0 >= ALL && LA70_0 <= ANY) || LA70_0 == BETWEEN || LA70_0 == COUNT || LA70_0 == DOT || (LA70_0 >= EXISTS && LA70_0 <= FALSE) || LA70_0 == IN || LA70_0 == LIKE || LA70_0 == NULL || LA70_0 == SELECT || LA70_0 == SOME || (LA70_0 >= TRUE && LA70_0 <= UNION) || LA70_0 == CASE || LA70_0 == AGGREGATE || LA70_0 == CASE2 || (LA70_0 >= INDEX_OP && LA70_0 <= NOT_LIKE) || LA70_0 == UNARY_MINUS || LA70_0 == VECTOR_EXPR || (LA70_0 >= CONSTANT && LA70_0 <= JAVA_CONSTANT) || LA70_0 == EQ || (LA70_0 >= PARAM && LA70_0 <= NE) || (LA70_0 >= LT && LA70_0 <= GE) || (LA70_0 >= BNOT && LA70_0 <= DIV) || (LA70_0 >= QUOTED_String && LA70_0 <= IDENT) || LA70_0 == ALIAS_REF || LA70_0 == SQL_TOKEN || LA70_0 == NAMED_PARAM) )
            	    {
            	        alt70 = 1;
            	    }


            	    switch (alt70) 
            		{
            			case 1 :
            			    // SqlGenerator.g:355:36: ( expr | comparisonExpr[true] )
            			    {
            			    	if ( (state.backtracking==0) )
            			    	{
            			    	   CommaBetweenParameters(", "); 
            			    	}
            			    	// SqlGenerator.g:355:70: ( expr | comparisonExpr[true] )
            			    	int alt69 = 2;
            			    	int LA69_0 = input.LA(1);

            			    	if ( ((LA69_0 >= ALL && LA69_0 <= ANY) || LA69_0 == COUNT || LA69_0 == DOT || LA69_0 == FALSE || LA69_0 == NULL || LA69_0 == SELECT || LA69_0 == SOME || (LA69_0 >= TRUE && LA69_0 <= UNION) || LA69_0 == CASE || LA69_0 == AGGREGATE || LA69_0 == CASE2 || LA69_0 == INDEX_OP || LA69_0 == METHOD_CALL || LA69_0 == UNARY_MINUS || LA69_0 == VECTOR_EXPR || (LA69_0 >= CONSTANT && LA69_0 <= JAVA_CONSTANT) || LA69_0 == PARAM || (LA69_0 >= BNOT && LA69_0 <= DIV) || (LA69_0 >= QUOTED_String && LA69_0 <= IDENT) || LA69_0 == ALIAS_REF || LA69_0 == SQL_TOKEN || LA69_0 == NAMED_PARAM) )
            			    	{
            			    	    alt69 = 1;
            			    	}
            			    	else if ( (LA69_0 == BETWEEN || LA69_0 == EXISTS || LA69_0 == IN || LA69_0 == LIKE || (LA69_0 >= IS_NOT_NULL && LA69_0 <= IS_NULL) || (LA69_0 >= NOT_BETWEEN && LA69_0 <= NOT_LIKE) || LA69_0 == EQ || LA69_0 == NE || (LA69_0 >= LT && LA69_0 <= GE)) )
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
            			    	        // SqlGenerator.g:355:71: expr
            			    	        {
            			    	        	PushFollow(FOLLOW_expr_in_arguments2048);
            			    	        	expr();
            			    	        	state.followingStackPointer--;
            			    	        	if (state.failed) return ;

            			    	        }
            			    	        break;
            			    	    case 2 :
            			    	        // SqlGenerator.g:355:78: comparisonExpr[true]
            			    	        {
            			    	        	PushFollow(FOLLOW_comparisonExpr_in_arguments2052);
            			    	        	comparisonExpr(true);
            			    	        	state.followingStackPointer--;
            			    	        	if (state.failed) return ;

            			    	        }
            			    	        break;

            			    	}


            			    }
            			    break;

            			default:
            			    goto loop70;
            	    }
            	} while (true);

            	loop70:
            		;	// Stops C# compiler whining that label 'loop70' has no statements


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
    // SqlGenerator.g:358:1: parameter : (n= NAMED_PARAM | p= PARAM );
    public void parameter() // throws RecognitionException [1]
    {   
        IASTNode n = null;
        IASTNode p = null;

        try 
    	{
            // SqlGenerator.g:359:2: (n= NAMED_PARAM | p= PARAM )
            int alt71 = 2;
            int LA71_0 = input.LA(1);

            if ( (LA71_0 == NAMED_PARAM) )
            {
                alt71 = 1;
            }
            else if ( (LA71_0 == PARAM) )
            {
                alt71 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d71s0 =
                    new NoViableAltException("", 71, 0, input);

                throw nvae_d71s0;
            }
            switch (alt71) 
            {
                case 1 :
                    // SqlGenerator.g:359:4: n= NAMED_PARAM
                    {
                    	n=(IASTNode)Match(input,NAMED_PARAM,FOLLOW_NAMED_PARAM_in_parameter2070); if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(n); 
                    	}

                    }
                    break;
                case 2 :
                    // SqlGenerator.g:360:4: p= PARAM
                    {
                    	p=(IASTNode)Match(input,PARAM,FOLLOW_PARAM_in_parameter2079); if (state.failed) return ;
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

    public class limitValue_return : TreeRuleReturnScope
    {
    };

    // $ANTLR start "limitValue"
    // SqlGenerator.g:363:1: limitValue : ( NUM_INT | NAMED_PARAM | PARAM );
    public SqlGenerator.limitValue_return limitValue() // throws RecognitionException [1]
    {   
        SqlGenerator.limitValue_return retval = new SqlGenerator.limitValue_return();
        retval.Start = input.LT(1);

        try 
    	{
            // SqlGenerator.g:364:2: ( NUM_INT | NAMED_PARAM | PARAM )
            // SqlGenerator.g:
            {
            	if ( input.LA(1) == NUM_INT || input.LA(1) == PARAM || input.LA(1) == NAMED_PARAM ) 
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
    // $ANTLR end "limitValue"


    // $ANTLR start "addrExpr"
    // SqlGenerator.g:369:1: addrExpr : ( ^(r= DOT . . ) | i= ALIAS_REF | ^(j= INDEX_OP ( . )* ) );
    public void addrExpr() // throws RecognitionException [1]
    {   
        IASTNode r = null;
        IASTNode i = null;
        IASTNode j = null;

        try 
    	{
            // SqlGenerator.g:370:2: ( ^(r= DOT . . ) | i= ALIAS_REF | ^(j= INDEX_OP ( . )* ) )
            int alt73 = 3;
            switch ( input.LA(1) ) 
            {
            case DOT:
            	{
                alt73 = 1;
                }
                break;
            case ALIAS_REF:
            	{
                alt73 = 2;
                }
                break;
            case INDEX_OP:
            	{
                alt73 = 3;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d73s0 =
            	        new NoViableAltException("", 73, 0, input);

            	    throw nvae_d73s0;
            }

            switch (alt73) 
            {
                case 1 :
                    // SqlGenerator.g:370:4: ^(r= DOT . . )
                    {
                    	r=(IASTNode)Match(input,DOT,FOLLOW_DOT_in_addrExpr2116); if (state.failed) return ;

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
                    // SqlGenerator.g:371:4: i= ALIAS_REF
                    {
                    	i=(IASTNode)Match(input,ALIAS_REF,FOLLOW_ALIAS_REF_in_addrExpr2130); if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	   Out(i); 
                    	}

                    }
                    break;
                case 3 :
                    // SqlGenerator.g:372:4: ^(j= INDEX_OP ( . )* )
                    {
                    	j=(IASTNode)Match(input,INDEX_OP,FOLLOW_INDEX_OP_in_addrExpr2140); if (state.failed) return ;

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return ;
                    	    // SqlGenerator.g:372:17: ( . )*
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
                    	    		    // SqlGenerator.g:372:17: .
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
    // SqlGenerator.g:375:1: sqlToken : ^(t= SQL_TOKEN ( . )* ) ;
    public void sqlToken() // throws RecognitionException [1]
    {   
        IASTNode t = null;

        try 
    	{
            // SqlGenerator.g:376:2: ( ^(t= SQL_TOKEN ( . )* ) )
            // SqlGenerator.g:376:4: ^(t= SQL_TOKEN ( . )* )
            {
            	t=(IASTNode)Match(input,SQL_TOKEN,FOLLOW_SQL_TOKEN_in_sqlToken2160); if (state.failed) return ;

            	if ( (state.backtracking==0) )
            	{
            	   Out(t); 
            	}

            	if ( input.LA(1) == Token.DOWN )
            	{
            	    Match(input, Token.DOWN, null); if (state.failed) return ;
            	    // SqlGenerator.g:376:30: ( . )*
            	    do 
            	    {
            	        int alt74 = 2;
            	        int LA74_0 = input.LA(1);

            	        if ( ((LA74_0 >= ALL && LA74_0 <= BOGUS)) )
            	        {
            	            alt74 = 1;
            	        }
            	        else if ( (LA74_0 == UP) )
            	        {
            	            alt74 = 2;
            	        }


            	        switch (alt74) 
            	    	{
            	    		case 1 :
            	    		    // SqlGenerator.g:376:30: .
            	    		    {
            	    		    	MatchAny(input); if (state.failed) return ;

            	    		    }
            	    		    break;

            	    		default:
            	    		    goto loop74;
            	        }
            	    } while (true);

            	    loop74:
            	    	;	// Stops C# compiler whining that label 'loop74' has no statements


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
        // SqlGenerator.g:85:4: ( SQL_TOKEN )
        // SqlGenerator.g:85:5: SQL_TOKEN
        {
        	Match(input,SQL_TOKEN,FOLLOW_SQL_TOKEN_in_synpred1_SqlGenerator366); if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred1_SqlGenerator"

    // $ANTLR start "synpred2_SqlGenerator"
    public void synpred2_SqlGenerator_fragment() {
        // SqlGenerator.g:321:4: ( additiveExpr )
        // SqlGenerator.g:321:5: additiveExpr
        {
        	PushFollow(FOLLOW_additiveExpr_in_synpred2_SqlGenerator1790);
        	additiveExpr();
        	state.followingStackPointer--;
        	if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred2_SqlGenerator"

    // $ANTLR start "synpred3_SqlGenerator"
    public void synpred3_SqlGenerator_fragment() {
        // SqlGenerator.g:322:4: ( bitwiseExpr )
        // SqlGenerator.g:322:5: bitwiseExpr
        {
        	PushFollow(FOLLOW_bitwiseExpr_in_synpred3_SqlGenerator1805);
        	bitwiseExpr();
        	state.followingStackPointer--;
        	if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred3_SqlGenerator"

    // $ANTLR start "synpred4_SqlGenerator"
    public void synpred4_SqlGenerator_fragment() {
        // SqlGenerator.g:328:4: ( arithmeticExpr )
        // SqlGenerator.g:328:5: arithmeticExpr
        {
        	PushFollow(FOLLOW_arithmeticExpr_in_synpred4_SqlGenerator1834);
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


   	protected DFA59 dfa59;
   	protected DFA60 dfa60;
	private void InitializeCyclicDFAs()
	{
    	this.dfa59 = new DFA59(this);
    	this.dfa60 = new DFA60(this);
	    this.dfa59.specialStateTransitionHandler = new DFA.SpecialStateTransitionHandler(DFA59_SpecialStateTransition);
	    this.dfa60.specialStateTransitionHandler = new DFA.SpecialStateTransitionHandler(DFA60_SpecialStateTransition);
	}

    const string DFA59_eotS =
        "\x1f\uffff";
    const string DFA59_eofS =
        "\x1f\uffff";
    const string DFA59_minS =
        "\x01\x04\x06\x00\x18\uffff";
    const string DFA59_maxS =
        "\x01\u0095\x06\x00\x18\uffff";
    const string DFA59_acceptS =
        "\x07\uffff\x01\x03\x15\uffff\x01\x01\x01\x02";
    const string DFA59_specialS =
        "\x01\uffff\x01\x00\x01\x01\x01\x02\x01\x03\x01\x04\x01\x05\x18"+
        "\uffff}>";
    static readonly string[] DFA59_transitionS = {
            "\x02\x07\x06\uffff\x01\x07\x02\uffff\x01\x07\x04\uffff\x01"+
            "\x07\x12\uffff\x01\x07\x05\uffff\x01\x07\x02\uffff\x01\x07\x02"+
            "\uffff\x02\x07\x04\uffff\x01\x07\x0d\uffff\x01\x07\x02\uffff"+
            "\x01\x07\x03\uffff\x01\x07\x02\uffff\x01\x07\x08\uffff\x01\x07"+
            "\x01\uffff\x01\x07\x01\uffff\x07\x07\x05\uffff\x01\x07\x07\uffff"+
            "\x01\x06\x01\x04\x01\x05\x01\x03\x01\x01\x01\x02\x02\x07\x02"+
            "\uffff\x02\x07\x0f\uffff\x01\x07\x01\uffff\x01\x07\x05\uffff"+
            "\x01\x07",
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

    static readonly short[] DFA59_eot = DFA.UnpackEncodedString(DFA59_eotS);
    static readonly short[] DFA59_eof = DFA.UnpackEncodedString(DFA59_eofS);
    static readonly char[] DFA59_min = DFA.UnpackEncodedStringToUnsignedChars(DFA59_minS);
    static readonly char[] DFA59_max = DFA.UnpackEncodedStringToUnsignedChars(DFA59_maxS);
    static readonly short[] DFA59_accept = DFA.UnpackEncodedString(DFA59_acceptS);
    static readonly short[] DFA59_special = DFA.UnpackEncodedString(DFA59_specialS);
    static readonly short[][] DFA59_transition = DFA.UnpackEncodedStringArray(DFA59_transitionS);

    protected class DFA59 : DFA
    {
        public DFA59(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 59;
            this.eot = DFA59_eot;
            this.eof = DFA59_eof;
            this.min = DFA59_min;
            this.max = DFA59_max;
            this.accept = DFA59_accept;
            this.special = DFA59_special;
            this.transition = DFA59_transition;

        }

        override public string Description
        {
            get { return "319:1: nestedExpr : ( ( additiveExpr )=> additiveExpr | ( bitwiseExpr )=> bitwiseExpr | expr );"; }
        }

    }


    protected internal int DFA59_SpecialStateTransition(DFA dfa, int s, IIntStream _input) //throws NoViableAltException
    {
            ITreeNodeStream input = (ITreeNodeStream)_input;
    	int _s = s;
        switch ( s )
        {
               	case 0 : 
                   	int LA59_1 = input.LA(1);

                   	 
                   	int index59_1 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred2_SqlGenerator()) ) { s = 29; }

                   	else if ( (true) ) { s = 7; }

                   	 
                   	input.Seek(index59_1);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 1 : 
                   	int LA59_2 = input.LA(1);

                   	 
                   	int index59_2 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred2_SqlGenerator()) ) { s = 29; }

                   	else if ( (true) ) { s = 7; }

                   	 
                   	input.Seek(index59_2);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 2 : 
                   	int LA59_3 = input.LA(1);

                   	 
                   	int index59_3 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred3_SqlGenerator()) ) { s = 30; }

                   	else if ( (true) ) { s = 7; }

                   	 
                   	input.Seek(index59_3);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 3 : 
                   	int LA59_4 = input.LA(1);

                   	 
                   	int index59_4 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred3_SqlGenerator()) ) { s = 30; }

                   	else if ( (true) ) { s = 7; }

                   	 
                   	input.Seek(index59_4);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 4 : 
                   	int LA59_5 = input.LA(1);

                   	 
                   	int index59_5 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred3_SqlGenerator()) ) { s = 30; }

                   	else if ( (true) ) { s = 7; }

                   	 
                   	input.Seek(index59_5);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 5 : 
                   	int LA59_6 = input.LA(1);

                   	 
                   	int index59_6 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred3_SqlGenerator()) ) { s = 30; }

                   	else if ( (true) ) { s = 7; }

                   	 
                   	input.Seek(index59_6);
                   	if ( s >= 0 ) return s;
                   	break;
        }
        if (state.backtracking > 0) {state.failed = true; return -1;}
        NoViableAltException nvae59 =
            new NoViableAltException(dfa.Description, 59, _s, input);
        dfa.Error(nvae59);
        throw nvae59;
    }
    const string DFA60_eotS =
        "\x1e\uffff";
    const string DFA60_eofS =
        "\x1e\uffff";
    const string DFA60_minS =
        "\x01\x04\x0b\x00\x12\uffff";
    const string DFA60_maxS =
        "\x01\u0095\x0b\x00\x12\uffff";
    const string DFA60_acceptS =
        "\x0c\uffff\x01\x02\x10\uffff\x01\x01";
    const string DFA60_specialS =
        "\x01\uffff\x01\x00\x01\x01\x01\x02\x01\x03\x01\x04\x01\x05\x01"+
        "\x06\x01\x07\x01\x08\x01\x09\x01\x0a\x12\uffff}>";
    static readonly string[] DFA60_transitionS = {
            "\x02\x0c\x06\uffff\x01\x0c\x02\uffff\x01\x0c\x04\uffff\x01"+
            "\x0c\x12\uffff\x01\x0c\x05\uffff\x01\x0c\x02\uffff\x01\x0c\x02"+
            "\uffff\x02\x0c\x04\uffff\x01\x0a\x0d\uffff\x01\x0c\x02\uffff"+
            "\x01\x0b\x03\uffff\x01\x0c\x02\uffff\x01\x0c\x08\uffff\x01\x09"+
            "\x01\uffff\x01\x0c\x01\uffff\x07\x0c\x05\uffff\x01\x0c\x07\uffff"+
            "\x01\x06\x01\x04\x01\x05\x01\x03\x01\x01\x01\x02\x01\x07\x01"+
            "\x08\x02\uffff\x02\x0c\x0f\uffff\x01\x0c\x01\uffff\x01\x0c\x05"+
            "\uffff\x01\x0c",
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

    static readonly short[] DFA60_eot = DFA.UnpackEncodedString(DFA60_eotS);
    static readonly short[] DFA60_eof = DFA.UnpackEncodedString(DFA60_eofS);
    static readonly char[] DFA60_min = DFA.UnpackEncodedStringToUnsignedChars(DFA60_minS);
    static readonly char[] DFA60_max = DFA.UnpackEncodedStringToUnsignedChars(DFA60_maxS);
    static readonly short[] DFA60_accept = DFA.UnpackEncodedString(DFA60_acceptS);
    static readonly short[] DFA60_special = DFA.UnpackEncodedString(DFA60_specialS);
    static readonly short[][] DFA60_transition = DFA.UnpackEncodedStringArray(DFA60_transitionS);

    protected class DFA60 : DFA
    {
        public DFA60(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 60;
            this.eot = DFA60_eot;
            this.eof = DFA60_eof;
            this.min = DFA60_min;
            this.max = DFA60_max;
            this.accept = DFA60_accept;
            this.special = DFA60_special;
            this.transition = DFA60_transition;

        }

        override public string Description
        {
            get { return "326:1: nestedExprAfterMinusDiv : ( ( arithmeticExpr )=> arithmeticExpr | expr );"; }
        }

    }


    protected internal int DFA60_SpecialStateTransition(DFA dfa, int s, IIntStream _input) //throws NoViableAltException
    {
            ITreeNodeStream input = (ITreeNodeStream)_input;
    	int _s = s;
        switch ( s )
        {
               	case 0 : 
                   	int LA60_1 = input.LA(1);

                   	 
                   	int index60_1 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred4_SqlGenerator()) ) { s = 29; }

                   	else if ( (true) ) { s = 12; }

                   	 
                   	input.Seek(index60_1);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 1 : 
                   	int LA60_2 = input.LA(1);

                   	 
                   	int index60_2 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred4_SqlGenerator()) ) { s = 29; }

                   	else if ( (true) ) { s = 12; }

                   	 
                   	input.Seek(index60_2);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 2 : 
                   	int LA60_3 = input.LA(1);

                   	 
                   	int index60_3 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred4_SqlGenerator()) ) { s = 29; }

                   	else if ( (true) ) { s = 12; }

                   	 
                   	input.Seek(index60_3);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 3 : 
                   	int LA60_4 = input.LA(1);

                   	 
                   	int index60_4 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred4_SqlGenerator()) ) { s = 29; }

                   	else if ( (true) ) { s = 12; }

                   	 
                   	input.Seek(index60_4);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 4 : 
                   	int LA60_5 = input.LA(1);

                   	 
                   	int index60_5 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred4_SqlGenerator()) ) { s = 29; }

                   	else if ( (true) ) { s = 12; }

                   	 
                   	input.Seek(index60_5);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 5 : 
                   	int LA60_6 = input.LA(1);

                   	 
                   	int index60_6 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred4_SqlGenerator()) ) { s = 29; }

                   	else if ( (true) ) { s = 12; }

                   	 
                   	input.Seek(index60_6);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 6 : 
                   	int LA60_7 = input.LA(1);

                   	 
                   	int index60_7 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred4_SqlGenerator()) ) { s = 29; }

                   	else if ( (true) ) { s = 12; }

                   	 
                   	input.Seek(index60_7);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 7 : 
                   	int LA60_8 = input.LA(1);

                   	 
                   	int index60_8 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred4_SqlGenerator()) ) { s = 29; }

                   	else if ( (true) ) { s = 12; }

                   	 
                   	input.Seek(index60_8);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 8 : 
                   	int LA60_9 = input.LA(1);

                   	 
                   	int index60_9 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred4_SqlGenerator()) ) { s = 29; }

                   	else if ( (true) ) { s = 12; }

                   	 
                   	input.Seek(index60_9);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 9 : 
                   	int LA60_10 = input.LA(1);

                   	 
                   	int index60_10 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred4_SqlGenerator()) ) { s = 29; }

                   	else if ( (true) ) { s = 12; }

                   	 
                   	input.Seek(index60_10);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 10 : 
                   	int LA60_11 = input.LA(1);

                   	 
                   	int index60_11 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred4_SqlGenerator()) ) { s = 29; }

                   	else if ( (true) ) { s = 12; }

                   	 
                   	input.Seek(index60_11);
                   	if ( s >= 0 ) return s;
                   	break;
        }
        if (state.backtracking > 0) {state.failed = true; return -1;}
        NoViableAltException nvae60 =
            new NoViableAltException(dfa.Description, 60, _s, input);
        dfa.Error(nvae60);
        throw nvae60;
    }
 

    public static readonly BitSet FOLLOW_selectStatement_in_statement57 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_updateStatement_in_statement62 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_deleteStatement_in_statement67 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_insertStatement_in_statement72 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_SELECT_in_selectStatement84 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_selectClause_in_selectStatement90 = new BitSet(new ulong[]{0x0000000000400000UL});
    public static readonly BitSet FOLLOW_from_in_selectStatement94 = new BitSet(new ulong[]{0x0084820003000008UL});
    public static readonly BitSet FOLLOW_WHERE_in_selectStatement101 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_whereExpr_in_selectStatement105 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_GROUP_in_selectStatement117 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_groupExprs_in_selectStatement121 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_HAVING_in_selectStatement133 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_booleanExpr_in_selectStatement137 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_ORDER_in_selectStatement149 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_orderExprs_in_selectStatement153 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_SKIP_in_selectStatement165 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_limitValue_in_selectStatement169 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_TAKE_in_selectStatement181 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_limitValue_in_selectStatement185 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_UPDATE_in_updateStatement212 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_FROM_in_updateStatement220 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_fromTable_in_updateStatement222 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_setClause_in_updateStatement228 = new BitSet(new ulong[]{0x0080000000000008UL});
    public static readonly BitSet FOLLOW_whereClause_in_updateStatement233 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_DELETE_in_deleteStatement252 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_from_in_deleteStatement258 = new BitSet(new ulong[]{0x0080000000000008UL});
    public static readonly BitSet FOLLOW_whereClause_in_deleteStatement263 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_INSERT_in_insertStatement280 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_INTO_in_insertStatement289 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_selectStatement_in_insertStatement299 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_SET_in_setClause319 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_comparisonExpr_in_setClause323 = new BitSet(new ulong[]{0x0000000404080408UL,0x0001E840001D8000UL});
    public static readonly BitSet FOLLOW_comparisonExpr_in_setClause330 = new BitSet(new ulong[]{0x0000000404080408UL,0x0001E840001D8000UL});
    public static readonly BitSet FOLLOW_WHERE_in_whereClause348 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_whereClauseExpr_in_whereClause352 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_conditionList_in_whereClauseExpr371 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_booleanExpr_in_whereClauseExpr376 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_expr_in_orderExprs392 = new BitSet(new ulong[]{0x021920800010D132UL,0x33FC041FD4024480UL,0x000000000020A000UL});
    public static readonly BitSet FOLLOW_orderDirection_in_orderExprs399 = new BitSet(new ulong[]{0x0219208000109032UL,0x33FC041FD4024480UL,0x000000000020A000UL});
    public static readonly BitSet FOLLOW_orderExprs_in_orderExprs409 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_expr_in_groupExprs424 = new BitSet(new ulong[]{0x0219208000109032UL,0x33FC041FD4024480UL,0x000000000020A000UL});
    public static readonly BitSet FOLLOW_groupExprs_in_groupExprs430 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_set_in_orderDirection0 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_filters_in_whereExpr465 = new BitSet(new ulong[]{0x0000014404080442UL,0x0001E840001F8000UL,0x0000000000048000UL});
    public static readonly BitSet FOLLOW_thetaJoins_in_whereExpr473 = new BitSet(new ulong[]{0x0000014404080442UL,0x0001E840001F8000UL,0x0000000000008000UL});
    public static readonly BitSet FOLLOW_booleanExpr_in_whereExpr484 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_thetaJoins_in_whereExpr494 = new BitSet(new ulong[]{0x0000014404080442UL,0x0001E840001F8000UL,0x0000000000008000UL});
    public static readonly BitSet FOLLOW_booleanExpr_in_whereExpr502 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_booleanExpr_in_whereExpr513 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_FILTERS_in_filters526 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_conditionList_in_filters528 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_THETA_JOINS_in_thetaJoins542 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_conditionList_in_thetaJoins544 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_sqlToken_in_conditionList557 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000000000000UL,0x0000000000008000UL});
    public static readonly BitSet FOLLOW_conditionList_in_conditionList563 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_SELECT_CLAUSE_in_selectClause578 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_distinctOrAll_in_selectClause581 = new BitSet(new ulong[]{0x0208208000109000UL,0x33FC041FC4024680UL,0x000000000022A000UL});
    public static readonly BitSet FOLLOW_selectColumn_in_selectClause587 = new BitSet(new ulong[]{0x0208208000109008UL,0x33FC041FC4024680UL,0x000000000022A000UL});
    public static readonly BitSet FOLLOW_selectExpr_in_selectColumn605 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000000000000UL,0x0000000000010000UL});
    public static readonly BitSet FOLLOW_SELECT_COLUMNS_in_selectColumn610 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_selectAtom_in_selectExpr630 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_count_in_selectExpr637 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_CONSTRUCTOR_in_selectExpr643 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_set_in_selectExpr645 = new BitSet(new ulong[]{0x0208208000109000UL,0x33FC041FC4024680UL,0x000000000022A000UL});
    public static readonly BitSet FOLLOW_selectColumn_in_selectExpr655 = new BitSet(new ulong[]{0x0208208000109008UL,0x33FC041FC4024680UL,0x000000000022A000UL});
    public static readonly BitSet FOLLOW_methodCall_in_selectExpr665 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_aggregate_in_selectExpr670 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_constant_in_selectExpr677 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_arithmeticExpr_in_selectExpr684 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_parameter_in_selectExpr689 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_selectStatement_in_selectExpr698 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_COUNT_in_count712 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_distinctOrAll_in_count719 = new BitSet(new ulong[]{0x0208008000109000UL,0x33FC041FC5024480UL,0x000000000020A000UL});
    public static readonly BitSet FOLLOW_countExpr_in_count725 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_DISTINCT_in_distinctOrAll740 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ALL_in_distinctOrAll748 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_ROW_STAR_in_countExpr767 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_simpleExpr_in_countExpr774 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_DOT_in_selectAtom786 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_SQL_TOKEN_in_selectAtom796 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_ALIAS_REF_in_selectAtom806 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_SELECT_EXPR_in_selectAtom816 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_FROM_in_from839 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_fromTable_in_from846 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000000000000UL,0x0000000000000280UL});
    public static readonly BitSet FOLLOW_FROM_FRAGMENT_in_fromTable872 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_tableJoin_in_fromTable878 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000000000000UL,0x0000000000000280UL});
    public static readonly BitSet FOLLOW_JOIN_FRAGMENT_in_fromTable893 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_tableJoin_in_fromTable899 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000000000000UL,0x0000000000000280UL});
    public static readonly BitSet FOLLOW_JOIN_FRAGMENT_in_tableJoin922 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_tableJoin_in_tableJoin927 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000000000000UL,0x0000000000000280UL});
    public static readonly BitSet FOLLOW_FROM_FRAGMENT_in_tableJoin943 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_tableJoin_in_tableJoin948 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000000000000UL,0x0000000000000280UL});
    public static readonly BitSet FOLLOW_AND_in_booleanOp968 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_booleanExpr_in_booleanOp970 = new BitSet(new ulong[]{0x0000014404080440UL,0x0001E840001F8000UL,0x0000000000008000UL});
    public static readonly BitSet FOLLOW_booleanExpr_in_booleanOp975 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_OR_in_booleanOp983 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_booleanExpr_in_booleanOp987 = new BitSet(new ulong[]{0x0000014404080440UL,0x0001E840001F8000UL,0x0000000000008000UL});
    public static readonly BitSet FOLLOW_booleanExpr_in_booleanOp992 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NOT_in_booleanOp1002 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_booleanExpr_in_booleanOp1006 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_booleanOp_in_booleanExpr1023 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_comparisonExpr_in_booleanExpr1030 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_methodCall_in_booleanExpr1037 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_SQL_TOKEN_in_booleanExpr1044 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_binaryComparisonExpression_in_comparisonExpr1060 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_exoticComparisonExpression_in_comparisonExpr1067 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_EQ_in_binaryComparisonExpression1082 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1084 = new BitSet(new ulong[]{0x0219208000109030UL,0x33FC041FD4024480UL,0x000000000020A000UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1088 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NE_in_binaryComparisonExpression1095 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1097 = new BitSet(new ulong[]{0x0219208000109030UL,0x33FC041FD4024480UL,0x000000000020A000UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1101 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_GT_in_binaryComparisonExpression1108 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1110 = new BitSet(new ulong[]{0x0219208000109030UL,0x33FC041FD4024480UL,0x000000000020A000UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1114 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_GE_in_binaryComparisonExpression1121 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1123 = new BitSet(new ulong[]{0x0219208000109030UL,0x33FC041FD4024480UL,0x000000000020A000UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1127 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_LT_in_binaryComparisonExpression1134 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1136 = new BitSet(new ulong[]{0x0219208000109030UL,0x33FC041FD4024480UL,0x000000000020A000UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1140 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_LE_in_binaryComparisonExpression1147 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1149 = new BitSet(new ulong[]{0x0219208000109030UL,0x33FC041FD4024480UL,0x000000000020A000UL});
    public static readonly BitSet FOLLOW_expr_in_binaryComparisonExpression1153 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_LIKE_in_exoticComparisonExpression1167 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1169 = new BitSet(new ulong[]{0x0219208000109030UL,0x33FC041FD4024480UL,0x000000000020A000UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1173 = new BitSet(new ulong[]{0x0000000000040008UL});
    public static readonly BitSet FOLLOW_likeEscape_in_exoticComparisonExpression1175 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NOT_LIKE_in_exoticComparisonExpression1183 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1185 = new BitSet(new ulong[]{0x0219208000109030UL,0x33FC041FD4024480UL,0x000000000020A000UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1189 = new BitSet(new ulong[]{0x0000000000040008UL});
    public static readonly BitSet FOLLOW_likeEscape_in_exoticComparisonExpression1191 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_BETWEEN_in_exoticComparisonExpression1198 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1200 = new BitSet(new ulong[]{0x0219208000109030UL,0x33FC041FD4024480UL,0x000000000020A000UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1204 = new BitSet(new ulong[]{0x0219208000109030UL,0x33FC041FD4024480UL,0x000000000020A000UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1208 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NOT_BETWEEN_in_exoticComparisonExpression1215 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1217 = new BitSet(new ulong[]{0x0219208000109030UL,0x33FC041FD4024480UL,0x000000000020A000UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1221 = new BitSet(new ulong[]{0x0219208000109030UL,0x33FC041FD4024480UL,0x000000000020A000UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1225 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_IN_in_exoticComparisonExpression1232 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1234 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000002000UL});
    public static readonly BitSet FOLLOW_inList_in_exoticComparisonExpression1238 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NOT_IN_in_exoticComparisonExpression1246 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1248 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000002000UL});
    public static readonly BitSet FOLLOW_inList_in_exoticComparisonExpression1252 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_EXISTS_in_exoticComparisonExpression1260 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_quantified_in_exoticComparisonExpression1264 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_IS_NULL_in_exoticComparisonExpression1272 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1274 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_IS_NOT_NULL_in_exoticComparisonExpression1283 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_exoticComparisonExpression1285 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_ESCAPE_in_likeEscape1302 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_likeEscape1306 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_IN_LIST_in_inList1322 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_parenSelect_in_inList1328 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_simpleExprList_in_inList1332 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_simpleExpr_in_simpleExprList1353 = new BitSet(new ulong[]{0x0208008000109002UL,0x33FC041FC4024480UL,0x000000000020A000UL});
    public static readonly BitSet FOLLOW_simpleExpr_in_expr1372 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_VECTOR_EXPR_in_expr1379 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_expr1386 = new BitSet(new ulong[]{0x0219208000109038UL,0x33FC041FD4024480UL,0x000000000020A000UL});
    public static readonly BitSet FOLLOW_parenSelect_in_expr1401 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ANY_in_expr1407 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_quantified_in_expr1411 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_ALL_in_expr1419 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_quantified_in_expr1423 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_SOME_in_expr1431 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_quantified_in_expr1435 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_sqlToken_in_quantified1453 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_selectStatement_in_quantified1457 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_selectStatement_in_parenSelect1476 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_UNION_in_parenSelect1485 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_selectStatement_in_parenSelect1489 = new BitSet(new ulong[]{0x0010200000000000UL});
    public static readonly BitSet FOLLOW_parenSelect_in_parenSelect1493 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_constant_in_simpleExpr1510 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_NULL_in_simpleExpr1517 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_addrExpr_in_simpleExpr1524 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_sqlToken_in_simpleExpr1529 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_aggregate_in_simpleExpr1534 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_methodCall_in_simpleExpr1539 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_count_in_simpleExpr1544 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_parameter_in_simpleExpr1549 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_arithmeticExpr_in_simpleExpr1554 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_set_in_constant0 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_additiveExpr_in_arithmeticExpr1628 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_bitwiseExpr_in_arithmeticExpr1633 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_multiplicativeExpr_in_arithmeticExpr1638 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_UNARY_MINUS_in_arithmeticExpr1645 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_arithmeticExpr1649 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_caseExpr_in_arithmeticExpr1655 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_PLUS_in_additiveExpr1667 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_additiveExpr1669 = new BitSet(new ulong[]{0x0219208000109030UL,0x33FC041FD4024480UL,0x000000000020A000UL});
    public static readonly BitSet FOLLOW_expr_in_additiveExpr1673 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_MINUS_in_additiveExpr1680 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_additiveExpr1682 = new BitSet(new ulong[]{0x0219208000109030UL,0x33FC041FD4024480UL,0x000000000020A000UL});
    public static readonly BitSet FOLLOW_nestedExprAfterMinusDiv_in_additiveExpr1686 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_BAND_in_bitwiseExpr1699 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_bitwiseExpr1701 = new BitSet(new ulong[]{0x0219208000109030UL,0x33FC041FD4024480UL,0x000000000020A000UL});
    public static readonly BitSet FOLLOW_nestedExpr_in_bitwiseExpr1705 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_BOR_in_bitwiseExpr1712 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_bitwiseExpr1714 = new BitSet(new ulong[]{0x0219208000109030UL,0x33FC041FD4024480UL,0x000000000020A000UL});
    public static readonly BitSet FOLLOW_nestedExpr_in_bitwiseExpr1718 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_BXOR_in_bitwiseExpr1725 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_bitwiseExpr1727 = new BitSet(new ulong[]{0x0219208000109030UL,0x33FC041FD4024480UL,0x000000000020A000UL});
    public static readonly BitSet FOLLOW_nestedExpr_in_bitwiseExpr1731 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_BNOT_in_bitwiseExpr1738 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_nestedExpr_in_bitwiseExpr1742 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_STAR_in_multiplicativeExpr1756 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_nestedExpr_in_multiplicativeExpr1758 = new BitSet(new ulong[]{0x0219208000109030UL,0x33FC041FD4024480UL,0x000000000020A000UL});
    public static readonly BitSet FOLLOW_nestedExpr_in_multiplicativeExpr1762 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_DIV_in_multiplicativeExpr1769 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_nestedExpr_in_multiplicativeExpr1771 = new BitSet(new ulong[]{0x0219208000109030UL,0x33FC041FD4024480UL,0x000000000020A000UL});
    public static readonly BitSet FOLLOW_nestedExprAfterMinusDiv_in_multiplicativeExpr1775 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_additiveExpr_in_nestedExpr1797 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_bitwiseExpr_in_nestedExpr1812 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_expr_in_nestedExpr1819 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_arithmeticExpr_in_nestedExprAfterMinusDiv1841 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_expr_in_nestedExprAfterMinusDiv1848 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_CASE_in_caseExpr1860 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_WHEN_in_caseExpr1870 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_booleanExpr_in_caseExpr1874 = new BitSet(new ulong[]{0x0219208000109030UL,0x33FC041FD4024480UL,0x000000000020A000UL});
    public static readonly BitSet FOLLOW_expr_in_caseExpr1879 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_ELSE_in_caseExpr1891 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_caseExpr1895 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_CASE2_in_caseExpr1911 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_caseExpr1915 = new BitSet(new ulong[]{0x2000000000000000UL});
    public static readonly BitSet FOLLOW_WHEN_in_caseExpr1922 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_caseExpr1926 = new BitSet(new ulong[]{0x0219208000109030UL,0x33FC041FD4024480UL,0x000000000020A000UL});
    public static readonly BitSet FOLLOW_expr_in_caseExpr1930 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_ELSE_in_caseExpr1942 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_caseExpr1946 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_AGGREGATE_in_aggregate1970 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_aggregate1975 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_METHOD_CALL_in_methodCall1994 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_METHOD_NAME_in_methodCall1998 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000000000800UL});
    public static readonly BitSet FOLLOW_EXPR_LIST_in_methodCall2007 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_arguments_in_methodCall2010 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_expr_in_arguments2035 = new BitSet(new ulong[]{0x0219208404189432UL,0x33FDEC5FD41FC480UL,0x000000000020A000UL});
    public static readonly BitSet FOLLOW_comparisonExpr_in_arguments2039 = new BitSet(new ulong[]{0x0219208404189432UL,0x33FDEC5FD41FC480UL,0x000000000020A000UL});
    public static readonly BitSet FOLLOW_expr_in_arguments2048 = new BitSet(new ulong[]{0x0219208404189432UL,0x33FDEC5FD41FC480UL,0x000000000020A000UL});
    public static readonly BitSet FOLLOW_comparisonExpr_in_arguments2052 = new BitSet(new ulong[]{0x0219208404189432UL,0x33FDEC5FD41FC480UL,0x000000000020A000UL});
    public static readonly BitSet FOLLOW_NAMED_PARAM_in_parameter2070 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_PARAM_in_parameter2079 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_set_in_limitValue0 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_DOT_in_addrExpr2116 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_ALIAS_REF_in_addrExpr2130 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_INDEX_OP_in_addrExpr2140 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_SQL_TOKEN_in_sqlToken2160 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_SQL_TOKEN_in_synpred1_SqlGenerator366 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_additiveExpr_in_synpred2_SqlGenerator1790 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_bitwiseExpr_in_synpred3_SqlGenerator1805 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_arithmeticExpr_in_synpred4_SqlGenerator1834 = new BitSet(new ulong[]{0x0000000000000002UL});

}
}