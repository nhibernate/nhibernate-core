// $ANTLR 3.1.2 /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g 2009-08-19 15:39:23

// The variable 'variable' is assigned but its value is never used.
#pragma warning disable 168, 219
// Unreachable code detected.
#pragma warning disable 162
namespace  NHibernate.Hql.Ast.ANTLR 
{

using NHibernate.Hql.Ast.ANTLR.Tree;


using System;
using Antlr.Runtime;
using IList 		= System.Collections.IList;
using ArrayList 	= System.Collections.ArrayList;
using Stack 		= Antlr.Runtime.Collections.StackList;



using Antlr.Runtime.Tree;

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
		"'descending'"
    };

    public const int COMMA = 99;
    public const int EXISTS = 19;
    public const int EXPR_LIST = 73;
    public const int FETCH = 21;
    public const int MINUS = 115;
    public const int AS = 7;
    public const int END = 56;
    public const int INTO = 30;
    public const int FALSE = 20;
    public const int ELEMENTS = 17;
    public const int THEN = 58;
    public const int ALIAS = 70;
    public const int BOR = 111;
    public const int ON = 60;
    public const int DOT = 15;
    public const int ORDER = 41;
    public const int AND = 6;
    public const int CONSTANT = 92;
    public const int RIGHT = 44;
    public const int METHOD_CALL = 79;
    public const int UNARY_MINUS = 88;
    public const int CONCAT = 109;
    public const int PROPERTIES = 43;
    public const int SELECT = 45;
    public const int LE = 107;
    public const int BETWEEN = 10;
    public const int NUM_INT = 93;
    public const int BOTH = 62;
    public const int PLUS = 114;
    public const int VERSIONED = 52;
    public const int MEMBER = 65;
    public const int NUM_DECIMAL = 95;
    public const int UNION = 50;
    public const int DISTINCT = 16;
    public const int RANGE = 85;
    public const int FILTER_ENTITY = 74;
    public const int IDENT = 123;
    public const int WHEN = 59;
    public const int DESCENDING = 14;
    public const int WS = 127;
    public const int EQ = 100;
    public const int NEW = 37;
    public const int LT = 105;
    public const int ESCqs = 126;
    public const int OF = 67;
    public const int UPDATE = 51;
    public const int SELECT_FROM = 87;
    public const int LITERAL_by = 54;
    public const int FLOAT_SUFFIX = 129;
    public const int ANY = 5;
    public const int UNARY_PLUS = 89;
    public const int NUM_FLOAT = 96;
    public const int GE = 108;
    public const int CASE = 55;
    public const int OPEN_BRACKET = 118;
    public const int ELSE = 57;
    public const int OPEN = 101;
    public const int COUNT = 12;
    public const int NULL = 39;
    public const int T__132 = 132;
    public const int COLON = 120;
    public const int DIV = 117;
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
    public const int BNOT = 110;
    public const int LEADING = 64;
    public const int CLOSE_BRACKET = 119;
    public const int NUM_DOUBLE = 94;
    public const int INNER = 28;
    public const int QUERY = 84;
    public const int ORDER_ELEMENT = 83;
    public const int OR = 40;
    public const int FULL = 23;
    public const int INDICES = 27;
    public const int IS_NULL = 78;
    public const int GROUP = 24;
    public const int ESCAPE = 18;
    public const int PARAM = 121;
    public const int ID_LETTER = 125;
    public const int INDEX_OP = 76;
    public const int HEX_DIGIT = 130;
    public const int LEFT = 33;
    public const int TRAILING = 68;
    public const int JOIN = 32;
    public const int NOT_BETWEEN = 80;
    public const int BAND = 113;
    public const int SUM = 48;
    public const int ROW_STAR = 86;
    public const int OUTER = 42;
    public const int FROM = 22;
    public const int NOT_IN = 81;
    public const int DELETE = 13;
    public const int OBJECT = 66;
    public const int MAX = 35;
    public const int QUOTED_String = 122;
    public const int EMPTY = 63;
    public const int NOT_LIKE = 82;
    public const int ASCENDING = 8;
    public const int NUM_LONG = 97;
    public const int IS = 31;
    public const int SQL_NE = 104;
    public const int IN_LIST = 75;
    public const int WEIRD_IDENT = 91;
    public const int GT = 106;
    public const int NE = 103;
    public const int MIN = 36;
    public const int LIKE = 34;
    public const int WITH = 61;
    public const int IN = 26;
    public const int CONSTRUCTOR = 71;
    public const int CLASS = 11;
    public const int SOME = 47;
    public const int EXPONENT = 128;
    public const int ID_START_LETTER = 124;
    public const int EOF = -1;
    public const int CLOSE = 102;
    public const int AVG = 9;
    public const int STAR = 116;
    public const int BXOR = 112;
    public const int NOT = 38;
    public const int JAVA_CONSTANT = 98;

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
		get { return "/Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g"; }
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:127:1: statement : ( updateStatement | deleteStatement | selectStatement | insertStatement ) ;
    public HqlParser.statement_return statement() // throws RecognitionException [1]
    {   
        HqlParser.statement_return retval = new HqlParser.statement_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.updateStatement_return updateStatement1 = default(HqlParser.updateStatement_return);

        HqlParser.deleteStatement_return deleteStatement2 = default(HqlParser.deleteStatement_return);

        HqlParser.selectStatement_return selectStatement3 = default(HqlParser.selectStatement_return);

        HqlParser.insertStatement_return insertStatement4 = default(HqlParser.insertStatement_return);



        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:128:2: ( ( updateStatement | deleteStatement | selectStatement | insertStatement ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:128:4: ( updateStatement | deleteStatement | selectStatement | insertStatement )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:128:4: ( updateStatement | deleteStatement | selectStatement | insertStatement )
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
            	case ORDER:
            	case SELECT:
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
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:128:6: updateStatement
            	        {
            	        	PushFollow(FOLLOW_updateStatement_in_statement599);
            	        	updateStatement1 = updateStatement();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, updateStatement1.Tree);

            	        }
            	        break;
            	    case 2 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:128:24: deleteStatement
            	        {
            	        	PushFollow(FOLLOW_deleteStatement_in_statement603);
            	        	deleteStatement2 = deleteStatement();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, deleteStatement2.Tree);

            	        }
            	        break;
            	    case 3 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:128:42: selectStatement
            	        {
            	        	PushFollow(FOLLOW_selectStatement_in_statement607);
            	        	selectStatement3 = selectStatement();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, selectStatement3.Tree);

            	        }
            	        break;
            	    case 4 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:128:60: insertStatement
            	        {
            	        	PushFollow(FOLLOW_insertStatement_in_statement611);
            	        	insertStatement4 = insertStatement();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, insertStatement4.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:131:1: updateStatement : UPDATE ( VERSIONED )? optionalFromTokenFromClause setClause ( whereClause )? ;
    public HqlParser.updateStatement_return updateStatement() // throws RecognitionException [1]
    {   
        HqlParser.updateStatement_return retval = new HqlParser.updateStatement_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken UPDATE5 = null;
        IToken VERSIONED6 = null;
        HqlParser.optionalFromTokenFromClause_return optionalFromTokenFromClause7 = default(HqlParser.optionalFromTokenFromClause_return);

        HqlParser.setClause_return setClause8 = default(HqlParser.setClause_return);

        HqlParser.whereClause_return whereClause9 = default(HqlParser.whereClause_return);


        IASTNode UPDATE5_tree=null;
        IASTNode VERSIONED6_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:132:2: ( UPDATE ( VERSIONED )? optionalFromTokenFromClause setClause ( whereClause )? )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:132:4: UPDATE ( VERSIONED )? optionalFromTokenFromClause setClause ( whereClause )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	UPDATE5=(IToken)Match(input,UPDATE,FOLLOW_UPDATE_in_updateStatement624); 
            		UPDATE5_tree = (IASTNode)adaptor.Create(UPDATE5);
            		root_0 = (IASTNode)adaptor.BecomeRoot(UPDATE5_tree, root_0);

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:132:12: ( VERSIONED )?
            	int alt2 = 2;
            	int LA2_0 = input.LA(1);

            	if ( (LA2_0 == VERSIONED) )
            	{
            	    alt2 = 1;
            	}
            	switch (alt2) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:132:13: VERSIONED
            	        {
            	        	VERSIONED6=(IToken)Match(input,VERSIONED,FOLLOW_VERSIONED_in_updateStatement628); 
            	        		VERSIONED6_tree = (IASTNode)adaptor.Create(VERSIONED6);
            	        		adaptor.AddChild(root_0, VERSIONED6_tree);


            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_optionalFromTokenFromClause_in_updateStatement634);
            	optionalFromTokenFromClause7 = optionalFromTokenFromClause();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, optionalFromTokenFromClause7.Tree);
            	PushFollow(FOLLOW_setClause_in_updateStatement638);
            	setClause8 = setClause();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, setClause8.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:135:3: ( whereClause )?
            	int alt3 = 2;
            	int LA3_0 = input.LA(1);

            	if ( (LA3_0 == WHERE) )
            	{
            	    alt3 = 1;
            	}
            	switch (alt3) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:135:4: whereClause
            	        {
            	        	PushFollow(FOLLOW_whereClause_in_updateStatement643);
            	        	whereClause9 = whereClause();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, whereClause9.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:138:1: setClause : ( SET assignment ( COMMA assignment )* ) ;
    public HqlParser.setClause_return setClause() // throws RecognitionException [1]
    {   
        HqlParser.setClause_return retval = new HqlParser.setClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken SET10 = null;
        IToken COMMA12 = null;
        HqlParser.assignment_return assignment11 = default(HqlParser.assignment_return);

        HqlParser.assignment_return assignment13 = default(HqlParser.assignment_return);


        IASTNode SET10_tree=null;
        IASTNode COMMA12_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:139:2: ( ( SET assignment ( COMMA assignment )* ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:139:4: ( SET assignment ( COMMA assignment )* )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:139:4: ( SET assignment ( COMMA assignment )* )
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:139:5: SET assignment ( COMMA assignment )*
            	{
            		SET10=(IToken)Match(input,SET,FOLLOW_SET_in_setClause657); 
            			SET10_tree = (IASTNode)adaptor.Create(SET10);
            			root_0 = (IASTNode)adaptor.BecomeRoot(SET10_tree, root_0);

            		PushFollow(FOLLOW_assignment_in_setClause660);
            		assignment11 = assignment();
            		state.followingStackPointer--;

            		adaptor.AddChild(root_0, assignment11.Tree);
            		// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:139:21: ( COMMA assignment )*
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
            				    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:139:22: COMMA assignment
            				    {
            				    	COMMA12=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_setClause663); 
            				    	PushFollow(FOLLOW_assignment_in_setClause666);
            				    	assignment13 = assignment();
            				    	state.followingStackPointer--;

            				    	adaptor.AddChild(root_0, assignment13.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:142:1: assignment : stateField EQ newValue ;
    public HqlParser.assignment_return assignment() // throws RecognitionException [1]
    {   
        HqlParser.assignment_return retval = new HqlParser.assignment_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken EQ15 = null;
        HqlParser.stateField_return stateField14 = default(HqlParser.stateField_return);

        HqlParser.newValue_return newValue16 = default(HqlParser.newValue_return);


        IASTNode EQ15_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:143:2: ( stateField EQ newValue )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:143:4: stateField EQ newValue
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_stateField_in_assignment680);
            	stateField14 = stateField();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, stateField14.Tree);
            	EQ15=(IToken)Match(input,EQ,FOLLOW_EQ_in_assignment682); 
            		EQ15_tree = (IASTNode)adaptor.Create(EQ15);
            		root_0 = (IASTNode)adaptor.BecomeRoot(EQ15_tree, root_0);

            	PushFollow(FOLLOW_newValue_in_assignment685);
            	newValue16 = newValue();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, newValue16.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:148:1: stateField : path ;
    public HqlParser.stateField_return stateField() // throws RecognitionException [1]
    {   
        HqlParser.stateField_return retval = new HqlParser.stateField_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.path_return path17 = default(HqlParser.path_return);



        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:149:2: ( path )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:149:4: path
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_path_in_stateField698);
            	path17 = path();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, path17.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:154:1: newValue : concatenation ;
    public HqlParser.newValue_return newValue() // throws RecognitionException [1]
    {   
        HqlParser.newValue_return retval = new HqlParser.newValue_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.concatenation_return concatenation18 = default(HqlParser.concatenation_return);



        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:155:2: ( concatenation )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:155:4: concatenation
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_concatenation_in_newValue711);
            	concatenation18 = concatenation();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, concatenation18.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:158:1: deleteStatement : DELETE ( optionalFromTokenFromClause ) ( whereClause )? ;
    public HqlParser.deleteStatement_return deleteStatement() // throws RecognitionException [1]
    {   
        HqlParser.deleteStatement_return retval = new HqlParser.deleteStatement_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken DELETE19 = null;
        HqlParser.optionalFromTokenFromClause_return optionalFromTokenFromClause20 = default(HqlParser.optionalFromTokenFromClause_return);

        HqlParser.whereClause_return whereClause21 = default(HqlParser.whereClause_return);


        IASTNode DELETE19_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:159:2: ( DELETE ( optionalFromTokenFromClause ) ( whereClause )? )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:159:4: DELETE ( optionalFromTokenFromClause ) ( whereClause )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	DELETE19=(IToken)Match(input,DELETE,FOLLOW_DELETE_in_deleteStatement722); 
            		DELETE19_tree = (IASTNode)adaptor.Create(DELETE19);
            		root_0 = (IASTNode)adaptor.BecomeRoot(DELETE19_tree, root_0);

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:160:3: ( optionalFromTokenFromClause )
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:160:4: optionalFromTokenFromClause
            	{
            		PushFollow(FOLLOW_optionalFromTokenFromClause_in_deleteStatement728);
            		optionalFromTokenFromClause20 = optionalFromTokenFromClause();
            		state.followingStackPointer--;

            		adaptor.AddChild(root_0, optionalFromTokenFromClause20.Tree);

            	}

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:161:3: ( whereClause )?
            	int alt5 = 2;
            	int LA5_0 = input.LA(1);

            	if ( (LA5_0 == WHERE) )
            	{
            	    alt5 = 1;
            	}
            	switch (alt5) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:161:4: whereClause
            	        {
            	        	PushFollow(FOLLOW_whereClause_in_deleteStatement734);
            	        	whereClause21 = whereClause();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, whereClause21.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:166:1: optionalFromTokenFromClause : optionalFromTokenFromClause2 path ( asAlias )? -> ^( FROM ^( RANGE path ( asAlias )? ) ) ;
    public HqlParser.optionalFromTokenFromClause_return optionalFromTokenFromClause() // throws RecognitionException [1]
    {   
        HqlParser.optionalFromTokenFromClause_return retval = new HqlParser.optionalFromTokenFromClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.optionalFromTokenFromClause2_return optionalFromTokenFromClause222 = default(HqlParser.optionalFromTokenFromClause2_return);

        HqlParser.path_return path23 = default(HqlParser.path_return);

        HqlParser.asAlias_return asAlias24 = default(HqlParser.asAlias_return);


        RewriteRuleSubtreeStream stream_path = new RewriteRuleSubtreeStream(adaptor,"rule path");
        RewriteRuleSubtreeStream stream_asAlias = new RewriteRuleSubtreeStream(adaptor,"rule asAlias");
        RewriteRuleSubtreeStream stream_optionalFromTokenFromClause2 = new RewriteRuleSubtreeStream(adaptor,"rule optionalFromTokenFromClause2");
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:167:2: ( optionalFromTokenFromClause2 path ( asAlias )? -> ^( FROM ^( RANGE path ( asAlias )? ) ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:167:4: optionalFromTokenFromClause2 path ( asAlias )?
            {
            	PushFollow(FOLLOW_optionalFromTokenFromClause2_in_optionalFromTokenFromClause749);
            	optionalFromTokenFromClause222 = optionalFromTokenFromClause2();
            	state.followingStackPointer--;

            	stream_optionalFromTokenFromClause2.Add(optionalFromTokenFromClause222.Tree);
            	PushFollow(FOLLOW_path_in_optionalFromTokenFromClause751);
            	path23 = path();
            	state.followingStackPointer--;

            	stream_path.Add(path23.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:167:38: ( asAlias )?
            	int alt6 = 2;
            	int LA6_0 = input.LA(1);

            	if ( (LA6_0 == AS || LA6_0 == IDENT) )
            	{
            	    alt6 = 1;
            	}
            	switch (alt6) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:167:39: asAlias
            	        {
            	        	PushFollow(FOLLOW_asAlias_in_optionalFromTokenFromClause754);
            	        	asAlias24 = asAlias();
            	        	state.followingStackPointer--;

            	        	stream_asAlias.Add(asAlias24.Tree);

            	        }
            	        break;

            	}



            	// AST REWRITE
            	// elements:          asAlias, path
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (IASTNode)adaptor.GetNilNode();
            	// 168:3: -> ^( FROM ^( RANGE path ( asAlias )? ) )
            	{
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:168:6: ^( FROM ^( RANGE path ( asAlias )? ) )
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(FROM, "FROM"), root_1);

            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:168:13: ^( RANGE path ( asAlias )? )
            	    {
            	    IASTNode root_2 = (IASTNode)adaptor.GetNilNode();
            	    root_2 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(RANGE, "RANGE"), root_2);

            	    adaptor.AddChild(root_2, stream_path.NextTree());
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:168:26: ( asAlias )?
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:171:1: optionalFromTokenFromClause2 : ( FROM )? ;
    public HqlParser.optionalFromTokenFromClause2_return optionalFromTokenFromClause2() // throws RecognitionException [1]
    {   
        HqlParser.optionalFromTokenFromClause2_return retval = new HqlParser.optionalFromTokenFromClause2_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken FROM25 = null;

        IASTNode FROM25_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:172:2: ( ( FROM )? )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:172:4: ( FROM )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:172:4: ( FROM )?
            	int alt7 = 2;
            	int LA7_0 = input.LA(1);

            	if ( (LA7_0 == FROM) )
            	{
            	    alt7 = 1;
            	}
            	switch (alt7) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:172:4: FROM
            	        {
            	        	FROM25=(IToken)Match(input,FROM,FOLLOW_FROM_in_optionalFromTokenFromClause2785); 
            	        		FROM25_tree = (IASTNode)adaptor.Create(FROM25);
            	        		adaptor.AddChild(root_0, FROM25_tree);


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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:175:1: selectStatement : q= queryRule -> ^( QUERY[\"query\"] $q) ;
    public HqlParser.selectStatement_return selectStatement() // throws RecognitionException [1]
    {   
        HqlParser.selectStatement_return retval = new HqlParser.selectStatement_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.queryRule_return q = default(HqlParser.queryRule_return);


        RewriteRuleSubtreeStream stream_queryRule = new RewriteRuleSubtreeStream(adaptor,"rule queryRule");
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:176:2: (q= queryRule -> ^( QUERY[\"query\"] $q) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:176:4: q= queryRule
            {
            	PushFollow(FOLLOW_queryRule_in_selectStatement799);
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
            	// 177:2: -> ^( QUERY[\"query\"] $q)
            	{
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:177:5: ^( QUERY[\"query\"] $q)
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:180:1: insertStatement : INSERT intoClause selectStatement ;
    public HqlParser.insertStatement_return insertStatement() // throws RecognitionException [1]
    {   
        HqlParser.insertStatement_return retval = new HqlParser.insertStatement_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken INSERT26 = null;
        HqlParser.intoClause_return intoClause27 = default(HqlParser.intoClause_return);

        HqlParser.selectStatement_return selectStatement28 = default(HqlParser.selectStatement_return);


        IASTNode INSERT26_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:184:2: ( INSERT intoClause selectStatement )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:184:4: INSERT intoClause selectStatement
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	INSERT26=(IToken)Match(input,INSERT,FOLLOW_INSERT_in_insertStatement828); 
            		INSERT26_tree = (IASTNode)adaptor.Create(INSERT26);
            		root_0 = (IASTNode)adaptor.BecomeRoot(INSERT26_tree, root_0);

            	PushFollow(FOLLOW_intoClause_in_insertStatement831);
            	intoClause27 = intoClause();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, intoClause27.Tree);
            	PushFollow(FOLLOW_selectStatement_in_insertStatement833);
            	selectStatement28 = selectStatement();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, selectStatement28.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:187:1: intoClause : INTO path insertablePropertySpec ;
    public HqlParser.intoClause_return intoClause() // throws RecognitionException [1]
    {   
        HqlParser.intoClause_return retval = new HqlParser.intoClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken INTO29 = null;
        HqlParser.path_return path30 = default(HqlParser.path_return);

        HqlParser.insertablePropertySpec_return insertablePropertySpec31 = default(HqlParser.insertablePropertySpec_return);


        IASTNode INTO29_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:188:2: ( INTO path insertablePropertySpec )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:188:4: INTO path insertablePropertySpec
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	INTO29=(IToken)Match(input,INTO,FOLLOW_INTO_in_intoClause844); 
            		INTO29_tree = (IASTNode)adaptor.Create(INTO29);
            		root_0 = (IASTNode)adaptor.BecomeRoot(INTO29_tree, root_0);

            	PushFollow(FOLLOW_path_in_intoClause847);
            	path30 = path();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, path30.Tree);
            	 WeakKeywords(); 
            	PushFollow(FOLLOW_insertablePropertySpec_in_intoClause851);
            	insertablePropertySpec31 = insertablePropertySpec();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, insertablePropertySpec31.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:191:1: insertablePropertySpec : OPEN primaryExpression ( COMMA primaryExpression )* CLOSE -> ^( RANGE[\"column-spec\"] ( primaryExpression )* ) ;
    public HqlParser.insertablePropertySpec_return insertablePropertySpec() // throws RecognitionException [1]
    {   
        HqlParser.insertablePropertySpec_return retval = new HqlParser.insertablePropertySpec_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken OPEN32 = null;
        IToken COMMA34 = null;
        IToken CLOSE36 = null;
        HqlParser.primaryExpression_return primaryExpression33 = default(HqlParser.primaryExpression_return);

        HqlParser.primaryExpression_return primaryExpression35 = default(HqlParser.primaryExpression_return);


        IASTNode OPEN32_tree=null;
        IASTNode COMMA34_tree=null;
        IASTNode CLOSE36_tree=null;
        RewriteRuleTokenStream stream_COMMA = new RewriteRuleTokenStream(adaptor,"token COMMA");
        RewriteRuleTokenStream stream_CLOSE = new RewriteRuleTokenStream(adaptor,"token CLOSE");
        RewriteRuleTokenStream stream_OPEN = new RewriteRuleTokenStream(adaptor,"token OPEN");
        RewriteRuleSubtreeStream stream_primaryExpression = new RewriteRuleSubtreeStream(adaptor,"rule primaryExpression");
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:192:2: ( OPEN primaryExpression ( COMMA primaryExpression )* CLOSE -> ^( RANGE[\"column-spec\"] ( primaryExpression )* ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:192:4: OPEN primaryExpression ( COMMA primaryExpression )* CLOSE
            {
            	OPEN32=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_insertablePropertySpec862);  
            	stream_OPEN.Add(OPEN32);

            	PushFollow(FOLLOW_primaryExpression_in_insertablePropertySpec864);
            	primaryExpression33 = primaryExpression();
            	state.followingStackPointer--;

            	stream_primaryExpression.Add(primaryExpression33.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:192:27: ( COMMA primaryExpression )*
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
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:192:29: COMMA primaryExpression
            			    {
            			    	COMMA34=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_insertablePropertySpec868);  
            			    	stream_COMMA.Add(COMMA34);

            			    	PushFollow(FOLLOW_primaryExpression_in_insertablePropertySpec870);
            			    	primaryExpression35 = primaryExpression();
            			    	state.followingStackPointer--;

            			    	stream_primaryExpression.Add(primaryExpression35.Tree);

            			    }
            			    break;

            			default:
            			    goto loop8;
            	    }
            	} while (true);

            	loop8:
            		;	// Stops C# compiler whining that label 'loop8' has no statements

            	CLOSE36=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_insertablePropertySpec875);  
            	stream_CLOSE.Add(CLOSE36);



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
            	// 193:3: -> ^( RANGE[\"column-spec\"] ( primaryExpression )* )
            	{
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:193:6: ^( RANGE[\"column-spec\"] ( primaryExpression )* )
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(RANGE, "column-spec"), root_1);

            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:193:29: ( primaryExpression )*
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:199:1: queryRule : selectFrom ( whereClause )? ( groupByClause )? ( orderByClause )? ;
    public HqlParser.queryRule_return queryRule() // throws RecognitionException [1]
    {   
        HqlParser.queryRule_return retval = new HqlParser.queryRule_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.selectFrom_return selectFrom37 = default(HqlParser.selectFrom_return);

        HqlParser.whereClause_return whereClause38 = default(HqlParser.whereClause_return);

        HqlParser.groupByClause_return groupByClause39 = default(HqlParser.groupByClause_return);

        HqlParser.orderByClause_return orderByClause40 = default(HqlParser.orderByClause_return);



        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:200:2: ( selectFrom ( whereClause )? ( groupByClause )? ( orderByClause )? )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:200:4: selectFrom ( whereClause )? ( groupByClause )? ( orderByClause )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_selectFrom_in_queryRule901);
            	selectFrom37 = selectFrom();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, selectFrom37.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:201:3: ( whereClause )?
            	int alt9 = 2;
            	int LA9_0 = input.LA(1);

            	if ( (LA9_0 == WHERE) )
            	{
            	    alt9 = 1;
            	}
            	switch (alt9) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:201:4: whereClause
            	        {
            	        	PushFollow(FOLLOW_whereClause_in_queryRule906);
            	        	whereClause38 = whereClause();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, whereClause38.Tree);

            	        }
            	        break;

            	}

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:202:3: ( groupByClause )?
            	int alt10 = 2;
            	int LA10_0 = input.LA(1);

            	if ( (LA10_0 == GROUP) )
            	{
            	    alt10 = 1;
            	}
            	switch (alt10) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:202:4: groupByClause
            	        {
            	        	PushFollow(FOLLOW_groupByClause_in_queryRule913);
            	        	groupByClause39 = groupByClause();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, groupByClause39.Tree);

            	        }
            	        break;

            	}

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:203:3: ( orderByClause )?
            	int alt11 = 2;
            	int LA11_0 = input.LA(1);

            	if ( (LA11_0 == ORDER) )
            	{
            	    alt11 = 1;
            	}
            	switch (alt11) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:203:4: orderByClause
            	        {
            	        	PushFollow(FOLLOW_orderByClause_in_queryRule920);
            	        	orderByClause40 = orderByClause();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, orderByClause40.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:206:1: selectFrom : (s= selectClause )? (f= fromClause )? -> {$f.tree == null && filter}? ^( SELECT_FROM FROM[\"{filter-implied FROM}\"] ( selectClause )? ) -> ^( SELECT_FROM ( fromClause )? ( selectClause )? ) ;
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
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:207:2: ( (s= selectClause )? (f= fromClause )? -> {$f.tree == null && filter}? ^( SELECT_FROM FROM[\"{filter-implied FROM}\"] ( selectClause )? ) -> ^( SELECT_FROM ( fromClause )? ( selectClause )? ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:207:5: (s= selectClause )? (f= fromClause )?
            {
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:207:5: (s= selectClause )?
            	int alt12 = 2;
            	int LA12_0 = input.LA(1);

            	if ( (LA12_0 == SELECT) )
            	{
            	    alt12 = 1;
            	}
            	switch (alt12) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:207:6: s= selectClause
            	        {
            	        	PushFollow(FOLLOW_selectClause_in_selectFrom938);
            	        	s = selectClause();
            	        	state.followingStackPointer--;

            	        	stream_selectClause.Add(s.Tree);

            	        }
            	        break;

            	}

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:207:23: (f= fromClause )?
            	int alt13 = 2;
            	int LA13_0 = input.LA(1);

            	if ( (LA13_0 == FROM) )
            	{
            	    alt13 = 1;
            	}
            	switch (alt13) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:207:24: f= fromClause
            	        {
            	        	PushFollow(FOLLOW_fromClause_in_selectFrom945);
            	        	f = fromClause();
            	        	state.followingStackPointer--;

            	        	stream_fromClause.Add(f.Tree);

            	        }
            	        break;

            	}


            				if (((f != null) ? ((IASTNode)f.Tree) : null) == null && !filter) 
            					throw new RecognitionException("FROM expected (non-filter queries must contain a FROM clause)");
            			


            	// AST REWRITE
            	// elements:          selectClause, fromClause, selectClause
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (IASTNode)adaptor.GetNilNode();
            	// 212:3: -> {$f.tree == null && filter}? ^( SELECT_FROM FROM[\"{filter-implied FROM}\"] ( selectClause )? )
            	if (((f != null) ? ((IASTNode)f.Tree) : null) == null && filter)
            	{
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:212:35: ^( SELECT_FROM FROM[\"{filter-implied FROM}\"] ( selectClause )? )
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(SELECT_FROM, "SELECT_FROM"), root_1);

            	    adaptor.AddChild(root_1, (IASTNode)adaptor.Create(FROM, "{filter-implied FROM}"));
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:212:79: ( selectClause )?
            	    if ( stream_selectClause.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_selectClause.NextTree());

            	    }
            	    stream_selectClause.Reset();

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}
            	else // 213:3: -> ^( SELECT_FROM ( fromClause )? ( selectClause )? )
            	{
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:213:6: ^( SELECT_FROM ( fromClause )? ( selectClause )? )
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(SELECT_FROM, "SELECT_FROM"), root_1);

            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:213:20: ( fromClause )?
            	    if ( stream_fromClause.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_fromClause.NextTree());

            	    }
            	    stream_fromClause.Reset();
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:213:32: ( selectClause )?
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:217:1: selectClause : SELECT ( DISTINCT )? ( selectedPropertiesList | newExpression | selectObject ) ;
    public HqlParser.selectClause_return selectClause() // throws RecognitionException [1]
    {   
        HqlParser.selectClause_return retval = new HqlParser.selectClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken SELECT41 = null;
        IToken DISTINCT42 = null;
        HqlParser.selectedPropertiesList_return selectedPropertiesList43 = default(HqlParser.selectedPropertiesList_return);

        HqlParser.newExpression_return newExpression44 = default(HqlParser.newExpression_return);

        HqlParser.selectObject_return selectObject45 = default(HqlParser.selectObject_return);


        IASTNode SELECT41_tree=null;
        IASTNode DISTINCT42_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:218:2: ( SELECT ( DISTINCT )? ( selectedPropertiesList | newExpression | selectObject ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:218:4: SELECT ( DISTINCT )? ( selectedPropertiesList | newExpression | selectObject )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	SELECT41=(IToken)Match(input,SELECT,FOLLOW_SELECT_in_selectClause994); 
            		SELECT41_tree = (IASTNode)adaptor.Create(SELECT41);
            		root_0 = (IASTNode)adaptor.BecomeRoot(SELECT41_tree, root_0);

            	 WeakKeywords(); 
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:220:3: ( DISTINCT )?
            	int alt14 = 2;
            	int LA14_0 = input.LA(1);

            	if ( (LA14_0 == DISTINCT) )
            	{
            	    alt14 = 1;
            	}
            	switch (alt14) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:220:4: DISTINCT
            	        {
            	        	DISTINCT42=(IToken)Match(input,DISTINCT,FOLLOW_DISTINCT_in_selectClause1006); 
            	        		DISTINCT42_tree = (IASTNode)adaptor.Create(DISTINCT42);
            	        		adaptor.AddChild(root_0, DISTINCT42_tree);


            	        }
            	        break;

            	}

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:220:15: ( selectedPropertiesList | newExpression | selectObject )
            	int alt15 = 3;
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
            	    alt15 = 1;
            	    }
            	    break;
            	case NEW:
            		{
            	    alt15 = 2;
            	    }
            	    break;
            	case OBJECT:
            		{
            	    alt15 = 3;
            	    }
            	    break;
            		default:
            		    NoViableAltException nvae_d15s0 =
            		        new NoViableAltException("", 15, 0, input);

            		    throw nvae_d15s0;
            	}

            	switch (alt15) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:220:17: selectedPropertiesList
            	        {
            	        	PushFollow(FOLLOW_selectedPropertiesList_in_selectClause1012);
            	        	selectedPropertiesList43 = selectedPropertiesList();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, selectedPropertiesList43.Tree);

            	        }
            	        break;
            	    case 2 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:220:42: newExpression
            	        {
            	        	PushFollow(FOLLOW_newExpression_in_selectClause1016);
            	        	newExpression44 = newExpression();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, newExpression44.Tree);

            	        }
            	        break;
            	    case 3 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:220:58: selectObject
            	        {
            	        	PushFollow(FOLLOW_selectObject_in_selectClause1020);
            	        	selectObject45 = selectObject();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, selectObject45.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:223:1: newExpression : ( NEW path ) op= OPEN selectedPropertiesList CLOSE -> ^( CONSTRUCTOR[$op] path selectedPropertiesList ) ;
    public HqlParser.newExpression_return newExpression() // throws RecognitionException [1]
    {   
        HqlParser.newExpression_return retval = new HqlParser.newExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken op = null;
        IToken NEW46 = null;
        IToken CLOSE49 = null;
        HqlParser.path_return path47 = default(HqlParser.path_return);

        HqlParser.selectedPropertiesList_return selectedPropertiesList48 = default(HqlParser.selectedPropertiesList_return);


        IASTNode op_tree=null;
        IASTNode NEW46_tree=null;
        IASTNode CLOSE49_tree=null;
        RewriteRuleTokenStream stream_CLOSE = new RewriteRuleTokenStream(adaptor,"token CLOSE");
        RewriteRuleTokenStream stream_OPEN = new RewriteRuleTokenStream(adaptor,"token OPEN");
        RewriteRuleTokenStream stream_NEW = new RewriteRuleTokenStream(adaptor,"token NEW");
        RewriteRuleSubtreeStream stream_selectedPropertiesList = new RewriteRuleSubtreeStream(adaptor,"rule selectedPropertiesList");
        RewriteRuleSubtreeStream stream_path = new RewriteRuleSubtreeStream(adaptor,"rule path");
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:224:2: ( ( NEW path ) op= OPEN selectedPropertiesList CLOSE -> ^( CONSTRUCTOR[$op] path selectedPropertiesList ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:224:4: ( NEW path ) op= OPEN selectedPropertiesList CLOSE
            {
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:224:4: ( NEW path )
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:224:5: NEW path
            	{
            		NEW46=(IToken)Match(input,NEW,FOLLOW_NEW_in_newExpression1034);  
            		stream_NEW.Add(NEW46);

            		PushFollow(FOLLOW_path_in_newExpression1036);
            		path47 = path();
            		state.followingStackPointer--;

            		stream_path.Add(path47.Tree);

            	}

            	op=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_newExpression1041);  
            	stream_OPEN.Add(op);

            	PushFollow(FOLLOW_selectedPropertiesList_in_newExpression1043);
            	selectedPropertiesList48 = selectedPropertiesList();
            	state.followingStackPointer--;

            	stream_selectedPropertiesList.Add(selectedPropertiesList48.Tree);
            	CLOSE49=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_newExpression1045);  
            	stream_CLOSE.Add(CLOSE49);



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
            	// 225:3: -> ^( CONSTRUCTOR[$op] path selectedPropertiesList )
            	{
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:225:6: ^( CONSTRUCTOR[$op] path selectedPropertiesList )
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:228:1: selectObject : OBJECT OPEN identifier CLOSE ;
    public HqlParser.selectObject_return selectObject() // throws RecognitionException [1]
    {   
        HqlParser.selectObject_return retval = new HqlParser.selectObject_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken OBJECT50 = null;
        IToken OPEN51 = null;
        IToken CLOSE53 = null;
        HqlParser.identifier_return identifier52 = default(HqlParser.identifier_return);


        IASTNode OBJECT50_tree=null;
        IASTNode OPEN51_tree=null;
        IASTNode CLOSE53_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:229:4: ( OBJECT OPEN identifier CLOSE )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:229:6: OBJECT OPEN identifier CLOSE
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	OBJECT50=(IToken)Match(input,OBJECT,FOLLOW_OBJECT_in_selectObject1071); 
            		OBJECT50_tree = (IASTNode)adaptor.Create(OBJECT50);
            		root_0 = (IASTNode)adaptor.BecomeRoot(OBJECT50_tree, root_0);

            	OPEN51=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_selectObject1074); 
            	PushFollow(FOLLOW_identifier_in_selectObject1077);
            	identifier52 = identifier();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, identifier52.Tree);
            	CLOSE53=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_selectObject1079); 

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:236:1: fromClause : FROM fromRange ( fromJoin | COMMA fromRange )* ;
    public HqlParser.fromClause_return fromClause() // throws RecognitionException [1]
    {   
        HqlParser.fromClause_return retval = new HqlParser.fromClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken FROM54 = null;
        IToken COMMA57 = null;
        HqlParser.fromRange_return fromRange55 = default(HqlParser.fromRange_return);

        HqlParser.fromJoin_return fromJoin56 = default(HqlParser.fromJoin_return);

        HqlParser.fromRange_return fromRange58 = default(HqlParser.fromRange_return);


        IASTNode FROM54_tree=null;
        IASTNode COMMA57_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:237:2: ( FROM fromRange ( fromJoin | COMMA fromRange )* )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:237:4: FROM fromRange ( fromJoin | COMMA fromRange )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	FROM54=(IToken)Match(input,FROM,FOLLOW_FROM_in_fromClause1097); 
            		FROM54_tree = (IASTNode)adaptor.Create(FROM54);
            		root_0 = (IASTNode)adaptor.BecomeRoot(FROM54_tree, root_0);

            	 WeakKeywords(); 
            	PushFollow(FOLLOW_fromRange_in_fromClause1102);
            	fromRange55 = fromRange();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, fromRange55.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:237:40: ( fromJoin | COMMA fromRange )*
            	do 
            	{
            	    int alt16 = 3;
            	    int LA16_0 = input.LA(1);

            	    if ( (LA16_0 == FULL || LA16_0 == INNER || (LA16_0 >= JOIN && LA16_0 <= LEFT) || LA16_0 == RIGHT) )
            	    {
            	        alt16 = 1;
            	    }
            	    else if ( (LA16_0 == COMMA) )
            	    {
            	        alt16 = 2;
            	    }


            	    switch (alt16) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:237:42: fromJoin
            			    {
            			    	PushFollow(FOLLOW_fromJoin_in_fromClause1106);
            			    	fromJoin56 = fromJoin();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, fromJoin56.Tree);

            			    }
            			    break;
            			case 2 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:237:53: COMMA fromRange
            			    {
            			    	COMMA57=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_fromClause1110); 
            			    	 WeakKeywords(); 
            			    	PushFollow(FOLLOW_fromRange_in_fromClause1115);
            			    	fromRange58 = fromRange();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, fromRange58.Tree);

            			    }
            			    break;

            			default:
            			    goto loop16;
            	    }
            	} while (true);

            	loop16:
            		;	// Stops C# compiler whining that label 'loop16' has no statements


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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:240:1: fromJoin : ( ( ( ( LEFT | RIGHT ) ( OUTER )? ) | FULL | INNER )? JOIN ( FETCH )? path ( asAlias )? ( propertyFetch )? ( withClause )? | ( ( ( LEFT | RIGHT ) ( OUTER )? ) | FULL | INNER )? JOIN ( FETCH )? ELEMENTS OPEN path CLOSE ( asAlias )? ( propertyFetch )? ( withClause )? );
    public HqlParser.fromJoin_return fromJoin() // throws RecognitionException [1]
    {   
        HqlParser.fromJoin_return retval = new HqlParser.fromJoin_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken set59 = null;
        IToken OUTER60 = null;
        IToken FULL61 = null;
        IToken INNER62 = null;
        IToken JOIN63 = null;
        IToken FETCH64 = null;
        IToken set69 = null;
        IToken OUTER70 = null;
        IToken FULL71 = null;
        IToken INNER72 = null;
        IToken JOIN73 = null;
        IToken FETCH74 = null;
        IToken ELEMENTS75 = null;
        IToken OPEN76 = null;
        IToken CLOSE78 = null;
        HqlParser.path_return path65 = default(HqlParser.path_return);

        HqlParser.asAlias_return asAlias66 = default(HqlParser.asAlias_return);

        HqlParser.propertyFetch_return propertyFetch67 = default(HqlParser.propertyFetch_return);

        HqlParser.withClause_return withClause68 = default(HqlParser.withClause_return);

        HqlParser.path_return path77 = default(HqlParser.path_return);

        HqlParser.asAlias_return asAlias79 = default(HqlParser.asAlias_return);

        HqlParser.propertyFetch_return propertyFetch80 = default(HqlParser.propertyFetch_return);

        HqlParser.withClause_return withClause81 = default(HqlParser.withClause_return);


        IASTNode set59_tree=null;
        IASTNode OUTER60_tree=null;
        IASTNode FULL61_tree=null;
        IASTNode INNER62_tree=null;
        IASTNode JOIN63_tree=null;
        IASTNode FETCH64_tree=null;
        IASTNode set69_tree=null;
        IASTNode OUTER70_tree=null;
        IASTNode FULL71_tree=null;
        IASTNode INNER72_tree=null;
        IASTNode JOIN73_tree=null;
        IASTNode FETCH74_tree=null;
        IASTNode ELEMENTS75_tree=null;
        IASTNode OPEN76_tree=null;
        IASTNode CLOSE78_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:241:2: ( ( ( ( LEFT | RIGHT ) ( OUTER )? ) | FULL | INNER )? JOIN ( FETCH )? path ( asAlias )? ( propertyFetch )? ( withClause )? | ( ( ( LEFT | RIGHT ) ( OUTER )? ) | FULL | INNER )? JOIN ( FETCH )? ELEMENTS OPEN path CLOSE ( asAlias )? ( propertyFetch )? ( withClause )? )
            int alt29 = 2;
            switch ( input.LA(1) ) 
            {
            case LEFT:
            case RIGHT:
            	{
                int LA29_1 = input.LA(2);

                if ( (LA29_1 == OUTER) )
                {
                    int LA29_5 = input.LA(3);

                    if ( (LA29_5 == JOIN) )
                    {
                        switch ( input.LA(4) ) 
                        {
                        case FETCH:
                        	{
                            int LA29_6 = input.LA(5);

                            if ( (LA29_6 == ELEMENTS) )
                            {
                                alt29 = 2;
                            }
                            else if ( (LA29_6 == IDENT) )
                            {
                                alt29 = 1;
                            }
                            else 
                            {
                                NoViableAltException nvae_d29s6 =
                                    new NoViableAltException("", 29, 6, input);

                                throw nvae_d29s6;
                            }
                            }
                            break;
                        case IDENT:
                        	{
                            alt29 = 1;
                            }
                            break;
                        case ELEMENTS:
                        	{
                            alt29 = 2;
                            }
                            break;
                        	default:
                        	    NoViableAltException nvae_d29s4 =
                        	        new NoViableAltException("", 29, 4, input);

                        	    throw nvae_d29s4;
                        }

                    }
                    else 
                    {
                        NoViableAltException nvae_d29s5 =
                            new NoViableAltException("", 29, 5, input);

                        throw nvae_d29s5;
                    }
                }
                else if ( (LA29_1 == JOIN) )
                {
                    switch ( input.LA(3) ) 
                    {
                    case FETCH:
                    	{
                        int LA29_6 = input.LA(4);

                        if ( (LA29_6 == ELEMENTS) )
                        {
                            alt29 = 2;
                        }
                        else if ( (LA29_6 == IDENT) )
                        {
                            alt29 = 1;
                        }
                        else 
                        {
                            NoViableAltException nvae_d29s6 =
                                new NoViableAltException("", 29, 6, input);

                            throw nvae_d29s6;
                        }
                        }
                        break;
                    case IDENT:
                    	{
                        alt29 = 1;
                        }
                        break;
                    case ELEMENTS:
                    	{
                        alt29 = 2;
                        }
                        break;
                    	default:
                    	    NoViableAltException nvae_d29s4 =
                    	        new NoViableAltException("", 29, 4, input);

                    	    throw nvae_d29s4;
                    }

                }
                else 
                {
                    NoViableAltException nvae_d29s1 =
                        new NoViableAltException("", 29, 1, input);

                    throw nvae_d29s1;
                }
                }
                break;
            case FULL:
            	{
                int LA29_2 = input.LA(2);

                if ( (LA29_2 == JOIN) )
                {
                    switch ( input.LA(3) ) 
                    {
                    case FETCH:
                    	{
                        int LA29_6 = input.LA(4);

                        if ( (LA29_6 == ELEMENTS) )
                        {
                            alt29 = 2;
                        }
                        else if ( (LA29_6 == IDENT) )
                        {
                            alt29 = 1;
                        }
                        else 
                        {
                            NoViableAltException nvae_d29s6 =
                                new NoViableAltException("", 29, 6, input);

                            throw nvae_d29s6;
                        }
                        }
                        break;
                    case IDENT:
                    	{
                        alt29 = 1;
                        }
                        break;
                    case ELEMENTS:
                    	{
                        alt29 = 2;
                        }
                        break;
                    	default:
                    	    NoViableAltException nvae_d29s4 =
                    	        new NoViableAltException("", 29, 4, input);

                    	    throw nvae_d29s4;
                    }

                }
                else 
                {
                    NoViableAltException nvae_d29s2 =
                        new NoViableAltException("", 29, 2, input);

                    throw nvae_d29s2;
                }
                }
                break;
            case INNER:
            	{
                int LA29_3 = input.LA(2);

                if ( (LA29_3 == JOIN) )
                {
                    switch ( input.LA(3) ) 
                    {
                    case FETCH:
                    	{
                        int LA29_6 = input.LA(4);

                        if ( (LA29_6 == ELEMENTS) )
                        {
                            alt29 = 2;
                        }
                        else if ( (LA29_6 == IDENT) )
                        {
                            alt29 = 1;
                        }
                        else 
                        {
                            NoViableAltException nvae_d29s6 =
                                new NoViableAltException("", 29, 6, input);

                            throw nvae_d29s6;
                        }
                        }
                        break;
                    case IDENT:
                    	{
                        alt29 = 1;
                        }
                        break;
                    case ELEMENTS:
                    	{
                        alt29 = 2;
                        }
                        break;
                    	default:
                    	    NoViableAltException nvae_d29s4 =
                    	        new NoViableAltException("", 29, 4, input);

                    	    throw nvae_d29s4;
                    }

                }
                else 
                {
                    NoViableAltException nvae_d29s3 =
                        new NoViableAltException("", 29, 3, input);

                    throw nvae_d29s3;
                }
                }
                break;
            case JOIN:
            	{
                switch ( input.LA(2) ) 
                {
                case FETCH:
                	{
                    int LA29_6 = input.LA(3);

                    if ( (LA29_6 == ELEMENTS) )
                    {
                        alt29 = 2;
                    }
                    else if ( (LA29_6 == IDENT) )
                    {
                        alt29 = 1;
                    }
                    else 
                    {
                        NoViableAltException nvae_d29s6 =
                            new NoViableAltException("", 29, 6, input);

                        throw nvae_d29s6;
                    }
                    }
                    break;
                case IDENT:
                	{
                    alt29 = 1;
                    }
                    break;
                case ELEMENTS:
                	{
                    alt29 = 2;
                    }
                    break;
                	default:
                	    NoViableAltException nvae_d29s4 =
                	        new NoViableAltException("", 29, 4, input);

                	    throw nvae_d29s4;
                }

                }
                break;
            	default:
            	    NoViableAltException nvae_d29s0 =
            	        new NoViableAltException("", 29, 0, input);

            	    throw nvae_d29s0;
            }

            switch (alt29) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:241:4: ( ( ( LEFT | RIGHT ) ( OUTER )? ) | FULL | INNER )? JOIN ( FETCH )? path ( asAlias )? ( propertyFetch )? ( withClause )?
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:241:4: ( ( ( LEFT | RIGHT ) ( OUTER )? ) | FULL | INNER )?
                    	int alt18 = 4;
                    	switch ( input.LA(1) ) 
                    	{
                    	    case LEFT:
                    	    case RIGHT:
                    	    	{
                    	        alt18 = 1;
                    	        }
                    	        break;
                    	    case FULL:
                    	    	{
                    	        alt18 = 2;
                    	        }
                    	        break;
                    	    case INNER:
                    	    	{
                    	        alt18 = 3;
                    	        }
                    	        break;
                    	}

                    	switch (alt18) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:241:6: ( ( LEFT | RIGHT ) ( OUTER )? )
                    	        {
                    	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:241:6: ( ( LEFT | RIGHT ) ( OUTER )? )
                    	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:241:8: ( LEFT | RIGHT ) ( OUTER )?
                    	        	{
                    	        		set59 = (IToken)input.LT(1);
                    	        		if ( input.LA(1) == LEFT || input.LA(1) == RIGHT ) 
                    	        		{
                    	        		    input.Consume();
                    	        		    adaptor.AddChild(root_0, (IASTNode)adaptor.Create(set59));
                    	        		    state.errorRecovery = false;
                    	        		}
                    	        		else 
                    	        		{
                    	        		    MismatchedSetException mse = new MismatchedSetException(null,input);
                    	        		    throw mse;
                    	        		}

                    	        		// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:241:25: ( OUTER )?
                    	        		int alt17 = 2;
                    	        		int LA17_0 = input.LA(1);

                    	        		if ( (LA17_0 == OUTER) )
                    	        		{
                    	        		    alt17 = 1;
                    	        		}
                    	        		switch (alt17) 
                    	        		{
                    	        		    case 1 :
                    	        		        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:241:26: OUTER
                    	        		        {
                    	        		        	OUTER60=(IToken)Match(input,OUTER,FOLLOW_OUTER_in_fromJoin1144); 
                    	        		        		OUTER60_tree = (IASTNode)adaptor.Create(OUTER60);
                    	        		        		adaptor.AddChild(root_0, OUTER60_tree);


                    	        		        }
                    	        		        break;

                    	        		}


                    	        	}


                    	        }
                    	        break;
                    	    case 2 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:241:38: FULL
                    	        {
                    	        	FULL61=(IToken)Match(input,FULL,FOLLOW_FULL_in_fromJoin1152); 
                    	        		FULL61_tree = (IASTNode)adaptor.Create(FULL61);
                    	        		adaptor.AddChild(root_0, FULL61_tree);


                    	        }
                    	        break;
                    	    case 3 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:241:45: INNER
                    	        {
                    	        	INNER62=(IToken)Match(input,INNER,FOLLOW_INNER_in_fromJoin1156); 
                    	        		INNER62_tree = (IASTNode)adaptor.Create(INNER62);
                    	        		adaptor.AddChild(root_0, INNER62_tree);


                    	        }
                    	        break;

                    	}

                    	JOIN63=(IToken)Match(input,JOIN,FOLLOW_JOIN_in_fromJoin1161); 
                    		JOIN63_tree = (IASTNode)adaptor.Create(JOIN63);
                    		root_0 = (IASTNode)adaptor.BecomeRoot(JOIN63_tree, root_0);

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:241:60: ( FETCH )?
                    	int alt19 = 2;
                    	int LA19_0 = input.LA(1);

                    	if ( (LA19_0 == FETCH) )
                    	{
                    	    alt19 = 1;
                    	}
                    	switch (alt19) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:241:61: FETCH
                    	        {
                    	        	FETCH64=(IToken)Match(input,FETCH,FOLLOW_FETCH_in_fromJoin1165); 
                    	        		FETCH64_tree = (IASTNode)adaptor.Create(FETCH64);
                    	        		adaptor.AddChild(root_0, FETCH64_tree);


                    	        }
                    	        break;

                    	}

                    	PushFollow(FOLLOW_path_in_fromJoin1169);
                    	path65 = path();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, path65.Tree);
                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:241:74: ( asAlias )?
                    	int alt20 = 2;
                    	int LA20_0 = input.LA(1);

                    	if ( (LA20_0 == AS || LA20_0 == IDENT) )
                    	{
                    	    alt20 = 1;
                    	}
                    	switch (alt20) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:241:75: asAlias
                    	        {
                    	        	PushFollow(FOLLOW_asAlias_in_fromJoin1172);
                    	        	asAlias66 = asAlias();
                    	        	state.followingStackPointer--;

                    	        	adaptor.AddChild(root_0, asAlias66.Tree);

                    	        }
                    	        break;

                    	}

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:241:85: ( propertyFetch )?
                    	int alt21 = 2;
                    	int LA21_0 = input.LA(1);

                    	if ( (LA21_0 == FETCH) )
                    	{
                    	    alt21 = 1;
                    	}
                    	switch (alt21) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:241:86: propertyFetch
                    	        {
                    	        	PushFollow(FOLLOW_propertyFetch_in_fromJoin1177);
                    	        	propertyFetch67 = propertyFetch();
                    	        	state.followingStackPointer--;

                    	        	adaptor.AddChild(root_0, propertyFetch67.Tree);

                    	        }
                    	        break;

                    	}

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:241:102: ( withClause )?
                    	int alt22 = 2;
                    	int LA22_0 = input.LA(1);

                    	if ( (LA22_0 == WITH) )
                    	{
                    	    alt22 = 1;
                    	}
                    	switch (alt22) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:241:103: withClause
                    	        {
                    	        	PushFollow(FOLLOW_withClause_in_fromJoin1182);
                    	        	withClause68 = withClause();
                    	        	state.followingStackPointer--;

                    	        	adaptor.AddChild(root_0, withClause68.Tree);

                    	        }
                    	        break;

                    	}


                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:242:4: ( ( ( LEFT | RIGHT ) ( OUTER )? ) | FULL | INNER )? JOIN ( FETCH )? ELEMENTS OPEN path CLOSE ( asAlias )? ( propertyFetch )? ( withClause )?
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:242:4: ( ( ( LEFT | RIGHT ) ( OUTER )? ) | FULL | INNER )?
                    	int alt24 = 4;
                    	switch ( input.LA(1) ) 
                    	{
                    	    case LEFT:
                    	    case RIGHT:
                    	    	{
                    	        alt24 = 1;
                    	        }
                    	        break;
                    	    case FULL:
                    	    	{
                    	        alt24 = 2;
                    	        }
                    	        break;
                    	    case INNER:
                    	    	{
                    	        alt24 = 3;
                    	        }
                    	        break;
                    	}

                    	switch (alt24) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:242:6: ( ( LEFT | RIGHT ) ( OUTER )? )
                    	        {
                    	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:242:6: ( ( LEFT | RIGHT ) ( OUTER )? )
                    	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:242:8: ( LEFT | RIGHT ) ( OUTER )?
                    	        	{
                    	        		set69 = (IToken)input.LT(1);
                    	        		if ( input.LA(1) == LEFT || input.LA(1) == RIGHT ) 
                    	        		{
                    	        		    input.Consume();
                    	        		    adaptor.AddChild(root_0, (IASTNode)adaptor.Create(set69));
                    	        		    state.errorRecovery = false;
                    	        		}
                    	        		else 
                    	        		{
                    	        		    MismatchedSetException mse = new MismatchedSetException(null,input);
                    	        		    throw mse;
                    	        		}

                    	        		// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:242:25: ( OUTER )?
                    	        		int alt23 = 2;
                    	        		int LA23_0 = input.LA(1);

                    	        		if ( (LA23_0 == OUTER) )
                    	        		{
                    	        		    alt23 = 1;
                    	        		}
                    	        		switch (alt23) 
                    	        		{
                    	        		    case 1 :
                    	        		        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:242:26: OUTER
                    	        		        {
                    	        		        	OUTER70=(IToken)Match(input,OUTER,FOLLOW_OUTER_in_fromJoin1204); 
                    	        		        		OUTER70_tree = (IASTNode)adaptor.Create(OUTER70);
                    	        		        		adaptor.AddChild(root_0, OUTER70_tree);


                    	        		        }
                    	        		        break;

                    	        		}


                    	        	}


                    	        }
                    	        break;
                    	    case 2 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:242:38: FULL
                    	        {
                    	        	FULL71=(IToken)Match(input,FULL,FOLLOW_FULL_in_fromJoin1212); 
                    	        		FULL71_tree = (IASTNode)adaptor.Create(FULL71);
                    	        		adaptor.AddChild(root_0, FULL71_tree);


                    	        }
                    	        break;
                    	    case 3 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:242:45: INNER
                    	        {
                    	        	INNER72=(IToken)Match(input,INNER,FOLLOW_INNER_in_fromJoin1216); 
                    	        		INNER72_tree = (IASTNode)adaptor.Create(INNER72);
                    	        		adaptor.AddChild(root_0, INNER72_tree);


                    	        }
                    	        break;

                    	}

                    	JOIN73=(IToken)Match(input,JOIN,FOLLOW_JOIN_in_fromJoin1221); 
                    		JOIN73_tree = (IASTNode)adaptor.Create(JOIN73);
                    		root_0 = (IASTNode)adaptor.BecomeRoot(JOIN73_tree, root_0);

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:242:60: ( FETCH )?
                    	int alt25 = 2;
                    	int LA25_0 = input.LA(1);

                    	if ( (LA25_0 == FETCH) )
                    	{
                    	    alt25 = 1;
                    	}
                    	switch (alt25) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:242:61: FETCH
                    	        {
                    	        	FETCH74=(IToken)Match(input,FETCH,FOLLOW_FETCH_in_fromJoin1225); 
                    	        		FETCH74_tree = (IASTNode)adaptor.Create(FETCH74);
                    	        		adaptor.AddChild(root_0, FETCH74_tree);


                    	        }
                    	        break;

                    	}

                    	ELEMENTS75=(IToken)Match(input,ELEMENTS,FOLLOW_ELEMENTS_in_fromJoin1229); 
                    	OPEN76=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_fromJoin1232); 
                    	PushFollow(FOLLOW_path_in_fromJoin1235);
                    	path77 = path();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, path77.Tree);
                    	CLOSE78=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_fromJoin1237); 
                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:242:97: ( asAlias )?
                    	int alt26 = 2;
                    	int LA26_0 = input.LA(1);

                    	if ( (LA26_0 == AS || LA26_0 == IDENT) )
                    	{
                    	    alt26 = 1;
                    	}
                    	switch (alt26) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:242:98: asAlias
                    	        {
                    	        	PushFollow(FOLLOW_asAlias_in_fromJoin1241);
                    	        	asAlias79 = asAlias();
                    	        	state.followingStackPointer--;

                    	        	adaptor.AddChild(root_0, asAlias79.Tree);

                    	        }
                    	        break;

                    	}

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:242:108: ( propertyFetch )?
                    	int alt27 = 2;
                    	int LA27_0 = input.LA(1);

                    	if ( (LA27_0 == FETCH) )
                    	{
                    	    alt27 = 1;
                    	}
                    	switch (alt27) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:242:109: propertyFetch
                    	        {
                    	        	PushFollow(FOLLOW_propertyFetch_in_fromJoin1246);
                    	        	propertyFetch80 = propertyFetch();
                    	        	state.followingStackPointer--;

                    	        	adaptor.AddChild(root_0, propertyFetch80.Tree);

                    	        }
                    	        break;

                    	}

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:242:125: ( withClause )?
                    	int alt28 = 2;
                    	int LA28_0 = input.LA(1);

                    	if ( (LA28_0 == WITH) )
                    	{
                    	    alt28 = 1;
                    	}
                    	switch (alt28) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:242:126: withClause
                    	        {
                    	        	PushFollow(FOLLOW_withClause_in_fromJoin1251);
                    	        	withClause81 = withClause();
                    	        	state.followingStackPointer--;

                    	        	adaptor.AddChild(root_0, withClause81.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:245:1: withClause : WITH logicalExpression ;
    public HqlParser.withClause_return withClause() // throws RecognitionException [1]
    {   
        HqlParser.withClause_return retval = new HqlParser.withClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken WITH82 = null;
        HqlParser.logicalExpression_return logicalExpression83 = default(HqlParser.logicalExpression_return);


        IASTNode WITH82_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:246:2: ( WITH logicalExpression )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:246:4: WITH logicalExpression
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	WITH82=(IToken)Match(input,WITH,FOLLOW_WITH_in_withClause1264); 
            		WITH82_tree = (IASTNode)adaptor.Create(WITH82);
            		root_0 = (IASTNode)adaptor.BecomeRoot(WITH82_tree, root_0);

            	PushFollow(FOLLOW_logicalExpression_in_withClause1267);
            	logicalExpression83 = logicalExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, logicalExpression83.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:249:1: fromRange : ( fromClassOrOuterQueryPath | inClassDeclaration | inCollectionDeclaration | inCollectionElementsDeclaration );
    public HqlParser.fromRange_return fromRange() // throws RecognitionException [1]
    {   
        HqlParser.fromRange_return retval = new HqlParser.fromRange_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.fromClassOrOuterQueryPath_return fromClassOrOuterQueryPath84 = default(HqlParser.fromClassOrOuterQueryPath_return);

        HqlParser.inClassDeclaration_return inClassDeclaration85 = default(HqlParser.inClassDeclaration_return);

        HqlParser.inCollectionDeclaration_return inCollectionDeclaration86 = default(HqlParser.inCollectionDeclaration_return);

        HqlParser.inCollectionElementsDeclaration_return inCollectionElementsDeclaration87 = default(HqlParser.inCollectionElementsDeclaration_return);



        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:250:2: ( fromClassOrOuterQueryPath | inClassDeclaration | inCollectionDeclaration | inCollectionElementsDeclaration )
            int alt30 = 4;
            switch ( input.LA(1) ) 
            {
            case IDENT:
            	{
                int LA30_1 = input.LA(2);

                if ( (LA30_1 == EOF || LA30_1 == AS || LA30_1 == DOT || LA30_1 == FETCH || (LA30_1 >= FULL && LA30_1 <= GROUP) || LA30_1 == INNER || (LA30_1 >= JOIN && LA30_1 <= LEFT) || LA30_1 == ORDER || LA30_1 == RIGHT || LA30_1 == UNION || LA30_1 == WHERE || LA30_1 == COMMA || LA30_1 == CLOSE || LA30_1 == IDENT) )
                {
                    alt30 = 1;
                }
                else if ( (LA30_1 == IN) )
                {
                    int LA30_5 = input.LA(3);

                    if ( (LA30_5 == ELEMENTS) )
                    {
                        alt30 = 4;
                    }
                    else if ( (LA30_5 == CLASS || LA30_5 == IDENT) )
                    {
                        alt30 = 2;
                    }
                    else 
                    {
                        NoViableAltException nvae_d30s5 =
                            new NoViableAltException("", 30, 5, input);

                        throw nvae_d30s5;
                    }
                }
                else 
                {
                    NoViableAltException nvae_d30s1 =
                        new NoViableAltException("", 30, 1, input);

                    throw nvae_d30s1;
                }
                }
                break;
            case IN:
            	{
                alt30 = 3;
                }
                break;
            case ELEMENTS:
            	{
                alt30 = 4;
                }
                break;
            	default:
            	    NoViableAltException nvae_d30s0 =
            	        new NoViableAltException("", 30, 0, input);

            	    throw nvae_d30s0;
            }

            switch (alt30) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:250:4: fromClassOrOuterQueryPath
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_fromClassOrOuterQueryPath_in_fromRange1278);
                    	fromClassOrOuterQueryPath84 = fromClassOrOuterQueryPath();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, fromClassOrOuterQueryPath84.Tree);

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:251:4: inClassDeclaration
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_inClassDeclaration_in_fromRange1283);
                    	inClassDeclaration85 = inClassDeclaration();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, inClassDeclaration85.Tree);

                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:252:4: inCollectionDeclaration
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_inCollectionDeclaration_in_fromRange1288);
                    	inCollectionDeclaration86 = inCollectionDeclaration();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, inCollectionDeclaration86.Tree);

                    }
                    break;
                case 4 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:253:4: inCollectionElementsDeclaration
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_inCollectionElementsDeclaration_in_fromRange1293);
                    	inCollectionElementsDeclaration87 = inCollectionElementsDeclaration();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, inCollectionElementsDeclaration87.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:256:1: fromClassOrOuterQueryPath : path ( asAlias )? ( propertyFetch )? -> ^( RANGE path ( asAlias )? ( propertyFetch )? ) ;
    public HqlParser.fromClassOrOuterQueryPath_return fromClassOrOuterQueryPath() // throws RecognitionException [1]
    {   
        HqlParser.fromClassOrOuterQueryPath_return retval = new HqlParser.fromClassOrOuterQueryPath_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.path_return path88 = default(HqlParser.path_return);

        HqlParser.asAlias_return asAlias89 = default(HqlParser.asAlias_return);

        HqlParser.propertyFetch_return propertyFetch90 = default(HqlParser.propertyFetch_return);


        RewriteRuleSubtreeStream stream_path = new RewriteRuleSubtreeStream(adaptor,"rule path");
        RewriteRuleSubtreeStream stream_propertyFetch = new RewriteRuleSubtreeStream(adaptor,"rule propertyFetch");
        RewriteRuleSubtreeStream stream_asAlias = new RewriteRuleSubtreeStream(adaptor,"rule asAlias");
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:257:2: ( path ( asAlias )? ( propertyFetch )? -> ^( RANGE path ( asAlias )? ( propertyFetch )? ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:257:4: path ( asAlias )? ( propertyFetch )?
            {
            	PushFollow(FOLLOW_path_in_fromClassOrOuterQueryPath1305);
            	path88 = path();
            	state.followingStackPointer--;

            	stream_path.Add(path88.Tree);
            	 WeakKeywords(); 
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:257:29: ( asAlias )?
            	int alt31 = 2;
            	int LA31_0 = input.LA(1);

            	if ( (LA31_0 == AS || LA31_0 == IDENT) )
            	{
            	    alt31 = 1;
            	}
            	switch (alt31) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:257:30: asAlias
            	        {
            	        	PushFollow(FOLLOW_asAlias_in_fromClassOrOuterQueryPath1310);
            	        	asAlias89 = asAlias();
            	        	state.followingStackPointer--;

            	        	stream_asAlias.Add(asAlias89.Tree);

            	        }
            	        break;

            	}

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:257:40: ( propertyFetch )?
            	int alt32 = 2;
            	int LA32_0 = input.LA(1);

            	if ( (LA32_0 == FETCH) )
            	{
            	    alt32 = 1;
            	}
            	switch (alt32) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:257:41: propertyFetch
            	        {
            	        	PushFollow(FOLLOW_propertyFetch_in_fromClassOrOuterQueryPath1315);
            	        	propertyFetch90 = propertyFetch();
            	        	state.followingStackPointer--;

            	        	stream_propertyFetch.Add(propertyFetch90.Tree);

            	        }
            	        break;

            	}



            	// AST REWRITE
            	// elements:          path, asAlias, propertyFetch
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (IASTNode)adaptor.GetNilNode();
            	// 258:3: -> ^( RANGE path ( asAlias )? ( propertyFetch )? )
            	{
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:258:6: ^( RANGE path ( asAlias )? ( propertyFetch )? )
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(RANGE, "RANGE"), root_1);

            	    adaptor.AddChild(root_1, stream_path.NextTree());
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:258:19: ( asAlias )?
            	    if ( stream_asAlias.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_asAlias.NextTree());

            	    }
            	    stream_asAlias.Reset();
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:258:28: ( propertyFetch )?
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:261:1: inClassDeclaration : alias IN ( CLASS )? path -> ^( RANGE path alias ) ;
    public HqlParser.inClassDeclaration_return inClassDeclaration() // throws RecognitionException [1]
    {   
        HqlParser.inClassDeclaration_return retval = new HqlParser.inClassDeclaration_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken IN92 = null;
        IToken CLASS93 = null;
        HqlParser.alias_return alias91 = default(HqlParser.alias_return);

        HqlParser.path_return path94 = default(HqlParser.path_return);


        IASTNode IN92_tree=null;
        IASTNode CLASS93_tree=null;
        RewriteRuleTokenStream stream_CLASS = new RewriteRuleTokenStream(adaptor,"token CLASS");
        RewriteRuleTokenStream stream_IN = new RewriteRuleTokenStream(adaptor,"token IN");
        RewriteRuleSubtreeStream stream_path = new RewriteRuleSubtreeStream(adaptor,"rule path");
        RewriteRuleSubtreeStream stream_alias = new RewriteRuleSubtreeStream(adaptor,"rule alias");
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:262:2: ( alias IN ( CLASS )? path -> ^( RANGE path alias ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:262:4: alias IN ( CLASS )? path
            {
            	PushFollow(FOLLOW_alias_in_inClassDeclaration1345);
            	alias91 = alias();
            	state.followingStackPointer--;

            	stream_alias.Add(alias91.Tree);
            	IN92=(IToken)Match(input,IN,FOLLOW_IN_in_inClassDeclaration1347);  
            	stream_IN.Add(IN92);

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:262:13: ( CLASS )?
            	int alt33 = 2;
            	int LA33_0 = input.LA(1);

            	if ( (LA33_0 == CLASS) )
            	{
            	    alt33 = 1;
            	}
            	switch (alt33) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:262:13: CLASS
            	        {
            	        	CLASS93=(IToken)Match(input,CLASS,FOLLOW_CLASS_in_inClassDeclaration1349);  
            	        	stream_CLASS.Add(CLASS93);


            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_path_in_inClassDeclaration1352);
            	path94 = path();
            	state.followingStackPointer--;

            	stream_path.Add(path94.Tree);


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
            	// 263:3: -> ^( RANGE path alias )
            	{
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:263:6: ^( RANGE path alias )
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:266:1: inCollectionDeclaration : IN OPEN path CLOSE alias -> ^( JOIN[\"join\"] INNER[\"inner\"] path alias ) ;
    public HqlParser.inCollectionDeclaration_return inCollectionDeclaration() // throws RecognitionException [1]
    {   
        HqlParser.inCollectionDeclaration_return retval = new HqlParser.inCollectionDeclaration_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken IN95 = null;
        IToken OPEN96 = null;
        IToken CLOSE98 = null;
        HqlParser.path_return path97 = default(HqlParser.path_return);

        HqlParser.alias_return alias99 = default(HqlParser.alias_return);


        IASTNode IN95_tree=null;
        IASTNode OPEN96_tree=null;
        IASTNode CLOSE98_tree=null;
        RewriteRuleTokenStream stream_CLOSE = new RewriteRuleTokenStream(adaptor,"token CLOSE");
        RewriteRuleTokenStream stream_OPEN = new RewriteRuleTokenStream(adaptor,"token OPEN");
        RewriteRuleTokenStream stream_IN = new RewriteRuleTokenStream(adaptor,"token IN");
        RewriteRuleSubtreeStream stream_path = new RewriteRuleSubtreeStream(adaptor,"rule path");
        RewriteRuleSubtreeStream stream_alias = new RewriteRuleSubtreeStream(adaptor,"rule alias");
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:267:5: ( IN OPEN path CLOSE alias -> ^( JOIN[\"join\"] INNER[\"inner\"] path alias ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:267:7: IN OPEN path CLOSE alias
            {
            	IN95=(IToken)Match(input,IN,FOLLOW_IN_in_inCollectionDeclaration1380);  
            	stream_IN.Add(IN95);

            	OPEN96=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_inCollectionDeclaration1382);  
            	stream_OPEN.Add(OPEN96);

            	PushFollow(FOLLOW_path_in_inCollectionDeclaration1384);
            	path97 = path();
            	state.followingStackPointer--;

            	stream_path.Add(path97.Tree);
            	CLOSE98=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_inCollectionDeclaration1386);  
            	stream_CLOSE.Add(CLOSE98);

            	PushFollow(FOLLOW_alias_in_inCollectionDeclaration1388);
            	alias99 = alias();
            	state.followingStackPointer--;

            	stream_alias.Add(alias99.Tree);


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
            	// 268:6: -> ^( JOIN[\"join\"] INNER[\"inner\"] path alias )
            	{
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:268:9: ^( JOIN[\"join\"] INNER[\"inner\"] path alias )
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:271:1: inCollectionElementsDeclaration : ( alias IN ELEMENTS OPEN path CLOSE -> ^( JOIN[\"join\"] INNER[\"inner\"] path alias ) | ELEMENTS OPEN path CLOSE AS alias -> ^( JOIN[\"join\"] INNER[\"inner\"] path alias ) );
    public HqlParser.inCollectionElementsDeclaration_return inCollectionElementsDeclaration() // throws RecognitionException [1]
    {   
        HqlParser.inCollectionElementsDeclaration_return retval = new HqlParser.inCollectionElementsDeclaration_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken IN101 = null;
        IToken ELEMENTS102 = null;
        IToken OPEN103 = null;
        IToken CLOSE105 = null;
        IToken ELEMENTS106 = null;
        IToken OPEN107 = null;
        IToken CLOSE109 = null;
        IToken AS110 = null;
        HqlParser.alias_return alias100 = default(HqlParser.alias_return);

        HqlParser.path_return path104 = default(HqlParser.path_return);

        HqlParser.path_return path108 = default(HqlParser.path_return);

        HqlParser.alias_return alias111 = default(HqlParser.alias_return);


        IASTNode IN101_tree=null;
        IASTNode ELEMENTS102_tree=null;
        IASTNode OPEN103_tree=null;
        IASTNode CLOSE105_tree=null;
        IASTNode ELEMENTS106_tree=null;
        IASTNode OPEN107_tree=null;
        IASTNode CLOSE109_tree=null;
        IASTNode AS110_tree=null;
        RewriteRuleTokenStream stream_CLOSE = new RewriteRuleTokenStream(adaptor,"token CLOSE");
        RewriteRuleTokenStream stream_ELEMENTS = new RewriteRuleTokenStream(adaptor,"token ELEMENTS");
        RewriteRuleTokenStream stream_AS = new RewriteRuleTokenStream(adaptor,"token AS");
        RewriteRuleTokenStream stream_OPEN = new RewriteRuleTokenStream(adaptor,"token OPEN");
        RewriteRuleTokenStream stream_IN = new RewriteRuleTokenStream(adaptor,"token IN");
        RewriteRuleSubtreeStream stream_path = new RewriteRuleSubtreeStream(adaptor,"rule path");
        RewriteRuleSubtreeStream stream_alias = new RewriteRuleSubtreeStream(adaptor,"rule alias");
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:272:2: ( alias IN ELEMENTS OPEN path CLOSE -> ^( JOIN[\"join\"] INNER[\"inner\"] path alias ) | ELEMENTS OPEN path CLOSE AS alias -> ^( JOIN[\"join\"] INNER[\"inner\"] path alias ) )
            int alt34 = 2;
            int LA34_0 = input.LA(1);

            if ( (LA34_0 == IDENT) )
            {
                alt34 = 1;
            }
            else if ( (LA34_0 == ELEMENTS) )
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
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:272:4: alias IN ELEMENTS OPEN path CLOSE
                    {
                    	PushFollow(FOLLOW_alias_in_inCollectionElementsDeclaration1422);
                    	alias100 = alias();
                    	state.followingStackPointer--;

                    	stream_alias.Add(alias100.Tree);
                    	IN101=(IToken)Match(input,IN,FOLLOW_IN_in_inCollectionElementsDeclaration1424);  
                    	stream_IN.Add(IN101);

                    	ELEMENTS102=(IToken)Match(input,ELEMENTS,FOLLOW_ELEMENTS_in_inCollectionElementsDeclaration1426);  
                    	stream_ELEMENTS.Add(ELEMENTS102);

                    	OPEN103=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_inCollectionElementsDeclaration1428);  
                    	stream_OPEN.Add(OPEN103);

                    	PushFollow(FOLLOW_path_in_inCollectionElementsDeclaration1430);
                    	path104 = path();
                    	state.followingStackPointer--;

                    	stream_path.Add(path104.Tree);
                    	CLOSE105=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_inCollectionElementsDeclaration1432);  
                    	stream_CLOSE.Add(CLOSE105);



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
                    	// 273:3: -> ^( JOIN[\"join\"] INNER[\"inner\"] path alias )
                    	{
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:273:6: ^( JOIN[\"join\"] INNER[\"inner\"] path alias )
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
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:274:4: ELEMENTS OPEN path CLOSE AS alias
                    {
                    	ELEMENTS106=(IToken)Match(input,ELEMENTS,FOLLOW_ELEMENTS_in_inCollectionElementsDeclaration1454);  
                    	stream_ELEMENTS.Add(ELEMENTS106);

                    	OPEN107=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_inCollectionElementsDeclaration1456);  
                    	stream_OPEN.Add(OPEN107);

                    	PushFollow(FOLLOW_path_in_inCollectionElementsDeclaration1458);
                    	path108 = path();
                    	state.followingStackPointer--;

                    	stream_path.Add(path108.Tree);
                    	CLOSE109=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_inCollectionElementsDeclaration1460);  
                    	stream_CLOSE.Add(CLOSE109);

                    	AS110=(IToken)Match(input,AS,FOLLOW_AS_in_inCollectionElementsDeclaration1462);  
                    	stream_AS.Add(AS110);

                    	PushFollow(FOLLOW_alias_in_inCollectionElementsDeclaration1464);
                    	alias111 = alias();
                    	state.followingStackPointer--;

                    	stream_alias.Add(alias111.Tree);


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
                    	// 275:3: -> ^( JOIN[\"join\"] INNER[\"inner\"] path alias )
                    	{
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:275:6: ^( JOIN[\"join\"] INNER[\"inner\"] path alias )
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:279:1: asAlias : ( AS )? alias ;
    public HqlParser.asAlias_return asAlias() // throws RecognitionException [1]
    {   
        HqlParser.asAlias_return retval = new HqlParser.asAlias_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken AS112 = null;
        HqlParser.alias_return alias113 = default(HqlParser.alias_return);


        IASTNode AS112_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:280:2: ( ( AS )? alias )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:280:4: ( AS )? alias
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:280:4: ( AS )?
            	int alt35 = 2;
            	int LA35_0 = input.LA(1);

            	if ( (LA35_0 == AS) )
            	{
            	    alt35 = 1;
            	}
            	switch (alt35) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:280:5: AS
            	        {
            	        	AS112=(IToken)Match(input,AS,FOLLOW_AS_in_asAlias1496); 

            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_alias_in_asAlias1501);
            	alias113 = alias();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, alias113.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:282:1: alias : i= identifier -> ^( ALIAS[$i.start] ) ;
    public HqlParser.alias_return alias() // throws RecognitionException [1]
    {   
        HqlParser.alias_return retval = new HqlParser.alias_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.identifier_return i = default(HqlParser.identifier_return);


        RewriteRuleSubtreeStream stream_identifier = new RewriteRuleSubtreeStream(adaptor,"rule identifier");
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:283:2: (i= identifier -> ^( ALIAS[$i.start] ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:283:4: i= identifier
            {
            	PushFollow(FOLLOW_identifier_in_alias1513);
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
            	// 284:2: -> ^( ALIAS[$i.start] )
            	{
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:284:5: ^( ALIAS[$i.start] )
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:287:1: propertyFetch : FETCH ALL PROPERTIES ;
    public HqlParser.propertyFetch_return propertyFetch() // throws RecognitionException [1]
    {   
        HqlParser.propertyFetch_return retval = new HqlParser.propertyFetch_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken FETCH114 = null;
        IToken ALL115 = null;
        IToken PROPERTIES116 = null;

        IASTNode FETCH114_tree=null;
        IASTNode ALL115_tree=null;
        IASTNode PROPERTIES116_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:288:2: ( FETCH ALL PROPERTIES )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:288:4: FETCH ALL PROPERTIES
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	FETCH114=(IToken)Match(input,FETCH,FOLLOW_FETCH_in_propertyFetch1532); 
            		FETCH114_tree = (IASTNode)adaptor.Create(FETCH114);
            		adaptor.AddChild(root_0, FETCH114_tree);

            	ALL115=(IToken)Match(input,ALL,FOLLOW_ALL_in_propertyFetch1534); 
            	PROPERTIES116=(IToken)Match(input,PROPERTIES,FOLLOW_PROPERTIES_in_propertyFetch1537); 

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:291:1: groupByClause : GROUP 'by' expression ( COMMA expression )* ( havingClause )? ;
    public HqlParser.groupByClause_return groupByClause() // throws RecognitionException [1]
    {   
        HqlParser.groupByClause_return retval = new HqlParser.groupByClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken GROUP117 = null;
        IToken string_literal118 = null;
        IToken COMMA120 = null;
        HqlParser.expression_return expression119 = default(HqlParser.expression_return);

        HqlParser.expression_return expression121 = default(HqlParser.expression_return);

        HqlParser.havingClause_return havingClause122 = default(HqlParser.havingClause_return);


        IASTNode GROUP117_tree=null;
        IASTNode string_literal118_tree=null;
        IASTNode COMMA120_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:292:2: ( GROUP 'by' expression ( COMMA expression )* ( havingClause )? )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:292:4: GROUP 'by' expression ( COMMA expression )* ( havingClause )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	GROUP117=(IToken)Match(input,GROUP,FOLLOW_GROUP_in_groupByClause1549); 
            		GROUP117_tree = (IASTNode)adaptor.Create(GROUP117);
            		root_0 = (IASTNode)adaptor.BecomeRoot(GROUP117_tree, root_0);

            	string_literal118=(IToken)Match(input,LITERAL_by,FOLLOW_LITERAL_by_in_groupByClause1555); 
            	PushFollow(FOLLOW_expression_in_groupByClause1558);
            	expression119 = expression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, expression119.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:293:20: ( COMMA expression )*
            	do 
            	{
            	    int alt36 = 2;
            	    int LA36_0 = input.LA(1);

            	    if ( (LA36_0 == COMMA) )
            	    {
            	        alt36 = 1;
            	    }


            	    switch (alt36) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:293:22: COMMA expression
            			    {
            			    	COMMA120=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_groupByClause1562); 
            			    	PushFollow(FOLLOW_expression_in_groupByClause1565);
            			    	expression121 = expression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, expression121.Tree);

            			    }
            			    break;

            			default:
            			    goto loop36;
            	    }
            	} while (true);

            	loop36:
            		;	// Stops C# compiler whining that label 'loop36' has no statements

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:294:3: ( havingClause )?
            	int alt37 = 2;
            	int LA37_0 = input.LA(1);

            	if ( (LA37_0 == HAVING) )
            	{
            	    alt37 = 1;
            	}
            	switch (alt37) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:294:4: havingClause
            	        {
            	        	PushFollow(FOLLOW_havingClause_in_groupByClause1573);
            	        	havingClause122 = havingClause();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, havingClause122.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:297:1: orderByClause : ORDER 'by' orderElement ( COMMA orderElement )* ;
    public HqlParser.orderByClause_return orderByClause() // throws RecognitionException [1]
    {   
        HqlParser.orderByClause_return retval = new HqlParser.orderByClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken ORDER123 = null;
        IToken string_literal124 = null;
        IToken COMMA126 = null;
        HqlParser.orderElement_return orderElement125 = default(HqlParser.orderElement_return);

        HqlParser.orderElement_return orderElement127 = default(HqlParser.orderElement_return);


        IASTNode ORDER123_tree=null;
        IASTNode string_literal124_tree=null;
        IASTNode COMMA126_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:298:2: ( ORDER 'by' orderElement ( COMMA orderElement )* )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:298:4: ORDER 'by' orderElement ( COMMA orderElement )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	ORDER123=(IToken)Match(input,ORDER,FOLLOW_ORDER_in_orderByClause1586); 
            		ORDER123_tree = (IASTNode)adaptor.Create(ORDER123);
            		root_0 = (IASTNode)adaptor.BecomeRoot(ORDER123_tree, root_0);

            	string_literal124=(IToken)Match(input,LITERAL_by,FOLLOW_LITERAL_by_in_orderByClause1589); 
            	PushFollow(FOLLOW_orderElement_in_orderByClause1592);
            	orderElement125 = orderElement();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, orderElement125.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:298:30: ( COMMA orderElement )*
            	do 
            	{
            	    int alt38 = 2;
            	    int LA38_0 = input.LA(1);

            	    if ( (LA38_0 == COMMA) )
            	    {
            	        alt38 = 1;
            	    }


            	    switch (alt38) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:298:32: COMMA orderElement
            			    {
            			    	COMMA126=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_orderByClause1596); 
            			    	PushFollow(FOLLOW_orderElement_in_orderByClause1599);
            			    	orderElement127 = orderElement();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, orderElement127.Tree);

            			    }
            			    break;

            			default:
            			    goto loop38;
            	    }
            	} while (true);

            	loop38:
            		;	// Stops C# compiler whining that label 'loop38' has no statements


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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:301:1: orderElement : expression ( ascendingOrDescending )? ;
    public HqlParser.orderElement_return orderElement() // throws RecognitionException [1]
    {   
        HqlParser.orderElement_return retval = new HqlParser.orderElement_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.expression_return expression128 = default(HqlParser.expression_return);

        HqlParser.ascendingOrDescending_return ascendingOrDescending129 = default(HqlParser.ascendingOrDescending_return);



        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:302:2: ( expression ( ascendingOrDescending )? )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:302:4: expression ( ascendingOrDescending )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_expression_in_orderElement1613);
            	expression128 = expression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, expression128.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:302:15: ( ascendingOrDescending )?
            	int alt39 = 2;
            	int LA39_0 = input.LA(1);

            	if ( (LA39_0 == ASCENDING || LA39_0 == DESCENDING || (LA39_0 >= 131 && LA39_0 <= 132)) )
            	{
            	    alt39 = 1;
            	}
            	switch (alt39) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:302:17: ascendingOrDescending
            	        {
            	        	PushFollow(FOLLOW_ascendingOrDescending_in_orderElement1617);
            	        	ascendingOrDescending129 = ascendingOrDescending();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, ascendingOrDescending129.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:305:1: ascendingOrDescending : ( (a= 'asc' | a= 'ascending' ) -> ^( ASCENDING[$a.Text] ) | (d= 'desc' | d= 'descending' ) -> ^( DESCENDING[$d.Text] ) );
    public HqlParser.ascendingOrDescending_return ascendingOrDescending() // throws RecognitionException [1]
    {   
        HqlParser.ascendingOrDescending_return retval = new HqlParser.ascendingOrDescending_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken a = null;
        IToken d = null;

        IASTNode a_tree=null;
        IASTNode d_tree=null;
        RewriteRuleTokenStream stream_131 = new RewriteRuleTokenStream(adaptor,"token 131");
        RewriteRuleTokenStream stream_ASCENDING = new RewriteRuleTokenStream(adaptor,"token ASCENDING");
        RewriteRuleTokenStream stream_132 = new RewriteRuleTokenStream(adaptor,"token 132");
        RewriteRuleTokenStream stream_DESCENDING = new RewriteRuleTokenStream(adaptor,"token DESCENDING");

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:306:2: ( (a= 'asc' | a= 'ascending' ) -> ^( ASCENDING[$a.Text] ) | (d= 'desc' | d= 'descending' ) -> ^( DESCENDING[$d.Text] ) )
            int alt42 = 2;
            int LA42_0 = input.LA(1);

            if ( (LA42_0 == ASCENDING || LA42_0 == 131) )
            {
                alt42 = 1;
            }
            else if ( (LA42_0 == DESCENDING || LA42_0 == 132) )
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
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:306:4: (a= 'asc' | a= 'ascending' )
                    {
                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:306:4: (a= 'asc' | a= 'ascending' )
                    	int alt40 = 2;
                    	int LA40_0 = input.LA(1);

                    	if ( (LA40_0 == ASCENDING) )
                    	{
                    	    alt40 = 1;
                    	}
                    	else if ( (LA40_0 == 131) )
                    	{
                    	    alt40 = 2;
                    	}
                    	else 
                    	{
                    	    NoViableAltException nvae_d40s0 =
                    	        new NoViableAltException("", 40, 0, input);

                    	    throw nvae_d40s0;
                    	}
                    	switch (alt40) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:306:6: a= 'asc'
                    	        {
                    	        	a=(IToken)Match(input,ASCENDING,FOLLOW_ASCENDING_in_ascendingOrDescending1635);  
                    	        	stream_ASCENDING.Add(a);


                    	        }
                    	        break;
                    	    case 2 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:306:16: a= 'ascending'
                    	        {
                    	        	a=(IToken)Match(input,131,FOLLOW_131_in_ascendingOrDescending1641);  
                    	        	stream_131.Add(a);


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
                    	// 307:3: -> ^( ASCENDING[$a.Text] )
                    	{
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:307:6: ^( ASCENDING[$a.Text] )
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
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:308:4: (d= 'desc' | d= 'descending' )
                    {
                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:308:4: (d= 'desc' | d= 'descending' )
                    	int alt41 = 2;
                    	int LA41_0 = input.LA(1);

                    	if ( (LA41_0 == DESCENDING) )
                    	{
                    	    alt41 = 1;
                    	}
                    	else if ( (LA41_0 == 132) )
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
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:308:6: d= 'desc'
                    	        {
                    	        	d=(IToken)Match(input,DESCENDING,FOLLOW_DESCENDING_in_ascendingOrDescending1661);  
                    	        	stream_DESCENDING.Add(d);


                    	        }
                    	        break;
                    	    case 2 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:308:17: d= 'descending'
                    	        {
                    	        	d=(IToken)Match(input,132,FOLLOW_132_in_ascendingOrDescending1667);  
                    	        	stream_132.Add(d);


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
                    	// 309:3: -> ^( DESCENDING[$d.Text] )
                    	{
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:309:6: ^( DESCENDING[$d.Text] )
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:312:1: havingClause : HAVING logicalExpression ;
    public HqlParser.havingClause_return havingClause() // throws RecognitionException [1]
    {   
        HqlParser.havingClause_return retval = new HqlParser.havingClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken HAVING130 = null;
        HqlParser.logicalExpression_return logicalExpression131 = default(HqlParser.logicalExpression_return);


        IASTNode HAVING130_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:313:2: ( HAVING logicalExpression )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:313:4: HAVING logicalExpression
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	HAVING130=(IToken)Match(input,HAVING,FOLLOW_HAVING_in_havingClause1688); 
            		HAVING130_tree = (IASTNode)adaptor.Create(HAVING130);
            		root_0 = (IASTNode)adaptor.BecomeRoot(HAVING130_tree, root_0);

            	PushFollow(FOLLOW_logicalExpression_in_havingClause1691);
            	logicalExpression131 = logicalExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, logicalExpression131.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:316:1: whereClause : WHERE logicalExpression ;
    public HqlParser.whereClause_return whereClause() // throws RecognitionException [1]
    {   
        HqlParser.whereClause_return retval = new HqlParser.whereClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken WHERE132 = null;
        HqlParser.logicalExpression_return logicalExpression133 = default(HqlParser.logicalExpression_return);


        IASTNode WHERE132_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:317:2: ( WHERE logicalExpression )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:317:4: WHERE logicalExpression
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	WHERE132=(IToken)Match(input,WHERE,FOLLOW_WHERE_in_whereClause1702); 
            		WHERE132_tree = (IASTNode)adaptor.Create(WHERE132);
            		root_0 = (IASTNode)adaptor.BecomeRoot(WHERE132_tree, root_0);

            	PushFollow(FOLLOW_logicalExpression_in_whereClause1705);
            	logicalExpression133 = logicalExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, logicalExpression133.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:320:1: selectedPropertiesList : aliasedExpression ( COMMA aliasedExpression )* ;
    public HqlParser.selectedPropertiesList_return selectedPropertiesList() // throws RecognitionException [1]
    {   
        HqlParser.selectedPropertiesList_return retval = new HqlParser.selectedPropertiesList_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken COMMA135 = null;
        HqlParser.aliasedExpression_return aliasedExpression134 = default(HqlParser.aliasedExpression_return);

        HqlParser.aliasedExpression_return aliasedExpression136 = default(HqlParser.aliasedExpression_return);


        IASTNode COMMA135_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:321:2: ( aliasedExpression ( COMMA aliasedExpression )* )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:321:4: aliasedExpression ( COMMA aliasedExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_aliasedExpression_in_selectedPropertiesList1716);
            	aliasedExpression134 = aliasedExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, aliasedExpression134.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:321:22: ( COMMA aliasedExpression )*
            	do 
            	{
            	    int alt43 = 2;
            	    int LA43_0 = input.LA(1);

            	    if ( (LA43_0 == COMMA) )
            	    {
            	        alt43 = 1;
            	    }


            	    switch (alt43) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:321:24: COMMA aliasedExpression
            			    {
            			    	COMMA135=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_selectedPropertiesList1720); 
            			    	PushFollow(FOLLOW_aliasedExpression_in_selectedPropertiesList1723);
            			    	aliasedExpression136 = aliasedExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, aliasedExpression136.Tree);

            			    }
            			    break;

            			default:
            			    goto loop43;
            	    }
            	} while (true);

            	loop43:
            		;	// Stops C# compiler whining that label 'loop43' has no statements


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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:324:1: aliasedExpression : expression ( AS identifier )? ;
    public HqlParser.aliasedExpression_return aliasedExpression() // throws RecognitionException [1]
    {   
        HqlParser.aliasedExpression_return retval = new HqlParser.aliasedExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken AS138 = null;
        HqlParser.expression_return expression137 = default(HqlParser.expression_return);

        HqlParser.identifier_return identifier139 = default(HqlParser.identifier_return);


        IASTNode AS138_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:325:2: ( expression ( AS identifier )? )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:325:4: expression ( AS identifier )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_expression_in_aliasedExpression1738);
            	expression137 = expression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, expression137.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:325:15: ( AS identifier )?
            	int alt44 = 2;
            	int LA44_0 = input.LA(1);

            	if ( (LA44_0 == AS) )
            	{
            	    alt44 = 1;
            	}
            	switch (alt44) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:325:17: AS identifier
            	        {
            	        	AS138=(IToken)Match(input,AS,FOLLOW_AS_in_aliasedExpression1742); 
            	        		AS138_tree = (IASTNode)adaptor.Create(AS138);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(AS138_tree, root_0);

            	        	PushFollow(FOLLOW_identifier_in_aliasedExpression1745);
            	        	identifier139 = identifier();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, identifier139.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:353:1: logicalExpression : expression ;
    public HqlParser.logicalExpression_return logicalExpression() // throws RecognitionException [1]
    {   
        HqlParser.logicalExpression_return retval = new HqlParser.logicalExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.expression_return expression140 = default(HqlParser.expression_return);



        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:354:2: ( expression )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:354:4: expression
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_expression_in_logicalExpression1784);
            	expression140 = expression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, expression140.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:358:1: expression : logicalOrExpression ;
    public HqlParser.expression_return expression() // throws RecognitionException [1]
    {   
        HqlParser.expression_return retval = new HqlParser.expression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.logicalOrExpression_return logicalOrExpression141 = default(HqlParser.logicalOrExpression_return);



        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:359:2: ( logicalOrExpression )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:359:4: logicalOrExpression
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_logicalOrExpression_in_expression1796);
            	logicalOrExpression141 = logicalOrExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, logicalOrExpression141.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:363:1: logicalOrExpression : logicalAndExpression ( OR logicalAndExpression )* ;
    public HqlParser.logicalOrExpression_return logicalOrExpression() // throws RecognitionException [1]
    {   
        HqlParser.logicalOrExpression_return retval = new HqlParser.logicalOrExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken OR143 = null;
        HqlParser.logicalAndExpression_return logicalAndExpression142 = default(HqlParser.logicalAndExpression_return);

        HqlParser.logicalAndExpression_return logicalAndExpression144 = default(HqlParser.logicalAndExpression_return);


        IASTNode OR143_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:364:2: ( logicalAndExpression ( OR logicalAndExpression )* )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:364:4: logicalAndExpression ( OR logicalAndExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_logicalAndExpression_in_logicalOrExpression1808);
            	logicalAndExpression142 = logicalAndExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, logicalAndExpression142.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:364:25: ( OR logicalAndExpression )*
            	do 
            	{
            	    int alt45 = 2;
            	    int LA45_0 = input.LA(1);

            	    if ( (LA45_0 == OR) )
            	    {
            	        alt45 = 1;
            	    }


            	    switch (alt45) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:364:27: OR logicalAndExpression
            			    {
            			    	OR143=(IToken)Match(input,OR,FOLLOW_OR_in_logicalOrExpression1812); 
            			    		OR143_tree = (IASTNode)adaptor.Create(OR143);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(OR143_tree, root_0);

            			    	PushFollow(FOLLOW_logicalAndExpression_in_logicalOrExpression1815);
            			    	logicalAndExpression144 = logicalAndExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, logicalAndExpression144.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:368:1: logicalAndExpression : negatedExpression ( AND negatedExpression )* ;
    public HqlParser.logicalAndExpression_return logicalAndExpression() // throws RecognitionException [1]
    {   
        HqlParser.logicalAndExpression_return retval = new HqlParser.logicalAndExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken AND146 = null;
        HqlParser.negatedExpression_return negatedExpression145 = default(HqlParser.negatedExpression_return);

        HqlParser.negatedExpression_return negatedExpression147 = default(HqlParser.negatedExpression_return);


        IASTNode AND146_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:369:2: ( negatedExpression ( AND negatedExpression )* )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:369:4: negatedExpression ( AND negatedExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_negatedExpression_in_logicalAndExpression1830);
            	negatedExpression145 = negatedExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, negatedExpression145.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:369:22: ( AND negatedExpression )*
            	do 
            	{
            	    int alt46 = 2;
            	    int LA46_0 = input.LA(1);

            	    if ( (LA46_0 == AND) )
            	    {
            	        alt46 = 1;
            	    }


            	    switch (alt46) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:369:24: AND negatedExpression
            			    {
            			    	AND146=(IToken)Match(input,AND,FOLLOW_AND_in_logicalAndExpression1834); 
            			    		AND146_tree = (IASTNode)adaptor.Create(AND146);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(AND146_tree, root_0);

            			    	PushFollow(FOLLOW_negatedExpression_in_logicalAndExpression1837);
            			    	negatedExpression147 = negatedExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, negatedExpression147.Tree);

            			    }
            			    break;

            			default:
            			    goto loop46;
            	    }
            	} while (true);

            	loop46:
            		;	// Stops C# compiler whining that label 'loop46' has no statements


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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:374:1: negatedExpression : ( NOT x= negatedExpression -> ^() | equalityExpression -> ^( equalityExpression ) );
    public HqlParser.negatedExpression_return negatedExpression() // throws RecognitionException [1]
    {   
        HqlParser.negatedExpression_return retval = new HqlParser.negatedExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken NOT148 = null;
        HqlParser.negatedExpression_return x = default(HqlParser.negatedExpression_return);

        HqlParser.equalityExpression_return equalityExpression149 = default(HqlParser.equalityExpression_return);


        IASTNode NOT148_tree=null;
        RewriteRuleTokenStream stream_NOT = new RewriteRuleTokenStream(adaptor,"token NOT");
        RewriteRuleSubtreeStream stream_negatedExpression = new RewriteRuleSubtreeStream(adaptor,"rule negatedExpression");
        RewriteRuleSubtreeStream stream_equalityExpression = new RewriteRuleSubtreeStream(adaptor,"rule equalityExpression");
         WeakKeywords(); 
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:376:2: ( NOT x= negatedExpression -> ^() | equalityExpression -> ^( equalityExpression ) )
            int alt47 = 2;
            int LA47_0 = input.LA(1);

            if ( (LA47_0 == NOT) )
            {
                alt47 = 1;
            }
            else if ( ((LA47_0 >= ALL && LA47_0 <= ANY) || LA47_0 == AVG || LA47_0 == COUNT || LA47_0 == ELEMENTS || (LA47_0 >= EXISTS && LA47_0 <= FALSE) || LA47_0 == INDICES || (LA47_0 >= MAX && LA47_0 <= MIN) || LA47_0 == NULL || (LA47_0 >= SOME && LA47_0 <= TRUE) || LA47_0 == CASE || LA47_0 == EMPTY || (LA47_0 >= NUM_INT && LA47_0 <= NUM_LONG) || LA47_0 == OPEN || LA47_0 == BNOT || (LA47_0 >= PLUS && LA47_0 <= MINUS) || (LA47_0 >= COLON && LA47_0 <= IDENT)) )
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
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:376:4: NOT x= negatedExpression
                    {
                    	NOT148=(IToken)Match(input,NOT,FOLLOW_NOT_in_negatedExpression1858);  
                    	stream_NOT.Add(NOT148);

                    	PushFollow(FOLLOW_negatedExpression_in_negatedExpression1862);
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
                    	// 377:3: -> ^()
                    	{
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:377:6: ^()
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
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:378:4: equalityExpression
                    {
                    	PushFollow(FOLLOW_equalityExpression_in_negatedExpression1875);
                    	equalityExpression149 = equalityExpression();
                    	state.followingStackPointer--;

                    	stream_equalityExpression.Add(equalityExpression149.Tree);


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
                    	// 379:3: -> ^( equalityExpression )
                    	{
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:379:6: ^( equalityExpression )
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:385:1: equalityExpression : x= relationalExpression ( ( EQ | isx= IS ( NOT )? | NE | ne= SQL_NE ) y= relationalExpression )* ;
    public HqlParser.equalityExpression_return equalityExpression() // throws RecognitionException [1]
    {   
        HqlParser.equalityExpression_return retval = new HqlParser.equalityExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken isx = null;
        IToken ne = null;
        IToken EQ150 = null;
        IToken NOT151 = null;
        IToken NE152 = null;
        HqlParser.relationalExpression_return x = default(HqlParser.relationalExpression_return);

        HqlParser.relationalExpression_return y = default(HqlParser.relationalExpression_return);


        IASTNode isx_tree=null;
        IASTNode ne_tree=null;
        IASTNode EQ150_tree=null;
        IASTNode NOT151_tree=null;
        IASTNode NE152_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:390:2: (x= relationalExpression ( ( EQ | isx= IS ( NOT )? | NE | ne= SQL_NE ) y= relationalExpression )* )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:390:4: x= relationalExpression ( ( EQ | isx= IS ( NOT )? | NE | ne= SQL_NE ) y= relationalExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_relationalExpression_in_equalityExpression1905);
            	x = relationalExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, x.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:390:27: ( ( EQ | isx= IS ( NOT )? | NE | ne= SQL_NE ) y= relationalExpression )*
            	do 
            	{
            	    int alt50 = 2;
            	    int LA50_0 = input.LA(1);

            	    if ( (LA50_0 == IS || LA50_0 == EQ || (LA50_0 >= NE && LA50_0 <= SQL_NE)) )
            	    {
            	        alt50 = 1;
            	    }


            	    switch (alt50) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:391:3: ( EQ | isx= IS ( NOT )? | NE | ne= SQL_NE ) y= relationalExpression
            			    {
            			    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:391:3: ( EQ | isx= IS ( NOT )? | NE | ne= SQL_NE )
            			    	int alt49 = 4;
            			    	switch ( input.LA(1) ) 
            			    	{
            			    	case EQ:
            			    		{
            			    	    alt49 = 1;
            			    	    }
            			    	    break;
            			    	case IS:
            			    		{
            			    	    alt49 = 2;
            			    	    }
            			    	    break;
            			    	case NE:
            			    		{
            			    	    alt49 = 3;
            			    	    }
            			    	    break;
            			    	case SQL_NE:
            			    		{
            			    	    alt49 = 4;
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
            			    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:391:5: EQ
            			    	        {
            			    	        	EQ150=(IToken)Match(input,EQ,FOLLOW_EQ_in_equalityExpression1913); 
            			    	        		EQ150_tree = (IASTNode)adaptor.Create(EQ150);
            			    	        		root_0 = (IASTNode)adaptor.BecomeRoot(EQ150_tree, root_0);


            			    	        }
            			    	        break;
            			    	    case 2 :
            			    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:392:5: isx= IS ( NOT )?
            			    	        {
            			    	        	isx=(IToken)Match(input,IS,FOLLOW_IS_in_equalityExpression1922); 
            			    	        		isx_tree = (IASTNode)adaptor.Create(isx);
            			    	        		root_0 = (IASTNode)adaptor.BecomeRoot(isx_tree, root_0);

            			    	        	 isx.Type = EQ; 
            			    	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:392:33: ( NOT )?
            			    	        	int alt48 = 2;
            			    	        	int LA48_0 = input.LA(1);

            			    	        	if ( (LA48_0 == NOT) )
            			    	        	{
            			    	        	    alt48 = 1;
            			    	        	}
            			    	        	switch (alt48) 
            			    	        	{
            			    	        	    case 1 :
            			    	        	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:392:34: NOT
            			    	        	        {
            			    	        	        	NOT151=(IToken)Match(input,NOT,FOLLOW_NOT_in_equalityExpression1928); 
            			    	        	        	 isx.Type =NE; 

            			    	        	        }
            			    	        	        break;

            			    	        	}


            			    	        }
            			    	        break;
            			    	    case 3 :
            			    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:393:5: NE
            			    	        {
            			    	        	NE152=(IToken)Match(input,NE,FOLLOW_NE_in_equalityExpression1940); 
            			    	        		NE152_tree = (IASTNode)adaptor.Create(NE152);
            			    	        		root_0 = (IASTNode)adaptor.BecomeRoot(NE152_tree, root_0);


            			    	        }
            			    	        break;
            			    	    case 4 :
            			    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:394:5: ne= SQL_NE
            			    	        {
            			    	        	ne=(IToken)Match(input,SQL_NE,FOLLOW_SQL_NE_in_equalityExpression1949); 
            			    	        		ne_tree = (IASTNode)adaptor.Create(ne);
            			    	        		root_0 = (IASTNode)adaptor.BecomeRoot(ne_tree, root_0);

            			    	        	 ne.Type = NE; 

            			    	        }
            			    	        break;

            			    	}

            			    	PushFollow(FOLLOW_relationalExpression_in_equalityExpression1960);
            			    	y = relationalExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, y.Tree);

            			    }
            			    break;

            			default:
            			    goto loop50;
            	    }
            	} while (true);

            	loop50:
            		;	// Stops C# compiler whining that label 'loop50' has no statements


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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:402:1: relationalExpression : concatenation ( ( ( ( LT | GT | LE | GE ) bitwiseNotExpression )* ) | (n= NOT )? ( (i= IN inList ) | (b= BETWEEN betweenList ) | (l= LIKE concatenation likeEscape ) | ( MEMBER ( OF )? p= path ) ) ) ;
    public HqlParser.relationalExpression_return relationalExpression() // throws RecognitionException [1]
    {   
        HqlParser.relationalExpression_return retval = new HqlParser.relationalExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken n = null;
        IToken i = null;
        IToken b = null;
        IToken l = null;
        IToken LT154 = null;
        IToken GT155 = null;
        IToken LE156 = null;
        IToken GE157 = null;
        IToken MEMBER163 = null;
        IToken OF164 = null;
        HqlParser.path_return p = default(HqlParser.path_return);

        HqlParser.concatenation_return concatenation153 = default(HqlParser.concatenation_return);

        HqlParser.bitwiseNotExpression_return bitwiseNotExpression158 = default(HqlParser.bitwiseNotExpression_return);

        HqlParser.inList_return inList159 = default(HqlParser.inList_return);

        HqlParser.betweenList_return betweenList160 = default(HqlParser.betweenList_return);

        HqlParser.concatenation_return concatenation161 = default(HqlParser.concatenation_return);

        HqlParser.likeEscape_return likeEscape162 = default(HqlParser.likeEscape_return);


        IASTNode n_tree=null;
        IASTNode i_tree=null;
        IASTNode b_tree=null;
        IASTNode l_tree=null;
        IASTNode LT154_tree=null;
        IASTNode GT155_tree=null;
        IASTNode LE156_tree=null;
        IASTNode GE157_tree=null;
        IASTNode MEMBER163_tree=null;
        IASTNode OF164_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:403:2: ( concatenation ( ( ( ( LT | GT | LE | GE ) bitwiseNotExpression )* ) | (n= NOT )? ( (i= IN inList ) | (b= BETWEEN betweenList ) | (l= LIKE concatenation likeEscape ) | ( MEMBER ( OF )? p= path ) ) ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:403:4: concatenation ( ( ( ( LT | GT | LE | GE ) bitwiseNotExpression )* ) | (n= NOT )? ( (i= IN inList ) | (b= BETWEEN betweenList ) | (l= LIKE concatenation likeEscape ) | ( MEMBER ( OF )? p= path ) ) )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_concatenation_in_relationalExpression1977);
            	concatenation153 = concatenation();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, concatenation153.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:403:18: ( ( ( ( LT | GT | LE | GE ) bitwiseNotExpression )* ) | (n= NOT )? ( (i= IN inList ) | (b= BETWEEN betweenList ) | (l= LIKE concatenation likeEscape ) | ( MEMBER ( OF )? p= path ) ) )
            	int alt56 = 2;
            	int LA56_0 = input.LA(1);

            	if ( (LA56_0 == EOF || (LA56_0 >= AND && LA56_0 <= ASCENDING) || LA56_0 == DESCENDING || (LA56_0 >= FROM && LA56_0 <= HAVING) || LA56_0 == INNER || (LA56_0 >= IS && LA56_0 <= LEFT) || (LA56_0 >= OR && LA56_0 <= ORDER) || LA56_0 == RIGHT || LA56_0 == UNION || LA56_0 == WHERE || (LA56_0 >= END && LA56_0 <= WHEN) || (LA56_0 >= COMMA && LA56_0 <= EQ) || (LA56_0 >= CLOSE && LA56_0 <= GE) || LA56_0 == CLOSE_BRACKET || (LA56_0 >= 131 && LA56_0 <= 132)) )
            	{
            	    alt56 = 1;
            	}
            	else if ( (LA56_0 == BETWEEN || LA56_0 == IN || LA56_0 == LIKE || LA56_0 == NOT || LA56_0 == MEMBER) )
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
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:404:3: ( ( ( LT | GT | LE | GE ) bitwiseNotExpression )* )
            	        {
            	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:404:3: ( ( ( LT | GT | LE | GE ) bitwiseNotExpression )* )
            	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:404:5: ( ( LT | GT | LE | GE ) bitwiseNotExpression )*
            	        	{
            	        		// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:404:5: ( ( LT | GT | LE | GE ) bitwiseNotExpression )*
            	        		do 
            	        		{
            	        		    int alt52 = 2;
            	        		    int LA52_0 = input.LA(1);

            	        		    if ( ((LA52_0 >= LT && LA52_0 <= GE)) )
            	        		    {
            	        		        alt52 = 1;
            	        		    }


            	        		    switch (alt52) 
            	        			{
            	        				case 1 :
            	        				    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:404:7: ( LT | GT | LE | GE ) bitwiseNotExpression
            	        				    {
            	        				    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:404:7: ( LT | GT | LE | GE )
            	        				    	int alt51 = 4;
            	        				    	switch ( input.LA(1) ) 
            	        				    	{
            	        				    	case LT:
            	        				    		{
            	        				    	    alt51 = 1;
            	        				    	    }
            	        				    	    break;
            	        				    	case GT:
            	        				    		{
            	        				    	    alt51 = 2;
            	        				    	    }
            	        				    	    break;
            	        				    	case LE:
            	        				    		{
            	        				    	    alt51 = 3;
            	        				    	    }
            	        				    	    break;
            	        				    	case GE:
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
            	        				    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:404:9: LT
            	        				    	        {
            	        				    	        	LT154=(IToken)Match(input,LT,FOLLOW_LT_in_relationalExpression1989); 
            	        				    	        		LT154_tree = (IASTNode)adaptor.Create(LT154);
            	        				    	        		root_0 = (IASTNode)adaptor.BecomeRoot(LT154_tree, root_0);


            	        				    	        }
            	        				    	        break;
            	        				    	    case 2 :
            	        				    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:404:15: GT
            	        				    	        {
            	        				    	        	GT155=(IToken)Match(input,GT,FOLLOW_GT_in_relationalExpression1994); 
            	        				    	        		GT155_tree = (IASTNode)adaptor.Create(GT155);
            	        				    	        		root_0 = (IASTNode)adaptor.BecomeRoot(GT155_tree, root_0);


            	        				    	        }
            	        				    	        break;
            	        				    	    case 3 :
            	        				    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:404:21: LE
            	        				    	        {
            	        				    	        	LE156=(IToken)Match(input,LE,FOLLOW_LE_in_relationalExpression1999); 
            	        				    	        		LE156_tree = (IASTNode)adaptor.Create(LE156);
            	        				    	        		root_0 = (IASTNode)adaptor.BecomeRoot(LE156_tree, root_0);


            	        				    	        }
            	        				    	        break;
            	        				    	    case 4 :
            	        				    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:404:27: GE
            	        				    	        {
            	        				    	        	GE157=(IToken)Match(input,GE,FOLLOW_GE_in_relationalExpression2004); 
            	        				    	        		GE157_tree = (IASTNode)adaptor.Create(GE157);
            	        				    	        		root_0 = (IASTNode)adaptor.BecomeRoot(GE157_tree, root_0);


            	        				    	        }
            	        				    	        break;

            	        				    	}

            	        				    	PushFollow(FOLLOW_bitwiseNotExpression_in_relationalExpression2009);
            	        				    	bitwiseNotExpression158 = bitwiseNotExpression();
            	        				    	state.followingStackPointer--;

            	        				    	adaptor.AddChild(root_0, bitwiseNotExpression158.Tree);

            	        				    }
            	        				    break;

            	        				default:
            	        				    goto loop52;
            	        		    }
            	        		} while (true);

            	        		loop52:
            	        			;	// Stops C# compiler whining that label 'loop52' has no statements


            	        	}


            	        }
            	        break;
            	    case 2 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:406:5: (n= NOT )? ( (i= IN inList ) | (b= BETWEEN betweenList ) | (l= LIKE concatenation likeEscape ) | ( MEMBER ( OF )? p= path ) )
            	        {
            	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:406:5: (n= NOT )?
            	        	int alt53 = 2;
            	        	int LA53_0 = input.LA(1);

            	        	if ( (LA53_0 == NOT) )
            	        	{
            	        	    alt53 = 1;
            	        	}
            	        	switch (alt53) 
            	        	{
            	        	    case 1 :
            	        	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:406:6: n= NOT
            	        	        {
            	        	        	n=(IToken)Match(input,NOT,FOLLOW_NOT_in_relationalExpression2026); 

            	        	        }
            	        	        break;

            	        	}

            	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:406:15: ( (i= IN inList ) | (b= BETWEEN betweenList ) | (l= LIKE concatenation likeEscape ) | ( MEMBER ( OF )? p= path ) )
            	        	int alt55 = 4;
            	        	switch ( input.LA(1) ) 
            	        	{
            	        	case IN:
            	        		{
            	        	    alt55 = 1;
            	        	    }
            	        	    break;
            	        	case BETWEEN:
            	        		{
            	        	    alt55 = 2;
            	        	    }
            	        	    break;
            	        	case LIKE:
            	        		{
            	        	    alt55 = 3;
            	        	    }
            	        	    break;
            	        	case MEMBER:
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
            	        	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:409:4: (i= IN inList )
            	        	        {
            	        	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:409:4: (i= IN inList )
            	        	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:409:5: i= IN inList
            	        	        	{
            	        	        		i=(IToken)Match(input,IN,FOLLOW_IN_in_relationalExpression2047); 
            	        	        			i_tree = (IASTNode)adaptor.Create(i);
            	        	        			root_0 = (IASTNode)adaptor.BecomeRoot(i_tree, root_0);


            	        	        							i.Type = (n == null) ? IN : NOT_IN;
            	        	        							i.Text = (n == null) ? "in" : "not in";
            	        	        						
            	        	        		PushFollow(FOLLOW_inList_in_relationalExpression2056);
            	        	        		inList159 = inList();
            	        	        		state.followingStackPointer--;

            	        	        		adaptor.AddChild(root_0, inList159.Tree);

            	        	        	}


            	        	        }
            	        	        break;
            	        	    case 2 :
            	        	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:414:6: (b= BETWEEN betweenList )
            	        	        {
            	        	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:414:6: (b= BETWEEN betweenList )
            	        	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:414:7: b= BETWEEN betweenList
            	        	        	{
            	        	        		b=(IToken)Match(input,BETWEEN,FOLLOW_BETWEEN_in_relationalExpression2067); 
            	        	        			b_tree = (IASTNode)adaptor.Create(b);
            	        	        			root_0 = (IASTNode)adaptor.BecomeRoot(b_tree, root_0);


            	        	        							b.Type = (n == null) ? BETWEEN : NOT_BETWEEN;
            	        	        							b.Text = (n == null) ? "between" : "not between";
            	        	        						
            	        	        		PushFollow(FOLLOW_betweenList_in_relationalExpression2076);
            	        	        		betweenList160 = betweenList();
            	        	        		state.followingStackPointer--;

            	        	        		adaptor.AddChild(root_0, betweenList160.Tree);

            	        	        	}


            	        	        }
            	        	        break;
            	        	    case 3 :
            	        	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:419:6: (l= LIKE concatenation likeEscape )
            	        	        {
            	        	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:419:6: (l= LIKE concatenation likeEscape )
            	        	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:419:7: l= LIKE concatenation likeEscape
            	        	        	{
            	        	        		l=(IToken)Match(input,LIKE,FOLLOW_LIKE_in_relationalExpression2088); 
            	        	        			l_tree = (IASTNode)adaptor.Create(l);
            	        	        			root_0 = (IASTNode)adaptor.BecomeRoot(l_tree, root_0);


            	        	        							l.Type = (n == null) ? LIKE : NOT_LIKE;
            	        	        							l.Text = (n == null) ? "like" : "not like";
            	        	        						
            	        	        		PushFollow(FOLLOW_concatenation_in_relationalExpression2097);
            	        	        		concatenation161 = concatenation();
            	        	        		state.followingStackPointer--;

            	        	        		adaptor.AddChild(root_0, concatenation161.Tree);
            	        	        		PushFollow(FOLLOW_likeEscape_in_relationalExpression2099);
            	        	        		likeEscape162 = likeEscape();
            	        	        		state.followingStackPointer--;

            	        	        		adaptor.AddChild(root_0, likeEscape162.Tree);

            	        	        	}


            	        	        }
            	        	        break;
            	        	    case 4 :
            	        	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:424:6: ( MEMBER ( OF )? p= path )
            	        	        {
            	        	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:424:6: ( MEMBER ( OF )? p= path )
            	        	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:424:7: MEMBER ( OF )? p= path
            	        	        	{
            	        	        		MEMBER163=(IToken)Match(input,MEMBER,FOLLOW_MEMBER_in_relationalExpression2108); 
            	        	        		// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:424:15: ( OF )?
            	        	        		int alt54 = 2;
            	        	        		int LA54_0 = input.LA(1);

            	        	        		if ( (LA54_0 == OF) )
            	        	        		{
            	        	        		    alt54 = 1;
            	        	        		}
            	        	        		switch (alt54) 
            	        	        		{
            	        	        		    case 1 :
            	        	        		        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:424:16: OF
            	        	        		        {
            	        	        		        	OF164=(IToken)Match(input,OF,FOLLOW_OF_in_relationalExpression2112); 

            	        	        		        }
            	        	        		        break;

            	        	        		}

            	        	        		PushFollow(FOLLOW_path_in_relationalExpression2119);
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:431:1: likeEscape : ( ESCAPE concatenation )? ;
    public HqlParser.likeEscape_return likeEscape() // throws RecognitionException [1]
    {   
        HqlParser.likeEscape_return retval = new HqlParser.likeEscape_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken ESCAPE165 = null;
        HqlParser.concatenation_return concatenation166 = default(HqlParser.concatenation_return);


        IASTNode ESCAPE165_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:432:2: ( ( ESCAPE concatenation )? )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:432:4: ( ESCAPE concatenation )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:432:4: ( ESCAPE concatenation )?
            	int alt57 = 2;
            	int LA57_0 = input.LA(1);

            	if ( (LA57_0 == ESCAPE) )
            	{
            	    alt57 = 1;
            	}
            	switch (alt57) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:432:5: ESCAPE concatenation
            	        {
            	        	ESCAPE165=(IToken)Match(input,ESCAPE,FOLLOW_ESCAPE_in_likeEscape2146); 
            	        		ESCAPE165_tree = (IASTNode)adaptor.Create(ESCAPE165);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(ESCAPE165_tree, root_0);

            	        	PushFollow(FOLLOW_concatenation_in_likeEscape2149);
            	        	concatenation166 = concatenation();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, concatenation166.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:435:1: inList : compoundExpr -> ^( IN_LIST[\"inList\"] compoundExpr ) ;
    public HqlParser.inList_return inList() // throws RecognitionException [1]
    {   
        HqlParser.inList_return retval = new HqlParser.inList_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.compoundExpr_return compoundExpr167 = default(HqlParser.compoundExpr_return);


        RewriteRuleSubtreeStream stream_compoundExpr = new RewriteRuleSubtreeStream(adaptor,"rule compoundExpr");
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:436:2: ( compoundExpr -> ^( IN_LIST[\"inList\"] compoundExpr ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:436:4: compoundExpr
            {
            	PushFollow(FOLLOW_compoundExpr_in_inList2162);
            	compoundExpr167 = compoundExpr();
            	state.followingStackPointer--;

            	stream_compoundExpr.Add(compoundExpr167.Tree);


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
            	// 437:2: -> ^( IN_LIST[\"inList\"] compoundExpr )
            	{
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:437:5: ^( IN_LIST[\"inList\"] compoundExpr )
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:440:1: betweenList : concatenation AND concatenation ;
    public HqlParser.betweenList_return betweenList() // throws RecognitionException [1]
    {   
        HqlParser.betweenList_return retval = new HqlParser.betweenList_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken AND169 = null;
        HqlParser.concatenation_return concatenation168 = default(HqlParser.concatenation_return);

        HqlParser.concatenation_return concatenation170 = default(HqlParser.concatenation_return);


        IASTNode AND169_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:441:2: ( concatenation AND concatenation )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:441:4: concatenation AND concatenation
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_concatenation_in_betweenList2183);
            	concatenation168 = concatenation();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, concatenation168.Tree);
            	AND169=(IToken)Match(input,AND,FOLLOW_AND_in_betweenList2185); 
            	PushFollow(FOLLOW_concatenation_in_betweenList2188);
            	concatenation170 = concatenation();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, concatenation170.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:445:1: concatenation : a= bitwiseNotExpression (c= CONCAT bitwiseNotExpression ( CONCAT bitwiseNotExpression )* )? ;
    public HqlParser.concatenation_return concatenation() // throws RecognitionException [1]
    {   
        HqlParser.concatenation_return retval = new HqlParser.concatenation_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken c = null;
        IToken CONCAT172 = null;
        HqlParser.bitwiseNotExpression_return a = default(HqlParser.bitwiseNotExpression_return);

        HqlParser.bitwiseNotExpression_return bitwiseNotExpression171 = default(HqlParser.bitwiseNotExpression_return);

        HqlParser.bitwiseNotExpression_return bitwiseNotExpression173 = default(HqlParser.bitwiseNotExpression_return);


        IASTNode c_tree=null;
        IASTNode CONCAT172_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:456:2: (a= bitwiseNotExpression (c= CONCAT bitwiseNotExpression ( CONCAT bitwiseNotExpression )* )? )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:456:4: a= bitwiseNotExpression (c= CONCAT bitwiseNotExpression ( CONCAT bitwiseNotExpression )* )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_bitwiseNotExpression_in_concatenation2207);
            	a = bitwiseNotExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, a.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:457:2: (c= CONCAT bitwiseNotExpression ( CONCAT bitwiseNotExpression )* )?
            	int alt59 = 2;
            	int LA59_0 = input.LA(1);

            	if ( (LA59_0 == CONCAT) )
            	{
            	    alt59 = 1;
            	}
            	switch (alt59) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:457:4: c= CONCAT bitwiseNotExpression ( CONCAT bitwiseNotExpression )*
            	        {
            	        	c=(IToken)Match(input,CONCAT,FOLLOW_CONCAT_in_concatenation2215); 
            	        		c_tree = (IASTNode)adaptor.Create(c);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(c_tree, root_0);

            	        	 c.Type = EXPR_LIST; c.Text = "concatList"; 
            	        	PushFollow(FOLLOW_bitwiseNotExpression_in_concatenation2224);
            	        	bitwiseNotExpression171 = bitwiseNotExpression();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, bitwiseNotExpression171.Tree);
            	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:459:4: ( CONCAT bitwiseNotExpression )*
            	        	do 
            	        	{
            	        	    int alt58 = 2;
            	        	    int LA58_0 = input.LA(1);

            	        	    if ( (LA58_0 == CONCAT) )
            	        	    {
            	        	        alt58 = 1;
            	        	    }


            	        	    switch (alt58) 
            	        		{
            	        			case 1 :
            	        			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:459:6: CONCAT bitwiseNotExpression
            	        			    {
            	        			    	CONCAT172=(IToken)Match(input,CONCAT,FOLLOW_CONCAT_in_concatenation2231); 
            	        			    	PushFollow(FOLLOW_bitwiseNotExpression_in_concatenation2234);
            	        			    	bitwiseNotExpression173 = bitwiseNotExpression();
            	        			    	state.followingStackPointer--;

            	        			    	adaptor.AddChild(root_0, bitwiseNotExpression173.Tree);

            	        			    }
            	        			    break;

            	        			default:
            	        			    goto loop58;
            	        	    }
            	        	} while (true);

            	        	loop58:
            	        		;	// Stops C# compiler whining that label 'loop58' has no statements


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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:464:1: bitwiseNotExpression : ( ( BNOT bitwiseOrExpression ) | bitwiseOrExpression );
    public HqlParser.bitwiseNotExpression_return bitwiseNotExpression() // throws RecognitionException [1]
    {   
        HqlParser.bitwiseNotExpression_return retval = new HqlParser.bitwiseNotExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken BNOT174 = null;
        HqlParser.bitwiseOrExpression_return bitwiseOrExpression175 = default(HqlParser.bitwiseOrExpression_return);

        HqlParser.bitwiseOrExpression_return bitwiseOrExpression176 = default(HqlParser.bitwiseOrExpression_return);


        IASTNode BNOT174_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:465:2: ( ( BNOT bitwiseOrExpression ) | bitwiseOrExpression )
            int alt60 = 2;
            int LA60_0 = input.LA(1);

            if ( (LA60_0 == BNOT) )
            {
                alt60 = 1;
            }
            else if ( ((LA60_0 >= ALL && LA60_0 <= ANY) || LA60_0 == AVG || LA60_0 == COUNT || LA60_0 == ELEMENTS || (LA60_0 >= EXISTS && LA60_0 <= FALSE) || LA60_0 == INDICES || (LA60_0 >= MAX && LA60_0 <= MIN) || LA60_0 == NULL || (LA60_0 >= SOME && LA60_0 <= TRUE) || LA60_0 == CASE || LA60_0 == EMPTY || (LA60_0 >= NUM_INT && LA60_0 <= NUM_LONG) || LA60_0 == OPEN || (LA60_0 >= PLUS && LA60_0 <= MINUS) || (LA60_0 >= COLON && LA60_0 <= IDENT)) )
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
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:465:4: ( BNOT bitwiseOrExpression )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:465:4: ( BNOT bitwiseOrExpression )
                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:465:5: BNOT bitwiseOrExpression
                    	{
                    		BNOT174=(IToken)Match(input,BNOT,FOLLOW_BNOT_in_bitwiseNotExpression2258); 
                    			BNOT174_tree = (IASTNode)adaptor.Create(BNOT174);
                    			root_0 = (IASTNode)adaptor.BecomeRoot(BNOT174_tree, root_0);

                    		PushFollow(FOLLOW_bitwiseOrExpression_in_bitwiseNotExpression2261);
                    		bitwiseOrExpression175 = bitwiseOrExpression();
                    		state.followingStackPointer--;

                    		adaptor.AddChild(root_0, bitwiseOrExpression175.Tree);

                    	}


                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:466:4: bitwiseOrExpression
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_bitwiseOrExpression_in_bitwiseNotExpression2267);
                    	bitwiseOrExpression176 = bitwiseOrExpression();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, bitwiseOrExpression176.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:469:1: bitwiseOrExpression : bitwiseXOrExpression ( BOR bitwiseXOrExpression )* ;
    public HqlParser.bitwiseOrExpression_return bitwiseOrExpression() // throws RecognitionException [1]
    {   
        HqlParser.bitwiseOrExpression_return retval = new HqlParser.bitwiseOrExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken BOR178 = null;
        HqlParser.bitwiseXOrExpression_return bitwiseXOrExpression177 = default(HqlParser.bitwiseXOrExpression_return);

        HqlParser.bitwiseXOrExpression_return bitwiseXOrExpression179 = default(HqlParser.bitwiseXOrExpression_return);


        IASTNode BOR178_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:470:2: ( bitwiseXOrExpression ( BOR bitwiseXOrExpression )* )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:470:4: bitwiseXOrExpression ( BOR bitwiseXOrExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_bitwiseXOrExpression_in_bitwiseOrExpression2279);
            	bitwiseXOrExpression177 = bitwiseXOrExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, bitwiseXOrExpression177.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:470:25: ( BOR bitwiseXOrExpression )*
            	do 
            	{
            	    int alt61 = 2;
            	    int LA61_0 = input.LA(1);

            	    if ( (LA61_0 == BOR) )
            	    {
            	        alt61 = 1;
            	    }


            	    switch (alt61) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:470:26: BOR bitwiseXOrExpression
            			    {
            			    	BOR178=(IToken)Match(input,BOR,FOLLOW_BOR_in_bitwiseOrExpression2282); 
            			    		BOR178_tree = (IASTNode)adaptor.Create(BOR178);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(BOR178_tree, root_0);

            			    	PushFollow(FOLLOW_bitwiseXOrExpression_in_bitwiseOrExpression2285);
            			    	bitwiseXOrExpression179 = bitwiseXOrExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, bitwiseXOrExpression179.Tree);

            			    }
            			    break;

            			default:
            			    goto loop61;
            	    }
            	} while (true);

            	loop61:
            		;	// Stops C# compiler whining that label 'loop61' has no statements


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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:473:1: bitwiseXOrExpression : bitwiseAndExpression ( BXOR bitwiseAndExpression )* ;
    public HqlParser.bitwiseXOrExpression_return bitwiseXOrExpression() // throws RecognitionException [1]
    {   
        HqlParser.bitwiseXOrExpression_return retval = new HqlParser.bitwiseXOrExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken BXOR181 = null;
        HqlParser.bitwiseAndExpression_return bitwiseAndExpression180 = default(HqlParser.bitwiseAndExpression_return);

        HqlParser.bitwiseAndExpression_return bitwiseAndExpression182 = default(HqlParser.bitwiseAndExpression_return);


        IASTNode BXOR181_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:474:2: ( bitwiseAndExpression ( BXOR bitwiseAndExpression )* )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:474:4: bitwiseAndExpression ( BXOR bitwiseAndExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_bitwiseAndExpression_in_bitwiseXOrExpression2299);
            	bitwiseAndExpression180 = bitwiseAndExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, bitwiseAndExpression180.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:474:25: ( BXOR bitwiseAndExpression )*
            	do 
            	{
            	    int alt62 = 2;
            	    int LA62_0 = input.LA(1);

            	    if ( (LA62_0 == BXOR) )
            	    {
            	        alt62 = 1;
            	    }


            	    switch (alt62) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:474:26: BXOR bitwiseAndExpression
            			    {
            			    	BXOR181=(IToken)Match(input,BXOR,FOLLOW_BXOR_in_bitwiseXOrExpression2302); 
            			    		BXOR181_tree = (IASTNode)adaptor.Create(BXOR181);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(BXOR181_tree, root_0);

            			    	PushFollow(FOLLOW_bitwiseAndExpression_in_bitwiseXOrExpression2305);
            			    	bitwiseAndExpression182 = bitwiseAndExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, bitwiseAndExpression182.Tree);

            			    }
            			    break;

            			default:
            			    goto loop62;
            	    }
            	} while (true);

            	loop62:
            		;	// Stops C# compiler whining that label 'loop62' has no statements


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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:477:1: bitwiseAndExpression : additiveExpression ( BAND additiveExpression )* ;
    public HqlParser.bitwiseAndExpression_return bitwiseAndExpression() // throws RecognitionException [1]
    {   
        HqlParser.bitwiseAndExpression_return retval = new HqlParser.bitwiseAndExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken BAND184 = null;
        HqlParser.additiveExpression_return additiveExpression183 = default(HqlParser.additiveExpression_return);

        HqlParser.additiveExpression_return additiveExpression185 = default(HqlParser.additiveExpression_return);


        IASTNode BAND184_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:478:2: ( additiveExpression ( BAND additiveExpression )* )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:478:4: additiveExpression ( BAND additiveExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_additiveExpression_in_bitwiseAndExpression2319);
            	additiveExpression183 = additiveExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, additiveExpression183.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:478:23: ( BAND additiveExpression )*
            	do 
            	{
            	    int alt63 = 2;
            	    int LA63_0 = input.LA(1);

            	    if ( (LA63_0 == BAND) )
            	    {
            	        alt63 = 1;
            	    }


            	    switch (alt63) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:478:24: BAND additiveExpression
            			    {
            			    	BAND184=(IToken)Match(input,BAND,FOLLOW_BAND_in_bitwiseAndExpression2322); 
            			    		BAND184_tree = (IASTNode)adaptor.Create(BAND184);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(BAND184_tree, root_0);

            			    	PushFollow(FOLLOW_additiveExpression_in_bitwiseAndExpression2325);
            			    	additiveExpression185 = additiveExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, additiveExpression185.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:482:1: additiveExpression : multiplyExpression ( ( PLUS | MINUS ) multiplyExpression )* ;
    public HqlParser.additiveExpression_return additiveExpression() // throws RecognitionException [1]
    {   
        HqlParser.additiveExpression_return retval = new HqlParser.additiveExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken PLUS187 = null;
        IToken MINUS188 = null;
        HqlParser.multiplyExpression_return multiplyExpression186 = default(HqlParser.multiplyExpression_return);

        HqlParser.multiplyExpression_return multiplyExpression189 = default(HqlParser.multiplyExpression_return);


        IASTNode PLUS187_tree=null;
        IASTNode MINUS188_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:483:2: ( multiplyExpression ( ( PLUS | MINUS ) multiplyExpression )* )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:483:4: multiplyExpression ( ( PLUS | MINUS ) multiplyExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_multiplyExpression_in_additiveExpression2339);
            	multiplyExpression186 = multiplyExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, multiplyExpression186.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:483:23: ( ( PLUS | MINUS ) multiplyExpression )*
            	do 
            	{
            	    int alt65 = 2;
            	    int LA65_0 = input.LA(1);

            	    if ( ((LA65_0 >= PLUS && LA65_0 <= MINUS)) )
            	    {
            	        alt65 = 1;
            	    }


            	    switch (alt65) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:483:25: ( PLUS | MINUS ) multiplyExpression
            			    {
            			    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:483:25: ( PLUS | MINUS )
            			    	int alt64 = 2;
            			    	int LA64_0 = input.LA(1);

            			    	if ( (LA64_0 == PLUS) )
            			    	{
            			    	    alt64 = 1;
            			    	}
            			    	else if ( (LA64_0 == MINUS) )
            			    	{
            			    	    alt64 = 2;
            			    	}
            			    	else 
            			    	{
            			    	    NoViableAltException nvae_d64s0 =
            			    	        new NoViableAltException("", 64, 0, input);

            			    	    throw nvae_d64s0;
            			    	}
            			    	switch (alt64) 
            			    	{
            			    	    case 1 :
            			    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:483:27: PLUS
            			    	        {
            			    	        	PLUS187=(IToken)Match(input,PLUS,FOLLOW_PLUS_in_additiveExpression2345); 
            			    	        		PLUS187_tree = (IASTNode)adaptor.Create(PLUS187);
            			    	        		root_0 = (IASTNode)adaptor.BecomeRoot(PLUS187_tree, root_0);


            			    	        }
            			    	        break;
            			    	    case 2 :
            			    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:483:35: MINUS
            			    	        {
            			    	        	MINUS188=(IToken)Match(input,MINUS,FOLLOW_MINUS_in_additiveExpression2350); 
            			    	        		MINUS188_tree = (IASTNode)adaptor.Create(MINUS188);
            			    	        		root_0 = (IASTNode)adaptor.BecomeRoot(MINUS188_tree, root_0);


            			    	        }
            			    	        break;

            			    	}

            			    	PushFollow(FOLLOW_multiplyExpression_in_additiveExpression2355);
            			    	multiplyExpression189 = multiplyExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, multiplyExpression189.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:487:1: multiplyExpression : unaryExpression ( ( STAR | DIV ) unaryExpression )* ;
    public HqlParser.multiplyExpression_return multiplyExpression() // throws RecognitionException [1]
    {   
        HqlParser.multiplyExpression_return retval = new HqlParser.multiplyExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken STAR191 = null;
        IToken DIV192 = null;
        HqlParser.unaryExpression_return unaryExpression190 = default(HqlParser.unaryExpression_return);

        HqlParser.unaryExpression_return unaryExpression193 = default(HqlParser.unaryExpression_return);


        IASTNode STAR191_tree=null;
        IASTNode DIV192_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:488:2: ( unaryExpression ( ( STAR | DIV ) unaryExpression )* )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:488:4: unaryExpression ( ( STAR | DIV ) unaryExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_unaryExpression_in_multiplyExpression2370);
            	unaryExpression190 = unaryExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, unaryExpression190.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:488:20: ( ( STAR | DIV ) unaryExpression )*
            	do 
            	{
            	    int alt67 = 2;
            	    int LA67_0 = input.LA(1);

            	    if ( ((LA67_0 >= STAR && LA67_0 <= DIV)) )
            	    {
            	        alt67 = 1;
            	    }


            	    switch (alt67) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:488:22: ( STAR | DIV ) unaryExpression
            			    {
            			    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:488:22: ( STAR | DIV )
            			    	int alt66 = 2;
            			    	int LA66_0 = input.LA(1);

            			    	if ( (LA66_0 == STAR) )
            			    	{
            			    	    alt66 = 1;
            			    	}
            			    	else if ( (LA66_0 == DIV) )
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
            			    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:488:24: STAR
            			    	        {
            			    	        	STAR191=(IToken)Match(input,STAR,FOLLOW_STAR_in_multiplyExpression2376); 
            			    	        		STAR191_tree = (IASTNode)adaptor.Create(STAR191);
            			    	        		root_0 = (IASTNode)adaptor.BecomeRoot(STAR191_tree, root_0);


            			    	        }
            			    	        break;
            			    	    case 2 :
            			    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:488:32: DIV
            			    	        {
            			    	        	DIV192=(IToken)Match(input,DIV,FOLLOW_DIV_in_multiplyExpression2381); 
            			    	        		DIV192_tree = (IASTNode)adaptor.Create(DIV192);
            			    	        		root_0 = (IASTNode)adaptor.BecomeRoot(DIV192_tree, root_0);


            			    	        }
            			    	        break;

            			    	}

            			    	PushFollow(FOLLOW_unaryExpression_in_multiplyExpression2386);
            			    	unaryExpression193 = unaryExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, unaryExpression193.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:492:1: unaryExpression : (m= MINUS mu= unaryExpression -> ^( UNARY_MINUS[$m] $mu) | p= PLUS pu= unaryExpression -> ^( UNARY_PLUS[$p] $pu) | c= caseExpression -> ^( $c) | q= quantifiedExpression -> ^( $q) | a= atom -> ^( $a) );
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
        RewriteRuleTokenStream stream_MINUS = new RewriteRuleTokenStream(adaptor,"token MINUS");
        RewriteRuleTokenStream stream_PLUS = new RewriteRuleTokenStream(adaptor,"token PLUS");
        RewriteRuleSubtreeStream stream_unaryExpression = new RewriteRuleSubtreeStream(adaptor,"rule unaryExpression");
        RewriteRuleSubtreeStream stream_atom = new RewriteRuleSubtreeStream(adaptor,"rule atom");
        RewriteRuleSubtreeStream stream_quantifiedExpression = new RewriteRuleSubtreeStream(adaptor,"rule quantifiedExpression");
        RewriteRuleSubtreeStream stream_caseExpression = new RewriteRuleSubtreeStream(adaptor,"rule caseExpression");
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:493:2: (m= MINUS mu= unaryExpression -> ^( UNARY_MINUS[$m] $mu) | p= PLUS pu= unaryExpression -> ^( UNARY_PLUS[$p] $pu) | c= caseExpression -> ^( $c) | q= quantifiedExpression -> ^( $q) | a= atom -> ^( $a) )
            int alt68 = 5;
            switch ( input.LA(1) ) 
            {
            case MINUS:
            	{
                alt68 = 1;
                }
                break;
            case PLUS:
            	{
                alt68 = 2;
                }
                break;
            case CASE:
            	{
                alt68 = 3;
                }
                break;
            case ALL:
            case ANY:
            case EXISTS:
            case SOME:
            	{
                alt68 = 4;
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
                alt68 = 5;
                }
                break;
            	default:
            	    NoViableAltException nvae_d68s0 =
            	        new NoViableAltException("", 68, 0, input);

            	    throw nvae_d68s0;
            }

            switch (alt68) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:493:4: m= MINUS mu= unaryExpression
                    {
                    	m=(IToken)Match(input,MINUS,FOLLOW_MINUS_in_unaryExpression2404);  
                    	stream_MINUS.Add(m);

                    	PushFollow(FOLLOW_unaryExpression_in_unaryExpression2408);
                    	mu = unaryExpression();
                    	state.followingStackPointer--;

                    	stream_unaryExpression.Add(mu.Tree);


                    	// AST REWRITE
                    	// elements:          mu
                    	// token labels:      
                    	// rule labels:       mu, retval
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_mu = new RewriteRuleSubtreeStream(adaptor, "rule mu", mu!=null ? mu.Tree : null);
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

                    	root_0 = (IASTNode)adaptor.GetNilNode();
                    	// 493:31: -> ^( UNARY_MINUS[$m] $mu)
                    	{
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:493:34: ^( UNARY_MINUS[$m] $mu)
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
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:494:4: p= PLUS pu= unaryExpression
                    {
                    	p=(IToken)Match(input,PLUS,FOLLOW_PLUS_in_unaryExpression2425);  
                    	stream_PLUS.Add(p);

                    	PushFollow(FOLLOW_unaryExpression_in_unaryExpression2429);
                    	pu = unaryExpression();
                    	state.followingStackPointer--;

                    	stream_unaryExpression.Add(pu.Tree);


                    	// AST REWRITE
                    	// elements:          pu
                    	// token labels:      
                    	// rule labels:       pu, retval
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_pu = new RewriteRuleSubtreeStream(adaptor, "rule pu", pu!=null ? pu.Tree : null);
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

                    	root_0 = (IASTNode)adaptor.GetNilNode();
                    	// 494:30: -> ^( UNARY_PLUS[$p] $pu)
                    	{
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:494:33: ^( UNARY_PLUS[$p] $pu)
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
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:495:4: c= caseExpression
                    {
                    	PushFollow(FOLLOW_caseExpression_in_unaryExpression2446);
                    	c = caseExpression();
                    	state.followingStackPointer--;

                    	stream_caseExpression.Add(c.Tree);


                    	// AST REWRITE
                    	// elements:          c
                    	// token labels:      
                    	// rule labels:       c, retval
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_c = new RewriteRuleSubtreeStream(adaptor, "rule c", c!=null ? c.Tree : null);
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

                    	root_0 = (IASTNode)adaptor.GetNilNode();
                    	// 495:21: -> ^( $c)
                    	{
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:495:24: ^( $c)
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
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:496:4: q= quantifiedExpression
                    {
                    	PushFollow(FOLLOW_quantifiedExpression_in_unaryExpression2460);
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
                    	// 496:27: -> ^( $q)
                    	{
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:496:30: ^( $q)
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
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:497:4: a= atom
                    {
                    	PushFollow(FOLLOW_atom_in_unaryExpression2475);
                    	a = atom();
                    	state.followingStackPointer--;

                    	stream_atom.Add(a.Tree);


                    	// AST REWRITE
                    	// elements:          a
                    	// token labels:      
                    	// rule labels:       a, retval
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_a = new RewriteRuleSubtreeStream(adaptor, "rule a", a!=null ? a.Tree : null);
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

                    	root_0 = (IASTNode)adaptor.GetNilNode();
                    	// 497:11: -> ^( $a)
                    	{
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:497:14: ^( $a)
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:500:1: caseExpression : ( CASE ( whenClause )+ ( elseClause )? END -> ^( CASE whenClause ( elseClause )? ) | CASE unaryExpression ( altWhenClause )+ ( elseClause )? END -> ^( CASE2 unaryExpression ( altWhenClause )+ ( elseClause )? ) );
    public HqlParser.caseExpression_return caseExpression() // throws RecognitionException [1]
    {   
        HqlParser.caseExpression_return retval = new HqlParser.caseExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken CASE194 = null;
        IToken END197 = null;
        IToken CASE198 = null;
        IToken END202 = null;
        HqlParser.whenClause_return whenClause195 = default(HqlParser.whenClause_return);

        HqlParser.elseClause_return elseClause196 = default(HqlParser.elseClause_return);

        HqlParser.unaryExpression_return unaryExpression199 = default(HqlParser.unaryExpression_return);

        HqlParser.altWhenClause_return altWhenClause200 = default(HqlParser.altWhenClause_return);

        HqlParser.elseClause_return elseClause201 = default(HqlParser.elseClause_return);


        IASTNode CASE194_tree=null;
        IASTNode END197_tree=null;
        IASTNode CASE198_tree=null;
        IASTNode END202_tree=null;
        RewriteRuleTokenStream stream_CASE = new RewriteRuleTokenStream(adaptor,"token CASE");
        RewriteRuleTokenStream stream_END = new RewriteRuleTokenStream(adaptor,"token END");
        RewriteRuleSubtreeStream stream_unaryExpression = new RewriteRuleSubtreeStream(adaptor,"rule unaryExpression");
        RewriteRuleSubtreeStream stream_whenClause = new RewriteRuleSubtreeStream(adaptor,"rule whenClause");
        RewriteRuleSubtreeStream stream_elseClause = new RewriteRuleSubtreeStream(adaptor,"rule elseClause");
        RewriteRuleSubtreeStream stream_altWhenClause = new RewriteRuleSubtreeStream(adaptor,"rule altWhenClause");
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:501:2: ( CASE ( whenClause )+ ( elseClause )? END -> ^( CASE whenClause ( elseClause )? ) | CASE unaryExpression ( altWhenClause )+ ( elseClause )? END -> ^( CASE2 unaryExpression ( altWhenClause )+ ( elseClause )? ) )
            int alt73 = 2;
            int LA73_0 = input.LA(1);

            if ( (LA73_0 == CASE) )
            {
                int LA73_1 = input.LA(2);

                if ( (LA73_1 == WHEN) )
                {
                    alt73 = 1;
                }
                else if ( ((LA73_1 >= ALL && LA73_1 <= ANY) || LA73_1 == AVG || LA73_1 == COUNT || LA73_1 == ELEMENTS || (LA73_1 >= EXISTS && LA73_1 <= FALSE) || LA73_1 == INDICES || (LA73_1 >= MAX && LA73_1 <= MIN) || LA73_1 == NULL || (LA73_1 >= SOME && LA73_1 <= TRUE) || LA73_1 == CASE || LA73_1 == EMPTY || (LA73_1 >= NUM_INT && LA73_1 <= NUM_LONG) || LA73_1 == OPEN || (LA73_1 >= PLUS && LA73_1 <= MINUS) || (LA73_1 >= COLON && LA73_1 <= IDENT)) )
                {
                    alt73 = 2;
                }
                else 
                {
                    NoViableAltException nvae_d73s1 =
                        new NoViableAltException("", 73, 1, input);

                    throw nvae_d73s1;
                }
            }
            else 
            {
                NoViableAltException nvae_d73s0 =
                    new NoViableAltException("", 73, 0, input);

                throw nvae_d73s0;
            }
            switch (alt73) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:501:4: CASE ( whenClause )+ ( elseClause )? END
                    {
                    	CASE194=(IToken)Match(input,CASE,FOLLOW_CASE_in_caseExpression2494);  
                    	stream_CASE.Add(CASE194);

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:501:9: ( whenClause )+
                    	int cnt69 = 0;
                    	do 
                    	{
                    	    int alt69 = 2;
                    	    int LA69_0 = input.LA(1);

                    	    if ( (LA69_0 == WHEN) )
                    	    {
                    	        alt69 = 1;
                    	    }


                    	    switch (alt69) 
                    		{
                    			case 1 :
                    			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:501:10: whenClause
                    			    {
                    			    	PushFollow(FOLLOW_whenClause_in_caseExpression2497);
                    			    	whenClause195 = whenClause();
                    			    	state.followingStackPointer--;

                    			    	stream_whenClause.Add(whenClause195.Tree);

                    			    }
                    			    break;

                    			default:
                    			    if ( cnt69 >= 1 ) goto loop69;
                    		            EarlyExitException eee69 =
                    		                new EarlyExitException(69, input);
                    		            throw eee69;
                    	    }
                    	    cnt69++;
                    	} while (true);

                    	loop69:
                    		;	// Stops C# compiler whinging that label 'loop69' has no statements

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:501:23: ( elseClause )?
                    	int alt70 = 2;
                    	int LA70_0 = input.LA(1);

                    	if ( (LA70_0 == ELSE) )
                    	{
                    	    alt70 = 1;
                    	}
                    	switch (alt70) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:501:24: elseClause
                    	        {
                    	        	PushFollow(FOLLOW_elseClause_in_caseExpression2502);
                    	        	elseClause196 = elseClause();
                    	        	state.followingStackPointer--;

                    	        	stream_elseClause.Add(elseClause196.Tree);

                    	        }
                    	        break;

                    	}

                    	END197=(IToken)Match(input,END,FOLLOW_END_in_caseExpression2506);  
                    	stream_END.Add(END197);



                    	// AST REWRITE
                    	// elements:          CASE, elseClause, whenClause
                    	// token labels:      
                    	// rule labels:       retval
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

                    	root_0 = (IASTNode)adaptor.GetNilNode();
                    	// 502:3: -> ^( CASE whenClause ( elseClause )? )
                    	{
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:502:6: ^( CASE whenClause ( elseClause )? )
                    	    {
                    	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
                    	    root_1 = (IASTNode)adaptor.BecomeRoot(stream_CASE.NextNode(), root_1);

                    	    adaptor.AddChild(root_1, stream_whenClause.NextTree());
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:502:24: ( elseClause )?
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
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:503:4: CASE unaryExpression ( altWhenClause )+ ( elseClause )? END
                    {
                    	CASE198=(IToken)Match(input,CASE,FOLLOW_CASE_in_caseExpression2525);  
                    	stream_CASE.Add(CASE198);

                    	PushFollow(FOLLOW_unaryExpression_in_caseExpression2527);
                    	unaryExpression199 = unaryExpression();
                    	state.followingStackPointer--;

                    	stream_unaryExpression.Add(unaryExpression199.Tree);
                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:503:25: ( altWhenClause )+
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
                    			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:503:26: altWhenClause
                    			    {
                    			    	PushFollow(FOLLOW_altWhenClause_in_caseExpression2530);
                    			    	altWhenClause200 = altWhenClause();
                    			    	state.followingStackPointer--;

                    			    	stream_altWhenClause.Add(altWhenClause200.Tree);

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
                    		;	// Stops C# compiler whinging that label 'loop71' has no statements

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:503:42: ( elseClause )?
                    	int alt72 = 2;
                    	int LA72_0 = input.LA(1);

                    	if ( (LA72_0 == ELSE) )
                    	{
                    	    alt72 = 1;
                    	}
                    	switch (alt72) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:503:43: elseClause
                    	        {
                    	        	PushFollow(FOLLOW_elseClause_in_caseExpression2535);
                    	        	elseClause201 = elseClause();
                    	        	state.followingStackPointer--;

                    	        	stream_elseClause.Add(elseClause201.Tree);

                    	        }
                    	        break;

                    	}

                    	END202=(IToken)Match(input,END,FOLLOW_END_in_caseExpression2539);  
                    	stream_END.Add(END202);



                    	// AST REWRITE
                    	// elements:          unaryExpression, elseClause, altWhenClause
                    	// token labels:      
                    	// rule labels:       retval
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

                    	root_0 = (IASTNode)adaptor.GetNilNode();
                    	// 504:3: -> ^( CASE2 unaryExpression ( altWhenClause )+ ( elseClause )? )
                    	{
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:504:6: ^( CASE2 unaryExpression ( altWhenClause )+ ( elseClause )? )
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
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:504:45: ( elseClause )?
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:507:1: whenClause : ( WHEN logicalExpression THEN expression ) ;
    public HqlParser.whenClause_return whenClause() // throws RecognitionException [1]
    {   
        HqlParser.whenClause_return retval = new HqlParser.whenClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken WHEN203 = null;
        IToken THEN205 = null;
        HqlParser.logicalExpression_return logicalExpression204 = default(HqlParser.logicalExpression_return);

        HqlParser.expression_return expression206 = default(HqlParser.expression_return);


        IASTNode WHEN203_tree=null;
        IASTNode THEN205_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:508:2: ( ( WHEN logicalExpression THEN expression ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:508:4: ( WHEN logicalExpression THEN expression )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:508:4: ( WHEN logicalExpression THEN expression )
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:508:5: WHEN logicalExpression THEN expression
            	{
            		WHEN203=(IToken)Match(input,WHEN,FOLLOW_WHEN_in_whenClause2568); 
            			WHEN203_tree = (IASTNode)adaptor.Create(WHEN203);
            			root_0 = (IASTNode)adaptor.BecomeRoot(WHEN203_tree, root_0);

            		PushFollow(FOLLOW_logicalExpression_in_whenClause2571);
            		logicalExpression204 = logicalExpression();
            		state.followingStackPointer--;

            		adaptor.AddChild(root_0, logicalExpression204.Tree);
            		THEN205=(IToken)Match(input,THEN,FOLLOW_THEN_in_whenClause2573); 
            		PushFollow(FOLLOW_expression_in_whenClause2576);
            		expression206 = expression();
            		state.followingStackPointer--;

            		adaptor.AddChild(root_0, expression206.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:511:1: altWhenClause : ( WHEN unaryExpression THEN expression ) ;
    public HqlParser.altWhenClause_return altWhenClause() // throws RecognitionException [1]
    {   
        HqlParser.altWhenClause_return retval = new HqlParser.altWhenClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken WHEN207 = null;
        IToken THEN209 = null;
        HqlParser.unaryExpression_return unaryExpression208 = default(HqlParser.unaryExpression_return);

        HqlParser.expression_return expression210 = default(HqlParser.expression_return);


        IASTNode WHEN207_tree=null;
        IASTNode THEN209_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:512:2: ( ( WHEN unaryExpression THEN expression ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:512:4: ( WHEN unaryExpression THEN expression )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:512:4: ( WHEN unaryExpression THEN expression )
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:512:5: WHEN unaryExpression THEN expression
            	{
            		WHEN207=(IToken)Match(input,WHEN,FOLLOW_WHEN_in_altWhenClause2590); 
            			WHEN207_tree = (IASTNode)adaptor.Create(WHEN207);
            			root_0 = (IASTNode)adaptor.BecomeRoot(WHEN207_tree, root_0);

            		PushFollow(FOLLOW_unaryExpression_in_altWhenClause2593);
            		unaryExpression208 = unaryExpression();
            		state.followingStackPointer--;

            		adaptor.AddChild(root_0, unaryExpression208.Tree);
            		THEN209=(IToken)Match(input,THEN,FOLLOW_THEN_in_altWhenClause2595); 
            		PushFollow(FOLLOW_expression_in_altWhenClause2598);
            		expression210 = expression();
            		state.followingStackPointer--;

            		adaptor.AddChild(root_0, expression210.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:515:1: elseClause : ( ELSE expression ) ;
    public HqlParser.elseClause_return elseClause() // throws RecognitionException [1]
    {   
        HqlParser.elseClause_return retval = new HqlParser.elseClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken ELSE211 = null;
        HqlParser.expression_return expression212 = default(HqlParser.expression_return);


        IASTNode ELSE211_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:516:2: ( ( ELSE expression ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:516:4: ( ELSE expression )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:516:4: ( ELSE expression )
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:516:5: ELSE expression
            	{
            		ELSE211=(IToken)Match(input,ELSE,FOLLOW_ELSE_in_elseClause2612); 
            			ELSE211_tree = (IASTNode)adaptor.Create(ELSE211);
            			root_0 = (IASTNode)adaptor.BecomeRoot(ELSE211_tree, root_0);

            		PushFollow(FOLLOW_expression_in_elseClause2615);
            		expression212 = expression();
            		state.followingStackPointer--;

            		adaptor.AddChild(root_0, expression212.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:519:1: quantifiedExpression : ( SOME | EXISTS | ALL | ANY ) ( identifier | collectionExpr | ( OPEN ( subQuery ) CLOSE ) ) ;
    public HqlParser.quantifiedExpression_return quantifiedExpression() // throws RecognitionException [1]
    {   
        HqlParser.quantifiedExpression_return retval = new HqlParser.quantifiedExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken SOME213 = null;
        IToken EXISTS214 = null;
        IToken ALL215 = null;
        IToken ANY216 = null;
        IToken OPEN219 = null;
        IToken CLOSE221 = null;
        HqlParser.identifier_return identifier217 = default(HqlParser.identifier_return);

        HqlParser.collectionExpr_return collectionExpr218 = default(HqlParser.collectionExpr_return);

        HqlParser.subQuery_return subQuery220 = default(HqlParser.subQuery_return);


        IASTNode SOME213_tree=null;
        IASTNode EXISTS214_tree=null;
        IASTNode ALL215_tree=null;
        IASTNode ANY216_tree=null;
        IASTNode OPEN219_tree=null;
        IASTNode CLOSE221_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:520:2: ( ( SOME | EXISTS | ALL | ANY ) ( identifier | collectionExpr | ( OPEN ( subQuery ) CLOSE ) ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:520:4: ( SOME | EXISTS | ALL | ANY ) ( identifier | collectionExpr | ( OPEN ( subQuery ) CLOSE ) )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:520:4: ( SOME | EXISTS | ALL | ANY )
            	int alt74 = 4;
            	switch ( input.LA(1) ) 
            	{
            	case SOME:
            		{
            	    alt74 = 1;
            	    }
            	    break;
            	case EXISTS:
            		{
            	    alt74 = 2;
            	    }
            	    break;
            	case ALL:
            		{
            	    alt74 = 3;
            	    }
            	    break;
            	case ANY:
            		{
            	    alt74 = 4;
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
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:520:6: SOME
            	        {
            	        	SOME213=(IToken)Match(input,SOME,FOLLOW_SOME_in_quantifiedExpression2630); 
            	        		SOME213_tree = (IASTNode)adaptor.Create(SOME213);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(SOME213_tree, root_0);


            	        }
            	        break;
            	    case 2 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:520:14: EXISTS
            	        {
            	        	EXISTS214=(IToken)Match(input,EXISTS,FOLLOW_EXISTS_in_quantifiedExpression2635); 
            	        		EXISTS214_tree = (IASTNode)adaptor.Create(EXISTS214);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(EXISTS214_tree, root_0);


            	        }
            	        break;
            	    case 3 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:520:24: ALL
            	        {
            	        	ALL215=(IToken)Match(input,ALL,FOLLOW_ALL_in_quantifiedExpression2640); 
            	        		ALL215_tree = (IASTNode)adaptor.Create(ALL215);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(ALL215_tree, root_0);


            	        }
            	        break;
            	    case 4 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:520:31: ANY
            	        {
            	        	ANY216=(IToken)Match(input,ANY,FOLLOW_ANY_in_quantifiedExpression2645); 
            	        		ANY216_tree = (IASTNode)adaptor.Create(ANY216);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(ANY216_tree, root_0);


            	        }
            	        break;

            	}

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:521:2: ( identifier | collectionExpr | ( OPEN ( subQuery ) CLOSE ) )
            	int alt75 = 3;
            	switch ( input.LA(1) ) 
            	{
            	case IDENT:
            		{
            	    alt75 = 1;
            	    }
            	    break;
            	case ELEMENTS:
            	case INDICES:
            		{
            	    alt75 = 2;
            	    }
            	    break;
            	case OPEN:
            		{
            	    alt75 = 3;
            	    }
            	    break;
            		default:
            		    NoViableAltException nvae_d75s0 =
            		        new NoViableAltException("", 75, 0, input);

            		    throw nvae_d75s0;
            	}

            	switch (alt75) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:521:4: identifier
            	        {
            	        	PushFollow(FOLLOW_identifier_in_quantifiedExpression2654);
            	        	identifier217 = identifier();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, identifier217.Tree);

            	        }
            	        break;
            	    case 2 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:521:17: collectionExpr
            	        {
            	        	PushFollow(FOLLOW_collectionExpr_in_quantifiedExpression2658);
            	        	collectionExpr218 = collectionExpr();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, collectionExpr218.Tree);

            	        }
            	        break;
            	    case 3 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:521:34: ( OPEN ( subQuery ) CLOSE )
            	        {
            	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:521:34: ( OPEN ( subQuery ) CLOSE )
            	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:521:35: OPEN ( subQuery ) CLOSE
            	        	{
            	        		OPEN219=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_quantifiedExpression2663); 
            	        		// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:521:41: ( subQuery )
            	        		// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:521:43: subQuery
            	        		{
            	        			PushFollow(FOLLOW_subQuery_in_quantifiedExpression2668);
            	        			subQuery220 = subQuery();
            	        			state.followingStackPointer--;

            	        			adaptor.AddChild(root_0, subQuery220.Tree);

            	        		}

            	        		CLOSE221=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_quantifiedExpression2672); 

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:527:1: atom : primaryExpression ( DOT identifier ( options {greedy=true; } : (op= OPEN exprList CLOSE ) )? | lb= OPEN_BRACKET expression CLOSE_BRACKET )* ;
    public HqlParser.atom_return atom() // throws RecognitionException [1]
    {   
        HqlParser.atom_return retval = new HqlParser.atom_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken op = null;
        IToken lb = null;
        IToken DOT223 = null;
        IToken CLOSE226 = null;
        IToken CLOSE_BRACKET228 = null;
        HqlParser.primaryExpression_return primaryExpression222 = default(HqlParser.primaryExpression_return);

        HqlParser.identifier_return identifier224 = default(HqlParser.identifier_return);

        HqlParser.exprList_return exprList225 = default(HqlParser.exprList_return);

        HqlParser.expression_return expression227 = default(HqlParser.expression_return);


        IASTNode op_tree=null;
        IASTNode lb_tree=null;
        IASTNode DOT223_tree=null;
        IASTNode CLOSE226_tree=null;
        IASTNode CLOSE_BRACKET228_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:528:3: ( primaryExpression ( DOT identifier ( options {greedy=true; } : (op= OPEN exprList CLOSE ) )? | lb= OPEN_BRACKET expression CLOSE_BRACKET )* )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:528:5: primaryExpression ( DOT identifier ( options {greedy=true; } : (op= OPEN exprList CLOSE ) )? | lb= OPEN_BRACKET expression CLOSE_BRACKET )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_primaryExpression_in_atom2691);
            	primaryExpression222 = primaryExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, primaryExpression222.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:529:3: ( DOT identifier ( options {greedy=true; } : (op= OPEN exprList CLOSE ) )? | lb= OPEN_BRACKET expression CLOSE_BRACKET )*
            	do 
            	{
            	    int alt77 = 3;
            	    int LA77_0 = input.LA(1);

            	    if ( (LA77_0 == DOT) )
            	    {
            	        alt77 = 1;
            	    }
            	    else if ( (LA77_0 == OPEN_BRACKET) )
            	    {
            	        alt77 = 2;
            	    }


            	    switch (alt77) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:530:4: DOT identifier ( options {greedy=true; } : (op= OPEN exprList CLOSE ) )?
            			    {
            			    	DOT223=(IToken)Match(input,DOT,FOLLOW_DOT_in_atom2700); 
            			    		DOT223_tree = (IASTNode)adaptor.Create(DOT223);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(DOT223_tree, root_0);

            			    	PushFollow(FOLLOW_identifier_in_atom2703);
            			    	identifier224 = identifier();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, identifier224.Tree);
            			    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:531:5: ( options {greedy=true; } : (op= OPEN exprList CLOSE ) )?
            			    	int alt76 = 2;
            			    	int LA76_0 = input.LA(1);

            			    	if ( (LA76_0 == OPEN) )
            			    	{
            			    	    alt76 = 1;
            			    	}
            			    	switch (alt76) 
            			    	{
            			    	    case 1 :
            			    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:532:6: (op= OPEN exprList CLOSE )
            			    	        {
            			    	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:532:6: (op= OPEN exprList CLOSE )
            			    	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:532:8: op= OPEN exprList CLOSE
            			    	        	{
            			    	        		op=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_atom2731); 
            			    	        			op_tree = (IASTNode)adaptor.Create(op);
            			    	        			root_0 = (IASTNode)adaptor.BecomeRoot(op_tree, root_0);

            			    	        		op.Type = METHOD_CALL; 
            			    	        		PushFollow(FOLLOW_exprList_in_atom2736);
            			    	        		exprList225 = exprList();
            			    	        		state.followingStackPointer--;

            			    	        		adaptor.AddChild(root_0, exprList225.Tree);
            			    	        		CLOSE226=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_atom2738); 

            			    	        	}


            			    	        }
            			    	        break;

            			    	}


            			    }
            			    break;
            			case 2 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:533:5: lb= OPEN_BRACKET expression CLOSE_BRACKET
            			    {
            			    	lb=(IToken)Match(input,OPEN_BRACKET,FOLLOW_OPEN_BRACKET_in_atom2752); 
            			    		lb_tree = (IASTNode)adaptor.Create(lb);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(lb_tree, root_0);

            			    	lb.Type = INDEX_OP; 
            			    	PushFollow(FOLLOW_expression_in_atom2757);
            			    	expression227 = expression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, expression227.Tree);
            			    	CLOSE_BRACKET228=(IToken)Match(input,CLOSE_BRACKET,FOLLOW_CLOSE_BRACKET_in_atom2759); 

            			    }
            			    break;

            			default:
            			    goto loop77;
            	    }
            	} while (true);

            	loop77:
            		;	// Stops C# compiler whining that label 'loop77' has no statements


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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:538:1: primaryExpression : ( identPrimary ( options {greedy=true; } : DOT 'class' )? | constant | COLON identifier | OPEN ( expressionOrVector | subQuery ) CLOSE | PARAM ( NUM_INT )? );
    public HqlParser.primaryExpression_return primaryExpression() // throws RecognitionException [1]
    {   
        HqlParser.primaryExpression_return retval = new HqlParser.primaryExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken DOT230 = null;
        IToken string_literal231 = null;
        IToken COLON233 = null;
        IToken OPEN235 = null;
        IToken CLOSE238 = null;
        IToken PARAM239 = null;
        IToken NUM_INT240 = null;
        HqlParser.identPrimary_return identPrimary229 = default(HqlParser.identPrimary_return);

        HqlParser.constant_return constant232 = default(HqlParser.constant_return);

        HqlParser.identifier_return identifier234 = default(HqlParser.identifier_return);

        HqlParser.expressionOrVector_return expressionOrVector236 = default(HqlParser.expressionOrVector_return);

        HqlParser.subQuery_return subQuery237 = default(HqlParser.subQuery_return);


        IASTNode DOT230_tree=null;
        IASTNode string_literal231_tree=null;
        IASTNode COLON233_tree=null;
        IASTNode OPEN235_tree=null;
        IASTNode CLOSE238_tree=null;
        IASTNode PARAM239_tree=null;
        IASTNode NUM_INT240_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:539:2: ( identPrimary ( options {greedy=true; } : DOT 'class' )? | constant | COLON identifier | OPEN ( expressionOrVector | subQuery ) CLOSE | PARAM ( NUM_INT )? )
            int alt81 = 5;
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
                alt81 = 1;
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
                alt81 = 2;
                }
                break;
            case COLON:
            	{
                alt81 = 3;
                }
                break;
            case OPEN:
            	{
                alt81 = 4;
                }
                break;
            case PARAM:
            	{
                alt81 = 5;
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
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:539:6: identPrimary ( options {greedy=true; } : DOT 'class' )?
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_identPrimary_in_primaryExpression2779);
                    	identPrimary229 = identPrimary();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, identPrimary229.Tree);
                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:539:19: ( options {greedy=true; } : DOT 'class' )?
                    	int alt78 = 2;
                    	int LA78_0 = input.LA(1);

                    	if ( (LA78_0 == DOT) )
                    	{
                    	    int LA78_1 = input.LA(2);

                    	    if ( (LA78_1 == CLASS) )
                    	    {
                    	        alt78 = 1;
                    	    }
                    	}
                    	switch (alt78) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:539:46: DOT 'class'
                    	        {
                    	        	DOT230=(IToken)Match(input,DOT,FOLLOW_DOT_in_primaryExpression2792); 
                    	        		DOT230_tree = (IASTNode)adaptor.Create(DOT230);
                    	        		root_0 = (IASTNode)adaptor.BecomeRoot(DOT230_tree, root_0);

                    	        	string_literal231=(IToken)Match(input,CLASS,FOLLOW_CLASS_in_primaryExpression2795); 
                    	        		string_literal231_tree = (IASTNode)adaptor.Create(string_literal231);
                    	        		adaptor.AddChild(root_0, string_literal231_tree);


                    	        }
                    	        break;

                    	}


                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:540:6: constant
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_constant_in_primaryExpression2805);
                    	constant232 = constant();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, constant232.Tree);

                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:541:6: COLON identifier
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	COLON233=(IToken)Match(input,COLON,FOLLOW_COLON_in_primaryExpression2812); 
                    		COLON233_tree = (IASTNode)adaptor.Create(COLON233);
                    		root_0 = (IASTNode)adaptor.BecomeRoot(COLON233_tree, root_0);

                    	PushFollow(FOLLOW_identifier_in_primaryExpression2815);
                    	identifier234 = identifier();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, identifier234.Tree);

                    }
                    break;
                case 4 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:543:6: OPEN ( expressionOrVector | subQuery ) CLOSE
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	OPEN235=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_primaryExpression2824); 
                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:543:12: ( expressionOrVector | subQuery )
                    	int alt79 = 2;
                    	int LA79_0 = input.LA(1);

                    	if ( ((LA79_0 >= ALL && LA79_0 <= ANY) || LA79_0 == AVG || LA79_0 == COUNT || LA79_0 == ELEMENTS || (LA79_0 >= EXISTS && LA79_0 <= FALSE) || LA79_0 == INDICES || (LA79_0 >= MAX && LA79_0 <= MIN) || (LA79_0 >= NOT && LA79_0 <= NULL) || (LA79_0 >= SOME && LA79_0 <= TRUE) || LA79_0 == CASE || LA79_0 == EMPTY || (LA79_0 >= NUM_INT && LA79_0 <= NUM_LONG) || LA79_0 == OPEN || LA79_0 == BNOT || (LA79_0 >= PLUS && LA79_0 <= MINUS) || (LA79_0 >= COLON && LA79_0 <= IDENT)) )
                    	{
                    	    alt79 = 1;
                    	}
                    	else if ( (LA79_0 == EOF || LA79_0 == FROM || LA79_0 == GROUP || LA79_0 == ORDER || LA79_0 == SELECT || LA79_0 == UNION || LA79_0 == WHERE || LA79_0 == CLOSE) )
                    	{
                    	    alt79 = 2;
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
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:543:13: expressionOrVector
                    	        {
                    	        	PushFollow(FOLLOW_expressionOrVector_in_primaryExpression2828);
                    	        	expressionOrVector236 = expressionOrVector();
                    	        	state.followingStackPointer--;

                    	        	adaptor.AddChild(root_0, expressionOrVector236.Tree);

                    	        }
                    	        break;
                    	    case 2 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:543:34: subQuery
                    	        {
                    	        	PushFollow(FOLLOW_subQuery_in_primaryExpression2832);
                    	        	subQuery237 = subQuery();
                    	        	state.followingStackPointer--;

                    	        	adaptor.AddChild(root_0, subQuery237.Tree);

                    	        }
                    	        break;

                    	}

                    	CLOSE238=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_primaryExpression2835); 

                    }
                    break;
                case 5 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:544:6: PARAM ( NUM_INT )?
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PARAM239=(IToken)Match(input,PARAM,FOLLOW_PARAM_in_primaryExpression2843); 
                    		PARAM239_tree = (IASTNode)adaptor.Create(PARAM239);
                    		root_0 = (IASTNode)adaptor.BecomeRoot(PARAM239_tree, root_0);

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:544:13: ( NUM_INT )?
                    	int alt80 = 2;
                    	int LA80_0 = input.LA(1);

                    	if ( (LA80_0 == NUM_INT) )
                    	{
                    	    alt80 = 1;
                    	}
                    	switch (alt80) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:544:14: NUM_INT
                    	        {
                    	        	NUM_INT240=(IToken)Match(input,NUM_INT,FOLLOW_NUM_INT_in_primaryExpression2847); 
                    	        		NUM_INT240_tree = (IASTNode)adaptor.Create(NUM_INT240);
                    	        		adaptor.AddChild(root_0, NUM_INT240_tree);


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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:549:1: expressionOrVector : e= expression (v= vectorExpr )? -> {v != null}? ^( VECTOR_EXPR[\"{vector}\"] $e $v) -> ^( $e) ;
    public HqlParser.expressionOrVector_return expressionOrVector() // throws RecognitionException [1]
    {   
        HqlParser.expressionOrVector_return retval = new HqlParser.expressionOrVector_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.expression_return e = default(HqlParser.expression_return);

        HqlParser.vectorExpr_return v = default(HqlParser.vectorExpr_return);


        RewriteRuleSubtreeStream stream_vectorExpr = new RewriteRuleSubtreeStream(adaptor,"rule vectorExpr");
        RewriteRuleSubtreeStream stream_expression = new RewriteRuleSubtreeStream(adaptor,"rule expression");
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:550:2: (e= expression (v= vectorExpr )? -> {v != null}? ^( VECTOR_EXPR[\"{vector}\"] $e $v) -> ^( $e) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:550:4: e= expression (v= vectorExpr )?
            {
            	PushFollow(FOLLOW_expression_in_expressionOrVector2865);
            	e = expression();
            	state.followingStackPointer--;

            	stream_expression.Add(e.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:550:17: (v= vectorExpr )?
            	int alt82 = 2;
            	int LA82_0 = input.LA(1);

            	if ( (LA82_0 == COMMA) )
            	{
            	    alt82 = 1;
            	}
            	switch (alt82) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:550:19: v= vectorExpr
            	        {
            	        	PushFollow(FOLLOW_vectorExpr_in_expressionOrVector2871);
            	        	v = vectorExpr();
            	        	state.followingStackPointer--;

            	        	stream_vectorExpr.Add(v.Tree);

            	        }
            	        break;

            	}



            	// AST REWRITE
            	// elements:          e, e, v
            	// token labels:      
            	// rule labels:       retval, v, e
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);
            	RewriteRuleSubtreeStream stream_v = new RewriteRuleSubtreeStream(adaptor, "rule v", v!=null ? v.Tree : null);
            	RewriteRuleSubtreeStream stream_e = new RewriteRuleSubtreeStream(adaptor, "rule e", e!=null ? e.Tree : null);

            	root_0 = (IASTNode)adaptor.GetNilNode();
            	// 551:2: -> {v != null}? ^( VECTOR_EXPR[\"{vector}\"] $e $v)
            	if (v != null)
            	{
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:551:18: ^( VECTOR_EXPR[\"{vector}\"] $e $v)
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(VECTOR_EXPR, "{vector}"), root_1);

            	    adaptor.AddChild(root_1, stream_e.NextTree());
            	    adaptor.AddChild(root_1, stream_v.NextTree());

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}
            	else // 552:2: -> ^( $e)
            	{
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:552:5: ^( $e)
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:555:1: vectorExpr : COMMA expression ( COMMA expression )* ;
    public HqlParser.vectorExpr_return vectorExpr() // throws RecognitionException [1]
    {   
        HqlParser.vectorExpr_return retval = new HqlParser.vectorExpr_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken COMMA241 = null;
        IToken COMMA243 = null;
        HqlParser.expression_return expression242 = default(HqlParser.expression_return);

        HqlParser.expression_return expression244 = default(HqlParser.expression_return);


        IASTNode COMMA241_tree=null;
        IASTNode COMMA243_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:556:2: ( COMMA expression ( COMMA expression )* )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:556:4: COMMA expression ( COMMA expression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	COMMA241=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_vectorExpr2910); 
            	PushFollow(FOLLOW_expression_in_vectorExpr2913);
            	expression242 = expression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, expression242.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:556:22: ( COMMA expression )*
            	do 
            	{
            	    int alt83 = 2;
            	    int LA83_0 = input.LA(1);

            	    if ( (LA83_0 == COMMA) )
            	    {
            	        alt83 = 1;
            	    }


            	    switch (alt83) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:556:23: COMMA expression
            			    {
            			    	COMMA243=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_vectorExpr2916); 
            			    	PushFollow(FOLLOW_expression_in_vectorExpr2919);
            			    	expression244 = expression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, expression244.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:562:1: identPrimary : ( identifier ( options {greedy=true; } : DOT ( identifier | o= OBJECT ) )* ( (op= OPEN exprList CLOSE ) )? | aggregate );
    public HqlParser.identPrimary_return identPrimary() // throws RecognitionException [1]
    {   
        HqlParser.identPrimary_return retval = new HqlParser.identPrimary_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken o = null;
        IToken op = null;
        IToken DOT246 = null;
        IToken CLOSE249 = null;
        HqlParser.identifier_return identifier245 = default(HqlParser.identifier_return);

        HqlParser.identifier_return identifier247 = default(HqlParser.identifier_return);

        HqlParser.exprList_return exprList248 = default(HqlParser.exprList_return);

        HqlParser.aggregate_return aggregate250 = default(HqlParser.aggregate_return);


        IASTNode o_tree=null;
        IASTNode op_tree=null;
        IASTNode DOT246_tree=null;
        IASTNode CLOSE249_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:563:2: ( identifier ( options {greedy=true; } : DOT ( identifier | o= OBJECT ) )* ( (op= OPEN exprList CLOSE ) )? | aggregate )
            int alt87 = 2;
            int LA87_0 = input.LA(1);

            if ( (LA87_0 == IDENT) )
            {
                alt87 = 1;
            }
            else if ( (LA87_0 == AVG || LA87_0 == COUNT || LA87_0 == ELEMENTS || LA87_0 == INDICES || (LA87_0 >= MAX && LA87_0 <= MIN) || LA87_0 == SUM) )
            {
                alt87 = 2;
            }
            else 
            {
                NoViableAltException nvae_d87s0 =
                    new NoViableAltException("", 87, 0, input);

                throw nvae_d87s0;
            }
            switch (alt87) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:563:4: identifier ( options {greedy=true; } : DOT ( identifier | o= OBJECT ) )* ( (op= OPEN exprList CLOSE ) )?
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_identifier_in_identPrimary2935);
                    	identifier245 = identifier();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, identifier245.Tree);
                    	 HandleDotIdent(); 
                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:564:4: ( options {greedy=true; } : DOT ( identifier | o= OBJECT ) )*
                    	do 
                    	{
                    	    int alt85 = 2;
                    	    int LA85_0 = input.LA(1);

                    	    if ( (LA85_0 == DOT) )
                    	    {
                    	        int LA85_2 = input.LA(2);

                    	        if ( (LA85_2 == OBJECT || LA85_2 == IDENT) )
                    	        {
                    	            alt85 = 1;
                    	        }


                    	    }


                    	    switch (alt85) 
                    		{
                    			case 1 :
                    			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:564:31: DOT ( identifier | o= OBJECT )
                    			    {
                    			    	DOT246=(IToken)Match(input,DOT,FOLLOW_DOT_in_identPrimary2953); 
                    			    		DOT246_tree = (IASTNode)adaptor.Create(DOT246);
                    			    		root_0 = (IASTNode)adaptor.BecomeRoot(DOT246_tree, root_0);

                    			    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:564:36: ( identifier | o= OBJECT )
                    			    	int alt84 = 2;
                    			    	int LA84_0 = input.LA(1);

                    			    	if ( (LA84_0 == IDENT) )
                    			    	{
                    			    	    alt84 = 1;
                    			    	}
                    			    	else if ( (LA84_0 == OBJECT) )
                    			    	{
                    			    	    alt84 = 2;
                    			    	}
                    			    	else 
                    			    	{
                    			    	    NoViableAltException nvae_d84s0 =
                    			    	        new NoViableAltException("", 84, 0, input);

                    			    	    throw nvae_d84s0;
                    			    	}
                    			    	switch (alt84) 
                    			    	{
                    			    	    case 1 :
                    			    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:564:38: identifier
                    			    	        {
                    			    	        	PushFollow(FOLLOW_identifier_in_identPrimary2958);
                    			    	        	identifier247 = identifier();
                    			    	        	state.followingStackPointer--;

                    			    	        	adaptor.AddChild(root_0, identifier247.Tree);

                    			    	        }
                    			    	        break;
                    			    	    case 2 :
                    			    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:564:51: o= OBJECT
                    			    	        {
                    			    	        	o=(IToken)Match(input,OBJECT,FOLLOW_OBJECT_in_identPrimary2964); 
                    			    	        		o_tree = (IASTNode)adaptor.Create(o);
                    			    	        		adaptor.AddChild(root_0, o_tree);

                    			    	        	 o.Type = IDENT; 

                    			    	        }
                    			    	        break;

                    			    	}


                    			    }
                    			    break;

                    			default:
                    			    goto loop85;
                    	    }
                    	} while (true);

                    	loop85:
                    		;	// Stops C# compiler whining that label 'loop85' has no statements

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:565:4: ( (op= OPEN exprList CLOSE ) )?
                    	int alt86 = 2;
                    	int LA86_0 = input.LA(1);

                    	if ( (LA86_0 == OPEN) )
                    	{
                    	    alt86 = 1;
                    	}
                    	switch (alt86) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:565:6: (op= OPEN exprList CLOSE )
                    	        {
                    	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:565:6: (op= OPEN exprList CLOSE )
                    	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:565:8: op= OPEN exprList CLOSE
                    	        	{
                    	        		op=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_identPrimary2982); 
                    	        			op_tree = (IASTNode)adaptor.Create(op);
                    	        			root_0 = (IASTNode)adaptor.BecomeRoot(op_tree, root_0);

                    	        		 op.Type = METHOD_CALL;
                    	        		PushFollow(FOLLOW_exprList_in_identPrimary2987);
                    	        		exprList248 = exprList();
                    	        		state.followingStackPointer--;

                    	        		adaptor.AddChild(root_0, exprList248.Tree);
                    	        		CLOSE249=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_identPrimary2989); 

                    	        	}


                    	        }
                    	        break;

                    	}


                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:568:4: aggregate
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_aggregate_in_identPrimary3005);
                    	aggregate250 = aggregate();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, aggregate250.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:576:1: aggregate : ( (op= SUM | op= AVG | op= MAX | op= MIN ) OPEN additiveExpression CLOSE -> ^( AGGREGATE[$op] additiveExpression ) | COUNT OPEN (s= STAR | p= aggregateDistinctAll ) CLOSE -> {s == null}? ^( COUNT $p) -> ^( COUNT ^( ROW_STAR[\"*\"] ) ) | collectionExpr );
    public HqlParser.aggregate_return aggregate() // throws RecognitionException [1]
    {   
        HqlParser.aggregate_return retval = new HqlParser.aggregate_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken op = null;
        IToken s = null;
        IToken OPEN251 = null;
        IToken CLOSE253 = null;
        IToken COUNT254 = null;
        IToken OPEN255 = null;
        IToken CLOSE256 = null;
        HqlParser.aggregateDistinctAll_return p = default(HqlParser.aggregateDistinctAll_return);

        HqlParser.additiveExpression_return additiveExpression252 = default(HqlParser.additiveExpression_return);

        HqlParser.collectionExpr_return collectionExpr257 = default(HqlParser.collectionExpr_return);


        IASTNode op_tree=null;
        IASTNode s_tree=null;
        IASTNode OPEN251_tree=null;
        IASTNode CLOSE253_tree=null;
        IASTNode COUNT254_tree=null;
        IASTNode OPEN255_tree=null;
        IASTNode CLOSE256_tree=null;
        RewriteRuleTokenStream stream_CLOSE = new RewriteRuleTokenStream(adaptor,"token CLOSE");
        RewriteRuleTokenStream stream_AVG = new RewriteRuleTokenStream(adaptor,"token AVG");
        RewriteRuleTokenStream stream_MAX = new RewriteRuleTokenStream(adaptor,"token MAX");
        RewriteRuleTokenStream stream_MIN = new RewriteRuleTokenStream(adaptor,"token MIN");
        RewriteRuleTokenStream stream_STAR = new RewriteRuleTokenStream(adaptor,"token STAR");
        RewriteRuleTokenStream stream_SUM = new RewriteRuleTokenStream(adaptor,"token SUM");
        RewriteRuleTokenStream stream_OPEN = new RewriteRuleTokenStream(adaptor,"token OPEN");
        RewriteRuleTokenStream stream_COUNT = new RewriteRuleTokenStream(adaptor,"token COUNT");
        RewriteRuleSubtreeStream stream_aggregateDistinctAll = new RewriteRuleSubtreeStream(adaptor,"rule aggregateDistinctAll");
        RewriteRuleSubtreeStream stream_additiveExpression = new RewriteRuleSubtreeStream(adaptor,"rule additiveExpression");
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:577:2: ( (op= SUM | op= AVG | op= MAX | op= MIN ) OPEN additiveExpression CLOSE -> ^( AGGREGATE[$op] additiveExpression ) | COUNT OPEN (s= STAR | p= aggregateDistinctAll ) CLOSE -> {s == null}? ^( COUNT $p) -> ^( COUNT ^( ROW_STAR[\"*\"] ) ) | collectionExpr )
            int alt90 = 3;
            switch ( input.LA(1) ) 
            {
            case AVG:
            case MAX:
            case MIN:
            case SUM:
            	{
                alt90 = 1;
                }
                break;
            case COUNT:
            	{
                alt90 = 2;
                }
                break;
            case ELEMENTS:
            case INDICES:
            	{
                alt90 = 3;
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
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:577:4: (op= SUM | op= AVG | op= MAX | op= MIN ) OPEN additiveExpression CLOSE
                    {
                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:577:4: (op= SUM | op= AVG | op= MAX | op= MIN )
                    	int alt88 = 4;
                    	switch ( input.LA(1) ) 
                    	{
                    	case SUM:
                    		{
                    	    alt88 = 1;
                    	    }
                    	    break;
                    	case AVG:
                    		{
                    	    alt88 = 2;
                    	    }
                    	    break;
                    	case MAX:
                    		{
                    	    alt88 = 3;
                    	    }
                    	    break;
                    	case MIN:
                    		{
                    	    alt88 = 4;
                    	    }
                    	    break;
                    		default:
                    		    NoViableAltException nvae_d88s0 =
                    		        new NoViableAltException("", 88, 0, input);

                    		    throw nvae_d88s0;
                    	}

                    	switch (alt88) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:577:6: op= SUM
                    	        {
                    	        	op=(IToken)Match(input,SUM,FOLLOW_SUM_in_aggregate3026);  
                    	        	stream_SUM.Add(op);


                    	        }
                    	        break;
                    	    case 2 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:577:15: op= AVG
                    	        {
                    	        	op=(IToken)Match(input,AVG,FOLLOW_AVG_in_aggregate3032);  
                    	        	stream_AVG.Add(op);


                    	        }
                    	        break;
                    	    case 3 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:577:24: op= MAX
                    	        {
                    	        	op=(IToken)Match(input,MAX,FOLLOW_MAX_in_aggregate3038);  
                    	        	stream_MAX.Add(op);


                    	        }
                    	        break;
                    	    case 4 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:577:33: op= MIN
                    	        {
                    	        	op=(IToken)Match(input,MIN,FOLLOW_MIN_in_aggregate3044);  
                    	        	stream_MIN.Add(op);


                    	        }
                    	        break;

                    	}

                    	OPEN251=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_aggregate3048);  
                    	stream_OPEN.Add(OPEN251);

                    	PushFollow(FOLLOW_additiveExpression_in_aggregate3050);
                    	additiveExpression252 = additiveExpression();
                    	state.followingStackPointer--;

                    	stream_additiveExpression.Add(additiveExpression252.Tree);
                    	CLOSE253=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_aggregate3052);  
                    	stream_CLOSE.Add(CLOSE253);



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
                    	// 578:3: -> ^( AGGREGATE[$op] additiveExpression )
                    	{
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:578:6: ^( AGGREGATE[$op] additiveExpression )
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
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:580:5: COUNT OPEN (s= STAR | p= aggregateDistinctAll ) CLOSE
                    {
                    	COUNT254=(IToken)Match(input,COUNT,FOLLOW_COUNT_in_aggregate3071);  
                    	stream_COUNT.Add(COUNT254);

                    	OPEN255=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_aggregate3073);  
                    	stream_OPEN.Add(OPEN255);

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:580:16: (s= STAR | p= aggregateDistinctAll )
                    	int alt89 = 2;
                    	int LA89_0 = input.LA(1);

                    	if ( (LA89_0 == STAR) )
                    	{
                    	    alt89 = 1;
                    	}
                    	else if ( (LA89_0 == ALL || (LA89_0 >= DISTINCT && LA89_0 <= ELEMENTS) || LA89_0 == INDICES || LA89_0 == IDENT) )
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
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:580:18: s= STAR
                    	        {
                    	        	s=(IToken)Match(input,STAR,FOLLOW_STAR_in_aggregate3079);  
                    	        	stream_STAR.Add(s);


                    	        }
                    	        break;
                    	    case 2 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:580:27: p= aggregateDistinctAll
                    	        {
                    	        	PushFollow(FOLLOW_aggregateDistinctAll_in_aggregate3085);
                    	        	p = aggregateDistinctAll();
                    	        	state.followingStackPointer--;

                    	        	stream_aggregateDistinctAll.Add(p.Tree);

                    	        }
                    	        break;

                    	}

                    	CLOSE256=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_aggregate3089);  
                    	stream_CLOSE.Add(CLOSE256);



                    	// AST REWRITE
                    	// elements:          COUNT, COUNT, p
                    	// token labels:      
                    	// rule labels:       p, retval
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_p = new RewriteRuleSubtreeStream(adaptor, "rule p", p!=null ? p.Tree : null);
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

                    	root_0 = (IASTNode)adaptor.GetNilNode();
                    	// 581:3: -> {s == null}? ^( COUNT $p)
                    	if (s == null)
                    	{
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:581:19: ^( COUNT $p)
                    	    {
                    	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
                    	    root_1 = (IASTNode)adaptor.BecomeRoot(stream_COUNT.NextNode(), root_1);

                    	    adaptor.AddChild(root_1, stream_p.NextTree());

                    	    adaptor.AddChild(root_0, root_1);
                    	    }

                    	}
                    	else // 582:3: -> ^( COUNT ^( ROW_STAR[\"*\"] ) )
                    	{
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:582:6: ^( COUNT ^( ROW_STAR[\"*\"] ) )
                    	    {
                    	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
                    	    root_1 = (IASTNode)adaptor.BecomeRoot(stream_COUNT.NextNode(), root_1);

                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:582:14: ^( ROW_STAR[\"*\"] )
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
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:583:5: collectionExpr
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_collectionExpr_in_aggregate3121);
                    	collectionExpr257 = collectionExpr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, collectionExpr257.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:586:1: aggregateDistinctAll : ( ( DISTINCT | ALL )? ( path | collectionExpr ) ) ;
    public HqlParser.aggregateDistinctAll_return aggregateDistinctAll() // throws RecognitionException [1]
    {   
        HqlParser.aggregateDistinctAll_return retval = new HqlParser.aggregateDistinctAll_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken set258 = null;
        HqlParser.path_return path259 = default(HqlParser.path_return);

        HqlParser.collectionExpr_return collectionExpr260 = default(HqlParser.collectionExpr_return);


        IASTNode set258_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:587:2: ( ( ( DISTINCT | ALL )? ( path | collectionExpr ) ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:587:4: ( ( DISTINCT | ALL )? ( path | collectionExpr ) )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:587:4: ( ( DISTINCT | ALL )? ( path | collectionExpr ) )
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:587:6: ( DISTINCT | ALL )? ( path | collectionExpr )
            	{
            		// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:587:6: ( DISTINCT | ALL )?
            		int alt91 = 2;
            		int LA91_0 = input.LA(1);

            		if ( (LA91_0 == ALL || LA91_0 == DISTINCT) )
            		{
            		    alt91 = 1;
            		}
            		switch (alt91) 
            		{
            		    case 1 :
            		        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:
            		        {
            		        	set258 = (IToken)input.LT(1);
            		        	if ( input.LA(1) == ALL || input.LA(1) == DISTINCT ) 
            		        	{
            		        	    input.Consume();
            		        	    adaptor.AddChild(root_0, (IASTNode)adaptor.Create(set258));
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

            		// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:587:26: ( path | collectionExpr )
            		int alt92 = 2;
            		int LA92_0 = input.LA(1);

            		if ( (LA92_0 == IDENT) )
            		{
            		    alt92 = 1;
            		}
            		else if ( (LA92_0 == ELEMENTS || LA92_0 == INDICES) )
            		{
            		    alt92 = 2;
            		}
            		else 
            		{
            		    NoViableAltException nvae_d92s0 =
            		        new NoViableAltException("", 92, 0, input);

            		    throw nvae_d92s0;
            		}
            		switch (alt92) 
            		{
            		    case 1 :
            		        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:587:28: path
            		        {
            		        	PushFollow(FOLLOW_path_in_aggregateDistinctAll3147);
            		        	path259 = path();
            		        	state.followingStackPointer--;

            		        	adaptor.AddChild(root_0, path259.Tree);

            		        }
            		        break;
            		    case 2 :
            		        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:587:35: collectionExpr
            		        {
            		        	PushFollow(FOLLOW_collectionExpr_in_aggregateDistinctAll3151);
            		        	collectionExpr260 = collectionExpr();
            		        	state.followingStackPointer--;

            		        	adaptor.AddChild(root_0, collectionExpr260.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:592:1: collectionExpr : ( ELEMENTS | INDICES ) OPEN path CLOSE ;
    public HqlParser.collectionExpr_return collectionExpr() // throws RecognitionException [1]
    {   
        HqlParser.collectionExpr_return retval = new HqlParser.collectionExpr_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken ELEMENTS261 = null;
        IToken INDICES262 = null;
        IToken OPEN263 = null;
        IToken CLOSE265 = null;
        HqlParser.path_return path264 = default(HqlParser.path_return);


        IASTNode ELEMENTS261_tree=null;
        IASTNode INDICES262_tree=null;
        IASTNode OPEN263_tree=null;
        IASTNode CLOSE265_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:593:2: ( ( ELEMENTS | INDICES ) OPEN path CLOSE )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:593:4: ( ELEMENTS | INDICES ) OPEN path CLOSE
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:593:4: ( ELEMENTS | INDICES )
            	int alt93 = 2;
            	int LA93_0 = input.LA(1);

            	if ( (LA93_0 == ELEMENTS) )
            	{
            	    alt93 = 1;
            	}
            	else if ( (LA93_0 == INDICES) )
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
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:593:5: ELEMENTS
            	        {
            	        	ELEMENTS261=(IToken)Match(input,ELEMENTS,FOLLOW_ELEMENTS_in_collectionExpr3170); 
            	        		ELEMENTS261_tree = (IASTNode)adaptor.Create(ELEMENTS261);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(ELEMENTS261_tree, root_0);


            	        }
            	        break;
            	    case 2 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:593:17: INDICES
            	        {
            	        	INDICES262=(IToken)Match(input,INDICES,FOLLOW_INDICES_in_collectionExpr3175); 
            	        		INDICES262_tree = (IASTNode)adaptor.Create(INDICES262);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(INDICES262_tree, root_0);


            	        }
            	        break;

            	}

            	OPEN263=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_collectionExpr3179); 
            	PushFollow(FOLLOW_path_in_collectionExpr3182);
            	path264 = path();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, path264.Tree);
            	CLOSE265=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_collectionExpr3184); 

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:596:1: compoundExpr : ( collectionExpr | path | ( OPEN ( subQuery | ( expression ( COMMA expression )* ) ) CLOSE ) );
    public HqlParser.compoundExpr_return compoundExpr() // throws RecognitionException [1]
    {   
        HqlParser.compoundExpr_return retval = new HqlParser.compoundExpr_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken OPEN268 = null;
        IToken COMMA271 = null;
        IToken CLOSE273 = null;
        HqlParser.collectionExpr_return collectionExpr266 = default(HqlParser.collectionExpr_return);

        HqlParser.path_return path267 = default(HqlParser.path_return);

        HqlParser.subQuery_return subQuery269 = default(HqlParser.subQuery_return);

        HqlParser.expression_return expression270 = default(HqlParser.expression_return);

        HqlParser.expression_return expression272 = default(HqlParser.expression_return);


        IASTNode OPEN268_tree=null;
        IASTNode COMMA271_tree=null;
        IASTNode CLOSE273_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:597:2: ( collectionExpr | path | ( OPEN ( subQuery | ( expression ( COMMA expression )* ) ) CLOSE ) )
            int alt96 = 3;
            switch ( input.LA(1) ) 
            {
            case ELEMENTS:
            case INDICES:
            	{
                alt96 = 1;
                }
                break;
            case IDENT:
            	{
                alt96 = 2;
                }
                break;
            case OPEN:
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
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:597:4: collectionExpr
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_collectionExpr_in_compoundExpr3239);
                    	collectionExpr266 = collectionExpr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, collectionExpr266.Tree);

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:598:4: path
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_path_in_compoundExpr3244);
                    	path267 = path();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, path267.Tree);

                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:599:4: ( OPEN ( subQuery | ( expression ( COMMA expression )* ) ) CLOSE )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:599:4: ( OPEN ( subQuery | ( expression ( COMMA expression )* ) ) CLOSE )
                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:599:5: OPEN ( subQuery | ( expression ( COMMA expression )* ) ) CLOSE
                    	{
                    		OPEN268=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_compoundExpr3250); 
                    		// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:599:11: ( subQuery | ( expression ( COMMA expression )* ) )
                    		int alt95 = 2;
                    		int LA95_0 = input.LA(1);

                    		if ( (LA95_0 == EOF || LA95_0 == FROM || LA95_0 == GROUP || LA95_0 == ORDER || LA95_0 == SELECT || LA95_0 == UNION || LA95_0 == WHERE || LA95_0 == CLOSE) )
                    		{
                    		    alt95 = 1;
                    		}
                    		else if ( ((LA95_0 >= ALL && LA95_0 <= ANY) || LA95_0 == AVG || LA95_0 == COUNT || LA95_0 == ELEMENTS || (LA95_0 >= EXISTS && LA95_0 <= FALSE) || LA95_0 == INDICES || (LA95_0 >= MAX && LA95_0 <= MIN) || (LA95_0 >= NOT && LA95_0 <= NULL) || (LA95_0 >= SOME && LA95_0 <= TRUE) || LA95_0 == CASE || LA95_0 == EMPTY || (LA95_0 >= NUM_INT && LA95_0 <= NUM_LONG) || LA95_0 == OPEN || LA95_0 == BNOT || (LA95_0 >= PLUS && LA95_0 <= MINUS) || (LA95_0 >= COLON && LA95_0 <= IDENT)) )
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
                    		        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:599:13: subQuery
                    		        {
                    		        	PushFollow(FOLLOW_subQuery_in_compoundExpr3255);
                    		        	subQuery269 = subQuery();
                    		        	state.followingStackPointer--;

                    		        	adaptor.AddChild(root_0, subQuery269.Tree);

                    		        }
                    		        break;
                    		    case 2 :
                    		        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:599:24: ( expression ( COMMA expression )* )
                    		        {
                    		        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:599:24: ( expression ( COMMA expression )* )
                    		        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:599:25: expression ( COMMA expression )*
                    		        	{
                    		        		PushFollow(FOLLOW_expression_in_compoundExpr3260);
                    		        		expression270 = expression();
                    		        		state.followingStackPointer--;

                    		        		adaptor.AddChild(root_0, expression270.Tree);
                    		        		// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:599:36: ( COMMA expression )*
                    		        		do 
                    		        		{
                    		        		    int alt94 = 2;
                    		        		    int LA94_0 = input.LA(1);

                    		        		    if ( (LA94_0 == COMMA) )
                    		        		    {
                    		        		        alt94 = 1;
                    		        		    }


                    		        		    switch (alt94) 
                    		        			{
                    		        				case 1 :
                    		        				    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:599:37: COMMA expression
                    		        				    {
                    		        				    	COMMA271=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_compoundExpr3263); 
                    		        				    	PushFollow(FOLLOW_expression_in_compoundExpr3266);
                    		        				    	expression272 = expression();
                    		        				    	state.followingStackPointer--;

                    		        				    	adaptor.AddChild(root_0, expression272.Tree);

                    		        				    }
                    		        				    break;

                    		        				default:
                    		        				    goto loop94;
                    		        		    }
                    		        		} while (true);

                    		        		loop94:
                    		        			;	// Stops C# compiler whining that label 'loop94' has no statements


                    		        	}


                    		        }
                    		        break;

                    		}

                    		CLOSE273=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_compoundExpr3273); 

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:602:1: exprList : ( TRAILING | LEADING | BOTH )? ( expression ( ( COMMA expression )+ | f= FROM expression | AS identifier )? | f2= FROM expression )? ;
    public HqlParser.exprList_return exprList() // throws RecognitionException [1]
    {   
        HqlParser.exprList_return retval = new HqlParser.exprList_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken f = null;
        IToken f2 = null;
        IToken TRAILING274 = null;
        IToken LEADING275 = null;
        IToken BOTH276 = null;
        IToken COMMA278 = null;
        IToken AS281 = null;
        HqlParser.expression_return expression277 = default(HqlParser.expression_return);

        HqlParser.expression_return expression279 = default(HqlParser.expression_return);

        HqlParser.expression_return expression280 = default(HqlParser.expression_return);

        HqlParser.identifier_return identifier282 = default(HqlParser.identifier_return);

        HqlParser.expression_return expression283 = default(HqlParser.expression_return);


        IASTNode f_tree=null;
        IASTNode f2_tree=null;
        IASTNode TRAILING274_tree=null;
        IASTNode LEADING275_tree=null;
        IASTNode BOTH276_tree=null;
        IASTNode COMMA278_tree=null;
        IASTNode AS281_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:608:2: ( ( TRAILING | LEADING | BOTH )? ( expression ( ( COMMA expression )+ | f= FROM expression | AS identifier )? | f2= FROM expression )? )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:608:4: ( TRAILING | LEADING | BOTH )? ( expression ( ( COMMA expression )+ | f= FROM expression | AS identifier )? | f2= FROM expression )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:608:4: ( TRAILING | LEADING | BOTH )?
            	int alt97 = 4;
            	switch ( input.LA(1) ) 
            	{
            	    case TRAILING:
            	    	{
            	        alt97 = 1;
            	        }
            	        break;
            	    case LEADING:
            	    	{
            	        alt97 = 2;
            	        }
            	        break;
            	    case BOTH:
            	    	{
            	        alt97 = 3;
            	        }
            	        break;
            	}

            	switch (alt97) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:608:5: TRAILING
            	        {
            	        	TRAILING274=(IToken)Match(input,TRAILING,FOLLOW_TRAILING_in_exprList3292); 
            	        		TRAILING274_tree = (IASTNode)adaptor.Create(TRAILING274);
            	        		adaptor.AddChild(root_0, TRAILING274_tree);

            	        	TRAILING274.Type = IDENT;

            	        }
            	        break;
            	    case 2 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:609:10: LEADING
            	        {
            	        	LEADING275=(IToken)Match(input,LEADING,FOLLOW_LEADING_in_exprList3305); 
            	        		LEADING275_tree = (IASTNode)adaptor.Create(LEADING275);
            	        		adaptor.AddChild(root_0, LEADING275_tree);

            	        	LEADING275.Type = IDENT;

            	        }
            	        break;
            	    case 3 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:610:10: BOTH
            	        {
            	        	BOTH276=(IToken)Match(input,BOTH,FOLLOW_BOTH_in_exprList3318); 
            	        		BOTH276_tree = (IASTNode)adaptor.Create(BOTH276);
            	        		adaptor.AddChild(root_0, BOTH276_tree);

            	        	BOTH276.Type = IDENT;

            	        }
            	        break;

            	}

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:612:4: ( expression ( ( COMMA expression )+ | f= FROM expression | AS identifier )? | f2= FROM expression )?
            	int alt100 = 3;
            	int LA100_0 = input.LA(1);

            	if ( ((LA100_0 >= ALL && LA100_0 <= ANY) || LA100_0 == AVG || LA100_0 == COUNT || LA100_0 == ELEMENTS || (LA100_0 >= EXISTS && LA100_0 <= FALSE) || LA100_0 == INDICES || (LA100_0 >= MAX && LA100_0 <= MIN) || (LA100_0 >= NOT && LA100_0 <= NULL) || (LA100_0 >= SOME && LA100_0 <= TRUE) || LA100_0 == CASE || LA100_0 == EMPTY || (LA100_0 >= NUM_INT && LA100_0 <= NUM_LONG) || LA100_0 == OPEN || LA100_0 == BNOT || (LA100_0 >= PLUS && LA100_0 <= MINUS) || (LA100_0 >= COLON && LA100_0 <= IDENT)) )
            	{
            	    alt100 = 1;
            	}
            	else if ( (LA100_0 == FROM) )
            	{
            	    alt100 = 2;
            	}
            	switch (alt100) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:613:5: expression ( ( COMMA expression )+ | f= FROM expression | AS identifier )?
            	        {
            	        	PushFollow(FOLLOW_expression_in_exprList3342);
            	        	expression277 = expression();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, expression277.Tree);
            	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:613:16: ( ( COMMA expression )+ | f= FROM expression | AS identifier )?
            	        	int alt99 = 4;
            	        	switch ( input.LA(1) ) 
            	        	{
            	        	    case COMMA:
            	        	    	{
            	        	        alt99 = 1;
            	        	        }
            	        	        break;
            	        	    case FROM:
            	        	    	{
            	        	        alt99 = 2;
            	        	        }
            	        	        break;
            	        	    case AS:
            	        	    	{
            	        	        alt99 = 3;
            	        	        }
            	        	        break;
            	        	}

            	        	switch (alt99) 
            	        	{
            	        	    case 1 :
            	        	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:613:18: ( COMMA expression )+
            	        	        {
            	        	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:613:18: ( COMMA expression )+
            	        	        	int cnt98 = 0;
            	        	        	do 
            	        	        	{
            	        	        	    int alt98 = 2;
            	        	        	    int LA98_0 = input.LA(1);

            	        	        	    if ( (LA98_0 == COMMA) )
            	        	        	    {
            	        	        	        alt98 = 1;
            	        	        	    }


            	        	        	    switch (alt98) 
            	        	        		{
            	        	        			case 1 :
            	        	        			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:613:19: COMMA expression
            	        	        			    {
            	        	        			    	COMMA278=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_exprList3347); 
            	        	        			    	PushFollow(FOLLOW_expression_in_exprList3350);
            	        	        			    	expression279 = expression();
            	        	        			    	state.followingStackPointer--;

            	        	        			    	adaptor.AddChild(root_0, expression279.Tree);

            	        	        			    }
            	        	        			    break;

            	        	        			default:
            	        	        			    if ( cnt98 >= 1 ) goto loop98;
            	        	        		            EarlyExitException eee98 =
            	        	        		                new EarlyExitException(98, input);
            	        	        		            throw eee98;
            	        	        	    }
            	        	        	    cnt98++;
            	        	        	} while (true);

            	        	        	loop98:
            	        	        		;	// Stops C# compiler whinging that label 'loop98' has no statements


            	        	        }
            	        	        break;
            	        	    case 2 :
            	        	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:614:9: f= FROM expression
            	        	        {
            	        	        	f=(IToken)Match(input,FROM,FOLLOW_FROM_in_exprList3365); 
            	        	        		f_tree = (IASTNode)adaptor.Create(f);
            	        	        		adaptor.AddChild(root_0, f_tree);

            	        	        	PushFollow(FOLLOW_expression_in_exprList3367);
            	        	        	expression280 = expression();
            	        	        	state.followingStackPointer--;

            	        	        	adaptor.AddChild(root_0, expression280.Tree);
            	        	        	f.Type = IDENT;

            	        	        }
            	        	        break;
            	        	    case 3 :
            	        	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:615:9: AS identifier
            	        	        {
            	        	        	AS281=(IToken)Match(input,AS,FOLLOW_AS_in_exprList3379); 
            	        	        	PushFollow(FOLLOW_identifier_in_exprList3382);
            	        	        	identifier282 = identifier();
            	        	        	state.followingStackPointer--;

            	        	        	adaptor.AddChild(root_0, identifier282.Tree);

            	        	        }
            	        	        break;

            	        	}


            	        }
            	        break;
            	    case 2 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:616:7: f2= FROM expression
            	        {
            	        	f2=(IToken)Match(input,FROM,FOLLOW_FROM_in_exprList3396); 
            	        		f2_tree = (IASTNode)adaptor.Create(f2);
            	        		adaptor.AddChild(root_0, f2_tree);

            	        	PushFollow(FOLLOW_expression_in_exprList3398);
            	        	expression283 = expression();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, expression283.Tree);
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:620:1: subQuery : innerSubQuery ( UNION innerSubQuery )* ;
    public HqlParser.subQuery_return subQuery() // throws RecognitionException [1]
    {   
        HqlParser.subQuery_return retval = new HqlParser.subQuery_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken UNION285 = null;
        HqlParser.innerSubQuery_return innerSubQuery284 = default(HqlParser.innerSubQuery_return);

        HqlParser.innerSubQuery_return innerSubQuery286 = default(HqlParser.innerSubQuery_return);


        IASTNode UNION285_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:621:2: ( innerSubQuery ( UNION innerSubQuery )* )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:621:4: innerSubQuery ( UNION innerSubQuery )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_innerSubQuery_in_subQuery3418);
            	innerSubQuery284 = innerSubQuery();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, innerSubQuery284.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:621:18: ( UNION innerSubQuery )*
            	do 
            	{
            	    int alt101 = 2;
            	    int LA101_0 = input.LA(1);

            	    if ( (LA101_0 == UNION) )
            	    {
            	        alt101 = 1;
            	    }


            	    switch (alt101) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:621:19: UNION innerSubQuery
            			    {
            			    	UNION285=(IToken)Match(input,UNION,FOLLOW_UNION_in_subQuery3421); 
            			    		UNION285_tree = (IASTNode)adaptor.Create(UNION285);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(UNION285_tree, root_0);

            			    	PushFollow(FOLLOW_innerSubQuery_in_subQuery3424);
            			    	innerSubQuery286 = innerSubQuery();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, innerSubQuery286.Tree);

            			    }
            			    break;

            			default:
            			    goto loop101;
            	    }
            	} while (true);

            	loop101:
            		;	// Stops C# compiler whining that label 'loop101' has no statements


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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:624:1: innerSubQuery : queryRule -> ^( QUERY[\"query\"] queryRule ) ;
    public HqlParser.innerSubQuery_return innerSubQuery() // throws RecognitionException [1]
    {   
        HqlParser.innerSubQuery_return retval = new HqlParser.innerSubQuery_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.queryRule_return queryRule287 = default(HqlParser.queryRule_return);


        RewriteRuleSubtreeStream stream_queryRule = new RewriteRuleSubtreeStream(adaptor,"rule queryRule");
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:625:2: ( queryRule -> ^( QUERY[\"query\"] queryRule ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:625:4: queryRule
            {
            	PushFollow(FOLLOW_queryRule_in_innerSubQuery3438);
            	queryRule287 = queryRule();
            	state.followingStackPointer--;

            	stream_queryRule.Add(queryRule287.Tree);


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
            	// 626:2: -> ^( QUERY[\"query\"] queryRule )
            	{
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:626:5: ^( QUERY[\"query\"] queryRule )
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:629:1: constant : ( NUM_INT | NUM_FLOAT | NUM_LONG | NUM_DOUBLE | NUM_DECIMAL | QUOTED_String | NULL | TRUE | FALSE | EMPTY );
    public HqlParser.constant_return constant() // throws RecognitionException [1]
    {   
        HqlParser.constant_return retval = new HqlParser.constant_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken set288 = null;

        IASTNode set288_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:630:2: ( NUM_INT | NUM_FLOAT | NUM_LONG | NUM_DOUBLE | NUM_DECIMAL | QUOTED_String | NULL | TRUE | FALSE | EMPTY )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	set288 = (IToken)input.LT(1);
            	if ( input.LA(1) == FALSE || input.LA(1) == NULL || input.LA(1) == TRUE || input.LA(1) == EMPTY || (input.LA(1) >= NUM_INT && input.LA(1) <= NUM_LONG) || input.LA(1) == QUOTED_String ) 
            	{
            	    input.Consume();
            	    adaptor.AddChild(root_0, (IASTNode)adaptor.Create(set288));
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:648:1: path : identifier ( DOT identifier )* ;
    public HqlParser.path_return path() // throws RecognitionException [1]
    {   
        HqlParser.path_return retval = new HqlParser.path_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken DOT290 = null;
        HqlParser.identifier_return identifier289 = default(HqlParser.identifier_return);

        HqlParser.identifier_return identifier291 = default(HqlParser.identifier_return);


        IASTNode DOT290_tree=null;


        // TODO - need to clean up DotIdent - suspect that DotIdent2 supersedes the other one, but need to do the analysis
        //HandleDotIdent2();

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:653:2: ( identifier ( DOT identifier )* )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:653:4: identifier ( DOT identifier )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_identifier_in_path3526);
            	identifier289 = identifier();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, identifier289.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:653:15: ( DOT identifier )*
            	do 
            	{
            	    int alt102 = 2;
            	    int LA102_0 = input.LA(1);

            	    if ( (LA102_0 == DOT) )
            	    {
            	        alt102 = 1;
            	    }


            	    switch (alt102) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:653:17: DOT identifier
            			    {
            			    	DOT290=(IToken)Match(input,DOT,FOLLOW_DOT_in_path3530); 
            			    		DOT290_tree = (IASTNode)adaptor.Create(DOT290);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(DOT290_tree, root_0);

            			    	 WeakKeywords(); 
            			    	PushFollow(FOLLOW_identifier_in_path3535);
            			    	identifier291 = identifier();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, identifier291.Tree);

            			    }
            			    break;

            			default:
            			    goto loop102;
            	    }
            	} while (true);

            	loop102:
            		;	// Stops C# compiler whining that label 'loop102' has no statements


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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:658:1: identifier : IDENT ;
    public HqlParser.identifier_return identifier() // throws RecognitionException [1]
    {   
        HqlParser.identifier_return retval = new HqlParser.identifier_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken IDENT292 = null;

        IASTNode IDENT292_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:659:2: ( IDENT )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:659:4: IDENT
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	IDENT292=(IToken)Match(input,IDENT,FOLLOW_IDENT_in_identifier3551); 
            		IDENT292_tree = (IASTNode)adaptor.Create(IDENT292);
            		adaptor.AddChild(root_0, IDENT292_tree);


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

 

    public static readonly BitSet FOLLOW_updateStatement_in_statement599 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_deleteStatement_in_statement603 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_selectStatement_in_statement607 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_insertStatement_in_statement611 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_UPDATE_in_updateStatement624 = new BitSet(new ulong[]{0x0010000000400000UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_VERSIONED_in_updateStatement628 = new BitSet(new ulong[]{0x0010000000400000UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_optionalFromTokenFromClause_in_updateStatement634 = new BitSet(new ulong[]{0x0000400000000000UL});
    public static readonly BitSet FOLLOW_setClause_in_updateStatement638 = new BitSet(new ulong[]{0x0020000000000002UL});
    public static readonly BitSet FOLLOW_whereClause_in_updateStatement643 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_SET_in_setClause657 = new BitSet(new ulong[]{0x0010000000400000UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_assignment_in_setClause660 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000800000000UL});
    public static readonly BitSet FOLLOW_COMMA_in_setClause663 = new BitSet(new ulong[]{0x0010000000400000UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_assignment_in_setClause666 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000800000000UL});
    public static readonly BitSet FOLLOW_stateField_in_assignment680 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000001000000000UL});
    public static readonly BitSet FOLLOW_EQ_in_assignment682 = new BitSet(new ulong[]{0x80938098085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_newValue_in_assignment685 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_path_in_stateField698 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_concatenation_in_newValue711 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_DELETE_in_deleteStatement722 = new BitSet(new ulong[]{0x0010000000400000UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_optionalFromTokenFromClause_in_deleteStatement728 = new BitSet(new ulong[]{0x0020000000000002UL});
    public static readonly BitSet FOLLOW_whereClause_in_deleteStatement734 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_optionalFromTokenFromClause2_in_optionalFromTokenFromClause749 = new BitSet(new ulong[]{0x0010000000400000UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_path_in_optionalFromTokenFromClause751 = new BitSet(new ulong[]{0x0010000000400082UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_asAlias_in_optionalFromTokenFromClause754 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_FROM_in_optionalFromTokenFromClause2785 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_queryRule_in_selectStatement799 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_INSERT_in_insertStatement828 = new BitSet(new ulong[]{0x0000000040000000UL});
    public static readonly BitSet FOLLOW_intoClause_in_insertStatement831 = new BitSet(new ulong[]{0x0020220001400000UL});
    public static readonly BitSet FOLLOW_selectStatement_in_insertStatement833 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_INTO_in_intoClause844 = new BitSet(new ulong[]{0x0010000000400000UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_path_in_intoClause847 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_insertablePropertySpec_in_intoClause851 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_OPEN_in_insertablePropertySpec862 = new BitSet(new ulong[]{0x80938098085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_primaryExpression_in_insertablePropertySpec864 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000004800000000UL});
    public static readonly BitSet FOLLOW_COMMA_in_insertablePropertySpec868 = new BitSet(new ulong[]{0x80938098085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_primaryExpression_in_insertablePropertySpec870 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000004800000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_insertablePropertySpec875 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_selectFrom_in_queryRule901 = new BitSet(new ulong[]{0x0020020001000002UL});
    public static readonly BitSet FOLLOW_whereClause_in_queryRule906 = new BitSet(new ulong[]{0x0000020001000002UL});
    public static readonly BitSet FOLLOW_groupByClause_in_queryRule913 = new BitSet(new ulong[]{0x0000020000000002UL});
    public static readonly BitSet FOLLOW_orderByClause_in_queryRule920 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_selectClause_in_selectFrom938 = new BitSet(new ulong[]{0x0000000000400002UL});
    public static readonly BitSet FOLLOW_fromClause_in_selectFrom945 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_SELECT_in_selectClause994 = new BitSet(new ulong[]{0x809380F8085B1230UL,0x0F0C4023E0000004UL});
    public static readonly BitSet FOLLOW_DISTINCT_in_selectClause1006 = new BitSet(new ulong[]{0x809380F8085B1230UL,0x0F0C4023E0000004UL});
    public static readonly BitSet FOLLOW_selectedPropertiesList_in_selectClause1012 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_newExpression_in_selectClause1016 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_selectObject_in_selectClause1020 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_NEW_in_newExpression1034 = new BitSet(new ulong[]{0x0010000000400000UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_path_in_newExpression1036 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_newExpression1041 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_selectedPropertiesList_in_newExpression1043 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000004000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_newExpression1045 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_OBJECT_in_selectObject1071 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_selectObject1074 = new BitSet(new ulong[]{0x0010000000400000UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_identifier_in_selectObject1077 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000004000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_selectObject1079 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_FROM_in_fromClause1097 = new BitSet(new ulong[]{0x0010000004420080UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_fromRange_in_fromClause1102 = new BitSet(new ulong[]{0x0000100310800002UL,0x0000000800000000UL});
    public static readonly BitSet FOLLOW_fromJoin_in_fromClause1106 = new BitSet(new ulong[]{0x0000100310800002UL,0x0000000800000000UL});
    public static readonly BitSet FOLLOW_COMMA_in_fromClause1110 = new BitSet(new ulong[]{0x0010000004420080UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_fromRange_in_fromClause1115 = new BitSet(new ulong[]{0x0000100310800002UL,0x0000000800000000UL});
    public static readonly BitSet FOLLOW_set_in_fromJoin1133 = new BitSet(new ulong[]{0x0000040100000000UL});
    public static readonly BitSet FOLLOW_OUTER_in_fromJoin1144 = new BitSet(new ulong[]{0x0000000100000000UL});
    public static readonly BitSet FOLLOW_FULL_in_fromJoin1152 = new BitSet(new ulong[]{0x0000000100000000UL});
    public static readonly BitSet FOLLOW_INNER_in_fromJoin1156 = new BitSet(new ulong[]{0x0000000100000000UL});
    public static readonly BitSet FOLLOW_JOIN_in_fromJoin1161 = new BitSet(new ulong[]{0x0010000000600000UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_FETCH_in_fromJoin1165 = new BitSet(new ulong[]{0x0010000000400000UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_path_in_fromJoin1169 = new BitSet(new ulong[]{0x2010000000600082UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_asAlias_in_fromJoin1172 = new BitSet(new ulong[]{0x2000000000200002UL});
    public static readonly BitSet FOLLOW_propertyFetch_in_fromJoin1177 = new BitSet(new ulong[]{0x2000000000000002UL});
    public static readonly BitSet FOLLOW_withClause_in_fromJoin1182 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_set_in_fromJoin1193 = new BitSet(new ulong[]{0x0000040100000000UL});
    public static readonly BitSet FOLLOW_OUTER_in_fromJoin1204 = new BitSet(new ulong[]{0x0000000100000000UL});
    public static readonly BitSet FOLLOW_FULL_in_fromJoin1212 = new BitSet(new ulong[]{0x0000000100000000UL});
    public static readonly BitSet FOLLOW_INNER_in_fromJoin1216 = new BitSet(new ulong[]{0x0000000100000000UL});
    public static readonly BitSet FOLLOW_JOIN_in_fromJoin1221 = new BitSet(new ulong[]{0x0000000000220000UL});
    public static readonly BitSet FOLLOW_FETCH_in_fromJoin1225 = new BitSet(new ulong[]{0x0000000000020000UL});
    public static readonly BitSet FOLLOW_ELEMENTS_in_fromJoin1229 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_fromJoin1232 = new BitSet(new ulong[]{0x0010000000400000UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_path_in_fromJoin1235 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000004000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_fromJoin1237 = new BitSet(new ulong[]{0x2010000000600082UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_asAlias_in_fromJoin1241 = new BitSet(new ulong[]{0x2000000000200002UL});
    public static readonly BitSet FOLLOW_propertyFetch_in_fromJoin1246 = new BitSet(new ulong[]{0x2000000000000002UL});
    public static readonly BitSet FOLLOW_withClause_in_fromJoin1251 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WITH_in_withClause1264 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_logicalExpression_in_withClause1267 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_fromClassOrOuterQueryPath_in_fromRange1278 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_inClassDeclaration_in_fromRange1283 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_inCollectionDeclaration_in_fromRange1288 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_inCollectionElementsDeclaration_in_fromRange1293 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_path_in_fromClassOrOuterQueryPath1305 = new BitSet(new ulong[]{0x0010000000600082UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_asAlias_in_fromClassOrOuterQueryPath1310 = new BitSet(new ulong[]{0x0000000000200002UL});
    public static readonly BitSet FOLLOW_propertyFetch_in_fromClassOrOuterQueryPath1315 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_alias_in_inClassDeclaration1345 = new BitSet(new ulong[]{0x0000000004000000UL});
    public static readonly BitSet FOLLOW_IN_in_inClassDeclaration1347 = new BitSet(new ulong[]{0x0010000000400800UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_CLASS_in_inClassDeclaration1349 = new BitSet(new ulong[]{0x0010000000400000UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_path_in_inClassDeclaration1352 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_IN_in_inCollectionDeclaration1380 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_inCollectionDeclaration1382 = new BitSet(new ulong[]{0x0010000000400000UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_path_in_inCollectionDeclaration1384 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000004000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_inCollectionDeclaration1386 = new BitSet(new ulong[]{0x0010000000400080UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_alias_in_inCollectionDeclaration1388 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_alias_in_inCollectionElementsDeclaration1422 = new BitSet(new ulong[]{0x0000000004000000UL});
    public static readonly BitSet FOLLOW_IN_in_inCollectionElementsDeclaration1424 = new BitSet(new ulong[]{0x0000000000020000UL});
    public static readonly BitSet FOLLOW_ELEMENTS_in_inCollectionElementsDeclaration1426 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_inCollectionElementsDeclaration1428 = new BitSet(new ulong[]{0x0010000000400000UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_path_in_inCollectionElementsDeclaration1430 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000004000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_inCollectionElementsDeclaration1432 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ELEMENTS_in_inCollectionElementsDeclaration1454 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_inCollectionElementsDeclaration1456 = new BitSet(new ulong[]{0x0010000000400000UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_path_in_inCollectionElementsDeclaration1458 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000004000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_inCollectionElementsDeclaration1460 = new BitSet(new ulong[]{0x0000000000000080UL});
    public static readonly BitSet FOLLOW_AS_in_inCollectionElementsDeclaration1462 = new BitSet(new ulong[]{0x0010000000400080UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_alias_in_inCollectionElementsDeclaration1464 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_AS_in_asAlias1496 = new BitSet(new ulong[]{0x0010000000400080UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_alias_in_asAlias1501 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_identifier_in_alias1513 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_FETCH_in_propertyFetch1532 = new BitSet(new ulong[]{0x0000000000000010UL});
    public static readonly BitSet FOLLOW_ALL_in_propertyFetch1534 = new BitSet(new ulong[]{0x0000080000000000UL});
    public static readonly BitSet FOLLOW_PROPERTIES_in_propertyFetch1537 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_GROUP_in_groupByClause1549 = new BitSet(new ulong[]{0x0040000000000000UL});
    public static readonly BitSet FOLLOW_LITERAL_by_in_groupByClause1555 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_expression_in_groupByClause1558 = new BitSet(new ulong[]{0x0000000002000002UL,0x0000000800000000UL});
    public static readonly BitSet FOLLOW_COMMA_in_groupByClause1562 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_expression_in_groupByClause1565 = new BitSet(new ulong[]{0x0000000002000002UL,0x0000000800000000UL});
    public static readonly BitSet FOLLOW_havingClause_in_groupByClause1573 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ORDER_in_orderByClause1586 = new BitSet(new ulong[]{0x0040000000000000UL});
    public static readonly BitSet FOLLOW_LITERAL_by_in_orderByClause1589 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_orderElement_in_orderByClause1592 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000800000000UL});
    public static readonly BitSet FOLLOW_COMMA_in_orderByClause1596 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_orderElement_in_orderByClause1599 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000800000000UL});
    public static readonly BitSet FOLLOW_expression_in_orderElement1613 = new BitSet(new ulong[]{0x0000000000004102UL,0x0000000000000000UL,0x0000000000000018UL});
    public static readonly BitSet FOLLOW_ascendingOrDescending_in_orderElement1617 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ASCENDING_in_ascendingOrDescending1635 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_131_in_ascendingOrDescending1641 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_DESCENDING_in_ascendingOrDescending1661 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_132_in_ascendingOrDescending1667 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_HAVING_in_havingClause1688 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_logicalExpression_in_havingClause1691 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WHERE_in_whereClause1702 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_logicalExpression_in_whereClause1705 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_aliasedExpression_in_selectedPropertiesList1716 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000800000000UL});
    public static readonly BitSet FOLLOW_COMMA_in_selectedPropertiesList1720 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_aliasedExpression_in_selectedPropertiesList1723 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000800000000UL});
    public static readonly BitSet FOLLOW_expression_in_aliasedExpression1738 = new BitSet(new ulong[]{0x0000000000000082UL});
    public static readonly BitSet FOLLOW_AS_in_aliasedExpression1742 = new BitSet(new ulong[]{0x0010000000400000UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_identifier_in_aliasedExpression1745 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_expression_in_logicalExpression1784 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_logicalOrExpression_in_expression1796 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_logicalAndExpression_in_logicalOrExpression1808 = new BitSet(new ulong[]{0x0000010000000002UL});
    public static readonly BitSet FOLLOW_OR_in_logicalOrExpression1812 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_logicalAndExpression_in_logicalOrExpression1815 = new BitSet(new ulong[]{0x0000010000000002UL});
    public static readonly BitSet FOLLOW_negatedExpression_in_logicalAndExpression1830 = new BitSet(new ulong[]{0x0000000000000042UL});
    public static readonly BitSet FOLLOW_AND_in_logicalAndExpression1834 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_negatedExpression_in_logicalAndExpression1837 = new BitSet(new ulong[]{0x0000000000000042UL});
    public static readonly BitSet FOLLOW_NOT_in_negatedExpression1858 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_negatedExpression_in_negatedExpression1862 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_equalityExpression_in_negatedExpression1875 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_relationalExpression_in_equalityExpression1905 = new BitSet(new ulong[]{0x0000000080000002UL,0x0000019000000000UL});
    public static readonly BitSet FOLLOW_EQ_in_equalityExpression1913 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_IS_in_equalityExpression1922 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_NOT_in_equalityExpression1928 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_NE_in_equalityExpression1940 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_SQL_NE_in_equalityExpression1949 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_relationalExpression_in_equalityExpression1960 = new BitSet(new ulong[]{0x0000000080000002UL,0x0000019000000000UL});
    public static readonly BitSet FOLLOW_concatenation_in_relationalExpression1977 = new BitSet(new ulong[]{0x0000004404000402UL,0x00001E0000000002UL});
    public static readonly BitSet FOLLOW_LT_in_relationalExpression1989 = new BitSet(new ulong[]{0x80938098085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_GT_in_relationalExpression1994 = new BitSet(new ulong[]{0x80938098085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_LE_in_relationalExpression1999 = new BitSet(new ulong[]{0x80938098085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_GE_in_relationalExpression2004 = new BitSet(new ulong[]{0x80938098085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_bitwiseNotExpression_in_relationalExpression2009 = new BitSet(new ulong[]{0x0000000000000002UL,0x00001E0000000000UL});
    public static readonly BitSet FOLLOW_NOT_in_relationalExpression2026 = new BitSet(new ulong[]{0x0000000404000400UL,0x0000000000000002UL});
    public static readonly BitSet FOLLOW_IN_in_relationalExpression2047 = new BitSet(new ulong[]{0x0011001808421200UL,0x0800002000000000UL});
    public static readonly BitSet FOLLOW_inList_in_relationalExpression2056 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_BETWEEN_in_relationalExpression2067 = new BitSet(new ulong[]{0x80938098085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_betweenList_in_relationalExpression2076 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_LIKE_in_relationalExpression2088 = new BitSet(new ulong[]{0x80938098085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_concatenation_in_relationalExpression2097 = new BitSet(new ulong[]{0x0000000000040000UL});
    public static readonly BitSet FOLLOW_likeEscape_in_relationalExpression2099 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_MEMBER_in_relationalExpression2108 = new BitSet(new ulong[]{0x0010000000400000UL,0x0800000000000008UL});
    public static readonly BitSet FOLLOW_OF_in_relationalExpression2112 = new BitSet(new ulong[]{0x0010000000400000UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_path_in_relationalExpression2119 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ESCAPE_in_likeEscape2146 = new BitSet(new ulong[]{0x80938098085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_concatenation_in_likeEscape2149 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_compoundExpr_in_inList2162 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_concatenation_in_betweenList2183 = new BitSet(new ulong[]{0x0000000000000040UL});
    public static readonly BitSet FOLLOW_AND_in_betweenList2185 = new BitSet(new ulong[]{0x80938098085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_concatenation_in_betweenList2188 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_bitwiseNotExpression_in_concatenation2207 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000200000000000UL});
    public static readonly BitSet FOLLOW_CONCAT_in_concatenation2215 = new BitSet(new ulong[]{0x80938098085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_bitwiseNotExpression_in_concatenation2224 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000200000000000UL});
    public static readonly BitSet FOLLOW_CONCAT_in_concatenation2231 = new BitSet(new ulong[]{0x80938098085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_bitwiseNotExpression_in_concatenation2234 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000200000000000UL});
    public static readonly BitSet FOLLOW_BNOT_in_bitwiseNotExpression2258 = new BitSet(new ulong[]{0x80938098085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_bitwiseOrExpression_in_bitwiseNotExpression2261 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_bitwiseOrExpression_in_bitwiseNotExpression2267 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_bitwiseXOrExpression_in_bitwiseOrExpression2279 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000800000000000UL});
    public static readonly BitSet FOLLOW_BOR_in_bitwiseOrExpression2282 = new BitSet(new ulong[]{0x80938098085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_bitwiseXOrExpression_in_bitwiseOrExpression2285 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000800000000000UL});
    public static readonly BitSet FOLLOW_bitwiseAndExpression_in_bitwiseXOrExpression2299 = new BitSet(new ulong[]{0x0000000000000002UL,0x0001000000000000UL});
    public static readonly BitSet FOLLOW_BXOR_in_bitwiseXOrExpression2302 = new BitSet(new ulong[]{0x80938098085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_bitwiseAndExpression_in_bitwiseXOrExpression2305 = new BitSet(new ulong[]{0x0000000000000002UL,0x0001000000000000UL});
    public static readonly BitSet FOLLOW_additiveExpression_in_bitwiseAndExpression2319 = new BitSet(new ulong[]{0x0000000000000002UL,0x0002000000000000UL});
    public static readonly BitSet FOLLOW_BAND_in_bitwiseAndExpression2322 = new BitSet(new ulong[]{0x80938098085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_additiveExpression_in_bitwiseAndExpression2325 = new BitSet(new ulong[]{0x0000000000000002UL,0x0002000000000000UL});
    public static readonly BitSet FOLLOW_multiplyExpression_in_additiveExpression2339 = new BitSet(new ulong[]{0x0000000000000002UL,0x000C000000000000UL});
    public static readonly BitSet FOLLOW_PLUS_in_additiveExpression2345 = new BitSet(new ulong[]{0x80938098085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_MINUS_in_additiveExpression2350 = new BitSet(new ulong[]{0x80938098085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_multiplyExpression_in_additiveExpression2355 = new BitSet(new ulong[]{0x0000000000000002UL,0x000C000000000000UL});
    public static readonly BitSet FOLLOW_unaryExpression_in_multiplyExpression2370 = new BitSet(new ulong[]{0x0000000000000002UL,0x0030000000000000UL});
    public static readonly BitSet FOLLOW_STAR_in_multiplyExpression2376 = new BitSet(new ulong[]{0x80938098085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_DIV_in_multiplyExpression2381 = new BitSet(new ulong[]{0x80938098085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_unaryExpression_in_multiplyExpression2386 = new BitSet(new ulong[]{0x0000000000000002UL,0x0030000000000000UL});
    public static readonly BitSet FOLLOW_MINUS_in_unaryExpression2404 = new BitSet(new ulong[]{0x80938098085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_unaryExpression_in_unaryExpression2408 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_PLUS_in_unaryExpression2425 = new BitSet(new ulong[]{0x80938098085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_unaryExpression_in_unaryExpression2429 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_caseExpression_in_unaryExpression2446 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_quantifiedExpression_in_unaryExpression2460 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_atom_in_unaryExpression2475 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_CASE_in_caseExpression2494 = new BitSet(new ulong[]{0x0800000000000000UL});
    public static readonly BitSet FOLLOW_whenClause_in_caseExpression2497 = new BitSet(new ulong[]{0x0B00000000000000UL});
    public static readonly BitSet FOLLOW_elseClause_in_caseExpression2502 = new BitSet(new ulong[]{0x0100000000000000UL});
    public static readonly BitSet FOLLOW_END_in_caseExpression2506 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_CASE_in_caseExpression2525 = new BitSet(new ulong[]{0x80938098085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_unaryExpression_in_caseExpression2527 = new BitSet(new ulong[]{0x0800000000000000UL});
    public static readonly BitSet FOLLOW_altWhenClause_in_caseExpression2530 = new BitSet(new ulong[]{0x0B00000000000000UL});
    public static readonly BitSet FOLLOW_elseClause_in_caseExpression2535 = new BitSet(new ulong[]{0x0100000000000000UL});
    public static readonly BitSet FOLLOW_END_in_caseExpression2539 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WHEN_in_whenClause2568 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_logicalExpression_in_whenClause2571 = new BitSet(new ulong[]{0x0400000000000000UL});
    public static readonly BitSet FOLLOW_THEN_in_whenClause2573 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_expression_in_whenClause2576 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WHEN_in_altWhenClause2590 = new BitSet(new ulong[]{0x80938098085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_unaryExpression_in_altWhenClause2593 = new BitSet(new ulong[]{0x0400000000000000UL});
    public static readonly BitSet FOLLOW_THEN_in_altWhenClause2595 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_expression_in_altWhenClause2598 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ELSE_in_elseClause2612 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_expression_in_elseClause2615 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_SOME_in_quantifiedExpression2630 = new BitSet(new ulong[]{0x0011001808421200UL,0x0800002000000000UL});
    public static readonly BitSet FOLLOW_EXISTS_in_quantifiedExpression2635 = new BitSet(new ulong[]{0x0011001808421200UL,0x0800002000000000UL});
    public static readonly BitSet FOLLOW_ALL_in_quantifiedExpression2640 = new BitSet(new ulong[]{0x0011001808421200UL,0x0800002000000000UL});
    public static readonly BitSet FOLLOW_ANY_in_quantifiedExpression2645 = new BitSet(new ulong[]{0x0011001808421200UL,0x0800002000000000UL});
    public static readonly BitSet FOLLOW_identifier_in_quantifiedExpression2654 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_collectionExpr_in_quantifiedExpression2658 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_OPEN_in_quantifiedExpression2663 = new BitSet(new ulong[]{0x0020220001400000UL});
    public static readonly BitSet FOLLOW_subQuery_in_quantifiedExpression2668 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000004000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_quantifiedExpression2672 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_primaryExpression_in_atom2691 = new BitSet(new ulong[]{0x0000000000008002UL,0x0040000000000000UL});
    public static readonly BitSet FOLLOW_DOT_in_atom2700 = new BitSet(new ulong[]{0x0010000000400000UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_identifier_in_atom2703 = new BitSet(new ulong[]{0x0000000000008002UL,0x0040002000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_atom2731 = new BitSet(new ulong[]{0xC09380D8085A1230UL,0x0F0C4063E0000011UL});
    public static readonly BitSet FOLLOW_exprList_in_atom2736 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000004000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_atom2738 = new BitSet(new ulong[]{0x0000000000008002UL,0x0040000000000000UL});
    public static readonly BitSet FOLLOW_OPEN_BRACKET_in_atom2752 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_expression_in_atom2757 = new BitSet(new ulong[]{0x0000000000000000UL,0x0080000000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_BRACKET_in_atom2759 = new BitSet(new ulong[]{0x0000000000008002UL,0x0040000000000000UL});
    public static readonly BitSet FOLLOW_identPrimary_in_primaryExpression2779 = new BitSet(new ulong[]{0x0000000000008002UL});
    public static readonly BitSet FOLLOW_DOT_in_primaryExpression2792 = new BitSet(new ulong[]{0x0000000000000800UL});
    public static readonly BitSet FOLLOW_CLASS_in_primaryExpression2795 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_constant_in_primaryExpression2805 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_COLON_in_primaryExpression2812 = new BitSet(new ulong[]{0x0010000000400000UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_identifier_in_primaryExpression2815 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_OPEN_in_primaryExpression2824 = new BitSet(new ulong[]{0x80B3A2D8095A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_expressionOrVector_in_primaryExpression2828 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000004000000000UL});
    public static readonly BitSet FOLLOW_subQuery_in_primaryExpression2832 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000004000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_primaryExpression2835 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_PARAM_in_primaryExpression2843 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000020000000UL});
    public static readonly BitSet FOLLOW_NUM_INT_in_primaryExpression2847 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_expression_in_expressionOrVector2865 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000800000000UL});
    public static readonly BitSet FOLLOW_vectorExpr_in_expressionOrVector2871 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_COMMA_in_vectorExpr2910 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_expression_in_vectorExpr2913 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000800000000UL});
    public static readonly BitSet FOLLOW_COMMA_in_vectorExpr2916 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_expression_in_vectorExpr2919 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000800000000UL});
    public static readonly BitSet FOLLOW_identifier_in_identPrimary2935 = new BitSet(new ulong[]{0x0000000000008002UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_DOT_in_identPrimary2953 = new BitSet(new ulong[]{0x0010000000400000UL,0x0800000000000004UL});
    public static readonly BitSet FOLLOW_identifier_in_identPrimary2958 = new BitSet(new ulong[]{0x0000000000008002UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_OBJECT_in_identPrimary2964 = new BitSet(new ulong[]{0x0000000000008002UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_identPrimary2982 = new BitSet(new ulong[]{0xC09380D8085A1230UL,0x0F0C4063E0000011UL});
    public static readonly BitSet FOLLOW_exprList_in_identPrimary2987 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000004000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_identPrimary2989 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_aggregate_in_identPrimary3005 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_SUM_in_aggregate3026 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_AVG_in_aggregate3032 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_MAX_in_aggregate3038 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_MIN_in_aggregate3044 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_aggregate3048 = new BitSet(new ulong[]{0x80938098085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_additiveExpression_in_aggregate3050 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000004000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_aggregate3052 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_COUNT_in_aggregate3071 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_aggregate3073 = new BitSet(new ulong[]{0x0011001808431210UL,0x0810000000000000UL});
    public static readonly BitSet FOLLOW_STAR_in_aggregate3079 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000004000000000UL});
    public static readonly BitSet FOLLOW_aggregateDistinctAll_in_aggregate3085 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000004000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_aggregate3089 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_collectionExpr_in_aggregate3121 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_set_in_aggregateDistinctAll3134 = new BitSet(new ulong[]{0x0011001808421200UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_path_in_aggregateDistinctAll3147 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_collectionExpr_in_aggregateDistinctAll3151 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ELEMENTS_in_collectionExpr3170 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_INDICES_in_collectionExpr3175 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_collectionExpr3179 = new BitSet(new ulong[]{0x0010000000400000UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_path_in_collectionExpr3182 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000004000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_collectionExpr3184 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_collectionExpr_in_compoundExpr3239 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_path_in_compoundExpr3244 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_OPEN_in_compoundExpr3250 = new BitSet(new ulong[]{0x80B3A2D8095A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_subQuery_in_compoundExpr3255 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000004000000000UL});
    public static readonly BitSet FOLLOW_expression_in_compoundExpr3260 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000004800000000UL});
    public static readonly BitSet FOLLOW_COMMA_in_compoundExpr3263 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_expression_in_compoundExpr3266 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000004800000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_compoundExpr3273 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_TRAILING_in_exprList3292 = new BitSet(new ulong[]{0x809380D8085A1232UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_LEADING_in_exprList3305 = new BitSet(new ulong[]{0x809380D8085A1232UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_BOTH_in_exprList3318 = new BitSet(new ulong[]{0x809380D8085A1232UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_expression_in_exprList3342 = new BitSet(new ulong[]{0x0000000000400082UL,0x0000000800000000UL});
    public static readonly BitSet FOLLOW_COMMA_in_exprList3347 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_expression_in_exprList3350 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000800000000UL});
    public static readonly BitSet FOLLOW_FROM_in_exprList3365 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_expression_in_exprList3367 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_AS_in_exprList3379 = new BitSet(new ulong[]{0x0010000000400000UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_identifier_in_exprList3382 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_FROM_in_exprList3396 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x0F0C4023E0000000UL});
    public static readonly BitSet FOLLOW_expression_in_exprList3398 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_innerSubQuery_in_subQuery3418 = new BitSet(new ulong[]{0x0004000000000002UL});
    public static readonly BitSet FOLLOW_UNION_in_subQuery3421 = new BitSet(new ulong[]{0x0020220001400000UL});
    public static readonly BitSet FOLLOW_innerSubQuery_in_subQuery3424 = new BitSet(new ulong[]{0x0004000000000002UL});
    public static readonly BitSet FOLLOW_queryRule_in_innerSubQuery3438 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_set_in_constant0 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_identifier_in_path3526 = new BitSet(new ulong[]{0x0000000000008002UL});
    public static readonly BitSet FOLLOW_DOT_in_path3530 = new BitSet(new ulong[]{0x0010000000400000UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_identifier_in_path3535 = new BitSet(new ulong[]{0x0000000000008002UL});
    public static readonly BitSet FOLLOW_IDENT_in_identifier3551 = new BitSet(new ulong[]{0x0000000000000002UL});

}
}