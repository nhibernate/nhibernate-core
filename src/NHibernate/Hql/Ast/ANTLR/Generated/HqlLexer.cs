// $ANTLR 3.2 Sep 23, 2009 12:02:23 Hql.g 2011-05-22 07:45:50

// The variable 'variable' is assigned but its value is never used.
#pragma warning disable 168, 219
// Unreachable code detected.
#pragma warning disable 162


using System;
using Antlr.Runtime;
using IList 		= System.Collections.IList;
using ArrayList 	= System.Collections.ArrayList;
using Stack 		= Antlr.Runtime.Collections.StackList;

using IDictionary	= System.Collections.IDictionary;
using Hashtable 	= System.Collections.Hashtable;
namespace  NHibernate.Hql.Ast.ANTLR 
{
public partial class HqlLexer : Lexer {
    public const int LT = 109;
    public const int EXPONENT = 130;
    public const int STAR = 120;
    public const int FLOAT_SUFFIX = 131;
    public const int LITERAL_by = 56;
    public const int CASE = 57;
    public const int NEW = 37;
    public const int FILTER_ENTITY = 76;
    public const int PARAM = 106;
    public const int COUNT = 12;
    public const int NOT = 38;
    public const int EOF = -1;
    public const int UNARY_PLUS = 91;
    public const int QUOTED_String = 124;
    public const int ESCqs = 128;
    public const int WEIRD_IDENT = 93;
    public const int OPEN_BRACKET = 122;
    public const int FULL = 23;
    public const int ORDER_ELEMENT = 85;
    public const int IS_NULL = 80;
    public const int ESCAPE = 18;
    public const int INSERT = 29;
    public const int BOTH = 64;
    public const int NUM_DECIMAL = 97;
    public const int VERSIONED = 54;
    public const int EQ = 102;
    public const int SELECT = 45;
    public const int INTO = 30;
    public const int NE = 107;
    public const int GE = 112;
    public const int TAKE = 50;
    public const int CONCAT = 113;
    public const int ID_LETTER = 127;
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
    public const int SKIP = 47;
    public const int EMPTY = 65;
    public const int GROUP = 24;
    public const int WS = 129;
    public const int FETCH = 21;
    public const int VECTOR_EXPR = 92;
    public const int NOT_IN = 83;
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
    public const int ORDER = 41;
    public const int MAX = 35;
    public const int UPDATE = 53;
    public const int SQL_NE = 108;
    public const int AND = 6;
    public const int SUM = 49;
    public const int ASCENDING = 8;
    public const int EXPR_LIST = 75;
    public const int AS = 7;
    public const int IN = 26;
    public const int THEN = 60;
    public const int OBJECT = 68;
    public const int COMMA = 101;
    public const int IS = 31;
    public const int AVG = 9;
    public const int LEFT = 33;
    public const int SOME = 48;
    public const int ALL = 4;
    public const int BOR = 115;
    public const int IDENT = 125;
    public const int CASE2 = 74;
    public const int BXOR = 116;
    public const int PLUS = 118;
    public const int EXISTS = 19;
    public const int DOT = 15;
    public const int WITH = 63;
    public const int LIKE = 34;
    public const int OUTER = 42;
    public const int ID_START_LETTER = 126;
    public const int ROW_STAR = 88;
    public const int NOT_LIKE = 84;
    public const int RANGE = 87;
    public const int NOT_BETWEEN = 82;
    public const int HEX_DIGIT = 132;
    public const int SET = 46;
    public const int RIGHT = 44;
    public const int HAVING = 25;
    public const int MIN = 36;
    public const int IS_NOT_NULL = 79;
    public const int MINUS = 119;
    public const int ELEMENTS = 17;
    public const int BAND = 117;
    public const int TRUE = 51;
    public const int JOIN = 32;
    public const int IN_LIST = 77;
    public const int UNION = 52;
    public const int OPEN = 103;
    public const int COLON = 105;
    public const int ANY = 5;
    public const int CLOSE = 104;
    public const int WHEN = 61;
    public const int DIV = 121;
    public const int DESCENDING = 14;
    public const int AGGREGATE = 71;
    public const int BETWEEN = 10;
    public const int LE = 111;

    // delegates
    // delegators

    public HqlLexer() 
    {
		InitializeCyclicDFAs();
    }
    public HqlLexer(ICharStream input)
		: this(input, null) {
    }
    public HqlLexer(ICharStream input, RecognizerSharedState state)
		: base(input, state) {
		InitializeCyclicDFAs(); 

    }
    
    override public string GrammarFileName
    {
    	get { return "Hql.g";} 
    }

    // $ANTLR start "ALL"
    public void mALL() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = ALL;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:9:5: ( 'all' )
            // Hql.g:9:7: 'all'
            {
            	Match("all"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "ALL"

    // $ANTLR start "ANY"
    public void mANY() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = ANY;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:10:5: ( 'any' )
            // Hql.g:10:7: 'any'
            {
            	Match("any"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "ANY"

    // $ANTLR start "AND"
    public void mAND() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = AND;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:11:5: ( 'and' )
            // Hql.g:11:7: 'and'
            {
            	Match("and"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "AND"

    // $ANTLR start "AS"
    public void mAS() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = AS;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:12:4: ( 'as' )
            // Hql.g:12:6: 'as'
            {
            	Match("as"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "AS"

    // $ANTLR start "ASCENDING"
    public void mASCENDING() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = ASCENDING;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:13:11: ( 'asc' )
            // Hql.g:13:13: 'asc'
            {
            	Match("asc"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "ASCENDING"

    // $ANTLR start "AVG"
    public void mAVG() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = AVG;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:14:5: ( 'avg' )
            // Hql.g:14:7: 'avg'
            {
            	Match("avg"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "AVG"

    // $ANTLR start "BETWEEN"
    public void mBETWEEN() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = BETWEEN;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:15:9: ( 'between' )
            // Hql.g:15:11: 'between'
            {
            	Match("between"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "BETWEEN"

    // $ANTLR start "CLASS"
    public void mCLASS() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = CLASS;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:16:7: ( 'class' )
            // Hql.g:16:9: 'class'
            {
            	Match("class"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "CLASS"

    // $ANTLR start "COUNT"
    public void mCOUNT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = COUNT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:17:7: ( 'count' )
            // Hql.g:17:9: 'count'
            {
            	Match("count"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "COUNT"

    // $ANTLR start "DELETE"
    public void mDELETE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = DELETE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:18:8: ( 'delete' )
            // Hql.g:18:10: 'delete'
            {
            	Match("delete"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "DELETE"

    // $ANTLR start "DESCENDING"
    public void mDESCENDING() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = DESCENDING;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:19:12: ( 'desc' )
            // Hql.g:19:14: 'desc'
            {
            	Match("desc"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "DESCENDING"

    // $ANTLR start "DISTINCT"
    public void mDISTINCT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = DISTINCT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:20:10: ( 'distinct' )
            // Hql.g:20:12: 'distinct'
            {
            	Match("distinct"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "DISTINCT"

    // $ANTLR start "ELEMENTS"
    public void mELEMENTS() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = ELEMENTS;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:21:10: ( 'elements' )
            // Hql.g:21:12: 'elements'
            {
            	Match("elements"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "ELEMENTS"

    // $ANTLR start "ESCAPE"
    public void mESCAPE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = ESCAPE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:22:8: ( 'escape' )
            // Hql.g:22:10: 'escape'
            {
            	Match("escape"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "ESCAPE"

    // $ANTLR start "EXISTS"
    public void mEXISTS() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = EXISTS;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:23:8: ( 'exists' )
            // Hql.g:23:10: 'exists'
            {
            	Match("exists"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "EXISTS"

    // $ANTLR start "FALSE"
    public void mFALSE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = FALSE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:24:7: ( 'false' )
            // Hql.g:24:9: 'false'
            {
            	Match("false"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "FALSE"

    // $ANTLR start "FETCH"
    public void mFETCH() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = FETCH;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:25:7: ( 'fetch' )
            // Hql.g:25:9: 'fetch'
            {
            	Match("fetch"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "FETCH"

    // $ANTLR start "FROM"
    public void mFROM() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = FROM;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:26:6: ( 'from' )
            // Hql.g:26:8: 'from'
            {
            	Match("from"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "FROM"

    // $ANTLR start "FULL"
    public void mFULL() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = FULL;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:27:6: ( 'full' )
            // Hql.g:27:8: 'full'
            {
            	Match("full"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "FULL"

    // $ANTLR start "GROUP"
    public void mGROUP() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = GROUP;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:28:7: ( 'group' )
            // Hql.g:28:9: 'group'
            {
            	Match("group"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "GROUP"

    // $ANTLR start "HAVING"
    public void mHAVING() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = HAVING;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:29:8: ( 'having' )
            // Hql.g:29:10: 'having'
            {
            	Match("having"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "HAVING"

    // $ANTLR start "IN"
    public void mIN() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = IN;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:30:4: ( 'in' )
            // Hql.g:30:6: 'in'
            {
            	Match("in"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "IN"

    // $ANTLR start "INDICES"
    public void mINDICES() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = INDICES;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:31:9: ( 'indices' )
            // Hql.g:31:11: 'indices'
            {
            	Match("indices"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "INDICES"

    // $ANTLR start "INNER"
    public void mINNER() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = INNER;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:32:7: ( 'inner' )
            // Hql.g:32:9: 'inner'
            {
            	Match("inner"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "INNER"

    // $ANTLR start "INSERT"
    public void mINSERT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = INSERT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:33:8: ( 'insert' )
            // Hql.g:33:10: 'insert'
            {
            	Match("insert"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "INSERT"

    // $ANTLR start "INTO"
    public void mINTO() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = INTO;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:34:6: ( 'into' )
            // Hql.g:34:8: 'into'
            {
            	Match("into"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "INTO"

    // $ANTLR start "IS"
    public void mIS() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = IS;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:35:4: ( 'is' )
            // Hql.g:35:6: 'is'
            {
            	Match("is"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "IS"

    // $ANTLR start "JOIN"
    public void mJOIN() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = JOIN;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:36:6: ( 'join' )
            // Hql.g:36:8: 'join'
            {
            	Match("join"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "JOIN"

    // $ANTLR start "LEFT"
    public void mLEFT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = LEFT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:37:6: ( 'left' )
            // Hql.g:37:8: 'left'
            {
            	Match("left"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "LEFT"

    // $ANTLR start "LIKE"
    public void mLIKE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = LIKE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:38:6: ( 'like' )
            // Hql.g:38:8: 'like'
            {
            	Match("like"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "LIKE"

    // $ANTLR start "MAX"
    public void mMAX() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = MAX;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:39:5: ( 'max' )
            // Hql.g:39:7: 'max'
            {
            	Match("max"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "MAX"

    // $ANTLR start "MIN"
    public void mMIN() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = MIN;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:40:5: ( 'min' )
            // Hql.g:40:7: 'min'
            {
            	Match("min"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "MIN"

    // $ANTLR start "NEW"
    public void mNEW() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = NEW;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:41:5: ( 'new' )
            // Hql.g:41:7: 'new'
            {
            	Match("new"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "NEW"

    // $ANTLR start "NOT"
    public void mNOT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = NOT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:42:5: ( 'not' )
            // Hql.g:42:7: 'not'
            {
            	Match("not"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "NOT"

    // $ANTLR start "NULL"
    public void mNULL() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = NULL;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:43:6: ( 'null' )
            // Hql.g:43:8: 'null'
            {
            	Match("null"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "NULL"

    // $ANTLR start "OR"
    public void mOR() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = OR;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:44:4: ( 'or' )
            // Hql.g:44:6: 'or'
            {
            	Match("or"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "OR"

    // $ANTLR start "ORDER"
    public void mORDER() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = ORDER;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:45:7: ( 'order' )
            // Hql.g:45:9: 'order'
            {
            	Match("order"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "ORDER"

    // $ANTLR start "OUTER"
    public void mOUTER() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = OUTER;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:46:7: ( 'outer' )
            // Hql.g:46:9: 'outer'
            {
            	Match("outer"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "OUTER"

    // $ANTLR start "PROPERTIES"
    public void mPROPERTIES() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = PROPERTIES;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:47:12: ( 'properties' )
            // Hql.g:47:14: 'properties'
            {
            	Match("properties"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "PROPERTIES"

    // $ANTLR start "RIGHT"
    public void mRIGHT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = RIGHT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:48:7: ( 'right' )
            // Hql.g:48:9: 'right'
            {
            	Match("right"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "RIGHT"

    // $ANTLR start "SELECT"
    public void mSELECT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = SELECT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:49:8: ( 'select' )
            // Hql.g:49:10: 'select'
            {
            	Match("select"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "SELECT"

    // $ANTLR start "SET"
    public void mSET() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = SET;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:50:5: ( 'set' )
            // Hql.g:50:7: 'set'
            {
            	Match("set"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "SET"

    // $ANTLR start "SKIP"
    public void mSKIP() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = SKIP;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:51:6: ( 'skip' )
            // Hql.g:51:8: 'skip'
            {
            	Match("skip"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "SKIP"

    // $ANTLR start "SOME"
    public void mSOME() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = SOME;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:52:6: ( 'some' )
            // Hql.g:52:8: 'some'
            {
            	Match("some"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "SOME"

    // $ANTLR start "SUM"
    public void mSUM() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = SUM;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:53:5: ( 'sum' )
            // Hql.g:53:7: 'sum'
            {
            	Match("sum"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "SUM"

    // $ANTLR start "TAKE"
    public void mTAKE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = TAKE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:54:6: ( 'take' )
            // Hql.g:54:8: 'take'
            {
            	Match("take"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "TAKE"

    // $ANTLR start "TRUE"
    public void mTRUE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = TRUE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:55:6: ( 'true' )
            // Hql.g:55:8: 'true'
            {
            	Match("true"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "TRUE"

    // $ANTLR start "UNION"
    public void mUNION() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = UNION;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:56:7: ( 'union' )
            // Hql.g:56:9: 'union'
            {
            	Match("union"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "UNION"

    // $ANTLR start "UPDATE"
    public void mUPDATE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = UPDATE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:57:8: ( 'update' )
            // Hql.g:57:10: 'update'
            {
            	Match("update"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "UPDATE"

    // $ANTLR start "VERSIONED"
    public void mVERSIONED() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = VERSIONED;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:58:11: ( 'versioned' )
            // Hql.g:58:13: 'versioned'
            {
            	Match("versioned"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "VERSIONED"

    // $ANTLR start "WHERE"
    public void mWHERE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = WHERE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:59:7: ( 'where' )
            // Hql.g:59:9: 'where'
            {
            	Match("where"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "WHERE"

    // $ANTLR start "LITERAL_by"
    public void mLITERAL_by() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = LITERAL_by;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:60:12: ( 'by' )
            // Hql.g:60:14: 'by'
            {
            	Match("by"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "LITERAL_by"

    // $ANTLR start "CASE"
    public void mCASE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = CASE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:61:6: ( 'case' )
            // Hql.g:61:8: 'case'
            {
            	Match("case"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "CASE"

    // $ANTLR start "END"
    public void mEND() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = END;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:62:5: ( 'end' )
            // Hql.g:62:7: 'end'
            {
            	Match("end"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "END"

    // $ANTLR start "ELSE"
    public void mELSE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = ELSE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:63:6: ( 'else' )
            // Hql.g:63:8: 'else'
            {
            	Match("else"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "ELSE"

    // $ANTLR start "THEN"
    public void mTHEN() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = THEN;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:64:6: ( 'then' )
            // Hql.g:64:8: 'then'
            {
            	Match("then"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "THEN"

    // $ANTLR start "WHEN"
    public void mWHEN() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = WHEN;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:65:6: ( 'when' )
            // Hql.g:65:8: 'when'
            {
            	Match("when"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "WHEN"

    // $ANTLR start "ON"
    public void mON() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = ON;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:66:4: ( 'on' )
            // Hql.g:66:6: 'on'
            {
            	Match("on"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "ON"

    // $ANTLR start "WITH"
    public void mWITH() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = WITH;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:67:6: ( 'with' )
            // Hql.g:67:8: 'with'
            {
            	Match("with"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "WITH"

    // $ANTLR start "BOTH"
    public void mBOTH() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = BOTH;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:68:6: ( 'both' )
            // Hql.g:68:8: 'both'
            {
            	Match("both"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "BOTH"

    // $ANTLR start "EMPTY"
    public void mEMPTY() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = EMPTY;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:69:7: ( 'empty' )
            // Hql.g:69:9: 'empty'
            {
            	Match("empty"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "EMPTY"

    // $ANTLR start "LEADING"
    public void mLEADING() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = LEADING;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:70:9: ( 'leading' )
            // Hql.g:70:11: 'leading'
            {
            	Match("leading"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "LEADING"

    // $ANTLR start "MEMBER"
    public void mMEMBER() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = MEMBER;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:71:8: ( 'member' )
            // Hql.g:71:10: 'member'
            {
            	Match("member"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "MEMBER"

    // $ANTLR start "OBJECT"
    public void mOBJECT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = OBJECT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:72:8: ( 'object' )
            // Hql.g:72:10: 'object'
            {
            	Match("object"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "OBJECT"

    // $ANTLR start "OF"
    public void mOF() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = OF;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:73:4: ( 'of' )
            // Hql.g:73:6: 'of'
            {
            	Match("of"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "OF"

    // $ANTLR start "TRAILING"
    public void mTRAILING() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = TRAILING;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:74:10: ( 'trailing' )
            // Hql.g:74:12: 'trailing'
            {
            	Match("trailing"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "TRAILING"

    // $ANTLR start "T__133"
    public void mT__133() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__133;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:75:8: ( 'ascending' )
            // Hql.g:75:10: 'ascending'
            {
            	Match("ascending"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__133"

    // $ANTLR start "T__134"
    public void mT__134() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__134;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:76:8: ( 'descending' )
            // Hql.g:76:10: 'descending'
            {
            	Match("descending"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__134"

    // $ANTLR start "EQ"
    public void mEQ() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = EQ;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:689:3: ( '=' )
            // Hql.g:689:5: '='
            {
            	Match('='); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "EQ"

    // $ANTLR start "LT"
    public void mLT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = LT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:690:3: ( '<' )
            // Hql.g:690:5: '<'
            {
            	Match('<'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "LT"

    // $ANTLR start "GT"
    public void mGT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = GT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:691:3: ( '>' )
            // Hql.g:691:5: '>'
            {
            	Match('>'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "GT"

    // $ANTLR start "SQL_NE"
    public void mSQL_NE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = SQL_NE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:692:7: ( '<>' )
            // Hql.g:692:9: '<>'
            {
            	Match("<>"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "SQL_NE"

    // $ANTLR start "NE"
    public void mNE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = NE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:693:3: ( '!=' | '^=' )
            int alt1 = 2;
            int LA1_0 = input.LA(1);

            if ( (LA1_0 == '!') )
            {
                alt1 = 1;
            }
            else if ( (LA1_0 == '^') )
            {
                alt1 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d1s0 =
                    new NoViableAltException("", 1, 0, input);

                throw nvae_d1s0;
            }
            switch (alt1) 
            {
                case 1 :
                    // Hql.g:693:5: '!='
                    {
                    	Match("!="); if (state.failed) return ;


                    }
                    break;
                case 2 :
                    // Hql.g:693:12: '^='
                    {
                    	Match("^="); if (state.failed) return ;


                    }
                    break;

            }
            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "NE"

    // $ANTLR start "LE"
    public void mLE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = LE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:694:3: ( '<=' )
            // Hql.g:694:5: '<='
            {
            	Match("<="); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "LE"

    // $ANTLR start "GE"
    public void mGE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = GE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:695:3: ( '>=' )
            // Hql.g:695:5: '>='
            {
            	Match(">="); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "GE"

    // $ANTLR start "BOR"
    public void mBOR() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = BOR;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:697:5: ( '|' )
            // Hql.g:697:8: '|'
            {
            	Match('|'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "BOR"

    // $ANTLR start "BXOR"
    public void mBXOR() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = BXOR;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:698:6: ( '^' )
            // Hql.g:698:8: '^'
            {
            	Match('^'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "BXOR"

    // $ANTLR start "BAND"
    public void mBAND() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = BAND;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:699:6: ( '&' )
            // Hql.g:699:8: '&'
            {
            	Match('&'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "BAND"

    // $ANTLR start "BNOT"
    public void mBNOT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = BNOT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:700:6: ( '!' )
            // Hql.g:700:8: '!'
            {
            	Match('!'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "BNOT"

    // $ANTLR start "COMMA"
    public void mCOMMA() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = COMMA;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:702:6: ( ',' )
            // Hql.g:702:8: ','
            {
            	Match(','); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "COMMA"

    // $ANTLR start "OPEN"
    public void mOPEN() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = OPEN;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:704:5: ( '(' )
            // Hql.g:704:7: '('
            {
            	Match('('); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "OPEN"

    // $ANTLR start "CLOSE"
    public void mCLOSE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = CLOSE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:705:6: ( ')' )
            // Hql.g:705:8: ')'
            {
            	Match(')'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "CLOSE"

    // $ANTLR start "OPEN_BRACKET"
    public void mOPEN_BRACKET() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = OPEN_BRACKET;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:706:13: ( '[' )
            // Hql.g:706:15: '['
            {
            	Match('['); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "OPEN_BRACKET"

    // $ANTLR start "CLOSE_BRACKET"
    public void mCLOSE_BRACKET() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = CLOSE_BRACKET;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:707:14: ( ']' )
            // Hql.g:707:16: ']'
            {
            	Match(']'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "CLOSE_BRACKET"

    // $ANTLR start "CONCAT"
    public void mCONCAT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = CONCAT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:709:7: ( '||' )
            // Hql.g:709:9: '||'
            {
            	Match("||"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "CONCAT"

    // $ANTLR start "PLUS"
    public void mPLUS() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = PLUS;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:710:5: ( '+' )
            // Hql.g:710:7: '+'
            {
            	Match('+'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "PLUS"

    // $ANTLR start "MINUS"
    public void mMINUS() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = MINUS;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:711:6: ( '-' )
            // Hql.g:711:8: '-'
            {
            	Match('-'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "MINUS"

    // $ANTLR start "STAR"
    public void mSTAR() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = STAR;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:712:5: ( '*' )
            // Hql.g:712:7: '*'
            {
            	Match('*'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "STAR"

    // $ANTLR start "DIV"
    public void mDIV() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = DIV;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:713:4: ( '/' )
            // Hql.g:713:6: '/'
            {
            	Match('/'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "DIV"

    // $ANTLR start "COLON"
    public void mCOLON() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = COLON;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:714:6: ( ':' )
            // Hql.g:714:8: ':'
            {
            	Match(':'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "COLON"

    // $ANTLR start "PARAM"
    public void mPARAM() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = PARAM;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:715:6: ( '?' )
            // Hql.g:715:8: '?'
            {
            	Match('?'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "PARAM"

    // $ANTLR start "IDENT"
    public void mIDENT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = IDENT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:718:2: ( ID_START_LETTER ( ID_LETTER )* )
            // Hql.g:718:4: ID_START_LETTER ( ID_LETTER )*
            {
            	mID_START_LETTER(); if (state.failed) return ;
            	// Hql.g:718:20: ( ID_LETTER )*
            	do 
            	{
            	    int alt2 = 2;
            	    int LA2_0 = input.LA(1);

            	    if ( (LA2_0 == '$' || (LA2_0 >= '0' && LA2_0 <= '9') || (LA2_0 >= 'A' && LA2_0 <= 'Z') || LA2_0 == '_' || (LA2_0 >= 'a' && LA2_0 <= 'z') || (LA2_0 >= '\u0080' && LA2_0 <= '\uFFFE')) )
            	    {
            	        alt2 = 1;
            	    }


            	    switch (alt2) 
            		{
            			case 1 :
            			    // Hql.g:718:22: ID_LETTER
            			    {
            			    	mID_LETTER(); if (state.failed) return ;

            			    }
            			    break;

            			default:
            			    goto loop2;
            	    }
            	} while (true);

            	loop2:
            		;	// Stops C# compiler whining that label 'loop2' has no statements


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "IDENT"

    // $ANTLR start "ID_START_LETTER"
    public void mID_START_LETTER() // throws RecognitionException [2]
    {
    		try
    		{
            // Hql.g:723:5: ( '_' | '$' | 'a' .. 'z' | 'A' .. 'Z' | '\\u0080' .. '\\ufffe' )
            // Hql.g:
            {
            	if ( input.LA(1) == '$' || (input.LA(1) >= 'A' && input.LA(1) <= 'Z') || input.LA(1) == '_' || (input.LA(1) >= 'a' && input.LA(1) <= 'z') || (input.LA(1) >= '\u0080' && input.LA(1) <= '\uFFFE') ) 
            	{
            	    input.Consume();
            	state.failed = false;
            	}
            	else 
            	{
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	    Recover(mse);
            	    throw mse;}


            }

        }
        finally 
    	{
        }
    }
    // $ANTLR end "ID_START_LETTER"

    // $ANTLR start "ID_LETTER"
    public void mID_LETTER() // throws RecognitionException [2]
    {
    		try
    		{
            // Hql.g:732:5: ( ID_START_LETTER | '0' .. '9' )
            // Hql.g:
            {
            	if ( input.LA(1) == '$' || (input.LA(1) >= '0' && input.LA(1) <= '9') || (input.LA(1) >= 'A' && input.LA(1) <= 'Z') || input.LA(1) == '_' || (input.LA(1) >= 'a' && input.LA(1) <= 'z') || (input.LA(1) >= '\u0080' && input.LA(1) <= '\uFFFE') ) 
            	{
            	    input.Consume();
            	state.failed = false;
            	}
            	else 
            	{
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	    Recover(mse);
            	    throw mse;}


            }

        }
        finally 
    	{
        }
    }
    // $ANTLR end "ID_LETTER"

    // $ANTLR start "QUOTED_String"
    public void mQUOTED_String() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = QUOTED_String;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:737:4: ( '\\'' ( ( ESCqs )=> ESCqs | ~ '\\'' )* '\\'' )
            // Hql.g:737:6: '\\'' ( ( ESCqs )=> ESCqs | ~ '\\'' )* '\\''
            {
            	Match('\''); if (state.failed) return ;
            	// Hql.g:737:11: ( ( ESCqs )=> ESCqs | ~ '\\'' )*
            	do 
            	{
            	    int alt3 = 3;
            	    int LA3_0 = input.LA(1);

            	    if ( (LA3_0 == '\'') )
            	    {
            	        int LA3_1 = input.LA(2);

            	        if ( (LA3_1 == '\'') && (synpred1_Hql()) )
            	        {
            	            alt3 = 1;
            	        }


            	    }
            	    else if ( ((LA3_0 >= '\u0000' && LA3_0 <= '&') || (LA3_0 >= '(' && LA3_0 <= '\uFFFF')) )
            	    {
            	        alt3 = 2;
            	    }


            	    switch (alt3) 
            		{
            			case 1 :
            			    // Hql.g:737:13: ( ESCqs )=> ESCqs
            			    {
            			    	mESCqs(); if (state.failed) return ;

            			    }
            			    break;
            			case 2 :
            			    // Hql.g:737:31: ~ '\\''
            			    {
            			    	if ( (input.LA(1) >= '\u0000' && input.LA(1) <= '&') || (input.LA(1) >= '(' && input.LA(1) <= '\uFFFF') ) 
            			    	{
            			    	    input.Consume();
            			    	state.failed = false;
            			    	}
            			    	else 
            			    	{
            			    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            			    	    MismatchedSetException mse = new MismatchedSetException(null,input);
            			    	    Recover(mse);
            			    	    throw mse;}


            			    }
            			    break;

            			default:
            			    goto loop3;
            	    }
            	} while (true);

            	loop3:
            		;	// Stops C# compiler whining that label 'loop3' has no statements

            	Match('\''); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "QUOTED_String"

    // $ANTLR start "ESCqs"
    public void mESCqs() // throws RecognitionException [2]
    {
    		try
    		{
            // Hql.g:742:2: ( '\\'' '\\'' )
            // Hql.g:743:3: '\\'' '\\''
            {
            	Match('\''); if (state.failed) return ;
            	Match('\''); if (state.failed) return ;

            }

        }
        finally 
    	{
        }
    }
    // $ANTLR end "ESCqs"

    // $ANTLR start "WS"
    public void mWS() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = WS;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // Hql.g:746:5: ( ( ' ' | '\\t' | '\\r' '\\n' | '\\n' | '\\r' ) )
            // Hql.g:746:9: ( ' ' | '\\t' | '\\r' '\\n' | '\\n' | '\\r' )
            {
            	// Hql.g:746:9: ( ' ' | '\\t' | '\\r' '\\n' | '\\n' | '\\r' )
            	int alt4 = 5;
            	switch ( input.LA(1) ) 
            	{
            	case ' ':
            		{
            	    alt4 = 1;
            	    }
            	    break;
            	case '\t':
            		{
            	    alt4 = 2;
            	    }
            	    break;
            	case '\r':
            		{
            	    int LA4_3 = input.LA(2);

            	    if ( (LA4_3 == '\n') )
            	    {
            	        alt4 = 3;
            	    }
            	    else 
            	    {
            	        alt4 = 5;}
            	    }
            	    break;
            	case '\n':
            		{
            	    alt4 = 4;
            	    }
            	    break;
            		default:
            		    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            		    NoViableAltException nvae_d4s0 =
            		        new NoViableAltException("", 4, 0, input);

            		    throw nvae_d4s0;
            	}

            	switch (alt4) 
            	{
            	    case 1 :
            	        // Hql.g:746:13: ' '
            	        {
            	        	Match(' '); if (state.failed) return ;

            	        }
            	        break;
            	    case 2 :
            	        // Hql.g:747:7: '\\t'
            	        {
            	        	Match('\t'); if (state.failed) return ;

            	        }
            	        break;
            	    case 3 :
            	        // Hql.g:748:7: '\\r' '\\n'
            	        {
            	        	Match('\r'); if (state.failed) return ;
            	        	Match('\n'); if (state.failed) return ;

            	        }
            	        break;
            	    case 4 :
            	        // Hql.g:749:7: '\\n'
            	        {
            	        	Match('\n'); if (state.failed) return ;

            	        }
            	        break;
            	    case 5 :
            	        // Hql.g:750:7: '\\r'
            	        {
            	        	Match('\r'); if (state.failed) return ;

            	        }
            	        break;

            	}

            	if ( (state.backtracking==0) )
            	{
            	  Skip();
            	}

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "WS"

    // $ANTLR start "NUM_INT"
    public void mNUM_INT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = NUM_INT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            IToken f1 = null;
            IToken f2 = null;
            IToken f3 = null;
            IToken f4 = null;

            bool isDecimal=false; IToken t=null;
            // Hql.g:759:2: ( '.' ( ( '0' .. '9' )+ ( EXPONENT )? (f1= FLOAT_SUFFIX )? )? | ( '0' ( ( 'x' ) ( HEX_DIGIT )+ | ( '0' .. '7' )+ )? | ( '1' .. '9' ) ( '0' .. '9' )* ) ( ( 'l' ) | {...}? ( '.' ( '0' .. '9' )* ( EXPONENT )? (f2= FLOAT_SUFFIX )? | EXPONENT (f3= FLOAT_SUFFIX )? | f4= FLOAT_SUFFIX ) )? )
            int alt20 = 2;
            int LA20_0 = input.LA(1);

            if ( (LA20_0 == '.') )
            {
                alt20 = 1;
            }
            else if ( ((LA20_0 >= '0' && LA20_0 <= '9')) )
            {
                alt20 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d20s0 =
                    new NoViableAltException("", 20, 0, input);

                throw nvae_d20s0;
            }
            switch (alt20) 
            {
                case 1 :
                    // Hql.g:759:6: '.' ( ( '0' .. '9' )+ ( EXPONENT )? (f1= FLOAT_SUFFIX )? )?
                    {
                    	Match('.'); if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	  _type = DOT;
                    	}
                    	// Hql.g:760:4: ( ( '0' .. '9' )+ ( EXPONENT )? (f1= FLOAT_SUFFIX )? )?
                    	int alt8 = 2;
                    	int LA8_0 = input.LA(1);

                    	if ( ((LA8_0 >= '0' && LA8_0 <= '9')) )
                    	{
                    	    alt8 = 1;
                    	}
                    	switch (alt8) 
                    	{
                    	    case 1 :
                    	        // Hql.g:760:6: ( '0' .. '9' )+ ( EXPONENT )? (f1= FLOAT_SUFFIX )?
                    	        {
                    	        	// Hql.g:760:6: ( '0' .. '9' )+
                    	        	int cnt5 = 0;
                    	        	do 
                    	        	{
                    	        	    int alt5 = 2;
                    	        	    int LA5_0 = input.LA(1);

                    	        	    if ( ((LA5_0 >= '0' && LA5_0 <= '9')) )
                    	        	    {
                    	        	        alt5 = 1;
                    	        	    }


                    	        	    switch (alt5) 
                    	        		{
                    	        			case 1 :
                    	        			    // Hql.g:760:7: '0' .. '9'
                    	        			    {
                    	        			    	MatchRange('0','9'); if (state.failed) return ;

                    	        			    }
                    	        			    break;

                    	        			default:
                    	        			    if ( cnt5 >= 1 ) goto loop5;
                    	        			    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	        		            EarlyExitException eee5 =
                    	        		                new EarlyExitException(5, input);
                    	        		            throw eee5;
                    	        	    }
                    	        	    cnt5++;
                    	        	} while (true);

                    	        	loop5:
                    	        		;	// Stops C# compiler whining that label 'loop5' has no statements

                    	        	// Hql.g:760:18: ( EXPONENT )?
                    	        	int alt6 = 2;
                    	        	int LA6_0 = input.LA(1);

                    	        	if ( (LA6_0 == 'e') )
                    	        	{
                    	        	    alt6 = 1;
                    	        	}
                    	        	switch (alt6) 
                    	        	{
                    	        	    case 1 :
                    	        	        // Hql.g:760:19: EXPONENT
                    	        	        {
                    	        	        	mEXPONENT(); if (state.failed) return ;

                    	        	        }
                    	        	        break;

                    	        	}

                    	        	// Hql.g:760:30: (f1= FLOAT_SUFFIX )?
                    	        	int alt7 = 2;
                    	        	int LA7_0 = input.LA(1);

                    	        	if ( (LA7_0 == 'd' || LA7_0 == 'f' || LA7_0 == 'm') )
                    	        	{
                    	        	    alt7 = 1;
                    	        	}
                    	        	switch (alt7) 
                    	        	{
                    	        	    case 1 :
                    	        	        // Hql.g:760:31: f1= FLOAT_SUFFIX
                    	        	        {
                    	        	        	int f1Start1034 = CharIndex;
                    	        	        	mFLOAT_SUFFIX(); if (state.failed) return ;
                    	        	        	f1 = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, f1Start1034, CharIndex-1);
                    	        	        	if ( (state.backtracking==0) )
                    	        	        	{
                    	        	        	  t=f1;
                    	        	        	}

                    	        	        }
                    	        	        break;

                    	        	}

                    	        	if ( (state.backtracking==0) )
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

                    	        }
                    	        break;

                    	}


                    }
                    break;
                case 2 :
                    // Hql.g:776:4: ( '0' ( ( 'x' ) ( HEX_DIGIT )+ | ( '0' .. '7' )+ )? | ( '1' .. '9' ) ( '0' .. '9' )* ) ( ( 'l' ) | {...}? ( '.' ( '0' .. '9' )* ( EXPONENT )? (f2= FLOAT_SUFFIX )? | EXPONENT (f3= FLOAT_SUFFIX )? | f4= FLOAT_SUFFIX ) )?
                    {
                    	// Hql.g:776:4: ( '0' ( ( 'x' ) ( HEX_DIGIT )+ | ( '0' .. '7' )+ )? | ( '1' .. '9' ) ( '0' .. '9' )* )
                    	int alt13 = 2;
                    	int LA13_0 = input.LA(1);

                    	if ( (LA13_0 == '0') )
                    	{
                    	    alt13 = 1;
                    	}
                    	else if ( ((LA13_0 >= '1' && LA13_0 <= '9')) )
                    	{
                    	    alt13 = 2;
                    	}
                    	else 
                    	{
                    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	    NoViableAltException nvae_d13s0 =
                    	        new NoViableAltException("", 13, 0, input);

                    	    throw nvae_d13s0;
                    	}
                    	switch (alt13) 
                    	{
                    	    case 1 :
                    	        // Hql.g:776:6: '0' ( ( 'x' ) ( HEX_DIGIT )+ | ( '0' .. '7' )+ )?
                    	        {
                    	        	Match('0'); if (state.failed) return ;
                    	        	if ( (state.backtracking==0) )
                    	        	{
                    	        	  isDecimal = true;
                    	        	}
                    	        	// Hql.g:777:4: ( ( 'x' ) ( HEX_DIGIT )+ | ( '0' .. '7' )+ )?
                    	        	int alt11 = 3;
                    	        	int LA11_0 = input.LA(1);

                    	        	if ( (LA11_0 == 'x') )
                    	        	{
                    	        	    alt11 = 1;
                    	        	}
                    	        	else if ( ((LA11_0 >= '0' && LA11_0 <= '7')) )
                    	        	{
                    	        	    alt11 = 2;
                    	        	}
                    	        	switch (alt11) 
                    	        	{
                    	        	    case 1 :
                    	        	        // Hql.g:777:6: ( 'x' ) ( HEX_DIGIT )+
                    	        	        {
                    	        	        	// Hql.g:777:6: ( 'x' )
                    	        	        	// Hql.g:777:7: 'x'
                    	        	        	{
                    	        	        		Match('x'); if (state.failed) return ;

                    	        	        	}

                    	        	        	// Hql.g:778:5: ( HEX_DIGIT )+
                    	        	        	int cnt9 = 0;
                    	        	        	do 
                    	        	        	{
                    	        	        	    int alt9 = 2;
                    	        	        	    switch ( input.LA(1) ) 
                    	        	        	    {
                    	        	        	    case 'e':
                    	        	        	    	{
                    	        	        	        int LA9_2 = input.LA(2);

                    	        	        	        if ( ((LA9_2 >= '0' && LA9_2 <= '9')) )
                    	        	        	        {
                    	        	        	            int LA9_5 = input.LA(3);

                    	        	        	            if ( (!(((isDecimal)))) )
                    	        	        	            {
                    	        	        	                alt9 = 1;
                    	        	        	            }


                    	        	        	        }

                    	        	        	        else 
                    	        	        	        {
                    	        	        	            alt9 = 1;
                    	        	        	        }

                    	        	        	        }
                    	        	        	        break;
                    	        	        	    case 'd':
                    	        	        	    case 'f':
                    	        	        	    	{
                    	        	        	        int LA9_3 = input.LA(2);

                    	        	        	        if ( (!(((isDecimal)))) )
                    	        	        	        {
                    	        	        	            alt9 = 1;
                    	        	        	        }


                    	        	        	        }
                    	        	        	        break;
                    	        	        	    case '0':
                    	        	        	    case '1':
                    	        	        	    case '2':
                    	        	        	    case '3':
                    	        	        	    case '4':
                    	        	        	    case '5':
                    	        	        	    case '6':
                    	        	        	    case '7':
                    	        	        	    case '8':
                    	        	        	    case '9':
                    	        	        	    case 'a':
                    	        	        	    case 'b':
                    	        	        	    case 'c':
                    	        	        	    	{
                    	        	        	        alt9 = 1;
                    	        	        	        }
                    	        	        	        break;

                    	        	        	    }

                    	        	        	    switch (alt9) 
                    	        	        		{
                    	        	        			case 1 :
                    	        	        			    // Hql.g:785:7: HEX_DIGIT
                    	        	        			    {
                    	        	        			    	mHEX_DIGIT(); if (state.failed) return ;

                    	        	        			    }
                    	        	        			    break;

                    	        	        			default:
                    	        	        			    if ( cnt9 >= 1 ) goto loop9;
                    	        	        			    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	        	        		            EarlyExitException eee9 =
                    	        	        		                new EarlyExitException(9, input);
                    	        	        		            throw eee9;
                    	        	        	    }
                    	        	        	    cnt9++;
                    	        	        	} while (true);

                    	        	        	loop9:
                    	        	        		;	// Stops C# compiler whining that label 'loop9' has no statements


                    	        	        }
                    	        	        break;
                    	        	    case 2 :
                    	        	        // Hql.g:787:6: ( '0' .. '7' )+
                    	        	        {
                    	        	        	// Hql.g:787:6: ( '0' .. '7' )+
                    	        	        	int cnt10 = 0;
                    	        	        	do 
                    	        	        	{
                    	        	        	    int alt10 = 2;
                    	        	        	    int LA10_0 = input.LA(1);

                    	        	        	    if ( ((LA10_0 >= '0' && LA10_0 <= '7')) )
                    	        	        	    {
                    	        	        	        alt10 = 1;
                    	        	        	    }


                    	        	        	    switch (alt10) 
                    	        	        		{
                    	        	        			case 1 :
                    	        	        			    // Hql.g:787:7: '0' .. '7'
                    	        	        			    {
                    	        	        			    	MatchRange('0','7'); if (state.failed) return ;

                    	        	        			    }
                    	        	        			    break;

                    	        	        			default:
                    	        	        			    if ( cnt10 >= 1 ) goto loop10;
                    	        	        			    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	        	        		            EarlyExitException eee10 =
                    	        	        		                new EarlyExitException(10, input);
                    	        	        		            throw eee10;
                    	        	        	    }
                    	        	        	    cnt10++;
                    	        	        	} while (true);

                    	        	        	loop10:
                    	        	        		;	// Stops C# compiler whining that label 'loop10' has no statements


                    	        	        }
                    	        	        break;

                    	        	}


                    	        }
                    	        break;
                    	    case 2 :
                    	        // Hql.g:789:5: ( '1' .. '9' ) ( '0' .. '9' )*
                    	        {
                    	        	// Hql.g:789:5: ( '1' .. '9' )
                    	        	// Hql.g:789:6: '1' .. '9'
                    	        	{
                    	        		MatchRange('1','9'); if (state.failed) return ;

                    	        	}

                    	        	// Hql.g:789:16: ( '0' .. '9' )*
                    	        	do 
                    	        	{
                    	        	    int alt12 = 2;
                    	        	    int LA12_0 = input.LA(1);

                    	        	    if ( ((LA12_0 >= '0' && LA12_0 <= '9')) )
                    	        	    {
                    	        	        alt12 = 1;
                    	        	    }


                    	        	    switch (alt12) 
                    	        		{
                    	        			case 1 :
                    	        			    // Hql.g:789:17: '0' .. '9'
                    	        			    {
                    	        			    	MatchRange('0','9'); if (state.failed) return ;

                    	        			    }
                    	        			    break;

                    	        			default:
                    	        			    goto loop12;
                    	        	    }
                    	        	} while (true);

                    	        	loop12:
                    	        		;	// Stops C# compiler whining that label 'loop12' has no statements

                    	        	if ( (state.backtracking==0) )
                    	        	{
                    	        	  isDecimal=true;
                    	        	}

                    	        }
                    	        break;

                    	}

                    	// Hql.g:791:3: ( ( 'l' ) | {...}? ( '.' ( '0' .. '9' )* ( EXPONENT )? (f2= FLOAT_SUFFIX )? | EXPONENT (f3= FLOAT_SUFFIX )? | f4= FLOAT_SUFFIX ) )?
                    	int alt19 = 3;
                    	int LA19_0 = input.LA(1);

                    	if ( (LA19_0 == 'l') )
                    	{
                    	    alt19 = 1;
                    	}
                    	else if ( (LA19_0 == '.' || (LA19_0 >= 'd' && LA19_0 <= 'f') || LA19_0 == 'm') )
                    	{
                    	    alt19 = 2;
                    	}
                    	switch (alt19) 
                    	{
                    	    case 1 :
                    	        // Hql.g:791:5: ( 'l' )
                    	        {
                    	        	// Hql.g:791:5: ( 'l' )
                    	        	// Hql.g:791:6: 'l'
                    	        	{
                    	        		Match('l'); if (state.failed) return ;

                    	        	}

                    	        	if ( (state.backtracking==0) )
                    	        	{
                    	        	   _type = NUM_LONG; 
                    	        	}

                    	        }
                    	        break;
                    	    case 2 :
                    	        // Hql.g:794:5: {...}? ( '.' ( '0' .. '9' )* ( EXPONENT )? (f2= FLOAT_SUFFIX )? | EXPONENT (f3= FLOAT_SUFFIX )? | f4= FLOAT_SUFFIX )
                    	        {
                    	        	if ( !((isDecimal)) ) 
                    	        	{
                    	        	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	        	    throw new FailedPredicateException(input, "NUM_INT", "isDecimal");
                    	        	}
                    	        	// Hql.g:795:4: ( '.' ( '0' .. '9' )* ( EXPONENT )? (f2= FLOAT_SUFFIX )? | EXPONENT (f3= FLOAT_SUFFIX )? | f4= FLOAT_SUFFIX )
                    	        	int alt18 = 3;
                    	        	switch ( input.LA(1) ) 
                    	        	{
                    	        	case '.':
                    	        		{
                    	        	    alt18 = 1;
                    	        	    }
                    	        	    break;
                    	        	case 'e':
                    	        		{
                    	        	    alt18 = 2;
                    	        	    }
                    	        	    break;
                    	        	case 'd':
                    	        	case 'f':
                    	        	case 'm':
                    	        		{
                    	        	    alt18 = 3;
                    	        	    }
                    	        	    break;
                    	        		default:
                    	        		    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	        		    NoViableAltException nvae_d18s0 =
                    	        		        new NoViableAltException("", 18, 0, input);

                    	        		    throw nvae_d18s0;
                    	        	}

                    	        	switch (alt18) 
                    	        	{
                    	        	    case 1 :
                    	        	        // Hql.g:795:8: '.' ( '0' .. '9' )* ( EXPONENT )? (f2= FLOAT_SUFFIX )?
                    	        	        {
                    	        	        	Match('.'); if (state.failed) return ;
                    	        	        	// Hql.g:795:12: ( '0' .. '9' )*
                    	        	        	do 
                    	        	        	{
                    	        	        	    int alt14 = 2;
                    	        	        	    int LA14_0 = input.LA(1);

                    	        	        	    if ( ((LA14_0 >= '0' && LA14_0 <= '9')) )
                    	        	        	    {
                    	        	        	        alt14 = 1;
                    	        	        	    }


                    	        	        	    switch (alt14) 
                    	        	        		{
                    	        	        			case 1 :
                    	        	        			    // Hql.g:795:13: '0' .. '9'
                    	        	        			    {
                    	        	        			    	MatchRange('0','9'); if (state.failed) return ;

                    	        	        			    }
                    	        	        			    break;

                    	        	        			default:
                    	        	        			    goto loop14;
                    	        	        	    }
                    	        	        	} while (true);

                    	        	        	loop14:
                    	        	        		;	// Stops C# compiler whining that label 'loop14' has no statements

                    	        	        	// Hql.g:795:24: ( EXPONENT )?
                    	        	        	int alt15 = 2;
                    	        	        	int LA15_0 = input.LA(1);

                    	        	        	if ( (LA15_0 == 'e') )
                    	        	        	{
                    	        	        	    alt15 = 1;
                    	        	        	}
                    	        	        	switch (alt15) 
                    	        	        	{
                    	        	        	    case 1 :
                    	        	        	        // Hql.g:795:25: EXPONENT
                    	        	        	        {
                    	        	        	        	mEXPONENT(); if (state.failed) return ;

                    	        	        	        }
                    	        	        	        break;

                    	        	        	}

                    	        	        	// Hql.g:795:36: (f2= FLOAT_SUFFIX )?
                    	        	        	int alt16 = 2;
                    	        	        	int LA16_0 = input.LA(1);

                    	        	        	if ( (LA16_0 == 'd' || LA16_0 == 'f' || LA16_0 == 'm') )
                    	        	        	{
                    	        	        	    alt16 = 1;
                    	        	        	}
                    	        	        	switch (alt16) 
                    	        	        	{
                    	        	        	    case 1 :
                    	        	        	        // Hql.g:795:37: f2= FLOAT_SUFFIX
                    	        	        	        {
                    	        	        	        	int f2Start1236 = CharIndex;
                    	        	        	        	mFLOAT_SUFFIX(); if (state.failed) return ;
                    	        	        	        	f2 = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, f2Start1236, CharIndex-1);
                    	        	        	        	if ( (state.backtracking==0) )
                    	        	        	        	{
                    	        	        	        	  t=f2;
                    	        	        	        	}

                    	        	        	        }
                    	        	        	        break;

                    	        	        	}


                    	        	        }
                    	        	        break;
                    	        	    case 2 :
                    	        	        // Hql.g:796:8: EXPONENT (f3= FLOAT_SUFFIX )?
                    	        	        {
                    	        	        	mEXPONENT(); if (state.failed) return ;
                    	        	        	// Hql.g:796:17: (f3= FLOAT_SUFFIX )?
                    	        	        	int alt17 = 2;
                    	        	        	int LA17_0 = input.LA(1);

                    	        	        	if ( (LA17_0 == 'd' || LA17_0 == 'f' || LA17_0 == 'm') )
                    	        	        	{
                    	        	        	    alt17 = 1;
                    	        	        	}
                    	        	        	switch (alt17) 
                    	        	        	{
                    	        	        	    case 1 :
                    	        	        	        // Hql.g:796:18: f3= FLOAT_SUFFIX
                    	        	        	        {
                    	        	        	        	int f3Start1254 = CharIndex;
                    	        	        	        	mFLOAT_SUFFIX(); if (state.failed) return ;
                    	        	        	        	f3 = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, f3Start1254, CharIndex-1);
                    	        	        	        	if ( (state.backtracking==0) )
                    	        	        	        	{
                    	        	        	        	  t=f3;
                    	        	        	        	}

                    	        	        	        }
                    	        	        	        break;

                    	        	        	}


                    	        	        }
                    	        	        break;
                    	        	    case 3 :
                    	        	        // Hql.g:797:8: f4= FLOAT_SUFFIX
                    	        	        {
                    	        	        	int f4Start1269 = CharIndex;
                    	        	        	mFLOAT_SUFFIX(); if (state.failed) return ;
                    	        	        	f4 = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, f4Start1269, CharIndex-1);
                    	        	        	if ( (state.backtracking==0) )
                    	        	        	{
                    	        	        	  t=f4;
                    	        	        	}

                    	        	        }
                    	        	        break;

                    	        	}

                    	        	if ( (state.backtracking==0) )
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

                    	        }
                    	        break;

                    	}


                    }
                    break;

            }
            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "NUM_INT"

    // $ANTLR start "HEX_DIGIT"
    public void mHEX_DIGIT() // throws RecognitionException [2]
    {
    		try
    		{
            // Hql.g:819:2: ( ( '0' .. '9' | 'a' .. 'f' ) )
            // Hql.g:819:4: ( '0' .. '9' | 'a' .. 'f' )
            {
            	if ( (input.LA(1) >= '0' && input.LA(1) <= '9') || (input.LA(1) >= 'a' && input.LA(1) <= 'f') ) 
            	{
            	    input.Consume();
            	state.failed = false;
            	}
            	else 
            	{
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	    Recover(mse);
            	    throw mse;}


            }

        }
        finally 
    	{
        }
    }
    // $ANTLR end "HEX_DIGIT"

    // $ANTLR start "EXPONENT"
    public void mEXPONENT() // throws RecognitionException [2]
    {
    		try
    		{
            // Hql.g:825:2: ( ( 'e' ) ( '+' | '-' )? ( '0' .. '9' )+ )
            // Hql.g:825:4: ( 'e' ) ( '+' | '-' )? ( '0' .. '9' )+
            {
            	// Hql.g:825:4: ( 'e' )
            	// Hql.g:825:5: 'e'
            	{
            		Match('e'); if (state.failed) return ;

            	}

            	// Hql.g:825:10: ( '+' | '-' )?
            	int alt21 = 2;
            	int LA21_0 = input.LA(1);

            	if ( (LA21_0 == '+' || LA21_0 == '-') )
            	{
            	    alt21 = 1;
            	}
            	switch (alt21) 
            	{
            	    case 1 :
            	        // Hql.g:
            	        {
            	        	if ( input.LA(1) == '+' || input.LA(1) == '-' ) 
            	        	{
            	        	    input.Consume();
            	        	state.failed = false;
            	        	}
            	        	else 
            	        	{
            	        	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	        	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	        	    Recover(mse);
            	        	    throw mse;}


            	        }
            	        break;

            	}

            	// Hql.g:825:21: ( '0' .. '9' )+
            	int cnt22 = 0;
            	do 
            	{
            	    int alt22 = 2;
            	    int LA22_0 = input.LA(1);

            	    if ( ((LA22_0 >= '0' && LA22_0 <= '9')) )
            	    {
            	        alt22 = 1;
            	    }


            	    switch (alt22) 
            		{
            			case 1 :
            			    // Hql.g:825:22: '0' .. '9'
            			    {
            			    	MatchRange('0','9'); if (state.failed) return ;

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


            }

        }
        finally 
    	{
        }
    }
    // $ANTLR end "EXPONENT"

    // $ANTLR start "FLOAT_SUFFIX"
    public void mFLOAT_SUFFIX() // throws RecognitionException [2]
    {
    		try
    		{
            // Hql.g:830:2: ( 'f' | 'd' | 'm' )
            // Hql.g:
            {
            	if ( input.LA(1) == 'd' || input.LA(1) == 'f' || input.LA(1) == 'm' ) 
            	{
            	    input.Consume();
            	state.failed = false;
            	}
            	else 
            	{
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	    Recover(mse);
            	    throw mse;}


            }

        }
        finally 
    	{
        }
    }
    // $ANTLR end "FLOAT_SUFFIX"

    override public void mTokens() // throws RecognitionException 
    {
        // Hql.g:1:8: ( ALL | ANY | AND | AS | ASCENDING | AVG | BETWEEN | CLASS | COUNT | DELETE | DESCENDING | DISTINCT | ELEMENTS | ESCAPE | EXISTS | FALSE | FETCH | FROM | FULL | GROUP | HAVING | IN | INDICES | INNER | INSERT | INTO | IS | JOIN | LEFT | LIKE | MAX | MIN | NEW | NOT | NULL | OR | ORDER | OUTER | PROPERTIES | RIGHT | SELECT | SET | SKIP | SOME | SUM | TAKE | TRUE | UNION | UPDATE | VERSIONED | WHERE | LITERAL_by | CASE | END | ELSE | THEN | WHEN | ON | WITH | BOTH | EMPTY | LEADING | MEMBER | OBJECT | OF | TRAILING | T__133 | T__134 | EQ | LT | GT | SQL_NE | NE | LE | GE | BOR | BXOR | BAND | BNOT | COMMA | OPEN | CLOSE | OPEN_BRACKET | CLOSE_BRACKET | CONCAT | PLUS | MINUS | STAR | DIV | COLON | PARAM | IDENT | QUOTED_String | WS | NUM_INT )
        int alt23 = 95;
        alt23 = dfa23.Predict(input);
        switch (alt23) 
        {
            case 1 :
                // Hql.g:1:10: ALL
                {
                	mALL(); if (state.failed) return ;

                }
                break;
            case 2 :
                // Hql.g:1:14: ANY
                {
                	mANY(); if (state.failed) return ;

                }
                break;
            case 3 :
                // Hql.g:1:18: AND
                {
                	mAND(); if (state.failed) return ;

                }
                break;
            case 4 :
                // Hql.g:1:22: AS
                {
                	mAS(); if (state.failed) return ;

                }
                break;
            case 5 :
                // Hql.g:1:25: ASCENDING
                {
                	mASCENDING(); if (state.failed) return ;

                }
                break;
            case 6 :
                // Hql.g:1:35: AVG
                {
                	mAVG(); if (state.failed) return ;

                }
                break;
            case 7 :
                // Hql.g:1:39: BETWEEN
                {
                	mBETWEEN(); if (state.failed) return ;

                }
                break;
            case 8 :
                // Hql.g:1:47: CLASS
                {
                	mCLASS(); if (state.failed) return ;

                }
                break;
            case 9 :
                // Hql.g:1:53: COUNT
                {
                	mCOUNT(); if (state.failed) return ;

                }
                break;
            case 10 :
                // Hql.g:1:59: DELETE
                {
                	mDELETE(); if (state.failed) return ;

                }
                break;
            case 11 :
                // Hql.g:1:66: DESCENDING
                {
                	mDESCENDING(); if (state.failed) return ;

                }
                break;
            case 12 :
                // Hql.g:1:77: DISTINCT
                {
                	mDISTINCT(); if (state.failed) return ;

                }
                break;
            case 13 :
                // Hql.g:1:86: ELEMENTS
                {
                	mELEMENTS(); if (state.failed) return ;

                }
                break;
            case 14 :
                // Hql.g:1:95: ESCAPE
                {
                	mESCAPE(); if (state.failed) return ;

                }
                break;
            case 15 :
                // Hql.g:1:102: EXISTS
                {
                	mEXISTS(); if (state.failed) return ;

                }
                break;
            case 16 :
                // Hql.g:1:109: FALSE
                {
                	mFALSE(); if (state.failed) return ;

                }
                break;
            case 17 :
                // Hql.g:1:115: FETCH
                {
                	mFETCH(); if (state.failed) return ;

                }
                break;
            case 18 :
                // Hql.g:1:121: FROM
                {
                	mFROM(); if (state.failed) return ;

                }
                break;
            case 19 :
                // Hql.g:1:126: FULL
                {
                	mFULL(); if (state.failed) return ;

                }
                break;
            case 20 :
                // Hql.g:1:131: GROUP
                {
                	mGROUP(); if (state.failed) return ;

                }
                break;
            case 21 :
                // Hql.g:1:137: HAVING
                {
                	mHAVING(); if (state.failed) return ;

                }
                break;
            case 22 :
                // Hql.g:1:144: IN
                {
                	mIN(); if (state.failed) return ;

                }
                break;
            case 23 :
                // Hql.g:1:147: INDICES
                {
                	mINDICES(); if (state.failed) return ;

                }
                break;
            case 24 :
                // Hql.g:1:155: INNER
                {
                	mINNER(); if (state.failed) return ;

                }
                break;
            case 25 :
                // Hql.g:1:161: INSERT
                {
                	mINSERT(); if (state.failed) return ;

                }
                break;
            case 26 :
                // Hql.g:1:168: INTO
                {
                	mINTO(); if (state.failed) return ;

                }
                break;
            case 27 :
                // Hql.g:1:173: IS
                {
                	mIS(); if (state.failed) return ;

                }
                break;
            case 28 :
                // Hql.g:1:176: JOIN
                {
                	mJOIN(); if (state.failed) return ;

                }
                break;
            case 29 :
                // Hql.g:1:181: LEFT
                {
                	mLEFT(); if (state.failed) return ;

                }
                break;
            case 30 :
                // Hql.g:1:186: LIKE
                {
                	mLIKE(); if (state.failed) return ;

                }
                break;
            case 31 :
                // Hql.g:1:191: MAX
                {
                	mMAX(); if (state.failed) return ;

                }
                break;
            case 32 :
                // Hql.g:1:195: MIN
                {
                	mMIN(); if (state.failed) return ;

                }
                break;
            case 33 :
                // Hql.g:1:199: NEW
                {
                	mNEW(); if (state.failed) return ;

                }
                break;
            case 34 :
                // Hql.g:1:203: NOT
                {
                	mNOT(); if (state.failed) return ;

                }
                break;
            case 35 :
                // Hql.g:1:207: NULL
                {
                	mNULL(); if (state.failed) return ;

                }
                break;
            case 36 :
                // Hql.g:1:212: OR
                {
                	mOR(); if (state.failed) return ;

                }
                break;
            case 37 :
                // Hql.g:1:215: ORDER
                {
                	mORDER(); if (state.failed) return ;

                }
                break;
            case 38 :
                // Hql.g:1:221: OUTER
                {
                	mOUTER(); if (state.failed) return ;

                }
                break;
            case 39 :
                // Hql.g:1:227: PROPERTIES
                {
                	mPROPERTIES(); if (state.failed) return ;

                }
                break;
            case 40 :
                // Hql.g:1:238: RIGHT
                {
                	mRIGHT(); if (state.failed) return ;

                }
                break;
            case 41 :
                // Hql.g:1:244: SELECT
                {
                	mSELECT(); if (state.failed) return ;

                }
                break;
            case 42 :
                // Hql.g:1:251: SET
                {
                	mSET(); if (state.failed) return ;

                }
                break;
            case 43 :
                // Hql.g:1:255: SKIP
                {
                	mSKIP(); if (state.failed) return ;

                }
                break;
            case 44 :
                // Hql.g:1:260: SOME
                {
                	mSOME(); if (state.failed) return ;

                }
                break;
            case 45 :
                // Hql.g:1:265: SUM
                {
                	mSUM(); if (state.failed) return ;

                }
                break;
            case 46 :
                // Hql.g:1:269: TAKE
                {
                	mTAKE(); if (state.failed) return ;

                }
                break;
            case 47 :
                // Hql.g:1:274: TRUE
                {
                	mTRUE(); if (state.failed) return ;

                }
                break;
            case 48 :
                // Hql.g:1:279: UNION
                {
                	mUNION(); if (state.failed) return ;

                }
                break;
            case 49 :
                // Hql.g:1:285: UPDATE
                {
                	mUPDATE(); if (state.failed) return ;

                }
                break;
            case 50 :
                // Hql.g:1:292: VERSIONED
                {
                	mVERSIONED(); if (state.failed) return ;

                }
                break;
            case 51 :
                // Hql.g:1:302: WHERE
                {
                	mWHERE(); if (state.failed) return ;

                }
                break;
            case 52 :
                // Hql.g:1:308: LITERAL_by
                {
                	mLITERAL_by(); if (state.failed) return ;

                }
                break;
            case 53 :
                // Hql.g:1:319: CASE
                {
                	mCASE(); if (state.failed) return ;

                }
                break;
            case 54 :
                // Hql.g:1:324: END
                {
                	mEND(); if (state.failed) return ;

                }
                break;
            case 55 :
                // Hql.g:1:328: ELSE
                {
                	mELSE(); if (state.failed) return ;

                }
                break;
            case 56 :
                // Hql.g:1:333: THEN
                {
                	mTHEN(); if (state.failed) return ;

                }
                break;
            case 57 :
                // Hql.g:1:338: WHEN
                {
                	mWHEN(); if (state.failed) return ;

                }
                break;
            case 58 :
                // Hql.g:1:343: ON
                {
                	mON(); if (state.failed) return ;

                }
                break;
            case 59 :
                // Hql.g:1:346: WITH
                {
                	mWITH(); if (state.failed) return ;

                }
                break;
            case 60 :
                // Hql.g:1:351: BOTH
                {
                	mBOTH(); if (state.failed) return ;

                }
                break;
            case 61 :
                // Hql.g:1:356: EMPTY
                {
                	mEMPTY(); if (state.failed) return ;

                }
                break;
            case 62 :
                // Hql.g:1:362: LEADING
                {
                	mLEADING(); if (state.failed) return ;

                }
                break;
            case 63 :
                // Hql.g:1:370: MEMBER
                {
                	mMEMBER(); if (state.failed) return ;

                }
                break;
            case 64 :
                // Hql.g:1:377: OBJECT
                {
                	mOBJECT(); if (state.failed) return ;

                }
                break;
            case 65 :
                // Hql.g:1:384: OF
                {
                	mOF(); if (state.failed) return ;

                }
                break;
            case 66 :
                // Hql.g:1:387: TRAILING
                {
                	mTRAILING(); if (state.failed) return ;

                }
                break;
            case 67 :
                // Hql.g:1:396: T__133
                {
                	mT__133(); if (state.failed) return ;

                }
                break;
            case 68 :
                // Hql.g:1:403: T__134
                {
                	mT__134(); if (state.failed) return ;

                }
                break;
            case 69 :
                // Hql.g:1:410: EQ
                {
                	mEQ(); if (state.failed) return ;

                }
                break;
            case 70 :
                // Hql.g:1:413: LT
                {
                	mLT(); if (state.failed) return ;

                }
                break;
            case 71 :
                // Hql.g:1:416: GT
                {
                	mGT(); if (state.failed) return ;

                }
                break;
            case 72 :
                // Hql.g:1:419: SQL_NE
                {
                	mSQL_NE(); if (state.failed) return ;

                }
                break;
            case 73 :
                // Hql.g:1:426: NE
                {
                	mNE(); if (state.failed) return ;

                }
                break;
            case 74 :
                // Hql.g:1:429: LE
                {
                	mLE(); if (state.failed) return ;

                }
                break;
            case 75 :
                // Hql.g:1:432: GE
                {
                	mGE(); if (state.failed) return ;

                }
                break;
            case 76 :
                // Hql.g:1:435: BOR
                {
                	mBOR(); if (state.failed) return ;

                }
                break;
            case 77 :
                // Hql.g:1:439: BXOR
                {
                	mBXOR(); if (state.failed) return ;

                }
                break;
            case 78 :
                // Hql.g:1:444: BAND
                {
                	mBAND(); if (state.failed) return ;

                }
                break;
            case 79 :
                // Hql.g:1:449: BNOT
                {
                	mBNOT(); if (state.failed) return ;

                }
                break;
            case 80 :
                // Hql.g:1:454: COMMA
                {
                	mCOMMA(); if (state.failed) return ;

                }
                break;
            case 81 :
                // Hql.g:1:460: OPEN
                {
                	mOPEN(); if (state.failed) return ;

                }
                break;
            case 82 :
                // Hql.g:1:465: CLOSE
                {
                	mCLOSE(); if (state.failed) return ;

                }
                break;
            case 83 :
                // Hql.g:1:471: OPEN_BRACKET
                {
                	mOPEN_BRACKET(); if (state.failed) return ;

                }
                break;
            case 84 :
                // Hql.g:1:484: CLOSE_BRACKET
                {
                	mCLOSE_BRACKET(); if (state.failed) return ;

                }
                break;
            case 85 :
                // Hql.g:1:498: CONCAT
                {
                	mCONCAT(); if (state.failed) return ;

                }
                break;
            case 86 :
                // Hql.g:1:505: PLUS
                {
                	mPLUS(); if (state.failed) return ;

                }
                break;
            case 87 :
                // Hql.g:1:510: MINUS
                {
                	mMINUS(); if (state.failed) return ;

                }
                break;
            case 88 :
                // Hql.g:1:516: STAR
                {
                	mSTAR(); if (state.failed) return ;

                }
                break;
            case 89 :
                // Hql.g:1:521: DIV
                {
                	mDIV(); if (state.failed) return ;

                }
                break;
            case 90 :
                // Hql.g:1:525: COLON
                {
                	mCOLON(); if (state.failed) return ;

                }
                break;
            case 91 :
                // Hql.g:1:531: PARAM
                {
                	mPARAM(); if (state.failed) return ;

                }
                break;
            case 92 :
                // Hql.g:1:537: IDENT
                {
                	mIDENT(); if (state.failed) return ;

                }
                break;
            case 93 :
                // Hql.g:1:543: QUOTED_String
                {
                	mQUOTED_String(); if (state.failed) return ;

                }
                break;
            case 94 :
                // Hql.g:1:557: WS
                {
                	mWS(); if (state.failed) return ;

                }
                break;
            case 95 :
                // Hql.g:1:560: NUM_INT
                {
                	mNUM_INT(); if (state.failed) return ;

                }
                break;

        }

    }

    // $ANTLR start "synpred1_Hql"
    public void synpred1_Hql_fragment() {
        // Hql.g:737:13: ( ESCqs )
        // Hql.g:737:14: ESCqs
        {
        	mESCqs(); if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred1_Hql"

   	public bool synpred1_Hql() 
   	{
   	    state.backtracking++;
   	    int start = input.Mark();
   	    try 
   	    {
   	        synpred1_Hql_fragment(); // can never throw exception
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


    protected DFA23 dfa23;
	private void InitializeCyclicDFAs()
	{
	    this.dfa23 = new DFA23(this);
	}

    const string DFA23_eotS =
        "\x01\uffff\x15\x28\x01\uffff\x01\x63\x01\x65\x01\x67\x01\x68\x01"+
        "\x6a\x10\uffff\x02\x28\x01\x6f\x02\x28\x01\x72\x11\x28\x01\u008a"+
        "\x01\u008b\x09\x28\x01\u0097\x01\x28\x01\u0099\x01\x28\x01\u009b"+
        "\x0e\x28\x0a\uffff\x01\u00ac\x01\u00ad\x01\u00ae\x01\u00b0\x01\uffff"+
        "\x01\u00b1\x01\x28\x01\uffff\x0b\x28\x01\u00be\x0b\x28\x02\uffff"+
        "\x04\x28\x01\u00ce\x01\u00cf\x01\x28\x01\u00d1\x01\u00d2\x02\x28"+
        "\x01\uffff\x01\x28\x01\uffff\x01\x28\x01\uffff\x03\x28\x01\u00da"+
        "\x02\x28\x01\u00dd\x09\x28\x03\uffff\x01\x28\x02\uffff\x01\x28\x01"+
        "\u00ea\x02\x28\x01\u00ed\x01\x28\x01\u00f0\x02\x28\x01\u00f3\x02"+
        "\x28\x01\uffff\x03\x28\x01\u00f9\x01\u00fa\x05\x28\x01\u0100\x01"+
        "\u0101\x01\u0102\x01\x28\x01\u0104\x02\uffff\x01\x28\x02\uffff\x01"+
        "\u0106\x06\x28\x01\uffff\x01\u010d\x01\u010e\x01\uffff\x01\u010f"+
        "\x01\u0110\x01\x28\x01\u0112\x04\x28\x01\u0117\x01\u0118\x02\x28"+
        "\x01\uffff\x01\u011b\x01\u011c\x01\uffff\x02\x28\x01\uffff\x02\x28"+
        "\x01\uffff\x02\x28\x01\u0123\x01\u0124\x01\u0125\x02\uffff\x01\u0126"+
        "\x02\x28\x01\u0129\x01\x28\x03\uffff\x01\x28\x01\uffff\x01\x28\x01"+
        "\uffff\x01\u012d\x01\u012e\x02\x28\x01\u0131\x01\x28\x04\uffff\x01"+
        "\x28\x01\uffff\x01\u0134\x02\x28\x01\u0137\x02\uffff\x02\x28\x02"+
        "\uffff\x01\u013a\x03\x28\x01\u013e\x01\u013f\x04\uffff\x01\u0140"+
        "\x01\x28\x01\uffff\x01\u0142\x01\x28\x01\u0144\x02\uffff\x01\u0145"+
        "\x01\x28\x01\uffff\x01\u0147\x01\x28\x01\uffff\x01\u0149\x01\x28"+
        "\x01\uffff\x01\x28\x01\u014c\x01\uffff\x03\x28\x03\uffff\x01\u0150"+
        "\x01\uffff\x01\u0151\x02\uffff\x01\x28\x01\uffff\x01\x28\x01\uffff"+
        "\x02\x28\x01\uffff\x01\x28\x01\u0157\x01\u0158\x02\uffff\x01\x28"+
        "\x01\u015a\x01\x28\x01\u015c\x01\x28\x02\uffff\x01\x28\x01\uffff"+
        "\x01\u015f\x01\uffff\x01\u0160\x01\u0161\x03\uffff";
    const string DFA23_eofS =
        "\u0162\uffff";
    const string DFA23_minS =
        "\x01\x09\x01\x6c\x01\x65\x01\x61\x01\x65\x01\x6c\x01\x61\x01\x72"+
        "\x01\x61\x01\x6e\x01\x6f\x01\x65\x01\x61\x01\x65\x01\x62\x01\x72"+
        "\x01\x69\x01\x65\x01\x61\x01\x6e\x01\x65\x01\x68\x01\uffff\x04\x3d"+
        "\x01\x7c\x10\uffff\x01\x6c\x01\x64\x01\x24\x01\x67\x01\x74\x01\x24"+
        "\x01\x74\x01\x61\x01\x75\x01\x73\x01\x6c\x01\x73\x01\x65\x01\x63"+
        "\x01\x69\x01\x64\x01\x70\x01\x6c\x01\x74\x01\x6f\x01\x6c\x01\x6f"+
        "\x01\x76\x02\x24\x01\x69\x01\x61\x01\x6b\x01\x78\x01\x6e\x01\x6d"+
        "\x01\x77\x01\x74\x01\x6c\x01\x24\x01\x74\x01\x24\x01\x6a\x01\x24"+
        "\x01\x6f\x01\x67\x01\x6c\x01\x69\x02\x6d\x01\x6b\x01\x61\x01\x65"+
        "\x01\x69\x01\x64\x01\x72\x01\x65\x01\x74\x0a\uffff\x04\x24\x01\uffff"+
        "\x01\x24\x01\x77\x01\uffff\x01\x68\x01\x73\x01\x6e\x02\x65\x01\x63"+
        "\x01\x74\x01\x6d\x01\x65\x01\x61\x01\x73\x01\x24\x01\x74\x01\x73"+
        "\x01\x63\x01\x6d\x01\x6c\x01\x75\x02\x69\x02\x65\x01\x6f\x02\uffff"+
        "\x01\x6e\x01\x74\x01\x64\x01\x65\x02\x24\x01\x62\x02\x24\x01\x6c"+
        "\x01\x65\x01\uffff\x01\x65\x01\uffff\x01\x65\x01\uffff\x01\x70\x01"+
        "\x68\x01\x65\x01\x24\x01\x70\x01\x65\x01\x24\x02\x65\x01\x69\x01"+
        "\x6e\x01\x6f\x01\x61\x01\x73\x01\x6e\x01\x68\x03\uffff\x01\x6e\x02"+
        "\uffff\x01\x65\x01\x24\x01\x73\x01\x74\x01\x24\x01\x74\x01\x24\x01"+
        "\x69\x01\x65\x01\x24\x01\x70\x01\x74\x01\uffff\x01\x79\x01\x65\x01"+
        "\x68\x02\x24\x01\x70\x01\x6e\x01\x63\x02\x72\x03\x24\x01\x69\x01"+
        "\x24\x02\uffff\x01\x65\x02\uffff\x01\x24\x02\x72\x01\x63\x01\x65"+
        "\x01\x74\x01\x63\x01\uffff\x02\x24\x01\uffff\x02\x24\x01\x6c\x01"+
        "\x24\x01\x6e\x01\x74\x01\x69\x01\x65\x02\x24\x01\x64\x01\x65\x01"+
        "\uffff\x02\x24\x01\uffff\x01\x65\x01\x6e\x01\uffff\x02\x6e\x01\uffff"+
        "\x01\x65\x01\x73\x03\x24\x02\uffff\x01\x24\x01\x67\x01\x65\x01\x24"+
        "\x01\x74\x03\uffff\x01\x6e\x01\uffff\x01\x72\x01\uffff\x02\x24\x01"+
        "\x74\x01\x72\x01\x24\x01\x74\x04\uffff\x01\x69\x01\uffff\x01\x24"+
        "\x01\x65\x01\x6f\x01\x24\x02\uffff\x01\x69\x01\x6e\x02\uffff\x01"+
        "\x24\x01\x64\x01\x63\x01\x74\x02\x24\x04\uffff\x01\x24\x01\x73\x01"+
        "\uffff\x01\x24\x01\x67\x01\x24\x02\uffff\x01\x24\x01\x74\x01\uffff"+
        "\x01\x24\x01\x6e\x01\uffff\x01\x24\x01\x6e\x01\uffff\x01\x6e\x01"+
        "\x24\x01\uffff\x01\x69\x01\x74\x01\x73\x03\uffff\x01\x24\x01\uffff"+
        "\x01\x24\x02\uffff\x01\x69\x01\uffff\x01\x67\x01\uffff\x01\x65\x01"+
        "\x67\x01\uffff\x01\x6e\x02\x24\x02\uffff\x01\x65\x01\x24\x01\x64"+
        "\x01\x24\x01\x67\x02\uffff\x01\x73\x01\uffff\x01\x24\x01\uffff\x02"+
        "\x24\x03\uffff";
    const string DFA23_maxS =
        "\x01\ufffe\x01\x76\x01\x79\x01\x6f\x01\x69\x01\x78\x01\x75\x01"+
        "\x72\x01\x61\x01\x73\x01\x6f\x02\x69\x02\x75\x01\x72\x01\x69\x01"+
        "\x75\x01\x72\x01\x70\x01\x65\x01\x69\x01\uffff\x01\x3e\x03\x3d\x01"+
        "\x7c\x10\uffff\x01\x6c\x01\x79\x01\ufffe\x01\x67\x01\x74\x01\ufffe"+
        "\x01\x74\x01\x61\x01\x75\x04\x73\x01\x63\x01\x69\x01\x64\x01\x70"+
        "\x01\x6c\x01\x74\x01\x6f\x01\x6c\x01\x6f\x01\x76\x02\ufffe\x01\x69"+
        "\x01\x66\x01\x6b\x01\x78\x01\x6e\x01\x6d\x01\x77\x01\x74\x01\x6c"+
        "\x01\ufffe\x01\x74\x01\ufffe\x01\x6a\x01\ufffe\x01\x6f\x01\x67\x01"+
        "\x74\x01\x69\x02\x6d\x01\x6b\x01\x75\x01\x65\x01\x69\x01\x64\x01"+
        "\x72\x01\x65\x01\x74\x0a\uffff\x04\ufffe\x01\uffff\x01\ufffe\x01"+
        "\x77\x01\uffff\x01\x68\x01\x73\x01\x6e\x02\x65\x01\x63\x01\x74\x01"+
        "\x6d\x01\x65\x01\x61\x01\x73\x01\ufffe\x01\x74\x01\x73\x01\x63\x01"+
        "\x6d\x01\x6c\x01\x75\x02\x69\x02\x65\x01\x6f\x02\uffff\x01\x6e\x01"+
        "\x74\x01\x64\x01\x65\x02\ufffe\x01\x62\x02\ufffe\x01\x6c\x01\x65"+
        "\x01\uffff\x01\x65\x01\uffff\x01\x65\x01\uffff\x01\x70\x01\x68\x01"+
        "\x65\x01\ufffe\x01\x70\x01\x65\x01\ufffe\x02\x65\x01\x69\x01\x6e"+
        "\x01\x6f\x01\x61\x01\x73\x01\x72\x01\x68\x03\uffff\x01\x6e\x02\uffff"+
        "\x01\x65\x01\ufffe\x01\x73\x01\x74\x01\ufffe\x01\x74\x01\ufffe\x01"+
        "\x69\x01\x65\x01\ufffe\x01\x70\x01\x74\x01\uffff\x01\x79\x01\x65"+
        "\x01\x68\x02\ufffe\x01\x70\x01\x6e\x01\x63\x02\x72\x03\ufffe\x01"+
        "\x69\x01\ufffe\x02\uffff\x01\x65\x02\uffff\x01\ufffe\x02\x72\x01"+
        "\x63\x01\x65\x01\x74\x01\x63\x01\uffff\x02\ufffe\x01\uffff\x02\ufffe"+
        "\x01\x6c\x01\ufffe\x01\x6e\x01\x74\x01\x69\x01\x65\x02\ufffe\x01"+
        "\x64\x01\x65\x01\uffff\x02\ufffe\x01\uffff\x01\x65\x01\x6e\x01\uffff"+
        "\x02\x6e\x01\uffff\x01\x65\x01\x73\x03\ufffe\x02\uffff\x01\ufffe"+
        "\x01\x67\x01\x65\x01\ufffe\x01\x74\x03\uffff\x01\x6e\x01\uffff\x01"+
        "\x72\x01\uffff\x02\ufffe\x01\x74\x01\x72\x01\ufffe\x01\x74\x04\uffff"+
        "\x01\x69\x01\uffff\x01\ufffe\x01\x65\x01\x6f\x01\ufffe\x02\uffff"+
        "\x01\x69\x01\x6e\x02\uffff\x01\ufffe\x01\x64\x01\x63\x01\x74\x02"+
        "\ufffe\x04\uffff\x01\ufffe\x01\x73\x01\uffff\x01\ufffe\x01\x67\x01"+
        "\ufffe\x02\uffff\x01\ufffe\x01\x74\x01\uffff\x01\ufffe\x01\x6e\x01"+
        "\uffff\x01\ufffe\x01\x6e\x01\uffff\x01\x6e\x01\ufffe\x01\uffff\x01"+
        "\x69\x01\x74\x01\x73\x03\uffff\x01\ufffe\x01\uffff\x01\ufffe\x02"+
        "\uffff\x01\x69\x01\uffff\x01\x67\x01\uffff\x01\x65\x01\x67\x01\uffff"+
        "\x01\x6e\x02\ufffe\x02\uffff\x01\x65\x01\ufffe\x01\x64\x01\ufffe"+
        "\x01\x67\x02\uffff\x01\x73\x01\uffff\x01\ufffe\x01\uffff\x02\ufffe"+
        "\x03\uffff";
    const string DFA23_acceptS =
        "\x16\uffff\x01\x45\x05\uffff\x01\x4e\x01\x50\x01\x51\x01\x52\x01"+
        "\x53\x01\x54\x01\x56\x01\x57\x01\x58\x01\x59\x01\x5a\x01\x5b\x01"+
        "\x5c\x01\x5d\x01\x5e\x01\x5f\x35\uffff\x01\x48\x01\x4a\x01\x46\x01"+
        "\x4b\x01\x47\x01\x49\x01\x4f\x01\x4d\x01\x55\x01\x4c\x04\uffff\x01"+
        "\x04\x02\uffff\x01\x34\x17\uffff\x01\x16\x01\x1b\x0b\uffff\x01\x24"+
        "\x01\uffff\x01\x3a\x01\uffff\x01\x41\x10\uffff\x01\x01\x01\x02\x01"+
        "\x03\x01\uffff\x01\x05\x01\x06\x0c\uffff\x01\x36\x0f\uffff\x01\x1f"+
        "\x01\x20\x01\uffff\x01\x21\x01\x22\x07\uffff\x01\x2a\x02\uffff\x01"+
        "\x2d\x0c\uffff\x01\x3c\x02\uffff\x01\x35\x02\uffff\x01\x0b\x02\uffff"+
        "\x01\x37\x05\uffff\x01\x12\x01\x13\x05\uffff\x01\x1a\x01\x1c\x01"+
        "\x1d\x01\uffff\x01\x1e\x01\uffff\x01\x23\x06\uffff\x01\x2b\x01\x2c"+
        "\x01\x2e\x01\x2f\x01\uffff\x01\x38\x04\uffff\x01\x39\x01\x3b\x02"+
        "\uffff\x01\x08\x01\x09\x06\uffff\x01\x3d\x01\x10\x01\x11\x01\x14"+
        "\x02\uffff\x01\x18\x03\uffff\x01\x25\x01\x26\x02\uffff\x01\x28\x02"+
        "\uffff\x01\x30\x02\uffff\x01\x33\x02\uffff\x01\x0a\x03\uffff\x01"+
        "\x0e\x01\x0f\x01\x15\x01\uffff\x01\x19\x01\uffff\x01\x3f\x01\x40"+
        "\x01\uffff\x01\x29\x01\uffff\x01\x31\x02\uffff\x01\x07\x03\uffff"+
        "\x01\x17\x01\x3e\x05\uffff\x01\x0c\x01\x0d\x01\uffff\x01\x42\x01"+
        "\uffff\x01\x43\x02\uffff\x01\x32\x01\x44\x01\x27";
    const string DFA23_specialS =
        "\u0162\uffff}>";
    static readonly string[] DFA23_transitionS = {
            "\x02\x2a\x02\uffff\x01\x2a\x12\uffff\x01\x2a\x01\x19\x02\uffff"+
            "\x01\x28\x01\uffff\x01\x1c\x01\x29\x01\x1e\x01\x1f\x01\x24\x01"+
            "\x22\x01\x1d\x01\x23\x01\x2b\x01\x25\x0a\x2b\x01\x26\x01\uffff"+
            "\x01\x17\x01\x16\x01\x18\x01\x27\x01\uffff\x1a\x28\x01\x20\x01"+
            "\uffff\x01\x21\x01\x1a\x01\x28\x01\uffff\x01\x01\x01\x02\x01"+
            "\x03\x01\x04\x01\x05\x01\x06\x01\x07\x01\x08\x01\x09\x01\x0a"+
            "\x01\x28\x01\x0b\x01\x0c\x01\x0d\x01\x0e\x01\x0f\x01\x28\x01"+
            "\x10\x01\x11\x01\x12\x01\x13\x01\x14\x01\x15\x03\x28\x01\uffff"+
            "\x01\x1b\x03\uffff\uff7f\x28",
            "\x01\x2c\x01\uffff\x01\x2d\x04\uffff\x01\x2e\x02\uffff\x01"+
            "\x2f",
            "\x01\x30\x09\uffff\x01\x32\x09\uffff\x01\x31",
            "\x01\x35\x0a\uffff\x01\x33\x02\uffff\x01\x34",
            "\x01\x36\x03\uffff\x01\x37",
            "\x01\x38\x01\x3c\x01\x3b\x04\uffff\x01\x39\x04\uffff\x01\x3a",
            "\x01\x3d\x03\uffff\x01\x3e\x0c\uffff\x01\x3f\x02\uffff\x01"+
            "\x40",
            "\x01\x41",
            "\x01\x42",
            "\x01\x43\x04\uffff\x01\x44",
            "\x01\x45",
            "\x01\x46\x03\uffff\x01\x47",
            "\x01\x48\x03\uffff\x01\x4a\x03\uffff\x01\x49",
            "\x01\x4b\x09\uffff\x01\x4c\x05\uffff\x01\x4d",
            "\x01\x51\x03\uffff\x01\x52\x07\uffff\x01\x50\x03\uffff\x01"+
            "\x4e\x02\uffff\x01\x4f",
            "\x01\x53",
            "\x01\x54",
            "\x01\x55\x05\uffff\x01\x56\x03\uffff\x01\x57\x05\uffff\x01"+
            "\x58",
            "\x01\x59\x06\uffff\x01\x5b\x09\uffff\x01\x5a",
            "\x01\x5c\x01\uffff\x01\x5d",
            "\x01\x5e",
            "\x01\x5f\x01\x60",
            "",
            "\x01\x62\x01\x61",
            "\x01\x64",
            "\x01\x66",
            "\x01\x66",
            "\x01\x69",
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
            "\x01\x6b",
            "\x01\x6d\x14\uffff\x01\x6c",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x02\x28\x01\x6e\x17\x28\x05\uffff\uff7f\x28",
            "\x01\x70",
            "\x01\x71",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\x73",
            "\x01\x74",
            "\x01\x75",
            "\x01\x76",
            "\x01\x77\x06\uffff\x01\x78",
            "\x01\x79",
            "\x01\x7a\x0d\uffff\x01\x7b",
            "\x01\x7c",
            "\x01\x7d",
            "\x01\x7e",
            "\x01\x7f",
            "\x01\u0080",
            "\x01\u0081",
            "\x01\u0082",
            "\x01\u0083",
            "\x01\u0084",
            "\x01\u0085",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x03\x28\x01\u0086\x09\x28\x01\u0087\x04\x28\x01"+
            "\u0088\x01\u0089\x06\x28\x05\uffff\uff7f\x28",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\u008c",
            "\x01\u008e\x04\uffff\x01\u008d",
            "\x01\u008f",
            "\x01\u0090",
            "\x01\u0091",
            "\x01\u0092",
            "\x01\u0093",
            "\x01\u0094",
            "\x01\u0095",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x03\x28\x01\u0096\x16\x28\x05\uffff\uff7f\x28",
            "\x01\u0098",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\u009a",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\u009c",
            "\x01\u009d",
            "\x01\u009e\x07\uffff\x01\u009f",
            "\x01\u00a0",
            "\x01\u00a1",
            "\x01\u00a2",
            "\x01\u00a3",
            "\x01\u00a5\x13\uffff\x01\u00a4",
            "\x01\u00a6",
            "\x01\u00a7",
            "\x01\u00a8",
            "\x01\u00a9",
            "\x01\u00aa",
            "\x01\u00ab",
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
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x04\x28\x01\u00af\x15\x28\x05\uffff\uff7f\x28",
            "",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\u00b2",
            "",
            "\x01\u00b3",
            "\x01\u00b4",
            "\x01\u00b5",
            "\x01\u00b6",
            "\x01\u00b7",
            "\x01\u00b8",
            "\x01\u00b9",
            "\x01\u00ba",
            "\x01\u00bb",
            "\x01\u00bc",
            "\x01\u00bd",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\u00bf",
            "\x01\u00c0",
            "\x01\u00c1",
            "\x01\u00c2",
            "\x01\u00c3",
            "\x01\u00c4",
            "\x01\u00c5",
            "\x01\u00c6",
            "\x01\u00c7",
            "\x01\u00c8",
            "\x01\u00c9",
            "",
            "",
            "\x01\u00ca",
            "\x01\u00cb",
            "\x01\u00cc",
            "\x01\u00cd",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\u00d0",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\u00d3",
            "\x01\u00d4",
            "",
            "\x01\u00d5",
            "",
            "\x01\u00d6",
            "",
            "\x01\u00d7",
            "\x01\u00d8",
            "\x01\u00d9",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\u00db",
            "\x01\u00dc",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\u00de",
            "\x01\u00df",
            "\x01\u00e0",
            "\x01\u00e1",
            "\x01\u00e2",
            "\x01\u00e3",
            "\x01\u00e4",
            "\x01\u00e6\x03\uffff\x01\u00e5",
            "\x01\u00e7",
            "",
            "",
            "",
            "\x01\u00e8",
            "",
            "",
            "\x01\u00e9",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\u00eb",
            "\x01\u00ec",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\u00ee",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x04\x28\x01\u00ef\x15\x28\x05\uffff\uff7f\x28",
            "\x01\u00f1",
            "\x01\u00f2",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\u00f4",
            "\x01\u00f5",
            "",
            "\x01\u00f6",
            "\x01\u00f7",
            "\x01\u00f8",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\u00fb",
            "\x01\u00fc",
            "\x01\u00fd",
            "\x01\u00fe",
            "\x01\u00ff",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\u0103",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "",
            "",
            "\x01\u0105",
            "",
            "",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\u0107",
            "\x01\u0108",
            "\x01\u0109",
            "\x01\u010a",
            "\x01\u010b",
            "\x01\u010c",
            "",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\u0111",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\u0113",
            "\x01\u0114",
            "\x01\u0115",
            "\x01\u0116",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\u0119",
            "\x01\u011a",
            "",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "",
            "\x01\u011d",
            "\x01\u011e",
            "",
            "\x01\u011f",
            "\x01\u0120",
            "",
            "\x01\u0121",
            "\x01\u0122",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "",
            "",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\u0127",
            "\x01\u0128",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\u012a",
            "",
            "",
            "",
            "\x01\u012b",
            "",
            "\x01\u012c",
            "",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\u012f",
            "\x01\u0130",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\u0132",
            "",
            "",
            "",
            "",
            "\x01\u0133",
            "",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\u0135",
            "\x01\u0136",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "",
            "",
            "\x01\u0138",
            "\x01\u0139",
            "",
            "",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\u013b",
            "\x01\u013c",
            "\x01\u013d",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "",
            "",
            "",
            "",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\u0141",
            "",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\u0143",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "",
            "",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\u0146",
            "",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\u0148",
            "",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\u014a",
            "",
            "\x01\u014b",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "",
            "\x01\u014d",
            "\x01\u014e",
            "\x01\u014f",
            "",
            "",
            "",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "",
            "",
            "\x01\u0152",
            "",
            "\x01\u0153",
            "",
            "\x01\u0154",
            "\x01\u0155",
            "",
            "\x01\u0156",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "",
            "",
            "\x01\u0159",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\u015b",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\u015d",
            "",
            "",
            "\x01\u015e",
            "",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "\x01\x28\x0b\uffff\x0a\x28\x07\uffff\x1a\x28\x04\uffff\x01"+
            "\x28\x01\uffff\x1a\x28\x05\uffff\uff7f\x28",
            "",
            "",
            ""
    };

    static readonly short[] DFA23_eot = DFA.UnpackEncodedString(DFA23_eotS);
    static readonly short[] DFA23_eof = DFA.UnpackEncodedString(DFA23_eofS);
    static readonly char[] DFA23_min = DFA.UnpackEncodedStringToUnsignedChars(DFA23_minS);
    static readonly char[] DFA23_max = DFA.UnpackEncodedStringToUnsignedChars(DFA23_maxS);
    static readonly short[] DFA23_accept = DFA.UnpackEncodedString(DFA23_acceptS);
    static readonly short[] DFA23_special = DFA.UnpackEncodedString(DFA23_specialS);
    static readonly short[][] DFA23_transition = DFA.UnpackEncodedStringArray(DFA23_transitionS);

    protected class DFA23 : DFA
    {
        public DFA23(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 23;
            this.eot = DFA23_eot;
            this.eof = DFA23_eof;
            this.min = DFA23_min;
            this.max = DFA23_max;
            this.accept = DFA23_accept;
            this.special = DFA23_special;
            this.transition = DFA23_transition;

        }

        override public string Description
        {
            get { return "1:1: Tokens : ( ALL | ANY | AND | AS | ASCENDING | AVG | BETWEEN | CLASS | COUNT | DELETE | DESCENDING | DISTINCT | ELEMENTS | ESCAPE | EXISTS | FALSE | FETCH | FROM | FULL | GROUP | HAVING | IN | INDICES | INNER | INSERT | INTO | IS | JOIN | LEFT | LIKE | MAX | MIN | NEW | NOT | NULL | OR | ORDER | OUTER | PROPERTIES | RIGHT | SELECT | SET | SKIP | SOME | SUM | TAKE | TRUE | UNION | UPDATE | VERSIONED | WHERE | LITERAL_by | CASE | END | ELSE | THEN | WHEN | ON | WITH | BOTH | EMPTY | LEADING | MEMBER | OBJECT | OF | TRAILING | T__133 | T__134 | EQ | LT | GT | SQL_NE | NE | LE | GE | BOR | BXOR | BAND | BNOT | COMMA | OPEN | CLOSE | OPEN_BRACKET | CLOSE_BRACKET | CONCAT | PLUS | MINUS | STAR | DIV | COLON | PARAM | IDENT | QUOTED_String | WS | NUM_INT );"; }
        }

    }

 
    
}
}