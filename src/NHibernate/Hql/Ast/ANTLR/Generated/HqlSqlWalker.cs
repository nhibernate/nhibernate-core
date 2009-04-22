// $ANTLR 3.1.2 /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g 2009-04-16 21:34:40

// The variable 'variable' is assigned but its value is never used.
#pragma warning disable 168, 219
// Unreachable code detected.
#pragma warning disable 162
namespace  NHibernate.Hql.Ast.ANTLR 
{

using System.Text;
using NHibernate.Hql.Ast.ANTLR.Tree;


using System;
using Antlr.Runtime;
using Antlr.Runtime.Tree;using IList 		= System.Collections.IList;
using ArrayList 	= System.Collections.ArrayList;
using Stack 		= Antlr.Runtime.Collections.StackList;



public partial class HqlSqlWalker : TreeParser
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

    public const int EXPR_LIST = 73;
    public const int EXISTS = 19;
    public const int COMMA = 98;
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
    public const int UNARY_MINUS = 88;
    public const int METHOD_CALL = 79;
    public const int RIGHT = 44;
    public const int CONCAT = 108;
    public const int PROPERTIES = 43;
    public const int SELECT = 45;
    public const int LE = 106;
    public const int RIGHT_OUTER = 133;
    public const int BETWEEN = 10;
    public const int SQL_TOKEN = 136;
    public const int NUM_INT = 93;
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
    public const int PROPERTY_REF = 135;
    public const int CONSTRUCTOR = 71;
    public const int SOME = 47;
    public const int CLASS = 11;
    public const int SELECT_COLUMNS = 137;
    public const int EXPONENT = 123;
    public const int BOGUS = 143;
    public const int ID_START_LETTER = 119;
    public const int EOF = -1;
    public const int CLOSE = 101;
    public const int AVG = 9;
    public const int SELECT_CLAUSE = 131;
    public const int STAR = 111;
    public const int NOT = 38;
    public const int JAVA_CONSTANT = 97;

    // delegates
    // delegators



        public HqlSqlWalker(ITreeNodeStream input)
    		: this(input, new RecognizerSharedState()) {
        }

        public HqlSqlWalker(ITreeNodeStream input, RecognizerSharedState state)
    		: base(input, state) {
            InitializeCyclicDFAs();

             
        }
        
    protected ITreeAdaptor adaptor = new CommonTreeAdaptor();

    public ITreeAdaptor TreeAdaptor
    {
        get { return this.adaptor; }
        set {
    	this.adaptor = value;
    	}
    }

    override public string[] TokenNames {
		get { return HqlSqlWalker.tokenNames; }
    }

    override public string GrammarFileName {
		get { return "/Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g"; }
    }


    public class statement_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "statement"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:40:1: statement : ( selectStatement | updateStatement | deleteStatement | insertStatement );
    public HqlSqlWalker.statement_return statement() // throws RecognitionException [1]
    {   
        HqlSqlWalker.statement_return retval = new HqlSqlWalker.statement_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        HqlSqlWalker.selectStatement_return selectStatement1 = default(HqlSqlWalker.selectStatement_return);

        HqlSqlWalker.updateStatement_return updateStatement2 = default(HqlSqlWalker.updateStatement_return);

        HqlSqlWalker.deleteStatement_return deleteStatement3 = default(HqlSqlWalker.deleteStatement_return);

        HqlSqlWalker.insertStatement_return insertStatement4 = default(HqlSqlWalker.insertStatement_return);



        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:41:2: ( selectStatement | updateStatement | deleteStatement | insertStatement )
            int alt1 = 4;
            switch ( input.LA(1) ) 
            {
            case QUERY:
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
            	    NoViableAltException nvae_d1s0 =
            	        new NoViableAltException("", 1, 0, input);

            	    throw nvae_d1s0;
            }

            switch (alt1) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:41:4: selectStatement
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_selectStatement_in_statement168);
                    	selectStatement1 = selectStatement();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, selectStatement1.Tree);

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:41:22: updateStatement
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_updateStatement_in_statement172);
                    	updateStatement2 = updateStatement();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, updateStatement2.Tree);

                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:41:40: deleteStatement
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_deleteStatement_in_statement176);
                    	deleteStatement3 = deleteStatement();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, deleteStatement3.Tree);

                    }
                    break;
                case 4 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:41:58: insertStatement
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_insertStatement_in_statement180);
                    	insertStatement4 = insertStatement();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, insertStatement4.Tree);

                    }
                    break;

            }
            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "statement"

    public class selectStatement_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "selectStatement"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:44:1: selectStatement : query ;
    public HqlSqlWalker.selectStatement_return selectStatement() // throws RecognitionException [1]
    {   
        HqlSqlWalker.selectStatement_return retval = new HqlSqlWalker.selectStatement_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        HqlSqlWalker.query_return query5 = default(HqlSqlWalker.query_return);



        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:45:2: ( query )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:45:4: query
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_query_in_selectStatement191);
            	query5 = query();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, query5.Tree);

            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "selectStatement"

    public class updateStatement_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "updateStatement"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:51:1: updateStatement : ^(u= UPDATE (v= VERSIONED )? f= fromClause s= setClause (w= whereClause )? ) -> ^( $u $f $s ( $w)? ) ;
    public HqlSqlWalker.updateStatement_return updateStatement() // throws RecognitionException [1]
    {   
        HqlSqlWalker.updateStatement_return retval = new HqlSqlWalker.updateStatement_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode u = null;
        IASTNode v = null;
        HqlSqlWalker.fromClause_return f = default(HqlSqlWalker.fromClause_return);

        HqlSqlWalker.setClause_return s = default(HqlSqlWalker.setClause_return);

        HqlSqlWalker.whereClause_return w = default(HqlSqlWalker.whereClause_return);


        IASTNode u_tree=null;
        IASTNode v_tree=null;
        RewriteRuleNodeStream stream_UPDATE = new RewriteRuleNodeStream(adaptor,"token UPDATE");
        RewriteRuleNodeStream stream_VERSIONED = new RewriteRuleNodeStream(adaptor,"token VERSIONED");
        RewriteRuleSubtreeStream stream_fromClause = new RewriteRuleSubtreeStream(adaptor,"rule fromClause");
        RewriteRuleSubtreeStream stream_whereClause = new RewriteRuleSubtreeStream(adaptor,"rule whereClause");
        RewriteRuleSubtreeStream stream_setClause = new RewriteRuleSubtreeStream(adaptor,"rule setClause");
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:58:2: ( ^(u= UPDATE (v= VERSIONED )? f= fromClause s= setClause (w= whereClause )? ) -> ^( $u $f $s ( $w)? ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:58:4: ^(u= UPDATE (v= VERSIONED )? f= fromClause s= setClause (w= whereClause )? )
            {
            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	u=(IASTNode)Match(input,UPDATE,FOLLOW_UPDATE_in_updateStatement215);  
            	stream_UPDATE.Add(u);


            	 BeforeStatement( "update", UPDATE ); 

            	Match(input, Token.DOWN, null); 
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:58:57: (v= VERSIONED )?
            	int alt2 = 2;
            	int LA2_0 = input.LA(1);

            	if ( (LA2_0 == VERSIONED) )
            	{
            	    alt2 = 1;
            	}
            	switch (alt2) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:58:58: v= VERSIONED
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	v=(IASTNode)Match(input,VERSIONED,FOLLOW_VERSIONED_in_updateStatement222);  
            	        	stream_VERSIONED.Add(v);


            	        }
            	        break;

            	}

            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_fromClause_in_updateStatement228);
            	f = fromClause();
            	state.followingStackPointer--;

            	stream_fromClause.Add(f.Tree);
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_setClause_in_updateStatement232);
            	s = setClause();
            	state.followingStackPointer--;

            	stream_setClause.Add(s.Tree);
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:58:97: (w= whereClause )?
            	int alt3 = 2;
            	int LA3_0 = input.LA(1);

            	if ( (LA3_0 == WHERE) )
            	{
            	    alt3 = 1;
            	}
            	switch (alt3) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:58:98: w= whereClause
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_whereClause_in_updateStatement237);
            	        	w = whereClause();
            	        	state.followingStackPointer--;

            	        	stream_whereClause.Add(w.Tree);

            	        }
            	        break;

            	}


            	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	}



            	// AST REWRITE
            	// elements:          w, u, s, f
            	// token labels:      u
            	// rule labels:       w, f, retval, s
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	retval.Tree = root_0;
            	RewriteRuleNodeStream stream_u = new RewriteRuleNodeStream(adaptor, "token u", u);
            	RewriteRuleSubtreeStream stream_w = new RewriteRuleSubtreeStream(adaptor, "rule w", w!=null ? w.Tree : null);
            	RewriteRuleSubtreeStream stream_f = new RewriteRuleSubtreeStream(adaptor, "rule f", f!=null ? f.Tree : null);
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);
            	RewriteRuleSubtreeStream stream_s = new RewriteRuleSubtreeStream(adaptor, "rule s", s!=null ? s.Tree : null);

            	root_0 = (IASTNode)adaptor.GetNilNode();
            	// 59:3: -> ^( $u $f $s ( $w)? )
            	{
            	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:59:6: ^( $u $f $s ( $w)? )
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot(stream_u.NextNode(), root_1);

            	    adaptor.AddChild(root_1, stream_f.NextTree());
            	    adaptor.AddChild(root_1, stream_s.NextTree());
            	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:59:17: ( $w)?
            	    if ( stream_w.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_w.NextTree());

            	    }
            	    stream_w.Reset();

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;
            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);


            		BeforeStatementCompletion( "update" );
            		PrepareVersioned( ((IASTNode)retval.Tree), v_tree );
            		PostProcessUpdate( ((IASTNode)retval.Tree) );
            		AfterStatementCompletion( "update" );
            	
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
    // $ANTLR end "updateStatement"

    public class deleteStatement_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "deleteStatement"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:62:1: deleteStatement : ^( DELETE fromClause ( whereClause )? ) ;
    public HqlSqlWalker.deleteStatement_return deleteStatement() // throws RecognitionException [1]
    {   
        HqlSqlWalker.deleteStatement_return retval = new HqlSqlWalker.deleteStatement_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode DELETE6 = null;
        HqlSqlWalker.fromClause_return fromClause7 = default(HqlSqlWalker.fromClause_return);

        HqlSqlWalker.whereClause_return whereClause8 = default(HqlSqlWalker.whereClause_return);


        IASTNode DELETE6_tree=null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:68:2: ( ^( DELETE fromClause ( whereClause )? ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:68:4: ^( DELETE fromClause ( whereClause )? )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	DELETE6=(IASTNode)Match(input,DELETE,FOLLOW_DELETE_in_deleteStatement280); 
            		DELETE6_tree = (IASTNode)adaptor.DupNode(DELETE6);

            		root_1 = (IASTNode)adaptor.BecomeRoot(DELETE6_tree, root_1);


            	 BeforeStatement( "delete", DELETE ); 

            	Match(input, Token.DOWN, null); 
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_fromClause_in_deleteStatement284);
            	fromClause7 = fromClause();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_1, fromClause7.Tree);
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:68:66: ( whereClause )?
            	int alt4 = 2;
            	int LA4_0 = input.LA(1);

            	if ( (LA4_0 == WHERE) )
            	{
            	    alt4 = 1;
            	}
            	switch (alt4) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:68:67: whereClause
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_whereClause_in_deleteStatement287);
            	        	whereClause8 = whereClause();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, whereClause8.Tree);

            	        }
            	        break;

            	}


            	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	}


            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);


            		BeforeStatementCompletion( "delete" );
            		PostProcessDelete( ((IASTNode)retval.Tree) );
            		AfterStatementCompletion( "delete" );
            	
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
    // $ANTLR end "deleteStatement"

    public class insertStatement_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "insertStatement"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:71:1: insertStatement : ^( INSERT intoClause query ) ;
    public HqlSqlWalker.insertStatement_return insertStatement() // throws RecognitionException [1]
    {   
        HqlSqlWalker.insertStatement_return retval = new HqlSqlWalker.insertStatement_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode INSERT9 = null;
        HqlSqlWalker.intoClause_return intoClause10 = default(HqlSqlWalker.intoClause_return);

        HqlSqlWalker.query_return query11 = default(HqlSqlWalker.query_return);


        IASTNode INSERT9_tree=null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:80:2: ( ^( INSERT intoClause query ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:80:4: ^( INSERT intoClause query )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	INSERT9=(IASTNode)Match(input,INSERT,FOLLOW_INSERT_in_insertStatement317); 
            		INSERT9_tree = (IASTNode)adaptor.DupNode(INSERT9);

            		root_1 = (IASTNode)adaptor.BecomeRoot(INSERT9_tree, root_1);


            	 BeforeStatement( "insert", INSERT ); 

            	Match(input, Token.DOWN, null); 
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_intoClause_in_insertStatement321);
            	intoClause10 = intoClause();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_1, intoClause10.Tree);
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_query_in_insertStatement323);
            	query11 = query();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_1, query11.Tree);

            	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	}


            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);


            		BeforeStatementCompletion( "insert" );
            		PostProcessInsert( ((IASTNode)retval.Tree) );
            		AfterStatementCompletion( "insert" );
            	
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
    // $ANTLR end "insertStatement"

    public class intoClause_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "intoClause"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:83:1: intoClause : ^( INTO (p= path ) ps= insertablePropertySpec ) ;
    public HqlSqlWalker.intoClause_return intoClause() // throws RecognitionException [1]
    {   
        HqlSqlWalker.intoClause_return retval = new HqlSqlWalker.intoClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode INTO12 = null;
        HqlSqlWalker.path_return p = default(HqlSqlWalker.path_return);

        HqlSqlWalker.insertablePropertySpec_return ps = default(HqlSqlWalker.insertablePropertySpec_return);


        IASTNode INTO12_tree=null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:84:2: ( ^( INTO (p= path ) ps= insertablePropertySpec ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:84:4: ^( INTO (p= path ) ps= insertablePropertySpec )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	INTO12=(IASTNode)Match(input,INTO,FOLLOW_INTO_in_intoClause341); 
            		INTO12_tree = (IASTNode)adaptor.DupNode(INTO12);

            		root_1 = (IASTNode)adaptor.BecomeRoot(INTO12_tree, root_1);


            	 HandleClauseStart( INTO ); 

            	Match(input, Token.DOWN, null); 
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:84:43: (p= path )
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:84:44: p= path
            	{
            		_last = (IASTNode)input.LT(1);
            		PushFollow(FOLLOW_path_in_intoClause348);
            		p = path();
            		state.followingStackPointer--;

            		adaptor.AddChild(root_1, p.Tree);

            	}

            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_insertablePropertySpec_in_intoClause353);
            	ps = insertablePropertySpec();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_1, ps.Tree);

            	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	}


            			retval.Tree =  CreateIntoClause(((p != null) ? p.p : default(String)), ((ps != null) ? ((IASTNode)ps.Tree) : null));
            		

            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "intoClause"

    public class insertablePropertySpec_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "insertablePropertySpec"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:89:1: insertablePropertySpec : ^( RANGE ( IDENT )+ ) ;
    public HqlSqlWalker.insertablePropertySpec_return insertablePropertySpec() // throws RecognitionException [1]
    {   
        HqlSqlWalker.insertablePropertySpec_return retval = new HqlSqlWalker.insertablePropertySpec_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode RANGE13 = null;
        IASTNode IDENT14 = null;

        IASTNode RANGE13_tree=null;
        IASTNode IDENT14_tree=null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:90:2: ( ^( RANGE ( IDENT )+ ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:90:4: ^( RANGE ( IDENT )+ )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	RANGE13=(IASTNode)Match(input,RANGE,FOLLOW_RANGE_in_insertablePropertySpec370); 
            		RANGE13_tree = (IASTNode)adaptor.DupNode(RANGE13);

            		root_1 = (IASTNode)adaptor.BecomeRoot(RANGE13_tree, root_1);



            	Match(input, Token.DOWN, null); 
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:90:13: ( IDENT )+
            	int cnt5 = 0;
            	do 
            	{
            	    int alt5 = 2;
            	    int LA5_0 = input.LA(1);

            	    if ( (LA5_0 == IDENT) )
            	    {
            	        alt5 = 1;
            	    }


            	    switch (alt5) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:90:14: IDENT
            			    {
            			    	_last = (IASTNode)input.LT(1);
            			    	IDENT14=(IASTNode)Match(input,IDENT,FOLLOW_IDENT_in_insertablePropertySpec373); 
            			    		IDENT14_tree = (IASTNode)adaptor.DupNode(IDENT14);

            			    		adaptor.AddChild(root_1, IDENT14_tree);


            			    }
            			    break;

            			default:
            			    if ( cnt5 >= 1 ) goto loop5;
            		            EarlyExitException eee5 =
            		                new EarlyExitException(5, input);
            		            throw eee5;
            	    }
            	    cnt5++;
            	} while (true);

            	loop5:
            		;	// Stops C# compiler whinging that label 'loop5' has no statements


            	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	}


            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "insertablePropertySpec"

    public class setClause_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "setClause"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:93:1: setClause : ^( SET ( assignment )* ) ;
    public HqlSqlWalker.setClause_return setClause() // throws RecognitionException [1]
    {   
        HqlSqlWalker.setClause_return retval = new HqlSqlWalker.setClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode SET15 = null;
        HqlSqlWalker.assignment_return assignment16 = default(HqlSqlWalker.assignment_return);


        IASTNode SET15_tree=null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:94:2: ( ^( SET ( assignment )* ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:94:4: ^( SET ( assignment )* )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	SET15=(IASTNode)Match(input,SET,FOLLOW_SET_in_setClause390); 
            		SET15_tree = (IASTNode)adaptor.DupNode(SET15);

            		root_1 = (IASTNode)adaptor.BecomeRoot(SET15_tree, root_1);


            	 HandleClauseStart( SET ); 

            	if ( input.LA(1) == Token.DOWN )
            	{
            	    Match(input, Token.DOWN, null); 
            	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:94:41: ( assignment )*
            	    do 
            	    {
            	        int alt6 = 2;
            	        int LA6_0 = input.LA(1);

            	        if ( (LA6_0 == EQ) )
            	        {
            	            alt6 = 1;
            	        }


            	        switch (alt6) 
            	    	{
            	    		case 1 :
            	    		    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:94:42: assignment
            	    		    {
            	    		    	_last = (IASTNode)input.LT(1);
            	    		    	PushFollow(FOLLOW_assignment_in_setClause395);
            	    		    	assignment16 = assignment();
            	    		    	state.followingStackPointer--;

            	    		    	adaptor.AddChild(root_1, assignment16.Tree);

            	    		    }
            	    		    break;

            	    		default:
            	    		    goto loop6;
            	        }
            	    } while (true);

            	    loop6:
            	    	;	// Stops C# compiler whining that label 'loop6' has no statements


            	    Match(input, Token.UP, null); 
            	}adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	}


            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "setClause"

    public class assignment_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "assignment"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:97:1: assignment : ^( EQ (p= propertyRef ) ( newValue ) ) ;
    public HqlSqlWalker.assignment_return assignment() // throws RecognitionException [1]
    {   
        HqlSqlWalker.assignment_return retval = new HqlSqlWalker.assignment_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode EQ17 = null;
        HqlSqlWalker.propertyRef_return p = default(HqlSqlWalker.propertyRef_return);

        HqlSqlWalker.newValue_return newValue18 = default(HqlSqlWalker.newValue_return);


        IASTNode EQ17_tree=null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:103:2: ( ^( EQ (p= propertyRef ) ( newValue ) ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:103:4: ^( EQ (p= propertyRef ) ( newValue ) )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	EQ17=(IASTNode)Match(input,EQ,FOLLOW_EQ_in_assignment422); 
            		EQ17_tree = (IASTNode)adaptor.DupNode(EQ17);

            		root_1 = (IASTNode)adaptor.BecomeRoot(EQ17_tree, root_1);



            	Match(input, Token.DOWN, null); 
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:103:10: (p= propertyRef )
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:103:11: p= propertyRef
            	{
            		_last = (IASTNode)input.LT(1);
            		PushFollow(FOLLOW_propertyRef_in_assignment427);
            		p = propertyRef();
            		state.followingStackPointer--;

            		adaptor.AddChild(root_1, p.Tree);

            	}

            	 Resolve(((p != null) ? ((IASTNode)p.Tree) : null)); 
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:103:48: ( newValue )
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:103:49: newValue
            	{
            		_last = (IASTNode)input.LT(1);
            		PushFollow(FOLLOW_newValue_in_assignment433);
            		newValue18 = newValue();
            		state.followingStackPointer--;

            		adaptor.AddChild(root_1, newValue18.Tree);

            	}


            	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	}


            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);


            		EvaluateAssignment( ((IASTNode)retval.Tree) );
            	
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
    // $ANTLR end "assignment"

    public class newValue_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "newValue"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:107:1: newValue : ( expr | query );
    public HqlSqlWalker.newValue_return newValue() // throws RecognitionException [1]
    {   
        HqlSqlWalker.newValue_return retval = new HqlSqlWalker.newValue_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        HqlSqlWalker.expr_return expr19 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.query_return query20 = default(HqlSqlWalker.query_return);



        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:108:2: ( expr | query )
            int alt7 = 2;
            int LA7_0 = input.LA(1);

            if ( (LA7_0 == COUNT || LA7_0 == DOT || LA7_0 == FALSE || LA7_0 == NULL || LA7_0 == TRUE || LA7_0 == CASE || LA7_0 == AGGREGATE || LA7_0 == CASE2 || LA7_0 == INDEX_OP || LA7_0 == METHOD_CALL || LA7_0 == UNARY_MINUS || (LA7_0 >= VECTOR_EXPR && LA7_0 <= WEIRD_IDENT) || (LA7_0 >= NUM_INT && LA7_0 <= JAVA_CONSTANT) || (LA7_0 >= PLUS && LA7_0 <= DIV) || (LA7_0 >= COLON && LA7_0 <= IDENT)) )
            {
                alt7 = 1;
            }
            else if ( (LA7_0 == QUERY) )
            {
                alt7 = 2;
            }
            else 
            {
                NoViableAltException nvae_d7s0 =
                    new NoViableAltException("", 7, 0, input);

                throw nvae_d7s0;
            }
            switch (alt7) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:108:4: expr
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_expr_in_newValue449);
                    	expr19 = expr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, expr19.Tree);

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:108:11: query
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_query_in_newValue453);
                    	query20 = query();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, query20.Tree);

                    }
                    break;

            }
            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "newValue"

    public class query_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "query"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:113:1: query : ^( QUERY ^( SELECT_FROM f= fromClause (s= selectClause )? ) (w= whereClause )? (g= groupClause )? (o= orderClause )? ) -> ^( SELECT ( $s)? $f ( $w)? ( $g)? ( $o)? ) ;
    public HqlSqlWalker.query_return query() // throws RecognitionException [1]
    {   
        HqlSqlWalker.query_return retval = new HqlSqlWalker.query_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode QUERY21 = null;
        IASTNode SELECT_FROM22 = null;
        HqlSqlWalker.fromClause_return f = default(HqlSqlWalker.fromClause_return);

        HqlSqlWalker.selectClause_return s = default(HqlSqlWalker.selectClause_return);

        HqlSqlWalker.whereClause_return w = default(HqlSqlWalker.whereClause_return);

        HqlSqlWalker.groupClause_return g = default(HqlSqlWalker.groupClause_return);

        HqlSqlWalker.orderClause_return o = default(HqlSqlWalker.orderClause_return);


        IASTNode QUERY21_tree=null;
        IASTNode SELECT_FROM22_tree=null;
        RewriteRuleNodeStream stream_QUERY = new RewriteRuleNodeStream(adaptor,"token QUERY");
        RewriteRuleNodeStream stream_SELECT_FROM = new RewriteRuleNodeStream(adaptor,"token SELECT_FROM");
        RewriteRuleSubtreeStream stream_selectClause = new RewriteRuleSubtreeStream(adaptor,"rule selectClause");
        RewriteRuleSubtreeStream stream_fromClause = new RewriteRuleSubtreeStream(adaptor,"rule fromClause");
        RewriteRuleSubtreeStream stream_orderClause = new RewriteRuleSubtreeStream(adaptor,"rule orderClause");
        RewriteRuleSubtreeStream stream_whereClause = new RewriteRuleSubtreeStream(adaptor,"rule whereClause");
        RewriteRuleSubtreeStream stream_groupClause = new RewriteRuleSubtreeStream(adaptor,"rule groupClause");
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:120:2: ( ^( QUERY ^( SELECT_FROM f= fromClause (s= selectClause )? ) (w= whereClause )? (g= groupClause )? (o= orderClause )? ) -> ^( SELECT ( $s)? $f ( $w)? ( $g)? ( $o)? ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:120:4: ^( QUERY ^( SELECT_FROM f= fromClause (s= selectClause )? ) (w= whereClause )? (g= groupClause )? (o= orderClause )? )
            {
            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	QUERY21=(IASTNode)Match(input,QUERY,FOLLOW_QUERY_in_query475);  
            	stream_QUERY.Add(QUERY21);


            	 BeforeStatement( "select", SELECT ); 

            	Match(input, Token.DOWN, null); 
            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_2 = _last;
            	IASTNode _first_2 = null;
            	IASTNode root_2 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	SELECT_FROM22=(IASTNode)Match(input,SELECT_FROM,FOLLOW_SELECT_FROM_in_query487);  
            	stream_SELECT_FROM.Add(SELECT_FROM22);



            	Match(input, Token.DOWN, null); 
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_fromClause_in_query495);
            	f = fromClause();
            	state.followingStackPointer--;

            	stream_fromClause.Add(f.Tree);
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:124:5: (s= selectClause )?
            	int alt8 = 2;
            	int LA8_0 = input.LA(1);

            	if ( (LA8_0 == SELECT) )
            	{
            	    alt8 = 1;
            	}
            	switch (alt8) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:124:6: s= selectClause
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_selectClause_in_query504);
            	        	s = selectClause();
            	        	state.followingStackPointer--;

            	        	stream_selectClause.Add(s.Tree);

            	        }
            	        break;

            	}


            	Match(input, Token.UP, null); adaptor.AddChild(root_1, root_2);_last = _save_last_2;
            	}

            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:126:4: (w= whereClause )?
            	int alt9 = 2;
            	int LA9_0 = input.LA(1);

            	if ( (LA9_0 == WHERE) )
            	{
            	    alt9 = 1;
            	}
            	switch (alt9) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:126:5: w= whereClause
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_whereClause_in_query519);
            	        	w = whereClause();
            	        	state.followingStackPointer--;

            	        	stream_whereClause.Add(w.Tree);

            	        }
            	        break;

            	}

            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:127:4: (g= groupClause )?
            	int alt10 = 2;
            	int LA10_0 = input.LA(1);

            	if ( (LA10_0 == GROUP) )
            	{
            	    alt10 = 1;
            	}
            	switch (alt10) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:127:5: g= groupClause
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_groupClause_in_query529);
            	        	g = groupClause();
            	        	state.followingStackPointer--;

            	        	stream_groupClause.Add(g.Tree);

            	        }
            	        break;

            	}

            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:128:4: (o= orderClause )?
            	int alt11 = 2;
            	int LA11_0 = input.LA(1);

            	if ( (LA11_0 == ORDER) )
            	{
            	    alt11 = 1;
            	}
            	switch (alt11) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:128:5: o= orderClause
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_orderClause_in_query539);
            	        	o = orderClause();
            	        	state.followingStackPointer--;

            	        	stream_orderClause.Add(o.Tree);

            	        }
            	        break;

            	}


            	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	}



            	// AST REWRITE
            	// elements:          g, s, w, f, o
            	// token labels:      
            	// rule labels:       o, w, f, retval, g, s
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_o = new RewriteRuleSubtreeStream(adaptor, "rule o", o!=null ? o.Tree : null);
            	RewriteRuleSubtreeStream stream_w = new RewriteRuleSubtreeStream(adaptor, "rule w", w!=null ? w.Tree : null);
            	RewriteRuleSubtreeStream stream_f = new RewriteRuleSubtreeStream(adaptor, "rule f", f!=null ? f.Tree : null);
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);
            	RewriteRuleSubtreeStream stream_g = new RewriteRuleSubtreeStream(adaptor, "rule g", g!=null ? g.Tree : null);
            	RewriteRuleSubtreeStream stream_s = new RewriteRuleSubtreeStream(adaptor, "rule s", s!=null ? s.Tree : null);

            	root_0 = (IASTNode)adaptor.GetNilNode();
            	// 130:2: -> ^( SELECT ( $s)? $f ( $w)? ( $g)? ( $o)? )
            	{
            	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:130:5: ^( SELECT ( $s)? $f ( $w)? ( $g)? ( $o)? )
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(SELECT, "SELECT"), root_1);

            	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:130:14: ( $s)?
            	    if ( stream_s.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_s.NextTree());

            	    }
            	    stream_s.Reset();
            	    adaptor.AddChild(root_1, stream_f.NextTree());
            	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:130:21: ( $w)?
            	    if ( stream_w.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_w.NextTree());

            	    }
            	    stream_w.Reset();
            	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:130:25: ( $g)?
            	    if ( stream_g.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_g.NextTree());

            	    }
            	    stream_g.Reset();
            	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:130:29: ( $o)?
            	    if ( stream_o.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_o.NextTree());

            	    }
            	    stream_o.Reset();

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;
            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);


            		// Antlr note: #x_in refers to the input AST, #x refers to the output AST
            		BeforeStatementCompletion( "select" );
            		ProcessQuery( ((s != null) ? ((IASTNode)s.Tree) : null), ((IASTNode)retval.Tree) );
            		AfterStatementCompletion( "select" );
            	
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
    // $ANTLR end "query"

    public class orderClause_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "orderClause"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:133:1: orderClause : ^( ORDER orderExprs ) ;
    public HqlSqlWalker.orderClause_return orderClause() // throws RecognitionException [1]
    {   
        HqlSqlWalker.orderClause_return retval = new HqlSqlWalker.orderClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode ORDER23 = null;
        HqlSqlWalker.orderExprs_return orderExprs24 = default(HqlSqlWalker.orderExprs_return);


        IASTNode ORDER23_tree=null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:134:2: ( ^( ORDER orderExprs ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:134:4: ^( ORDER orderExprs )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	ORDER23=(IASTNode)Match(input,ORDER,FOLLOW_ORDER_in_orderClause584); 
            		ORDER23_tree = (IASTNode)adaptor.DupNode(ORDER23);

            		root_1 = (IASTNode)adaptor.BecomeRoot(ORDER23_tree, root_1);


            	 HandleClauseStart( ORDER ); 

            	Match(input, Token.DOWN, null); 
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_orderExprs_in_orderClause588);
            	orderExprs24 = orderExprs();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_1, orderExprs24.Tree);

            	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	}


            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "orderClause"

    public class orderExprs_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "orderExprs"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:137:1: orderExprs : expr ( ASCENDING | DESCENDING )? ( orderExprs )? ;
    public HqlSqlWalker.orderExprs_return orderExprs() // throws RecognitionException [1]
    {   
        HqlSqlWalker.orderExprs_return retval = new HqlSqlWalker.orderExprs_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode set26 = null;
        HqlSqlWalker.expr_return expr25 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.orderExprs_return orderExprs27 = default(HqlSqlWalker.orderExprs_return);


        IASTNode set26_tree=null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:138:2: ( expr ( ASCENDING | DESCENDING )? ( orderExprs )? )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:138:4: expr ( ASCENDING | DESCENDING )? ( orderExprs )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_expr_in_orderExprs600);
            	expr25 = expr();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, expr25.Tree);
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:138:9: ( ASCENDING | DESCENDING )?
            	int alt12 = 2;
            	int LA12_0 = input.LA(1);

            	if ( (LA12_0 == ASCENDING || LA12_0 == DESCENDING) )
            	{
            	    alt12 = 1;
            	}
            	switch (alt12) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	set26 = (IASTNode)input.LT(1);
            	        	if ( input.LA(1) == ASCENDING || input.LA(1) == DESCENDING ) 
            	        	{
            	        	    input.Consume();

            	        	    set26_tree = (IASTNode)adaptor.DupNode(set26);

            	        	    adaptor.AddChild(root_0, set26_tree);

            	        	    state.errorRecovery = false;
            	        	}
            	        	else 
            	        	{
            	        	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	        	    throw mse;
            	        	}


            	        }
            	        break;

            	}

            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:138:37: ( orderExprs )?
            	int alt13 = 2;
            	int LA13_0 = input.LA(1);

            	if ( (LA13_0 == COUNT || LA13_0 == DOT || LA13_0 == FALSE || LA13_0 == NULL || LA13_0 == TRUE || LA13_0 == CASE || LA13_0 == AGGREGATE || LA13_0 == CASE2 || LA13_0 == INDEX_OP || LA13_0 == METHOD_CALL || LA13_0 == UNARY_MINUS || (LA13_0 >= VECTOR_EXPR && LA13_0 <= WEIRD_IDENT) || (LA13_0 >= NUM_INT && LA13_0 <= JAVA_CONSTANT) || (LA13_0 >= PLUS && LA13_0 <= DIV) || (LA13_0 >= COLON && LA13_0 <= IDENT)) )
            	{
            	    alt13 = 1;
            	}
            	switch (alt13) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:138:38: orderExprs
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_orderExprs_in_orderExprs614);
            	        	orderExprs27 = orderExprs();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, orderExprs27.Tree);

            	        }
            	        break;

            	}


            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "orderExprs"

    public class groupClause_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "groupClause"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:141:1: groupClause : ^( GROUP ( expr )+ ( ^( HAVING logicalExpr ) )? ) ;
    public HqlSqlWalker.groupClause_return groupClause() // throws RecognitionException [1]
    {   
        HqlSqlWalker.groupClause_return retval = new HqlSqlWalker.groupClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode GROUP28 = null;
        IASTNode HAVING30 = null;
        HqlSqlWalker.expr_return expr29 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.logicalExpr_return logicalExpr31 = default(HqlSqlWalker.logicalExpr_return);


        IASTNode GROUP28_tree=null;
        IASTNode HAVING30_tree=null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:142:2: ( ^( GROUP ( expr )+ ( ^( HAVING logicalExpr ) )? ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:142:4: ^( GROUP ( expr )+ ( ^( HAVING logicalExpr ) )? )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	GROUP28=(IASTNode)Match(input,GROUP,FOLLOW_GROUP_in_groupClause628); 
            		GROUP28_tree = (IASTNode)adaptor.DupNode(GROUP28);

            		root_1 = (IASTNode)adaptor.BecomeRoot(GROUP28_tree, root_1);


            	 HandleClauseStart( GROUP ); 

            	Match(input, Token.DOWN, null); 
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:142:44: ( expr )+
            	int cnt14 = 0;
            	do 
            	{
            	    int alt14 = 2;
            	    int LA14_0 = input.LA(1);

            	    if ( (LA14_0 == COUNT || LA14_0 == DOT || LA14_0 == FALSE || LA14_0 == NULL || LA14_0 == TRUE || LA14_0 == CASE || LA14_0 == AGGREGATE || LA14_0 == CASE2 || LA14_0 == INDEX_OP || LA14_0 == METHOD_CALL || LA14_0 == UNARY_MINUS || (LA14_0 >= VECTOR_EXPR && LA14_0 <= WEIRD_IDENT) || (LA14_0 >= NUM_INT && LA14_0 <= JAVA_CONSTANT) || (LA14_0 >= PLUS && LA14_0 <= DIV) || (LA14_0 >= COLON && LA14_0 <= IDENT)) )
            	    {
            	        alt14 = 1;
            	    }


            	    switch (alt14) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:142:45: expr
            			    {
            			    	_last = (IASTNode)input.LT(1);
            			    	PushFollow(FOLLOW_expr_in_groupClause633);
            			    	expr29 = expr();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_1, expr29.Tree);

            			    }
            			    break;

            			default:
            			    if ( cnt14 >= 1 ) goto loop14;
            		            EarlyExitException eee14 =
            		                new EarlyExitException(14, input);
            		            throw eee14;
            	    }
            	    cnt14++;
            	} while (true);

            	loop14:
            		;	// Stops C# compiler whinging that label 'loop14' has no statements

            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:142:52: ( ^( HAVING logicalExpr ) )?
            	int alt15 = 2;
            	int LA15_0 = input.LA(1);

            	if ( (LA15_0 == HAVING) )
            	{
            	    alt15 = 1;
            	}
            	switch (alt15) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:142:54: ^( HAVING logicalExpr )
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	{
            	        	IASTNode _save_last_2 = _last;
            	        	IASTNode _first_2 = null;
            	        	IASTNode root_2 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	HAVING30=(IASTNode)Match(input,HAVING,FOLLOW_HAVING_in_groupClause640); 
            	        		HAVING30_tree = (IASTNode)adaptor.DupNode(HAVING30);

            	        		root_2 = (IASTNode)adaptor.BecomeRoot(HAVING30_tree, root_2);



            	        	Match(input, Token.DOWN, null); 
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_logicalExpr_in_groupClause642);
            	        	logicalExpr31 = logicalExpr();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_2, logicalExpr31.Tree);

            	        	Match(input, Token.UP, null); adaptor.AddChild(root_1, root_2);_last = _save_last_2;
            	        	}


            	        }
            	        break;

            	}


            	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	}


            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "groupClause"

    public class selectClause_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "selectClause"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:145:1: selectClause : ^( SELECT (d= DISTINCT )? x= selectExprList ) -> ^( SELECT_CLAUSE[\"{select clause}\"] ( $d)? $x) ;
    public HqlSqlWalker.selectClause_return selectClause() // throws RecognitionException [1]
    {   
        HqlSqlWalker.selectClause_return retval = new HqlSqlWalker.selectClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode d = null;
        IASTNode SELECT32 = null;
        HqlSqlWalker.selectExprList_return x = default(HqlSqlWalker.selectExprList_return);


        IASTNode d_tree=null;
        IASTNode SELECT32_tree=null;
        RewriteRuleNodeStream stream_DISTINCT = new RewriteRuleNodeStream(adaptor,"token DISTINCT");
        RewriteRuleNodeStream stream_SELECT = new RewriteRuleNodeStream(adaptor,"token SELECT");
        RewriteRuleSubtreeStream stream_selectExprList = new RewriteRuleSubtreeStream(adaptor,"rule selectExprList");
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:146:2: ( ^( SELECT (d= DISTINCT )? x= selectExprList ) -> ^( SELECT_CLAUSE[\"{select clause}\"] ( $d)? $x) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:146:4: ^( SELECT (d= DISTINCT )? x= selectExprList )
            {
            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	SELECT32=(IASTNode)Match(input,SELECT,FOLLOW_SELECT_in_selectClause661);  
            	stream_SELECT.Add(SELECT32);


            	 HandleClauseStart( SELECT ); BeforeSelectClause(); 

            	Match(input, Token.DOWN, null); 
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:146:68: (d= DISTINCT )?
            	int alt16 = 2;
            	int LA16_0 = input.LA(1);

            	if ( (LA16_0 == DISTINCT) )
            	{
            	    alt16 = 1;
            	}
            	switch (alt16) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:146:69: d= DISTINCT
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	d=(IASTNode)Match(input,DISTINCT,FOLLOW_DISTINCT_in_selectClause668);  
            	        	stream_DISTINCT.Add(d);


            	        }
            	        break;

            	}

            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_selectExprList_in_selectClause674);
            	x = selectExprList();
            	state.followingStackPointer--;

            	stream_selectExprList.Add(x.Tree);

            	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	}



            	// AST REWRITE
            	// elements:          d, x
            	// token labels:      d
            	// rule labels:       retval, x
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	retval.Tree = root_0;
            	RewriteRuleNodeStream stream_d = new RewriteRuleNodeStream(adaptor, "token d", d);
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);
            	RewriteRuleSubtreeStream stream_x = new RewriteRuleSubtreeStream(adaptor, "rule x", x!=null ? x.Tree : null);

            	root_0 = (IASTNode)adaptor.GetNilNode();
            	// 147:2: -> ^( SELECT_CLAUSE[\"{select clause}\"] ( $d)? $x)
            	{
            	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:147:5: ^( SELECT_CLAUSE[\"{select clause}\"] ( $d)? $x)
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(SELECT_CLAUSE, "{select clause}"), root_1);

            	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:147:40: ( $d)?
            	    if ( stream_d.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_d.NextNode());

            	    }
            	    stream_d.Reset();
            	    adaptor.AddChild(root_1, stream_x.NextTree());

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;
            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "selectClause"

    public class selectExprList_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "selectExprList"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:150:1: selectExprList : ( selectExpr | aliasedSelectExpr )+ ;
    public HqlSqlWalker.selectExprList_return selectExprList() // throws RecognitionException [1]
    {   
        HqlSqlWalker.selectExprList_return retval = new HqlSqlWalker.selectExprList_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        HqlSqlWalker.selectExpr_return selectExpr33 = default(HqlSqlWalker.selectExpr_return);

        HqlSqlWalker.aliasedSelectExpr_return aliasedSelectExpr34 = default(HqlSqlWalker.aliasedSelectExpr_return);




        		bool oldInSelect = _inSelect;
        		_inSelect = true;
        	
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:154:2: ( ( selectExpr | aliasedSelectExpr )+ )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:154:4: ( selectExpr | aliasedSelectExpr )+
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:154:4: ( selectExpr | aliasedSelectExpr )+
            	int cnt17 = 0;
            	do 
            	{
            	    int alt17 = 3;
            	    int LA17_0 = input.LA(1);

            	    if ( (LA17_0 == ALL || LA17_0 == COUNT || LA17_0 == DOT || LA17_0 == ELEMENTS || LA17_0 == INDICES || LA17_0 == CASE || LA17_0 == OBJECT || LA17_0 == AGGREGATE || (LA17_0 >= CONSTRUCTOR && LA17_0 <= CASE2) || LA17_0 == METHOD_CALL || LA17_0 == QUERY || LA17_0 == UNARY_MINUS || LA17_0 == WEIRD_IDENT || (LA17_0 >= NUM_INT && LA17_0 <= NUM_LONG) || (LA17_0 >= PLUS && LA17_0 <= DIV) || (LA17_0 >= QUOTED_String && LA17_0 <= IDENT)) )
            	    {
            	        alt17 = 1;
            	    }
            	    else if ( (LA17_0 == AS) )
            	    {
            	        alt17 = 2;
            	    }


            	    switch (alt17) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:154:6: selectExpr
            			    {
            			    	_last = (IASTNode)input.LT(1);
            			    	PushFollow(FOLLOW_selectExpr_in_selectExprList709);
            			    	selectExpr33 = selectExpr();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, selectExpr33.Tree);

            			    }
            			    break;
            			case 2 :
            			    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:154:19: aliasedSelectExpr
            			    {
            			    	_last = (IASTNode)input.LT(1);
            			    	PushFollow(FOLLOW_aliasedSelectExpr_in_selectExprList713);
            			    	aliasedSelectExpr34 = aliasedSelectExpr();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, aliasedSelectExpr34.Tree);

            			    }
            			    break;

            			default:
            			    if ( cnt17 >= 1 ) goto loop17;
            		            EarlyExitException eee17 =
            		                new EarlyExitException(17, input);
            		            throw eee17;
            	    }
            	    cnt17++;
            	} while (true);

            	loop17:
            		;	// Stops C# compiler whinging that label 'loop17' has no statements


            			_inSelect = oldInSelect;
            		

            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "selectExprList"

    public class aliasedSelectExpr_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "aliasedSelectExpr"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:159:1: aliasedSelectExpr : ^( AS se= selectExpr i= identifier ) ;
    public HqlSqlWalker.aliasedSelectExpr_return aliasedSelectExpr() // throws RecognitionException [1]
    {   
        HqlSqlWalker.aliasedSelectExpr_return retval = new HqlSqlWalker.aliasedSelectExpr_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode AS35 = null;
        HqlSqlWalker.selectExpr_return se = default(HqlSqlWalker.selectExpr_return);

        HqlSqlWalker.identifier_return i = default(HqlSqlWalker.identifier_return);


        IASTNode AS35_tree=null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:164:2: ( ^( AS se= selectExpr i= identifier ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:164:4: ^( AS se= selectExpr i= identifier )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	AS35=(IASTNode)Match(input,AS,FOLLOW_AS_in_aliasedSelectExpr737); 
            		AS35_tree = (IASTNode)adaptor.DupNode(AS35);

            		root_1 = (IASTNode)adaptor.BecomeRoot(AS35_tree, root_1);



            	Match(input, Token.DOWN, null); 
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_selectExpr_in_aliasedSelectExpr741);
            	se = selectExpr();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_1, se.Tree);
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_identifier_in_aliasedSelectExpr745);
            	i = identifier();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_1, i.Tree);

            	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	}


            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);


            	    SetAlias(((se != null) ? ((IASTNode)se.Tree) : null),((i != null) ? ((IASTNode)i.Tree) : null));
            	    retval.Tree =  ((se != null) ? ((IASTNode)se.Tree) : null);
            	
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
    // $ANTLR end "aliasedSelectExpr"

    public class selectExpr_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "selectExpr"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:167:1: selectExpr : (p= propertyRef | ^( ALL ar2= aliasRef ) | ^( OBJECT ar3= aliasRef ) | con= constructor | functionCall | count | collectionFunction | literal | arithmeticExpr | query );
    public HqlSqlWalker.selectExpr_return selectExpr() // throws RecognitionException [1]
    {   
        HqlSqlWalker.selectExpr_return retval = new HqlSqlWalker.selectExpr_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode ALL36 = null;
        IASTNode OBJECT37 = null;
        HqlSqlWalker.propertyRef_return p = default(HqlSqlWalker.propertyRef_return);

        HqlSqlWalker.aliasRef_return ar2 = default(HqlSqlWalker.aliasRef_return);

        HqlSqlWalker.aliasRef_return ar3 = default(HqlSqlWalker.aliasRef_return);

        HqlSqlWalker.constructor_return con = default(HqlSqlWalker.constructor_return);

        HqlSqlWalker.functionCall_return functionCall38 = default(HqlSqlWalker.functionCall_return);

        HqlSqlWalker.count_return count39 = default(HqlSqlWalker.count_return);

        HqlSqlWalker.collectionFunction_return collectionFunction40 = default(HqlSqlWalker.collectionFunction_return);

        HqlSqlWalker.literal_return literal41 = default(HqlSqlWalker.literal_return);

        HqlSqlWalker.arithmeticExpr_return arithmeticExpr42 = default(HqlSqlWalker.arithmeticExpr_return);

        HqlSqlWalker.query_return query43 = default(HqlSqlWalker.query_return);


        IASTNode ALL36_tree=null;
        IASTNode OBJECT37_tree=null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:168:2: (p= propertyRef | ^( ALL ar2= aliasRef ) | ^( OBJECT ar3= aliasRef ) | con= constructor | functionCall | count | collectionFunction | literal | arithmeticExpr | query )
            int alt18 = 10;
            switch ( input.LA(1) ) 
            {
            case DOT:
            case WEIRD_IDENT:
            case IDENT:
            	{
                alt18 = 1;
                }
                break;
            case ALL:
            	{
                alt18 = 2;
                }
                break;
            case OBJECT:
            	{
                alt18 = 3;
                }
                break;
            case CONSTRUCTOR:
            	{
                alt18 = 4;
                }
                break;
            case AGGREGATE:
            case METHOD_CALL:
            	{
                alt18 = 5;
                }
                break;
            case COUNT:
            	{
                alt18 = 6;
                }
                break;
            case ELEMENTS:
            case INDICES:
            	{
                alt18 = 7;
                }
                break;
            case NUM_INT:
            case NUM_DOUBLE:
            case NUM_FLOAT:
            case NUM_LONG:
            case QUOTED_String:
            	{
                alt18 = 8;
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
                alt18 = 9;
                }
                break;
            case QUERY:
            	{
                alt18 = 10;
                }
                break;
            	default:
            	    NoViableAltException nvae_d18s0 =
            	        new NoViableAltException("", 18, 0, input);

            	    throw nvae_d18s0;
            }

            switch (alt18) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:168:4: p= propertyRef
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_propertyRef_in_selectExpr760);
                    	p = propertyRef();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, p.Tree);
                    	 ResolveSelectExpression(((p != null) ? ((IASTNode)p.Tree) : null)); 

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:169:4: ^( ALL ar2= aliasRef )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	ALL36=(IASTNode)Match(input,ALL,FOLLOW_ALL_in_selectExpr772); 
                    		ALL36_tree = (IASTNode)adaptor.DupNode(ALL36);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(ALL36_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_aliasRef_in_selectExpr776);
                    	ar2 = aliasRef();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, ar2.Tree);

                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}

                    	 ResolveSelectExpression(((ar2 != null) ? ((IASTNode)ar2.Tree) : null)); retval.Tree =  ((ar2 != null) ? ((IASTNode)ar2.Tree) : null); 

                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:170:4: ^( OBJECT ar3= aliasRef )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	OBJECT37=(IASTNode)Match(input,OBJECT,FOLLOW_OBJECT_in_selectExpr788); 
                    		OBJECT37_tree = (IASTNode)adaptor.DupNode(OBJECT37);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(OBJECT37_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_aliasRef_in_selectExpr792);
                    	ar3 = aliasRef();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, ar3.Tree);

                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}

                    	 ResolveSelectExpression(((ar3 != null) ? ((IASTNode)ar3.Tree) : null)); retval.Tree =  ((ar3 != null) ? ((IASTNode)ar3.Tree) : null); 

                    }
                    break;
                case 4 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:171:4: con= constructor
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_constructor_in_selectExpr803);
                    	con = constructor();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, con.Tree);
                    	 ProcessConstructor(((con != null) ? ((IASTNode)con.Tree) : null)); 

                    }
                    break;
                case 5 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:172:4: functionCall
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_functionCall_in_selectExpr814);
                    	functionCall38 = functionCall();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, functionCall38.Tree);

                    }
                    break;
                case 6 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:173:4: count
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_count_in_selectExpr819);
                    	count39 = count();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, count39.Tree);

                    }
                    break;
                case 7 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:174:4: collectionFunction
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_collectionFunction_in_selectExpr824);
                    	collectionFunction40 = collectionFunction();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, collectionFunction40.Tree);

                    }
                    break;
                case 8 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:175:4: literal
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_literal_in_selectExpr832);
                    	literal41 = literal();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, literal41.Tree);

                    }
                    break;
                case 9 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:176:4: arithmeticExpr
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_arithmeticExpr_in_selectExpr837);
                    	arithmeticExpr42 = arithmeticExpr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, arithmeticExpr42.Tree);

                    }
                    break;
                case 10 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:177:4: query
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_query_in_selectExpr842);
                    	query43 = query();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, query43.Tree);

                    }
                    break;

            }
            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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

    public class count_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "count"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:180:1: count : ^( COUNT ( DISTINCT | ALL )? ( aggregateExpr | ROW_STAR ) ) ;
    public HqlSqlWalker.count_return count() // throws RecognitionException [1]
    {   
        HqlSqlWalker.count_return retval = new HqlSqlWalker.count_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode COUNT44 = null;
        IASTNode set45 = null;
        IASTNode ROW_STAR47 = null;
        HqlSqlWalker.aggregateExpr_return aggregateExpr46 = default(HqlSqlWalker.aggregateExpr_return);


        IASTNode COUNT44_tree=null;
        IASTNode set45_tree=null;
        IASTNode ROW_STAR47_tree=null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:181:2: ( ^( COUNT ( DISTINCT | ALL )? ( aggregateExpr | ROW_STAR ) ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:181:4: ^( COUNT ( DISTINCT | ALL )? ( aggregateExpr | ROW_STAR ) )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	COUNT44=(IASTNode)Match(input,COUNT,FOLLOW_COUNT_in_count854); 
            		COUNT44_tree = (IASTNode)adaptor.DupNode(COUNT44);

            		root_1 = (IASTNode)adaptor.BecomeRoot(COUNT44_tree, root_1);



            	Match(input, Token.DOWN, null); 
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:181:12: ( DISTINCT | ALL )?
            	int alt19 = 2;
            	int LA19_0 = input.LA(1);

            	if ( (LA19_0 == ALL || LA19_0 == DISTINCT) )
            	{
            	    alt19 = 1;
            	}
            	switch (alt19) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	set45 = (IASTNode)input.LT(1);
            	        	if ( input.LA(1) == ALL || input.LA(1) == DISTINCT ) 
            	        	{
            	        	    input.Consume();

            	        	    set45_tree = (IASTNode)adaptor.DupNode(set45);

            	        	    adaptor.AddChild(root_1, set45_tree);

            	        	    state.errorRecovery = false;
            	        	}
            	        	else 
            	        	{
            	        	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	        	    throw mse;
            	        	}


            	        }
            	        break;

            	}

            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:181:32: ( aggregateExpr | ROW_STAR )
            	int alt20 = 2;
            	int LA20_0 = input.LA(1);

            	if ( (LA20_0 == COUNT || LA20_0 == DOT || LA20_0 == ELEMENTS || LA20_0 == FALSE || LA20_0 == INDICES || LA20_0 == NULL || LA20_0 == TRUE || LA20_0 == CASE || LA20_0 == AGGREGATE || LA20_0 == CASE2 || LA20_0 == INDEX_OP || LA20_0 == METHOD_CALL || LA20_0 == UNARY_MINUS || (LA20_0 >= VECTOR_EXPR && LA20_0 <= WEIRD_IDENT) || (LA20_0 >= NUM_INT && LA20_0 <= JAVA_CONSTANT) || (LA20_0 >= PLUS && LA20_0 <= DIV) || (LA20_0 >= COLON && LA20_0 <= IDENT)) )
            	{
            	    alt20 = 1;
            	}
            	else if ( (LA20_0 == ROW_STAR) )
            	{
            	    alt20 = 2;
            	}
            	else 
            	{
            	    NoViableAltException nvae_d20s0 =
            	        new NoViableAltException("", 20, 0, input);

            	    throw nvae_d20s0;
            	}
            	switch (alt20) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:181:34: aggregateExpr
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_aggregateExpr_in_count869);
            	        	aggregateExpr46 = aggregateExpr();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, aggregateExpr46.Tree);

            	        }
            	        break;
            	    case 2 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:181:50: ROW_STAR
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	ROW_STAR47=(IASTNode)Match(input,ROW_STAR,FOLLOW_ROW_STAR_in_count873); 
            	        		ROW_STAR47_tree = (IASTNode)adaptor.DupNode(ROW_STAR47);

            	        		adaptor.AddChild(root_1, ROW_STAR47_tree);


            	        }
            	        break;

            	}


            	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	}


            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "count"

    public class constructor_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "constructor"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:184:1: constructor : ^( CONSTRUCTOR path ( selectExpr | aliasedSelectExpr )* ) ;
    public HqlSqlWalker.constructor_return constructor() // throws RecognitionException [1]
    {   
        HqlSqlWalker.constructor_return retval = new HqlSqlWalker.constructor_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode CONSTRUCTOR48 = null;
        HqlSqlWalker.path_return path49 = default(HqlSqlWalker.path_return);

        HqlSqlWalker.selectExpr_return selectExpr50 = default(HqlSqlWalker.selectExpr_return);

        HqlSqlWalker.aliasedSelectExpr_return aliasedSelectExpr51 = default(HqlSqlWalker.aliasedSelectExpr_return);


        IASTNode CONSTRUCTOR48_tree=null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:185:2: ( ^( CONSTRUCTOR path ( selectExpr | aliasedSelectExpr )* ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:185:4: ^( CONSTRUCTOR path ( selectExpr | aliasedSelectExpr )* )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	CONSTRUCTOR48=(IASTNode)Match(input,CONSTRUCTOR,FOLLOW_CONSTRUCTOR_in_constructor889); 
            		CONSTRUCTOR48_tree = (IASTNode)adaptor.DupNode(CONSTRUCTOR48);

            		root_1 = (IASTNode)adaptor.BecomeRoot(CONSTRUCTOR48_tree, root_1);



            	Match(input, Token.DOWN, null); 
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_path_in_constructor891);
            	path49 = path();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_1, path49.Tree);
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:185:23: ( selectExpr | aliasedSelectExpr )*
            	do 
            	{
            	    int alt21 = 3;
            	    int LA21_0 = input.LA(1);

            	    if ( (LA21_0 == ALL || LA21_0 == COUNT || LA21_0 == DOT || LA21_0 == ELEMENTS || LA21_0 == INDICES || LA21_0 == CASE || LA21_0 == OBJECT || LA21_0 == AGGREGATE || (LA21_0 >= CONSTRUCTOR && LA21_0 <= CASE2) || LA21_0 == METHOD_CALL || LA21_0 == QUERY || LA21_0 == UNARY_MINUS || LA21_0 == WEIRD_IDENT || (LA21_0 >= NUM_INT && LA21_0 <= NUM_LONG) || (LA21_0 >= PLUS && LA21_0 <= DIV) || (LA21_0 >= QUOTED_String && LA21_0 <= IDENT)) )
            	    {
            	        alt21 = 1;
            	    }
            	    else if ( (LA21_0 == AS) )
            	    {
            	        alt21 = 2;
            	    }


            	    switch (alt21) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:185:25: selectExpr
            			    {
            			    	_last = (IASTNode)input.LT(1);
            			    	PushFollow(FOLLOW_selectExpr_in_constructor895);
            			    	selectExpr50 = selectExpr();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_1, selectExpr50.Tree);

            			    }
            			    break;
            			case 2 :
            			    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:185:38: aliasedSelectExpr
            			    {
            			    	_last = (IASTNode)input.LT(1);
            			    	PushFollow(FOLLOW_aliasedSelectExpr_in_constructor899);
            			    	aliasedSelectExpr51 = aliasedSelectExpr();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_1, aliasedSelectExpr51.Tree);

            			    }
            			    break;

            			default:
            			    goto loop21;
            	    }
            	} while (true);

            	loop21:
            		;	// Stops C# compiler whining that label 'loop21' has no statements


            	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	}


            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "constructor"

    public class aggregateExpr_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "aggregateExpr"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:188:1: aggregateExpr : ( expr | collectionFunction );
    public HqlSqlWalker.aggregateExpr_return aggregateExpr() // throws RecognitionException [1]
    {   
        HqlSqlWalker.aggregateExpr_return retval = new HqlSqlWalker.aggregateExpr_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        HqlSqlWalker.expr_return expr52 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.collectionFunction_return collectionFunction53 = default(HqlSqlWalker.collectionFunction_return);



        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:189:2: ( expr | collectionFunction )
            int alt22 = 2;
            int LA22_0 = input.LA(1);

            if ( (LA22_0 == COUNT || LA22_0 == DOT || LA22_0 == FALSE || LA22_0 == NULL || LA22_0 == TRUE || LA22_0 == CASE || LA22_0 == AGGREGATE || LA22_0 == CASE2 || LA22_0 == INDEX_OP || LA22_0 == METHOD_CALL || LA22_0 == UNARY_MINUS || (LA22_0 >= VECTOR_EXPR && LA22_0 <= WEIRD_IDENT) || (LA22_0 >= NUM_INT && LA22_0 <= JAVA_CONSTANT) || (LA22_0 >= PLUS && LA22_0 <= DIV) || (LA22_0 >= COLON && LA22_0 <= IDENT)) )
            {
                alt22 = 1;
            }
            else if ( (LA22_0 == ELEMENTS || LA22_0 == INDICES) )
            {
                alt22 = 2;
            }
            else 
            {
                NoViableAltException nvae_d22s0 =
                    new NoViableAltException("", 22, 0, input);

                throw nvae_d22s0;
            }
            switch (alt22) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:189:4: expr
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_expr_in_aggregateExpr915);
                    	expr52 = expr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, expr52.Tree);

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:190:4: collectionFunction
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_collectionFunction_in_aggregateExpr921);
                    	collectionFunction53 = collectionFunction();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, collectionFunction53.Tree);

                    }
                    break;

            }
            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "aggregateExpr"

    public class fromClause_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "fromClause"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:194:1: fromClause : ^(f= FROM fromElementList ) ;
    public HqlSqlWalker.fromClause_return fromClause() // throws RecognitionException [1]
    {   
        HqlSqlWalker.fromClause_return retval = new HqlSqlWalker.fromClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode f = null;
        HqlSqlWalker.fromElementList_return fromElementList54 = default(HqlSqlWalker.fromElementList_return);


        IASTNode f_tree=null;


        		// NOTE: This references the INPUT AST! (see http://www.antlr.org/doc/trees.html#Action Translation)
        		// the ouput AST (#fromClause) has not been built yet.
        		PrepareFromClauseInputTree((IASTNode) input.LT(1), input);
        	
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:200:2: ( ^(f= FROM fromElementList ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:200:4: ^(f= FROM fromElementList )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	f=(IASTNode)Match(input,FROM,FOLLOW_FROM_in_fromClause941); 
            		f_tree = (IASTNode)adaptor.DupNode(f);

            		root_1 = (IASTNode)adaptor.BecomeRoot(f_tree, root_1);


            	 PushFromClause(f_tree); HandleClauseStart( FROM ); 

            	Match(input, Token.DOWN, null); 
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_fromElementList_in_fromClause945);
            	fromElementList54 = fromElementList();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_1, fromElementList54.Tree);

            	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	}


            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "fromClause"

    public class fromElementList_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "fromElementList"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:203:1: fromElementList : ( fromElement )+ ;
    public HqlSqlWalker.fromElementList_return fromElementList() // throws RecognitionException [1]
    {   
        HqlSqlWalker.fromElementList_return retval = new HqlSqlWalker.fromElementList_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        HqlSqlWalker.fromElement_return fromElement55 = default(HqlSqlWalker.fromElement_return);




        		bool oldInFrom = _inFrom;
        		_inFrom = true;
        		
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:207:2: ( ( fromElement )+ )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:207:4: ( fromElement )+
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:207:4: ( fromElement )+
            	int cnt23 = 0;
            	do 
            	{
            	    int alt23 = 2;
            	    int LA23_0 = input.LA(1);

            	    if ( (LA23_0 == JOIN || LA23_0 == FILTER_ENTITY || LA23_0 == RANGE) )
            	    {
            	        alt23 = 1;
            	    }


            	    switch (alt23) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:207:5: fromElement
            			    {
            			    	_last = (IASTNode)input.LT(1);
            			    	PushFollow(FOLLOW_fromElement_in_fromElementList963);
            			    	fromElement55 = fromElement();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, fromElement55.Tree);

            			    }
            			    break;

            			default:
            			    if ( cnt23 >= 1 ) goto loop23;
            		            EarlyExitException eee23 =
            		                new EarlyExitException(23, input);
            		            throw eee23;
            	    }
            	    cnt23++;
            	} while (true);

            	loop23:
            		;	// Stops C# compiler whinging that label 'loop23' has no statements


            			_inFrom = oldInFrom;
            			

            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "fromElementList"

    public class fromElement_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "fromElement"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:212:1: fromElement : ( ^( RANGE p= path (a= ALIAS )? (pf= FETCH )? ) -> {fromElement != null}? ^() -> | je= joinElement -> | fe= FILTER_ENTITY a3= ALIAS -> ^() );
    public HqlSqlWalker.fromElement_return fromElement() // throws RecognitionException [1]
    {   
        HqlSqlWalker.fromElement_return retval = new HqlSqlWalker.fromElement_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode a = null;
        IASTNode pf = null;
        IASTNode fe = null;
        IASTNode a3 = null;
        IASTNode RANGE56 = null;
        HqlSqlWalker.path_return p = default(HqlSqlWalker.path_return);

        HqlSqlWalker.joinElement_return je = default(HqlSqlWalker.joinElement_return);


        IASTNode a_tree=null;
        IASTNode pf_tree=null;
        IASTNode fe_tree=null;
        IASTNode a3_tree=null;
        IASTNode RANGE56_tree=null;
        RewriteRuleNodeStream stream_FETCH = new RewriteRuleNodeStream(adaptor,"token FETCH");
        RewriteRuleNodeStream stream_RANGE = new RewriteRuleNodeStream(adaptor,"token RANGE");
        RewriteRuleNodeStream stream_FILTER_ENTITY = new RewriteRuleNodeStream(adaptor,"token FILTER_ENTITY");
        RewriteRuleNodeStream stream_ALIAS = new RewriteRuleNodeStream(adaptor,"token ALIAS");
        RewriteRuleSubtreeStream stream_joinElement = new RewriteRuleSubtreeStream(adaptor,"rule joinElement");
        RewriteRuleSubtreeStream stream_path = new RewriteRuleSubtreeStream(adaptor,"rule path");

           IASTNode fromElement = null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:217:2: ( ^( RANGE p= path (a= ALIAS )? (pf= FETCH )? ) -> {fromElement != null}? ^() -> | je= joinElement -> | fe= FILTER_ENTITY a3= ALIAS -> ^() )
            int alt26 = 3;
            switch ( input.LA(1) ) 
            {
            case RANGE:
            	{
                alt26 = 1;
                }
                break;
            case JOIN:
            	{
                alt26 = 2;
                }
                break;
            case FILTER_ENTITY:
            	{
                alt26 = 3;
                }
                break;
            	default:
            	    NoViableAltException nvae_d26s0 =
            	        new NoViableAltException("", 26, 0, input);

            	    throw nvae_d26s0;
            }

            switch (alt26) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:217:4: ^( RANGE p= path (a= ALIAS )? (pf= FETCH )? )
                    {
                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	RANGE56=(IASTNode)Match(input,RANGE,FOLLOW_RANGE_in_fromElement988);  
                    	stream_RANGE.Add(RANGE56);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_path_in_fromElement992);
                    	p = path();
                    	state.followingStackPointer--;

                    	stream_path.Add(p.Tree);
                    	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:217:19: (a= ALIAS )?
                    	int alt24 = 2;
                    	int LA24_0 = input.LA(1);

                    	if ( (LA24_0 == ALIAS) )
                    	{
                    	    alt24 = 1;
                    	}
                    	switch (alt24) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:217:20: a= ALIAS
                    	        {
                    	        	_last = (IASTNode)input.LT(1);
                    	        	a=(IASTNode)Match(input,ALIAS,FOLLOW_ALIAS_in_fromElement997);  
                    	        	stream_ALIAS.Add(a);


                    	        }
                    	        break;

                    	}

                    	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:217:30: (pf= FETCH )?
                    	int alt25 = 2;
                    	int LA25_0 = input.LA(1);

                    	if ( (LA25_0 == FETCH) )
                    	{
                    	    alt25 = 1;
                    	}
                    	switch (alt25) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:217:31: pf= FETCH
                    	        {
                    	        	_last = (IASTNode)input.LT(1);
                    	        	pf=(IASTNode)Match(input,FETCH,FOLLOW_FETCH_in_fromElement1004);  
                    	        	stream_FETCH.Add(pf);


                    	        }
                    	        break;

                    	}


                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}

                    	 fromElement = CreateFromElement(((p != null) ? p.p : default(String)), ((p != null) ? ((IASTNode)p.Tree) : null), a, pf); 


                    	// AST REWRITE
                    	// elements:          
                    	// token labels:      
                    	// rule labels:       retval
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

                    	root_0 = (IASTNode)adaptor.GetNilNode();
                    	// 218:3: -> {fromElement != null}? ^()
                    	if (fromElement != null)
                    	{
                    	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:218:29: ^()
                    	    {
                    	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
                    	    root_1 = (IASTNode)adaptor.BecomeRoot(fromElement, root_1);

                    	    adaptor.AddChild(root_0, root_1);
                    	    }

                    	}
                    	else // 219:3: ->
                    	{
                    	    root_0 = null;
                    	}

                    	retval.Tree = root_0;
                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:220:4: je= joinElement
                    {
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_joinElement_in_fromElement1031);
                    	je = joinElement();
                    	state.followingStackPointer--;

                    	stream_joinElement.Add(je.Tree);


                    	// AST REWRITE
                    	// elements:          
                    	// token labels:      
                    	// rule labels:       retval
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

                    	root_0 = (IASTNode)adaptor.GetNilNode();
                    	// 221:3: ->
                    	{
                    	    root_0 = null;
                    	}

                    	retval.Tree = root_0;
                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:223:4: fe= FILTER_ENTITY a3= ALIAS
                    {
                    	_last = (IASTNode)input.LT(1);
                    	fe=(IASTNode)Match(input,FILTER_ENTITY,FOLLOW_FILTER_ENTITY_in_fromElement1046);  
                    	stream_FILTER_ENTITY.Add(fe);

                    	_last = (IASTNode)input.LT(1);
                    	a3=(IASTNode)Match(input,ALIAS,FOLLOW_ALIAS_in_fromElement1050);  
                    	stream_ALIAS.Add(a3);



                    	// AST REWRITE
                    	// elements:          
                    	// token labels:      
                    	// rule labels:       retval
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

                    	root_0 = (IASTNode)adaptor.GetNilNode();
                    	// 224:3: -> ^()
                    	{
                    	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:224:6: ^()
                    	    {
                    	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
                    	    root_1 = (IASTNode)adaptor.BecomeRoot(CreateFromFilterElement(fe,a3), root_1);

                    	    adaptor.AddChild(root_0, root_1);
                    	    }

                    	}

                    	retval.Tree = root_0;
                    }
                    break;

            }
            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "fromElement"

    public class joinElement_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "joinElement"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:227:1: joinElement : ^( JOIN (j= joinType )? (f= FETCH )? pRef= propertyRef (a= ALIAS )? (pf= FETCH )? (with= WITH )? ) ;
    public HqlSqlWalker.joinElement_return joinElement() // throws RecognitionException [1]
    {   
        HqlSqlWalker.joinElement_return retval = new HqlSqlWalker.joinElement_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode f = null;
        IASTNode a = null;
        IASTNode pf = null;
        IASTNode with = null;
        IASTNode JOIN57 = null;
        HqlSqlWalker.joinType_return j = default(HqlSqlWalker.joinType_return);

        HqlSqlWalker.propertyRef_return pRef = default(HqlSqlWalker.propertyRef_return);


        IASTNode f_tree=null;
        IASTNode a_tree=null;
        IASTNode pf_tree=null;
        IASTNode with_tree=null;
        IASTNode JOIN57_tree=null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:231:2: ( ^( JOIN (j= joinType )? (f= FETCH )? pRef= propertyRef (a= ALIAS )? (pf= FETCH )? (with= WITH )? ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:231:4: ^( JOIN (j= joinType )? (f= FETCH )? pRef= propertyRef (a= ALIAS )? (pf= FETCH )? (with= WITH )? )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	JOIN57=(IASTNode)Match(input,JOIN,FOLLOW_JOIN_in_joinElement1079); 
            		JOIN57_tree = (IASTNode)adaptor.DupNode(JOIN57);

            		root_1 = (IASTNode)adaptor.BecomeRoot(JOIN57_tree, root_1);



            	Match(input, Token.DOWN, null); 
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:231:11: (j= joinType )?
            	int alt27 = 2;
            	int LA27_0 = input.LA(1);

            	if ( (LA27_0 == FULL || LA27_0 == INNER || LA27_0 == LEFT || LA27_0 == RIGHT) )
            	{
            	    alt27 = 1;
            	}
            	switch (alt27) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:231:12: j= joinType
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_joinType_in_joinElement1084);
            	        	j = joinType();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, j.Tree);
            	        	 SetImpliedJoinType(((j != null) ? j.j : default(int))); 

            	        }
            	        break;

            	}

            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:231:56: (f= FETCH )?
            	int alt28 = 2;
            	int LA28_0 = input.LA(1);

            	if ( (LA28_0 == FETCH) )
            	{
            	    alt28 = 1;
            	}
            	switch (alt28) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:231:57: f= FETCH
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	f=(IASTNode)Match(input,FETCH,FOLLOW_FETCH_in_joinElement1094); 
            	        		f_tree = (IASTNode)adaptor.DupNode(f);

            	        		adaptor.AddChild(root_1, f_tree);


            	        }
            	        break;

            	}

            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_propertyRef_in_joinElement1100);
            	pRef = propertyRef();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_1, pRef.Tree);
            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:231:84: (a= ALIAS )?
            	int alt29 = 2;
            	int LA29_0 = input.LA(1);

            	if ( (LA29_0 == ALIAS) )
            	{
            	    alt29 = 1;
            	}
            	switch (alt29) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:231:85: a= ALIAS
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	a=(IASTNode)Match(input,ALIAS,FOLLOW_ALIAS_in_joinElement1105); 
            	        		a_tree = (IASTNode)adaptor.DupNode(a);

            	        		adaptor.AddChild(root_1, a_tree);


            	        }
            	        break;

            	}

            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:231:95: (pf= FETCH )?
            	int alt30 = 2;
            	int LA30_0 = input.LA(1);

            	if ( (LA30_0 == FETCH) )
            	{
            	    alt30 = 1;
            	}
            	switch (alt30) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:231:96: pf= FETCH
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	pf=(IASTNode)Match(input,FETCH,FOLLOW_FETCH_in_joinElement1112); 
            	        		pf_tree = (IASTNode)adaptor.DupNode(pf);

            	        		adaptor.AddChild(root_1, pf_tree);


            	        }
            	        break;

            	}

            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:231:107: (with= WITH )?
            	int alt31 = 2;
            	int LA31_0 = input.LA(1);

            	if ( (LA31_0 == WITH) )
            	{
            	    alt31 = 1;
            	}
            	switch (alt31) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:231:108: with= WITH
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	with=(IASTNode)Match(input,WITH,FOLLOW_WITH_in_joinElement1119); 
            	        		with_tree = (IASTNode)adaptor.DupNode(with);

            	        		adaptor.AddChild(root_1, with_tree);


            	        }
            	        break;

            	}


            	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	}


            			CreateFromJoinElement(((pRef != null) ? ((IASTNode)pRef.Tree) : null),a,((j != null) ? j.j : default(int)),f, pf, with);
            			SetImpliedJoinType(INNER);	// Reset the implied join type.
            		

            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "joinElement"

    public class joinType_return : TreeRuleReturnScope
    {
        public int j;
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "joinType"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:240:1: joinType returns [int j] : ( ( (left= LEFT | right= RIGHT ) (outer= OUTER )? ) | FULL | INNER );
    public HqlSqlWalker.joinType_return joinType() // throws RecognitionException [1]
    {   
        HqlSqlWalker.joinType_return retval = new HqlSqlWalker.joinType_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode left = null;
        IASTNode right = null;
        IASTNode outer = null;
        IASTNode FULL58 = null;
        IASTNode INNER59 = null;

        IASTNode left_tree=null;
        IASTNode right_tree=null;
        IASTNode outer_tree=null;
        IASTNode FULL58_tree=null;
        IASTNode INNER59_tree=null;


           retval.j =  INNER;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:244:2: ( ( (left= LEFT | right= RIGHT ) (outer= OUTER )? ) | FULL | INNER )
            int alt34 = 3;
            switch ( input.LA(1) ) 
            {
            case LEFT:
            case RIGHT:
            	{
                alt34 = 1;
                }
                break;
            case FULL:
            	{
                alt34 = 2;
                }
                break;
            case INNER:
            	{
                alt34 = 3;
                }
                break;
            	default:
            	    NoViableAltException nvae_d34s0 =
            	        new NoViableAltException("", 34, 0, input);

            	    throw nvae_d34s0;
            }

            switch (alt34) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:244:4: ( (left= LEFT | right= RIGHT ) (outer= OUTER )? )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:244:4: ( (left= LEFT | right= RIGHT ) (outer= OUTER )? )
                    	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:244:6: (left= LEFT | right= RIGHT ) (outer= OUTER )?
                    	{
                    		// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:244:6: (left= LEFT | right= RIGHT )
                    		int alt32 = 2;
                    		int LA32_0 = input.LA(1);

                    		if ( (LA32_0 == LEFT) )
                    		{
                    		    alt32 = 1;
                    		}
                    		else if ( (LA32_0 == RIGHT) )
                    		{
                    		    alt32 = 2;
                    		}
                    		else 
                    		{
                    		    NoViableAltException nvae_d32s0 =
                    		        new NoViableAltException("", 32, 0, input);

                    		    throw nvae_d32s0;
                    		}
                    		switch (alt32) 
                    		{
                    		    case 1 :
                    		        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:244:7: left= LEFT
                    		        {
                    		        	_last = (IASTNode)input.LT(1);
                    		        	left=(IASTNode)Match(input,LEFT,FOLLOW_LEFT_in_joinType1155); 
                    		        		left_tree = (IASTNode)adaptor.DupNode(left);

                    		        		adaptor.AddChild(root_0, left_tree);


                    		        }
                    		        break;
                    		    case 2 :
                    		        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:244:19: right= RIGHT
                    		        {
                    		        	_last = (IASTNode)input.LT(1);
                    		        	right=(IASTNode)Match(input,RIGHT,FOLLOW_RIGHT_in_joinType1161); 
                    		        		right_tree = (IASTNode)adaptor.DupNode(right);

                    		        		adaptor.AddChild(root_0, right_tree);


                    		        }
                    		        break;

                    		}

                    		// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:244:32: (outer= OUTER )?
                    		int alt33 = 2;
                    		int LA33_0 = input.LA(1);

                    		if ( (LA33_0 == OUTER) )
                    		{
                    		    alt33 = 1;
                    		}
                    		switch (alt33) 
                    		{
                    		    case 1 :
                    		        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:244:33: outer= OUTER
                    		        {
                    		        	_last = (IASTNode)input.LT(1);
                    		        	outer=(IASTNode)Match(input,OUTER,FOLLOW_OUTER_in_joinType1167); 
                    		        		outer_tree = (IASTNode)adaptor.DupNode(outer);

                    		        		adaptor.AddChild(root_0, outer_tree);


                    		        }
                    		        break;

                    		}


                    				if (left != null)       retval.j =  LEFT_OUTER;
                    				else if (right != null) retval.j =  RIGHT_OUTER;
                    				else if (outer != null) retval.j =  RIGHT_OUTER;
                    			

                    	}


                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:250:4: FULL
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	FULL58=(IASTNode)Match(input,FULL,FOLLOW_FULL_in_joinType1181); 
                    		FULL58_tree = (IASTNode)adaptor.DupNode(FULL58);

                    		adaptor.AddChild(root_0, FULL58_tree);


                    			retval.j =  FULL;
                    		

                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:253:4: INNER
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	INNER59=(IASTNode)Match(input,INNER,FOLLOW_INNER_in_joinType1188); 
                    		INNER59_tree = (IASTNode)adaptor.DupNode(INNER59);

                    		adaptor.AddChild(root_0, INNER59_tree);


                    			retval.j =  INNER;
                    		

                    }
                    break;

            }
            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "joinType"

    public class path_return : TreeRuleReturnScope
    {
        public String p;
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "path"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:260:1: path returns [String p] : (a= identifier | ^( DOT x= path y= identifier ) );
    public HqlSqlWalker.path_return path() // throws RecognitionException [1]
    {   
        HqlSqlWalker.path_return retval = new HqlSqlWalker.path_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode DOT60 = null;
        HqlSqlWalker.identifier_return a = default(HqlSqlWalker.identifier_return);

        HqlSqlWalker.path_return x = default(HqlSqlWalker.path_return);

        HqlSqlWalker.identifier_return y = default(HqlSqlWalker.identifier_return);


        IASTNode DOT60_tree=null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:261:2: (a= identifier | ^( DOT x= path y= identifier ) )
            int alt35 = 2;
            int LA35_0 = input.LA(1);

            if ( (LA35_0 == WEIRD_IDENT || LA35_0 == IDENT) )
            {
                alt35 = 1;
            }
            else if ( (LA35_0 == DOT) )
            {
                alt35 = 2;
            }
            else 
            {
                NoViableAltException nvae_d35s0 =
                    new NoViableAltException("", 35, 0, input);

                throw nvae_d35s0;
            }
            switch (alt35) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:261:4: a= identifier
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_identifier_in_path1210);
                    	a = identifier();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, a.Tree);
                    	 retval.p =  ((a != null) ? ((IASTNode)a.Start) : null).ToString();

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:262:4: ^( DOT x= path y= identifier )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	DOT60=(IASTNode)Match(input,DOT,FOLLOW_DOT_in_path1218); 
                    		DOT60_tree = (IASTNode)adaptor.DupNode(DOT60);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(DOT60_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_path_in_path1222);
                    	x = path();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, x.Tree);
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_identifier_in_path1226);
                    	y = identifier();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, y.Tree);

                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}


                    				StringBuilder buf = new StringBuilder();
                    				buf.Append(((x != null) ? x.p : default(String))).Append('.').Append(((y != null) ? ((IASTNode)y.Start) : null).ToString());
                    				retval.p =  buf.ToString();
                    			

                    }
                    break;

            }
            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "path"

    public class pathAsIdent_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "pathAsIdent"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:270:1: pathAsIdent : path -> ^( IDENT[$path.p] ) ;
    public HqlSqlWalker.pathAsIdent_return pathAsIdent() // throws RecognitionException [1]
    {   
        HqlSqlWalker.pathAsIdent_return retval = new HqlSqlWalker.pathAsIdent_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        HqlSqlWalker.path_return path61 = default(HqlSqlWalker.path_return);


        RewriteRuleSubtreeStream stream_path = new RewriteRuleSubtreeStream(adaptor,"rule path");
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:271:5: ( path -> ^( IDENT[$path.p] ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:271:7: path
            {
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_path_in_pathAsIdent1245);
            	path61 = path();
            	state.followingStackPointer--;

            	stream_path.Add(path61.Tree);


            	// AST REWRITE
            	// elements:          
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (IASTNode)adaptor.GetNilNode();
            	// 272:5: -> ^( IDENT[$path.p] )
            	{
            	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:272:8: ^( IDENT[$path.p] )
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(IDENT, ((path61 != null) ? path61.p : default(String))), root_1);

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;
            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "pathAsIdent"

    public class withClause_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "withClause"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:275:1: withClause : ^(w= WITH b= logicalExpr ) -> ^( $w $b) ;
    public HqlSqlWalker.withClause_return withClause() // throws RecognitionException [1]
    {   
        HqlSqlWalker.withClause_return retval = new HqlSqlWalker.withClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode w = null;
        HqlSqlWalker.logicalExpr_return b = default(HqlSqlWalker.logicalExpr_return);


        IASTNode w_tree=null;
        RewriteRuleNodeStream stream_WITH = new RewriteRuleNodeStream(adaptor,"token WITH");
        RewriteRuleSubtreeStream stream_logicalExpr = new RewriteRuleSubtreeStream(adaptor,"rule logicalExpr");
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:282:2: ( ^(w= WITH b= logicalExpr ) -> ^( $w $b) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:282:4: ^(w= WITH b= logicalExpr )
            {
            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	w=(IASTNode)Match(input,WITH,FOLLOW_WITH_in_withClause1286);  
            	stream_WITH.Add(w);


            	 HandleClauseStart( WITH ); 

            	Match(input, Token.DOWN, null); 
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_logicalExpr_in_withClause1292);
            	b = logicalExpr();
            	state.followingStackPointer--;

            	stream_logicalExpr.Add(b.Tree);

            	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	}



            	// AST REWRITE
            	// elements:          b, w
            	// token labels:      w
            	// rule labels:       retval, b
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	retval.Tree = root_0;
            	RewriteRuleNodeStream stream_w = new RewriteRuleNodeStream(adaptor, "token w", w);
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);
            	RewriteRuleSubtreeStream stream_b = new RewriteRuleSubtreeStream(adaptor, "rule b", b!=null ? b.Tree : null);

            	root_0 = (IASTNode)adaptor.GetNilNode();
            	// 283:2: -> ^( $w $b)
            	{
            	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:283:5: ^( $w $b)
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot(stream_w.NextNode(), root_1);

            	    adaptor.AddChild(root_1, stream_b.NextTree());

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;
            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "withClause"

    public class whereClause_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "whereClause"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:286:1: whereClause : ^(w= WHERE b= logicalExpr ) -> ^( $w $b) ;
    public HqlSqlWalker.whereClause_return whereClause() // throws RecognitionException [1]
    {   
        HqlSqlWalker.whereClause_return retval = new HqlSqlWalker.whereClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode w = null;
        HqlSqlWalker.logicalExpr_return b = default(HqlSqlWalker.logicalExpr_return);


        IASTNode w_tree=null;
        RewriteRuleNodeStream stream_WHERE = new RewriteRuleNodeStream(adaptor,"token WHERE");
        RewriteRuleSubtreeStream stream_logicalExpr = new RewriteRuleSubtreeStream(adaptor,"rule logicalExpr");
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:287:2: ( ^(w= WHERE b= logicalExpr ) -> ^( $w $b) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:287:4: ^(w= WHERE b= logicalExpr )
            {
            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	w=(IASTNode)Match(input,WHERE,FOLLOW_WHERE_in_whereClause1320);  
            	stream_WHERE.Add(w);


            	 HandleClauseStart( WHERE ); 

            	Match(input, Token.DOWN, null); 
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_logicalExpr_in_whereClause1326);
            	b = logicalExpr();
            	state.followingStackPointer--;

            	stream_logicalExpr.Add(b.Tree);

            	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	}



            	// AST REWRITE
            	// elements:          b, w
            	// token labels:      w
            	// rule labels:       retval, b
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	retval.Tree = root_0;
            	RewriteRuleNodeStream stream_w = new RewriteRuleNodeStream(adaptor, "token w", w);
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);
            	RewriteRuleSubtreeStream stream_b = new RewriteRuleSubtreeStream(adaptor, "rule b", b!=null ? b.Tree : null);

            	root_0 = (IASTNode)adaptor.GetNilNode();
            	// 288:2: -> ^( $w $b)
            	{
            	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:288:5: ^( $w $b)
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot(stream_w.NextNode(), root_1);

            	    adaptor.AddChild(root_1, stream_b.NextTree());

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;
            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "whereClause"

    public class logicalExpr_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "logicalExpr"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:291:1: logicalExpr : ( ^( AND logicalExpr logicalExpr ) | ^( OR logicalExpr logicalExpr ) | ^( NOT logicalExpr ) | comparisonExpr );
    public HqlSqlWalker.logicalExpr_return logicalExpr() // throws RecognitionException [1]
    {   
        HqlSqlWalker.logicalExpr_return retval = new HqlSqlWalker.logicalExpr_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode AND62 = null;
        IASTNode OR65 = null;
        IASTNode NOT68 = null;
        HqlSqlWalker.logicalExpr_return logicalExpr63 = default(HqlSqlWalker.logicalExpr_return);

        HqlSqlWalker.logicalExpr_return logicalExpr64 = default(HqlSqlWalker.logicalExpr_return);

        HqlSqlWalker.logicalExpr_return logicalExpr66 = default(HqlSqlWalker.logicalExpr_return);

        HqlSqlWalker.logicalExpr_return logicalExpr67 = default(HqlSqlWalker.logicalExpr_return);

        HqlSqlWalker.logicalExpr_return logicalExpr69 = default(HqlSqlWalker.logicalExpr_return);

        HqlSqlWalker.comparisonExpr_return comparisonExpr70 = default(HqlSqlWalker.comparisonExpr_return);


        IASTNode AND62_tree=null;
        IASTNode OR65_tree=null;
        IASTNode NOT68_tree=null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:292:2: ( ^( AND logicalExpr logicalExpr ) | ^( OR logicalExpr logicalExpr ) | ^( NOT logicalExpr ) | comparisonExpr )
            int alt36 = 4;
            switch ( input.LA(1) ) 
            {
            case AND:
            	{
                alt36 = 1;
                }
                break;
            case OR:
            	{
                alt36 = 2;
                }
                break;
            case NOT:
            	{
                alt36 = 3;
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
                alt36 = 4;
                }
                break;
            	default:
            	    NoViableAltException nvae_d36s0 =
            	        new NoViableAltException("", 36, 0, input);

            	    throw nvae_d36s0;
            }

            switch (alt36) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:292:4: ^( AND logicalExpr logicalExpr )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	AND62=(IASTNode)Match(input,AND,FOLLOW_AND_in_logicalExpr1352); 
                    		AND62_tree = (IASTNode)adaptor.DupNode(AND62);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(AND62_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_logicalExpr_in_logicalExpr1354);
                    	logicalExpr63 = logicalExpr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, logicalExpr63.Tree);
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_logicalExpr_in_logicalExpr1356);
                    	logicalExpr64 = logicalExpr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, logicalExpr64.Tree);

                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}


                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:293:4: ^( OR logicalExpr logicalExpr )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	OR65=(IASTNode)Match(input,OR,FOLLOW_OR_in_logicalExpr1363); 
                    		OR65_tree = (IASTNode)adaptor.DupNode(OR65);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(OR65_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_logicalExpr_in_logicalExpr1365);
                    	logicalExpr66 = logicalExpr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, logicalExpr66.Tree);
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_logicalExpr_in_logicalExpr1367);
                    	logicalExpr67 = logicalExpr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, logicalExpr67.Tree);

                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}


                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:294:4: ^( NOT logicalExpr )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	NOT68=(IASTNode)Match(input,NOT,FOLLOW_NOT_in_logicalExpr1374); 
                    		NOT68_tree = (IASTNode)adaptor.DupNode(NOT68);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(NOT68_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_logicalExpr_in_logicalExpr1376);
                    	logicalExpr69 = logicalExpr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, logicalExpr69.Tree);

                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}


                    }
                    break;
                case 4 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:295:4: comparisonExpr
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_comparisonExpr_in_logicalExpr1382);
                    	comparisonExpr70 = comparisonExpr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, comparisonExpr70.Tree);

                    }
                    break;

            }
            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "logicalExpr"

    public class comparisonExpr_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "comparisonExpr"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:299:1: comparisonExpr : ( ^( EQ exprOrSubquery exprOrSubquery ) | ^( NE exprOrSubquery exprOrSubquery ) | ^( LT exprOrSubquery exprOrSubquery ) | ^( GT exprOrSubquery exprOrSubquery ) | ^( LE exprOrSubquery exprOrSubquery ) | ^( GE exprOrSubquery exprOrSubquery ) | ^( LIKE exprOrSubquery expr ( ^( ESCAPE expr ) )? ) | ^( NOT_LIKE exprOrSubquery expr ( ^( ESCAPE expr ) )? ) | ^( BETWEEN exprOrSubquery exprOrSubquery exprOrSubquery ) | ^( NOT_BETWEEN exprOrSubquery exprOrSubquery exprOrSubquery ) | ^( IN exprOrSubquery inRhs ) | ^( NOT_IN exprOrSubquery inRhs ) | ^( IS_NULL exprOrSubquery ) | ^( IS_NOT_NULL exprOrSubquery ) | ^( EXISTS ( expr | collectionFunctionOrSubselect ) ) ) ;
    public HqlSqlWalker.comparisonExpr_return comparisonExpr() // throws RecognitionException [1]
    {   
        HqlSqlWalker.comparisonExpr_return retval = new HqlSqlWalker.comparisonExpr_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode EQ71 = null;
        IASTNode NE74 = null;
        IASTNode LT77 = null;
        IASTNode GT80 = null;
        IASTNode LE83 = null;
        IASTNode GE86 = null;
        IASTNode LIKE89 = null;
        IASTNode ESCAPE92 = null;
        IASTNode NOT_LIKE94 = null;
        IASTNode ESCAPE97 = null;
        IASTNode BETWEEN99 = null;
        IASTNode NOT_BETWEEN103 = null;
        IASTNode IN107 = null;
        IASTNode NOT_IN110 = null;
        IASTNode IS_NULL113 = null;
        IASTNode IS_NOT_NULL115 = null;
        IASTNode EXISTS117 = null;
        HqlSqlWalker.exprOrSubquery_return exprOrSubquery72 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery73 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery75 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery76 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery78 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery79 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery81 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery82 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery84 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery85 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery87 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery88 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery90 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.expr_return expr91 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.expr_return expr93 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery95 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.expr_return expr96 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.expr_return expr98 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery100 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery101 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery102 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery104 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery105 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery106 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery108 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.inRhs_return inRhs109 = default(HqlSqlWalker.inRhs_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery111 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.inRhs_return inRhs112 = default(HqlSqlWalker.inRhs_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery114 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery116 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.expr_return expr118 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.collectionFunctionOrSubselect_return collectionFunctionOrSubselect119 = default(HqlSqlWalker.collectionFunctionOrSubselect_return);


        IASTNode EQ71_tree=null;
        IASTNode NE74_tree=null;
        IASTNode LT77_tree=null;
        IASTNode GT80_tree=null;
        IASTNode LE83_tree=null;
        IASTNode GE86_tree=null;
        IASTNode LIKE89_tree=null;
        IASTNode ESCAPE92_tree=null;
        IASTNode NOT_LIKE94_tree=null;
        IASTNode ESCAPE97_tree=null;
        IASTNode BETWEEN99_tree=null;
        IASTNode NOT_BETWEEN103_tree=null;
        IASTNode IN107_tree=null;
        IASTNode NOT_IN110_tree=null;
        IASTNode IS_NULL113_tree=null;
        IASTNode IS_NOT_NULL115_tree=null;
        IASTNode EXISTS117_tree=null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:303:2: ( ( ^( EQ exprOrSubquery exprOrSubquery ) | ^( NE exprOrSubquery exprOrSubquery ) | ^( LT exprOrSubquery exprOrSubquery ) | ^( GT exprOrSubquery exprOrSubquery ) | ^( LE exprOrSubquery exprOrSubquery ) | ^( GE exprOrSubquery exprOrSubquery ) | ^( LIKE exprOrSubquery expr ( ^( ESCAPE expr ) )? ) | ^( NOT_LIKE exprOrSubquery expr ( ^( ESCAPE expr ) )? ) | ^( BETWEEN exprOrSubquery exprOrSubquery exprOrSubquery ) | ^( NOT_BETWEEN exprOrSubquery exprOrSubquery exprOrSubquery ) | ^( IN exprOrSubquery inRhs ) | ^( NOT_IN exprOrSubquery inRhs ) | ^( IS_NULL exprOrSubquery ) | ^( IS_NOT_NULL exprOrSubquery ) | ^( EXISTS ( expr | collectionFunctionOrSubselect ) ) ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:304:2: ( ^( EQ exprOrSubquery exprOrSubquery ) | ^( NE exprOrSubquery exprOrSubquery ) | ^( LT exprOrSubquery exprOrSubquery ) | ^( GT exprOrSubquery exprOrSubquery ) | ^( LE exprOrSubquery exprOrSubquery ) | ^( GE exprOrSubquery exprOrSubquery ) | ^( LIKE exprOrSubquery expr ( ^( ESCAPE expr ) )? ) | ^( NOT_LIKE exprOrSubquery expr ( ^( ESCAPE expr ) )? ) | ^( BETWEEN exprOrSubquery exprOrSubquery exprOrSubquery ) | ^( NOT_BETWEEN exprOrSubquery exprOrSubquery exprOrSubquery ) | ^( IN exprOrSubquery inRhs ) | ^( NOT_IN exprOrSubquery inRhs ) | ^( IS_NULL exprOrSubquery ) | ^( IS_NOT_NULL exprOrSubquery ) | ^( EXISTS ( expr | collectionFunctionOrSubselect ) ) )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:304:2: ( ^( EQ exprOrSubquery exprOrSubquery ) | ^( NE exprOrSubquery exprOrSubquery ) | ^( LT exprOrSubquery exprOrSubquery ) | ^( GT exprOrSubquery exprOrSubquery ) | ^( LE exprOrSubquery exprOrSubquery ) | ^( GE exprOrSubquery exprOrSubquery ) | ^( LIKE exprOrSubquery expr ( ^( ESCAPE expr ) )? ) | ^( NOT_LIKE exprOrSubquery expr ( ^( ESCAPE expr ) )? ) | ^( BETWEEN exprOrSubquery exprOrSubquery exprOrSubquery ) | ^( NOT_BETWEEN exprOrSubquery exprOrSubquery exprOrSubquery ) | ^( IN exprOrSubquery inRhs ) | ^( NOT_IN exprOrSubquery inRhs ) | ^( IS_NULL exprOrSubquery ) | ^( IS_NOT_NULL exprOrSubquery ) | ^( EXISTS ( expr | collectionFunctionOrSubselect ) ) )
            	int alt40 = 15;
            	switch ( input.LA(1) ) 
            	{
            	case EQ:
            		{
            	    alt40 = 1;
            	    }
            	    break;
            	case NE:
            		{
            	    alt40 = 2;
            	    }
            	    break;
            	case LT:
            		{
            	    alt40 = 3;
            	    }
            	    break;
            	case GT:
            		{
            	    alt40 = 4;
            	    }
            	    break;
            	case LE:
            		{
            	    alt40 = 5;
            	    }
            	    break;
            	case GE:
            		{
            	    alt40 = 6;
            	    }
            	    break;
            	case LIKE:
            		{
            	    alt40 = 7;
            	    }
            	    break;
            	case NOT_LIKE:
            		{
            	    alt40 = 8;
            	    }
            	    break;
            	case BETWEEN:
            		{
            	    alt40 = 9;
            	    }
            	    break;
            	case NOT_BETWEEN:
            		{
            	    alt40 = 10;
            	    }
            	    break;
            	case IN:
            		{
            	    alt40 = 11;
            	    }
            	    break;
            	case NOT_IN:
            		{
            	    alt40 = 12;
            	    }
            	    break;
            	case IS_NULL:
            		{
            	    alt40 = 13;
            	    }
            	    break;
            	case IS_NOT_NULL:
            		{
            	    alt40 = 14;
            	    }
            	    break;
            	case EXISTS:
            		{
            	    alt40 = 15;
            	    }
            	    break;
            		default:
            		    NoViableAltException nvae_d40s0 =
            		        new NoViableAltException("", 40, 0, input);

            		    throw nvae_d40s0;
            	}

            	switch (alt40) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:304:4: ^( EQ exprOrSubquery exprOrSubquery )
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	{
            	        	IASTNode _save_last_1 = _last;
            	        	IASTNode _first_1 = null;
            	        	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	EQ71=(IASTNode)Match(input,EQ,FOLLOW_EQ_in_comparisonExpr1404); 
            	        		EQ71_tree = (IASTNode)adaptor.DupNode(EQ71);

            	        		root_1 = (IASTNode)adaptor.BecomeRoot(EQ71_tree, root_1);



            	        	Match(input, Token.DOWN, null); 
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1406);
            	        	exprOrSubquery72 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery72.Tree);
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1408);
            	        	exprOrSubquery73 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery73.Tree);

            	        	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	        	}


            	        }
            	        break;
            	    case 2 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:305:4: ^( NE exprOrSubquery exprOrSubquery )
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	{
            	        	IASTNode _save_last_1 = _last;
            	        	IASTNode _first_1 = null;
            	        	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	NE74=(IASTNode)Match(input,NE,FOLLOW_NE_in_comparisonExpr1415); 
            	        		NE74_tree = (IASTNode)adaptor.DupNode(NE74);

            	        		root_1 = (IASTNode)adaptor.BecomeRoot(NE74_tree, root_1);



            	        	Match(input, Token.DOWN, null); 
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1417);
            	        	exprOrSubquery75 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery75.Tree);
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1419);
            	        	exprOrSubquery76 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery76.Tree);

            	        	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	        	}


            	        }
            	        break;
            	    case 3 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:306:4: ^( LT exprOrSubquery exprOrSubquery )
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	{
            	        	IASTNode _save_last_1 = _last;
            	        	IASTNode _first_1 = null;
            	        	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	LT77=(IASTNode)Match(input,LT,FOLLOW_LT_in_comparisonExpr1426); 
            	        		LT77_tree = (IASTNode)adaptor.DupNode(LT77);

            	        		root_1 = (IASTNode)adaptor.BecomeRoot(LT77_tree, root_1);



            	        	Match(input, Token.DOWN, null); 
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1428);
            	        	exprOrSubquery78 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery78.Tree);
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1430);
            	        	exprOrSubquery79 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery79.Tree);

            	        	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	        	}


            	        }
            	        break;
            	    case 4 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:307:4: ^( GT exprOrSubquery exprOrSubquery )
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	{
            	        	IASTNode _save_last_1 = _last;
            	        	IASTNode _first_1 = null;
            	        	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	GT80=(IASTNode)Match(input,GT,FOLLOW_GT_in_comparisonExpr1437); 
            	        		GT80_tree = (IASTNode)adaptor.DupNode(GT80);

            	        		root_1 = (IASTNode)adaptor.BecomeRoot(GT80_tree, root_1);



            	        	Match(input, Token.DOWN, null); 
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1439);
            	        	exprOrSubquery81 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery81.Tree);
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1441);
            	        	exprOrSubquery82 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery82.Tree);

            	        	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	        	}


            	        }
            	        break;
            	    case 5 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:308:4: ^( LE exprOrSubquery exprOrSubquery )
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	{
            	        	IASTNode _save_last_1 = _last;
            	        	IASTNode _first_1 = null;
            	        	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	LE83=(IASTNode)Match(input,LE,FOLLOW_LE_in_comparisonExpr1448); 
            	        		LE83_tree = (IASTNode)adaptor.DupNode(LE83);

            	        		root_1 = (IASTNode)adaptor.BecomeRoot(LE83_tree, root_1);



            	        	Match(input, Token.DOWN, null); 
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1450);
            	        	exprOrSubquery84 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery84.Tree);
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1452);
            	        	exprOrSubquery85 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery85.Tree);

            	        	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	        	}


            	        }
            	        break;
            	    case 6 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:309:4: ^( GE exprOrSubquery exprOrSubquery )
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	{
            	        	IASTNode _save_last_1 = _last;
            	        	IASTNode _first_1 = null;
            	        	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	GE86=(IASTNode)Match(input,GE,FOLLOW_GE_in_comparisonExpr1459); 
            	        		GE86_tree = (IASTNode)adaptor.DupNode(GE86);

            	        		root_1 = (IASTNode)adaptor.BecomeRoot(GE86_tree, root_1);



            	        	Match(input, Token.DOWN, null); 
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1461);
            	        	exprOrSubquery87 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery87.Tree);
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1463);
            	        	exprOrSubquery88 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery88.Tree);

            	        	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	        	}


            	        }
            	        break;
            	    case 7 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:310:4: ^( LIKE exprOrSubquery expr ( ^( ESCAPE expr ) )? )
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	{
            	        	IASTNode _save_last_1 = _last;
            	        	IASTNode _first_1 = null;
            	        	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	LIKE89=(IASTNode)Match(input,LIKE,FOLLOW_LIKE_in_comparisonExpr1470); 
            	        		LIKE89_tree = (IASTNode)adaptor.DupNode(LIKE89);

            	        		root_1 = (IASTNode)adaptor.BecomeRoot(LIKE89_tree, root_1);



            	        	Match(input, Token.DOWN, null); 
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1472);
            	        	exprOrSubquery90 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery90.Tree);
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_expr_in_comparisonExpr1474);
            	        	expr91 = expr();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, expr91.Tree);
            	        	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:310:31: ( ^( ESCAPE expr ) )?
            	        	int alt37 = 2;
            	        	int LA37_0 = input.LA(1);

            	        	if ( (LA37_0 == ESCAPE) )
            	        	{
            	        	    alt37 = 1;
            	        	}
            	        	switch (alt37) 
            	        	{
            	        	    case 1 :
            	        	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:310:33: ^( ESCAPE expr )
            	        	        {
            	        	        	_last = (IASTNode)input.LT(1);
            	        	        	{
            	        	        	IASTNode _save_last_2 = _last;
            	        	        	IASTNode _first_2 = null;
            	        	        	IASTNode root_2 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	        	ESCAPE92=(IASTNode)Match(input,ESCAPE,FOLLOW_ESCAPE_in_comparisonExpr1479); 
            	        	        		ESCAPE92_tree = (IASTNode)adaptor.DupNode(ESCAPE92);

            	        	        		root_2 = (IASTNode)adaptor.BecomeRoot(ESCAPE92_tree, root_2);



            	        	        	Match(input, Token.DOWN, null); 
            	        	        	_last = (IASTNode)input.LT(1);
            	        	        	PushFollow(FOLLOW_expr_in_comparisonExpr1481);
            	        	        	expr93 = expr();
            	        	        	state.followingStackPointer--;

            	        	        	adaptor.AddChild(root_2, expr93.Tree);

            	        	        	Match(input, Token.UP, null); adaptor.AddChild(root_1, root_2);_last = _save_last_2;
            	        	        	}


            	        	        }
            	        	        break;

            	        	}


            	        	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	        	}


            	        }
            	        break;
            	    case 8 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:311:4: ^( NOT_LIKE exprOrSubquery expr ( ^( ESCAPE expr ) )? )
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	{
            	        	IASTNode _save_last_1 = _last;
            	        	IASTNode _first_1 = null;
            	        	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	NOT_LIKE94=(IASTNode)Match(input,NOT_LIKE,FOLLOW_NOT_LIKE_in_comparisonExpr1493); 
            	        		NOT_LIKE94_tree = (IASTNode)adaptor.DupNode(NOT_LIKE94);

            	        		root_1 = (IASTNode)adaptor.BecomeRoot(NOT_LIKE94_tree, root_1);



            	        	Match(input, Token.DOWN, null); 
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1495);
            	        	exprOrSubquery95 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery95.Tree);
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_expr_in_comparisonExpr1497);
            	        	expr96 = expr();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, expr96.Tree);
            	        	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:311:35: ( ^( ESCAPE expr ) )?
            	        	int alt38 = 2;
            	        	int LA38_0 = input.LA(1);

            	        	if ( (LA38_0 == ESCAPE) )
            	        	{
            	        	    alt38 = 1;
            	        	}
            	        	switch (alt38) 
            	        	{
            	        	    case 1 :
            	        	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:311:37: ^( ESCAPE expr )
            	        	        {
            	        	        	_last = (IASTNode)input.LT(1);
            	        	        	{
            	        	        	IASTNode _save_last_2 = _last;
            	        	        	IASTNode _first_2 = null;
            	        	        	IASTNode root_2 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	        	ESCAPE97=(IASTNode)Match(input,ESCAPE,FOLLOW_ESCAPE_in_comparisonExpr1502); 
            	        	        		ESCAPE97_tree = (IASTNode)adaptor.DupNode(ESCAPE97);

            	        	        		root_2 = (IASTNode)adaptor.BecomeRoot(ESCAPE97_tree, root_2);



            	        	        	Match(input, Token.DOWN, null); 
            	        	        	_last = (IASTNode)input.LT(1);
            	        	        	PushFollow(FOLLOW_expr_in_comparisonExpr1504);
            	        	        	expr98 = expr();
            	        	        	state.followingStackPointer--;

            	        	        	adaptor.AddChild(root_2, expr98.Tree);

            	        	        	Match(input, Token.UP, null); adaptor.AddChild(root_1, root_2);_last = _save_last_2;
            	        	        	}


            	        	        }
            	        	        break;

            	        	}


            	        	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	        	}


            	        }
            	        break;
            	    case 9 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:312:4: ^( BETWEEN exprOrSubquery exprOrSubquery exprOrSubquery )
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	{
            	        	IASTNode _save_last_1 = _last;
            	        	IASTNode _first_1 = null;
            	        	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	BETWEEN99=(IASTNode)Match(input,BETWEEN,FOLLOW_BETWEEN_in_comparisonExpr1516); 
            	        		BETWEEN99_tree = (IASTNode)adaptor.DupNode(BETWEEN99);

            	        		root_1 = (IASTNode)adaptor.BecomeRoot(BETWEEN99_tree, root_1);



            	        	Match(input, Token.DOWN, null); 
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1518);
            	        	exprOrSubquery100 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery100.Tree);
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1520);
            	        	exprOrSubquery101 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery101.Tree);
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1522);
            	        	exprOrSubquery102 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery102.Tree);

            	        	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	        	}


            	        }
            	        break;
            	    case 10 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:313:4: ^( NOT_BETWEEN exprOrSubquery exprOrSubquery exprOrSubquery )
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	{
            	        	IASTNode _save_last_1 = _last;
            	        	IASTNode _first_1 = null;
            	        	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	NOT_BETWEEN103=(IASTNode)Match(input,NOT_BETWEEN,FOLLOW_NOT_BETWEEN_in_comparisonExpr1529); 
            	        		NOT_BETWEEN103_tree = (IASTNode)adaptor.DupNode(NOT_BETWEEN103);

            	        		root_1 = (IASTNode)adaptor.BecomeRoot(NOT_BETWEEN103_tree, root_1);



            	        	Match(input, Token.DOWN, null); 
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1531);
            	        	exprOrSubquery104 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery104.Tree);
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1533);
            	        	exprOrSubquery105 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery105.Tree);
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1535);
            	        	exprOrSubquery106 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery106.Tree);

            	        	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	        	}


            	        }
            	        break;
            	    case 11 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:314:4: ^( IN exprOrSubquery inRhs )
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	{
            	        	IASTNode _save_last_1 = _last;
            	        	IASTNode _first_1 = null;
            	        	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	IN107=(IASTNode)Match(input,IN,FOLLOW_IN_in_comparisonExpr1542); 
            	        		IN107_tree = (IASTNode)adaptor.DupNode(IN107);

            	        		root_1 = (IASTNode)adaptor.BecomeRoot(IN107_tree, root_1);



            	        	Match(input, Token.DOWN, null); 
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1544);
            	        	exprOrSubquery108 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery108.Tree);
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_inRhs_in_comparisonExpr1546);
            	        	inRhs109 = inRhs();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, inRhs109.Tree);

            	        	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	        	}


            	        }
            	        break;
            	    case 12 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:315:4: ^( NOT_IN exprOrSubquery inRhs )
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	{
            	        	IASTNode _save_last_1 = _last;
            	        	IASTNode _first_1 = null;
            	        	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	NOT_IN110=(IASTNode)Match(input,NOT_IN,FOLLOW_NOT_IN_in_comparisonExpr1554); 
            	        		NOT_IN110_tree = (IASTNode)adaptor.DupNode(NOT_IN110);

            	        		root_1 = (IASTNode)adaptor.BecomeRoot(NOT_IN110_tree, root_1);



            	        	Match(input, Token.DOWN, null); 
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1556);
            	        	exprOrSubquery111 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery111.Tree);
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_inRhs_in_comparisonExpr1558);
            	        	inRhs112 = inRhs();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, inRhs112.Tree);

            	        	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	        	}


            	        }
            	        break;
            	    case 13 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:316:4: ^( IS_NULL exprOrSubquery )
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	{
            	        	IASTNode _save_last_1 = _last;
            	        	IASTNode _first_1 = null;
            	        	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	IS_NULL113=(IASTNode)Match(input,IS_NULL,FOLLOW_IS_NULL_in_comparisonExpr1566); 
            	        		IS_NULL113_tree = (IASTNode)adaptor.DupNode(IS_NULL113);

            	        		root_1 = (IASTNode)adaptor.BecomeRoot(IS_NULL113_tree, root_1);



            	        	Match(input, Token.DOWN, null); 
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1568);
            	        	exprOrSubquery114 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery114.Tree);

            	        	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	        	}


            	        }
            	        break;
            	    case 14 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:317:4: ^( IS_NOT_NULL exprOrSubquery )
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	{
            	        	IASTNode _save_last_1 = _last;
            	        	IASTNode _first_1 = null;
            	        	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	IS_NOT_NULL115=(IASTNode)Match(input,IS_NOT_NULL,FOLLOW_IS_NOT_NULL_in_comparisonExpr1575); 
            	        		IS_NOT_NULL115_tree = (IASTNode)adaptor.DupNode(IS_NOT_NULL115);

            	        		root_1 = (IASTNode)adaptor.BecomeRoot(IS_NOT_NULL115_tree, root_1);



            	        	Match(input, Token.DOWN, null); 
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1577);
            	        	exprOrSubquery116 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery116.Tree);

            	        	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	        	}


            	        }
            	        break;
            	    case 15 :
            	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:320:4: ^( EXISTS ( expr | collectionFunctionOrSubselect ) )
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	{
            	        	IASTNode _save_last_1 = _last;
            	        	IASTNode _first_1 = null;
            	        	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	EXISTS117=(IASTNode)Match(input,EXISTS,FOLLOW_EXISTS_in_comparisonExpr1586); 
            	        		EXISTS117_tree = (IASTNode)adaptor.DupNode(EXISTS117);

            	        		root_1 = (IASTNode)adaptor.BecomeRoot(EXISTS117_tree, root_1);



            	        	Match(input, Token.DOWN, null); 
            	        	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:320:13: ( expr | collectionFunctionOrSubselect )
            	        	int alt39 = 2;
            	        	int LA39_0 = input.LA(1);

            	        	if ( (LA39_0 == COUNT || LA39_0 == DOT || LA39_0 == FALSE || LA39_0 == NULL || LA39_0 == TRUE || LA39_0 == CASE || LA39_0 == AGGREGATE || LA39_0 == CASE2 || LA39_0 == INDEX_OP || LA39_0 == METHOD_CALL || LA39_0 == UNARY_MINUS || (LA39_0 >= VECTOR_EXPR && LA39_0 <= WEIRD_IDENT) || (LA39_0 >= NUM_INT && LA39_0 <= JAVA_CONSTANT) || (LA39_0 >= PLUS && LA39_0 <= DIV) || (LA39_0 >= COLON && LA39_0 <= IDENT)) )
            	        	{
            	        	    alt39 = 1;
            	        	}
            	        	else if ( (LA39_0 == ELEMENTS || LA39_0 == INDICES || LA39_0 == QUERY) )
            	        	{
            	        	    alt39 = 2;
            	        	}
            	        	else 
            	        	{
            	        	    NoViableAltException nvae_d39s0 =
            	        	        new NoViableAltException("", 39, 0, input);

            	        	    throw nvae_d39s0;
            	        	}
            	        	switch (alt39) 
            	        	{
            	        	    case 1 :
            	        	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:320:15: expr
            	        	        {
            	        	        	_last = (IASTNode)input.LT(1);
            	        	        	PushFollow(FOLLOW_expr_in_comparisonExpr1590);
            	        	        	expr118 = expr();
            	        	        	state.followingStackPointer--;

            	        	        	adaptor.AddChild(root_1, expr118.Tree);

            	        	        }
            	        	        break;
            	        	    case 2 :
            	        	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:320:22: collectionFunctionOrSubselect
            	        	        {
            	        	        	_last = (IASTNode)input.LT(1);
            	        	        	PushFollow(FOLLOW_collectionFunctionOrSubselect_in_comparisonExpr1594);
            	        	        	collectionFunctionOrSubselect119 = collectionFunctionOrSubselect();
            	        	        	state.followingStackPointer--;

            	        	        	adaptor.AddChild(root_1, collectionFunctionOrSubselect119.Tree);

            	        	        }
            	        	        break;

            	        	}


            	        	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	        	}


            	        }
            	        break;

            	}


            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);


            	    PrepareLogicOperator( ((IASTNode)retval.Tree) );
            	
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
    // $ANTLR end "comparisonExpr"

    public class inRhs_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "inRhs"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:324:1: inRhs : ^( IN_LIST ( collectionFunctionOrSubselect | ( expr )* ) ) ;
    public HqlSqlWalker.inRhs_return inRhs() // throws RecognitionException [1]
    {   
        HqlSqlWalker.inRhs_return retval = new HqlSqlWalker.inRhs_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode IN_LIST120 = null;
        HqlSqlWalker.collectionFunctionOrSubselect_return collectionFunctionOrSubselect121 = default(HqlSqlWalker.collectionFunctionOrSubselect_return);

        HqlSqlWalker.expr_return expr122 = default(HqlSqlWalker.expr_return);


        IASTNode IN_LIST120_tree=null;

        	int UP = 99999;		// TODO - added this to get compile working.  It's bogus & should be removed
        	
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:326:2: ( ^( IN_LIST ( collectionFunctionOrSubselect | ( expr )* ) ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:326:4: ^( IN_LIST ( collectionFunctionOrSubselect | ( expr )* ) )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	IN_LIST120=(IASTNode)Match(input,IN_LIST,FOLLOW_IN_LIST_in_inRhs1619); 
            		IN_LIST120_tree = (IASTNode)adaptor.DupNode(IN_LIST120);

            		root_1 = (IASTNode)adaptor.BecomeRoot(IN_LIST120_tree, root_1);



            	if ( input.LA(1) == Token.DOWN )
            	{
            	    Match(input, Token.DOWN, null); 
            	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:326:14: ( collectionFunctionOrSubselect | ( expr )* )
            	    int alt42 = 2;
            	    int LA42_0 = input.LA(1);

            	    if ( (LA42_0 == ELEMENTS || LA42_0 == INDICES || LA42_0 == QUERY) )
            	    {
            	        alt42 = 1;
            	    }
            	    else if ( (LA42_0 == UP || LA42_0 == COUNT || LA42_0 == DOT || LA42_0 == FALSE || LA42_0 == NULL || LA42_0 == TRUE || LA42_0 == CASE || LA42_0 == AGGREGATE || LA42_0 == CASE2 || LA42_0 == INDEX_OP || LA42_0 == METHOD_CALL || LA42_0 == UNARY_MINUS || (LA42_0 >= VECTOR_EXPR && LA42_0 <= WEIRD_IDENT) || (LA42_0 >= NUM_INT && LA42_0 <= JAVA_CONSTANT) || (LA42_0 >= PLUS && LA42_0 <= DIV) || (LA42_0 >= COLON && LA42_0 <= IDENT)) )
            	    {
            	        alt42 = 2;
            	    }
            	    else 
            	    {
            	        NoViableAltException nvae_d42s0 =
            	            new NoViableAltException("", 42, 0, input);

            	        throw nvae_d42s0;
            	    }
            	    switch (alt42) 
            	    {
            	        case 1 :
            	            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:326:16: collectionFunctionOrSubselect
            	            {
            	            	_last = (IASTNode)input.LT(1);
            	            	PushFollow(FOLLOW_collectionFunctionOrSubselect_in_inRhs1623);
            	            	collectionFunctionOrSubselect121 = collectionFunctionOrSubselect();
            	            	state.followingStackPointer--;

            	            	adaptor.AddChild(root_1, collectionFunctionOrSubselect121.Tree);

            	            }
            	            break;
            	        case 2 :
            	            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:326:48: ( expr )*
            	            {
            	            	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:326:48: ( expr )*
            	            	do 
            	            	{
            	            	    int alt41 = 2;
            	            	    int LA41_0 = input.LA(1);

            	            	    if ( (LA41_0 == COUNT || LA41_0 == DOT || LA41_0 == FALSE || LA41_0 == NULL || LA41_0 == TRUE || LA41_0 == CASE || LA41_0 == AGGREGATE || LA41_0 == CASE2 || LA41_0 == INDEX_OP || LA41_0 == METHOD_CALL || LA41_0 == UNARY_MINUS || (LA41_0 >= VECTOR_EXPR && LA41_0 <= WEIRD_IDENT) || (LA41_0 >= NUM_INT && LA41_0 <= JAVA_CONSTANT) || (LA41_0 >= PLUS && LA41_0 <= DIV) || (LA41_0 >= COLON && LA41_0 <= IDENT)) )
            	            	    {
            	            	        alt41 = 1;
            	            	    }


            	            	    switch (alt41) 
            	            		{
            	            			case 1 :
            	            			    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:326:48: expr
            	            			    {
            	            			    	_last = (IASTNode)input.LT(1);
            	            			    	PushFollow(FOLLOW_expr_in_inRhs1627);
            	            			    	expr122 = expr();
            	            			    	state.followingStackPointer--;

            	            			    	adaptor.AddChild(root_1, expr122.Tree);

            	            			    }
            	            			    break;

            	            			default:
            	            			    goto loop41;
            	            	    }
            	            	} while (true);

            	            	loop41:
            	            		;	// Stops C# compiler whining that label 'loop41' has no statements


            	            }
            	            break;

            	    }


            	    Match(input, Token.UP, null); 
            	}adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	}


            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "inRhs"

    public class exprOrSubquery_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "exprOrSubquery"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:329:1: exprOrSubquery : ( expr | query | ^( ANY collectionFunctionOrSubselect ) | ^( ALL collectionFunctionOrSubselect ) | ^( SOME collectionFunctionOrSubselect ) );
    public HqlSqlWalker.exprOrSubquery_return exprOrSubquery() // throws RecognitionException [1]
    {   
        HqlSqlWalker.exprOrSubquery_return retval = new HqlSqlWalker.exprOrSubquery_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode ANY125 = null;
        IASTNode ALL127 = null;
        IASTNode SOME129 = null;
        HqlSqlWalker.expr_return expr123 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.query_return query124 = default(HqlSqlWalker.query_return);

        HqlSqlWalker.collectionFunctionOrSubselect_return collectionFunctionOrSubselect126 = default(HqlSqlWalker.collectionFunctionOrSubselect_return);

        HqlSqlWalker.collectionFunctionOrSubselect_return collectionFunctionOrSubselect128 = default(HqlSqlWalker.collectionFunctionOrSubselect_return);

        HqlSqlWalker.collectionFunctionOrSubselect_return collectionFunctionOrSubselect130 = default(HqlSqlWalker.collectionFunctionOrSubselect_return);


        IASTNode ANY125_tree=null;
        IASTNode ALL127_tree=null;
        IASTNode SOME129_tree=null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:330:2: ( expr | query | ^( ANY collectionFunctionOrSubselect ) | ^( ALL collectionFunctionOrSubselect ) | ^( SOME collectionFunctionOrSubselect ) )
            int alt43 = 5;
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
            case VECTOR_EXPR:
            case WEIRD_IDENT:
            case NUM_INT:
            case NUM_DOUBLE:
            case NUM_FLOAT:
            case NUM_LONG:
            case JAVA_CONSTANT:
            case PLUS:
            case MINUS:
            case STAR:
            case DIV:
            case COLON:
            case PARAM:
            case QUOTED_String:
            case IDENT:
            	{
                alt43 = 1;
                }
                break;
            case QUERY:
            	{
                alt43 = 2;
                }
                break;
            case ANY:
            	{
                alt43 = 3;
                }
                break;
            case ALL:
            	{
                alt43 = 4;
                }
                break;
            case SOME:
            	{
                alt43 = 5;
                }
                break;
            	default:
            	    NoViableAltException nvae_d43s0 =
            	        new NoViableAltException("", 43, 0, input);

            	    throw nvae_d43s0;
            }

            switch (alt43) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:330:4: expr
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_expr_in_exprOrSubquery1643);
                    	expr123 = expr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, expr123.Tree);

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:331:4: query
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_query_in_exprOrSubquery1648);
                    	query124 = query();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, query124.Tree);

                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:332:4: ^( ANY collectionFunctionOrSubselect )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	ANY125=(IASTNode)Match(input,ANY,FOLLOW_ANY_in_exprOrSubquery1654); 
                    		ANY125_tree = (IASTNode)adaptor.DupNode(ANY125);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(ANY125_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_collectionFunctionOrSubselect_in_exprOrSubquery1656);
                    	collectionFunctionOrSubselect126 = collectionFunctionOrSubselect();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, collectionFunctionOrSubselect126.Tree);

                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}


                    }
                    break;
                case 4 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:333:4: ^( ALL collectionFunctionOrSubselect )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	ALL127=(IASTNode)Match(input,ALL,FOLLOW_ALL_in_exprOrSubquery1663); 
                    		ALL127_tree = (IASTNode)adaptor.DupNode(ALL127);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(ALL127_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_collectionFunctionOrSubselect_in_exprOrSubquery1665);
                    	collectionFunctionOrSubselect128 = collectionFunctionOrSubselect();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, collectionFunctionOrSubselect128.Tree);

                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}


                    }
                    break;
                case 5 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:334:4: ^( SOME collectionFunctionOrSubselect )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	SOME129=(IASTNode)Match(input,SOME,FOLLOW_SOME_in_exprOrSubquery1672); 
                    		SOME129_tree = (IASTNode)adaptor.DupNode(SOME129);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(SOME129_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_collectionFunctionOrSubselect_in_exprOrSubquery1674);
                    	collectionFunctionOrSubselect130 = collectionFunctionOrSubselect();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, collectionFunctionOrSubselect130.Tree);

                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}


                    }
                    break;

            }
            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "exprOrSubquery"

    public class collectionFunctionOrSubselect_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "collectionFunctionOrSubselect"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:337:1: collectionFunctionOrSubselect : ( collectionFunction | query );
    public HqlSqlWalker.collectionFunctionOrSubselect_return collectionFunctionOrSubselect() // throws RecognitionException [1]
    {   
        HqlSqlWalker.collectionFunctionOrSubselect_return retval = new HqlSqlWalker.collectionFunctionOrSubselect_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        HqlSqlWalker.collectionFunction_return collectionFunction131 = default(HqlSqlWalker.collectionFunction_return);

        HqlSqlWalker.query_return query132 = default(HqlSqlWalker.query_return);



        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:338:2: ( collectionFunction | query )
            int alt44 = 2;
            int LA44_0 = input.LA(1);

            if ( (LA44_0 == ELEMENTS || LA44_0 == INDICES) )
            {
                alt44 = 1;
            }
            else if ( (LA44_0 == QUERY) )
            {
                alt44 = 2;
            }
            else 
            {
                NoViableAltException nvae_d44s0 =
                    new NoViableAltException("", 44, 0, input);

                throw nvae_d44s0;
            }
            switch (alt44) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:338:4: collectionFunction
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_collectionFunction_in_collectionFunctionOrSubselect1687);
                    	collectionFunction131 = collectionFunction();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, collectionFunction131.Tree);

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:339:4: query
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_query_in_collectionFunctionOrSubselect1692);
                    	query132 = query();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, query132.Tree);

                    }
                    break;

            }
            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "collectionFunctionOrSubselect"

    public class expr_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "expr"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:342:1: expr : (ae= addrExpr[ true ] | ^( VECTOR_EXPR ( expr )* ) | constant | arithmeticExpr | functionCall | parameter | count );
    public HqlSqlWalker.expr_return expr() // throws RecognitionException [1]
    {   
        HqlSqlWalker.expr_return retval = new HqlSqlWalker.expr_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode VECTOR_EXPR133 = null;
        HqlSqlWalker.addrExpr_return ae = default(HqlSqlWalker.addrExpr_return);

        HqlSqlWalker.expr_return expr134 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.constant_return constant135 = default(HqlSqlWalker.constant_return);

        HqlSqlWalker.arithmeticExpr_return arithmeticExpr136 = default(HqlSqlWalker.arithmeticExpr_return);

        HqlSqlWalker.functionCall_return functionCall137 = default(HqlSqlWalker.functionCall_return);

        HqlSqlWalker.parameter_return parameter138 = default(HqlSqlWalker.parameter_return);

        HqlSqlWalker.count_return count139 = default(HqlSqlWalker.count_return);


        IASTNode VECTOR_EXPR133_tree=null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:343:2: (ae= addrExpr[ true ] | ^( VECTOR_EXPR ( expr )* ) | constant | arithmeticExpr | functionCall | parameter | count )
            int alt46 = 7;
            switch ( input.LA(1) ) 
            {
            case DOT:
            case INDEX_OP:
            case WEIRD_IDENT:
            case IDENT:
            	{
                alt46 = 1;
                }
                break;
            case VECTOR_EXPR:
            	{
                alt46 = 2;
                }
                break;
            case FALSE:
            case NULL:
            case TRUE:
            case NUM_INT:
            case NUM_DOUBLE:
            case NUM_FLOAT:
            case NUM_LONG:
            case JAVA_CONSTANT:
            case QUOTED_String:
            	{
                alt46 = 3;
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
                alt46 = 4;
                }
                break;
            case AGGREGATE:
            case METHOD_CALL:
            	{
                alt46 = 5;
                }
                break;
            case COLON:
            case PARAM:
            	{
                alt46 = 6;
                }
                break;
            case COUNT:
            	{
                alt46 = 7;
                }
                break;
            	default:
            	    NoViableAltException nvae_d46s0 =
            	        new NoViableAltException("", 46, 0, input);

            	    throw nvae_d46s0;
            }

            switch (alt46) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:343:4: ae= addrExpr[ true ]
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_addrExpr_in_expr1706);
                    	ae = addrExpr(true);
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, ae.Tree);
                    	 Resolve(((ae != null) ? ((IASTNode)ae.Tree) : null)); 

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:344:4: ^( VECTOR_EXPR ( expr )* )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	VECTOR_EXPR133=(IASTNode)Match(input,VECTOR_EXPR,FOLLOW_VECTOR_EXPR_in_expr1718); 
                    		VECTOR_EXPR133_tree = (IASTNode)adaptor.DupNode(VECTOR_EXPR133);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(VECTOR_EXPR133_tree, root_1);



                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); 
                    	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:344:19: ( expr )*
                    	    do 
                    	    {
                    	        int alt45 = 2;
                    	        int LA45_0 = input.LA(1);

                    	        if ( (LA45_0 == COUNT || LA45_0 == DOT || LA45_0 == FALSE || LA45_0 == NULL || LA45_0 == TRUE || LA45_0 == CASE || LA45_0 == AGGREGATE || LA45_0 == CASE2 || LA45_0 == INDEX_OP || LA45_0 == METHOD_CALL || LA45_0 == UNARY_MINUS || (LA45_0 >= VECTOR_EXPR && LA45_0 <= WEIRD_IDENT) || (LA45_0 >= NUM_INT && LA45_0 <= JAVA_CONSTANT) || (LA45_0 >= PLUS && LA45_0 <= DIV) || (LA45_0 >= COLON && LA45_0 <= IDENT)) )
                    	        {
                    	            alt45 = 1;
                    	        }


                    	        switch (alt45) 
                    	    	{
                    	    		case 1 :
                    	    		    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:344:20: expr
                    	    		    {
                    	    		    	_last = (IASTNode)input.LT(1);
                    	    		    	PushFollow(FOLLOW_expr_in_expr1721);
                    	    		    	expr134 = expr();
                    	    		    	state.followingStackPointer--;

                    	    		    	adaptor.AddChild(root_1, expr134.Tree);

                    	    		    }
                    	    		    break;

                    	    		default:
                    	    		    goto loop45;
                    	        }
                    	    } while (true);

                    	    loop45:
                    	    	;	// Stops C# compiler whining that label 'loop45' has no statements


                    	    Match(input, Token.UP, null); 
                    	}adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}


                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:345:4: constant
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_constant_in_expr1730);
                    	constant135 = constant();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, constant135.Tree);

                    }
                    break;
                case 4 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:346:4: arithmeticExpr
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_arithmeticExpr_in_expr1735);
                    	arithmeticExpr136 = arithmeticExpr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, arithmeticExpr136.Tree);

                    }
                    break;
                case 5 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:347:4: functionCall
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_functionCall_in_expr1740);
                    	functionCall137 = functionCall();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, functionCall137.Tree);

                    }
                    break;
                case 6 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:348:4: parameter
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_parameter_in_expr1752);
                    	parameter138 = parameter();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, parameter138.Tree);

                    }
                    break;
                case 7 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:349:4: count
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_count_in_expr1757);
                    	count139 = count();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, count139.Tree);

                    }
                    break;

            }
            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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

    public class arithmeticExpr_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "arithmeticExpr"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:352:1: arithmeticExpr : ( ^( PLUS expr expr ) | ^( MINUS expr expr ) | ^( DIV expr expr ) | ^( STAR expr expr ) | ^( UNARY_MINUS expr ) | c= caseExpr );
    public HqlSqlWalker.arithmeticExpr_return arithmeticExpr() // throws RecognitionException [1]
    {   
        HqlSqlWalker.arithmeticExpr_return retval = new HqlSqlWalker.arithmeticExpr_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode PLUS140 = null;
        IASTNode MINUS143 = null;
        IASTNode DIV146 = null;
        IASTNode STAR149 = null;
        IASTNode UNARY_MINUS152 = null;
        HqlSqlWalker.caseExpr_return c = default(HqlSqlWalker.caseExpr_return);

        HqlSqlWalker.expr_return expr141 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.expr_return expr142 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.expr_return expr144 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.expr_return expr145 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.expr_return expr147 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.expr_return expr148 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.expr_return expr150 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.expr_return expr151 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.expr_return expr153 = default(HqlSqlWalker.expr_return);


        IASTNode PLUS140_tree=null;
        IASTNode MINUS143_tree=null;
        IASTNode DIV146_tree=null;
        IASTNode STAR149_tree=null;
        IASTNode UNARY_MINUS152_tree=null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:359:2: ( ^( PLUS expr expr ) | ^( MINUS expr expr ) | ^( DIV expr expr ) | ^( STAR expr expr ) | ^( UNARY_MINUS expr ) | c= caseExpr )
            int alt47 = 6;
            switch ( input.LA(1) ) 
            {
            case PLUS:
            	{
                alt47 = 1;
                }
                break;
            case MINUS:
            	{
                alt47 = 2;
                }
                break;
            case DIV:
            	{
                alt47 = 3;
                }
                break;
            case STAR:
            	{
                alt47 = 4;
                }
                break;
            case UNARY_MINUS:
            	{
                alt47 = 5;
                }
                break;
            case CASE:
            case CASE2:
            	{
                alt47 = 6;
                }
                break;
            	default:
            	    NoViableAltException nvae_d47s0 =
            	        new NoViableAltException("", 47, 0, input);

            	    throw nvae_d47s0;
            }

            switch (alt47) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:359:4: ^( PLUS expr expr )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	PLUS140=(IASTNode)Match(input,PLUS,FOLLOW_PLUS_in_arithmeticExpr1785); 
                    		PLUS140_tree = (IASTNode)adaptor.DupNode(PLUS140);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(PLUS140_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_expr_in_arithmeticExpr1787);
                    	expr141 = expr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, expr141.Tree);
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_expr_in_arithmeticExpr1789);
                    	expr142 = expr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, expr142.Tree);

                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}


                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:360:4: ^( MINUS expr expr )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	MINUS143=(IASTNode)Match(input,MINUS,FOLLOW_MINUS_in_arithmeticExpr1796); 
                    		MINUS143_tree = (IASTNode)adaptor.DupNode(MINUS143);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(MINUS143_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_expr_in_arithmeticExpr1798);
                    	expr144 = expr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, expr144.Tree);
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_expr_in_arithmeticExpr1800);
                    	expr145 = expr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, expr145.Tree);

                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}


                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:361:4: ^( DIV expr expr )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	DIV146=(IASTNode)Match(input,DIV,FOLLOW_DIV_in_arithmeticExpr1807); 
                    		DIV146_tree = (IASTNode)adaptor.DupNode(DIV146);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(DIV146_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_expr_in_arithmeticExpr1809);
                    	expr147 = expr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, expr147.Tree);
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_expr_in_arithmeticExpr1811);
                    	expr148 = expr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, expr148.Tree);

                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}


                    }
                    break;
                case 4 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:362:4: ^( STAR expr expr )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	STAR149=(IASTNode)Match(input,STAR,FOLLOW_STAR_in_arithmeticExpr1818); 
                    		STAR149_tree = (IASTNode)adaptor.DupNode(STAR149);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(STAR149_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_expr_in_arithmeticExpr1820);
                    	expr150 = expr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, expr150.Tree);
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_expr_in_arithmeticExpr1822);
                    	expr151 = expr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, expr151.Tree);

                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}


                    }
                    break;
                case 5 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:364:4: ^( UNARY_MINUS expr )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	UNARY_MINUS152=(IASTNode)Match(input,UNARY_MINUS,FOLLOW_UNARY_MINUS_in_arithmeticExpr1830); 
                    		UNARY_MINUS152_tree = (IASTNode)adaptor.DupNode(UNARY_MINUS152);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(UNARY_MINUS152_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_expr_in_arithmeticExpr1832);
                    	expr153 = expr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, expr153.Tree);

                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}


                    }
                    break;
                case 6 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:365:4: c= caseExpr
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_caseExpr_in_arithmeticExpr1840);
                    	c = caseExpr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, c.Tree);

                    }
                    break;

            }
            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);


            		if (((c != null) ? ((IASTNode)c.Tree) : null) == null)
            		{
            			PrepareArithmeticOperator( ((IASTNode)retval.Tree) );
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
    // $ANTLR end "arithmeticExpr"

    public class caseExpr_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "caseExpr"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:368:1: caseExpr : ( ^( CASE ( ^( WHEN logicalExpr expr ) )+ ( ^( ELSE expr ) )? ) | ^( CASE2 expr ( ^( WHEN expr expr ) )+ ( ^( ELSE expr ) )? ) );
    public HqlSqlWalker.caseExpr_return caseExpr() // throws RecognitionException [1]
    {   
        HqlSqlWalker.caseExpr_return retval = new HqlSqlWalker.caseExpr_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode CASE154 = null;
        IASTNode WHEN155 = null;
        IASTNode ELSE158 = null;
        IASTNode CASE2160 = null;
        IASTNode WHEN162 = null;
        IASTNode ELSE165 = null;
        HqlSqlWalker.logicalExpr_return logicalExpr156 = default(HqlSqlWalker.logicalExpr_return);

        HqlSqlWalker.expr_return expr157 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.expr_return expr159 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.expr_return expr161 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.expr_return expr163 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.expr_return expr164 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.expr_return expr166 = default(HqlSqlWalker.expr_return);


        IASTNode CASE154_tree=null;
        IASTNode WHEN155_tree=null;
        IASTNode ELSE158_tree=null;
        IASTNode CASE2160_tree=null;
        IASTNode WHEN162_tree=null;
        IASTNode ELSE165_tree=null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:369:2: ( ^( CASE ( ^( WHEN logicalExpr expr ) )+ ( ^( ELSE expr ) )? ) | ^( CASE2 expr ( ^( WHEN expr expr ) )+ ( ^( ELSE expr ) )? ) )
            int alt52 = 2;
            int LA52_0 = input.LA(1);

            if ( (LA52_0 == CASE) )
            {
                alt52 = 1;
            }
            else if ( (LA52_0 == CASE2) )
            {
                alt52 = 2;
            }
            else 
            {
                NoViableAltException nvae_d52s0 =
                    new NoViableAltException("", 52, 0, input);

                throw nvae_d52s0;
            }
            switch (alt52) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:369:4: ^( CASE ( ^( WHEN logicalExpr expr ) )+ ( ^( ELSE expr ) )? )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	CASE154=(IASTNode)Match(input,CASE,FOLLOW_CASE_in_caseExpr1852); 
                    		CASE154_tree = (IASTNode)adaptor.DupNode(CASE154);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(CASE154_tree, root_1);


                    	 _inCase = true; 

                    	Match(input, Token.DOWN, null); 
                    	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:369:31: ( ^( WHEN logicalExpr expr ) )+
                    	int cnt48 = 0;
                    	do 
                    	{
                    	    int alt48 = 2;
                    	    int LA48_0 = input.LA(1);

                    	    if ( (LA48_0 == WHEN) )
                    	    {
                    	        alt48 = 1;
                    	    }


                    	    switch (alt48) 
                    		{
                    			case 1 :
                    			    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:369:32: ^( WHEN logicalExpr expr )
                    			    {
                    			    	_last = (IASTNode)input.LT(1);
                    			    	{
                    			    	IASTNode _save_last_2 = _last;
                    			    	IASTNode _first_2 = null;
                    			    	IASTNode root_2 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    			    	WHEN155=(IASTNode)Match(input,WHEN,FOLLOW_WHEN_in_caseExpr1858); 
                    			    		WHEN155_tree = (IASTNode)adaptor.DupNode(WHEN155);

                    			    		root_2 = (IASTNode)adaptor.BecomeRoot(WHEN155_tree, root_2);



                    			    	Match(input, Token.DOWN, null); 
                    			    	_last = (IASTNode)input.LT(1);
                    			    	PushFollow(FOLLOW_logicalExpr_in_caseExpr1860);
                    			    	logicalExpr156 = logicalExpr();
                    			    	state.followingStackPointer--;

                    			    	adaptor.AddChild(root_2, logicalExpr156.Tree);
                    			    	_last = (IASTNode)input.LT(1);
                    			    	PushFollow(FOLLOW_expr_in_caseExpr1862);
                    			    	expr157 = expr();
                    			    	state.followingStackPointer--;

                    			    	adaptor.AddChild(root_2, expr157.Tree);

                    			    	Match(input, Token.UP, null); adaptor.AddChild(root_1, root_2);_last = _save_last_2;
                    			    	}


                    			    }
                    			    break;

                    			default:
                    			    if ( cnt48 >= 1 ) goto loop48;
                    		            EarlyExitException eee48 =
                    		                new EarlyExitException(48, input);
                    		            throw eee48;
                    	    }
                    	    cnt48++;
                    	} while (true);

                    	loop48:
                    		;	// Stops C# compiler whinging that label 'loop48' has no statements

                    	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:369:59: ( ^( ELSE expr ) )?
                    	int alt49 = 2;
                    	int LA49_0 = input.LA(1);

                    	if ( (LA49_0 == ELSE) )
                    	{
                    	    alt49 = 1;
                    	}
                    	switch (alt49) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:369:60: ^( ELSE expr )
                    	        {
                    	        	_last = (IASTNode)input.LT(1);
                    	        	{
                    	        	IASTNode _save_last_2 = _last;
                    	        	IASTNode _first_2 = null;
                    	        	IASTNode root_2 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	        	ELSE158=(IASTNode)Match(input,ELSE,FOLLOW_ELSE_in_caseExpr1869); 
                    	        		ELSE158_tree = (IASTNode)adaptor.DupNode(ELSE158);

                    	        		root_2 = (IASTNode)adaptor.BecomeRoot(ELSE158_tree, root_2);



                    	        	Match(input, Token.DOWN, null); 
                    	        	_last = (IASTNode)input.LT(1);
                    	        	PushFollow(FOLLOW_expr_in_caseExpr1871);
                    	        	expr159 = expr();
                    	        	state.followingStackPointer--;

                    	        	adaptor.AddChild(root_2, expr159.Tree);

                    	        	Match(input, Token.UP, null); adaptor.AddChild(root_1, root_2);_last = _save_last_2;
                    	        	}


                    	        }
                    	        break;

                    	}


                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}

                    	 _inCase = false; 

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:370:4: ^( CASE2 expr ( ^( WHEN expr expr ) )+ ( ^( ELSE expr ) )? )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	CASE2160=(IASTNode)Match(input,CASE2,FOLLOW_CASE2_in_caseExpr1883); 
                    		CASE2160_tree = (IASTNode)adaptor.DupNode(CASE2160);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(CASE2160_tree, root_1);


                    	 _inCase = true; 

                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_expr_in_caseExpr1887);
                    	expr161 = expr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, expr161.Tree);
                    	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:370:37: ( ^( WHEN expr expr ) )+
                    	int cnt50 = 0;
                    	do 
                    	{
                    	    int alt50 = 2;
                    	    int LA50_0 = input.LA(1);

                    	    if ( (LA50_0 == WHEN) )
                    	    {
                    	        alt50 = 1;
                    	    }


                    	    switch (alt50) 
                    		{
                    			case 1 :
                    			    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:370:38: ^( WHEN expr expr )
                    			    {
                    			    	_last = (IASTNode)input.LT(1);
                    			    	{
                    			    	IASTNode _save_last_2 = _last;
                    			    	IASTNode _first_2 = null;
                    			    	IASTNode root_2 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    			    	WHEN162=(IASTNode)Match(input,WHEN,FOLLOW_WHEN_in_caseExpr1891); 
                    			    		WHEN162_tree = (IASTNode)adaptor.DupNode(WHEN162);

                    			    		root_2 = (IASTNode)adaptor.BecomeRoot(WHEN162_tree, root_2);



                    			    	Match(input, Token.DOWN, null); 
                    			    	_last = (IASTNode)input.LT(1);
                    			    	PushFollow(FOLLOW_expr_in_caseExpr1893);
                    			    	expr163 = expr();
                    			    	state.followingStackPointer--;

                    			    	adaptor.AddChild(root_2, expr163.Tree);
                    			    	_last = (IASTNode)input.LT(1);
                    			    	PushFollow(FOLLOW_expr_in_caseExpr1895);
                    			    	expr164 = expr();
                    			    	state.followingStackPointer--;

                    			    	adaptor.AddChild(root_2, expr164.Tree);

                    			    	Match(input, Token.UP, null); adaptor.AddChild(root_1, root_2);_last = _save_last_2;
                    			    	}


                    			    }
                    			    break;

                    			default:
                    			    if ( cnt50 >= 1 ) goto loop50;
                    		            EarlyExitException eee50 =
                    		                new EarlyExitException(50, input);
                    		            throw eee50;
                    	    }
                    	    cnt50++;
                    	} while (true);

                    	loop50:
                    		;	// Stops C# compiler whinging that label 'loop50' has no statements

                    	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:370:58: ( ^( ELSE expr ) )?
                    	int alt51 = 2;
                    	int LA51_0 = input.LA(1);

                    	if ( (LA51_0 == ELSE) )
                    	{
                    	    alt51 = 1;
                    	}
                    	switch (alt51) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:370:59: ^( ELSE expr )
                    	        {
                    	        	_last = (IASTNode)input.LT(1);
                    	        	{
                    	        	IASTNode _save_last_2 = _last;
                    	        	IASTNode _first_2 = null;
                    	        	IASTNode root_2 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	        	ELSE165=(IASTNode)Match(input,ELSE,FOLLOW_ELSE_in_caseExpr1902); 
                    	        		ELSE165_tree = (IASTNode)adaptor.DupNode(ELSE165);

                    	        		root_2 = (IASTNode)adaptor.BecomeRoot(ELSE165_tree, root_2);



                    	        	Match(input, Token.DOWN, null); 
                    	        	_last = (IASTNode)input.LT(1);
                    	        	PushFollow(FOLLOW_expr_in_caseExpr1904);
                    	        	expr166 = expr();
                    	        	state.followingStackPointer--;

                    	        	adaptor.AddChild(root_2, expr166.Tree);

                    	        	Match(input, Token.UP, null); adaptor.AddChild(root_1, root_2);_last = _save_last_2;
                    	        	}


                    	        }
                    	        break;

                    	}


                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}

                    	 _inCase = false; 

                    }
                    break;

            }
            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "caseExpr"

    public class collectionFunction_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "collectionFunction"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:375:1: collectionFunction : ( ^(e= ELEMENTS p1= propertyRef ) | ^(i= INDICES p2= propertyRef ) );
    public HqlSqlWalker.collectionFunction_return collectionFunction() // throws RecognitionException [1]
    {   
        HqlSqlWalker.collectionFunction_return retval = new HqlSqlWalker.collectionFunction_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode e = null;
        IASTNode i = null;
        HqlSqlWalker.propertyRef_return p1 = default(HqlSqlWalker.propertyRef_return);

        HqlSqlWalker.propertyRef_return p2 = default(HqlSqlWalker.propertyRef_return);


        IASTNode e_tree=null;
        IASTNode i_tree=null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:376:2: ( ^(e= ELEMENTS p1= propertyRef ) | ^(i= INDICES p2= propertyRef ) )
            int alt53 = 2;
            int LA53_0 = input.LA(1);

            if ( (LA53_0 == ELEMENTS) )
            {
                alt53 = 1;
            }
            else if ( (LA53_0 == INDICES) )
            {
                alt53 = 2;
            }
            else 
            {
                NoViableAltException nvae_d53s0 =
                    new NoViableAltException("", 53, 0, input);

                throw nvae_d53s0;
            }
            switch (alt53) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:376:4: ^(e= ELEMENTS p1= propertyRef )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	e=(IASTNode)Match(input,ELEMENTS,FOLLOW_ELEMENTS_in_collectionFunction1926); 
                    		e_tree = (IASTNode)adaptor.DupNode(e);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(e_tree, root_1);


                    	_inFunctionCall=true;

                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_propertyRef_in_collectionFunction1932);
                    	p1 = propertyRef();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, p1.Tree);
                    	 Resolve(((p1 != null) ? ((IASTNode)p1.Tree) : null)); 

                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}

                    	 ProcessFunction(e_tree,_inSelect); 
                    	_inFunctionCall=false;

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:378:4: ^(i= INDICES p2= propertyRef )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	i=(IASTNode)Match(input,INDICES,FOLLOW_INDICES_in_collectionFunction1951); 
                    		i_tree = (IASTNode)adaptor.DupNode(i);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(i_tree, root_1);


                    	_inFunctionCall=true;

                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_propertyRef_in_collectionFunction1957);
                    	p2 = propertyRef();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, p2.Tree);
                    	 Resolve(((p2 != null) ? ((IASTNode)p2.Tree) : null)); 

                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}

                    	 ProcessFunction(i_tree,_inSelect); 
                    	_inFunctionCall=false;

                    }
                    break;

            }
            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "collectionFunction"

    public class functionCall_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "functionCall"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:382:1: functionCall : ( ^(m= METHOD_CALL pathAsIdent ( ^( EXPR_LIST ( expr | comparisonExpr )* ) )? ) | ^( AGGREGATE aggregateExpr ) );
    public HqlSqlWalker.functionCall_return functionCall() // throws RecognitionException [1]
    {   
        HqlSqlWalker.functionCall_return retval = new HqlSqlWalker.functionCall_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode m = null;
        IASTNode EXPR_LIST168 = null;
        IASTNode AGGREGATE171 = null;
        HqlSqlWalker.pathAsIdent_return pathAsIdent167 = default(HqlSqlWalker.pathAsIdent_return);

        HqlSqlWalker.expr_return expr169 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.comparisonExpr_return comparisonExpr170 = default(HqlSqlWalker.comparisonExpr_return);

        HqlSqlWalker.aggregateExpr_return aggregateExpr172 = default(HqlSqlWalker.aggregateExpr_return);


        IASTNode m_tree=null;
        IASTNode EXPR_LIST168_tree=null;
        IASTNode AGGREGATE171_tree=null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:383:2: ( ^(m= METHOD_CALL pathAsIdent ( ^( EXPR_LIST ( expr | comparisonExpr )* ) )? ) | ^( AGGREGATE aggregateExpr ) )
            int alt56 = 2;
            int LA56_0 = input.LA(1);

            if ( (LA56_0 == METHOD_CALL) )
            {
                alt56 = 1;
            }
            else if ( (LA56_0 == AGGREGATE) )
            {
                alt56 = 2;
            }
            else 
            {
                NoViableAltException nvae_d56s0 =
                    new NoViableAltException("", 56, 0, input);

                throw nvae_d56s0;
            }
            switch (alt56) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:383:4: ^(m= METHOD_CALL pathAsIdent ( ^( EXPR_LIST ( expr | comparisonExpr )* ) )? )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	m=(IASTNode)Match(input,METHOD_CALL,FOLLOW_METHOD_CALL_in_functionCall1982); 
                    		m_tree = (IASTNode)adaptor.DupNode(m);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(m_tree, root_1);


                    	_inFunctionCall=true;

                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_pathAsIdent_in_functionCall1987);
                    	pathAsIdent167 = pathAsIdent();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, pathAsIdent167.Tree);
                    	// /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:383:57: ( ^( EXPR_LIST ( expr | comparisonExpr )* ) )?
                    	int alt55 = 2;
                    	int LA55_0 = input.LA(1);

                    	if ( (LA55_0 == EXPR_LIST) )
                    	{
                    	    alt55 = 1;
                    	}
                    	switch (alt55) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:383:59: ^( EXPR_LIST ( expr | comparisonExpr )* )
                    	        {
                    	        	_last = (IASTNode)input.LT(1);
                    	        	{
                    	        	IASTNode _save_last_2 = _last;
                    	        	IASTNode _first_2 = null;
                    	        	IASTNode root_2 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	        	EXPR_LIST168=(IASTNode)Match(input,EXPR_LIST,FOLLOW_EXPR_LIST_in_functionCall1992); 
                    	        		EXPR_LIST168_tree = (IASTNode)adaptor.DupNode(EXPR_LIST168);

                    	        		root_2 = (IASTNode)adaptor.BecomeRoot(EXPR_LIST168_tree, root_2);



                    	        	if ( input.LA(1) == Token.DOWN )
                    	        	{
                    	        	    Match(input, Token.DOWN, null); 
                    	        	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:383:71: ( expr | comparisonExpr )*
                    	        	    do 
                    	        	    {
                    	        	        int alt54 = 3;
                    	        	        int LA54_0 = input.LA(1);

                    	        	        if ( (LA54_0 == COUNT || LA54_0 == DOT || LA54_0 == FALSE || LA54_0 == NULL || LA54_0 == TRUE || LA54_0 == CASE || LA54_0 == AGGREGATE || LA54_0 == CASE2 || LA54_0 == INDEX_OP || LA54_0 == METHOD_CALL || LA54_0 == UNARY_MINUS || (LA54_0 >= VECTOR_EXPR && LA54_0 <= WEIRD_IDENT) || (LA54_0 >= NUM_INT && LA54_0 <= JAVA_CONSTANT) || (LA54_0 >= PLUS && LA54_0 <= DIV) || (LA54_0 >= COLON && LA54_0 <= IDENT)) )
                    	        	        {
                    	        	            alt54 = 1;
                    	        	        }
                    	        	        else if ( (LA54_0 == BETWEEN || LA54_0 == EXISTS || LA54_0 == IN || LA54_0 == LIKE || (LA54_0 >= IS_NOT_NULL && LA54_0 <= IS_NULL) || (LA54_0 >= NOT_BETWEEN && LA54_0 <= NOT_LIKE) || LA54_0 == EQ || LA54_0 == NE || (LA54_0 >= LT && LA54_0 <= GE)) )
                    	        	        {
                    	        	            alt54 = 2;
                    	        	        }


                    	        	        switch (alt54) 
                    	        	    	{
                    	        	    		case 1 :
                    	        	    		    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:383:72: expr
                    	        	    		    {
                    	        	    		    	_last = (IASTNode)input.LT(1);
                    	        	    		    	PushFollow(FOLLOW_expr_in_functionCall1995);
                    	        	    		    	expr169 = expr();
                    	        	    		    	state.followingStackPointer--;

                    	        	    		    	adaptor.AddChild(root_2, expr169.Tree);

                    	        	    		    }
                    	        	    		    break;
                    	        	    		case 2 :
                    	        	    		    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:383:79: comparisonExpr
                    	        	    		    {
                    	        	    		    	_last = (IASTNode)input.LT(1);
                    	        	    		    	PushFollow(FOLLOW_comparisonExpr_in_functionCall1999);
                    	        	    		    	comparisonExpr170 = comparisonExpr();
                    	        	    		    	state.followingStackPointer--;

                    	        	    		    	adaptor.AddChild(root_2, comparisonExpr170.Tree);

                    	        	    		    }
                    	        	    		    break;

                    	        	    		default:
                    	        	    		    goto loop54;
                    	        	        }
                    	        	    } while (true);

                    	        	    loop54:
                    	        	    	;	// Stops C# compiler whining that label 'loop54' has no statements


                    	        	    Match(input, Token.UP, null); 
                    	        	}adaptor.AddChild(root_1, root_2);_last = _save_last_2;
                    	        	}


                    	        }
                    	        break;

                    	}


                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}

                    	 ProcessFunction(m_tree,_inSelect); _inFunctionCall=false; 

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:385:4: ^( AGGREGATE aggregateExpr )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	AGGREGATE171=(IASTNode)Match(input,AGGREGATE,FOLLOW_AGGREGATE_in_functionCall2018); 
                    		AGGREGATE171_tree = (IASTNode)adaptor.DupNode(AGGREGATE171);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(AGGREGATE171_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_aggregateExpr_in_functionCall2020);
                    	aggregateExpr172 = aggregateExpr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, aggregateExpr172.Tree);

                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}


                    }
                    break;

            }
            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "functionCall"

    public class constant_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "constant"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:388:1: constant : ( literal | NULL | t= TRUE | f= FALSE | JAVA_CONSTANT );
    public HqlSqlWalker.constant_return constant() // throws RecognitionException [1]
    {   
        HqlSqlWalker.constant_return retval = new HqlSqlWalker.constant_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode t = null;
        IASTNode f = null;
        IASTNode NULL174 = null;
        IASTNode JAVA_CONSTANT175 = null;
        HqlSqlWalker.literal_return literal173 = default(HqlSqlWalker.literal_return);


        IASTNode t_tree=null;
        IASTNode f_tree=null;
        IASTNode NULL174_tree=null;
        IASTNode JAVA_CONSTANT175_tree=null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:389:2: ( literal | NULL | t= TRUE | f= FALSE | JAVA_CONSTANT )
            int alt57 = 5;
            switch ( input.LA(1) ) 
            {
            case NUM_INT:
            case NUM_DOUBLE:
            case NUM_FLOAT:
            case NUM_LONG:
            case QUOTED_String:
            	{
                alt57 = 1;
                }
                break;
            case NULL:
            	{
                alt57 = 2;
                }
                break;
            case TRUE:
            	{
                alt57 = 3;
                }
                break;
            case FALSE:
            	{
                alt57 = 4;
                }
                break;
            case JAVA_CONSTANT:
            	{
                alt57 = 5;
                }
                break;
            	default:
            	    NoViableAltException nvae_d57s0 =
            	        new NoViableAltException("", 57, 0, input);

            	    throw nvae_d57s0;
            }

            switch (alt57) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:389:4: literal
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_literal_in_constant2033);
                    	literal173 = literal();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, literal173.Tree);

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:390:4: NULL
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	NULL174=(IASTNode)Match(input,NULL,FOLLOW_NULL_in_constant2038); 
                    		NULL174_tree = (IASTNode)adaptor.DupNode(NULL174);

                    		adaptor.AddChild(root_0, NULL174_tree);


                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:391:4: t= TRUE
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	t=(IASTNode)Match(input,TRUE,FOLLOW_TRUE_in_constant2045); 
                    		t_tree = (IASTNode)adaptor.DupNode(t);

                    		adaptor.AddChild(root_0, t_tree);

                    	 ProcessBool(t); 

                    }
                    break;
                case 4 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:392:4: f= FALSE
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	f=(IASTNode)Match(input,FALSE,FOLLOW_FALSE_in_constant2055); 
                    		f_tree = (IASTNode)adaptor.DupNode(f);

                    		adaptor.AddChild(root_0, f_tree);

                    	 ProcessBool(f); 

                    }
                    break;
                case 5 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:393:4: JAVA_CONSTANT
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	JAVA_CONSTANT175=(IASTNode)Match(input,JAVA_CONSTANT,FOLLOW_JAVA_CONSTANT_in_constant2062); 
                    		JAVA_CONSTANT175_tree = (IASTNode)adaptor.DupNode(JAVA_CONSTANT175);

                    		adaptor.AddChild(root_0, JAVA_CONSTANT175_tree);


                    }
                    break;

            }
            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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

    public class literal_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "literal"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:396:1: literal : ( numericLiteral | stringLiteral );
    public HqlSqlWalker.literal_return literal() // throws RecognitionException [1]
    {   
        HqlSqlWalker.literal_return retval = new HqlSqlWalker.literal_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        HqlSqlWalker.numericLiteral_return numericLiteral176 = default(HqlSqlWalker.numericLiteral_return);

        HqlSqlWalker.stringLiteral_return stringLiteral177 = default(HqlSqlWalker.stringLiteral_return);



        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:397:2: ( numericLiteral | stringLiteral )
            int alt58 = 2;
            int LA58_0 = input.LA(1);

            if ( ((LA58_0 >= NUM_INT && LA58_0 <= NUM_LONG)) )
            {
                alt58 = 1;
            }
            else if ( (LA58_0 == QUOTED_String) )
            {
                alt58 = 2;
            }
            else 
            {
                NoViableAltException nvae_d58s0 =
                    new NoViableAltException("", 58, 0, input);

                throw nvae_d58s0;
            }
            switch (alt58) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:397:4: numericLiteral
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_numericLiteral_in_literal2073);
                    	numericLiteral176 = numericLiteral();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, numericLiteral176.Tree);

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:398:4: stringLiteral
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_stringLiteral_in_literal2078);
                    	stringLiteral177 = stringLiteral();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, stringLiteral177.Tree);

                    }
                    break;

            }
            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "literal"

    public class numericLiteral_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "numericLiteral"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:401:1: numericLiteral : ( NUM_INT | NUM_LONG | NUM_FLOAT | NUM_DOUBLE );
    public HqlSqlWalker.numericLiteral_return numericLiteral() // throws RecognitionException [1]
    {   
        HqlSqlWalker.numericLiteral_return retval = new HqlSqlWalker.numericLiteral_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode set178 = null;

        IASTNode set178_tree=null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:406:2: ( NUM_INT | NUM_LONG | NUM_FLOAT | NUM_DOUBLE )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	set178 = (IASTNode)input.LT(1);
            	if ( (input.LA(1) >= NUM_INT && input.LA(1) <= NUM_LONG) ) 
            	{
            	    input.Consume();

            	    set178_tree = (IASTNode)adaptor.DupNode(set178);

            	    adaptor.AddChild(root_0, set178_tree);

            	    state.errorRecovery = false;
            	}
            	else 
            	{
            	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	    throw mse;
            	}

            	 

            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);


            	ProcessNumericLiteral( ((IASTNode)retval.Tree) );

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
    // $ANTLR end "numericLiteral"

    public class stringLiteral_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "stringLiteral"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:412:1: stringLiteral : QUOTED_String ;
    public HqlSqlWalker.stringLiteral_return stringLiteral() // throws RecognitionException [1]
    {   
        HqlSqlWalker.stringLiteral_return retval = new HqlSqlWalker.stringLiteral_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode QUOTED_String179 = null;

        IASTNode QUOTED_String179_tree=null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:413:2: ( QUOTED_String )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:413:4: QUOTED_String
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	QUOTED_String179=(IASTNode)Match(input,QUOTED_String,FOLLOW_QUOTED_String_in_stringLiteral2120); 
            		QUOTED_String179_tree = (IASTNode)adaptor.DupNode(QUOTED_String179);

            		adaptor.AddChild(root_0, QUOTED_String179_tree);


            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "stringLiteral"

    public class identifier_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "identifier"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:416:1: identifier : ( IDENT | WEIRD_IDENT ) ;
    public HqlSqlWalker.identifier_return identifier() // throws RecognitionException [1]
    {   
        HqlSqlWalker.identifier_return retval = new HqlSqlWalker.identifier_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode set180 = null;

        IASTNode set180_tree=null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:417:2: ( ( IDENT | WEIRD_IDENT ) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:417:4: ( IDENT | WEIRD_IDENT )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	set180 = (IASTNode)input.LT(1);
            	if ( input.LA(1) == WEIRD_IDENT || input.LA(1) == IDENT ) 
            	{
            	    input.Consume();

            	    set180_tree = (IASTNode)adaptor.DupNode(set180);

            	    adaptor.AddChild(root_0, set180_tree);

            	    state.errorRecovery = false;
            	}
            	else 
            	{
            	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	    throw mse;
            	}


            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "identifier"

    public class addrExpr_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "addrExpr"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:420:1: addrExpr[ bool root ] : ( addrExprDot[root] | addrExprIndex[root] | addrExprIdent[root] );
    public HqlSqlWalker.addrExpr_return addrExpr(bool root) // throws RecognitionException [1]
    {   
        HqlSqlWalker.addrExpr_return retval = new HqlSqlWalker.addrExpr_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        HqlSqlWalker.addrExprDot_return addrExprDot181 = default(HqlSqlWalker.addrExprDot_return);

        HqlSqlWalker.addrExprIndex_return addrExprIndex182 = default(HqlSqlWalker.addrExprIndex_return);

        HqlSqlWalker.addrExprIdent_return addrExprIdent183 = default(HqlSqlWalker.addrExprIdent_return);



        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:421:2: ( addrExprDot[root] | addrExprIndex[root] | addrExprIdent[root] )
            int alt59 = 3;
            switch ( input.LA(1) ) 
            {
            case DOT:
            	{
                alt59 = 1;
                }
                break;
            case INDEX_OP:
            	{
                alt59 = 2;
                }
                break;
            case WEIRD_IDENT:
            case IDENT:
            	{
                alt59 = 3;
                }
                break;
            	default:
            	    NoViableAltException nvae_d59s0 =
            	        new NoViableAltException("", 59, 0, input);

            	    throw nvae_d59s0;
            }

            switch (alt59) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:421:4: addrExprDot[root]
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_addrExprDot_in_addrExpr2150);
                    	addrExprDot181 = addrExprDot(root);
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, addrExprDot181.Tree);

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:422:4: addrExprIndex[root]
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_addrExprIndex_in_addrExpr2157);
                    	addrExprIndex182 = addrExprIndex(root);
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, addrExprIndex182.Tree);

                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:423:4: addrExprIdent[root]
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_addrExprIdent_in_addrExpr2164);
                    	addrExprIdent183 = addrExprIdent(root);
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, addrExprIdent183.Tree);

                    }
                    break;

            }
            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "addrExpr"

    public class addrExprDot_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "addrExprDot"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:426:1: addrExprDot[ bool root ] : ^(d= DOT lhs= addrExprLhs rhs= propertyName ) -> ^( $d $lhs $rhs) ;
    public HqlSqlWalker.addrExprDot_return addrExprDot(bool root) // throws RecognitionException [1]
    {   
        HqlSqlWalker.addrExprDot_return retval = new HqlSqlWalker.addrExprDot_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode d = null;
        HqlSqlWalker.addrExprLhs_return lhs = default(HqlSqlWalker.addrExprLhs_return);

        HqlSqlWalker.propertyName_return rhs = default(HqlSqlWalker.propertyName_return);


        IASTNode d_tree=null;
        RewriteRuleNodeStream stream_DOT = new RewriteRuleNodeStream(adaptor,"token DOT");
        RewriteRuleSubtreeStream stream_propertyName = new RewriteRuleSubtreeStream(adaptor,"rule propertyName");
        RewriteRuleSubtreeStream stream_addrExprLhs = new RewriteRuleSubtreeStream(adaptor,"rule addrExprLhs");
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:431:2: ( ^(d= DOT lhs= addrExprLhs rhs= propertyName ) -> ^( $d $lhs $rhs) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:431:4: ^(d= DOT lhs= addrExprLhs rhs= propertyName )
            {
            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	d=(IASTNode)Match(input,DOT,FOLLOW_DOT_in_addrExprDot2188);  
            	stream_DOT.Add(d);



            	Match(input, Token.DOWN, null); 
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_addrExprLhs_in_addrExprDot2192);
            	lhs = addrExprLhs();
            	state.followingStackPointer--;

            	stream_addrExprLhs.Add(lhs.Tree);
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_propertyName_in_addrExprDot2196);
            	rhs = propertyName();
            	state.followingStackPointer--;

            	stream_propertyName.Add(rhs.Tree);

            	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	}



            	// AST REWRITE
            	// elements:          d, lhs, rhs
            	// token labels:      d
            	// rule labels:       lhs, retval, rhs
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	retval.Tree = root_0;
            	RewriteRuleNodeStream stream_d = new RewriteRuleNodeStream(adaptor, "token d", d);
            	RewriteRuleSubtreeStream stream_lhs = new RewriteRuleSubtreeStream(adaptor, "rule lhs", lhs!=null ? lhs.Tree : null);
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);
            	RewriteRuleSubtreeStream stream_rhs = new RewriteRuleSubtreeStream(adaptor, "rule rhs", rhs!=null ? rhs.Tree : null);

            	root_0 = (IASTNode)adaptor.GetNilNode();
            	// 432:3: -> ^( $d $lhs $rhs)
            	{
            	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:432:6: ^( $d $lhs $rhs)
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot(stream_d.NextNode(), root_1);

            	    adaptor.AddChild(root_1, stream_lhs.NextTree());
            	    adaptor.AddChild(root_1, stream_rhs.NextTree());

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;
            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);


            	LookupProperty(((IASTNode)retval.Tree),root,false);

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
    // $ANTLR end "addrExprDot"

    public class addrExprIndex_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "addrExprIndex"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:435:1: addrExprIndex[ bool root ] : ^(i= INDEX_OP lhs2= addrExprLhs rhs2= expr ) -> ^( $i $lhs2 $rhs2) ;
    public HqlSqlWalker.addrExprIndex_return addrExprIndex(bool root) // throws RecognitionException [1]
    {   
        HqlSqlWalker.addrExprIndex_return retval = new HqlSqlWalker.addrExprIndex_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode i = null;
        HqlSqlWalker.addrExprLhs_return lhs2 = default(HqlSqlWalker.addrExprLhs_return);

        HqlSqlWalker.expr_return rhs2 = default(HqlSqlWalker.expr_return);


        IASTNode i_tree=null;
        RewriteRuleNodeStream stream_INDEX_OP = new RewriteRuleNodeStream(adaptor,"token INDEX_OP");
        RewriteRuleSubtreeStream stream_expr = new RewriteRuleSubtreeStream(adaptor,"rule expr");
        RewriteRuleSubtreeStream stream_addrExprLhs = new RewriteRuleSubtreeStream(adaptor,"rule addrExprLhs");
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:441:2: ( ^(i= INDEX_OP lhs2= addrExprLhs rhs2= expr ) -> ^( $i $lhs2 $rhs2) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:441:4: ^(i= INDEX_OP lhs2= addrExprLhs rhs2= expr )
            {
            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	i=(IASTNode)Match(input,INDEX_OP,FOLLOW_INDEX_OP_in_addrExprIndex2235);  
            	stream_INDEX_OP.Add(i);



            	Match(input, Token.DOWN, null); 
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_addrExprLhs_in_addrExprIndex2239);
            	lhs2 = addrExprLhs();
            	state.followingStackPointer--;

            	stream_addrExprLhs.Add(lhs2.Tree);
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_expr_in_addrExprIndex2243);
            	rhs2 = expr();
            	state.followingStackPointer--;

            	stream_expr.Add(rhs2.Tree);

            	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	}



            	// AST REWRITE
            	// elements:          i, lhs2, rhs2
            	// token labels:      i
            	// rule labels:       rhs2, retval, lhs2
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	retval.Tree = root_0;
            	RewriteRuleNodeStream stream_i = new RewriteRuleNodeStream(adaptor, "token i", i);
            	RewriteRuleSubtreeStream stream_rhs2 = new RewriteRuleSubtreeStream(adaptor, "rule rhs2", rhs2!=null ? rhs2.Tree : null);
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);
            	RewriteRuleSubtreeStream stream_lhs2 = new RewriteRuleSubtreeStream(adaptor, "rule lhs2", lhs2!=null ? lhs2.Tree : null);

            	root_0 = (IASTNode)adaptor.GetNilNode();
            	// 442:3: -> ^( $i $lhs2 $rhs2)
            	{
            	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:442:6: ^( $i $lhs2 $rhs2)
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot(stream_i.NextNode(), root_1);

            	    adaptor.AddChild(root_1, stream_lhs2.NextTree());
            	    adaptor.AddChild(root_1, stream_rhs2.NextTree());

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;
            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);


            	ProcessIndex(((IASTNode)retval.Tree));

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
    // $ANTLR end "addrExprIndex"

    public class addrExprIdent_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "addrExprIdent"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:445:1: addrExprIdent[ bool root ] : p= identifier -> {IsNonQualifiedPropertyRef($p.tree)}? ^() -> ^() ;
    public HqlSqlWalker.addrExprIdent_return addrExprIdent(bool root) // throws RecognitionException [1]
    {   
        HqlSqlWalker.addrExprIdent_return retval = new HqlSqlWalker.addrExprIdent_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        HqlSqlWalker.identifier_return p = default(HqlSqlWalker.identifier_return);


        RewriteRuleSubtreeStream stream_identifier = new RewriteRuleSubtreeStream(adaptor,"rule identifier");
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:446:2: (p= identifier -> {IsNonQualifiedPropertyRef($p.tree)}? ^() -> ^() )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:446:4: p= identifier
            {
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_identifier_in_addrExprIdent2275);
            	p = identifier();
            	state.followingStackPointer--;

            	stream_identifier.Add(p.Tree);


            	// AST REWRITE
            	// elements:          
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (IASTNode)adaptor.GetNilNode();
            	// 447:2: -> {IsNonQualifiedPropertyRef($p.tree)}? ^()
            	if (IsNonQualifiedPropertyRef(((p != null) ? ((IASTNode)p.Tree) : null)))
            	{
            	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:447:43: ^()
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot(LookupNonQualifiedProperty(((p != null) ? ((IASTNode)p.Tree) : null)), root_1);

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}
            	else // 448:2: -> ^()
            	{
            	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:448:5: ^()
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot(Resolve(((p != null) ? ((IASTNode)p.Tree) : null)), root_1);

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;
            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "addrExprIdent"

    public class addrExprLhs_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "addrExprLhs"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:451:1: addrExprLhs : addrExpr[ false ] ;
    public HqlSqlWalker.addrExprLhs_return addrExprLhs() // throws RecognitionException [1]
    {   
        HqlSqlWalker.addrExprLhs_return retval = new HqlSqlWalker.addrExprLhs_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        HqlSqlWalker.addrExpr_return addrExpr184 = default(HqlSqlWalker.addrExpr_return);



        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:452:2: ( addrExpr[ false ] )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:452:4: addrExpr[ false ]
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_addrExpr_in_addrExprLhs2303);
            	addrExpr184 = addrExpr(false);
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, addrExpr184.Tree);

            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "addrExprLhs"

    public class propertyName_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "propertyName"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:455:1: propertyName : ( identifier | CLASS | ELEMENTS | INDICES );
    public HqlSqlWalker.propertyName_return propertyName() // throws RecognitionException [1]
    {   
        HqlSqlWalker.propertyName_return retval = new HqlSqlWalker.propertyName_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode CLASS186 = null;
        IASTNode ELEMENTS187 = null;
        IASTNode INDICES188 = null;
        HqlSqlWalker.identifier_return identifier185 = default(HqlSqlWalker.identifier_return);


        IASTNode CLASS186_tree=null;
        IASTNode ELEMENTS187_tree=null;
        IASTNode INDICES188_tree=null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:456:2: ( identifier | CLASS | ELEMENTS | INDICES )
            int alt60 = 4;
            switch ( input.LA(1) ) 
            {
            case WEIRD_IDENT:
            case IDENT:
            	{
                alt60 = 1;
                }
                break;
            case CLASS:
            	{
                alt60 = 2;
                }
                break;
            case ELEMENTS:
            	{
                alt60 = 3;
                }
                break;
            case INDICES:
            	{
                alt60 = 4;
                }
                break;
            	default:
            	    NoViableAltException nvae_d60s0 =
            	        new NoViableAltException("", 60, 0, input);

            	    throw nvae_d60s0;
            }

            switch (alt60) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:456:4: identifier
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_identifier_in_propertyName2316);
                    	identifier185 = identifier();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, identifier185.Tree);

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:457:4: CLASS
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	CLASS186=(IASTNode)Match(input,CLASS,FOLLOW_CLASS_in_propertyName2321); 
                    		CLASS186_tree = (IASTNode)adaptor.DupNode(CLASS186);

                    		adaptor.AddChild(root_0, CLASS186_tree);


                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:458:4: ELEMENTS
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	ELEMENTS187=(IASTNode)Match(input,ELEMENTS,FOLLOW_ELEMENTS_in_propertyName2326); 
                    		ELEMENTS187_tree = (IASTNode)adaptor.DupNode(ELEMENTS187);

                    		adaptor.AddChild(root_0, ELEMENTS187_tree);


                    }
                    break;
                case 4 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:459:4: INDICES
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	INDICES188=(IASTNode)Match(input,INDICES,FOLLOW_INDICES_in_propertyName2331); 
                    		INDICES188_tree = (IASTNode)adaptor.DupNode(INDICES188);

                    		adaptor.AddChild(root_0, INDICES188_tree);


                    }
                    break;

            }
            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "propertyName"

    public class propertyRef_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "propertyRef"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:462:1: propertyRef : ( propertyRefPath | propertyRefIdent );
    public HqlSqlWalker.propertyRef_return propertyRef() // throws RecognitionException [1]
    {   
        HqlSqlWalker.propertyRef_return retval = new HqlSqlWalker.propertyRef_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        HqlSqlWalker.propertyRefPath_return propertyRefPath189 = default(HqlSqlWalker.propertyRefPath_return);

        HqlSqlWalker.propertyRefIdent_return propertyRefIdent190 = default(HqlSqlWalker.propertyRefIdent_return);



        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:463:2: ( propertyRefPath | propertyRefIdent )
            int alt61 = 2;
            int LA61_0 = input.LA(1);

            if ( (LA61_0 == DOT) )
            {
                alt61 = 1;
            }
            else if ( (LA61_0 == WEIRD_IDENT || LA61_0 == IDENT) )
            {
                alt61 = 2;
            }
            else 
            {
                NoViableAltException nvae_d61s0 =
                    new NoViableAltException("", 61, 0, input);

                throw nvae_d61s0;
            }
            switch (alt61) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:463:4: propertyRefPath
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_propertyRefPath_in_propertyRef2343);
                    	propertyRefPath189 = propertyRefPath();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, propertyRefPath189.Tree);

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:464:4: propertyRefIdent
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_propertyRefIdent_in_propertyRef2348);
                    	propertyRefIdent190 = propertyRefIdent();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, propertyRefIdent190.Tree);

                    }
                    break;

            }
            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "propertyRef"

    public class propertyRefPath_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "propertyRefPath"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:467:1: propertyRefPath : ^(d= DOT lhs= propertyRefLhs rhs= propertyName ) -> ^( $d $lhs $rhs) ;
    public HqlSqlWalker.propertyRefPath_return propertyRefPath() // throws RecognitionException [1]
    {   
        HqlSqlWalker.propertyRefPath_return retval = new HqlSqlWalker.propertyRefPath_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode d = null;
        HqlSqlWalker.propertyRefLhs_return lhs = default(HqlSqlWalker.propertyRefLhs_return);

        HqlSqlWalker.propertyName_return rhs = default(HqlSqlWalker.propertyName_return);


        IASTNode d_tree=null;
        RewriteRuleNodeStream stream_DOT = new RewriteRuleNodeStream(adaptor,"token DOT");
        RewriteRuleSubtreeStream stream_propertyName = new RewriteRuleSubtreeStream(adaptor,"rule propertyName");
        RewriteRuleSubtreeStream stream_propertyRefLhs = new RewriteRuleSubtreeStream(adaptor,"rule propertyRefLhs");
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:472:2: ( ^(d= DOT lhs= propertyRefLhs rhs= propertyName ) -> ^( $d $lhs $rhs) )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:472:4: ^(d= DOT lhs= propertyRefLhs rhs= propertyName )
            {
            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	d=(IASTNode)Match(input,DOT,FOLLOW_DOT_in_propertyRefPath2368);  
            	stream_DOT.Add(d);



            	Match(input, Token.DOWN, null); 
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_propertyRefLhs_in_propertyRefPath2372);
            	lhs = propertyRefLhs();
            	state.followingStackPointer--;

            	stream_propertyRefLhs.Add(lhs.Tree);
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_propertyName_in_propertyRefPath2376);
            	rhs = propertyName();
            	state.followingStackPointer--;

            	stream_propertyName.Add(rhs.Tree);

            	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	}



            	// AST REWRITE
            	// elements:          lhs, rhs, d
            	// token labels:      d
            	// rule labels:       lhs, retval, rhs
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	retval.Tree = root_0;
            	RewriteRuleNodeStream stream_d = new RewriteRuleNodeStream(adaptor, "token d", d);
            	RewriteRuleSubtreeStream stream_lhs = new RewriteRuleSubtreeStream(adaptor, "rule lhs", lhs!=null ? lhs.Tree : null);
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);
            	RewriteRuleSubtreeStream stream_rhs = new RewriteRuleSubtreeStream(adaptor, "rule rhs", rhs!=null ? rhs.Tree : null);

            	root_0 = (IASTNode)adaptor.GetNilNode();
            	// 473:3: -> ^( $d $lhs $rhs)
            	{
            	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:473:6: ^( $d $lhs $rhs)
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot(stream_d.NextNode(), root_1);

            	    adaptor.AddChild(root_1, stream_lhs.NextTree());
            	    adaptor.AddChild(root_1, stream_rhs.NextTree());

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;
            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);


            	// This gives lookupProperty() a chance to transform the tree to process collection properties (.elements, etc).
            	retval.Tree = LookupProperty((IASTNode) retval.Tree,false,true);

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
    // $ANTLR end "propertyRefPath"

    public class propertyRefIdent_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "propertyRefIdent"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:476:1: propertyRefIdent : p= identifier ;
    public HqlSqlWalker.propertyRefIdent_return propertyRefIdent() // throws RecognitionException [1]
    {   
        HqlSqlWalker.propertyRefIdent_return retval = new HqlSqlWalker.propertyRefIdent_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        HqlSqlWalker.identifier_return p = default(HqlSqlWalker.identifier_return);



        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:490:2: (p= identifier )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:490:4: p= identifier
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_identifier_in_propertyRefIdent2413);
            	p = identifier();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, p.Tree);

            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);


            	// In many cases, things other than property-refs are recognized
            	// by this propertyRef rule.  Some of those I have seen:
            	//  1) select-clause from-aliases
            	//  2) sql-functions
            	if ( IsNonQualifiedPropertyRef(((p != null) ? ((IASTNode)p.Tree) : null)) ) {
            		retval.Tree = LookupNonQualifiedProperty(((p != null) ? ((IASTNode)p.Tree) : null));
            	}
            	else {
            		Resolve(((p != null) ? ((IASTNode)p.Tree) : null));
            		retval.Tree = ((p != null) ? ((IASTNode)p.Tree) : null);
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
    // $ANTLR end "propertyRefIdent"

    public class propertyRefLhs_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "propertyRefLhs"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:493:1: propertyRefLhs : propertyRef ;
    public HqlSqlWalker.propertyRefLhs_return propertyRefLhs() // throws RecognitionException [1]
    {   
        HqlSqlWalker.propertyRefLhs_return retval = new HqlSqlWalker.propertyRefLhs_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        HqlSqlWalker.propertyRef_return propertyRef191 = default(HqlSqlWalker.propertyRef_return);



        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:494:2: ( propertyRef )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:494:4: propertyRef
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_propertyRef_in_propertyRefLhs2425);
            	propertyRef191 = propertyRef();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, propertyRef191.Tree);

            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "propertyRefLhs"

    public class aliasRef_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "aliasRef"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:497:1: aliasRef : i= identifier ;
    public HqlSqlWalker.aliasRef_return aliasRef() // throws RecognitionException [1]
    {   
        HqlSqlWalker.aliasRef_return retval = new HqlSqlWalker.aliasRef_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        HqlSqlWalker.identifier_return i = default(HqlSqlWalker.identifier_return);



        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:502:2: (i= identifier )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:502:4: i= identifier
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_identifier_in_aliasRef2446);
            	i = identifier();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, i.Tree);

            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);


            		LookupAlias(((IASTNode)retval.Tree));
            	
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
    // $ANTLR end "aliasRef"

    public class parameter_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "parameter"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:506:1: parameter : ( ^(c= COLON a= identifier ) -> ^() | ^(p= PARAM (n= NUM_INT )? ) -> {n != null}? ^() -> ^() );
    public HqlSqlWalker.parameter_return parameter() // throws RecognitionException [1]
    {   
        HqlSqlWalker.parameter_return retval = new HqlSqlWalker.parameter_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode c = null;
        IASTNode p = null;
        IASTNode n = null;
        HqlSqlWalker.identifier_return a = default(HqlSqlWalker.identifier_return);


        IASTNode c_tree=null;
        IASTNode p_tree=null;
        IASTNode n_tree=null;
        RewriteRuleNodeStream stream_PARAM = new RewriteRuleNodeStream(adaptor,"token PARAM");
        RewriteRuleNodeStream stream_NUM_INT = new RewriteRuleNodeStream(adaptor,"token NUM_INT");
        RewriteRuleNodeStream stream_COLON = new RewriteRuleNodeStream(adaptor,"token COLON");
        RewriteRuleSubtreeStream stream_identifier = new RewriteRuleSubtreeStream(adaptor,"rule identifier");
        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:507:2: ( ^(c= COLON a= identifier ) -> ^() | ^(p= PARAM (n= NUM_INT )? ) -> {n != null}? ^() -> ^() )
            int alt63 = 2;
            int LA63_0 = input.LA(1);

            if ( (LA63_0 == COLON) )
            {
                alt63 = 1;
            }
            else if ( (LA63_0 == PARAM) )
            {
                alt63 = 2;
            }
            else 
            {
                NoViableAltException nvae_d63s0 =
                    new NoViableAltException("", 63, 0, input);

                throw nvae_d63s0;
            }
            switch (alt63) 
            {
                case 1 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:507:4: ^(c= COLON a= identifier )
                    {
                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	c=(IASTNode)Match(input,COLON,FOLLOW_COLON_in_parameter2464);  
                    	stream_COLON.Add(c);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_identifier_in_parameter2468);
                    	a = identifier();
                    	state.followingStackPointer--;

                    	stream_identifier.Add(a.Tree);

                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}



                    	// AST REWRITE
                    	// elements:          
                    	// token labels:      
                    	// rule labels:       retval
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

                    	root_0 = (IASTNode)adaptor.GetNilNode();
                    	// 509:3: -> ^()
                    	{
                    	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:509:6: ^()
                    	    {
                    	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
                    	    root_1 = (IASTNode)adaptor.BecomeRoot(GenerateNamedParameter( c, ((a != null) ? ((IASTNode)a.Tree) : null) ), root_1);

                    	    adaptor.AddChild(root_0, root_1);
                    	    }

                    	}

                    	retval.Tree = root_0;
                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:510:4: ^(p= PARAM (n= NUM_INT )? )
                    {
                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	p=(IASTNode)Match(input,PARAM,FOLLOW_PARAM_in_parameter2489);  
                    	stream_PARAM.Add(p);



                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); 
                    	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:510:14: (n= NUM_INT )?
                    	    int alt62 = 2;
                    	    int LA62_0 = input.LA(1);

                    	    if ( (LA62_0 == NUM_INT) )
                    	    {
                    	        alt62 = 1;
                    	    }
                    	    switch (alt62) 
                    	    {
                    	        case 1 :
                    	            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:510:15: n= NUM_INT
                    	            {
                    	            	_last = (IASTNode)input.LT(1);
                    	            	n=(IASTNode)Match(input,NUM_INT,FOLLOW_NUM_INT_in_parameter2494);  
                    	            	stream_NUM_INT.Add(n);


                    	            }
                    	            break;

                    	    }


                    	    Match(input, Token.UP, null); 
                    	}adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}



                    	// AST REWRITE
                    	// elements:          
                    	// token labels:      
                    	// rule labels:       retval
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

                    	root_0 = (IASTNode)adaptor.GetNilNode();
                    	// 511:3: -> {n != null}? ^()
                    	if (n != null)
                    	{
                    	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:511:19: ^()
                    	    {
                    	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
                    	    root_1 = (IASTNode)adaptor.BecomeRoot(GenerateNamedParameter( p, n ), root_1);

                    	    adaptor.AddChild(root_0, root_1);
                    	    }

                    	}
                    	else // 512:3: -> ^()
                    	{
                    	    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:512:6: ^()
                    	    {
                    	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
                    	    root_1 = (IASTNode)adaptor.BecomeRoot(GeneratePositionalParameter( p ), root_1);

                    	    adaptor.AddChild(root_0, root_1);
                    	    }

                    	}

                    	retval.Tree = root_0;
                    }
                    break;

            }
            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "parameter"

    public class numericInteger_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "numericInteger"
    // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:515:1: numericInteger : NUM_INT ;
    public HqlSqlWalker.numericInteger_return numericInteger() // throws RecognitionException [1]
    {   
        HqlSqlWalker.numericInteger_return retval = new HqlSqlWalker.numericInteger_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode NUM_INT192 = null;

        IASTNode NUM_INT192_tree=null;

        try 
    	{
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:516:2: ( NUM_INT )
            // /Users/Steve/Projects/uNhAddins/Trunk/ANTLR-HQL/ANTLR-HQL/HqlSqlWalker.g:516:4: NUM_INT
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	NUM_INT192=(IASTNode)Match(input,NUM_INT,FOLLOW_NUM_INT_in_numericInteger2527); 
            		NUM_INT192_tree = (IASTNode)adaptor.DupNode(NUM_INT192);

            		adaptor.AddChild(root_0, NUM_INT192_tree);


            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);

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
    // $ANTLR end "numericInteger"

    // Delegated rules


	private void InitializeCyclicDFAs()
	{
	}

 

    public static readonly BitSet FOLLOW_selectStatement_in_statement168 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_updateStatement_in_statement172 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_deleteStatement_in_statement176 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_insertStatement_in_statement180 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_query_in_selectStatement191 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_UPDATE_in_updateStatement215 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_VERSIONED_in_updateStatement222 = new BitSet(new ulong[]{0x0000000000400000UL});
    public static readonly BitSet FOLLOW_fromClause_in_updateStatement228 = new BitSet(new ulong[]{0x0000400000000000UL});
    public static readonly BitSet FOLLOW_setClause_in_updateStatement232 = new BitSet(new ulong[]{0x0020000000000008UL});
    public static readonly BitSet FOLLOW_whereClause_in_updateStatement237 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_DELETE_in_deleteStatement280 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_fromClause_in_deleteStatement284 = new BitSet(new ulong[]{0x0020000000000008UL});
    public static readonly BitSet FOLLOW_whereClause_in_deleteStatement287 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_INSERT_in_insertStatement317 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_intoClause_in_insertStatement321 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000100000UL});
    public static readonly BitSet FOLLOW_query_in_insertStatement323 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_INTO_in_intoClause341 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_path_in_intoClause348 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000200000UL});
    public static readonly BitSet FOLLOW_insertablePropertySpec_in_intoClause353 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_RANGE_in_insertablePropertySpec370 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_IDENT_in_insertablePropertySpec373 = new BitSet(new ulong[]{0x0000000000000008UL,0x0040000000000000UL});
    public static readonly BitSet FOLLOW_SET_in_setClause390 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_assignment_in_setClause395 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000800000000UL});
    public static readonly BitSet FOLLOW_EQ_in_assignment422 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_propertyRef_in_assignment427 = new BitSet(new ulong[]{0x0082008000109000UL,0x0079E003ED109120UL});
    public static readonly BitSet FOLLOW_newValue_in_assignment433 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_expr_in_newValue449 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_query_in_newValue453 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_QUERY_in_query475 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_SELECT_FROM_in_query487 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_fromClause_in_query495 = new BitSet(new ulong[]{0x0000200000000008UL});
    public static readonly BitSet FOLLOW_selectClause_in_query504 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_whereClause_in_query519 = new BitSet(new ulong[]{0x0000020001000008UL});
    public static readonly BitSet FOLLOW_groupClause_in_query529 = new BitSet(new ulong[]{0x0000020000000008UL});
    public static readonly BitSet FOLLOW_orderClause_in_query539 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_ORDER_in_orderClause584 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_orderExprs_in_orderClause588 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_expr_in_orderExprs600 = new BitSet(new ulong[]{0x008200800010D102UL,0x0079E003ED009120UL});
    public static readonly BitSet FOLLOW_set_in_orderExprs602 = new BitSet(new ulong[]{0x0082008000109002UL,0x0079E003ED009120UL});
    public static readonly BitSet FOLLOW_orderExprs_in_orderExprs614 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_GROUP_in_groupClause628 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_groupClause633 = new BitSet(new ulong[]{0x0082008002109008UL,0x0079E003ED009120UL});
    public static readonly BitSet FOLLOW_HAVING_in_groupClause640 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_logicalExpr_in_groupClause642 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_SELECT_in_selectClause661 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_DISTINCT_in_selectClause668 = new BitSet(new ulong[]{0x0082008008129090UL,0x0079E003ED1091A4UL});
    public static readonly BitSet FOLLOW_selectExprList_in_selectClause674 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_selectExpr_in_selectExprList709 = new BitSet(new ulong[]{0x0082008008129092UL,0x0079E003ED1091A4UL});
    public static readonly BitSet FOLLOW_aliasedSelectExpr_in_selectExprList713 = new BitSet(new ulong[]{0x0082008008129092UL,0x0079E003ED1091A4UL});
    public static readonly BitSet FOLLOW_AS_in_aliasedSelectExpr737 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_selectExpr_in_aliasedSelectExpr741 = new BitSet(new ulong[]{0x0000000000008000UL,0x0040000008001000UL});
    public static readonly BitSet FOLLOW_identifier_in_aliasedSelectExpr745 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_propertyRef_in_selectExpr760 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ALL_in_selectExpr772 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_aliasRef_in_selectExpr776 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_OBJECT_in_selectExpr788 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_aliasRef_in_selectExpr792 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_constructor_in_selectExpr803 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_functionCall_in_selectExpr814 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_count_in_selectExpr819 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_collectionFunction_in_selectExpr824 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_literal_in_selectExpr832 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_arithmeticExpr_in_selectExpr837 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_query_in_selectExpr842 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_COUNT_in_count854 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_set_in_count856 = new BitSet(new ulong[]{0x0082008008129000UL,0x0079E003ED409120UL});
    public static readonly BitSet FOLLOW_aggregateExpr_in_count869 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_ROW_STAR_in_count873 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_CONSTRUCTOR_in_constructor889 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_path_in_constructor891 = new BitSet(new ulong[]{0x0082008008129098UL,0x0079E003ED1091A4UL});
    public static readonly BitSet FOLLOW_selectExpr_in_constructor895 = new BitSet(new ulong[]{0x0082008008129098UL,0x0079E003ED1091A4UL});
    public static readonly BitSet FOLLOW_aliasedSelectExpr_in_constructor899 = new BitSet(new ulong[]{0x0082008008129098UL,0x0079E003ED1091A4UL});
    public static readonly BitSet FOLLOW_expr_in_aggregateExpr915 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_collectionFunction_in_aggregateExpr921 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_FROM_in_fromClause941 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_fromElementList_in_fromClause945 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_fromElement_in_fromElementList963 = new BitSet(new ulong[]{0x0000000100000002UL,0x0000000000200400UL});
    public static readonly BitSet FOLLOW_RANGE_in_fromElement988 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_path_in_fromElement992 = new BitSet(new ulong[]{0x0000000000200008UL,0x0000000000000040UL});
    public static readonly BitSet FOLLOW_ALIAS_in_fromElement997 = new BitSet(new ulong[]{0x0000000000200008UL});
    public static readonly BitSet FOLLOW_FETCH_in_fromElement1004 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_joinElement_in_fromElement1031 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_FILTER_ENTITY_in_fromElement1046 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000040UL});
    public static readonly BitSet FOLLOW_ALIAS_in_fromElement1050 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_JOIN_in_joinElement1079 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_joinType_in_joinElement1084 = new BitSet(new ulong[]{0x0000000000208000UL,0x0040000008001000UL});
    public static readonly BitSet FOLLOW_FETCH_in_joinElement1094 = new BitSet(new ulong[]{0x0000000000008000UL,0x0040000008001000UL});
    public static readonly BitSet FOLLOW_propertyRef_in_joinElement1100 = new BitSet(new ulong[]{0x2000000000200008UL,0x0000000000000040UL});
    public static readonly BitSet FOLLOW_ALIAS_in_joinElement1105 = new BitSet(new ulong[]{0x2000000000200008UL});
    public static readonly BitSet FOLLOW_FETCH_in_joinElement1112 = new BitSet(new ulong[]{0x2000000000000008UL});
    public static readonly BitSet FOLLOW_WITH_in_joinElement1119 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_LEFT_in_joinType1155 = new BitSet(new ulong[]{0x0000040000000002UL});
    public static readonly BitSet FOLLOW_RIGHT_in_joinType1161 = new BitSet(new ulong[]{0x0000040000000002UL});
    public static readonly BitSet FOLLOW_OUTER_in_joinType1167 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_FULL_in_joinType1181 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_INNER_in_joinType1188 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_identifier_in_path1210 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_DOT_in_path1218 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_path_in_path1222 = new BitSet(new ulong[]{0x0000000000008000UL,0x0040000008001000UL});
    public static readonly BitSet FOLLOW_identifier_in_path1226 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_path_in_pathAsIdent1245 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WITH_in_withClause1286 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_logicalExpr_in_withClause1292 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_WHERE_in_whereClause1320 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_logicalExpr_in_whereClause1326 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_AND_in_logicalExpr1352 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_logicalExpr_in_logicalExpr1354 = new BitSet(new ulong[]{0x0000014404080440UL,0x00000F4800076000UL});
    public static readonly BitSet FOLLOW_logicalExpr_in_logicalExpr1356 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_OR_in_logicalExpr1363 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_logicalExpr_in_logicalExpr1365 = new BitSet(new ulong[]{0x0000014404080440UL,0x00000F4800076000UL});
    public static readonly BitSet FOLLOW_logicalExpr_in_logicalExpr1367 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NOT_in_logicalExpr1374 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_logicalExpr_in_logicalExpr1376 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_comparisonExpr_in_logicalExpr1382 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_EQ_in_comparisonExpr1404 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1406 = new BitSet(new ulong[]{0x0082808000109030UL,0x0079E003ED109120UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1408 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NE_in_comparisonExpr1415 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1417 = new BitSet(new ulong[]{0x0082808000109030UL,0x0079E003ED109120UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1419 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_LT_in_comparisonExpr1426 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1428 = new BitSet(new ulong[]{0x0082808000109030UL,0x0079E003ED109120UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1430 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_GT_in_comparisonExpr1437 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1439 = new BitSet(new ulong[]{0x0082808000109030UL,0x0079E003ED109120UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1441 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_LE_in_comparisonExpr1448 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1450 = new BitSet(new ulong[]{0x0082808000109030UL,0x0079E003ED109120UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1452 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_GE_in_comparisonExpr1459 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1461 = new BitSet(new ulong[]{0x0082808000109030UL,0x0079E003ED109120UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1463 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_LIKE_in_comparisonExpr1470 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1472 = new BitSet(new ulong[]{0x0082008000109000UL,0x0079E003ED009120UL});
    public static readonly BitSet FOLLOW_expr_in_comparisonExpr1474 = new BitSet(new ulong[]{0x0000000000040008UL});
    public static readonly BitSet FOLLOW_ESCAPE_in_comparisonExpr1479 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_comparisonExpr1481 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NOT_LIKE_in_comparisonExpr1493 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1495 = new BitSet(new ulong[]{0x0082008000109000UL,0x0079E003ED009120UL});
    public static readonly BitSet FOLLOW_expr_in_comparisonExpr1497 = new BitSet(new ulong[]{0x0000000000040008UL});
    public static readonly BitSet FOLLOW_ESCAPE_in_comparisonExpr1502 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_comparisonExpr1504 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_BETWEEN_in_comparisonExpr1516 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1518 = new BitSet(new ulong[]{0x0082808000109030UL,0x0079E003ED109120UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1520 = new BitSet(new ulong[]{0x0082808000109030UL,0x0079E003ED109120UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1522 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NOT_BETWEEN_in_comparisonExpr1529 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1531 = new BitSet(new ulong[]{0x0082808000109030UL,0x0079E003ED109120UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1533 = new BitSet(new ulong[]{0x0082808000109030UL,0x0079E003ED109120UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1535 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_IN_in_comparisonExpr1542 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1544 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000800UL});
    public static readonly BitSet FOLLOW_inRhs_in_comparisonExpr1546 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NOT_IN_in_comparisonExpr1554 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1556 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000800UL});
    public static readonly BitSet FOLLOW_inRhs_in_comparisonExpr1558 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_IS_NULL_in_comparisonExpr1566 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1568 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_IS_NOT_NULL_in_comparisonExpr1575 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1577 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_EXISTS_in_comparisonExpr1586 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_comparisonExpr1590 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_collectionFunctionOrSubselect_in_comparisonExpr1594 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_IN_LIST_in_inRhs1619 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_collectionFunctionOrSubselect_in_inRhs1623 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_expr_in_inRhs1627 = new BitSet(new ulong[]{0x0082008000109008UL,0x0079E003ED009120UL});
    public static readonly BitSet FOLLOW_expr_in_exprOrSubquery1643 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_query_in_exprOrSubquery1648 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ANY_in_exprOrSubquery1654 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_collectionFunctionOrSubselect_in_exprOrSubquery1656 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_ALL_in_exprOrSubquery1663 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_collectionFunctionOrSubselect_in_exprOrSubquery1665 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_SOME_in_exprOrSubquery1672 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_collectionFunctionOrSubselect_in_exprOrSubquery1674 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_collectionFunction_in_collectionFunctionOrSubselect1687 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_query_in_collectionFunctionOrSubselect1692 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_addrExpr_in_expr1706 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_VECTOR_EXPR_in_expr1718 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_expr1721 = new BitSet(new ulong[]{0x0082008000109008UL,0x0079E003ED009120UL});
    public static readonly BitSet FOLLOW_constant_in_expr1730 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_arithmeticExpr_in_expr1735 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_functionCall_in_expr1740 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_parameter_in_expr1752 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_count_in_expr1757 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_PLUS_in_arithmeticExpr1785 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_arithmeticExpr1787 = new BitSet(new ulong[]{0x0082008000109000UL,0x0079E003ED009120UL});
    public static readonly BitSet FOLLOW_expr_in_arithmeticExpr1789 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_MINUS_in_arithmeticExpr1796 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_arithmeticExpr1798 = new BitSet(new ulong[]{0x0082008000109000UL,0x0079E003ED009120UL});
    public static readonly BitSet FOLLOW_expr_in_arithmeticExpr1800 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_DIV_in_arithmeticExpr1807 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_arithmeticExpr1809 = new BitSet(new ulong[]{0x0082008000109000UL,0x0079E003ED009120UL});
    public static readonly BitSet FOLLOW_expr_in_arithmeticExpr1811 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_STAR_in_arithmeticExpr1818 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_arithmeticExpr1820 = new BitSet(new ulong[]{0x0082008000109000UL,0x0079E003ED009120UL});
    public static readonly BitSet FOLLOW_expr_in_arithmeticExpr1822 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_UNARY_MINUS_in_arithmeticExpr1830 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_arithmeticExpr1832 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_caseExpr_in_arithmeticExpr1840 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_CASE_in_caseExpr1852 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_WHEN_in_caseExpr1858 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_logicalExpr_in_caseExpr1860 = new BitSet(new ulong[]{0x0082008000109000UL,0x0079E003ED009120UL});
    public static readonly BitSet FOLLOW_expr_in_caseExpr1862 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_ELSE_in_caseExpr1869 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_caseExpr1871 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_CASE2_in_caseExpr1883 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_caseExpr1887 = new BitSet(new ulong[]{0x0800000000000000UL});
    public static readonly BitSet FOLLOW_WHEN_in_caseExpr1891 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_caseExpr1893 = new BitSet(new ulong[]{0x0082008000109000UL,0x0079E003ED009120UL});
    public static readonly BitSet FOLLOW_expr_in_caseExpr1895 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_ELSE_in_caseExpr1902 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_caseExpr1904 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_ELEMENTS_in_collectionFunction1926 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_propertyRef_in_collectionFunction1932 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_INDICES_in_collectionFunction1951 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_propertyRef_in_collectionFunction1957 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_METHOD_CALL_in_functionCall1982 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_pathAsIdent_in_functionCall1987 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000000000200UL});
    public static readonly BitSet FOLLOW_EXPR_LIST_in_functionCall1992 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_functionCall1995 = new BitSet(new ulong[]{0x008201C404189448UL,0x0079EF4BED07F120UL});
    public static readonly BitSet FOLLOW_comparisonExpr_in_functionCall1999 = new BitSet(new ulong[]{0x008201C404189448UL,0x0079EF4BED07F120UL});
    public static readonly BitSet FOLLOW_AGGREGATE_in_functionCall2018 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_aggregateExpr_in_functionCall2020 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_literal_in_constant2033 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_NULL_in_constant2038 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_TRUE_in_constant2045 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_FALSE_in_constant2055 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_JAVA_CONSTANT_in_constant2062 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_numericLiteral_in_literal2073 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_stringLiteral_in_literal2078 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_set_in_numericLiteral0 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_QUOTED_String_in_stringLiteral2120 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_set_in_identifier2131 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_addrExprDot_in_addrExpr2150 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_addrExprIndex_in_addrExpr2157 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_addrExprIdent_in_addrExpr2164 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_DOT_in_addrExprDot2188 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_addrExprLhs_in_addrExprDot2192 = new BitSet(new ulong[]{0x0000000008028800UL,0x0040000008001000UL});
    public static readonly BitSet FOLLOW_propertyName_in_addrExprDot2196 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_INDEX_OP_in_addrExprIndex2235 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_addrExprLhs_in_addrExprIndex2239 = new BitSet(new ulong[]{0x0082008000109000UL,0x0079E003ED009120UL});
    public static readonly BitSet FOLLOW_expr_in_addrExprIndex2243 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_identifier_in_addrExprIdent2275 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_addrExpr_in_addrExprLhs2303 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_identifier_in_propertyName2316 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_CLASS_in_propertyName2321 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ELEMENTS_in_propertyName2326 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_INDICES_in_propertyName2331 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_propertyRefPath_in_propertyRef2343 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_propertyRefIdent_in_propertyRef2348 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_DOT_in_propertyRefPath2368 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_propertyRefLhs_in_propertyRefPath2372 = new BitSet(new ulong[]{0x0000000008028800UL,0x0040000008001000UL});
    public static readonly BitSet FOLLOW_propertyName_in_propertyRefPath2376 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_identifier_in_propertyRefIdent2413 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_propertyRef_in_propertyRefLhs2425 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_identifier_in_aliasRef2446 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_COLON_in_parameter2464 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_identifier_in_parameter2468 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_PARAM_in_parameter2489 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_NUM_INT_in_parameter2494 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NUM_INT_in_numericInteger2527 = new BitSet(new ulong[]{0x0000000000000002UL});

}
}