// $ANTLR 3.2 Sep 23, 2009 12:02:23 Hql.g 2011-04-11 10:19:39

// The variable 'variable' is assigned but its value is never used.
#pragma warning disable 168, 219
// Unreachable code detected.
#pragma warning disable 162


using NHibernate.Hql.Ast.ANTLR.Tree;


using System;
using Antlr.Runtime;
using IList 		= System.Collections.IList;
using ArrayList 	= System.Collections.ArrayList;
using Stack 		= Antlr.Runtime.Collections.StackList;



using Antlr.Runtime.Tree;

namespace  NHibernate.Hql.Ast.ANTLR 
{
public partial class HqlParser : Parser
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
		"'descending'"
    };

    public const int EXPONENT = 130;
    public const int LT = 107;
    public const int FLOAT_SUFFIX = 131;
    public const int STAR = 118;
    public const int LITERAL_by = 56;
    public const int CASE = 57;
    public const int NEW = 37;
    public const int FILTER_ENTITY = 76;
    public const int PARAM = 123;
    public const int COUNT = 12;
    public const int NOT = 38;
    public const int EOF = -1;
    public const int UNARY_PLUS = 91;
    public const int QUOTED_String = 124;
    public const int ESCqs = 128;
    public const int WEIRD_IDENT = 93;
    public const int OPEN_BRACKET = 120;
    public const int FULL = 23;
    public const int ORDER_ELEMENT = 85;
    public const int INSERT = 29;
    public const int ESCAPE = 18;
    public const int IS_NULL = 80;
    public const int BOTH = 64;
    public const int NUM_DECIMAL = 97;
    public const int EQ = 102;
    public const int VERSIONED = 54;
    public const int SELECT = 45;
    public const int INTO = 30;
    public const int NE = 105;
    public const int GE = 110;
    public const int TAKE = 50;
    public const int ID_LETTER = 127;
    public const int CONCAT = 111;
    public const int NULL = 39;
    public const int ELSE = 59;
    public const int SELECT_FROM = 89;
    public const int NUM_LONG = 99;
    public const int ON = 62;
    public const int TRAILING = 70;
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
    public const int GT = 108;
    public const int QUERY = 86;
    public const int BNOT = 112;
    public const int INDEX_OP = 78;
    public const int NUM_FLOAT = 98;
    public const int FROM = 22;
    public const int END = 58;
    public const int FALSE = 20;
    public const int DISTINCT = 16;
    public const int CONSTRUCTOR = 73;
    public const int T__133 = 133;
    public const int T__134 = 134;
    public const int CLOSE_BRACKET = 121;
    public const int WHERE = 55;
    public const int CLASS = 11;
    public const int MEMBER = 67;
    public const int INNER = 28;
    public const int PROPERTIES = 43;
    public const int ORDER = 41;
    public const int MAX = 35;
    public const int UPDATE = 53;
    public const int SQL_NE = 106;
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
    public const int LEFT = 33;
    public const int AVG = 9;
    public const int SOME = 48;
    public const int BOR = 113;
    public const int ALL = 4;
    public const int IDENT = 125;
    public const int PLUS = 116;
    public const int BXOR = 114;
    public const int CASE2 = 74;
    public const int EXISTS = 19;
    public const int DOT = 15;
    public const int LIKE = 34;
    public const int WITH = 63;
    public const int OUTER = 42;
    public const int ID_START_LETTER = 126;
    public const int ROW_STAR = 88;
    public const int NOT_LIKE = 84;
    public const int HEX_DIGIT = 132;
    public const int NOT_BETWEEN = 82;
    public const int RANGE = 87;
    public const int RIGHT = 44;
    public const int SET = 46;
    public const int HAVING = 25;
    public const int MIN = 36;
    public const int MINUS = 117;
    public const int IS_NOT_NULL = 79;
    public const int BAND = 115;
    public const int ELEMENTS = 17;
    public const int TRUE = 51;
    public const int JOIN = 32;
    public const int UNION = 52;
    public const int IN_LIST = 77;
    public const int COLON = 122;
    public const int OPEN = 103;
    public const int ANY = 5;
    public const int CLOSE = 104;
    public const int WHEN = 61;
    public const int DIV = 119;
    public const int DESCENDING = 14;
    public const int BETWEEN = 10;
    public const int AGGREGATE = 71;
    public const int LE = 109;

    // delegates
    // delegators



        public HqlParser(ITokenStream input)
    		: this(input, new RecognizerSharedState()) {
        }

        public HqlParser(ITokenStream input, RecognizerSharedState state)
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
		get { return HqlParser.tokenNames; }
    }

    override public string GrammarFileName {
		get { return "Hql.g"; }
    }


    public class statement_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "statement"
    // Hql.g:129:1: statement : ( updateStatement | deleteStatement | selectStatement | insertStatement ) EOF ;
    public HqlParser.statement_return statement() // throws RecognitionException [1]
    {   
        HqlParser.statement_return retval = new HqlParser.statement_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken EOF5 = null;
        HqlParser.updateStatement_return updateStatement1 = default(HqlParser.updateStatement_return);

        HqlParser.deleteStatement_return deleteStatement2 = default(HqlParser.deleteStatement_return);

        HqlParser.selectStatement_return selectStatement3 = default(HqlParser.selectStatement_return);

        HqlParser.insertStatement_return insertStatement4 = default(HqlParser.insertStatement_return);


        IASTNode EOF5_tree=null;

        try 
    	{
            // Hql.g:130:2: ( ( updateStatement | deleteStatement | selectStatement | insertStatement ) EOF )
            // Hql.g:130:4: ( updateStatement | deleteStatement | selectStatement | insertStatement ) EOF
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// Hql.g:130:4: ( updateStatement | deleteStatement | selectStatement | insertStatement )
            	int alt1 = 4;
            	switch ( input.LA(1) ) 
            	{
            	case UPDATE:
            		{
            	    alt1 = 1;
            	    }
            	    break;
            	case DELETE:
            		{
            	    alt1 = 2;
            	    }
            	    break;
            	case EOF:
            	case FROM:
            	case GROUP:
            	case HAVING:
            	case ORDER:
            	case SELECT:
            	case SKIP:
            	case TAKE:
            	case UNION:
            	case WHERE:
            	case CLOSE:
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
            	        // Hql.g:130:6: updateStatement
            	        {
            	        	PushFollow(FOLLOW_updateStatement_in_statement611);
            	        	updateStatement1 = updateStatement();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, updateStatement1.Tree);

            	        }
            	        break;
            	    case 2 :
            	        // Hql.g:130:24: deleteStatement
            	        {
            	        	PushFollow(FOLLOW_deleteStatement_in_statement615);
            	        	deleteStatement2 = deleteStatement();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, deleteStatement2.Tree);

            	        }
            	        break;
            	    case 3 :
            	        // Hql.g:130:42: selectStatement
            	        {
            	        	PushFollow(FOLLOW_selectStatement_in_statement619);
            	        	selectStatement3 = selectStatement();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, selectStatement3.Tree);

            	        }
            	        break;
            	    case 4 :
            	        // Hql.g:130:60: insertStatement
            	        {
            	        	PushFollow(FOLLOW_insertStatement_in_statement623);
            	        	insertStatement4 = insertStatement();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, insertStatement4.Tree);

            	        }
            	        break;

            	}

            	EOF5=(IToken)Match(input,EOF,FOLLOW_EOF_in_statement627); 

            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "statement"

    public class updateStatement_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "updateStatement"
    // Hql.g:133:1: updateStatement : UPDATE ( VERSIONED )? optionalFromTokenFromClause setClause ( whereClause )? ;
    public HqlParser.updateStatement_return updateStatement() // throws RecognitionException [1]
    {   
        HqlParser.updateStatement_return retval = new HqlParser.updateStatement_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken UPDATE6 = null;
        IToken VERSIONED7 = null;
        HqlParser.optionalFromTokenFromClause_return optionalFromTokenFromClause8 = default(HqlParser.optionalFromTokenFromClause_return);

        HqlParser.setClause_return setClause9 = default(HqlParser.setClause_return);

        HqlParser.whereClause_return whereClause10 = default(HqlParser.whereClause_return);


        IASTNode UPDATE6_tree=null;
        IASTNode VERSIONED7_tree=null;

        try 
    	{
            // Hql.g:134:2: ( UPDATE ( VERSIONED )? optionalFromTokenFromClause setClause ( whereClause )? )
            // Hql.g:134:4: UPDATE ( VERSIONED )? optionalFromTokenFromClause setClause ( whereClause )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	UPDATE6=(IToken)Match(input,UPDATE,FOLLOW_UPDATE_in_updateStatement639); 
            		UPDATE6_tree = (IASTNode)adaptor.Create(UPDATE6);
            		root_0 = (IASTNode)adaptor.BecomeRoot(UPDATE6_tree, root_0);

            	// Hql.g:134:12: ( VERSIONED )?
            	int alt2 = 2;
            	int LA2_0 = input.LA(1);

            	if ( (LA2_0 == VERSIONED) )
            	{
            	    alt2 = 1;
            	}
            	switch (alt2) 
            	{
            	    case 1 :
            	        // Hql.g:134:13: VERSIONED
            	        {
            	        	VERSIONED7=(IToken)Match(input,VERSIONED,FOLLOW_VERSIONED_in_updateStatement643); 
            	        		VERSIONED7_tree = (IASTNode)adaptor.Create(VERSIONED7);
            	        		adaptor.AddChild(root_0, VERSIONED7_tree);


            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_optionalFromTokenFromClause_in_updateStatement649);
            	optionalFromTokenFromClause8 = optionalFromTokenFromClause();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, optionalFromTokenFromClause8.Tree);
            	PushFollow(FOLLOW_setClause_in_updateStatement653);
            	setClause9 = setClause();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, setClause9.Tree);
            	// Hql.g:137:3: ( whereClause )?
            	int alt3 = 2;
            	int LA3_0 = input.LA(1);

            	if ( (LA3_0 == WHERE) )
            	{
            	    alt3 = 1;
            	}
            	switch (alt3) 
            	{
            	    case 1 :
            	        // Hql.g:137:4: whereClause
            	        {
            	        	PushFollow(FOLLOW_whereClause_in_updateStatement658);
            	        	whereClause10 = whereClause();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, whereClause10.Tree);

            	        }
            	        break;

            	}


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "updateStatement"

    public class setClause_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "setClause"
    // Hql.g:140:1: setClause : ( SET assignment ( COMMA assignment )* ) ;
    public HqlParser.setClause_return setClause() // throws RecognitionException [1]
    {   
        HqlParser.setClause_return retval = new HqlParser.setClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken SET11 = null;
        IToken COMMA13 = null;
        HqlParser.assignment_return assignment12 = default(HqlParser.assignment_return);

        HqlParser.assignment_return assignment14 = default(HqlParser.assignment_return);


        IASTNode SET11_tree=null;
        IASTNode COMMA13_tree=null;

        try 
    	{
            // Hql.g:141:2: ( ( SET assignment ( COMMA assignment )* ) )
            // Hql.g:141:4: ( SET assignment ( COMMA assignment )* )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// Hql.g:141:4: ( SET assignment ( COMMA assignment )* )
            	// Hql.g:141:5: SET assignment ( COMMA assignment )*
            	{
            		SET11=(IToken)Match(input,SET,FOLLOW_SET_in_setClause672); 
            			SET11_tree = (IASTNode)adaptor.Create(SET11);
            			root_0 = (IASTNode)adaptor.BecomeRoot(SET11_tree, root_0);

            		PushFollow(FOLLOW_assignment_in_setClause675);
            		assignment12 = assignment();
            		state.followingStackPointer--;

            		adaptor.AddChild(root_0, assignment12.Tree);
            		// Hql.g:141:21: ( COMMA assignment )*
            		do 
            		{
            		    int alt4 = 2;
            		    int LA4_0 = input.LA(1);

            		    if ( (LA4_0 == COMMA) )
            		    {
            		        alt4 = 1;
            		    }


            		    switch (alt4) 
            			{
            				case 1 :
            				    // Hql.g:141:22: COMMA assignment
            				    {
            				    	COMMA13=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_setClause678); 
            				    	PushFollow(FOLLOW_assignment_in_setClause681);
            				    	assignment14 = assignment();
            				    	state.followingStackPointer--;

            				    	adaptor.AddChild(root_0, assignment14.Tree);

            				    }
            				    break;

            				default:
            				    goto loop4;
            		    }
            		} while (true);

            		loop4:
            			;	// Stops C# compiler whining that label 'loop4' has no statements


            	}


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "setClause"

    public class assignment_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "assignment"
    // Hql.g:144:1: assignment : stateField EQ newValue ;
    public HqlParser.assignment_return assignment() // throws RecognitionException [1]
    {   
        HqlParser.assignment_return retval = new HqlParser.assignment_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken EQ16 = null;
        HqlParser.stateField_return stateField15 = default(HqlParser.stateField_return);

        HqlParser.newValue_return newValue17 = default(HqlParser.newValue_return);


        IASTNode EQ16_tree=null;

        try 
    	{
            // Hql.g:145:2: ( stateField EQ newValue )
            // Hql.g:145:4: stateField EQ newValue
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_stateField_in_assignment695);
            	stateField15 = stateField();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, stateField15.Tree);
            	EQ16=(IToken)Match(input,EQ,FOLLOW_EQ_in_assignment697); 
            		EQ16_tree = (IASTNode)adaptor.Create(EQ16);
            		root_0 = (IASTNode)adaptor.BecomeRoot(EQ16_tree, root_0);

            	PushFollow(FOLLOW_newValue_in_assignment700);
            	newValue17 = newValue();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, newValue17.Tree);

            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "assignment"

    public class stateField_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "stateField"
    // Hql.g:150:1: stateField : path ;
    public HqlParser.stateField_return stateField() // throws RecognitionException [1]
    {   
        HqlParser.stateField_return retval = new HqlParser.stateField_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.path_return path18 = default(HqlParser.path_return);



        try 
    	{
            // Hql.g:151:2: ( path )
            // Hql.g:151:4: path
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_path_in_stateField713);
            	path18 = path();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, path18.Tree);

            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "stateField"

    public class newValue_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "newValue"
    // Hql.g:156:1: newValue : concatenation ;
    public HqlParser.newValue_return newValue() // throws RecognitionException [1]
    {   
        HqlParser.newValue_return retval = new HqlParser.newValue_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.concatenation_return concatenation19 = default(HqlParser.concatenation_return);



        try 
    	{
            // Hql.g:157:2: ( concatenation )
            // Hql.g:157:4: concatenation
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_concatenation_in_newValue726);
            	concatenation19 = concatenation();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, concatenation19.Tree);

            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "newValue"

    public class deleteStatement_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "deleteStatement"
    // Hql.g:160:1: deleteStatement : DELETE ( optionalFromTokenFromClause ) ( whereClause )? ;
    public HqlParser.deleteStatement_return deleteStatement() // throws RecognitionException [1]
    {   
        HqlParser.deleteStatement_return retval = new HqlParser.deleteStatement_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken DELETE20 = null;
        HqlParser.optionalFromTokenFromClause_return optionalFromTokenFromClause21 = default(HqlParser.optionalFromTokenFromClause_return);

        HqlParser.whereClause_return whereClause22 = default(HqlParser.whereClause_return);


        IASTNode DELETE20_tree=null;

        try 
    	{
            // Hql.g:161:2: ( DELETE ( optionalFromTokenFromClause ) ( whereClause )? )
            // Hql.g:161:4: DELETE ( optionalFromTokenFromClause ) ( whereClause )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	DELETE20=(IToken)Match(input,DELETE,FOLLOW_DELETE_in_deleteStatement737); 
            		DELETE20_tree = (IASTNode)adaptor.Create(DELETE20);
            		root_0 = (IASTNode)adaptor.BecomeRoot(DELETE20_tree, root_0);

            	// Hql.g:162:3: ( optionalFromTokenFromClause )
            	// Hql.g:162:4: optionalFromTokenFromClause
            	{
            		PushFollow(FOLLOW_optionalFromTokenFromClause_in_deleteStatement743);
            		optionalFromTokenFromClause21 = optionalFromTokenFromClause();
            		state.followingStackPointer--;

            		adaptor.AddChild(root_0, optionalFromTokenFromClause21.Tree);

            	}

            	// Hql.g:163:3: ( whereClause )?
            	int alt5 = 2;
            	int LA5_0 = input.LA(1);

            	if ( (LA5_0 == WHERE) )
            	{
            	    alt5 = 1;
            	}
            	switch (alt5) 
            	{
            	    case 1 :
            	        // Hql.g:163:4: whereClause
            	        {
            	        	PushFollow(FOLLOW_whereClause_in_deleteStatement749);
            	        	whereClause22 = whereClause();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, whereClause22.Tree);

            	        }
            	        break;

            	}


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "deleteStatement"

    public class optionalFromTokenFromClause_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "optionalFromTokenFromClause"
    // Hql.g:168:1: optionalFromTokenFromClause : optionalFromTokenFromClause2 path ( asAlias )? -> ^( FROM ^( RANGE path ( asAlias )? ) ) ;
    public HqlParser.optionalFromTokenFromClause_return optionalFromTokenFromClause() // throws RecognitionException [1]
    {   
        HqlParser.optionalFromTokenFromClause_return retval = new HqlParser.optionalFromTokenFromClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.optionalFromTokenFromClause2_return optionalFromTokenFromClause223 = default(HqlParser.optionalFromTokenFromClause2_return);

        HqlParser.path_return path24 = default(HqlParser.path_return);

        HqlParser.asAlias_return asAlias25 = default(HqlParser.asAlias_return);


        RewriteRuleSubtreeStream stream_optionalFromTokenFromClause2 = new RewriteRuleSubtreeStream(adaptor,"rule optionalFromTokenFromClause2");
        RewriteRuleSubtreeStream stream_path = new RewriteRuleSubtreeStream(adaptor,"rule path");
        RewriteRuleSubtreeStream stream_asAlias = new RewriteRuleSubtreeStream(adaptor,"rule asAlias");
        try 
    	{
            // Hql.g:169:2: ( optionalFromTokenFromClause2 path ( asAlias )? -> ^( FROM ^( RANGE path ( asAlias )? ) ) )
            // Hql.g:169:4: optionalFromTokenFromClause2 path ( asAlias )?
            {
            	PushFollow(FOLLOW_optionalFromTokenFromClause2_in_optionalFromTokenFromClause764);
            	optionalFromTokenFromClause223 = optionalFromTokenFromClause2();
            	state.followingStackPointer--;

            	stream_optionalFromTokenFromClause2.Add(optionalFromTokenFromClause223.Tree);
            	PushFollow(FOLLOW_path_in_optionalFromTokenFromClause766);
            	path24 = path();
            	state.followingStackPointer--;

            	stream_path.Add(path24.Tree);
            	// Hql.g:169:38: ( asAlias )?
            	int alt6 = 2;
            	int LA6_0 = input.LA(1);

            	if ( (LA6_0 == AS || LA6_0 == IDENT) )
            	{
            	    alt6 = 1;
            	}
            	switch (alt6) 
            	{
            	    case 1 :
            	        // Hql.g:169:39: asAlias
            	        {
            	        	PushFollow(FOLLOW_asAlias_in_optionalFromTokenFromClause769);
            	        	asAlias25 = asAlias();
            	        	state.followingStackPointer--;

            	        	stream_asAlias.Add(asAlias25.Tree);

            	        }
            	        break;

            	}



            	// AST REWRITE
            	// elements:          path, asAlias
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (IASTNode)adaptor.GetNilNode();
            	// 170:3: -> ^( FROM ^( RANGE path ( asAlias )? ) )
            	{
            	    // Hql.g:170:6: ^( FROM ^( RANGE path ( asAlias )? ) )
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(FROM, "FROM"), root_1);

            	    // Hql.g:170:13: ^( RANGE path ( asAlias )? )
            	    {
            	    IASTNode root_2 = (IASTNode)adaptor.GetNilNode();
            	    root_2 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(RANGE, "RANGE"), root_2);

            	    adaptor.AddChild(root_2, stream_path.NextTree());
            	    // Hql.g:170:26: ( asAlias )?
            	    if ( stream_asAlias.HasNext() )
            	    {
            	        adaptor.AddChild(root_2, stream_asAlias.NextTree());

            	    }
            	    stream_asAlias.Reset();

            	    adaptor.AddChild(root_1, root_2);
            	    }

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;
            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "optionalFromTokenFromClause"

    public class optionalFromTokenFromClause2_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "optionalFromTokenFromClause2"
    // Hql.g:173:1: optionalFromTokenFromClause2 : ( FROM )? ;
    public HqlParser.optionalFromTokenFromClause2_return optionalFromTokenFromClause2() // throws RecognitionException [1]
    {   
        HqlParser.optionalFromTokenFromClause2_return retval = new HqlParser.optionalFromTokenFromClause2_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken FROM26 = null;

        IASTNode FROM26_tree=null;

        try 
    	{
            // Hql.g:174:2: ( ( FROM )? )
            // Hql.g:174:4: ( FROM )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// Hql.g:174:4: ( FROM )?
            	int alt7 = 2;
            	int LA7_0 = input.LA(1);

            	if ( (LA7_0 == FROM) )
            	{
            	    alt7 = 1;
            	}
            	switch (alt7) 
            	{
            	    case 1 :
            	        // Hql.g:174:4: FROM
            	        {
            	        	FROM26=(IToken)Match(input,FROM,FOLLOW_FROM_in_optionalFromTokenFromClause2800); 
            	        		FROM26_tree = (IASTNode)adaptor.Create(FROM26);
            	        		adaptor.AddChild(root_0, FROM26_tree);


            	        }
            	        break;

            	}


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "optionalFromTokenFromClause2"

    public class selectStatement_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "selectStatement"
    // Hql.g:177:1: selectStatement : q= queryRule -> ^( QUERY[\"query\"] $q) ;
    public HqlParser.selectStatement_return selectStatement() // throws RecognitionException [1]
    {   
        HqlParser.selectStatement_return retval = new HqlParser.selectStatement_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.queryRule_return q = default(HqlParser.queryRule_return);


        RewriteRuleSubtreeStream stream_queryRule = new RewriteRuleSubtreeStream(adaptor,"rule queryRule");
        try 
    	{
            // Hql.g:178:2: (q= queryRule -> ^( QUERY[\"query\"] $q) )
            // Hql.g:178:4: q= queryRule
            {
            	PushFollow(FOLLOW_queryRule_in_selectStatement814);
            	q = queryRule();
            	state.followingStackPointer--;

            	stream_queryRule.Add(q.Tree);


            	// AST REWRITE
            	// elements:          q
            	// token labels:      
            	// rule labels:       retval, q
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);
            	RewriteRuleSubtreeStream stream_q = new RewriteRuleSubtreeStream(adaptor, "rule q", q!=null ? q.Tree : null);

            	root_0 = (IASTNode)adaptor.GetNilNode();
            	// 179:2: -> ^( QUERY[\"query\"] $q)
            	{
            	    // Hql.g:179:5: ^( QUERY[\"query\"] $q)
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(QUERY, "query"), root_1);

            	    adaptor.AddChild(root_1, stream_q.NextTree());

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;
            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "selectStatement"

    public class insertStatement_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "insertStatement"
    // Hql.g:182:1: insertStatement : INSERT intoClause selectStatement ;
    public HqlParser.insertStatement_return insertStatement() // throws RecognitionException [1]
    {   
        HqlParser.insertStatement_return retval = new HqlParser.insertStatement_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken INSERT27 = null;
        HqlParser.intoClause_return intoClause28 = default(HqlParser.intoClause_return);

        HqlParser.selectStatement_return selectStatement29 = default(HqlParser.selectStatement_return);


        IASTNode INSERT27_tree=null;

        try 
    	{
            // Hql.g:186:2: ( INSERT intoClause selectStatement )
            // Hql.g:186:4: INSERT intoClause selectStatement
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	INSERT27=(IToken)Match(input,INSERT,FOLLOW_INSERT_in_insertStatement843); 
            		INSERT27_tree = (IASTNode)adaptor.Create(INSERT27);
            		root_0 = (IASTNode)adaptor.BecomeRoot(INSERT27_tree, root_0);

            	PushFollow(FOLLOW_intoClause_in_insertStatement846);
            	intoClause28 = intoClause();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, intoClause28.Tree);
            	PushFollow(FOLLOW_selectStatement_in_insertStatement848);
            	selectStatement29 = selectStatement();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, selectStatement29.Tree);

            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "insertStatement"

    public class intoClause_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "intoClause"
    // Hql.g:189:1: intoClause : INTO path insertablePropertySpec ;
    public HqlParser.intoClause_return intoClause() // throws RecognitionException [1]
    {   
        HqlParser.intoClause_return retval = new HqlParser.intoClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken INTO30 = null;
        HqlParser.path_return path31 = default(HqlParser.path_return);

        HqlParser.insertablePropertySpec_return insertablePropertySpec32 = default(HqlParser.insertablePropertySpec_return);


        IASTNode INTO30_tree=null;

        try 
    	{
            // Hql.g:190:2: ( INTO path insertablePropertySpec )
            // Hql.g:190:4: INTO path insertablePropertySpec
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	INTO30=(IToken)Match(input,INTO,FOLLOW_INTO_in_intoClause859); 
            		INTO30_tree = (IASTNode)adaptor.Create(INTO30);
            		root_0 = (IASTNode)adaptor.BecomeRoot(INTO30_tree, root_0);

            	PushFollow(FOLLOW_path_in_intoClause862);
            	path31 = path();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, path31.Tree);
            	 WeakKeywords(); 
            	PushFollow(FOLLOW_insertablePropertySpec_in_intoClause866);
            	insertablePropertySpec32 = insertablePropertySpec();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, insertablePropertySpec32.Tree);

            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "intoClause"

    public class insertablePropertySpec_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "insertablePropertySpec"
    // Hql.g:193:1: insertablePropertySpec : OPEN primaryExpression ( COMMA primaryExpression )* CLOSE -> ^( RANGE[\"column-spec\"] ( primaryExpression )* ) ;
    public HqlParser.insertablePropertySpec_return insertablePropertySpec() // throws RecognitionException [1]
    {   
        HqlParser.insertablePropertySpec_return retval = new HqlParser.insertablePropertySpec_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken OPEN33 = null;
        IToken COMMA35 = null;
        IToken CLOSE37 = null;
        HqlParser.primaryExpression_return primaryExpression34 = default(HqlParser.primaryExpression_return);

        HqlParser.primaryExpression_return primaryExpression36 = default(HqlParser.primaryExpression_return);


        IASTNode OPEN33_tree=null;
        IASTNode COMMA35_tree=null;
        IASTNode CLOSE37_tree=null;
        RewriteRuleTokenStream stream_OPEN = new RewriteRuleTokenStream(adaptor,"token OPEN");
        RewriteRuleTokenStream stream_CLOSE = new RewriteRuleTokenStream(adaptor,"token CLOSE");
        RewriteRuleTokenStream stream_COMMA = new RewriteRuleTokenStream(adaptor,"token COMMA");
        RewriteRuleSubtreeStream stream_primaryExpression = new RewriteRuleSubtreeStream(adaptor,"rule primaryExpression");
        try 
    	{
            // Hql.g:194:2: ( OPEN primaryExpression ( COMMA primaryExpression )* CLOSE -> ^( RANGE[\"column-spec\"] ( primaryExpression )* ) )
            // Hql.g:194:4: OPEN primaryExpression ( COMMA primaryExpression )* CLOSE
            {
            	OPEN33=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_insertablePropertySpec877);  
            	stream_OPEN.Add(OPEN33);

            	PushFollow(FOLLOW_primaryExpression_in_insertablePropertySpec879);
            	primaryExpression34 = primaryExpression();
            	state.followingStackPointer--;

            	stream_primaryExpression.Add(primaryExpression34.Tree);
            	// Hql.g:194:27: ( COMMA primaryExpression )*
            	do 
            	{
            	    int alt8 = 2;
            	    int LA8_0 = input.LA(1);

            	    if ( (LA8_0 == COMMA) )
            	    {
            	        alt8 = 1;
            	    }


            	    switch (alt8) 
            		{
            			case 1 :
            			    // Hql.g:194:29: COMMA primaryExpression
            			    {
            			    	COMMA35=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_insertablePropertySpec883);  
            			    	stream_COMMA.Add(COMMA35);

            			    	PushFollow(FOLLOW_primaryExpression_in_insertablePropertySpec885);
            			    	primaryExpression36 = primaryExpression();
            			    	state.followingStackPointer--;

            			    	stream_primaryExpression.Add(primaryExpression36.Tree);

            			    }
            			    break;

            			default:
            			    goto loop8;
            	    }
            	} while (true);

            	loop8:
            		;	// Stops C# compiler whining that label 'loop8' has no statements

            	CLOSE37=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_insertablePropertySpec890);  
            	stream_CLOSE.Add(CLOSE37);



            	// AST REWRITE
            	// elements:          primaryExpression
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (IASTNode)adaptor.GetNilNode();
            	// 195:3: -> ^( RANGE[\"column-spec\"] ( primaryExpression )* )
            	{
            	    // Hql.g:195:6: ^( RANGE[\"column-spec\"] ( primaryExpression )* )
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(RANGE, "column-spec"), root_1);

            	    // Hql.g:195:29: ( primaryExpression )*
            	    while ( stream_primaryExpression.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_primaryExpression.NextTree());

            	    }
            	    stream_primaryExpression.Reset();

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;
            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "insertablePropertySpec"

    public class queryRule_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "queryRule"
    // Hql.g:201:1: queryRule : selectFrom ( whereClause )? ( groupByClause )? ( havingClause )? ( orderByClause )? ( skipClause )? ( takeClause )? ;
    public HqlParser.queryRule_return queryRule() // throws RecognitionException [1]
    {   
        HqlParser.queryRule_return retval = new HqlParser.queryRule_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.selectFrom_return selectFrom38 = default(HqlParser.selectFrom_return);

        HqlParser.whereClause_return whereClause39 = default(HqlParser.whereClause_return);

        HqlParser.groupByClause_return groupByClause40 = default(HqlParser.groupByClause_return);

        HqlParser.havingClause_return havingClause41 = default(HqlParser.havingClause_return);

        HqlParser.orderByClause_return orderByClause42 = default(HqlParser.orderByClause_return);

        HqlParser.skipClause_return skipClause43 = default(HqlParser.skipClause_return);

        HqlParser.takeClause_return takeClause44 = default(HqlParser.takeClause_return);



        try 
    	{
            // Hql.g:202:2: ( selectFrom ( whereClause )? ( groupByClause )? ( havingClause )? ( orderByClause )? ( skipClause )? ( takeClause )? )
            // Hql.g:202:4: selectFrom ( whereClause )? ( groupByClause )? ( havingClause )? ( orderByClause )? ( skipClause )? ( takeClause )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_selectFrom_in_queryRule916);
            	selectFrom38 = selectFrom();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, selectFrom38.Tree);
            	// Hql.g:203:3: ( whereClause )?
            	int alt9 = 2;
            	int LA9_0 = input.LA(1);

            	if ( (LA9_0 == WHERE) )
            	{
            	    alt9 = 1;
            	}
            	switch (alt9) 
            	{
            	    case 1 :
            	        // Hql.g:203:4: whereClause
            	        {
            	        	PushFollow(FOLLOW_whereClause_in_queryRule921);
            	        	whereClause39 = whereClause();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, whereClause39.Tree);

            	        }
            	        break;

            	}

            	// Hql.g:204:3: ( groupByClause )?
            	int alt10 = 2;
            	int LA10_0 = input.LA(1);

            	if ( (LA10_0 == GROUP) )
            	{
            	    alt10 = 1;
            	}
            	switch (alt10) 
            	{
            	    case 1 :
            	        // Hql.g:204:4: groupByClause
            	        {
            	        	PushFollow(FOLLOW_groupByClause_in_queryRule928);
            	        	groupByClause40 = groupByClause();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, groupByClause40.Tree);

            	        }
            	        break;

            	}

            	// Hql.g:205:3: ( havingClause )?
            	int alt11 = 2;
            	int LA11_0 = input.LA(1);

            	if ( (LA11_0 == HAVING) )
            	{
            	    alt11 = 1;
            	}
            	switch (alt11) 
            	{
            	    case 1 :
            	        // Hql.g:205:4: havingClause
            	        {
            	        	PushFollow(FOLLOW_havingClause_in_queryRule935);
            	        	havingClause41 = havingClause();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, havingClause41.Tree);

            	        }
            	        break;

            	}

            	// Hql.g:206:3: ( orderByClause )?
            	int alt12 = 2;
            	int LA12_0 = input.LA(1);

            	if ( (LA12_0 == ORDER) )
            	{
            	    alt12 = 1;
            	}
            	switch (alt12) 
            	{
            	    case 1 :
            	        // Hql.g:206:4: orderByClause
            	        {
            	        	PushFollow(FOLLOW_orderByClause_in_queryRule942);
            	        	orderByClause42 = orderByClause();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, orderByClause42.Tree);

            	        }
            	        break;

            	}

            	// Hql.g:207:3: ( skipClause )?
            	int alt13 = 2;
            	int LA13_0 = input.LA(1);

            	if ( (LA13_0 == SKIP) )
            	{
            	    alt13 = 1;
            	}
            	switch (alt13) 
            	{
            	    case 1 :
            	        // Hql.g:207:4: skipClause
            	        {
            	        	PushFollow(FOLLOW_skipClause_in_queryRule949);
            	        	skipClause43 = skipClause();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, skipClause43.Tree);

            	        }
            	        break;

            	}

            	// Hql.g:208:3: ( takeClause )?
            	int alt14 = 2;
            	int LA14_0 = input.LA(1);

            	if ( (LA14_0 == TAKE) )
            	{
            	    alt14 = 1;
            	}
            	switch (alt14) 
            	{
            	    case 1 :
            	        // Hql.g:208:4: takeClause
            	        {
            	        	PushFollow(FOLLOW_takeClause_in_queryRule956);
            	        	takeClause44 = takeClause();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, takeClause44.Tree);

            	        }
            	        break;

            	}


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "queryRule"

    public class selectFrom_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "selectFrom"
    // Hql.g:211:1: selectFrom : (s= selectClause )? (f= fromClause )? -> {$f.tree == null && filter}? ^( SELECT_FROM FROM[\"{filter-implied FROM}\"] ( selectClause )? ) -> ^( SELECT_FROM ( fromClause )? ( selectClause )? ) ;
    public HqlParser.selectFrom_return selectFrom() // throws RecognitionException [1]
    {   
        HqlParser.selectFrom_return retval = new HqlParser.selectFrom_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.selectClause_return s = default(HqlParser.selectClause_return);

        HqlParser.fromClause_return f = default(HqlParser.fromClause_return);


        RewriteRuleSubtreeStream stream_selectClause = new RewriteRuleSubtreeStream(adaptor,"rule selectClause");
        RewriteRuleSubtreeStream stream_fromClause = new RewriteRuleSubtreeStream(adaptor,"rule fromClause");
        try 
    	{
            // Hql.g:212:2: ( (s= selectClause )? (f= fromClause )? -> {$f.tree == null && filter}? ^( SELECT_FROM FROM[\"{filter-implied FROM}\"] ( selectClause )? ) -> ^( SELECT_FROM ( fromClause )? ( selectClause )? ) )
            // Hql.g:212:5: (s= selectClause )? (f= fromClause )?
            {
            	// Hql.g:212:5: (s= selectClause )?
            	int alt15 = 2;
            	int LA15_0 = input.LA(1);

            	if ( (LA15_0 == SELECT) )
            	{
            	    alt15 = 1;
            	}
            	switch (alt15) 
            	{
            	    case 1 :
            	        // Hql.g:212:6: s= selectClause
            	        {
            	        	PushFollow(FOLLOW_selectClause_in_selectFrom974);
            	        	s = selectClause();
            	        	state.followingStackPointer--;

            	        	stream_selectClause.Add(s.Tree);

            	        }
            	        break;

            	}

            	// Hql.g:212:23: (f= fromClause )?
            	int alt16 = 2;
            	int LA16_0 = input.LA(1);

            	if ( (LA16_0 == FROM) )
            	{
            	    alt16 = 1;
            	}
            	switch (alt16) 
            	{
            	    case 1 :
            	        // Hql.g:212:24: f= fromClause
            	        {
            	        	PushFollow(FOLLOW_fromClause_in_selectFrom981);
            	        	f = fromClause();
            	        	state.followingStackPointer--;

            	        	stream_fromClause.Add(f.Tree);

            	        }
            	        break;

            	}


            				if (((f != null) ? ((IASTNode)f.Tree) : null) == null && !filter) 
            					throw new RecognitionException("FROM expected (non-filter queries must contain a FROM clause)");
            			


            	// AST REWRITE
            	// elements:          fromClause, selectClause, selectClause
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (IASTNode)adaptor.GetNilNode();
            	// 217:3: -> {$f.tree == null && filter}? ^( SELECT_FROM FROM[\"{filter-implied FROM}\"] ( selectClause )? )
            	if (((f != null) ? ((IASTNode)f.Tree) : null) == null && filter)
            	{
            	    // Hql.g:217:35: ^( SELECT_FROM FROM[\"{filter-implied FROM}\"] ( selectClause )? )
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(SELECT_FROM, "SELECT_FROM"), root_1);

            	    adaptor.AddChild(root_1, (IASTNode)adaptor.Create(FROM, "{filter-implied FROM}"));
            	    // Hql.g:217:79: ( selectClause )?
            	    if ( stream_selectClause.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_selectClause.NextTree());

            	    }
            	    stream_selectClause.Reset();

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}
            	else // 218:3: -> ^( SELECT_FROM ( fromClause )? ( selectClause )? )
            	{
            	    // Hql.g:218:6: ^( SELECT_FROM ( fromClause )? ( selectClause )? )
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(SELECT_FROM, "SELECT_FROM"), root_1);

            	    // Hql.g:218:20: ( fromClause )?
            	    if ( stream_fromClause.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_fromClause.NextTree());

            	    }
            	    stream_fromClause.Reset();
            	    // Hql.g:218:32: ( selectClause )?
            	    if ( stream_selectClause.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_selectClause.NextTree());

            	    }
            	    stream_selectClause.Reset();

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;
            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "selectFrom"

    public class selectClause_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "selectClause"
    // Hql.g:222:1: selectClause : SELECT ( DISTINCT )? ( selectedPropertiesList | newExpression | selectObject ) ;
    public HqlParser.selectClause_return selectClause() // throws RecognitionException [1]
    {   
        HqlParser.selectClause_return retval = new HqlParser.selectClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken SELECT45 = null;
        IToken DISTINCT46 = null;
        HqlParser.selectedPropertiesList_return selectedPropertiesList47 = default(HqlParser.selectedPropertiesList_return);

        HqlParser.newExpression_return newExpression48 = default(HqlParser.newExpression_return);

        HqlParser.selectObject_return selectObject49 = default(HqlParser.selectObject_return);


        IASTNode SELECT45_tree=null;
        IASTNode DISTINCT46_tree=null;

        try 
    	{
            // Hql.g:223:2: ( SELECT ( DISTINCT )? ( selectedPropertiesList | newExpression | selectObject ) )
            // Hql.g:223:4: SELECT ( DISTINCT )? ( selectedPropertiesList | newExpression | selectObject )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	SELECT45=(IToken)Match(input,SELECT,FOLLOW_SELECT_in_selectClause1030); 
            		SELECT45_tree = (IASTNode)adaptor.Create(SELECT45);
            		root_0 = (IASTNode)adaptor.BecomeRoot(SELECT45_tree, root_0);

            	 WeakKeywords(); 
            	// Hql.g:225:3: ( DISTINCT )?
            	int alt17 = 2;
            	int LA17_0 = input.LA(1);

            	if ( (LA17_0 == DISTINCT) )
            	{
            	    alt17 = 1;
            	}
            	switch (alt17) 
            	{
            	    case 1 :
            	        // Hql.g:225:4: DISTINCT
            	        {
            	        	DISTINCT46=(IToken)Match(input,DISTINCT,FOLLOW_DISTINCT_in_selectClause1042); 
            	        		DISTINCT46_tree = (IASTNode)adaptor.Create(DISTINCT46);
            	        		adaptor.AddChild(root_0, DISTINCT46_tree);


            	        }
            	        break;

            	}

            	// Hql.g:225:15: ( selectedPropertiesList | newExpression | selectObject )
            	int alt18 = 3;
            	switch ( input.LA(1) ) 
            	{
            	case ALL:
            	case ANY:
            	case AVG:
            	case COUNT:
            	case ELEMENTS:
            	case EXISTS:
            	case FALSE:
            	case INDICES:
            	case MAX:
            	case MIN:
            	case NOT:
            	case NULL:
            	case SOME:
            	case SUM:
            	case TRUE:
            	case CASE:
            	case EMPTY:
            	case NUM_INT:
            	case NUM_DOUBLE:
            	case NUM_DECIMAL:
            	case NUM_FLOAT:
            	case NUM_LONG:
            	case OPEN:
            	case BNOT:
            	case PLUS:
            	case MINUS:
            	case COLON:
            	case PARAM:
            	case QUOTED_String:
            	case IDENT:
            		{
            	    alt18 = 1;
            	    }
            	    break;
            	case NEW:
            		{
            	    alt18 = 2;
            	    }
            	    break;
            	case OBJECT:
            		{
            	    alt18 = 3;
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
            	        // Hql.g:225:17: selectedPropertiesList
            	        {
            	        	PushFollow(FOLLOW_selectedPropertiesList_in_selectClause1048);
            	        	selectedPropertiesList47 = selectedPropertiesList();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, selectedPropertiesList47.Tree);

            	        }
            	        break;
            	    case 2 :
            	        // Hql.g:225:42: newExpression
            	        {
            	        	PushFollow(FOLLOW_newExpression_in_selectClause1052);
            	        	newExpression48 = newExpression();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, newExpression48.Tree);

            	        }
            	        break;
            	    case 3 :
            	        // Hql.g:225:58: selectObject
            	        {
            	        	PushFollow(FOLLOW_selectObject_in_selectClause1056);
            	        	selectObject49 = selectObject();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, selectObject49.Tree);

            	        }
            	        break;

            	}


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "selectClause"

    public class newExpression_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "newExpression"
    // Hql.g:228:1: newExpression : ( NEW path ) op= OPEN selectedPropertiesList CLOSE -> ^( CONSTRUCTOR[$op] path selectedPropertiesList ) ;
    public HqlParser.newExpression_return newExpression() // throws RecognitionException [1]
    {   
        HqlParser.newExpression_return retval = new HqlParser.newExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken op = null;
        IToken NEW50 = null;
        IToken CLOSE53 = null;
        HqlParser.path_return path51 = default(HqlParser.path_return);

        HqlParser.selectedPropertiesList_return selectedPropertiesList52 = default(HqlParser.selectedPropertiesList_return);


        IASTNode op_tree=null;
        IASTNode NEW50_tree=null;
        IASTNode CLOSE53_tree=null;
        RewriteRuleTokenStream stream_NEW = new RewriteRuleTokenStream(adaptor,"token NEW");
        RewriteRuleTokenStream stream_OPEN = new RewriteRuleTokenStream(adaptor,"token OPEN");
        RewriteRuleTokenStream stream_CLOSE = new RewriteRuleTokenStream(adaptor,"token CLOSE");
        RewriteRuleSubtreeStream stream_path = new RewriteRuleSubtreeStream(adaptor,"rule path");
        RewriteRuleSubtreeStream stream_selectedPropertiesList = new RewriteRuleSubtreeStream(adaptor,"rule selectedPropertiesList");
        try 
    	{
            // Hql.g:229:2: ( ( NEW path ) op= OPEN selectedPropertiesList CLOSE -> ^( CONSTRUCTOR[$op] path selectedPropertiesList ) )
            // Hql.g:229:4: ( NEW path ) op= OPEN selectedPropertiesList CLOSE
            {
            	// Hql.g:229:4: ( NEW path )
            	// Hql.g:229:5: NEW path
            	{
            		NEW50=(IToken)Match(input,NEW,FOLLOW_NEW_in_newExpression1070);  
            		stream_NEW.Add(NEW50);

            		PushFollow(FOLLOW_path_in_newExpression1072);
            		path51 = path();
            		state.followingStackPointer--;

            		stream_path.Add(path51.Tree);

            	}

            	op=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_newExpression1077);  
            	stream_OPEN.Add(op);

            	PushFollow(FOLLOW_selectedPropertiesList_in_newExpression1079);
            	selectedPropertiesList52 = selectedPropertiesList();
            	state.followingStackPointer--;

            	stream_selectedPropertiesList.Add(selectedPropertiesList52.Tree);
            	CLOSE53=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_newExpression1081);  
            	stream_CLOSE.Add(CLOSE53);



            	// AST REWRITE
            	// elements:          path, selectedPropertiesList
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (IASTNode)adaptor.GetNilNode();
            	// 230:3: -> ^( CONSTRUCTOR[$op] path selectedPropertiesList )
            	{
            	    // Hql.g:230:6: ^( CONSTRUCTOR[$op] path selectedPropertiesList )
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(CONSTRUCTOR, op), root_1);

            	    adaptor.AddChild(root_1, stream_path.NextTree());
            	    adaptor.AddChild(root_1, stream_selectedPropertiesList.NextTree());

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;
            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "newExpression"

    public class selectObject_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "selectObject"
    // Hql.g:233:1: selectObject : OBJECT OPEN identifier CLOSE ;
    public HqlParser.selectObject_return selectObject() // throws RecognitionException [1]
    {   
        HqlParser.selectObject_return retval = new HqlParser.selectObject_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken OBJECT54 = null;
        IToken OPEN55 = null;
        IToken CLOSE57 = null;
        HqlParser.identifier_return identifier56 = default(HqlParser.identifier_return);


        IASTNode OBJECT54_tree=null;
        IASTNode OPEN55_tree=null;
        IASTNode CLOSE57_tree=null;

        try 
    	{
            // Hql.g:234:4: ( OBJECT OPEN identifier CLOSE )
            // Hql.g:234:6: OBJECT OPEN identifier CLOSE
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	OBJECT54=(IToken)Match(input,OBJECT,FOLLOW_OBJECT_in_selectObject1107); 
            		OBJECT54_tree = (IASTNode)adaptor.Create(OBJECT54);
            		root_0 = (IASTNode)adaptor.BecomeRoot(OBJECT54_tree, root_0);

            	OPEN55=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_selectObject1110); 
            	PushFollow(FOLLOW_identifier_in_selectObject1113);
            	identifier56 = identifier();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, identifier56.Tree);
            	CLOSE57=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_selectObject1115); 

            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "selectObject"

    public class fromClause_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "fromClause"
    // Hql.g:241:1: fromClause : FROM fromRange ( fromJoin | COMMA fromRange )* ;
    public HqlParser.fromClause_return fromClause() // throws RecognitionException [1]
    {   
        HqlParser.fromClause_return retval = new HqlParser.fromClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken FROM58 = null;
        IToken COMMA61 = null;
        HqlParser.fromRange_return fromRange59 = default(HqlParser.fromRange_return);

        HqlParser.fromJoin_return fromJoin60 = default(HqlParser.fromJoin_return);

        HqlParser.fromRange_return fromRange62 = default(HqlParser.fromRange_return);


        IASTNode FROM58_tree=null;
        IASTNode COMMA61_tree=null;

        try 
    	{
            // Hql.g:242:2: ( FROM fromRange ( fromJoin | COMMA fromRange )* )
            // Hql.g:242:4: FROM fromRange ( fromJoin | COMMA fromRange )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	FROM58=(IToken)Match(input,FROM,FOLLOW_FROM_in_fromClause1133); 
            		FROM58_tree = (IASTNode)adaptor.Create(FROM58);
            		root_0 = (IASTNode)adaptor.BecomeRoot(FROM58_tree, root_0);

            	 WeakKeywords(); 
            	PushFollow(FOLLOW_fromRange_in_fromClause1138);
            	fromRange59 = fromRange();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, fromRange59.Tree);
            	// Hql.g:242:40: ( fromJoin | COMMA fromRange )*
            	do 
            	{
            	    int alt19 = 3;
            	    int LA19_0 = input.LA(1);

            	    if ( (LA19_0 == FULL || LA19_0 == INNER || (LA19_0 >= JOIN && LA19_0 <= LEFT) || LA19_0 == RIGHT) )
            	    {
            	        alt19 = 1;
            	    }
            	    else if ( (LA19_0 == COMMA) )
            	    {
            	        alt19 = 2;
            	    }


            	    switch (alt19) 
            		{
            			case 1 :
            			    // Hql.g:242:42: fromJoin
            			    {
            			    	PushFollow(FOLLOW_fromJoin_in_fromClause1142);
            			    	fromJoin60 = fromJoin();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, fromJoin60.Tree);

            			    }
            			    break;
            			case 2 :
            			    // Hql.g:242:53: COMMA fromRange
            			    {
            			    	COMMA61=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_fromClause1146); 
            			    	 WeakKeywords(); 
            			    	PushFollow(FOLLOW_fromRange_in_fromClause1151);
            			    	fromRange62 = fromRange();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, fromRange62.Tree);

            			    }
            			    break;

            			default:
            			    goto loop19;
            	    }
            	} while (true);

            	loop19:
            		;	// Stops C# compiler whining that label 'loop19' has no statements


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "fromClause"

    public class fromJoin_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "fromJoin"
    // Hql.g:245:1: fromJoin : ( ( ( ( LEFT | RIGHT ) ( OUTER )? ) | FULL | INNER )? JOIN ( FETCH )? path ( asAlias )? ( propertyFetch )? ( withClause )? | ( ( ( LEFT | RIGHT ) ( OUTER )? ) | FULL | INNER )? JOIN ( FETCH )? ELEMENTS OPEN path CLOSE ( asAlias )? ( propertyFetch )? ( withClause )? );
    public HqlParser.fromJoin_return fromJoin() // throws RecognitionException [1]
    {   
        HqlParser.fromJoin_return retval = new HqlParser.fromJoin_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken set63 = null;
        IToken OUTER64 = null;
        IToken FULL65 = null;
        IToken INNER66 = null;
        IToken JOIN67 = null;
        IToken FETCH68 = null;
        IToken set73 = null;
        IToken OUTER74 = null;
        IToken FULL75 = null;
        IToken INNER76 = null;
        IToken JOIN77 = null;
        IToken FETCH78 = null;
        IToken ELEMENTS79 = null;
        IToken OPEN80 = null;
        IToken CLOSE82 = null;
        HqlParser.path_return path69 = default(HqlParser.path_return);

        HqlParser.asAlias_return asAlias70 = default(HqlParser.asAlias_return);

        HqlParser.propertyFetch_return propertyFetch71 = default(HqlParser.propertyFetch_return);

        HqlParser.withClause_return withClause72 = default(HqlParser.withClause_return);

        HqlParser.path_return path81 = default(HqlParser.path_return);

        HqlParser.asAlias_return asAlias83 = default(HqlParser.asAlias_return);

        HqlParser.propertyFetch_return propertyFetch84 = default(HqlParser.propertyFetch_return);

        HqlParser.withClause_return withClause85 = default(HqlParser.withClause_return);


        IASTNode set63_tree=null;
        IASTNode OUTER64_tree=null;
        IASTNode FULL65_tree=null;
        IASTNode INNER66_tree=null;
        IASTNode JOIN67_tree=null;
        IASTNode FETCH68_tree=null;
        IASTNode set73_tree=null;
        IASTNode OUTER74_tree=null;
        IASTNode FULL75_tree=null;
        IASTNode INNER76_tree=null;
        IASTNode JOIN77_tree=null;
        IASTNode FETCH78_tree=null;
        IASTNode ELEMENTS79_tree=null;
        IASTNode OPEN80_tree=null;
        IASTNode CLOSE82_tree=null;

        try 
    	{
            // Hql.g:246:2: ( ( ( ( LEFT | RIGHT ) ( OUTER )? ) | FULL | INNER )? JOIN ( FETCH )? path ( asAlias )? ( propertyFetch )? ( withClause )? | ( ( ( LEFT | RIGHT ) ( OUTER )? ) | FULL | INNER )? JOIN ( FETCH )? ELEMENTS OPEN path CLOSE ( asAlias )? ( propertyFetch )? ( withClause )? )
            int alt32 = 2;
            switch ( input.LA(1) ) 
            {
            case LEFT:
            case RIGHT:
            	{
                int LA32_1 = input.LA(2);

                if ( (LA32_1 == OUTER) )
                {
                    int LA32_5 = input.LA(3);

                    if ( (LA32_5 == JOIN) )
                    {
                        switch ( input.LA(4) ) 
                        {
                        case FETCH:
                        	{
                            int LA32_6 = input.LA(5);

                            if ( (LA32_6 == ELEMENTS) )
                            {
                                alt32 = 2;
                            }
                            else if ( (LA32_6 == IDENT) )
                            {
                                alt32 = 1;
                            }
                            else 
                            {
                                NoViableAltException nvae_d32s6 =
                                    new NoViableAltException("", 32, 6, input);

                                throw nvae_d32s6;
                            }
                            }
                            break;
                        case ELEMENTS:
                        	{
                            alt32 = 2;
                            }
                            break;
                        case IDENT:
                        	{
                            alt32 = 1;
                            }
                            break;
                        	default:
                        	    NoViableAltException nvae_d32s4 =
                        	        new NoViableAltException("", 32, 4, input);

                        	    throw nvae_d32s4;
                        }

                    }
                    else 
                    {
                        NoViableAltException nvae_d32s5 =
                            new NoViableAltException("", 32, 5, input);

                        throw nvae_d32s5;
                    }
                }
                else if ( (LA32_1 == JOIN) )
                {
                    switch ( input.LA(3) ) 
                    {
                    case FETCH:
                    	{
                        int LA32_6 = input.LA(4);

                        if ( (LA32_6 == ELEMENTS) )
                        {
                            alt32 = 2;
                        }
                        else if ( (LA32_6 == IDENT) )
                        {
                            alt32 = 1;
                        }
                        else 
                        {
                            NoViableAltException nvae_d32s6 =
                                new NoViableAltException("", 32, 6, input);

                            throw nvae_d32s6;
                        }
                        }
                        break;
                    case ELEMENTS:
                    	{
                        alt32 = 2;
                        }
                        break;
                    case IDENT:
                    	{
                        alt32 = 1;
                        }
                        break;
                    	default:
                    	    NoViableAltException nvae_d32s4 =
                    	        new NoViableAltException("", 32, 4, input);

                    	    throw nvae_d32s4;
                    }

                }
                else 
                {
                    NoViableAltException nvae_d32s1 =
                        new NoViableAltException("", 32, 1, input);

                    throw nvae_d32s1;
                }
                }
                break;
            case FULL:
            	{
                int LA32_2 = input.LA(2);

                if ( (LA32_2 == JOIN) )
                {
                    switch ( input.LA(3) ) 
                    {
                    case FETCH:
                    	{
                        int LA32_6 = input.LA(4);

                        if ( (LA32_6 == ELEMENTS) )
                        {
                            alt32 = 2;
                        }
                        else if ( (LA32_6 == IDENT) )
                        {
                            alt32 = 1;
                        }
                        else 
                        {
                            NoViableAltException nvae_d32s6 =
                                new NoViableAltException("", 32, 6, input);

                            throw nvae_d32s6;
                        }
                        }
                        break;
                    case ELEMENTS:
                    	{
                        alt32 = 2;
                        }
                        break;
                    case IDENT:
                    	{
                        alt32 = 1;
                        }
                        break;
                    	default:
                    	    NoViableAltException nvae_d32s4 =
                    	        new NoViableAltException("", 32, 4, input);

                    	    throw nvae_d32s4;
                    }

                }
                else 
                {
                    NoViableAltException nvae_d32s2 =
                        new NoViableAltException("", 32, 2, input);

                    throw nvae_d32s2;
                }
                }
                break;
            case INNER:
            	{
                int LA32_3 = input.LA(2);

                if ( (LA32_3 == JOIN) )
                {
                    switch ( input.LA(3) ) 
                    {
                    case FETCH:
                    	{
                        int LA32_6 = input.LA(4);

                        if ( (LA32_6 == ELEMENTS) )
                        {
                            alt32 = 2;
                        }
                        else if ( (LA32_6 == IDENT) )
                        {
                            alt32 = 1;
                        }
                        else 
                        {
                            NoViableAltException nvae_d32s6 =
                                new NoViableAltException("", 32, 6, input);

                            throw nvae_d32s6;
                        }
                        }
                        break;
                    case ELEMENTS:
                    	{
                        alt32 = 2;
                        }
                        break;
                    case IDENT:
                    	{
                        alt32 = 1;
                        }
                        break;
                    	default:
                    	    NoViableAltException nvae_d32s4 =
                    	        new NoViableAltException("", 32, 4, input);

                    	    throw nvae_d32s4;
                    }

                }
                else 
                {
                    NoViableAltException nvae_d32s3 =
                        new NoViableAltException("", 32, 3, input);

                    throw nvae_d32s3;
                }
                }
                break;
            case JOIN:
            	{
                switch ( input.LA(2) ) 
                {
                case FETCH:
                	{
                    int LA32_6 = input.LA(3);

                    if ( (LA32_6 == ELEMENTS) )
                    {
                        alt32 = 2;
                    }
                    else if ( (LA32_6 == IDENT) )
                    {
                        alt32 = 1;
                    }
                    else 
                    {
                        NoViableAltException nvae_d32s6 =
                            new NoViableAltException("", 32, 6, input);

                        throw nvae_d32s6;
                    }
                    }
                    break;
                case ELEMENTS:
                	{
                    alt32 = 2;
                    }
                    break;
                case IDENT:
                	{
                    alt32 = 1;
                    }
                    break;
                	default:
                	    NoViableAltException nvae_d32s4 =
                	        new NoViableAltException("", 32, 4, input);

                	    throw nvae_d32s4;
                }

                }
                break;
            	default:
            	    NoViableAltException nvae_d32s0 =
            	        new NoViableAltException("", 32, 0, input);

            	    throw nvae_d32s0;
            }

            switch (alt32) 
            {
                case 1 :
                    // Hql.g:246:4: ( ( ( LEFT | RIGHT ) ( OUTER )? ) | FULL | INNER )? JOIN ( FETCH )? path ( asAlias )? ( propertyFetch )? ( withClause )?
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	// Hql.g:246:4: ( ( ( LEFT | RIGHT ) ( OUTER )? ) | FULL | INNER )?
                    	int alt21 = 4;
                    	switch ( input.LA(1) ) 
                    	{
                    	    case LEFT:
                    	    case RIGHT:
                    	    	{
                    	        alt21 = 1;
                    	        }
                    	        break;
                    	    case FULL:
                    	    	{
                    	        alt21 = 2;
                    	        }
                    	        break;
                    	    case INNER:
                    	    	{
                    	        alt21 = 3;
                    	        }
                    	        break;
                    	}

                    	switch (alt21) 
                    	{
                    	    case 1 :
                    	        // Hql.g:246:6: ( ( LEFT | RIGHT ) ( OUTER )? )
                    	        {
                    	        	// Hql.g:246:6: ( ( LEFT | RIGHT ) ( OUTER )? )
                    	        	// Hql.g:246:8: ( LEFT | RIGHT ) ( OUTER )?
                    	        	{
                    	        		set63 = (IToken)input.LT(1);
                    	        		if ( input.LA(1) == LEFT || input.LA(1) == RIGHT ) 
                    	        		{
                    	        		    input.Consume();
                    	        		    adaptor.AddChild(root_0, (IASTNode)adaptor.Create(set63));
                    	        		    state.errorRecovery = false;
                    	        		}
                    	        		else 
                    	        		{
                    	        		    MismatchedSetException mse = new MismatchedSetException(null,input);
                    	        		    throw mse;
                    	        		}

                    	        		// Hql.g:246:25: ( OUTER )?
                    	        		int alt20 = 2;
                    	        		int LA20_0 = input.LA(1);

                    	        		if ( (LA20_0 == OUTER) )
                    	        		{
                    	        		    alt20 = 1;
                    	        		}
                    	        		switch (alt20) 
                    	        		{
                    	        		    case 1 :
                    	        		        // Hql.g:246:26: OUTER
                    	        		        {
                    	        		        	OUTER64=(IToken)Match(input,OUTER,FOLLOW_OUTER_in_fromJoin1180); 
                    	        		        		OUTER64_tree = (IASTNode)adaptor.Create(OUTER64);
                    	        		        		adaptor.AddChild(root_0, OUTER64_tree);


                    	        		        }
                    	        		        break;

                    	        		}


                    	        	}


                    	        }
                    	        break;
                    	    case 2 :
                    	        // Hql.g:246:38: FULL
                    	        {
                    	        	FULL65=(IToken)Match(input,FULL,FOLLOW_FULL_in_fromJoin1188); 
                    	        		FULL65_tree = (IASTNode)adaptor.Create(FULL65);
                    	        		adaptor.AddChild(root_0, FULL65_tree);


                    	        }
                    	        break;
                    	    case 3 :
                    	        // Hql.g:246:45: INNER
                    	        {
                    	        	INNER66=(IToken)Match(input,INNER,FOLLOW_INNER_in_fromJoin1192); 
                    	        		INNER66_tree = (IASTNode)adaptor.Create(INNER66);
                    	        		adaptor.AddChild(root_0, INNER66_tree);


                    	        }
                    	        break;

                    	}

                    	JOIN67=(IToken)Match(input,JOIN,FOLLOW_JOIN_in_fromJoin1197); 
                    		JOIN67_tree = (IASTNode)adaptor.Create(JOIN67);
                    		root_0 = (IASTNode)adaptor.BecomeRoot(JOIN67_tree, root_0);

                    	// Hql.g:246:60: ( FETCH )?
                    	int alt22 = 2;
                    	int LA22_0 = input.LA(1);

                    	if ( (LA22_0 == FETCH) )
                    	{
                    	    alt22 = 1;
                    	}
                    	switch (alt22) 
                    	{
                    	    case 1 :
                    	        // Hql.g:246:61: FETCH
                    	        {
                    	        	FETCH68=(IToken)Match(input,FETCH,FOLLOW_FETCH_in_fromJoin1201); 
                    	        		FETCH68_tree = (IASTNode)adaptor.Create(FETCH68);
                    	        		adaptor.AddChild(root_0, FETCH68_tree);


                    	        }
                    	        break;

                    	}

                    	PushFollow(FOLLOW_path_in_fromJoin1205);
                    	path69 = path();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, path69.Tree);
                    	// Hql.g:246:74: ( asAlias )?
                    	int alt23 = 2;
                    	int LA23_0 = input.LA(1);

                    	if ( (LA23_0 == AS || LA23_0 == IDENT) )
                    	{
                    	    alt23 = 1;
                    	}
                    	switch (alt23) 
                    	{
                    	    case 1 :
                    	        // Hql.g:246:75: asAlias
                    	        {
                    	        	PushFollow(FOLLOW_asAlias_in_fromJoin1208);
                    	        	asAlias70 = asAlias();
                    	        	state.followingStackPointer--;

                    	        	adaptor.AddChild(root_0, asAlias70.Tree);

                    	        }
                    	        break;

                    	}

                    	// Hql.g:246:85: ( propertyFetch )?
                    	int alt24 = 2;
                    	int LA24_0 = input.LA(1);

                    	if ( (LA24_0 == FETCH) )
                    	{
                    	    alt24 = 1;
                    	}
                    	switch (alt24) 
                    	{
                    	    case 1 :
                    	        // Hql.g:246:86: propertyFetch
                    	        {
                    	        	PushFollow(FOLLOW_propertyFetch_in_fromJoin1213);
                    	        	propertyFetch71 = propertyFetch();
                    	        	state.followingStackPointer--;

                    	        	adaptor.AddChild(root_0, propertyFetch71.Tree);

                    	        }
                    	        break;

                    	}

                    	// Hql.g:246:102: ( withClause )?
                    	int alt25 = 2;
                    	int LA25_0 = input.LA(1);

                    	if ( (LA25_0 == WITH) )
                    	{
                    	    alt25 = 1;
                    	}
                    	switch (alt25) 
                    	{
                    	    case 1 :
                    	        // Hql.g:246:103: withClause
                    	        {
                    	        	PushFollow(FOLLOW_withClause_in_fromJoin1218);
                    	        	withClause72 = withClause();
                    	        	state.followingStackPointer--;

                    	        	adaptor.AddChild(root_0, withClause72.Tree);

                    	        }
                    	        break;

                    	}


                    }
                    break;
                case 2 :
                    // Hql.g:247:4: ( ( ( LEFT | RIGHT ) ( OUTER )? ) | FULL | INNER )? JOIN ( FETCH )? ELEMENTS OPEN path CLOSE ( asAlias )? ( propertyFetch )? ( withClause )?
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	// Hql.g:247:4: ( ( ( LEFT | RIGHT ) ( OUTER )? ) | FULL | INNER )?
                    	int alt27 = 4;
                    	switch ( input.LA(1) ) 
                    	{
                    	    case LEFT:
                    	    case RIGHT:
                    	    	{
                    	        alt27 = 1;
                    	        }
                    	        break;
                    	    case FULL:
                    	    	{
                    	        alt27 = 2;
                    	        }
                    	        break;
                    	    case INNER:
                    	    	{
                    	        alt27 = 3;
                    	        }
                    	        break;
                    	}

                    	switch (alt27) 
                    	{
                    	    case 1 :
                    	        // Hql.g:247:6: ( ( LEFT | RIGHT ) ( OUTER )? )
                    	        {
                    	        	// Hql.g:247:6: ( ( LEFT | RIGHT ) ( OUTER )? )
                    	        	// Hql.g:247:8: ( LEFT | RIGHT ) ( OUTER )?
                    	        	{
                    	        		set73 = (IToken)input.LT(1);
                    	        		if ( input.LA(1) == LEFT || input.LA(1) == RIGHT ) 
                    	        		{
                    	        		    input.Consume();
                    	        		    adaptor.AddChild(root_0, (IASTNode)adaptor.Create(set73));
                    	        		    state.errorRecovery = false;
                    	        		}
                    	        		else 
                    	        		{
                    	        		    MismatchedSetException mse = new MismatchedSetException(null,input);
                    	        		    throw mse;
                    	        		}

                    	        		// Hql.g:247:25: ( OUTER )?
                    	        		int alt26 = 2;
                    	        		int LA26_0 = input.LA(1);

                    	        		if ( (LA26_0 == OUTER) )
                    	        		{
                    	        		    alt26 = 1;
                    	        		}
                    	        		switch (alt26) 
                    	        		{
                    	        		    case 1 :
                    	        		        // Hql.g:247:26: OUTER
                    	        		        {
                    	        		        	OUTER74=(IToken)Match(input,OUTER,FOLLOW_OUTER_in_fromJoin1240); 
                    	        		        		OUTER74_tree = (IASTNode)adaptor.Create(OUTER74);
                    	        		        		adaptor.AddChild(root_0, OUTER74_tree);


                    	        		        }
                    	        		        break;

                    	        		}


                    	        	}


                    	        }
                    	        break;
                    	    case 2 :
                    	        // Hql.g:247:38: FULL
                    	        {
                    	        	FULL75=(IToken)Match(input,FULL,FOLLOW_FULL_in_fromJoin1248); 
                    	        		FULL75_tree = (IASTNode)adaptor.Create(FULL75);
                    	        		adaptor.AddChild(root_0, FULL75_tree);


                    	        }
                    	        break;
                    	    case 3 :
                    	        // Hql.g:247:45: INNER
                    	        {
                    	        	INNER76=(IToken)Match(input,INNER,FOLLOW_INNER_in_fromJoin1252); 
                    	        		INNER76_tree = (IASTNode)adaptor.Create(INNER76);
                    	        		adaptor.AddChild(root_0, INNER76_tree);


                    	        }
                    	        break;

                    	}

                    	JOIN77=(IToken)Match(input,JOIN,FOLLOW_JOIN_in_fromJoin1257); 
                    		JOIN77_tree = (IASTNode)adaptor.Create(JOIN77);
                    		root_0 = (IASTNode)adaptor.BecomeRoot(JOIN77_tree, root_0);

                    	// Hql.g:247:60: ( FETCH )?
                    	int alt28 = 2;
                    	int LA28_0 = input.LA(1);

                    	if ( (LA28_0 == FETCH) )
                    	{
                    	    alt28 = 1;
                    	}
                    	switch (alt28) 
                    	{
                    	    case 1 :
                    	        // Hql.g:247:61: FETCH
                    	        {
                    	        	FETCH78=(IToken)Match(input,FETCH,FOLLOW_FETCH_in_fromJoin1261); 
                    	        		FETCH78_tree = (IASTNode)adaptor.Create(FETCH78);
                    	        		adaptor.AddChild(root_0, FETCH78_tree);


                    	        }
                    	        break;

                    	}

                    	ELEMENTS79=(IToken)Match(input,ELEMENTS,FOLLOW_ELEMENTS_in_fromJoin1265); 
                    	OPEN80=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_fromJoin1268); 
                    	PushFollow(FOLLOW_path_in_fromJoin1271);
                    	path81 = path();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, path81.Tree);
                    	CLOSE82=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_fromJoin1273); 
                    	// Hql.g:247:97: ( asAlias )?
                    	int alt29 = 2;
                    	int LA29_0 = input.LA(1);

                    	if ( (LA29_0 == AS || LA29_0 == IDENT) )
                    	{
                    	    alt29 = 1;
                    	}
                    	switch (alt29) 
                    	{
                    	    case 1 :
                    	        // Hql.g:247:98: asAlias
                    	        {
                    	        	PushFollow(FOLLOW_asAlias_in_fromJoin1277);
                    	        	asAlias83 = asAlias();
                    	        	state.followingStackPointer--;

                    	        	adaptor.AddChild(root_0, asAlias83.Tree);

                    	        }
                    	        break;

                    	}

                    	// Hql.g:247:108: ( propertyFetch )?
                    	int alt30 = 2;
                    	int LA30_0 = input.LA(1);

                    	if ( (LA30_0 == FETCH) )
                    	{
                    	    alt30 = 1;
                    	}
                    	switch (alt30) 
                    	{
                    	    case 1 :
                    	        // Hql.g:247:109: propertyFetch
                    	        {
                    	        	PushFollow(FOLLOW_propertyFetch_in_fromJoin1282);
                    	        	propertyFetch84 = propertyFetch();
                    	        	state.followingStackPointer--;

                    	        	adaptor.AddChild(root_0, propertyFetch84.Tree);

                    	        }
                    	        break;

                    	}

                    	// Hql.g:247:125: ( withClause )?
                    	int alt31 = 2;
                    	int LA31_0 = input.LA(1);

                    	if ( (LA31_0 == WITH) )
                    	{
                    	    alt31 = 1;
                    	}
                    	switch (alt31) 
                    	{
                    	    case 1 :
                    	        // Hql.g:247:126: withClause
                    	        {
                    	        	PushFollow(FOLLOW_withClause_in_fromJoin1287);
                    	        	withClause85 = withClause();
                    	        	state.followingStackPointer--;

                    	        	adaptor.AddChild(root_0, withClause85.Tree);

                    	        }
                    	        break;

                    	}


                    }
                    break;

            }
            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "fromJoin"

    public class withClause_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "withClause"
    // Hql.g:250:1: withClause : WITH logicalExpression ;
    public HqlParser.withClause_return withClause() // throws RecognitionException [1]
    {   
        HqlParser.withClause_return retval = new HqlParser.withClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken WITH86 = null;
        HqlParser.logicalExpression_return logicalExpression87 = default(HqlParser.logicalExpression_return);


        IASTNode WITH86_tree=null;

        try 
    	{
            // Hql.g:251:2: ( WITH logicalExpression )
            // Hql.g:251:4: WITH logicalExpression
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	WITH86=(IToken)Match(input,WITH,FOLLOW_WITH_in_withClause1300); 
            		WITH86_tree = (IASTNode)adaptor.Create(WITH86);
            		root_0 = (IASTNode)adaptor.BecomeRoot(WITH86_tree, root_0);

            	PushFollow(FOLLOW_logicalExpression_in_withClause1303);
            	logicalExpression87 = logicalExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, logicalExpression87.Tree);

            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "withClause"

    public class fromRange_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "fromRange"
    // Hql.g:254:1: fromRange : ( fromClassOrOuterQueryPath | inClassDeclaration | inCollectionDeclaration | inCollectionElementsDeclaration );
    public HqlParser.fromRange_return fromRange() // throws RecognitionException [1]
    {   
        HqlParser.fromRange_return retval = new HqlParser.fromRange_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.fromClassOrOuterQueryPath_return fromClassOrOuterQueryPath88 = default(HqlParser.fromClassOrOuterQueryPath_return);

        HqlParser.inClassDeclaration_return inClassDeclaration89 = default(HqlParser.inClassDeclaration_return);

        HqlParser.inCollectionDeclaration_return inCollectionDeclaration90 = default(HqlParser.inCollectionDeclaration_return);

        HqlParser.inCollectionElementsDeclaration_return inCollectionElementsDeclaration91 = default(HqlParser.inCollectionElementsDeclaration_return);



        try 
    	{
            // Hql.g:255:2: ( fromClassOrOuterQueryPath | inClassDeclaration | inCollectionDeclaration | inCollectionElementsDeclaration )
            int alt33 = 4;
            switch ( input.LA(1) ) 
            {
            case IDENT:
            	{
                int LA33_1 = input.LA(2);

                if ( (LA33_1 == EOF || LA33_1 == AS || LA33_1 == DOT || LA33_1 == FETCH || (LA33_1 >= FULL && LA33_1 <= HAVING) || LA33_1 == INNER || (LA33_1 >= JOIN && LA33_1 <= LEFT) || LA33_1 == ORDER || LA33_1 == RIGHT || LA33_1 == SKIP || LA33_1 == TAKE || LA33_1 == UNION || LA33_1 == WHERE || LA33_1 == COMMA || LA33_1 == CLOSE || LA33_1 == IDENT) )
                {
                    alt33 = 1;
                }
                else if ( (LA33_1 == IN) )
                {
                    int LA33_5 = input.LA(3);

                    if ( (LA33_5 == ELEMENTS) )
                    {
                        alt33 = 4;
                    }
                    else if ( (LA33_5 == CLASS || LA33_5 == IDENT) )
                    {
                        alt33 = 2;
                    }
                    else 
                    {
                        NoViableAltException nvae_d33s5 =
                            new NoViableAltException("", 33, 5, input);

                        throw nvae_d33s5;
                    }
                }
                else 
                {
                    NoViableAltException nvae_d33s1 =
                        new NoViableAltException("", 33, 1, input);

                    throw nvae_d33s1;
                }
                }
                break;
            case IN:
            	{
                alt33 = 3;
                }
                break;
            case ELEMENTS:
            	{
                alt33 = 4;
                }
                break;
            	default:
            	    NoViableAltException nvae_d33s0 =
            	        new NoViableAltException("", 33, 0, input);

            	    throw nvae_d33s0;
            }

            switch (alt33) 
            {
                case 1 :
                    // Hql.g:255:4: fromClassOrOuterQueryPath
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_fromClassOrOuterQueryPath_in_fromRange1314);
                    	fromClassOrOuterQueryPath88 = fromClassOrOuterQueryPath();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, fromClassOrOuterQueryPath88.Tree);

                    }
                    break;
                case 2 :
                    // Hql.g:256:4: inClassDeclaration
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_inClassDeclaration_in_fromRange1319);
                    	inClassDeclaration89 = inClassDeclaration();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, inClassDeclaration89.Tree);

                    }
                    break;
                case 3 :
                    // Hql.g:257:4: inCollectionDeclaration
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_inCollectionDeclaration_in_fromRange1324);
                    	inCollectionDeclaration90 = inCollectionDeclaration();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, inCollectionDeclaration90.Tree);

                    }
                    break;
                case 4 :
                    // Hql.g:258:4: inCollectionElementsDeclaration
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_inCollectionElementsDeclaration_in_fromRange1329);
                    	inCollectionElementsDeclaration91 = inCollectionElementsDeclaration();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, inCollectionElementsDeclaration91.Tree);

                    }
                    break;

            }
            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "fromRange"

    public class fromClassOrOuterQueryPath_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "fromClassOrOuterQueryPath"
    // Hql.g:261:1: fromClassOrOuterQueryPath : path ( asAlias )? ( propertyFetch )? -> ^( RANGE path ( asAlias )? ( propertyFetch )? ) ;
    public HqlParser.fromClassOrOuterQueryPath_return fromClassOrOuterQueryPath() // throws RecognitionException [1]
    {   
        HqlParser.fromClassOrOuterQueryPath_return retval = new HqlParser.fromClassOrOuterQueryPath_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.path_return path92 = default(HqlParser.path_return);

        HqlParser.asAlias_return asAlias93 = default(HqlParser.asAlias_return);

        HqlParser.propertyFetch_return propertyFetch94 = default(HqlParser.propertyFetch_return);


        RewriteRuleSubtreeStream stream_propertyFetch = new RewriteRuleSubtreeStream(adaptor,"rule propertyFetch");
        RewriteRuleSubtreeStream stream_path = new RewriteRuleSubtreeStream(adaptor,"rule path");
        RewriteRuleSubtreeStream stream_asAlias = new RewriteRuleSubtreeStream(adaptor,"rule asAlias");
        try 
    	{
            // Hql.g:262:2: ( path ( asAlias )? ( propertyFetch )? -> ^( RANGE path ( asAlias )? ( propertyFetch )? ) )
            // Hql.g:262:4: path ( asAlias )? ( propertyFetch )?
            {
            	PushFollow(FOLLOW_path_in_fromClassOrOuterQueryPath1341);
            	path92 = path();
            	state.followingStackPointer--;

            	stream_path.Add(path92.Tree);
            	 WeakKeywords(); 
            	// Hql.g:262:29: ( asAlias )?
            	int alt34 = 2;
            	int LA34_0 = input.LA(1);

            	if ( (LA34_0 == AS || LA34_0 == IDENT) )
            	{
            	    alt34 = 1;
            	}
            	switch (alt34) 
            	{
            	    case 1 :
            	        // Hql.g:262:30: asAlias
            	        {
            	        	PushFollow(FOLLOW_asAlias_in_fromClassOrOuterQueryPath1346);
            	        	asAlias93 = asAlias();
            	        	state.followingStackPointer--;

            	        	stream_asAlias.Add(asAlias93.Tree);

            	        }
            	        break;

            	}

            	// Hql.g:262:40: ( propertyFetch )?
            	int alt35 = 2;
            	int LA35_0 = input.LA(1);

            	if ( (LA35_0 == FETCH) )
            	{
            	    alt35 = 1;
            	}
            	switch (alt35) 
            	{
            	    case 1 :
            	        // Hql.g:262:41: propertyFetch
            	        {
            	        	PushFollow(FOLLOW_propertyFetch_in_fromClassOrOuterQueryPath1351);
            	        	propertyFetch94 = propertyFetch();
            	        	state.followingStackPointer--;

            	        	stream_propertyFetch.Add(propertyFetch94.Tree);

            	        }
            	        break;

            	}



            	// AST REWRITE
            	// elements:          propertyFetch, path, asAlias
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (IASTNode)adaptor.GetNilNode();
            	// 263:3: -> ^( RANGE path ( asAlias )? ( propertyFetch )? )
            	{
            	    // Hql.g:263:6: ^( RANGE path ( asAlias )? ( propertyFetch )? )
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(RANGE, "RANGE"), root_1);

            	    adaptor.AddChild(root_1, stream_path.NextTree());
            	    // Hql.g:263:19: ( asAlias )?
            	    if ( stream_asAlias.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_asAlias.NextTree());

            	    }
            	    stream_asAlias.Reset();
            	    // Hql.g:263:28: ( propertyFetch )?
            	    if ( stream_propertyFetch.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_propertyFetch.NextTree());

            	    }
            	    stream_propertyFetch.Reset();

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;
            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "fromClassOrOuterQueryPath"

    public class inClassDeclaration_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "inClassDeclaration"
    // Hql.g:266:1: inClassDeclaration : alias IN ( CLASS )? path -> ^( RANGE path alias ) ;
    public HqlParser.inClassDeclaration_return inClassDeclaration() // throws RecognitionException [1]
    {   
        HqlParser.inClassDeclaration_return retval = new HqlParser.inClassDeclaration_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken IN96 = null;
        IToken CLASS97 = null;
        HqlParser.alias_return alias95 = default(HqlParser.alias_return);

        HqlParser.path_return path98 = default(HqlParser.path_return);


        IASTNode IN96_tree=null;
        IASTNode CLASS97_tree=null;
        RewriteRuleTokenStream stream_CLASS = new RewriteRuleTokenStream(adaptor,"token CLASS");
        RewriteRuleTokenStream stream_IN = new RewriteRuleTokenStream(adaptor,"token IN");
        RewriteRuleSubtreeStream stream_alias = new RewriteRuleSubtreeStream(adaptor,"rule alias");
        RewriteRuleSubtreeStream stream_path = new RewriteRuleSubtreeStream(adaptor,"rule path");
        try 
    	{
            // Hql.g:267:2: ( alias IN ( CLASS )? path -> ^( RANGE path alias ) )
            // Hql.g:267:4: alias IN ( CLASS )? path
            {
            	PushFollow(FOLLOW_alias_in_inClassDeclaration1381);
            	alias95 = alias();
            	state.followingStackPointer--;

            	stream_alias.Add(alias95.Tree);
            	IN96=(IToken)Match(input,IN,FOLLOW_IN_in_inClassDeclaration1383);  
            	stream_IN.Add(IN96);

            	// Hql.g:267:13: ( CLASS )?
            	int alt36 = 2;
            	int LA36_0 = input.LA(1);

            	if ( (LA36_0 == CLASS) )
            	{
            	    alt36 = 1;
            	}
            	switch (alt36) 
            	{
            	    case 1 :
            	        // Hql.g:267:13: CLASS
            	        {
            	        	CLASS97=(IToken)Match(input,CLASS,FOLLOW_CLASS_in_inClassDeclaration1385);  
            	        	stream_CLASS.Add(CLASS97);


            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_path_in_inClassDeclaration1388);
            	path98 = path();
            	state.followingStackPointer--;

            	stream_path.Add(path98.Tree);


            	// AST REWRITE
            	// elements:          alias, path
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (IASTNode)adaptor.GetNilNode();
            	// 268:3: -> ^( RANGE path alias )
            	{
            	    // Hql.g:268:6: ^( RANGE path alias )
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(RANGE, "RANGE"), root_1);

            	    adaptor.AddChild(root_1, stream_path.NextTree());
            	    adaptor.AddChild(root_1, stream_alias.NextTree());

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;
            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "inClassDeclaration"

    public class inCollectionDeclaration_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "inCollectionDeclaration"
    // Hql.g:271:1: inCollectionDeclaration : IN OPEN path CLOSE alias -> ^( JOIN[\"join\"] INNER[\"inner\"] path alias ) ;
    public HqlParser.inCollectionDeclaration_return inCollectionDeclaration() // throws RecognitionException [1]
    {   
        HqlParser.inCollectionDeclaration_return retval = new HqlParser.inCollectionDeclaration_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken IN99 = null;
        IToken OPEN100 = null;
        IToken CLOSE102 = null;
        HqlParser.path_return path101 = default(HqlParser.path_return);

        HqlParser.alias_return alias103 = default(HqlParser.alias_return);


        IASTNode IN99_tree=null;
        IASTNode OPEN100_tree=null;
        IASTNode CLOSE102_tree=null;
        RewriteRuleTokenStream stream_OPEN = new RewriteRuleTokenStream(adaptor,"token OPEN");
        RewriteRuleTokenStream stream_IN = new RewriteRuleTokenStream(adaptor,"token IN");
        RewriteRuleTokenStream stream_CLOSE = new RewriteRuleTokenStream(adaptor,"token CLOSE");
        RewriteRuleSubtreeStream stream_alias = new RewriteRuleSubtreeStream(adaptor,"rule alias");
        RewriteRuleSubtreeStream stream_path = new RewriteRuleSubtreeStream(adaptor,"rule path");
        try 
    	{
            // Hql.g:272:5: ( IN OPEN path CLOSE alias -> ^( JOIN[\"join\"] INNER[\"inner\"] path alias ) )
            // Hql.g:272:7: IN OPEN path CLOSE alias
            {
            	IN99=(IToken)Match(input,IN,FOLLOW_IN_in_inCollectionDeclaration1416);  
            	stream_IN.Add(IN99);

            	OPEN100=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_inCollectionDeclaration1418);  
            	stream_OPEN.Add(OPEN100);

            	PushFollow(FOLLOW_path_in_inCollectionDeclaration1420);
            	path101 = path();
            	state.followingStackPointer--;

            	stream_path.Add(path101.Tree);
            	CLOSE102=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_inCollectionDeclaration1422);  
            	stream_CLOSE.Add(CLOSE102);

            	PushFollow(FOLLOW_alias_in_inCollectionDeclaration1424);
            	alias103 = alias();
            	state.followingStackPointer--;

            	stream_alias.Add(alias103.Tree);


            	// AST REWRITE
            	// elements:          alias, path
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (IASTNode)adaptor.GetNilNode();
            	// 273:6: -> ^( JOIN[\"join\"] INNER[\"inner\"] path alias )
            	{
            	    // Hql.g:273:9: ^( JOIN[\"join\"] INNER[\"inner\"] path alias )
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(JOIN, "join"), root_1);

            	    adaptor.AddChild(root_1, (IASTNode)adaptor.Create(INNER, "inner"));
            	    adaptor.AddChild(root_1, stream_path.NextTree());
            	    adaptor.AddChild(root_1, stream_alias.NextTree());

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;
            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "inCollectionDeclaration"

    public class inCollectionElementsDeclaration_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "inCollectionElementsDeclaration"
    // Hql.g:276:1: inCollectionElementsDeclaration : ( alias IN ELEMENTS OPEN path CLOSE -> ^( JOIN[\"join\"] INNER[\"inner\"] path alias ) | ELEMENTS OPEN path CLOSE AS alias -> ^( JOIN[\"join\"] INNER[\"inner\"] path alias ) );
    public HqlParser.inCollectionElementsDeclaration_return inCollectionElementsDeclaration() // throws RecognitionException [1]
    {   
        HqlParser.inCollectionElementsDeclaration_return retval = new HqlParser.inCollectionElementsDeclaration_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken IN105 = null;
        IToken ELEMENTS106 = null;
        IToken OPEN107 = null;
        IToken CLOSE109 = null;
        IToken ELEMENTS110 = null;
        IToken OPEN111 = null;
        IToken CLOSE113 = null;
        IToken AS114 = null;
        HqlParser.alias_return alias104 = default(HqlParser.alias_return);

        HqlParser.path_return path108 = default(HqlParser.path_return);

        HqlParser.path_return path112 = default(HqlParser.path_return);

        HqlParser.alias_return alias115 = default(HqlParser.alias_return);


        IASTNode IN105_tree=null;
        IASTNode ELEMENTS106_tree=null;
        IASTNode OPEN107_tree=null;
        IASTNode CLOSE109_tree=null;
        IASTNode ELEMENTS110_tree=null;
        IASTNode OPEN111_tree=null;
        IASTNode CLOSE113_tree=null;
        IASTNode AS114_tree=null;
        RewriteRuleTokenStream stream_OPEN = new RewriteRuleTokenStream(adaptor,"token OPEN");
        RewriteRuleTokenStream stream_AS = new RewriteRuleTokenStream(adaptor,"token AS");
        RewriteRuleTokenStream stream_IN = new RewriteRuleTokenStream(adaptor,"token IN");
        RewriteRuleTokenStream stream_CLOSE = new RewriteRuleTokenStream(adaptor,"token CLOSE");
        RewriteRuleTokenStream stream_ELEMENTS = new RewriteRuleTokenStream(adaptor,"token ELEMENTS");
        RewriteRuleSubtreeStream stream_alias = new RewriteRuleSubtreeStream(adaptor,"rule alias");
        RewriteRuleSubtreeStream stream_path = new RewriteRuleSubtreeStream(adaptor,"rule path");
        try 
    	{
            // Hql.g:277:2: ( alias IN ELEMENTS OPEN path CLOSE -> ^( JOIN[\"join\"] INNER[\"inner\"] path alias ) | ELEMENTS OPEN path CLOSE AS alias -> ^( JOIN[\"join\"] INNER[\"inner\"] path alias ) )
            int alt37 = 2;
            int LA37_0 = input.LA(1);

            if ( (LA37_0 == IDENT) )
            {
                alt37 = 1;
            }
            else if ( (LA37_0 == ELEMENTS) )
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
                    // Hql.g:277:4: alias IN ELEMENTS OPEN path CLOSE
                    {
                    	PushFollow(FOLLOW_alias_in_inCollectionElementsDeclaration1458);
                    	alias104 = alias();
                    	state.followingStackPointer--;

                    	stream_alias.Add(alias104.Tree);
                    	IN105=(IToken)Match(input,IN,FOLLOW_IN_in_inCollectionElementsDeclaration1460);  
                    	stream_IN.Add(IN105);

                    	ELEMENTS106=(IToken)Match(input,ELEMENTS,FOLLOW_ELEMENTS_in_inCollectionElementsDeclaration1462);  
                    	stream_ELEMENTS.Add(ELEMENTS106);

                    	OPEN107=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_inCollectionElementsDeclaration1464);  
                    	stream_OPEN.Add(OPEN107);

                    	PushFollow(FOLLOW_path_in_inCollectionElementsDeclaration1466);
                    	path108 = path();
                    	state.followingStackPointer--;

                    	stream_path.Add(path108.Tree);
                    	CLOSE109=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_inCollectionElementsDeclaration1468);  
                    	stream_CLOSE.Add(CLOSE109);



                    	// AST REWRITE
                    	// elements:          path, alias
                    	// token labels:      
                    	// rule labels:       retval
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

                    	root_0 = (IASTNode)adaptor.GetNilNode();
                    	// 278:3: -> ^( JOIN[\"join\"] INNER[\"inner\"] path alias )
                    	{
                    	    // Hql.g:278:6: ^( JOIN[\"join\"] INNER[\"inner\"] path alias )
                    	    {
                    	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
                    	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(JOIN, "join"), root_1);

                    	    adaptor.AddChild(root_1, (IASTNode)adaptor.Create(INNER, "inner"));
                    	    adaptor.AddChild(root_1, stream_path.NextTree());
                    	    adaptor.AddChild(root_1, stream_alias.NextTree());

                    	    adaptor.AddChild(root_0, root_1);
                    	    }

                    	}

                    	retval.Tree = root_0;retval.Tree = root_0;
                    }
                    break;
                case 2 :
                    // Hql.g:279:4: ELEMENTS OPEN path CLOSE AS alias
                    {
                    	ELEMENTS110=(IToken)Match(input,ELEMENTS,FOLLOW_ELEMENTS_in_inCollectionElementsDeclaration1490);  
                    	stream_ELEMENTS.Add(ELEMENTS110);

                    	OPEN111=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_inCollectionElementsDeclaration1492);  
                    	stream_OPEN.Add(OPEN111);

                    	PushFollow(FOLLOW_path_in_inCollectionElementsDeclaration1494);
                    	path112 = path();
                    	state.followingStackPointer--;

                    	stream_path.Add(path112.Tree);
                    	CLOSE113=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_inCollectionElementsDeclaration1496);  
                    	stream_CLOSE.Add(CLOSE113);

                    	AS114=(IToken)Match(input,AS,FOLLOW_AS_in_inCollectionElementsDeclaration1498);  
                    	stream_AS.Add(AS114);

                    	PushFollow(FOLLOW_alias_in_inCollectionElementsDeclaration1500);
                    	alias115 = alias();
                    	state.followingStackPointer--;

                    	stream_alias.Add(alias115.Tree);


                    	// AST REWRITE
                    	// elements:          path, alias
                    	// token labels:      
                    	// rule labels:       retval
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

                    	root_0 = (IASTNode)adaptor.GetNilNode();
                    	// 280:3: -> ^( JOIN[\"join\"] INNER[\"inner\"] path alias )
                    	{
                    	    // Hql.g:280:6: ^( JOIN[\"join\"] INNER[\"inner\"] path alias )
                    	    {
                    	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
                    	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(JOIN, "join"), root_1);

                    	    adaptor.AddChild(root_1, (IASTNode)adaptor.Create(INNER, "inner"));
                    	    adaptor.AddChild(root_1, stream_path.NextTree());
                    	    adaptor.AddChild(root_1, stream_alias.NextTree());

                    	    adaptor.AddChild(root_0, root_1);
                    	    }

                    	}

                    	retval.Tree = root_0;retval.Tree = root_0;
                    }
                    break;

            }
            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "inCollectionElementsDeclaration"

    public class asAlias_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "asAlias"
    // Hql.g:284:1: asAlias : ( AS )? alias ;
    public HqlParser.asAlias_return asAlias() // throws RecognitionException [1]
    {   
        HqlParser.asAlias_return retval = new HqlParser.asAlias_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken AS116 = null;
        HqlParser.alias_return alias117 = default(HqlParser.alias_return);


        IASTNode AS116_tree=null;

        try 
    	{
            // Hql.g:285:2: ( ( AS )? alias )
            // Hql.g:285:4: ( AS )? alias
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// Hql.g:285:4: ( AS )?
            	int alt38 = 2;
            	int LA38_0 = input.LA(1);

            	if ( (LA38_0 == AS) )
            	{
            	    alt38 = 1;
            	}
            	switch (alt38) 
            	{
            	    case 1 :
            	        // Hql.g:285:5: AS
            	        {
            	        	AS116=(IToken)Match(input,AS,FOLLOW_AS_in_asAlias1532); 

            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_alias_in_asAlias1537);
            	alias117 = alias();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, alias117.Tree);

            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "asAlias"

    public class alias_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "alias"
    // Hql.g:287:1: alias : i= identifier -> ^( ALIAS[$i.start] ) ;
    public HqlParser.alias_return alias() // throws RecognitionException [1]
    {   
        HqlParser.alias_return retval = new HqlParser.alias_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.identifier_return i = default(HqlParser.identifier_return);


        RewriteRuleSubtreeStream stream_identifier = new RewriteRuleSubtreeStream(adaptor,"rule identifier");
        try 
    	{
            // Hql.g:288:2: (i= identifier -> ^( ALIAS[$i.start] ) )
            // Hql.g:288:4: i= identifier
            {
            	PushFollow(FOLLOW_identifier_in_alias1549);
            	i = identifier();
            	state.followingStackPointer--;

            	stream_identifier.Add(i.Tree);


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
            	// 289:2: -> ^( ALIAS[$i.start] )
            	{
            	    // Hql.g:289:5: ^( ALIAS[$i.start] )
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(ALIAS, ((i != null) ? ((IToken)i.Start) : null)), root_1);

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;
            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "alias"

    public class propertyFetch_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "propertyFetch"
    // Hql.g:292:1: propertyFetch : FETCH ALL PROPERTIES ;
    public HqlParser.propertyFetch_return propertyFetch() // throws RecognitionException [1]
    {   
        HqlParser.propertyFetch_return retval = new HqlParser.propertyFetch_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken FETCH118 = null;
        IToken ALL119 = null;
        IToken PROPERTIES120 = null;

        IASTNode FETCH118_tree=null;
        IASTNode ALL119_tree=null;
        IASTNode PROPERTIES120_tree=null;

        try 
    	{
            // Hql.g:293:2: ( FETCH ALL PROPERTIES )
            // Hql.g:293:4: FETCH ALL PROPERTIES
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	FETCH118=(IToken)Match(input,FETCH,FOLLOW_FETCH_in_propertyFetch1568); 
            		FETCH118_tree = (IASTNode)adaptor.Create(FETCH118);
            		adaptor.AddChild(root_0, FETCH118_tree);

            	ALL119=(IToken)Match(input,ALL,FOLLOW_ALL_in_propertyFetch1570); 
            	PROPERTIES120=(IToken)Match(input,PROPERTIES,FOLLOW_PROPERTIES_in_propertyFetch1573); 

            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "propertyFetch"

    public class groupByClause_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "groupByClause"
    // Hql.g:296:1: groupByClause : GROUP 'by' expression ( COMMA expression )* ;
    public HqlParser.groupByClause_return groupByClause() // throws RecognitionException [1]
    {   
        HqlParser.groupByClause_return retval = new HqlParser.groupByClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken GROUP121 = null;
        IToken string_literal122 = null;
        IToken COMMA124 = null;
        HqlParser.expression_return expression123 = default(HqlParser.expression_return);

        HqlParser.expression_return expression125 = default(HqlParser.expression_return);


        IASTNode GROUP121_tree=null;
        IASTNode string_literal122_tree=null;
        IASTNode COMMA124_tree=null;

        try 
    	{
            // Hql.g:297:2: ( GROUP 'by' expression ( COMMA expression )* )
            // Hql.g:297:4: GROUP 'by' expression ( COMMA expression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	GROUP121=(IToken)Match(input,GROUP,FOLLOW_GROUP_in_groupByClause1585); 
            		GROUP121_tree = (IASTNode)adaptor.Create(GROUP121);
            		root_0 = (IASTNode)adaptor.BecomeRoot(GROUP121_tree, root_0);

            	string_literal122=(IToken)Match(input,LITERAL_by,FOLLOW_LITERAL_by_in_groupByClause1591); 
            	PushFollow(FOLLOW_expression_in_groupByClause1594);
            	expression123 = expression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, expression123.Tree);
            	// Hql.g:298:20: ( COMMA expression )*
            	do 
            	{
            	    int alt39 = 2;
            	    int LA39_0 = input.LA(1);

            	    if ( (LA39_0 == COMMA) )
            	    {
            	        alt39 = 1;
            	    }


            	    switch (alt39) 
            		{
            			case 1 :
            			    // Hql.g:298:22: COMMA expression
            			    {
            			    	COMMA124=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_groupByClause1598); 
            			    	PushFollow(FOLLOW_expression_in_groupByClause1601);
            			    	expression125 = expression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, expression125.Tree);

            			    }
            			    break;

            			default:
            			    goto loop39;
            	    }
            	} while (true);

            	loop39:
            		;	// Stops C# compiler whining that label 'loop39' has no statements


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "groupByClause"

    public class orderByClause_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "orderByClause"
    // Hql.g:301:1: orderByClause : ORDER 'by' orderElement ( COMMA orderElement )* ;
    public HqlParser.orderByClause_return orderByClause() // throws RecognitionException [1]
    {   
        HqlParser.orderByClause_return retval = new HqlParser.orderByClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken ORDER126 = null;
        IToken string_literal127 = null;
        IToken COMMA129 = null;
        HqlParser.orderElement_return orderElement128 = default(HqlParser.orderElement_return);

        HqlParser.orderElement_return orderElement130 = default(HqlParser.orderElement_return);


        IASTNode ORDER126_tree=null;
        IASTNode string_literal127_tree=null;
        IASTNode COMMA129_tree=null;

        try 
    	{
            // Hql.g:302:2: ( ORDER 'by' orderElement ( COMMA orderElement )* )
            // Hql.g:302:4: ORDER 'by' orderElement ( COMMA orderElement )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	ORDER126=(IToken)Match(input,ORDER,FOLLOW_ORDER_in_orderByClause1615); 
            		ORDER126_tree = (IASTNode)adaptor.Create(ORDER126);
            		root_0 = (IASTNode)adaptor.BecomeRoot(ORDER126_tree, root_0);

            	string_literal127=(IToken)Match(input,LITERAL_by,FOLLOW_LITERAL_by_in_orderByClause1618); 
            	PushFollow(FOLLOW_orderElement_in_orderByClause1621);
            	orderElement128 = orderElement();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, orderElement128.Tree);
            	// Hql.g:302:30: ( COMMA orderElement )*
            	do 
            	{
            	    int alt40 = 2;
            	    int LA40_0 = input.LA(1);

            	    if ( (LA40_0 == COMMA) )
            	    {
            	        alt40 = 1;
            	    }


            	    switch (alt40) 
            		{
            			case 1 :
            			    // Hql.g:302:32: COMMA orderElement
            			    {
            			    	COMMA129=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_orderByClause1625); 
            			    	PushFollow(FOLLOW_orderElement_in_orderByClause1628);
            			    	orderElement130 = orderElement();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, orderElement130.Tree);

            			    }
            			    break;

            			default:
            			    goto loop40;
            	    }
            	} while (true);

            	loop40:
            		;	// Stops C# compiler whining that label 'loop40' has no statements


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "orderByClause"

    public class skipClause_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "skipClause"
    // Hql.g:305:1: skipClause : SKIP NUM_INT ;
    public HqlParser.skipClause_return skipClause() // throws RecognitionException [1]
    {   
        HqlParser.skipClause_return retval = new HqlParser.skipClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken SKIP131 = null;
        IToken NUM_INT132 = null;

        IASTNode SKIP131_tree=null;
        IASTNode NUM_INT132_tree=null;

        try 
    	{
            // Hql.g:306:2: ( SKIP NUM_INT )
            // Hql.g:306:4: SKIP NUM_INT
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	SKIP131=(IToken)Match(input,SKIP,FOLLOW_SKIP_in_skipClause1642); 
            		SKIP131_tree = (IASTNode)adaptor.Create(SKIP131);
            		root_0 = (IASTNode)adaptor.BecomeRoot(SKIP131_tree, root_0);

            	NUM_INT132=(IToken)Match(input,NUM_INT,FOLLOW_NUM_INT_in_skipClause1645); 
            		NUM_INT132_tree = (IASTNode)adaptor.Create(NUM_INT132);
            		adaptor.AddChild(root_0, NUM_INT132_tree);


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "skipClause"

    public class takeClause_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "takeClause"
    // Hql.g:309:1: takeClause : TAKE NUM_INT ;
    public HqlParser.takeClause_return takeClause() // throws RecognitionException [1]
    {   
        HqlParser.takeClause_return retval = new HqlParser.takeClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken TAKE133 = null;
        IToken NUM_INT134 = null;

        IASTNode TAKE133_tree=null;
        IASTNode NUM_INT134_tree=null;

        try 
    	{
            // Hql.g:310:2: ( TAKE NUM_INT )
            // Hql.g:310:4: TAKE NUM_INT
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	TAKE133=(IToken)Match(input,TAKE,FOLLOW_TAKE_in_takeClause1656); 
            		TAKE133_tree = (IASTNode)adaptor.Create(TAKE133);
            		root_0 = (IASTNode)adaptor.BecomeRoot(TAKE133_tree, root_0);

            	NUM_INT134=(IToken)Match(input,NUM_INT,FOLLOW_NUM_INT_in_takeClause1659); 
            		NUM_INT134_tree = (IASTNode)adaptor.Create(NUM_INT134);
            		adaptor.AddChild(root_0, NUM_INT134_tree);


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "takeClause"

    public class orderElement_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "orderElement"
    // Hql.g:313:1: orderElement : expression ( ascendingOrDescending )? ;
    public HqlParser.orderElement_return orderElement() // throws RecognitionException [1]
    {   
        HqlParser.orderElement_return retval = new HqlParser.orderElement_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.expression_return expression135 = default(HqlParser.expression_return);

        HqlParser.ascendingOrDescending_return ascendingOrDescending136 = default(HqlParser.ascendingOrDescending_return);



        try 
    	{
            // Hql.g:314:2: ( expression ( ascendingOrDescending )? )
            // Hql.g:314:4: expression ( ascendingOrDescending )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_expression_in_orderElement1670);
            	expression135 = expression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, expression135.Tree);
            	// Hql.g:314:15: ( ascendingOrDescending )?
            	int alt41 = 2;
            	int LA41_0 = input.LA(1);

            	if ( (LA41_0 == ASCENDING || LA41_0 == DESCENDING || (LA41_0 >= 133 && LA41_0 <= 134)) )
            	{
            	    alt41 = 1;
            	}
            	switch (alt41) 
            	{
            	    case 1 :
            	        // Hql.g:314:17: ascendingOrDescending
            	        {
            	        	PushFollow(FOLLOW_ascendingOrDescending_in_orderElement1674);
            	        	ascendingOrDescending136 = ascendingOrDescending();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, ascendingOrDescending136.Tree);

            	        }
            	        break;

            	}


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "orderElement"

    public class ascendingOrDescending_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "ascendingOrDescending"
    // Hql.g:317:1: ascendingOrDescending : ( (a= 'asc' | a= 'ascending' ) -> ^( ASCENDING[$a.Text] ) | (d= 'desc' | d= 'descending' ) -> ^( DESCENDING[$d.Text] ) );
    public HqlParser.ascendingOrDescending_return ascendingOrDescending() // throws RecognitionException [1]
    {   
        HqlParser.ascendingOrDescending_return retval = new HqlParser.ascendingOrDescending_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken a = null;
        IToken d = null;

        IASTNode a_tree=null;
        IASTNode d_tree=null;
        RewriteRuleTokenStream stream_134 = new RewriteRuleTokenStream(adaptor,"token 134");
        RewriteRuleTokenStream stream_133 = new RewriteRuleTokenStream(adaptor,"token 133");
        RewriteRuleTokenStream stream_DESCENDING = new RewriteRuleTokenStream(adaptor,"token DESCENDING");
        RewriteRuleTokenStream stream_ASCENDING = new RewriteRuleTokenStream(adaptor,"token ASCENDING");

        try 
    	{
            // Hql.g:318:2: ( (a= 'asc' | a= 'ascending' ) -> ^( ASCENDING[$a.Text] ) | (d= 'desc' | d= 'descending' ) -> ^( DESCENDING[$d.Text] ) )
            int alt44 = 2;
            int LA44_0 = input.LA(1);

            if ( (LA44_0 == ASCENDING || LA44_0 == 133) )
            {
                alt44 = 1;
            }
            else if ( (LA44_0 == DESCENDING || LA44_0 == 134) )
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
                    // Hql.g:318:4: (a= 'asc' | a= 'ascending' )
                    {
                    	// Hql.g:318:4: (a= 'asc' | a= 'ascending' )
                    	int alt42 = 2;
                    	int LA42_0 = input.LA(1);

                    	if ( (LA42_0 == ASCENDING) )
                    	{
                    	    alt42 = 1;
                    	}
                    	else if ( (LA42_0 == 133) )
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
                    	        // Hql.g:318:6: a= 'asc'
                    	        {
                    	        	a=(IToken)Match(input,ASCENDING,FOLLOW_ASCENDING_in_ascendingOrDescending1692);  
                    	        	stream_ASCENDING.Add(a);


                    	        }
                    	        break;
                    	    case 2 :
                    	        // Hql.g:318:16: a= 'ascending'
                    	        {
                    	        	a=(IToken)Match(input,133,FOLLOW_133_in_ascendingOrDescending1698);  
                    	        	stream_133.Add(a);


                    	        }
                    	        break;

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
                    	// 319:3: -> ^( ASCENDING[$a.Text] )
                    	{
                    	    // Hql.g:319:6: ^( ASCENDING[$a.Text] )
                    	    {
                    	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
                    	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(ASCENDING, a.Text), root_1);

                    	    adaptor.AddChild(root_0, root_1);
                    	    }

                    	}

                    	retval.Tree = root_0;retval.Tree = root_0;
                    }
                    break;
                case 2 :
                    // Hql.g:320:4: (d= 'desc' | d= 'descending' )
                    {
                    	// Hql.g:320:4: (d= 'desc' | d= 'descending' )
                    	int alt43 = 2;
                    	int LA43_0 = input.LA(1);

                    	if ( (LA43_0 == DESCENDING) )
                    	{
                    	    alt43 = 1;
                    	}
                    	else if ( (LA43_0 == 134) )
                    	{
                    	    alt43 = 2;
                    	}
                    	else 
                    	{
                    	    NoViableAltException nvae_d43s0 =
                    	        new NoViableAltException("", 43, 0, input);

                    	    throw nvae_d43s0;
                    	}
                    	switch (alt43) 
                    	{
                    	    case 1 :
                    	        // Hql.g:320:6: d= 'desc'
                    	        {
                    	        	d=(IToken)Match(input,DESCENDING,FOLLOW_DESCENDING_in_ascendingOrDescending1718);  
                    	        	stream_DESCENDING.Add(d);


                    	        }
                    	        break;
                    	    case 2 :
                    	        // Hql.g:320:17: d= 'descending'
                    	        {
                    	        	d=(IToken)Match(input,134,FOLLOW_134_in_ascendingOrDescending1724);  
                    	        	stream_134.Add(d);


                    	        }
                    	        break;

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
                    	// 321:3: -> ^( DESCENDING[$d.Text] )
                    	{
                    	    // Hql.g:321:6: ^( DESCENDING[$d.Text] )
                    	    {
                    	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
                    	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(DESCENDING, d.Text), root_1);

                    	    adaptor.AddChild(root_0, root_1);
                    	    }

                    	}

                    	retval.Tree = root_0;retval.Tree = root_0;
                    }
                    break;

            }
            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "ascendingOrDescending"

    public class havingClause_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "havingClause"
    // Hql.g:324:1: havingClause : HAVING logicalExpression ;
    public HqlParser.havingClause_return havingClause() // throws RecognitionException [1]
    {   
        HqlParser.havingClause_return retval = new HqlParser.havingClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken HAVING137 = null;
        HqlParser.logicalExpression_return logicalExpression138 = default(HqlParser.logicalExpression_return);


        IASTNode HAVING137_tree=null;

        try 
    	{
            // Hql.g:325:2: ( HAVING logicalExpression )
            // Hql.g:325:4: HAVING logicalExpression
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	HAVING137=(IToken)Match(input,HAVING,FOLLOW_HAVING_in_havingClause1745); 
            		HAVING137_tree = (IASTNode)adaptor.Create(HAVING137);
            		root_0 = (IASTNode)adaptor.BecomeRoot(HAVING137_tree, root_0);

            	PushFollow(FOLLOW_logicalExpression_in_havingClause1748);
            	logicalExpression138 = logicalExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, logicalExpression138.Tree);

            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "havingClause"

    public class whereClause_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "whereClause"
    // Hql.g:328:1: whereClause : WHERE logicalExpression ;
    public HqlParser.whereClause_return whereClause() // throws RecognitionException [1]
    {   
        HqlParser.whereClause_return retval = new HqlParser.whereClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken WHERE139 = null;
        HqlParser.logicalExpression_return logicalExpression140 = default(HqlParser.logicalExpression_return);


        IASTNode WHERE139_tree=null;

        try 
    	{
            // Hql.g:329:2: ( WHERE logicalExpression )
            // Hql.g:329:4: WHERE logicalExpression
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	WHERE139=(IToken)Match(input,WHERE,FOLLOW_WHERE_in_whereClause1759); 
            		WHERE139_tree = (IASTNode)adaptor.Create(WHERE139);
            		root_0 = (IASTNode)adaptor.BecomeRoot(WHERE139_tree, root_0);

            	PushFollow(FOLLOW_logicalExpression_in_whereClause1762);
            	logicalExpression140 = logicalExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, logicalExpression140.Tree);

            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "whereClause"

    public class selectedPropertiesList_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "selectedPropertiesList"
    // Hql.g:332:1: selectedPropertiesList : aliasedExpression ( COMMA aliasedExpression )* ;
    public HqlParser.selectedPropertiesList_return selectedPropertiesList() // throws RecognitionException [1]
    {   
        HqlParser.selectedPropertiesList_return retval = new HqlParser.selectedPropertiesList_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken COMMA142 = null;
        HqlParser.aliasedExpression_return aliasedExpression141 = default(HqlParser.aliasedExpression_return);

        HqlParser.aliasedExpression_return aliasedExpression143 = default(HqlParser.aliasedExpression_return);


        IASTNode COMMA142_tree=null;

        try 
    	{
            // Hql.g:333:2: ( aliasedExpression ( COMMA aliasedExpression )* )
            // Hql.g:333:4: aliasedExpression ( COMMA aliasedExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_aliasedExpression_in_selectedPropertiesList1773);
            	aliasedExpression141 = aliasedExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, aliasedExpression141.Tree);
            	// Hql.g:333:22: ( COMMA aliasedExpression )*
            	do 
            	{
            	    int alt45 = 2;
            	    int LA45_0 = input.LA(1);

            	    if ( (LA45_0 == COMMA) )
            	    {
            	        alt45 = 1;
            	    }


            	    switch (alt45) 
            		{
            			case 1 :
            			    // Hql.g:333:24: COMMA aliasedExpression
            			    {
            			    	COMMA142=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_selectedPropertiesList1777); 
            			    	PushFollow(FOLLOW_aliasedExpression_in_selectedPropertiesList1780);
            			    	aliasedExpression143 = aliasedExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, aliasedExpression143.Tree);

            			    }
            			    break;

            			default:
            			    goto loop45;
            	    }
            	} while (true);

            	loop45:
            		;	// Stops C# compiler whining that label 'loop45' has no statements


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "selectedPropertiesList"

    public class aliasedExpression_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "aliasedExpression"
    // Hql.g:336:1: aliasedExpression : expression ( AS identifier )? ;
    public HqlParser.aliasedExpression_return aliasedExpression() // throws RecognitionException [1]
    {   
        HqlParser.aliasedExpression_return retval = new HqlParser.aliasedExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken AS145 = null;
        HqlParser.expression_return expression144 = default(HqlParser.expression_return);

        HqlParser.identifier_return identifier146 = default(HqlParser.identifier_return);


        IASTNode AS145_tree=null;

        try 
    	{
            // Hql.g:337:2: ( expression ( AS identifier )? )
            // Hql.g:337:4: expression ( AS identifier )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_expression_in_aliasedExpression1795);
            	expression144 = expression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, expression144.Tree);
            	// Hql.g:337:15: ( AS identifier )?
            	int alt46 = 2;
            	int LA46_0 = input.LA(1);

            	if ( (LA46_0 == AS) )
            	{
            	    alt46 = 1;
            	}
            	switch (alt46) 
            	{
            	    case 1 :
            	        // Hql.g:337:17: AS identifier
            	        {
            	        	AS145=(IToken)Match(input,AS,FOLLOW_AS_in_aliasedExpression1799); 
            	        		AS145_tree = (IASTNode)adaptor.Create(AS145);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(AS145_tree, root_0);

            	        	PushFollow(FOLLOW_identifier_in_aliasedExpression1802);
            	        	identifier146 = identifier();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, identifier146.Tree);

            	        }
            	        break;

            	}


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "aliasedExpression"

    public class logicalExpression_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "logicalExpression"
    // Hql.g:365:1: logicalExpression : expression ;
    public HqlParser.logicalExpression_return logicalExpression() // throws RecognitionException [1]
    {   
        HqlParser.logicalExpression_return retval = new HqlParser.logicalExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.expression_return expression147 = default(HqlParser.expression_return);



        try 
    	{
            // Hql.g:366:2: ( expression )
            // Hql.g:366:4: expression
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_expression_in_logicalExpression1841);
            	expression147 = expression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, expression147.Tree);

            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "logicalExpression"

    public class expression_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "expression"
    // Hql.g:370:1: expression : logicalOrExpression ;
    public HqlParser.expression_return expression() // throws RecognitionException [1]
    {   
        HqlParser.expression_return retval = new HqlParser.expression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.logicalOrExpression_return logicalOrExpression148 = default(HqlParser.logicalOrExpression_return);



        try 
    	{
            // Hql.g:371:2: ( logicalOrExpression )
            // Hql.g:371:4: logicalOrExpression
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_logicalOrExpression_in_expression1853);
            	logicalOrExpression148 = logicalOrExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, logicalOrExpression148.Tree);

            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "expression"

    public class logicalOrExpression_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "logicalOrExpression"
    // Hql.g:375:1: logicalOrExpression : logicalAndExpression ( OR logicalAndExpression )* ;
    public HqlParser.logicalOrExpression_return logicalOrExpression() // throws RecognitionException [1]
    {   
        HqlParser.logicalOrExpression_return retval = new HqlParser.logicalOrExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken OR150 = null;
        HqlParser.logicalAndExpression_return logicalAndExpression149 = default(HqlParser.logicalAndExpression_return);

        HqlParser.logicalAndExpression_return logicalAndExpression151 = default(HqlParser.logicalAndExpression_return);


        IASTNode OR150_tree=null;

        try 
    	{
            // Hql.g:376:2: ( logicalAndExpression ( OR logicalAndExpression )* )
            // Hql.g:376:4: logicalAndExpression ( OR logicalAndExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_logicalAndExpression_in_logicalOrExpression1865);
            	logicalAndExpression149 = logicalAndExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, logicalAndExpression149.Tree);
            	// Hql.g:376:25: ( OR logicalAndExpression )*
            	do 
            	{
            	    int alt47 = 2;
            	    int LA47_0 = input.LA(1);

            	    if ( (LA47_0 == OR) )
            	    {
            	        alt47 = 1;
            	    }


            	    switch (alt47) 
            		{
            			case 1 :
            			    // Hql.g:376:27: OR logicalAndExpression
            			    {
            			    	OR150=(IToken)Match(input,OR,FOLLOW_OR_in_logicalOrExpression1869); 
            			    		OR150_tree = (IASTNode)adaptor.Create(OR150);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(OR150_tree, root_0);

            			    	PushFollow(FOLLOW_logicalAndExpression_in_logicalOrExpression1872);
            			    	logicalAndExpression151 = logicalAndExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, logicalAndExpression151.Tree);

            			    }
            			    break;

            			default:
            			    goto loop47;
            	    }
            	} while (true);

            	loop47:
            		;	// Stops C# compiler whining that label 'loop47' has no statements


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "logicalOrExpression"

    public class logicalAndExpression_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "logicalAndExpression"
    // Hql.g:380:1: logicalAndExpression : negatedExpression ( AND negatedExpression )* ;
    public HqlParser.logicalAndExpression_return logicalAndExpression() // throws RecognitionException [1]
    {   
        HqlParser.logicalAndExpression_return retval = new HqlParser.logicalAndExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken AND153 = null;
        HqlParser.negatedExpression_return negatedExpression152 = default(HqlParser.negatedExpression_return);

        HqlParser.negatedExpression_return negatedExpression154 = default(HqlParser.negatedExpression_return);


        IASTNode AND153_tree=null;

        try 
    	{
            // Hql.g:381:2: ( negatedExpression ( AND negatedExpression )* )
            // Hql.g:381:4: negatedExpression ( AND negatedExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_negatedExpression_in_logicalAndExpression1887);
            	negatedExpression152 = negatedExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, negatedExpression152.Tree);
            	// Hql.g:381:22: ( AND negatedExpression )*
            	do 
            	{
            	    int alt48 = 2;
            	    int LA48_0 = input.LA(1);

            	    if ( (LA48_0 == AND) )
            	    {
            	        alt48 = 1;
            	    }


            	    switch (alt48) 
            		{
            			case 1 :
            			    // Hql.g:381:24: AND negatedExpression
            			    {
            			    	AND153=(IToken)Match(input,AND,FOLLOW_AND_in_logicalAndExpression1891); 
            			    		AND153_tree = (IASTNode)adaptor.Create(AND153);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(AND153_tree, root_0);

            			    	PushFollow(FOLLOW_negatedExpression_in_logicalAndExpression1894);
            			    	negatedExpression154 = negatedExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, negatedExpression154.Tree);

            			    }
            			    break;

            			default:
            			    goto loop48;
            	    }
            	} while (true);

            	loop48:
            		;	// Stops C# compiler whining that label 'loop48' has no statements


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "logicalAndExpression"

    public class negatedExpression_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "negatedExpression"
    // Hql.g:386:1: negatedExpression : ( NOT x= negatedExpression -> ^() | equalityExpression -> ^( equalityExpression ) );
    public HqlParser.negatedExpression_return negatedExpression() // throws RecognitionException [1]
    {   
        HqlParser.negatedExpression_return retval = new HqlParser.negatedExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken NOT155 = null;
        HqlParser.negatedExpression_return x = default(HqlParser.negatedExpression_return);

        HqlParser.equalityExpression_return equalityExpression156 = default(HqlParser.equalityExpression_return);


        IASTNode NOT155_tree=null;
        RewriteRuleTokenStream stream_NOT = new RewriteRuleTokenStream(adaptor,"token NOT");
        RewriteRuleSubtreeStream stream_equalityExpression = new RewriteRuleSubtreeStream(adaptor,"rule equalityExpression");
        RewriteRuleSubtreeStream stream_negatedExpression = new RewriteRuleSubtreeStream(adaptor,"rule negatedExpression");
         WeakKeywords(); 
        try 
    	{
            // Hql.g:388:2: ( NOT x= negatedExpression -> ^() | equalityExpression -> ^( equalityExpression ) )
            int alt49 = 2;
            int LA49_0 = input.LA(1);

            if ( (LA49_0 == NOT) )
            {
                alt49 = 1;
            }
            else if ( ((LA49_0 >= ALL && LA49_0 <= ANY) || LA49_0 == AVG || LA49_0 == COUNT || LA49_0 == ELEMENTS || (LA49_0 >= EXISTS && LA49_0 <= FALSE) || LA49_0 == INDICES || (LA49_0 >= MAX && LA49_0 <= MIN) || LA49_0 == NULL || (LA49_0 >= SOME && LA49_0 <= SUM) || LA49_0 == TRUE || LA49_0 == CASE || LA49_0 == EMPTY || (LA49_0 >= NUM_INT && LA49_0 <= NUM_LONG) || LA49_0 == OPEN || LA49_0 == BNOT || (LA49_0 >= PLUS && LA49_0 <= MINUS) || (LA49_0 >= COLON && LA49_0 <= IDENT)) )
            {
                alt49 = 2;
            }
            else 
            {
                NoViableAltException nvae_d49s0 =
                    new NoViableAltException("", 49, 0, input);

                throw nvae_d49s0;
            }
            switch (alt49) 
            {
                case 1 :
                    // Hql.g:388:4: NOT x= negatedExpression
                    {
                    	NOT155=(IToken)Match(input,NOT,FOLLOW_NOT_in_negatedExpression1915);  
                    	stream_NOT.Add(NOT155);

                    	PushFollow(FOLLOW_negatedExpression_in_negatedExpression1919);
                    	x = negatedExpression();
                    	state.followingStackPointer--;

                    	stream_negatedExpression.Add(x.Tree);


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
                    	// 389:3: -> ^()
                    	{
                    	    // Hql.g:389:6: ^()
                    	    {
                    	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
                    	    root_1 = (IASTNode)adaptor.BecomeRoot(NegateNode(((x != null) ? ((IASTNode)x.Tree) : null)), root_1);

                    	    adaptor.AddChild(root_0, root_1);
                    	    }

                    	}

                    	retval.Tree = root_0;retval.Tree = root_0;
                    }
                    break;
                case 2 :
                    // Hql.g:390:4: equalityExpression
                    {
                    	PushFollow(FOLLOW_equalityExpression_in_negatedExpression1932);
                    	equalityExpression156 = equalityExpression();
                    	state.followingStackPointer--;

                    	stream_equalityExpression.Add(equalityExpression156.Tree);


                    	// AST REWRITE
                    	// elements:          equalityExpression
                    	// token labels:      
                    	// rule labels:       retval
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

                    	root_0 = (IASTNode)adaptor.GetNilNode();
                    	// 391:3: -> ^( equalityExpression )
                    	{
                    	    // Hql.g:391:6: ^( equalityExpression )
                    	    {
                    	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
                    	    root_1 = (IASTNode)adaptor.BecomeRoot(stream_equalityExpression.NextNode(), root_1);

                    	    adaptor.AddChild(root_0, root_1);
                    	    }

                    	}

                    	retval.Tree = root_0;retval.Tree = root_0;
                    }
                    break;

            }
            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "negatedExpression"

    public class equalityExpression_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "equalityExpression"
    // Hql.g:397:1: equalityExpression : x= relationalExpression ( ( EQ | isx= IS ( NOT )? | NE | ne= SQL_NE ) y= relationalExpression )* ;
    public HqlParser.equalityExpression_return equalityExpression() // throws RecognitionException [1]
    {   
        HqlParser.equalityExpression_return retval = new HqlParser.equalityExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken isx = null;
        IToken ne = null;
        IToken EQ157 = null;
        IToken NOT158 = null;
        IToken NE159 = null;
        HqlParser.relationalExpression_return x = default(HqlParser.relationalExpression_return);

        HqlParser.relationalExpression_return y = default(HqlParser.relationalExpression_return);


        IASTNode isx_tree=null;
        IASTNode ne_tree=null;
        IASTNode EQ157_tree=null;
        IASTNode NOT158_tree=null;
        IASTNode NE159_tree=null;

        try 
    	{
            // Hql.g:402:2: (x= relationalExpression ( ( EQ | isx= IS ( NOT )? | NE | ne= SQL_NE ) y= relationalExpression )* )
            // Hql.g:402:4: x= relationalExpression ( ( EQ | isx= IS ( NOT )? | NE | ne= SQL_NE ) y= relationalExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_relationalExpression_in_equalityExpression1962);
            	x = relationalExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, x.Tree);
            	// Hql.g:402:27: ( ( EQ | isx= IS ( NOT )? | NE | ne= SQL_NE ) y= relationalExpression )*
            	do 
            	{
            	    int alt52 = 2;
            	    int LA52_0 = input.LA(1);

            	    if ( (LA52_0 == IS || LA52_0 == EQ || (LA52_0 >= NE && LA52_0 <= SQL_NE)) )
            	    {
            	        alt52 = 1;
            	    }


            	    switch (alt52) 
            		{
            			case 1 :
            			    // Hql.g:403:3: ( EQ | isx= IS ( NOT )? | NE | ne= SQL_NE ) y= relationalExpression
            			    {
            			    	// Hql.g:403:3: ( EQ | isx= IS ( NOT )? | NE | ne= SQL_NE )
            			    	int alt51 = 4;
            			    	switch ( input.LA(1) ) 
            			    	{
            			    	case EQ:
            			    		{
            			    	    alt51 = 1;
            			    	    }
            			    	    break;
            			    	case IS:
            			    		{
            			    	    alt51 = 2;
            			    	    }
            			    	    break;
            			    	case NE:
            			    		{
            			    	    alt51 = 3;
            			    	    }
            			    	    break;
            			    	case SQL_NE:
            			    		{
            			    	    alt51 = 4;
            			    	    }
            			    	    break;
            			    		default:
            			    		    NoViableAltException nvae_d51s0 =
            			    		        new NoViableAltException("", 51, 0, input);

            			    		    throw nvae_d51s0;
            			    	}

            			    	switch (alt51) 
            			    	{
            			    	    case 1 :
            			    	        // Hql.g:403:5: EQ
            			    	        {
            			    	        	EQ157=(IToken)Match(input,EQ,FOLLOW_EQ_in_equalityExpression1970); 
            			    	        		EQ157_tree = (IASTNode)adaptor.Create(EQ157);
            			    	        		root_0 = (IASTNode)adaptor.BecomeRoot(EQ157_tree, root_0);


            			    	        }
            			    	        break;
            			    	    case 2 :
            			    	        // Hql.g:404:5: isx= IS ( NOT )?
            			    	        {
            			    	        	isx=(IToken)Match(input,IS,FOLLOW_IS_in_equalityExpression1979); 
            			    	        		isx_tree = (IASTNode)adaptor.Create(isx);
            			    	        		root_0 = (IASTNode)adaptor.BecomeRoot(isx_tree, root_0);

            			    	        	 isx.Type = EQ; 
            			    	        	// Hql.g:404:33: ( NOT )?
            			    	        	int alt50 = 2;
            			    	        	int LA50_0 = input.LA(1);

            			    	        	if ( (LA50_0 == NOT) )
            			    	        	{
            			    	        	    alt50 = 1;
            			    	        	}
            			    	        	switch (alt50) 
            			    	        	{
            			    	        	    case 1 :
            			    	        	        // Hql.g:404:34: NOT
            			    	        	        {
            			    	        	        	NOT158=(IToken)Match(input,NOT,FOLLOW_NOT_in_equalityExpression1985); 
            			    	        	        	 isx.Type =NE; 

            			    	        	        }
            			    	        	        break;

            			    	        	}


            			    	        }
            			    	        break;
            			    	    case 3 :
            			    	        // Hql.g:405:5: NE
            			    	        {
            			    	        	NE159=(IToken)Match(input,NE,FOLLOW_NE_in_equalityExpression1997); 
            			    	        		NE159_tree = (IASTNode)adaptor.Create(NE159);
            			    	        		root_0 = (IASTNode)adaptor.BecomeRoot(NE159_tree, root_0);


            			    	        }
            			    	        break;
            			    	    case 4 :
            			    	        // Hql.g:406:5: ne= SQL_NE
            			    	        {
            			    	        	ne=(IToken)Match(input,SQL_NE,FOLLOW_SQL_NE_in_equalityExpression2006); 
            			    	        		ne_tree = (IASTNode)adaptor.Create(ne);
            			    	        		root_0 = (IASTNode)adaptor.BecomeRoot(ne_tree, root_0);

            			    	        	 ne.Type = NE; 

            			    	        }
            			    	        break;

            			    	}

            			    	PushFollow(FOLLOW_relationalExpression_in_equalityExpression2017);
            			    	y = relationalExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, y.Tree);

            			    }
            			    break;

            			default:
            			    goto loop52;
            	    }
            	} while (true);

            	loop52:
            		;	// Stops C# compiler whining that label 'loop52' has no statements


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);

            			// Post process the equality expression to clean up 'is null', etc.
            			retval.Tree =  ProcessEqualityExpression(((IASTNode)retval.Tree));
            		
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "equalityExpression"

    public class relationalExpression_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "relationalExpression"
    // Hql.g:414:1: relationalExpression : concatenation ( ( ( ( LT | GT | LE | GE ) bitwiseNotExpression )* ) | (n= NOT )? ( (i= IN inList ) | (b= BETWEEN betweenList ) | (l= LIKE concatenation likeEscape ) | ( MEMBER ( OF )? p= path ) ) ) ;
    public HqlParser.relationalExpression_return relationalExpression() // throws RecognitionException [1]
    {   
        HqlParser.relationalExpression_return retval = new HqlParser.relationalExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken n = null;
        IToken i = null;
        IToken b = null;
        IToken l = null;
        IToken LT161 = null;
        IToken GT162 = null;
        IToken LE163 = null;
        IToken GE164 = null;
        IToken MEMBER170 = null;
        IToken OF171 = null;
        HqlParser.path_return p = default(HqlParser.path_return);

        HqlParser.concatenation_return concatenation160 = default(HqlParser.concatenation_return);

        HqlParser.bitwiseNotExpression_return bitwiseNotExpression165 = default(HqlParser.bitwiseNotExpression_return);

        HqlParser.inList_return inList166 = default(HqlParser.inList_return);

        HqlParser.betweenList_return betweenList167 = default(HqlParser.betweenList_return);

        HqlParser.concatenation_return concatenation168 = default(HqlParser.concatenation_return);

        HqlParser.likeEscape_return likeEscape169 = default(HqlParser.likeEscape_return);


        IASTNode n_tree=null;
        IASTNode i_tree=null;
        IASTNode b_tree=null;
        IASTNode l_tree=null;
        IASTNode LT161_tree=null;
        IASTNode GT162_tree=null;
        IASTNode LE163_tree=null;
        IASTNode GE164_tree=null;
        IASTNode MEMBER170_tree=null;
        IASTNode OF171_tree=null;

        try 
    	{
            // Hql.g:415:2: ( concatenation ( ( ( ( LT | GT | LE | GE ) bitwiseNotExpression )* ) | (n= NOT )? ( (i= IN inList ) | (b= BETWEEN betweenList ) | (l= LIKE concatenation likeEscape ) | ( MEMBER ( OF )? p= path ) ) ) )
            // Hql.g:415:4: concatenation ( ( ( ( LT | GT | LE | GE ) bitwiseNotExpression )* ) | (n= NOT )? ( (i= IN inList ) | (b= BETWEEN betweenList ) | (l= LIKE concatenation likeEscape ) | ( MEMBER ( OF )? p= path ) ) )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_concatenation_in_relationalExpression2034);
            	concatenation160 = concatenation();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, concatenation160.Tree);
            	// Hql.g:415:18: ( ( ( ( LT | GT | LE | GE ) bitwiseNotExpression )* ) | (n= NOT )? ( (i= IN inList ) | (b= BETWEEN betweenList ) | (l= LIKE concatenation likeEscape ) | ( MEMBER ( OF )? p= path ) ) )
            	int alt58 = 2;
            	int LA58_0 = input.LA(1);

            	if ( (LA58_0 == EOF || (LA58_0 >= AND && LA58_0 <= ASCENDING) || LA58_0 == DESCENDING || (LA58_0 >= FROM && LA58_0 <= HAVING) || LA58_0 == INNER || (LA58_0 >= IS && LA58_0 <= LEFT) || (LA58_0 >= OR && LA58_0 <= ORDER) || LA58_0 == RIGHT || LA58_0 == SKIP || LA58_0 == TAKE || LA58_0 == UNION || LA58_0 == WHERE || (LA58_0 >= END && LA58_0 <= WHEN) || (LA58_0 >= COMMA && LA58_0 <= EQ) || (LA58_0 >= CLOSE && LA58_0 <= GE) || LA58_0 == CLOSE_BRACKET || (LA58_0 >= 133 && LA58_0 <= 134)) )
            	{
            	    alt58 = 1;
            	}
            	else if ( (LA58_0 == BETWEEN || LA58_0 == IN || LA58_0 == LIKE || LA58_0 == NOT || LA58_0 == MEMBER) )
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
            	        // Hql.g:416:3: ( ( ( LT | GT | LE | GE ) bitwiseNotExpression )* )
            	        {
            	        	// Hql.g:416:3: ( ( ( LT | GT | LE | GE ) bitwiseNotExpression )* )
            	        	// Hql.g:416:5: ( ( LT | GT | LE | GE ) bitwiseNotExpression )*
            	        	{
            	        		// Hql.g:416:5: ( ( LT | GT | LE | GE ) bitwiseNotExpression )*
            	        		do 
            	        		{
            	        		    int alt54 = 2;
            	        		    int LA54_0 = input.LA(1);

            	        		    if ( ((LA54_0 >= LT && LA54_0 <= GE)) )
            	        		    {
            	        		        alt54 = 1;
            	        		    }


            	        		    switch (alt54) 
            	        			{
            	        				case 1 :
            	        				    // Hql.g:416:7: ( LT | GT | LE | GE ) bitwiseNotExpression
            	        				    {
            	        				    	// Hql.g:416:7: ( LT | GT | LE | GE )
            	        				    	int alt53 = 4;
            	        				    	switch ( input.LA(1) ) 
            	        				    	{
            	        				    	case LT:
            	        				    		{
            	        				    	    alt53 = 1;
            	        				    	    }
            	        				    	    break;
            	        				    	case GT:
            	        				    		{
            	        				    	    alt53 = 2;
            	        				    	    }
            	        				    	    break;
            	        				    	case LE:
            	        				    		{
            	        				    	    alt53 = 3;
            	        				    	    }
            	        				    	    break;
            	        				    	case GE:
            	        				    		{
            	        				    	    alt53 = 4;
            	        				    	    }
            	        				    	    break;
            	        				    		default:
            	        				    		    NoViableAltException nvae_d53s0 =
            	        				    		        new NoViableAltException("", 53, 0, input);

            	        				    		    throw nvae_d53s0;
            	        				    	}

            	        				    	switch (alt53) 
            	        				    	{
            	        				    	    case 1 :
            	        				    	        // Hql.g:416:9: LT
            	        				    	        {
            	        				    	        	LT161=(IToken)Match(input,LT,FOLLOW_LT_in_relationalExpression2046); 
            	        				    	        		LT161_tree = (IASTNode)adaptor.Create(LT161);
            	        				    	        		root_0 = (IASTNode)adaptor.BecomeRoot(LT161_tree, root_0);


            	        				    	        }
            	        				    	        break;
            	        				    	    case 2 :
            	        				    	        // Hql.g:416:15: GT
            	        				    	        {
            	        				    	        	GT162=(IToken)Match(input,GT,FOLLOW_GT_in_relationalExpression2051); 
            	        				    	        		GT162_tree = (IASTNode)adaptor.Create(GT162);
            	        				    	        		root_0 = (IASTNode)adaptor.BecomeRoot(GT162_tree, root_0);


            	        				    	        }
            	        				    	        break;
            	        				    	    case 3 :
            	        				    	        // Hql.g:416:21: LE
            	        				    	        {
            	        				    	        	LE163=(IToken)Match(input,LE,FOLLOW_LE_in_relationalExpression2056); 
            	        				    	        		LE163_tree = (IASTNode)adaptor.Create(LE163);
            	        				    	        		root_0 = (IASTNode)adaptor.BecomeRoot(LE163_tree, root_0);


            	        				    	        }
            	        				    	        break;
            	        				    	    case 4 :
            	        				    	        // Hql.g:416:27: GE
            	        				    	        {
            	        				    	        	GE164=(IToken)Match(input,GE,FOLLOW_GE_in_relationalExpression2061); 
            	        				    	        		GE164_tree = (IASTNode)adaptor.Create(GE164);
            	        				    	        		root_0 = (IASTNode)adaptor.BecomeRoot(GE164_tree, root_0);


            	        				    	        }
            	        				    	        break;

            	        				    	}

            	        				    	PushFollow(FOLLOW_bitwiseNotExpression_in_relationalExpression2066);
            	        				    	bitwiseNotExpression165 = bitwiseNotExpression();
            	        				    	state.followingStackPointer--;

            	        				    	adaptor.AddChild(root_0, bitwiseNotExpression165.Tree);

            	        				    }
            	        				    break;

            	        				default:
            	        				    goto loop54;
            	        		    }
            	        		} while (true);

            	        		loop54:
            	        			;	// Stops C# compiler whining that label 'loop54' has no statements


            	        	}


            	        }
            	        break;
            	    case 2 :
            	        // Hql.g:418:5: (n= NOT )? ( (i= IN inList ) | (b= BETWEEN betweenList ) | (l= LIKE concatenation likeEscape ) | ( MEMBER ( OF )? p= path ) )
            	        {
            	        	// Hql.g:418:5: (n= NOT )?
            	        	int alt55 = 2;
            	        	int LA55_0 = input.LA(1);

            	        	if ( (LA55_0 == NOT) )
            	        	{
            	        	    alt55 = 1;
            	        	}
            	        	switch (alt55) 
            	        	{
            	        	    case 1 :
            	        	        // Hql.g:418:6: n= NOT
            	        	        {
            	        	        	n=(IToken)Match(input,NOT,FOLLOW_NOT_in_relationalExpression2083); 

            	        	        }
            	        	        break;

            	        	}

            	        	// Hql.g:418:15: ( (i= IN inList ) | (b= BETWEEN betweenList ) | (l= LIKE concatenation likeEscape ) | ( MEMBER ( OF )? p= path ) )
            	        	int alt57 = 4;
            	        	switch ( input.LA(1) ) 
            	        	{
            	        	case IN:
            	        		{
            	        	    alt57 = 1;
            	        	    }
            	        	    break;
            	        	case BETWEEN:
            	        		{
            	        	    alt57 = 2;
            	        	    }
            	        	    break;
            	        	case LIKE:
            	        		{
            	        	    alt57 = 3;
            	        	    }
            	        	    break;
            	        	case MEMBER:
            	        		{
            	        	    alt57 = 4;
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
            	        	        // Hql.g:421:4: (i= IN inList )
            	        	        {
            	        	        	// Hql.g:421:4: (i= IN inList )
            	        	        	// Hql.g:421:5: i= IN inList
            	        	        	{
            	        	        		i=(IToken)Match(input,IN,FOLLOW_IN_in_relationalExpression2104); 
            	        	        			i_tree = (IASTNode)adaptor.Create(i);
            	        	        			root_0 = (IASTNode)adaptor.BecomeRoot(i_tree, root_0);


            	        	        							i.Type = (n == null) ? IN : NOT_IN;
            	        	        							i.Text = (n == null) ? "in" : "not in";
            	        	        						
            	        	        		PushFollow(FOLLOW_inList_in_relationalExpression2113);
            	        	        		inList166 = inList();
            	        	        		state.followingStackPointer--;

            	        	        		adaptor.AddChild(root_0, inList166.Tree);

            	        	        	}


            	        	        }
            	        	        break;
            	        	    case 2 :
            	        	        // Hql.g:426:6: (b= BETWEEN betweenList )
            	        	        {
            	        	        	// Hql.g:426:6: (b= BETWEEN betweenList )
            	        	        	// Hql.g:426:7: b= BETWEEN betweenList
            	        	        	{
            	        	        		b=(IToken)Match(input,BETWEEN,FOLLOW_BETWEEN_in_relationalExpression2124); 
            	        	        			b_tree = (IASTNode)adaptor.Create(b);
            	        	        			root_0 = (IASTNode)adaptor.BecomeRoot(b_tree, root_0);


            	        	        							b.Type = (n == null) ? BETWEEN : NOT_BETWEEN;
            	        	        							b.Text = (n == null) ? "between" : "not between";
            	        	        						
            	        	        		PushFollow(FOLLOW_betweenList_in_relationalExpression2133);
            	        	        		betweenList167 = betweenList();
            	        	        		state.followingStackPointer--;

            	        	        		adaptor.AddChild(root_0, betweenList167.Tree);

            	        	        	}


            	        	        }
            	        	        break;
            	        	    case 3 :
            	        	        // Hql.g:431:6: (l= LIKE concatenation likeEscape )
            	        	        {
            	        	        	// Hql.g:431:6: (l= LIKE concatenation likeEscape )
            	        	        	// Hql.g:431:7: l= LIKE concatenation likeEscape
            	        	        	{
            	        	        		l=(IToken)Match(input,LIKE,FOLLOW_LIKE_in_relationalExpression2145); 
            	        	        			l_tree = (IASTNode)adaptor.Create(l);
            	        	        			root_0 = (IASTNode)adaptor.BecomeRoot(l_tree, root_0);


            	        	        							l.Type = (n == null) ? LIKE : NOT_LIKE;
            	        	        							l.Text = (n == null) ? "like" : "not like";
            	        	        						
            	        	        		PushFollow(FOLLOW_concatenation_in_relationalExpression2154);
            	        	        		concatenation168 = concatenation();
            	        	        		state.followingStackPointer--;

            	        	        		adaptor.AddChild(root_0, concatenation168.Tree);
            	        	        		PushFollow(FOLLOW_likeEscape_in_relationalExpression2156);
            	        	        		likeEscape169 = likeEscape();
            	        	        		state.followingStackPointer--;

            	        	        		adaptor.AddChild(root_0, likeEscape169.Tree);

            	        	        	}


            	        	        }
            	        	        break;
            	        	    case 4 :
            	        	        // Hql.g:436:6: ( MEMBER ( OF )? p= path )
            	        	        {
            	        	        	// Hql.g:436:6: ( MEMBER ( OF )? p= path )
            	        	        	// Hql.g:436:7: MEMBER ( OF )? p= path
            	        	        	{
            	        	        		MEMBER170=(IToken)Match(input,MEMBER,FOLLOW_MEMBER_in_relationalExpression2165); 
            	        	        		// Hql.g:436:15: ( OF )?
            	        	        		int alt56 = 2;
            	        	        		int LA56_0 = input.LA(1);

            	        	        		if ( (LA56_0 == OF) )
            	        	        		{
            	        	        		    alt56 = 1;
            	        	        		}
            	        	        		switch (alt56) 
            	        	        		{
            	        	        		    case 1 :
            	        	        		        // Hql.g:436:16: OF
            	        	        		        {
            	        	        		        	OF171=(IToken)Match(input,OF,FOLLOW_OF_in_relationalExpression2169); 

            	        	        		        }
            	        	        		        break;

            	        	        		}

            	        	        		PushFollow(FOLLOW_path_in_relationalExpression2176);
            	        	        		p = path();
            	        	        		state.followingStackPointer--;


            	        	        						root_0 = ProcessMemberOf(n,((p != null) ? ((IASTNode)p.Tree) : null), root_0);
            	        	        					  

            	        	        	}


            	        	        }
            	        	        break;

            	        	}


            	        }
            	        break;

            	}


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "relationalExpression"

    public class likeEscape_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "likeEscape"
    // Hql.g:443:1: likeEscape : ( ESCAPE concatenation )? ;
    public HqlParser.likeEscape_return likeEscape() // throws RecognitionException [1]
    {   
        HqlParser.likeEscape_return retval = new HqlParser.likeEscape_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken ESCAPE172 = null;
        HqlParser.concatenation_return concatenation173 = default(HqlParser.concatenation_return);


        IASTNode ESCAPE172_tree=null;

        try 
    	{
            // Hql.g:444:2: ( ( ESCAPE concatenation )? )
            // Hql.g:444:4: ( ESCAPE concatenation )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// Hql.g:444:4: ( ESCAPE concatenation )?
            	int alt59 = 2;
            	int LA59_0 = input.LA(1);

            	if ( (LA59_0 == ESCAPE) )
            	{
            	    alt59 = 1;
            	}
            	switch (alt59) 
            	{
            	    case 1 :
            	        // Hql.g:444:5: ESCAPE concatenation
            	        {
            	        	ESCAPE172=(IToken)Match(input,ESCAPE,FOLLOW_ESCAPE_in_likeEscape2203); 
            	        		ESCAPE172_tree = (IASTNode)adaptor.Create(ESCAPE172);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(ESCAPE172_tree, root_0);

            	        	PushFollow(FOLLOW_concatenation_in_likeEscape2206);
            	        	concatenation173 = concatenation();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, concatenation173.Tree);

            	        }
            	        break;

            	}


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "likeEscape"

    public class inList_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "inList"
    // Hql.g:447:1: inList : compoundExpr -> ^( IN_LIST[\"inList\"] compoundExpr ) ;
    public HqlParser.inList_return inList() // throws RecognitionException [1]
    {   
        HqlParser.inList_return retval = new HqlParser.inList_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.compoundExpr_return compoundExpr174 = default(HqlParser.compoundExpr_return);


        RewriteRuleSubtreeStream stream_compoundExpr = new RewriteRuleSubtreeStream(adaptor,"rule compoundExpr");
        try 
    	{
            // Hql.g:448:2: ( compoundExpr -> ^( IN_LIST[\"inList\"] compoundExpr ) )
            // Hql.g:448:4: compoundExpr
            {
            	PushFollow(FOLLOW_compoundExpr_in_inList2219);
            	compoundExpr174 = compoundExpr();
            	state.followingStackPointer--;

            	stream_compoundExpr.Add(compoundExpr174.Tree);


            	// AST REWRITE
            	// elements:          compoundExpr
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (IASTNode)adaptor.GetNilNode();
            	// 449:2: -> ^( IN_LIST[\"inList\"] compoundExpr )
            	{
            	    // Hql.g:449:5: ^( IN_LIST[\"inList\"] compoundExpr )
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(IN_LIST, "inList"), root_1);

            	    adaptor.AddChild(root_1, stream_compoundExpr.NextTree());

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;
            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "inList"

    public class betweenList_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "betweenList"
    // Hql.g:452:1: betweenList : concatenation AND concatenation ;
    public HqlParser.betweenList_return betweenList() // throws RecognitionException [1]
    {   
        HqlParser.betweenList_return retval = new HqlParser.betweenList_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken AND176 = null;
        HqlParser.concatenation_return concatenation175 = default(HqlParser.concatenation_return);

        HqlParser.concatenation_return concatenation177 = default(HqlParser.concatenation_return);


        IASTNode AND176_tree=null;

        try 
    	{
            // Hql.g:453:2: ( concatenation AND concatenation )
            // Hql.g:453:4: concatenation AND concatenation
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_concatenation_in_betweenList2240);
            	concatenation175 = concatenation();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, concatenation175.Tree);
            	AND176=(IToken)Match(input,AND,FOLLOW_AND_in_betweenList2242); 
            	PushFollow(FOLLOW_concatenation_in_betweenList2245);
            	concatenation177 = concatenation();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, concatenation177.Tree);

            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "betweenList"

    public class concatenation_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "concatenation"
    // Hql.g:457:1: concatenation : a= bitwiseNotExpression (c= CONCAT bitwiseNotExpression ( CONCAT bitwiseNotExpression )* )? ;
    public HqlParser.concatenation_return concatenation() // throws RecognitionException [1]
    {   
        HqlParser.concatenation_return retval = new HqlParser.concatenation_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken c = null;
        IToken CONCAT179 = null;
        HqlParser.bitwiseNotExpression_return a = default(HqlParser.bitwiseNotExpression_return);

        HqlParser.bitwiseNotExpression_return bitwiseNotExpression178 = default(HqlParser.bitwiseNotExpression_return);

        HqlParser.bitwiseNotExpression_return bitwiseNotExpression180 = default(HqlParser.bitwiseNotExpression_return);


        IASTNode c_tree=null;
        IASTNode CONCAT179_tree=null;

        try 
    	{
            // Hql.g:468:2: (a= bitwiseNotExpression (c= CONCAT bitwiseNotExpression ( CONCAT bitwiseNotExpression )* )? )
            // Hql.g:468:4: a= bitwiseNotExpression (c= CONCAT bitwiseNotExpression ( CONCAT bitwiseNotExpression )* )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_bitwiseNotExpression_in_concatenation2264);
            	a = bitwiseNotExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, a.Tree);
            	// Hql.g:469:2: (c= CONCAT bitwiseNotExpression ( CONCAT bitwiseNotExpression )* )?
            	int alt61 = 2;
            	int LA61_0 = input.LA(1);

            	if ( (LA61_0 == CONCAT) )
            	{
            	    alt61 = 1;
            	}
            	switch (alt61) 
            	{
            	    case 1 :
            	        // Hql.g:469:4: c= CONCAT bitwiseNotExpression ( CONCAT bitwiseNotExpression )*
            	        {
            	        	c=(IToken)Match(input,CONCAT,FOLLOW_CONCAT_in_concatenation2272); 
            	        		c_tree = (IASTNode)adaptor.Create(c);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(c_tree, root_0);

            	        	 c.Type = EXPR_LIST; c.Text = "concatList"; 
            	        	PushFollow(FOLLOW_bitwiseNotExpression_in_concatenation2281);
            	        	bitwiseNotExpression178 = bitwiseNotExpression();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, bitwiseNotExpression178.Tree);
            	        	// Hql.g:471:4: ( CONCAT bitwiseNotExpression )*
            	        	do 
            	        	{
            	        	    int alt60 = 2;
            	        	    int LA60_0 = input.LA(1);

            	        	    if ( (LA60_0 == CONCAT) )
            	        	    {
            	        	        alt60 = 1;
            	        	    }


            	        	    switch (alt60) 
            	        		{
            	        			case 1 :
            	        			    // Hql.g:471:6: CONCAT bitwiseNotExpression
            	        			    {
            	        			    	CONCAT179=(IToken)Match(input,CONCAT,FOLLOW_CONCAT_in_concatenation2288); 
            	        			    	PushFollow(FOLLOW_bitwiseNotExpression_in_concatenation2291);
            	        			    	bitwiseNotExpression180 = bitwiseNotExpression();
            	        			    	state.followingStackPointer--;

            	        			    	adaptor.AddChild(root_0, bitwiseNotExpression180.Tree);

            	        			    }
            	        			    break;

            	        			default:
            	        			    goto loop60;
            	        	    }
            	        	} while (true);

            	        	loop60:
            	        		;	// Stops C# compiler whining that label 'loop60' has no statements


            	        }
            	        break;

            	}


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);

               if (c != null)
               {
                  IASTNode mc = (IASTNode) adaptor.Create(METHOD_CALL, "||");
                  IASTNode concat = (IASTNode) adaptor.Create(IDENT, "concat");
                  mc.AddChild(concat);
                  mc.AddChild((IASTNode) retval.Tree);
                  retval.Tree = mc;
               }

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "concatenation"

    public class bitwiseNotExpression_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "bitwiseNotExpression"
    // Hql.g:476:1: bitwiseNotExpression : ( ( BNOT bitwiseOrExpression ) | bitwiseOrExpression );
    public HqlParser.bitwiseNotExpression_return bitwiseNotExpression() // throws RecognitionException [1]
    {   
        HqlParser.bitwiseNotExpression_return retval = new HqlParser.bitwiseNotExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken BNOT181 = null;
        HqlParser.bitwiseOrExpression_return bitwiseOrExpression182 = default(HqlParser.bitwiseOrExpression_return);

        HqlParser.bitwiseOrExpression_return bitwiseOrExpression183 = default(HqlParser.bitwiseOrExpression_return);


        IASTNode BNOT181_tree=null;

        try 
    	{
            // Hql.g:477:2: ( ( BNOT bitwiseOrExpression ) | bitwiseOrExpression )
            int alt62 = 2;
            int LA62_0 = input.LA(1);

            if ( (LA62_0 == BNOT) )
            {
                alt62 = 1;
            }
            else if ( ((LA62_0 >= ALL && LA62_0 <= ANY) || LA62_0 == AVG || LA62_0 == COUNT || LA62_0 == ELEMENTS || (LA62_0 >= EXISTS && LA62_0 <= FALSE) || LA62_0 == INDICES || (LA62_0 >= MAX && LA62_0 <= MIN) || LA62_0 == NULL || (LA62_0 >= SOME && LA62_0 <= SUM) || LA62_0 == TRUE || LA62_0 == CASE || LA62_0 == EMPTY || (LA62_0 >= NUM_INT && LA62_0 <= NUM_LONG) || LA62_0 == OPEN || (LA62_0 >= PLUS && LA62_0 <= MINUS) || (LA62_0 >= COLON && LA62_0 <= IDENT)) )
            {
                alt62 = 2;
            }
            else 
            {
                NoViableAltException nvae_d62s0 =
                    new NoViableAltException("", 62, 0, input);

                throw nvae_d62s0;
            }
            switch (alt62) 
            {
                case 1 :
                    // Hql.g:477:4: ( BNOT bitwiseOrExpression )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	// Hql.g:477:4: ( BNOT bitwiseOrExpression )
                    	// Hql.g:477:5: BNOT bitwiseOrExpression
                    	{
                    		BNOT181=(IToken)Match(input,BNOT,FOLLOW_BNOT_in_bitwiseNotExpression2315); 
                    			BNOT181_tree = (IASTNode)adaptor.Create(BNOT181);
                    			root_0 = (IASTNode)adaptor.BecomeRoot(BNOT181_tree, root_0);

                    		PushFollow(FOLLOW_bitwiseOrExpression_in_bitwiseNotExpression2318);
                    		bitwiseOrExpression182 = bitwiseOrExpression();
                    		state.followingStackPointer--;

                    		adaptor.AddChild(root_0, bitwiseOrExpression182.Tree);

                    	}


                    }
                    break;
                case 2 :
                    // Hql.g:478:4: bitwiseOrExpression
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_bitwiseOrExpression_in_bitwiseNotExpression2324);
                    	bitwiseOrExpression183 = bitwiseOrExpression();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, bitwiseOrExpression183.Tree);

                    }
                    break;

            }
            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "bitwiseNotExpression"

    public class bitwiseOrExpression_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "bitwiseOrExpression"
    // Hql.g:481:1: bitwiseOrExpression : bitwiseXOrExpression ( BOR bitwiseXOrExpression )* ;
    public HqlParser.bitwiseOrExpression_return bitwiseOrExpression() // throws RecognitionException [1]
    {   
        HqlParser.bitwiseOrExpression_return retval = new HqlParser.bitwiseOrExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken BOR185 = null;
        HqlParser.bitwiseXOrExpression_return bitwiseXOrExpression184 = default(HqlParser.bitwiseXOrExpression_return);

        HqlParser.bitwiseXOrExpression_return bitwiseXOrExpression186 = default(HqlParser.bitwiseXOrExpression_return);


        IASTNode BOR185_tree=null;

        try 
    	{
            // Hql.g:482:2: ( bitwiseXOrExpression ( BOR bitwiseXOrExpression )* )
            // Hql.g:482:4: bitwiseXOrExpression ( BOR bitwiseXOrExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_bitwiseXOrExpression_in_bitwiseOrExpression2336);
            	bitwiseXOrExpression184 = bitwiseXOrExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, bitwiseXOrExpression184.Tree);
            	// Hql.g:482:25: ( BOR bitwiseXOrExpression )*
            	do 
            	{
            	    int alt63 = 2;
            	    int LA63_0 = input.LA(1);

            	    if ( (LA63_0 == BOR) )
            	    {
            	        alt63 = 1;
            	    }


            	    switch (alt63) 
            		{
            			case 1 :
            			    // Hql.g:482:26: BOR bitwiseXOrExpression
            			    {
            			    	BOR185=(IToken)Match(input,BOR,FOLLOW_BOR_in_bitwiseOrExpression2339); 
            			    		BOR185_tree = (IASTNode)adaptor.Create(BOR185);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(BOR185_tree, root_0);

            			    	PushFollow(FOLLOW_bitwiseXOrExpression_in_bitwiseOrExpression2342);
            			    	bitwiseXOrExpression186 = bitwiseXOrExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, bitwiseXOrExpression186.Tree);

            			    }
            			    break;

            			default:
            			    goto loop63;
            	    }
            	} while (true);

            	loop63:
            		;	// Stops C# compiler whining that label 'loop63' has no statements


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "bitwiseOrExpression"

    public class bitwiseXOrExpression_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "bitwiseXOrExpression"
    // Hql.g:485:1: bitwiseXOrExpression : bitwiseAndExpression ( BXOR bitwiseAndExpression )* ;
    public HqlParser.bitwiseXOrExpression_return bitwiseXOrExpression() // throws RecognitionException [1]
    {   
        HqlParser.bitwiseXOrExpression_return retval = new HqlParser.bitwiseXOrExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken BXOR188 = null;
        HqlParser.bitwiseAndExpression_return bitwiseAndExpression187 = default(HqlParser.bitwiseAndExpression_return);

        HqlParser.bitwiseAndExpression_return bitwiseAndExpression189 = default(HqlParser.bitwiseAndExpression_return);


        IASTNode BXOR188_tree=null;

        try 
    	{
            // Hql.g:486:2: ( bitwiseAndExpression ( BXOR bitwiseAndExpression )* )
            // Hql.g:486:4: bitwiseAndExpression ( BXOR bitwiseAndExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_bitwiseAndExpression_in_bitwiseXOrExpression2356);
            	bitwiseAndExpression187 = bitwiseAndExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, bitwiseAndExpression187.Tree);
            	// Hql.g:486:25: ( BXOR bitwiseAndExpression )*
            	do 
            	{
            	    int alt64 = 2;
            	    int LA64_0 = input.LA(1);

            	    if ( (LA64_0 == BXOR) )
            	    {
            	        alt64 = 1;
            	    }


            	    switch (alt64) 
            		{
            			case 1 :
            			    // Hql.g:486:26: BXOR bitwiseAndExpression
            			    {
            			    	BXOR188=(IToken)Match(input,BXOR,FOLLOW_BXOR_in_bitwiseXOrExpression2359); 
            			    		BXOR188_tree = (IASTNode)adaptor.Create(BXOR188);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(BXOR188_tree, root_0);

            			    	PushFollow(FOLLOW_bitwiseAndExpression_in_bitwiseXOrExpression2362);
            			    	bitwiseAndExpression189 = bitwiseAndExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, bitwiseAndExpression189.Tree);

            			    }
            			    break;

            			default:
            			    goto loop64;
            	    }
            	} while (true);

            	loop64:
            		;	// Stops C# compiler whining that label 'loop64' has no statements


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "bitwiseXOrExpression"

    public class bitwiseAndExpression_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "bitwiseAndExpression"
    // Hql.g:489:1: bitwiseAndExpression : additiveExpression ( BAND additiveExpression )* ;
    public HqlParser.bitwiseAndExpression_return bitwiseAndExpression() // throws RecognitionException [1]
    {   
        HqlParser.bitwiseAndExpression_return retval = new HqlParser.bitwiseAndExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken BAND191 = null;
        HqlParser.additiveExpression_return additiveExpression190 = default(HqlParser.additiveExpression_return);

        HqlParser.additiveExpression_return additiveExpression192 = default(HqlParser.additiveExpression_return);


        IASTNode BAND191_tree=null;

        try 
    	{
            // Hql.g:490:2: ( additiveExpression ( BAND additiveExpression )* )
            // Hql.g:490:4: additiveExpression ( BAND additiveExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_additiveExpression_in_bitwiseAndExpression2376);
            	additiveExpression190 = additiveExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, additiveExpression190.Tree);
            	// Hql.g:490:23: ( BAND additiveExpression )*
            	do 
            	{
            	    int alt65 = 2;
            	    int LA65_0 = input.LA(1);

            	    if ( (LA65_0 == BAND) )
            	    {
            	        alt65 = 1;
            	    }


            	    switch (alt65) 
            		{
            			case 1 :
            			    // Hql.g:490:24: BAND additiveExpression
            			    {
            			    	BAND191=(IToken)Match(input,BAND,FOLLOW_BAND_in_bitwiseAndExpression2379); 
            			    		BAND191_tree = (IASTNode)adaptor.Create(BAND191);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(BAND191_tree, root_0);

            			    	PushFollow(FOLLOW_additiveExpression_in_bitwiseAndExpression2382);
            			    	additiveExpression192 = additiveExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, additiveExpression192.Tree);

            			    }
            			    break;

            			default:
            			    goto loop65;
            	    }
            	} while (true);

            	loop65:
            		;	// Stops C# compiler whining that label 'loop65' has no statements


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "bitwiseAndExpression"

    public class additiveExpression_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "additiveExpression"
    // Hql.g:494:1: additiveExpression : multiplyExpression ( ( PLUS | MINUS ) multiplyExpression )* ;
    public HqlParser.additiveExpression_return additiveExpression() // throws RecognitionException [1]
    {   
        HqlParser.additiveExpression_return retval = new HqlParser.additiveExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken PLUS194 = null;
        IToken MINUS195 = null;
        HqlParser.multiplyExpression_return multiplyExpression193 = default(HqlParser.multiplyExpression_return);

        HqlParser.multiplyExpression_return multiplyExpression196 = default(HqlParser.multiplyExpression_return);


        IASTNode PLUS194_tree=null;
        IASTNode MINUS195_tree=null;

        try 
    	{
            // Hql.g:495:2: ( multiplyExpression ( ( PLUS | MINUS ) multiplyExpression )* )
            // Hql.g:495:4: multiplyExpression ( ( PLUS | MINUS ) multiplyExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_multiplyExpression_in_additiveExpression2396);
            	multiplyExpression193 = multiplyExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, multiplyExpression193.Tree);
            	// Hql.g:495:23: ( ( PLUS | MINUS ) multiplyExpression )*
            	do 
            	{
            	    int alt67 = 2;
            	    int LA67_0 = input.LA(1);

            	    if ( ((LA67_0 >= PLUS && LA67_0 <= MINUS)) )
            	    {
            	        alt67 = 1;
            	    }


            	    switch (alt67) 
            		{
            			case 1 :
            			    // Hql.g:495:25: ( PLUS | MINUS ) multiplyExpression
            			    {
            			    	// Hql.g:495:25: ( PLUS | MINUS )
            			    	int alt66 = 2;
            			    	int LA66_0 = input.LA(1);

            			    	if ( (LA66_0 == PLUS) )
            			    	{
            			    	    alt66 = 1;
            			    	}
            			    	else if ( (LA66_0 == MINUS) )
            			    	{
            			    	    alt66 = 2;
            			    	}
            			    	else 
            			    	{
            			    	    NoViableAltException nvae_d66s0 =
            			    	        new NoViableAltException("", 66, 0, input);

            			    	    throw nvae_d66s0;
            			    	}
            			    	switch (alt66) 
            			    	{
            			    	    case 1 :
            			    	        // Hql.g:495:27: PLUS
            			    	        {
            			    	        	PLUS194=(IToken)Match(input,PLUS,FOLLOW_PLUS_in_additiveExpression2402); 
            			    	        		PLUS194_tree = (IASTNode)adaptor.Create(PLUS194);
            			    	        		root_0 = (IASTNode)adaptor.BecomeRoot(PLUS194_tree, root_0);


            			    	        }
            			    	        break;
            			    	    case 2 :
            			    	        // Hql.g:495:35: MINUS
            			    	        {
            			    	        	MINUS195=(IToken)Match(input,MINUS,FOLLOW_MINUS_in_additiveExpression2407); 
            			    	        		MINUS195_tree = (IASTNode)adaptor.Create(MINUS195);
            			    	        		root_0 = (IASTNode)adaptor.BecomeRoot(MINUS195_tree, root_0);


            			    	        }
            			    	        break;

            			    	}

            			    	PushFollow(FOLLOW_multiplyExpression_in_additiveExpression2412);
            			    	multiplyExpression196 = multiplyExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, multiplyExpression196.Tree);

            			    }
            			    break;

            			default:
            			    goto loop67;
            	    }
            	} while (true);

            	loop67:
            		;	// Stops C# compiler whining that label 'loop67' has no statements


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "additiveExpression"

    public class multiplyExpression_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "multiplyExpression"
    // Hql.g:499:1: multiplyExpression : unaryExpression ( ( STAR | DIV ) unaryExpression )* ;
    public HqlParser.multiplyExpression_return multiplyExpression() // throws RecognitionException [1]
    {   
        HqlParser.multiplyExpression_return retval = new HqlParser.multiplyExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken STAR198 = null;
        IToken DIV199 = null;
        HqlParser.unaryExpression_return unaryExpression197 = default(HqlParser.unaryExpression_return);

        HqlParser.unaryExpression_return unaryExpression200 = default(HqlParser.unaryExpression_return);


        IASTNode STAR198_tree=null;
        IASTNode DIV199_tree=null;

        try 
    	{
            // Hql.g:500:2: ( unaryExpression ( ( STAR | DIV ) unaryExpression )* )
            // Hql.g:500:4: unaryExpression ( ( STAR | DIV ) unaryExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_unaryExpression_in_multiplyExpression2427);
            	unaryExpression197 = unaryExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, unaryExpression197.Tree);
            	// Hql.g:500:20: ( ( STAR | DIV ) unaryExpression )*
            	do 
            	{
            	    int alt69 = 2;
            	    int LA69_0 = input.LA(1);

            	    if ( ((LA69_0 >= STAR && LA69_0 <= DIV)) )
            	    {
            	        alt69 = 1;
            	    }


            	    switch (alt69) 
            		{
            			case 1 :
            			    // Hql.g:500:22: ( STAR | DIV ) unaryExpression
            			    {
            			    	// Hql.g:500:22: ( STAR | DIV )
            			    	int alt68 = 2;
            			    	int LA68_0 = input.LA(1);

            			    	if ( (LA68_0 == STAR) )
            			    	{
            			    	    alt68 = 1;
            			    	}
            			    	else if ( (LA68_0 == DIV) )
            			    	{
            			    	    alt68 = 2;
            			    	}
            			    	else 
            			    	{
            			    	    NoViableAltException nvae_d68s0 =
            			    	        new NoViableAltException("", 68, 0, input);

            			    	    throw nvae_d68s0;
            			    	}
            			    	switch (alt68) 
            			    	{
            			    	    case 1 :
            			    	        // Hql.g:500:24: STAR
            			    	        {
            			    	        	STAR198=(IToken)Match(input,STAR,FOLLOW_STAR_in_multiplyExpression2433); 
            			    	        		STAR198_tree = (IASTNode)adaptor.Create(STAR198);
            			    	        		root_0 = (IASTNode)adaptor.BecomeRoot(STAR198_tree, root_0);


            			    	        }
            			    	        break;
            			    	    case 2 :
            			    	        // Hql.g:500:32: DIV
            			    	        {
            			    	        	DIV199=(IToken)Match(input,DIV,FOLLOW_DIV_in_multiplyExpression2438); 
            			    	        		DIV199_tree = (IASTNode)adaptor.Create(DIV199);
            			    	        		root_0 = (IASTNode)adaptor.BecomeRoot(DIV199_tree, root_0);


            			    	        }
            			    	        break;

            			    	}

            			    	PushFollow(FOLLOW_unaryExpression_in_multiplyExpression2443);
            			    	unaryExpression200 = unaryExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, unaryExpression200.Tree);

            			    }
            			    break;

            			default:
            			    goto loop69;
            	    }
            	} while (true);

            	loop69:
            		;	// Stops C# compiler whining that label 'loop69' has no statements


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "multiplyExpression"

    public class unaryExpression_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "unaryExpression"
    // Hql.g:504:1: unaryExpression : (m= MINUS mu= unaryExpression -> ^( UNARY_MINUS[$m] $mu) | p= PLUS pu= unaryExpression -> ^( UNARY_PLUS[$p] $pu) | c= caseExpression -> ^( $c) | q= quantifiedExpression -> ^( $q) | a= atom -> ^( $a) );
    public HqlParser.unaryExpression_return unaryExpression() // throws RecognitionException [1]
    {   
        HqlParser.unaryExpression_return retval = new HqlParser.unaryExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken m = null;
        IToken p = null;
        HqlParser.unaryExpression_return mu = default(HqlParser.unaryExpression_return);

        HqlParser.unaryExpression_return pu = default(HqlParser.unaryExpression_return);

        HqlParser.caseExpression_return c = default(HqlParser.caseExpression_return);

        HqlParser.quantifiedExpression_return q = default(HqlParser.quantifiedExpression_return);

        HqlParser.atom_return a = default(HqlParser.atom_return);


        IASTNode m_tree=null;
        IASTNode p_tree=null;
        RewriteRuleTokenStream stream_PLUS = new RewriteRuleTokenStream(adaptor,"token PLUS");
        RewriteRuleTokenStream stream_MINUS = new RewriteRuleTokenStream(adaptor,"token MINUS");
        RewriteRuleSubtreeStream stream_atom = new RewriteRuleSubtreeStream(adaptor,"rule atom");
        RewriteRuleSubtreeStream stream_caseExpression = new RewriteRuleSubtreeStream(adaptor,"rule caseExpression");
        RewriteRuleSubtreeStream stream_quantifiedExpression = new RewriteRuleSubtreeStream(adaptor,"rule quantifiedExpression");
        RewriteRuleSubtreeStream stream_unaryExpression = new RewriteRuleSubtreeStream(adaptor,"rule unaryExpression");
        try 
    	{
            // Hql.g:505:2: (m= MINUS mu= unaryExpression -> ^( UNARY_MINUS[$m] $mu) | p= PLUS pu= unaryExpression -> ^( UNARY_PLUS[$p] $pu) | c= caseExpression -> ^( $c) | q= quantifiedExpression -> ^( $q) | a= atom -> ^( $a) )
            int alt70 = 5;
            switch ( input.LA(1) ) 
            {
            case MINUS:
            	{
                alt70 = 1;
                }
                break;
            case PLUS:
            	{
                alt70 = 2;
                }
                break;
            case CASE:
            	{
                alt70 = 3;
                }
                break;
            case ALL:
            case ANY:
            case EXISTS:
            case SOME:
            	{
                alt70 = 4;
                }
                break;
            case AVG:
            case COUNT:
            case ELEMENTS:
            case FALSE:
            case INDICES:
            case MAX:
            case MIN:
            case NULL:
            case SUM:
            case TRUE:
            case EMPTY:
            case NUM_INT:
            case NUM_DOUBLE:
            case NUM_DECIMAL:
            case NUM_FLOAT:
            case NUM_LONG:
            case OPEN:
            case COLON:
            case PARAM:
            case QUOTED_String:
            case IDENT:
            	{
                alt70 = 5;
                }
                break;
            	default:
            	    NoViableAltException nvae_d70s0 =
            	        new NoViableAltException("", 70, 0, input);

            	    throw nvae_d70s0;
            }

            switch (alt70) 
            {
                case 1 :
                    // Hql.g:505:4: m= MINUS mu= unaryExpression
                    {
                    	m=(IToken)Match(input,MINUS,FOLLOW_MINUS_in_unaryExpression2461);  
                    	stream_MINUS.Add(m);

                    	PushFollow(FOLLOW_unaryExpression_in_unaryExpression2465);
                    	mu = unaryExpression();
                    	state.followingStackPointer--;

                    	stream_unaryExpression.Add(mu.Tree);


                    	// AST REWRITE
                    	// elements:          mu
                    	// token labels:      
                    	// rule labels:       retval, mu
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);
                    	RewriteRuleSubtreeStream stream_mu = new RewriteRuleSubtreeStream(adaptor, "rule mu", mu!=null ? mu.Tree : null);

                    	root_0 = (IASTNode)adaptor.GetNilNode();
                    	// 505:31: -> ^( UNARY_MINUS[$m] $mu)
                    	{
                    	    // Hql.g:505:34: ^( UNARY_MINUS[$m] $mu)
                    	    {
                    	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
                    	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(UNARY_MINUS, m), root_1);

                    	    adaptor.AddChild(root_1, stream_mu.NextTree());

                    	    adaptor.AddChild(root_0, root_1);
                    	    }

                    	}

                    	retval.Tree = root_0;retval.Tree = root_0;
                    }
                    break;
                case 2 :
                    // Hql.g:506:4: p= PLUS pu= unaryExpression
                    {
                    	p=(IToken)Match(input,PLUS,FOLLOW_PLUS_in_unaryExpression2482);  
                    	stream_PLUS.Add(p);

                    	PushFollow(FOLLOW_unaryExpression_in_unaryExpression2486);
                    	pu = unaryExpression();
                    	state.followingStackPointer--;

                    	stream_unaryExpression.Add(pu.Tree);


                    	// AST REWRITE
                    	// elements:          pu
                    	// token labels:      
                    	// rule labels:       retval, pu
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);
                    	RewriteRuleSubtreeStream stream_pu = new RewriteRuleSubtreeStream(adaptor, "rule pu", pu!=null ? pu.Tree : null);

                    	root_0 = (IASTNode)adaptor.GetNilNode();
                    	// 506:30: -> ^( UNARY_PLUS[$p] $pu)
                    	{
                    	    // Hql.g:506:33: ^( UNARY_PLUS[$p] $pu)
                    	    {
                    	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
                    	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(UNARY_PLUS, p), root_1);

                    	    adaptor.AddChild(root_1, stream_pu.NextTree());

                    	    adaptor.AddChild(root_0, root_1);
                    	    }

                    	}

                    	retval.Tree = root_0;retval.Tree = root_0;
                    }
                    break;
                case 3 :
                    // Hql.g:507:4: c= caseExpression
                    {
                    	PushFollow(FOLLOW_caseExpression_in_unaryExpression2503);
                    	c = caseExpression();
                    	state.followingStackPointer--;

                    	stream_caseExpression.Add(c.Tree);


                    	// AST REWRITE
                    	// elements:          c
                    	// token labels:      
                    	// rule labels:       retval, c
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);
                    	RewriteRuleSubtreeStream stream_c = new RewriteRuleSubtreeStream(adaptor, "rule c", c!=null ? c.Tree : null);

                    	root_0 = (IASTNode)adaptor.GetNilNode();
                    	// 507:21: -> ^( $c)
                    	{
                    	    // Hql.g:507:24: ^( $c)
                    	    {
                    	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
                    	    root_1 = (IASTNode)adaptor.BecomeRoot(stream_c.NextNode(), root_1);

                    	    adaptor.AddChild(root_0, root_1);
                    	    }

                    	}

                    	retval.Tree = root_0;retval.Tree = root_0;
                    }
                    break;
                case 4 :
                    // Hql.g:508:4: q= quantifiedExpression
                    {
                    	PushFollow(FOLLOW_quantifiedExpression_in_unaryExpression2517);
                    	q = quantifiedExpression();
                    	state.followingStackPointer--;

                    	stream_quantifiedExpression.Add(q.Tree);


                    	// AST REWRITE
                    	// elements:          q
                    	// token labels:      
                    	// rule labels:       retval, q
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);
                    	RewriteRuleSubtreeStream stream_q = new RewriteRuleSubtreeStream(adaptor, "rule q", q!=null ? q.Tree : null);

                    	root_0 = (IASTNode)adaptor.GetNilNode();
                    	// 508:27: -> ^( $q)
                    	{
                    	    // Hql.g:508:30: ^( $q)
                    	    {
                    	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
                    	    root_1 = (IASTNode)adaptor.BecomeRoot(stream_q.NextNode(), root_1);

                    	    adaptor.AddChild(root_0, root_1);
                    	    }

                    	}

                    	retval.Tree = root_0;retval.Tree = root_0;
                    }
                    break;
                case 5 :
                    // Hql.g:509:4: a= atom
                    {
                    	PushFollow(FOLLOW_atom_in_unaryExpression2532);
                    	a = atom();
                    	state.followingStackPointer--;

                    	stream_atom.Add(a.Tree);


                    	// AST REWRITE
                    	// elements:          a
                    	// token labels:      
                    	// rule labels:       retval, a
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);
                    	RewriteRuleSubtreeStream stream_a = new RewriteRuleSubtreeStream(adaptor, "rule a", a!=null ? a.Tree : null);

                    	root_0 = (IASTNode)adaptor.GetNilNode();
                    	// 509:11: -> ^( $a)
                    	{
                    	    // Hql.g:509:14: ^( $a)
                    	    {
                    	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
                    	    root_1 = (IASTNode)adaptor.BecomeRoot(stream_a.NextNode(), root_1);

                    	    adaptor.AddChild(root_0, root_1);
                    	    }

                    	}

                    	retval.Tree = root_0;retval.Tree = root_0;
                    }
                    break;

            }
            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "unaryExpression"

    public class caseExpression_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "caseExpression"
    // Hql.g:512:1: caseExpression : ( CASE ( whenClause )+ ( elseClause )? END -> ^( CASE ( whenClause )+ ( elseClause )? ) | CASE unaryExpression ( altWhenClause )+ ( elseClause )? END -> ^( CASE2 unaryExpression ( altWhenClause )+ ( elseClause )? ) );
    public HqlParser.caseExpression_return caseExpression() // throws RecognitionException [1]
    {   
        HqlParser.caseExpression_return retval = new HqlParser.caseExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken CASE201 = null;
        IToken END204 = null;
        IToken CASE205 = null;
        IToken END209 = null;
        HqlParser.whenClause_return whenClause202 = default(HqlParser.whenClause_return);

        HqlParser.elseClause_return elseClause203 = default(HqlParser.elseClause_return);

        HqlParser.unaryExpression_return unaryExpression206 = default(HqlParser.unaryExpression_return);

        HqlParser.altWhenClause_return altWhenClause207 = default(HqlParser.altWhenClause_return);

        HqlParser.elseClause_return elseClause208 = default(HqlParser.elseClause_return);


        IASTNode CASE201_tree=null;
        IASTNode END204_tree=null;
        IASTNode CASE205_tree=null;
        IASTNode END209_tree=null;
        RewriteRuleTokenStream stream_END = new RewriteRuleTokenStream(adaptor,"token END");
        RewriteRuleTokenStream stream_CASE = new RewriteRuleTokenStream(adaptor,"token CASE");
        RewriteRuleSubtreeStream stream_whenClause = new RewriteRuleSubtreeStream(adaptor,"rule whenClause");
        RewriteRuleSubtreeStream stream_unaryExpression = new RewriteRuleSubtreeStream(adaptor,"rule unaryExpression");
        RewriteRuleSubtreeStream stream_altWhenClause = new RewriteRuleSubtreeStream(adaptor,"rule altWhenClause");
        RewriteRuleSubtreeStream stream_elseClause = new RewriteRuleSubtreeStream(adaptor,"rule elseClause");
        try 
    	{
            // Hql.g:513:2: ( CASE ( whenClause )+ ( elseClause )? END -> ^( CASE ( whenClause )+ ( elseClause )? ) | CASE unaryExpression ( altWhenClause )+ ( elseClause )? END -> ^( CASE2 unaryExpression ( altWhenClause )+ ( elseClause )? ) )
            int alt75 = 2;
            int LA75_0 = input.LA(1);

            if ( (LA75_0 == CASE) )
            {
                int LA75_1 = input.LA(2);

                if ( ((LA75_1 >= ALL && LA75_1 <= ANY) || LA75_1 == AVG || LA75_1 == COUNT || LA75_1 == ELEMENTS || (LA75_1 >= EXISTS && LA75_1 <= FALSE) || LA75_1 == INDICES || (LA75_1 >= MAX && LA75_1 <= MIN) || LA75_1 == NULL || (LA75_1 >= SOME && LA75_1 <= SUM) || LA75_1 == TRUE || LA75_1 == CASE || LA75_1 == EMPTY || (LA75_1 >= NUM_INT && LA75_1 <= NUM_LONG) || LA75_1 == OPEN || (LA75_1 >= PLUS && LA75_1 <= MINUS) || (LA75_1 >= COLON && LA75_1 <= IDENT)) )
                {
                    alt75 = 2;
                }
                else if ( (LA75_1 == WHEN) )
                {
                    alt75 = 1;
                }
                else 
                {
                    NoViableAltException nvae_d75s1 =
                        new NoViableAltException("", 75, 1, input);

                    throw nvae_d75s1;
                }
            }
            else 
            {
                NoViableAltException nvae_d75s0 =
                    new NoViableAltException("", 75, 0, input);

                throw nvae_d75s0;
            }
            switch (alt75) 
            {
                case 1 :
                    // Hql.g:513:4: CASE ( whenClause )+ ( elseClause )? END
                    {
                    	CASE201=(IToken)Match(input,CASE,FOLLOW_CASE_in_caseExpression2551);  
                    	stream_CASE.Add(CASE201);

                    	// Hql.g:513:9: ( whenClause )+
                    	int cnt71 = 0;
                    	do 
                    	{
                    	    int alt71 = 2;
                    	    int LA71_0 = input.LA(1);

                    	    if ( (LA71_0 == WHEN) )
                    	    {
                    	        alt71 = 1;
                    	    }


                    	    switch (alt71) 
                    		{
                    			case 1 :
                    			    // Hql.g:513:10: whenClause
                    			    {
                    			    	PushFollow(FOLLOW_whenClause_in_caseExpression2554);
                    			    	whenClause202 = whenClause();
                    			    	state.followingStackPointer--;

                    			    	stream_whenClause.Add(whenClause202.Tree);

                    			    }
                    			    break;

                    			default:
                    			    if ( cnt71 >= 1 ) goto loop71;
                    		            EarlyExitException eee71 =
                    		                new EarlyExitException(71, input);
                    		            throw eee71;
                    	    }
                    	    cnt71++;
                    	} while (true);

                    	loop71:
                    		;	// Stops C# compiler whining that label 'loop71' has no statements

                    	// Hql.g:513:23: ( elseClause )?
                    	int alt72 = 2;
                    	int LA72_0 = input.LA(1);

                    	if ( (LA72_0 == ELSE) )
                    	{
                    	    alt72 = 1;
                    	}
                    	switch (alt72) 
                    	{
                    	    case 1 :
                    	        // Hql.g:513:24: elseClause
                    	        {
                    	        	PushFollow(FOLLOW_elseClause_in_caseExpression2559);
                    	        	elseClause203 = elseClause();
                    	        	state.followingStackPointer--;

                    	        	stream_elseClause.Add(elseClause203.Tree);

                    	        }
                    	        break;

                    	}

                    	END204=(IToken)Match(input,END,FOLLOW_END_in_caseExpression2563);  
                    	stream_END.Add(END204);



                    	// AST REWRITE
                    	// elements:          whenClause, elseClause, CASE
                    	// token labels:      
                    	// rule labels:       retval
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

                    	root_0 = (IASTNode)adaptor.GetNilNode();
                    	// 514:3: -> ^( CASE ( whenClause )+ ( elseClause )? )
                    	{
                    	    // Hql.g:514:6: ^( CASE ( whenClause )+ ( elseClause )? )
                    	    {
                    	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
                    	    root_1 = (IASTNode)adaptor.BecomeRoot(stream_CASE.NextNode(), root_1);

                    	    if ( !(stream_whenClause.HasNext()) ) {
                    	        throw new RewriteEarlyExitException();
                    	    }
                    	    while ( stream_whenClause.HasNext() )
                    	    {
                    	        adaptor.AddChild(root_1, stream_whenClause.NextTree());

                    	    }
                    	    stream_whenClause.Reset();
                    	    // Hql.g:514:25: ( elseClause )?
                    	    if ( stream_elseClause.HasNext() )
                    	    {
                    	        adaptor.AddChild(root_1, stream_elseClause.NextTree());

                    	    }
                    	    stream_elseClause.Reset();

                    	    adaptor.AddChild(root_0, root_1);
                    	    }

                    	}

                    	retval.Tree = root_0;retval.Tree = root_0;
                    }
                    break;
                case 2 :
                    // Hql.g:515:4: CASE unaryExpression ( altWhenClause )+ ( elseClause )? END
                    {
                    	CASE205=(IToken)Match(input,CASE,FOLLOW_CASE_in_caseExpression2583);  
                    	stream_CASE.Add(CASE205);

                    	PushFollow(FOLLOW_unaryExpression_in_caseExpression2585);
                    	unaryExpression206 = unaryExpression();
                    	state.followingStackPointer--;

                    	stream_unaryExpression.Add(unaryExpression206.Tree);
                    	// Hql.g:515:25: ( altWhenClause )+
                    	int cnt73 = 0;
                    	do 
                    	{
                    	    int alt73 = 2;
                    	    int LA73_0 = input.LA(1);

                    	    if ( (LA73_0 == WHEN) )
                    	    {
                    	        alt73 = 1;
                    	    }


                    	    switch (alt73) 
                    		{
                    			case 1 :
                    			    // Hql.g:515:26: altWhenClause
                    			    {
                    			    	PushFollow(FOLLOW_altWhenClause_in_caseExpression2588);
                    			    	altWhenClause207 = altWhenClause();
                    			    	state.followingStackPointer--;

                    			    	stream_altWhenClause.Add(altWhenClause207.Tree);

                    			    }
                    			    break;

                    			default:
                    			    if ( cnt73 >= 1 ) goto loop73;
                    		            EarlyExitException eee73 =
                    		                new EarlyExitException(73, input);
                    		            throw eee73;
                    	    }
                    	    cnt73++;
                    	} while (true);

                    	loop73:
                    		;	// Stops C# compiler whining that label 'loop73' has no statements

                    	// Hql.g:515:42: ( elseClause )?
                    	int alt74 = 2;
                    	int LA74_0 = input.LA(1);

                    	if ( (LA74_0 == ELSE) )
                    	{
                    	    alt74 = 1;
                    	}
                    	switch (alt74) 
                    	{
                    	    case 1 :
                    	        // Hql.g:515:43: elseClause
                    	        {
                    	        	PushFollow(FOLLOW_elseClause_in_caseExpression2593);
                    	        	elseClause208 = elseClause();
                    	        	state.followingStackPointer--;

                    	        	stream_elseClause.Add(elseClause208.Tree);

                    	        }
                    	        break;

                    	}

                    	END209=(IToken)Match(input,END,FOLLOW_END_in_caseExpression2597);  
                    	stream_END.Add(END209);



                    	// AST REWRITE
                    	// elements:          altWhenClause, unaryExpression, elseClause
                    	// token labels:      
                    	// rule labels:       retval
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

                    	root_0 = (IASTNode)adaptor.GetNilNode();
                    	// 516:3: -> ^( CASE2 unaryExpression ( altWhenClause )+ ( elseClause )? )
                    	{
                    	    // Hql.g:516:6: ^( CASE2 unaryExpression ( altWhenClause )+ ( elseClause )? )
                    	    {
                    	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
                    	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(CASE2, "CASE2"), root_1);

                    	    adaptor.AddChild(root_1, stream_unaryExpression.NextTree());
                    	    if ( !(stream_altWhenClause.HasNext()) ) {
                    	        throw new RewriteEarlyExitException();
                    	    }
                    	    while ( stream_altWhenClause.HasNext() )
                    	    {
                    	        adaptor.AddChild(root_1, stream_altWhenClause.NextTree());

                    	    }
                    	    stream_altWhenClause.Reset();
                    	    // Hql.g:516:45: ( elseClause )?
                    	    if ( stream_elseClause.HasNext() )
                    	    {
                    	        adaptor.AddChild(root_1, stream_elseClause.NextTree());

                    	    }
                    	    stream_elseClause.Reset();

                    	    adaptor.AddChild(root_0, root_1);
                    	    }

                    	}

                    	retval.Tree = root_0;retval.Tree = root_0;
                    }
                    break;

            }
            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "caseExpression"

    public class whenClause_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "whenClause"
    // Hql.g:519:1: whenClause : ( WHEN logicalExpression THEN expression ) ;
    public HqlParser.whenClause_return whenClause() // throws RecognitionException [1]
    {   
        HqlParser.whenClause_return retval = new HqlParser.whenClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken WHEN210 = null;
        IToken THEN212 = null;
        HqlParser.logicalExpression_return logicalExpression211 = default(HqlParser.logicalExpression_return);

        HqlParser.expression_return expression213 = default(HqlParser.expression_return);


        IASTNode WHEN210_tree=null;
        IASTNode THEN212_tree=null;

        try 
    	{
            // Hql.g:520:2: ( ( WHEN logicalExpression THEN expression ) )
            // Hql.g:520:4: ( WHEN logicalExpression THEN expression )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// Hql.g:520:4: ( WHEN logicalExpression THEN expression )
            	// Hql.g:520:5: WHEN logicalExpression THEN expression
            	{
            		WHEN210=(IToken)Match(input,WHEN,FOLLOW_WHEN_in_whenClause2626); 
            			WHEN210_tree = (IASTNode)adaptor.Create(WHEN210);
            			root_0 = (IASTNode)adaptor.BecomeRoot(WHEN210_tree, root_0);

            		PushFollow(FOLLOW_logicalExpression_in_whenClause2629);
            		logicalExpression211 = logicalExpression();
            		state.followingStackPointer--;

            		adaptor.AddChild(root_0, logicalExpression211.Tree);
            		THEN212=(IToken)Match(input,THEN,FOLLOW_THEN_in_whenClause2631); 
            		PushFollow(FOLLOW_expression_in_whenClause2634);
            		expression213 = expression();
            		state.followingStackPointer--;

            		adaptor.AddChild(root_0, expression213.Tree);

            	}


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "whenClause"

    public class altWhenClause_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "altWhenClause"
    // Hql.g:523:1: altWhenClause : ( WHEN unaryExpression THEN expression ) ;
    public HqlParser.altWhenClause_return altWhenClause() // throws RecognitionException [1]
    {   
        HqlParser.altWhenClause_return retval = new HqlParser.altWhenClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken WHEN214 = null;
        IToken THEN216 = null;
        HqlParser.unaryExpression_return unaryExpression215 = default(HqlParser.unaryExpression_return);

        HqlParser.expression_return expression217 = default(HqlParser.expression_return);


        IASTNode WHEN214_tree=null;
        IASTNode THEN216_tree=null;

        try 
    	{
            // Hql.g:524:2: ( ( WHEN unaryExpression THEN expression ) )
            // Hql.g:524:4: ( WHEN unaryExpression THEN expression )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// Hql.g:524:4: ( WHEN unaryExpression THEN expression )
            	// Hql.g:524:5: WHEN unaryExpression THEN expression
            	{
            		WHEN214=(IToken)Match(input,WHEN,FOLLOW_WHEN_in_altWhenClause2648); 
            			WHEN214_tree = (IASTNode)adaptor.Create(WHEN214);
            			root_0 = (IASTNode)adaptor.BecomeRoot(WHEN214_tree, root_0);

            		PushFollow(FOLLOW_unaryExpression_in_altWhenClause2651);
            		unaryExpression215 = unaryExpression();
            		state.followingStackPointer--;

            		adaptor.AddChild(root_0, unaryExpression215.Tree);
            		THEN216=(IToken)Match(input,THEN,FOLLOW_THEN_in_altWhenClause2653); 
            		PushFollow(FOLLOW_expression_in_altWhenClause2656);
            		expression217 = expression();
            		state.followingStackPointer--;

            		adaptor.AddChild(root_0, expression217.Tree);

            	}


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "altWhenClause"

    public class elseClause_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "elseClause"
    // Hql.g:527:1: elseClause : ( ELSE expression ) ;
    public HqlParser.elseClause_return elseClause() // throws RecognitionException [1]
    {   
        HqlParser.elseClause_return retval = new HqlParser.elseClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken ELSE218 = null;
        HqlParser.expression_return expression219 = default(HqlParser.expression_return);


        IASTNode ELSE218_tree=null;

        try 
    	{
            // Hql.g:528:2: ( ( ELSE expression ) )
            // Hql.g:528:4: ( ELSE expression )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// Hql.g:528:4: ( ELSE expression )
            	// Hql.g:528:5: ELSE expression
            	{
            		ELSE218=(IToken)Match(input,ELSE,FOLLOW_ELSE_in_elseClause2670); 
            			ELSE218_tree = (IASTNode)adaptor.Create(ELSE218);
            			root_0 = (IASTNode)adaptor.BecomeRoot(ELSE218_tree, root_0);

            		PushFollow(FOLLOW_expression_in_elseClause2673);
            		expression219 = expression();
            		state.followingStackPointer--;

            		adaptor.AddChild(root_0, expression219.Tree);

            	}


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "elseClause"

    public class quantifiedExpression_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "quantifiedExpression"
    // Hql.g:531:1: quantifiedExpression : ( SOME | EXISTS | ALL | ANY ) ( identifier | collectionExpr | ( OPEN ( subQuery ) CLOSE ) ) ;
    public HqlParser.quantifiedExpression_return quantifiedExpression() // throws RecognitionException [1]
    {   
        HqlParser.quantifiedExpression_return retval = new HqlParser.quantifiedExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken SOME220 = null;
        IToken EXISTS221 = null;
        IToken ALL222 = null;
        IToken ANY223 = null;
        IToken OPEN226 = null;
        IToken CLOSE228 = null;
        HqlParser.identifier_return identifier224 = default(HqlParser.identifier_return);

        HqlParser.collectionExpr_return collectionExpr225 = default(HqlParser.collectionExpr_return);

        HqlParser.subQuery_return subQuery227 = default(HqlParser.subQuery_return);


        IASTNode SOME220_tree=null;
        IASTNode EXISTS221_tree=null;
        IASTNode ALL222_tree=null;
        IASTNode ANY223_tree=null;
        IASTNode OPEN226_tree=null;
        IASTNode CLOSE228_tree=null;

        try 
    	{
            // Hql.g:532:2: ( ( SOME | EXISTS | ALL | ANY ) ( identifier | collectionExpr | ( OPEN ( subQuery ) CLOSE ) ) )
            // Hql.g:532:4: ( SOME | EXISTS | ALL | ANY ) ( identifier | collectionExpr | ( OPEN ( subQuery ) CLOSE ) )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// Hql.g:532:4: ( SOME | EXISTS | ALL | ANY )
            	int alt76 = 4;
            	switch ( input.LA(1) ) 
            	{
            	case SOME:
            		{
            	    alt76 = 1;
            	    }
            	    break;
            	case EXISTS:
            		{
            	    alt76 = 2;
            	    }
            	    break;
            	case ALL:
            		{
            	    alt76 = 3;
            	    }
            	    break;
            	case ANY:
            		{
            	    alt76 = 4;
            	    }
            	    break;
            		default:
            		    NoViableAltException nvae_d76s0 =
            		        new NoViableAltException("", 76, 0, input);

            		    throw nvae_d76s0;
            	}

            	switch (alt76) 
            	{
            	    case 1 :
            	        // Hql.g:532:6: SOME
            	        {
            	        	SOME220=(IToken)Match(input,SOME,FOLLOW_SOME_in_quantifiedExpression2688); 
            	        		SOME220_tree = (IASTNode)adaptor.Create(SOME220);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(SOME220_tree, root_0);


            	        }
            	        break;
            	    case 2 :
            	        // Hql.g:532:14: EXISTS
            	        {
            	        	EXISTS221=(IToken)Match(input,EXISTS,FOLLOW_EXISTS_in_quantifiedExpression2693); 
            	        		EXISTS221_tree = (IASTNode)adaptor.Create(EXISTS221);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(EXISTS221_tree, root_0);


            	        }
            	        break;
            	    case 3 :
            	        // Hql.g:532:24: ALL
            	        {
            	        	ALL222=(IToken)Match(input,ALL,FOLLOW_ALL_in_quantifiedExpression2698); 
            	        		ALL222_tree = (IASTNode)adaptor.Create(ALL222);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(ALL222_tree, root_0);


            	        }
            	        break;
            	    case 4 :
            	        // Hql.g:532:31: ANY
            	        {
            	        	ANY223=(IToken)Match(input,ANY,FOLLOW_ANY_in_quantifiedExpression2703); 
            	        		ANY223_tree = (IASTNode)adaptor.Create(ANY223);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(ANY223_tree, root_0);


            	        }
            	        break;

            	}

            	// Hql.g:533:2: ( identifier | collectionExpr | ( OPEN ( subQuery ) CLOSE ) )
            	int alt77 = 3;
            	switch ( input.LA(1) ) 
            	{
            	case IDENT:
            		{
            	    alt77 = 1;
            	    }
            	    break;
            	case ELEMENTS:
            	case INDICES:
            		{
            	    alt77 = 2;
            	    }
            	    break;
            	case OPEN:
            		{
            	    alt77 = 3;
            	    }
            	    break;
            		default:
            		    NoViableAltException nvae_d77s0 =
            		        new NoViableAltException("", 77, 0, input);

            		    throw nvae_d77s0;
            	}

            	switch (alt77) 
            	{
            	    case 1 :
            	        // Hql.g:533:4: identifier
            	        {
            	        	PushFollow(FOLLOW_identifier_in_quantifiedExpression2712);
            	        	identifier224 = identifier();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, identifier224.Tree);

            	        }
            	        break;
            	    case 2 :
            	        // Hql.g:533:17: collectionExpr
            	        {
            	        	PushFollow(FOLLOW_collectionExpr_in_quantifiedExpression2716);
            	        	collectionExpr225 = collectionExpr();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, collectionExpr225.Tree);

            	        }
            	        break;
            	    case 3 :
            	        // Hql.g:533:34: ( OPEN ( subQuery ) CLOSE )
            	        {
            	        	// Hql.g:533:34: ( OPEN ( subQuery ) CLOSE )
            	        	// Hql.g:533:35: OPEN ( subQuery ) CLOSE
            	        	{
            	        		OPEN226=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_quantifiedExpression2721); 
            	        		// Hql.g:533:41: ( subQuery )
            	        		// Hql.g:533:43: subQuery
            	        		{
            	        			PushFollow(FOLLOW_subQuery_in_quantifiedExpression2726);
            	        			subQuery227 = subQuery();
            	        			state.followingStackPointer--;

            	        			adaptor.AddChild(root_0, subQuery227.Tree);

            	        		}

            	        		CLOSE228=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_quantifiedExpression2730); 

            	        	}


            	        }
            	        break;

            	}


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "quantifiedExpression"

    public class atom_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "atom"
    // Hql.g:539:1: atom : primaryExpression ( DOT identifier ( options {greedy=true; } : (op= OPEN exprList CLOSE ) )? | lb= OPEN_BRACKET expression CLOSE_BRACKET )* ;
    public HqlParser.atom_return atom() // throws RecognitionException [1]
    {   
        HqlParser.atom_return retval = new HqlParser.atom_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken op = null;
        IToken lb = null;
        IToken DOT230 = null;
        IToken CLOSE233 = null;
        IToken CLOSE_BRACKET235 = null;
        HqlParser.primaryExpression_return primaryExpression229 = default(HqlParser.primaryExpression_return);

        HqlParser.identifier_return identifier231 = default(HqlParser.identifier_return);

        HqlParser.exprList_return exprList232 = default(HqlParser.exprList_return);

        HqlParser.expression_return expression234 = default(HqlParser.expression_return);


        IASTNode op_tree=null;
        IASTNode lb_tree=null;
        IASTNode DOT230_tree=null;
        IASTNode CLOSE233_tree=null;
        IASTNode CLOSE_BRACKET235_tree=null;

        try 
    	{
            // Hql.g:540:3: ( primaryExpression ( DOT identifier ( options {greedy=true; } : (op= OPEN exprList CLOSE ) )? | lb= OPEN_BRACKET expression CLOSE_BRACKET )* )
            // Hql.g:540:5: primaryExpression ( DOT identifier ( options {greedy=true; } : (op= OPEN exprList CLOSE ) )? | lb= OPEN_BRACKET expression CLOSE_BRACKET )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_primaryExpression_in_atom2749);
            	primaryExpression229 = primaryExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, primaryExpression229.Tree);
            	// Hql.g:541:3: ( DOT identifier ( options {greedy=true; } : (op= OPEN exprList CLOSE ) )? | lb= OPEN_BRACKET expression CLOSE_BRACKET )*
            	do 
            	{
            	    int alt79 = 3;
            	    int LA79_0 = input.LA(1);

            	    if ( (LA79_0 == DOT) )
            	    {
            	        alt79 = 1;
            	    }
            	    else if ( (LA79_0 == OPEN_BRACKET) )
            	    {
            	        alt79 = 2;
            	    }


            	    switch (alt79) 
            		{
            			case 1 :
            			    // Hql.g:542:4: DOT identifier ( options {greedy=true; } : (op= OPEN exprList CLOSE ) )?
            			    {
            			    	DOT230=(IToken)Match(input,DOT,FOLLOW_DOT_in_atom2758); 
            			    		DOT230_tree = (IASTNode)adaptor.Create(DOT230);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(DOT230_tree, root_0);

            			    	PushFollow(FOLLOW_identifier_in_atom2761);
            			    	identifier231 = identifier();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, identifier231.Tree);
            			    	// Hql.g:543:5: ( options {greedy=true; } : (op= OPEN exprList CLOSE ) )?
            			    	int alt78 = 2;
            			    	int LA78_0 = input.LA(1);

            			    	if ( (LA78_0 == OPEN) )
            			    	{
            			    	    alt78 = 1;
            			    	}
            			    	switch (alt78) 
            			    	{
            			    	    case 1 :
            			    	        // Hql.g:544:6: (op= OPEN exprList CLOSE )
            			    	        {
            			    	        	// Hql.g:544:6: (op= OPEN exprList CLOSE )
            			    	        	// Hql.g:544:8: op= OPEN exprList CLOSE
            			    	        	{
            			    	        		op=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_atom2789); 
            			    	        			op_tree = (IASTNode)adaptor.Create(op);
            			    	        			root_0 = (IASTNode)adaptor.BecomeRoot(op_tree, root_0);

            			    	        		op.Type = METHOD_CALL; 
            			    	        		PushFollow(FOLLOW_exprList_in_atom2794);
            			    	        		exprList232 = exprList();
            			    	        		state.followingStackPointer--;

            			    	        		adaptor.AddChild(root_0, exprList232.Tree);
            			    	        		CLOSE233=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_atom2796); 

            			    	        	}


            			    	        }
            			    	        break;

            			    	}


            			    }
            			    break;
            			case 2 :
            			    // Hql.g:545:5: lb= OPEN_BRACKET expression CLOSE_BRACKET
            			    {
            			    	lb=(IToken)Match(input,OPEN_BRACKET,FOLLOW_OPEN_BRACKET_in_atom2810); 
            			    		lb_tree = (IASTNode)adaptor.Create(lb);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(lb_tree, root_0);

            			    	lb.Type = INDEX_OP; 
            			    	PushFollow(FOLLOW_expression_in_atom2815);
            			    	expression234 = expression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, expression234.Tree);
            			    	CLOSE_BRACKET235=(IToken)Match(input,CLOSE_BRACKET,FOLLOW_CLOSE_BRACKET_in_atom2817); 

            			    }
            			    break;

            			default:
            			    goto loop79;
            	    }
            	} while (true);

            	loop79:
            		;	// Stops C# compiler whining that label 'loop79' has no statements


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "atom"

    public class primaryExpression_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "primaryExpression"
    // Hql.g:550:1: primaryExpression : ( identPrimary ( options {greedy=true; } : DOT 'class' )? | constant | COLON identifier | OPEN ( expressionOrVector | subQuery ) CLOSE | PARAM ( NUM_INT )? );
    public HqlParser.primaryExpression_return primaryExpression() // throws RecognitionException [1]
    {   
        HqlParser.primaryExpression_return retval = new HqlParser.primaryExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken DOT237 = null;
        IToken string_literal238 = null;
        IToken COLON240 = null;
        IToken OPEN242 = null;
        IToken CLOSE245 = null;
        IToken PARAM246 = null;
        IToken NUM_INT247 = null;
        HqlParser.identPrimary_return identPrimary236 = default(HqlParser.identPrimary_return);

        HqlParser.constant_return constant239 = default(HqlParser.constant_return);

        HqlParser.identifier_return identifier241 = default(HqlParser.identifier_return);

        HqlParser.expressionOrVector_return expressionOrVector243 = default(HqlParser.expressionOrVector_return);

        HqlParser.subQuery_return subQuery244 = default(HqlParser.subQuery_return);


        IASTNode DOT237_tree=null;
        IASTNode string_literal238_tree=null;
        IASTNode COLON240_tree=null;
        IASTNode OPEN242_tree=null;
        IASTNode CLOSE245_tree=null;
        IASTNode PARAM246_tree=null;
        IASTNode NUM_INT247_tree=null;

        try 
    	{
            // Hql.g:551:2: ( identPrimary ( options {greedy=true; } : DOT 'class' )? | constant | COLON identifier | OPEN ( expressionOrVector | subQuery ) CLOSE | PARAM ( NUM_INT )? )
            int alt83 = 5;
            switch ( input.LA(1) ) 
            {
            case AVG:
            case COUNT:
            case ELEMENTS:
            case INDICES:
            case MAX:
            case MIN:
            case SUM:
            case IDENT:
            	{
                alt83 = 1;
                }
                break;
            case FALSE:
            case NULL:
            case TRUE:
            case EMPTY:
            case NUM_INT:
            case NUM_DOUBLE:
            case NUM_DECIMAL:
            case NUM_FLOAT:
            case NUM_LONG:
            case QUOTED_String:
            	{
                alt83 = 2;
                }
                break;
            case COLON:
            	{
                alt83 = 3;
                }
                break;
            case OPEN:
            	{
                alt83 = 4;
                }
                break;
            case PARAM:
            	{
                alt83 = 5;
                }
                break;
            	default:
            	    NoViableAltException nvae_d83s0 =
            	        new NoViableAltException("", 83, 0, input);

            	    throw nvae_d83s0;
            }

            switch (alt83) 
            {
                case 1 :
                    // Hql.g:551:6: identPrimary ( options {greedy=true; } : DOT 'class' )?
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_identPrimary_in_primaryExpression2837);
                    	identPrimary236 = identPrimary();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, identPrimary236.Tree);
                    	// Hql.g:551:19: ( options {greedy=true; } : DOT 'class' )?
                    	int alt80 = 2;
                    	int LA80_0 = input.LA(1);

                    	if ( (LA80_0 == DOT) )
                    	{
                    	    int LA80_1 = input.LA(2);

                    	    if ( (LA80_1 == CLASS) )
                    	    {
                    	        alt80 = 1;
                    	    }
                    	}
                    	switch (alt80) 
                    	{
                    	    case 1 :
                    	        // Hql.g:551:46: DOT 'class'
                    	        {
                    	        	DOT237=(IToken)Match(input,DOT,FOLLOW_DOT_in_primaryExpression2850); 
                    	        		DOT237_tree = (IASTNode)adaptor.Create(DOT237);
                    	        		root_0 = (IASTNode)adaptor.BecomeRoot(DOT237_tree, root_0);

                    	        	string_literal238=(IToken)Match(input,CLASS,FOLLOW_CLASS_in_primaryExpression2853); 
                    	        		string_literal238_tree = (IASTNode)adaptor.Create(string_literal238);
                    	        		adaptor.AddChild(root_0, string_literal238_tree);


                    	        }
                    	        break;

                    	}


                    }
                    break;
                case 2 :
                    // Hql.g:552:6: constant
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_constant_in_primaryExpression2863);
                    	constant239 = constant();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, constant239.Tree);

                    }
                    break;
                case 3 :
                    // Hql.g:553:6: COLON identifier
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	COLON240=(IToken)Match(input,COLON,FOLLOW_COLON_in_primaryExpression2870); 
                    		COLON240_tree = (IASTNode)adaptor.Create(COLON240);
                    		root_0 = (IASTNode)adaptor.BecomeRoot(COLON240_tree, root_0);

                    	PushFollow(FOLLOW_identifier_in_primaryExpression2873);
                    	identifier241 = identifier();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, identifier241.Tree);

                    }
                    break;
                case 4 :
                    // Hql.g:555:6: OPEN ( expressionOrVector | subQuery ) CLOSE
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	OPEN242=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_primaryExpression2882); 
                    	// Hql.g:555:12: ( expressionOrVector | subQuery )
                    	int alt81 = 2;
                    	int LA81_0 = input.LA(1);

                    	if ( ((LA81_0 >= ALL && LA81_0 <= ANY) || LA81_0 == AVG || LA81_0 == COUNT || LA81_0 == ELEMENTS || (LA81_0 >= EXISTS && LA81_0 <= FALSE) || LA81_0 == INDICES || (LA81_0 >= MAX && LA81_0 <= MIN) || (LA81_0 >= NOT && LA81_0 <= NULL) || (LA81_0 >= SOME && LA81_0 <= SUM) || LA81_0 == TRUE || LA81_0 == CASE || LA81_0 == EMPTY || (LA81_0 >= NUM_INT && LA81_0 <= NUM_LONG) || LA81_0 == OPEN || LA81_0 == BNOT || (LA81_0 >= PLUS && LA81_0 <= MINUS) || (LA81_0 >= COLON && LA81_0 <= IDENT)) )
                    	{
                    	    alt81 = 1;
                    	}
                    	else if ( (LA81_0 == EOF || LA81_0 == FROM || (LA81_0 >= GROUP && LA81_0 <= HAVING) || LA81_0 == ORDER || LA81_0 == SELECT || LA81_0 == SKIP || LA81_0 == TAKE || LA81_0 == UNION || LA81_0 == WHERE || LA81_0 == CLOSE) )
                    	{
                    	    alt81 = 2;
                    	}
                    	else 
                    	{
                    	    NoViableAltException nvae_d81s0 =
                    	        new NoViableAltException("", 81, 0, input);

                    	    throw nvae_d81s0;
                    	}
                    	switch (alt81) 
                    	{
                    	    case 1 :
                    	        // Hql.g:555:13: expressionOrVector
                    	        {
                    	        	PushFollow(FOLLOW_expressionOrVector_in_primaryExpression2886);
                    	        	expressionOrVector243 = expressionOrVector();
                    	        	state.followingStackPointer--;

                    	        	adaptor.AddChild(root_0, expressionOrVector243.Tree);

                    	        }
                    	        break;
                    	    case 2 :
                    	        // Hql.g:555:34: subQuery
                    	        {
                    	        	PushFollow(FOLLOW_subQuery_in_primaryExpression2890);
                    	        	subQuery244 = subQuery();
                    	        	state.followingStackPointer--;

                    	        	adaptor.AddChild(root_0, subQuery244.Tree);

                    	        }
                    	        break;

                    	}

                    	CLOSE245=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_primaryExpression2893); 

                    }
                    break;
                case 5 :
                    // Hql.g:556:6: PARAM ( NUM_INT )?
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PARAM246=(IToken)Match(input,PARAM,FOLLOW_PARAM_in_primaryExpression2901); 
                    		PARAM246_tree = (IASTNode)adaptor.Create(PARAM246);
                    		root_0 = (IASTNode)adaptor.BecomeRoot(PARAM246_tree, root_0);

                    	// Hql.g:556:13: ( NUM_INT )?
                    	int alt82 = 2;
                    	int LA82_0 = input.LA(1);

                    	if ( (LA82_0 == NUM_INT) )
                    	{
                    	    alt82 = 1;
                    	}
                    	switch (alt82) 
                    	{
                    	    case 1 :
                    	        // Hql.g:556:14: NUM_INT
                    	        {
                    	        	NUM_INT247=(IToken)Match(input,NUM_INT,FOLLOW_NUM_INT_in_primaryExpression2905); 
                    	        		NUM_INT247_tree = (IASTNode)adaptor.Create(NUM_INT247);
                    	        		adaptor.AddChild(root_0, NUM_INT247_tree);


                    	        }
                    	        break;

                    	}


                    }
                    break;

            }
            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "primaryExpression"

    public class expressionOrVector_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "expressionOrVector"
    // Hql.g:561:1: expressionOrVector : e= expression (v= vectorExpr )? -> {v != null}? ^( VECTOR_EXPR[\"{vector}\"] $e $v) -> ^( $e) ;
    public HqlParser.expressionOrVector_return expressionOrVector() // throws RecognitionException [1]
    {   
        HqlParser.expressionOrVector_return retval = new HqlParser.expressionOrVector_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.expression_return e = default(HqlParser.expression_return);

        HqlParser.vectorExpr_return v = default(HqlParser.vectorExpr_return);


        RewriteRuleSubtreeStream stream_expression = new RewriteRuleSubtreeStream(adaptor,"rule expression");
        RewriteRuleSubtreeStream stream_vectorExpr = new RewriteRuleSubtreeStream(adaptor,"rule vectorExpr");
        try 
    	{
            // Hql.g:562:2: (e= expression (v= vectorExpr )? -> {v != null}? ^( VECTOR_EXPR[\"{vector}\"] $e $v) -> ^( $e) )
            // Hql.g:562:4: e= expression (v= vectorExpr )?
            {
            	PushFollow(FOLLOW_expression_in_expressionOrVector2923);
            	e = expression();
            	state.followingStackPointer--;

            	stream_expression.Add(e.Tree);
            	// Hql.g:562:17: (v= vectorExpr )?
            	int alt84 = 2;
            	int LA84_0 = input.LA(1);

            	if ( (LA84_0 == COMMA) )
            	{
            	    alt84 = 1;
            	}
            	switch (alt84) 
            	{
            	    case 1 :
            	        // Hql.g:562:19: v= vectorExpr
            	        {
            	        	PushFollow(FOLLOW_vectorExpr_in_expressionOrVector2929);
            	        	v = vectorExpr();
            	        	state.followingStackPointer--;

            	        	stream_vectorExpr.Add(v.Tree);

            	        }
            	        break;

            	}



            	// AST REWRITE
            	// elements:          e, e, v
            	// token labels:      
            	// rule labels:       v, retval, e
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_v = new RewriteRuleSubtreeStream(adaptor, "rule v", v!=null ? v.Tree : null);
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);
            	RewriteRuleSubtreeStream stream_e = new RewriteRuleSubtreeStream(adaptor, "rule e", e!=null ? e.Tree : null);

            	root_0 = (IASTNode)adaptor.GetNilNode();
            	// 563:2: -> {v != null}? ^( VECTOR_EXPR[\"{vector}\"] $e $v)
            	if (v != null)
            	{
            	    // Hql.g:563:18: ^( VECTOR_EXPR[\"{vector}\"] $e $v)
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(VECTOR_EXPR, "{vector}"), root_1);

            	    adaptor.AddChild(root_1, stream_e.NextTree());
            	    adaptor.AddChild(root_1, stream_v.NextTree());

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}
            	else // 564:2: -> ^( $e)
            	{
            	    // Hql.g:564:5: ^( $e)
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot(stream_e.NextNode(), root_1);

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;
            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "expressionOrVector"

    public class vectorExpr_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "vectorExpr"
    // Hql.g:567:1: vectorExpr : COMMA expression ( COMMA expression )* ;
    public HqlParser.vectorExpr_return vectorExpr() // throws RecognitionException [1]
    {   
        HqlParser.vectorExpr_return retval = new HqlParser.vectorExpr_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken COMMA248 = null;
        IToken COMMA250 = null;
        HqlParser.expression_return expression249 = default(HqlParser.expression_return);

        HqlParser.expression_return expression251 = default(HqlParser.expression_return);


        IASTNode COMMA248_tree=null;
        IASTNode COMMA250_tree=null;

        try 
    	{
            // Hql.g:568:2: ( COMMA expression ( COMMA expression )* )
            // Hql.g:568:4: COMMA expression ( COMMA expression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	COMMA248=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_vectorExpr2968); 
            	PushFollow(FOLLOW_expression_in_vectorExpr2971);
            	expression249 = expression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, expression249.Tree);
            	// Hql.g:568:22: ( COMMA expression )*
            	do 
            	{
            	    int alt85 = 2;
            	    int LA85_0 = input.LA(1);

            	    if ( (LA85_0 == COMMA) )
            	    {
            	        alt85 = 1;
            	    }


            	    switch (alt85) 
            		{
            			case 1 :
            			    // Hql.g:568:23: COMMA expression
            			    {
            			    	COMMA250=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_vectorExpr2974); 
            			    	PushFollow(FOLLOW_expression_in_vectorExpr2977);
            			    	expression251 = expression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, expression251.Tree);

            			    }
            			    break;

            			default:
            			    goto loop85;
            	    }
            	} while (true);

            	loop85:
            		;	// Stops C# compiler whining that label 'loop85' has no statements


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "vectorExpr"

    public class identPrimary_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "identPrimary"
    // Hql.g:574:1: identPrimary : ( identifier ( options {greedy=true; } : DOT ( identifier | o= OBJECT ) )* ( (op= OPEN exprList CLOSE ) )? | aggregate );
    public HqlParser.identPrimary_return identPrimary() // throws RecognitionException [1]
    {   
        HqlParser.identPrimary_return retval = new HqlParser.identPrimary_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken o = null;
        IToken op = null;
        IToken DOT253 = null;
        IToken CLOSE256 = null;
        HqlParser.identifier_return identifier252 = default(HqlParser.identifier_return);

        HqlParser.identifier_return identifier254 = default(HqlParser.identifier_return);

        HqlParser.exprList_return exprList255 = default(HqlParser.exprList_return);

        HqlParser.aggregate_return aggregate257 = default(HqlParser.aggregate_return);


        IASTNode o_tree=null;
        IASTNode op_tree=null;
        IASTNode DOT253_tree=null;
        IASTNode CLOSE256_tree=null;

        try 
    	{
            // Hql.g:575:2: ( identifier ( options {greedy=true; } : DOT ( identifier | o= OBJECT ) )* ( (op= OPEN exprList CLOSE ) )? | aggregate )
            int alt89 = 2;
            int LA89_0 = input.LA(1);

            if ( (LA89_0 == IDENT) )
            {
                alt89 = 1;
            }
            else if ( (LA89_0 == AVG || LA89_0 == COUNT || LA89_0 == ELEMENTS || LA89_0 == INDICES || (LA89_0 >= MAX && LA89_0 <= MIN) || LA89_0 == SUM) )
            {
                alt89 = 2;
            }
            else 
            {
                NoViableAltException nvae_d89s0 =
                    new NoViableAltException("", 89, 0, input);

                throw nvae_d89s0;
            }
            switch (alt89) 
            {
                case 1 :
                    // Hql.g:575:4: identifier ( options {greedy=true; } : DOT ( identifier | o= OBJECT ) )* ( (op= OPEN exprList CLOSE ) )?
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_identifier_in_identPrimary2993);
                    	identifier252 = identifier();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, identifier252.Tree);
                    	 HandleDotIdent(); 
                    	// Hql.g:576:4: ( options {greedy=true; } : DOT ( identifier | o= OBJECT ) )*
                    	do 
                    	{
                    	    int alt87 = 2;
                    	    int LA87_0 = input.LA(1);

                    	    if ( (LA87_0 == DOT) )
                    	    {
                    	        int LA87_2 = input.LA(2);

                    	        if ( (LA87_2 == OBJECT || LA87_2 == IDENT) )
                    	        {
                    	            alt87 = 1;
                    	        }


                    	    }


                    	    switch (alt87) 
                    		{
                    			case 1 :
                    			    // Hql.g:576:31: DOT ( identifier | o= OBJECT )
                    			    {
                    			    	DOT253=(IToken)Match(input,DOT,FOLLOW_DOT_in_identPrimary3011); 
                    			    		DOT253_tree = (IASTNode)adaptor.Create(DOT253);
                    			    		root_0 = (IASTNode)adaptor.BecomeRoot(DOT253_tree, root_0);

                    			    	// Hql.g:576:36: ( identifier | o= OBJECT )
                    			    	int alt86 = 2;
                    			    	int LA86_0 = input.LA(1);

                    			    	if ( (LA86_0 == IDENT) )
                    			    	{
                    			    	    alt86 = 1;
                    			    	}
                    			    	else if ( (LA86_0 == OBJECT) )
                    			    	{
                    			    	    alt86 = 2;
                    			    	}
                    			    	else 
                    			    	{
                    			    	    NoViableAltException nvae_d86s0 =
                    			    	        new NoViableAltException("", 86, 0, input);

                    			    	    throw nvae_d86s0;
                    			    	}
                    			    	switch (alt86) 
                    			    	{
                    			    	    case 1 :
                    			    	        // Hql.g:576:38: identifier
                    			    	        {
                    			    	        	PushFollow(FOLLOW_identifier_in_identPrimary3016);
                    			    	        	identifier254 = identifier();
                    			    	        	state.followingStackPointer--;

                    			    	        	adaptor.AddChild(root_0, identifier254.Tree);

                    			    	        }
                    			    	        break;
                    			    	    case 2 :
                    			    	        // Hql.g:576:51: o= OBJECT
                    			    	        {
                    			    	        	o=(IToken)Match(input,OBJECT,FOLLOW_OBJECT_in_identPrimary3022); 
                    			    	        		o_tree = (IASTNode)adaptor.Create(o);
                    			    	        		adaptor.AddChild(root_0, o_tree);

                    			    	        	 o.Type = IDENT; 

                    			    	        }
                    			    	        break;

                    			    	}


                    			    }
                    			    break;

                    			default:
                    			    goto loop87;
                    	    }
                    	} while (true);

                    	loop87:
                    		;	// Stops C# compiler whining that label 'loop87' has no statements

                    	// Hql.g:577:4: ( (op= OPEN exprList CLOSE ) )?
                    	int alt88 = 2;
                    	int LA88_0 = input.LA(1);

                    	if ( (LA88_0 == OPEN) )
                    	{
                    	    alt88 = 1;
                    	}
                    	switch (alt88) 
                    	{
                    	    case 1 :
                    	        // Hql.g:577:6: (op= OPEN exprList CLOSE )
                    	        {
                    	        	// Hql.g:577:6: (op= OPEN exprList CLOSE )
                    	        	// Hql.g:577:8: op= OPEN exprList CLOSE
                    	        	{
                    	        		op=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_identPrimary3040); 
                    	        			op_tree = (IASTNode)adaptor.Create(op);
                    	        			root_0 = (IASTNode)adaptor.BecomeRoot(op_tree, root_0);

                    	        		 op.Type = METHOD_CALL;
                    	        		PushFollow(FOLLOW_exprList_in_identPrimary3045);
                    	        		exprList255 = exprList();
                    	        		state.followingStackPointer--;

                    	        		adaptor.AddChild(root_0, exprList255.Tree);
                    	        		CLOSE256=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_identPrimary3047); 

                    	        	}


                    	        }
                    	        break;

                    	}


                    }
                    break;
                case 2 :
                    // Hql.g:580:4: aggregate
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_aggregate_in_identPrimary3063);
                    	aggregate257 = aggregate();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, aggregate257.Tree);

                    }
                    break;

            }
            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "identPrimary"

    public class aggregate_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "aggregate"
    // Hql.g:588:1: aggregate : ( (op= SUM | op= AVG | op= MAX | op= MIN ) OPEN additiveExpression CLOSE -> ^( AGGREGATE[$op] additiveExpression ) | COUNT OPEN (s= STAR | p= aggregateDistinctAll ) CLOSE -> {s == null}? ^( COUNT $p) -> ^( COUNT ^( ROW_STAR[\"*\"] ) ) | collectionExpr );
    public HqlParser.aggregate_return aggregate() // throws RecognitionException [1]
    {   
        HqlParser.aggregate_return retval = new HqlParser.aggregate_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken op = null;
        IToken s = null;
        IToken OPEN258 = null;
        IToken CLOSE260 = null;
        IToken COUNT261 = null;
        IToken OPEN262 = null;
        IToken CLOSE263 = null;
        HqlParser.aggregateDistinctAll_return p = default(HqlParser.aggregateDistinctAll_return);

        HqlParser.additiveExpression_return additiveExpression259 = default(HqlParser.additiveExpression_return);

        HqlParser.collectionExpr_return collectionExpr264 = default(HqlParser.collectionExpr_return);


        IASTNode op_tree=null;
        IASTNode s_tree=null;
        IASTNode OPEN258_tree=null;
        IASTNode CLOSE260_tree=null;
        IASTNode COUNT261_tree=null;
        IASTNode OPEN262_tree=null;
        IASTNode CLOSE263_tree=null;
        RewriteRuleTokenStream stream_OPEN = new RewriteRuleTokenStream(adaptor,"token OPEN");
        RewriteRuleTokenStream stream_MAX = new RewriteRuleTokenStream(adaptor,"token MAX");
        RewriteRuleTokenStream stream_COUNT = new RewriteRuleTokenStream(adaptor,"token COUNT");
        RewriteRuleTokenStream stream_STAR = new RewriteRuleTokenStream(adaptor,"token STAR");
        RewriteRuleTokenStream stream_MIN = new RewriteRuleTokenStream(adaptor,"token MIN");
        RewriteRuleTokenStream stream_CLOSE = new RewriteRuleTokenStream(adaptor,"token CLOSE");
        RewriteRuleTokenStream stream_SUM = new RewriteRuleTokenStream(adaptor,"token SUM");
        RewriteRuleTokenStream stream_AVG = new RewriteRuleTokenStream(adaptor,"token AVG");
        RewriteRuleSubtreeStream stream_aggregateDistinctAll = new RewriteRuleSubtreeStream(adaptor,"rule aggregateDistinctAll");
        RewriteRuleSubtreeStream stream_additiveExpression = new RewriteRuleSubtreeStream(adaptor,"rule additiveExpression");
        try 
    	{
            // Hql.g:589:2: ( (op= SUM | op= AVG | op= MAX | op= MIN ) OPEN additiveExpression CLOSE -> ^( AGGREGATE[$op] additiveExpression ) | COUNT OPEN (s= STAR | p= aggregateDistinctAll ) CLOSE -> {s == null}? ^( COUNT $p) -> ^( COUNT ^( ROW_STAR[\"*\"] ) ) | collectionExpr )
            int alt92 = 3;
            switch ( input.LA(1) ) 
            {
            case AVG:
            case MAX:
            case MIN:
            case SUM:
            	{
                alt92 = 1;
                }
                break;
            case COUNT:
            	{
                alt92 = 2;
                }
                break;
            case ELEMENTS:
            case INDICES:
            	{
                alt92 = 3;
                }
                break;
            	default:
            	    NoViableAltException nvae_d92s0 =
            	        new NoViableAltException("", 92, 0, input);

            	    throw nvae_d92s0;
            }

            switch (alt92) 
            {
                case 1 :
                    // Hql.g:589:4: (op= SUM | op= AVG | op= MAX | op= MIN ) OPEN additiveExpression CLOSE
                    {
                    	// Hql.g:589:4: (op= SUM | op= AVG | op= MAX | op= MIN )
                    	int alt90 = 4;
                    	switch ( input.LA(1) ) 
                    	{
                    	case SUM:
                    		{
                    	    alt90 = 1;
                    	    }
                    	    break;
                    	case AVG:
                    		{
                    	    alt90 = 2;
                    	    }
                    	    break;
                    	case MAX:
                    		{
                    	    alt90 = 3;
                    	    }
                    	    break;
                    	case MIN:
                    		{
                    	    alt90 = 4;
                    	    }
                    	    break;
                    		default:
                    		    NoViableAltException nvae_d90s0 =
                    		        new NoViableAltException("", 90, 0, input);

                    		    throw nvae_d90s0;
                    	}

                    	switch (alt90) 
                    	{
                    	    case 1 :
                    	        // Hql.g:589:6: op= SUM
                    	        {
                    	        	op=(IToken)Match(input,SUM,FOLLOW_SUM_in_aggregate3084);  
                    	        	stream_SUM.Add(op);


                    	        }
                    	        break;
                    	    case 2 :
                    	        // Hql.g:589:15: op= AVG
                    	        {
                    	        	op=(IToken)Match(input,AVG,FOLLOW_AVG_in_aggregate3090);  
                    	        	stream_AVG.Add(op);


                    	        }
                    	        break;
                    	    case 3 :
                    	        // Hql.g:589:24: op= MAX
                    	        {
                    	        	op=(IToken)Match(input,MAX,FOLLOW_MAX_in_aggregate3096);  
                    	        	stream_MAX.Add(op);


                    	        }
                    	        break;
                    	    case 4 :
                    	        // Hql.g:589:33: op= MIN
                    	        {
                    	        	op=(IToken)Match(input,MIN,FOLLOW_MIN_in_aggregate3102);  
                    	        	stream_MIN.Add(op);


                    	        }
                    	        break;

                    	}

                    	OPEN258=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_aggregate3106);  
                    	stream_OPEN.Add(OPEN258);

                    	PushFollow(FOLLOW_additiveExpression_in_aggregate3108);
                    	additiveExpression259 = additiveExpression();
                    	state.followingStackPointer--;

                    	stream_additiveExpression.Add(additiveExpression259.Tree);
                    	CLOSE260=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_aggregate3110);  
                    	stream_CLOSE.Add(CLOSE260);



                    	// AST REWRITE
                    	// elements:          additiveExpression
                    	// token labels:      
                    	// rule labels:       retval
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

                    	root_0 = (IASTNode)adaptor.GetNilNode();
                    	// 590:3: -> ^( AGGREGATE[$op] additiveExpression )
                    	{
                    	    // Hql.g:590:6: ^( AGGREGATE[$op] additiveExpression )
                    	    {
                    	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
                    	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(AGGREGATE, op), root_1);

                    	    adaptor.AddChild(root_1, stream_additiveExpression.NextTree());

                    	    adaptor.AddChild(root_0, root_1);
                    	    }

                    	}

                    	retval.Tree = root_0;retval.Tree = root_0;
                    }
                    break;
                case 2 :
                    // Hql.g:592:5: COUNT OPEN (s= STAR | p= aggregateDistinctAll ) CLOSE
                    {
                    	COUNT261=(IToken)Match(input,COUNT,FOLLOW_COUNT_in_aggregate3129);  
                    	stream_COUNT.Add(COUNT261);

                    	OPEN262=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_aggregate3131);  
                    	stream_OPEN.Add(OPEN262);

                    	// Hql.g:592:16: (s= STAR | p= aggregateDistinctAll )
                    	int alt91 = 2;
                    	int LA91_0 = input.LA(1);

                    	if ( (LA91_0 == STAR) )
                    	{
                    	    alt91 = 1;
                    	}
                    	else if ( (LA91_0 == ALL || (LA91_0 >= DISTINCT && LA91_0 <= ELEMENTS) || LA91_0 == INDICES || LA91_0 == IDENT) )
                    	{
                    	    alt91 = 2;
                    	}
                    	else 
                    	{
                    	    NoViableAltException nvae_d91s0 =
                    	        new NoViableAltException("", 91, 0, input);

                    	    throw nvae_d91s0;
                    	}
                    	switch (alt91) 
                    	{
                    	    case 1 :
                    	        // Hql.g:592:18: s= STAR
                    	        {
                    	        	s=(IToken)Match(input,STAR,FOLLOW_STAR_in_aggregate3137);  
                    	        	stream_STAR.Add(s);


                    	        }
                    	        break;
                    	    case 2 :
                    	        // Hql.g:592:27: p= aggregateDistinctAll
                    	        {
                    	        	PushFollow(FOLLOW_aggregateDistinctAll_in_aggregate3143);
                    	        	p = aggregateDistinctAll();
                    	        	state.followingStackPointer--;

                    	        	stream_aggregateDistinctAll.Add(p.Tree);

                    	        }
                    	        break;

                    	}

                    	CLOSE263=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_aggregate3147);  
                    	stream_CLOSE.Add(CLOSE263);



                    	// AST REWRITE
                    	// elements:          p, COUNT, COUNT
                    	// token labels:      
                    	// rule labels:       retval, p
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);
                    	RewriteRuleSubtreeStream stream_p = new RewriteRuleSubtreeStream(adaptor, "rule p", p!=null ? p.Tree : null);

                    	root_0 = (IASTNode)adaptor.GetNilNode();
                    	// 593:3: -> {s == null}? ^( COUNT $p)
                    	if (s == null)
                    	{
                    	    // Hql.g:593:19: ^( COUNT $p)
                    	    {
                    	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
                    	    root_1 = (IASTNode)adaptor.BecomeRoot(stream_COUNT.NextNode(), root_1);

                    	    adaptor.AddChild(root_1, stream_p.NextTree());

                    	    adaptor.AddChild(root_0, root_1);
                    	    }

                    	}
                    	else // 594:3: -> ^( COUNT ^( ROW_STAR[\"*\"] ) )
                    	{
                    	    // Hql.g:594:6: ^( COUNT ^( ROW_STAR[\"*\"] ) )
                    	    {
                    	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
                    	    root_1 = (IASTNode)adaptor.BecomeRoot(stream_COUNT.NextNode(), root_1);

                    	    // Hql.g:594:14: ^( ROW_STAR[\"*\"] )
                    	    {
                    	    IASTNode root_2 = (IASTNode)adaptor.GetNilNode();
                    	    root_2 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(ROW_STAR, "*"), root_2);

                    	    adaptor.AddChild(root_1, root_2);
                    	    }

                    	    adaptor.AddChild(root_0, root_1);
                    	    }

                    	}

                    	retval.Tree = root_0;retval.Tree = root_0;
                    }
                    break;
                case 3 :
                    // Hql.g:595:5: collectionExpr
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_collectionExpr_in_aggregate3179);
                    	collectionExpr264 = collectionExpr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, collectionExpr264.Tree);

                    }
                    break;

            }
            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "aggregate"

    public class aggregateDistinctAll_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "aggregateDistinctAll"
    // Hql.g:598:1: aggregateDistinctAll : ( ( DISTINCT | ALL )? ( path | collectionExpr ) ) ;
    public HqlParser.aggregateDistinctAll_return aggregateDistinctAll() // throws RecognitionException [1]
    {   
        HqlParser.aggregateDistinctAll_return retval = new HqlParser.aggregateDistinctAll_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken set265 = null;
        HqlParser.path_return path266 = default(HqlParser.path_return);

        HqlParser.collectionExpr_return collectionExpr267 = default(HqlParser.collectionExpr_return);


        IASTNode set265_tree=null;

        try 
    	{
            // Hql.g:599:2: ( ( ( DISTINCT | ALL )? ( path | collectionExpr ) ) )
            // Hql.g:599:4: ( ( DISTINCT | ALL )? ( path | collectionExpr ) )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// Hql.g:599:4: ( ( DISTINCT | ALL )? ( path | collectionExpr ) )
            	// Hql.g:599:6: ( DISTINCT | ALL )? ( path | collectionExpr )
            	{
            		// Hql.g:599:6: ( DISTINCT | ALL )?
            		int alt93 = 2;
            		int LA93_0 = input.LA(1);

            		if ( (LA93_0 == ALL || LA93_0 == DISTINCT) )
            		{
            		    alt93 = 1;
            		}
            		switch (alt93) 
            		{
            		    case 1 :
            		        // Hql.g:
            		        {
            		        	set265 = (IToken)input.LT(1);
            		        	if ( input.LA(1) == ALL || input.LA(1) == DISTINCT ) 
            		        	{
            		        	    input.Consume();
            		        	    adaptor.AddChild(root_0, (IASTNode)adaptor.Create(set265));
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

            		// Hql.g:599:26: ( path | collectionExpr )
            		int alt94 = 2;
            		int LA94_0 = input.LA(1);

            		if ( (LA94_0 == IDENT) )
            		{
            		    alt94 = 1;
            		}
            		else if ( (LA94_0 == ELEMENTS || LA94_0 == INDICES) )
            		{
            		    alt94 = 2;
            		}
            		else 
            		{
            		    NoViableAltException nvae_d94s0 =
            		        new NoViableAltException("", 94, 0, input);

            		    throw nvae_d94s0;
            		}
            		switch (alt94) 
            		{
            		    case 1 :
            		        // Hql.g:599:28: path
            		        {
            		        	PushFollow(FOLLOW_path_in_aggregateDistinctAll3205);
            		        	path266 = path();
            		        	state.followingStackPointer--;

            		        	adaptor.AddChild(root_0, path266.Tree);

            		        }
            		        break;
            		    case 2 :
            		        // Hql.g:599:35: collectionExpr
            		        {
            		        	PushFollow(FOLLOW_collectionExpr_in_aggregateDistinctAll3209);
            		        	collectionExpr267 = collectionExpr();
            		        	state.followingStackPointer--;

            		        	adaptor.AddChild(root_0, collectionExpr267.Tree);

            		        }
            		        break;

            		}


            	}


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "aggregateDistinctAll"

    public class collectionExpr_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "collectionExpr"
    // Hql.g:604:1: collectionExpr : ( ELEMENTS | INDICES ) OPEN path CLOSE ;
    public HqlParser.collectionExpr_return collectionExpr() // throws RecognitionException [1]
    {   
        HqlParser.collectionExpr_return retval = new HqlParser.collectionExpr_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken ELEMENTS268 = null;
        IToken INDICES269 = null;
        IToken OPEN270 = null;
        IToken CLOSE272 = null;
        HqlParser.path_return path271 = default(HqlParser.path_return);


        IASTNode ELEMENTS268_tree=null;
        IASTNode INDICES269_tree=null;
        IASTNode OPEN270_tree=null;
        IASTNode CLOSE272_tree=null;

        try 
    	{
            // Hql.g:605:2: ( ( ELEMENTS | INDICES ) OPEN path CLOSE )
            // Hql.g:605:4: ( ELEMENTS | INDICES ) OPEN path CLOSE
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// Hql.g:605:4: ( ELEMENTS | INDICES )
            	int alt95 = 2;
            	int LA95_0 = input.LA(1);

            	if ( (LA95_0 == ELEMENTS) )
            	{
            	    alt95 = 1;
            	}
            	else if ( (LA95_0 == INDICES) )
            	{
            	    alt95 = 2;
            	}
            	else 
            	{
            	    NoViableAltException nvae_d95s0 =
            	        new NoViableAltException("", 95, 0, input);

            	    throw nvae_d95s0;
            	}
            	switch (alt95) 
            	{
            	    case 1 :
            	        // Hql.g:605:5: ELEMENTS
            	        {
            	        	ELEMENTS268=(IToken)Match(input,ELEMENTS,FOLLOW_ELEMENTS_in_collectionExpr3228); 
            	        		ELEMENTS268_tree = (IASTNode)adaptor.Create(ELEMENTS268);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(ELEMENTS268_tree, root_0);


            	        }
            	        break;
            	    case 2 :
            	        // Hql.g:605:17: INDICES
            	        {
            	        	INDICES269=(IToken)Match(input,INDICES,FOLLOW_INDICES_in_collectionExpr3233); 
            	        		INDICES269_tree = (IASTNode)adaptor.Create(INDICES269);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(INDICES269_tree, root_0);


            	        }
            	        break;

            	}

            	OPEN270=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_collectionExpr3237); 
            	PushFollow(FOLLOW_path_in_collectionExpr3240);
            	path271 = path();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, path271.Tree);
            	CLOSE272=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_collectionExpr3242); 

            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "collectionExpr"

    public class compoundExpr_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "compoundExpr"
    // Hql.g:608:1: compoundExpr : ( collectionExpr | path | ( OPEN ( subQuery | ( expression ( COMMA expression )* ) ) CLOSE ) );
    public HqlParser.compoundExpr_return compoundExpr() // throws RecognitionException [1]
    {   
        HqlParser.compoundExpr_return retval = new HqlParser.compoundExpr_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken OPEN275 = null;
        IToken COMMA278 = null;
        IToken CLOSE280 = null;
        HqlParser.collectionExpr_return collectionExpr273 = default(HqlParser.collectionExpr_return);

        HqlParser.path_return path274 = default(HqlParser.path_return);

        HqlParser.subQuery_return subQuery276 = default(HqlParser.subQuery_return);

        HqlParser.expression_return expression277 = default(HqlParser.expression_return);

        HqlParser.expression_return expression279 = default(HqlParser.expression_return);


        IASTNode OPEN275_tree=null;
        IASTNode COMMA278_tree=null;
        IASTNode CLOSE280_tree=null;

        try 
    	{
            // Hql.g:609:2: ( collectionExpr | path | ( OPEN ( subQuery | ( expression ( COMMA expression )* ) ) CLOSE ) )
            int alt98 = 3;
            switch ( input.LA(1) ) 
            {
            case ELEMENTS:
            case INDICES:
            	{
                alt98 = 1;
                }
                break;
            case IDENT:
            	{
                alt98 = 2;
                }
                break;
            case OPEN:
            	{
                alt98 = 3;
                }
                break;
            	default:
            	    NoViableAltException nvae_d98s0 =
            	        new NoViableAltException("", 98, 0, input);

            	    throw nvae_d98s0;
            }

            switch (alt98) 
            {
                case 1 :
                    // Hql.g:609:4: collectionExpr
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_collectionExpr_in_compoundExpr3297);
                    	collectionExpr273 = collectionExpr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, collectionExpr273.Tree);

                    }
                    break;
                case 2 :
                    // Hql.g:610:4: path
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_path_in_compoundExpr3302);
                    	path274 = path();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, path274.Tree);

                    }
                    break;
                case 3 :
                    // Hql.g:611:4: ( OPEN ( subQuery | ( expression ( COMMA expression )* ) ) CLOSE )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	// Hql.g:611:4: ( OPEN ( subQuery | ( expression ( COMMA expression )* ) ) CLOSE )
                    	// Hql.g:611:5: OPEN ( subQuery | ( expression ( COMMA expression )* ) ) CLOSE
                    	{
                    		OPEN275=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_compoundExpr3308); 
                    		// Hql.g:611:11: ( subQuery | ( expression ( COMMA expression )* ) )
                    		int alt97 = 2;
                    		int LA97_0 = input.LA(1);

                    		if ( (LA97_0 == EOF || LA97_0 == FROM || (LA97_0 >= GROUP && LA97_0 <= HAVING) || LA97_0 == ORDER || LA97_0 == SELECT || LA97_0 == SKIP || LA97_0 == TAKE || LA97_0 == UNION || LA97_0 == WHERE || LA97_0 == CLOSE) )
                    		{
                    		    alt97 = 1;
                    		}
                    		else if ( ((LA97_0 >= ALL && LA97_0 <= ANY) || LA97_0 == AVG || LA97_0 == COUNT || LA97_0 == ELEMENTS || (LA97_0 >= EXISTS && LA97_0 <= FALSE) || LA97_0 == INDICES || (LA97_0 >= MAX && LA97_0 <= MIN) || (LA97_0 >= NOT && LA97_0 <= NULL) || (LA97_0 >= SOME && LA97_0 <= SUM) || LA97_0 == TRUE || LA97_0 == CASE || LA97_0 == EMPTY || (LA97_0 >= NUM_INT && LA97_0 <= NUM_LONG) || LA97_0 == OPEN || LA97_0 == BNOT || (LA97_0 >= PLUS && LA97_0 <= MINUS) || (LA97_0 >= COLON && LA97_0 <= IDENT)) )
                    		{
                    		    alt97 = 2;
                    		}
                    		else 
                    		{
                    		    NoViableAltException nvae_d97s0 =
                    		        new NoViableAltException("", 97, 0, input);

                    		    throw nvae_d97s0;
                    		}
                    		switch (alt97) 
                    		{
                    		    case 1 :
                    		        // Hql.g:611:13: subQuery
                    		        {
                    		        	PushFollow(FOLLOW_subQuery_in_compoundExpr3313);
                    		        	subQuery276 = subQuery();
                    		        	state.followingStackPointer--;

                    		        	adaptor.AddChild(root_0, subQuery276.Tree);

                    		        }
                    		        break;
                    		    case 2 :
                    		        // Hql.g:611:24: ( expression ( COMMA expression )* )
                    		        {
                    		        	// Hql.g:611:24: ( expression ( COMMA expression )* )
                    		        	// Hql.g:611:25: expression ( COMMA expression )*
                    		        	{
                    		        		PushFollow(FOLLOW_expression_in_compoundExpr3318);
                    		        		expression277 = expression();
                    		        		state.followingStackPointer--;

                    		        		adaptor.AddChild(root_0, expression277.Tree);
                    		        		// Hql.g:611:36: ( COMMA expression )*
                    		        		do 
                    		        		{
                    		        		    int alt96 = 2;
                    		        		    int LA96_0 = input.LA(1);

                    		        		    if ( (LA96_0 == COMMA) )
                    		        		    {
                    		        		        alt96 = 1;
                    		        		    }


                    		        		    switch (alt96) 
                    		        			{
                    		        				case 1 :
                    		        				    // Hql.g:611:37: COMMA expression
                    		        				    {
                    		        				    	COMMA278=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_compoundExpr3321); 
                    		        				    	PushFollow(FOLLOW_expression_in_compoundExpr3324);
                    		        				    	expression279 = expression();
                    		        				    	state.followingStackPointer--;

                    		        				    	adaptor.AddChild(root_0, expression279.Tree);

                    		        				    }
                    		        				    break;

                    		        				default:
                    		        				    goto loop96;
                    		        		    }
                    		        		} while (true);

                    		        		loop96:
                    		        			;	// Stops C# compiler whining that label 'loop96' has no statements


                    		        	}


                    		        }
                    		        break;

                    		}

                    		CLOSE280=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_compoundExpr3331); 

                    	}


                    }
                    break;

            }
            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "compoundExpr"

    public class exprList_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "exprList"
    // Hql.g:614:1: exprList : ( TRAILING | LEADING | BOTH )? ( expression ( ( COMMA expression )+ | f= FROM expression | AS identifier )? | f2= FROM expression )? ;
    public HqlParser.exprList_return exprList() // throws RecognitionException [1]
    {   
        HqlParser.exprList_return retval = new HqlParser.exprList_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken f = null;
        IToken f2 = null;
        IToken TRAILING281 = null;
        IToken LEADING282 = null;
        IToken BOTH283 = null;
        IToken COMMA285 = null;
        IToken AS288 = null;
        HqlParser.expression_return expression284 = default(HqlParser.expression_return);

        HqlParser.expression_return expression286 = default(HqlParser.expression_return);

        HqlParser.expression_return expression287 = default(HqlParser.expression_return);

        HqlParser.identifier_return identifier289 = default(HqlParser.identifier_return);

        HqlParser.expression_return expression290 = default(HqlParser.expression_return);


        IASTNode f_tree=null;
        IASTNode f2_tree=null;
        IASTNode TRAILING281_tree=null;
        IASTNode LEADING282_tree=null;
        IASTNode BOTH283_tree=null;
        IASTNode COMMA285_tree=null;
        IASTNode AS288_tree=null;

        try 
    	{
            // Hql.g:620:2: ( ( TRAILING | LEADING | BOTH )? ( expression ( ( COMMA expression )+ | f= FROM expression | AS identifier )? | f2= FROM expression )? )
            // Hql.g:620:4: ( TRAILING | LEADING | BOTH )? ( expression ( ( COMMA expression )+ | f= FROM expression | AS identifier )? | f2= FROM expression )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// Hql.g:620:4: ( TRAILING | LEADING | BOTH )?
            	int alt99 = 4;
            	switch ( input.LA(1) ) 
            	{
            	    case TRAILING:
            	    	{
            	        alt99 = 1;
            	        }
            	        break;
            	    case LEADING:
            	    	{
            	        alt99 = 2;
            	        }
            	        break;
            	    case BOTH:
            	    	{
            	        alt99 = 3;
            	        }
            	        break;
            	}

            	switch (alt99) 
            	{
            	    case 1 :
            	        // Hql.g:620:5: TRAILING
            	        {
            	        	TRAILING281=(IToken)Match(input,TRAILING,FOLLOW_TRAILING_in_exprList3350); 
            	        		TRAILING281_tree = (IASTNode)adaptor.Create(TRAILING281);
            	        		adaptor.AddChild(root_0, TRAILING281_tree);

            	        	TRAILING281.Type = IDENT;

            	        }
            	        break;
            	    case 2 :
            	        // Hql.g:621:10: LEADING
            	        {
            	        	LEADING282=(IToken)Match(input,LEADING,FOLLOW_LEADING_in_exprList3363); 
            	        		LEADING282_tree = (IASTNode)adaptor.Create(LEADING282);
            	        		adaptor.AddChild(root_0, LEADING282_tree);

            	        	LEADING282.Type = IDENT;

            	        }
            	        break;
            	    case 3 :
            	        // Hql.g:622:10: BOTH
            	        {
            	        	BOTH283=(IToken)Match(input,BOTH,FOLLOW_BOTH_in_exprList3376); 
            	        		BOTH283_tree = (IASTNode)adaptor.Create(BOTH283);
            	        		adaptor.AddChild(root_0, BOTH283_tree);

            	        	BOTH283.Type = IDENT;

            	        }
            	        break;

            	}

            	// Hql.g:624:4: ( expression ( ( COMMA expression )+ | f= FROM expression | AS identifier )? | f2= FROM expression )?
            	int alt102 = 3;
            	int LA102_0 = input.LA(1);

            	if ( ((LA102_0 >= ALL && LA102_0 <= ANY) || LA102_0 == AVG || LA102_0 == COUNT || LA102_0 == ELEMENTS || (LA102_0 >= EXISTS && LA102_0 <= FALSE) || LA102_0 == INDICES || (LA102_0 >= MAX && LA102_0 <= MIN) || (LA102_0 >= NOT && LA102_0 <= NULL) || (LA102_0 >= SOME && LA102_0 <= SUM) || LA102_0 == TRUE || LA102_0 == CASE || LA102_0 == EMPTY || (LA102_0 >= NUM_INT && LA102_0 <= NUM_LONG) || LA102_0 == OPEN || LA102_0 == BNOT || (LA102_0 >= PLUS && LA102_0 <= MINUS) || (LA102_0 >= COLON && LA102_0 <= IDENT)) )
            	{
            	    alt102 = 1;
            	}
            	else if ( (LA102_0 == FROM) )
            	{
            	    alt102 = 2;
            	}
            	switch (alt102) 
            	{
            	    case 1 :
            	        // Hql.g:625:5: expression ( ( COMMA expression )+ | f= FROM expression | AS identifier )?
            	        {
            	        	PushFollow(FOLLOW_expression_in_exprList3400);
            	        	expression284 = expression();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, expression284.Tree);
            	        	// Hql.g:625:16: ( ( COMMA expression )+ | f= FROM expression | AS identifier )?
            	        	int alt101 = 4;
            	        	switch ( input.LA(1) ) 
            	        	{
            	        	    case COMMA:
            	        	    	{
            	        	        alt101 = 1;
            	        	        }
            	        	        break;
            	        	    case FROM:
            	        	    	{
            	        	        alt101 = 2;
            	        	        }
            	        	        break;
            	        	    case AS:
            	        	    	{
            	        	        alt101 = 3;
            	        	        }
            	        	        break;
            	        	}

            	        	switch (alt101) 
            	        	{
            	        	    case 1 :
            	        	        // Hql.g:625:18: ( COMMA expression )+
            	        	        {
            	        	        	// Hql.g:625:18: ( COMMA expression )+
            	        	        	int cnt100 = 0;
            	        	        	do 
            	        	        	{
            	        	        	    int alt100 = 2;
            	        	        	    int LA100_0 = input.LA(1);

            	        	        	    if ( (LA100_0 == COMMA) )
            	        	        	    {
            	        	        	        alt100 = 1;
            	        	        	    }


            	        	        	    switch (alt100) 
            	        	        		{
            	        	        			case 1 :
            	        	        			    // Hql.g:625:19: COMMA expression
            	        	        			    {
            	        	        			    	COMMA285=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_exprList3405); 
            	        	        			    	PushFollow(FOLLOW_expression_in_exprList3408);
            	        	        			    	expression286 = expression();
            	        	        			    	state.followingStackPointer--;

            	        	        			    	adaptor.AddChild(root_0, expression286.Tree);

            	        	        			    }
            	        	        			    break;

            	        	        			default:
            	        	        			    if ( cnt100 >= 1 ) goto loop100;
            	        	        		            EarlyExitException eee100 =
            	        	        		                new EarlyExitException(100, input);
            	        	        		            throw eee100;
            	        	        	    }
            	        	        	    cnt100++;
            	        	        	} while (true);

            	        	        	loop100:
            	        	        		;	// Stops C# compiler whining that label 'loop100' has no statements


            	        	        }
            	        	        break;
            	        	    case 2 :
            	        	        // Hql.g:626:9: f= FROM expression
            	        	        {
            	        	        	f=(IToken)Match(input,FROM,FOLLOW_FROM_in_exprList3423); 
            	        	        		f_tree = (IASTNode)adaptor.Create(f);
            	        	        		adaptor.AddChild(root_0, f_tree);

            	        	        	PushFollow(FOLLOW_expression_in_exprList3425);
            	        	        	expression287 = expression();
            	        	        	state.followingStackPointer--;

            	        	        	adaptor.AddChild(root_0, expression287.Tree);
            	        	        	f.Type = IDENT;

            	        	        }
            	        	        break;
            	        	    case 3 :
            	        	        // Hql.g:627:9: AS identifier
            	        	        {
            	        	        	AS288=(IToken)Match(input,AS,FOLLOW_AS_in_exprList3437); 
            	        	        	PushFollow(FOLLOW_identifier_in_exprList3440);
            	        	        	identifier289 = identifier();
            	        	        	state.followingStackPointer--;

            	        	        	adaptor.AddChild(root_0, identifier289.Tree);

            	        	        }
            	        	        break;

            	        	}


            	        }
            	        break;
            	    case 2 :
            	        // Hql.g:628:7: f2= FROM expression
            	        {
            	        	f2=(IToken)Match(input,FROM,FOLLOW_FROM_in_exprList3454); 
            	        		f2_tree = (IASTNode)adaptor.Create(f2);
            	        		adaptor.AddChild(root_0, f2_tree);

            	        	PushFollow(FOLLOW_expression_in_exprList3456);
            	        	expression290 = expression();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, expression290.Tree);
            	        	f2.Type = IDENT;

            	        }
            	        break;

            	}


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);

               IASTNode root = (IASTNode) adaptor.Create(EXPR_LIST, "exprList");
               root.AddChild((IASTNode)retval.Tree);
               retval.Tree = root;

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "exprList"

    public class subQuery_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "subQuery"
    // Hql.g:632:1: subQuery : innerSubQuery ( UNION innerSubQuery )* ;
    public HqlParser.subQuery_return subQuery() // throws RecognitionException [1]
    {   
        HqlParser.subQuery_return retval = new HqlParser.subQuery_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken UNION292 = null;
        HqlParser.innerSubQuery_return innerSubQuery291 = default(HqlParser.innerSubQuery_return);

        HqlParser.innerSubQuery_return innerSubQuery293 = default(HqlParser.innerSubQuery_return);


        IASTNode UNION292_tree=null;

        try 
    	{
            // Hql.g:633:2: ( innerSubQuery ( UNION innerSubQuery )* )
            // Hql.g:633:4: innerSubQuery ( UNION innerSubQuery )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_innerSubQuery_in_subQuery3476);
            	innerSubQuery291 = innerSubQuery();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, innerSubQuery291.Tree);
            	// Hql.g:633:18: ( UNION innerSubQuery )*
            	do 
            	{
            	    int alt103 = 2;
            	    int LA103_0 = input.LA(1);

            	    if ( (LA103_0 == UNION) )
            	    {
            	        alt103 = 1;
            	    }


            	    switch (alt103) 
            		{
            			case 1 :
            			    // Hql.g:633:19: UNION innerSubQuery
            			    {
            			    	UNION292=(IToken)Match(input,UNION,FOLLOW_UNION_in_subQuery3479); 
            			    		UNION292_tree = (IASTNode)adaptor.Create(UNION292);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(UNION292_tree, root_0);

            			    	PushFollow(FOLLOW_innerSubQuery_in_subQuery3482);
            			    	innerSubQuery293 = innerSubQuery();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, innerSubQuery293.Tree);

            			    }
            			    break;

            			default:
            			    goto loop103;
            	    }
            	} while (true);

            	loop103:
            		;	// Stops C# compiler whining that label 'loop103' has no statements


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "subQuery"

    public class innerSubQuery_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "innerSubQuery"
    // Hql.g:636:1: innerSubQuery : queryRule -> ^( QUERY[\"query\"] queryRule ) ;
    public HqlParser.innerSubQuery_return innerSubQuery() // throws RecognitionException [1]
    {   
        HqlParser.innerSubQuery_return retval = new HqlParser.innerSubQuery_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.queryRule_return queryRule294 = default(HqlParser.queryRule_return);


        RewriteRuleSubtreeStream stream_queryRule = new RewriteRuleSubtreeStream(adaptor,"rule queryRule");
        try 
    	{
            // Hql.g:637:2: ( queryRule -> ^( QUERY[\"query\"] queryRule ) )
            // Hql.g:637:4: queryRule
            {
            	PushFollow(FOLLOW_queryRule_in_innerSubQuery3496);
            	queryRule294 = queryRule();
            	state.followingStackPointer--;

            	stream_queryRule.Add(queryRule294.Tree);


            	// AST REWRITE
            	// elements:          queryRule
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (IASTNode)adaptor.GetNilNode();
            	// 638:2: -> ^( QUERY[\"query\"] queryRule )
            	{
            	    // Hql.g:638:5: ^( QUERY[\"query\"] queryRule )
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(QUERY, "query"), root_1);

            	    adaptor.AddChild(root_1, stream_queryRule.NextTree());

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;
            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "innerSubQuery"

    public class constant_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "constant"
    // Hql.g:641:1: constant : ( NUM_INT | NUM_FLOAT | NUM_LONG | NUM_DOUBLE | NUM_DECIMAL | QUOTED_String | NULL | TRUE | FALSE | EMPTY );
    public HqlParser.constant_return constant() // throws RecognitionException [1]
    {   
        HqlParser.constant_return retval = new HqlParser.constant_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken set295 = null;

        IASTNode set295_tree=null;

        try 
    	{
            // Hql.g:642:2: ( NUM_INT | NUM_FLOAT | NUM_LONG | NUM_DOUBLE | NUM_DECIMAL | QUOTED_String | NULL | TRUE | FALSE | EMPTY )
            // Hql.g:
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	set295 = (IToken)input.LT(1);
            	if ( input.LA(1) == FALSE || input.LA(1) == NULL || input.LA(1) == TRUE || input.LA(1) == EMPTY || (input.LA(1) >= NUM_INT && input.LA(1) <= NUM_LONG) || input.LA(1) == QUOTED_String ) 
            	{
            	    input.Consume();
            	    adaptor.AddChild(root_0, (IASTNode)adaptor.Create(set295));
            	    state.errorRecovery = false;
            	}
            	else 
            	{
            	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	    throw mse;
            	}


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "constant"

    public class path_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "path"
    // Hql.g:660:1: path : identifier ( DOT identifier )* ;
    public HqlParser.path_return path() // throws RecognitionException [1]
    {   
        HqlParser.path_return retval = new HqlParser.path_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken DOT297 = null;
        HqlParser.identifier_return identifier296 = default(HqlParser.identifier_return);

        HqlParser.identifier_return identifier298 = default(HqlParser.identifier_return);


        IASTNode DOT297_tree=null;


        // TODO - need to clean up DotIdent - suspect that DotIdent2 supersedes the other one, but need to do the analysis
        //HandleDotIdent2();

        try 
    	{
            // Hql.g:665:2: ( identifier ( DOT identifier )* )
            // Hql.g:665:4: identifier ( DOT identifier )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_identifier_in_path3584);
            	identifier296 = identifier();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, identifier296.Tree);
            	// Hql.g:665:15: ( DOT identifier )*
            	do 
            	{
            	    int alt104 = 2;
            	    int LA104_0 = input.LA(1);

            	    if ( (LA104_0 == DOT) )
            	    {
            	        alt104 = 1;
            	    }


            	    switch (alt104) 
            		{
            			case 1 :
            			    // Hql.g:665:17: DOT identifier
            			    {
            			    	DOT297=(IToken)Match(input,DOT,FOLLOW_DOT_in_path3588); 
            			    		DOT297_tree = (IASTNode)adaptor.Create(DOT297);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(DOT297_tree, root_0);

            			    	 WeakKeywords(); 
            			    	PushFollow(FOLLOW_identifier_in_path3593);
            			    	identifier298 = identifier();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, identifier298.Tree);

            			    }
            			    break;

            			default:
            			    goto loop104;
            	    }
            	} while (true);

            	loop104:
            		;	// Stops C# compiler whining that label 'loop104' has no statements


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (IASTNode)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "path"

    public class identifier_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "identifier"
    // Hql.g:670:1: identifier : IDENT ;
    public HqlParser.identifier_return identifier() // throws RecognitionException [1]
    {   
        HqlParser.identifier_return retval = new HqlParser.identifier_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken IDENT299 = null;

        IASTNode IDENT299_tree=null;

        try 
    	{
            // Hql.g:671:2: ( IDENT )
            // Hql.g:671:4: IDENT
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	IDENT299=(IToken)Match(input,IDENT,FOLLOW_IDENT_in_identifier3609); 
            		IDENT299_tree = (IASTNode)adaptor.Create(IDENT299);
            		adaptor.AddChild(root_0, IDENT299_tree);


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (IASTNode)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException ex) 
        {

            		retval.Tree = HandleIdentifierError(input.LT(1),ex);
            	
        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "identifier"

    // Delegated rules


	private void InitializeCyclicDFAs()
	{
	}

 

    public static readonly BitSet FOLLOW_updateStatement_in_statement611 = new BitSet(new ulong[]{0x0000000000000000UL});
    public static readonly BitSet FOLLOW_deleteStatement_in_statement615 = new BitSet(new ulong[]{0x0000000000000000UL});
    public static readonly BitSet FOLLOW_selectStatement_in_statement619 = new BitSet(new ulong[]{0x0000000000000000UL});
    public static readonly BitSet FOLLOW_insertStatement_in_statement623 = new BitSet(new ulong[]{0x0000000000000000UL});
    public static readonly BitSet FOLLOW_EOF_in_statement627 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_UPDATE_in_updateStatement639 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_VERSIONED_in_updateStatement643 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_optionalFromTokenFromClause_in_updateStatement649 = new BitSet(new ulong[]{0x0000400000000000UL});
    public static readonly BitSet FOLLOW_setClause_in_updateStatement653 = new BitSet(new ulong[]{0x0080000000000002UL});
    public static readonly BitSet FOLLOW_whereClause_in_updateStatement658 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_SET_in_setClause672 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_assignment_in_setClause675 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_COMMA_in_setClause678 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_assignment_in_setClause681 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_stateField_in_assignment695 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000004000000000UL});
    public static readonly BitSet FOLLOW_EQ_in_assignment697 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_newValue_in_assignment700 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_path_in_stateField713 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_concatenation_in_newValue726 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_DELETE_in_deleteStatement737 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_optionalFromTokenFromClause_in_deleteStatement743 = new BitSet(new ulong[]{0x0080000000000002UL});
    public static readonly BitSet FOLLOW_whereClause_in_deleteStatement749 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_optionalFromTokenFromClause2_in_optionalFromTokenFromClause764 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_path_in_optionalFromTokenFromClause766 = new BitSet(new ulong[]{0x0040000000400082UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_asAlias_in_optionalFromTokenFromClause769 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_FROM_in_optionalFromTokenFromClause2800 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_queryRule_in_selectStatement814 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_INSERT_in_insertStatement843 = new BitSet(new ulong[]{0x0000000040000000UL});
    public static readonly BitSet FOLLOW_intoClause_in_insertStatement846 = new BitSet(new ulong[]{0x0084A20003400000UL});
    public static readonly BitSet FOLLOW_selectStatement_in_insertStatement848 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_INTO_in_intoClause859 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_path_in_intoClause862 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000008000000000UL});
    public static readonly BitSet FOLLOW_insertablePropertySpec_in_intoClause866 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_OPEN_in_insertablePropertySpec877 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_primaryExpression_in_insertablePropertySpec879 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000012000000000UL});
    public static readonly BitSet FOLLOW_COMMA_in_insertablePropertySpec883 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_primaryExpression_in_insertablePropertySpec885 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000012000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_insertablePropertySpec890 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_selectFrom_in_queryRule916 = new BitSet(new ulong[]{0x0084820003000002UL});
    public static readonly BitSet FOLLOW_whereClause_in_queryRule921 = new BitSet(new ulong[]{0x0004820003000002UL});
    public static readonly BitSet FOLLOW_groupByClause_in_queryRule928 = new BitSet(new ulong[]{0x0004820002000002UL});
    public static readonly BitSet FOLLOW_havingClause_in_queryRule935 = new BitSet(new ulong[]{0x0004820000000002UL});
    public static readonly BitSet FOLLOW_orderByClause_in_queryRule942 = new BitSet(new ulong[]{0x0004800000000002UL});
    public static readonly BitSet FOLLOW_skipClause_in_queryRule949 = new BitSet(new ulong[]{0x0004000000000002UL});
    public static readonly BitSet FOLLOW_takeClause_in_queryRule956 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_selectClause_in_selectFrom974 = new BitSet(new ulong[]{0x0000000000400002UL});
    public static readonly BitSet FOLLOW_fromClause_in_selectFrom981 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_SELECT_in_selectClause1030 = new BitSet(new ulong[]{0x024B00F8085B1230UL,0x3C31008F80000012UL});
    public static readonly BitSet FOLLOW_DISTINCT_in_selectClause1042 = new BitSet(new ulong[]{0x024B00F8085B1230UL,0x3C31008F80000012UL});
    public static readonly BitSet FOLLOW_selectedPropertiesList_in_selectClause1048 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_newExpression_in_selectClause1052 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_selectObject_in_selectClause1056 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_NEW_in_newExpression1070 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_path_in_newExpression1072 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000008000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_newExpression1077 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_selectedPropertiesList_in_newExpression1079 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000010000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_newExpression1081 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_OBJECT_in_selectObject1107 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000008000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_selectObject1110 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_identifier_in_selectObject1113 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000010000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_selectObject1115 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_FROM_in_fromClause1133 = new BitSet(new ulong[]{0x0040000004420080UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_fromRange_in_fromClause1138 = new BitSet(new ulong[]{0x0000100310800002UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_fromJoin_in_fromClause1142 = new BitSet(new ulong[]{0x0000100310800002UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_COMMA_in_fromClause1146 = new BitSet(new ulong[]{0x0040000004420080UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_fromRange_in_fromClause1151 = new BitSet(new ulong[]{0x0000100310800002UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_set_in_fromJoin1169 = new BitSet(new ulong[]{0x0000040100000000UL});
    public static readonly BitSet FOLLOW_OUTER_in_fromJoin1180 = new BitSet(new ulong[]{0x0000000100000000UL});
    public static readonly BitSet FOLLOW_FULL_in_fromJoin1188 = new BitSet(new ulong[]{0x0000000100000000UL});
    public static readonly BitSet FOLLOW_INNER_in_fromJoin1192 = new BitSet(new ulong[]{0x0000000100000000UL});
    public static readonly BitSet FOLLOW_JOIN_in_fromJoin1197 = new BitSet(new ulong[]{0x0040000000600000UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_FETCH_in_fromJoin1201 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_path_in_fromJoin1205 = new BitSet(new ulong[]{0x8040000000600082UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_asAlias_in_fromJoin1208 = new BitSet(new ulong[]{0x8000000000200002UL});
    public static readonly BitSet FOLLOW_propertyFetch_in_fromJoin1213 = new BitSet(new ulong[]{0x8000000000000002UL});
    public static readonly BitSet FOLLOW_withClause_in_fromJoin1218 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_set_in_fromJoin1229 = new BitSet(new ulong[]{0x0000040100000000UL});
    public static readonly BitSet FOLLOW_OUTER_in_fromJoin1240 = new BitSet(new ulong[]{0x0000000100000000UL});
    public static readonly BitSet FOLLOW_FULL_in_fromJoin1248 = new BitSet(new ulong[]{0x0000000100000000UL});
    public static readonly BitSet FOLLOW_INNER_in_fromJoin1252 = new BitSet(new ulong[]{0x0000000100000000UL});
    public static readonly BitSet FOLLOW_JOIN_in_fromJoin1257 = new BitSet(new ulong[]{0x0000000000220000UL});
    public static readonly BitSet FOLLOW_FETCH_in_fromJoin1261 = new BitSet(new ulong[]{0x0000000000020000UL});
    public static readonly BitSet FOLLOW_ELEMENTS_in_fromJoin1265 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000008000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_fromJoin1268 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_path_in_fromJoin1271 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000010000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_fromJoin1273 = new BitSet(new ulong[]{0x8040000000600082UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_asAlias_in_fromJoin1277 = new BitSet(new ulong[]{0x8000000000200002UL});
    public static readonly BitSet FOLLOW_propertyFetch_in_fromJoin1282 = new BitSet(new ulong[]{0x8000000000000002UL});
    public static readonly BitSet FOLLOW_withClause_in_fromJoin1287 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WITH_in_withClause1300 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_logicalExpression_in_withClause1303 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_fromClassOrOuterQueryPath_in_fromRange1314 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_inClassDeclaration_in_fromRange1319 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_inCollectionDeclaration_in_fromRange1324 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_inCollectionElementsDeclaration_in_fromRange1329 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_path_in_fromClassOrOuterQueryPath1341 = new BitSet(new ulong[]{0x0040000000600082UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_asAlias_in_fromClassOrOuterQueryPath1346 = new BitSet(new ulong[]{0x0000000000200002UL});
    public static readonly BitSet FOLLOW_propertyFetch_in_fromClassOrOuterQueryPath1351 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_alias_in_inClassDeclaration1381 = new BitSet(new ulong[]{0x0000000004000000UL});
    public static readonly BitSet FOLLOW_IN_in_inClassDeclaration1383 = new BitSet(new ulong[]{0x0040000000400800UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_CLASS_in_inClassDeclaration1385 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_path_in_inClassDeclaration1388 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_IN_in_inCollectionDeclaration1416 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000008000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_inCollectionDeclaration1418 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_path_in_inCollectionDeclaration1420 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000010000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_inCollectionDeclaration1422 = new BitSet(new ulong[]{0x0040000000400080UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_alias_in_inCollectionDeclaration1424 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_alias_in_inCollectionElementsDeclaration1458 = new BitSet(new ulong[]{0x0000000004000000UL});
    public static readonly BitSet FOLLOW_IN_in_inCollectionElementsDeclaration1460 = new BitSet(new ulong[]{0x0000000000020000UL});
    public static readonly BitSet FOLLOW_ELEMENTS_in_inCollectionElementsDeclaration1462 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000008000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_inCollectionElementsDeclaration1464 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_path_in_inCollectionElementsDeclaration1466 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000010000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_inCollectionElementsDeclaration1468 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ELEMENTS_in_inCollectionElementsDeclaration1490 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000008000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_inCollectionElementsDeclaration1492 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_path_in_inCollectionElementsDeclaration1494 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000010000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_inCollectionElementsDeclaration1496 = new BitSet(new ulong[]{0x0000000000000080UL});
    public static readonly BitSet FOLLOW_AS_in_inCollectionElementsDeclaration1498 = new BitSet(new ulong[]{0x0040000000400080UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_alias_in_inCollectionElementsDeclaration1500 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_AS_in_asAlias1532 = new BitSet(new ulong[]{0x0040000000400080UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_alias_in_asAlias1537 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_identifier_in_alias1549 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_FETCH_in_propertyFetch1568 = new BitSet(new ulong[]{0x0000000000000010UL});
    public static readonly BitSet FOLLOW_ALL_in_propertyFetch1570 = new BitSet(new ulong[]{0x0000080000000000UL});
    public static readonly BitSet FOLLOW_PROPERTIES_in_propertyFetch1573 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_GROUP_in_groupByClause1585 = new BitSet(new ulong[]{0x0100000000000000UL});
    public static readonly BitSet FOLLOW_LITERAL_by_in_groupByClause1591 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_expression_in_groupByClause1594 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_COMMA_in_groupByClause1598 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_expression_in_groupByClause1601 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_ORDER_in_orderByClause1615 = new BitSet(new ulong[]{0x0100000000000000UL});
    public static readonly BitSet FOLLOW_LITERAL_by_in_orderByClause1618 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_orderElement_in_orderByClause1621 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_COMMA_in_orderByClause1625 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_orderElement_in_orderByClause1628 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_SKIP_in_skipClause1642 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000080000000UL});
    public static readonly BitSet FOLLOW_NUM_INT_in_skipClause1645 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_TAKE_in_takeClause1656 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000080000000UL});
    public static readonly BitSet FOLLOW_NUM_INT_in_takeClause1659 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_expression_in_orderElement1670 = new BitSet(new ulong[]{0x0000000000004102UL,0x0000000000000000UL,0x0000000000000060UL});
    public static readonly BitSet FOLLOW_ascendingOrDescending_in_orderElement1674 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ASCENDING_in_ascendingOrDescending1692 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_133_in_ascendingOrDescending1698 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_DESCENDING_in_ascendingOrDescending1718 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_134_in_ascendingOrDescending1724 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_HAVING_in_havingClause1745 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_logicalExpression_in_havingClause1748 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WHERE_in_whereClause1759 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_logicalExpression_in_whereClause1762 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_aliasedExpression_in_selectedPropertiesList1773 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_COMMA_in_selectedPropertiesList1777 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_aliasedExpression_in_selectedPropertiesList1780 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_expression_in_aliasedExpression1795 = new BitSet(new ulong[]{0x0000000000000082UL});
    public static readonly BitSet FOLLOW_AS_in_aliasedExpression1799 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_identifier_in_aliasedExpression1802 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_expression_in_logicalExpression1841 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_logicalOrExpression_in_expression1853 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_logicalAndExpression_in_logicalOrExpression1865 = new BitSet(new ulong[]{0x0000010000000002UL});
    public static readonly BitSet FOLLOW_OR_in_logicalOrExpression1869 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_logicalAndExpression_in_logicalOrExpression1872 = new BitSet(new ulong[]{0x0000010000000002UL});
    public static readonly BitSet FOLLOW_negatedExpression_in_logicalAndExpression1887 = new BitSet(new ulong[]{0x0000000000000042UL});
    public static readonly BitSet FOLLOW_AND_in_logicalAndExpression1891 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_negatedExpression_in_logicalAndExpression1894 = new BitSet(new ulong[]{0x0000000000000042UL});
    public static readonly BitSet FOLLOW_NOT_in_negatedExpression1915 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_negatedExpression_in_negatedExpression1919 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_equalityExpression_in_negatedExpression1932 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_relationalExpression_in_equalityExpression1962 = new BitSet(new ulong[]{0x0000000080000002UL,0x0000064000000000UL});
    public static readonly BitSet FOLLOW_EQ_in_equalityExpression1970 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_IS_in_equalityExpression1979 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_NOT_in_equalityExpression1985 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_NE_in_equalityExpression1997 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_SQL_NE_in_equalityExpression2006 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_relationalExpression_in_equalityExpression2017 = new BitSet(new ulong[]{0x0000000080000002UL,0x0000064000000000UL});
    public static readonly BitSet FOLLOW_concatenation_in_relationalExpression2034 = new BitSet(new ulong[]{0x0000004404000402UL,0x0000780000000008UL});
    public static readonly BitSet FOLLOW_LT_in_relationalExpression2046 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_GT_in_relationalExpression2051 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_LE_in_relationalExpression2056 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_GE_in_relationalExpression2061 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_bitwiseNotExpression_in_relationalExpression2066 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000780000000000UL});
    public static readonly BitSet FOLLOW_NOT_in_relationalExpression2083 = new BitSet(new ulong[]{0x0000000404000400UL,0x0000000000000008UL});
    public static readonly BitSet FOLLOW_IN_in_relationalExpression2104 = new BitSet(new ulong[]{0x0042001808421200UL,0x2000008000000000UL});
    public static readonly BitSet FOLLOW_inList_in_relationalExpression2113 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_BETWEEN_in_relationalExpression2124 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_betweenList_in_relationalExpression2133 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_LIKE_in_relationalExpression2145 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_concatenation_in_relationalExpression2154 = new BitSet(new ulong[]{0x0000000000040000UL});
    public static readonly BitSet FOLLOW_likeEscape_in_relationalExpression2156 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_MEMBER_in_relationalExpression2165 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000020UL});
    public static readonly BitSet FOLLOW_OF_in_relationalExpression2169 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_path_in_relationalExpression2176 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ESCAPE_in_likeEscape2203 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_concatenation_in_likeEscape2206 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_compoundExpr_in_inList2219 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_concatenation_in_betweenList2240 = new BitSet(new ulong[]{0x0000000000000040UL});
    public static readonly BitSet FOLLOW_AND_in_betweenList2242 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_concatenation_in_betweenList2245 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_bitwiseNotExpression_in_concatenation2264 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000800000000000UL});
    public static readonly BitSet FOLLOW_CONCAT_in_concatenation2272 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_bitwiseNotExpression_in_concatenation2281 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000800000000000UL});
    public static readonly BitSet FOLLOW_CONCAT_in_concatenation2288 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_bitwiseNotExpression_in_concatenation2291 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000800000000000UL});
    public static readonly BitSet FOLLOW_BNOT_in_bitwiseNotExpression2315 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_bitwiseOrExpression_in_bitwiseNotExpression2318 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_bitwiseOrExpression_in_bitwiseNotExpression2324 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_bitwiseXOrExpression_in_bitwiseOrExpression2336 = new BitSet(new ulong[]{0x0000000000000002UL,0x0002000000000000UL});
    public static readonly BitSet FOLLOW_BOR_in_bitwiseOrExpression2339 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_bitwiseXOrExpression_in_bitwiseOrExpression2342 = new BitSet(new ulong[]{0x0000000000000002UL,0x0002000000000000UL});
    public static readonly BitSet FOLLOW_bitwiseAndExpression_in_bitwiseXOrExpression2356 = new BitSet(new ulong[]{0x0000000000000002UL,0x0004000000000000UL});
    public static readonly BitSet FOLLOW_BXOR_in_bitwiseXOrExpression2359 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_bitwiseAndExpression_in_bitwiseXOrExpression2362 = new BitSet(new ulong[]{0x0000000000000002UL,0x0004000000000000UL});
    public static readonly BitSet FOLLOW_additiveExpression_in_bitwiseAndExpression2376 = new BitSet(new ulong[]{0x0000000000000002UL,0x0008000000000000UL});
    public static readonly BitSet FOLLOW_BAND_in_bitwiseAndExpression2379 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_additiveExpression_in_bitwiseAndExpression2382 = new BitSet(new ulong[]{0x0000000000000002UL,0x0008000000000000UL});
    public static readonly BitSet FOLLOW_multiplyExpression_in_additiveExpression2396 = new BitSet(new ulong[]{0x0000000000000002UL,0x0030000000000000UL});
    public static readonly BitSet FOLLOW_PLUS_in_additiveExpression2402 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_MINUS_in_additiveExpression2407 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_multiplyExpression_in_additiveExpression2412 = new BitSet(new ulong[]{0x0000000000000002UL,0x0030000000000000UL});
    public static readonly BitSet FOLLOW_unaryExpression_in_multiplyExpression2427 = new BitSet(new ulong[]{0x0000000000000002UL,0x00C0000000000000UL});
    public static readonly BitSet FOLLOW_STAR_in_multiplyExpression2433 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_DIV_in_multiplyExpression2438 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_unaryExpression_in_multiplyExpression2443 = new BitSet(new ulong[]{0x0000000000000002UL,0x00C0000000000000UL});
    public static readonly BitSet FOLLOW_MINUS_in_unaryExpression2461 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_unaryExpression_in_unaryExpression2465 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_PLUS_in_unaryExpression2482 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_unaryExpression_in_unaryExpression2486 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_caseExpression_in_unaryExpression2503 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_quantifiedExpression_in_unaryExpression2517 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_atom_in_unaryExpression2532 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_CASE_in_caseExpression2551 = new BitSet(new ulong[]{0x2000000000000000UL});
    public static readonly BitSet FOLLOW_whenClause_in_caseExpression2554 = new BitSet(new ulong[]{0x2C00000000000000UL});
    public static readonly BitSet FOLLOW_elseClause_in_caseExpression2559 = new BitSet(new ulong[]{0x0400000000000000UL});
    public static readonly BitSet FOLLOW_END_in_caseExpression2563 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_CASE_in_caseExpression2583 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_unaryExpression_in_caseExpression2585 = new BitSet(new ulong[]{0x2000000000000000UL});
    public static readonly BitSet FOLLOW_altWhenClause_in_caseExpression2588 = new BitSet(new ulong[]{0x2C00000000000000UL});
    public static readonly BitSet FOLLOW_elseClause_in_caseExpression2593 = new BitSet(new ulong[]{0x0400000000000000UL});
    public static readonly BitSet FOLLOW_END_in_caseExpression2597 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WHEN_in_whenClause2626 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_logicalExpression_in_whenClause2629 = new BitSet(new ulong[]{0x1000000000000000UL});
    public static readonly BitSet FOLLOW_THEN_in_whenClause2631 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_expression_in_whenClause2634 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WHEN_in_altWhenClause2648 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_unaryExpression_in_altWhenClause2651 = new BitSet(new ulong[]{0x1000000000000000UL});
    public static readonly BitSet FOLLOW_THEN_in_altWhenClause2653 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_expression_in_altWhenClause2656 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ELSE_in_elseClause2670 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_expression_in_elseClause2673 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_SOME_in_quantifiedExpression2688 = new BitSet(new ulong[]{0x0042001808421200UL,0x2000008000000000UL});
    public static readonly BitSet FOLLOW_EXISTS_in_quantifiedExpression2693 = new BitSet(new ulong[]{0x0042001808421200UL,0x2000008000000000UL});
    public static readonly BitSet FOLLOW_ALL_in_quantifiedExpression2698 = new BitSet(new ulong[]{0x0042001808421200UL,0x2000008000000000UL});
    public static readonly BitSet FOLLOW_ANY_in_quantifiedExpression2703 = new BitSet(new ulong[]{0x0042001808421200UL,0x2000008000000000UL});
    public static readonly BitSet FOLLOW_identifier_in_quantifiedExpression2712 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_collectionExpr_in_quantifiedExpression2716 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_OPEN_in_quantifiedExpression2721 = new BitSet(new ulong[]{0x0084A20003400000UL});
    public static readonly BitSet FOLLOW_subQuery_in_quantifiedExpression2726 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000010000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_quantifiedExpression2730 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_primaryExpression_in_atom2749 = new BitSet(new ulong[]{0x0000000000008002UL,0x0100000000000000UL});
    public static readonly BitSet FOLLOW_DOT_in_atom2758 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_identifier_in_atom2761 = new BitSet(new ulong[]{0x0000000000008002UL,0x0100008000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_atom2789 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x3C31018F80000047UL});
    public static readonly BitSet FOLLOW_exprList_in_atom2794 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000010000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_atom2796 = new BitSet(new ulong[]{0x0000000000008002UL,0x0100000000000000UL});
    public static readonly BitSet FOLLOW_OPEN_BRACKET_in_atom2810 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_expression_in_atom2815 = new BitSet(new ulong[]{0x0000000000000000UL,0x0200000000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_BRACKET_in_atom2817 = new BitSet(new ulong[]{0x0000000000008002UL,0x0100000000000000UL});
    public static readonly BitSet FOLLOW_identPrimary_in_primaryExpression2837 = new BitSet(new ulong[]{0x0000000000008002UL});
    public static readonly BitSet FOLLOW_DOT_in_primaryExpression2850 = new BitSet(new ulong[]{0x0000000000000800UL});
    public static readonly BitSet FOLLOW_CLASS_in_primaryExpression2853 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_constant_in_primaryExpression2863 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_COLON_in_primaryExpression2870 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_identifier_in_primaryExpression2873 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_OPEN_in_primaryExpression2882 = new BitSet(new ulong[]{0x02CFA2D80B5A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_expressionOrVector_in_primaryExpression2886 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000010000000000UL});
    public static readonly BitSet FOLLOW_subQuery_in_primaryExpression2890 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000010000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_primaryExpression2893 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_PARAM_in_primaryExpression2901 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000080000000UL});
    public static readonly BitSet FOLLOW_NUM_INT_in_primaryExpression2905 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_expression_in_expressionOrVector2923 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_vectorExpr_in_expressionOrVector2929 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_COMMA_in_vectorExpr2968 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_expression_in_vectorExpr2971 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_COMMA_in_vectorExpr2974 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_expression_in_vectorExpr2977 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_identifier_in_identPrimary2993 = new BitSet(new ulong[]{0x0000000000008002UL,0x0000008000000000UL});
    public static readonly BitSet FOLLOW_DOT_in_identPrimary3011 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000010UL});
    public static readonly BitSet FOLLOW_identifier_in_identPrimary3016 = new BitSet(new ulong[]{0x0000000000008002UL,0x0000008000000000UL});
    public static readonly BitSet FOLLOW_OBJECT_in_identPrimary3022 = new BitSet(new ulong[]{0x0000000000008002UL,0x0000008000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_identPrimary3040 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x3C31018F80000047UL});
    public static readonly BitSet FOLLOW_exprList_in_identPrimary3045 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000010000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_identPrimary3047 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_aggregate_in_identPrimary3063 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_SUM_in_aggregate3084 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000008000000000UL});
    public static readonly BitSet FOLLOW_AVG_in_aggregate3090 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000008000000000UL});
    public static readonly BitSet FOLLOW_MAX_in_aggregate3096 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000008000000000UL});
    public static readonly BitSet FOLLOW_MIN_in_aggregate3102 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000008000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_aggregate3106 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_additiveExpression_in_aggregate3108 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000010000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_aggregate3110 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_COUNT_in_aggregate3129 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000008000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_aggregate3131 = new BitSet(new ulong[]{0x0042001808431210UL,0x2040000000000000UL});
    public static readonly BitSet FOLLOW_STAR_in_aggregate3137 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000010000000000UL});
    public static readonly BitSet FOLLOW_aggregateDistinctAll_in_aggregate3143 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000010000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_aggregate3147 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_collectionExpr_in_aggregate3179 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_set_in_aggregateDistinctAll3192 = new BitSet(new ulong[]{0x0042001808421200UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_path_in_aggregateDistinctAll3205 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_collectionExpr_in_aggregateDistinctAll3209 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ELEMENTS_in_collectionExpr3228 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000008000000000UL});
    public static readonly BitSet FOLLOW_INDICES_in_collectionExpr3233 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000008000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_collectionExpr3237 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_path_in_collectionExpr3240 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000010000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_collectionExpr3242 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_collectionExpr_in_compoundExpr3297 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_path_in_compoundExpr3302 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_OPEN_in_compoundExpr3308 = new BitSet(new ulong[]{0x02CFA2D80B5A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_subQuery_in_compoundExpr3313 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000010000000000UL});
    public static readonly BitSet FOLLOW_expression_in_compoundExpr3318 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000012000000000UL});
    public static readonly BitSet FOLLOW_COMMA_in_compoundExpr3321 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_expression_in_compoundExpr3324 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000012000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_compoundExpr3331 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_TRAILING_in_exprList3350 = new BitSet(new ulong[]{0x024B00D8085A1232UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_LEADING_in_exprList3363 = new BitSet(new ulong[]{0x024B00D8085A1232UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_BOTH_in_exprList3376 = new BitSet(new ulong[]{0x024B00D8085A1232UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_expression_in_exprList3400 = new BitSet(new ulong[]{0x0000000000400082UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_COMMA_in_exprList3405 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_expression_in_exprList3408 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_FROM_in_exprList3423 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_expression_in_exprList3425 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_AS_in_exprList3437 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_identifier_in_exprList3440 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_FROM_in_exprList3454 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x3C31008F80000002UL});
    public static readonly BitSet FOLLOW_expression_in_exprList3456 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_innerSubQuery_in_subQuery3476 = new BitSet(new ulong[]{0x0010000000000002UL});
    public static readonly BitSet FOLLOW_UNION_in_subQuery3479 = new BitSet(new ulong[]{0x0084A20003400000UL});
    public static readonly BitSet FOLLOW_innerSubQuery_in_subQuery3482 = new BitSet(new ulong[]{0x0010000000000002UL});
    public static readonly BitSet FOLLOW_queryRule_in_innerSubQuery3496 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_set_in_constant0 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_identifier_in_path3584 = new BitSet(new ulong[]{0x0000000000008002UL});
    public static readonly BitSet FOLLOW_DOT_in_path3588 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_identifier_in_path3593 = new BitSet(new ulong[]{0x0000000000008002UL});
    public static readonly BitSet FOLLOW_IDENT_in_identifier3609 = new BitSet(new ulong[]{0x0000000000000002UL});

}
}