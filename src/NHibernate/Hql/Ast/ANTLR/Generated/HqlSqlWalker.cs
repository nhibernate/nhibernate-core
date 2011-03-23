// $ANTLR 3.2 Sep 23, 2009 12:02:23 HqlSqlWalker.g 2011-03-23 12:04:52

// The variable 'variable' is assigned but its value is never used.
#pragma warning disable 168, 219
// Unreachable code detected.
#pragma warning disable 162


using System.Text;
using NHibernate.Hql.Ast.ANTLR.Tree;


using System;
using Antlr.Runtime;
using Antlr.Runtime.Tree;using IList 		= System.Collections.IList;
using ArrayList 	= System.Collections.ArrayList;
using Stack 		= Antlr.Runtime.Collections.StackList;



namespace  NHibernate.Hql.Ast.ANTLR 
{
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
		"NUM_DECIMAL", 
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

    public const int SELECT_COLUMNS = 142;
    public const int LT = 105;
    public const int EXPONENT = 128;
    public const int STAR = 116;
    public const int FLOAT_SUFFIX = 129;
    public const int FILTERS = 145;
    public const int LITERAL_by = 54;
    public const int PROPERTY_REF = 140;
    public const int THETA_JOINS = 144;
    public const int CASE = 55;
    public const int NEW = 37;
    public const int FILTER_ENTITY = 74;
    public const int PARAM = 121;
    public const int COUNT = 12;
    public const int NOT = 38;
    public const int EOF = -1;
    public const int UNARY_PLUS = 89;
    public const int QUOTED_String = 122;
    public const int ESCqs = 126;
    public const int WEIRD_IDENT = 91;
    public const int OPEN_BRACKET = 118;
    public const int FULL = 23;
    public const int ORDER_ELEMENT = 83;
    public const int IS_NULL = 78;
    public const int ESCAPE = 18;
    public const int INSERT = 29;
    public const int FROM_FRAGMENT = 133;
    public const int NAMED_PARAM = 147;
    public const int BOTH = 62;
    public const int SELECT_CLAUSE = 136;
    public const int NUM_DECIMAL = 95;
    public const int VERSIONED = 52;
    public const int EQ = 100;
    public const int SELECT = 45;
    public const int INTO = 30;
    public const int NE = 103;
    public const int GE = 108;
    public const int CONCAT = 109;
    public const int ID_LETTER = 125;
    public const int NULL = 39;
    public const int ELSE = 57;
    public const int SELECT_FROM = 87;
    public const int TRAILING = 68;
    public const int ON = 60;
    public const int NUM_LONG = 97;
    public const int NUM_DOUBLE = 94;
    public const int UNARY_MINUS = 88;
    public const int DELETE = 13;
    public const int INDICES = 27;
    public const int OF = 67;
    public const int METHOD_CALL = 79;
    public const int LEADING = 64;
    public const int METHOD_NAME = 146;
    public const int EMPTY = 63;
    public const int GROUP = 24;
    public const int WS = 127;
    public const int FETCH = 21;
    public const int VECTOR_EXPR = 90;
    public const int NOT_IN = 81;
    public const int SELECT_EXPR = 143;
    public const int NUM_INT = 93;
    public const int OR = 40;
    public const int ALIAS = 70;
    public const int JAVA_CONSTANT = 98;
    public const int CONSTANT = 92;
    public const int GT = 106;
    public const int QUERY = 84;
    public const int BNOT = 110;
    public const int INDEX_OP = 76;
    public const int NUM_FLOAT = 96;
    public const int FROM = 22;
    public const int END = 56;
    public const int FALSE = 20;
    public const int DISTINCT = 16;
    public const int T__131 = 131;
    public const int CONSTRUCTOR = 71;
    public const int T__132 = 132;
    public const int CLOSE_BRACKET = 119;
    public const int WHERE = 53;
    public const int CLASS = 11;
    public const int MEMBER = 65;
    public const int INNER = 28;
    public const int PROPERTIES = 43;
    public const int BOGUS = 148;
    public const int ORDER = 41;
    public const int MAX = 35;
    public const int UPDATE = 51;
    public const int JOIN_FRAGMENT = 135;
    public const int SQL_NE = 104;
    public const int AND = 6;
    public const int SUM = 48;
    public const int ASCENDING = 8;
    public const int EXPR_LIST = 73;
    public const int AS = 7;
    public const int IN = 26;
    public const int THEN = 58;
    public const int OBJECT = 66;
    public const int COMMA = 99;
    public const int SQL_TOKEN = 141;
    public const int IS = 31;
    public const int AVG = 9;
    public const int LEFT = 33;
    public const int SOME = 47;
    public const int ALL = 4;
    public const int BOR = 111;
    public const int IMPLIED_FROM = 134;
    public const int IDENT = 123;
    public const int CASE2 = 72;
    public const int BXOR = 112;
    public const int PLUS = 114;
    public const int EXISTS = 19;
    public const int DOT = 15;
    public const int WITH = 61;
    public const int LIKE = 34;
    public const int OUTER = 42;
    public const int ID_START_LETTER = 124;
    public const int LEFT_OUTER = 137;
    public const int ROW_STAR = 86;
    public const int NOT_LIKE = 82;
    public const int RIGHT_OUTER = 138;
    public const int RANGE = 85;
    public const int NOT_BETWEEN = 80;
    public const int HEX_DIGIT = 130;
    public const int SET = 46;
    public const int RIGHT = 44;
    public const int HAVING = 25;
    public const int MIN = 36;
    public const int IS_NOT_NULL = 77;
    public const int MINUS = 115;
    public const int ELEMENTS = 17;
    public const int BAND = 113;
    public const int TRUE = 49;
    public const int JOIN = 32;
    public const int IN_LIST = 75;
    public const int UNION = 50;
    public const int OPEN = 101;
    public const int COLON = 120;
    public const int ANY = 5;
    public const int CLOSE = 102;
    public const int WHEN = 59;
    public const int ALIAS_REF = 139;
    public const int DIV = 117;
    public const int DESCENDING = 14;
    public const int AGGREGATE = 69;
    public const int BETWEEN = 10;
    public const int LE = 107;

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
		get { return "HqlSqlWalker.g"; }
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
    // HqlSqlWalker.g:40:1: statement : ( selectStatement | updateStatement | deleteStatement | insertStatement );
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
            // HqlSqlWalker.g:41:2: ( selectStatement | updateStatement | deleteStatement | insertStatement )
            int alt1 = 4;
            switch ( input.LA(1) ) 
            {
            case UNION:
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
                    // HqlSqlWalker.g:41:4: selectStatement
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
                    // HqlSqlWalker.g:41:22: updateStatement
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
                    // HqlSqlWalker.g:41:40: deleteStatement
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
                    // HqlSqlWalker.g:41:58: insertStatement
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
    // HqlSqlWalker.g:44:1: selectStatement : query ;
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
            // HqlSqlWalker.g:45:2: ( query )
            // HqlSqlWalker.g:45:4: query
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
    // HqlSqlWalker.g:51:1: updateStatement : ^(u= UPDATE (v= VERSIONED )? f= fromClause s= setClause (w= whereClause )? ) -> ^( $u $f $s ( $w)? ) ;
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
        RewriteRuleSubtreeStream stream_whereClause = new RewriteRuleSubtreeStream(adaptor,"rule whereClause");
        RewriteRuleSubtreeStream stream_setClause = new RewriteRuleSubtreeStream(adaptor,"rule setClause");
        RewriteRuleSubtreeStream stream_fromClause = new RewriteRuleSubtreeStream(adaptor,"rule fromClause");
        try 
    	{
            // HqlSqlWalker.g:58:2: ( ^(u= UPDATE (v= VERSIONED )? f= fromClause s= setClause (w= whereClause )? ) -> ^( $u $f $s ( $w)? ) )
            // HqlSqlWalker.g:58:4: ^(u= UPDATE (v= VERSIONED )? f= fromClause s= setClause (w= whereClause )? )
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
            	// HqlSqlWalker.g:58:57: (v= VERSIONED )?
            	int alt2 = 2;
            	int LA2_0 = input.LA(1);

            	if ( (LA2_0 == VERSIONED) )
            	{
            	    alt2 = 1;
            	}
            	switch (alt2) 
            	{
            	    case 1 :
            	        // HqlSqlWalker.g:58:58: v= VERSIONED
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
            	// HqlSqlWalker.g:58:97: (w= whereClause )?
            	int alt3 = 2;
            	int LA3_0 = input.LA(1);

            	if ( (LA3_0 == WHERE) )
            	{
            	    alt3 = 1;
            	}
            	switch (alt3) 
            	{
            	    case 1 :
            	        // HqlSqlWalker.g:58:98: w= whereClause
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
            	// elements:          u, w, s, f
            	// token labels:      u
            	// rule labels:       f, w, retval, s
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	retval.Tree = root_0;
            	RewriteRuleNodeStream stream_u = new RewriteRuleNodeStream(adaptor, "token u", u);
            	RewriteRuleSubtreeStream stream_f = new RewriteRuleSubtreeStream(adaptor, "rule f", f!=null ? f.Tree : null);
            	RewriteRuleSubtreeStream stream_w = new RewriteRuleSubtreeStream(adaptor, "rule w", w!=null ? w.Tree : null);
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);
            	RewriteRuleSubtreeStream stream_s = new RewriteRuleSubtreeStream(adaptor, "rule s", s!=null ? s.Tree : null);

            	root_0 = (IASTNode)adaptor.GetNilNode();
            	// 59:3: -> ^( $u $f $s ( $w)? )
            	{
            	    // HqlSqlWalker.g:59:6: ^( $u $f $s ( $w)? )
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot(stream_u.NextNode(), root_1);

            	    adaptor.AddChild(root_1, stream_f.NextTree());
            	    adaptor.AddChild(root_1, stream_s.NextTree());
            	    // HqlSqlWalker.g:59:17: ( $w)?
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
            		PrepareVersioned( ((IASTNode)retval.Tree), v );
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
    // HqlSqlWalker.g:62:1: deleteStatement : ^( DELETE fromClause ( whereClause )? ) ;
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
            // HqlSqlWalker.g:68:2: ( ^( DELETE fromClause ( whereClause )? ) )
            // HqlSqlWalker.g:68:4: ^( DELETE fromClause ( whereClause )? )
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
            	// HqlSqlWalker.g:68:66: ( whereClause )?
            	int alt4 = 2;
            	int LA4_0 = input.LA(1);

            	if ( (LA4_0 == WHERE) )
            	{
            	    alt4 = 1;
            	}
            	switch (alt4) 
            	{
            	    case 1 :
            	        // HqlSqlWalker.g:68:67: whereClause
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
    // HqlSqlWalker.g:71:1: insertStatement : ^( INSERT intoClause query ) ;
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
            // HqlSqlWalker.g:80:2: ( ^( INSERT intoClause query ) )
            // HqlSqlWalker.g:80:4: ^( INSERT intoClause query )
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
    // HqlSqlWalker.g:83:1: intoClause : ^( INTO (p= path ) ps= insertablePropertySpec ) ;
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
            // HqlSqlWalker.g:87:2: ( ^( INTO (p= path ) ps= insertablePropertySpec ) )
            // HqlSqlWalker.g:87:4: ^( INTO (p= path ) ps= insertablePropertySpec )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	INTO12=(IASTNode)Match(input,INTO,FOLLOW_INTO_in_intoClause347); 
            		INTO12_tree = (IASTNode)adaptor.DupNode(INTO12);

            		root_1 = (IASTNode)adaptor.BecomeRoot(INTO12_tree, root_1);


            	 HandleClauseStart( INTO ); 

            	Match(input, Token.DOWN, null); 
            	// HqlSqlWalker.g:87:43: (p= path )
            	// HqlSqlWalker.g:87:44: p= path
            	{
            		_last = (IASTNode)input.LT(1);
            		PushFollow(FOLLOW_path_in_intoClause354);
            		p = path();
            		state.followingStackPointer--;

            		adaptor.AddChild(root_1, p.Tree);

            	}

            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_insertablePropertySpec_in_intoClause359);
            	ps = insertablePropertySpec();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_1, ps.Tree);

            	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	}


            }

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);


            		retval.Tree =  CreateIntoClause(((p != null) ? p.p : default(String)), ((ps != null) ? ((IASTNode)ps.Tree) : null));
            	
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
    // HqlSqlWalker.g:90:1: insertablePropertySpec : ^( RANGE ( IDENT )+ ) ;
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
            // HqlSqlWalker.g:91:2: ( ^( RANGE ( IDENT )+ ) )
            // HqlSqlWalker.g:91:4: ^( RANGE ( IDENT )+ )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	RANGE13=(IASTNode)Match(input,RANGE,FOLLOW_RANGE_in_insertablePropertySpec375); 
            		RANGE13_tree = (IASTNode)adaptor.DupNode(RANGE13);

            		root_1 = (IASTNode)adaptor.BecomeRoot(RANGE13_tree, root_1);



            	Match(input, Token.DOWN, null); 
            	// HqlSqlWalker.g:91:13: ( IDENT )+
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
            			    // HqlSqlWalker.g:91:14: IDENT
            			    {
            			    	_last = (IASTNode)input.LT(1);
            			    	IDENT14=(IASTNode)Match(input,IDENT,FOLLOW_IDENT_in_insertablePropertySpec378); 
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
            		;	// Stops C# compiler whining that label 'loop5' has no statements


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
    // HqlSqlWalker.g:94:1: setClause : ^( SET ( assignment )* ) ;
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
            // HqlSqlWalker.g:95:2: ( ^( SET ( assignment )* ) )
            // HqlSqlWalker.g:95:4: ^( SET ( assignment )* )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	SET15=(IASTNode)Match(input,SET,FOLLOW_SET_in_setClause395); 
            		SET15_tree = (IASTNode)adaptor.DupNode(SET15);

            		root_1 = (IASTNode)adaptor.BecomeRoot(SET15_tree, root_1);


            	 HandleClauseStart( SET ); 

            	if ( input.LA(1) == Token.DOWN )
            	{
            	    Match(input, Token.DOWN, null); 
            	    // HqlSqlWalker.g:95:41: ( assignment )*
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
            	    		    // HqlSqlWalker.g:95:42: assignment
            	    		    {
            	    		    	_last = (IASTNode)input.LT(1);
            	    		    	PushFollow(FOLLOW_assignment_in_setClause400);
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
    // HqlSqlWalker.g:98:1: assignment : ^( EQ (p= propertyRef ) ( newValue ) ) ;
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
            // HqlSqlWalker.g:104:2: ( ^( EQ (p= propertyRef ) ( newValue ) ) )
            // HqlSqlWalker.g:104:4: ^( EQ (p= propertyRef ) ( newValue ) )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	EQ17=(IASTNode)Match(input,EQ,FOLLOW_EQ_in_assignment427); 
            		EQ17_tree = (IASTNode)adaptor.DupNode(EQ17);

            		root_1 = (IASTNode)adaptor.BecomeRoot(EQ17_tree, root_1);



            	Match(input, Token.DOWN, null); 
            	// HqlSqlWalker.g:104:10: (p= propertyRef )
            	// HqlSqlWalker.g:104:11: p= propertyRef
            	{
            		_last = (IASTNode)input.LT(1);
            		PushFollow(FOLLOW_propertyRef_in_assignment432);
            		p = propertyRef();
            		state.followingStackPointer--;

            		adaptor.AddChild(root_1, p.Tree);

            	}

            	 Resolve(((p != null) ? ((IASTNode)p.Tree) : null)); 
            	// HqlSqlWalker.g:104:48: ( newValue )
            	// HqlSqlWalker.g:104:49: newValue
            	{
            		_last = (IASTNode)input.LT(1);
            		PushFollow(FOLLOW_newValue_in_assignment438);
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
    // HqlSqlWalker.g:108:1: newValue : ( expr | query );
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
            // HqlSqlWalker.g:109:2: ( expr | query )
            int alt7 = 2;
            int LA7_0 = input.LA(1);

            if ( (LA7_0 == COUNT || LA7_0 == DOT || LA7_0 == FALSE || LA7_0 == NULL || LA7_0 == TRUE || LA7_0 == CASE || LA7_0 == AGGREGATE || LA7_0 == CASE2 || LA7_0 == INDEX_OP || LA7_0 == METHOD_CALL || LA7_0 == UNARY_MINUS || (LA7_0 >= VECTOR_EXPR && LA7_0 <= WEIRD_IDENT) || (LA7_0 >= NUM_INT && LA7_0 <= JAVA_CONSTANT) || (LA7_0 >= BNOT && LA7_0 <= DIV) || (LA7_0 >= COLON && LA7_0 <= IDENT)) )
            {
                alt7 = 1;
            }
            else if ( (LA7_0 == UNION || LA7_0 == QUERY) )
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
                    // HqlSqlWalker.g:109:4: expr
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_expr_in_newValue454);
                    	expr19 = expr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, expr19.Tree);

                    }
                    break;
                case 2 :
                    // HqlSqlWalker.g:109:11: query
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_query_in_newValue458);
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
    // HqlSqlWalker.g:112:1: query : ( unionedQuery | ^( UNION unionedQuery query ) );
    public HqlSqlWalker.query_return query() // throws RecognitionException [1]
    {   
        HqlSqlWalker.query_return retval = new HqlSqlWalker.query_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode UNION22 = null;
        HqlSqlWalker.unionedQuery_return unionedQuery21 = default(HqlSqlWalker.unionedQuery_return);

        HqlSqlWalker.unionedQuery_return unionedQuery23 = default(HqlSqlWalker.unionedQuery_return);

        HqlSqlWalker.query_return query24 = default(HqlSqlWalker.query_return);


        IASTNode UNION22_tree=null;

        try 
    	{
            // HqlSqlWalker.g:113:2: ( unionedQuery | ^( UNION unionedQuery query ) )
            int alt8 = 2;
            int LA8_0 = input.LA(1);

            if ( (LA8_0 == QUERY) )
            {
                alt8 = 1;
            }
            else if ( (LA8_0 == UNION) )
            {
                alt8 = 2;
            }
            else 
            {
                NoViableAltException nvae_d8s0 =
                    new NoViableAltException("", 8, 0, input);

                throw nvae_d8s0;
            }
            switch (alt8) 
            {
                case 1 :
                    // HqlSqlWalker.g:113:4: unionedQuery
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_unionedQuery_in_query469);
                    	unionedQuery21 = unionedQuery();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, unionedQuery21.Tree);

                    }
                    break;
                case 2 :
                    // HqlSqlWalker.g:114:4: ^( UNION unionedQuery query )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	UNION22=(IASTNode)Match(input,UNION,FOLLOW_UNION_in_query476); 
                    		UNION22_tree = (IASTNode)adaptor.DupNode(UNION22);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(UNION22_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_unionedQuery_in_query478);
                    	unionedQuery23 = unionedQuery();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, unionedQuery23.Tree);
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_query_in_query480);
                    	query24 = query();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, query24.Tree);

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
    // $ANTLR end "query"

    public class unionedQuery_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "unionedQuery"
    // HqlSqlWalker.g:119:1: unionedQuery : ^( QUERY ^( SELECT_FROM f= fromClause (s= selectClause )? ) (w= whereClause )? (g= groupClause )? (o= orderClause )? ) -> ^( SELECT ( $s)? $f ( $w)? ( $g)? ( $o)? ) ;
    public HqlSqlWalker.unionedQuery_return unionedQuery() // throws RecognitionException [1]
    {   
        HqlSqlWalker.unionedQuery_return retval = new HqlSqlWalker.unionedQuery_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode QUERY25 = null;
        IASTNode SELECT_FROM26 = null;
        HqlSqlWalker.fromClause_return f = default(HqlSqlWalker.fromClause_return);

        HqlSqlWalker.selectClause_return s = default(HqlSqlWalker.selectClause_return);

        HqlSqlWalker.whereClause_return w = default(HqlSqlWalker.whereClause_return);

        HqlSqlWalker.groupClause_return g = default(HqlSqlWalker.groupClause_return);

        HqlSqlWalker.orderClause_return o = default(HqlSqlWalker.orderClause_return);


        IASTNode QUERY25_tree=null;
        IASTNode SELECT_FROM26_tree=null;
        RewriteRuleNodeStream stream_SELECT_FROM = new RewriteRuleNodeStream(adaptor,"token SELECT_FROM");
        RewriteRuleNodeStream stream_QUERY = new RewriteRuleNodeStream(adaptor,"token QUERY");
        RewriteRuleSubtreeStream stream_whereClause = new RewriteRuleSubtreeStream(adaptor,"rule whereClause");
        RewriteRuleSubtreeStream stream_orderClause = new RewriteRuleSubtreeStream(adaptor,"rule orderClause");
        RewriteRuleSubtreeStream stream_groupClause = new RewriteRuleSubtreeStream(adaptor,"rule groupClause");
        RewriteRuleSubtreeStream stream_fromClause = new RewriteRuleSubtreeStream(adaptor,"rule fromClause");
        RewriteRuleSubtreeStream stream_selectClause = new RewriteRuleSubtreeStream(adaptor,"rule selectClause");
        try 
    	{
            // HqlSqlWalker.g:126:2: ( ^( QUERY ^( SELECT_FROM f= fromClause (s= selectClause )? ) (w= whereClause )? (g= groupClause )? (o= orderClause )? ) -> ^( SELECT ( $s)? $f ( $w)? ( $g)? ( $o)? ) )
            // HqlSqlWalker.g:126:4: ^( QUERY ^( SELECT_FROM f= fromClause (s= selectClause )? ) (w= whereClause )? (g= groupClause )? (o= orderClause )? )
            {
            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	QUERY25=(IASTNode)Match(input,QUERY,FOLLOW_QUERY_in_unionedQuery503);  
            	stream_QUERY.Add(QUERY25);


            	 BeforeStatement( "select", SELECT ); 

            	Match(input, Token.DOWN, null); 
            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_2 = _last;
            	IASTNode _first_2 = null;
            	IASTNode root_2 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	SELECT_FROM26=(IASTNode)Match(input,SELECT_FROM,FOLLOW_SELECT_FROM_in_unionedQuery515);  
            	stream_SELECT_FROM.Add(SELECT_FROM26);



            	Match(input, Token.DOWN, null); 
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_fromClause_in_unionedQuery523);
            	f = fromClause();
            	state.followingStackPointer--;

            	stream_fromClause.Add(f.Tree);
            	// HqlSqlWalker.g:130:5: (s= selectClause )?
            	int alt9 = 2;
            	int LA9_0 = input.LA(1);

            	if ( (LA9_0 == SELECT) )
            	{
            	    alt9 = 1;
            	}
            	switch (alt9) 
            	{
            	    case 1 :
            	        // HqlSqlWalker.g:130:6: s= selectClause
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_selectClause_in_unionedQuery532);
            	        	s = selectClause();
            	        	state.followingStackPointer--;

            	        	stream_selectClause.Add(s.Tree);

            	        }
            	        break;

            	}


            	Match(input, Token.UP, null); adaptor.AddChild(root_1, root_2);_last = _save_last_2;
            	}

            	// HqlSqlWalker.g:132:4: (w= whereClause )?
            	int alt10 = 2;
            	int LA10_0 = input.LA(1);

            	if ( (LA10_0 == WHERE) )
            	{
            	    alt10 = 1;
            	}
            	switch (alt10) 
            	{
            	    case 1 :
            	        // HqlSqlWalker.g:132:5: w= whereClause
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_whereClause_in_unionedQuery547);
            	        	w = whereClause();
            	        	state.followingStackPointer--;

            	        	stream_whereClause.Add(w.Tree);

            	        }
            	        break;

            	}

            	// HqlSqlWalker.g:133:4: (g= groupClause )?
            	int alt11 = 2;
            	int LA11_0 = input.LA(1);

            	if ( (LA11_0 == GROUP) )
            	{
            	    alt11 = 1;
            	}
            	switch (alt11) 
            	{
            	    case 1 :
            	        // HqlSqlWalker.g:133:5: g= groupClause
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_groupClause_in_unionedQuery557);
            	        	g = groupClause();
            	        	state.followingStackPointer--;

            	        	stream_groupClause.Add(g.Tree);

            	        }
            	        break;

            	}

            	// HqlSqlWalker.g:134:4: (o= orderClause )?
            	int alt12 = 2;
            	int LA12_0 = input.LA(1);

            	if ( (LA12_0 == ORDER) )
            	{
            	    alt12 = 1;
            	}
            	switch (alt12) 
            	{
            	    case 1 :
            	        // HqlSqlWalker.g:134:5: o= orderClause
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_orderClause_in_unionedQuery567);
            	        	o = orderClause();
            	        	state.followingStackPointer--;

            	        	stream_orderClause.Add(o.Tree);

            	        }
            	        break;

            	}


            	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	}



            	// AST REWRITE
            	// elements:          s, o, f, g, w
            	// token labels:      
            	// rule labels:       w, f, g, retval, s, o
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_w = new RewriteRuleSubtreeStream(adaptor, "rule w", w!=null ? w.Tree : null);
            	RewriteRuleSubtreeStream stream_f = new RewriteRuleSubtreeStream(adaptor, "rule f", f!=null ? f.Tree : null);
            	RewriteRuleSubtreeStream stream_g = new RewriteRuleSubtreeStream(adaptor, "rule g", g!=null ? g.Tree : null);
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);
            	RewriteRuleSubtreeStream stream_s = new RewriteRuleSubtreeStream(adaptor, "rule s", s!=null ? s.Tree : null);
            	RewriteRuleSubtreeStream stream_o = new RewriteRuleSubtreeStream(adaptor, "rule o", o!=null ? o.Tree : null);

            	root_0 = (IASTNode)adaptor.GetNilNode();
            	// 136:2: -> ^( SELECT ( $s)? $f ( $w)? ( $g)? ( $o)? )
            	{
            	    // HqlSqlWalker.g:136:5: ^( SELECT ( $s)? $f ( $w)? ( $g)? ( $o)? )
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(SELECT, "SELECT"), root_1);

            	    // HqlSqlWalker.g:136:14: ( $s)?
            	    if ( stream_s.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_s.NextTree());

            	    }
            	    stream_s.Reset();
            	    adaptor.AddChild(root_1, stream_f.NextTree());
            	    // HqlSqlWalker.g:136:21: ( $w)?
            	    if ( stream_w.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_w.NextTree());

            	    }
            	    stream_w.Reset();
            	    // HqlSqlWalker.g:136:25: ( $g)?
            	    if ( stream_g.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_g.NextTree());

            	    }
            	    stream_g.Reset();
            	    // HqlSqlWalker.g:136:29: ( $o)?
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
    // $ANTLR end "unionedQuery"

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
    // HqlSqlWalker.g:139:1: orderClause : ^( ORDER orderExprs ) ;
    public HqlSqlWalker.orderClause_return orderClause() // throws RecognitionException [1]
    {   
        HqlSqlWalker.orderClause_return retval = new HqlSqlWalker.orderClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode ORDER27 = null;
        HqlSqlWalker.orderExprs_return orderExprs28 = default(HqlSqlWalker.orderExprs_return);


        IASTNode ORDER27_tree=null;

        try 
    	{
            // HqlSqlWalker.g:140:2: ( ^( ORDER orderExprs ) )
            // HqlSqlWalker.g:140:4: ^( ORDER orderExprs )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	ORDER27=(IASTNode)Match(input,ORDER,FOLLOW_ORDER_in_orderClause612); 
            		ORDER27_tree = (IASTNode)adaptor.DupNode(ORDER27);

            		root_1 = (IASTNode)adaptor.BecomeRoot(ORDER27_tree, root_1);


            	 HandleClauseStart( ORDER ); 

            	Match(input, Token.DOWN, null); 
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_orderExprs_in_orderClause616);
            	orderExprs28 = orderExprs();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_1, orderExprs28.Tree);

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
    // HqlSqlWalker.g:143:1: orderExprs : expr ( ASCENDING | DESCENDING )? ( orderExprs )? ;
    public HqlSqlWalker.orderExprs_return orderExprs() // throws RecognitionException [1]
    {   
        HqlSqlWalker.orderExprs_return retval = new HqlSqlWalker.orderExprs_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode set30 = null;
        HqlSqlWalker.expr_return expr29 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.orderExprs_return orderExprs31 = default(HqlSqlWalker.orderExprs_return);


        IASTNode set30_tree=null;

        try 
    	{
            // HqlSqlWalker.g:144:2: ( expr ( ASCENDING | DESCENDING )? ( orderExprs )? )
            // HqlSqlWalker.g:144:4: expr ( ASCENDING | DESCENDING )? ( orderExprs )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_expr_in_orderExprs628);
            	expr29 = expr();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, expr29.Tree);
            	// HqlSqlWalker.g:144:9: ( ASCENDING | DESCENDING )?
            	int alt13 = 2;
            	int LA13_0 = input.LA(1);

            	if ( (LA13_0 == ASCENDING || LA13_0 == DESCENDING) )
            	{
            	    alt13 = 1;
            	}
            	switch (alt13) 
            	{
            	    case 1 :
            	        // HqlSqlWalker.g:
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	set30 = (IASTNode)input.LT(1);
            	        	if ( input.LA(1) == ASCENDING || input.LA(1) == DESCENDING ) 
            	        	{
            	        	    input.Consume();

            	        	    set30_tree = (IASTNode)adaptor.DupNode(set30);

            	        	    adaptor.AddChild(root_0, set30_tree);

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

            	// HqlSqlWalker.g:144:37: ( orderExprs )?
            	int alt14 = 2;
            	int LA14_0 = input.LA(1);

            	if ( (LA14_0 == COUNT || LA14_0 == DOT || LA14_0 == FALSE || LA14_0 == NULL || LA14_0 == TRUE || LA14_0 == CASE || LA14_0 == AGGREGATE || LA14_0 == CASE2 || LA14_0 == INDEX_OP || LA14_0 == METHOD_CALL || LA14_0 == UNARY_MINUS || (LA14_0 >= VECTOR_EXPR && LA14_0 <= WEIRD_IDENT) || (LA14_0 >= NUM_INT && LA14_0 <= JAVA_CONSTANT) || (LA14_0 >= BNOT && LA14_0 <= DIV) || (LA14_0 >= COLON && LA14_0 <= IDENT)) )
            	{
            	    alt14 = 1;
            	}
            	switch (alt14) 
            	{
            	    case 1 :
            	        // HqlSqlWalker.g:144:38: orderExprs
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_orderExprs_in_orderExprs642);
            	        	orderExprs31 = orderExprs();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, orderExprs31.Tree);

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
    // HqlSqlWalker.g:147:1: groupClause : ^( GROUP ( expr )+ ( ^( HAVING logicalExpr ) )? ) ;
    public HqlSqlWalker.groupClause_return groupClause() // throws RecognitionException [1]
    {   
        HqlSqlWalker.groupClause_return retval = new HqlSqlWalker.groupClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode GROUP32 = null;
        IASTNode HAVING34 = null;
        HqlSqlWalker.expr_return expr33 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.logicalExpr_return logicalExpr35 = default(HqlSqlWalker.logicalExpr_return);


        IASTNode GROUP32_tree=null;
        IASTNode HAVING34_tree=null;

        try 
    	{
            // HqlSqlWalker.g:148:2: ( ^( GROUP ( expr )+ ( ^( HAVING logicalExpr ) )? ) )
            // HqlSqlWalker.g:148:4: ^( GROUP ( expr )+ ( ^( HAVING logicalExpr ) )? )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	GROUP32=(IASTNode)Match(input,GROUP,FOLLOW_GROUP_in_groupClause656); 
            		GROUP32_tree = (IASTNode)adaptor.DupNode(GROUP32);

            		root_1 = (IASTNode)adaptor.BecomeRoot(GROUP32_tree, root_1);


            	 HandleClauseStart( GROUP ); 

            	Match(input, Token.DOWN, null); 
            	// HqlSqlWalker.g:148:44: ( expr )+
            	int cnt15 = 0;
            	do 
            	{
            	    int alt15 = 2;
            	    int LA15_0 = input.LA(1);

            	    if ( (LA15_0 == COUNT || LA15_0 == DOT || LA15_0 == FALSE || LA15_0 == NULL || LA15_0 == TRUE || LA15_0 == CASE || LA15_0 == AGGREGATE || LA15_0 == CASE2 || LA15_0 == INDEX_OP || LA15_0 == METHOD_CALL || LA15_0 == UNARY_MINUS || (LA15_0 >= VECTOR_EXPR && LA15_0 <= WEIRD_IDENT) || (LA15_0 >= NUM_INT && LA15_0 <= JAVA_CONSTANT) || (LA15_0 >= BNOT && LA15_0 <= DIV) || (LA15_0 >= COLON && LA15_0 <= IDENT)) )
            	    {
            	        alt15 = 1;
            	    }


            	    switch (alt15) 
            		{
            			case 1 :
            			    // HqlSqlWalker.g:148:45: expr
            			    {
            			    	_last = (IASTNode)input.LT(1);
            			    	PushFollow(FOLLOW_expr_in_groupClause661);
            			    	expr33 = expr();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_1, expr33.Tree);

            			    }
            			    break;

            			default:
            			    if ( cnt15 >= 1 ) goto loop15;
            		            EarlyExitException eee15 =
            		                new EarlyExitException(15, input);
            		            throw eee15;
            	    }
            	    cnt15++;
            	} while (true);

            	loop15:
            		;	// Stops C# compiler whining that label 'loop15' has no statements

            	// HqlSqlWalker.g:148:52: ( ^( HAVING logicalExpr ) )?
            	int alt16 = 2;
            	int LA16_0 = input.LA(1);

            	if ( (LA16_0 == HAVING) )
            	{
            	    alt16 = 1;
            	}
            	switch (alt16) 
            	{
            	    case 1 :
            	        // HqlSqlWalker.g:148:54: ^( HAVING logicalExpr )
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	{
            	        	IASTNode _save_last_2 = _last;
            	        	IASTNode _first_2 = null;
            	        	IASTNode root_2 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	HAVING34=(IASTNode)Match(input,HAVING,FOLLOW_HAVING_in_groupClause668); 
            	        		HAVING34_tree = (IASTNode)adaptor.DupNode(HAVING34);

            	        		root_2 = (IASTNode)adaptor.BecomeRoot(HAVING34_tree, root_2);



            	        	Match(input, Token.DOWN, null); 
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_logicalExpr_in_groupClause670);
            	        	logicalExpr35 = logicalExpr();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_2, logicalExpr35.Tree);

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
    // HqlSqlWalker.g:151:1: selectClause : ^( SELECT (d= DISTINCT )? x= selectExprList ) -> ^( SELECT_CLAUSE[\"{select clause}\"] ( $d)? $x) ;
    public HqlSqlWalker.selectClause_return selectClause() // throws RecognitionException [1]
    {   
        HqlSqlWalker.selectClause_return retval = new HqlSqlWalker.selectClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode d = null;
        IASTNode SELECT36 = null;
        HqlSqlWalker.selectExprList_return x = default(HqlSqlWalker.selectExprList_return);


        IASTNode d_tree=null;
        IASTNode SELECT36_tree=null;
        RewriteRuleNodeStream stream_SELECT = new RewriteRuleNodeStream(adaptor,"token SELECT");
        RewriteRuleNodeStream stream_DISTINCT = new RewriteRuleNodeStream(adaptor,"token DISTINCT");
        RewriteRuleSubtreeStream stream_selectExprList = new RewriteRuleSubtreeStream(adaptor,"rule selectExprList");
        try 
    	{
            // HqlSqlWalker.g:152:2: ( ^( SELECT (d= DISTINCT )? x= selectExprList ) -> ^( SELECT_CLAUSE[\"{select clause}\"] ( $d)? $x) )
            // HqlSqlWalker.g:152:4: ^( SELECT (d= DISTINCT )? x= selectExprList )
            {
            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	SELECT36=(IASTNode)Match(input,SELECT,FOLLOW_SELECT_in_selectClause689);  
            	stream_SELECT.Add(SELECT36);


            	 HandleClauseStart( SELECT ); BeforeSelectClause(); 

            	Match(input, Token.DOWN, null); 
            	// HqlSqlWalker.g:152:68: (d= DISTINCT )?
            	int alt17 = 2;
            	int LA17_0 = input.LA(1);

            	if ( (LA17_0 == DISTINCT) )
            	{
            	    alt17 = 1;
            	}
            	switch (alt17) 
            	{
            	    case 1 :
            	        // HqlSqlWalker.g:152:69: d= DISTINCT
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	d=(IASTNode)Match(input,DISTINCT,FOLLOW_DISTINCT_in_selectClause696);  
            	        	stream_DISTINCT.Add(d);


            	        }
            	        break;

            	}

            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_selectExprList_in_selectClause702);
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
            	// 153:2: -> ^( SELECT_CLAUSE[\"{select clause}\"] ( $d)? $x)
            	{
            	    // HqlSqlWalker.g:153:5: ^( SELECT_CLAUSE[\"{select clause}\"] ( $d)? $x)
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(SELECT_CLAUSE, "{select clause}"), root_1);

            	    // HqlSqlWalker.g:153:40: ( $d)?
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
    // HqlSqlWalker.g:156:1: selectExprList : ( selectExpr | aliasedSelectExpr )+ ;
    public HqlSqlWalker.selectExprList_return selectExprList() // throws RecognitionException [1]
    {   
        HqlSqlWalker.selectExprList_return retval = new HqlSqlWalker.selectExprList_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        HqlSqlWalker.selectExpr_return selectExpr37 = default(HqlSqlWalker.selectExpr_return);

        HqlSqlWalker.aliasedSelectExpr_return aliasedSelectExpr38 = default(HqlSqlWalker.aliasedSelectExpr_return);




        		bool oldInSelect = _inSelect;
        		_inSelect = true;
        	
        try 
    	{
            // HqlSqlWalker.g:160:2: ( ( selectExpr | aliasedSelectExpr )+ )
            // HqlSqlWalker.g:160:4: ( selectExpr | aliasedSelectExpr )+
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// HqlSqlWalker.g:160:4: ( selectExpr | aliasedSelectExpr )+
            	int cnt18 = 0;
            	do 
            	{
            	    int alt18 = 3;
            	    int LA18_0 = input.LA(1);

            	    if ( (LA18_0 == ALL || LA18_0 == COUNT || LA18_0 == DOT || LA18_0 == ELEMENTS || LA18_0 == INDICES || LA18_0 == UNION || LA18_0 == CASE || LA18_0 == OBJECT || LA18_0 == AGGREGATE || (LA18_0 >= CONSTRUCTOR && LA18_0 <= CASE2) || LA18_0 == METHOD_CALL || LA18_0 == QUERY || LA18_0 == UNARY_MINUS || LA18_0 == WEIRD_IDENT || (LA18_0 >= NUM_INT && LA18_0 <= NUM_LONG) || (LA18_0 >= BNOT && LA18_0 <= DIV) || (LA18_0 >= COLON && LA18_0 <= IDENT)) )
            	    {
            	        alt18 = 1;
            	    }
            	    else if ( (LA18_0 == AS) )
            	    {
            	        alt18 = 2;
            	    }


            	    switch (alt18) 
            		{
            			case 1 :
            			    // HqlSqlWalker.g:160:6: selectExpr
            			    {
            			    	_last = (IASTNode)input.LT(1);
            			    	PushFollow(FOLLOW_selectExpr_in_selectExprList737);
            			    	selectExpr37 = selectExpr();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, selectExpr37.Tree);

            			    }
            			    break;
            			case 2 :
            			    // HqlSqlWalker.g:160:19: aliasedSelectExpr
            			    {
            			    	_last = (IASTNode)input.LT(1);
            			    	PushFollow(FOLLOW_aliasedSelectExpr_in_selectExprList741);
            			    	aliasedSelectExpr38 = aliasedSelectExpr();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, aliasedSelectExpr38.Tree);

            			    }
            			    break;

            			default:
            			    if ( cnt18 >= 1 ) goto loop18;
            		            EarlyExitException eee18 =
            		                new EarlyExitException(18, input);
            		            throw eee18;
            	    }
            	    cnt18++;
            	} while (true);

            	loop18:
            		;	// Stops C# compiler whining that label 'loop18' has no statements


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
    // HqlSqlWalker.g:165:1: aliasedSelectExpr : ^( AS se= selectExpr i= identifier ) ;
    public HqlSqlWalker.aliasedSelectExpr_return aliasedSelectExpr() // throws RecognitionException [1]
    {   
        HqlSqlWalker.aliasedSelectExpr_return retval = new HqlSqlWalker.aliasedSelectExpr_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode AS39 = null;
        HqlSqlWalker.selectExpr_return se = default(HqlSqlWalker.selectExpr_return);

        HqlSqlWalker.identifier_return i = default(HqlSqlWalker.identifier_return);


        IASTNode AS39_tree=null;

        try 
    	{
            // HqlSqlWalker.g:170:2: ( ^( AS se= selectExpr i= identifier ) )
            // HqlSqlWalker.g:170:4: ^( AS se= selectExpr i= identifier )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	AS39=(IASTNode)Match(input,AS,FOLLOW_AS_in_aliasedSelectExpr765); 
            		AS39_tree = (IASTNode)adaptor.DupNode(AS39);

            		root_1 = (IASTNode)adaptor.BecomeRoot(AS39_tree, root_1);



            	Match(input, Token.DOWN, null); 
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_selectExpr_in_aliasedSelectExpr769);
            	se = selectExpr();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_1, se.Tree);
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_identifier_in_aliasedSelectExpr773);
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
    // HqlSqlWalker.g:173:1: selectExpr : (p= propertyRef | ^( ALL ar2= aliasRef ) | ^( OBJECT ar3= aliasRef ) | con= constructor | functionCall | parameter | count | collectionFunction | literal | arithmeticExpr | query );
    public HqlSqlWalker.selectExpr_return selectExpr() // throws RecognitionException [1]
    {   
        HqlSqlWalker.selectExpr_return retval = new HqlSqlWalker.selectExpr_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode ALL40 = null;
        IASTNode OBJECT41 = null;
        HqlSqlWalker.propertyRef_return p = default(HqlSqlWalker.propertyRef_return);

        HqlSqlWalker.aliasRef_return ar2 = default(HqlSqlWalker.aliasRef_return);

        HqlSqlWalker.aliasRef_return ar3 = default(HqlSqlWalker.aliasRef_return);

        HqlSqlWalker.constructor_return con = default(HqlSqlWalker.constructor_return);

        HqlSqlWalker.functionCall_return functionCall42 = default(HqlSqlWalker.functionCall_return);

        HqlSqlWalker.parameter_return parameter43 = default(HqlSqlWalker.parameter_return);

        HqlSqlWalker.count_return count44 = default(HqlSqlWalker.count_return);

        HqlSqlWalker.collectionFunction_return collectionFunction45 = default(HqlSqlWalker.collectionFunction_return);

        HqlSqlWalker.literal_return literal46 = default(HqlSqlWalker.literal_return);

        HqlSqlWalker.arithmeticExpr_return arithmeticExpr47 = default(HqlSqlWalker.arithmeticExpr_return);

        HqlSqlWalker.query_return query48 = default(HqlSqlWalker.query_return);


        IASTNode ALL40_tree=null;
        IASTNode OBJECT41_tree=null;

        try 
    	{
            // HqlSqlWalker.g:174:2: (p= propertyRef | ^( ALL ar2= aliasRef ) | ^( OBJECT ar3= aliasRef ) | con= constructor | functionCall | parameter | count | collectionFunction | literal | arithmeticExpr | query )
            int alt19 = 11;
            switch ( input.LA(1) ) 
            {
            case DOT:
            case WEIRD_IDENT:
            case IDENT:
            	{
                alt19 = 1;
                }
                break;
            case ALL:
            	{
                alt19 = 2;
                }
                break;
            case OBJECT:
            	{
                alt19 = 3;
                }
                break;
            case CONSTRUCTOR:
            	{
                alt19 = 4;
                }
                break;
            case AGGREGATE:
            case METHOD_CALL:
            	{
                alt19 = 5;
                }
                break;
            case COLON:
            case PARAM:
            	{
                alt19 = 6;
                }
                break;
            case COUNT:
            	{
                alt19 = 7;
                }
                break;
            case ELEMENTS:
            case INDICES:
            	{
                alt19 = 8;
                }
                break;
            case NUM_INT:
            case NUM_DOUBLE:
            case NUM_DECIMAL:
            case NUM_FLOAT:
            case NUM_LONG:
            case QUOTED_String:
            	{
                alt19 = 9;
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
                alt19 = 10;
                }
                break;
            case UNION:
            case QUERY:
            	{
                alt19 = 11;
                }
                break;
            	default:
            	    NoViableAltException nvae_d19s0 =
            	        new NoViableAltException("", 19, 0, input);

            	    throw nvae_d19s0;
            }

            switch (alt19) 
            {
                case 1 :
                    // HqlSqlWalker.g:174:4: p= propertyRef
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_propertyRef_in_selectExpr788);
                    	p = propertyRef();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, p.Tree);
                    	 ResolveSelectExpression(((p != null) ? ((IASTNode)p.Tree) : null)); 

                    }
                    break;
                case 2 :
                    // HqlSqlWalker.g:175:4: ^( ALL ar2= aliasRef )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	ALL40=(IASTNode)Match(input,ALL,FOLLOW_ALL_in_selectExpr800); 
                    		ALL40_tree = (IASTNode)adaptor.DupNode(ALL40);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(ALL40_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_aliasRef_in_selectExpr804);
                    	ar2 = aliasRef();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, ar2.Tree);

                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}

                    	 ResolveSelectExpression(((ar2 != null) ? ((IASTNode)ar2.Tree) : null)); retval.Tree =  ((ar2 != null) ? ((IASTNode)ar2.Tree) : null); 

                    }
                    break;
                case 3 :
                    // HqlSqlWalker.g:176:4: ^( OBJECT ar3= aliasRef )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	OBJECT41=(IASTNode)Match(input,OBJECT,FOLLOW_OBJECT_in_selectExpr816); 
                    		OBJECT41_tree = (IASTNode)adaptor.DupNode(OBJECT41);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(OBJECT41_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_aliasRef_in_selectExpr820);
                    	ar3 = aliasRef();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, ar3.Tree);

                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}

                    	 ResolveSelectExpression(((ar3 != null) ? ((IASTNode)ar3.Tree) : null)); retval.Tree =  ((ar3 != null) ? ((IASTNode)ar3.Tree) : null); 

                    }
                    break;
                case 4 :
                    // HqlSqlWalker.g:177:4: con= constructor
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_constructor_in_selectExpr831);
                    	con = constructor();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, con.Tree);
                    	 ProcessConstructor(((con != null) ? ((IASTNode)con.Tree) : null)); 

                    }
                    break;
                case 5 :
                    // HqlSqlWalker.g:178:4: functionCall
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_functionCall_in_selectExpr842);
                    	functionCall42 = functionCall();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, functionCall42.Tree);

                    }
                    break;
                case 6 :
                    // HqlSqlWalker.g:179:4: parameter
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_parameter_in_selectExpr847);
                    	parameter43 = parameter();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, parameter43.Tree);

                    }
                    break;
                case 7 :
                    // HqlSqlWalker.g:180:4: count
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_count_in_selectExpr852);
                    	count44 = count();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, count44.Tree);

                    }
                    break;
                case 8 :
                    // HqlSqlWalker.g:181:4: collectionFunction
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_collectionFunction_in_selectExpr857);
                    	collectionFunction45 = collectionFunction();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, collectionFunction45.Tree);

                    }
                    break;
                case 9 :
                    // HqlSqlWalker.g:182:4: literal
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_literal_in_selectExpr865);
                    	literal46 = literal();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, literal46.Tree);

                    }
                    break;
                case 10 :
                    // HqlSqlWalker.g:183:4: arithmeticExpr
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_arithmeticExpr_in_selectExpr870);
                    	arithmeticExpr47 = arithmeticExpr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, arithmeticExpr47.Tree);

                    }
                    break;
                case 11 :
                    // HqlSqlWalker.g:184:4: query
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_query_in_selectExpr875);
                    	query48 = query();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, query48.Tree);

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
    // HqlSqlWalker.g:187:1: count : ^( COUNT ( DISTINCT | ALL )? ( aggregateExpr | ROW_STAR ) ) ;
    public HqlSqlWalker.count_return count() // throws RecognitionException [1]
    {   
        HqlSqlWalker.count_return retval = new HqlSqlWalker.count_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode COUNT49 = null;
        IASTNode set50 = null;
        IASTNode ROW_STAR52 = null;
        HqlSqlWalker.aggregateExpr_return aggregateExpr51 = default(HqlSqlWalker.aggregateExpr_return);


        IASTNode COUNT49_tree=null;
        IASTNode set50_tree=null;
        IASTNode ROW_STAR52_tree=null;

        try 
    	{
            // HqlSqlWalker.g:188:2: ( ^( COUNT ( DISTINCT | ALL )? ( aggregateExpr | ROW_STAR ) ) )
            // HqlSqlWalker.g:188:4: ^( COUNT ( DISTINCT | ALL )? ( aggregateExpr | ROW_STAR ) )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	COUNT49=(IASTNode)Match(input,COUNT,FOLLOW_COUNT_in_count887); 
            		COUNT49_tree = (IASTNode)adaptor.DupNode(COUNT49);

            		root_1 = (IASTNode)adaptor.BecomeRoot(COUNT49_tree, root_1);



            	Match(input, Token.DOWN, null); 
            	// HqlSqlWalker.g:188:12: ( DISTINCT | ALL )?
            	int alt20 = 2;
            	int LA20_0 = input.LA(1);

            	if ( (LA20_0 == ALL || LA20_0 == DISTINCT) )
            	{
            	    alt20 = 1;
            	}
            	switch (alt20) 
            	{
            	    case 1 :
            	        // HqlSqlWalker.g:
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	set50 = (IASTNode)input.LT(1);
            	        	if ( input.LA(1) == ALL || input.LA(1) == DISTINCT ) 
            	        	{
            	        	    input.Consume();

            	        	    set50_tree = (IASTNode)adaptor.DupNode(set50);

            	        	    adaptor.AddChild(root_1, set50_tree);

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

            	// HqlSqlWalker.g:188:32: ( aggregateExpr | ROW_STAR )
            	int alt21 = 2;
            	int LA21_0 = input.LA(1);

            	if ( (LA21_0 == COUNT || LA21_0 == DOT || LA21_0 == ELEMENTS || LA21_0 == FALSE || LA21_0 == INDICES || LA21_0 == NULL || LA21_0 == TRUE || LA21_0 == CASE || LA21_0 == AGGREGATE || LA21_0 == CASE2 || LA21_0 == INDEX_OP || LA21_0 == METHOD_CALL || LA21_0 == UNARY_MINUS || (LA21_0 >= VECTOR_EXPR && LA21_0 <= WEIRD_IDENT) || (LA21_0 >= NUM_INT && LA21_0 <= JAVA_CONSTANT) || (LA21_0 >= BNOT && LA21_0 <= DIV) || (LA21_0 >= COLON && LA21_0 <= IDENT)) )
            	{
            	    alt21 = 1;
            	}
            	else if ( (LA21_0 == ROW_STAR) )
            	{
            	    alt21 = 2;
            	}
            	else 
            	{
            	    NoViableAltException nvae_d21s0 =
            	        new NoViableAltException("", 21, 0, input);

            	    throw nvae_d21s0;
            	}
            	switch (alt21) 
            	{
            	    case 1 :
            	        // HqlSqlWalker.g:188:34: aggregateExpr
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_aggregateExpr_in_count902);
            	        	aggregateExpr51 = aggregateExpr();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, aggregateExpr51.Tree);

            	        }
            	        break;
            	    case 2 :
            	        // HqlSqlWalker.g:188:50: ROW_STAR
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	ROW_STAR52=(IASTNode)Match(input,ROW_STAR,FOLLOW_ROW_STAR_in_count906); 
            	        		ROW_STAR52_tree = (IASTNode)adaptor.DupNode(ROW_STAR52);

            	        		adaptor.AddChild(root_1, ROW_STAR52_tree);


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
    // HqlSqlWalker.g:191:1: constructor : ^( CONSTRUCTOR path ( selectExpr | aliasedSelectExpr )* ) ;
    public HqlSqlWalker.constructor_return constructor() // throws RecognitionException [1]
    {   
        HqlSqlWalker.constructor_return retval = new HqlSqlWalker.constructor_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode CONSTRUCTOR53 = null;
        HqlSqlWalker.path_return path54 = default(HqlSqlWalker.path_return);

        HqlSqlWalker.selectExpr_return selectExpr55 = default(HqlSqlWalker.selectExpr_return);

        HqlSqlWalker.aliasedSelectExpr_return aliasedSelectExpr56 = default(HqlSqlWalker.aliasedSelectExpr_return);


        IASTNode CONSTRUCTOR53_tree=null;

        try 
    	{
            // HqlSqlWalker.g:192:2: ( ^( CONSTRUCTOR path ( selectExpr | aliasedSelectExpr )* ) )
            // HqlSqlWalker.g:192:4: ^( CONSTRUCTOR path ( selectExpr | aliasedSelectExpr )* )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	CONSTRUCTOR53=(IASTNode)Match(input,CONSTRUCTOR,FOLLOW_CONSTRUCTOR_in_constructor922); 
            		CONSTRUCTOR53_tree = (IASTNode)adaptor.DupNode(CONSTRUCTOR53);

            		root_1 = (IASTNode)adaptor.BecomeRoot(CONSTRUCTOR53_tree, root_1);



            	Match(input, Token.DOWN, null); 
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_path_in_constructor924);
            	path54 = path();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_1, path54.Tree);
            	// HqlSqlWalker.g:192:23: ( selectExpr | aliasedSelectExpr )*
            	do 
            	{
            	    int alt22 = 3;
            	    int LA22_0 = input.LA(1);

            	    if ( (LA22_0 == ALL || LA22_0 == COUNT || LA22_0 == DOT || LA22_0 == ELEMENTS || LA22_0 == INDICES || LA22_0 == UNION || LA22_0 == CASE || LA22_0 == OBJECT || LA22_0 == AGGREGATE || (LA22_0 >= CONSTRUCTOR && LA22_0 <= CASE2) || LA22_0 == METHOD_CALL || LA22_0 == QUERY || LA22_0 == UNARY_MINUS || LA22_0 == WEIRD_IDENT || (LA22_0 >= NUM_INT && LA22_0 <= NUM_LONG) || (LA22_0 >= BNOT && LA22_0 <= DIV) || (LA22_0 >= COLON && LA22_0 <= IDENT)) )
            	    {
            	        alt22 = 1;
            	    }
            	    else if ( (LA22_0 == AS) )
            	    {
            	        alt22 = 2;
            	    }


            	    switch (alt22) 
            		{
            			case 1 :
            			    // HqlSqlWalker.g:192:25: selectExpr
            			    {
            			    	_last = (IASTNode)input.LT(1);
            			    	PushFollow(FOLLOW_selectExpr_in_constructor928);
            			    	selectExpr55 = selectExpr();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_1, selectExpr55.Tree);

            			    }
            			    break;
            			case 2 :
            			    // HqlSqlWalker.g:192:38: aliasedSelectExpr
            			    {
            			    	_last = (IASTNode)input.LT(1);
            			    	PushFollow(FOLLOW_aliasedSelectExpr_in_constructor932);
            			    	aliasedSelectExpr56 = aliasedSelectExpr();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_1, aliasedSelectExpr56.Tree);

            			    }
            			    break;

            			default:
            			    goto loop22;
            	    }
            	} while (true);

            	loop22:
            		;	// Stops C# compiler whining that label 'loop22' has no statements


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
    // HqlSqlWalker.g:195:1: aggregateExpr : ( expr | collectionFunction );
    public HqlSqlWalker.aggregateExpr_return aggregateExpr() // throws RecognitionException [1]
    {   
        HqlSqlWalker.aggregateExpr_return retval = new HqlSqlWalker.aggregateExpr_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        HqlSqlWalker.expr_return expr57 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.collectionFunction_return collectionFunction58 = default(HqlSqlWalker.collectionFunction_return);



        try 
    	{
            // HqlSqlWalker.g:196:2: ( expr | collectionFunction )
            int alt23 = 2;
            int LA23_0 = input.LA(1);

            if ( (LA23_0 == COUNT || LA23_0 == DOT || LA23_0 == FALSE || LA23_0 == NULL || LA23_0 == TRUE || LA23_0 == CASE || LA23_0 == AGGREGATE || LA23_0 == CASE2 || LA23_0 == INDEX_OP || LA23_0 == METHOD_CALL || LA23_0 == UNARY_MINUS || (LA23_0 >= VECTOR_EXPR && LA23_0 <= WEIRD_IDENT) || (LA23_0 >= NUM_INT && LA23_0 <= JAVA_CONSTANT) || (LA23_0 >= BNOT && LA23_0 <= DIV) || (LA23_0 >= COLON && LA23_0 <= IDENT)) )
            {
                alt23 = 1;
            }
            else if ( (LA23_0 == ELEMENTS || LA23_0 == INDICES) )
            {
                alt23 = 2;
            }
            else 
            {
                NoViableAltException nvae_d23s0 =
                    new NoViableAltException("", 23, 0, input);

                throw nvae_d23s0;
            }
            switch (alt23) 
            {
                case 1 :
                    // HqlSqlWalker.g:196:4: expr
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_expr_in_aggregateExpr948);
                    	expr57 = expr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, expr57.Tree);

                    }
                    break;
                case 2 :
                    // HqlSqlWalker.g:197:4: collectionFunction
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_collectionFunction_in_aggregateExpr954);
                    	collectionFunction58 = collectionFunction();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, collectionFunction58.Tree);

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
    // HqlSqlWalker.g:201:1: fromClause : ^(f= FROM fromElementList ) ;
    public HqlSqlWalker.fromClause_return fromClause() // throws RecognitionException [1]
    {   
        HqlSqlWalker.fromClause_return retval = new HqlSqlWalker.fromClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode f = null;
        HqlSqlWalker.fromElementList_return fromElementList59 = default(HqlSqlWalker.fromElementList_return);


        IASTNode f_tree=null;


        		// NOTE: This references the INPUT AST! (see http://www.antlr.org/doc/trees.html#Action Translation)
        		// the ouput AST (#fromClause) has not been built yet.
        		PrepareFromClauseInputTree((IASTNode) input.LT(1), input);
        	
        try 
    	{
            // HqlSqlWalker.g:207:2: ( ^(f= FROM fromElementList ) )
            // HqlSqlWalker.g:207:4: ^(f= FROM fromElementList )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	f=(IASTNode)Match(input,FROM,FOLLOW_FROM_in_fromClause974); 
            		f_tree = (IASTNode)adaptor.DupNode(f);

            		root_1 = (IASTNode)adaptor.BecomeRoot(f_tree, root_1);


            	 PushFromClause(f_tree); HandleClauseStart( FROM ); 

            	Match(input, Token.DOWN, null); 
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_fromElementList_in_fromClause978);
            	fromElementList59 = fromElementList();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_1, fromElementList59.Tree);

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
    // HqlSqlWalker.g:210:1: fromElementList : ( fromElement )+ ;
    public HqlSqlWalker.fromElementList_return fromElementList() // throws RecognitionException [1]
    {   
        HqlSqlWalker.fromElementList_return retval = new HqlSqlWalker.fromElementList_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        HqlSqlWalker.fromElement_return fromElement60 = default(HqlSqlWalker.fromElement_return);




        		bool oldInFrom = _inFrom;
        		_inFrom = true;
        		
        try 
    	{
            // HqlSqlWalker.g:214:2: ( ( fromElement )+ )
            // HqlSqlWalker.g:214:4: ( fromElement )+
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// HqlSqlWalker.g:214:4: ( fromElement )+
            	int cnt24 = 0;
            	do 
            	{
            	    int alt24 = 2;
            	    int LA24_0 = input.LA(1);

            	    if ( (LA24_0 == JOIN || LA24_0 == FILTER_ENTITY || LA24_0 == RANGE) )
            	    {
            	        alt24 = 1;
            	    }


            	    switch (alt24) 
            		{
            			case 1 :
            			    // HqlSqlWalker.g:214:5: fromElement
            			    {
            			    	_last = (IASTNode)input.LT(1);
            			    	PushFollow(FOLLOW_fromElement_in_fromElementList996);
            			    	fromElement60 = fromElement();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, fromElement60.Tree);

            			    }
            			    break;

            			default:
            			    if ( cnt24 >= 1 ) goto loop24;
            		            EarlyExitException eee24 =
            		                new EarlyExitException(24, input);
            		            throw eee24;
            	    }
            	    cnt24++;
            	} while (true);

            	loop24:
            		;	// Stops C# compiler whining that label 'loop24' has no statements


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
    // HqlSqlWalker.g:219:1: fromElement : ( ^( RANGE p= path (a= ALIAS )? (pf= FETCH )? ) -> {fromElement != null}? ^() -> | je= joinElement -> | fe= FILTER_ENTITY a3= ALIAS -> ^() );
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
        IASTNode RANGE61 = null;
        HqlSqlWalker.path_return p = default(HqlSqlWalker.path_return);

        HqlSqlWalker.joinElement_return je = default(HqlSqlWalker.joinElement_return);


        IASTNode a_tree=null;
        IASTNode pf_tree=null;
        IASTNode fe_tree=null;
        IASTNode a3_tree=null;
        IASTNode RANGE61_tree=null;
        RewriteRuleNodeStream stream_FILTER_ENTITY = new RewriteRuleNodeStream(adaptor,"token FILTER_ENTITY");
        RewriteRuleNodeStream stream_RANGE = new RewriteRuleNodeStream(adaptor,"token RANGE");
        RewriteRuleNodeStream stream_FETCH = new RewriteRuleNodeStream(adaptor,"token FETCH");
        RewriteRuleNodeStream stream_ALIAS = new RewriteRuleNodeStream(adaptor,"token ALIAS");
        RewriteRuleSubtreeStream stream_joinElement = new RewriteRuleSubtreeStream(adaptor,"rule joinElement");
        RewriteRuleSubtreeStream stream_path = new RewriteRuleSubtreeStream(adaptor,"rule path");

           IASTNode fromElement = null;

        try 
    	{
            // HqlSqlWalker.g:224:2: ( ^( RANGE p= path (a= ALIAS )? (pf= FETCH )? ) -> {fromElement != null}? ^() -> | je= joinElement -> | fe= FILTER_ENTITY a3= ALIAS -> ^() )
            int alt27 = 3;
            switch ( input.LA(1) ) 
            {
            case RANGE:
            	{
                alt27 = 1;
                }
                break;
            case JOIN:
            	{
                alt27 = 2;
                }
                break;
            case FILTER_ENTITY:
            	{
                alt27 = 3;
                }
                break;
            	default:
            	    NoViableAltException nvae_d27s0 =
            	        new NoViableAltException("", 27, 0, input);

            	    throw nvae_d27s0;
            }

            switch (alt27) 
            {
                case 1 :
                    // HqlSqlWalker.g:224:4: ^( RANGE p= path (a= ALIAS )? (pf= FETCH )? )
                    {
                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	RANGE61=(IASTNode)Match(input,RANGE,FOLLOW_RANGE_in_fromElement1021);  
                    	stream_RANGE.Add(RANGE61);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_path_in_fromElement1025);
                    	p = path();
                    	state.followingStackPointer--;

                    	stream_path.Add(p.Tree);
                    	// HqlSqlWalker.g:224:19: (a= ALIAS )?
                    	int alt25 = 2;
                    	int LA25_0 = input.LA(1);

                    	if ( (LA25_0 == ALIAS) )
                    	{
                    	    alt25 = 1;
                    	}
                    	switch (alt25) 
                    	{
                    	    case 1 :
                    	        // HqlSqlWalker.g:224:20: a= ALIAS
                    	        {
                    	        	_last = (IASTNode)input.LT(1);
                    	        	a=(IASTNode)Match(input,ALIAS,FOLLOW_ALIAS_in_fromElement1030);  
                    	        	stream_ALIAS.Add(a);


                    	        }
                    	        break;

                    	}

                    	// HqlSqlWalker.g:224:30: (pf= FETCH )?
                    	int alt26 = 2;
                    	int LA26_0 = input.LA(1);

                    	if ( (LA26_0 == FETCH) )
                    	{
                    	    alt26 = 1;
                    	}
                    	switch (alt26) 
                    	{
                    	    case 1 :
                    	        // HqlSqlWalker.g:224:31: pf= FETCH
                    	        {
                    	        	_last = (IASTNode)input.LT(1);
                    	        	pf=(IASTNode)Match(input,FETCH,FOLLOW_FETCH_in_fromElement1037);  
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
                    	// 225:3: -> {fromElement != null}? ^()
                    	if (fromElement != null)
                    	{
                    	    // HqlSqlWalker.g:225:29: ^()
                    	    {
                    	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
                    	    root_1 = (IASTNode)adaptor.BecomeRoot(fromElement, root_1);

                    	    adaptor.AddChild(root_0, root_1);
                    	    }

                    	}
                    	else // 226:3: ->
                    	{
                    	    root_0 = null;
                    	}

                    	retval.Tree = root_0;
                    }
                    break;
                case 2 :
                    // HqlSqlWalker.g:227:4: je= joinElement
                    {
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_joinElement_in_fromElement1064);
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
                    	// 228:3: ->
                    	{
                    	    root_0 = null;
                    	}

                    	retval.Tree = root_0;
                    }
                    break;
                case 3 :
                    // HqlSqlWalker.g:230:4: fe= FILTER_ENTITY a3= ALIAS
                    {
                    	_last = (IASTNode)input.LT(1);
                    	fe=(IASTNode)Match(input,FILTER_ENTITY,FOLLOW_FILTER_ENTITY_in_fromElement1079);  
                    	stream_FILTER_ENTITY.Add(fe);

                    	_last = (IASTNode)input.LT(1);
                    	a3=(IASTNode)Match(input,ALIAS,FOLLOW_ALIAS_in_fromElement1083);  
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
                    	// 231:3: -> ^()
                    	{
                    	    // HqlSqlWalker.g:231:6: ^()
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
    // HqlSqlWalker.g:234:1: joinElement : ^( JOIN (j= joinType )? (f= FETCH )? pRef= propertyRef (a= ALIAS )? (pf= FETCH )? ( ^( (with= WITH ) ( . )* ) )? ) ;
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
        IASTNode JOIN62 = null;
        IASTNode wildcard63 = null;
        HqlSqlWalker.joinType_return j = default(HqlSqlWalker.joinType_return);

        HqlSqlWalker.propertyRef_return pRef = default(HqlSqlWalker.propertyRef_return);


        IASTNode f_tree=null;
        IASTNode a_tree=null;
        IASTNode pf_tree=null;
        IASTNode with_tree=null;
        IASTNode JOIN62_tree=null;
        IASTNode wildcard63_tree=null;

        try 
    	{
            // HqlSqlWalker.g:238:2: ( ^( JOIN (j= joinType )? (f= FETCH )? pRef= propertyRef (a= ALIAS )? (pf= FETCH )? ( ^( (with= WITH ) ( . )* ) )? ) )
            // HqlSqlWalker.g:238:4: ^( JOIN (j= joinType )? (f= FETCH )? pRef= propertyRef (a= ALIAS )? (pf= FETCH )? ( ^( (with= WITH ) ( . )* ) )? )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	JOIN62=(IASTNode)Match(input,JOIN,FOLLOW_JOIN_in_joinElement1112); 
            		JOIN62_tree = (IASTNode)adaptor.DupNode(JOIN62);

            		root_1 = (IASTNode)adaptor.BecomeRoot(JOIN62_tree, root_1);



            	Match(input, Token.DOWN, null); 
            	// HqlSqlWalker.g:238:11: (j= joinType )?
            	int alt28 = 2;
            	int LA28_0 = input.LA(1);

            	if ( (LA28_0 == FULL || LA28_0 == INNER || LA28_0 == LEFT || LA28_0 == RIGHT) )
            	{
            	    alt28 = 1;
            	}
            	switch (alt28) 
            	{
            	    case 1 :
            	        // HqlSqlWalker.g:238:12: j= joinType
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_joinType_in_joinElement1117);
            	        	j = joinType();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, j.Tree);
            	        	 SetImpliedJoinType(((j != null) ? j.j : default(int))); 

            	        }
            	        break;

            	}

            	// HqlSqlWalker.g:238:56: (f= FETCH )?
            	int alt29 = 2;
            	int LA29_0 = input.LA(1);

            	if ( (LA29_0 == FETCH) )
            	{
            	    alt29 = 1;
            	}
            	switch (alt29) 
            	{
            	    case 1 :
            	        // HqlSqlWalker.g:238:57: f= FETCH
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	f=(IASTNode)Match(input,FETCH,FOLLOW_FETCH_in_joinElement1127); 
            	        		f_tree = (IASTNode)adaptor.DupNode(f);

            	        		adaptor.AddChild(root_1, f_tree);


            	        }
            	        break;

            	}

            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_propertyRef_in_joinElement1133);
            	pRef = propertyRef();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_1, pRef.Tree);
            	// HqlSqlWalker.g:238:84: (a= ALIAS )?
            	int alt30 = 2;
            	int LA30_0 = input.LA(1);

            	if ( (LA30_0 == ALIAS) )
            	{
            	    alt30 = 1;
            	}
            	switch (alt30) 
            	{
            	    case 1 :
            	        // HqlSqlWalker.g:238:85: a= ALIAS
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	a=(IASTNode)Match(input,ALIAS,FOLLOW_ALIAS_in_joinElement1138); 
            	        		a_tree = (IASTNode)adaptor.DupNode(a);

            	        		adaptor.AddChild(root_1, a_tree);


            	        }
            	        break;

            	}

            	// HqlSqlWalker.g:238:95: (pf= FETCH )?
            	int alt31 = 2;
            	int LA31_0 = input.LA(1);

            	if ( (LA31_0 == FETCH) )
            	{
            	    alt31 = 1;
            	}
            	switch (alt31) 
            	{
            	    case 1 :
            	        // HqlSqlWalker.g:238:96: pf= FETCH
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	pf=(IASTNode)Match(input,FETCH,FOLLOW_FETCH_in_joinElement1145); 
            	        		pf_tree = (IASTNode)adaptor.DupNode(pf);

            	        		adaptor.AddChild(root_1, pf_tree);


            	        }
            	        break;

            	}

            	// HqlSqlWalker.g:238:107: ( ^( (with= WITH ) ( . )* ) )?
            	int alt33 = 2;
            	int LA33_0 = input.LA(1);

            	if ( (LA33_0 == WITH) )
            	{
            	    alt33 = 1;
            	}
            	switch (alt33) 
            	{
            	    case 1 :
            	        // HqlSqlWalker.g:238:108: ^( (with= WITH ) ( . )* )
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	{
            	        	IASTNode _save_last_2 = _last;
            	        	IASTNode _first_2 = null;
            	        	IASTNode root_2 = (IASTNode)adaptor.GetNilNode();// HqlSqlWalker.g:238:110: (with= WITH )
            	        	// HqlSqlWalker.g:238:111: with= WITH
            	        	{
            	        		_last = (IASTNode)input.LT(1);
            	        		with=(IASTNode)Match(input,WITH,FOLLOW_WITH_in_joinElement1154); 
            	        			with_tree = (IASTNode)adaptor.DupNode(with);

            	        			adaptor.AddChild(root_2, with_tree);


            	        	}



            	        	if ( input.LA(1) == Token.DOWN )
            	        	{
            	        	    Match(input, Token.DOWN, null); 
            	        	    // HqlSqlWalker.g:238:122: ( . )*
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
            	        	    		    // HqlSqlWalker.g:238:122: .
            	        	    		    {
            	        	    		    	_last = (IASTNode)input.LT(1);
            	        	    		    	wildcard63 = (IASTNode)input.LT(1);
            	        	    		    	MatchAny(input); 
            	        	    		    	wildcard63_tree = (IASTNode)adaptor.DupTree(wildcard63);
            	        	    		    	adaptor.AddChild(root_2, wildcard63_tree);


            	        	    		    }
            	        	    		    break;

            	        	    		default:
            	        	    		    goto loop32;
            	        	        }
            	        	    } while (true);

            	        	    loop32:
            	        	    	;	// Stops C# compiler whining that label 'loop32' has no statements


            	        	    Match(input, Token.UP, null); 
            	        	}adaptor.AddChild(root_1, root_2);_last = _save_last_2;
            	        	}


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
    // HqlSqlWalker.g:247:1: joinType returns [int j] : ( ( (left= LEFT | right= RIGHT ) (outer= OUTER )? ) | FULL | INNER );
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
        IASTNode FULL64 = null;
        IASTNode INNER65 = null;

        IASTNode left_tree=null;
        IASTNode right_tree=null;
        IASTNode outer_tree=null;
        IASTNode FULL64_tree=null;
        IASTNode INNER65_tree=null;


           retval.j =  INNER;

        try 
    	{
            // HqlSqlWalker.g:251:2: ( ( (left= LEFT | right= RIGHT ) (outer= OUTER )? ) | FULL | INNER )
            int alt36 = 3;
            switch ( input.LA(1) ) 
            {
            case LEFT:
            case RIGHT:
            	{
                alt36 = 1;
                }
                break;
            case FULL:
            	{
                alt36 = 2;
                }
                break;
            case INNER:
            	{
                alt36 = 3;
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
                    // HqlSqlWalker.g:251:4: ( (left= LEFT | right= RIGHT ) (outer= OUTER )? )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	// HqlSqlWalker.g:251:4: ( (left= LEFT | right= RIGHT ) (outer= OUTER )? )
                    	// HqlSqlWalker.g:251:6: (left= LEFT | right= RIGHT ) (outer= OUTER )?
                    	{
                    		// HqlSqlWalker.g:251:6: (left= LEFT | right= RIGHT )
                    		int alt34 = 2;
                    		int LA34_0 = input.LA(1);

                    		if ( (LA34_0 == LEFT) )
                    		{
                    		    alt34 = 1;
                    		}
                    		else if ( (LA34_0 == RIGHT) )
                    		{
                    		    alt34 = 2;
                    		}
                    		else 
                    		{
                    		    NoViableAltException nvae_d34s0 =
                    		        new NoViableAltException("", 34, 0, input);

                    		    throw nvae_d34s0;
                    		}
                    		switch (alt34) 
                    		{
                    		    case 1 :
                    		        // HqlSqlWalker.g:251:7: left= LEFT
                    		        {
                    		        	_last = (IASTNode)input.LT(1);
                    		        	left=(IASTNode)Match(input,LEFT,FOLLOW_LEFT_in_joinType1195); 
                    		        		left_tree = (IASTNode)adaptor.DupNode(left);

                    		        		adaptor.AddChild(root_0, left_tree);


                    		        }
                    		        break;
                    		    case 2 :
                    		        // HqlSqlWalker.g:251:19: right= RIGHT
                    		        {
                    		        	_last = (IASTNode)input.LT(1);
                    		        	right=(IASTNode)Match(input,RIGHT,FOLLOW_RIGHT_in_joinType1201); 
                    		        		right_tree = (IASTNode)adaptor.DupNode(right);

                    		        		adaptor.AddChild(root_0, right_tree);


                    		        }
                    		        break;

                    		}

                    		// HqlSqlWalker.g:251:32: (outer= OUTER )?
                    		int alt35 = 2;
                    		int LA35_0 = input.LA(1);

                    		if ( (LA35_0 == OUTER) )
                    		{
                    		    alt35 = 1;
                    		}
                    		switch (alt35) 
                    		{
                    		    case 1 :
                    		        // HqlSqlWalker.g:251:33: outer= OUTER
                    		        {
                    		        	_last = (IASTNode)input.LT(1);
                    		        	outer=(IASTNode)Match(input,OUTER,FOLLOW_OUTER_in_joinType1207); 
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
                    // HqlSqlWalker.g:257:4: FULL
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	FULL64=(IASTNode)Match(input,FULL,FOLLOW_FULL_in_joinType1221); 
                    		FULL64_tree = (IASTNode)adaptor.DupNode(FULL64);

                    		adaptor.AddChild(root_0, FULL64_tree);


                    			retval.j =  FULL;
                    		

                    }
                    break;
                case 3 :
                    // HqlSqlWalker.g:260:4: INNER
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	INNER65=(IASTNode)Match(input,INNER,FOLLOW_INNER_in_joinType1228); 
                    		INNER65_tree = (IASTNode)adaptor.DupNode(INNER65);

                    		adaptor.AddChild(root_0, INNER65_tree);


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
    // HqlSqlWalker.g:267:1: path returns [String p] : (a= identifier | ^( DOT x= path y= identifier ) );
    public HqlSqlWalker.path_return path() // throws RecognitionException [1]
    {   
        HqlSqlWalker.path_return retval = new HqlSqlWalker.path_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode DOT66 = null;
        HqlSqlWalker.identifier_return a = default(HqlSqlWalker.identifier_return);

        HqlSqlWalker.path_return x = default(HqlSqlWalker.path_return);

        HqlSqlWalker.identifier_return y = default(HqlSqlWalker.identifier_return);


        IASTNode DOT66_tree=null;

        try 
    	{
            // HqlSqlWalker.g:268:2: (a= identifier | ^( DOT x= path y= identifier ) )
            int alt37 = 2;
            int LA37_0 = input.LA(1);

            if ( (LA37_0 == WEIRD_IDENT || LA37_0 == IDENT) )
            {
                alt37 = 1;
            }
            else if ( (LA37_0 == DOT) )
            {
                alt37 = 2;
            }
            else 
            {
                NoViableAltException nvae_d37s0 =
                    new NoViableAltException("", 37, 0, input);

                throw nvae_d37s0;
            }
            switch (alt37) 
            {
                case 1 :
                    // HqlSqlWalker.g:268:4: a= identifier
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_identifier_in_path1250);
                    	a = identifier();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, a.Tree);
                    	 retval.p =  ((a != null) ? ((IASTNode)a.Start) : null).ToString();

                    }
                    break;
                case 2 :
                    // HqlSqlWalker.g:269:4: ^( DOT x= path y= identifier )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	DOT66=(IASTNode)Match(input,DOT,FOLLOW_DOT_in_path1258); 
                    		DOT66_tree = (IASTNode)adaptor.DupNode(DOT66);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(DOT66_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_path_in_path1262);
                    	x = path();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, x.Tree);
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_identifier_in_path1266);
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
    // HqlSqlWalker.g:277:1: pathAsIdent : path -> ^( IDENT[$path.p] ) ;
    public HqlSqlWalker.pathAsIdent_return pathAsIdent() // throws RecognitionException [1]
    {   
        HqlSqlWalker.pathAsIdent_return retval = new HqlSqlWalker.pathAsIdent_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        HqlSqlWalker.path_return path67 = default(HqlSqlWalker.path_return);


        RewriteRuleSubtreeStream stream_path = new RewriteRuleSubtreeStream(adaptor,"rule path");
        try 
    	{
            // HqlSqlWalker.g:278:5: ( path -> ^( IDENT[$path.p] ) )
            // HqlSqlWalker.g:278:7: path
            {
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_path_in_pathAsIdent1285);
            	path67 = path();
            	state.followingStackPointer--;

            	stream_path.Add(path67.Tree);


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
            	// 279:5: -> ^( IDENT[$path.p] )
            	{
            	    // HqlSqlWalker.g:279:8: ^( IDENT[$path.p] )
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(IDENT, ((path67 != null) ? path67.p : default(String))), root_1);

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
    // HqlSqlWalker.g:282:1: withClause : ^(w= WITH b= logicalExpr ) -> ^( $w $b) ;
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
            // HqlSqlWalker.g:289:2: ( ^(w= WITH b= logicalExpr ) -> ^( $w $b) )
            // HqlSqlWalker.g:289:4: ^(w= WITH b= logicalExpr )
            {
            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	w=(IASTNode)Match(input,WITH,FOLLOW_WITH_in_withClause1326);  
            	stream_WITH.Add(w);


            	 HandleClauseStart( WITH ); 

            	Match(input, Token.DOWN, null); 
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_logicalExpr_in_withClause1332);
            	b = logicalExpr();
            	state.followingStackPointer--;

            	stream_logicalExpr.Add(b.Tree);

            	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	}



            	// AST REWRITE
            	// elements:          w, b
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
            	// 290:2: -> ^( $w $b)
            	{
            	    // HqlSqlWalker.g:290:5: ^( $w $b)
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
    // HqlSqlWalker.g:293:1: whereClause : ^(w= WHERE b= logicalExpr ) -> ^( $w $b) ;
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
            // HqlSqlWalker.g:294:2: ( ^(w= WHERE b= logicalExpr ) -> ^( $w $b) )
            // HqlSqlWalker.g:294:4: ^(w= WHERE b= logicalExpr )
            {
            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	w=(IASTNode)Match(input,WHERE,FOLLOW_WHERE_in_whereClause1360);  
            	stream_WHERE.Add(w);


            	 HandleClauseStart( WHERE ); 

            	Match(input, Token.DOWN, null); 
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_logicalExpr_in_whereClause1366);
            	b = logicalExpr();
            	state.followingStackPointer--;

            	stream_logicalExpr.Add(b.Tree);

            	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	}



            	// AST REWRITE
            	// elements:          w, b
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
            	// 295:2: -> ^( $w $b)
            	{
            	    // HqlSqlWalker.g:295:5: ^( $w $b)
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
    // HqlSqlWalker.g:298:1: logicalExpr : ( ^( AND logicalExpr logicalExpr ) | ^( OR logicalExpr logicalExpr ) | ^( NOT logicalExpr ) | comparisonExpr | functionCall | logicalPath );
    public HqlSqlWalker.logicalExpr_return logicalExpr() // throws RecognitionException [1]
    {   
        HqlSqlWalker.logicalExpr_return retval = new HqlSqlWalker.logicalExpr_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode AND68 = null;
        IASTNode OR71 = null;
        IASTNode NOT74 = null;
        HqlSqlWalker.logicalExpr_return logicalExpr69 = default(HqlSqlWalker.logicalExpr_return);

        HqlSqlWalker.logicalExpr_return logicalExpr70 = default(HqlSqlWalker.logicalExpr_return);

        HqlSqlWalker.logicalExpr_return logicalExpr72 = default(HqlSqlWalker.logicalExpr_return);

        HqlSqlWalker.logicalExpr_return logicalExpr73 = default(HqlSqlWalker.logicalExpr_return);

        HqlSqlWalker.logicalExpr_return logicalExpr75 = default(HqlSqlWalker.logicalExpr_return);

        HqlSqlWalker.comparisonExpr_return comparisonExpr76 = default(HqlSqlWalker.comparisonExpr_return);

        HqlSqlWalker.functionCall_return functionCall77 = default(HqlSqlWalker.functionCall_return);

        HqlSqlWalker.logicalPath_return logicalPath78 = default(HqlSqlWalker.logicalPath_return);


        IASTNode AND68_tree=null;
        IASTNode OR71_tree=null;
        IASTNode NOT74_tree=null;

        try 
    	{
            // HqlSqlWalker.g:299:2: ( ^( AND logicalExpr logicalExpr ) | ^( OR logicalExpr logicalExpr ) | ^( NOT logicalExpr ) | comparisonExpr | functionCall | logicalPath )
            int alt38 = 6;
            switch ( input.LA(1) ) 
            {
            case AND:
            	{
                alt38 = 1;
                }
                break;
            case OR:
            	{
                alt38 = 2;
                }
                break;
            case NOT:
            	{
                alt38 = 3;
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
                alt38 = 4;
                }
                break;
            case AGGREGATE:
            case METHOD_CALL:
            	{
                alt38 = 5;
                }
                break;
            case DOT:
            case INDEX_OP:
            case WEIRD_IDENT:
            case IDENT:
            	{
                alt38 = 6;
                }
                break;
            	default:
            	    NoViableAltException nvae_d38s0 =
            	        new NoViableAltException("", 38, 0, input);

            	    throw nvae_d38s0;
            }

            switch (alt38) 
            {
                case 1 :
                    // HqlSqlWalker.g:299:4: ^( AND logicalExpr logicalExpr )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	AND68=(IASTNode)Match(input,AND,FOLLOW_AND_in_logicalExpr1392); 
                    		AND68_tree = (IASTNode)adaptor.DupNode(AND68);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(AND68_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_logicalExpr_in_logicalExpr1394);
                    	logicalExpr69 = logicalExpr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, logicalExpr69.Tree);
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_logicalExpr_in_logicalExpr1396);
                    	logicalExpr70 = logicalExpr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, logicalExpr70.Tree);

                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}


                    }
                    break;
                case 2 :
                    // HqlSqlWalker.g:300:4: ^( OR logicalExpr logicalExpr )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	OR71=(IASTNode)Match(input,OR,FOLLOW_OR_in_logicalExpr1403); 
                    		OR71_tree = (IASTNode)adaptor.DupNode(OR71);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(OR71_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_logicalExpr_in_logicalExpr1405);
                    	logicalExpr72 = logicalExpr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, logicalExpr72.Tree);
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_logicalExpr_in_logicalExpr1407);
                    	logicalExpr73 = logicalExpr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, logicalExpr73.Tree);

                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}


                    }
                    break;
                case 3 :
                    // HqlSqlWalker.g:301:4: ^( NOT logicalExpr )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	NOT74=(IASTNode)Match(input,NOT,FOLLOW_NOT_in_logicalExpr1414); 
                    		NOT74_tree = (IASTNode)adaptor.DupNode(NOT74);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(NOT74_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_logicalExpr_in_logicalExpr1416);
                    	logicalExpr75 = logicalExpr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, logicalExpr75.Tree);

                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}


                    }
                    break;
                case 4 :
                    // HqlSqlWalker.g:302:4: comparisonExpr
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_comparisonExpr_in_logicalExpr1422);
                    	comparisonExpr76 = comparisonExpr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, comparisonExpr76.Tree);

                    }
                    break;
                case 5 :
                    // HqlSqlWalker.g:303:4: functionCall
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_functionCall_in_logicalExpr1427);
                    	functionCall77 = functionCall();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, functionCall77.Tree);

                    }
                    break;
                case 6 :
                    // HqlSqlWalker.g:304:4: logicalPath
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_logicalPath_in_logicalExpr1432);
                    	logicalPath78 = logicalPath();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, logicalPath78.Tree);

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

    public class logicalPath_return : TreeRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "logicalPath"
    // HqlSqlWalker.g:307:1: logicalPath : p= addrExpr[ true ] -> ^( EQ $p TRUE ) ;
    public HqlSqlWalker.logicalPath_return logicalPath() // throws RecognitionException [1]
    {   
        HqlSqlWalker.logicalPath_return retval = new HqlSqlWalker.logicalPath_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        HqlSqlWalker.addrExpr_return p = default(HqlSqlWalker.addrExpr_return);


        RewriteRuleSubtreeStream stream_addrExpr = new RewriteRuleSubtreeStream(adaptor,"rule addrExpr");
        try 
    	{
            // HqlSqlWalker.g:311:2: (p= addrExpr[ true ] -> ^( EQ $p TRUE ) )
            // HqlSqlWalker.g:311:4: p= addrExpr[ true ]
            {
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_addrExpr_in_logicalPath1451);
            	p = addrExpr(true);
            	state.followingStackPointer--;

            	stream_addrExpr.Add(p.Tree);
            	Resolve(((p != null) ? ((IASTNode)p.Tree) : null));


            	// AST REWRITE
            	// elements:          p
            	// token labels:      
            	// rule labels:       retval, p
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);
            	RewriteRuleSubtreeStream stream_p = new RewriteRuleSubtreeStream(adaptor, "rule p", p!=null ? p.Tree : null);

            	root_0 = (IASTNode)adaptor.GetNilNode();
            	// 311:45: -> ^( EQ $p TRUE )
            	{
            	    // HqlSqlWalker.g:311:48: ^( EQ $p TRUE )
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(EQ, "EQ"), root_1);

            	    adaptor.AddChild(root_1, stream_p.NextTree());
            	    adaptor.AddChild(root_1, (IASTNode)adaptor.Create(TRUE, "TRUE"));

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;
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
    // $ANTLR end "logicalPath"

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
    // HqlSqlWalker.g:315:1: comparisonExpr : ( ^( EQ exprOrSubquery exprOrSubquery ) | ^( NE exprOrSubquery exprOrSubquery ) | ^( LT exprOrSubquery exprOrSubquery ) | ^( GT exprOrSubquery exprOrSubquery ) | ^( LE exprOrSubquery exprOrSubquery ) | ^( GE exprOrSubquery exprOrSubquery ) | ^( LIKE exprOrSubquery expr ( ^( ESCAPE expr ) )? ) | ^( NOT_LIKE exprOrSubquery expr ( ^( ESCAPE expr ) )? ) | ^( BETWEEN exprOrSubquery exprOrSubquery exprOrSubquery ) | ^( NOT_BETWEEN exprOrSubquery exprOrSubquery exprOrSubquery ) | ^( IN exprOrSubquery inRhs ) | ^( NOT_IN exprOrSubquery inRhs ) | ^( IS_NULL exprOrSubquery ) | ^( IS_NOT_NULL exprOrSubquery ) | ^( EXISTS ( expr | collectionFunctionOrSubselect ) ) ) ;
    public HqlSqlWalker.comparisonExpr_return comparisonExpr() // throws RecognitionException [1]
    {   
        HqlSqlWalker.comparisonExpr_return retval = new HqlSqlWalker.comparisonExpr_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode EQ79 = null;
        IASTNode NE82 = null;
        IASTNode LT85 = null;
        IASTNode GT88 = null;
        IASTNode LE91 = null;
        IASTNode GE94 = null;
        IASTNode LIKE97 = null;
        IASTNode ESCAPE100 = null;
        IASTNode NOT_LIKE102 = null;
        IASTNode ESCAPE105 = null;
        IASTNode BETWEEN107 = null;
        IASTNode NOT_BETWEEN111 = null;
        IASTNode IN115 = null;
        IASTNode NOT_IN118 = null;
        IASTNode IS_NULL121 = null;
        IASTNode IS_NOT_NULL123 = null;
        IASTNode EXISTS125 = null;
        HqlSqlWalker.exprOrSubquery_return exprOrSubquery80 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery81 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery83 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery84 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery86 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery87 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery89 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery90 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery92 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery93 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery95 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery96 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery98 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.expr_return expr99 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.expr_return expr101 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery103 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.expr_return expr104 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.expr_return expr106 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery108 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery109 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery110 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery112 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery113 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery114 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery116 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.inRhs_return inRhs117 = default(HqlSqlWalker.inRhs_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery119 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.inRhs_return inRhs120 = default(HqlSqlWalker.inRhs_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery122 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery124 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.expr_return expr126 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.collectionFunctionOrSubselect_return collectionFunctionOrSubselect127 = default(HqlSqlWalker.collectionFunctionOrSubselect_return);


        IASTNode EQ79_tree=null;
        IASTNode NE82_tree=null;
        IASTNode LT85_tree=null;
        IASTNode GT88_tree=null;
        IASTNode LE91_tree=null;
        IASTNode GE94_tree=null;
        IASTNode LIKE97_tree=null;
        IASTNode ESCAPE100_tree=null;
        IASTNode NOT_LIKE102_tree=null;
        IASTNode ESCAPE105_tree=null;
        IASTNode BETWEEN107_tree=null;
        IASTNode NOT_BETWEEN111_tree=null;
        IASTNode IN115_tree=null;
        IASTNode NOT_IN118_tree=null;
        IASTNode IS_NULL121_tree=null;
        IASTNode IS_NOT_NULL123_tree=null;
        IASTNode EXISTS125_tree=null;

        try 
    	{
            // HqlSqlWalker.g:319:2: ( ( ^( EQ exprOrSubquery exprOrSubquery ) | ^( NE exprOrSubquery exprOrSubquery ) | ^( LT exprOrSubquery exprOrSubquery ) | ^( GT exprOrSubquery exprOrSubquery ) | ^( LE exprOrSubquery exprOrSubquery ) | ^( GE exprOrSubquery exprOrSubquery ) | ^( LIKE exprOrSubquery expr ( ^( ESCAPE expr ) )? ) | ^( NOT_LIKE exprOrSubquery expr ( ^( ESCAPE expr ) )? ) | ^( BETWEEN exprOrSubquery exprOrSubquery exprOrSubquery ) | ^( NOT_BETWEEN exprOrSubquery exprOrSubquery exprOrSubquery ) | ^( IN exprOrSubquery inRhs ) | ^( NOT_IN exprOrSubquery inRhs ) | ^( IS_NULL exprOrSubquery ) | ^( IS_NOT_NULL exprOrSubquery ) | ^( EXISTS ( expr | collectionFunctionOrSubselect ) ) ) )
            // HqlSqlWalker.g:320:2: ( ^( EQ exprOrSubquery exprOrSubquery ) | ^( NE exprOrSubquery exprOrSubquery ) | ^( LT exprOrSubquery exprOrSubquery ) | ^( GT exprOrSubquery exprOrSubquery ) | ^( LE exprOrSubquery exprOrSubquery ) | ^( GE exprOrSubquery exprOrSubquery ) | ^( LIKE exprOrSubquery expr ( ^( ESCAPE expr ) )? ) | ^( NOT_LIKE exprOrSubquery expr ( ^( ESCAPE expr ) )? ) | ^( BETWEEN exprOrSubquery exprOrSubquery exprOrSubquery ) | ^( NOT_BETWEEN exprOrSubquery exprOrSubquery exprOrSubquery ) | ^( IN exprOrSubquery inRhs ) | ^( NOT_IN exprOrSubquery inRhs ) | ^( IS_NULL exprOrSubquery ) | ^( IS_NOT_NULL exprOrSubquery ) | ^( EXISTS ( expr | collectionFunctionOrSubselect ) ) )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// HqlSqlWalker.g:320:2: ( ^( EQ exprOrSubquery exprOrSubquery ) | ^( NE exprOrSubquery exprOrSubquery ) | ^( LT exprOrSubquery exprOrSubquery ) | ^( GT exprOrSubquery exprOrSubquery ) | ^( LE exprOrSubquery exprOrSubquery ) | ^( GE exprOrSubquery exprOrSubquery ) | ^( LIKE exprOrSubquery expr ( ^( ESCAPE expr ) )? ) | ^( NOT_LIKE exprOrSubquery expr ( ^( ESCAPE expr ) )? ) | ^( BETWEEN exprOrSubquery exprOrSubquery exprOrSubquery ) | ^( NOT_BETWEEN exprOrSubquery exprOrSubquery exprOrSubquery ) | ^( IN exprOrSubquery inRhs ) | ^( NOT_IN exprOrSubquery inRhs ) | ^( IS_NULL exprOrSubquery ) | ^( IS_NOT_NULL exprOrSubquery ) | ^( EXISTS ( expr | collectionFunctionOrSubselect ) ) )
            	int alt42 = 15;
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
            	case LT:
            		{
            	    alt42 = 3;
            	    }
            	    break;
            	case GT:
            		{
            	    alt42 = 4;
            	    }
            	    break;
            	case LE:
            		{
            	    alt42 = 5;
            	    }
            	    break;
            	case GE:
            		{
            	    alt42 = 6;
            	    }
            	    break;
            	case LIKE:
            		{
            	    alt42 = 7;
            	    }
            	    break;
            	case NOT_LIKE:
            		{
            	    alt42 = 8;
            	    }
            	    break;
            	case BETWEEN:
            		{
            	    alt42 = 9;
            	    }
            	    break;
            	case NOT_BETWEEN:
            		{
            	    alt42 = 10;
            	    }
            	    break;
            	case IN:
            		{
            	    alt42 = 11;
            	    }
            	    break;
            	case NOT_IN:
            		{
            	    alt42 = 12;
            	    }
            	    break;
            	case IS_NULL:
            		{
            	    alt42 = 13;
            	    }
            	    break;
            	case IS_NOT_NULL:
            		{
            	    alt42 = 14;
            	    }
            	    break;
            	case EXISTS:
            		{
            	    alt42 = 15;
            	    }
            	    break;
            		default:
            		    NoViableAltException nvae_d42s0 =
            		        new NoViableAltException("", 42, 0, input);

            		    throw nvae_d42s0;
            	}

            	switch (alt42) 
            	{
            	    case 1 :
            	        // HqlSqlWalker.g:320:4: ^( EQ exprOrSubquery exprOrSubquery )
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	{
            	        	IASTNode _save_last_1 = _last;
            	        	IASTNode _first_1 = null;
            	        	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	EQ79=(IASTNode)Match(input,EQ,FOLLOW_EQ_in_comparisonExpr1489); 
            	        		EQ79_tree = (IASTNode)adaptor.DupNode(EQ79);

            	        		root_1 = (IASTNode)adaptor.BecomeRoot(EQ79_tree, root_1);



            	        	Match(input, Token.DOWN, null); 
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1491);
            	        	exprOrSubquery80 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery80.Tree);
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1493);
            	        	exprOrSubquery81 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery81.Tree);

            	        	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	        	}


            	        }
            	        break;
            	    case 2 :
            	        // HqlSqlWalker.g:321:4: ^( NE exprOrSubquery exprOrSubquery )
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	{
            	        	IASTNode _save_last_1 = _last;
            	        	IASTNode _first_1 = null;
            	        	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	NE82=(IASTNode)Match(input,NE,FOLLOW_NE_in_comparisonExpr1500); 
            	        		NE82_tree = (IASTNode)adaptor.DupNode(NE82);

            	        		root_1 = (IASTNode)adaptor.BecomeRoot(NE82_tree, root_1);



            	        	Match(input, Token.DOWN, null); 
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1502);
            	        	exprOrSubquery83 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery83.Tree);
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1504);
            	        	exprOrSubquery84 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery84.Tree);

            	        	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	        	}


            	        }
            	        break;
            	    case 3 :
            	        // HqlSqlWalker.g:322:4: ^( LT exprOrSubquery exprOrSubquery )
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	{
            	        	IASTNode _save_last_1 = _last;
            	        	IASTNode _first_1 = null;
            	        	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	LT85=(IASTNode)Match(input,LT,FOLLOW_LT_in_comparisonExpr1511); 
            	        		LT85_tree = (IASTNode)adaptor.DupNode(LT85);

            	        		root_1 = (IASTNode)adaptor.BecomeRoot(LT85_tree, root_1);



            	        	Match(input, Token.DOWN, null); 
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1513);
            	        	exprOrSubquery86 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery86.Tree);
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1515);
            	        	exprOrSubquery87 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery87.Tree);

            	        	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	        	}


            	        }
            	        break;
            	    case 4 :
            	        // HqlSqlWalker.g:323:4: ^( GT exprOrSubquery exprOrSubquery )
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	{
            	        	IASTNode _save_last_1 = _last;
            	        	IASTNode _first_1 = null;
            	        	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	GT88=(IASTNode)Match(input,GT,FOLLOW_GT_in_comparisonExpr1522); 
            	        		GT88_tree = (IASTNode)adaptor.DupNode(GT88);

            	        		root_1 = (IASTNode)adaptor.BecomeRoot(GT88_tree, root_1);



            	        	Match(input, Token.DOWN, null); 
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1524);
            	        	exprOrSubquery89 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery89.Tree);
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1526);
            	        	exprOrSubquery90 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery90.Tree);

            	        	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	        	}


            	        }
            	        break;
            	    case 5 :
            	        // HqlSqlWalker.g:324:4: ^( LE exprOrSubquery exprOrSubquery )
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	{
            	        	IASTNode _save_last_1 = _last;
            	        	IASTNode _first_1 = null;
            	        	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	LE91=(IASTNode)Match(input,LE,FOLLOW_LE_in_comparisonExpr1533); 
            	        		LE91_tree = (IASTNode)adaptor.DupNode(LE91);

            	        		root_1 = (IASTNode)adaptor.BecomeRoot(LE91_tree, root_1);



            	        	Match(input, Token.DOWN, null); 
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1535);
            	        	exprOrSubquery92 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery92.Tree);
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1537);
            	        	exprOrSubquery93 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery93.Tree);

            	        	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	        	}


            	        }
            	        break;
            	    case 6 :
            	        // HqlSqlWalker.g:325:4: ^( GE exprOrSubquery exprOrSubquery )
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	{
            	        	IASTNode _save_last_1 = _last;
            	        	IASTNode _first_1 = null;
            	        	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	GE94=(IASTNode)Match(input,GE,FOLLOW_GE_in_comparisonExpr1544); 
            	        		GE94_tree = (IASTNode)adaptor.DupNode(GE94);

            	        		root_1 = (IASTNode)adaptor.BecomeRoot(GE94_tree, root_1);



            	        	Match(input, Token.DOWN, null); 
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1546);
            	        	exprOrSubquery95 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery95.Tree);
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1548);
            	        	exprOrSubquery96 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery96.Tree);

            	        	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	        	}


            	        }
            	        break;
            	    case 7 :
            	        // HqlSqlWalker.g:326:4: ^( LIKE exprOrSubquery expr ( ^( ESCAPE expr ) )? )
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	{
            	        	IASTNode _save_last_1 = _last;
            	        	IASTNode _first_1 = null;
            	        	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	LIKE97=(IASTNode)Match(input,LIKE,FOLLOW_LIKE_in_comparisonExpr1555); 
            	        		LIKE97_tree = (IASTNode)adaptor.DupNode(LIKE97);

            	        		root_1 = (IASTNode)adaptor.BecomeRoot(LIKE97_tree, root_1);



            	        	Match(input, Token.DOWN, null); 
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1557);
            	        	exprOrSubquery98 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery98.Tree);
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_expr_in_comparisonExpr1559);
            	        	expr99 = expr();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, expr99.Tree);
            	        	// HqlSqlWalker.g:326:31: ( ^( ESCAPE expr ) )?
            	        	int alt39 = 2;
            	        	int LA39_0 = input.LA(1);

            	        	if ( (LA39_0 == ESCAPE) )
            	        	{
            	        	    alt39 = 1;
            	        	}
            	        	switch (alt39) 
            	        	{
            	        	    case 1 :
            	        	        // HqlSqlWalker.g:326:33: ^( ESCAPE expr )
            	        	        {
            	        	        	_last = (IASTNode)input.LT(1);
            	        	        	{
            	        	        	IASTNode _save_last_2 = _last;
            	        	        	IASTNode _first_2 = null;
            	        	        	IASTNode root_2 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	        	ESCAPE100=(IASTNode)Match(input,ESCAPE,FOLLOW_ESCAPE_in_comparisonExpr1564); 
            	        	        		ESCAPE100_tree = (IASTNode)adaptor.DupNode(ESCAPE100);

            	        	        		root_2 = (IASTNode)adaptor.BecomeRoot(ESCAPE100_tree, root_2);



            	        	        	Match(input, Token.DOWN, null); 
            	        	        	_last = (IASTNode)input.LT(1);
            	        	        	PushFollow(FOLLOW_expr_in_comparisonExpr1566);
            	        	        	expr101 = expr();
            	        	        	state.followingStackPointer--;

            	        	        	adaptor.AddChild(root_2, expr101.Tree);

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
            	        // HqlSqlWalker.g:327:4: ^( NOT_LIKE exprOrSubquery expr ( ^( ESCAPE expr ) )? )
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	{
            	        	IASTNode _save_last_1 = _last;
            	        	IASTNode _first_1 = null;
            	        	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	NOT_LIKE102=(IASTNode)Match(input,NOT_LIKE,FOLLOW_NOT_LIKE_in_comparisonExpr1578); 
            	        		NOT_LIKE102_tree = (IASTNode)adaptor.DupNode(NOT_LIKE102);

            	        		root_1 = (IASTNode)adaptor.BecomeRoot(NOT_LIKE102_tree, root_1);



            	        	Match(input, Token.DOWN, null); 
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1580);
            	        	exprOrSubquery103 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery103.Tree);
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_expr_in_comparisonExpr1582);
            	        	expr104 = expr();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, expr104.Tree);
            	        	// HqlSqlWalker.g:327:35: ( ^( ESCAPE expr ) )?
            	        	int alt40 = 2;
            	        	int LA40_0 = input.LA(1);

            	        	if ( (LA40_0 == ESCAPE) )
            	        	{
            	        	    alt40 = 1;
            	        	}
            	        	switch (alt40) 
            	        	{
            	        	    case 1 :
            	        	        // HqlSqlWalker.g:327:37: ^( ESCAPE expr )
            	        	        {
            	        	        	_last = (IASTNode)input.LT(1);
            	        	        	{
            	        	        	IASTNode _save_last_2 = _last;
            	        	        	IASTNode _first_2 = null;
            	        	        	IASTNode root_2 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	        	ESCAPE105=(IASTNode)Match(input,ESCAPE,FOLLOW_ESCAPE_in_comparisonExpr1587); 
            	        	        		ESCAPE105_tree = (IASTNode)adaptor.DupNode(ESCAPE105);

            	        	        		root_2 = (IASTNode)adaptor.BecomeRoot(ESCAPE105_tree, root_2);



            	        	        	Match(input, Token.DOWN, null); 
            	        	        	_last = (IASTNode)input.LT(1);
            	        	        	PushFollow(FOLLOW_expr_in_comparisonExpr1589);
            	        	        	expr106 = expr();
            	        	        	state.followingStackPointer--;

            	        	        	adaptor.AddChild(root_2, expr106.Tree);

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
            	        // HqlSqlWalker.g:328:4: ^( BETWEEN exprOrSubquery exprOrSubquery exprOrSubquery )
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	{
            	        	IASTNode _save_last_1 = _last;
            	        	IASTNode _first_1 = null;
            	        	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	BETWEEN107=(IASTNode)Match(input,BETWEEN,FOLLOW_BETWEEN_in_comparisonExpr1601); 
            	        		BETWEEN107_tree = (IASTNode)adaptor.DupNode(BETWEEN107);

            	        		root_1 = (IASTNode)adaptor.BecomeRoot(BETWEEN107_tree, root_1);



            	        	Match(input, Token.DOWN, null); 
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1603);
            	        	exprOrSubquery108 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery108.Tree);
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1605);
            	        	exprOrSubquery109 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery109.Tree);
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1607);
            	        	exprOrSubquery110 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery110.Tree);

            	        	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	        	}


            	        }
            	        break;
            	    case 10 :
            	        // HqlSqlWalker.g:329:4: ^( NOT_BETWEEN exprOrSubquery exprOrSubquery exprOrSubquery )
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	{
            	        	IASTNode _save_last_1 = _last;
            	        	IASTNode _first_1 = null;
            	        	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	NOT_BETWEEN111=(IASTNode)Match(input,NOT_BETWEEN,FOLLOW_NOT_BETWEEN_in_comparisonExpr1614); 
            	        		NOT_BETWEEN111_tree = (IASTNode)adaptor.DupNode(NOT_BETWEEN111);

            	        		root_1 = (IASTNode)adaptor.BecomeRoot(NOT_BETWEEN111_tree, root_1);



            	        	Match(input, Token.DOWN, null); 
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1616);
            	        	exprOrSubquery112 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery112.Tree);
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1618);
            	        	exprOrSubquery113 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery113.Tree);
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1620);
            	        	exprOrSubquery114 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery114.Tree);

            	        	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	        	}


            	        }
            	        break;
            	    case 11 :
            	        // HqlSqlWalker.g:330:4: ^( IN exprOrSubquery inRhs )
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	{
            	        	IASTNode _save_last_1 = _last;
            	        	IASTNode _first_1 = null;
            	        	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	IN115=(IASTNode)Match(input,IN,FOLLOW_IN_in_comparisonExpr1627); 
            	        		IN115_tree = (IASTNode)adaptor.DupNode(IN115);

            	        		root_1 = (IASTNode)adaptor.BecomeRoot(IN115_tree, root_1);



            	        	Match(input, Token.DOWN, null); 
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1629);
            	        	exprOrSubquery116 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery116.Tree);
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_inRhs_in_comparisonExpr1631);
            	        	inRhs117 = inRhs();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, inRhs117.Tree);

            	        	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	        	}


            	        }
            	        break;
            	    case 12 :
            	        // HqlSqlWalker.g:331:4: ^( NOT_IN exprOrSubquery inRhs )
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	{
            	        	IASTNode _save_last_1 = _last;
            	        	IASTNode _first_1 = null;
            	        	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	NOT_IN118=(IASTNode)Match(input,NOT_IN,FOLLOW_NOT_IN_in_comparisonExpr1639); 
            	        		NOT_IN118_tree = (IASTNode)adaptor.DupNode(NOT_IN118);

            	        		root_1 = (IASTNode)adaptor.BecomeRoot(NOT_IN118_tree, root_1);



            	        	Match(input, Token.DOWN, null); 
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1641);
            	        	exprOrSubquery119 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery119.Tree);
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_inRhs_in_comparisonExpr1643);
            	        	inRhs120 = inRhs();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, inRhs120.Tree);

            	        	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	        	}


            	        }
            	        break;
            	    case 13 :
            	        // HqlSqlWalker.g:332:4: ^( IS_NULL exprOrSubquery )
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	{
            	        	IASTNode _save_last_1 = _last;
            	        	IASTNode _first_1 = null;
            	        	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	IS_NULL121=(IASTNode)Match(input,IS_NULL,FOLLOW_IS_NULL_in_comparisonExpr1651); 
            	        		IS_NULL121_tree = (IASTNode)adaptor.DupNode(IS_NULL121);

            	        		root_1 = (IASTNode)adaptor.BecomeRoot(IS_NULL121_tree, root_1);



            	        	Match(input, Token.DOWN, null); 
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1653);
            	        	exprOrSubquery122 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery122.Tree);

            	        	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	        	}


            	        }
            	        break;
            	    case 14 :
            	        // HqlSqlWalker.g:333:4: ^( IS_NOT_NULL exprOrSubquery )
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	{
            	        	IASTNode _save_last_1 = _last;
            	        	IASTNode _first_1 = null;
            	        	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	IS_NOT_NULL123=(IASTNode)Match(input,IS_NOT_NULL,FOLLOW_IS_NOT_NULL_in_comparisonExpr1660); 
            	        		IS_NOT_NULL123_tree = (IASTNode)adaptor.DupNode(IS_NOT_NULL123);

            	        		root_1 = (IASTNode)adaptor.BecomeRoot(IS_NOT_NULL123_tree, root_1);



            	        	Match(input, Token.DOWN, null); 
            	        	_last = (IASTNode)input.LT(1);
            	        	PushFollow(FOLLOW_exprOrSubquery_in_comparisonExpr1662);
            	        	exprOrSubquery124 = exprOrSubquery();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_1, exprOrSubquery124.Tree);

            	        	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	        	}


            	        }
            	        break;
            	    case 15 :
            	        // HqlSqlWalker.g:336:4: ^( EXISTS ( expr | collectionFunctionOrSubselect ) )
            	        {
            	        	_last = (IASTNode)input.LT(1);
            	        	{
            	        	IASTNode _save_last_1 = _last;
            	        	IASTNode _first_1 = null;
            	        	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	        	EXISTS125=(IASTNode)Match(input,EXISTS,FOLLOW_EXISTS_in_comparisonExpr1671); 
            	        		EXISTS125_tree = (IASTNode)adaptor.DupNode(EXISTS125);

            	        		root_1 = (IASTNode)adaptor.BecomeRoot(EXISTS125_tree, root_1);



            	        	Match(input, Token.DOWN, null); 
            	        	// HqlSqlWalker.g:336:13: ( expr | collectionFunctionOrSubselect )
            	        	int alt41 = 2;
            	        	int LA41_0 = input.LA(1);

            	        	if ( (LA41_0 == COUNT || LA41_0 == DOT || LA41_0 == FALSE || LA41_0 == NULL || LA41_0 == TRUE || LA41_0 == CASE || LA41_0 == AGGREGATE || LA41_0 == CASE2 || LA41_0 == INDEX_OP || LA41_0 == METHOD_CALL || LA41_0 == UNARY_MINUS || (LA41_0 >= VECTOR_EXPR && LA41_0 <= WEIRD_IDENT) || (LA41_0 >= NUM_INT && LA41_0 <= JAVA_CONSTANT) || (LA41_0 >= BNOT && LA41_0 <= DIV) || (LA41_0 >= COLON && LA41_0 <= IDENT)) )
            	        	{
            	        	    alt41 = 1;
            	        	}
            	        	else if ( (LA41_0 == ELEMENTS || LA41_0 == INDICES || LA41_0 == UNION || LA41_0 == QUERY) )
            	        	{
            	        	    alt41 = 2;
            	        	}
            	        	else 
            	        	{
            	        	    NoViableAltException nvae_d41s0 =
            	        	        new NoViableAltException("", 41, 0, input);

            	        	    throw nvae_d41s0;
            	        	}
            	        	switch (alt41) 
            	        	{
            	        	    case 1 :
            	        	        // HqlSqlWalker.g:336:15: expr
            	        	        {
            	        	        	_last = (IASTNode)input.LT(1);
            	        	        	PushFollow(FOLLOW_expr_in_comparisonExpr1675);
            	        	        	expr126 = expr();
            	        	        	state.followingStackPointer--;

            	        	        	adaptor.AddChild(root_1, expr126.Tree);

            	        	        }
            	        	        break;
            	        	    case 2 :
            	        	        // HqlSqlWalker.g:336:22: collectionFunctionOrSubselect
            	        	        {
            	        	        	_last = (IASTNode)input.LT(1);
            	        	        	PushFollow(FOLLOW_collectionFunctionOrSubselect_in_comparisonExpr1679);
            	        	        	collectionFunctionOrSubselect127 = collectionFunctionOrSubselect();
            	        	        	state.followingStackPointer--;

            	        	        	adaptor.AddChild(root_1, collectionFunctionOrSubselect127.Tree);

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
    // HqlSqlWalker.g:340:1: inRhs : ^( IN_LIST ( collectionFunctionOrSubselect | ( expr )* ) ) ;
    public HqlSqlWalker.inRhs_return inRhs() // throws RecognitionException [1]
    {   
        HqlSqlWalker.inRhs_return retval = new HqlSqlWalker.inRhs_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode IN_LIST128 = null;
        HqlSqlWalker.collectionFunctionOrSubselect_return collectionFunctionOrSubselect129 = default(HqlSqlWalker.collectionFunctionOrSubselect_return);

        HqlSqlWalker.expr_return expr130 = default(HqlSqlWalker.expr_return);


        IASTNode IN_LIST128_tree=null;

        	int UP = 99999;		// TODO - added this to get compile working.  It's bogus & should be removed
        	
        try 
    	{
            // HqlSqlWalker.g:342:2: ( ^( IN_LIST ( collectionFunctionOrSubselect | ( expr )* ) ) )
            // HqlSqlWalker.g:342:4: ^( IN_LIST ( collectionFunctionOrSubselect | ( expr )* ) )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	IN_LIST128=(IASTNode)Match(input,IN_LIST,FOLLOW_IN_LIST_in_inRhs1703); 
            		IN_LIST128_tree = (IASTNode)adaptor.DupNode(IN_LIST128);

            		root_1 = (IASTNode)adaptor.BecomeRoot(IN_LIST128_tree, root_1);



            	if ( input.LA(1) == Token.DOWN )
            	{
            	    Match(input, Token.DOWN, null); 
            	    // HqlSqlWalker.g:342:14: ( collectionFunctionOrSubselect | ( expr )* )
            	    int alt44 = 2;
            	    int LA44_0 = input.LA(1);

            	    if ( (LA44_0 == ELEMENTS || LA44_0 == INDICES || LA44_0 == UNION || LA44_0 == QUERY) )
            	    {
            	        alt44 = 1;
            	    }
            	    else if ( (LA44_0 == UP || LA44_0 == COUNT || LA44_0 == DOT || LA44_0 == FALSE || LA44_0 == NULL || LA44_0 == TRUE || LA44_0 == CASE || LA44_0 == AGGREGATE || LA44_0 == CASE2 || LA44_0 == INDEX_OP || LA44_0 == METHOD_CALL || LA44_0 == UNARY_MINUS || (LA44_0 >= VECTOR_EXPR && LA44_0 <= WEIRD_IDENT) || (LA44_0 >= NUM_INT && LA44_0 <= JAVA_CONSTANT) || (LA44_0 >= BNOT && LA44_0 <= DIV) || (LA44_0 >= COLON && LA44_0 <= IDENT)) )
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
            	            // HqlSqlWalker.g:342:16: collectionFunctionOrSubselect
            	            {
            	            	_last = (IASTNode)input.LT(1);
            	            	PushFollow(FOLLOW_collectionFunctionOrSubselect_in_inRhs1707);
            	            	collectionFunctionOrSubselect129 = collectionFunctionOrSubselect();
            	            	state.followingStackPointer--;

            	            	adaptor.AddChild(root_1, collectionFunctionOrSubselect129.Tree);

            	            }
            	            break;
            	        case 2 :
            	            // HqlSqlWalker.g:342:48: ( expr )*
            	            {
            	            	// HqlSqlWalker.g:342:48: ( expr )*
            	            	do 
            	            	{
            	            	    int alt43 = 2;
            	            	    int LA43_0 = input.LA(1);

            	            	    if ( (LA43_0 == COUNT || LA43_0 == DOT || LA43_0 == FALSE || LA43_0 == NULL || LA43_0 == TRUE || LA43_0 == CASE || LA43_0 == AGGREGATE || LA43_0 == CASE2 || LA43_0 == INDEX_OP || LA43_0 == METHOD_CALL || LA43_0 == UNARY_MINUS || (LA43_0 >= VECTOR_EXPR && LA43_0 <= WEIRD_IDENT) || (LA43_0 >= NUM_INT && LA43_0 <= JAVA_CONSTANT) || (LA43_0 >= BNOT && LA43_0 <= DIV) || (LA43_0 >= COLON && LA43_0 <= IDENT)) )
            	            	    {
            	            	        alt43 = 1;
            	            	    }


            	            	    switch (alt43) 
            	            		{
            	            			case 1 :
            	            			    // HqlSqlWalker.g:342:48: expr
            	            			    {
            	            			    	_last = (IASTNode)input.LT(1);
            	            			    	PushFollow(FOLLOW_expr_in_inRhs1711);
            	            			    	expr130 = expr();
            	            			    	state.followingStackPointer--;

            	            			    	adaptor.AddChild(root_1, expr130.Tree);

            	            			    }
            	            			    break;

            	            			default:
            	            			    goto loop43;
            	            	    }
            	            	} while (true);

            	            	loop43:
            	            		;	// Stops C# compiler whining that label 'loop43' has no statements


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
    // HqlSqlWalker.g:345:1: exprOrSubquery : ( expr | query | ^( ANY collectionFunctionOrSubselect ) | ^( ALL collectionFunctionOrSubselect ) | ^( SOME collectionFunctionOrSubselect ) );
    public HqlSqlWalker.exprOrSubquery_return exprOrSubquery() // throws RecognitionException [1]
    {   
        HqlSqlWalker.exprOrSubquery_return retval = new HqlSqlWalker.exprOrSubquery_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode ANY133 = null;
        IASTNode ALL135 = null;
        IASTNode SOME137 = null;
        HqlSqlWalker.expr_return expr131 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.query_return query132 = default(HqlSqlWalker.query_return);

        HqlSqlWalker.collectionFunctionOrSubselect_return collectionFunctionOrSubselect134 = default(HqlSqlWalker.collectionFunctionOrSubselect_return);

        HqlSqlWalker.collectionFunctionOrSubselect_return collectionFunctionOrSubselect136 = default(HqlSqlWalker.collectionFunctionOrSubselect_return);

        HqlSqlWalker.collectionFunctionOrSubselect_return collectionFunctionOrSubselect138 = default(HqlSqlWalker.collectionFunctionOrSubselect_return);


        IASTNode ANY133_tree=null;
        IASTNode ALL135_tree=null;
        IASTNode SOME137_tree=null;

        try 
    	{
            // HqlSqlWalker.g:346:2: ( expr | query | ^( ANY collectionFunctionOrSubselect ) | ^( ALL collectionFunctionOrSubselect ) | ^( SOME collectionFunctionOrSubselect ) )
            int alt45 = 5;
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
            case NUM_DECIMAL:
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
            case COLON:
            case PARAM:
            case QUOTED_String:
            case IDENT:
            	{
                alt45 = 1;
                }
                break;
            case UNION:
            case QUERY:
            	{
                alt45 = 2;
                }
                break;
            case ANY:
            	{
                alt45 = 3;
                }
                break;
            case ALL:
            	{
                alt45 = 4;
                }
                break;
            case SOME:
            	{
                alt45 = 5;
                }
                break;
            	default:
            	    NoViableAltException nvae_d45s0 =
            	        new NoViableAltException("", 45, 0, input);

            	    throw nvae_d45s0;
            }

            switch (alt45) 
            {
                case 1 :
                    // HqlSqlWalker.g:346:4: expr
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_expr_in_exprOrSubquery1727);
                    	expr131 = expr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, expr131.Tree);

                    }
                    break;
                case 2 :
                    // HqlSqlWalker.g:347:4: query
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_query_in_exprOrSubquery1732);
                    	query132 = query();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, query132.Tree);

                    }
                    break;
                case 3 :
                    // HqlSqlWalker.g:348:4: ^( ANY collectionFunctionOrSubselect )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	ANY133=(IASTNode)Match(input,ANY,FOLLOW_ANY_in_exprOrSubquery1738); 
                    		ANY133_tree = (IASTNode)adaptor.DupNode(ANY133);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(ANY133_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_collectionFunctionOrSubselect_in_exprOrSubquery1740);
                    	collectionFunctionOrSubselect134 = collectionFunctionOrSubselect();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, collectionFunctionOrSubselect134.Tree);

                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}


                    }
                    break;
                case 4 :
                    // HqlSqlWalker.g:349:4: ^( ALL collectionFunctionOrSubselect )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	ALL135=(IASTNode)Match(input,ALL,FOLLOW_ALL_in_exprOrSubquery1747); 
                    		ALL135_tree = (IASTNode)adaptor.DupNode(ALL135);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(ALL135_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_collectionFunctionOrSubselect_in_exprOrSubquery1749);
                    	collectionFunctionOrSubselect136 = collectionFunctionOrSubselect();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, collectionFunctionOrSubselect136.Tree);

                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}


                    }
                    break;
                case 5 :
                    // HqlSqlWalker.g:350:4: ^( SOME collectionFunctionOrSubselect )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	SOME137=(IASTNode)Match(input,SOME,FOLLOW_SOME_in_exprOrSubquery1756); 
                    		SOME137_tree = (IASTNode)adaptor.DupNode(SOME137);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(SOME137_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_collectionFunctionOrSubselect_in_exprOrSubquery1758);
                    	collectionFunctionOrSubselect138 = collectionFunctionOrSubselect();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, collectionFunctionOrSubselect138.Tree);

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
    // HqlSqlWalker.g:353:1: collectionFunctionOrSubselect : ( collectionFunction | query );
    public HqlSqlWalker.collectionFunctionOrSubselect_return collectionFunctionOrSubselect() // throws RecognitionException [1]
    {   
        HqlSqlWalker.collectionFunctionOrSubselect_return retval = new HqlSqlWalker.collectionFunctionOrSubselect_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        HqlSqlWalker.collectionFunction_return collectionFunction139 = default(HqlSqlWalker.collectionFunction_return);

        HqlSqlWalker.query_return query140 = default(HqlSqlWalker.query_return);



        try 
    	{
            // HqlSqlWalker.g:354:2: ( collectionFunction | query )
            int alt46 = 2;
            int LA46_0 = input.LA(1);

            if ( (LA46_0 == ELEMENTS || LA46_0 == INDICES) )
            {
                alt46 = 1;
            }
            else if ( (LA46_0 == UNION || LA46_0 == QUERY) )
            {
                alt46 = 2;
            }
            else 
            {
                NoViableAltException nvae_d46s0 =
                    new NoViableAltException("", 46, 0, input);

                throw nvae_d46s0;
            }
            switch (alt46) 
            {
                case 1 :
                    // HqlSqlWalker.g:354:4: collectionFunction
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_collectionFunction_in_collectionFunctionOrSubselect1771);
                    	collectionFunction139 = collectionFunction();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, collectionFunction139.Tree);

                    }
                    break;
                case 2 :
                    // HqlSqlWalker.g:355:4: query
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_query_in_collectionFunctionOrSubselect1776);
                    	query140 = query();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, query140.Tree);

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
    // HqlSqlWalker.g:358:1: expr : (ae= addrExpr[ true ] | ^( VECTOR_EXPR ( expr )* ) | constant | arithmeticExpr | functionCall | parameter | count );
    public HqlSqlWalker.expr_return expr() // throws RecognitionException [1]
    {   
        HqlSqlWalker.expr_return retval = new HqlSqlWalker.expr_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode VECTOR_EXPR141 = null;
        HqlSqlWalker.addrExpr_return ae = default(HqlSqlWalker.addrExpr_return);

        HqlSqlWalker.expr_return expr142 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.constant_return constant143 = default(HqlSqlWalker.constant_return);

        HqlSqlWalker.arithmeticExpr_return arithmeticExpr144 = default(HqlSqlWalker.arithmeticExpr_return);

        HqlSqlWalker.functionCall_return functionCall145 = default(HqlSqlWalker.functionCall_return);

        HqlSqlWalker.parameter_return parameter146 = default(HqlSqlWalker.parameter_return);

        HqlSqlWalker.count_return count147 = default(HqlSqlWalker.count_return);


        IASTNode VECTOR_EXPR141_tree=null;

        try 
    	{
            // HqlSqlWalker.g:359:2: (ae= addrExpr[ true ] | ^( VECTOR_EXPR ( expr )* ) | constant | arithmeticExpr | functionCall | parameter | count )
            int alt48 = 7;
            switch ( input.LA(1) ) 
            {
            case DOT:
            case INDEX_OP:
            case WEIRD_IDENT:
            case IDENT:
            	{
                alt48 = 1;
                }
                break;
            case VECTOR_EXPR:
            	{
                alt48 = 2;
                }
                break;
            case FALSE:
            case NULL:
            case TRUE:
            case NUM_INT:
            case NUM_DOUBLE:
            case NUM_DECIMAL:
            case NUM_FLOAT:
            case NUM_LONG:
            case JAVA_CONSTANT:
            case QUOTED_String:
            	{
                alt48 = 3;
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
                alt48 = 4;
                }
                break;
            case AGGREGATE:
            case METHOD_CALL:
            	{
                alt48 = 5;
                }
                break;
            case COLON:
            case PARAM:
            	{
                alt48 = 6;
                }
                break;
            case COUNT:
            	{
                alt48 = 7;
                }
                break;
            	default:
            	    NoViableAltException nvae_d48s0 =
            	        new NoViableAltException("", 48, 0, input);

            	    throw nvae_d48s0;
            }

            switch (alt48) 
            {
                case 1 :
                    // HqlSqlWalker.g:359:4: ae= addrExpr[ true ]
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_addrExpr_in_expr1790);
                    	ae = addrExpr(true);
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, ae.Tree);
                    	 Resolve(((ae != null) ? ((IASTNode)ae.Tree) : null)); 

                    }
                    break;
                case 2 :
                    // HqlSqlWalker.g:360:4: ^( VECTOR_EXPR ( expr )* )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	VECTOR_EXPR141=(IASTNode)Match(input,VECTOR_EXPR,FOLLOW_VECTOR_EXPR_in_expr1802); 
                    		VECTOR_EXPR141_tree = (IASTNode)adaptor.DupNode(VECTOR_EXPR141);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(VECTOR_EXPR141_tree, root_1);



                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); 
                    	    // HqlSqlWalker.g:360:19: ( expr )*
                    	    do 
                    	    {
                    	        int alt47 = 2;
                    	        int LA47_0 = input.LA(1);

                    	        if ( (LA47_0 == COUNT || LA47_0 == DOT || LA47_0 == FALSE || LA47_0 == NULL || LA47_0 == TRUE || LA47_0 == CASE || LA47_0 == AGGREGATE || LA47_0 == CASE2 || LA47_0 == INDEX_OP || LA47_0 == METHOD_CALL || LA47_0 == UNARY_MINUS || (LA47_0 >= VECTOR_EXPR && LA47_0 <= WEIRD_IDENT) || (LA47_0 >= NUM_INT && LA47_0 <= JAVA_CONSTANT) || (LA47_0 >= BNOT && LA47_0 <= DIV) || (LA47_0 >= COLON && LA47_0 <= IDENT)) )
                    	        {
                    	            alt47 = 1;
                    	        }


                    	        switch (alt47) 
                    	    	{
                    	    		case 1 :
                    	    		    // HqlSqlWalker.g:360:20: expr
                    	    		    {
                    	    		    	_last = (IASTNode)input.LT(1);
                    	    		    	PushFollow(FOLLOW_expr_in_expr1805);
                    	    		    	expr142 = expr();
                    	    		    	state.followingStackPointer--;

                    	    		    	adaptor.AddChild(root_1, expr142.Tree);

                    	    		    }
                    	    		    break;

                    	    		default:
                    	    		    goto loop47;
                    	        }
                    	    } while (true);

                    	    loop47:
                    	    	;	// Stops C# compiler whining that label 'loop47' has no statements


                    	    Match(input, Token.UP, null); 
                    	}adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}


                    }
                    break;
                case 3 :
                    // HqlSqlWalker.g:361:4: constant
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_constant_in_expr1814);
                    	constant143 = constant();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, constant143.Tree);

                    }
                    break;
                case 4 :
                    // HqlSqlWalker.g:362:4: arithmeticExpr
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_arithmeticExpr_in_expr1819);
                    	arithmeticExpr144 = arithmeticExpr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, arithmeticExpr144.Tree);

                    }
                    break;
                case 5 :
                    // HqlSqlWalker.g:363:4: functionCall
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_functionCall_in_expr1824);
                    	functionCall145 = functionCall();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, functionCall145.Tree);

                    }
                    break;
                case 6 :
                    // HqlSqlWalker.g:364:4: parameter
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_parameter_in_expr1836);
                    	parameter146 = parameter();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, parameter146.Tree);

                    }
                    break;
                case 7 :
                    // HqlSqlWalker.g:365:4: count
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_count_in_expr1841);
                    	count147 = count();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, count147.Tree);

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
    // HqlSqlWalker.g:368:1: arithmeticExpr : ( ^( PLUS exprOrSubquery exprOrSubquery ) | ^( MINUS exprOrSubquery exprOrSubquery ) | ^( DIV exprOrSubquery exprOrSubquery ) | ^( STAR exprOrSubquery exprOrSubquery ) | ^( BNOT exprOrSubquery ) | ^( BAND exprOrSubquery exprOrSubquery ) | ^( BOR exprOrSubquery exprOrSubquery ) | ^( BXOR exprOrSubquery exprOrSubquery ) | ^( UNARY_MINUS exprOrSubquery ) | c= caseExpr );
    public HqlSqlWalker.arithmeticExpr_return arithmeticExpr() // throws RecognitionException [1]
    {   
        HqlSqlWalker.arithmeticExpr_return retval = new HqlSqlWalker.arithmeticExpr_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode PLUS148 = null;
        IASTNode MINUS151 = null;
        IASTNode DIV154 = null;
        IASTNode STAR157 = null;
        IASTNode BNOT160 = null;
        IASTNode BAND162 = null;
        IASTNode BOR165 = null;
        IASTNode BXOR168 = null;
        IASTNode UNARY_MINUS171 = null;
        HqlSqlWalker.caseExpr_return c = default(HqlSqlWalker.caseExpr_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery149 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery150 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery152 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery153 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery155 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery156 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery158 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery159 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery161 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery163 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery164 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery166 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery167 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery169 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery170 = default(HqlSqlWalker.exprOrSubquery_return);

        HqlSqlWalker.exprOrSubquery_return exprOrSubquery172 = default(HqlSqlWalker.exprOrSubquery_return);


        IASTNode PLUS148_tree=null;
        IASTNode MINUS151_tree=null;
        IASTNode DIV154_tree=null;
        IASTNode STAR157_tree=null;
        IASTNode BNOT160_tree=null;
        IASTNode BAND162_tree=null;
        IASTNode BOR165_tree=null;
        IASTNode BXOR168_tree=null;
        IASTNode UNARY_MINUS171_tree=null;

        try 
    	{
            // HqlSqlWalker.g:375:2: ( ^( PLUS exprOrSubquery exprOrSubquery ) | ^( MINUS exprOrSubquery exprOrSubquery ) | ^( DIV exprOrSubquery exprOrSubquery ) | ^( STAR exprOrSubquery exprOrSubquery ) | ^( BNOT exprOrSubquery ) | ^( BAND exprOrSubquery exprOrSubquery ) | ^( BOR exprOrSubquery exprOrSubquery ) | ^( BXOR exprOrSubquery exprOrSubquery ) | ^( UNARY_MINUS exprOrSubquery ) | c= caseExpr )
            int alt49 = 10;
            switch ( input.LA(1) ) 
            {
            case PLUS:
            	{
                alt49 = 1;
                }
                break;
            case MINUS:
            	{
                alt49 = 2;
                }
                break;
            case DIV:
            	{
                alt49 = 3;
                }
                break;
            case STAR:
            	{
                alt49 = 4;
                }
                break;
            case BNOT:
            	{
                alt49 = 5;
                }
                break;
            case BAND:
            	{
                alt49 = 6;
                }
                break;
            case BOR:
            	{
                alt49 = 7;
                }
                break;
            case BXOR:
            	{
                alt49 = 8;
                }
                break;
            case UNARY_MINUS:
            	{
                alt49 = 9;
                }
                break;
            case CASE:
            case CASE2:
            	{
                alt49 = 10;
                }
                break;
            	default:
            	    NoViableAltException nvae_d49s0 =
            	        new NoViableAltException("", 49, 0, input);

            	    throw nvae_d49s0;
            }

            switch (alt49) 
            {
                case 1 :
                    // HqlSqlWalker.g:375:4: ^( PLUS exprOrSubquery exprOrSubquery )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	PLUS148=(IASTNode)Match(input,PLUS,FOLLOW_PLUS_in_arithmeticExpr1869); 
                    		PLUS148_tree = (IASTNode)adaptor.DupNode(PLUS148);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(PLUS148_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_exprOrSubquery_in_arithmeticExpr1871);
                    	exprOrSubquery149 = exprOrSubquery();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, exprOrSubquery149.Tree);
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_exprOrSubquery_in_arithmeticExpr1873);
                    	exprOrSubquery150 = exprOrSubquery();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, exprOrSubquery150.Tree);

                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}


                    }
                    break;
                case 2 :
                    // HqlSqlWalker.g:376:4: ^( MINUS exprOrSubquery exprOrSubquery )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	MINUS151=(IASTNode)Match(input,MINUS,FOLLOW_MINUS_in_arithmeticExpr1880); 
                    		MINUS151_tree = (IASTNode)adaptor.DupNode(MINUS151);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(MINUS151_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_exprOrSubquery_in_arithmeticExpr1882);
                    	exprOrSubquery152 = exprOrSubquery();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, exprOrSubquery152.Tree);
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_exprOrSubquery_in_arithmeticExpr1884);
                    	exprOrSubquery153 = exprOrSubquery();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, exprOrSubquery153.Tree);

                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}


                    }
                    break;
                case 3 :
                    // HqlSqlWalker.g:377:4: ^( DIV exprOrSubquery exprOrSubquery )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	DIV154=(IASTNode)Match(input,DIV,FOLLOW_DIV_in_arithmeticExpr1891); 
                    		DIV154_tree = (IASTNode)adaptor.DupNode(DIV154);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(DIV154_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_exprOrSubquery_in_arithmeticExpr1893);
                    	exprOrSubquery155 = exprOrSubquery();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, exprOrSubquery155.Tree);
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_exprOrSubquery_in_arithmeticExpr1895);
                    	exprOrSubquery156 = exprOrSubquery();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, exprOrSubquery156.Tree);

                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}


                    }
                    break;
                case 4 :
                    // HqlSqlWalker.g:378:4: ^( STAR exprOrSubquery exprOrSubquery )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	STAR157=(IASTNode)Match(input,STAR,FOLLOW_STAR_in_arithmeticExpr1902); 
                    		STAR157_tree = (IASTNode)adaptor.DupNode(STAR157);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(STAR157_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_exprOrSubquery_in_arithmeticExpr1904);
                    	exprOrSubquery158 = exprOrSubquery();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, exprOrSubquery158.Tree);
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_exprOrSubquery_in_arithmeticExpr1906);
                    	exprOrSubquery159 = exprOrSubquery();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, exprOrSubquery159.Tree);

                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}


                    }
                    break;
                case 5 :
                    // HqlSqlWalker.g:379:4: ^( BNOT exprOrSubquery )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	BNOT160=(IASTNode)Match(input,BNOT,FOLLOW_BNOT_in_arithmeticExpr1913); 
                    		BNOT160_tree = (IASTNode)adaptor.DupNode(BNOT160);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(BNOT160_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_exprOrSubquery_in_arithmeticExpr1915);
                    	exprOrSubquery161 = exprOrSubquery();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, exprOrSubquery161.Tree);

                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}


                    }
                    break;
                case 6 :
                    // HqlSqlWalker.g:380:4: ^( BAND exprOrSubquery exprOrSubquery )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	BAND162=(IASTNode)Match(input,BAND,FOLLOW_BAND_in_arithmeticExpr1922); 
                    		BAND162_tree = (IASTNode)adaptor.DupNode(BAND162);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(BAND162_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_exprOrSubquery_in_arithmeticExpr1924);
                    	exprOrSubquery163 = exprOrSubquery();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, exprOrSubquery163.Tree);
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_exprOrSubquery_in_arithmeticExpr1926);
                    	exprOrSubquery164 = exprOrSubquery();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, exprOrSubquery164.Tree);

                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}


                    }
                    break;
                case 7 :
                    // HqlSqlWalker.g:381:4: ^( BOR exprOrSubquery exprOrSubquery )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	BOR165=(IASTNode)Match(input,BOR,FOLLOW_BOR_in_arithmeticExpr1933); 
                    		BOR165_tree = (IASTNode)adaptor.DupNode(BOR165);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(BOR165_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_exprOrSubquery_in_arithmeticExpr1935);
                    	exprOrSubquery166 = exprOrSubquery();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, exprOrSubquery166.Tree);
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_exprOrSubquery_in_arithmeticExpr1937);
                    	exprOrSubquery167 = exprOrSubquery();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, exprOrSubquery167.Tree);

                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}


                    }
                    break;
                case 8 :
                    // HqlSqlWalker.g:382:4: ^( BXOR exprOrSubquery exprOrSubquery )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	BXOR168=(IASTNode)Match(input,BXOR,FOLLOW_BXOR_in_arithmeticExpr1944); 
                    		BXOR168_tree = (IASTNode)adaptor.DupNode(BXOR168);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(BXOR168_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_exprOrSubquery_in_arithmeticExpr1946);
                    	exprOrSubquery169 = exprOrSubquery();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, exprOrSubquery169.Tree);
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_exprOrSubquery_in_arithmeticExpr1948);
                    	exprOrSubquery170 = exprOrSubquery();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, exprOrSubquery170.Tree);

                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}


                    }
                    break;
                case 9 :
                    // HqlSqlWalker.g:384:4: ^( UNARY_MINUS exprOrSubquery )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	UNARY_MINUS171=(IASTNode)Match(input,UNARY_MINUS,FOLLOW_UNARY_MINUS_in_arithmeticExpr1956); 
                    		UNARY_MINUS171_tree = (IASTNode)adaptor.DupNode(UNARY_MINUS171);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(UNARY_MINUS171_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_exprOrSubquery_in_arithmeticExpr1958);
                    	exprOrSubquery172 = exprOrSubquery();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, exprOrSubquery172.Tree);

                    	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
                    	}


                    }
                    break;
                case 10 :
                    // HqlSqlWalker.g:385:4: c= caseExpr
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_caseExpr_in_arithmeticExpr1966);
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
    // HqlSqlWalker.g:388:1: caseExpr : ( ^( CASE ( ^( WHEN logicalExpr expr ) )+ ( ^( ELSE expr ) )? ) | ^( CASE2 expr ( ^( WHEN expr expr ) )+ ( ^( ELSE expr ) )? ) );
    public HqlSqlWalker.caseExpr_return caseExpr() // throws RecognitionException [1]
    {   
        HqlSqlWalker.caseExpr_return retval = new HqlSqlWalker.caseExpr_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode CASE173 = null;
        IASTNode WHEN174 = null;
        IASTNode ELSE177 = null;
        IASTNode CASE2179 = null;
        IASTNode WHEN181 = null;
        IASTNode ELSE184 = null;
        HqlSqlWalker.logicalExpr_return logicalExpr175 = default(HqlSqlWalker.logicalExpr_return);

        HqlSqlWalker.expr_return expr176 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.expr_return expr178 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.expr_return expr180 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.expr_return expr182 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.expr_return expr183 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.expr_return expr185 = default(HqlSqlWalker.expr_return);


        IASTNode CASE173_tree=null;
        IASTNode WHEN174_tree=null;
        IASTNode ELSE177_tree=null;
        IASTNode CASE2179_tree=null;
        IASTNode WHEN181_tree=null;
        IASTNode ELSE184_tree=null;

        try 
    	{
            // HqlSqlWalker.g:389:2: ( ^( CASE ( ^( WHEN logicalExpr expr ) )+ ( ^( ELSE expr ) )? ) | ^( CASE2 expr ( ^( WHEN expr expr ) )+ ( ^( ELSE expr ) )? ) )
            int alt54 = 2;
            int LA54_0 = input.LA(1);

            if ( (LA54_0 == CASE) )
            {
                alt54 = 1;
            }
            else if ( (LA54_0 == CASE2) )
            {
                alt54 = 2;
            }
            else 
            {
                NoViableAltException nvae_d54s0 =
                    new NoViableAltException("", 54, 0, input);

                throw nvae_d54s0;
            }
            switch (alt54) 
            {
                case 1 :
                    // HqlSqlWalker.g:389:4: ^( CASE ( ^( WHEN logicalExpr expr ) )+ ( ^( ELSE expr ) )? )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	CASE173=(IASTNode)Match(input,CASE,FOLLOW_CASE_in_caseExpr1978); 
                    		CASE173_tree = (IASTNode)adaptor.DupNode(CASE173);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(CASE173_tree, root_1);


                    	 _inCase = true; 

                    	Match(input, Token.DOWN, null); 
                    	// HqlSqlWalker.g:389:31: ( ^( WHEN logicalExpr expr ) )+
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
                    			    // HqlSqlWalker.g:389:32: ^( WHEN logicalExpr expr )
                    			    {
                    			    	_last = (IASTNode)input.LT(1);
                    			    	{
                    			    	IASTNode _save_last_2 = _last;
                    			    	IASTNode _first_2 = null;
                    			    	IASTNode root_2 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    			    	WHEN174=(IASTNode)Match(input,WHEN,FOLLOW_WHEN_in_caseExpr1984); 
                    			    		WHEN174_tree = (IASTNode)adaptor.DupNode(WHEN174);

                    			    		root_2 = (IASTNode)adaptor.BecomeRoot(WHEN174_tree, root_2);



                    			    	Match(input, Token.DOWN, null); 
                    			    	_last = (IASTNode)input.LT(1);
                    			    	PushFollow(FOLLOW_logicalExpr_in_caseExpr1986);
                    			    	logicalExpr175 = logicalExpr();
                    			    	state.followingStackPointer--;

                    			    	adaptor.AddChild(root_2, logicalExpr175.Tree);
                    			    	_last = (IASTNode)input.LT(1);
                    			    	PushFollow(FOLLOW_expr_in_caseExpr1988);
                    			    	expr176 = expr();
                    			    	state.followingStackPointer--;

                    			    	adaptor.AddChild(root_2, expr176.Tree);

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
                    		;	// Stops C# compiler whining that label 'loop50' has no statements

                    	// HqlSqlWalker.g:389:59: ( ^( ELSE expr ) )?
                    	int alt51 = 2;
                    	int LA51_0 = input.LA(1);

                    	if ( (LA51_0 == ELSE) )
                    	{
                    	    alt51 = 1;
                    	}
                    	switch (alt51) 
                    	{
                    	    case 1 :
                    	        // HqlSqlWalker.g:389:60: ^( ELSE expr )
                    	        {
                    	        	_last = (IASTNode)input.LT(1);
                    	        	{
                    	        	IASTNode _save_last_2 = _last;
                    	        	IASTNode _first_2 = null;
                    	        	IASTNode root_2 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	        	ELSE177=(IASTNode)Match(input,ELSE,FOLLOW_ELSE_in_caseExpr1995); 
                    	        		ELSE177_tree = (IASTNode)adaptor.DupNode(ELSE177);

                    	        		root_2 = (IASTNode)adaptor.BecomeRoot(ELSE177_tree, root_2);



                    	        	Match(input, Token.DOWN, null); 
                    	        	_last = (IASTNode)input.LT(1);
                    	        	PushFollow(FOLLOW_expr_in_caseExpr1997);
                    	        	expr178 = expr();
                    	        	state.followingStackPointer--;

                    	        	adaptor.AddChild(root_2, expr178.Tree);

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
                    // HqlSqlWalker.g:390:4: ^( CASE2 expr ( ^( WHEN expr expr ) )+ ( ^( ELSE expr ) )? )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	CASE2179=(IASTNode)Match(input,CASE2,FOLLOW_CASE2_in_caseExpr2009); 
                    		CASE2179_tree = (IASTNode)adaptor.DupNode(CASE2179);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(CASE2179_tree, root_1);


                    	 _inCase = true; 

                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_expr_in_caseExpr2013);
                    	expr180 = expr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, expr180.Tree);
                    	// HqlSqlWalker.g:390:37: ( ^( WHEN expr expr ) )+
                    	int cnt52 = 0;
                    	do 
                    	{
                    	    int alt52 = 2;
                    	    int LA52_0 = input.LA(1);

                    	    if ( (LA52_0 == WHEN) )
                    	    {
                    	        alt52 = 1;
                    	    }


                    	    switch (alt52) 
                    		{
                    			case 1 :
                    			    // HqlSqlWalker.g:390:38: ^( WHEN expr expr )
                    			    {
                    			    	_last = (IASTNode)input.LT(1);
                    			    	{
                    			    	IASTNode _save_last_2 = _last;
                    			    	IASTNode _first_2 = null;
                    			    	IASTNode root_2 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    			    	WHEN181=(IASTNode)Match(input,WHEN,FOLLOW_WHEN_in_caseExpr2017); 
                    			    		WHEN181_tree = (IASTNode)adaptor.DupNode(WHEN181);

                    			    		root_2 = (IASTNode)adaptor.BecomeRoot(WHEN181_tree, root_2);



                    			    	Match(input, Token.DOWN, null); 
                    			    	_last = (IASTNode)input.LT(1);
                    			    	PushFollow(FOLLOW_expr_in_caseExpr2019);
                    			    	expr182 = expr();
                    			    	state.followingStackPointer--;

                    			    	adaptor.AddChild(root_2, expr182.Tree);
                    			    	_last = (IASTNode)input.LT(1);
                    			    	PushFollow(FOLLOW_expr_in_caseExpr2021);
                    			    	expr183 = expr();
                    			    	state.followingStackPointer--;

                    			    	adaptor.AddChild(root_2, expr183.Tree);

                    			    	Match(input, Token.UP, null); adaptor.AddChild(root_1, root_2);_last = _save_last_2;
                    			    	}


                    			    }
                    			    break;

                    			default:
                    			    if ( cnt52 >= 1 ) goto loop52;
                    		            EarlyExitException eee52 =
                    		                new EarlyExitException(52, input);
                    		            throw eee52;
                    	    }
                    	    cnt52++;
                    	} while (true);

                    	loop52:
                    		;	// Stops C# compiler whining that label 'loop52' has no statements

                    	// HqlSqlWalker.g:390:58: ( ^( ELSE expr ) )?
                    	int alt53 = 2;
                    	int LA53_0 = input.LA(1);

                    	if ( (LA53_0 == ELSE) )
                    	{
                    	    alt53 = 1;
                    	}
                    	switch (alt53) 
                    	{
                    	    case 1 :
                    	        // HqlSqlWalker.g:390:59: ^( ELSE expr )
                    	        {
                    	        	_last = (IASTNode)input.LT(1);
                    	        	{
                    	        	IASTNode _save_last_2 = _last;
                    	        	IASTNode _first_2 = null;
                    	        	IASTNode root_2 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	        	ELSE184=(IASTNode)Match(input,ELSE,FOLLOW_ELSE_in_caseExpr2028); 
                    	        		ELSE184_tree = (IASTNode)adaptor.DupNode(ELSE184);

                    	        		root_2 = (IASTNode)adaptor.BecomeRoot(ELSE184_tree, root_2);



                    	        	Match(input, Token.DOWN, null); 
                    	        	_last = (IASTNode)input.LT(1);
                    	        	PushFollow(FOLLOW_expr_in_caseExpr2030);
                    	        	expr185 = expr();
                    	        	state.followingStackPointer--;

                    	        	adaptor.AddChild(root_2, expr185.Tree);

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
    // HqlSqlWalker.g:395:1: collectionFunction : ( ^(e= ELEMENTS p1= propertyRef ) | ^(i= INDICES p2= propertyRef ) );
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
            // HqlSqlWalker.g:396:2: ( ^(e= ELEMENTS p1= propertyRef ) | ^(i= INDICES p2= propertyRef ) )
            int alt55 = 2;
            int LA55_0 = input.LA(1);

            if ( (LA55_0 == ELEMENTS) )
            {
                alt55 = 1;
            }
            else if ( (LA55_0 == INDICES) )
            {
                alt55 = 2;
            }
            else 
            {
                NoViableAltException nvae_d55s0 =
                    new NoViableAltException("", 55, 0, input);

                throw nvae_d55s0;
            }
            switch (alt55) 
            {
                case 1 :
                    // HqlSqlWalker.g:396:4: ^(e= ELEMENTS p1= propertyRef )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	e=(IASTNode)Match(input,ELEMENTS,FOLLOW_ELEMENTS_in_collectionFunction2052); 
                    		e_tree = (IASTNode)adaptor.DupNode(e);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(e_tree, root_1);


                    	_inFunctionCall=true;

                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_propertyRef_in_collectionFunction2058);
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
                    // HqlSqlWalker.g:398:4: ^(i= INDICES p2= propertyRef )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	i=(IASTNode)Match(input,INDICES,FOLLOW_INDICES_in_collectionFunction2077); 
                    		i_tree = (IASTNode)adaptor.DupNode(i);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(i_tree, root_1);


                    	_inFunctionCall=true;

                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_propertyRef_in_collectionFunction2083);
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
    // HqlSqlWalker.g:402:1: functionCall : ( ^(m= METHOD_CALL pathAsIdent ( ^( EXPR_LIST ( expr | query | comparisonExpr )* ) )? ) | ^( AGGREGATE aggregateExpr ) );
    public HqlSqlWalker.functionCall_return functionCall() // throws RecognitionException [1]
    {   
        HqlSqlWalker.functionCall_return retval = new HqlSqlWalker.functionCall_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode m = null;
        IASTNode EXPR_LIST187 = null;
        IASTNode AGGREGATE191 = null;
        HqlSqlWalker.pathAsIdent_return pathAsIdent186 = default(HqlSqlWalker.pathAsIdent_return);

        HqlSqlWalker.expr_return expr188 = default(HqlSqlWalker.expr_return);

        HqlSqlWalker.query_return query189 = default(HqlSqlWalker.query_return);

        HqlSqlWalker.comparisonExpr_return comparisonExpr190 = default(HqlSqlWalker.comparisonExpr_return);

        HqlSqlWalker.aggregateExpr_return aggregateExpr192 = default(HqlSqlWalker.aggregateExpr_return);


        IASTNode m_tree=null;
        IASTNode EXPR_LIST187_tree=null;
        IASTNode AGGREGATE191_tree=null;

        try 
    	{
            // HqlSqlWalker.g:403:2: ( ^(m= METHOD_CALL pathAsIdent ( ^( EXPR_LIST ( expr | query | comparisonExpr )* ) )? ) | ^( AGGREGATE aggregateExpr ) )
            int alt58 = 2;
            int LA58_0 = input.LA(1);

            if ( (LA58_0 == METHOD_CALL) )
            {
                alt58 = 1;
            }
            else if ( (LA58_0 == AGGREGATE) )
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
                    // HqlSqlWalker.g:403:4: ^(m= METHOD_CALL pathAsIdent ( ^( EXPR_LIST ( expr | query | comparisonExpr )* ) )? )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	m=(IASTNode)Match(input,METHOD_CALL,FOLLOW_METHOD_CALL_in_functionCall2108); 
                    		m_tree = (IASTNode)adaptor.DupNode(m);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(m_tree, root_1);


                    	_inFunctionCall=true;

                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_pathAsIdent_in_functionCall2113);
                    	pathAsIdent186 = pathAsIdent();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, pathAsIdent186.Tree);
                    	// HqlSqlWalker.g:403:57: ( ^( EXPR_LIST ( expr | query | comparisonExpr )* ) )?
                    	int alt57 = 2;
                    	int LA57_0 = input.LA(1);

                    	if ( (LA57_0 == EXPR_LIST) )
                    	{
                    	    alt57 = 1;
                    	}
                    	switch (alt57) 
                    	{
                    	    case 1 :
                    	        // HqlSqlWalker.g:403:59: ^( EXPR_LIST ( expr | query | comparisonExpr )* )
                    	        {
                    	        	_last = (IASTNode)input.LT(1);
                    	        	{
                    	        	IASTNode _save_last_2 = _last;
                    	        	IASTNode _first_2 = null;
                    	        	IASTNode root_2 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	        	EXPR_LIST187=(IASTNode)Match(input,EXPR_LIST,FOLLOW_EXPR_LIST_in_functionCall2118); 
                    	        		EXPR_LIST187_tree = (IASTNode)adaptor.DupNode(EXPR_LIST187);

                    	        		root_2 = (IASTNode)adaptor.BecomeRoot(EXPR_LIST187_tree, root_2);



                    	        	if ( input.LA(1) == Token.DOWN )
                    	        	{
                    	        	    Match(input, Token.DOWN, null); 
                    	        	    // HqlSqlWalker.g:403:71: ( expr | query | comparisonExpr )*
                    	        	    do 
                    	        	    {
                    	        	        int alt56 = 4;
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
                    	        	        case NUM_DECIMAL:
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
                    	        	        case COLON:
                    	        	        case PARAM:
                    	        	        case QUOTED_String:
                    	        	        case IDENT:
                    	        	        	{
                    	        	            alt56 = 1;
                    	        	            }
                    	        	            break;
                    	        	        case UNION:
                    	        	        case QUERY:
                    	        	        	{
                    	        	            alt56 = 2;
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
                    	        	            alt56 = 3;
                    	        	            }
                    	        	            break;

                    	        	        }

                    	        	        switch (alt56) 
                    	        	    	{
                    	        	    		case 1 :
                    	        	    		    // HqlSqlWalker.g:403:72: expr
                    	        	    		    {
                    	        	    		    	_last = (IASTNode)input.LT(1);
                    	        	    		    	PushFollow(FOLLOW_expr_in_functionCall2121);
                    	        	    		    	expr188 = expr();
                    	        	    		    	state.followingStackPointer--;

                    	        	    		    	adaptor.AddChild(root_2, expr188.Tree);

                    	        	    		    }
                    	        	    		    break;
                    	        	    		case 2 :
                    	        	    		    // HqlSqlWalker.g:403:79: query
                    	        	    		    {
                    	        	    		    	_last = (IASTNode)input.LT(1);
                    	        	    		    	PushFollow(FOLLOW_query_in_functionCall2125);
                    	        	    		    	query189 = query();
                    	        	    		    	state.followingStackPointer--;

                    	        	    		    	adaptor.AddChild(root_2, query189.Tree);

                    	        	    		    }
                    	        	    		    break;
                    	        	    		case 3 :
                    	        	    		    // HqlSqlWalker.g:403:87: comparisonExpr
                    	        	    		    {
                    	        	    		    	_last = (IASTNode)input.LT(1);
                    	        	    		    	PushFollow(FOLLOW_comparisonExpr_in_functionCall2129);
                    	        	    		    	comparisonExpr190 = comparisonExpr();
                    	        	    		    	state.followingStackPointer--;

                    	        	    		    	adaptor.AddChild(root_2, comparisonExpr190.Tree);

                    	        	    		    }
                    	        	    		    break;

                    	        	    		default:
                    	        	    		    goto loop56;
                    	        	        }
                    	        	    } while (true);

                    	        	    loop56:
                    	        	    	;	// Stops C# compiler whining that label 'loop56' has no statements


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
                    // HqlSqlWalker.g:405:4: ^( AGGREGATE aggregateExpr )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	AGGREGATE191=(IASTNode)Match(input,AGGREGATE,FOLLOW_AGGREGATE_in_functionCall2148); 
                    		AGGREGATE191_tree = (IASTNode)adaptor.DupNode(AGGREGATE191);

                    		root_1 = (IASTNode)adaptor.BecomeRoot(AGGREGATE191_tree, root_1);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_aggregateExpr_in_functionCall2150);
                    	aggregateExpr192 = aggregateExpr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_1, aggregateExpr192.Tree);

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
    // HqlSqlWalker.g:408:1: constant : ( literal | NULL | t= TRUE | f= FALSE | JAVA_CONSTANT );
    public HqlSqlWalker.constant_return constant() // throws RecognitionException [1]
    {   
        HqlSqlWalker.constant_return retval = new HqlSqlWalker.constant_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode t = null;
        IASTNode f = null;
        IASTNode NULL194 = null;
        IASTNode JAVA_CONSTANT195 = null;
        HqlSqlWalker.literal_return literal193 = default(HqlSqlWalker.literal_return);


        IASTNode t_tree=null;
        IASTNode f_tree=null;
        IASTNode NULL194_tree=null;
        IASTNode JAVA_CONSTANT195_tree=null;

        try 
    	{
            // HqlSqlWalker.g:409:2: ( literal | NULL | t= TRUE | f= FALSE | JAVA_CONSTANT )
            int alt59 = 5;
            switch ( input.LA(1) ) 
            {
            case NUM_INT:
            case NUM_DOUBLE:
            case NUM_DECIMAL:
            case NUM_FLOAT:
            case NUM_LONG:
            case QUOTED_String:
            	{
                alt59 = 1;
                }
                break;
            case NULL:
            	{
                alt59 = 2;
                }
                break;
            case TRUE:
            	{
                alt59 = 3;
                }
                break;
            case FALSE:
            	{
                alt59 = 4;
                }
                break;
            case JAVA_CONSTANT:
            	{
                alt59 = 5;
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
                    // HqlSqlWalker.g:409:4: literal
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_literal_in_constant2163);
                    	literal193 = literal();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, literal193.Tree);

                    }
                    break;
                case 2 :
                    // HqlSqlWalker.g:410:4: NULL
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	NULL194=(IASTNode)Match(input,NULL,FOLLOW_NULL_in_constant2168); 
                    		NULL194_tree = (IASTNode)adaptor.DupNode(NULL194);

                    		adaptor.AddChild(root_0, NULL194_tree);


                    }
                    break;
                case 3 :
                    // HqlSqlWalker.g:411:4: t= TRUE
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	t=(IASTNode)Match(input,TRUE,FOLLOW_TRUE_in_constant2175); 
                    		t_tree = (IASTNode)adaptor.DupNode(t);

                    		adaptor.AddChild(root_0, t_tree);

                    	 ProcessBool(t); 

                    }
                    break;
                case 4 :
                    // HqlSqlWalker.g:412:4: f= FALSE
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	f=(IASTNode)Match(input,FALSE,FOLLOW_FALSE_in_constant2185); 
                    		f_tree = (IASTNode)adaptor.DupNode(f);

                    		adaptor.AddChild(root_0, f_tree);

                    	 ProcessBool(f); 

                    }
                    break;
                case 5 :
                    // HqlSqlWalker.g:413:4: JAVA_CONSTANT
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	JAVA_CONSTANT195=(IASTNode)Match(input,JAVA_CONSTANT,FOLLOW_JAVA_CONSTANT_in_constant2192); 
                    		JAVA_CONSTANT195_tree = (IASTNode)adaptor.DupNode(JAVA_CONSTANT195);

                    		adaptor.AddChild(root_0, JAVA_CONSTANT195_tree);


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
    // HqlSqlWalker.g:416:1: literal : ( numericLiteral | stringLiteral );
    public HqlSqlWalker.literal_return literal() // throws RecognitionException [1]
    {   
        HqlSqlWalker.literal_return retval = new HqlSqlWalker.literal_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        HqlSqlWalker.numericLiteral_return numericLiteral196 = default(HqlSqlWalker.numericLiteral_return);

        HqlSqlWalker.stringLiteral_return stringLiteral197 = default(HqlSqlWalker.stringLiteral_return);



        try 
    	{
            // HqlSqlWalker.g:417:2: ( numericLiteral | stringLiteral )
            int alt60 = 2;
            int LA60_0 = input.LA(1);

            if ( ((LA60_0 >= NUM_INT && LA60_0 <= NUM_LONG)) )
            {
                alt60 = 1;
            }
            else if ( (LA60_0 == QUOTED_String) )
            {
                alt60 = 2;
            }
            else 
            {
                NoViableAltException nvae_d60s0 =
                    new NoViableAltException("", 60, 0, input);

                throw nvae_d60s0;
            }
            switch (alt60) 
            {
                case 1 :
                    // HqlSqlWalker.g:417:4: numericLiteral
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_numericLiteral_in_literal2203);
                    	numericLiteral196 = numericLiteral();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, numericLiteral196.Tree);

                    }
                    break;
                case 2 :
                    // HqlSqlWalker.g:418:4: stringLiteral
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_stringLiteral_in_literal2208);
                    	stringLiteral197 = stringLiteral();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, stringLiteral197.Tree);

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
    // HqlSqlWalker.g:421:1: numericLiteral : ( NUM_INT | NUM_LONG | NUM_FLOAT | NUM_DOUBLE | NUM_DECIMAL );
    public HqlSqlWalker.numericLiteral_return numericLiteral() // throws RecognitionException [1]
    {   
        HqlSqlWalker.numericLiteral_return retval = new HqlSqlWalker.numericLiteral_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode set198 = null;

        IASTNode set198_tree=null;

        try 
    	{
            // HqlSqlWalker.g:426:2: ( NUM_INT | NUM_LONG | NUM_FLOAT | NUM_DOUBLE | NUM_DECIMAL )
            // HqlSqlWalker.g:
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	set198 = (IASTNode)input.LT(1);
            	if ( (input.LA(1) >= NUM_INT && input.LA(1) <= NUM_LONG) ) 
            	{
            	    input.Consume();

            	    set198_tree = (IASTNode)adaptor.DupNode(set198);

            	    adaptor.AddChild(root_0, set198_tree);

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
    // HqlSqlWalker.g:433:1: stringLiteral : QUOTED_String ;
    public HqlSqlWalker.stringLiteral_return stringLiteral() // throws RecognitionException [1]
    {   
        HqlSqlWalker.stringLiteral_return retval = new HqlSqlWalker.stringLiteral_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode QUOTED_String199 = null;

        IASTNode QUOTED_String199_tree=null;

        try 
    	{
            // HqlSqlWalker.g:434:2: ( QUOTED_String )
            // HqlSqlWalker.g:434:4: QUOTED_String
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	QUOTED_String199=(IASTNode)Match(input,QUOTED_String,FOLLOW_QUOTED_String_in_stringLiteral2255); 
            		QUOTED_String199_tree = (IASTNode)adaptor.DupNode(QUOTED_String199);

            		adaptor.AddChild(root_0, QUOTED_String199_tree);


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
    // HqlSqlWalker.g:437:1: identifier : ( IDENT | WEIRD_IDENT ) ;
    public HqlSqlWalker.identifier_return identifier() // throws RecognitionException [1]
    {   
        HqlSqlWalker.identifier_return retval = new HqlSqlWalker.identifier_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode set200 = null;

        IASTNode set200_tree=null;

        try 
    	{
            // HqlSqlWalker.g:438:2: ( ( IDENT | WEIRD_IDENT ) )
            // HqlSqlWalker.g:438:4: ( IDENT | WEIRD_IDENT )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	set200 = (IASTNode)input.LT(1);
            	if ( input.LA(1) == WEIRD_IDENT || input.LA(1) == IDENT ) 
            	{
            	    input.Consume();

            	    set200_tree = (IASTNode)adaptor.DupNode(set200);

            	    adaptor.AddChild(root_0, set200_tree);

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
    // HqlSqlWalker.g:441:1: addrExpr[ bool root ] : ( addrExprDot[root] | addrExprIndex[root] | addrExprIdent[root] );
    public HqlSqlWalker.addrExpr_return addrExpr(bool root) // throws RecognitionException [1]
    {   
        HqlSqlWalker.addrExpr_return retval = new HqlSqlWalker.addrExpr_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        HqlSqlWalker.addrExprDot_return addrExprDot201 = default(HqlSqlWalker.addrExprDot_return);

        HqlSqlWalker.addrExprIndex_return addrExprIndex202 = default(HqlSqlWalker.addrExprIndex_return);

        HqlSqlWalker.addrExprIdent_return addrExprIdent203 = default(HqlSqlWalker.addrExprIdent_return);



        try 
    	{
            // HqlSqlWalker.g:442:2: ( addrExprDot[root] | addrExprIndex[root] | addrExprIdent[root] )
            int alt61 = 3;
            switch ( input.LA(1) ) 
            {
            case DOT:
            	{
                alt61 = 1;
                }
                break;
            case INDEX_OP:
            	{
                alt61 = 2;
                }
                break;
            case WEIRD_IDENT:
            case IDENT:
            	{
                alt61 = 3;
                }
                break;
            	default:
            	    NoViableAltException nvae_d61s0 =
            	        new NoViableAltException("", 61, 0, input);

            	    throw nvae_d61s0;
            }

            switch (alt61) 
            {
                case 1 :
                    // HqlSqlWalker.g:442:4: addrExprDot[root]
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_addrExprDot_in_addrExpr2285);
                    	addrExprDot201 = addrExprDot(root);
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, addrExprDot201.Tree);

                    }
                    break;
                case 2 :
                    // HqlSqlWalker.g:443:4: addrExprIndex[root]
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_addrExprIndex_in_addrExpr2292);
                    	addrExprIndex202 = addrExprIndex(root);
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, addrExprIndex202.Tree);

                    }
                    break;
                case 3 :
                    // HqlSqlWalker.g:444:4: addrExprIdent[root]
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_addrExprIdent_in_addrExpr2299);
                    	addrExprIdent203 = addrExprIdent(root);
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, addrExprIdent203.Tree);

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
    // HqlSqlWalker.g:447:1: addrExprDot[ bool root ] : ^(d= DOT lhs= addrExprLhs rhs= propertyName ) -> ^( $d $lhs $rhs) ;
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
            // HqlSqlWalker.g:452:2: ( ^(d= DOT lhs= addrExprLhs rhs= propertyName ) -> ^( $d $lhs $rhs) )
            // HqlSqlWalker.g:452:4: ^(d= DOT lhs= addrExprLhs rhs= propertyName )
            {
            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	d=(IASTNode)Match(input,DOT,FOLLOW_DOT_in_addrExprDot2323);  
            	stream_DOT.Add(d);



            	Match(input, Token.DOWN, null); 
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_addrExprLhs_in_addrExprDot2327);
            	lhs = addrExprLhs();
            	state.followingStackPointer--;

            	stream_addrExprLhs.Add(lhs.Tree);
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_propertyName_in_addrExprDot2331);
            	rhs = propertyName();
            	state.followingStackPointer--;

            	stream_propertyName.Add(rhs.Tree);

            	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	}



            	// AST REWRITE
            	// elements:          d, rhs, lhs
            	// token labels:      d
            	// rule labels:       retval, rhs, lhs
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	retval.Tree = root_0;
            	RewriteRuleNodeStream stream_d = new RewriteRuleNodeStream(adaptor, "token d", d);
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);
            	RewriteRuleSubtreeStream stream_rhs = new RewriteRuleSubtreeStream(adaptor, "rule rhs", rhs!=null ? rhs.Tree : null);
            	RewriteRuleSubtreeStream stream_lhs = new RewriteRuleSubtreeStream(adaptor, "rule lhs", lhs!=null ? lhs.Tree : null);

            	root_0 = (IASTNode)adaptor.GetNilNode();
            	// 453:3: -> ^( $d $lhs $rhs)
            	{
            	    // HqlSqlWalker.g:453:6: ^( $d $lhs $rhs)
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
    // HqlSqlWalker.g:456:1: addrExprIndex[ bool root ] : ^(i= INDEX_OP lhs2= addrExprLhs rhs2= expr ) -> ^( $i $lhs2 $rhs2) ;
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
        RewriteRuleSubtreeStream stream_addrExprLhs = new RewriteRuleSubtreeStream(adaptor,"rule addrExprLhs");
        RewriteRuleSubtreeStream stream_expr = new RewriteRuleSubtreeStream(adaptor,"rule expr");
        try 
    	{
            // HqlSqlWalker.g:462:2: ( ^(i= INDEX_OP lhs2= addrExprLhs rhs2= expr ) -> ^( $i $lhs2 $rhs2) )
            // HqlSqlWalker.g:462:4: ^(i= INDEX_OP lhs2= addrExprLhs rhs2= expr )
            {
            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	i=(IASTNode)Match(input,INDEX_OP,FOLLOW_INDEX_OP_in_addrExprIndex2370);  
            	stream_INDEX_OP.Add(i);



            	Match(input, Token.DOWN, null); 
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_addrExprLhs_in_addrExprIndex2374);
            	lhs2 = addrExprLhs();
            	state.followingStackPointer--;

            	stream_addrExprLhs.Add(lhs2.Tree);
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_expr_in_addrExprIndex2378);
            	rhs2 = expr();
            	state.followingStackPointer--;

            	stream_expr.Add(rhs2.Tree);

            	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	}



            	// AST REWRITE
            	// elements:          rhs2, i, lhs2
            	// token labels:      i
            	// rule labels:       retval, rhs2, lhs2
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	retval.Tree = root_0;
            	RewriteRuleNodeStream stream_i = new RewriteRuleNodeStream(adaptor, "token i", i);
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);
            	RewriteRuleSubtreeStream stream_rhs2 = new RewriteRuleSubtreeStream(adaptor, "rule rhs2", rhs2!=null ? rhs2.Tree : null);
            	RewriteRuleSubtreeStream stream_lhs2 = new RewriteRuleSubtreeStream(adaptor, "rule lhs2", lhs2!=null ? lhs2.Tree : null);

            	root_0 = (IASTNode)adaptor.GetNilNode();
            	// 463:3: -> ^( $i $lhs2 $rhs2)
            	{
            	    // HqlSqlWalker.g:463:6: ^( $i $lhs2 $rhs2)
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
    // HqlSqlWalker.g:466:1: addrExprIdent[ bool root ] : p= identifier -> {IsNonQualifiedPropertyRef($p.tree)}? ^() -> ^() ;
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
            // HqlSqlWalker.g:467:2: (p= identifier -> {IsNonQualifiedPropertyRef($p.tree)}? ^() -> ^() )
            // HqlSqlWalker.g:467:4: p= identifier
            {
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_identifier_in_addrExprIdent2410);
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
            	// 468:2: -> {IsNonQualifiedPropertyRef($p.tree)}? ^()
            	if (IsNonQualifiedPropertyRef(((p != null) ? ((IASTNode)p.Tree) : null)))
            	{
            	    // HqlSqlWalker.g:468:43: ^()
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot(LookupNonQualifiedProperty(((p != null) ? ((IASTNode)p.Tree) : null)), root_1);

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}
            	else // 469:2: -> ^()
            	{
            	    // HqlSqlWalker.g:469:5: ^()
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
    // HqlSqlWalker.g:472:1: addrExprLhs : addrExpr[ false ] ;
    public HqlSqlWalker.addrExprLhs_return addrExprLhs() // throws RecognitionException [1]
    {   
        HqlSqlWalker.addrExprLhs_return retval = new HqlSqlWalker.addrExprLhs_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        HqlSqlWalker.addrExpr_return addrExpr204 = default(HqlSqlWalker.addrExpr_return);



        try 
    	{
            // HqlSqlWalker.g:473:2: ( addrExpr[ false ] )
            // HqlSqlWalker.g:473:4: addrExpr[ false ]
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_addrExpr_in_addrExprLhs2438);
            	addrExpr204 = addrExpr(false);
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, addrExpr204.Tree);

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
    // HqlSqlWalker.g:476:1: propertyName : ( identifier | CLASS | ELEMENTS | INDICES );
    public HqlSqlWalker.propertyName_return propertyName() // throws RecognitionException [1]
    {   
        HqlSqlWalker.propertyName_return retval = new HqlSqlWalker.propertyName_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode CLASS206 = null;
        IASTNode ELEMENTS207 = null;
        IASTNode INDICES208 = null;
        HqlSqlWalker.identifier_return identifier205 = default(HqlSqlWalker.identifier_return);


        IASTNode CLASS206_tree=null;
        IASTNode ELEMENTS207_tree=null;
        IASTNode INDICES208_tree=null;

        try 
    	{
            // HqlSqlWalker.g:477:2: ( identifier | CLASS | ELEMENTS | INDICES )
            int alt62 = 4;
            switch ( input.LA(1) ) 
            {
            case WEIRD_IDENT:
            case IDENT:
            	{
                alt62 = 1;
                }
                break;
            case CLASS:
            	{
                alt62 = 2;
                }
                break;
            case ELEMENTS:
            	{
                alt62 = 3;
                }
                break;
            case INDICES:
            	{
                alt62 = 4;
                }
                break;
            	default:
            	    NoViableAltException nvae_d62s0 =
            	        new NoViableAltException("", 62, 0, input);

            	    throw nvae_d62s0;
            }

            switch (alt62) 
            {
                case 1 :
                    // HqlSqlWalker.g:477:4: identifier
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_identifier_in_propertyName2451);
                    	identifier205 = identifier();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, identifier205.Tree);

                    }
                    break;
                case 2 :
                    // HqlSqlWalker.g:478:4: CLASS
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	CLASS206=(IASTNode)Match(input,CLASS,FOLLOW_CLASS_in_propertyName2456); 
                    		CLASS206_tree = (IASTNode)adaptor.DupNode(CLASS206);

                    		adaptor.AddChild(root_0, CLASS206_tree);


                    }
                    break;
                case 3 :
                    // HqlSqlWalker.g:479:4: ELEMENTS
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	ELEMENTS207=(IASTNode)Match(input,ELEMENTS,FOLLOW_ELEMENTS_in_propertyName2461); 
                    		ELEMENTS207_tree = (IASTNode)adaptor.DupNode(ELEMENTS207);

                    		adaptor.AddChild(root_0, ELEMENTS207_tree);


                    }
                    break;
                case 4 :
                    // HqlSqlWalker.g:480:4: INDICES
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	INDICES208=(IASTNode)Match(input,INDICES,FOLLOW_INDICES_in_propertyName2466); 
                    		INDICES208_tree = (IASTNode)adaptor.DupNode(INDICES208);

                    		adaptor.AddChild(root_0, INDICES208_tree);


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
    // HqlSqlWalker.g:483:1: propertyRef : ( propertyRefPath | propertyRefIdent );
    public HqlSqlWalker.propertyRef_return propertyRef() // throws RecognitionException [1]
    {   
        HqlSqlWalker.propertyRef_return retval = new HqlSqlWalker.propertyRef_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        HqlSqlWalker.propertyRefPath_return propertyRefPath209 = default(HqlSqlWalker.propertyRefPath_return);

        HqlSqlWalker.propertyRefIdent_return propertyRefIdent210 = default(HqlSqlWalker.propertyRefIdent_return);



        try 
    	{
            // HqlSqlWalker.g:484:2: ( propertyRefPath | propertyRefIdent )
            int alt63 = 2;
            int LA63_0 = input.LA(1);

            if ( (LA63_0 == DOT) )
            {
                alt63 = 1;
            }
            else if ( (LA63_0 == WEIRD_IDENT || LA63_0 == IDENT) )
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
                    // HqlSqlWalker.g:484:4: propertyRefPath
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_propertyRefPath_in_propertyRef2478);
                    	propertyRefPath209 = propertyRefPath();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, propertyRefPath209.Tree);

                    }
                    break;
                case 2 :
                    // HqlSqlWalker.g:485:4: propertyRefIdent
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_propertyRefIdent_in_propertyRef2483);
                    	propertyRefIdent210 = propertyRefIdent();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, propertyRefIdent210.Tree);

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
    // HqlSqlWalker.g:488:1: propertyRefPath : ^(d= DOT lhs= propertyRefLhs rhs= propertyName ) -> ^( $d $lhs $rhs) ;
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
            // HqlSqlWalker.g:493:2: ( ^(d= DOT lhs= propertyRefLhs rhs= propertyName ) -> ^( $d $lhs $rhs) )
            // HqlSqlWalker.g:493:4: ^(d= DOT lhs= propertyRefLhs rhs= propertyName )
            {
            	_last = (IASTNode)input.LT(1);
            	{
            	IASTNode _save_last_1 = _last;
            	IASTNode _first_1 = null;
            	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
            	d=(IASTNode)Match(input,DOT,FOLLOW_DOT_in_propertyRefPath2503);  
            	stream_DOT.Add(d);



            	Match(input, Token.DOWN, null); 
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_propertyRefLhs_in_propertyRefPath2507);
            	lhs = propertyRefLhs();
            	state.followingStackPointer--;

            	stream_propertyRefLhs.Add(lhs.Tree);
            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_propertyName_in_propertyRefPath2511);
            	rhs = propertyName();
            	state.followingStackPointer--;

            	stream_propertyName.Add(rhs.Tree);

            	Match(input, Token.UP, null); adaptor.AddChild(root_0, root_1);_last = _save_last_1;
            	}



            	// AST REWRITE
            	// elements:          rhs, d, lhs
            	// token labels:      d
            	// rule labels:       retval, rhs, lhs
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	retval.Tree = root_0;
            	RewriteRuleNodeStream stream_d = new RewriteRuleNodeStream(adaptor, "token d", d);
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);
            	RewriteRuleSubtreeStream stream_rhs = new RewriteRuleSubtreeStream(adaptor, "rule rhs", rhs!=null ? rhs.Tree : null);
            	RewriteRuleSubtreeStream stream_lhs = new RewriteRuleSubtreeStream(adaptor, "rule lhs", lhs!=null ? lhs.Tree : null);

            	root_0 = (IASTNode)adaptor.GetNilNode();
            	// 494:3: -> ^( $d $lhs $rhs)
            	{
            	    // HqlSqlWalker.g:494:6: ^( $d $lhs $rhs)
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
    // HqlSqlWalker.g:497:1: propertyRefIdent : p= identifier ;
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
            // HqlSqlWalker.g:511:2: (p= identifier )
            // HqlSqlWalker.g:511:4: p= identifier
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_identifier_in_propertyRefIdent2548);
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
    // HqlSqlWalker.g:514:1: propertyRefLhs : propertyRef ;
    public HqlSqlWalker.propertyRefLhs_return propertyRefLhs() // throws RecognitionException [1]
    {   
        HqlSqlWalker.propertyRefLhs_return retval = new HqlSqlWalker.propertyRefLhs_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        HqlSqlWalker.propertyRef_return propertyRef211 = default(HqlSqlWalker.propertyRef_return);



        try 
    	{
            // HqlSqlWalker.g:515:2: ( propertyRef )
            // HqlSqlWalker.g:515:4: propertyRef
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_propertyRef_in_propertyRefLhs2560);
            	propertyRef211 = propertyRef();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, propertyRef211.Tree);

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
    // HqlSqlWalker.g:518:1: aliasRef : i= identifier ;
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
            // HqlSqlWalker.g:523:2: (i= identifier )
            // HqlSqlWalker.g:523:4: i= identifier
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	PushFollow(FOLLOW_identifier_in_aliasRef2581);
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
    // HqlSqlWalker.g:527:1: parameter : ( ^(c= COLON a= identifier ) -> ^() | ^(p= PARAM (n= NUM_INT )? ) -> {n != null}? ^() -> ^() );
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
        RewriteRuleNodeStream stream_COLON = new RewriteRuleNodeStream(adaptor,"token COLON");
        RewriteRuleNodeStream stream_PARAM = new RewriteRuleNodeStream(adaptor,"token PARAM");
        RewriteRuleNodeStream stream_NUM_INT = new RewriteRuleNodeStream(adaptor,"token NUM_INT");
        RewriteRuleSubtreeStream stream_identifier = new RewriteRuleSubtreeStream(adaptor,"rule identifier");
        try 
    	{
            // HqlSqlWalker.g:528:2: ( ^(c= COLON a= identifier ) -> ^() | ^(p= PARAM (n= NUM_INT )? ) -> {n != null}? ^() -> ^() )
            int alt65 = 2;
            int LA65_0 = input.LA(1);

            if ( (LA65_0 == COLON) )
            {
                alt65 = 1;
            }
            else if ( (LA65_0 == PARAM) )
            {
                alt65 = 2;
            }
            else 
            {
                NoViableAltException nvae_d65s0 =
                    new NoViableAltException("", 65, 0, input);

                throw nvae_d65s0;
            }
            switch (alt65) 
            {
                case 1 :
                    // HqlSqlWalker.g:528:4: ^(c= COLON a= identifier )
                    {
                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	c=(IASTNode)Match(input,COLON,FOLLOW_COLON_in_parameter2599);  
                    	stream_COLON.Add(c);



                    	Match(input, Token.DOWN, null); 
                    	_last = (IASTNode)input.LT(1);
                    	PushFollow(FOLLOW_identifier_in_parameter2603);
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
                    	// 530:3: -> ^()
                    	{
                    	    // HqlSqlWalker.g:530:6: ^()
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
                    // HqlSqlWalker.g:531:4: ^(p= PARAM (n= NUM_INT )? )
                    {
                    	_last = (IASTNode)input.LT(1);
                    	{
                    	IASTNode _save_last_1 = _last;
                    	IASTNode _first_1 = null;
                    	IASTNode root_1 = (IASTNode)adaptor.GetNilNode();_last = (IASTNode)input.LT(1);
                    	p=(IASTNode)Match(input,PARAM,FOLLOW_PARAM_in_parameter2624);  
                    	stream_PARAM.Add(p);



                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); 
                    	    // HqlSqlWalker.g:531:14: (n= NUM_INT )?
                    	    int alt64 = 2;
                    	    int LA64_0 = input.LA(1);

                    	    if ( (LA64_0 == NUM_INT) )
                    	    {
                    	        alt64 = 1;
                    	    }
                    	    switch (alt64) 
                    	    {
                    	        case 1 :
                    	            // HqlSqlWalker.g:531:15: n= NUM_INT
                    	            {
                    	            	_last = (IASTNode)input.LT(1);
                    	            	n=(IASTNode)Match(input,NUM_INT,FOLLOW_NUM_INT_in_parameter2629);  
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
                    	// 532:3: -> {n != null}? ^()
                    	if (n != null)
                    	{
                    	    // HqlSqlWalker.g:532:19: ^()
                    	    {
                    	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
                    	    root_1 = (IASTNode)adaptor.BecomeRoot(GenerateNamedParameter( p, n ), root_1);

                    	    adaptor.AddChild(root_0, root_1);
                    	    }

                    	}
                    	else // 533:3: -> ^()
                    	{
                    	    // HqlSqlWalker.g:533:6: ^()
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
    // HqlSqlWalker.g:536:1: numericInteger : NUM_INT ;
    public HqlSqlWalker.numericInteger_return numericInteger() // throws RecognitionException [1]
    {   
        HqlSqlWalker.numericInteger_return retval = new HqlSqlWalker.numericInteger_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IASTNode _first_0 = null;
        IASTNode _last = null;

        IASTNode NUM_INT212 = null;

        IASTNode NUM_INT212_tree=null;

        try 
    	{
            // HqlSqlWalker.g:537:2: ( NUM_INT )
            // HqlSqlWalker.g:537:4: NUM_INT
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	_last = (IASTNode)input.LT(1);
            	NUM_INT212=(IASTNode)Match(input,NUM_INT,FOLLOW_NUM_INT_in_numericInteger2662); 
            		NUM_INT212_tree = (IASTNode)adaptor.DupNode(NUM_INT212);

            		adaptor.AddChild(root_0, NUM_INT212_tree);


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
    public static readonly BitSet FOLLOW_intoClause_in_insertStatement321 = new BitSet(new ulong[]{0x0004000000000000UL,0x0000000000100000UL});
    public static readonly BitSet FOLLOW_query_in_insertStatement323 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_INTO_in_intoClause347 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_path_in_intoClause354 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000200000UL});
    public static readonly BitSet FOLLOW_insertablePropertySpec_in_intoClause359 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_RANGE_in_insertablePropertySpec375 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_IDENT_in_insertablePropertySpec378 = new BitSet(new ulong[]{0x0000000000000008UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_SET_in_setClause395 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_assignment_in_setClause400 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000001000000000UL});
    public static readonly BitSet FOLLOW_EQ_in_assignment427 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_propertyRef_in_assignment432 = new BitSet(new ulong[]{0x0086008000109000UL,0x0F3FC007ED109120UL});
    public static readonly BitSet FOLLOW_newValue_in_assignment438 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_expr_in_newValue454 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_query_in_newValue458 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_unionedQuery_in_query469 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_UNION_in_query476 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_unionedQuery_in_query478 = new BitSet(new ulong[]{0x0004000000000000UL,0x0000000000100000UL});
    public static readonly BitSet FOLLOW_query_in_query480 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_QUERY_in_unionedQuery503 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_SELECT_FROM_in_unionedQuery515 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_fromClause_in_unionedQuery523 = new BitSet(new ulong[]{0x0000200000000008UL});
    public static readonly BitSet FOLLOW_selectClause_in_unionedQuery532 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_whereClause_in_unionedQuery547 = new BitSet(new ulong[]{0x0000020001000008UL});
    public static readonly BitSet FOLLOW_groupClause_in_unionedQuery557 = new BitSet(new ulong[]{0x0000020000000008UL});
    public static readonly BitSet FOLLOW_orderClause_in_unionedQuery567 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_ORDER_in_orderClause612 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_orderExprs_in_orderClause616 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_expr_in_orderExprs628 = new BitSet(new ulong[]{0x008200800010D102UL,0x0F3FC007ED009120UL});
    public static readonly BitSet FOLLOW_set_in_orderExprs630 = new BitSet(new ulong[]{0x0082008000109002UL,0x0F3FC007ED009120UL});
    public static readonly BitSet FOLLOW_orderExprs_in_orderExprs642 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_GROUP_in_groupClause656 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_groupClause661 = new BitSet(new ulong[]{0x0082008002109008UL,0x0F3FC007ED009120UL});
    public static readonly BitSet FOLLOW_HAVING_in_groupClause668 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_logicalExpr_in_groupClause670 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_SELECT_in_selectClause689 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_DISTINCT_in_selectClause696 = new BitSet(new ulong[]{0x0086008008129090UL,0x0F3FC007ED1091A4UL});
    public static readonly BitSet FOLLOW_selectExprList_in_selectClause702 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_selectExpr_in_selectExprList737 = new BitSet(new ulong[]{0x0086008008129092UL,0x0F3FC007ED1091A4UL});
    public static readonly BitSet FOLLOW_aliasedSelectExpr_in_selectExprList741 = new BitSet(new ulong[]{0x0086008008129092UL,0x0F3FC007ED1091A4UL});
    public static readonly BitSet FOLLOW_AS_in_aliasedSelectExpr765 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_selectExpr_in_aliasedSelectExpr769 = new BitSet(new ulong[]{0x0000000000008000UL,0x0800000008001000UL});
    public static readonly BitSet FOLLOW_identifier_in_aliasedSelectExpr773 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_propertyRef_in_selectExpr788 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ALL_in_selectExpr800 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_aliasRef_in_selectExpr804 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_OBJECT_in_selectExpr816 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_aliasRef_in_selectExpr820 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_constructor_in_selectExpr831 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_functionCall_in_selectExpr842 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_parameter_in_selectExpr847 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_count_in_selectExpr852 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_collectionFunction_in_selectExpr857 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_literal_in_selectExpr865 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_arithmeticExpr_in_selectExpr870 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_query_in_selectExpr875 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_COUNT_in_count887 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_set_in_count889 = new BitSet(new ulong[]{0x0082008008129000UL,0x0F3FC007ED409120UL});
    public static readonly BitSet FOLLOW_aggregateExpr_in_count902 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_ROW_STAR_in_count906 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_CONSTRUCTOR_in_constructor922 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_path_in_constructor924 = new BitSet(new ulong[]{0x0086008008129098UL,0x0F3FC007ED1091A4UL});
    public static readonly BitSet FOLLOW_selectExpr_in_constructor928 = new BitSet(new ulong[]{0x0086008008129098UL,0x0F3FC007ED1091A4UL});
    public static readonly BitSet FOLLOW_aliasedSelectExpr_in_constructor932 = new BitSet(new ulong[]{0x0086008008129098UL,0x0F3FC007ED1091A4UL});
    public static readonly BitSet FOLLOW_expr_in_aggregateExpr948 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_collectionFunction_in_aggregateExpr954 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_FROM_in_fromClause974 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_fromElementList_in_fromClause978 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_fromElement_in_fromElementList996 = new BitSet(new ulong[]{0x0000000100000002UL,0x0000000000200400UL});
    public static readonly BitSet FOLLOW_RANGE_in_fromElement1021 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_path_in_fromElement1025 = new BitSet(new ulong[]{0x0000000000200008UL,0x0000000000000040UL});
    public static readonly BitSet FOLLOW_ALIAS_in_fromElement1030 = new BitSet(new ulong[]{0x0000000000200008UL});
    public static readonly BitSet FOLLOW_FETCH_in_fromElement1037 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_joinElement_in_fromElement1064 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_FILTER_ENTITY_in_fromElement1079 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000040UL});
    public static readonly BitSet FOLLOW_ALIAS_in_fromElement1083 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_JOIN_in_joinElement1112 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_joinType_in_joinElement1117 = new BitSet(new ulong[]{0x0000000000208000UL,0x0800000008001000UL});
    public static readonly BitSet FOLLOW_FETCH_in_joinElement1127 = new BitSet(new ulong[]{0x0000000000008000UL,0x0800000008001000UL});
    public static readonly BitSet FOLLOW_propertyRef_in_joinElement1133 = new BitSet(new ulong[]{0x2000000000200008UL,0x0000000000000040UL});
    public static readonly BitSet FOLLOW_ALIAS_in_joinElement1138 = new BitSet(new ulong[]{0x2000000000200008UL});
    public static readonly BitSet FOLLOW_FETCH_in_joinElement1145 = new BitSet(new ulong[]{0x2000000000000008UL});
    public static readonly BitSet FOLLOW_WITH_in_joinElement1154 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_LEFT_in_joinType1195 = new BitSet(new ulong[]{0x0000040000000002UL});
    public static readonly BitSet FOLLOW_RIGHT_in_joinType1201 = new BitSet(new ulong[]{0x0000040000000002UL});
    public static readonly BitSet FOLLOW_OUTER_in_joinType1207 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_FULL_in_joinType1221 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_INNER_in_joinType1228 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_identifier_in_path1250 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_DOT_in_path1258 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_path_in_path1262 = new BitSet(new ulong[]{0x0000000000008000UL,0x0800000008001000UL});
    public static readonly BitSet FOLLOW_identifier_in_path1266 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_path_in_pathAsIdent1285 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WITH_in_withClause1326 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_logicalExpr_in_withClause1332 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_WHERE_in_whereClause1360 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_logicalExpr_in_whereClause1366 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_AND_in_logicalExpr1392 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_logicalExpr_in_logicalExpr1394 = new BitSet(new ulong[]{0x0000014404088440UL,0x08001E900807F020UL});
    public static readonly BitSet FOLLOW_logicalExpr_in_logicalExpr1396 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_OR_in_logicalExpr1403 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_logicalExpr_in_logicalExpr1405 = new BitSet(new ulong[]{0x0000014404088440UL,0x08001E900807F020UL});
    public static readonly BitSet FOLLOW_logicalExpr_in_logicalExpr1407 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NOT_in_logicalExpr1414 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_logicalExpr_in_logicalExpr1416 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_comparisonExpr_in_logicalExpr1422 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_functionCall_in_logicalExpr1427 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_logicalPath_in_logicalExpr1432 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_addrExpr_in_logicalPath1451 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_EQ_in_comparisonExpr1489 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1491 = new BitSet(new ulong[]{0x0086808000109030UL,0x0F3FC007ED109120UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1493 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NE_in_comparisonExpr1500 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1502 = new BitSet(new ulong[]{0x0086808000109030UL,0x0F3FC007ED109120UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1504 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_LT_in_comparisonExpr1511 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1513 = new BitSet(new ulong[]{0x0086808000109030UL,0x0F3FC007ED109120UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1515 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_GT_in_comparisonExpr1522 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1524 = new BitSet(new ulong[]{0x0086808000109030UL,0x0F3FC007ED109120UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1526 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_LE_in_comparisonExpr1533 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1535 = new BitSet(new ulong[]{0x0086808000109030UL,0x0F3FC007ED109120UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1537 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_GE_in_comparisonExpr1544 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1546 = new BitSet(new ulong[]{0x0086808000109030UL,0x0F3FC007ED109120UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1548 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_LIKE_in_comparisonExpr1555 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1557 = new BitSet(new ulong[]{0x0082008000109000UL,0x0F3FC007ED009120UL});
    public static readonly BitSet FOLLOW_expr_in_comparisonExpr1559 = new BitSet(new ulong[]{0x0000000000040008UL});
    public static readonly BitSet FOLLOW_ESCAPE_in_comparisonExpr1564 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_comparisonExpr1566 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NOT_LIKE_in_comparisonExpr1578 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1580 = new BitSet(new ulong[]{0x0082008000109000UL,0x0F3FC007ED009120UL});
    public static readonly BitSet FOLLOW_expr_in_comparisonExpr1582 = new BitSet(new ulong[]{0x0000000000040008UL});
    public static readonly BitSet FOLLOW_ESCAPE_in_comparisonExpr1587 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_comparisonExpr1589 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_BETWEEN_in_comparisonExpr1601 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1603 = new BitSet(new ulong[]{0x0086808000109030UL,0x0F3FC007ED109120UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1605 = new BitSet(new ulong[]{0x0086808000109030UL,0x0F3FC007ED109120UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1607 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NOT_BETWEEN_in_comparisonExpr1614 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1616 = new BitSet(new ulong[]{0x0086808000109030UL,0x0F3FC007ED109120UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1618 = new BitSet(new ulong[]{0x0086808000109030UL,0x0F3FC007ED109120UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1620 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_IN_in_comparisonExpr1627 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1629 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000800UL});
    public static readonly BitSet FOLLOW_inRhs_in_comparisonExpr1631 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NOT_IN_in_comparisonExpr1639 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1641 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000800UL});
    public static readonly BitSet FOLLOW_inRhs_in_comparisonExpr1643 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_IS_NULL_in_comparisonExpr1651 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1653 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_IS_NOT_NULL_in_comparisonExpr1660 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_comparisonExpr1662 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_EXISTS_in_comparisonExpr1671 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_comparisonExpr1675 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_collectionFunctionOrSubselect_in_comparisonExpr1679 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_IN_LIST_in_inRhs1703 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_collectionFunctionOrSubselect_in_inRhs1707 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_expr_in_inRhs1711 = new BitSet(new ulong[]{0x0082008000109008UL,0x0F3FC007ED009120UL});
    public static readonly BitSet FOLLOW_expr_in_exprOrSubquery1727 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_query_in_exprOrSubquery1732 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ANY_in_exprOrSubquery1738 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_collectionFunctionOrSubselect_in_exprOrSubquery1740 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_ALL_in_exprOrSubquery1747 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_collectionFunctionOrSubselect_in_exprOrSubquery1749 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_SOME_in_exprOrSubquery1756 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_collectionFunctionOrSubselect_in_exprOrSubquery1758 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_collectionFunction_in_collectionFunctionOrSubselect1771 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_query_in_collectionFunctionOrSubselect1776 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_addrExpr_in_expr1790 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_VECTOR_EXPR_in_expr1802 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_expr1805 = new BitSet(new ulong[]{0x0082008000109008UL,0x0F3FC007ED009120UL});
    public static readonly BitSet FOLLOW_constant_in_expr1814 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_arithmeticExpr_in_expr1819 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_functionCall_in_expr1824 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_parameter_in_expr1836 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_count_in_expr1841 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_PLUS_in_arithmeticExpr1869 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_arithmeticExpr1871 = new BitSet(new ulong[]{0x0086808000109030UL,0x0F3FC007ED109120UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_arithmeticExpr1873 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_MINUS_in_arithmeticExpr1880 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_arithmeticExpr1882 = new BitSet(new ulong[]{0x0086808000109030UL,0x0F3FC007ED109120UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_arithmeticExpr1884 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_DIV_in_arithmeticExpr1891 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_arithmeticExpr1893 = new BitSet(new ulong[]{0x0086808000109030UL,0x0F3FC007ED109120UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_arithmeticExpr1895 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_STAR_in_arithmeticExpr1902 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_arithmeticExpr1904 = new BitSet(new ulong[]{0x0086808000109030UL,0x0F3FC007ED109120UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_arithmeticExpr1906 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_BNOT_in_arithmeticExpr1913 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_arithmeticExpr1915 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_BAND_in_arithmeticExpr1922 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_arithmeticExpr1924 = new BitSet(new ulong[]{0x0086808000109030UL,0x0F3FC007ED109120UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_arithmeticExpr1926 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_BOR_in_arithmeticExpr1933 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_arithmeticExpr1935 = new BitSet(new ulong[]{0x0086808000109030UL,0x0F3FC007ED109120UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_arithmeticExpr1937 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_BXOR_in_arithmeticExpr1944 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_arithmeticExpr1946 = new BitSet(new ulong[]{0x0086808000109030UL,0x0F3FC007ED109120UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_arithmeticExpr1948 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_UNARY_MINUS_in_arithmeticExpr1956 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprOrSubquery_in_arithmeticExpr1958 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_caseExpr_in_arithmeticExpr1966 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_CASE_in_caseExpr1978 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_WHEN_in_caseExpr1984 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_logicalExpr_in_caseExpr1986 = new BitSet(new ulong[]{0x0082008000109000UL,0x0F3FC007ED009120UL});
    public static readonly BitSet FOLLOW_expr_in_caseExpr1988 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_ELSE_in_caseExpr1995 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_caseExpr1997 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_CASE2_in_caseExpr2009 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_caseExpr2013 = new BitSet(new ulong[]{0x0800000000000000UL});
    public static readonly BitSet FOLLOW_WHEN_in_caseExpr2017 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_caseExpr2019 = new BitSet(new ulong[]{0x0082008000109000UL,0x0F3FC007ED009120UL});
    public static readonly BitSet FOLLOW_expr_in_caseExpr2021 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_ELSE_in_caseExpr2028 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_caseExpr2030 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_ELEMENTS_in_collectionFunction2052 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_propertyRef_in_collectionFunction2058 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_INDICES_in_collectionFunction2077 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_propertyRef_in_collectionFunction2083 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_METHOD_CALL_in_functionCall2108 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_pathAsIdent_in_functionCall2113 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000000000200UL});
    public static readonly BitSet FOLLOW_EXPR_LIST_in_functionCall2118 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_expr_in_functionCall2121 = new BitSet(new ulong[]{0x0086008404189408UL,0x0F3FDE97ED17F120UL});
    public static readonly BitSet FOLLOW_query_in_functionCall2125 = new BitSet(new ulong[]{0x0086008404189408UL,0x0F3FDE97ED17F120UL});
    public static readonly BitSet FOLLOW_comparisonExpr_in_functionCall2129 = new BitSet(new ulong[]{0x0086008404189408UL,0x0F3FDE97ED17F120UL});
    public static readonly BitSet FOLLOW_AGGREGATE_in_functionCall2148 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_aggregateExpr_in_functionCall2150 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_literal_in_constant2163 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_NULL_in_constant2168 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_TRUE_in_constant2175 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_FALSE_in_constant2185 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_JAVA_CONSTANT_in_constant2192 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_numericLiteral_in_literal2203 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_stringLiteral_in_literal2208 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_set_in_numericLiteral0 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_QUOTED_String_in_stringLiteral2255 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_set_in_identifier2266 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_addrExprDot_in_addrExpr2285 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_addrExprIndex_in_addrExpr2292 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_addrExprIdent_in_addrExpr2299 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_DOT_in_addrExprDot2323 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_addrExprLhs_in_addrExprDot2327 = new BitSet(new ulong[]{0x0000000008028800UL,0x0800000008001000UL});
    public static readonly BitSet FOLLOW_propertyName_in_addrExprDot2331 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_INDEX_OP_in_addrExprIndex2370 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_addrExprLhs_in_addrExprIndex2374 = new BitSet(new ulong[]{0x0082008000109000UL,0x0F3FC007ED009120UL});
    public static readonly BitSet FOLLOW_expr_in_addrExprIndex2378 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_identifier_in_addrExprIdent2410 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_addrExpr_in_addrExprLhs2438 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_identifier_in_propertyName2451 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_CLASS_in_propertyName2456 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ELEMENTS_in_propertyName2461 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_INDICES_in_propertyName2466 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_propertyRefPath_in_propertyRef2478 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_propertyRefIdent_in_propertyRef2483 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_DOT_in_propertyRefPath2503 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_propertyRefLhs_in_propertyRefPath2507 = new BitSet(new ulong[]{0x0000000008028800UL,0x0800000008001000UL});
    public static readonly BitSet FOLLOW_propertyName_in_propertyRefPath2511 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_identifier_in_propertyRefIdent2548 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_propertyRef_in_propertyRefLhs2560 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_identifier_in_aliasRef2581 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_COLON_in_parameter2599 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_identifier_in_parameter2603 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_PARAM_in_parameter2624 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_NUM_INT_in_parameter2629 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NUM_INT_in_numericInteger2662 = new BitSet(new ulong[]{0x0000000000000002UL});

}
}