// $ANTLR 3.1.2 /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g 2009-04-17 11:12:37

// The variable 'variable' is assigned but its value is never used.
#pragma warning disable 168, 219
// Unreachable code detected.
#pragma warning disable 162
namespace  NHibernate.Hql.Ast.ANTLR 
{

using System;
using Antlr.Runtime;
using IList 		= System.Collections.IList;
using ArrayList 	= System.Collections.ArrayList;
using Stack 		= Antlr.Runtime.Collections.StackList;

using IDictionary	= System.Collections.IDictionary;
using Hashtable 	= System.Collections.Hashtable;

public partial class HqlLexer : Lexer {
    public const int EXPR_LIST = 73;
    public const int EXISTS = 19;
    public const int COMMA = 98;
    public const int FETCH = 21;
    public const int MINUS = 110;
    public const int AS = 7;
    public const int END = 56;
    public const int INTO = 30;
    public const int FALSE = 20;
    public const int ELEMENTS = 17;
    public const int THEN = 58;
    public const int ALIAS = 70;
    public const int ON = 60;
    public const int DOT = 15;
    public const int ORDER = 41;
    public const int AND = 6;
    public const int CONSTANT = 92;
    public const int UNARY_MINUS = 88;
    public const int METHOD_CALL = 79;
    public const int RIGHT = 44;
    public const int CONCAT = 108;
    public const int PROPERTIES = 43;
    public const int SELECT = 45;
    public const int LE = 106;
    public const int BETWEEN = 10;
    public const int NUM_INT = 93;
    public const int BOTH = 62;
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
    public const int OR = 40;
    public const int FULL = 23;
    public const int INDICES = 27;
    public const int IS_NULL = 78;
    public const int GROUP = 24;
    public const int ESCAPE = 18;
    public const int T__127 = 127;
    public const int PARAM = 116;
    public const int ID_LETTER = 120;
    public const int INDEX_OP = 76;
    public const int HEX_DIGIT = 125;
    public const int LEFT = 33;
    public const int TRAILING = 68;
    public const int JOIN = 32;
    public const int NOT_BETWEEN = 80;
    public const int SUM = 48;
    public const int ROW_STAR = 86;
    public const int OUTER = 42;
    public const int NOT_IN = 81;
    public const int FROM = 22;
    public const int DELETE = 13;
    public const int OBJECT = 66;
    public const int MAX = 35;
    public const int NOT_LIKE = 82;
    public const int EMPTY = 63;
    public const int QUOTED_String = 117;
    public const int ASCENDING = 8;
    public const int NUM_LONG = 96;
    public const int IS = 31;
    public const int SQL_NE = 103;
    public const int IN_LIST = 75;
    public const int WEIRD_IDENT = 91;
    public const int NE = 102;
    public const int GT = 105;
    public const int MIN = 36;
    public const int LIKE = 34;
    public const int WITH = 61;
    public const int IN = 26;
    public const int CONSTRUCTOR = 71;
    public const int SOME = 47;
    public const int CLASS = 11;
    public const int EXPONENT = 123;
    public const int ID_START_LETTER = 119;
    public const int EOF = -1;
    public const int CLOSE = 101;
    public const int AVG = 9;
    public const int STAR = 111;
    public const int NOT = 38;
    public const int JAVA_CONSTANT = 97;

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
    	get { return "/Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g";} 
    }

    // $ANTLR start "ALL"
    public void mALL() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = ALL;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:9:5: ( 'all' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:9:7: 'all'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:10:5: ( 'any' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:10:7: 'any'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:11:5: ( 'and' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:11:7: 'and'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:12:4: ( 'as' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:12:6: 'as'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:13:11: ( 'asc' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:13:13: 'asc'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:14:5: ( 'avg' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:14:7: 'avg'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:15:9: ( 'between' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:15:11: 'between'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:16:7: ( 'class' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:16:9: 'class'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:17:7: ( 'count' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:17:9: 'count'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:18:8: ( 'delete' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:18:10: 'delete'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:19:12: ( 'desc' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:19:14: 'desc'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:20:10: ( 'distinct' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:20:12: 'distinct'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:21:10: ( 'elements' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:21:12: 'elements'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:22:8: ( 'escape' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:22:10: 'escape'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:23:8: ( 'exists' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:23:10: 'exists'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:24:7: ( 'false' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:24:9: 'false'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:25:7: ( 'fetch' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:25:9: 'fetch'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:26:6: ( 'from' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:26:8: 'from'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:27:6: ( 'full' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:27:8: 'full'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:28:7: ( 'group' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:28:9: 'group'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:29:8: ( 'having' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:29:10: 'having'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:30:4: ( 'in' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:30:6: 'in'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:31:9: ( 'indices' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:31:11: 'indices'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:32:7: ( 'inner' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:32:9: 'inner'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:33:8: ( 'insert' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:33:10: 'insert'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:34:6: ( 'into' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:34:8: 'into'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:35:4: ( 'is' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:35:6: 'is'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:36:6: ( 'join' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:36:8: 'join'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:37:6: ( 'left' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:37:8: 'left'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:38:6: ( 'like' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:38:8: 'like'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:39:5: ( 'max' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:39:7: 'max'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:40:5: ( 'min' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:40:7: 'min'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:41:5: ( 'new' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:41:7: 'new'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:42:5: ( 'not' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:42:7: 'not'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:43:6: ( 'null' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:43:8: 'null'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:44:4: ( 'or' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:44:6: 'or'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:45:7: ( 'order' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:45:9: 'order'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:46:7: ( 'outer' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:46:9: 'outer'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:47:12: ( 'properties' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:47:14: 'properties'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:48:7: ( 'right' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:48:9: 'right'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:49:8: ( 'select' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:49:10: 'select'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:50:5: ( 'set' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:50:7: 'set'
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

    // $ANTLR start "SOME"
    public void mSOME() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = SOME;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:51:6: ( 'some' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:51:8: 'some'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:52:5: ( 'sum' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:52:7: 'sum'
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

    // $ANTLR start "TRUE"
    public void mTRUE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = TRUE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:53:6: ( 'true' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:53:8: 'true'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:54:7: ( 'union' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:54:9: 'union'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:55:8: ( 'update' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:55:10: 'update'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:56:11: ( 'versioned' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:56:13: 'versioned'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:57:7: ( 'where' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:57:9: 'where'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:58:12: ( 'by' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:58:14: 'by'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:59:6: ( 'case' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:59:8: 'case'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:60:5: ( 'end' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:60:7: 'end'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:61:6: ( 'else' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:61:8: 'else'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:62:6: ( 'then' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:62:8: 'then'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:63:6: ( 'when' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:63:8: 'when'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:64:4: ( 'on' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:64:6: 'on'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:65:6: ( 'with' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:65:8: 'with'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:66:6: ( 'both' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:66:8: 'both'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:67:7: ( 'empty' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:67:9: 'empty'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:68:9: ( 'leading' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:68:11: 'leading'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:69:8: ( 'member' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:69:10: 'member'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:70:8: ( 'object' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:70:10: 'object'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:71:4: ( 'of' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:71:6: 'of'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:72:10: ( 'trailing' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:72:12: 'trailing'
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

    // $ANTLR start "T__126"
    public void mT__126() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__126;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:73:8: ( 'ascending' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:73:10: 'ascending'
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
    // $ANTLR end "T__126"

    // $ANTLR start "T__127"
    public void mT__127() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__127;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:74:8: ( 'descending' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:74:10: 'descending'
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
    // $ANTLR end "T__127"

    // $ANTLR start "EQ"
    public void mEQ() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = EQ;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:744:3: ( '=' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:744:5: '='
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:745:3: ( '<' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:745:5: '<'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:746:3: ( '>' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:746:5: '>'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:747:7: ( '<>' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:747:9: '<>'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:748:3: ( '!=' | '^=' )
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
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:748:5: '!='
                    {
                    	Match("!="); if (state.failed) return ;


                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:748:12: '^='
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:749:3: ( '<=' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:749:5: '<='
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:750:3: ( '>=' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:750:5: '>='
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

    // $ANTLR start "COMMA"
    public void mCOMMA() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = COMMA;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:752:6: ( ',' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:752:8: ','
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:754:5: ( '(' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:754:7: '('
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:755:6: ( ')' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:755:8: ')'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:756:13: ( '[' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:756:15: '['
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:757:14: ( ']' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:757:16: ']'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:759:7: ( '||' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:759:9: '||'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:760:5: ( '+' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:760:7: '+'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:761:6: ( '-' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:761:8: '-'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:762:5: ( '*' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:762:7: '*'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:763:4: ( '/' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:763:6: '/'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:764:6: ( ':' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:764:8: ':'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:765:6: ( '?' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:765:8: '?'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:768:2: ( ID_START_LETTER ( ID_LETTER )* )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:768:4: ID_START_LETTER ( ID_LETTER )*
            {
            	mID_START_LETTER(); if (state.failed) return ;
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:768:20: ( ID_LETTER )*
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
            			    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:768:22: ID_LETTER
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:773:5: ( '_' | '$' | 'a' .. 'z' | 'A' .. 'Z' | '\\u0080' .. '\\ufffe' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:782:5: ( ID_START_LETTER | '0' .. '9' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:787:4: ( '\\'' ( ( ESCqs )=> ESCqs | ~ '\\'' )* '\\'' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:787:6: '\\'' ( ( ESCqs )=> ESCqs | ~ '\\'' )* '\\''
            {
            	Match('\''); if (state.failed) return ;
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:787:11: ( ( ESCqs )=> ESCqs | ~ '\\'' )*
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
            			    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:787:13: ( ESCqs )=> ESCqs
            			    {
            			    	mESCqs(); if (state.failed) return ;

            			    }
            			    break;
            			case 2 :
            			    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:787:31: ~ '\\''
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:792:2: ( '\\'' '\\'' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:793:3: '\\'' '\\''
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:796:5: ( ( ' ' | '\\t' | '\\r' '\\n' | '\\n' | '\\r' ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:796:9: ( ' ' | '\\t' | '\\r' '\\n' | '\\n' | '\\r' )
            {
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:796:9: ( ' ' | '\\t' | '\\r' '\\n' | '\\n' | '\\r' )
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
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:796:13: ' '
            	        {
            	        	Match(' '); if (state.failed) return ;

            	        }
            	        break;
            	    case 2 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:797:7: '\\t'
            	        {
            	        	Match('\t'); if (state.failed) return ;

            	        }
            	        break;
            	    case 3 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:798:7: '\\r' '\\n'
            	        {
            	        	Match('\r'); if (state.failed) return ;
            	        	Match('\n'); if (state.failed) return ;

            	        }
            	        break;
            	    case 4 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:799:7: '\\n'
            	        {
            	        	Match('\n'); if (state.failed) return ;

            	        }
            	        break;
            	    case 5 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:800:7: '\\r'
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:809:2: ( '.' ( ( '0' .. '9' )+ ( EXPONENT )? (f1= FLOAT_SUFFIX )? )? | ( '0' ( ( 'x' ) ( HEX_DIGIT )+ | ( '0' .. '7' )+ )? | ( '1' .. '9' ) ( '0' .. '9' )* ) ( ( 'l' ) | {...}? ( '.' ( '0' .. '9' )* ( EXPONENT )? (f2= FLOAT_SUFFIX )? | EXPONENT (f3= FLOAT_SUFFIX )? | f4= FLOAT_SUFFIX ) )? )
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
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:809:6: '.' ( ( '0' .. '9' )+ ( EXPONENT )? (f1= FLOAT_SUFFIX )? )?
                    {
                    	Match('.'); if (state.failed) return ;
                    	if ( (state.backtracking==0) )
                    	{
                    	  _type = DOT;
                    	}
                    	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:810:4: ( ( '0' .. '9' )+ ( EXPONENT )? (f1= FLOAT_SUFFIX )? )?
                    	int alt8 = 2;
                    	int LA8_0 = input.LA(1);

                    	if ( ((LA8_0 >= '0' && LA8_0 <= '9')) )
                    	{
                    	    alt8 = 1;
                    	}
                    	switch (alt8) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:810:6: ( '0' .. '9' )+ ( EXPONENT )? (f1= FLOAT_SUFFIX )?
                    	        {
                    	        	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:810:6: ( '0' .. '9' )+
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
                    	        			    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:810:7: '0' .. '9'
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
                    	        		;	// Stops C# compiler whinging that label 'loop5' has no statements

                    	        	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:810:18: ( EXPONENT )?
                    	        	int alt6 = 2;
                    	        	int LA6_0 = input.LA(1);

                    	        	if ( (LA6_0 == 'e') )
                    	        	{
                    	        	    alt6 = 1;
                    	        	}
                    	        	switch (alt6) 
                    	        	{
                    	        	    case 1 :
                    	        	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:810:19: EXPONENT
                    	        	        {
                    	        	        	mEXPONENT(); if (state.failed) return ;

                    	        	        }
                    	        	        break;

                    	        	}

                    	        	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:810:30: (f1= FLOAT_SUFFIX )?
                    	        	int alt7 = 2;
                    	        	int LA7_0 = input.LA(1);

                    	        	if ( (LA7_0 == 'd' || LA7_0 == 'f') )
                    	        	{
                    	        	    alt7 = 1;
                    	        	}
                    	        	switch (alt7) 
                    	        	{
                    	        	    case 1 :
                    	        	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:810:31: f1= FLOAT_SUFFIX
                    	        	        {
                    	        	        	int f1Start992 = CharIndex;
                    	        	        	mFLOAT_SUFFIX(); if (state.failed) return ;
                    	        	        	f1 = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, f1Start992, CharIndex-1);
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
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:822:4: ( '0' ( ( 'x' ) ( HEX_DIGIT )+ | ( '0' .. '7' )+ )? | ( '1' .. '9' ) ( '0' .. '9' )* ) ( ( 'l' ) | {...}? ( '.' ( '0' .. '9' )* ( EXPONENT )? (f2= FLOAT_SUFFIX )? | EXPONENT (f3= FLOAT_SUFFIX )? | f4= FLOAT_SUFFIX ) )?
                    {
                    	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:822:4: ( '0' ( ( 'x' ) ( HEX_DIGIT )+ | ( '0' .. '7' )+ )? | ( '1' .. '9' ) ( '0' .. '9' )* )
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
                    	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:822:6: '0' ( ( 'x' ) ( HEX_DIGIT )+ | ( '0' .. '7' )+ )?
                    	        {
                    	        	Match('0'); if (state.failed) return ;
                    	        	if ( (state.backtracking==0) )
                    	        	{
                    	        	  isDecimal = true;
                    	        	}
                    	        	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:823:4: ( ( 'x' ) ( HEX_DIGIT )+ | ( '0' .. '7' )+ )?
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
                    	        	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:823:6: ( 'x' ) ( HEX_DIGIT )+
                    	        	        {
                    	        	        	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:823:6: ( 'x' )
                    	        	        	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:823:7: 'x'
                    	        	        	{
                    	        	        		Match('x'); if (state.failed) return ;

                    	        	        	}

                    	        	        	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:824:5: ( HEX_DIGIT )+
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
                    	        	        			    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:831:7: HEX_DIGIT
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
                    	        	        		;	// Stops C# compiler whinging that label 'loop9' has no statements


                    	        	        }
                    	        	        break;
                    	        	    case 2 :
                    	        	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:833:6: ( '0' .. '7' )+
                    	        	        {
                    	        	        	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:833:6: ( '0' .. '7' )+
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
                    	        	        			    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:833:7: '0' .. '7'
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
                    	        	        		;	// Stops C# compiler whinging that label 'loop10' has no statements


                    	        	        }
                    	        	        break;

                    	        	}


                    	        }
                    	        break;
                    	    case 2 :
                    	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:835:5: ( '1' .. '9' ) ( '0' .. '9' )*
                    	        {
                    	        	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:835:5: ( '1' .. '9' )
                    	        	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:835:6: '1' .. '9'
                    	        	{
                    	        		MatchRange('1','9'); if (state.failed) return ;

                    	        	}

                    	        	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:835:16: ( '0' .. '9' )*
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
                    	        			    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:835:17: '0' .. '9'
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

                    	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:837:3: ( ( 'l' ) | {...}? ( '.' ( '0' .. '9' )* ( EXPONENT )? (f2= FLOAT_SUFFIX )? | EXPONENT (f3= FLOAT_SUFFIX )? | f4= FLOAT_SUFFIX ) )?
                    	int alt19 = 3;
                    	int LA19_0 = input.LA(1);

                    	if ( (LA19_0 == 'l') )
                    	{
                    	    alt19 = 1;
                    	}
                    	else if ( (LA19_0 == '.' || (LA19_0 >= 'd' && LA19_0 <= 'f')) )
                    	{
                    	    alt19 = 2;
                    	}
                    	switch (alt19) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:837:5: ( 'l' )
                    	        {
                    	        	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:837:5: ( 'l' )
                    	        	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:837:6: 'l'
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
                    	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:840:5: {...}? ( '.' ( '0' .. '9' )* ( EXPONENT )? (f2= FLOAT_SUFFIX )? | EXPONENT (f3= FLOAT_SUFFIX )? | f4= FLOAT_SUFFIX )
                    	        {
                    	        	if ( !((isDecimal)) ) 
                    	        	{
                    	        	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	        	    throw new FailedPredicateException(input, "NUM_INT", "isDecimal");
                    	        	}
                    	        	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:841:4: ( '.' ( '0' .. '9' )* ( EXPONENT )? (f2= FLOAT_SUFFIX )? | EXPONENT (f3= FLOAT_SUFFIX )? | f4= FLOAT_SUFFIX )
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
                    	        	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:841:8: '.' ( '0' .. '9' )* ( EXPONENT )? (f2= FLOAT_SUFFIX )?
                    	        	        {
                    	        	        	Match('.'); if (state.failed) return ;
                    	        	        	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:841:12: ( '0' .. '9' )*
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
                    	        	        			    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:841:13: '0' .. '9'
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

                    	        	        	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:841:24: ( EXPONENT )?
                    	        	        	int alt15 = 2;
                    	        	        	int LA15_0 = input.LA(1);

                    	        	        	if ( (LA15_0 == 'e') )
                    	        	        	{
                    	        	        	    alt15 = 1;
                    	        	        	}
                    	        	        	switch (alt15) 
                    	        	        	{
                    	        	        	    case 1 :
                    	        	        	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:841:25: EXPONENT
                    	        	        	        {
                    	        	        	        	mEXPONENT(); if (state.failed) return ;

                    	        	        	        }
                    	        	        	        break;

                    	        	        	}

                    	        	        	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:841:36: (f2= FLOAT_SUFFIX )?
                    	        	        	int alt16 = 2;
                    	        	        	int LA16_0 = input.LA(1);

                    	        	        	if ( (LA16_0 == 'd' || LA16_0 == 'f') )
                    	        	        	{
                    	        	        	    alt16 = 1;
                    	        	        	}
                    	        	        	switch (alt16) 
                    	        	        	{
                    	        	        	    case 1 :
                    	        	        	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:841:37: f2= FLOAT_SUFFIX
                    	        	        	        {
                    	        	        	        	int f2Start1194 = CharIndex;
                    	        	        	        	mFLOAT_SUFFIX(); if (state.failed) return ;
                    	        	        	        	f2 = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, f2Start1194, CharIndex-1);
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
                    	        	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:842:8: EXPONENT (f3= FLOAT_SUFFIX )?
                    	        	        {
                    	        	        	mEXPONENT(); if (state.failed) return ;
                    	        	        	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:842:17: (f3= FLOAT_SUFFIX )?
                    	        	        	int alt17 = 2;
                    	        	        	int LA17_0 = input.LA(1);

                    	        	        	if ( (LA17_0 == 'd' || LA17_0 == 'f') )
                    	        	        	{
                    	        	        	    alt17 = 1;
                    	        	        	}
                    	        	        	switch (alt17) 
                    	        	        	{
                    	        	        	    case 1 :
                    	        	        	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:842:18: f3= FLOAT_SUFFIX
                    	        	        	        {
                    	        	        	        	int f3Start1212 = CharIndex;
                    	        	        	        	mFLOAT_SUFFIX(); if (state.failed) return ;
                    	        	        	        	f3 = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, f3Start1212, CharIndex-1);
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
                    	        	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:843:8: f4= FLOAT_SUFFIX
                    	        	        {
                    	        	        	int f4Start1227 = CharIndex;
                    	        	        	mFLOAT_SUFFIX(); if (state.failed) return ;
                    	        	        	f4 = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, f4Start1227, CharIndex-1);
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:861:2: ( ( '0' .. '9' | 'a' .. 'f' ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:861:4: ( '0' .. '9' | 'a' .. 'f' )
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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:867:2: ( ( 'e' ) ( '+' | '-' )? ( '0' .. '9' )+ )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:867:4: ( 'e' ) ( '+' | '-' )? ( '0' .. '9' )+
            {
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:867:4: ( 'e' )
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:867:5: 'e'
            	{
            		Match('e'); if (state.failed) return ;

            	}

            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:867:10: ( '+' | '-' )?
            	int alt21 = 2;
            	int LA21_0 = input.LA(1);

            	if ( (LA21_0 == '+' || LA21_0 == '-') )
            	{
            	    alt21 = 1;
            	}
            	switch (alt21) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:
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

            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:867:21: ( '0' .. '9' )+
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
            			    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:867:22: '0' .. '9'
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
            		;	// Stops C# compiler whinging that label 'loop22' has no statements


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
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:872:2: ( 'f' | 'd' )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:
            {
            	if ( input.LA(1) == 'd' || input.LA(1) == 'f' ) 
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
        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:8: ( ALL | ANY | AND | AS | ASCENDING | AVG | BETWEEN | CLASS | COUNT | DELETE | DESCENDING | DISTINCT | ELEMENTS | ESCAPE | EXISTS | FALSE | FETCH | FROM | FULL | GROUP | HAVING | IN | INDICES | INNER | INSERT | INTO | IS | JOIN | LEFT | LIKE | MAX | MIN | NEW | NOT | NULL | OR | ORDER | OUTER | PROPERTIES | RIGHT | SELECT | SET | SOME | SUM | TRUE | UNION | UPDATE | VERSIONED | WHERE | LITERAL_by | CASE | END | ELSE | THEN | WHEN | ON | WITH | BOTH | EMPTY | LEADING | MEMBER | OBJECT | OF | TRAILING | T__126 | T__127 | EQ | LT | GT | SQL_NE | NE | LE | GE | COMMA | OPEN | CLOSE | OPEN_BRACKET | CLOSE_BRACKET | CONCAT | PLUS | MINUS | STAR | DIV | COLON | PARAM | IDENT | QUOTED_String | WS | NUM_INT )
        int alt23 = 89;
        alt23 = dfa23.Predict(input);
        switch (alt23) 
        {
            case 1 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:10: ALL
                {
                	mALL(); if (state.failed) return ;

                }
                break;
            case 2 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:14: ANY
                {
                	mANY(); if (state.failed) return ;

                }
                break;
            case 3 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:18: AND
                {
                	mAND(); if (state.failed) return ;

                }
                break;
            case 4 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:22: AS
                {
                	mAS(); if (state.failed) return ;

                }
                break;
            case 5 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:25: ASCENDING
                {
                	mASCENDING(); if (state.failed) return ;

                }
                break;
            case 6 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:35: AVG
                {
                	mAVG(); if (state.failed) return ;

                }
                break;
            case 7 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:39: BETWEEN
                {
                	mBETWEEN(); if (state.failed) return ;

                }
                break;
            case 8 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:47: CLASS
                {
                	mCLASS(); if (state.failed) return ;

                }
                break;
            case 9 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:53: COUNT
                {
                	mCOUNT(); if (state.failed) return ;

                }
                break;
            case 10 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:59: DELETE
                {
                	mDELETE(); if (state.failed) return ;

                }
                break;
            case 11 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:66: DESCENDING
                {
                	mDESCENDING(); if (state.failed) return ;

                }
                break;
            case 12 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:77: DISTINCT
                {
                	mDISTINCT(); if (state.failed) return ;

                }
                break;
            case 13 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:86: ELEMENTS
                {
                	mELEMENTS(); if (state.failed) return ;

                }
                break;
            case 14 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:95: ESCAPE
                {
                	mESCAPE(); if (state.failed) return ;

                }
                break;
            case 15 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:102: EXISTS
                {
                	mEXISTS(); if (state.failed) return ;

                }
                break;
            case 16 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:109: FALSE
                {
                	mFALSE(); if (state.failed) return ;

                }
                break;
            case 17 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:115: FETCH
                {
                	mFETCH(); if (state.failed) return ;

                }
                break;
            case 18 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:121: FROM
                {
                	mFROM(); if (state.failed) return ;

                }
                break;
            case 19 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:126: FULL
                {
                	mFULL(); if (state.failed) return ;

                }
                break;
            case 20 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:131: GROUP
                {
                	mGROUP(); if (state.failed) return ;

                }
                break;
            case 21 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:137: HAVING
                {
                	mHAVING(); if (state.failed) return ;

                }
                break;
            case 22 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:144: IN
                {
                	mIN(); if (state.failed) return ;

                }
                break;
            case 23 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:147: INDICES
                {
                	mINDICES(); if (state.failed) return ;

                }
                break;
            case 24 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:155: INNER
                {
                	mINNER(); if (state.failed) return ;

                }
                break;
            case 25 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:161: INSERT
                {
                	mINSERT(); if (state.failed) return ;

                }
                break;
            case 26 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:168: INTO
                {
                	mINTO(); if (state.failed) return ;

                }
                break;
            case 27 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:173: IS
                {
                	mIS(); if (state.failed) return ;

                }
                break;
            case 28 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:176: JOIN
                {
                	mJOIN(); if (state.failed) return ;

                }
                break;
            case 29 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:181: LEFT
                {
                	mLEFT(); if (state.failed) return ;

                }
                break;
            case 30 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:186: LIKE
                {
                	mLIKE(); if (state.failed) return ;

                }
                break;
            case 31 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:191: MAX
                {
                	mMAX(); if (state.failed) return ;

                }
                break;
            case 32 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:195: MIN
                {
                	mMIN(); if (state.failed) return ;

                }
                break;
            case 33 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:199: NEW
                {
                	mNEW(); if (state.failed) return ;

                }
                break;
            case 34 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:203: NOT
                {
                	mNOT(); if (state.failed) return ;

                }
                break;
            case 35 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:207: NULL
                {
                	mNULL(); if (state.failed) return ;

                }
                break;
            case 36 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:212: OR
                {
                	mOR(); if (state.failed) return ;

                }
                break;
            case 37 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:215: ORDER
                {
                	mORDER(); if (state.failed) return ;

                }
                break;
            case 38 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:221: OUTER
                {
                	mOUTER(); if (state.failed) return ;

                }
                break;
            case 39 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:227: PROPERTIES
                {
                	mPROPERTIES(); if (state.failed) return ;

                }
                break;
            case 40 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:238: RIGHT
                {
                	mRIGHT(); if (state.failed) return ;

                }
                break;
            case 41 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:244: SELECT
                {
                	mSELECT(); if (state.failed) return ;

                }
                break;
            case 42 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:251: SET
                {
                	mSET(); if (state.failed) return ;

                }
                break;
            case 43 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:255: SOME
                {
                	mSOME(); if (state.failed) return ;

                }
                break;
            case 44 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:260: SUM
                {
                	mSUM(); if (state.failed) return ;

                }
                break;
            case 45 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:264: TRUE
                {
                	mTRUE(); if (state.failed) return ;

                }
                break;
            case 46 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:269: UNION
                {
                	mUNION(); if (state.failed) return ;

                }
                break;
            case 47 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:275: UPDATE
                {
                	mUPDATE(); if (state.failed) return ;

                }
                break;
            case 48 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:282: VERSIONED
                {
                	mVERSIONED(); if (state.failed) return ;

                }
                break;
            case 49 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:292: WHERE
                {
                	mWHERE(); if (state.failed) return ;

                }
                break;
            case 50 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:298: LITERAL_by
                {
                	mLITERAL_by(); if (state.failed) return ;

                }
                break;
            case 51 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:309: CASE
                {
                	mCASE(); if (state.failed) return ;

                }
                break;
            case 52 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:314: END
                {
                	mEND(); if (state.failed) return ;

                }
                break;
            case 53 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:318: ELSE
                {
                	mELSE(); if (state.failed) return ;

                }
                break;
            case 54 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:323: THEN
                {
                	mTHEN(); if (state.failed) return ;

                }
                break;
            case 55 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:328: WHEN
                {
                	mWHEN(); if (state.failed) return ;

                }
                break;
            case 56 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:333: ON
                {
                	mON(); if (state.failed) return ;

                }
                break;
            case 57 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:336: WITH
                {
                	mWITH(); if (state.failed) return ;

                }
                break;
            case 58 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:341: BOTH
                {
                	mBOTH(); if (state.failed) return ;

                }
                break;
            case 59 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:346: EMPTY
                {
                	mEMPTY(); if (state.failed) return ;

                }
                break;
            case 60 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:352: LEADING
                {
                	mLEADING(); if (state.failed) return ;

                }
                break;
            case 61 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:360: MEMBER
                {
                	mMEMBER(); if (state.failed) return ;

                }
                break;
            case 62 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:367: OBJECT
                {
                	mOBJECT(); if (state.failed) return ;

                }
                break;
            case 63 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:374: OF
                {
                	mOF(); if (state.failed) return ;

                }
                break;
            case 64 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:377: TRAILING
                {
                	mTRAILING(); if (state.failed) return ;

                }
                break;
            case 65 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:386: T__126
                {
                	mT__126(); if (state.failed) return ;

                }
                break;
            case 66 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:393: T__127
                {
                	mT__127(); if (state.failed) return ;

                }
                break;
            case 67 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:400: EQ
                {
                	mEQ(); if (state.failed) return ;

                }
                break;
            case 68 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:403: LT
                {
                	mLT(); if (state.failed) return ;

                }
                break;
            case 69 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:406: GT
                {
                	mGT(); if (state.failed) return ;

                }
                break;
            case 70 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:409: SQL_NE
                {
                	mSQL_NE(); if (state.failed) return ;

                }
                break;
            case 71 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:416: NE
                {
                	mNE(); if (state.failed) return ;

                }
                break;
            case 72 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:419: LE
                {
                	mLE(); if (state.failed) return ;

                }
                break;
            case 73 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:422: GE
                {
                	mGE(); if (state.failed) return ;

                }
                break;
            case 74 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:425: COMMA
                {
                	mCOMMA(); if (state.failed) return ;

                }
                break;
            case 75 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:431: OPEN
                {
                	mOPEN(); if (state.failed) return ;

                }
                break;
            case 76 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:436: CLOSE
                {
                	mCLOSE(); if (state.failed) return ;

                }
                break;
            case 77 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:442: OPEN_BRACKET
                {
                	mOPEN_BRACKET(); if (state.failed) return ;

                }
                break;
            case 78 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:455: CLOSE_BRACKET
                {
                	mCLOSE_BRACKET(); if (state.failed) return ;

                }
                break;
            case 79 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:469: CONCAT
                {
                	mCONCAT(); if (state.failed) return ;

                }
                break;
            case 80 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:476: PLUS
                {
                	mPLUS(); if (state.failed) return ;

                }
                break;
            case 81 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:481: MINUS
                {
                	mMINUS(); if (state.failed) return ;

                }
                break;
            case 82 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:487: STAR
                {
                	mSTAR(); if (state.failed) return ;

                }
                break;
            case 83 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:492: DIV
                {
                	mDIV(); if (state.failed) return ;

                }
                break;
            case 84 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:496: COLON
                {
                	mCOLON(); if (state.failed) return ;

                }
                break;
            case 85 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:502: PARAM
                {
                	mPARAM(); if (state.failed) return ;

                }
                break;
            case 86 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:508: IDENT
                {
                	mIDENT(); if (state.failed) return ;

                }
                break;
            case 87 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:514: QUOTED_String
                {
                	mQUOTED_String(); if (state.failed) return ;

                }
                break;
            case 88 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:528: WS
                {
                	mWS(); if (state.failed) return ;

                }
                break;
            case 89 :
                // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:1:531: NUM_INT
                {
                	mNUM_INT(); if (state.failed) return ;

                }
                break;

        }

    }

    // $ANTLR start "synpred1_Hql"
    public void synpred1_Hql_fragment() {
        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:787:13: ( ESCqs )
        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/Hql.g:787:14: ESCqs
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
        "\x01\uffff\x15\x26\x01\uffff\x01\x5f\x01\x61\x11\uffff\x02\x26\x01"+
        "\x66\x02\x26\x01\x69\x11\x26\x01\u0081\x01\u0082\x09\x26\x01\u008e"+
        "\x01\x26\x01\u0090\x01\x26\x01\u0092\x0c\x26\x05\uffff\x01\u00a1"+
        "\x01\u00a2\x01\u00a3\x01\u00a5\x01\uffff\x01\u00a6\x01\x26\x01\uffff"+
        "\x0b\x26\x01\u00b3\x0b\x26\x02\uffff\x04\x26\x01\u00c3\x01\u00c4"+
        "\x01\x26\x01\u00c6\x01\u00c7\x02\x26\x01\uffff\x01\x26\x01\uffff"+
        "\x01\x26\x01\uffff\x03\x26\x01\u00cf\x01\x26\x01\u00d1\x08\x26\x03"+
        "\uffff\x01\x26\x02\uffff\x01\x26\x01\u00dd\x02\x26\x01\u00e0\x01"+
        "\x26\x01\u00e3\x02\x26\x01\u00e6\x02\x26\x01\uffff\x03\x26\x01\u00ec"+
        "\x01\u00ed\x05\x26\x01\u00f3\x01\u00f4\x01\u00f5\x01\x26\x01\u00f7"+
        "\x02\uffff\x01\x26\x02\uffff\x01\u00f9\x06\x26\x01\uffff\x01\u0100"+
        "\x01\uffff\x01\u0101\x01\x26\x01\u0103\x04\x26\x01\u0108\x01\u0109"+
        "\x02\x26\x01\uffff\x01\u010c\x01\u010d\x01\uffff\x02\x26\x01\uffff"+
        "\x02\x26\x01\uffff\x02\x26\x01\u0114\x01\u0115\x01\u0116\x02\uffff"+
        "\x01\u0117\x02\x26\x01\u011a\x01\x26\x03\uffff\x01\x26\x01\uffff"+
        "\x01\x26\x01\uffff\x01\u011e\x01\u011f\x02\x26\x01\u0122\x01\x26"+
        "\x02\uffff\x01\x26\x01\uffff\x01\u0125\x02\x26\x01\u0128\x02\uffff"+
        "\x02\x26\x02\uffff\x01\u012b\x03\x26\x01\u012f\x01\u0130\x04\uffff"+
        "\x01\u0131\x01\x26\x01\uffff\x01\u0133\x01\x26\x01\u0135\x02\uffff"+
        "\x01\u0136\x01\x26\x01\uffff\x01\u0138\x01\x26\x01\uffff\x01\u013a"+
        "\x01\x26\x01\uffff\x01\x26\x01\u013d\x01\uffff\x03\x26\x03\uffff"+
        "\x01\u0141\x01\uffff\x01\u0142\x02\uffff\x01\x26\x01\uffff\x01\x26"+
        "\x01\uffff\x02\x26\x01\uffff\x01\x26\x01\u0148\x01\u0149\x02\uffff"+
        "\x01\x26\x01\u014b\x01\x26\x01\u014d\x01\x26\x02\uffff\x01\x26\x01"+
        "\uffff\x01\u0150\x01\uffff\x01\u0151\x01\u0152\x03\uffff";
    const string DFA23_eofS =
        "\u0153\uffff";
    const string DFA23_minS =
        "\x01\x09\x01\x6c\x01\x65\x01\x61\x01\x65\x01\x6c\x01\x61\x01\x72"+
        "\x01\x61\x01\x6e\x01\x6f\x01\x65\x01\x61\x01\x65\x01\x62\x01\x72"+
        "\x01\x69\x01\x65\x01\x68\x01\x6e\x01\x65\x01\x68\x01\uffff\x02\x3d"+
        "\x11\uffff\x01\x6c\x01\x64\x01\x24\x01\x67\x01\x74\x01\x24\x01\x74"+
        "\x01\x61\x01\x75\x01\x73\x01\x6c\x01\x73\x01\x65\x01\x63\x01\x69"+
        "\x01\x64\x01\x70\x01\x6c\x01\x74\x01\x6f\x01\x6c\x01\x6f\x01\x76"+
        "\x02\x24\x01\x69\x01\x61\x01\x6b\x01\x78\x01\x6e\x01\x6d\x01\x77"+
        "\x01\x74\x01\x6c\x01\x24\x01\x74\x01\x24\x01\x6a\x01\x24\x01\x6f"+
        "\x01\x67\x01\x6c\x02\x6d\x01\x61\x01\x65\x01\x69\x01\x64\x01\x72"+
        "\x01\x65\x01\x74\x05\uffff\x04\x24\x01\uffff\x01\x24\x01\x77\x01"+
        "\uffff\x01\x68\x01\x73\x01\x6e\x02\x65\x01\x63\x01\x74\x01\x6d\x01"+
        "\x65\x01\x61\x01\x73\x01\x24\x01\x74\x01\x73\x01\x63\x01\x6d\x01"+
        "\x6c\x01\x75\x02\x69\x02\x65\x01\x6f\x02\uffff\x01\x6e\x01\x74\x01"+
        "\x64\x01\x65\x02\x24\x01\x62\x02\x24\x01\x6c\x01\x65\x01\uffff\x01"+
        "\x65\x01\uffff\x01\x65\x01\uffff\x01\x70\x01\x68\x01\x65\x01\x24"+
        "\x01\x65\x01\x24\x01\x65\x01\x69\x01\x6e\x01\x6f\x01\x61\x01\x73"+
        "\x01\x6e\x01\x68\x03\uffff\x01\x6e\x02\uffff\x01\x65\x01\x24\x01"+
        "\x73\x01\x74\x01\x24\x01\x74\x01\x24\x01\x69\x01\x65\x01\x24\x01"+
        "\x70\x01\x74\x01\uffff\x01\x79\x01\x65\x01\x68\x02\x24\x01\x70\x01"+
        "\x6e\x01\x63\x02\x72\x03\x24\x01\x69\x01\x24\x02\uffff\x01\x65\x02"+
        "\uffff\x01\x24\x02\x72\x01\x63\x01\x65\x01\x74\x01\x63\x01\uffff"+
        "\x01\x24\x01\uffff\x01\x24\x01\x6c\x01\x24\x01\x6e\x01\x74\x01\x69"+
        "\x01\x65\x02\x24\x01\x64\x01\x65\x01\uffff\x02\x24\x01\uffff\x01"+
        "\x65\x01\x6e\x01\uffff\x02\x6e\x01\uffff\x01\x65\x01\x73\x03\x24"+
        "\x02\uffff\x01\x24\x01\x67\x01\x65\x01\x24\x01\x74\x03\uffff\x01"+
        "\x6e\x01\uffff\x01\x72\x01\uffff\x02\x24\x01\x74\x01\x72\x01\x24"+
        "\x01\x74\x02\uffff\x01\x69\x01\uffff\x01\x24\x01\x65\x01\x6f\x01"+
        "\x24\x02\uffff\x01\x69\x01\x6e\x02\uffff\x01\x24\x01\x64\x01\x63"+
        "\x01\x74\x02\x24\x04\uffff\x01\x24\x01\x73\x01\uffff\x01\x24\x01"+
        "\x67\x01\x24\x02\uffff\x01\x24\x01\x74\x01\uffff\x01\x24\x01\x6e"+
        "\x01\uffff\x01\x24\x01\x6e\x01\uffff\x01\x6e\x01\x24\x01\uffff\x01"+
        "\x69\x01\x74\x01\x73\x03\uffff\x01\x24\x01\uffff\x01\x24\x02\uffff"+
        "\x01\x69\x01\uffff\x01\x67\x01\uffff\x01\x65\x01\x67\x01\uffff\x01"+
        "\x6e\x02\x24\x02\uffff\x01\x65\x01\x24\x01\x64\x01\x24\x01\x67\x02"+
        "\uffff\x01\x73\x01\uffff\x01\x24\x01\uffff\x02\x24\x03\uffff";
    const string DFA23_maxS =
        "\x01\ufffe\x01\x76\x01\x79\x01\x6f\x01\x69\x01\x78\x01\x75\x01\x72"+
        "\x01\x61\x01\x73\x01\x6f\x02\x69\x02\x75\x01\x72\x01\x69\x01\x75"+
        "\x01\x72\x01\x70\x01\x65\x01\x69\x01\uffff\x01\x3e\x01\x3d\x11\uffff"+
        "\x01\x6c\x01\x79\x01\ufffe\x01\x67\x01\x74\x01\ufffe\x01\x74\x01"+
        "\x61\x01\x75\x04\x73\x01\x63\x01\x69\x01\x64\x01\x70\x01\x6c\x01"+
        "\x74\x01\x6f\x01\x6c\x01\x6f\x01\x76\x02\ufffe\x01\x69\x01\x66\x01"+
        "\x6b\x01\x78\x01\x6e\x01\x6d\x01\x77\x01\x74\x01\x6c\x01\ufffe\x01"+
        "\x74\x01\ufffe\x01\x6a\x01\ufffe\x01\x6f\x01\x67\x01\x74\x02\x6d"+
        "\x01\x75\x01\x65\x01\x69\x01\x64\x01\x72\x01\x65\x01\x74\x05\uffff"+
        "\x04\ufffe\x01\uffff\x01\ufffe\x01\x77\x01\uffff\x01\x68\x01\x73"+
        "\x01\x6e\x02\x65\x01\x63\x01\x74\x01\x6d\x01\x65\x01\x61\x01\x73"+
        "\x01\ufffe\x01\x74\x01\x73\x01\x63\x01\x6d\x01\x6c\x01\x75\x02\x69"+
        "\x02\x65\x01\x6f\x02\uffff\x01\x6e\x01\x74\x01\x64\x01\x65\x02\ufffe"+
        "\x01\x62\x02\ufffe\x01\x6c\x01\x65\x01\uffff\x01\x65\x01\uffff\x01"+
        "\x65\x01\uffff\x01\x70\x01\x68\x01\x65\x01\ufffe\x01\x65\x01\ufffe"+
        "\x01\x65\x01\x69\x01\x6e\x01\x6f\x01\x61\x01\x73\x01\x72\x01\x68"+
        "\x03\uffff\x01\x6e\x02\uffff\x01\x65\x01\ufffe\x01\x73\x01\x74\x01"+
        "\ufffe\x01\x74\x01\ufffe\x01\x69\x01\x65\x01\ufffe\x01\x70\x01\x74"+
        "\x01\uffff\x01\x79\x01\x65\x01\x68\x02\ufffe\x01\x70\x01\x6e\x01"+
        "\x63\x02\x72\x03\ufffe\x01\x69\x01\ufffe\x02\uffff\x01\x65\x02\uffff"+
        "\x01\ufffe\x02\x72\x01\x63\x01\x65\x01\x74\x01\x63\x01\uffff\x01"+
        "\ufffe\x01\uffff\x01\ufffe\x01\x6c\x01\ufffe\x01\x6e\x01\x74\x01"+
        "\x69\x01\x65\x02\ufffe\x01\x64\x01\x65\x01\uffff\x02\ufffe\x01\uffff"+
        "\x01\x65\x01\x6e\x01\uffff\x02\x6e\x01\uffff\x01\x65\x01\x73\x03"+
        "\ufffe\x02\uffff\x01\ufffe\x01\x67\x01\x65\x01\ufffe\x01\x74\x03"+
        "\uffff\x01\x6e\x01\uffff\x01\x72\x01\uffff\x02\ufffe\x01\x74\x01"+
        "\x72\x01\ufffe\x01\x74\x02\uffff\x01\x69\x01\uffff\x01\ufffe\x01"+
        "\x65\x01\x6f\x01\ufffe\x02\uffff\x01\x69\x01\x6e\x02\uffff\x01\ufffe"+
        "\x01\x64\x01\x63\x01\x74\x02\ufffe\x04\uffff\x01\ufffe\x01\x73\x01"+
        "\uffff\x01\ufffe\x01\x67\x01\ufffe\x02\uffff\x01\ufffe\x01\x74\x01"+
        "\uffff\x01\ufffe\x01\x6e\x01\uffff\x01\ufffe\x01\x6e\x01\uffff\x01"+
        "\x6e\x01\ufffe\x01\uffff\x01\x69\x01\x74\x01\x73\x03\uffff\x01\ufffe"+
        "\x01\uffff\x01\ufffe\x02\uffff\x01\x69\x01\uffff\x01\x67\x01\uffff"+
        "\x01\x65\x01\x67\x01\uffff\x01\x6e\x02\ufffe\x02\uffff\x01\x65\x01"+
        "\ufffe\x01\x64\x01\ufffe\x01\x67\x02\uffff\x01\x73\x01\uffff\x01"+
        "\ufffe\x01\uffff\x02\ufffe\x03\uffff";
    const string DFA23_acceptS =
        "\x16\uffff\x01\x43\x02\uffff\x01\x47\x01\x4a\x01\x4b\x01\x4c\x01"+
        "\x4d\x01\x4e\x01\x4f\x01\x50\x01\x51\x01\x52\x01\x53\x01\x54\x01"+
        "\x55\x01\x56\x01\x57\x01\x58\x01\x59\x33\uffff\x01\x46\x01\x48\x01"+
        "\x44\x01\x49\x01\x45\x04\uffff\x01\x04\x02\uffff\x01\x32\x17\uffff"+
        "\x01\x16\x01\x1b\x0b\uffff\x01\x24\x01\uffff\x01\x38\x01\uffff\x01"+
        "\x3f\x0e\uffff\x01\x01\x01\x02\x01\x03\x01\uffff\x01\x05\x01\x06"+
        "\x0c\uffff\x01\x34\x0f\uffff\x01\x1f\x01\x20\x01\uffff\x01\x21\x01"+
        "\x22\x07\uffff\x01\x2a\x01\uffff\x01\x2c\x0b\uffff\x01\x3a\x02\uffff"+
        "\x01\x33\x02\uffff\x01\x0b\x02\uffff\x01\x35\x05\uffff\x01\x12\x01"+
        "\x13\x05\uffff\x01\x1a\x01\x1c\x01\x1d\x01\uffff\x01\x1e\x01\uffff"+
        "\x01\x23\x06\uffff\x01\x2b\x01\x2d\x01\uffff\x01\x36\x04\uffff\x01"+
        "\x37\x01\x39\x02\uffff\x01\x08\x01\x09\x06\uffff\x01\x3b\x01\x10"+
        "\x01\x11\x01\x14\x02\uffff\x01\x18\x03\uffff\x01\x25\x01\x26\x02"+
        "\uffff\x01\x28\x02\uffff\x01\x2e\x02\uffff\x01\x31\x02\uffff\x01"+
        "\x0a\x03\uffff\x01\x0e\x01\x0f\x01\x15\x01\uffff\x01\x19\x01\uffff"+
        "\x01\x3d\x01\x3e\x01\uffff\x01\x29\x01\uffff\x01\x2f\x02\uffff\x01"+
        "\x07\x03\uffff\x01\x17\x01\x3c\x05\uffff\x01\x0c\x01\x0d\x01\uffff"+
        "\x01\x40\x01\uffff\x01\x41\x02\uffff\x01\x30\x01\x42\x01\x27";
    const string DFA23_specialS =
        "\u0153\uffff}>";
    static readonly string[] DFA23_transitionS = {
            "\x02\x28\x02\uffff\x01\x28\x12\uffff\x01\x28\x01\x19\x02\uffff"+
            "\x01\x26\x02\uffff\x01\x27\x01\x1b\x01\x1c\x01\x22\x01\x20\x01"+
            "\x1a\x01\x21\x01\x29\x01\x23\x0a\x29\x01\x24\x01\uffff\x01\x17"+
            "\x01\x16\x01\x18\x01\x25\x01\uffff\x1a\x26\x01\x1d\x01\uffff"+
            "\x01\x1e\x01\x19\x01\x26\x01\uffff\x01\x01\x01\x02\x01\x03\x01"+
            "\x04\x01\x05\x01\x06\x01\x07\x01\x08\x01\x09\x01\x0a\x01\x26"+
            "\x01\x0b\x01\x0c\x01\x0d\x01\x0e\x01\x0f\x01\x26\x01\x10\x01"+
            "\x11\x01\x12\x01\x13\x01\x14\x01\x15\x03\x26\x01\uffff\x01\x1f"+
            "\x03\uffff\uff7f\x26",
            "\x01\x2a\x01\uffff\x01\x2b\x04\uffff\x01\x2c\x02\uffff\x01"+
            "\x2d",
            "\x01\x2e\x09\uffff\x01\x30\x09\uffff\x01\x2f",
            "\x01\x33\x0a\uffff\x01\x31\x02\uffff\x01\x32",
            "\x01\x34\x03\uffff\x01\x35",
            "\x01\x36\x01\x3a\x01\x39\x04\uffff\x01\x37\x04\uffff\x01\x38",
            "\x01\x3b\x03\uffff\x01\x3c\x0c\uffff\x01\x3d\x02\uffff\x01"+
            "\x3e",
            "\x01\x3f",
            "\x01\x40",
            "\x01\x41\x04\uffff\x01\x42",
            "\x01\x43",
            "\x01\x44\x03\uffff\x01\x45",
            "\x01\x46\x03\uffff\x01\x48\x03\uffff\x01\x47",
            "\x01\x49\x09\uffff\x01\x4a\x05\uffff\x01\x4b",
            "\x01\x4f\x03\uffff\x01\x50\x07\uffff\x01\x4e\x03\uffff\x01"+
            "\x4c\x02\uffff\x01\x4d",
            "\x01\x51",
            "\x01\x52",
            "\x01\x53\x09\uffff\x01\x54\x05\uffff\x01\x55",
            "\x01\x57\x09\uffff\x01\x56",
            "\x01\x58\x01\uffff\x01\x59",
            "\x01\x5a",
            "\x01\x5b\x01\x5c",
            "",
            "\x01\x5e\x01\x5d",
            "\x01\x60",
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
            "\x01\x62",
            "\x01\x64\x14\uffff\x01\x63",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x02\x26\x01\x65\x17\x26\x05\uffff\uff7f\x26",
            "\x01\x67",
            "\x01\x68",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\x6a",
            "\x01\x6b",
            "\x01\x6c",
            "\x01\x6d",
            "\x01\x6e\x06\uffff\x01\x6f",
            "\x01\x70",
            "\x01\x71\x0d\uffff\x01\x72",
            "\x01\x73",
            "\x01\x74",
            "\x01\x75",
            "\x01\x76",
            "\x01\x77",
            "\x01\x78",
            "\x01\x79",
            "\x01\x7a",
            "\x01\x7b",
            "\x01\x7c",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x03\x26\x01\x7d\x09\x26\x01\x7e\x04\x26\x01\x7f"+
            "\x01\u0080\x06\x26\x05\uffff\uff7f\x26",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\u0083",
            "\x01\u0085\x04\uffff\x01\u0084",
            "\x01\u0086",
            "\x01\u0087",
            "\x01\u0088",
            "\x01\u0089",
            "\x01\u008a",
            "\x01\u008b",
            "\x01\u008c",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x03\x26\x01\u008d\x16\x26\x05\uffff\uff7f\x26",
            "\x01\u008f",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\u0091",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\u0093",
            "\x01\u0094",
            "\x01\u0095\x07\uffff\x01\u0096",
            "\x01\u0097",
            "\x01\u0098",
            "\x01\u009a\x13\uffff\x01\u0099",
            "\x01\u009b",
            "\x01\u009c",
            "\x01\u009d",
            "\x01\u009e",
            "\x01\u009f",
            "\x01\u00a0",
            "",
            "",
            "",
            "",
            "",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x04\x26\x01\u00a4\x15\x26\x05\uffff\uff7f\x26",
            "",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\u00a7",
            "",
            "\x01\u00a8",
            "\x01\u00a9",
            "\x01\u00aa",
            "\x01\u00ab",
            "\x01\u00ac",
            "\x01\u00ad",
            "\x01\u00ae",
            "\x01\u00af",
            "\x01\u00b0",
            "\x01\u00b1",
            "\x01\u00b2",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
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
            "\x01\u00be",
            "",
            "",
            "\x01\u00bf",
            "\x01\u00c0",
            "\x01\u00c1",
            "\x01\u00c2",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\u00c5",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\u00c8",
            "\x01\u00c9",
            "",
            "\x01\u00ca",
            "",
            "\x01\u00cb",
            "",
            "\x01\u00cc",
            "\x01\u00cd",
            "\x01\u00ce",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\u00d0",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\u00d2",
            "\x01\u00d3",
            "\x01\u00d4",
            "\x01\u00d5",
            "\x01\u00d6",
            "\x01\u00d7",
            "\x01\u00d9\x03\uffff\x01\u00d8",
            "\x01\u00da",
            "",
            "",
            "",
            "\x01\u00db",
            "",
            "",
            "\x01\u00dc",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\u00de",
            "\x01\u00df",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\u00e1",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x04\x26\x01\u00e2\x15\x26\x05\uffff\uff7f\x26",
            "\x01\u00e4",
            "\x01\u00e5",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\u00e7",
            "\x01\u00e8",
            "",
            "\x01\u00e9",
            "\x01\u00ea",
            "\x01\u00eb",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\u00ee",
            "\x01\u00ef",
            "\x01\u00f0",
            "\x01\u00f1",
            "\x01\u00f2",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\u00f6",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "",
            "",
            "\x01\u00f8",
            "",
            "",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\u00fa",
            "\x01\u00fb",
            "\x01\u00fc",
            "\x01\u00fd",
            "\x01\u00fe",
            "\x01\u00ff",
            "",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\u0102",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\u0104",
            "\x01\u0105",
            "\x01\u0106",
            "\x01\u0107",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\u010a",
            "\x01\u010b",
            "",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "",
            "\x01\u010e",
            "\x01\u010f",
            "",
            "\x01\u0110",
            "\x01\u0111",
            "",
            "\x01\u0112",
            "\x01\u0113",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "",
            "",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\u0118",
            "\x01\u0119",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\u011b",
            "",
            "",
            "",
            "\x01\u011c",
            "",
            "\x01\u011d",
            "",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\u0120",
            "\x01\u0121",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\u0123",
            "",
            "",
            "\x01\u0124",
            "",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\u0126",
            "\x01\u0127",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "",
            "",
            "\x01\u0129",
            "\x01\u012a",
            "",
            "",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\u012c",
            "\x01\u012d",
            "\x01\u012e",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "",
            "",
            "",
            "",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\u0132",
            "",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\u0134",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "",
            "",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\u0137",
            "",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\u0139",
            "",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\u013b",
            "",
            "\x01\u013c",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "",
            "\x01\u013e",
            "\x01\u013f",
            "\x01\u0140",
            "",
            "",
            "",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "",
            "",
            "\x01\u0143",
            "",
            "\x01\u0144",
            "",
            "\x01\u0145",
            "\x01\u0146",
            "",
            "\x01\u0147",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "",
            "",
            "\x01\u014a",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\u014c",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\u014e",
            "",
            "",
            "\x01\u014f",
            "",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
            "\x01\x26\x0b\uffff\x0a\x26\x07\uffff\x1a\x26\x04\uffff\x01"+
            "\x26\x01\uffff\x1a\x26\x05\uffff\uff7f\x26",
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
            get { return "1:1: Tokens : ( ALL | ANY | AND | AS | ASCENDING | AVG | BETWEEN | CLASS | COUNT | DELETE | DESCENDING | DISTINCT | ELEMENTS | ESCAPE | EXISTS | FALSE | FETCH | FROM | FULL | GROUP | HAVING | IN | INDICES | INNER | INSERT | INTO | IS | JOIN | LEFT | LIKE | MAX | MIN | NEW | NOT | NULL | OR | ORDER | OUTER | PROPERTIES | RIGHT | SELECT | SET | SOME | SUM | TRUE | UNION | UPDATE | VERSIONED | WHERE | LITERAL_by | CASE | END | ELSE | THEN | WHEN | ON | WITH | BOTH | EMPTY | LEADING | MEMBER | OBJECT | OF | TRAILING | T__126 | T__127 | EQ | LT | GT | SQL_NE | NE | LE | GE | COMMA | OPEN | CLOSE | OPEN_BRACKET | CLOSE_BRACKET | CONCAT | PLUS | MINUS | STAR | DIV | COLON | PARAM | IDENT | QUOTED_String | WS | NUM_INT );"; }
        }

    }

 
    
}
}