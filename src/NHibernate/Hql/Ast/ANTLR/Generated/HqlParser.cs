// $ANTLR 3.2 Sep 23, 2009 12:02:23 Hql.g 2011-05-22 07:45:49

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
		"'descending'"
    };

    public const int EXPONENT = 130;
    public const int LT = 109;
    public const int FLOAT_SUFFIX = 131;
    public const int STAR = 120;
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
    public const int INSERT = 29;
    public const int ESCAPE = 18;
    public const int IS_NULL = 80;
    public const int BOTH = 64;
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
    public const int LEFT = 33;
    public const int AVG = 9;
    public const int SOME = 48;
    public const int BOR = 115;
    public const int ALL = 4;
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
    public const int ROW_STAR = 88;
    public const int NOT_LIKE = 84;
    public const int HEX_DIGIT = 132;
    public const int NOT_BETWEEN = 82;
    public const int RANGE = 87;
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
    public const int DIV = 121;
    public const int DESCENDING = 14;
    public const int BETWEEN = 10;
    public const int AGGREGATE = 71;
    public const int LE = 111;

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
            	case COLON:
            	case PARAM:
            	case BNOT:
            	case PLUS:
            	case MINUS:
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
            	// elements:          selectedPropertiesList, path
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

                if ( (LA33_1 == IN) )
                {
                    int LA33_4 = input.LA(3);

                    if ( (LA33_4 == ELEMENTS) )
                    {
                        alt33 = 4;
                    }
                    else if ( (LA33_4 == CLASS || LA33_4 == IDENT) )
                    {
                        alt33 = 2;
                    }
                    else 
                    {
                        NoViableAltException nvae_d33s4 =
                            new NoViableAltException("", 33, 4, input);

                        throw nvae_d33s4;
                    }
                }
                else if ( (LA33_1 == EOF || LA33_1 == AS || LA33_1 == DOT || LA33_1 == FETCH || (LA33_1 >= FULL && LA33_1 <= HAVING) || LA33_1 == INNER || (LA33_1 >= JOIN && LA33_1 <= LEFT) || LA33_1 == ORDER || LA33_1 == RIGHT || LA33_1 == SKIP || LA33_1 == TAKE || LA33_1 == UNION || LA33_1 == WHERE || LA33_1 == COMMA || LA33_1 == CLOSE || LA33_1 == IDENT) )
                {
                    alt33 = 1;
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
            	// elements:          propertyFetch, asAlias, path
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
                    	// elements:          alias, path
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
    // Hql.g:305:1: skipClause : SKIP ( NUM_INT | parameter ) ;
    public HqlParser.skipClause_return skipClause() // throws RecognitionException [1]
    {   
        HqlParser.skipClause_return retval = new HqlParser.skipClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken SKIP131 = null;
        IToken NUM_INT132 = null;
        HqlParser.parameter_return parameter133 = default(HqlParser.parameter_return);


        IASTNode SKIP131_tree=null;
        IASTNode NUM_INT132_tree=null;

        try 
    	{
            // Hql.g:306:2: ( SKIP ( NUM_INT | parameter ) )
            // Hql.g:306:4: SKIP ( NUM_INT | parameter )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	SKIP131=(IToken)Match(input,SKIP,FOLLOW_SKIP_in_skipClause1642); 
            		SKIP131_tree = (IASTNode)adaptor.Create(SKIP131);
            		root_0 = (IASTNode)adaptor.BecomeRoot(SKIP131_tree, root_0);

            	// Hql.g:306:10: ( NUM_INT | parameter )
            	int alt41 = 2;
            	int LA41_0 = input.LA(1);

            	if ( (LA41_0 == NUM_INT) )
            	{
            	    alt41 = 1;
            	}
            	else if ( ((LA41_0 >= COLON && LA41_0 <= PARAM)) )
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
            	        // Hql.g:306:11: NUM_INT
            	        {
            	        	NUM_INT132=(IToken)Match(input,NUM_INT,FOLLOW_NUM_INT_in_skipClause1646); 
            	        		NUM_INT132_tree = (IASTNode)adaptor.Create(NUM_INT132);
            	        		adaptor.AddChild(root_0, NUM_INT132_tree);


            	        }
            	        break;
            	    case 2 :
            	        // Hql.g:306:21: parameter
            	        {
            	        	PushFollow(FOLLOW_parameter_in_skipClause1650);
            	        	parameter133 = parameter();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, parameter133.Tree);

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
    // Hql.g:309:1: takeClause : TAKE ( NUM_INT | parameter ) ;
    public HqlParser.takeClause_return takeClause() // throws RecognitionException [1]
    {   
        HqlParser.takeClause_return retval = new HqlParser.takeClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken TAKE134 = null;
        IToken NUM_INT135 = null;
        HqlParser.parameter_return parameter136 = default(HqlParser.parameter_return);


        IASTNode TAKE134_tree=null;
        IASTNode NUM_INT135_tree=null;

        try 
    	{
            // Hql.g:310:2: ( TAKE ( NUM_INT | parameter ) )
            // Hql.g:310:4: TAKE ( NUM_INT | parameter )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	TAKE134=(IToken)Match(input,TAKE,FOLLOW_TAKE_in_takeClause1662); 
            		TAKE134_tree = (IASTNode)adaptor.Create(TAKE134);
            		root_0 = (IASTNode)adaptor.BecomeRoot(TAKE134_tree, root_0);

            	// Hql.g:310:10: ( NUM_INT | parameter )
            	int alt42 = 2;
            	int LA42_0 = input.LA(1);

            	if ( (LA42_0 == NUM_INT) )
            	{
            	    alt42 = 1;
            	}
            	else if ( ((LA42_0 >= COLON && LA42_0 <= PARAM)) )
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
            	        // Hql.g:310:11: NUM_INT
            	        {
            	        	NUM_INT135=(IToken)Match(input,NUM_INT,FOLLOW_NUM_INT_in_takeClause1666); 
            	        		NUM_INT135_tree = (IASTNode)adaptor.Create(NUM_INT135);
            	        		adaptor.AddChild(root_0, NUM_INT135_tree);


            	        }
            	        break;
            	    case 2 :
            	        // Hql.g:310:21: parameter
            	        {
            	        	PushFollow(FOLLOW_parameter_in_takeClause1670);
            	        	parameter136 = parameter();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, parameter136.Tree);

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
    // $ANTLR end "takeClause"

    public class parameter_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "parameter"
    // Hql.g:313:1: parameter : ( COLON identifier | PARAM ( NUM_INT )? );
    public HqlParser.parameter_return parameter() // throws RecognitionException [1]
    {   
        HqlParser.parameter_return retval = new HqlParser.parameter_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken COLON137 = null;
        IToken PARAM139 = null;
        IToken NUM_INT140 = null;
        HqlParser.identifier_return identifier138 = default(HqlParser.identifier_return);


        IASTNode COLON137_tree=null;
        IASTNode PARAM139_tree=null;
        IASTNode NUM_INT140_tree=null;

        try 
    	{
            // Hql.g:314:2: ( COLON identifier | PARAM ( NUM_INT )? )
            int alt44 = 2;
            int LA44_0 = input.LA(1);

            if ( (LA44_0 == COLON) )
            {
                alt44 = 1;
            }
            else if ( (LA44_0 == PARAM) )
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
                    // Hql.g:314:4: COLON identifier
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	COLON137=(IToken)Match(input,COLON,FOLLOW_COLON_in_parameter1682); 
                    		COLON137_tree = (IASTNode)adaptor.Create(COLON137);
                    		root_0 = (IASTNode)adaptor.BecomeRoot(COLON137_tree, root_0);

                    	PushFollow(FOLLOW_identifier_in_parameter1685);
                    	identifier138 = identifier();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, identifier138.Tree);

                    }
                    break;
                case 2 :
                    // Hql.g:315:4: PARAM ( NUM_INT )?
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PARAM139=(IToken)Match(input,PARAM,FOLLOW_PARAM_in_parameter1690); 
                    		PARAM139_tree = (IASTNode)adaptor.Create(PARAM139);
                    		root_0 = (IASTNode)adaptor.BecomeRoot(PARAM139_tree, root_0);

                    	// Hql.g:315:11: ( NUM_INT )?
                    	int alt43 = 2;
                    	int LA43_0 = input.LA(1);

                    	if ( (LA43_0 == NUM_INT) )
                    	{
                    	    alt43 = 1;
                    	}
                    	switch (alt43) 
                    	{
                    	    case 1 :
                    	        // Hql.g:315:12: NUM_INT
                    	        {
                    	        	NUM_INT140=(IToken)Match(input,NUM_INT,FOLLOW_NUM_INT_in_parameter1694); 
                    	        		NUM_INT140_tree = (IASTNode)adaptor.Create(NUM_INT140);
                    	        		adaptor.AddChild(root_0, NUM_INT140_tree);


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
    // $ANTLR end "parameter"

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
    // Hql.g:318:1: orderElement : expression ( ascendingOrDescending )? ;
    public HqlParser.orderElement_return orderElement() // throws RecognitionException [1]
    {   
        HqlParser.orderElement_return retval = new HqlParser.orderElement_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.expression_return expression141 = default(HqlParser.expression_return);

        HqlParser.ascendingOrDescending_return ascendingOrDescending142 = default(HqlParser.ascendingOrDescending_return);



        try 
    	{
            // Hql.g:319:2: ( expression ( ascendingOrDescending )? )
            // Hql.g:319:4: expression ( ascendingOrDescending )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_expression_in_orderElement1707);
            	expression141 = expression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, expression141.Tree);
            	// Hql.g:319:15: ( ascendingOrDescending )?
            	int alt45 = 2;
            	int LA45_0 = input.LA(1);

            	if ( (LA45_0 == ASCENDING || LA45_0 == DESCENDING || (LA45_0 >= 133 && LA45_0 <= 134)) )
            	{
            	    alt45 = 1;
            	}
            	switch (alt45) 
            	{
            	    case 1 :
            	        // Hql.g:319:17: ascendingOrDescending
            	        {
            	        	PushFollow(FOLLOW_ascendingOrDescending_in_orderElement1711);
            	        	ascendingOrDescending142 = ascendingOrDescending();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, ascendingOrDescending142.Tree);

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
    // Hql.g:322:1: ascendingOrDescending : ( (a= 'asc' | a= 'ascending' ) -> ^( ASCENDING[$a.Text] ) | (d= 'desc' | d= 'descending' ) -> ^( DESCENDING[$d.Text] ) );
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
            // Hql.g:323:2: ( (a= 'asc' | a= 'ascending' ) -> ^( ASCENDING[$a.Text] ) | (d= 'desc' | d= 'descending' ) -> ^( DESCENDING[$d.Text] ) )
            int alt48 = 2;
            int LA48_0 = input.LA(1);

            if ( (LA48_0 == ASCENDING || LA48_0 == 133) )
            {
                alt48 = 1;
            }
            else if ( (LA48_0 == DESCENDING || LA48_0 == 134) )
            {
                alt48 = 2;
            }
            else 
            {
                NoViableAltException nvae_d48s0 =
                    new NoViableAltException("", 48, 0, input);

                throw nvae_d48s0;
            }
            switch (alt48) 
            {
                case 1 :
                    // Hql.g:323:4: (a= 'asc' | a= 'ascending' )
                    {
                    	// Hql.g:323:4: (a= 'asc' | a= 'ascending' )
                    	int alt46 = 2;
                    	int LA46_0 = input.LA(1);

                    	if ( (LA46_0 == ASCENDING) )
                    	{
                    	    alt46 = 1;
                    	}
                    	else if ( (LA46_0 == 133) )
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
                    	        // Hql.g:323:6: a= 'asc'
                    	        {
                    	        	a=(IToken)Match(input,ASCENDING,FOLLOW_ASCENDING_in_ascendingOrDescending1729);  
                    	        	stream_ASCENDING.Add(a);


                    	        }
                    	        break;
                    	    case 2 :
                    	        // Hql.g:323:16: a= 'ascending'
                    	        {
                    	        	a=(IToken)Match(input,133,FOLLOW_133_in_ascendingOrDescending1735);  
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
                    	// 324:3: -> ^( ASCENDING[$a.Text] )
                    	{
                    	    // Hql.g:324:6: ^( ASCENDING[$a.Text] )
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
                    // Hql.g:325:4: (d= 'desc' | d= 'descending' )
                    {
                    	// Hql.g:325:4: (d= 'desc' | d= 'descending' )
                    	int alt47 = 2;
                    	int LA47_0 = input.LA(1);

                    	if ( (LA47_0 == DESCENDING) )
                    	{
                    	    alt47 = 1;
                    	}
                    	else if ( (LA47_0 == 134) )
                    	{
                    	    alt47 = 2;
                    	}
                    	else 
                    	{
                    	    NoViableAltException nvae_d47s0 =
                    	        new NoViableAltException("", 47, 0, input);

                    	    throw nvae_d47s0;
                    	}
                    	switch (alt47) 
                    	{
                    	    case 1 :
                    	        // Hql.g:325:6: d= 'desc'
                    	        {
                    	        	d=(IToken)Match(input,DESCENDING,FOLLOW_DESCENDING_in_ascendingOrDescending1755);  
                    	        	stream_DESCENDING.Add(d);


                    	        }
                    	        break;
                    	    case 2 :
                    	        // Hql.g:325:17: d= 'descending'
                    	        {
                    	        	d=(IToken)Match(input,134,FOLLOW_134_in_ascendingOrDescending1761);  
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
                    	// 326:3: -> ^( DESCENDING[$d.Text] )
                    	{
                    	    // Hql.g:326:6: ^( DESCENDING[$d.Text] )
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
    // Hql.g:329:1: havingClause : HAVING logicalExpression ;
    public HqlParser.havingClause_return havingClause() // throws RecognitionException [1]
    {   
        HqlParser.havingClause_return retval = new HqlParser.havingClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken HAVING143 = null;
        HqlParser.logicalExpression_return logicalExpression144 = default(HqlParser.logicalExpression_return);


        IASTNode HAVING143_tree=null;

        try 
    	{
            // Hql.g:330:2: ( HAVING logicalExpression )
            // Hql.g:330:4: HAVING logicalExpression
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	HAVING143=(IToken)Match(input,HAVING,FOLLOW_HAVING_in_havingClause1782); 
            		HAVING143_tree = (IASTNode)adaptor.Create(HAVING143);
            		root_0 = (IASTNode)adaptor.BecomeRoot(HAVING143_tree, root_0);

            	PushFollow(FOLLOW_logicalExpression_in_havingClause1785);
            	logicalExpression144 = logicalExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, logicalExpression144.Tree);

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
    // Hql.g:333:1: whereClause : WHERE logicalExpression ;
    public HqlParser.whereClause_return whereClause() // throws RecognitionException [1]
    {   
        HqlParser.whereClause_return retval = new HqlParser.whereClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken WHERE145 = null;
        HqlParser.logicalExpression_return logicalExpression146 = default(HqlParser.logicalExpression_return);


        IASTNode WHERE145_tree=null;

        try 
    	{
            // Hql.g:334:2: ( WHERE logicalExpression )
            // Hql.g:334:4: WHERE logicalExpression
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	WHERE145=(IToken)Match(input,WHERE,FOLLOW_WHERE_in_whereClause1796); 
            		WHERE145_tree = (IASTNode)adaptor.Create(WHERE145);
            		root_0 = (IASTNode)adaptor.BecomeRoot(WHERE145_tree, root_0);

            	PushFollow(FOLLOW_logicalExpression_in_whereClause1799);
            	logicalExpression146 = logicalExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, logicalExpression146.Tree);

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
    // Hql.g:337:1: selectedPropertiesList : aliasedExpression ( COMMA aliasedExpression )* ;
    public HqlParser.selectedPropertiesList_return selectedPropertiesList() // throws RecognitionException [1]
    {   
        HqlParser.selectedPropertiesList_return retval = new HqlParser.selectedPropertiesList_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken COMMA148 = null;
        HqlParser.aliasedExpression_return aliasedExpression147 = default(HqlParser.aliasedExpression_return);

        HqlParser.aliasedExpression_return aliasedExpression149 = default(HqlParser.aliasedExpression_return);


        IASTNode COMMA148_tree=null;

        try 
    	{
            // Hql.g:338:2: ( aliasedExpression ( COMMA aliasedExpression )* )
            // Hql.g:338:4: aliasedExpression ( COMMA aliasedExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_aliasedExpression_in_selectedPropertiesList1810);
            	aliasedExpression147 = aliasedExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, aliasedExpression147.Tree);
            	// Hql.g:338:22: ( COMMA aliasedExpression )*
            	do 
            	{
            	    int alt49 = 2;
            	    int LA49_0 = input.LA(1);

            	    if ( (LA49_0 == COMMA) )
            	    {
            	        alt49 = 1;
            	    }


            	    switch (alt49) 
            		{
            			case 1 :
            			    // Hql.g:338:24: COMMA aliasedExpression
            			    {
            			    	COMMA148=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_selectedPropertiesList1814); 
            			    	PushFollow(FOLLOW_aliasedExpression_in_selectedPropertiesList1817);
            			    	aliasedExpression149 = aliasedExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, aliasedExpression149.Tree);

            			    }
            			    break;

            			default:
            			    goto loop49;
            	    }
            	} while (true);

            	loop49:
            		;	// Stops C# compiler whining that label 'loop49' has no statements


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
    // Hql.g:341:1: aliasedExpression : expression ( AS identifier )? ;
    public HqlParser.aliasedExpression_return aliasedExpression() // throws RecognitionException [1]
    {   
        HqlParser.aliasedExpression_return retval = new HqlParser.aliasedExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken AS151 = null;
        HqlParser.expression_return expression150 = default(HqlParser.expression_return);

        HqlParser.identifier_return identifier152 = default(HqlParser.identifier_return);


        IASTNode AS151_tree=null;

        try 
    	{
            // Hql.g:342:2: ( expression ( AS identifier )? )
            // Hql.g:342:4: expression ( AS identifier )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_expression_in_aliasedExpression1832);
            	expression150 = expression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, expression150.Tree);
            	// Hql.g:342:15: ( AS identifier )?
            	int alt50 = 2;
            	int LA50_0 = input.LA(1);

            	if ( (LA50_0 == AS) )
            	{
            	    alt50 = 1;
            	}
            	switch (alt50) 
            	{
            	    case 1 :
            	        // Hql.g:342:17: AS identifier
            	        {
            	        	AS151=(IToken)Match(input,AS,FOLLOW_AS_in_aliasedExpression1836); 
            	        		AS151_tree = (IASTNode)adaptor.Create(AS151);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(AS151_tree, root_0);

            	        	PushFollow(FOLLOW_identifier_in_aliasedExpression1839);
            	        	identifier152 = identifier();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, identifier152.Tree);

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
    // Hql.g:370:1: logicalExpression : expression ;
    public HqlParser.logicalExpression_return logicalExpression() // throws RecognitionException [1]
    {   
        HqlParser.logicalExpression_return retval = new HqlParser.logicalExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.expression_return expression153 = default(HqlParser.expression_return);



        try 
    	{
            // Hql.g:371:2: ( expression )
            // Hql.g:371:4: expression
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_expression_in_logicalExpression1878);
            	expression153 = expression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, expression153.Tree);

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
    // Hql.g:375:1: expression : logicalOrExpression ;
    public HqlParser.expression_return expression() // throws RecognitionException [1]
    {   
        HqlParser.expression_return retval = new HqlParser.expression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.logicalOrExpression_return logicalOrExpression154 = default(HqlParser.logicalOrExpression_return);



        try 
    	{
            // Hql.g:376:2: ( logicalOrExpression )
            // Hql.g:376:4: logicalOrExpression
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_logicalOrExpression_in_expression1890);
            	logicalOrExpression154 = logicalOrExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, logicalOrExpression154.Tree);

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
    // Hql.g:380:1: logicalOrExpression : logicalAndExpression ( OR logicalAndExpression )* ;
    public HqlParser.logicalOrExpression_return logicalOrExpression() // throws RecognitionException [1]
    {   
        HqlParser.logicalOrExpression_return retval = new HqlParser.logicalOrExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken OR156 = null;
        HqlParser.logicalAndExpression_return logicalAndExpression155 = default(HqlParser.logicalAndExpression_return);

        HqlParser.logicalAndExpression_return logicalAndExpression157 = default(HqlParser.logicalAndExpression_return);


        IASTNode OR156_tree=null;

        try 
    	{
            // Hql.g:381:2: ( logicalAndExpression ( OR logicalAndExpression )* )
            // Hql.g:381:4: logicalAndExpression ( OR logicalAndExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_logicalAndExpression_in_logicalOrExpression1902);
            	logicalAndExpression155 = logicalAndExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, logicalAndExpression155.Tree);
            	// Hql.g:381:25: ( OR logicalAndExpression )*
            	do 
            	{
            	    int alt51 = 2;
            	    int LA51_0 = input.LA(1);

            	    if ( (LA51_0 == OR) )
            	    {
            	        alt51 = 1;
            	    }


            	    switch (alt51) 
            		{
            			case 1 :
            			    // Hql.g:381:27: OR logicalAndExpression
            			    {
            			    	OR156=(IToken)Match(input,OR,FOLLOW_OR_in_logicalOrExpression1906); 
            			    		OR156_tree = (IASTNode)adaptor.Create(OR156);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(OR156_tree, root_0);

            			    	PushFollow(FOLLOW_logicalAndExpression_in_logicalOrExpression1909);
            			    	logicalAndExpression157 = logicalAndExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, logicalAndExpression157.Tree);

            			    }
            			    break;

            			default:
            			    goto loop51;
            	    }
            	} while (true);

            	loop51:
            		;	// Stops C# compiler whining that label 'loop51' has no statements


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
    // Hql.g:385:1: logicalAndExpression : negatedExpression ( AND negatedExpression )* ;
    public HqlParser.logicalAndExpression_return logicalAndExpression() // throws RecognitionException [1]
    {   
        HqlParser.logicalAndExpression_return retval = new HqlParser.logicalAndExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken AND159 = null;
        HqlParser.negatedExpression_return negatedExpression158 = default(HqlParser.negatedExpression_return);

        HqlParser.negatedExpression_return negatedExpression160 = default(HqlParser.negatedExpression_return);


        IASTNode AND159_tree=null;

        try 
    	{
            // Hql.g:386:2: ( negatedExpression ( AND negatedExpression )* )
            // Hql.g:386:4: negatedExpression ( AND negatedExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_negatedExpression_in_logicalAndExpression1924);
            	negatedExpression158 = negatedExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, negatedExpression158.Tree);
            	// Hql.g:386:22: ( AND negatedExpression )*
            	do 
            	{
            	    int alt52 = 2;
            	    int LA52_0 = input.LA(1);

            	    if ( (LA52_0 == AND) )
            	    {
            	        alt52 = 1;
            	    }


            	    switch (alt52) 
            		{
            			case 1 :
            			    // Hql.g:386:24: AND negatedExpression
            			    {
            			    	AND159=(IToken)Match(input,AND,FOLLOW_AND_in_logicalAndExpression1928); 
            			    		AND159_tree = (IASTNode)adaptor.Create(AND159);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(AND159_tree, root_0);

            			    	PushFollow(FOLLOW_negatedExpression_in_logicalAndExpression1931);
            			    	negatedExpression160 = negatedExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, negatedExpression160.Tree);

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
    // Hql.g:391:1: negatedExpression : ( NOT x= negatedExpression -> ^() | equalityExpression -> ^( equalityExpression ) );
    public HqlParser.negatedExpression_return negatedExpression() // throws RecognitionException [1]
    {   
        HqlParser.negatedExpression_return retval = new HqlParser.negatedExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken NOT161 = null;
        HqlParser.negatedExpression_return x = default(HqlParser.negatedExpression_return);

        HqlParser.equalityExpression_return equalityExpression162 = default(HqlParser.equalityExpression_return);


        IASTNode NOT161_tree=null;
        RewriteRuleTokenStream stream_NOT = new RewriteRuleTokenStream(adaptor,"token NOT");
        RewriteRuleSubtreeStream stream_equalityExpression = new RewriteRuleSubtreeStream(adaptor,"rule equalityExpression");
        RewriteRuleSubtreeStream stream_negatedExpression = new RewriteRuleSubtreeStream(adaptor,"rule negatedExpression");
         WeakKeywords(); 
        try 
    	{
            // Hql.g:393:2: ( NOT x= negatedExpression -> ^() | equalityExpression -> ^( equalityExpression ) )
            int alt53 = 2;
            int LA53_0 = input.LA(1);

            if ( (LA53_0 == NOT) )
            {
                alt53 = 1;
            }
            else if ( ((LA53_0 >= ALL && LA53_0 <= ANY) || LA53_0 == AVG || LA53_0 == COUNT || LA53_0 == ELEMENTS || (LA53_0 >= EXISTS && LA53_0 <= FALSE) || LA53_0 == INDICES || (LA53_0 >= MAX && LA53_0 <= MIN) || LA53_0 == NULL || (LA53_0 >= SOME && LA53_0 <= SUM) || LA53_0 == TRUE || LA53_0 == CASE || LA53_0 == EMPTY || (LA53_0 >= NUM_INT && LA53_0 <= NUM_LONG) || LA53_0 == OPEN || (LA53_0 >= COLON && LA53_0 <= PARAM) || LA53_0 == BNOT || (LA53_0 >= PLUS && LA53_0 <= MINUS) || (LA53_0 >= QUOTED_String && LA53_0 <= IDENT)) )
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
                    // Hql.g:393:4: NOT x= negatedExpression
                    {
                    	NOT161=(IToken)Match(input,NOT,FOLLOW_NOT_in_negatedExpression1952);  
                    	stream_NOT.Add(NOT161);

                    	PushFollow(FOLLOW_negatedExpression_in_negatedExpression1956);
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
                    	// 394:3: -> ^()
                    	{
                    	    // Hql.g:394:6: ^()
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
                    // Hql.g:395:4: equalityExpression
                    {
                    	PushFollow(FOLLOW_equalityExpression_in_negatedExpression1969);
                    	equalityExpression162 = equalityExpression();
                    	state.followingStackPointer--;

                    	stream_equalityExpression.Add(equalityExpression162.Tree);


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
                    	// 396:3: -> ^( equalityExpression )
                    	{
                    	    // Hql.g:396:6: ^( equalityExpression )
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
    // Hql.g:402:1: equalityExpression : x= relationalExpression ( ( EQ | isx= IS ( NOT )? | NE | ne= SQL_NE ) y= relationalExpression )* ;
    public HqlParser.equalityExpression_return equalityExpression() // throws RecognitionException [1]
    {   
        HqlParser.equalityExpression_return retval = new HqlParser.equalityExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken isx = null;
        IToken ne = null;
        IToken EQ163 = null;
        IToken NOT164 = null;
        IToken NE165 = null;
        HqlParser.relationalExpression_return x = default(HqlParser.relationalExpression_return);

        HqlParser.relationalExpression_return y = default(HqlParser.relationalExpression_return);


        IASTNode isx_tree=null;
        IASTNode ne_tree=null;
        IASTNode EQ163_tree=null;
        IASTNode NOT164_tree=null;
        IASTNode NE165_tree=null;

        try 
    	{
            // Hql.g:407:2: (x= relationalExpression ( ( EQ | isx= IS ( NOT )? | NE | ne= SQL_NE ) y= relationalExpression )* )
            // Hql.g:407:4: x= relationalExpression ( ( EQ | isx= IS ( NOT )? | NE | ne= SQL_NE ) y= relationalExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_relationalExpression_in_equalityExpression1999);
            	x = relationalExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, x.Tree);
            	// Hql.g:407:27: ( ( EQ | isx= IS ( NOT )? | NE | ne= SQL_NE ) y= relationalExpression )*
            	do 
            	{
            	    int alt56 = 2;
            	    int LA56_0 = input.LA(1);

            	    if ( (LA56_0 == IS || LA56_0 == EQ || (LA56_0 >= NE && LA56_0 <= SQL_NE)) )
            	    {
            	        alt56 = 1;
            	    }


            	    switch (alt56) 
            		{
            			case 1 :
            			    // Hql.g:408:3: ( EQ | isx= IS ( NOT )? | NE | ne= SQL_NE ) y= relationalExpression
            			    {
            			    	// Hql.g:408:3: ( EQ | isx= IS ( NOT )? | NE | ne= SQL_NE )
            			    	int alt55 = 4;
            			    	switch ( input.LA(1) ) 
            			    	{
            			    	case EQ:
            			    		{
            			    	    alt55 = 1;
            			    	    }
            			    	    break;
            			    	case IS:
            			    		{
            			    	    alt55 = 2;
            			    	    }
            			    	    break;
            			    	case NE:
            			    		{
            			    	    alt55 = 3;
            			    	    }
            			    	    break;
            			    	case SQL_NE:
            			    		{
            			    	    alt55 = 4;
            			    	    }
            			    	    break;
            			    		default:
            			    		    NoViableAltException nvae_d55s0 =
            			    		        new NoViableAltException("", 55, 0, input);

            			    		    throw nvae_d55s0;
            			    	}

            			    	switch (alt55) 
            			    	{
            			    	    case 1 :
            			    	        // Hql.g:408:5: EQ
            			    	        {
            			    	        	EQ163=(IToken)Match(input,EQ,FOLLOW_EQ_in_equalityExpression2007); 
            			    	        		EQ163_tree = (IASTNode)adaptor.Create(EQ163);
            			    	        		root_0 = (IASTNode)adaptor.BecomeRoot(EQ163_tree, root_0);


            			    	        }
            			    	        break;
            			    	    case 2 :
            			    	        // Hql.g:409:5: isx= IS ( NOT )?
            			    	        {
            			    	        	isx=(IToken)Match(input,IS,FOLLOW_IS_in_equalityExpression2016); 
            			    	        		isx_tree = (IASTNode)adaptor.Create(isx);
            			    	        		root_0 = (IASTNode)adaptor.BecomeRoot(isx_tree, root_0);

            			    	        	 isx.Type = EQ; 
            			    	        	// Hql.g:409:33: ( NOT )?
            			    	        	int alt54 = 2;
            			    	        	int LA54_0 = input.LA(1);

            			    	        	if ( (LA54_0 == NOT) )
            			    	        	{
            			    	        	    alt54 = 1;
            			    	        	}
            			    	        	switch (alt54) 
            			    	        	{
            			    	        	    case 1 :
            			    	        	        // Hql.g:409:34: NOT
            			    	        	        {
            			    	        	        	NOT164=(IToken)Match(input,NOT,FOLLOW_NOT_in_equalityExpression2022); 
            			    	        	        	 isx.Type =NE; 

            			    	        	        }
            			    	        	        break;

            			    	        	}


            			    	        }
            			    	        break;
            			    	    case 3 :
            			    	        // Hql.g:410:5: NE
            			    	        {
            			    	        	NE165=(IToken)Match(input,NE,FOLLOW_NE_in_equalityExpression2034); 
            			    	        		NE165_tree = (IASTNode)adaptor.Create(NE165);
            			    	        		root_0 = (IASTNode)adaptor.BecomeRoot(NE165_tree, root_0);


            			    	        }
            			    	        break;
            			    	    case 4 :
            			    	        // Hql.g:411:5: ne= SQL_NE
            			    	        {
            			    	        	ne=(IToken)Match(input,SQL_NE,FOLLOW_SQL_NE_in_equalityExpression2043); 
            			    	        		ne_tree = (IASTNode)adaptor.Create(ne);
            			    	        		root_0 = (IASTNode)adaptor.BecomeRoot(ne_tree, root_0);

            			    	        	 ne.Type = NE; 

            			    	        }
            			    	        break;

            			    	}

            			    	PushFollow(FOLLOW_relationalExpression_in_equalityExpression2054);
            			    	y = relationalExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, y.Tree);

            			    }
            			    break;

            			default:
            			    goto loop56;
            	    }
            	} while (true);

            	loop56:
            		;	// Stops C# compiler whining that label 'loop56' has no statements


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
    // Hql.g:419:1: relationalExpression : concatenation ( ( ( ( LT | GT | LE | GE ) bitwiseNotExpression )* ) | (n= NOT )? ( (i= IN inList ) | (b= BETWEEN betweenList ) | (l= LIKE concatenation likeEscape ) | ( MEMBER ( OF )? p= path ) ) ) ;
    public HqlParser.relationalExpression_return relationalExpression() // throws RecognitionException [1]
    {   
        HqlParser.relationalExpression_return retval = new HqlParser.relationalExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken n = null;
        IToken i = null;
        IToken b = null;
        IToken l = null;
        IToken LT167 = null;
        IToken GT168 = null;
        IToken LE169 = null;
        IToken GE170 = null;
        IToken MEMBER176 = null;
        IToken OF177 = null;
        HqlParser.path_return p = default(HqlParser.path_return);

        HqlParser.concatenation_return concatenation166 = default(HqlParser.concatenation_return);

        HqlParser.bitwiseNotExpression_return bitwiseNotExpression171 = default(HqlParser.bitwiseNotExpression_return);

        HqlParser.inList_return inList172 = default(HqlParser.inList_return);

        HqlParser.betweenList_return betweenList173 = default(HqlParser.betweenList_return);

        HqlParser.concatenation_return concatenation174 = default(HqlParser.concatenation_return);

        HqlParser.likeEscape_return likeEscape175 = default(HqlParser.likeEscape_return);


        IASTNode n_tree=null;
        IASTNode i_tree=null;
        IASTNode b_tree=null;
        IASTNode l_tree=null;
        IASTNode LT167_tree=null;
        IASTNode GT168_tree=null;
        IASTNode LE169_tree=null;
        IASTNode GE170_tree=null;
        IASTNode MEMBER176_tree=null;
        IASTNode OF177_tree=null;

        try 
    	{
            // Hql.g:420:2: ( concatenation ( ( ( ( LT | GT | LE | GE ) bitwiseNotExpression )* ) | (n= NOT )? ( (i= IN inList ) | (b= BETWEEN betweenList ) | (l= LIKE concatenation likeEscape ) | ( MEMBER ( OF )? p= path ) ) ) )
            // Hql.g:420:4: concatenation ( ( ( ( LT | GT | LE | GE ) bitwiseNotExpression )* ) | (n= NOT )? ( (i= IN inList ) | (b= BETWEEN betweenList ) | (l= LIKE concatenation likeEscape ) | ( MEMBER ( OF )? p= path ) ) )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_concatenation_in_relationalExpression2071);
            	concatenation166 = concatenation();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, concatenation166.Tree);
            	// Hql.g:420:18: ( ( ( ( LT | GT | LE | GE ) bitwiseNotExpression )* ) | (n= NOT )? ( (i= IN inList ) | (b= BETWEEN betweenList ) | (l= LIKE concatenation likeEscape ) | ( MEMBER ( OF )? p= path ) ) )
            	int alt62 = 2;
            	int LA62_0 = input.LA(1);

            	if ( (LA62_0 == EOF || (LA62_0 >= AND && LA62_0 <= ASCENDING) || LA62_0 == DESCENDING || (LA62_0 >= FROM && LA62_0 <= HAVING) || LA62_0 == INNER || (LA62_0 >= IS && LA62_0 <= LEFT) || (LA62_0 >= OR && LA62_0 <= ORDER) || LA62_0 == RIGHT || LA62_0 == SKIP || LA62_0 == TAKE || LA62_0 == UNION || LA62_0 == WHERE || (LA62_0 >= END && LA62_0 <= WHEN) || (LA62_0 >= COMMA && LA62_0 <= EQ) || LA62_0 == CLOSE || (LA62_0 >= NE && LA62_0 <= GE) || LA62_0 == CLOSE_BRACKET || (LA62_0 >= 133 && LA62_0 <= 134)) )
            	{
            	    alt62 = 1;
            	}
            	else if ( (LA62_0 == BETWEEN || LA62_0 == IN || LA62_0 == LIKE || LA62_0 == NOT || LA62_0 == MEMBER) )
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
            	        // Hql.g:421:3: ( ( ( LT | GT | LE | GE ) bitwiseNotExpression )* )
            	        {
            	        	// Hql.g:421:3: ( ( ( LT | GT | LE | GE ) bitwiseNotExpression )* )
            	        	// Hql.g:421:5: ( ( LT | GT | LE | GE ) bitwiseNotExpression )*
            	        	{
            	        		// Hql.g:421:5: ( ( LT | GT | LE | GE ) bitwiseNotExpression )*
            	        		do 
            	        		{
            	        		    int alt58 = 2;
            	        		    int LA58_0 = input.LA(1);

            	        		    if ( ((LA58_0 >= LT && LA58_0 <= GE)) )
            	        		    {
            	        		        alt58 = 1;
            	        		    }


            	        		    switch (alt58) 
            	        			{
            	        				case 1 :
            	        				    // Hql.g:421:7: ( LT | GT | LE | GE ) bitwiseNotExpression
            	        				    {
            	        				    	// Hql.g:421:7: ( LT | GT | LE | GE )
            	        				    	int alt57 = 4;
            	        				    	switch ( input.LA(1) ) 
            	        				    	{
            	        				    	case LT:
            	        				    		{
            	        				    	    alt57 = 1;
            	        				    	    }
            	        				    	    break;
            	        				    	case GT:
            	        				    		{
            	        				    	    alt57 = 2;
            	        				    	    }
            	        				    	    break;
            	        				    	case LE:
            	        				    		{
            	        				    	    alt57 = 3;
            	        				    	    }
            	        				    	    break;
            	        				    	case GE:
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
            	        				    	        // Hql.g:421:9: LT
            	        				    	        {
            	        				    	        	LT167=(IToken)Match(input,LT,FOLLOW_LT_in_relationalExpression2083); 
            	        				    	        		LT167_tree = (IASTNode)adaptor.Create(LT167);
            	        				    	        		root_0 = (IASTNode)adaptor.BecomeRoot(LT167_tree, root_0);


            	        				    	        }
            	        				    	        break;
            	        				    	    case 2 :
            	        				    	        // Hql.g:421:15: GT
            	        				    	        {
            	        				    	        	GT168=(IToken)Match(input,GT,FOLLOW_GT_in_relationalExpression2088); 
            	        				    	        		GT168_tree = (IASTNode)adaptor.Create(GT168);
            	        				    	        		root_0 = (IASTNode)adaptor.BecomeRoot(GT168_tree, root_0);


            	        				    	        }
            	        				    	        break;
            	        				    	    case 3 :
            	        				    	        // Hql.g:421:21: LE
            	        				    	        {
            	        				    	        	LE169=(IToken)Match(input,LE,FOLLOW_LE_in_relationalExpression2093); 
            	        				    	        		LE169_tree = (IASTNode)adaptor.Create(LE169);
            	        				    	        		root_0 = (IASTNode)adaptor.BecomeRoot(LE169_tree, root_0);


            	        				    	        }
            	        				    	        break;
            	        				    	    case 4 :
            	        				    	        // Hql.g:421:27: GE
            	        				    	        {
            	        				    	        	GE170=(IToken)Match(input,GE,FOLLOW_GE_in_relationalExpression2098); 
            	        				    	        		GE170_tree = (IASTNode)adaptor.Create(GE170);
            	        				    	        		root_0 = (IASTNode)adaptor.BecomeRoot(GE170_tree, root_0);


            	        				    	        }
            	        				    	        break;

            	        				    	}

            	        				    	PushFollow(FOLLOW_bitwiseNotExpression_in_relationalExpression2103);
            	        				    	bitwiseNotExpression171 = bitwiseNotExpression();
            	        				    	state.followingStackPointer--;

            	        				    	adaptor.AddChild(root_0, bitwiseNotExpression171.Tree);

            	        				    }
            	        				    break;

            	        				default:
            	        				    goto loop58;
            	        		    }
            	        		} while (true);

            	        		loop58:
            	        			;	// Stops C# compiler whining that label 'loop58' has no statements


            	        	}


            	        }
            	        break;
            	    case 2 :
            	        // Hql.g:423:5: (n= NOT )? ( (i= IN inList ) | (b= BETWEEN betweenList ) | (l= LIKE concatenation likeEscape ) | ( MEMBER ( OF )? p= path ) )
            	        {
            	        	// Hql.g:423:5: (n= NOT )?
            	        	int alt59 = 2;
            	        	int LA59_0 = input.LA(1);

            	        	if ( (LA59_0 == NOT) )
            	        	{
            	        	    alt59 = 1;
            	        	}
            	        	switch (alt59) 
            	        	{
            	        	    case 1 :
            	        	        // Hql.g:423:6: n= NOT
            	        	        {
            	        	        	n=(IToken)Match(input,NOT,FOLLOW_NOT_in_relationalExpression2120); 

            	        	        }
            	        	        break;

            	        	}

            	        	// Hql.g:423:15: ( (i= IN inList ) | (b= BETWEEN betweenList ) | (l= LIKE concatenation likeEscape ) | ( MEMBER ( OF )? p= path ) )
            	        	int alt61 = 4;
            	        	switch ( input.LA(1) ) 
            	        	{
            	        	case IN:
            	        		{
            	        	    alt61 = 1;
            	        	    }
            	        	    break;
            	        	case BETWEEN:
            	        		{
            	        	    alt61 = 2;
            	        	    }
            	        	    break;
            	        	case LIKE:
            	        		{
            	        	    alt61 = 3;
            	        	    }
            	        	    break;
            	        	case MEMBER:
            	        		{
            	        	    alt61 = 4;
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
            	        	        // Hql.g:426:4: (i= IN inList )
            	        	        {
            	        	        	// Hql.g:426:4: (i= IN inList )
            	        	        	// Hql.g:426:5: i= IN inList
            	        	        	{
            	        	        		i=(IToken)Match(input,IN,FOLLOW_IN_in_relationalExpression2141); 
            	        	        			i_tree = (IASTNode)adaptor.Create(i);
            	        	        			root_0 = (IASTNode)adaptor.BecomeRoot(i_tree, root_0);


            	        	        							i.Type = (n == null) ? IN : NOT_IN;
            	        	        							i.Text = (n == null) ? "in" : "not in";
            	        	        						
            	        	        		PushFollow(FOLLOW_inList_in_relationalExpression2150);
            	        	        		inList172 = inList();
            	        	        		state.followingStackPointer--;

            	        	        		adaptor.AddChild(root_0, inList172.Tree);

            	        	        	}


            	        	        }
            	        	        break;
            	        	    case 2 :
            	        	        // Hql.g:431:6: (b= BETWEEN betweenList )
            	        	        {
            	        	        	// Hql.g:431:6: (b= BETWEEN betweenList )
            	        	        	// Hql.g:431:7: b= BETWEEN betweenList
            	        	        	{
            	        	        		b=(IToken)Match(input,BETWEEN,FOLLOW_BETWEEN_in_relationalExpression2161); 
            	        	        			b_tree = (IASTNode)adaptor.Create(b);
            	        	        			root_0 = (IASTNode)adaptor.BecomeRoot(b_tree, root_0);


            	        	        							b.Type = (n == null) ? BETWEEN : NOT_BETWEEN;
            	        	        							b.Text = (n == null) ? "between" : "not between";
            	        	        						
            	        	        		PushFollow(FOLLOW_betweenList_in_relationalExpression2170);
            	        	        		betweenList173 = betweenList();
            	        	        		state.followingStackPointer--;

            	        	        		adaptor.AddChild(root_0, betweenList173.Tree);

            	        	        	}


            	        	        }
            	        	        break;
            	        	    case 3 :
            	        	        // Hql.g:436:6: (l= LIKE concatenation likeEscape )
            	        	        {
            	        	        	// Hql.g:436:6: (l= LIKE concatenation likeEscape )
            	        	        	// Hql.g:436:7: l= LIKE concatenation likeEscape
            	        	        	{
            	        	        		l=(IToken)Match(input,LIKE,FOLLOW_LIKE_in_relationalExpression2182); 
            	        	        			l_tree = (IASTNode)adaptor.Create(l);
            	        	        			root_0 = (IASTNode)adaptor.BecomeRoot(l_tree, root_0);


            	        	        							l.Type = (n == null) ? LIKE : NOT_LIKE;
            	        	        							l.Text = (n == null) ? "like" : "not like";
            	        	        						
            	        	        		PushFollow(FOLLOW_concatenation_in_relationalExpression2191);
            	        	        		concatenation174 = concatenation();
            	        	        		state.followingStackPointer--;

            	        	        		adaptor.AddChild(root_0, concatenation174.Tree);
            	        	        		PushFollow(FOLLOW_likeEscape_in_relationalExpression2193);
            	        	        		likeEscape175 = likeEscape();
            	        	        		state.followingStackPointer--;

            	        	        		adaptor.AddChild(root_0, likeEscape175.Tree);

            	        	        	}


            	        	        }
            	        	        break;
            	        	    case 4 :
            	        	        // Hql.g:441:6: ( MEMBER ( OF )? p= path )
            	        	        {
            	        	        	// Hql.g:441:6: ( MEMBER ( OF )? p= path )
            	        	        	// Hql.g:441:7: MEMBER ( OF )? p= path
            	        	        	{
            	        	        		MEMBER176=(IToken)Match(input,MEMBER,FOLLOW_MEMBER_in_relationalExpression2202); 
            	        	        		// Hql.g:441:15: ( OF )?
            	        	        		int alt60 = 2;
            	        	        		int LA60_0 = input.LA(1);

            	        	        		if ( (LA60_0 == OF) )
            	        	        		{
            	        	        		    alt60 = 1;
            	        	        		}
            	        	        		switch (alt60) 
            	        	        		{
            	        	        		    case 1 :
            	        	        		        // Hql.g:441:16: OF
            	        	        		        {
            	        	        		        	OF177=(IToken)Match(input,OF,FOLLOW_OF_in_relationalExpression2206); 

            	        	        		        }
            	        	        		        break;

            	        	        		}

            	        	        		PushFollow(FOLLOW_path_in_relationalExpression2213);
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
    // Hql.g:448:1: likeEscape : ( ESCAPE concatenation )? ;
    public HqlParser.likeEscape_return likeEscape() // throws RecognitionException [1]
    {   
        HqlParser.likeEscape_return retval = new HqlParser.likeEscape_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken ESCAPE178 = null;
        HqlParser.concatenation_return concatenation179 = default(HqlParser.concatenation_return);


        IASTNode ESCAPE178_tree=null;

        try 
    	{
            // Hql.g:449:2: ( ( ESCAPE concatenation )? )
            // Hql.g:449:4: ( ESCAPE concatenation )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// Hql.g:449:4: ( ESCAPE concatenation )?
            	int alt63 = 2;
            	int LA63_0 = input.LA(1);

            	if ( (LA63_0 == ESCAPE) )
            	{
            	    alt63 = 1;
            	}
            	switch (alt63) 
            	{
            	    case 1 :
            	        // Hql.g:449:5: ESCAPE concatenation
            	        {
            	        	ESCAPE178=(IToken)Match(input,ESCAPE,FOLLOW_ESCAPE_in_likeEscape2240); 
            	        		ESCAPE178_tree = (IASTNode)adaptor.Create(ESCAPE178);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(ESCAPE178_tree, root_0);

            	        	PushFollow(FOLLOW_concatenation_in_likeEscape2243);
            	        	concatenation179 = concatenation();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, concatenation179.Tree);

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
    // Hql.g:452:1: inList : compoundExpr -> ^( IN_LIST[\"inList\"] compoundExpr ) ;
    public HqlParser.inList_return inList() // throws RecognitionException [1]
    {   
        HqlParser.inList_return retval = new HqlParser.inList_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.compoundExpr_return compoundExpr180 = default(HqlParser.compoundExpr_return);


        RewriteRuleSubtreeStream stream_compoundExpr = new RewriteRuleSubtreeStream(adaptor,"rule compoundExpr");
        try 
    	{
            // Hql.g:453:2: ( compoundExpr -> ^( IN_LIST[\"inList\"] compoundExpr ) )
            // Hql.g:453:4: compoundExpr
            {
            	PushFollow(FOLLOW_compoundExpr_in_inList2256);
            	compoundExpr180 = compoundExpr();
            	state.followingStackPointer--;

            	stream_compoundExpr.Add(compoundExpr180.Tree);


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
            	// 454:2: -> ^( IN_LIST[\"inList\"] compoundExpr )
            	{
            	    // Hql.g:454:5: ^( IN_LIST[\"inList\"] compoundExpr )
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
    // Hql.g:457:1: betweenList : concatenation AND concatenation ;
    public HqlParser.betweenList_return betweenList() // throws RecognitionException [1]
    {   
        HqlParser.betweenList_return retval = new HqlParser.betweenList_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken AND182 = null;
        HqlParser.concatenation_return concatenation181 = default(HqlParser.concatenation_return);

        HqlParser.concatenation_return concatenation183 = default(HqlParser.concatenation_return);


        IASTNode AND182_tree=null;

        try 
    	{
            // Hql.g:458:2: ( concatenation AND concatenation )
            // Hql.g:458:4: concatenation AND concatenation
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_concatenation_in_betweenList2277);
            	concatenation181 = concatenation();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, concatenation181.Tree);
            	AND182=(IToken)Match(input,AND,FOLLOW_AND_in_betweenList2279); 
            	PushFollow(FOLLOW_concatenation_in_betweenList2282);
            	concatenation183 = concatenation();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, concatenation183.Tree);

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
    // Hql.g:462:1: concatenation : a= bitwiseNotExpression (c= CONCAT bitwiseNotExpression ( CONCAT bitwiseNotExpression )* )? ;
    public HqlParser.concatenation_return concatenation() // throws RecognitionException [1]
    {   
        HqlParser.concatenation_return retval = new HqlParser.concatenation_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken c = null;
        IToken CONCAT185 = null;
        HqlParser.bitwiseNotExpression_return a = default(HqlParser.bitwiseNotExpression_return);

        HqlParser.bitwiseNotExpression_return bitwiseNotExpression184 = default(HqlParser.bitwiseNotExpression_return);

        HqlParser.bitwiseNotExpression_return bitwiseNotExpression186 = default(HqlParser.bitwiseNotExpression_return);


        IASTNode c_tree=null;
        IASTNode CONCAT185_tree=null;

        try 
    	{
            // Hql.g:473:2: (a= bitwiseNotExpression (c= CONCAT bitwiseNotExpression ( CONCAT bitwiseNotExpression )* )? )
            // Hql.g:473:4: a= bitwiseNotExpression (c= CONCAT bitwiseNotExpression ( CONCAT bitwiseNotExpression )* )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_bitwiseNotExpression_in_concatenation2301);
            	a = bitwiseNotExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, a.Tree);
            	// Hql.g:474:2: (c= CONCAT bitwiseNotExpression ( CONCAT bitwiseNotExpression )* )?
            	int alt65 = 2;
            	int LA65_0 = input.LA(1);

            	if ( (LA65_0 == CONCAT) )
            	{
            	    alt65 = 1;
            	}
            	switch (alt65) 
            	{
            	    case 1 :
            	        // Hql.g:474:4: c= CONCAT bitwiseNotExpression ( CONCAT bitwiseNotExpression )*
            	        {
            	        	c=(IToken)Match(input,CONCAT,FOLLOW_CONCAT_in_concatenation2309); 
            	        		c_tree = (IASTNode)adaptor.Create(c);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(c_tree, root_0);

            	        	 c.Type = EXPR_LIST; c.Text = "concatList"; 
            	        	PushFollow(FOLLOW_bitwiseNotExpression_in_concatenation2318);
            	        	bitwiseNotExpression184 = bitwiseNotExpression();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, bitwiseNotExpression184.Tree);
            	        	// Hql.g:476:4: ( CONCAT bitwiseNotExpression )*
            	        	do 
            	        	{
            	        	    int alt64 = 2;
            	        	    int LA64_0 = input.LA(1);

            	        	    if ( (LA64_0 == CONCAT) )
            	        	    {
            	        	        alt64 = 1;
            	        	    }


            	        	    switch (alt64) 
            	        		{
            	        			case 1 :
            	        			    // Hql.g:476:6: CONCAT bitwiseNotExpression
            	        			    {
            	        			    	CONCAT185=(IToken)Match(input,CONCAT,FOLLOW_CONCAT_in_concatenation2325); 
            	        			    	PushFollow(FOLLOW_bitwiseNotExpression_in_concatenation2328);
            	        			    	bitwiseNotExpression186 = bitwiseNotExpression();
            	        			    	state.followingStackPointer--;

            	        			    	adaptor.AddChild(root_0, bitwiseNotExpression186.Tree);

            	        			    }
            	        			    break;

            	        			default:
            	        			    goto loop64;
            	        	    }
            	        	} while (true);

            	        	loop64:
            	        		;	// Stops C# compiler whining that label 'loop64' has no statements


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
    // Hql.g:481:1: bitwiseNotExpression : ( ( BNOT bitwiseOrExpression ) | bitwiseOrExpression );
    public HqlParser.bitwiseNotExpression_return bitwiseNotExpression() // throws RecognitionException [1]
    {   
        HqlParser.bitwiseNotExpression_return retval = new HqlParser.bitwiseNotExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken BNOT187 = null;
        HqlParser.bitwiseOrExpression_return bitwiseOrExpression188 = default(HqlParser.bitwiseOrExpression_return);

        HqlParser.bitwiseOrExpression_return bitwiseOrExpression189 = default(HqlParser.bitwiseOrExpression_return);


        IASTNode BNOT187_tree=null;

        try 
    	{
            // Hql.g:482:2: ( ( BNOT bitwiseOrExpression ) | bitwiseOrExpression )
            int alt66 = 2;
            int LA66_0 = input.LA(1);

            if ( (LA66_0 == BNOT) )
            {
                alt66 = 1;
            }
            else if ( ((LA66_0 >= ALL && LA66_0 <= ANY) || LA66_0 == AVG || LA66_0 == COUNT || LA66_0 == ELEMENTS || (LA66_0 >= EXISTS && LA66_0 <= FALSE) || LA66_0 == INDICES || (LA66_0 >= MAX && LA66_0 <= MIN) || LA66_0 == NULL || (LA66_0 >= SOME && LA66_0 <= SUM) || LA66_0 == TRUE || LA66_0 == CASE || LA66_0 == EMPTY || (LA66_0 >= NUM_INT && LA66_0 <= NUM_LONG) || LA66_0 == OPEN || (LA66_0 >= COLON && LA66_0 <= PARAM) || (LA66_0 >= PLUS && LA66_0 <= MINUS) || (LA66_0 >= QUOTED_String && LA66_0 <= IDENT)) )
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
                    // Hql.g:482:4: ( BNOT bitwiseOrExpression )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	// Hql.g:482:4: ( BNOT bitwiseOrExpression )
                    	// Hql.g:482:5: BNOT bitwiseOrExpression
                    	{
                    		BNOT187=(IToken)Match(input,BNOT,FOLLOW_BNOT_in_bitwiseNotExpression2352); 
                    			BNOT187_tree = (IASTNode)adaptor.Create(BNOT187);
                    			root_0 = (IASTNode)adaptor.BecomeRoot(BNOT187_tree, root_0);

                    		PushFollow(FOLLOW_bitwiseOrExpression_in_bitwiseNotExpression2355);
                    		bitwiseOrExpression188 = bitwiseOrExpression();
                    		state.followingStackPointer--;

                    		adaptor.AddChild(root_0, bitwiseOrExpression188.Tree);

                    	}


                    }
                    break;
                case 2 :
                    // Hql.g:483:4: bitwiseOrExpression
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_bitwiseOrExpression_in_bitwiseNotExpression2361);
                    	bitwiseOrExpression189 = bitwiseOrExpression();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, bitwiseOrExpression189.Tree);

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
    // Hql.g:486:1: bitwiseOrExpression : bitwiseXOrExpression ( BOR bitwiseXOrExpression )* ;
    public HqlParser.bitwiseOrExpression_return bitwiseOrExpression() // throws RecognitionException [1]
    {   
        HqlParser.bitwiseOrExpression_return retval = new HqlParser.bitwiseOrExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken BOR191 = null;
        HqlParser.bitwiseXOrExpression_return bitwiseXOrExpression190 = default(HqlParser.bitwiseXOrExpression_return);

        HqlParser.bitwiseXOrExpression_return bitwiseXOrExpression192 = default(HqlParser.bitwiseXOrExpression_return);


        IASTNode BOR191_tree=null;

        try 
    	{
            // Hql.g:487:2: ( bitwiseXOrExpression ( BOR bitwiseXOrExpression )* )
            // Hql.g:487:4: bitwiseXOrExpression ( BOR bitwiseXOrExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_bitwiseXOrExpression_in_bitwiseOrExpression2373);
            	bitwiseXOrExpression190 = bitwiseXOrExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, bitwiseXOrExpression190.Tree);
            	// Hql.g:487:25: ( BOR bitwiseXOrExpression )*
            	do 
            	{
            	    int alt67 = 2;
            	    int LA67_0 = input.LA(1);

            	    if ( (LA67_0 == BOR) )
            	    {
            	        alt67 = 1;
            	    }


            	    switch (alt67) 
            		{
            			case 1 :
            			    // Hql.g:487:26: BOR bitwiseXOrExpression
            			    {
            			    	BOR191=(IToken)Match(input,BOR,FOLLOW_BOR_in_bitwiseOrExpression2376); 
            			    		BOR191_tree = (IASTNode)adaptor.Create(BOR191);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(BOR191_tree, root_0);

            			    	PushFollow(FOLLOW_bitwiseXOrExpression_in_bitwiseOrExpression2379);
            			    	bitwiseXOrExpression192 = bitwiseXOrExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, bitwiseXOrExpression192.Tree);

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
    // Hql.g:490:1: bitwiseXOrExpression : bitwiseAndExpression ( BXOR bitwiseAndExpression )* ;
    public HqlParser.bitwiseXOrExpression_return bitwiseXOrExpression() // throws RecognitionException [1]
    {   
        HqlParser.bitwiseXOrExpression_return retval = new HqlParser.bitwiseXOrExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken BXOR194 = null;
        HqlParser.bitwiseAndExpression_return bitwiseAndExpression193 = default(HqlParser.bitwiseAndExpression_return);

        HqlParser.bitwiseAndExpression_return bitwiseAndExpression195 = default(HqlParser.bitwiseAndExpression_return);


        IASTNode BXOR194_tree=null;

        try 
    	{
            // Hql.g:491:2: ( bitwiseAndExpression ( BXOR bitwiseAndExpression )* )
            // Hql.g:491:4: bitwiseAndExpression ( BXOR bitwiseAndExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_bitwiseAndExpression_in_bitwiseXOrExpression2393);
            	bitwiseAndExpression193 = bitwiseAndExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, bitwiseAndExpression193.Tree);
            	// Hql.g:491:25: ( BXOR bitwiseAndExpression )*
            	do 
            	{
            	    int alt68 = 2;
            	    int LA68_0 = input.LA(1);

            	    if ( (LA68_0 == BXOR) )
            	    {
            	        alt68 = 1;
            	    }


            	    switch (alt68) 
            		{
            			case 1 :
            			    // Hql.g:491:26: BXOR bitwiseAndExpression
            			    {
            			    	BXOR194=(IToken)Match(input,BXOR,FOLLOW_BXOR_in_bitwiseXOrExpression2396); 
            			    		BXOR194_tree = (IASTNode)adaptor.Create(BXOR194);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(BXOR194_tree, root_0);

            			    	PushFollow(FOLLOW_bitwiseAndExpression_in_bitwiseXOrExpression2399);
            			    	bitwiseAndExpression195 = bitwiseAndExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, bitwiseAndExpression195.Tree);

            			    }
            			    break;

            			default:
            			    goto loop68;
            	    }
            	} while (true);

            	loop68:
            		;	// Stops C# compiler whining that label 'loop68' has no statements


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
    // Hql.g:494:1: bitwiseAndExpression : additiveExpression ( BAND additiveExpression )* ;
    public HqlParser.bitwiseAndExpression_return bitwiseAndExpression() // throws RecognitionException [1]
    {   
        HqlParser.bitwiseAndExpression_return retval = new HqlParser.bitwiseAndExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken BAND197 = null;
        HqlParser.additiveExpression_return additiveExpression196 = default(HqlParser.additiveExpression_return);

        HqlParser.additiveExpression_return additiveExpression198 = default(HqlParser.additiveExpression_return);


        IASTNode BAND197_tree=null;

        try 
    	{
            // Hql.g:495:2: ( additiveExpression ( BAND additiveExpression )* )
            // Hql.g:495:4: additiveExpression ( BAND additiveExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_additiveExpression_in_bitwiseAndExpression2413);
            	additiveExpression196 = additiveExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, additiveExpression196.Tree);
            	// Hql.g:495:23: ( BAND additiveExpression )*
            	do 
            	{
            	    int alt69 = 2;
            	    int LA69_0 = input.LA(1);

            	    if ( (LA69_0 == BAND) )
            	    {
            	        alt69 = 1;
            	    }


            	    switch (alt69) 
            		{
            			case 1 :
            			    // Hql.g:495:24: BAND additiveExpression
            			    {
            			    	BAND197=(IToken)Match(input,BAND,FOLLOW_BAND_in_bitwiseAndExpression2416); 
            			    		BAND197_tree = (IASTNode)adaptor.Create(BAND197);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(BAND197_tree, root_0);

            			    	PushFollow(FOLLOW_additiveExpression_in_bitwiseAndExpression2419);
            			    	additiveExpression198 = additiveExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, additiveExpression198.Tree);

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
    // Hql.g:499:1: additiveExpression : multiplyExpression ( ( PLUS | MINUS ) multiplyExpression )* ;
    public HqlParser.additiveExpression_return additiveExpression() // throws RecognitionException [1]
    {   
        HqlParser.additiveExpression_return retval = new HqlParser.additiveExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken PLUS200 = null;
        IToken MINUS201 = null;
        HqlParser.multiplyExpression_return multiplyExpression199 = default(HqlParser.multiplyExpression_return);

        HqlParser.multiplyExpression_return multiplyExpression202 = default(HqlParser.multiplyExpression_return);


        IASTNode PLUS200_tree=null;
        IASTNode MINUS201_tree=null;

        try 
    	{
            // Hql.g:500:2: ( multiplyExpression ( ( PLUS | MINUS ) multiplyExpression )* )
            // Hql.g:500:4: multiplyExpression ( ( PLUS | MINUS ) multiplyExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_multiplyExpression_in_additiveExpression2433);
            	multiplyExpression199 = multiplyExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, multiplyExpression199.Tree);
            	// Hql.g:500:23: ( ( PLUS | MINUS ) multiplyExpression )*
            	do 
            	{
            	    int alt71 = 2;
            	    int LA71_0 = input.LA(1);

            	    if ( ((LA71_0 >= PLUS && LA71_0 <= MINUS)) )
            	    {
            	        alt71 = 1;
            	    }


            	    switch (alt71) 
            		{
            			case 1 :
            			    // Hql.g:500:25: ( PLUS | MINUS ) multiplyExpression
            			    {
            			    	// Hql.g:500:25: ( PLUS | MINUS )
            			    	int alt70 = 2;
            			    	int LA70_0 = input.LA(1);

            			    	if ( (LA70_0 == PLUS) )
            			    	{
            			    	    alt70 = 1;
            			    	}
            			    	else if ( (LA70_0 == MINUS) )
            			    	{
            			    	    alt70 = 2;
            			    	}
            			    	else 
            			    	{
            			    	    NoViableAltException nvae_d70s0 =
            			    	        new NoViableAltException("", 70, 0, input);

            			    	    throw nvae_d70s0;
            			    	}
            			    	switch (alt70) 
            			    	{
            			    	    case 1 :
            			    	        // Hql.g:500:27: PLUS
            			    	        {
            			    	        	PLUS200=(IToken)Match(input,PLUS,FOLLOW_PLUS_in_additiveExpression2439); 
            			    	        		PLUS200_tree = (IASTNode)adaptor.Create(PLUS200);
            			    	        		root_0 = (IASTNode)adaptor.BecomeRoot(PLUS200_tree, root_0);


            			    	        }
            			    	        break;
            			    	    case 2 :
            			    	        // Hql.g:500:35: MINUS
            			    	        {
            			    	        	MINUS201=(IToken)Match(input,MINUS,FOLLOW_MINUS_in_additiveExpression2444); 
            			    	        		MINUS201_tree = (IASTNode)adaptor.Create(MINUS201);
            			    	        		root_0 = (IASTNode)adaptor.BecomeRoot(MINUS201_tree, root_0);


            			    	        }
            			    	        break;

            			    	}

            			    	PushFollow(FOLLOW_multiplyExpression_in_additiveExpression2449);
            			    	multiplyExpression202 = multiplyExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, multiplyExpression202.Tree);

            			    }
            			    break;

            			default:
            			    goto loop71;
            	    }
            	} while (true);

            	loop71:
            		;	// Stops C# compiler whining that label 'loop71' has no statements


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
    // Hql.g:504:1: multiplyExpression : unaryExpression ( ( STAR | DIV ) unaryExpression )* ;
    public HqlParser.multiplyExpression_return multiplyExpression() // throws RecognitionException [1]
    {   
        HqlParser.multiplyExpression_return retval = new HqlParser.multiplyExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken STAR204 = null;
        IToken DIV205 = null;
        HqlParser.unaryExpression_return unaryExpression203 = default(HqlParser.unaryExpression_return);

        HqlParser.unaryExpression_return unaryExpression206 = default(HqlParser.unaryExpression_return);


        IASTNode STAR204_tree=null;
        IASTNode DIV205_tree=null;

        try 
    	{
            // Hql.g:505:2: ( unaryExpression ( ( STAR | DIV ) unaryExpression )* )
            // Hql.g:505:4: unaryExpression ( ( STAR | DIV ) unaryExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_unaryExpression_in_multiplyExpression2464);
            	unaryExpression203 = unaryExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, unaryExpression203.Tree);
            	// Hql.g:505:20: ( ( STAR | DIV ) unaryExpression )*
            	do 
            	{
            	    int alt73 = 2;
            	    int LA73_0 = input.LA(1);

            	    if ( ((LA73_0 >= STAR && LA73_0 <= DIV)) )
            	    {
            	        alt73 = 1;
            	    }


            	    switch (alt73) 
            		{
            			case 1 :
            			    // Hql.g:505:22: ( STAR | DIV ) unaryExpression
            			    {
            			    	// Hql.g:505:22: ( STAR | DIV )
            			    	int alt72 = 2;
            			    	int LA72_0 = input.LA(1);

            			    	if ( (LA72_0 == STAR) )
            			    	{
            			    	    alt72 = 1;
            			    	}
            			    	else if ( (LA72_0 == DIV) )
            			    	{
            			    	    alt72 = 2;
            			    	}
            			    	else 
            			    	{
            			    	    NoViableAltException nvae_d72s0 =
            			    	        new NoViableAltException("", 72, 0, input);

            			    	    throw nvae_d72s0;
            			    	}
            			    	switch (alt72) 
            			    	{
            			    	    case 1 :
            			    	        // Hql.g:505:24: STAR
            			    	        {
            			    	        	STAR204=(IToken)Match(input,STAR,FOLLOW_STAR_in_multiplyExpression2470); 
            			    	        		STAR204_tree = (IASTNode)adaptor.Create(STAR204);
            			    	        		root_0 = (IASTNode)adaptor.BecomeRoot(STAR204_tree, root_0);


            			    	        }
            			    	        break;
            			    	    case 2 :
            			    	        // Hql.g:505:32: DIV
            			    	        {
            			    	        	DIV205=(IToken)Match(input,DIV,FOLLOW_DIV_in_multiplyExpression2475); 
            			    	        		DIV205_tree = (IASTNode)adaptor.Create(DIV205);
            			    	        		root_0 = (IASTNode)adaptor.BecomeRoot(DIV205_tree, root_0);


            			    	        }
            			    	        break;

            			    	}

            			    	PushFollow(FOLLOW_unaryExpression_in_multiplyExpression2480);
            			    	unaryExpression206 = unaryExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, unaryExpression206.Tree);

            			    }
            			    break;

            			default:
            			    goto loop73;
            	    }
            	} while (true);

            	loop73:
            		;	// Stops C# compiler whining that label 'loop73' has no statements


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
    // Hql.g:509:1: unaryExpression : (m= MINUS mu= unaryExpression -> ^( UNARY_MINUS[$m] $mu) | p= PLUS pu= unaryExpression -> ^( UNARY_PLUS[$p] $pu) | c= caseExpression -> ^( $c) | q= quantifiedExpression -> ^( $q) | a= atom -> ^( $a) );
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
            // Hql.g:510:2: (m= MINUS mu= unaryExpression -> ^( UNARY_MINUS[$m] $mu) | p= PLUS pu= unaryExpression -> ^( UNARY_PLUS[$p] $pu) | c= caseExpression -> ^( $c) | q= quantifiedExpression -> ^( $q) | a= atom -> ^( $a) )
            int alt74 = 5;
            switch ( input.LA(1) ) 
            {
            case MINUS:
            	{
                alt74 = 1;
                }
                break;
            case PLUS:
            	{
                alt74 = 2;
                }
                break;
            case CASE:
            	{
                alt74 = 3;
                }
                break;
            case ALL:
            case ANY:
            case EXISTS:
            case SOME:
            	{
                alt74 = 4;
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
                alt74 = 5;
                }
                break;
            	default:
            	    NoViableAltException nvae_d74s0 =
            	        new NoViableAltException("", 74, 0, input);

            	    throw nvae_d74s0;
            }

            switch (alt74) 
            {
                case 1 :
                    // Hql.g:510:4: m= MINUS mu= unaryExpression
                    {
                    	m=(IToken)Match(input,MINUS,FOLLOW_MINUS_in_unaryExpression2498);  
                    	stream_MINUS.Add(m);

                    	PushFollow(FOLLOW_unaryExpression_in_unaryExpression2502);
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
                    	// 510:31: -> ^( UNARY_MINUS[$m] $mu)
                    	{
                    	    // Hql.g:510:34: ^( UNARY_MINUS[$m] $mu)
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
                    // Hql.g:511:4: p= PLUS pu= unaryExpression
                    {
                    	p=(IToken)Match(input,PLUS,FOLLOW_PLUS_in_unaryExpression2519);  
                    	stream_PLUS.Add(p);

                    	PushFollow(FOLLOW_unaryExpression_in_unaryExpression2523);
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
                    	// 511:30: -> ^( UNARY_PLUS[$p] $pu)
                    	{
                    	    // Hql.g:511:33: ^( UNARY_PLUS[$p] $pu)
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
                    // Hql.g:512:4: c= caseExpression
                    {
                    	PushFollow(FOLLOW_caseExpression_in_unaryExpression2540);
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
                    	// 512:21: -> ^( $c)
                    	{
                    	    // Hql.g:512:24: ^( $c)
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
                    // Hql.g:513:4: q= quantifiedExpression
                    {
                    	PushFollow(FOLLOW_quantifiedExpression_in_unaryExpression2554);
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
                    	// 513:27: -> ^( $q)
                    	{
                    	    // Hql.g:513:30: ^( $q)
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
                    // Hql.g:514:4: a= atom
                    {
                    	PushFollow(FOLLOW_atom_in_unaryExpression2569);
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
                    	// 514:11: -> ^( $a)
                    	{
                    	    // Hql.g:514:14: ^( $a)
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
    // Hql.g:517:1: caseExpression : ( CASE ( whenClause )+ ( elseClause )? END -> ^( CASE ( whenClause )+ ( elseClause )? ) | CASE unaryExpression ( altWhenClause )+ ( elseClause )? END -> ^( CASE2 unaryExpression ( altWhenClause )+ ( elseClause )? ) );
    public HqlParser.caseExpression_return caseExpression() // throws RecognitionException [1]
    {   
        HqlParser.caseExpression_return retval = new HqlParser.caseExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken CASE207 = null;
        IToken END210 = null;
        IToken CASE211 = null;
        IToken END215 = null;
        HqlParser.whenClause_return whenClause208 = default(HqlParser.whenClause_return);

        HqlParser.elseClause_return elseClause209 = default(HqlParser.elseClause_return);

        HqlParser.unaryExpression_return unaryExpression212 = default(HqlParser.unaryExpression_return);

        HqlParser.altWhenClause_return altWhenClause213 = default(HqlParser.altWhenClause_return);

        HqlParser.elseClause_return elseClause214 = default(HqlParser.elseClause_return);


        IASTNode CASE207_tree=null;
        IASTNode END210_tree=null;
        IASTNode CASE211_tree=null;
        IASTNode END215_tree=null;
        RewriteRuleTokenStream stream_END = new RewriteRuleTokenStream(adaptor,"token END");
        RewriteRuleTokenStream stream_CASE = new RewriteRuleTokenStream(adaptor,"token CASE");
        RewriteRuleSubtreeStream stream_whenClause = new RewriteRuleSubtreeStream(adaptor,"rule whenClause");
        RewriteRuleSubtreeStream stream_unaryExpression = new RewriteRuleSubtreeStream(adaptor,"rule unaryExpression");
        RewriteRuleSubtreeStream stream_altWhenClause = new RewriteRuleSubtreeStream(adaptor,"rule altWhenClause");
        RewriteRuleSubtreeStream stream_elseClause = new RewriteRuleSubtreeStream(adaptor,"rule elseClause");
        try 
    	{
            // Hql.g:518:2: ( CASE ( whenClause )+ ( elseClause )? END -> ^( CASE ( whenClause )+ ( elseClause )? ) | CASE unaryExpression ( altWhenClause )+ ( elseClause )? END -> ^( CASE2 unaryExpression ( altWhenClause )+ ( elseClause )? ) )
            int alt79 = 2;
            int LA79_0 = input.LA(1);

            if ( (LA79_0 == CASE) )
            {
                int LA79_1 = input.LA(2);

                if ( ((LA79_1 >= ALL && LA79_1 <= ANY) || LA79_1 == AVG || LA79_1 == COUNT || LA79_1 == ELEMENTS || (LA79_1 >= EXISTS && LA79_1 <= FALSE) || LA79_1 == INDICES || (LA79_1 >= MAX && LA79_1 <= MIN) || LA79_1 == NULL || (LA79_1 >= SOME && LA79_1 <= SUM) || LA79_1 == TRUE || LA79_1 == CASE || LA79_1 == EMPTY || (LA79_1 >= NUM_INT && LA79_1 <= NUM_LONG) || LA79_1 == OPEN || (LA79_1 >= COLON && LA79_1 <= PARAM) || (LA79_1 >= PLUS && LA79_1 <= MINUS) || (LA79_1 >= QUOTED_String && LA79_1 <= IDENT)) )
                {
                    alt79 = 2;
                }
                else if ( (LA79_1 == WHEN) )
                {
                    alt79 = 1;
                }
                else 
                {
                    NoViableAltException nvae_d79s1 =
                        new NoViableAltException("", 79, 1, input);

                    throw nvae_d79s1;
                }
            }
            else 
            {
                NoViableAltException nvae_d79s0 =
                    new NoViableAltException("", 79, 0, input);

                throw nvae_d79s0;
            }
            switch (alt79) 
            {
                case 1 :
                    // Hql.g:518:4: CASE ( whenClause )+ ( elseClause )? END
                    {
                    	CASE207=(IToken)Match(input,CASE,FOLLOW_CASE_in_caseExpression2588);  
                    	stream_CASE.Add(CASE207);

                    	// Hql.g:518:9: ( whenClause )+
                    	int cnt75 = 0;
                    	do 
                    	{
                    	    int alt75 = 2;
                    	    int LA75_0 = input.LA(1);

                    	    if ( (LA75_0 == WHEN) )
                    	    {
                    	        alt75 = 1;
                    	    }


                    	    switch (alt75) 
                    		{
                    			case 1 :
                    			    // Hql.g:518:10: whenClause
                    			    {
                    			    	PushFollow(FOLLOW_whenClause_in_caseExpression2591);
                    			    	whenClause208 = whenClause();
                    			    	state.followingStackPointer--;

                    			    	stream_whenClause.Add(whenClause208.Tree);

                    			    }
                    			    break;

                    			default:
                    			    if ( cnt75 >= 1 ) goto loop75;
                    		            EarlyExitException eee75 =
                    		                new EarlyExitException(75, input);
                    		            throw eee75;
                    	    }
                    	    cnt75++;
                    	} while (true);

                    	loop75:
                    		;	// Stops C# compiler whining that label 'loop75' has no statements

                    	// Hql.g:518:23: ( elseClause )?
                    	int alt76 = 2;
                    	int LA76_0 = input.LA(1);

                    	if ( (LA76_0 == ELSE) )
                    	{
                    	    alt76 = 1;
                    	}
                    	switch (alt76) 
                    	{
                    	    case 1 :
                    	        // Hql.g:518:24: elseClause
                    	        {
                    	        	PushFollow(FOLLOW_elseClause_in_caseExpression2596);
                    	        	elseClause209 = elseClause();
                    	        	state.followingStackPointer--;

                    	        	stream_elseClause.Add(elseClause209.Tree);

                    	        }
                    	        break;

                    	}

                    	END210=(IToken)Match(input,END,FOLLOW_END_in_caseExpression2600);  
                    	stream_END.Add(END210);



                    	// AST REWRITE
                    	// elements:          whenClause, CASE, elseClause
                    	// token labels:      
                    	// rule labels:       retval
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

                    	root_0 = (IASTNode)adaptor.GetNilNode();
                    	// 519:3: -> ^( CASE ( whenClause )+ ( elseClause )? )
                    	{
                    	    // Hql.g:519:6: ^( CASE ( whenClause )+ ( elseClause )? )
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
                    	    // Hql.g:519:25: ( elseClause )?
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
                    // Hql.g:520:4: CASE unaryExpression ( altWhenClause )+ ( elseClause )? END
                    {
                    	CASE211=(IToken)Match(input,CASE,FOLLOW_CASE_in_caseExpression2620);  
                    	stream_CASE.Add(CASE211);

                    	PushFollow(FOLLOW_unaryExpression_in_caseExpression2622);
                    	unaryExpression212 = unaryExpression();
                    	state.followingStackPointer--;

                    	stream_unaryExpression.Add(unaryExpression212.Tree);
                    	// Hql.g:520:25: ( altWhenClause )+
                    	int cnt77 = 0;
                    	do 
                    	{
                    	    int alt77 = 2;
                    	    int LA77_0 = input.LA(1);

                    	    if ( (LA77_0 == WHEN) )
                    	    {
                    	        alt77 = 1;
                    	    }


                    	    switch (alt77) 
                    		{
                    			case 1 :
                    			    // Hql.g:520:26: altWhenClause
                    			    {
                    			    	PushFollow(FOLLOW_altWhenClause_in_caseExpression2625);
                    			    	altWhenClause213 = altWhenClause();
                    			    	state.followingStackPointer--;

                    			    	stream_altWhenClause.Add(altWhenClause213.Tree);

                    			    }
                    			    break;

                    			default:
                    			    if ( cnt77 >= 1 ) goto loop77;
                    		            EarlyExitException eee77 =
                    		                new EarlyExitException(77, input);
                    		            throw eee77;
                    	    }
                    	    cnt77++;
                    	} while (true);

                    	loop77:
                    		;	// Stops C# compiler whining that label 'loop77' has no statements

                    	// Hql.g:520:42: ( elseClause )?
                    	int alt78 = 2;
                    	int LA78_0 = input.LA(1);

                    	if ( (LA78_0 == ELSE) )
                    	{
                    	    alt78 = 1;
                    	}
                    	switch (alt78) 
                    	{
                    	    case 1 :
                    	        // Hql.g:520:43: elseClause
                    	        {
                    	        	PushFollow(FOLLOW_elseClause_in_caseExpression2630);
                    	        	elseClause214 = elseClause();
                    	        	state.followingStackPointer--;

                    	        	stream_elseClause.Add(elseClause214.Tree);

                    	        }
                    	        break;

                    	}

                    	END215=(IToken)Match(input,END,FOLLOW_END_in_caseExpression2634);  
                    	stream_END.Add(END215);



                    	// AST REWRITE
                    	// elements:          elseClause, altWhenClause, unaryExpression
                    	// token labels:      
                    	// rule labels:       retval
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

                    	root_0 = (IASTNode)adaptor.GetNilNode();
                    	// 521:3: -> ^( CASE2 unaryExpression ( altWhenClause )+ ( elseClause )? )
                    	{
                    	    // Hql.g:521:6: ^( CASE2 unaryExpression ( altWhenClause )+ ( elseClause )? )
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
                    	    // Hql.g:521:45: ( elseClause )?
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
    // Hql.g:524:1: whenClause : ( WHEN logicalExpression THEN expression ) ;
    public HqlParser.whenClause_return whenClause() // throws RecognitionException [1]
    {   
        HqlParser.whenClause_return retval = new HqlParser.whenClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken WHEN216 = null;
        IToken THEN218 = null;
        HqlParser.logicalExpression_return logicalExpression217 = default(HqlParser.logicalExpression_return);

        HqlParser.expression_return expression219 = default(HqlParser.expression_return);


        IASTNode WHEN216_tree=null;
        IASTNode THEN218_tree=null;

        try 
    	{
            // Hql.g:525:2: ( ( WHEN logicalExpression THEN expression ) )
            // Hql.g:525:4: ( WHEN logicalExpression THEN expression )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// Hql.g:525:4: ( WHEN logicalExpression THEN expression )
            	// Hql.g:525:5: WHEN logicalExpression THEN expression
            	{
            		WHEN216=(IToken)Match(input,WHEN,FOLLOW_WHEN_in_whenClause2663); 
            			WHEN216_tree = (IASTNode)adaptor.Create(WHEN216);
            			root_0 = (IASTNode)adaptor.BecomeRoot(WHEN216_tree, root_0);

            		PushFollow(FOLLOW_logicalExpression_in_whenClause2666);
            		logicalExpression217 = logicalExpression();
            		state.followingStackPointer--;

            		adaptor.AddChild(root_0, logicalExpression217.Tree);
            		THEN218=(IToken)Match(input,THEN,FOLLOW_THEN_in_whenClause2668); 
            		PushFollow(FOLLOW_expression_in_whenClause2671);
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
    // Hql.g:528:1: altWhenClause : ( WHEN unaryExpression THEN expression ) ;
    public HqlParser.altWhenClause_return altWhenClause() // throws RecognitionException [1]
    {   
        HqlParser.altWhenClause_return retval = new HqlParser.altWhenClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken WHEN220 = null;
        IToken THEN222 = null;
        HqlParser.unaryExpression_return unaryExpression221 = default(HqlParser.unaryExpression_return);

        HqlParser.expression_return expression223 = default(HqlParser.expression_return);


        IASTNode WHEN220_tree=null;
        IASTNode THEN222_tree=null;

        try 
    	{
            // Hql.g:529:2: ( ( WHEN unaryExpression THEN expression ) )
            // Hql.g:529:4: ( WHEN unaryExpression THEN expression )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// Hql.g:529:4: ( WHEN unaryExpression THEN expression )
            	// Hql.g:529:5: WHEN unaryExpression THEN expression
            	{
            		WHEN220=(IToken)Match(input,WHEN,FOLLOW_WHEN_in_altWhenClause2685); 
            			WHEN220_tree = (IASTNode)adaptor.Create(WHEN220);
            			root_0 = (IASTNode)adaptor.BecomeRoot(WHEN220_tree, root_0);

            		PushFollow(FOLLOW_unaryExpression_in_altWhenClause2688);
            		unaryExpression221 = unaryExpression();
            		state.followingStackPointer--;

            		adaptor.AddChild(root_0, unaryExpression221.Tree);
            		THEN222=(IToken)Match(input,THEN,FOLLOW_THEN_in_altWhenClause2690); 
            		PushFollow(FOLLOW_expression_in_altWhenClause2693);
            		expression223 = expression();
            		state.followingStackPointer--;

            		adaptor.AddChild(root_0, expression223.Tree);

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
    // Hql.g:532:1: elseClause : ( ELSE expression ) ;
    public HqlParser.elseClause_return elseClause() // throws RecognitionException [1]
    {   
        HqlParser.elseClause_return retval = new HqlParser.elseClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken ELSE224 = null;
        HqlParser.expression_return expression225 = default(HqlParser.expression_return);


        IASTNode ELSE224_tree=null;

        try 
    	{
            // Hql.g:533:2: ( ( ELSE expression ) )
            // Hql.g:533:4: ( ELSE expression )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// Hql.g:533:4: ( ELSE expression )
            	// Hql.g:533:5: ELSE expression
            	{
            		ELSE224=(IToken)Match(input,ELSE,FOLLOW_ELSE_in_elseClause2707); 
            			ELSE224_tree = (IASTNode)adaptor.Create(ELSE224);
            			root_0 = (IASTNode)adaptor.BecomeRoot(ELSE224_tree, root_0);

            		PushFollow(FOLLOW_expression_in_elseClause2710);
            		expression225 = expression();
            		state.followingStackPointer--;

            		adaptor.AddChild(root_0, expression225.Tree);

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
    // Hql.g:536:1: quantifiedExpression : ( SOME | EXISTS | ALL | ANY ) ( identifier | collectionExpr | ( OPEN ( subQuery ) CLOSE ) ) ;
    public HqlParser.quantifiedExpression_return quantifiedExpression() // throws RecognitionException [1]
    {   
        HqlParser.quantifiedExpression_return retval = new HqlParser.quantifiedExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken SOME226 = null;
        IToken EXISTS227 = null;
        IToken ALL228 = null;
        IToken ANY229 = null;
        IToken OPEN232 = null;
        IToken CLOSE234 = null;
        HqlParser.identifier_return identifier230 = default(HqlParser.identifier_return);

        HqlParser.collectionExpr_return collectionExpr231 = default(HqlParser.collectionExpr_return);

        HqlParser.subQuery_return subQuery233 = default(HqlParser.subQuery_return);


        IASTNode SOME226_tree=null;
        IASTNode EXISTS227_tree=null;
        IASTNode ALL228_tree=null;
        IASTNode ANY229_tree=null;
        IASTNode OPEN232_tree=null;
        IASTNode CLOSE234_tree=null;

        try 
    	{
            // Hql.g:537:2: ( ( SOME | EXISTS | ALL | ANY ) ( identifier | collectionExpr | ( OPEN ( subQuery ) CLOSE ) ) )
            // Hql.g:537:4: ( SOME | EXISTS | ALL | ANY ) ( identifier | collectionExpr | ( OPEN ( subQuery ) CLOSE ) )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// Hql.g:537:4: ( SOME | EXISTS | ALL | ANY )
            	int alt80 = 4;
            	switch ( input.LA(1) ) 
            	{
            	case SOME:
            		{
            	    alt80 = 1;
            	    }
            	    break;
            	case EXISTS:
            		{
            	    alt80 = 2;
            	    }
            	    break;
            	case ALL:
            		{
            	    alt80 = 3;
            	    }
            	    break;
            	case ANY:
            		{
            	    alt80 = 4;
            	    }
            	    break;
            		default:
            		    NoViableAltException nvae_d80s0 =
            		        new NoViableAltException("", 80, 0, input);

            		    throw nvae_d80s0;
            	}

            	switch (alt80) 
            	{
            	    case 1 :
            	        // Hql.g:537:6: SOME
            	        {
            	        	SOME226=(IToken)Match(input,SOME,FOLLOW_SOME_in_quantifiedExpression2725); 
            	        		SOME226_tree = (IASTNode)adaptor.Create(SOME226);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(SOME226_tree, root_0);


            	        }
            	        break;
            	    case 2 :
            	        // Hql.g:537:14: EXISTS
            	        {
            	        	EXISTS227=(IToken)Match(input,EXISTS,FOLLOW_EXISTS_in_quantifiedExpression2730); 
            	        		EXISTS227_tree = (IASTNode)adaptor.Create(EXISTS227);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(EXISTS227_tree, root_0);


            	        }
            	        break;
            	    case 3 :
            	        // Hql.g:537:24: ALL
            	        {
            	        	ALL228=(IToken)Match(input,ALL,FOLLOW_ALL_in_quantifiedExpression2735); 
            	        		ALL228_tree = (IASTNode)adaptor.Create(ALL228);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(ALL228_tree, root_0);


            	        }
            	        break;
            	    case 4 :
            	        // Hql.g:537:31: ANY
            	        {
            	        	ANY229=(IToken)Match(input,ANY,FOLLOW_ANY_in_quantifiedExpression2740); 
            	        		ANY229_tree = (IASTNode)adaptor.Create(ANY229);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(ANY229_tree, root_0);


            	        }
            	        break;

            	}

            	// Hql.g:538:2: ( identifier | collectionExpr | ( OPEN ( subQuery ) CLOSE ) )
            	int alt81 = 3;
            	switch ( input.LA(1) ) 
            	{
            	case IDENT:
            		{
            	    alt81 = 1;
            	    }
            	    break;
            	case ELEMENTS:
            	case INDICES:
            		{
            	    alt81 = 2;
            	    }
            	    break;
            	case OPEN:
            		{
            	    alt81 = 3;
            	    }
            	    break;
            		default:
            		    NoViableAltException nvae_d81s0 =
            		        new NoViableAltException("", 81, 0, input);

            		    throw nvae_d81s0;
            	}

            	switch (alt81) 
            	{
            	    case 1 :
            	        // Hql.g:538:4: identifier
            	        {
            	        	PushFollow(FOLLOW_identifier_in_quantifiedExpression2749);
            	        	identifier230 = identifier();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, identifier230.Tree);

            	        }
            	        break;
            	    case 2 :
            	        // Hql.g:538:17: collectionExpr
            	        {
            	        	PushFollow(FOLLOW_collectionExpr_in_quantifiedExpression2753);
            	        	collectionExpr231 = collectionExpr();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, collectionExpr231.Tree);

            	        }
            	        break;
            	    case 3 :
            	        // Hql.g:538:34: ( OPEN ( subQuery ) CLOSE )
            	        {
            	        	// Hql.g:538:34: ( OPEN ( subQuery ) CLOSE )
            	        	// Hql.g:538:35: OPEN ( subQuery ) CLOSE
            	        	{
            	        		OPEN232=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_quantifiedExpression2758); 
            	        		// Hql.g:538:41: ( subQuery )
            	        		// Hql.g:538:43: subQuery
            	        		{
            	        			PushFollow(FOLLOW_subQuery_in_quantifiedExpression2763);
            	        			subQuery233 = subQuery();
            	        			state.followingStackPointer--;

            	        			adaptor.AddChild(root_0, subQuery233.Tree);

            	        		}

            	        		CLOSE234=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_quantifiedExpression2767); 

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
    // Hql.g:544:1: atom : primaryExpression ( DOT identifier ( options {greedy=true; } : (op= OPEN exprList CLOSE ) )? | lb= OPEN_BRACKET expression CLOSE_BRACKET )* ;
    public HqlParser.atom_return atom() // throws RecognitionException [1]
    {   
        HqlParser.atom_return retval = new HqlParser.atom_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken op = null;
        IToken lb = null;
        IToken DOT236 = null;
        IToken CLOSE239 = null;
        IToken CLOSE_BRACKET241 = null;
        HqlParser.primaryExpression_return primaryExpression235 = default(HqlParser.primaryExpression_return);

        HqlParser.identifier_return identifier237 = default(HqlParser.identifier_return);

        HqlParser.exprList_return exprList238 = default(HqlParser.exprList_return);

        HqlParser.expression_return expression240 = default(HqlParser.expression_return);


        IASTNode op_tree=null;
        IASTNode lb_tree=null;
        IASTNode DOT236_tree=null;
        IASTNode CLOSE239_tree=null;
        IASTNode CLOSE_BRACKET241_tree=null;

        try 
    	{
            // Hql.g:545:3: ( primaryExpression ( DOT identifier ( options {greedy=true; } : (op= OPEN exprList CLOSE ) )? | lb= OPEN_BRACKET expression CLOSE_BRACKET )* )
            // Hql.g:545:5: primaryExpression ( DOT identifier ( options {greedy=true; } : (op= OPEN exprList CLOSE ) )? | lb= OPEN_BRACKET expression CLOSE_BRACKET )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_primaryExpression_in_atom2786);
            	primaryExpression235 = primaryExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, primaryExpression235.Tree);
            	// Hql.g:546:3: ( DOT identifier ( options {greedy=true; } : (op= OPEN exprList CLOSE ) )? | lb= OPEN_BRACKET expression CLOSE_BRACKET )*
            	do 
            	{
            	    int alt83 = 3;
            	    int LA83_0 = input.LA(1);

            	    if ( (LA83_0 == DOT) )
            	    {
            	        alt83 = 1;
            	    }
            	    else if ( (LA83_0 == OPEN_BRACKET) )
            	    {
            	        alt83 = 2;
            	    }


            	    switch (alt83) 
            		{
            			case 1 :
            			    // Hql.g:547:4: DOT identifier ( options {greedy=true; } : (op= OPEN exprList CLOSE ) )?
            			    {
            			    	DOT236=(IToken)Match(input,DOT,FOLLOW_DOT_in_atom2795); 
            			    		DOT236_tree = (IASTNode)adaptor.Create(DOT236);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(DOT236_tree, root_0);

            			    	PushFollow(FOLLOW_identifier_in_atom2798);
            			    	identifier237 = identifier();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, identifier237.Tree);
            			    	// Hql.g:548:5: ( options {greedy=true; } : (op= OPEN exprList CLOSE ) )?
            			    	int alt82 = 2;
            			    	int LA82_0 = input.LA(1);

            			    	if ( (LA82_0 == OPEN) )
            			    	{
            			    	    alt82 = 1;
            			    	}
            			    	switch (alt82) 
            			    	{
            			    	    case 1 :
            			    	        // Hql.g:549:6: (op= OPEN exprList CLOSE )
            			    	        {
            			    	        	// Hql.g:549:6: (op= OPEN exprList CLOSE )
            			    	        	// Hql.g:549:8: op= OPEN exprList CLOSE
            			    	        	{
            			    	        		op=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_atom2826); 
            			    	        			op_tree = (IASTNode)adaptor.Create(op);
            			    	        			root_0 = (IASTNode)adaptor.BecomeRoot(op_tree, root_0);

            			    	        		op.Type = METHOD_CALL; 
            			    	        		PushFollow(FOLLOW_exprList_in_atom2831);
            			    	        		exprList238 = exprList();
            			    	        		state.followingStackPointer--;

            			    	        		adaptor.AddChild(root_0, exprList238.Tree);
            			    	        		CLOSE239=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_atom2833); 

            			    	        	}


            			    	        }
            			    	        break;

            			    	}


            			    }
            			    break;
            			case 2 :
            			    // Hql.g:550:5: lb= OPEN_BRACKET expression CLOSE_BRACKET
            			    {
            			    	lb=(IToken)Match(input,OPEN_BRACKET,FOLLOW_OPEN_BRACKET_in_atom2847); 
            			    		lb_tree = (IASTNode)adaptor.Create(lb);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(lb_tree, root_0);

            			    	lb.Type = INDEX_OP; 
            			    	PushFollow(FOLLOW_expression_in_atom2852);
            			    	expression240 = expression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, expression240.Tree);
            			    	CLOSE_BRACKET241=(IToken)Match(input,CLOSE_BRACKET,FOLLOW_CLOSE_BRACKET_in_atom2854); 

            			    }
            			    break;

            			default:
            			    goto loop83;
            	    }
            	} while (true);

            	loop83:
            		;	// Stops C# compiler whining that label 'loop83' has no statements


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
    // Hql.g:555:1: primaryExpression : ( identPrimary ( options {greedy=true; } : DOT 'class' )? | constant | COLON identifier | OPEN ( expressionOrVector | subQuery ) CLOSE | PARAM ( NUM_INT )? );
    public HqlParser.primaryExpression_return primaryExpression() // throws RecognitionException [1]
    {   
        HqlParser.primaryExpression_return retval = new HqlParser.primaryExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken DOT243 = null;
        IToken string_literal244 = null;
        IToken COLON246 = null;
        IToken OPEN248 = null;
        IToken CLOSE251 = null;
        IToken PARAM252 = null;
        IToken NUM_INT253 = null;
        HqlParser.identPrimary_return identPrimary242 = default(HqlParser.identPrimary_return);

        HqlParser.constant_return constant245 = default(HqlParser.constant_return);

        HqlParser.identifier_return identifier247 = default(HqlParser.identifier_return);

        HqlParser.expressionOrVector_return expressionOrVector249 = default(HqlParser.expressionOrVector_return);

        HqlParser.subQuery_return subQuery250 = default(HqlParser.subQuery_return);


        IASTNode DOT243_tree=null;
        IASTNode string_literal244_tree=null;
        IASTNode COLON246_tree=null;
        IASTNode OPEN248_tree=null;
        IASTNode CLOSE251_tree=null;
        IASTNode PARAM252_tree=null;
        IASTNode NUM_INT253_tree=null;

        try 
    	{
            // Hql.g:556:2: ( identPrimary ( options {greedy=true; } : DOT 'class' )? | constant | COLON identifier | OPEN ( expressionOrVector | subQuery ) CLOSE | PARAM ( NUM_INT )? )
            int alt87 = 5;
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
                alt87 = 1;
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
                alt87 = 2;
                }
                break;
            case COLON:
            	{
                alt87 = 3;
                }
                break;
            case OPEN:
            	{
                alt87 = 4;
                }
                break;
            case PARAM:
            	{
                alt87 = 5;
                }
                break;
            	default:
            	    NoViableAltException nvae_d87s0 =
            	        new NoViableAltException("", 87, 0, input);

            	    throw nvae_d87s0;
            }

            switch (alt87) 
            {
                case 1 :
                    // Hql.g:556:6: identPrimary ( options {greedy=true; } : DOT 'class' )?
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_identPrimary_in_primaryExpression2874);
                    	identPrimary242 = identPrimary();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, identPrimary242.Tree);
                    	// Hql.g:556:19: ( options {greedy=true; } : DOT 'class' )?
                    	int alt84 = 2;
                    	int LA84_0 = input.LA(1);

                    	if ( (LA84_0 == DOT) )
                    	{
                    	    int LA84_1 = input.LA(2);

                    	    if ( (LA84_1 == CLASS) )
                    	    {
                    	        alt84 = 1;
                    	    }
                    	}
                    	switch (alt84) 
                    	{
                    	    case 1 :
                    	        // Hql.g:556:46: DOT 'class'
                    	        {
                    	        	DOT243=(IToken)Match(input,DOT,FOLLOW_DOT_in_primaryExpression2887); 
                    	        		DOT243_tree = (IASTNode)adaptor.Create(DOT243);
                    	        		root_0 = (IASTNode)adaptor.BecomeRoot(DOT243_tree, root_0);

                    	        	string_literal244=(IToken)Match(input,CLASS,FOLLOW_CLASS_in_primaryExpression2890); 
                    	        		string_literal244_tree = (IASTNode)adaptor.Create(string_literal244);
                    	        		adaptor.AddChild(root_0, string_literal244_tree);


                    	        }
                    	        break;

                    	}


                    }
                    break;
                case 2 :
                    // Hql.g:557:6: constant
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_constant_in_primaryExpression2900);
                    	constant245 = constant();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, constant245.Tree);

                    }
                    break;
                case 3 :
                    // Hql.g:558:6: COLON identifier
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	COLON246=(IToken)Match(input,COLON,FOLLOW_COLON_in_primaryExpression2907); 
                    		COLON246_tree = (IASTNode)adaptor.Create(COLON246);
                    		root_0 = (IASTNode)adaptor.BecomeRoot(COLON246_tree, root_0);

                    	PushFollow(FOLLOW_identifier_in_primaryExpression2910);
                    	identifier247 = identifier();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, identifier247.Tree);

                    }
                    break;
                case 4 :
                    // Hql.g:560:6: OPEN ( expressionOrVector | subQuery ) CLOSE
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	OPEN248=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_primaryExpression2919); 
                    	// Hql.g:560:12: ( expressionOrVector | subQuery )
                    	int alt85 = 2;
                    	int LA85_0 = input.LA(1);

                    	if ( ((LA85_0 >= ALL && LA85_0 <= ANY) || LA85_0 == AVG || LA85_0 == COUNT || LA85_0 == ELEMENTS || (LA85_0 >= EXISTS && LA85_0 <= FALSE) || LA85_0 == INDICES || (LA85_0 >= MAX && LA85_0 <= MIN) || (LA85_0 >= NOT && LA85_0 <= NULL) || (LA85_0 >= SOME && LA85_0 <= SUM) || LA85_0 == TRUE || LA85_0 == CASE || LA85_0 == EMPTY || (LA85_0 >= NUM_INT && LA85_0 <= NUM_LONG) || LA85_0 == OPEN || (LA85_0 >= COLON && LA85_0 <= PARAM) || LA85_0 == BNOT || (LA85_0 >= PLUS && LA85_0 <= MINUS) || (LA85_0 >= QUOTED_String && LA85_0 <= IDENT)) )
                    	{
                    	    alt85 = 1;
                    	}
                    	else if ( (LA85_0 == EOF || LA85_0 == FROM || (LA85_0 >= GROUP && LA85_0 <= HAVING) || LA85_0 == ORDER || LA85_0 == SELECT || LA85_0 == SKIP || LA85_0 == TAKE || LA85_0 == UNION || LA85_0 == WHERE || LA85_0 == CLOSE) )
                    	{
                    	    alt85 = 2;
                    	}
                    	else 
                    	{
                    	    NoViableAltException nvae_d85s0 =
                    	        new NoViableAltException("", 85, 0, input);

                    	    throw nvae_d85s0;
                    	}
                    	switch (alt85) 
                    	{
                    	    case 1 :
                    	        // Hql.g:560:13: expressionOrVector
                    	        {
                    	        	PushFollow(FOLLOW_expressionOrVector_in_primaryExpression2923);
                    	        	expressionOrVector249 = expressionOrVector();
                    	        	state.followingStackPointer--;

                    	        	adaptor.AddChild(root_0, expressionOrVector249.Tree);

                    	        }
                    	        break;
                    	    case 2 :
                    	        // Hql.g:560:34: subQuery
                    	        {
                    	        	PushFollow(FOLLOW_subQuery_in_primaryExpression2927);
                    	        	subQuery250 = subQuery();
                    	        	state.followingStackPointer--;

                    	        	adaptor.AddChild(root_0, subQuery250.Tree);

                    	        }
                    	        break;

                    	}

                    	CLOSE251=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_primaryExpression2930); 

                    }
                    break;
                case 5 :
                    // Hql.g:561:6: PARAM ( NUM_INT )?
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PARAM252=(IToken)Match(input,PARAM,FOLLOW_PARAM_in_primaryExpression2938); 
                    		PARAM252_tree = (IASTNode)adaptor.Create(PARAM252);
                    		root_0 = (IASTNode)adaptor.BecomeRoot(PARAM252_tree, root_0);

                    	// Hql.g:561:13: ( NUM_INT )?
                    	int alt86 = 2;
                    	int LA86_0 = input.LA(1);

                    	if ( (LA86_0 == NUM_INT) )
                    	{
                    	    alt86 = 1;
                    	}
                    	switch (alt86) 
                    	{
                    	    case 1 :
                    	        // Hql.g:561:14: NUM_INT
                    	        {
                    	        	NUM_INT253=(IToken)Match(input,NUM_INT,FOLLOW_NUM_INT_in_primaryExpression2942); 
                    	        		NUM_INT253_tree = (IASTNode)adaptor.Create(NUM_INT253);
                    	        		adaptor.AddChild(root_0, NUM_INT253_tree);


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
    // Hql.g:566:1: expressionOrVector : e= expression (v= vectorExpr )? -> {v != null}? ^( VECTOR_EXPR[\"{vector}\"] $e $v) -> ^( $e) ;
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
            // Hql.g:567:2: (e= expression (v= vectorExpr )? -> {v != null}? ^( VECTOR_EXPR[\"{vector}\"] $e $v) -> ^( $e) )
            // Hql.g:567:4: e= expression (v= vectorExpr )?
            {
            	PushFollow(FOLLOW_expression_in_expressionOrVector2960);
            	e = expression();
            	state.followingStackPointer--;

            	stream_expression.Add(e.Tree);
            	// Hql.g:567:17: (v= vectorExpr )?
            	int alt88 = 2;
            	int LA88_0 = input.LA(1);

            	if ( (LA88_0 == COMMA) )
            	{
            	    alt88 = 1;
            	}
            	switch (alt88) 
            	{
            	    case 1 :
            	        // Hql.g:567:19: v= vectorExpr
            	        {
            	        	PushFollow(FOLLOW_vectorExpr_in_expressionOrVector2966);
            	        	v = vectorExpr();
            	        	state.followingStackPointer--;

            	        	stream_vectorExpr.Add(v.Tree);

            	        }
            	        break;

            	}



            	// AST REWRITE
            	// elements:          v, e, e
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
            	// 568:2: -> {v != null}? ^( VECTOR_EXPR[\"{vector}\"] $e $v)
            	if (v != null)
            	{
            	    // Hql.g:568:18: ^( VECTOR_EXPR[\"{vector}\"] $e $v)
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(VECTOR_EXPR, "{vector}"), root_1);

            	    adaptor.AddChild(root_1, stream_e.NextTree());
            	    adaptor.AddChild(root_1, stream_v.NextTree());

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}
            	else // 569:2: -> ^( $e)
            	{
            	    // Hql.g:569:5: ^( $e)
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
    // Hql.g:572:1: vectorExpr : COMMA expression ( COMMA expression )* ;
    public HqlParser.vectorExpr_return vectorExpr() // throws RecognitionException [1]
    {   
        HqlParser.vectorExpr_return retval = new HqlParser.vectorExpr_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken COMMA254 = null;
        IToken COMMA256 = null;
        HqlParser.expression_return expression255 = default(HqlParser.expression_return);

        HqlParser.expression_return expression257 = default(HqlParser.expression_return);


        IASTNode COMMA254_tree=null;
        IASTNode COMMA256_tree=null;

        try 
    	{
            // Hql.g:573:2: ( COMMA expression ( COMMA expression )* )
            // Hql.g:573:4: COMMA expression ( COMMA expression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	COMMA254=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_vectorExpr3005); 
            	PushFollow(FOLLOW_expression_in_vectorExpr3008);
            	expression255 = expression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, expression255.Tree);
            	// Hql.g:573:22: ( COMMA expression )*
            	do 
            	{
            	    int alt89 = 2;
            	    int LA89_0 = input.LA(1);

            	    if ( (LA89_0 == COMMA) )
            	    {
            	        alt89 = 1;
            	    }


            	    switch (alt89) 
            		{
            			case 1 :
            			    // Hql.g:573:23: COMMA expression
            			    {
            			    	COMMA256=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_vectorExpr3011); 
            			    	PushFollow(FOLLOW_expression_in_vectorExpr3014);
            			    	expression257 = expression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, expression257.Tree);

            			    }
            			    break;

            			default:
            			    goto loop89;
            	    }
            	} while (true);

            	loop89:
            		;	// Stops C# compiler whining that label 'loop89' has no statements


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
    // Hql.g:579:1: identPrimary : ( identifier ( options {greedy=true; } : DOT ( identifier | o= OBJECT ) )* ( (op= OPEN exprList CLOSE ) )? | aggregate );
    public HqlParser.identPrimary_return identPrimary() // throws RecognitionException [1]
    {   
        HqlParser.identPrimary_return retval = new HqlParser.identPrimary_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken o = null;
        IToken op = null;
        IToken DOT259 = null;
        IToken CLOSE262 = null;
        HqlParser.identifier_return identifier258 = default(HqlParser.identifier_return);

        HqlParser.identifier_return identifier260 = default(HqlParser.identifier_return);

        HqlParser.exprList_return exprList261 = default(HqlParser.exprList_return);

        HqlParser.aggregate_return aggregate263 = default(HqlParser.aggregate_return);


        IASTNode o_tree=null;
        IASTNode op_tree=null;
        IASTNode DOT259_tree=null;
        IASTNode CLOSE262_tree=null;

        try 
    	{
            // Hql.g:580:2: ( identifier ( options {greedy=true; } : DOT ( identifier | o= OBJECT ) )* ( (op= OPEN exprList CLOSE ) )? | aggregate )
            int alt93 = 2;
            int LA93_0 = input.LA(1);

            if ( (LA93_0 == IDENT) )
            {
                alt93 = 1;
            }
            else if ( (LA93_0 == AVG || LA93_0 == COUNT || LA93_0 == ELEMENTS || LA93_0 == INDICES || (LA93_0 >= MAX && LA93_0 <= MIN) || LA93_0 == SUM) )
            {
                alt93 = 2;
            }
            else 
            {
                NoViableAltException nvae_d93s0 =
                    new NoViableAltException("", 93, 0, input);

                throw nvae_d93s0;
            }
            switch (alt93) 
            {
                case 1 :
                    // Hql.g:580:4: identifier ( options {greedy=true; } : DOT ( identifier | o= OBJECT ) )* ( (op= OPEN exprList CLOSE ) )?
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_identifier_in_identPrimary3030);
                    	identifier258 = identifier();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, identifier258.Tree);
                    	 HandleDotIdent(); 
                    	// Hql.g:581:4: ( options {greedy=true; } : DOT ( identifier | o= OBJECT ) )*
                    	do 
                    	{
                    	    int alt91 = 2;
                    	    int LA91_0 = input.LA(1);

                    	    if ( (LA91_0 == DOT) )
                    	    {
                    	        int LA91_2 = input.LA(2);

                    	        if ( (LA91_2 == OBJECT || LA91_2 == IDENT) )
                    	        {
                    	            alt91 = 1;
                    	        }


                    	    }


                    	    switch (alt91) 
                    		{
                    			case 1 :
                    			    // Hql.g:581:31: DOT ( identifier | o= OBJECT )
                    			    {
                    			    	DOT259=(IToken)Match(input,DOT,FOLLOW_DOT_in_identPrimary3048); 
                    			    		DOT259_tree = (IASTNode)adaptor.Create(DOT259);
                    			    		root_0 = (IASTNode)adaptor.BecomeRoot(DOT259_tree, root_0);

                    			    	// Hql.g:581:36: ( identifier | o= OBJECT )
                    			    	int alt90 = 2;
                    			    	int LA90_0 = input.LA(1);

                    			    	if ( (LA90_0 == IDENT) )
                    			    	{
                    			    	    alt90 = 1;
                    			    	}
                    			    	else if ( (LA90_0 == OBJECT) )
                    			    	{
                    			    	    alt90 = 2;
                    			    	}
                    			    	else 
                    			    	{
                    			    	    NoViableAltException nvae_d90s0 =
                    			    	        new NoViableAltException("", 90, 0, input);

                    			    	    throw nvae_d90s0;
                    			    	}
                    			    	switch (alt90) 
                    			    	{
                    			    	    case 1 :
                    			    	        // Hql.g:581:38: identifier
                    			    	        {
                    			    	        	PushFollow(FOLLOW_identifier_in_identPrimary3053);
                    			    	        	identifier260 = identifier();
                    			    	        	state.followingStackPointer--;

                    			    	        	adaptor.AddChild(root_0, identifier260.Tree);

                    			    	        }
                    			    	        break;
                    			    	    case 2 :
                    			    	        // Hql.g:581:51: o= OBJECT
                    			    	        {
                    			    	        	o=(IToken)Match(input,OBJECT,FOLLOW_OBJECT_in_identPrimary3059); 
                    			    	        		o_tree = (IASTNode)adaptor.Create(o);
                    			    	        		adaptor.AddChild(root_0, o_tree);

                    			    	        	 o.Type = IDENT; 

                    			    	        }
                    			    	        break;

                    			    	}


                    			    }
                    			    break;

                    			default:
                    			    goto loop91;
                    	    }
                    	} while (true);

                    	loop91:
                    		;	// Stops C# compiler whining that label 'loop91' has no statements

                    	// Hql.g:582:4: ( (op= OPEN exprList CLOSE ) )?
                    	int alt92 = 2;
                    	int LA92_0 = input.LA(1);

                    	if ( (LA92_0 == OPEN) )
                    	{
                    	    alt92 = 1;
                    	}
                    	switch (alt92) 
                    	{
                    	    case 1 :
                    	        // Hql.g:582:6: (op= OPEN exprList CLOSE )
                    	        {
                    	        	// Hql.g:582:6: (op= OPEN exprList CLOSE )
                    	        	// Hql.g:582:8: op= OPEN exprList CLOSE
                    	        	{
                    	        		op=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_identPrimary3077); 
                    	        			op_tree = (IASTNode)adaptor.Create(op);
                    	        			root_0 = (IASTNode)adaptor.BecomeRoot(op_tree, root_0);

                    	        		 op.Type = METHOD_CALL;
                    	        		PushFollow(FOLLOW_exprList_in_identPrimary3082);
                    	        		exprList261 = exprList();
                    	        		state.followingStackPointer--;

                    	        		adaptor.AddChild(root_0, exprList261.Tree);
                    	        		CLOSE262=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_identPrimary3084); 

                    	        	}


                    	        }
                    	        break;

                    	}


                    }
                    break;
                case 2 :
                    // Hql.g:585:4: aggregate
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_aggregate_in_identPrimary3100);
                    	aggregate263 = aggregate();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, aggregate263.Tree);

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
    // Hql.g:593:1: aggregate : ( (op= SUM | op= AVG | op= MAX | op= MIN ) OPEN additiveExpression CLOSE -> ^( AGGREGATE[$op] additiveExpression ) | COUNT OPEN (s= STAR | p= aggregateDistinctAll ) CLOSE -> {s == null}? ^( COUNT $p) -> ^( COUNT ^( ROW_STAR[\"*\"] ) ) | collectionExpr );
    public HqlParser.aggregate_return aggregate() // throws RecognitionException [1]
    {   
        HqlParser.aggregate_return retval = new HqlParser.aggregate_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken op = null;
        IToken s = null;
        IToken OPEN264 = null;
        IToken CLOSE266 = null;
        IToken COUNT267 = null;
        IToken OPEN268 = null;
        IToken CLOSE269 = null;
        HqlParser.aggregateDistinctAll_return p = default(HqlParser.aggregateDistinctAll_return);

        HqlParser.additiveExpression_return additiveExpression265 = default(HqlParser.additiveExpression_return);

        HqlParser.collectionExpr_return collectionExpr270 = default(HqlParser.collectionExpr_return);


        IASTNode op_tree=null;
        IASTNode s_tree=null;
        IASTNode OPEN264_tree=null;
        IASTNode CLOSE266_tree=null;
        IASTNode COUNT267_tree=null;
        IASTNode OPEN268_tree=null;
        IASTNode CLOSE269_tree=null;
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
            // Hql.g:594:2: ( (op= SUM | op= AVG | op= MAX | op= MIN ) OPEN additiveExpression CLOSE -> ^( AGGREGATE[$op] additiveExpression ) | COUNT OPEN (s= STAR | p= aggregateDistinctAll ) CLOSE -> {s == null}? ^( COUNT $p) -> ^( COUNT ^( ROW_STAR[\"*\"] ) ) | collectionExpr )
            int alt96 = 3;
            switch ( input.LA(1) ) 
            {
            case AVG:
            case MAX:
            case MIN:
            case SUM:
            	{
                alt96 = 1;
                }
                break;
            case COUNT:
            	{
                alt96 = 2;
                }
                break;
            case ELEMENTS:
            case INDICES:
            	{
                alt96 = 3;
                }
                break;
            	default:
            	    NoViableAltException nvae_d96s0 =
            	        new NoViableAltException("", 96, 0, input);

            	    throw nvae_d96s0;
            }

            switch (alt96) 
            {
                case 1 :
                    // Hql.g:594:4: (op= SUM | op= AVG | op= MAX | op= MIN ) OPEN additiveExpression CLOSE
                    {
                    	// Hql.g:594:4: (op= SUM | op= AVG | op= MAX | op= MIN )
                    	int alt94 = 4;
                    	switch ( input.LA(1) ) 
                    	{
                    	case SUM:
                    		{
                    	    alt94 = 1;
                    	    }
                    	    break;
                    	case AVG:
                    		{
                    	    alt94 = 2;
                    	    }
                    	    break;
                    	case MAX:
                    		{
                    	    alt94 = 3;
                    	    }
                    	    break;
                    	case MIN:
                    		{
                    	    alt94 = 4;
                    	    }
                    	    break;
                    		default:
                    		    NoViableAltException nvae_d94s0 =
                    		        new NoViableAltException("", 94, 0, input);

                    		    throw nvae_d94s0;
                    	}

                    	switch (alt94) 
                    	{
                    	    case 1 :
                    	        // Hql.g:594:6: op= SUM
                    	        {
                    	        	op=(IToken)Match(input,SUM,FOLLOW_SUM_in_aggregate3121);  
                    	        	stream_SUM.Add(op);


                    	        }
                    	        break;
                    	    case 2 :
                    	        // Hql.g:594:15: op= AVG
                    	        {
                    	        	op=(IToken)Match(input,AVG,FOLLOW_AVG_in_aggregate3127);  
                    	        	stream_AVG.Add(op);


                    	        }
                    	        break;
                    	    case 3 :
                    	        // Hql.g:594:24: op= MAX
                    	        {
                    	        	op=(IToken)Match(input,MAX,FOLLOW_MAX_in_aggregate3133);  
                    	        	stream_MAX.Add(op);


                    	        }
                    	        break;
                    	    case 4 :
                    	        // Hql.g:594:33: op= MIN
                    	        {
                    	        	op=(IToken)Match(input,MIN,FOLLOW_MIN_in_aggregate3139);  
                    	        	stream_MIN.Add(op);


                    	        }
                    	        break;

                    	}

                    	OPEN264=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_aggregate3143);  
                    	stream_OPEN.Add(OPEN264);

                    	PushFollow(FOLLOW_additiveExpression_in_aggregate3145);
                    	additiveExpression265 = additiveExpression();
                    	state.followingStackPointer--;

                    	stream_additiveExpression.Add(additiveExpression265.Tree);
                    	CLOSE266=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_aggregate3147);  
                    	stream_CLOSE.Add(CLOSE266);



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
                    	// 595:3: -> ^( AGGREGATE[$op] additiveExpression )
                    	{
                    	    // Hql.g:595:6: ^( AGGREGATE[$op] additiveExpression )
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
                    // Hql.g:597:5: COUNT OPEN (s= STAR | p= aggregateDistinctAll ) CLOSE
                    {
                    	COUNT267=(IToken)Match(input,COUNT,FOLLOW_COUNT_in_aggregate3166);  
                    	stream_COUNT.Add(COUNT267);

                    	OPEN268=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_aggregate3168);  
                    	stream_OPEN.Add(OPEN268);

                    	// Hql.g:597:16: (s= STAR | p= aggregateDistinctAll )
                    	int alt95 = 2;
                    	int LA95_0 = input.LA(1);

                    	if ( (LA95_0 == STAR) )
                    	{
                    	    alt95 = 1;
                    	}
                    	else if ( (LA95_0 == ALL || (LA95_0 >= DISTINCT && LA95_0 <= ELEMENTS) || LA95_0 == INDICES || LA95_0 == IDENT) )
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
                    	        // Hql.g:597:18: s= STAR
                    	        {
                    	        	s=(IToken)Match(input,STAR,FOLLOW_STAR_in_aggregate3174);  
                    	        	stream_STAR.Add(s);


                    	        }
                    	        break;
                    	    case 2 :
                    	        // Hql.g:597:27: p= aggregateDistinctAll
                    	        {
                    	        	PushFollow(FOLLOW_aggregateDistinctAll_in_aggregate3180);
                    	        	p = aggregateDistinctAll();
                    	        	state.followingStackPointer--;

                    	        	stream_aggregateDistinctAll.Add(p.Tree);

                    	        }
                    	        break;

                    	}

                    	CLOSE269=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_aggregate3184);  
                    	stream_CLOSE.Add(CLOSE269);



                    	// AST REWRITE
                    	// elements:          COUNT, COUNT, p
                    	// token labels:      
                    	// rule labels:       retval, p
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);
                    	RewriteRuleSubtreeStream stream_p = new RewriteRuleSubtreeStream(adaptor, "rule p", p!=null ? p.Tree : null);

                    	root_0 = (IASTNode)adaptor.GetNilNode();
                    	// 598:3: -> {s == null}? ^( COUNT $p)
                    	if (s == null)
                    	{
                    	    // Hql.g:598:19: ^( COUNT $p)
                    	    {
                    	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
                    	    root_1 = (IASTNode)adaptor.BecomeRoot(stream_COUNT.NextNode(), root_1);

                    	    adaptor.AddChild(root_1, stream_p.NextTree());

                    	    adaptor.AddChild(root_0, root_1);
                    	    }

                    	}
                    	else // 599:3: -> ^( COUNT ^( ROW_STAR[\"*\"] ) )
                    	{
                    	    // Hql.g:599:6: ^( COUNT ^( ROW_STAR[\"*\"] ) )
                    	    {
                    	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
                    	    root_1 = (IASTNode)adaptor.BecomeRoot(stream_COUNT.NextNode(), root_1);

                    	    // Hql.g:599:14: ^( ROW_STAR[\"*\"] )
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
                    // Hql.g:600:5: collectionExpr
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_collectionExpr_in_aggregate3216);
                    	collectionExpr270 = collectionExpr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, collectionExpr270.Tree);

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
    // Hql.g:603:1: aggregateDistinctAll : ( ( DISTINCT | ALL )? ( path | collectionExpr ) ) ;
    public HqlParser.aggregateDistinctAll_return aggregateDistinctAll() // throws RecognitionException [1]
    {   
        HqlParser.aggregateDistinctAll_return retval = new HqlParser.aggregateDistinctAll_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken set271 = null;
        HqlParser.path_return path272 = default(HqlParser.path_return);

        HqlParser.collectionExpr_return collectionExpr273 = default(HqlParser.collectionExpr_return);


        IASTNode set271_tree=null;

        try 
    	{
            // Hql.g:604:2: ( ( ( DISTINCT | ALL )? ( path | collectionExpr ) ) )
            // Hql.g:604:4: ( ( DISTINCT | ALL )? ( path | collectionExpr ) )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// Hql.g:604:4: ( ( DISTINCT | ALL )? ( path | collectionExpr ) )
            	// Hql.g:604:6: ( DISTINCT | ALL )? ( path | collectionExpr )
            	{
            		// Hql.g:604:6: ( DISTINCT | ALL )?
            		int alt97 = 2;
            		int LA97_0 = input.LA(1);

            		if ( (LA97_0 == ALL || LA97_0 == DISTINCT) )
            		{
            		    alt97 = 1;
            		}
            		switch (alt97) 
            		{
            		    case 1 :
            		        // Hql.g:
            		        {
            		        	set271 = (IToken)input.LT(1);
            		        	if ( input.LA(1) == ALL || input.LA(1) == DISTINCT ) 
            		        	{
            		        	    input.Consume();
            		        	    adaptor.AddChild(root_0, (IASTNode)adaptor.Create(set271));
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

            		// Hql.g:604:26: ( path | collectionExpr )
            		int alt98 = 2;
            		int LA98_0 = input.LA(1);

            		if ( (LA98_0 == IDENT) )
            		{
            		    alt98 = 1;
            		}
            		else if ( (LA98_0 == ELEMENTS || LA98_0 == INDICES) )
            		{
            		    alt98 = 2;
            		}
            		else 
            		{
            		    NoViableAltException nvae_d98s0 =
            		        new NoViableAltException("", 98, 0, input);

            		    throw nvae_d98s0;
            		}
            		switch (alt98) 
            		{
            		    case 1 :
            		        // Hql.g:604:28: path
            		        {
            		        	PushFollow(FOLLOW_path_in_aggregateDistinctAll3242);
            		        	path272 = path();
            		        	state.followingStackPointer--;

            		        	adaptor.AddChild(root_0, path272.Tree);

            		        }
            		        break;
            		    case 2 :
            		        // Hql.g:604:35: collectionExpr
            		        {
            		        	PushFollow(FOLLOW_collectionExpr_in_aggregateDistinctAll3246);
            		        	collectionExpr273 = collectionExpr();
            		        	state.followingStackPointer--;

            		        	adaptor.AddChild(root_0, collectionExpr273.Tree);

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
    // Hql.g:609:1: collectionExpr : ( ELEMENTS | INDICES ) OPEN path CLOSE ;
    public HqlParser.collectionExpr_return collectionExpr() // throws RecognitionException [1]
    {   
        HqlParser.collectionExpr_return retval = new HqlParser.collectionExpr_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken ELEMENTS274 = null;
        IToken INDICES275 = null;
        IToken OPEN276 = null;
        IToken CLOSE278 = null;
        HqlParser.path_return path277 = default(HqlParser.path_return);


        IASTNode ELEMENTS274_tree=null;
        IASTNode INDICES275_tree=null;
        IASTNode OPEN276_tree=null;
        IASTNode CLOSE278_tree=null;

        try 
    	{
            // Hql.g:610:2: ( ( ELEMENTS | INDICES ) OPEN path CLOSE )
            // Hql.g:610:4: ( ELEMENTS | INDICES ) OPEN path CLOSE
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// Hql.g:610:4: ( ELEMENTS | INDICES )
            	int alt99 = 2;
            	int LA99_0 = input.LA(1);

            	if ( (LA99_0 == ELEMENTS) )
            	{
            	    alt99 = 1;
            	}
            	else if ( (LA99_0 == INDICES) )
            	{
            	    alt99 = 2;
            	}
            	else 
            	{
            	    NoViableAltException nvae_d99s0 =
            	        new NoViableAltException("", 99, 0, input);

            	    throw nvae_d99s0;
            	}
            	switch (alt99) 
            	{
            	    case 1 :
            	        // Hql.g:610:5: ELEMENTS
            	        {
            	        	ELEMENTS274=(IToken)Match(input,ELEMENTS,FOLLOW_ELEMENTS_in_collectionExpr3265); 
            	        		ELEMENTS274_tree = (IASTNode)adaptor.Create(ELEMENTS274);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(ELEMENTS274_tree, root_0);


            	        }
            	        break;
            	    case 2 :
            	        // Hql.g:610:17: INDICES
            	        {
            	        	INDICES275=(IToken)Match(input,INDICES,FOLLOW_INDICES_in_collectionExpr3270); 
            	        		INDICES275_tree = (IASTNode)adaptor.Create(INDICES275);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(INDICES275_tree, root_0);


            	        }
            	        break;

            	}

            	OPEN276=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_collectionExpr3274); 
            	PushFollow(FOLLOW_path_in_collectionExpr3277);
            	path277 = path();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, path277.Tree);
            	CLOSE278=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_collectionExpr3279); 

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
    // Hql.g:613:1: compoundExpr : ( collectionExpr | path | ( OPEN ( subQuery | ( expression ( COMMA expression )* ) ) CLOSE ) );
    public HqlParser.compoundExpr_return compoundExpr() // throws RecognitionException [1]
    {   
        HqlParser.compoundExpr_return retval = new HqlParser.compoundExpr_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken OPEN281 = null;
        IToken COMMA284 = null;
        IToken CLOSE286 = null;
        HqlParser.collectionExpr_return collectionExpr279 = default(HqlParser.collectionExpr_return);

        HqlParser.path_return path280 = default(HqlParser.path_return);

        HqlParser.subQuery_return subQuery282 = default(HqlParser.subQuery_return);

        HqlParser.expression_return expression283 = default(HqlParser.expression_return);

        HqlParser.expression_return expression285 = default(HqlParser.expression_return);


        IASTNode OPEN281_tree=null;
        IASTNode COMMA284_tree=null;
        IASTNode CLOSE286_tree=null;

        try 
    	{
            // Hql.g:614:2: ( collectionExpr | path | ( OPEN ( subQuery | ( expression ( COMMA expression )* ) ) CLOSE ) )
            int alt102 = 3;
            switch ( input.LA(1) ) 
            {
            case ELEMENTS:
            case INDICES:
            	{
                alt102 = 1;
                }
                break;
            case IDENT:
            	{
                alt102 = 2;
                }
                break;
            case OPEN:
            	{
                alt102 = 3;
                }
                break;
            	default:
            	    NoViableAltException nvae_d102s0 =
            	        new NoViableAltException("", 102, 0, input);

            	    throw nvae_d102s0;
            }

            switch (alt102) 
            {
                case 1 :
                    // Hql.g:614:4: collectionExpr
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_collectionExpr_in_compoundExpr3334);
                    	collectionExpr279 = collectionExpr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, collectionExpr279.Tree);

                    }
                    break;
                case 2 :
                    // Hql.g:615:4: path
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_path_in_compoundExpr3339);
                    	path280 = path();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, path280.Tree);

                    }
                    break;
                case 3 :
                    // Hql.g:616:4: ( OPEN ( subQuery | ( expression ( COMMA expression )* ) ) CLOSE )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	// Hql.g:616:4: ( OPEN ( subQuery | ( expression ( COMMA expression )* ) ) CLOSE )
                    	// Hql.g:616:5: OPEN ( subQuery | ( expression ( COMMA expression )* ) ) CLOSE
                    	{
                    		OPEN281=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_compoundExpr3345); 
                    		// Hql.g:616:11: ( subQuery | ( expression ( COMMA expression )* ) )
                    		int alt101 = 2;
                    		int LA101_0 = input.LA(1);

                    		if ( (LA101_0 == EOF || LA101_0 == FROM || (LA101_0 >= GROUP && LA101_0 <= HAVING) || LA101_0 == ORDER || LA101_0 == SELECT || LA101_0 == SKIP || LA101_0 == TAKE || LA101_0 == UNION || LA101_0 == WHERE || LA101_0 == CLOSE) )
                    		{
                    		    alt101 = 1;
                    		}
                    		else if ( ((LA101_0 >= ALL && LA101_0 <= ANY) || LA101_0 == AVG || LA101_0 == COUNT || LA101_0 == ELEMENTS || (LA101_0 >= EXISTS && LA101_0 <= FALSE) || LA101_0 == INDICES || (LA101_0 >= MAX && LA101_0 <= MIN) || (LA101_0 >= NOT && LA101_0 <= NULL) || (LA101_0 >= SOME && LA101_0 <= SUM) || LA101_0 == TRUE || LA101_0 == CASE || LA101_0 == EMPTY || (LA101_0 >= NUM_INT && LA101_0 <= NUM_LONG) || LA101_0 == OPEN || (LA101_0 >= COLON && LA101_0 <= PARAM) || LA101_0 == BNOT || (LA101_0 >= PLUS && LA101_0 <= MINUS) || (LA101_0 >= QUOTED_String && LA101_0 <= IDENT)) )
                    		{
                    		    alt101 = 2;
                    		}
                    		else 
                    		{
                    		    NoViableAltException nvae_d101s0 =
                    		        new NoViableAltException("", 101, 0, input);

                    		    throw nvae_d101s0;
                    		}
                    		switch (alt101) 
                    		{
                    		    case 1 :
                    		        // Hql.g:616:13: subQuery
                    		        {
                    		        	PushFollow(FOLLOW_subQuery_in_compoundExpr3350);
                    		        	subQuery282 = subQuery();
                    		        	state.followingStackPointer--;

                    		        	adaptor.AddChild(root_0, subQuery282.Tree);

                    		        }
                    		        break;
                    		    case 2 :
                    		        // Hql.g:616:24: ( expression ( COMMA expression )* )
                    		        {
                    		        	// Hql.g:616:24: ( expression ( COMMA expression )* )
                    		        	// Hql.g:616:25: expression ( COMMA expression )*
                    		        	{
                    		        		PushFollow(FOLLOW_expression_in_compoundExpr3355);
                    		        		expression283 = expression();
                    		        		state.followingStackPointer--;

                    		        		adaptor.AddChild(root_0, expression283.Tree);
                    		        		// Hql.g:616:36: ( COMMA expression )*
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
                    		        				    // Hql.g:616:37: COMMA expression
                    		        				    {
                    		        				    	COMMA284=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_compoundExpr3358); 
                    		        				    	PushFollow(FOLLOW_expression_in_compoundExpr3361);
                    		        				    	expression285 = expression();
                    		        				    	state.followingStackPointer--;

                    		        				    	adaptor.AddChild(root_0, expression285.Tree);

                    		        				    }
                    		        				    break;

                    		        				default:
                    		        				    goto loop100;
                    		        		    }
                    		        		} while (true);

                    		        		loop100:
                    		        			;	// Stops C# compiler whining that label 'loop100' has no statements


                    		        	}


                    		        }
                    		        break;

                    		}

                    		CLOSE286=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_compoundExpr3368); 

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
    // Hql.g:619:1: exprList : ( TRAILING | LEADING | BOTH )? ( expression ( ( COMMA expression )+ | f= FROM expression | AS identifier )? | f2= FROM expression )? ;
    public HqlParser.exprList_return exprList() // throws RecognitionException [1]
    {   
        HqlParser.exprList_return retval = new HqlParser.exprList_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken f = null;
        IToken f2 = null;
        IToken TRAILING287 = null;
        IToken LEADING288 = null;
        IToken BOTH289 = null;
        IToken COMMA291 = null;
        IToken AS294 = null;
        HqlParser.expression_return expression290 = default(HqlParser.expression_return);

        HqlParser.expression_return expression292 = default(HqlParser.expression_return);

        HqlParser.expression_return expression293 = default(HqlParser.expression_return);

        HqlParser.identifier_return identifier295 = default(HqlParser.identifier_return);

        HqlParser.expression_return expression296 = default(HqlParser.expression_return);


        IASTNode f_tree=null;
        IASTNode f2_tree=null;
        IASTNode TRAILING287_tree=null;
        IASTNode LEADING288_tree=null;
        IASTNode BOTH289_tree=null;
        IASTNode COMMA291_tree=null;
        IASTNode AS294_tree=null;

        try 
    	{
            // Hql.g:625:2: ( ( TRAILING | LEADING | BOTH )? ( expression ( ( COMMA expression )+ | f= FROM expression | AS identifier )? | f2= FROM expression )? )
            // Hql.g:625:4: ( TRAILING | LEADING | BOTH )? ( expression ( ( COMMA expression )+ | f= FROM expression | AS identifier )? | f2= FROM expression )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// Hql.g:625:4: ( TRAILING | LEADING | BOTH )?
            	int alt103 = 4;
            	switch ( input.LA(1) ) 
            	{
            	    case TRAILING:
            	    	{
            	        alt103 = 1;
            	        }
            	        break;
            	    case LEADING:
            	    	{
            	        alt103 = 2;
            	        }
            	        break;
            	    case BOTH:
            	    	{
            	        alt103 = 3;
            	        }
            	        break;
            	}

            	switch (alt103) 
            	{
            	    case 1 :
            	        // Hql.g:625:5: TRAILING
            	        {
            	        	TRAILING287=(IToken)Match(input,TRAILING,FOLLOW_TRAILING_in_exprList3387); 
            	        		TRAILING287_tree = (IASTNode)adaptor.Create(TRAILING287);
            	        		adaptor.AddChild(root_0, TRAILING287_tree);

            	        	TRAILING287.Type = IDENT;

            	        }
            	        break;
            	    case 2 :
            	        // Hql.g:626:10: LEADING
            	        {
            	        	LEADING288=(IToken)Match(input,LEADING,FOLLOW_LEADING_in_exprList3400); 
            	        		LEADING288_tree = (IASTNode)adaptor.Create(LEADING288);
            	        		adaptor.AddChild(root_0, LEADING288_tree);

            	        	LEADING288.Type = IDENT;

            	        }
            	        break;
            	    case 3 :
            	        // Hql.g:627:10: BOTH
            	        {
            	        	BOTH289=(IToken)Match(input,BOTH,FOLLOW_BOTH_in_exprList3413); 
            	        		BOTH289_tree = (IASTNode)adaptor.Create(BOTH289);
            	        		adaptor.AddChild(root_0, BOTH289_tree);

            	        	BOTH289.Type = IDENT;

            	        }
            	        break;

            	}

            	// Hql.g:629:4: ( expression ( ( COMMA expression )+ | f= FROM expression | AS identifier )? | f2= FROM expression )?
            	int alt106 = 3;
            	int LA106_0 = input.LA(1);

            	if ( ((LA106_0 >= ALL && LA106_0 <= ANY) || LA106_0 == AVG || LA106_0 == COUNT || LA106_0 == ELEMENTS || (LA106_0 >= EXISTS && LA106_0 <= FALSE) || LA106_0 == INDICES || (LA106_0 >= MAX && LA106_0 <= MIN) || (LA106_0 >= NOT && LA106_0 <= NULL) || (LA106_0 >= SOME && LA106_0 <= SUM) || LA106_0 == TRUE || LA106_0 == CASE || LA106_0 == EMPTY || (LA106_0 >= NUM_INT && LA106_0 <= NUM_LONG) || LA106_0 == OPEN || (LA106_0 >= COLON && LA106_0 <= PARAM) || LA106_0 == BNOT || (LA106_0 >= PLUS && LA106_0 <= MINUS) || (LA106_0 >= QUOTED_String && LA106_0 <= IDENT)) )
            	{
            	    alt106 = 1;
            	}
            	else if ( (LA106_0 == FROM) )
            	{
            	    alt106 = 2;
            	}
            	switch (alt106) 
            	{
            	    case 1 :
            	        // Hql.g:630:5: expression ( ( COMMA expression )+ | f= FROM expression | AS identifier )?
            	        {
            	        	PushFollow(FOLLOW_expression_in_exprList3437);
            	        	expression290 = expression();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, expression290.Tree);
            	        	// Hql.g:630:16: ( ( COMMA expression )+ | f= FROM expression | AS identifier )?
            	        	int alt105 = 4;
            	        	switch ( input.LA(1) ) 
            	        	{
            	        	    case COMMA:
            	        	    	{
            	        	        alt105 = 1;
            	        	        }
            	        	        break;
            	        	    case FROM:
            	        	    	{
            	        	        alt105 = 2;
            	        	        }
            	        	        break;
            	        	    case AS:
            	        	    	{
            	        	        alt105 = 3;
            	        	        }
            	        	        break;
            	        	}

            	        	switch (alt105) 
            	        	{
            	        	    case 1 :
            	        	        // Hql.g:630:18: ( COMMA expression )+
            	        	        {
            	        	        	// Hql.g:630:18: ( COMMA expression )+
            	        	        	int cnt104 = 0;
            	        	        	do 
            	        	        	{
            	        	        	    int alt104 = 2;
            	        	        	    int LA104_0 = input.LA(1);

            	        	        	    if ( (LA104_0 == COMMA) )
            	        	        	    {
            	        	        	        alt104 = 1;
            	        	        	    }


            	        	        	    switch (alt104) 
            	        	        		{
            	        	        			case 1 :
            	        	        			    // Hql.g:630:19: COMMA expression
            	        	        			    {
            	        	        			    	COMMA291=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_exprList3442); 
            	        	        			    	PushFollow(FOLLOW_expression_in_exprList3445);
            	        	        			    	expression292 = expression();
            	        	        			    	state.followingStackPointer--;

            	        	        			    	adaptor.AddChild(root_0, expression292.Tree);

            	        	        			    }
            	        	        			    break;

            	        	        			default:
            	        	        			    if ( cnt104 >= 1 ) goto loop104;
            	        	        		            EarlyExitException eee104 =
            	        	        		                new EarlyExitException(104, input);
            	        	        		            throw eee104;
            	        	        	    }
            	        	        	    cnt104++;
            	        	        	} while (true);

            	        	        	loop104:
            	        	        		;	// Stops C# compiler whining that label 'loop104' has no statements


            	        	        }
            	        	        break;
            	        	    case 2 :
            	        	        // Hql.g:631:9: f= FROM expression
            	        	        {
            	        	        	f=(IToken)Match(input,FROM,FOLLOW_FROM_in_exprList3460); 
            	        	        		f_tree = (IASTNode)adaptor.Create(f);
            	        	        		adaptor.AddChild(root_0, f_tree);

            	        	        	PushFollow(FOLLOW_expression_in_exprList3462);
            	        	        	expression293 = expression();
            	        	        	state.followingStackPointer--;

            	        	        	adaptor.AddChild(root_0, expression293.Tree);
            	        	        	f.Type = IDENT;

            	        	        }
            	        	        break;
            	        	    case 3 :
            	        	        // Hql.g:632:9: AS identifier
            	        	        {
            	        	        	AS294=(IToken)Match(input,AS,FOLLOW_AS_in_exprList3474); 
            	        	        	PushFollow(FOLLOW_identifier_in_exprList3477);
            	        	        	identifier295 = identifier();
            	        	        	state.followingStackPointer--;

            	        	        	adaptor.AddChild(root_0, identifier295.Tree);

            	        	        }
            	        	        break;

            	        	}


            	        }
            	        break;
            	    case 2 :
            	        // Hql.g:633:7: f2= FROM expression
            	        {
            	        	f2=(IToken)Match(input,FROM,FOLLOW_FROM_in_exprList3491); 
            	        		f2_tree = (IASTNode)adaptor.Create(f2);
            	        		adaptor.AddChild(root_0, f2_tree);

            	        	PushFollow(FOLLOW_expression_in_exprList3493);
            	        	expression296 = expression();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, expression296.Tree);
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
    // Hql.g:637:1: subQuery : innerSubQuery ( UNION innerSubQuery )* ;
    public HqlParser.subQuery_return subQuery() // throws RecognitionException [1]
    {   
        HqlParser.subQuery_return retval = new HqlParser.subQuery_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken UNION298 = null;
        HqlParser.innerSubQuery_return innerSubQuery297 = default(HqlParser.innerSubQuery_return);

        HqlParser.innerSubQuery_return innerSubQuery299 = default(HqlParser.innerSubQuery_return);


        IASTNode UNION298_tree=null;

        try 
    	{
            // Hql.g:638:2: ( innerSubQuery ( UNION innerSubQuery )* )
            // Hql.g:638:4: innerSubQuery ( UNION innerSubQuery )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_innerSubQuery_in_subQuery3513);
            	innerSubQuery297 = innerSubQuery();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, innerSubQuery297.Tree);
            	// Hql.g:638:18: ( UNION innerSubQuery )*
            	do 
            	{
            	    int alt107 = 2;
            	    int LA107_0 = input.LA(1);

            	    if ( (LA107_0 == UNION) )
            	    {
            	        alt107 = 1;
            	    }


            	    switch (alt107) 
            		{
            			case 1 :
            			    // Hql.g:638:19: UNION innerSubQuery
            			    {
            			    	UNION298=(IToken)Match(input,UNION,FOLLOW_UNION_in_subQuery3516); 
            			    		UNION298_tree = (IASTNode)adaptor.Create(UNION298);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(UNION298_tree, root_0);

            			    	PushFollow(FOLLOW_innerSubQuery_in_subQuery3519);
            			    	innerSubQuery299 = innerSubQuery();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, innerSubQuery299.Tree);

            			    }
            			    break;

            			default:
            			    goto loop107;
            	    }
            	} while (true);

            	loop107:
            		;	// Stops C# compiler whining that label 'loop107' has no statements


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
    // Hql.g:641:1: innerSubQuery : queryRule -> ^( QUERY[\"query\"] queryRule ) ;
    public HqlParser.innerSubQuery_return innerSubQuery() // throws RecognitionException [1]
    {   
        HqlParser.innerSubQuery_return retval = new HqlParser.innerSubQuery_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.queryRule_return queryRule300 = default(HqlParser.queryRule_return);


        RewriteRuleSubtreeStream stream_queryRule = new RewriteRuleSubtreeStream(adaptor,"rule queryRule");
        try 
    	{
            // Hql.g:642:2: ( queryRule -> ^( QUERY[\"query\"] queryRule ) )
            // Hql.g:642:4: queryRule
            {
            	PushFollow(FOLLOW_queryRule_in_innerSubQuery3533);
            	queryRule300 = queryRule();
            	state.followingStackPointer--;

            	stream_queryRule.Add(queryRule300.Tree);


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
            	// 643:2: -> ^( QUERY[\"query\"] queryRule )
            	{
            	    // Hql.g:643:5: ^( QUERY[\"query\"] queryRule )
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
    // Hql.g:646:1: constant : ( NUM_INT | NUM_FLOAT | NUM_LONG | NUM_DOUBLE | NUM_DECIMAL | QUOTED_String | NULL | TRUE | FALSE | EMPTY );
    public HqlParser.constant_return constant() // throws RecognitionException [1]
    {   
        HqlParser.constant_return retval = new HqlParser.constant_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken set301 = null;

        IASTNode set301_tree=null;

        try 
    	{
            // Hql.g:647:2: ( NUM_INT | NUM_FLOAT | NUM_LONG | NUM_DOUBLE | NUM_DECIMAL | QUOTED_String | NULL | TRUE | FALSE | EMPTY )
            // Hql.g:
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	set301 = (IToken)input.LT(1);
            	if ( input.LA(1) == FALSE || input.LA(1) == NULL || input.LA(1) == TRUE || input.LA(1) == EMPTY || (input.LA(1) >= NUM_INT && input.LA(1) <= NUM_LONG) || input.LA(1) == QUOTED_String ) 
            	{
            	    input.Consume();
            	    adaptor.AddChild(root_0, (IASTNode)adaptor.Create(set301));
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
    // Hql.g:665:1: path : identifier ( DOT identifier )* ;
    public HqlParser.path_return path() // throws RecognitionException [1]
    {   
        HqlParser.path_return retval = new HqlParser.path_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken DOT303 = null;
        HqlParser.identifier_return identifier302 = default(HqlParser.identifier_return);

        HqlParser.identifier_return identifier304 = default(HqlParser.identifier_return);


        IASTNode DOT303_tree=null;


        // TODO - need to clean up DotIdent - suspect that DotIdent2 supersedes the other one, but need to do the analysis
        //HandleDotIdent2();

        try 
    	{
            // Hql.g:670:2: ( identifier ( DOT identifier )* )
            // Hql.g:670:4: identifier ( DOT identifier )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_identifier_in_path3621);
            	identifier302 = identifier();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, identifier302.Tree);
            	// Hql.g:670:15: ( DOT identifier )*
            	do 
            	{
            	    int alt108 = 2;
            	    int LA108_0 = input.LA(1);

            	    if ( (LA108_0 == DOT) )
            	    {
            	        alt108 = 1;
            	    }


            	    switch (alt108) 
            		{
            			case 1 :
            			    // Hql.g:670:17: DOT identifier
            			    {
            			    	DOT303=(IToken)Match(input,DOT,FOLLOW_DOT_in_path3625); 
            			    		DOT303_tree = (IASTNode)adaptor.Create(DOT303);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(DOT303_tree, root_0);

            			    	 WeakKeywords(); 
            			    	PushFollow(FOLLOW_identifier_in_path3630);
            			    	identifier304 = identifier();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, identifier304.Tree);

            			    }
            			    break;

            			default:
            			    goto loop108;
            	    }
            	} while (true);

            	loop108:
            		;	// Stops C# compiler whining that label 'loop108' has no statements


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
    // Hql.g:675:1: identifier : IDENT ;
    public HqlParser.identifier_return identifier() // throws RecognitionException [1]
    {   
        HqlParser.identifier_return retval = new HqlParser.identifier_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken IDENT305 = null;

        IASTNode IDENT305_tree=null;

        try 
    	{
            // Hql.g:676:2: ( IDENT )
            // Hql.g:676:4: IDENT
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	IDENT305=(IToken)Match(input,IDENT,FOLLOW_IDENT_in_identifier3646); 
            		IDENT305_tree = (IASTNode)adaptor.Create(IDENT305);
            		adaptor.AddChild(root_0, IDENT305_tree);


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
    public static readonly BitSet FOLLOW_EQ_in_assignment697 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x30C4068F80000002UL});
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
    public static readonly BitSet FOLLOW_OPEN_in_insertablePropertySpec877 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_primaryExpression_in_insertablePropertySpec879 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000012000000000UL});
    public static readonly BitSet FOLLOW_COMMA_in_insertablePropertySpec883 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x30C4068F80000002UL});
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
    public static readonly BitSet FOLLOW_SELECT_in_selectClause1030 = new BitSet(new ulong[]{0x024B00F8085B1230UL,0x30C4068F80000012UL});
    public static readonly BitSet FOLLOW_DISTINCT_in_selectClause1042 = new BitSet(new ulong[]{0x024B00F8085B1230UL,0x30C4068F80000012UL});
    public static readonly BitSet FOLLOW_selectedPropertiesList_in_selectClause1048 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_newExpression_in_selectClause1052 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_selectObject_in_selectClause1056 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_NEW_in_newExpression1070 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_path_in_newExpression1072 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000008000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_newExpression1077 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x30C4068F80000002UL});
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
    public static readonly BitSet FOLLOW_WITH_in_withClause1300 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x30C4068F80000002UL});
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
    public static readonly BitSet FOLLOW_LITERAL_by_in_groupByClause1591 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_expression_in_groupByClause1594 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_COMMA_in_groupByClause1598 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_expression_in_groupByClause1601 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_ORDER_in_orderByClause1615 = new BitSet(new ulong[]{0x0100000000000000UL});
    public static readonly BitSet FOLLOW_LITERAL_by_in_orderByClause1618 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_orderElement_in_orderByClause1621 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_COMMA_in_orderByClause1625 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_orderElement_in_orderByClause1628 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_SKIP_in_skipClause1642 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000060080000000UL});
    public static readonly BitSet FOLLOW_NUM_INT_in_skipClause1646 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_parameter_in_skipClause1650 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_TAKE_in_takeClause1662 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000060080000000UL});
    public static readonly BitSet FOLLOW_NUM_INT_in_takeClause1666 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_parameter_in_takeClause1670 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_COLON_in_parameter1682 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_identifier_in_parameter1685 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_PARAM_in_parameter1690 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000080000000UL});
    public static readonly BitSet FOLLOW_NUM_INT_in_parameter1694 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_expression_in_orderElement1707 = new BitSet(new ulong[]{0x0000000000004102UL,0x0000000000000000UL,0x0000000000000060UL});
    public static readonly BitSet FOLLOW_ascendingOrDescending_in_orderElement1711 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ASCENDING_in_ascendingOrDescending1729 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_133_in_ascendingOrDescending1735 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_DESCENDING_in_ascendingOrDescending1755 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_134_in_ascendingOrDescending1761 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_HAVING_in_havingClause1782 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_logicalExpression_in_havingClause1785 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WHERE_in_whereClause1796 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_logicalExpression_in_whereClause1799 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_aliasedExpression_in_selectedPropertiesList1810 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_COMMA_in_selectedPropertiesList1814 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_aliasedExpression_in_selectedPropertiesList1817 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_expression_in_aliasedExpression1832 = new BitSet(new ulong[]{0x0000000000000082UL});
    public static readonly BitSet FOLLOW_AS_in_aliasedExpression1836 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_identifier_in_aliasedExpression1839 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_expression_in_logicalExpression1878 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_logicalOrExpression_in_expression1890 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_logicalAndExpression_in_logicalOrExpression1902 = new BitSet(new ulong[]{0x0000010000000002UL});
    public static readonly BitSet FOLLOW_OR_in_logicalOrExpression1906 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_logicalAndExpression_in_logicalOrExpression1909 = new BitSet(new ulong[]{0x0000010000000002UL});
    public static readonly BitSet FOLLOW_negatedExpression_in_logicalAndExpression1924 = new BitSet(new ulong[]{0x0000000000000042UL});
    public static readonly BitSet FOLLOW_AND_in_logicalAndExpression1928 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_negatedExpression_in_logicalAndExpression1931 = new BitSet(new ulong[]{0x0000000000000042UL});
    public static readonly BitSet FOLLOW_NOT_in_negatedExpression1952 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_negatedExpression_in_negatedExpression1956 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_equalityExpression_in_negatedExpression1969 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_relationalExpression_in_equalityExpression1999 = new BitSet(new ulong[]{0x0000000080000002UL,0x0000184000000000UL});
    public static readonly BitSet FOLLOW_EQ_in_equalityExpression2007 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_IS_in_equalityExpression2016 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_NOT_in_equalityExpression2022 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_NE_in_equalityExpression2034 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_SQL_NE_in_equalityExpression2043 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_relationalExpression_in_equalityExpression2054 = new BitSet(new ulong[]{0x0000000080000002UL,0x0000184000000000UL});
    public static readonly BitSet FOLLOW_concatenation_in_relationalExpression2071 = new BitSet(new ulong[]{0x0000004404000402UL,0x0001E00000000008UL});
    public static readonly BitSet FOLLOW_LT_in_relationalExpression2083 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_GT_in_relationalExpression2088 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_LE_in_relationalExpression2093 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_GE_in_relationalExpression2098 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_bitwiseNotExpression_in_relationalExpression2103 = new BitSet(new ulong[]{0x0000000000000002UL,0x0001E00000000000UL});
    public static readonly BitSet FOLLOW_NOT_in_relationalExpression2120 = new BitSet(new ulong[]{0x0000000404000400UL,0x0000000000000008UL});
    public static readonly BitSet FOLLOW_IN_in_relationalExpression2141 = new BitSet(new ulong[]{0x0042001808421200UL,0x2000008000000000UL});
    public static readonly BitSet FOLLOW_inList_in_relationalExpression2150 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_BETWEEN_in_relationalExpression2161 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_betweenList_in_relationalExpression2170 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_LIKE_in_relationalExpression2182 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_concatenation_in_relationalExpression2191 = new BitSet(new ulong[]{0x0000000000040000UL});
    public static readonly BitSet FOLLOW_likeEscape_in_relationalExpression2193 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_MEMBER_in_relationalExpression2202 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000020UL});
    public static readonly BitSet FOLLOW_OF_in_relationalExpression2206 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_path_in_relationalExpression2213 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ESCAPE_in_likeEscape2240 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_concatenation_in_likeEscape2243 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_compoundExpr_in_inList2256 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_concatenation_in_betweenList2277 = new BitSet(new ulong[]{0x0000000000000040UL});
    public static readonly BitSet FOLLOW_AND_in_betweenList2279 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_concatenation_in_betweenList2282 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_bitwiseNotExpression_in_concatenation2301 = new BitSet(new ulong[]{0x0000000000000002UL,0x0002000000000000UL});
    public static readonly BitSet FOLLOW_CONCAT_in_concatenation2309 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_bitwiseNotExpression_in_concatenation2318 = new BitSet(new ulong[]{0x0000000000000002UL,0x0002000000000000UL});
    public static readonly BitSet FOLLOW_CONCAT_in_concatenation2325 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_bitwiseNotExpression_in_concatenation2328 = new BitSet(new ulong[]{0x0000000000000002UL,0x0002000000000000UL});
    public static readonly BitSet FOLLOW_BNOT_in_bitwiseNotExpression2352 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_bitwiseOrExpression_in_bitwiseNotExpression2355 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_bitwiseOrExpression_in_bitwiseNotExpression2361 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_bitwiseXOrExpression_in_bitwiseOrExpression2373 = new BitSet(new ulong[]{0x0000000000000002UL,0x0008000000000000UL});
    public static readonly BitSet FOLLOW_BOR_in_bitwiseOrExpression2376 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_bitwiseXOrExpression_in_bitwiseOrExpression2379 = new BitSet(new ulong[]{0x0000000000000002UL,0x0008000000000000UL});
    public static readonly BitSet FOLLOW_bitwiseAndExpression_in_bitwiseXOrExpression2393 = new BitSet(new ulong[]{0x0000000000000002UL,0x0010000000000000UL});
    public static readonly BitSet FOLLOW_BXOR_in_bitwiseXOrExpression2396 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_bitwiseAndExpression_in_bitwiseXOrExpression2399 = new BitSet(new ulong[]{0x0000000000000002UL,0x0010000000000000UL});
    public static readonly BitSet FOLLOW_additiveExpression_in_bitwiseAndExpression2413 = new BitSet(new ulong[]{0x0000000000000002UL,0x0020000000000000UL});
    public static readonly BitSet FOLLOW_BAND_in_bitwiseAndExpression2416 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_additiveExpression_in_bitwiseAndExpression2419 = new BitSet(new ulong[]{0x0000000000000002UL,0x0020000000000000UL});
    public static readonly BitSet FOLLOW_multiplyExpression_in_additiveExpression2433 = new BitSet(new ulong[]{0x0000000000000002UL,0x00C0000000000000UL});
    public static readonly BitSet FOLLOW_PLUS_in_additiveExpression2439 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_MINUS_in_additiveExpression2444 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_multiplyExpression_in_additiveExpression2449 = new BitSet(new ulong[]{0x0000000000000002UL,0x00C0000000000000UL});
    public static readonly BitSet FOLLOW_unaryExpression_in_multiplyExpression2464 = new BitSet(new ulong[]{0x0000000000000002UL,0x0300000000000000UL});
    public static readonly BitSet FOLLOW_STAR_in_multiplyExpression2470 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_DIV_in_multiplyExpression2475 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_unaryExpression_in_multiplyExpression2480 = new BitSet(new ulong[]{0x0000000000000002UL,0x0300000000000000UL});
    public static readonly BitSet FOLLOW_MINUS_in_unaryExpression2498 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_unaryExpression_in_unaryExpression2502 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_PLUS_in_unaryExpression2519 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_unaryExpression_in_unaryExpression2523 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_caseExpression_in_unaryExpression2540 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_quantifiedExpression_in_unaryExpression2554 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_atom_in_unaryExpression2569 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_CASE_in_caseExpression2588 = new BitSet(new ulong[]{0x2000000000000000UL});
    public static readonly BitSet FOLLOW_whenClause_in_caseExpression2591 = new BitSet(new ulong[]{0x2C00000000000000UL});
    public static readonly BitSet FOLLOW_elseClause_in_caseExpression2596 = new BitSet(new ulong[]{0x0400000000000000UL});
    public static readonly BitSet FOLLOW_END_in_caseExpression2600 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_CASE_in_caseExpression2620 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_unaryExpression_in_caseExpression2622 = new BitSet(new ulong[]{0x2000000000000000UL});
    public static readonly BitSet FOLLOW_altWhenClause_in_caseExpression2625 = new BitSet(new ulong[]{0x2C00000000000000UL});
    public static readonly BitSet FOLLOW_elseClause_in_caseExpression2630 = new BitSet(new ulong[]{0x0400000000000000UL});
    public static readonly BitSet FOLLOW_END_in_caseExpression2634 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WHEN_in_whenClause2663 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_logicalExpression_in_whenClause2666 = new BitSet(new ulong[]{0x1000000000000000UL});
    public static readonly BitSet FOLLOW_THEN_in_whenClause2668 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_expression_in_whenClause2671 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WHEN_in_altWhenClause2685 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_unaryExpression_in_altWhenClause2688 = new BitSet(new ulong[]{0x1000000000000000UL});
    public static readonly BitSet FOLLOW_THEN_in_altWhenClause2690 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_expression_in_altWhenClause2693 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ELSE_in_elseClause2707 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_expression_in_elseClause2710 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_SOME_in_quantifiedExpression2725 = new BitSet(new ulong[]{0x0042001808421200UL,0x2000008000000000UL});
    public static readonly BitSet FOLLOW_EXISTS_in_quantifiedExpression2730 = new BitSet(new ulong[]{0x0042001808421200UL,0x2000008000000000UL});
    public static readonly BitSet FOLLOW_ALL_in_quantifiedExpression2735 = new BitSet(new ulong[]{0x0042001808421200UL,0x2000008000000000UL});
    public static readonly BitSet FOLLOW_ANY_in_quantifiedExpression2740 = new BitSet(new ulong[]{0x0042001808421200UL,0x2000008000000000UL});
    public static readonly BitSet FOLLOW_identifier_in_quantifiedExpression2749 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_collectionExpr_in_quantifiedExpression2753 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_OPEN_in_quantifiedExpression2758 = new BitSet(new ulong[]{0x0084A20003400000UL});
    public static readonly BitSet FOLLOW_subQuery_in_quantifiedExpression2763 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000010000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_quantifiedExpression2767 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_primaryExpression_in_atom2786 = new BitSet(new ulong[]{0x0000000000008002UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_DOT_in_atom2795 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_identifier_in_atom2798 = new BitSet(new ulong[]{0x0000000000008002UL,0x0400008000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_atom2826 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x30C4078F80000047UL});
    public static readonly BitSet FOLLOW_exprList_in_atom2831 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000010000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_atom2833 = new BitSet(new ulong[]{0x0000000000008002UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_OPEN_BRACKET_in_atom2847 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_expression_in_atom2852 = new BitSet(new ulong[]{0x0000000000000000UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_BRACKET_in_atom2854 = new BitSet(new ulong[]{0x0000000000008002UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_identPrimary_in_primaryExpression2874 = new BitSet(new ulong[]{0x0000000000008002UL});
    public static readonly BitSet FOLLOW_DOT_in_primaryExpression2887 = new BitSet(new ulong[]{0x0000000000000800UL});
    public static readonly BitSet FOLLOW_CLASS_in_primaryExpression2890 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_constant_in_primaryExpression2900 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_COLON_in_primaryExpression2907 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_identifier_in_primaryExpression2910 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_OPEN_in_primaryExpression2919 = new BitSet(new ulong[]{0x02CFA2D80B5A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_expressionOrVector_in_primaryExpression2923 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000010000000000UL});
    public static readonly BitSet FOLLOW_subQuery_in_primaryExpression2927 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000010000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_primaryExpression2930 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_PARAM_in_primaryExpression2938 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000080000000UL});
    public static readonly BitSet FOLLOW_NUM_INT_in_primaryExpression2942 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_expression_in_expressionOrVector2960 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_vectorExpr_in_expressionOrVector2966 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_COMMA_in_vectorExpr3005 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_expression_in_vectorExpr3008 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_COMMA_in_vectorExpr3011 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_expression_in_vectorExpr3014 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_identifier_in_identPrimary3030 = new BitSet(new ulong[]{0x0000000000008002UL,0x0000008000000000UL});
    public static readonly BitSet FOLLOW_DOT_in_identPrimary3048 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000010UL});
    public static readonly BitSet FOLLOW_identifier_in_identPrimary3053 = new BitSet(new ulong[]{0x0000000000008002UL,0x0000008000000000UL});
    public static readonly BitSet FOLLOW_OBJECT_in_identPrimary3059 = new BitSet(new ulong[]{0x0000000000008002UL,0x0000008000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_identPrimary3077 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x30C4078F80000047UL});
    public static readonly BitSet FOLLOW_exprList_in_identPrimary3082 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000010000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_identPrimary3084 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_aggregate_in_identPrimary3100 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_SUM_in_aggregate3121 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000008000000000UL});
    public static readonly BitSet FOLLOW_AVG_in_aggregate3127 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000008000000000UL});
    public static readonly BitSet FOLLOW_MAX_in_aggregate3133 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000008000000000UL});
    public static readonly BitSet FOLLOW_MIN_in_aggregate3139 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000008000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_aggregate3143 = new BitSet(new ulong[]{0x024B0098085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_additiveExpression_in_aggregate3145 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000010000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_aggregate3147 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_COUNT_in_aggregate3166 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000008000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_aggregate3168 = new BitSet(new ulong[]{0x0042001808431210UL,0x2100000000000000UL});
    public static readonly BitSet FOLLOW_STAR_in_aggregate3174 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000010000000000UL});
    public static readonly BitSet FOLLOW_aggregateDistinctAll_in_aggregate3180 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000010000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_aggregate3184 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_collectionExpr_in_aggregate3216 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_set_in_aggregateDistinctAll3229 = new BitSet(new ulong[]{0x0042001808421200UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_path_in_aggregateDistinctAll3242 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_collectionExpr_in_aggregateDistinctAll3246 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ELEMENTS_in_collectionExpr3265 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000008000000000UL});
    public static readonly BitSet FOLLOW_INDICES_in_collectionExpr3270 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000008000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_collectionExpr3274 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_path_in_collectionExpr3277 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000010000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_collectionExpr3279 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_collectionExpr_in_compoundExpr3334 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_path_in_compoundExpr3339 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_OPEN_in_compoundExpr3345 = new BitSet(new ulong[]{0x02CFA2D80B5A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_subQuery_in_compoundExpr3350 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000010000000000UL});
    public static readonly BitSet FOLLOW_expression_in_compoundExpr3355 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000012000000000UL});
    public static readonly BitSet FOLLOW_COMMA_in_compoundExpr3358 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_expression_in_compoundExpr3361 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000012000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_compoundExpr3368 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_TRAILING_in_exprList3387 = new BitSet(new ulong[]{0x024B00D8085A1232UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_LEADING_in_exprList3400 = new BitSet(new ulong[]{0x024B00D8085A1232UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_BOTH_in_exprList3413 = new BitSet(new ulong[]{0x024B00D8085A1232UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_expression_in_exprList3437 = new BitSet(new ulong[]{0x0000000000400082UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_COMMA_in_exprList3442 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_expression_in_exprList3445 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_FROM_in_exprList3460 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_expression_in_exprList3462 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_AS_in_exprList3474 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_identifier_in_exprList3477 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_FROM_in_exprList3491 = new BitSet(new ulong[]{0x024B00D8085A1230UL,0x30C4068F80000002UL});
    public static readonly BitSet FOLLOW_expression_in_exprList3493 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_innerSubQuery_in_subQuery3513 = new BitSet(new ulong[]{0x0010000000000002UL});
    public static readonly BitSet FOLLOW_UNION_in_subQuery3516 = new BitSet(new ulong[]{0x0084A20003400000UL});
    public static readonly BitSet FOLLOW_innerSubQuery_in_subQuery3519 = new BitSet(new ulong[]{0x0010000000000002UL});
    public static readonly BitSet FOLLOW_queryRule_in_innerSubQuery3533 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_set_in_constant0 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_identifier_in_path3621 = new BitSet(new ulong[]{0x0000000000008002UL});
    public static readonly BitSet FOLLOW_DOT_in_path3625 = new BitSet(new ulong[]{0x0040000000400000UL,0x2000000000000000UL});
    public static readonly BitSet FOLLOW_identifier_in_path3630 = new BitSet(new ulong[]{0x0000000000008002UL});
    public static readonly BitSet FOLLOW_IDENT_in_identifier3646 = new BitSet(new ulong[]{0x0000000000000002UL});

}
}