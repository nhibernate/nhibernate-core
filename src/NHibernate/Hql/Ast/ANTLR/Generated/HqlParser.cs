// $ANTLR 3.1.2 /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g 2009-05-15 10:57:47

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
		"NUM_FLOAT", 
		"NUM_LONG", 
		"JAVA_CONSTANT", 
		"COMMA", 
		"EQ", 
		"OPEN", 
		"CLOSE", 
		"BNOT", 
		"BOR", 
		"BXOR", 
		"BAND", 
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
		"'descending'"
    };

    public const int COMMA = 98;
    public const int EXISTS = 19;
    public const int EXPR_LIST = 73;
    public const int FETCH = 21;
    public const int MINUS = 114;
    public const int AS = 7;
    public const int END = 56;
    public const int INTO = 30;
    public const int FALSE = 20;
    public const int ELEMENTS = 17;
    public const int THEN = 58;
    public const int ALIAS = 70;
    public const int BOR = 103;
    public const int ON = 60;
    public const int DOT = 15;
    public const int ORDER = 41;
    public const int AND = 6;
    public const int CONSTANT = 92;
    public const int RIGHT = 44;
    public const int METHOD_CALL = 79;
    public const int UNARY_MINUS = 88;
    public const int CONCAT = 112;
    public const int PROPERTIES = 43;
    public const int SELECT = 45;
    public const int LE = 110;
    public const int BETWEEN = 10;
    public const int NUM_INT = 93;
    public const int BOTH = 62;
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
    public const int LT = 108;
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
    public const int GE = 111;
    public const int CASE = 55;
    public const int OPEN_BRACKET = 117;
    public const int ELSE = 57;
    public const int OPEN = 100;
    public const int COUNT = 12;
    public const int NULL = 39;
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
    public const int BNOT = 102;
    public const int LEADING = 64;
    public const int CLOSE_BRACKET = 118;
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
    public const int PARAM = 120;
    public const int ID_LETTER = 124;
    public const int INDEX_OP = 76;
    public const int HEX_DIGIT = 129;
    public const int LEFT = 33;
    public const int TRAILING = 68;
    public const int JOIN = 32;
    public const int NOT_BETWEEN = 80;
    public const int BAND = 105;
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
    public const int SQL_NE = 107;
    public const int IN_LIST = 75;
    public const int WEIRD_IDENT = 91;
    public const int GT = 109;
    public const int NE = 106;
    public const int MIN = 36;
    public const int LIKE = 34;
    public const int WITH = 61;
    public const int IN = 26;
    public const int CONSTRUCTOR = 71;
    public const int CLASS = 11;
    public const int SOME = 47;
    public const int EXPONENT = 127;
    public const int ID_START_LETTER = 123;
    public const int EOF = -1;
    public const int CLOSE = 101;
    public const int AVG = 9;
    public const int STAR = 115;
    public const int BXOR = 104;
    public const int NOT = 38;
    public const int JAVA_CONSTANT = 97;

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:141:1: statement : ( updateStatement | deleteStatement | selectStatement | insertStatement ) ;
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
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:142:2: ( ( updateStatement | deleteStatement | selectStatement | insertStatement ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:142:4: ( updateStatement | deleteStatement | selectStatement | insertStatement )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:142:4: ( updateStatement | deleteStatement | selectStatement | insertStatement )
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
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:142:6: updateStatement
            	        {
            	        	PushFollow(FOLLOW_updateStatement_in_statement597);
            	        	updateStatement1 = updateStatement();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, updateStatement1.Tree);

            	        }
            	        break;
            	    case 2 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:142:24: deleteStatement
            	        {
            	        	PushFollow(FOLLOW_deleteStatement_in_statement601);
            	        	deleteStatement2 = deleteStatement();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, deleteStatement2.Tree);

            	        }
            	        break;
            	    case 3 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:142:42: selectStatement
            	        {
            	        	PushFollow(FOLLOW_selectStatement_in_statement605);
            	        	selectStatement3 = selectStatement();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, selectStatement3.Tree);

            	        }
            	        break;
            	    case 4 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:142:60: insertStatement
            	        {
            	        	PushFollow(FOLLOW_insertStatement_in_statement609);
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:145:1: updateStatement : UPDATE ( VERSIONED )? optionalFromTokenFromClause setClause ( whereClause )? ;
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
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:146:2: ( UPDATE ( VERSIONED )? optionalFromTokenFromClause setClause ( whereClause )? )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:146:4: UPDATE ( VERSIONED )? optionalFromTokenFromClause setClause ( whereClause )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	UPDATE5=(IToken)Match(input,UPDATE,FOLLOW_UPDATE_in_updateStatement622); 
            		UPDATE5_tree = (IASTNode)adaptor.Create(UPDATE5);
            		root_0 = (IASTNode)adaptor.BecomeRoot(UPDATE5_tree, root_0);

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:146:12: ( VERSIONED )?
            	int alt2 = 2;
            	int LA2_0 = input.LA(1);

            	if ( (LA2_0 == VERSIONED) )
            	{
            	    alt2 = 1;
            	}
            	switch (alt2) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:146:13: VERSIONED
            	        {
            	        	VERSIONED6=(IToken)Match(input,VERSIONED,FOLLOW_VERSIONED_in_updateStatement626); 
            	        		VERSIONED6_tree = (IASTNode)adaptor.Create(VERSIONED6);
            	        		adaptor.AddChild(root_0, VERSIONED6_tree);


            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_optionalFromTokenFromClause_in_updateStatement632);
            	optionalFromTokenFromClause7 = optionalFromTokenFromClause();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, optionalFromTokenFromClause7.Tree);
            	PushFollow(FOLLOW_setClause_in_updateStatement636);
            	setClause8 = setClause();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, setClause8.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:149:3: ( whereClause )?
            	int alt3 = 2;
            	int LA3_0 = input.LA(1);

            	if ( (LA3_0 == WHERE) )
            	{
            	    alt3 = 1;
            	}
            	switch (alt3) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:149:4: whereClause
            	        {
            	        	PushFollow(FOLLOW_whereClause_in_updateStatement641);
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:152:1: setClause : ( SET assignment ( COMMA assignment )* ) ;
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
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:153:2: ( ( SET assignment ( COMMA assignment )* ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:153:4: ( SET assignment ( COMMA assignment )* )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:153:4: ( SET assignment ( COMMA assignment )* )
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:153:5: SET assignment ( COMMA assignment )*
            	{
            		SET10=(IToken)Match(input,SET,FOLLOW_SET_in_setClause655); 
            			SET10_tree = (IASTNode)adaptor.Create(SET10);
            			root_0 = (IASTNode)adaptor.BecomeRoot(SET10_tree, root_0);

            		PushFollow(FOLLOW_assignment_in_setClause658);
            		assignment11 = assignment();
            		state.followingStackPointer--;

            		adaptor.AddChild(root_0, assignment11.Tree);
            		// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:153:21: ( COMMA assignment )*
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
            				    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:153:22: COMMA assignment
            				    {
            				    	COMMA12=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_setClause661); 
            				    	PushFollow(FOLLOW_assignment_in_setClause664);
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:156:1: assignment : stateField EQ newValue ;
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
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:157:2: ( stateField EQ newValue )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:157:4: stateField EQ newValue
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_stateField_in_assignment678);
            	stateField14 = stateField();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, stateField14.Tree);
            	EQ15=(IToken)Match(input,EQ,FOLLOW_EQ_in_assignment680); 
            		EQ15_tree = (IASTNode)adaptor.Create(EQ15);
            		root_0 = (IASTNode)adaptor.BecomeRoot(EQ15_tree, root_0);

            	PushFollow(FOLLOW_newValue_in_assignment683);
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:162:1: stateField : path ;
    public HqlParser.stateField_return stateField() // throws RecognitionException [1]
    {   
        HqlParser.stateField_return retval = new HqlParser.stateField_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.path_return path17 = default(HqlParser.path_return);



        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:163:2: ( path )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:163:4: path
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_path_in_stateField696);
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:168:1: newValue : concatenation ;
    public HqlParser.newValue_return newValue() // throws RecognitionException [1]
    {   
        HqlParser.newValue_return retval = new HqlParser.newValue_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.concatenation_return concatenation18 = default(HqlParser.concatenation_return);



        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:169:2: ( concatenation )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:169:4: concatenation
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_concatenation_in_newValue709);
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:172:1: deleteStatement : DELETE ( optionalFromTokenFromClause ) ( whereClause )? ;
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
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:173:2: ( DELETE ( optionalFromTokenFromClause ) ( whereClause )? )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:173:4: DELETE ( optionalFromTokenFromClause ) ( whereClause )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	DELETE19=(IToken)Match(input,DELETE,FOLLOW_DELETE_in_deleteStatement720); 
            		DELETE19_tree = (IASTNode)adaptor.Create(DELETE19);
            		root_0 = (IASTNode)adaptor.BecomeRoot(DELETE19_tree, root_0);

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:174:3: ( optionalFromTokenFromClause )
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:174:4: optionalFromTokenFromClause
            	{
            		PushFollow(FOLLOW_optionalFromTokenFromClause_in_deleteStatement726);
            		optionalFromTokenFromClause20 = optionalFromTokenFromClause();
            		state.followingStackPointer--;

            		adaptor.AddChild(root_0, optionalFromTokenFromClause20.Tree);

            	}

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:175:3: ( whereClause )?
            	int alt5 = 2;
            	int LA5_0 = input.LA(1);

            	if ( (LA5_0 == WHERE) )
            	{
            	    alt5 = 1;
            	}
            	switch (alt5) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:175:4: whereClause
            	        {
            	        	PushFollow(FOLLOW_whereClause_in_deleteStatement732);
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:180:1: optionalFromTokenFromClause : optionalFromTokenFromClause2 path ( asAlias )? -> ^( FROM ^( RANGE path ( asAlias )? ) ) ;
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
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:181:2: ( optionalFromTokenFromClause2 path ( asAlias )? -> ^( FROM ^( RANGE path ( asAlias )? ) ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:181:4: optionalFromTokenFromClause2 path ( asAlias )?
            {
            	PushFollow(FOLLOW_optionalFromTokenFromClause2_in_optionalFromTokenFromClause747);
            	optionalFromTokenFromClause222 = optionalFromTokenFromClause2();
            	state.followingStackPointer--;

            	stream_optionalFromTokenFromClause2.Add(optionalFromTokenFromClause222.Tree);
            	PushFollow(FOLLOW_path_in_optionalFromTokenFromClause749);
            	path23 = path();
            	state.followingStackPointer--;

            	stream_path.Add(path23.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:181:38: ( asAlias )?
            	int alt6 = 2;
            	int LA6_0 = input.LA(1);

            	if ( (LA6_0 == AS || LA6_0 == IDENT) )
            	{
            	    alt6 = 1;
            	}
            	switch (alt6) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:181:39: asAlias
            	        {
            	        	PushFollow(FOLLOW_asAlias_in_optionalFromTokenFromClause752);
            	        	asAlias24 = asAlias();
            	        	state.followingStackPointer--;

            	        	stream_asAlias.Add(asAlias24.Tree);

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
            	// 182:3: -> ^( FROM ^( RANGE path ( asAlias )? ) )
            	{
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:182:6: ^( FROM ^( RANGE path ( asAlias )? ) )
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(FROM, "FROM"), root_1);

            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:182:13: ^( RANGE path ( asAlias )? )
            	    {
            	    IASTNode root_2 = (IASTNode)adaptor.GetNilNode();
            	    root_2 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(RANGE, "RANGE"), root_2);

            	    adaptor.AddChild(root_2, stream_path.NextTree());
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:182:26: ( asAlias )?
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:185:1: optionalFromTokenFromClause2 : ( FROM )? ;
    public HqlParser.optionalFromTokenFromClause2_return optionalFromTokenFromClause2() // throws RecognitionException [1]
    {   
        HqlParser.optionalFromTokenFromClause2_return retval = new HqlParser.optionalFromTokenFromClause2_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken FROM25 = null;

        IASTNode FROM25_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:186:2: ( ( FROM )? )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:186:4: ( FROM )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:186:4: ( FROM )?
            	int alt7 = 2;
            	int LA7_0 = input.LA(1);

            	if ( (LA7_0 == FROM) )
            	{
            	    alt7 = 1;
            	}
            	switch (alt7) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:186:4: FROM
            	        {
            	        	FROM25=(IToken)Match(input,FROM,FOLLOW_FROM_in_optionalFromTokenFromClause2783); 
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:189:1: selectStatement : q= queryRule -> ^( QUERY[\"query\"] $q) ;
    public HqlParser.selectStatement_return selectStatement() // throws RecognitionException [1]
    {   
        HqlParser.selectStatement_return retval = new HqlParser.selectStatement_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.queryRule_return q = default(HqlParser.queryRule_return);


        RewriteRuleSubtreeStream stream_queryRule = new RewriteRuleSubtreeStream(adaptor,"rule queryRule");
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:190:2: (q= queryRule -> ^( QUERY[\"query\"] $q) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:190:4: q= queryRule
            {
            	PushFollow(FOLLOW_queryRule_in_selectStatement797);
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
            	// 191:2: -> ^( QUERY[\"query\"] $q)
            	{
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:191:5: ^( QUERY[\"query\"] $q)
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:194:1: insertStatement : INSERT intoClause selectStatement ;
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
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:198:2: ( INSERT intoClause selectStatement )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:198:4: INSERT intoClause selectStatement
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	INSERT26=(IToken)Match(input,INSERT,FOLLOW_INSERT_in_insertStatement826); 
            		INSERT26_tree = (IASTNode)adaptor.Create(INSERT26);
            		root_0 = (IASTNode)adaptor.BecomeRoot(INSERT26_tree, root_0);

            	PushFollow(FOLLOW_intoClause_in_insertStatement829);
            	intoClause27 = intoClause();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, intoClause27.Tree);
            	PushFollow(FOLLOW_selectStatement_in_insertStatement831);
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:201:1: intoClause : INTO path insertablePropertySpec ;
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
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:202:2: ( INTO path insertablePropertySpec )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:202:4: INTO path insertablePropertySpec
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	INTO29=(IToken)Match(input,INTO,FOLLOW_INTO_in_intoClause842); 
            		INTO29_tree = (IASTNode)adaptor.Create(INTO29);
            		root_0 = (IASTNode)adaptor.BecomeRoot(INTO29_tree, root_0);

            	PushFollow(FOLLOW_path_in_intoClause845);
            	path30 = path();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, path30.Tree);
            	 WeakKeywords(); 
            	PushFollow(FOLLOW_insertablePropertySpec_in_intoClause849);
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:205:1: insertablePropertySpec : OPEN primaryExpression ( COMMA primaryExpression )* CLOSE -> ^( RANGE[\"column-spec\"] ( primaryExpression )* ) ;
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
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:206:2: ( OPEN primaryExpression ( COMMA primaryExpression )* CLOSE -> ^( RANGE[\"column-spec\"] ( primaryExpression )* ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:206:4: OPEN primaryExpression ( COMMA primaryExpression )* CLOSE
            {
            	OPEN32=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_insertablePropertySpec860);  
            	stream_OPEN.Add(OPEN32);

            	PushFollow(FOLLOW_primaryExpression_in_insertablePropertySpec862);
            	primaryExpression33 = primaryExpression();
            	state.followingStackPointer--;

            	stream_primaryExpression.Add(primaryExpression33.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:206:27: ( COMMA primaryExpression )*
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
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:206:29: COMMA primaryExpression
            			    {
            			    	COMMA34=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_insertablePropertySpec866);  
            			    	stream_COMMA.Add(COMMA34);

            			    	PushFollow(FOLLOW_primaryExpression_in_insertablePropertySpec868);
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

            	CLOSE36=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_insertablePropertySpec873);  
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
            	// 207:3: -> ^( RANGE[\"column-spec\"] ( primaryExpression )* )
            	{
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:207:6: ^( RANGE[\"column-spec\"] ( primaryExpression )* )
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(RANGE, "column-spec"), root_1);

            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:207:29: ( primaryExpression )*
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

    public class union_return : ParserRuleReturnScope
    {
        private IASTNode tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (IASTNode) value; }
        }
    };

    // $ANTLR start "union"
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:210:1: union : queryRule ( UNION queryRule )* ;
    public HqlParser.union_return union() // throws RecognitionException [1]
    {   
        HqlParser.union_return retval = new HqlParser.union_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken UNION38 = null;
        HqlParser.queryRule_return queryRule37 = default(HqlParser.queryRule_return);

        HqlParser.queryRule_return queryRule39 = default(HqlParser.queryRule_return);


        IASTNode UNION38_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:211:2: ( queryRule ( UNION queryRule )* )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:211:4: queryRule ( UNION queryRule )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_queryRule_in_union896);
            	queryRule37 = queryRule();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, queryRule37.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:211:14: ( UNION queryRule )*
            	do 
            	{
            	    int alt9 = 2;
            	    int LA9_0 = input.LA(1);

            	    if ( (LA9_0 == UNION) )
            	    {
            	        alt9 = 1;
            	    }


            	    switch (alt9) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:211:15: UNION queryRule
            			    {
            			    	UNION38=(IToken)Match(input,UNION,FOLLOW_UNION_in_union899); 
            			    		UNION38_tree = (IASTNode)adaptor.Create(UNION38);
            			    		adaptor.AddChild(root_0, UNION38_tree);

            			    	PushFollow(FOLLOW_queryRule_in_union901);
            			    	queryRule39 = queryRule();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, queryRule39.Tree);

            			    }
            			    break;

            			default:
            			    goto loop9;
            	    }
            	} while (true);

            	loop9:
            		;	// Stops C# compiler whining that label 'loop9' has no statements


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
    // $ANTLR end "union"

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:217:1: queryRule : selectFrom ( whereClause )? ( groupByClause )? ( orderByClause )? ;
    public HqlParser.queryRule_return queryRule() // throws RecognitionException [1]
    {   
        HqlParser.queryRule_return retval = new HqlParser.queryRule_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.selectFrom_return selectFrom40 = default(HqlParser.selectFrom_return);

        HqlParser.whereClause_return whereClause41 = default(HqlParser.whereClause_return);

        HqlParser.groupByClause_return groupByClause42 = default(HqlParser.groupByClause_return);

        HqlParser.orderByClause_return orderByClause43 = default(HqlParser.orderByClause_return);



        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:218:2: ( selectFrom ( whereClause )? ( groupByClause )? ( orderByClause )? )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:218:4: selectFrom ( whereClause )? ( groupByClause )? ( orderByClause )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_selectFrom_in_queryRule917);
            	selectFrom40 = selectFrom();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, selectFrom40.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:219:3: ( whereClause )?
            	int alt10 = 2;
            	int LA10_0 = input.LA(1);

            	if ( (LA10_0 == WHERE) )
            	{
            	    alt10 = 1;
            	}
            	switch (alt10) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:219:4: whereClause
            	        {
            	        	PushFollow(FOLLOW_whereClause_in_queryRule922);
            	        	whereClause41 = whereClause();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, whereClause41.Tree);

            	        }
            	        break;

            	}

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:220:3: ( groupByClause )?
            	int alt11 = 2;
            	int LA11_0 = input.LA(1);

            	if ( (LA11_0 == GROUP) )
            	{
            	    alt11 = 1;
            	}
            	switch (alt11) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:220:4: groupByClause
            	        {
            	        	PushFollow(FOLLOW_groupByClause_in_queryRule929);
            	        	groupByClause42 = groupByClause();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, groupByClause42.Tree);

            	        }
            	        break;

            	}

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:221:3: ( orderByClause )?
            	int alt12 = 2;
            	int LA12_0 = input.LA(1);

            	if ( (LA12_0 == ORDER) )
            	{
            	    alt12 = 1;
            	}
            	switch (alt12) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:221:4: orderByClause
            	        {
            	        	PushFollow(FOLLOW_orderByClause_in_queryRule936);
            	        	orderByClause43 = orderByClause();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, orderByClause43.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:246:1: selectFrom : (s= selectClause )? (f= fromClause )? -> {$f.tree == null && filter}? ^( SELECT_FROM FROM[\"{filter-implied FROM}\"] ( selectClause )? ) -> ^( SELECT_FROM ( fromClause )? ( selectClause )? ) ;
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
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:247:2: ( (s= selectClause )? (f= fromClause )? -> {$f.tree == null && filter}? ^( SELECT_FROM FROM[\"{filter-implied FROM}\"] ( selectClause )? ) -> ^( SELECT_FROM ( fromClause )? ( selectClause )? ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:247:5: (s= selectClause )? (f= fromClause )?
            {
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:247:5: (s= selectClause )?
            	int alt13 = 2;
            	int LA13_0 = input.LA(1);

            	if ( (LA13_0 == SELECT) )
            	{
            	    alt13 = 1;
            	}
            	switch (alt13) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:247:6: s= selectClause
            	        {
            	        	PushFollow(FOLLOW_selectClause_in_selectFrom957);
            	        	s = selectClause();
            	        	state.followingStackPointer--;

            	        	stream_selectClause.Add(s.Tree);

            	        }
            	        break;

            	}

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:247:23: (f= fromClause )?
            	int alt14 = 2;
            	int LA14_0 = input.LA(1);

            	if ( (LA14_0 == FROM) )
            	{
            	    alt14 = 1;
            	}
            	switch (alt14) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:247:24: f= fromClause
            	        {
            	        	PushFollow(FOLLOW_fromClause_in_selectFrom964);
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
            	// 252:3: -> {$f.tree == null && filter}? ^( SELECT_FROM FROM[\"{filter-implied FROM}\"] ( selectClause )? )
            	if (((f != null) ? ((IASTNode)f.Tree) : null) == null && filter)
            	{
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:252:35: ^( SELECT_FROM FROM[\"{filter-implied FROM}\"] ( selectClause )? )
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(SELECT_FROM, "SELECT_FROM"), root_1);

            	    adaptor.AddChild(root_1, (IASTNode)adaptor.Create(FROM, "{filter-implied FROM}"));
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:252:79: ( selectClause )?
            	    if ( stream_selectClause.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_selectClause.NextTree());

            	    }
            	    stream_selectClause.Reset();

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}
            	else // 253:3: -> ^( SELECT_FROM ( fromClause )? ( selectClause )? )
            	{
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:253:6: ^( SELECT_FROM ( fromClause )? ( selectClause )? )
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(SELECT_FROM, "SELECT_FROM"), root_1);

            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:253:20: ( fromClause )?
            	    if ( stream_fromClause.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_fromClause.NextTree());

            	    }
            	    stream_fromClause.Reset();
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:253:32: ( selectClause )?
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:261:1: selectClause : SELECT ( DISTINCT )? ( selectedPropertiesList | newExpression | selectObject ) ;
    public HqlParser.selectClause_return selectClause() // throws RecognitionException [1]
    {   
        HqlParser.selectClause_return retval = new HqlParser.selectClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken SELECT44 = null;
        IToken DISTINCT45 = null;
        HqlParser.selectedPropertiesList_return selectedPropertiesList46 = default(HqlParser.selectedPropertiesList_return);

        HqlParser.newExpression_return newExpression47 = default(HqlParser.newExpression_return);

        HqlParser.selectObject_return selectObject48 = default(HqlParser.selectObject_return);


        IASTNode SELECT44_tree=null;
        IASTNode DISTINCT45_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:262:2: ( SELECT ( DISTINCT )? ( selectedPropertiesList | newExpression | selectObject ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:262:4: SELECT ( DISTINCT )? ( selectedPropertiesList | newExpression | selectObject )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	SELECT44=(IToken)Match(input,SELECT,FOLLOW_SELECT_in_selectClause1017); 
            		SELECT44_tree = (IASTNode)adaptor.Create(SELECT44);
            		root_0 = (IASTNode)adaptor.BecomeRoot(SELECT44_tree, root_0);

            	 WeakKeywords(); 
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:264:3: ( DISTINCT )?
            	int alt15 = 2;
            	int LA15_0 = input.LA(1);

            	if ( (LA15_0 == DISTINCT) )
            	{
            	    alt15 = 1;
            	}
            	switch (alt15) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:264:4: DISTINCT
            	        {
            	        	DISTINCT45=(IToken)Match(input,DISTINCT,FOLLOW_DISTINCT_in_selectClause1029); 
            	        		DISTINCT45_tree = (IASTNode)adaptor.Create(DISTINCT45);
            	        		adaptor.AddChild(root_0, DISTINCT45_tree);


            	        }
            	        break;

            	}

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:264:15: ( selectedPropertiesList | newExpression | selectObject )
            	int alt16 = 3;
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
            	    alt16 = 1;
            	    }
            	    break;
            	case NEW:
            		{
            	    alt16 = 2;
            	    }
            	    break;
            	case OBJECT:
            		{
            	    alt16 = 3;
            	    }
            	    break;
            		default:
            		    NoViableAltException nvae_d16s0 =
            		        new NoViableAltException("", 16, 0, input);

            		    throw nvae_d16s0;
            	}

            	switch (alt16) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:264:17: selectedPropertiesList
            	        {
            	        	PushFollow(FOLLOW_selectedPropertiesList_in_selectClause1035);
            	        	selectedPropertiesList46 = selectedPropertiesList();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, selectedPropertiesList46.Tree);

            	        }
            	        break;
            	    case 2 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:264:42: newExpression
            	        {
            	        	PushFollow(FOLLOW_newExpression_in_selectClause1039);
            	        	newExpression47 = newExpression();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, newExpression47.Tree);

            	        }
            	        break;
            	    case 3 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:264:58: selectObject
            	        {
            	        	PushFollow(FOLLOW_selectObject_in_selectClause1043);
            	        	selectObject48 = selectObject();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, selectObject48.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:267:1: newExpression : ( NEW path ) op= OPEN selectedPropertiesList CLOSE -> ^( CONSTRUCTOR[$op] path selectedPropertiesList ) ;
    public HqlParser.newExpression_return newExpression() // throws RecognitionException [1]
    {   
        HqlParser.newExpression_return retval = new HqlParser.newExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken op = null;
        IToken NEW49 = null;
        IToken CLOSE52 = null;
        HqlParser.path_return path50 = default(HqlParser.path_return);

        HqlParser.selectedPropertiesList_return selectedPropertiesList51 = default(HqlParser.selectedPropertiesList_return);


        IASTNode op_tree=null;
        IASTNode NEW49_tree=null;
        IASTNode CLOSE52_tree=null;
        RewriteRuleTokenStream stream_CLOSE = new RewriteRuleTokenStream(adaptor,"token CLOSE");
        RewriteRuleTokenStream stream_OPEN = new RewriteRuleTokenStream(adaptor,"token OPEN");
        RewriteRuleTokenStream stream_NEW = new RewriteRuleTokenStream(adaptor,"token NEW");
        RewriteRuleSubtreeStream stream_selectedPropertiesList = new RewriteRuleSubtreeStream(adaptor,"rule selectedPropertiesList");
        RewriteRuleSubtreeStream stream_path = new RewriteRuleSubtreeStream(adaptor,"rule path");
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:268:2: ( ( NEW path ) op= OPEN selectedPropertiesList CLOSE -> ^( CONSTRUCTOR[$op] path selectedPropertiesList ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:268:4: ( NEW path ) op= OPEN selectedPropertiesList CLOSE
            {
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:268:4: ( NEW path )
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:268:5: NEW path
            	{
            		NEW49=(IToken)Match(input,NEW,FOLLOW_NEW_in_newExpression1057);  
            		stream_NEW.Add(NEW49);

            		PushFollow(FOLLOW_path_in_newExpression1059);
            		path50 = path();
            		state.followingStackPointer--;

            		stream_path.Add(path50.Tree);

            	}

            	op=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_newExpression1064);  
            	stream_OPEN.Add(op);

            	PushFollow(FOLLOW_selectedPropertiesList_in_newExpression1066);
            	selectedPropertiesList51 = selectedPropertiesList();
            	state.followingStackPointer--;

            	stream_selectedPropertiesList.Add(selectedPropertiesList51.Tree);
            	CLOSE52=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_newExpression1068);  
            	stream_CLOSE.Add(CLOSE52);



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
            	// 269:3: -> ^( CONSTRUCTOR[$op] path selectedPropertiesList )
            	{
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:269:6: ^( CONSTRUCTOR[$op] path selectedPropertiesList )
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:272:1: selectObject : OBJECT OPEN identifier CLOSE ;
    public HqlParser.selectObject_return selectObject() // throws RecognitionException [1]
    {   
        HqlParser.selectObject_return retval = new HqlParser.selectObject_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken OBJECT53 = null;
        IToken OPEN54 = null;
        IToken CLOSE56 = null;
        HqlParser.identifier_return identifier55 = default(HqlParser.identifier_return);


        IASTNode OBJECT53_tree=null;
        IASTNode OPEN54_tree=null;
        IASTNode CLOSE56_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:273:4: ( OBJECT OPEN identifier CLOSE )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:273:6: OBJECT OPEN identifier CLOSE
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	OBJECT53=(IToken)Match(input,OBJECT,FOLLOW_OBJECT_in_selectObject1094); 
            		OBJECT53_tree = (IASTNode)adaptor.Create(OBJECT53);
            		root_0 = (IASTNode)adaptor.BecomeRoot(OBJECT53_tree, root_0);

            	OPEN54=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_selectObject1097); 
            	PushFollow(FOLLOW_identifier_in_selectObject1100);
            	identifier55 = identifier();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, identifier55.Tree);
            	CLOSE56=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_selectObject1102); 

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:283:1: fromClause : FROM fromRange ( fromJoin | COMMA fromRange )* ;
    public HqlParser.fromClause_return fromClause() // throws RecognitionException [1]
    {   
        HqlParser.fromClause_return retval = new HqlParser.fromClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken FROM57 = null;
        IToken COMMA60 = null;
        HqlParser.fromRange_return fromRange58 = default(HqlParser.fromRange_return);

        HqlParser.fromJoin_return fromJoin59 = default(HqlParser.fromJoin_return);

        HqlParser.fromRange_return fromRange61 = default(HqlParser.fromRange_return);


        IASTNode FROM57_tree=null;
        IASTNode COMMA60_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:284:2: ( FROM fromRange ( fromJoin | COMMA fromRange )* )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:284:4: FROM fromRange ( fromJoin | COMMA fromRange )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	FROM57=(IToken)Match(input,FROM,FOLLOW_FROM_in_fromClause1123); 
            		FROM57_tree = (IASTNode)adaptor.Create(FROM57);
            		root_0 = (IASTNode)adaptor.BecomeRoot(FROM57_tree, root_0);

            	 WeakKeywords(); 
            	PushFollow(FOLLOW_fromRange_in_fromClause1128);
            	fromRange58 = fromRange();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, fromRange58.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:284:40: ( fromJoin | COMMA fromRange )*
            	do 
            	{
            	    int alt17 = 3;
            	    int LA17_0 = input.LA(1);

            	    if ( (LA17_0 == FULL || LA17_0 == INNER || (LA17_0 >= JOIN && LA17_0 <= LEFT) || LA17_0 == RIGHT) )
            	    {
            	        alt17 = 1;
            	    }
            	    else if ( (LA17_0 == COMMA) )
            	    {
            	        alt17 = 2;
            	    }


            	    switch (alt17) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:284:42: fromJoin
            			    {
            			    	PushFollow(FOLLOW_fromJoin_in_fromClause1132);
            			    	fromJoin59 = fromJoin();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, fromJoin59.Tree);

            			    }
            			    break;
            			case 2 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:284:53: COMMA fromRange
            			    {
            			    	COMMA60=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_fromClause1136); 
            			    	 WeakKeywords(); 
            			    	PushFollow(FOLLOW_fromRange_in_fromClause1141);
            			    	fromRange61 = fromRange();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, fromRange61.Tree);

            			    }
            			    break;

            			default:
            			    goto loop17;
            	    }
            	} while (true);

            	loop17:
            		;	// Stops C# compiler whining that label 'loop17' has no statements


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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:290:1: fromJoin : ( ( ( ( LEFT | RIGHT ) ( OUTER )? ) | FULL | INNER )? JOIN ( FETCH )? path ( asAlias )? ( propertyFetch )? ( withClause )? | ( ( ( LEFT | RIGHT ) ( OUTER )? ) | FULL | INNER )? JOIN ( FETCH )? ELEMENTS OPEN path CLOSE ( asAlias )? ( propertyFetch )? ( withClause )? );
    public HqlParser.fromJoin_return fromJoin() // throws RecognitionException [1]
    {   
        HqlParser.fromJoin_return retval = new HqlParser.fromJoin_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken set62 = null;
        IToken OUTER63 = null;
        IToken FULL64 = null;
        IToken INNER65 = null;
        IToken JOIN66 = null;
        IToken FETCH67 = null;
        IToken set72 = null;
        IToken OUTER73 = null;
        IToken FULL74 = null;
        IToken INNER75 = null;
        IToken JOIN76 = null;
        IToken FETCH77 = null;
        IToken ELEMENTS78 = null;
        IToken OPEN79 = null;
        IToken CLOSE81 = null;
        HqlParser.path_return path68 = default(HqlParser.path_return);

        HqlParser.asAlias_return asAlias69 = default(HqlParser.asAlias_return);

        HqlParser.propertyFetch_return propertyFetch70 = default(HqlParser.propertyFetch_return);

        HqlParser.withClause_return withClause71 = default(HqlParser.withClause_return);

        HqlParser.path_return path80 = default(HqlParser.path_return);

        HqlParser.asAlias_return asAlias82 = default(HqlParser.asAlias_return);

        HqlParser.propertyFetch_return propertyFetch83 = default(HqlParser.propertyFetch_return);

        HqlParser.withClause_return withClause84 = default(HqlParser.withClause_return);


        IASTNode set62_tree=null;
        IASTNode OUTER63_tree=null;
        IASTNode FULL64_tree=null;
        IASTNode INNER65_tree=null;
        IASTNode JOIN66_tree=null;
        IASTNode FETCH67_tree=null;
        IASTNode set72_tree=null;
        IASTNode OUTER73_tree=null;
        IASTNode FULL74_tree=null;
        IASTNode INNER75_tree=null;
        IASTNode JOIN76_tree=null;
        IASTNode FETCH77_tree=null;
        IASTNode ELEMENTS78_tree=null;
        IASTNode OPEN79_tree=null;
        IASTNode CLOSE81_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:291:2: ( ( ( ( LEFT | RIGHT ) ( OUTER )? ) | FULL | INNER )? JOIN ( FETCH )? path ( asAlias )? ( propertyFetch )? ( withClause )? | ( ( ( LEFT | RIGHT ) ( OUTER )? ) | FULL | INNER )? JOIN ( FETCH )? ELEMENTS OPEN path CLOSE ( asAlias )? ( propertyFetch )? ( withClause )? )
            int alt30 = 2;
            switch ( input.LA(1) ) 
            {
            case LEFT:
            case RIGHT:
            	{
                int LA30_1 = input.LA(2);

                if ( (LA30_1 == OUTER) )
                {
                    int LA30_5 = input.LA(3);

                    if ( (LA30_5 == JOIN) )
                    {
                        switch ( input.LA(4) ) 
                        {
                        case FETCH:
                        	{
                            int LA30_6 = input.LA(5);

                            if ( (LA30_6 == IDENT) )
                            {
                                alt30 = 1;
                            }
                            else if ( (LA30_6 == ELEMENTS) )
                            {
                                alt30 = 2;
                            }
                            else 
                            {
                                NoViableAltException nvae_d30s6 =
                                    new NoViableAltException("", 30, 6, input);

                                throw nvae_d30s6;
                            }
                            }
                            break;
                        case IDENT:
                        	{
                            alt30 = 1;
                            }
                            break;
                        case ELEMENTS:
                        	{
                            alt30 = 2;
                            }
                            break;
                        	default:
                        	    NoViableAltException nvae_d30s4 =
                        	        new NoViableAltException("", 30, 4, input);

                        	    throw nvae_d30s4;
                        }

                    }
                    else 
                    {
                        NoViableAltException nvae_d30s5 =
                            new NoViableAltException("", 30, 5, input);

                        throw nvae_d30s5;
                    }
                }
                else if ( (LA30_1 == JOIN) )
                {
                    switch ( input.LA(3) ) 
                    {
                    case FETCH:
                    	{
                        int LA30_6 = input.LA(4);

                        if ( (LA30_6 == IDENT) )
                        {
                            alt30 = 1;
                        }
                        else if ( (LA30_6 == ELEMENTS) )
                        {
                            alt30 = 2;
                        }
                        else 
                        {
                            NoViableAltException nvae_d30s6 =
                                new NoViableAltException("", 30, 6, input);

                            throw nvae_d30s6;
                        }
                        }
                        break;
                    case IDENT:
                    	{
                        alt30 = 1;
                        }
                        break;
                    case ELEMENTS:
                    	{
                        alt30 = 2;
                        }
                        break;
                    	default:
                    	    NoViableAltException nvae_d30s4 =
                    	        new NoViableAltException("", 30, 4, input);

                    	    throw nvae_d30s4;
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
            case FULL:
            	{
                int LA30_2 = input.LA(2);

                if ( (LA30_2 == JOIN) )
                {
                    switch ( input.LA(3) ) 
                    {
                    case FETCH:
                    	{
                        int LA30_6 = input.LA(4);

                        if ( (LA30_6 == IDENT) )
                        {
                            alt30 = 1;
                        }
                        else if ( (LA30_6 == ELEMENTS) )
                        {
                            alt30 = 2;
                        }
                        else 
                        {
                            NoViableAltException nvae_d30s6 =
                                new NoViableAltException("", 30, 6, input);

                            throw nvae_d30s6;
                        }
                        }
                        break;
                    case IDENT:
                    	{
                        alt30 = 1;
                        }
                        break;
                    case ELEMENTS:
                    	{
                        alt30 = 2;
                        }
                        break;
                    	default:
                    	    NoViableAltException nvae_d30s4 =
                    	        new NoViableAltException("", 30, 4, input);

                    	    throw nvae_d30s4;
                    }

                }
                else 
                {
                    NoViableAltException nvae_d30s2 =
                        new NoViableAltException("", 30, 2, input);

                    throw nvae_d30s2;
                }
                }
                break;
            case INNER:
            	{
                int LA30_3 = input.LA(2);

                if ( (LA30_3 == JOIN) )
                {
                    switch ( input.LA(3) ) 
                    {
                    case FETCH:
                    	{
                        int LA30_6 = input.LA(4);

                        if ( (LA30_6 == IDENT) )
                        {
                            alt30 = 1;
                        }
                        else if ( (LA30_6 == ELEMENTS) )
                        {
                            alt30 = 2;
                        }
                        else 
                        {
                            NoViableAltException nvae_d30s6 =
                                new NoViableAltException("", 30, 6, input);

                            throw nvae_d30s6;
                        }
                        }
                        break;
                    case IDENT:
                    	{
                        alt30 = 1;
                        }
                        break;
                    case ELEMENTS:
                    	{
                        alt30 = 2;
                        }
                        break;
                    	default:
                    	    NoViableAltException nvae_d30s4 =
                    	        new NoViableAltException("", 30, 4, input);

                    	    throw nvae_d30s4;
                    }

                }
                else 
                {
                    NoViableAltException nvae_d30s3 =
                        new NoViableAltException("", 30, 3, input);

                    throw nvae_d30s3;
                }
                }
                break;
            case JOIN:
            	{
                switch ( input.LA(2) ) 
                {
                case FETCH:
                	{
                    int LA30_6 = input.LA(3);

                    if ( (LA30_6 == IDENT) )
                    {
                        alt30 = 1;
                    }
                    else if ( (LA30_6 == ELEMENTS) )
                    {
                        alt30 = 2;
                    }
                    else 
                    {
                        NoViableAltException nvae_d30s6 =
                            new NoViableAltException("", 30, 6, input);

                        throw nvae_d30s6;
                    }
                    }
                    break;
                case IDENT:
                	{
                    alt30 = 1;
                    }
                    break;
                case ELEMENTS:
                	{
                    alt30 = 2;
                    }
                    break;
                	default:
                	    NoViableAltException nvae_d30s4 =
                	        new NoViableAltException("", 30, 4, input);

                	    throw nvae_d30s4;
                }

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
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:291:4: ( ( ( LEFT | RIGHT ) ( OUTER )? ) | FULL | INNER )? JOIN ( FETCH )? path ( asAlias )? ( propertyFetch )? ( withClause )?
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:291:4: ( ( ( LEFT | RIGHT ) ( OUTER )? ) | FULL | INNER )?
                    	int alt19 = 4;
                    	switch ( input.LA(1) ) 
                    	{
                    	    case LEFT:
                    	    case RIGHT:
                    	    	{
                    	        alt19 = 1;
                    	        }
                    	        break;
                    	    case FULL:
                    	    	{
                    	        alt19 = 2;
                    	        }
                    	        break;
                    	    case INNER:
                    	    	{
                    	        alt19 = 3;
                    	        }
                    	        break;
                    	}

                    	switch (alt19) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:291:6: ( ( LEFT | RIGHT ) ( OUTER )? )
                    	        {
                    	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:291:6: ( ( LEFT | RIGHT ) ( OUTER )? )
                    	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:291:8: ( LEFT | RIGHT ) ( OUTER )?
                    	        	{
                    	        		set62 = (IToken)input.LT(1);
                    	        		if ( input.LA(1) == LEFT || input.LA(1) == RIGHT ) 
                    	        		{
                    	        		    input.Consume();
                    	        		    adaptor.AddChild(root_0, (IASTNode)adaptor.Create(set62));
                    	        		    state.errorRecovery = false;
                    	        		}
                    	        		else 
                    	        		{
                    	        		    MismatchedSetException mse = new MismatchedSetException(null,input);
                    	        		    throw mse;
                    	        		}

                    	        		// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:291:25: ( OUTER )?
                    	        		int alt18 = 2;
                    	        		int LA18_0 = input.LA(1);

                    	        		if ( (LA18_0 == OUTER) )
                    	        		{
                    	        		    alt18 = 1;
                    	        		}
                    	        		switch (alt18) 
                    	        		{
                    	        		    case 1 :
                    	        		        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:291:26: OUTER
                    	        		        {
                    	        		        	OUTER63=(IToken)Match(input,OUTER,FOLLOW_OUTER_in_fromJoin1173); 
                    	        		        		OUTER63_tree = (IASTNode)adaptor.Create(OUTER63);
                    	        		        		adaptor.AddChild(root_0, OUTER63_tree);


                    	        		        }
                    	        		        break;

                    	        		}


                    	        	}


                    	        }
                    	        break;
                    	    case 2 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:291:38: FULL
                    	        {
                    	        	FULL64=(IToken)Match(input,FULL,FOLLOW_FULL_in_fromJoin1181); 
                    	        		FULL64_tree = (IASTNode)adaptor.Create(FULL64);
                    	        		adaptor.AddChild(root_0, FULL64_tree);


                    	        }
                    	        break;
                    	    case 3 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:291:45: INNER
                    	        {
                    	        	INNER65=(IToken)Match(input,INNER,FOLLOW_INNER_in_fromJoin1185); 
                    	        		INNER65_tree = (IASTNode)adaptor.Create(INNER65);
                    	        		adaptor.AddChild(root_0, INNER65_tree);


                    	        }
                    	        break;

                    	}

                    	JOIN66=(IToken)Match(input,JOIN,FOLLOW_JOIN_in_fromJoin1190); 
                    		JOIN66_tree = (IASTNode)adaptor.Create(JOIN66);
                    		root_0 = (IASTNode)adaptor.BecomeRoot(JOIN66_tree, root_0);

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:291:60: ( FETCH )?
                    	int alt20 = 2;
                    	int LA20_0 = input.LA(1);

                    	if ( (LA20_0 == FETCH) )
                    	{
                    	    alt20 = 1;
                    	}
                    	switch (alt20) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:291:61: FETCH
                    	        {
                    	        	FETCH67=(IToken)Match(input,FETCH,FOLLOW_FETCH_in_fromJoin1194); 
                    	        		FETCH67_tree = (IASTNode)adaptor.Create(FETCH67);
                    	        		adaptor.AddChild(root_0, FETCH67_tree);


                    	        }
                    	        break;

                    	}

                    	PushFollow(FOLLOW_path_in_fromJoin1198);
                    	path68 = path();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, path68.Tree);
                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:291:74: ( asAlias )?
                    	int alt21 = 2;
                    	int LA21_0 = input.LA(1);

                    	if ( (LA21_0 == AS || LA21_0 == IDENT) )
                    	{
                    	    alt21 = 1;
                    	}
                    	switch (alt21) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:291:75: asAlias
                    	        {
                    	        	PushFollow(FOLLOW_asAlias_in_fromJoin1201);
                    	        	asAlias69 = asAlias();
                    	        	state.followingStackPointer--;

                    	        	adaptor.AddChild(root_0, asAlias69.Tree);

                    	        }
                    	        break;

                    	}

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:291:85: ( propertyFetch )?
                    	int alt22 = 2;
                    	int LA22_0 = input.LA(1);

                    	if ( (LA22_0 == FETCH) )
                    	{
                    	    alt22 = 1;
                    	}
                    	switch (alt22) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:291:86: propertyFetch
                    	        {
                    	        	PushFollow(FOLLOW_propertyFetch_in_fromJoin1206);
                    	        	propertyFetch70 = propertyFetch();
                    	        	state.followingStackPointer--;

                    	        	adaptor.AddChild(root_0, propertyFetch70.Tree);

                    	        }
                    	        break;

                    	}

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:291:102: ( withClause )?
                    	int alt23 = 2;
                    	int LA23_0 = input.LA(1);

                    	if ( (LA23_0 == WITH) )
                    	{
                    	    alt23 = 1;
                    	}
                    	switch (alt23) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:291:103: withClause
                    	        {
                    	        	PushFollow(FOLLOW_withClause_in_fromJoin1211);
                    	        	withClause71 = withClause();
                    	        	state.followingStackPointer--;

                    	        	adaptor.AddChild(root_0, withClause71.Tree);

                    	        }
                    	        break;

                    	}


                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:292:4: ( ( ( LEFT | RIGHT ) ( OUTER )? ) | FULL | INNER )? JOIN ( FETCH )? ELEMENTS OPEN path CLOSE ( asAlias )? ( propertyFetch )? ( withClause )?
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:292:4: ( ( ( LEFT | RIGHT ) ( OUTER )? ) | FULL | INNER )?
                    	int alt25 = 4;
                    	switch ( input.LA(1) ) 
                    	{
                    	    case LEFT:
                    	    case RIGHT:
                    	    	{
                    	        alt25 = 1;
                    	        }
                    	        break;
                    	    case FULL:
                    	    	{
                    	        alt25 = 2;
                    	        }
                    	        break;
                    	    case INNER:
                    	    	{
                    	        alt25 = 3;
                    	        }
                    	        break;
                    	}

                    	switch (alt25) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:292:6: ( ( LEFT | RIGHT ) ( OUTER )? )
                    	        {
                    	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:292:6: ( ( LEFT | RIGHT ) ( OUTER )? )
                    	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:292:8: ( LEFT | RIGHT ) ( OUTER )?
                    	        	{
                    	        		set72 = (IToken)input.LT(1);
                    	        		if ( input.LA(1) == LEFT || input.LA(1) == RIGHT ) 
                    	        		{
                    	        		    input.Consume();
                    	        		    adaptor.AddChild(root_0, (IASTNode)adaptor.Create(set72));
                    	        		    state.errorRecovery = false;
                    	        		}
                    	        		else 
                    	        		{
                    	        		    MismatchedSetException mse = new MismatchedSetException(null,input);
                    	        		    throw mse;
                    	        		}

                    	        		// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:292:25: ( OUTER )?
                    	        		int alt24 = 2;
                    	        		int LA24_0 = input.LA(1);

                    	        		if ( (LA24_0 == OUTER) )
                    	        		{
                    	        		    alt24 = 1;
                    	        		}
                    	        		switch (alt24) 
                    	        		{
                    	        		    case 1 :
                    	        		        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:292:26: OUTER
                    	        		        {
                    	        		        	OUTER73=(IToken)Match(input,OUTER,FOLLOW_OUTER_in_fromJoin1233); 
                    	        		        		OUTER73_tree = (IASTNode)adaptor.Create(OUTER73);
                    	        		        		adaptor.AddChild(root_0, OUTER73_tree);


                    	        		        }
                    	        		        break;

                    	        		}


                    	        	}


                    	        }
                    	        break;
                    	    case 2 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:292:38: FULL
                    	        {
                    	        	FULL74=(IToken)Match(input,FULL,FOLLOW_FULL_in_fromJoin1241); 
                    	        		FULL74_tree = (IASTNode)adaptor.Create(FULL74);
                    	        		adaptor.AddChild(root_0, FULL74_tree);


                    	        }
                    	        break;
                    	    case 3 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:292:45: INNER
                    	        {
                    	        	INNER75=(IToken)Match(input,INNER,FOLLOW_INNER_in_fromJoin1245); 
                    	        		INNER75_tree = (IASTNode)adaptor.Create(INNER75);
                    	        		adaptor.AddChild(root_0, INNER75_tree);


                    	        }
                    	        break;

                    	}

                    	JOIN76=(IToken)Match(input,JOIN,FOLLOW_JOIN_in_fromJoin1250); 
                    		JOIN76_tree = (IASTNode)adaptor.Create(JOIN76);
                    		root_0 = (IASTNode)adaptor.BecomeRoot(JOIN76_tree, root_0);

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:292:60: ( FETCH )?
                    	int alt26 = 2;
                    	int LA26_0 = input.LA(1);

                    	if ( (LA26_0 == FETCH) )
                    	{
                    	    alt26 = 1;
                    	}
                    	switch (alt26) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:292:61: FETCH
                    	        {
                    	        	FETCH77=(IToken)Match(input,FETCH,FOLLOW_FETCH_in_fromJoin1254); 
                    	        		FETCH77_tree = (IASTNode)adaptor.Create(FETCH77);
                    	        		adaptor.AddChild(root_0, FETCH77_tree);


                    	        }
                    	        break;

                    	}

                    	ELEMENTS78=(IToken)Match(input,ELEMENTS,FOLLOW_ELEMENTS_in_fromJoin1258); 
                    	OPEN79=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_fromJoin1261); 
                    	PushFollow(FOLLOW_path_in_fromJoin1264);
                    	path80 = path();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, path80.Tree);
                    	CLOSE81=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_fromJoin1266); 
                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:292:97: ( asAlias )?
                    	int alt27 = 2;
                    	int LA27_0 = input.LA(1);

                    	if ( (LA27_0 == AS || LA27_0 == IDENT) )
                    	{
                    	    alt27 = 1;
                    	}
                    	switch (alt27) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:292:98: asAlias
                    	        {
                    	        	PushFollow(FOLLOW_asAlias_in_fromJoin1270);
                    	        	asAlias82 = asAlias();
                    	        	state.followingStackPointer--;

                    	        	adaptor.AddChild(root_0, asAlias82.Tree);

                    	        }
                    	        break;

                    	}

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:292:108: ( propertyFetch )?
                    	int alt28 = 2;
                    	int LA28_0 = input.LA(1);

                    	if ( (LA28_0 == FETCH) )
                    	{
                    	    alt28 = 1;
                    	}
                    	switch (alt28) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:292:109: propertyFetch
                    	        {
                    	        	PushFollow(FOLLOW_propertyFetch_in_fromJoin1275);
                    	        	propertyFetch83 = propertyFetch();
                    	        	state.followingStackPointer--;

                    	        	adaptor.AddChild(root_0, propertyFetch83.Tree);

                    	        }
                    	        break;

                    	}

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:292:125: ( withClause )?
                    	int alt29 = 2;
                    	int LA29_0 = input.LA(1);

                    	if ( (LA29_0 == WITH) )
                    	{
                    	    alt29 = 1;
                    	}
                    	switch (alt29) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:292:126: withClause
                    	        {
                    	        	PushFollow(FOLLOW_withClause_in_fromJoin1280);
                    	        	withClause84 = withClause();
                    	        	state.followingStackPointer--;

                    	        	adaptor.AddChild(root_0, withClause84.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:295:1: withClause : WITH logicalExpression ;
    public HqlParser.withClause_return withClause() // throws RecognitionException [1]
    {   
        HqlParser.withClause_return retval = new HqlParser.withClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken WITH85 = null;
        HqlParser.logicalExpression_return logicalExpression86 = default(HqlParser.logicalExpression_return);


        IASTNode WITH85_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:296:2: ( WITH logicalExpression )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:296:4: WITH logicalExpression
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	WITH85=(IToken)Match(input,WITH,FOLLOW_WITH_in_withClause1293); 
            		WITH85_tree = (IASTNode)adaptor.Create(WITH85);
            		root_0 = (IASTNode)adaptor.BecomeRoot(WITH85_tree, root_0);

            	PushFollow(FOLLOW_logicalExpression_in_withClause1296);
            	logicalExpression86 = logicalExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, logicalExpression86.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:299:1: fromRange : ( fromClassOrOuterQueryPath | inClassDeclaration | inCollectionDeclaration | inCollectionElementsDeclaration );
    public HqlParser.fromRange_return fromRange() // throws RecognitionException [1]
    {   
        HqlParser.fromRange_return retval = new HqlParser.fromRange_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.fromClassOrOuterQueryPath_return fromClassOrOuterQueryPath87 = default(HqlParser.fromClassOrOuterQueryPath_return);

        HqlParser.inClassDeclaration_return inClassDeclaration88 = default(HqlParser.inClassDeclaration_return);

        HqlParser.inCollectionDeclaration_return inCollectionDeclaration89 = default(HqlParser.inCollectionDeclaration_return);

        HqlParser.inCollectionElementsDeclaration_return inCollectionElementsDeclaration90 = default(HqlParser.inCollectionElementsDeclaration_return);



        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:300:2: ( fromClassOrOuterQueryPath | inClassDeclaration | inCollectionDeclaration | inCollectionElementsDeclaration )
            int alt31 = 4;
            switch ( input.LA(1) ) 
            {
            case IDENT:
            	{
                int LA31_1 = input.LA(2);

                if ( (LA31_1 == IN) )
                {
                    int LA31_4 = input.LA(3);

                    if ( (LA31_4 == ELEMENTS) )
                    {
                        alt31 = 4;
                    }
                    else if ( (LA31_4 == CLASS || LA31_4 == IDENT) )
                    {
                        alt31 = 2;
                    }
                    else 
                    {
                        NoViableAltException nvae_d31s4 =
                            new NoViableAltException("", 31, 4, input);

                        throw nvae_d31s4;
                    }
                }
                else if ( (LA31_1 == EOF || LA31_1 == AS || LA31_1 == DOT || LA31_1 == FETCH || (LA31_1 >= FULL && LA31_1 <= GROUP) || LA31_1 == INNER || (LA31_1 >= JOIN && LA31_1 <= LEFT) || LA31_1 == ORDER || LA31_1 == RIGHT || LA31_1 == UNION || LA31_1 == WHERE || LA31_1 == COMMA || LA31_1 == CLOSE || LA31_1 == IDENT) )
                {
                    alt31 = 1;
                }
                else 
                {
                    NoViableAltException nvae_d31s1 =
                        new NoViableAltException("", 31, 1, input);

                    throw nvae_d31s1;
                }
                }
                break;
            case IN:
            	{
                alt31 = 3;
                }
                break;
            case ELEMENTS:
            	{
                alt31 = 4;
                }
                break;
            	default:
            	    NoViableAltException nvae_d31s0 =
            	        new NoViableAltException("", 31, 0, input);

            	    throw nvae_d31s0;
            }

            switch (alt31) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:300:4: fromClassOrOuterQueryPath
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_fromClassOrOuterQueryPath_in_fromRange1307);
                    	fromClassOrOuterQueryPath87 = fromClassOrOuterQueryPath();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, fromClassOrOuterQueryPath87.Tree);

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:301:4: inClassDeclaration
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_inClassDeclaration_in_fromRange1312);
                    	inClassDeclaration88 = inClassDeclaration();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, inClassDeclaration88.Tree);

                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:302:4: inCollectionDeclaration
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_inCollectionDeclaration_in_fromRange1317);
                    	inCollectionDeclaration89 = inCollectionDeclaration();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, inCollectionDeclaration89.Tree);

                    }
                    break;
                case 4 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:303:4: inCollectionElementsDeclaration
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_inCollectionElementsDeclaration_in_fromRange1322);
                    	inCollectionElementsDeclaration90 = inCollectionElementsDeclaration();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, inCollectionElementsDeclaration90.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:306:1: fromClassOrOuterQueryPath : path ( asAlias )? ( propertyFetch )? -> ^( RANGE path ( asAlias )? ( propertyFetch )? ) ;
    public HqlParser.fromClassOrOuterQueryPath_return fromClassOrOuterQueryPath() // throws RecognitionException [1]
    {   
        HqlParser.fromClassOrOuterQueryPath_return retval = new HqlParser.fromClassOrOuterQueryPath_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.path_return path91 = default(HqlParser.path_return);

        HqlParser.asAlias_return asAlias92 = default(HqlParser.asAlias_return);

        HqlParser.propertyFetch_return propertyFetch93 = default(HqlParser.propertyFetch_return);


        RewriteRuleSubtreeStream stream_path = new RewriteRuleSubtreeStream(adaptor,"rule path");
        RewriteRuleSubtreeStream stream_propertyFetch = new RewriteRuleSubtreeStream(adaptor,"rule propertyFetch");
        RewriteRuleSubtreeStream stream_asAlias = new RewriteRuleSubtreeStream(adaptor,"rule asAlias");
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:307:2: ( path ( asAlias )? ( propertyFetch )? -> ^( RANGE path ( asAlias )? ( propertyFetch )? ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:307:4: path ( asAlias )? ( propertyFetch )?
            {
            	PushFollow(FOLLOW_path_in_fromClassOrOuterQueryPath1334);
            	path91 = path();
            	state.followingStackPointer--;

            	stream_path.Add(path91.Tree);
            	 WeakKeywords(); 
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:307:29: ( asAlias )?
            	int alt32 = 2;
            	int LA32_0 = input.LA(1);

            	if ( (LA32_0 == AS || LA32_0 == IDENT) )
            	{
            	    alt32 = 1;
            	}
            	switch (alt32) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:307:30: asAlias
            	        {
            	        	PushFollow(FOLLOW_asAlias_in_fromClassOrOuterQueryPath1339);
            	        	asAlias92 = asAlias();
            	        	state.followingStackPointer--;

            	        	stream_asAlias.Add(asAlias92.Tree);

            	        }
            	        break;

            	}

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:307:40: ( propertyFetch )?
            	int alt33 = 2;
            	int LA33_0 = input.LA(1);

            	if ( (LA33_0 == FETCH) )
            	{
            	    alt33 = 1;
            	}
            	switch (alt33) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:307:41: propertyFetch
            	        {
            	        	PushFollow(FOLLOW_propertyFetch_in_fromClassOrOuterQueryPath1344);
            	        	propertyFetch93 = propertyFetch();
            	        	state.followingStackPointer--;

            	        	stream_propertyFetch.Add(propertyFetch93.Tree);

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
            	// 308:3: -> ^( RANGE path ( asAlias )? ( propertyFetch )? )
            	{
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:308:6: ^( RANGE path ( asAlias )? ( propertyFetch )? )
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(RANGE, "RANGE"), root_1);

            	    adaptor.AddChild(root_1, stream_path.NextTree());
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:308:19: ( asAlias )?
            	    if ( stream_asAlias.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_asAlias.NextTree());

            	    }
            	    stream_asAlias.Reset();
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:308:28: ( propertyFetch )?
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:311:1: inClassDeclaration : alias IN ( CLASS )? path -> ^( RANGE path alias ) ;
    public HqlParser.inClassDeclaration_return inClassDeclaration() // throws RecognitionException [1]
    {   
        HqlParser.inClassDeclaration_return retval = new HqlParser.inClassDeclaration_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken IN95 = null;
        IToken CLASS96 = null;
        HqlParser.alias_return alias94 = default(HqlParser.alias_return);

        HqlParser.path_return path97 = default(HqlParser.path_return);


        IASTNode IN95_tree=null;
        IASTNode CLASS96_tree=null;
        RewriteRuleTokenStream stream_CLASS = new RewriteRuleTokenStream(adaptor,"token CLASS");
        RewriteRuleTokenStream stream_IN = new RewriteRuleTokenStream(adaptor,"token IN");
        RewriteRuleSubtreeStream stream_path = new RewriteRuleSubtreeStream(adaptor,"rule path");
        RewriteRuleSubtreeStream stream_alias = new RewriteRuleSubtreeStream(adaptor,"rule alias");
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:312:2: ( alias IN ( CLASS )? path -> ^( RANGE path alias ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:312:4: alias IN ( CLASS )? path
            {
            	PushFollow(FOLLOW_alias_in_inClassDeclaration1374);
            	alias94 = alias();
            	state.followingStackPointer--;

            	stream_alias.Add(alias94.Tree);
            	IN95=(IToken)Match(input,IN,FOLLOW_IN_in_inClassDeclaration1376);  
            	stream_IN.Add(IN95);

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:312:13: ( CLASS )?
            	int alt34 = 2;
            	int LA34_0 = input.LA(1);

            	if ( (LA34_0 == CLASS) )
            	{
            	    alt34 = 1;
            	}
            	switch (alt34) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:312:13: CLASS
            	        {
            	        	CLASS96=(IToken)Match(input,CLASS,FOLLOW_CLASS_in_inClassDeclaration1378);  
            	        	stream_CLASS.Add(CLASS96);


            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_path_in_inClassDeclaration1381);
            	path97 = path();
            	state.followingStackPointer--;

            	stream_path.Add(path97.Tree);


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
            	// 313:3: -> ^( RANGE path alias )
            	{
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:313:6: ^( RANGE path alias )
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:316:1: inCollectionDeclaration : IN OPEN path CLOSE alias -> ^( JOIN[\"join\"] INNER[\"inner\"] path alias ) ;
    public HqlParser.inCollectionDeclaration_return inCollectionDeclaration() // throws RecognitionException [1]
    {   
        HqlParser.inCollectionDeclaration_return retval = new HqlParser.inCollectionDeclaration_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken IN98 = null;
        IToken OPEN99 = null;
        IToken CLOSE101 = null;
        HqlParser.path_return path100 = default(HqlParser.path_return);

        HqlParser.alias_return alias102 = default(HqlParser.alias_return);


        IASTNode IN98_tree=null;
        IASTNode OPEN99_tree=null;
        IASTNode CLOSE101_tree=null;
        RewriteRuleTokenStream stream_CLOSE = new RewriteRuleTokenStream(adaptor,"token CLOSE");
        RewriteRuleTokenStream stream_OPEN = new RewriteRuleTokenStream(adaptor,"token OPEN");
        RewriteRuleTokenStream stream_IN = new RewriteRuleTokenStream(adaptor,"token IN");
        RewriteRuleSubtreeStream stream_path = new RewriteRuleSubtreeStream(adaptor,"rule path");
        RewriteRuleSubtreeStream stream_alias = new RewriteRuleSubtreeStream(adaptor,"rule alias");
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:317:5: ( IN OPEN path CLOSE alias -> ^( JOIN[\"join\"] INNER[\"inner\"] path alias ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:317:7: IN OPEN path CLOSE alias
            {
            	IN98=(IToken)Match(input,IN,FOLLOW_IN_in_inCollectionDeclaration1409);  
            	stream_IN.Add(IN98);

            	OPEN99=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_inCollectionDeclaration1411);  
            	stream_OPEN.Add(OPEN99);

            	PushFollow(FOLLOW_path_in_inCollectionDeclaration1413);
            	path100 = path();
            	state.followingStackPointer--;

            	stream_path.Add(path100.Tree);
            	CLOSE101=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_inCollectionDeclaration1415);  
            	stream_CLOSE.Add(CLOSE101);

            	PushFollow(FOLLOW_alias_in_inCollectionDeclaration1417);
            	alias102 = alias();
            	state.followingStackPointer--;

            	stream_alias.Add(alias102.Tree);


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
            	// 318:6: -> ^( JOIN[\"join\"] INNER[\"inner\"] path alias )
            	{
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:318:9: ^( JOIN[\"join\"] INNER[\"inner\"] path alias )
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:321:1: inCollectionElementsDeclaration : ( alias IN ELEMENTS OPEN path CLOSE -> ^( JOIN[\"join\"] INNER[\"inner\"] path alias ) | ELEMENTS OPEN path CLOSE AS alias -> ^( JOIN[\"join\"] INNER[\"inner\"] path alias ) );
    public HqlParser.inCollectionElementsDeclaration_return inCollectionElementsDeclaration() // throws RecognitionException [1]
    {   
        HqlParser.inCollectionElementsDeclaration_return retval = new HqlParser.inCollectionElementsDeclaration_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken IN104 = null;
        IToken ELEMENTS105 = null;
        IToken OPEN106 = null;
        IToken CLOSE108 = null;
        IToken ELEMENTS109 = null;
        IToken OPEN110 = null;
        IToken CLOSE112 = null;
        IToken AS113 = null;
        HqlParser.alias_return alias103 = default(HqlParser.alias_return);

        HqlParser.path_return path107 = default(HqlParser.path_return);

        HqlParser.path_return path111 = default(HqlParser.path_return);

        HqlParser.alias_return alias114 = default(HqlParser.alias_return);


        IASTNode IN104_tree=null;
        IASTNode ELEMENTS105_tree=null;
        IASTNode OPEN106_tree=null;
        IASTNode CLOSE108_tree=null;
        IASTNode ELEMENTS109_tree=null;
        IASTNode OPEN110_tree=null;
        IASTNode CLOSE112_tree=null;
        IASTNode AS113_tree=null;
        RewriteRuleTokenStream stream_CLOSE = new RewriteRuleTokenStream(adaptor,"token CLOSE");
        RewriteRuleTokenStream stream_ELEMENTS = new RewriteRuleTokenStream(adaptor,"token ELEMENTS");
        RewriteRuleTokenStream stream_AS = new RewriteRuleTokenStream(adaptor,"token AS");
        RewriteRuleTokenStream stream_OPEN = new RewriteRuleTokenStream(adaptor,"token OPEN");
        RewriteRuleTokenStream stream_IN = new RewriteRuleTokenStream(adaptor,"token IN");
        RewriteRuleSubtreeStream stream_path = new RewriteRuleSubtreeStream(adaptor,"rule path");
        RewriteRuleSubtreeStream stream_alias = new RewriteRuleSubtreeStream(adaptor,"rule alias");
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:322:2: ( alias IN ELEMENTS OPEN path CLOSE -> ^( JOIN[\"join\"] INNER[\"inner\"] path alias ) | ELEMENTS OPEN path CLOSE AS alias -> ^( JOIN[\"join\"] INNER[\"inner\"] path alias ) )
            int alt35 = 2;
            int LA35_0 = input.LA(1);

            if ( (LA35_0 == IDENT) )
            {
                alt35 = 1;
            }
            else if ( (LA35_0 == ELEMENTS) )
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
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:322:4: alias IN ELEMENTS OPEN path CLOSE
                    {
                    	PushFollow(FOLLOW_alias_in_inCollectionElementsDeclaration1451);
                    	alias103 = alias();
                    	state.followingStackPointer--;

                    	stream_alias.Add(alias103.Tree);
                    	IN104=(IToken)Match(input,IN,FOLLOW_IN_in_inCollectionElementsDeclaration1453);  
                    	stream_IN.Add(IN104);

                    	ELEMENTS105=(IToken)Match(input,ELEMENTS,FOLLOW_ELEMENTS_in_inCollectionElementsDeclaration1455);  
                    	stream_ELEMENTS.Add(ELEMENTS105);

                    	OPEN106=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_inCollectionElementsDeclaration1457);  
                    	stream_OPEN.Add(OPEN106);

                    	PushFollow(FOLLOW_path_in_inCollectionElementsDeclaration1459);
                    	path107 = path();
                    	state.followingStackPointer--;

                    	stream_path.Add(path107.Tree);
                    	CLOSE108=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_inCollectionElementsDeclaration1461);  
                    	stream_CLOSE.Add(CLOSE108);



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
                    	// 323:3: -> ^( JOIN[\"join\"] INNER[\"inner\"] path alias )
                    	{
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:323:6: ^( JOIN[\"join\"] INNER[\"inner\"] path alias )
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
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:324:4: ELEMENTS OPEN path CLOSE AS alias
                    {
                    	ELEMENTS109=(IToken)Match(input,ELEMENTS,FOLLOW_ELEMENTS_in_inCollectionElementsDeclaration1483);  
                    	stream_ELEMENTS.Add(ELEMENTS109);

                    	OPEN110=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_inCollectionElementsDeclaration1485);  
                    	stream_OPEN.Add(OPEN110);

                    	PushFollow(FOLLOW_path_in_inCollectionElementsDeclaration1487);
                    	path111 = path();
                    	state.followingStackPointer--;

                    	stream_path.Add(path111.Tree);
                    	CLOSE112=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_inCollectionElementsDeclaration1489);  
                    	stream_CLOSE.Add(CLOSE112);

                    	AS113=(IToken)Match(input,AS,FOLLOW_AS_in_inCollectionElementsDeclaration1491);  
                    	stream_AS.Add(AS113);

                    	PushFollow(FOLLOW_alias_in_inCollectionElementsDeclaration1493);
                    	alias114 = alias();
                    	state.followingStackPointer--;

                    	stream_alias.Add(alias114.Tree);


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
                    	// 325:3: -> ^( JOIN[\"join\"] INNER[\"inner\"] path alias )
                    	{
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:325:6: ^( JOIN[\"join\"] INNER[\"inner\"] path alias )
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:329:1: asAlias : ( AS )? alias ;
    public HqlParser.asAlias_return asAlias() // throws RecognitionException [1]
    {   
        HqlParser.asAlias_return retval = new HqlParser.asAlias_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken AS115 = null;
        HqlParser.alias_return alias116 = default(HqlParser.alias_return);


        IASTNode AS115_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:330:2: ( ( AS )? alias )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:330:4: ( AS )? alias
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:330:4: ( AS )?
            	int alt36 = 2;
            	int LA36_0 = input.LA(1);

            	if ( (LA36_0 == AS) )
            	{
            	    alt36 = 1;
            	}
            	switch (alt36) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:330:5: AS
            	        {
            	        	AS115=(IToken)Match(input,AS,FOLLOW_AS_in_asAlias1525); 

            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_alias_in_asAlias1530);
            	alias116 = alias();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, alias116.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:332:1: alias : i= identifier -> ^( ALIAS[$i.start] ) ;
    public HqlParser.alias_return alias() // throws RecognitionException [1]
    {   
        HqlParser.alias_return retval = new HqlParser.alias_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.identifier_return i = default(HqlParser.identifier_return);


        RewriteRuleSubtreeStream stream_identifier = new RewriteRuleSubtreeStream(adaptor,"rule identifier");
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:333:2: (i= identifier -> ^( ALIAS[$i.start] ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:333:4: i= identifier
            {
            	PushFollow(FOLLOW_identifier_in_alias1542);
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
            	// 334:2: -> ^( ALIAS[$i.start] )
            	{
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:334:5: ^( ALIAS[$i.start] )
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:337:1: propertyFetch : FETCH ALL PROPERTIES ;
    public HqlParser.propertyFetch_return propertyFetch() // throws RecognitionException [1]
    {   
        HqlParser.propertyFetch_return retval = new HqlParser.propertyFetch_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken FETCH117 = null;
        IToken ALL118 = null;
        IToken PROPERTIES119 = null;

        IASTNode FETCH117_tree=null;
        IASTNode ALL118_tree=null;
        IASTNode PROPERTIES119_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:338:2: ( FETCH ALL PROPERTIES )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:338:4: FETCH ALL PROPERTIES
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	FETCH117=(IToken)Match(input,FETCH,FOLLOW_FETCH_in_propertyFetch1561); 
            		FETCH117_tree = (IASTNode)adaptor.Create(FETCH117);
            		adaptor.AddChild(root_0, FETCH117_tree);

            	ALL118=(IToken)Match(input,ALL,FOLLOW_ALL_in_propertyFetch1563); 
            	PROPERTIES119=(IToken)Match(input,PROPERTIES,FOLLOW_PROPERTIES_in_propertyFetch1566); 

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:344:1: groupByClause : GROUP 'by' expression ( COMMA expression )* ( havingClause )? ;
    public HqlParser.groupByClause_return groupByClause() // throws RecognitionException [1]
    {   
        HqlParser.groupByClause_return retval = new HqlParser.groupByClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken GROUP120 = null;
        IToken string_literal121 = null;
        IToken COMMA123 = null;
        HqlParser.expression_return expression122 = default(HqlParser.expression_return);

        HqlParser.expression_return expression124 = default(HqlParser.expression_return);

        HqlParser.havingClause_return havingClause125 = default(HqlParser.havingClause_return);


        IASTNode GROUP120_tree=null;
        IASTNode string_literal121_tree=null;
        IASTNode COMMA123_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:345:2: ( GROUP 'by' expression ( COMMA expression )* ( havingClause )? )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:345:4: GROUP 'by' expression ( COMMA expression )* ( havingClause )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	GROUP120=(IToken)Match(input,GROUP,FOLLOW_GROUP_in_groupByClause1581); 
            		GROUP120_tree = (IASTNode)adaptor.Create(GROUP120);
            		root_0 = (IASTNode)adaptor.BecomeRoot(GROUP120_tree, root_0);

            	string_literal121=(IToken)Match(input,LITERAL_by,FOLLOW_LITERAL_by_in_groupByClause1587); 
            	PushFollow(FOLLOW_expression_in_groupByClause1590);
            	expression122 = expression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, expression122.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:346:20: ( COMMA expression )*
            	do 
            	{
            	    int alt37 = 2;
            	    int LA37_0 = input.LA(1);

            	    if ( (LA37_0 == COMMA) )
            	    {
            	        alt37 = 1;
            	    }


            	    switch (alt37) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:346:22: COMMA expression
            			    {
            			    	COMMA123=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_groupByClause1594); 
            			    	PushFollow(FOLLOW_expression_in_groupByClause1597);
            			    	expression124 = expression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, expression124.Tree);

            			    }
            			    break;

            			default:
            			    goto loop37;
            	    }
            	} while (true);

            	loop37:
            		;	// Stops C# compiler whining that label 'loop37' has no statements

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:347:3: ( havingClause )?
            	int alt38 = 2;
            	int LA38_0 = input.LA(1);

            	if ( (LA38_0 == HAVING) )
            	{
            	    alt38 = 1;
            	}
            	switch (alt38) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:347:4: havingClause
            	        {
            	        	PushFollow(FOLLOW_havingClause_in_groupByClause1605);
            	        	havingClause125 = havingClause();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, havingClause125.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:353:1: orderByClause : ORDER 'by' orderElement ( COMMA orderElement )* ;
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
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:354:2: ( ORDER 'by' orderElement ( COMMA orderElement )* )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:354:4: ORDER 'by' orderElement ( COMMA orderElement )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	ORDER126=(IToken)Match(input,ORDER,FOLLOW_ORDER_in_orderByClause1621); 
            		ORDER126_tree = (IASTNode)adaptor.Create(ORDER126);
            		root_0 = (IASTNode)adaptor.BecomeRoot(ORDER126_tree, root_0);

            	string_literal127=(IToken)Match(input,LITERAL_by,FOLLOW_LITERAL_by_in_orderByClause1624); 
            	PushFollow(FOLLOW_orderElement_in_orderByClause1627);
            	orderElement128 = orderElement();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, orderElement128.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:354:30: ( COMMA orderElement )*
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
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:354:32: COMMA orderElement
            			    {
            			    	COMMA129=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_orderByClause1631); 
            			    	PushFollow(FOLLOW_orderElement_in_orderByClause1634);
            			    	orderElement130 = orderElement();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, orderElement130.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:357:1: orderElement : expression ( ascendingOrDescending )? ;
    public HqlParser.orderElement_return orderElement() // throws RecognitionException [1]
    {   
        HqlParser.orderElement_return retval = new HqlParser.orderElement_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.expression_return expression131 = default(HqlParser.expression_return);

        HqlParser.ascendingOrDescending_return ascendingOrDescending132 = default(HqlParser.ascendingOrDescending_return);



        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:358:2: ( expression ( ascendingOrDescending )? )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:358:4: expression ( ascendingOrDescending )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_expression_in_orderElement1648);
            	expression131 = expression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, expression131.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:358:15: ( ascendingOrDescending )?
            	int alt40 = 2;
            	int LA40_0 = input.LA(1);

            	if ( (LA40_0 == ASCENDING || LA40_0 == DESCENDING || (LA40_0 >= 130 && LA40_0 <= 131)) )
            	{
            	    alt40 = 1;
            	}
            	switch (alt40) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:358:17: ascendingOrDescending
            	        {
            	        	PushFollow(FOLLOW_ascendingOrDescending_in_orderElement1652);
            	        	ascendingOrDescending132 = ascendingOrDescending();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, ascendingOrDescending132.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:361:1: ascendingOrDescending : ( (a= 'asc' | a= 'ascending' ) -> ^( ASCENDING[$a.Text] ) | (d= 'desc' | d= 'descending' ) -> ^( DESCENDING[$d.Text] ) );
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
        RewriteRuleTokenStream stream_130 = new RewriteRuleTokenStream(adaptor,"token 130");
        RewriteRuleTokenStream stream_ASCENDING = new RewriteRuleTokenStream(adaptor,"token ASCENDING");
        RewriteRuleTokenStream stream_DESCENDING = new RewriteRuleTokenStream(adaptor,"token DESCENDING");

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:362:2: ( (a= 'asc' | a= 'ascending' ) -> ^( ASCENDING[$a.Text] ) | (d= 'desc' | d= 'descending' ) -> ^( DESCENDING[$d.Text] ) )
            int alt43 = 2;
            int LA43_0 = input.LA(1);

            if ( (LA43_0 == ASCENDING || LA43_0 == 130) )
            {
                alt43 = 1;
            }
            else if ( (LA43_0 == DESCENDING || LA43_0 == 131) )
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
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:362:4: (a= 'asc' | a= 'ascending' )
                    {
                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:362:4: (a= 'asc' | a= 'ascending' )
                    	int alt41 = 2;
                    	int LA41_0 = input.LA(1);

                    	if ( (LA41_0 == ASCENDING) )
                    	{
                    	    alt41 = 1;
                    	}
                    	else if ( (LA41_0 == 130) )
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
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:362:6: a= 'asc'
                    	        {
                    	        	a=(IToken)Match(input,ASCENDING,FOLLOW_ASCENDING_in_ascendingOrDescending1670);  
                    	        	stream_ASCENDING.Add(a);


                    	        }
                    	        break;
                    	    case 2 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:362:16: a= 'ascending'
                    	        {
                    	        	a=(IToken)Match(input,130,FOLLOW_130_in_ascendingOrDescending1676);  
                    	        	stream_130.Add(a);


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
                    	// 363:3: -> ^( ASCENDING[$a.Text] )
                    	{
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:363:6: ^( ASCENDING[$a.Text] )
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
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:364:4: (d= 'desc' | d= 'descending' )
                    {
                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:364:4: (d= 'desc' | d= 'descending' )
                    	int alt42 = 2;
                    	int LA42_0 = input.LA(1);

                    	if ( (LA42_0 == DESCENDING) )
                    	{
                    	    alt42 = 1;
                    	}
                    	else if ( (LA42_0 == 131) )
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
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:364:6: d= 'desc'
                    	        {
                    	        	d=(IToken)Match(input,DESCENDING,FOLLOW_DESCENDING_in_ascendingOrDescending1696);  
                    	        	stream_DESCENDING.Add(d);


                    	        }
                    	        break;
                    	    case 2 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:364:17: d= 'descending'
                    	        {
                    	        	d=(IToken)Match(input,131,FOLLOW_131_in_ascendingOrDescending1702);  
                    	        	stream_131.Add(d);


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
                    	// 365:3: -> ^( DESCENDING[$d.Text] )
                    	{
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:365:6: ^( DESCENDING[$d.Text] )
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:371:1: havingClause : HAVING logicalExpression ;
    public HqlParser.havingClause_return havingClause() // throws RecognitionException [1]
    {   
        HqlParser.havingClause_return retval = new HqlParser.havingClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken HAVING133 = null;
        HqlParser.logicalExpression_return logicalExpression134 = default(HqlParser.logicalExpression_return);


        IASTNode HAVING133_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:372:2: ( HAVING logicalExpression )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:372:4: HAVING logicalExpression
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	HAVING133=(IToken)Match(input,HAVING,FOLLOW_HAVING_in_havingClause1726); 
            		HAVING133_tree = (IASTNode)adaptor.Create(HAVING133);
            		root_0 = (IASTNode)adaptor.BecomeRoot(HAVING133_tree, root_0);

            	PushFollow(FOLLOW_logicalExpression_in_havingClause1729);
            	logicalExpression134 = logicalExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, logicalExpression134.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:378:1: whereClause : WHERE logicalExpression ;
    public HqlParser.whereClause_return whereClause() // throws RecognitionException [1]
    {   
        HqlParser.whereClause_return retval = new HqlParser.whereClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken WHERE135 = null;
        HqlParser.logicalExpression_return logicalExpression136 = default(HqlParser.logicalExpression_return);


        IASTNode WHERE135_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:379:2: ( WHERE logicalExpression )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:379:4: WHERE logicalExpression
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	WHERE135=(IToken)Match(input,WHERE,FOLLOW_WHERE_in_whereClause1743); 
            		WHERE135_tree = (IASTNode)adaptor.Create(WHERE135);
            		root_0 = (IASTNode)adaptor.BecomeRoot(WHERE135_tree, root_0);

            	PushFollow(FOLLOW_logicalExpression_in_whereClause1746);
            	logicalExpression136 = logicalExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, logicalExpression136.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:385:1: selectedPropertiesList : aliasedExpression ( COMMA aliasedExpression )* ;
    public HqlParser.selectedPropertiesList_return selectedPropertiesList() // throws RecognitionException [1]
    {   
        HqlParser.selectedPropertiesList_return retval = new HqlParser.selectedPropertiesList_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken COMMA138 = null;
        HqlParser.aliasedExpression_return aliasedExpression137 = default(HqlParser.aliasedExpression_return);

        HqlParser.aliasedExpression_return aliasedExpression139 = default(HqlParser.aliasedExpression_return);


        IASTNode COMMA138_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:386:2: ( aliasedExpression ( COMMA aliasedExpression )* )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:386:4: aliasedExpression ( COMMA aliasedExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_aliasedExpression_in_selectedPropertiesList1760);
            	aliasedExpression137 = aliasedExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, aliasedExpression137.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:386:22: ( COMMA aliasedExpression )*
            	do 
            	{
            	    int alt44 = 2;
            	    int LA44_0 = input.LA(1);

            	    if ( (LA44_0 == COMMA) )
            	    {
            	        alt44 = 1;
            	    }


            	    switch (alt44) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:386:24: COMMA aliasedExpression
            			    {
            			    	COMMA138=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_selectedPropertiesList1764); 
            			    	PushFollow(FOLLOW_aliasedExpression_in_selectedPropertiesList1767);
            			    	aliasedExpression139 = aliasedExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, aliasedExpression139.Tree);

            			    }
            			    break;

            			default:
            			    goto loop44;
            	    }
            	} while (true);

            	loop44:
            		;	// Stops C# compiler whining that label 'loop44' has no statements


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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:389:1: aliasedExpression : expression ( AS identifier )? ;
    public HqlParser.aliasedExpression_return aliasedExpression() // throws RecognitionException [1]
    {   
        HqlParser.aliasedExpression_return retval = new HqlParser.aliasedExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken AS141 = null;
        HqlParser.expression_return expression140 = default(HqlParser.expression_return);

        HqlParser.identifier_return identifier142 = default(HqlParser.identifier_return);


        IASTNode AS141_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:390:2: ( expression ( AS identifier )? )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:390:4: expression ( AS identifier )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_expression_in_aliasedExpression1782);
            	expression140 = expression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, expression140.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:390:15: ( AS identifier )?
            	int alt45 = 2;
            	int LA45_0 = input.LA(1);

            	if ( (LA45_0 == AS) )
            	{
            	    alt45 = 1;
            	}
            	switch (alt45) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:390:17: AS identifier
            	        {
            	        	AS141=(IToken)Match(input,AS,FOLLOW_AS_in_aliasedExpression1786); 
            	        		AS141_tree = (IASTNode)adaptor.Create(AS141);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(AS141_tree, root_0);

            	        	PushFollow(FOLLOW_identifier_in_aliasedExpression1789);
            	        	identifier142 = identifier();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, identifier142.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:418:1: logicalExpression : expression ;
    public HqlParser.logicalExpression_return logicalExpression() // throws RecognitionException [1]
    {   
        HqlParser.logicalExpression_return retval = new HqlParser.logicalExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.expression_return expression143 = default(HqlParser.expression_return);



        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:419:2: ( expression )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:419:4: expression
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_expression_in_logicalExpression1828);
            	expression143 = expression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, expression143.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:423:1: expression : logicalOrExpression ;
    public HqlParser.expression_return expression() // throws RecognitionException [1]
    {   
        HqlParser.expression_return retval = new HqlParser.expression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.logicalOrExpression_return logicalOrExpression144 = default(HqlParser.logicalOrExpression_return);



        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:424:2: ( logicalOrExpression )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:424:4: logicalOrExpression
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_logicalOrExpression_in_expression1840);
            	logicalOrExpression144 = logicalOrExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, logicalOrExpression144.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:428:1: logicalOrExpression : logicalAndExpression ( OR logicalAndExpression )* ;
    public HqlParser.logicalOrExpression_return logicalOrExpression() // throws RecognitionException [1]
    {   
        HqlParser.logicalOrExpression_return retval = new HqlParser.logicalOrExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken OR146 = null;
        HqlParser.logicalAndExpression_return logicalAndExpression145 = default(HqlParser.logicalAndExpression_return);

        HqlParser.logicalAndExpression_return logicalAndExpression147 = default(HqlParser.logicalAndExpression_return);


        IASTNode OR146_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:429:2: ( logicalAndExpression ( OR logicalAndExpression )* )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:429:4: logicalAndExpression ( OR logicalAndExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_logicalAndExpression_in_logicalOrExpression1852);
            	logicalAndExpression145 = logicalAndExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, logicalAndExpression145.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:429:25: ( OR logicalAndExpression )*
            	do 
            	{
            	    int alt46 = 2;
            	    int LA46_0 = input.LA(1);

            	    if ( (LA46_0 == OR) )
            	    {
            	        alt46 = 1;
            	    }


            	    switch (alt46) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:429:27: OR logicalAndExpression
            			    {
            			    	OR146=(IToken)Match(input,OR,FOLLOW_OR_in_logicalOrExpression1856); 
            			    		OR146_tree = (IASTNode)adaptor.Create(OR146);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(OR146_tree, root_0);

            			    	PushFollow(FOLLOW_logicalAndExpression_in_logicalOrExpression1859);
            			    	logicalAndExpression147 = logicalAndExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, logicalAndExpression147.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:433:1: logicalAndExpression : negatedExpression ( AND negatedExpression )* ;
    public HqlParser.logicalAndExpression_return logicalAndExpression() // throws RecognitionException [1]
    {   
        HqlParser.logicalAndExpression_return retval = new HqlParser.logicalAndExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken AND149 = null;
        HqlParser.negatedExpression_return negatedExpression148 = default(HqlParser.negatedExpression_return);

        HqlParser.negatedExpression_return negatedExpression150 = default(HqlParser.negatedExpression_return);


        IASTNode AND149_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:434:2: ( negatedExpression ( AND negatedExpression )* )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:434:4: negatedExpression ( AND negatedExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_negatedExpression_in_logicalAndExpression1874);
            	negatedExpression148 = negatedExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, negatedExpression148.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:434:22: ( AND negatedExpression )*
            	do 
            	{
            	    int alt47 = 2;
            	    int LA47_0 = input.LA(1);

            	    if ( (LA47_0 == AND) )
            	    {
            	        alt47 = 1;
            	    }


            	    switch (alt47) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:434:24: AND negatedExpression
            			    {
            			    	AND149=(IToken)Match(input,AND,FOLLOW_AND_in_logicalAndExpression1878); 
            			    		AND149_tree = (IASTNode)adaptor.Create(AND149);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(AND149_tree, root_0);

            			    	PushFollow(FOLLOW_negatedExpression_in_logicalAndExpression1881);
            			    	negatedExpression150 = negatedExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, negatedExpression150.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:439:1: negatedExpression : ( NOT x= negatedExpression -> ^() | bitwiseNotExpression -> ^( bitwiseNotExpression ) );
    public HqlParser.negatedExpression_return negatedExpression() // throws RecognitionException [1]
    {   
        HqlParser.negatedExpression_return retval = new HqlParser.negatedExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken NOT151 = null;
        HqlParser.negatedExpression_return x = default(HqlParser.negatedExpression_return);

        HqlParser.bitwiseNotExpression_return bitwiseNotExpression152 = default(HqlParser.bitwiseNotExpression_return);


        IASTNode NOT151_tree=null;
        RewriteRuleTokenStream stream_NOT = new RewriteRuleTokenStream(adaptor,"token NOT");
        RewriteRuleSubtreeStream stream_negatedExpression = new RewriteRuleSubtreeStream(adaptor,"rule negatedExpression");
        RewriteRuleSubtreeStream stream_bitwiseNotExpression = new RewriteRuleSubtreeStream(adaptor,"rule bitwiseNotExpression");
         WeakKeywords(); 
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:441:2: ( NOT x= negatedExpression -> ^() | bitwiseNotExpression -> ^( bitwiseNotExpression ) )
            int alt48 = 2;
            int LA48_0 = input.LA(1);

            if ( (LA48_0 == NOT) )
            {
                alt48 = 1;
            }
            else if ( ((LA48_0 >= ALL && LA48_0 <= ANY) || LA48_0 == AVG || LA48_0 == COUNT || LA48_0 == ELEMENTS || (LA48_0 >= EXISTS && LA48_0 <= FALSE) || LA48_0 == INDICES || (LA48_0 >= MAX && LA48_0 <= MIN) || LA48_0 == NULL || (LA48_0 >= SOME && LA48_0 <= TRUE) || LA48_0 == CASE || LA48_0 == EMPTY || (LA48_0 >= NUM_INT && LA48_0 <= NUM_LONG) || LA48_0 == OPEN || LA48_0 == BNOT || (LA48_0 >= PLUS && LA48_0 <= MINUS) || (LA48_0 >= COLON && LA48_0 <= IDENT)) )
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
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:441:4: NOT x= negatedExpression
                    {
                    	NOT151=(IToken)Match(input,NOT,FOLLOW_NOT_in_negatedExpression1902);  
                    	stream_NOT.Add(NOT151);

                    	PushFollow(FOLLOW_negatedExpression_in_negatedExpression1906);
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
                    	// 442:3: -> ^()
                    	{
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:442:6: ^()
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
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:443:4: bitwiseNotExpression
                    {
                    	PushFollow(FOLLOW_bitwiseNotExpression_in_negatedExpression1919);
                    	bitwiseNotExpression152 = bitwiseNotExpression();
                    	state.followingStackPointer--;

                    	stream_bitwiseNotExpression.Add(bitwiseNotExpression152.Tree);


                    	// AST REWRITE
                    	// elements:          bitwiseNotExpression
                    	// token labels:      
                    	// rule labels:       retval
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

                    	root_0 = (IASTNode)adaptor.GetNilNode();
                    	// 444:3: -> ^( bitwiseNotExpression )
                    	{
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:444:6: ^( bitwiseNotExpression )
                    	    {
                    	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
                    	    root_1 = (IASTNode)adaptor.BecomeRoot(stream_bitwiseNotExpression.NextNode(), root_1);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:450:1: bitwiseNotExpression : ( ( BNOT bitwiseOrExpression ) | bitwiseOrExpression );
    public HqlParser.bitwiseNotExpression_return bitwiseNotExpression() // throws RecognitionException [1]
    {   
        HqlParser.bitwiseNotExpression_return retval = new HqlParser.bitwiseNotExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken BNOT153 = null;
        HqlParser.bitwiseOrExpression_return bitwiseOrExpression154 = default(HqlParser.bitwiseOrExpression_return);

        HqlParser.bitwiseOrExpression_return bitwiseOrExpression155 = default(HqlParser.bitwiseOrExpression_return);


        IASTNode BNOT153_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:451:2: ( ( BNOT bitwiseOrExpression ) | bitwiseOrExpression )
            int alt49 = 2;
            int LA49_0 = input.LA(1);

            if ( (LA49_0 == BNOT) )
            {
                alt49 = 1;
            }
            else if ( ((LA49_0 >= ALL && LA49_0 <= ANY) || LA49_0 == AVG || LA49_0 == COUNT || LA49_0 == ELEMENTS || (LA49_0 >= EXISTS && LA49_0 <= FALSE) || LA49_0 == INDICES || (LA49_0 >= MAX && LA49_0 <= MIN) || LA49_0 == NULL || (LA49_0 >= SOME && LA49_0 <= TRUE) || LA49_0 == CASE || LA49_0 == EMPTY || (LA49_0 >= NUM_INT && LA49_0 <= NUM_LONG) || LA49_0 == OPEN || (LA49_0 >= PLUS && LA49_0 <= MINUS) || (LA49_0 >= COLON && LA49_0 <= IDENT)) )
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
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:451:4: ( BNOT bitwiseOrExpression )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:451:4: ( BNOT bitwiseOrExpression )
                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:451:5: BNOT bitwiseOrExpression
                    	{
                    		BNOT153=(IToken)Match(input,BNOT,FOLLOW_BNOT_in_bitwiseNotExpression1943); 
                    			BNOT153_tree = (IASTNode)adaptor.Create(BNOT153);
                    			root_0 = (IASTNode)adaptor.BecomeRoot(BNOT153_tree, root_0);

                    		PushFollow(FOLLOW_bitwiseOrExpression_in_bitwiseNotExpression1946);
                    		bitwiseOrExpression154 = bitwiseOrExpression();
                    		state.followingStackPointer--;

                    		adaptor.AddChild(root_0, bitwiseOrExpression154.Tree);

                    	}


                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:452:4: bitwiseOrExpression
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_bitwiseOrExpression_in_bitwiseNotExpression1952);
                    	bitwiseOrExpression155 = bitwiseOrExpression();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, bitwiseOrExpression155.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:455:1: bitwiseOrExpression : bitwiseXOrExpression ( BOR bitwiseXOrExpression )* ;
    public HqlParser.bitwiseOrExpression_return bitwiseOrExpression() // throws RecognitionException [1]
    {   
        HqlParser.bitwiseOrExpression_return retval = new HqlParser.bitwiseOrExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken BOR157 = null;
        HqlParser.bitwiseXOrExpression_return bitwiseXOrExpression156 = default(HqlParser.bitwiseXOrExpression_return);

        HqlParser.bitwiseXOrExpression_return bitwiseXOrExpression158 = default(HqlParser.bitwiseXOrExpression_return);


        IASTNode BOR157_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:456:2: ( bitwiseXOrExpression ( BOR bitwiseXOrExpression )* )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:456:4: bitwiseXOrExpression ( BOR bitwiseXOrExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_bitwiseXOrExpression_in_bitwiseOrExpression1964);
            	bitwiseXOrExpression156 = bitwiseXOrExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, bitwiseXOrExpression156.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:456:25: ( BOR bitwiseXOrExpression )*
            	do 
            	{
            	    int alt50 = 2;
            	    int LA50_0 = input.LA(1);

            	    if ( (LA50_0 == BOR) )
            	    {
            	        alt50 = 1;
            	    }


            	    switch (alt50) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:456:26: BOR bitwiseXOrExpression
            			    {
            			    	BOR157=(IToken)Match(input,BOR,FOLLOW_BOR_in_bitwiseOrExpression1967); 
            			    		BOR157_tree = (IASTNode)adaptor.Create(BOR157);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(BOR157_tree, root_0);

            			    	PushFollow(FOLLOW_bitwiseXOrExpression_in_bitwiseOrExpression1970);
            			    	bitwiseXOrExpression158 = bitwiseXOrExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, bitwiseXOrExpression158.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:459:1: bitwiseXOrExpression : bitwiseAndExpression ( BXOR bitwiseAndExpression )* ;
    public HqlParser.bitwiseXOrExpression_return bitwiseXOrExpression() // throws RecognitionException [1]
    {   
        HqlParser.bitwiseXOrExpression_return retval = new HqlParser.bitwiseXOrExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken BXOR160 = null;
        HqlParser.bitwiseAndExpression_return bitwiseAndExpression159 = default(HqlParser.bitwiseAndExpression_return);

        HqlParser.bitwiseAndExpression_return bitwiseAndExpression161 = default(HqlParser.bitwiseAndExpression_return);


        IASTNode BXOR160_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:460:2: ( bitwiseAndExpression ( BXOR bitwiseAndExpression )* )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:460:4: bitwiseAndExpression ( BXOR bitwiseAndExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_bitwiseAndExpression_in_bitwiseXOrExpression1984);
            	bitwiseAndExpression159 = bitwiseAndExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, bitwiseAndExpression159.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:460:25: ( BXOR bitwiseAndExpression )*
            	do 
            	{
            	    int alt51 = 2;
            	    int LA51_0 = input.LA(1);

            	    if ( (LA51_0 == BXOR) )
            	    {
            	        alt51 = 1;
            	    }


            	    switch (alt51) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:460:26: BXOR bitwiseAndExpression
            			    {
            			    	BXOR160=(IToken)Match(input,BXOR,FOLLOW_BXOR_in_bitwiseXOrExpression1987); 
            			    		BXOR160_tree = (IASTNode)adaptor.Create(BXOR160);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(BXOR160_tree, root_0);

            			    	PushFollow(FOLLOW_bitwiseAndExpression_in_bitwiseXOrExpression1990);
            			    	bitwiseAndExpression161 = bitwiseAndExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, bitwiseAndExpression161.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:463:1: bitwiseAndExpression : equalityExpression ( BAND equalityExpression )* ;
    public HqlParser.bitwiseAndExpression_return bitwiseAndExpression() // throws RecognitionException [1]
    {   
        HqlParser.bitwiseAndExpression_return retval = new HqlParser.bitwiseAndExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken BAND163 = null;
        HqlParser.equalityExpression_return equalityExpression162 = default(HqlParser.equalityExpression_return);

        HqlParser.equalityExpression_return equalityExpression164 = default(HqlParser.equalityExpression_return);


        IASTNode BAND163_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:464:2: ( equalityExpression ( BAND equalityExpression )* )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:464:4: equalityExpression ( BAND equalityExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_equalityExpression_in_bitwiseAndExpression2004);
            	equalityExpression162 = equalityExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, equalityExpression162.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:464:23: ( BAND equalityExpression )*
            	do 
            	{
            	    int alt52 = 2;
            	    int LA52_0 = input.LA(1);

            	    if ( (LA52_0 == BAND) )
            	    {
            	        alt52 = 1;
            	    }


            	    switch (alt52) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:464:24: BAND equalityExpression
            			    {
            			    	BAND163=(IToken)Match(input,BAND,FOLLOW_BAND_in_bitwiseAndExpression2007); 
            			    		BAND163_tree = (IASTNode)adaptor.Create(BAND163);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(BAND163_tree, root_0);

            			    	PushFollow(FOLLOW_equalityExpression_in_bitwiseAndExpression2010);
            			    	equalityExpression164 = equalityExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, equalityExpression164.Tree);

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
    // $ANTLR end "bitwiseAndExpression"

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:468:1: equalityExpression : x= relationalExpression ( ( EQ | isx= IS ( NOT )? | NE | ne= SQL_NE ) y= relationalExpression )* ;
    public HqlParser.equalityExpression_return equalityExpression() // throws RecognitionException [1]
    {   
        HqlParser.equalityExpression_return retval = new HqlParser.equalityExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken isx = null;
        IToken ne = null;
        IToken EQ165 = null;
        IToken NOT166 = null;
        IToken NE167 = null;
        HqlParser.relationalExpression_return x = default(HqlParser.relationalExpression_return);

        HqlParser.relationalExpression_return y = default(HqlParser.relationalExpression_return);


        IASTNode isx_tree=null;
        IASTNode ne_tree=null;
        IASTNode EQ165_tree=null;
        IASTNode NOT166_tree=null;
        IASTNode NE167_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:473:2: (x= relationalExpression ( ( EQ | isx= IS ( NOT )? | NE | ne= SQL_NE ) y= relationalExpression )* )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:473:4: x= relationalExpression ( ( EQ | isx= IS ( NOT )? | NE | ne= SQL_NE ) y= relationalExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_relationalExpression_in_equalityExpression2032);
            	x = relationalExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, x.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:473:27: ( ( EQ | isx= IS ( NOT )? | NE | ne= SQL_NE ) y= relationalExpression )*
            	do 
            	{
            	    int alt55 = 2;
            	    int LA55_0 = input.LA(1);

            	    if ( (LA55_0 == IS || LA55_0 == EQ || (LA55_0 >= NE && LA55_0 <= SQL_NE)) )
            	    {
            	        alt55 = 1;
            	    }


            	    switch (alt55) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:474:3: ( EQ | isx= IS ( NOT )? | NE | ne= SQL_NE ) y= relationalExpression
            			    {
            			    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:474:3: ( EQ | isx= IS ( NOT )? | NE | ne= SQL_NE )
            			    	int alt54 = 4;
            			    	switch ( input.LA(1) ) 
            			    	{
            			    	case EQ:
            			    		{
            			    	    alt54 = 1;
            			    	    }
            			    	    break;
            			    	case IS:
            			    		{
            			    	    alt54 = 2;
            			    	    }
            			    	    break;
            			    	case NE:
            			    		{
            			    	    alt54 = 3;
            			    	    }
            			    	    break;
            			    	case SQL_NE:
            			    		{
            			    	    alt54 = 4;
            			    	    }
            			    	    break;
            			    		default:
            			    		    NoViableAltException nvae_d54s0 =
            			    		        new NoViableAltException("", 54, 0, input);

            			    		    throw nvae_d54s0;
            			    	}

            			    	switch (alt54) 
            			    	{
            			    	    case 1 :
            			    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:474:5: EQ
            			    	        {
            			    	        	EQ165=(IToken)Match(input,EQ,FOLLOW_EQ_in_equalityExpression2040); 
            			    	        		EQ165_tree = (IASTNode)adaptor.Create(EQ165);
            			    	        		root_0 = (IASTNode)adaptor.BecomeRoot(EQ165_tree, root_0);


            			    	        }
            			    	        break;
            			    	    case 2 :
            			    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:475:5: isx= IS ( NOT )?
            			    	        {
            			    	        	isx=(IToken)Match(input,IS,FOLLOW_IS_in_equalityExpression2049); 
            			    	        		isx_tree = (IASTNode)adaptor.Create(isx);
            			    	        		root_0 = (IASTNode)adaptor.BecomeRoot(isx_tree, root_0);

            			    	        	 isx.Type = EQ; 
            			    	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:475:33: ( NOT )?
            			    	        	int alt53 = 2;
            			    	        	int LA53_0 = input.LA(1);

            			    	        	if ( (LA53_0 == NOT) )
            			    	        	{
            			    	        	    alt53 = 1;
            			    	        	}
            			    	        	switch (alt53) 
            			    	        	{
            			    	        	    case 1 :
            			    	        	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:475:34: NOT
            			    	        	        {
            			    	        	        	NOT166=(IToken)Match(input,NOT,FOLLOW_NOT_in_equalityExpression2055); 
            			    	        	        	 isx.Type =NE; 

            			    	        	        }
            			    	        	        break;

            			    	        	}


            			    	        }
            			    	        break;
            			    	    case 3 :
            			    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:476:5: NE
            			    	        {
            			    	        	NE167=(IToken)Match(input,NE,FOLLOW_NE_in_equalityExpression2067); 
            			    	        		NE167_tree = (IASTNode)adaptor.Create(NE167);
            			    	        		root_0 = (IASTNode)adaptor.BecomeRoot(NE167_tree, root_0);


            			    	        }
            			    	        break;
            			    	    case 4 :
            			    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:477:5: ne= SQL_NE
            			    	        {
            			    	        	ne=(IToken)Match(input,SQL_NE,FOLLOW_SQL_NE_in_equalityExpression2076); 
            			    	        		ne_tree = (IASTNode)adaptor.Create(ne);
            			    	        		root_0 = (IASTNode)adaptor.BecomeRoot(ne_tree, root_0);

            			    	        	 ne.Type = NE; 

            			    	        }
            			    	        break;

            			    	}

            			    	PushFollow(FOLLOW_relationalExpression_in_equalityExpression2087);
            			    	y = relationalExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, y.Tree);

            			    }
            			    break;

            			default:
            			    goto loop55;
            	    }
            	} while (true);

            	loop55:
            		;	// Stops C# compiler whining that label 'loop55' has no statements


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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:485:1: relationalExpression : concatenation ( ( ( ( LT | GT | LE | GE ) additiveExpression )* ) | (n= NOT )? ( (i= IN inList ) | (b= BETWEEN betweenList ) | (l= LIKE concatenation likeEscape ) | ( MEMBER ( OF )? p= path ) ) ) ;
    public HqlParser.relationalExpression_return relationalExpression() // throws RecognitionException [1]
    {   
        HqlParser.relationalExpression_return retval = new HqlParser.relationalExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken n = null;
        IToken i = null;
        IToken b = null;
        IToken l = null;
        IToken LT169 = null;
        IToken GT170 = null;
        IToken LE171 = null;
        IToken GE172 = null;
        IToken MEMBER178 = null;
        IToken OF179 = null;
        HqlParser.path_return p = default(HqlParser.path_return);

        HqlParser.concatenation_return concatenation168 = default(HqlParser.concatenation_return);

        HqlParser.additiveExpression_return additiveExpression173 = default(HqlParser.additiveExpression_return);

        HqlParser.inList_return inList174 = default(HqlParser.inList_return);

        HqlParser.betweenList_return betweenList175 = default(HqlParser.betweenList_return);

        HqlParser.concatenation_return concatenation176 = default(HqlParser.concatenation_return);

        HqlParser.likeEscape_return likeEscape177 = default(HqlParser.likeEscape_return);


        IASTNode n_tree=null;
        IASTNode i_tree=null;
        IASTNode b_tree=null;
        IASTNode l_tree=null;
        IASTNode LT169_tree=null;
        IASTNode GT170_tree=null;
        IASTNode LE171_tree=null;
        IASTNode GE172_tree=null;
        IASTNode MEMBER178_tree=null;
        IASTNode OF179_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:486:2: ( concatenation ( ( ( ( LT | GT | LE | GE ) additiveExpression )* ) | (n= NOT )? ( (i= IN inList ) | (b= BETWEEN betweenList ) | (l= LIKE concatenation likeEscape ) | ( MEMBER ( OF )? p= path ) ) ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:486:4: concatenation ( ( ( ( LT | GT | LE | GE ) additiveExpression )* ) | (n= NOT )? ( (i= IN inList ) | (b= BETWEEN betweenList ) | (l= LIKE concatenation likeEscape ) | ( MEMBER ( OF )? p= path ) ) )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_concatenation_in_relationalExpression2104);
            	concatenation168 = concatenation();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, concatenation168.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:486:18: ( ( ( ( LT | GT | LE | GE ) additiveExpression )* ) | (n= NOT )? ( (i= IN inList ) | (b= BETWEEN betweenList ) | (l= LIKE concatenation likeEscape ) | ( MEMBER ( OF )? p= path ) ) )
            	int alt61 = 2;
            	int LA61_0 = input.LA(1);

            	if ( (LA61_0 == EOF || (LA61_0 >= AND && LA61_0 <= ASCENDING) || LA61_0 == DESCENDING || (LA61_0 >= FROM && LA61_0 <= HAVING) || LA61_0 == INNER || (LA61_0 >= IS && LA61_0 <= LEFT) || (LA61_0 >= OR && LA61_0 <= ORDER) || LA61_0 == RIGHT || LA61_0 == UNION || LA61_0 == WHERE || LA61_0 == THEN || (LA61_0 >= COMMA && LA61_0 <= EQ) || LA61_0 == CLOSE || (LA61_0 >= BOR && LA61_0 <= GE) || LA61_0 == CLOSE_BRACKET || (LA61_0 >= 130 && LA61_0 <= 131)) )
            	{
            	    alt61 = 1;
            	}
            	else if ( (LA61_0 == BETWEEN || LA61_0 == IN || LA61_0 == LIKE || LA61_0 == NOT || LA61_0 == MEMBER) )
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
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:487:3: ( ( ( LT | GT | LE | GE ) additiveExpression )* )
            	        {
            	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:487:3: ( ( ( LT | GT | LE | GE ) additiveExpression )* )
            	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:487:5: ( ( LT | GT | LE | GE ) additiveExpression )*
            	        	{
            	        		// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:487:5: ( ( LT | GT | LE | GE ) additiveExpression )*
            	        		do 
            	        		{
            	        		    int alt57 = 2;
            	        		    int LA57_0 = input.LA(1);

            	        		    if ( ((LA57_0 >= LT && LA57_0 <= GE)) )
            	        		    {
            	        		        alt57 = 1;
            	        		    }


            	        		    switch (alt57) 
            	        			{
            	        				case 1 :
            	        				    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:487:7: ( LT | GT | LE | GE ) additiveExpression
            	        				    {
            	        				    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:487:7: ( LT | GT | LE | GE )
            	        				    	int alt56 = 4;
            	        				    	switch ( input.LA(1) ) 
            	        				    	{
            	        				    	case LT:
            	        				    		{
            	        				    	    alt56 = 1;
            	        				    	    }
            	        				    	    break;
            	        				    	case GT:
            	        				    		{
            	        				    	    alt56 = 2;
            	        				    	    }
            	        				    	    break;
            	        				    	case LE:
            	        				    		{
            	        				    	    alt56 = 3;
            	        				    	    }
            	        				    	    break;
            	        				    	case GE:
            	        				    		{
            	        				    	    alt56 = 4;
            	        				    	    }
            	        				    	    break;
            	        				    		default:
            	        				    		    NoViableAltException nvae_d56s0 =
            	        				    		        new NoViableAltException("", 56, 0, input);

            	        				    		    throw nvae_d56s0;
            	        				    	}

            	        				    	switch (alt56) 
            	        				    	{
            	        				    	    case 1 :
            	        				    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:487:9: LT
            	        				    	        {
            	        				    	        	LT169=(IToken)Match(input,LT,FOLLOW_LT_in_relationalExpression2116); 
            	        				    	        		LT169_tree = (IASTNode)adaptor.Create(LT169);
            	        				    	        		root_0 = (IASTNode)adaptor.BecomeRoot(LT169_tree, root_0);


            	        				    	        }
            	        				    	        break;
            	        				    	    case 2 :
            	        				    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:487:15: GT
            	        				    	        {
            	        				    	        	GT170=(IToken)Match(input,GT,FOLLOW_GT_in_relationalExpression2121); 
            	        				    	        		GT170_tree = (IASTNode)adaptor.Create(GT170);
            	        				    	        		root_0 = (IASTNode)adaptor.BecomeRoot(GT170_tree, root_0);


            	        				    	        }
            	        				    	        break;
            	        				    	    case 3 :
            	        				    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:487:21: LE
            	        				    	        {
            	        				    	        	LE171=(IToken)Match(input,LE,FOLLOW_LE_in_relationalExpression2126); 
            	        				    	        		LE171_tree = (IASTNode)adaptor.Create(LE171);
            	        				    	        		root_0 = (IASTNode)adaptor.BecomeRoot(LE171_tree, root_0);


            	        				    	        }
            	        				    	        break;
            	        				    	    case 4 :
            	        				    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:487:27: GE
            	        				    	        {
            	        				    	        	GE172=(IToken)Match(input,GE,FOLLOW_GE_in_relationalExpression2131); 
            	        				    	        		GE172_tree = (IASTNode)adaptor.Create(GE172);
            	        				    	        		root_0 = (IASTNode)adaptor.BecomeRoot(GE172_tree, root_0);


            	        				    	        }
            	        				    	        break;

            	        				    	}

            	        				    	PushFollow(FOLLOW_additiveExpression_in_relationalExpression2136);
            	        				    	additiveExpression173 = additiveExpression();
            	        				    	state.followingStackPointer--;

            	        				    	adaptor.AddChild(root_0, additiveExpression173.Tree);

            	        				    }
            	        				    break;

            	        				default:
            	        				    goto loop57;
            	        		    }
            	        		} while (true);

            	        		loop57:
            	        			;	// Stops C# compiler whining that label 'loop57' has no statements


            	        	}


            	        }
            	        break;
            	    case 2 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:489:5: (n= NOT )? ( (i= IN inList ) | (b= BETWEEN betweenList ) | (l= LIKE concatenation likeEscape ) | ( MEMBER ( OF )? p= path ) )
            	        {
            	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:489:5: (n= NOT )?
            	        	int alt58 = 2;
            	        	int LA58_0 = input.LA(1);

            	        	if ( (LA58_0 == NOT) )
            	        	{
            	        	    alt58 = 1;
            	        	}
            	        	switch (alt58) 
            	        	{
            	        	    case 1 :
            	        	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:489:6: n= NOT
            	        	        {
            	        	        	n=(IToken)Match(input,NOT,FOLLOW_NOT_in_relationalExpression2153); 

            	        	        }
            	        	        break;

            	        	}

            	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:489:15: ( (i= IN inList ) | (b= BETWEEN betweenList ) | (l= LIKE concatenation likeEscape ) | ( MEMBER ( OF )? p= path ) )
            	        	int alt60 = 4;
            	        	switch ( input.LA(1) ) 
            	        	{
            	        	case IN:
            	        		{
            	        	    alt60 = 1;
            	        	    }
            	        	    break;
            	        	case BETWEEN:
            	        		{
            	        	    alt60 = 2;
            	        	    }
            	        	    break;
            	        	case LIKE:
            	        		{
            	        	    alt60 = 3;
            	        	    }
            	        	    break;
            	        	case MEMBER:
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
            	        	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:492:4: (i= IN inList )
            	        	        {
            	        	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:492:4: (i= IN inList )
            	        	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:492:5: i= IN inList
            	        	        	{
            	        	        		i=(IToken)Match(input,IN,FOLLOW_IN_in_relationalExpression2174); 
            	        	        			i_tree = (IASTNode)adaptor.Create(i);
            	        	        			root_0 = (IASTNode)adaptor.BecomeRoot(i_tree, root_0);


            	        	        							i.Type = (n == null) ? IN : NOT_IN;
            	        	        							i.Text = (n == null) ? "in" : "not in";
            	        	        						
            	        	        		PushFollow(FOLLOW_inList_in_relationalExpression2183);
            	        	        		inList174 = inList();
            	        	        		state.followingStackPointer--;

            	        	        		adaptor.AddChild(root_0, inList174.Tree);

            	        	        	}


            	        	        }
            	        	        break;
            	        	    case 2 :
            	        	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:497:6: (b= BETWEEN betweenList )
            	        	        {
            	        	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:497:6: (b= BETWEEN betweenList )
            	        	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:497:7: b= BETWEEN betweenList
            	        	        	{
            	        	        		b=(IToken)Match(input,BETWEEN,FOLLOW_BETWEEN_in_relationalExpression2194); 
            	        	        			b_tree = (IASTNode)adaptor.Create(b);
            	        	        			root_0 = (IASTNode)adaptor.BecomeRoot(b_tree, root_0);


            	        	        							b.Type = (n == null) ? BETWEEN : NOT_BETWEEN;
            	        	        							b.Text = (n == null) ? "between" : "not between";
            	        	        						
            	        	        		PushFollow(FOLLOW_betweenList_in_relationalExpression2203);
            	        	        		betweenList175 = betweenList();
            	        	        		state.followingStackPointer--;

            	        	        		adaptor.AddChild(root_0, betweenList175.Tree);

            	        	        	}


            	        	        }
            	        	        break;
            	        	    case 3 :
            	        	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:502:6: (l= LIKE concatenation likeEscape )
            	        	        {
            	        	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:502:6: (l= LIKE concatenation likeEscape )
            	        	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:502:7: l= LIKE concatenation likeEscape
            	        	        	{
            	        	        		l=(IToken)Match(input,LIKE,FOLLOW_LIKE_in_relationalExpression2215); 
            	        	        			l_tree = (IASTNode)adaptor.Create(l);
            	        	        			root_0 = (IASTNode)adaptor.BecomeRoot(l_tree, root_0);


            	        	        							l.Type = (n == null) ? LIKE : NOT_LIKE;
            	        	        							l.Text = (n == null) ? "like" : "not like";
            	        	        						
            	        	        		PushFollow(FOLLOW_concatenation_in_relationalExpression2224);
            	        	        		concatenation176 = concatenation();
            	        	        		state.followingStackPointer--;

            	        	        		adaptor.AddChild(root_0, concatenation176.Tree);
            	        	        		PushFollow(FOLLOW_likeEscape_in_relationalExpression2226);
            	        	        		likeEscape177 = likeEscape();
            	        	        		state.followingStackPointer--;

            	        	        		adaptor.AddChild(root_0, likeEscape177.Tree);

            	        	        	}


            	        	        }
            	        	        break;
            	        	    case 4 :
            	        	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:507:6: ( MEMBER ( OF )? p= path )
            	        	        {
            	        	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:507:6: ( MEMBER ( OF )? p= path )
            	        	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:507:7: MEMBER ( OF )? p= path
            	        	        	{
            	        	        		MEMBER178=(IToken)Match(input,MEMBER,FOLLOW_MEMBER_in_relationalExpression2235); 
            	        	        		// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:507:15: ( OF )?
            	        	        		int alt59 = 2;
            	        	        		int LA59_0 = input.LA(1);

            	        	        		if ( (LA59_0 == OF) )
            	        	        		{
            	        	        		    alt59 = 1;
            	        	        		}
            	        	        		switch (alt59) 
            	        	        		{
            	        	        		    case 1 :
            	        	        		        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:507:16: OF
            	        	        		        {
            	        	        		        	OF179=(IToken)Match(input,OF,FOLLOW_OF_in_relationalExpression2239); 

            	        	        		        }
            	        	        		        break;

            	        	        		}

            	        	        		PushFollow(FOLLOW_path_in_relationalExpression2246);
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:514:1: likeEscape : ( ESCAPE concatenation )? ;
    public HqlParser.likeEscape_return likeEscape() // throws RecognitionException [1]
    {   
        HqlParser.likeEscape_return retval = new HqlParser.likeEscape_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken ESCAPE180 = null;
        HqlParser.concatenation_return concatenation181 = default(HqlParser.concatenation_return);


        IASTNode ESCAPE180_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:515:2: ( ( ESCAPE concatenation )? )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:515:4: ( ESCAPE concatenation )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:515:4: ( ESCAPE concatenation )?
            	int alt62 = 2;
            	int LA62_0 = input.LA(1);

            	if ( (LA62_0 == ESCAPE) )
            	{
            	    alt62 = 1;
            	}
            	switch (alt62) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:515:5: ESCAPE concatenation
            	        {
            	        	ESCAPE180=(IToken)Match(input,ESCAPE,FOLLOW_ESCAPE_in_likeEscape2273); 
            	        		ESCAPE180_tree = (IASTNode)adaptor.Create(ESCAPE180);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(ESCAPE180_tree, root_0);

            	        	PushFollow(FOLLOW_concatenation_in_likeEscape2276);
            	        	concatenation181 = concatenation();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, concatenation181.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:518:1: inList : compoundExpr -> ^( IN_LIST[\"inList\"] compoundExpr ) ;
    public HqlParser.inList_return inList() // throws RecognitionException [1]
    {   
        HqlParser.inList_return retval = new HqlParser.inList_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.compoundExpr_return compoundExpr182 = default(HqlParser.compoundExpr_return);


        RewriteRuleSubtreeStream stream_compoundExpr = new RewriteRuleSubtreeStream(adaptor,"rule compoundExpr");
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:519:2: ( compoundExpr -> ^( IN_LIST[\"inList\"] compoundExpr ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:519:4: compoundExpr
            {
            	PushFollow(FOLLOW_compoundExpr_in_inList2289);
            	compoundExpr182 = compoundExpr();
            	state.followingStackPointer--;

            	stream_compoundExpr.Add(compoundExpr182.Tree);


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
            	// 520:2: -> ^( IN_LIST[\"inList\"] compoundExpr )
            	{
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:520:5: ^( IN_LIST[\"inList\"] compoundExpr )
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:523:1: betweenList : concatenation AND concatenation ;
    public HqlParser.betweenList_return betweenList() // throws RecognitionException [1]
    {   
        HqlParser.betweenList_return retval = new HqlParser.betweenList_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken AND184 = null;
        HqlParser.concatenation_return concatenation183 = default(HqlParser.concatenation_return);

        HqlParser.concatenation_return concatenation185 = default(HqlParser.concatenation_return);


        IASTNode AND184_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:524:2: ( concatenation AND concatenation )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:524:4: concatenation AND concatenation
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_concatenation_in_betweenList2310);
            	concatenation183 = concatenation();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, concatenation183.Tree);
            	AND184=(IToken)Match(input,AND,FOLLOW_AND_in_betweenList2312); 
            	PushFollow(FOLLOW_concatenation_in_betweenList2315);
            	concatenation185 = concatenation();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, concatenation185.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:528:1: concatenation : a= additiveExpression (c= CONCAT additiveExpression ( CONCAT additiveExpression )* )? ;
    public HqlParser.concatenation_return concatenation() // throws RecognitionException [1]
    {   
        HqlParser.concatenation_return retval = new HqlParser.concatenation_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken c = null;
        IToken CONCAT187 = null;
        HqlParser.additiveExpression_return a = default(HqlParser.additiveExpression_return);

        HqlParser.additiveExpression_return additiveExpression186 = default(HqlParser.additiveExpression_return);

        HqlParser.additiveExpression_return additiveExpression188 = default(HqlParser.additiveExpression_return);


        IASTNode c_tree=null;
        IASTNode CONCAT187_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:539:2: (a= additiveExpression (c= CONCAT additiveExpression ( CONCAT additiveExpression )* )? )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:539:4: a= additiveExpression (c= CONCAT additiveExpression ( CONCAT additiveExpression )* )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_additiveExpression_in_concatenation2334);
            	a = additiveExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, a.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:540:2: (c= CONCAT additiveExpression ( CONCAT additiveExpression )* )?
            	int alt64 = 2;
            	int LA64_0 = input.LA(1);

            	if ( (LA64_0 == CONCAT) )
            	{
            	    alt64 = 1;
            	}
            	switch (alt64) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:540:4: c= CONCAT additiveExpression ( CONCAT additiveExpression )*
            	        {
            	        	c=(IToken)Match(input,CONCAT,FOLLOW_CONCAT_in_concatenation2342); 
            	        		c_tree = (IASTNode)adaptor.Create(c);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(c_tree, root_0);

            	        	 c.Type = EXPR_LIST; c.Text = "concatList"; 
            	        	PushFollow(FOLLOW_additiveExpression_in_concatenation2351);
            	        	additiveExpression186 = additiveExpression();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, additiveExpression186.Tree);
            	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:542:4: ( CONCAT additiveExpression )*
            	        	do 
            	        	{
            	        	    int alt63 = 2;
            	        	    int LA63_0 = input.LA(1);

            	        	    if ( (LA63_0 == CONCAT) )
            	        	    {
            	        	        alt63 = 1;
            	        	    }


            	        	    switch (alt63) 
            	        		{
            	        			case 1 :
            	        			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:542:6: CONCAT additiveExpression
            	        			    {
            	        			    	CONCAT187=(IToken)Match(input,CONCAT,FOLLOW_CONCAT_in_concatenation2358); 
            	        			    	PushFollow(FOLLOW_additiveExpression_in_concatenation2361);
            	        			    	additiveExpression188 = additiveExpression();
            	        			    	state.followingStackPointer--;

            	        			    	adaptor.AddChild(root_0, additiveExpression188.Tree);

            	        			    }
            	        			    break;

            	        			default:
            	        			    goto loop63;
            	        	    }
            	        	} while (true);

            	        	loop63:
            	        		;	// Stops C# compiler whining that label 'loop63' has no statements


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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:547:1: additiveExpression : multiplyExpression ( ( PLUS | MINUS ) multiplyExpression )* ;
    public HqlParser.additiveExpression_return additiveExpression() // throws RecognitionException [1]
    {   
        HqlParser.additiveExpression_return retval = new HqlParser.additiveExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken PLUS190 = null;
        IToken MINUS191 = null;
        HqlParser.multiplyExpression_return multiplyExpression189 = default(HqlParser.multiplyExpression_return);

        HqlParser.multiplyExpression_return multiplyExpression192 = default(HqlParser.multiplyExpression_return);


        IASTNode PLUS190_tree=null;
        IASTNode MINUS191_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:548:2: ( multiplyExpression ( ( PLUS | MINUS ) multiplyExpression )* )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:548:4: multiplyExpression ( ( PLUS | MINUS ) multiplyExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_multiplyExpression_in_additiveExpression2383);
            	multiplyExpression189 = multiplyExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, multiplyExpression189.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:548:23: ( ( PLUS | MINUS ) multiplyExpression )*
            	do 
            	{
            	    int alt66 = 2;
            	    int LA66_0 = input.LA(1);

            	    if ( ((LA66_0 >= PLUS && LA66_0 <= MINUS)) )
            	    {
            	        alt66 = 1;
            	    }


            	    switch (alt66) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:548:25: ( PLUS | MINUS ) multiplyExpression
            			    {
            			    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:548:25: ( PLUS | MINUS )
            			    	int alt65 = 2;
            			    	int LA65_0 = input.LA(1);

            			    	if ( (LA65_0 == PLUS) )
            			    	{
            			    	    alt65 = 1;
            			    	}
            			    	else if ( (LA65_0 == MINUS) )
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
            			    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:548:27: PLUS
            			    	        {
            			    	        	PLUS190=(IToken)Match(input,PLUS,FOLLOW_PLUS_in_additiveExpression2389); 
            			    	        		PLUS190_tree = (IASTNode)adaptor.Create(PLUS190);
            			    	        		root_0 = (IASTNode)adaptor.BecomeRoot(PLUS190_tree, root_0);


            			    	        }
            			    	        break;
            			    	    case 2 :
            			    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:548:35: MINUS
            			    	        {
            			    	        	MINUS191=(IToken)Match(input,MINUS,FOLLOW_MINUS_in_additiveExpression2394); 
            			    	        		MINUS191_tree = (IASTNode)adaptor.Create(MINUS191);
            			    	        		root_0 = (IASTNode)adaptor.BecomeRoot(MINUS191_tree, root_0);


            			    	        }
            			    	        break;

            			    	}

            			    	PushFollow(FOLLOW_multiplyExpression_in_additiveExpression2399);
            			    	multiplyExpression192 = multiplyExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, multiplyExpression192.Tree);

            			    }
            			    break;

            			default:
            			    goto loop66;
            	    }
            	} while (true);

            	loop66:
            		;	// Stops C# compiler whining that label 'loop66' has no statements


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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:552:1: multiplyExpression : unaryExpression ( ( STAR | DIV ) unaryExpression )* ;
    public HqlParser.multiplyExpression_return multiplyExpression() // throws RecognitionException [1]
    {   
        HqlParser.multiplyExpression_return retval = new HqlParser.multiplyExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken STAR194 = null;
        IToken DIV195 = null;
        HqlParser.unaryExpression_return unaryExpression193 = default(HqlParser.unaryExpression_return);

        HqlParser.unaryExpression_return unaryExpression196 = default(HqlParser.unaryExpression_return);


        IASTNode STAR194_tree=null;
        IASTNode DIV195_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:553:2: ( unaryExpression ( ( STAR | DIV ) unaryExpression )* )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:553:4: unaryExpression ( ( STAR | DIV ) unaryExpression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_unaryExpression_in_multiplyExpression2414);
            	unaryExpression193 = unaryExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, unaryExpression193.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:553:20: ( ( STAR | DIV ) unaryExpression )*
            	do 
            	{
            	    int alt68 = 2;
            	    int LA68_0 = input.LA(1);

            	    if ( ((LA68_0 >= STAR && LA68_0 <= DIV)) )
            	    {
            	        alt68 = 1;
            	    }


            	    switch (alt68) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:553:22: ( STAR | DIV ) unaryExpression
            			    {
            			    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:553:22: ( STAR | DIV )
            			    	int alt67 = 2;
            			    	int LA67_0 = input.LA(1);

            			    	if ( (LA67_0 == STAR) )
            			    	{
            			    	    alt67 = 1;
            			    	}
            			    	else if ( (LA67_0 == DIV) )
            			    	{
            			    	    alt67 = 2;
            			    	}
            			    	else 
            			    	{
            			    	    NoViableAltException nvae_d67s0 =
            			    	        new NoViableAltException("", 67, 0, input);

            			    	    throw nvae_d67s0;
            			    	}
            			    	switch (alt67) 
            			    	{
            			    	    case 1 :
            			    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:553:24: STAR
            			    	        {
            			    	        	STAR194=(IToken)Match(input,STAR,FOLLOW_STAR_in_multiplyExpression2420); 
            			    	        		STAR194_tree = (IASTNode)adaptor.Create(STAR194);
            			    	        		root_0 = (IASTNode)adaptor.BecomeRoot(STAR194_tree, root_0);


            			    	        }
            			    	        break;
            			    	    case 2 :
            			    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:553:32: DIV
            			    	        {
            			    	        	DIV195=(IToken)Match(input,DIV,FOLLOW_DIV_in_multiplyExpression2425); 
            			    	        		DIV195_tree = (IASTNode)adaptor.Create(DIV195);
            			    	        		root_0 = (IASTNode)adaptor.BecomeRoot(DIV195_tree, root_0);


            			    	        }
            			    	        break;

            			    	}

            			    	PushFollow(FOLLOW_unaryExpression_in_multiplyExpression2430);
            			    	unaryExpression196 = unaryExpression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, unaryExpression196.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:557:1: unaryExpression : (m= MINUS mu= unaryExpression -> ^( UNARY_MINUS[$m] $mu) | p= PLUS pu= unaryExpression -> ^( UNARY_PLUS[$p] $pu) | c= caseExpression -> ^( $c) | q= quantifiedExpression -> ^( $q) | a= atom -> ^( $a) );
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
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:558:2: (m= MINUS mu= unaryExpression -> ^( UNARY_MINUS[$m] $mu) | p= PLUS pu= unaryExpression -> ^( UNARY_PLUS[$p] $pu) | c= caseExpression -> ^( $c) | q= quantifiedExpression -> ^( $q) | a= atom -> ^( $a) )
            int alt69 = 5;
            switch ( input.LA(1) ) 
            {
            case MINUS:
            	{
                alt69 = 1;
                }
                break;
            case PLUS:
            	{
                alt69 = 2;
                }
                break;
            case CASE:
            	{
                alt69 = 3;
                }
                break;
            case ALL:
            case ANY:
            case EXISTS:
            case SOME:
            	{
                alt69 = 4;
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
            case NUM_FLOAT:
            case NUM_LONG:
            case OPEN:
            case COLON:
            case PARAM:
            case QUOTED_String:
            case IDENT:
            	{
                alt69 = 5;
                }
                break;
            	default:
            	    NoViableAltException nvae_d69s0 =
            	        new NoViableAltException("", 69, 0, input);

            	    throw nvae_d69s0;
            }

            switch (alt69) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:558:4: m= MINUS mu= unaryExpression
                    {
                    	m=(IToken)Match(input,MINUS,FOLLOW_MINUS_in_unaryExpression2448);  
                    	stream_MINUS.Add(m);

                    	PushFollow(FOLLOW_unaryExpression_in_unaryExpression2452);
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
                    	// 558:31: -> ^( UNARY_MINUS[$m] $mu)
                    	{
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:558:34: ^( UNARY_MINUS[$m] $mu)
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
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:559:4: p= PLUS pu= unaryExpression
                    {
                    	p=(IToken)Match(input,PLUS,FOLLOW_PLUS_in_unaryExpression2469);  
                    	stream_PLUS.Add(p);

                    	PushFollow(FOLLOW_unaryExpression_in_unaryExpression2473);
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
                    	// 559:30: -> ^( UNARY_PLUS[$p] $pu)
                    	{
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:559:33: ^( UNARY_PLUS[$p] $pu)
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
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:560:4: c= caseExpression
                    {
                    	PushFollow(FOLLOW_caseExpression_in_unaryExpression2490);
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
                    	// 560:21: -> ^( $c)
                    	{
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:560:24: ^( $c)
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
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:561:4: q= quantifiedExpression
                    {
                    	PushFollow(FOLLOW_quantifiedExpression_in_unaryExpression2504);
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
                    	// 561:27: -> ^( $q)
                    	{
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:561:30: ^( $q)
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
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:562:4: a= atom
                    {
                    	PushFollow(FOLLOW_atom_in_unaryExpression2519);
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
                    	// 562:11: -> ^( $a)
                    	{
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:562:14: ^( $a)
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:565:1: caseExpression : ( CASE ( whenClause )+ ( elseClause )? END -> ^( CASE whenClause ( elseClause )? ) | CASE unaryExpression ( altWhenClause )+ ( elseClause )? END -> ^( CASE2 unaryExpression ( altWhenClause )+ ( elseClause )? ) );
    public HqlParser.caseExpression_return caseExpression() // throws RecognitionException [1]
    {   
        HqlParser.caseExpression_return retval = new HqlParser.caseExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken CASE197 = null;
        IToken END200 = null;
        IToken CASE201 = null;
        IToken END205 = null;
        HqlParser.whenClause_return whenClause198 = default(HqlParser.whenClause_return);

        HqlParser.elseClause_return elseClause199 = default(HqlParser.elseClause_return);

        HqlParser.unaryExpression_return unaryExpression202 = default(HqlParser.unaryExpression_return);

        HqlParser.altWhenClause_return altWhenClause203 = default(HqlParser.altWhenClause_return);

        HqlParser.elseClause_return elseClause204 = default(HqlParser.elseClause_return);


        IASTNode CASE197_tree=null;
        IASTNode END200_tree=null;
        IASTNode CASE201_tree=null;
        IASTNode END205_tree=null;
        RewriteRuleTokenStream stream_CASE = new RewriteRuleTokenStream(adaptor,"token CASE");
        RewriteRuleTokenStream stream_END = new RewriteRuleTokenStream(adaptor,"token END");
        RewriteRuleSubtreeStream stream_unaryExpression = new RewriteRuleSubtreeStream(adaptor,"rule unaryExpression");
        RewriteRuleSubtreeStream stream_whenClause = new RewriteRuleSubtreeStream(adaptor,"rule whenClause");
        RewriteRuleSubtreeStream stream_elseClause = new RewriteRuleSubtreeStream(adaptor,"rule elseClause");
        RewriteRuleSubtreeStream stream_altWhenClause = new RewriteRuleSubtreeStream(adaptor,"rule altWhenClause");
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:566:2: ( CASE ( whenClause )+ ( elseClause )? END -> ^( CASE whenClause ( elseClause )? ) | CASE unaryExpression ( altWhenClause )+ ( elseClause )? END -> ^( CASE2 unaryExpression ( altWhenClause )+ ( elseClause )? ) )
            int alt74 = 2;
            int LA74_0 = input.LA(1);

            if ( (LA74_0 == CASE) )
            {
                int LA74_1 = input.LA(2);

                if ( ((LA74_1 >= ALL && LA74_1 <= ANY) || LA74_1 == AVG || LA74_1 == COUNT || LA74_1 == ELEMENTS || (LA74_1 >= EXISTS && LA74_1 <= FALSE) || LA74_1 == INDICES || (LA74_1 >= MAX && LA74_1 <= MIN) || LA74_1 == NULL || (LA74_1 >= SOME && LA74_1 <= TRUE) || LA74_1 == CASE || LA74_1 == EMPTY || (LA74_1 >= NUM_INT && LA74_1 <= NUM_LONG) || LA74_1 == OPEN || (LA74_1 >= PLUS && LA74_1 <= MINUS) || (LA74_1 >= COLON && LA74_1 <= IDENT)) )
                {
                    alt74 = 2;
                }
                else if ( (LA74_1 == WHEN) )
                {
                    alt74 = 1;
                }
                else 
                {
                    NoViableAltException nvae_d74s1 =
                        new NoViableAltException("", 74, 1, input);

                    throw nvae_d74s1;
                }
            }
            else 
            {
                NoViableAltException nvae_d74s0 =
                    new NoViableAltException("", 74, 0, input);

                throw nvae_d74s0;
            }
            switch (alt74) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:566:4: CASE ( whenClause )+ ( elseClause )? END
                    {
                    	CASE197=(IToken)Match(input,CASE,FOLLOW_CASE_in_caseExpression2538);  
                    	stream_CASE.Add(CASE197);

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:566:9: ( whenClause )+
                    	int cnt70 = 0;
                    	do 
                    	{
                    	    int alt70 = 2;
                    	    int LA70_0 = input.LA(1);

                    	    if ( (LA70_0 == WHEN) )
                    	    {
                    	        alt70 = 1;
                    	    }


                    	    switch (alt70) 
                    		{
                    			case 1 :
                    			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:566:10: whenClause
                    			    {
                    			    	PushFollow(FOLLOW_whenClause_in_caseExpression2541);
                    			    	whenClause198 = whenClause();
                    			    	state.followingStackPointer--;

                    			    	stream_whenClause.Add(whenClause198.Tree);

                    			    }
                    			    break;

                    			default:
                    			    if ( cnt70 >= 1 ) goto loop70;
                    		            EarlyExitException eee70 =
                    		                new EarlyExitException(70, input);
                    		            throw eee70;
                    	    }
                    	    cnt70++;
                    	} while (true);

                    	loop70:
                    		;	// Stops C# compiler whinging that label 'loop70' has no statements

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:566:23: ( elseClause )?
                    	int alt71 = 2;
                    	int LA71_0 = input.LA(1);

                    	if ( (LA71_0 == ELSE) )
                    	{
                    	    alt71 = 1;
                    	}
                    	switch (alt71) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:566:24: elseClause
                    	        {
                    	        	PushFollow(FOLLOW_elseClause_in_caseExpression2546);
                    	        	elseClause199 = elseClause();
                    	        	state.followingStackPointer--;

                    	        	stream_elseClause.Add(elseClause199.Tree);

                    	        }
                    	        break;

                    	}

                    	END200=(IToken)Match(input,END,FOLLOW_END_in_caseExpression2550);  
                    	stream_END.Add(END200);



                    	// AST REWRITE
                    	// elements:          elseClause, whenClause, CASE
                    	// token labels:      
                    	// rule labels:       retval
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

                    	root_0 = (IASTNode)adaptor.GetNilNode();
                    	// 567:3: -> ^( CASE whenClause ( elseClause )? )
                    	{
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:567:6: ^( CASE whenClause ( elseClause )? )
                    	    {
                    	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
                    	    root_1 = (IASTNode)adaptor.BecomeRoot(stream_CASE.NextNode(), root_1);

                    	    adaptor.AddChild(root_1, stream_whenClause.NextTree());
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:567:24: ( elseClause )?
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
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:568:4: CASE unaryExpression ( altWhenClause )+ ( elseClause )? END
                    {
                    	CASE201=(IToken)Match(input,CASE,FOLLOW_CASE_in_caseExpression2569);  
                    	stream_CASE.Add(CASE201);

                    	PushFollow(FOLLOW_unaryExpression_in_caseExpression2571);
                    	unaryExpression202 = unaryExpression();
                    	state.followingStackPointer--;

                    	stream_unaryExpression.Add(unaryExpression202.Tree);
                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:568:25: ( altWhenClause )+
                    	int cnt72 = 0;
                    	do 
                    	{
                    	    int alt72 = 2;
                    	    int LA72_0 = input.LA(1);

                    	    if ( (LA72_0 == WHEN) )
                    	    {
                    	        alt72 = 1;
                    	    }


                    	    switch (alt72) 
                    		{
                    			case 1 :
                    			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:568:26: altWhenClause
                    			    {
                    			    	PushFollow(FOLLOW_altWhenClause_in_caseExpression2574);
                    			    	altWhenClause203 = altWhenClause();
                    			    	state.followingStackPointer--;

                    			    	stream_altWhenClause.Add(altWhenClause203.Tree);

                    			    }
                    			    break;

                    			default:
                    			    if ( cnt72 >= 1 ) goto loop72;
                    		            EarlyExitException eee72 =
                    		                new EarlyExitException(72, input);
                    		            throw eee72;
                    	    }
                    	    cnt72++;
                    	} while (true);

                    	loop72:
                    		;	// Stops C# compiler whinging that label 'loop72' has no statements

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:568:42: ( elseClause )?
                    	int alt73 = 2;
                    	int LA73_0 = input.LA(1);

                    	if ( (LA73_0 == ELSE) )
                    	{
                    	    alt73 = 1;
                    	}
                    	switch (alt73) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:568:43: elseClause
                    	        {
                    	        	PushFollow(FOLLOW_elseClause_in_caseExpression2579);
                    	        	elseClause204 = elseClause();
                    	        	state.followingStackPointer--;

                    	        	stream_elseClause.Add(elseClause204.Tree);

                    	        }
                    	        break;

                    	}

                    	END205=(IToken)Match(input,END,FOLLOW_END_in_caseExpression2583);  
                    	stream_END.Add(END205);



                    	// AST REWRITE
                    	// elements:          unaryExpression, altWhenClause, elseClause
                    	// token labels:      
                    	// rule labels:       retval
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

                    	root_0 = (IASTNode)adaptor.GetNilNode();
                    	// 569:3: -> ^( CASE2 unaryExpression ( altWhenClause )+ ( elseClause )? )
                    	{
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:569:6: ^( CASE2 unaryExpression ( altWhenClause )+ ( elseClause )? )
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
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:569:45: ( elseClause )?
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:572:1: whenClause : ( WHEN logicalExpression THEN unaryExpression ) ;
    public HqlParser.whenClause_return whenClause() // throws RecognitionException [1]
    {   
        HqlParser.whenClause_return retval = new HqlParser.whenClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken WHEN206 = null;
        IToken THEN208 = null;
        HqlParser.logicalExpression_return logicalExpression207 = default(HqlParser.logicalExpression_return);

        HqlParser.unaryExpression_return unaryExpression209 = default(HqlParser.unaryExpression_return);


        IASTNode WHEN206_tree=null;
        IASTNode THEN208_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:573:2: ( ( WHEN logicalExpression THEN unaryExpression ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:573:4: ( WHEN logicalExpression THEN unaryExpression )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:573:4: ( WHEN logicalExpression THEN unaryExpression )
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:573:5: WHEN logicalExpression THEN unaryExpression
            	{
            		WHEN206=(IToken)Match(input,WHEN,FOLLOW_WHEN_in_whenClause2612); 
            			WHEN206_tree = (IASTNode)adaptor.Create(WHEN206);
            			root_0 = (IASTNode)adaptor.BecomeRoot(WHEN206_tree, root_0);

            		PushFollow(FOLLOW_logicalExpression_in_whenClause2615);
            		logicalExpression207 = logicalExpression();
            		state.followingStackPointer--;

            		adaptor.AddChild(root_0, logicalExpression207.Tree);
            		THEN208=(IToken)Match(input,THEN,FOLLOW_THEN_in_whenClause2617); 
            		PushFollow(FOLLOW_unaryExpression_in_whenClause2620);
            		unaryExpression209 = unaryExpression();
            		state.followingStackPointer--;

            		adaptor.AddChild(root_0, unaryExpression209.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:576:1: altWhenClause : ( WHEN unaryExpression THEN unaryExpression ) ;
    public HqlParser.altWhenClause_return altWhenClause() // throws RecognitionException [1]
    {   
        HqlParser.altWhenClause_return retval = new HqlParser.altWhenClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken WHEN210 = null;
        IToken THEN212 = null;
        HqlParser.unaryExpression_return unaryExpression211 = default(HqlParser.unaryExpression_return);

        HqlParser.unaryExpression_return unaryExpression213 = default(HqlParser.unaryExpression_return);


        IASTNode WHEN210_tree=null;
        IASTNode THEN212_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:577:2: ( ( WHEN unaryExpression THEN unaryExpression ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:577:4: ( WHEN unaryExpression THEN unaryExpression )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:577:4: ( WHEN unaryExpression THEN unaryExpression )
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:577:5: WHEN unaryExpression THEN unaryExpression
            	{
            		WHEN210=(IToken)Match(input,WHEN,FOLLOW_WHEN_in_altWhenClause2634); 
            			WHEN210_tree = (IASTNode)adaptor.Create(WHEN210);
            			root_0 = (IASTNode)adaptor.BecomeRoot(WHEN210_tree, root_0);

            		PushFollow(FOLLOW_unaryExpression_in_altWhenClause2637);
            		unaryExpression211 = unaryExpression();
            		state.followingStackPointer--;

            		adaptor.AddChild(root_0, unaryExpression211.Tree);
            		THEN212=(IToken)Match(input,THEN,FOLLOW_THEN_in_altWhenClause2639); 
            		PushFollow(FOLLOW_unaryExpression_in_altWhenClause2642);
            		unaryExpression213 = unaryExpression();
            		state.followingStackPointer--;

            		adaptor.AddChild(root_0, unaryExpression213.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:580:1: elseClause : ( ELSE unaryExpression ) ;
    public HqlParser.elseClause_return elseClause() // throws RecognitionException [1]
    {   
        HqlParser.elseClause_return retval = new HqlParser.elseClause_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken ELSE214 = null;
        HqlParser.unaryExpression_return unaryExpression215 = default(HqlParser.unaryExpression_return);


        IASTNode ELSE214_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:581:2: ( ( ELSE unaryExpression ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:581:4: ( ELSE unaryExpression )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:581:4: ( ELSE unaryExpression )
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:581:5: ELSE unaryExpression
            	{
            		ELSE214=(IToken)Match(input,ELSE,FOLLOW_ELSE_in_elseClause2656); 
            			ELSE214_tree = (IASTNode)adaptor.Create(ELSE214);
            			root_0 = (IASTNode)adaptor.BecomeRoot(ELSE214_tree, root_0);

            		PushFollow(FOLLOW_unaryExpression_in_elseClause2659);
            		unaryExpression215 = unaryExpression();
            		state.followingStackPointer--;

            		adaptor.AddChild(root_0, unaryExpression215.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:584:1: quantifiedExpression : ( SOME | EXISTS | ALL | ANY ) ( identifier | collectionExpr | ( OPEN ( subQuery ) CLOSE ) ) ;
    public HqlParser.quantifiedExpression_return quantifiedExpression() // throws RecognitionException [1]
    {   
        HqlParser.quantifiedExpression_return retval = new HqlParser.quantifiedExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken SOME216 = null;
        IToken EXISTS217 = null;
        IToken ALL218 = null;
        IToken ANY219 = null;
        IToken OPEN222 = null;
        IToken CLOSE224 = null;
        HqlParser.identifier_return identifier220 = default(HqlParser.identifier_return);

        HqlParser.collectionExpr_return collectionExpr221 = default(HqlParser.collectionExpr_return);

        HqlParser.subQuery_return subQuery223 = default(HqlParser.subQuery_return);


        IASTNode SOME216_tree=null;
        IASTNode EXISTS217_tree=null;
        IASTNode ALL218_tree=null;
        IASTNode ANY219_tree=null;
        IASTNode OPEN222_tree=null;
        IASTNode CLOSE224_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:585:2: ( ( SOME | EXISTS | ALL | ANY ) ( identifier | collectionExpr | ( OPEN ( subQuery ) CLOSE ) ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:585:4: ( SOME | EXISTS | ALL | ANY ) ( identifier | collectionExpr | ( OPEN ( subQuery ) CLOSE ) )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:585:4: ( SOME | EXISTS | ALL | ANY )
            	int alt75 = 4;
            	switch ( input.LA(1) ) 
            	{
            	case SOME:
            		{
            	    alt75 = 1;
            	    }
            	    break;
            	case EXISTS:
            		{
            	    alt75 = 2;
            	    }
            	    break;
            	case ALL:
            		{
            	    alt75 = 3;
            	    }
            	    break;
            	case ANY:
            		{
            	    alt75 = 4;
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
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:585:6: SOME
            	        {
            	        	SOME216=(IToken)Match(input,SOME,FOLLOW_SOME_in_quantifiedExpression2674); 
            	        		SOME216_tree = (IASTNode)adaptor.Create(SOME216);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(SOME216_tree, root_0);


            	        }
            	        break;
            	    case 2 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:585:14: EXISTS
            	        {
            	        	EXISTS217=(IToken)Match(input,EXISTS,FOLLOW_EXISTS_in_quantifiedExpression2679); 
            	        		EXISTS217_tree = (IASTNode)adaptor.Create(EXISTS217);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(EXISTS217_tree, root_0);


            	        }
            	        break;
            	    case 3 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:585:24: ALL
            	        {
            	        	ALL218=(IToken)Match(input,ALL,FOLLOW_ALL_in_quantifiedExpression2684); 
            	        		ALL218_tree = (IASTNode)adaptor.Create(ALL218);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(ALL218_tree, root_0);


            	        }
            	        break;
            	    case 4 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:585:31: ANY
            	        {
            	        	ANY219=(IToken)Match(input,ANY,FOLLOW_ANY_in_quantifiedExpression2689); 
            	        		ANY219_tree = (IASTNode)adaptor.Create(ANY219);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(ANY219_tree, root_0);


            	        }
            	        break;

            	}

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:586:2: ( identifier | collectionExpr | ( OPEN ( subQuery ) CLOSE ) )
            	int alt76 = 3;
            	switch ( input.LA(1) ) 
            	{
            	case IDENT:
            		{
            	    alt76 = 1;
            	    }
            	    break;
            	case ELEMENTS:
            	case INDICES:
            		{
            	    alt76 = 2;
            	    }
            	    break;
            	case OPEN:
            		{
            	    alt76 = 3;
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
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:586:4: identifier
            	        {
            	        	PushFollow(FOLLOW_identifier_in_quantifiedExpression2698);
            	        	identifier220 = identifier();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, identifier220.Tree);

            	        }
            	        break;
            	    case 2 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:586:17: collectionExpr
            	        {
            	        	PushFollow(FOLLOW_collectionExpr_in_quantifiedExpression2702);
            	        	collectionExpr221 = collectionExpr();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, collectionExpr221.Tree);

            	        }
            	        break;
            	    case 3 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:586:34: ( OPEN ( subQuery ) CLOSE )
            	        {
            	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:586:34: ( OPEN ( subQuery ) CLOSE )
            	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:586:35: OPEN ( subQuery ) CLOSE
            	        	{
            	        		OPEN222=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_quantifiedExpression2707); 
            	        		// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:586:41: ( subQuery )
            	        		// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:586:43: subQuery
            	        		{
            	        			PushFollow(FOLLOW_subQuery_in_quantifiedExpression2712);
            	        			subQuery223 = subQuery();
            	        			state.followingStackPointer--;

            	        			adaptor.AddChild(root_0, subQuery223.Tree);

            	        		}

            	        		CLOSE224=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_quantifiedExpression2716); 

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:592:1: atom : primaryExpression ( DOT identifier ( options {greedy=true; } : (op= OPEN exprList CLOSE ) )? | lb= OPEN_BRACKET expression CLOSE_BRACKET )* ;
    public HqlParser.atom_return atom() // throws RecognitionException [1]
    {   
        HqlParser.atom_return retval = new HqlParser.atom_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken op = null;
        IToken lb = null;
        IToken DOT226 = null;
        IToken CLOSE229 = null;
        IToken CLOSE_BRACKET231 = null;
        HqlParser.primaryExpression_return primaryExpression225 = default(HqlParser.primaryExpression_return);

        HqlParser.identifier_return identifier227 = default(HqlParser.identifier_return);

        HqlParser.exprList_return exprList228 = default(HqlParser.exprList_return);

        HqlParser.expression_return expression230 = default(HqlParser.expression_return);


        IASTNode op_tree=null;
        IASTNode lb_tree=null;
        IASTNode DOT226_tree=null;
        IASTNode CLOSE229_tree=null;
        IASTNode CLOSE_BRACKET231_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:593:3: ( primaryExpression ( DOT identifier ( options {greedy=true; } : (op= OPEN exprList CLOSE ) )? | lb= OPEN_BRACKET expression CLOSE_BRACKET )* )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:593:5: primaryExpression ( DOT identifier ( options {greedy=true; } : (op= OPEN exprList CLOSE ) )? | lb= OPEN_BRACKET expression CLOSE_BRACKET )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_primaryExpression_in_atom2735);
            	primaryExpression225 = primaryExpression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, primaryExpression225.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:594:3: ( DOT identifier ( options {greedy=true; } : (op= OPEN exprList CLOSE ) )? | lb= OPEN_BRACKET expression CLOSE_BRACKET )*
            	do 
            	{
            	    int alt78 = 3;
            	    int LA78_0 = input.LA(1);

            	    if ( (LA78_0 == DOT) )
            	    {
            	        alt78 = 1;
            	    }
            	    else if ( (LA78_0 == OPEN_BRACKET) )
            	    {
            	        alt78 = 2;
            	    }


            	    switch (alt78) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:595:4: DOT identifier ( options {greedy=true; } : (op= OPEN exprList CLOSE ) )?
            			    {
            			    	DOT226=(IToken)Match(input,DOT,FOLLOW_DOT_in_atom2744); 
            			    		DOT226_tree = (IASTNode)adaptor.Create(DOT226);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(DOT226_tree, root_0);

            			    	PushFollow(FOLLOW_identifier_in_atom2747);
            			    	identifier227 = identifier();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, identifier227.Tree);
            			    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:596:5: ( options {greedy=true; } : (op= OPEN exprList CLOSE ) )?
            			    	int alt77 = 2;
            			    	int LA77_0 = input.LA(1);

            			    	if ( (LA77_0 == OPEN) )
            			    	{
            			    	    alt77 = 1;
            			    	}
            			    	switch (alt77) 
            			    	{
            			    	    case 1 :
            			    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:597:6: (op= OPEN exprList CLOSE )
            			    	        {
            			    	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:597:6: (op= OPEN exprList CLOSE )
            			    	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:597:8: op= OPEN exprList CLOSE
            			    	        	{
            			    	        		op=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_atom2775); 
            			    	        			op_tree = (IASTNode)adaptor.Create(op);
            			    	        			root_0 = (IASTNode)adaptor.BecomeRoot(op_tree, root_0);

            			    	        		op.Type = METHOD_CALL; 
            			    	        		PushFollow(FOLLOW_exprList_in_atom2780);
            			    	        		exprList228 = exprList();
            			    	        		state.followingStackPointer--;

            			    	        		adaptor.AddChild(root_0, exprList228.Tree);
            			    	        		CLOSE229=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_atom2782); 

            			    	        	}


            			    	        }
            			    	        break;

            			    	}


            			    }
            			    break;
            			case 2 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:598:5: lb= OPEN_BRACKET expression CLOSE_BRACKET
            			    {
            			    	lb=(IToken)Match(input,OPEN_BRACKET,FOLLOW_OPEN_BRACKET_in_atom2796); 
            			    		lb_tree = (IASTNode)adaptor.Create(lb);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(lb_tree, root_0);

            			    	lb.Type = INDEX_OP; 
            			    	PushFollow(FOLLOW_expression_in_atom2801);
            			    	expression230 = expression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, expression230.Tree);
            			    	CLOSE_BRACKET231=(IToken)Match(input,CLOSE_BRACKET,FOLLOW_CLOSE_BRACKET_in_atom2803); 

            			    }
            			    break;

            			default:
            			    goto loop78;
            	    }
            	} while (true);

            	loop78:
            		;	// Stops C# compiler whining that label 'loop78' has no statements


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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:603:1: primaryExpression : ( identPrimary ( options {greedy=true; } : DOT 'class' )? | constant | COLON identifier | OPEN ( expressionOrVector | subQuery ) CLOSE | PARAM ( NUM_INT )? );
    public HqlParser.primaryExpression_return primaryExpression() // throws RecognitionException [1]
    {   
        HqlParser.primaryExpression_return retval = new HqlParser.primaryExpression_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken DOT233 = null;
        IToken string_literal234 = null;
        IToken COLON236 = null;
        IToken OPEN238 = null;
        IToken CLOSE241 = null;
        IToken PARAM242 = null;
        IToken NUM_INT243 = null;
        HqlParser.identPrimary_return identPrimary232 = default(HqlParser.identPrimary_return);

        HqlParser.constant_return constant235 = default(HqlParser.constant_return);

        HqlParser.identifier_return identifier237 = default(HqlParser.identifier_return);

        HqlParser.expressionOrVector_return expressionOrVector239 = default(HqlParser.expressionOrVector_return);

        HqlParser.subQuery_return subQuery240 = default(HqlParser.subQuery_return);


        IASTNode DOT233_tree=null;
        IASTNode string_literal234_tree=null;
        IASTNode COLON236_tree=null;
        IASTNode OPEN238_tree=null;
        IASTNode CLOSE241_tree=null;
        IASTNode PARAM242_tree=null;
        IASTNode NUM_INT243_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:604:2: ( identPrimary ( options {greedy=true; } : DOT 'class' )? | constant | COLON identifier | OPEN ( expressionOrVector | subQuery ) CLOSE | PARAM ( NUM_INT )? )
            int alt82 = 5;
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
                alt82 = 1;
                }
                break;
            case FALSE:
            case NULL:
            case TRUE:
            case EMPTY:
            case NUM_INT:
            case NUM_DOUBLE:
            case NUM_FLOAT:
            case NUM_LONG:
            case QUOTED_String:
            	{
                alt82 = 2;
                }
                break;
            case COLON:
            	{
                alt82 = 3;
                }
                break;
            case OPEN:
            	{
                alt82 = 4;
                }
                break;
            case PARAM:
            	{
                alt82 = 5;
                }
                break;
            	default:
            	    NoViableAltException nvae_d82s0 =
            	        new NoViableAltException("", 82, 0, input);

            	    throw nvae_d82s0;
            }

            switch (alt82) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:604:6: identPrimary ( options {greedy=true; } : DOT 'class' )?
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_identPrimary_in_primaryExpression2823);
                    	identPrimary232 = identPrimary();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, identPrimary232.Tree);
                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:604:19: ( options {greedy=true; } : DOT 'class' )?
                    	int alt79 = 2;
                    	int LA79_0 = input.LA(1);

                    	if ( (LA79_0 == DOT) )
                    	{
                    	    int LA79_1 = input.LA(2);

                    	    if ( (LA79_1 == CLASS) )
                    	    {
                    	        alt79 = 1;
                    	    }
                    	}
                    	switch (alt79) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:604:46: DOT 'class'
                    	        {
                    	        	DOT233=(IToken)Match(input,DOT,FOLLOW_DOT_in_primaryExpression2836); 
                    	        		DOT233_tree = (IASTNode)adaptor.Create(DOT233);
                    	        		root_0 = (IASTNode)adaptor.BecomeRoot(DOT233_tree, root_0);

                    	        	string_literal234=(IToken)Match(input,CLASS,FOLLOW_CLASS_in_primaryExpression2839); 
                    	        		string_literal234_tree = (IASTNode)adaptor.Create(string_literal234);
                    	        		adaptor.AddChild(root_0, string_literal234_tree);


                    	        }
                    	        break;

                    	}


                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:605:6: constant
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_constant_in_primaryExpression2849);
                    	constant235 = constant();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, constant235.Tree);

                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:606:6: COLON identifier
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	COLON236=(IToken)Match(input,COLON,FOLLOW_COLON_in_primaryExpression2856); 
                    		COLON236_tree = (IASTNode)adaptor.Create(COLON236);
                    		root_0 = (IASTNode)adaptor.BecomeRoot(COLON236_tree, root_0);

                    	PushFollow(FOLLOW_identifier_in_primaryExpression2859);
                    	identifier237 = identifier();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, identifier237.Tree);

                    }
                    break;
                case 4 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:608:6: OPEN ( expressionOrVector | subQuery ) CLOSE
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	OPEN238=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_primaryExpression2868); 
                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:608:12: ( expressionOrVector | subQuery )
                    	int alt80 = 2;
                    	int LA80_0 = input.LA(1);

                    	if ( ((LA80_0 >= ALL && LA80_0 <= ANY) || LA80_0 == AVG || LA80_0 == COUNT || LA80_0 == ELEMENTS || (LA80_0 >= EXISTS && LA80_0 <= FALSE) || LA80_0 == INDICES || (LA80_0 >= MAX && LA80_0 <= MIN) || (LA80_0 >= NOT && LA80_0 <= NULL) || (LA80_0 >= SOME && LA80_0 <= TRUE) || LA80_0 == CASE || LA80_0 == EMPTY || (LA80_0 >= NUM_INT && LA80_0 <= NUM_LONG) || LA80_0 == OPEN || LA80_0 == BNOT || (LA80_0 >= PLUS && LA80_0 <= MINUS) || (LA80_0 >= COLON && LA80_0 <= IDENT)) )
                    	{
                    	    alt80 = 1;
                    	}
                    	else if ( (LA80_0 == EOF || LA80_0 == FROM || LA80_0 == GROUP || LA80_0 == ORDER || LA80_0 == SELECT || LA80_0 == UNION || LA80_0 == WHERE || LA80_0 == CLOSE) )
                    	{
                    	    alt80 = 2;
                    	}
                    	else 
                    	{
                    	    NoViableAltException nvae_d80s0 =
                    	        new NoViableAltException("", 80, 0, input);

                    	    throw nvae_d80s0;
                    	}
                    	switch (alt80) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:608:13: expressionOrVector
                    	        {
                    	        	PushFollow(FOLLOW_expressionOrVector_in_primaryExpression2872);
                    	        	expressionOrVector239 = expressionOrVector();
                    	        	state.followingStackPointer--;

                    	        	adaptor.AddChild(root_0, expressionOrVector239.Tree);

                    	        }
                    	        break;
                    	    case 2 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:608:34: subQuery
                    	        {
                    	        	PushFollow(FOLLOW_subQuery_in_primaryExpression2876);
                    	        	subQuery240 = subQuery();
                    	        	state.followingStackPointer--;

                    	        	adaptor.AddChild(root_0, subQuery240.Tree);

                    	        }
                    	        break;

                    	}

                    	CLOSE241=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_primaryExpression2879); 

                    }
                    break;
                case 5 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:609:6: PARAM ( NUM_INT )?
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PARAM242=(IToken)Match(input,PARAM,FOLLOW_PARAM_in_primaryExpression2887); 
                    		PARAM242_tree = (IASTNode)adaptor.Create(PARAM242);
                    		root_0 = (IASTNode)adaptor.BecomeRoot(PARAM242_tree, root_0);

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:609:13: ( NUM_INT )?
                    	int alt81 = 2;
                    	int LA81_0 = input.LA(1);

                    	if ( (LA81_0 == NUM_INT) )
                    	{
                    	    alt81 = 1;
                    	}
                    	switch (alt81) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:609:14: NUM_INT
                    	        {
                    	        	NUM_INT243=(IToken)Match(input,NUM_INT,FOLLOW_NUM_INT_in_primaryExpression2891); 
                    	        		NUM_INT243_tree = (IASTNode)adaptor.Create(NUM_INT243);
                    	        		adaptor.AddChild(root_0, NUM_INT243_tree);


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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:614:1: expressionOrVector : e= expression (v= vectorExpr )? -> {v != null}? ^( VECTOR_EXPR[\"{vector}\"] $e $v) -> ^( $e) ;
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
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:615:2: (e= expression (v= vectorExpr )? -> {v != null}? ^( VECTOR_EXPR[\"{vector}\"] $e $v) -> ^( $e) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:615:4: e= expression (v= vectorExpr )?
            {
            	PushFollow(FOLLOW_expression_in_expressionOrVector2909);
            	e = expression();
            	state.followingStackPointer--;

            	stream_expression.Add(e.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:615:17: (v= vectorExpr )?
            	int alt83 = 2;
            	int LA83_0 = input.LA(1);

            	if ( (LA83_0 == COMMA) )
            	{
            	    alt83 = 1;
            	}
            	switch (alt83) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:615:19: v= vectorExpr
            	        {
            	        	PushFollow(FOLLOW_vectorExpr_in_expressionOrVector2915);
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
            	// 616:2: -> {v != null}? ^( VECTOR_EXPR[\"{vector}\"] $e $v)
            	if (v != null)
            	{
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:616:18: ^( VECTOR_EXPR[\"{vector}\"] $e $v)
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(VECTOR_EXPR, "{vector}"), root_1);

            	    adaptor.AddChild(root_1, stream_e.NextTree());
            	    adaptor.AddChild(root_1, stream_v.NextTree());

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}
            	else // 617:2: -> ^( $e)
            	{
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:617:5: ^( $e)
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:620:1: vectorExpr : COMMA expression ( COMMA expression )* ;
    public HqlParser.vectorExpr_return vectorExpr() // throws RecognitionException [1]
    {   
        HqlParser.vectorExpr_return retval = new HqlParser.vectorExpr_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken COMMA244 = null;
        IToken COMMA246 = null;
        HqlParser.expression_return expression245 = default(HqlParser.expression_return);

        HqlParser.expression_return expression247 = default(HqlParser.expression_return);


        IASTNode COMMA244_tree=null;
        IASTNode COMMA246_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:621:2: ( COMMA expression ( COMMA expression )* )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:621:4: COMMA expression ( COMMA expression )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	COMMA244=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_vectorExpr2954); 
            	PushFollow(FOLLOW_expression_in_vectorExpr2957);
            	expression245 = expression();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, expression245.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:621:22: ( COMMA expression )*
            	do 
            	{
            	    int alt84 = 2;
            	    int LA84_0 = input.LA(1);

            	    if ( (LA84_0 == COMMA) )
            	    {
            	        alt84 = 1;
            	    }


            	    switch (alt84) 
            		{
            			case 1 :
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:621:23: COMMA expression
            			    {
            			    	COMMA246=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_vectorExpr2960); 
            			    	PushFollow(FOLLOW_expression_in_vectorExpr2963);
            			    	expression247 = expression();
            			    	state.followingStackPointer--;

            			    	adaptor.AddChild(root_0, expression247.Tree);

            			    }
            			    break;

            			default:
            			    goto loop84;
            	    }
            	} while (true);

            	loop84:
            		;	// Stops C# compiler whining that label 'loop84' has no statements


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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:627:1: identPrimary : ( identifier ( options {greedy=true; } : DOT ( identifier | o= OBJECT ) )* ( (op= OPEN exprList CLOSE ) )? | aggregate );
    public HqlParser.identPrimary_return identPrimary() // throws RecognitionException [1]
    {   
        HqlParser.identPrimary_return retval = new HqlParser.identPrimary_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken o = null;
        IToken op = null;
        IToken DOT249 = null;
        IToken CLOSE252 = null;
        HqlParser.identifier_return identifier248 = default(HqlParser.identifier_return);

        HqlParser.identifier_return identifier250 = default(HqlParser.identifier_return);

        HqlParser.exprList_return exprList251 = default(HqlParser.exprList_return);

        HqlParser.aggregate_return aggregate253 = default(HqlParser.aggregate_return);


        IASTNode o_tree=null;
        IASTNode op_tree=null;
        IASTNode DOT249_tree=null;
        IASTNode CLOSE252_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:628:2: ( identifier ( options {greedy=true; } : DOT ( identifier | o= OBJECT ) )* ( (op= OPEN exprList CLOSE ) )? | aggregate )
            int alt88 = 2;
            int LA88_0 = input.LA(1);

            if ( (LA88_0 == IDENT) )
            {
                alt88 = 1;
            }
            else if ( (LA88_0 == AVG || LA88_0 == COUNT || LA88_0 == ELEMENTS || LA88_0 == INDICES || (LA88_0 >= MAX && LA88_0 <= MIN) || LA88_0 == SUM) )
            {
                alt88 = 2;
            }
            else 
            {
                NoViableAltException nvae_d88s0 =
                    new NoViableAltException("", 88, 0, input);

                throw nvae_d88s0;
            }
            switch (alt88) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:628:4: identifier ( options {greedy=true; } : DOT ( identifier | o= OBJECT ) )* ( (op= OPEN exprList CLOSE ) )?
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_identifier_in_identPrimary2979);
                    	identifier248 = identifier();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, identifier248.Tree);
                    	 HandleDotIdent(); 
                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:629:4: ( options {greedy=true; } : DOT ( identifier | o= OBJECT ) )*
                    	do 
                    	{
                    	    int alt86 = 2;
                    	    int LA86_0 = input.LA(1);

                    	    if ( (LA86_0 == DOT) )
                    	    {
                    	        int LA86_2 = input.LA(2);

                    	        if ( (LA86_2 == OBJECT || LA86_2 == IDENT) )
                    	        {
                    	            alt86 = 1;
                    	        }


                    	    }


                    	    switch (alt86) 
                    		{
                    			case 1 :
                    			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:629:31: DOT ( identifier | o= OBJECT )
                    			    {
                    			    	DOT249=(IToken)Match(input,DOT,FOLLOW_DOT_in_identPrimary2997); 
                    			    		DOT249_tree = (IASTNode)adaptor.Create(DOT249);
                    			    		root_0 = (IASTNode)adaptor.BecomeRoot(DOT249_tree, root_0);

                    			    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:629:36: ( identifier | o= OBJECT )
                    			    	int alt85 = 2;
                    			    	int LA85_0 = input.LA(1);

                    			    	if ( (LA85_0 == IDENT) )
                    			    	{
                    			    	    alt85 = 1;
                    			    	}
                    			    	else if ( (LA85_0 == OBJECT) )
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
                    			    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:629:38: identifier
                    			    	        {
                    			    	        	PushFollow(FOLLOW_identifier_in_identPrimary3002);
                    			    	        	identifier250 = identifier();
                    			    	        	state.followingStackPointer--;

                    			    	        	adaptor.AddChild(root_0, identifier250.Tree);

                    			    	        }
                    			    	        break;
                    			    	    case 2 :
                    			    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:629:51: o= OBJECT
                    			    	        {
                    			    	        	o=(IToken)Match(input,OBJECT,FOLLOW_OBJECT_in_identPrimary3008); 
                    			    	        		o_tree = (IASTNode)adaptor.Create(o);
                    			    	        		adaptor.AddChild(root_0, o_tree);

                    			    	        	 o.Type = IDENT; 

                    			    	        }
                    			    	        break;

                    			    	}


                    			    }
                    			    break;

                    			default:
                    			    goto loop86;
                    	    }
                    	} while (true);

                    	loop86:
                    		;	// Stops C# compiler whining that label 'loop86' has no statements

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:630:4: ( (op= OPEN exprList CLOSE ) )?
                    	int alt87 = 2;
                    	int LA87_0 = input.LA(1);

                    	if ( (LA87_0 == OPEN) )
                    	{
                    	    alt87 = 1;
                    	}
                    	switch (alt87) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:630:6: (op= OPEN exprList CLOSE )
                    	        {
                    	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:630:6: (op= OPEN exprList CLOSE )
                    	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:630:8: op= OPEN exprList CLOSE
                    	        	{
                    	        		op=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_identPrimary3026); 
                    	        			op_tree = (IASTNode)adaptor.Create(op);
                    	        			root_0 = (IASTNode)adaptor.BecomeRoot(op_tree, root_0);

                    	        		 op.Type = METHOD_CALL;
                    	        		PushFollow(FOLLOW_exprList_in_identPrimary3031);
                    	        		exprList251 = exprList();
                    	        		state.followingStackPointer--;

                    	        		adaptor.AddChild(root_0, exprList251.Tree);
                    	        		CLOSE252=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_identPrimary3033); 

                    	        	}


                    	        }
                    	        break;

                    	}


                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:633:4: aggregate
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_aggregate_in_identPrimary3049);
                    	aggregate253 = aggregate();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, aggregate253.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:641:1: aggregate : ( (op= SUM | op= AVG | op= MAX | op= MIN ) OPEN additiveExpression CLOSE -> ^( AGGREGATE[$op] additiveExpression ) | COUNT OPEN (s= STAR | p= aggregateDistinctAll ) CLOSE -> {s == null}? ^( COUNT $p) -> ^( COUNT ^( ROW_STAR[\"*\"] ) ) | collectionExpr );
    public HqlParser.aggregate_return aggregate() // throws RecognitionException [1]
    {   
        HqlParser.aggregate_return retval = new HqlParser.aggregate_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken op = null;
        IToken s = null;
        IToken OPEN254 = null;
        IToken CLOSE256 = null;
        IToken COUNT257 = null;
        IToken OPEN258 = null;
        IToken CLOSE259 = null;
        HqlParser.aggregateDistinctAll_return p = default(HqlParser.aggregateDistinctAll_return);

        HqlParser.additiveExpression_return additiveExpression255 = default(HqlParser.additiveExpression_return);

        HqlParser.collectionExpr_return collectionExpr260 = default(HqlParser.collectionExpr_return);


        IASTNode op_tree=null;
        IASTNode s_tree=null;
        IASTNode OPEN254_tree=null;
        IASTNode CLOSE256_tree=null;
        IASTNode COUNT257_tree=null;
        IASTNode OPEN258_tree=null;
        IASTNode CLOSE259_tree=null;
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
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:642:2: ( (op= SUM | op= AVG | op= MAX | op= MIN ) OPEN additiveExpression CLOSE -> ^( AGGREGATE[$op] additiveExpression ) | COUNT OPEN (s= STAR | p= aggregateDistinctAll ) CLOSE -> {s == null}? ^( COUNT $p) -> ^( COUNT ^( ROW_STAR[\"*\"] ) ) | collectionExpr )
            int alt91 = 3;
            switch ( input.LA(1) ) 
            {
            case AVG:
            case MAX:
            case MIN:
            case SUM:
            	{
                alt91 = 1;
                }
                break;
            case COUNT:
            	{
                alt91 = 2;
                }
                break;
            case ELEMENTS:
            case INDICES:
            	{
                alt91 = 3;
                }
                break;
            	default:
            	    NoViableAltException nvae_d91s0 =
            	        new NoViableAltException("", 91, 0, input);

            	    throw nvae_d91s0;
            }

            switch (alt91) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:642:4: (op= SUM | op= AVG | op= MAX | op= MIN ) OPEN additiveExpression CLOSE
                    {
                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:642:4: (op= SUM | op= AVG | op= MAX | op= MIN )
                    	int alt89 = 4;
                    	switch ( input.LA(1) ) 
                    	{
                    	case SUM:
                    		{
                    	    alt89 = 1;
                    	    }
                    	    break;
                    	case AVG:
                    		{
                    	    alt89 = 2;
                    	    }
                    	    break;
                    	case MAX:
                    		{
                    	    alt89 = 3;
                    	    }
                    	    break;
                    	case MIN:
                    		{
                    	    alt89 = 4;
                    	    }
                    	    break;
                    		default:
                    		    NoViableAltException nvae_d89s0 =
                    		        new NoViableAltException("", 89, 0, input);

                    		    throw nvae_d89s0;
                    	}

                    	switch (alt89) 
                    	{
                    	    case 1 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:642:6: op= SUM
                    	        {
                    	        	op=(IToken)Match(input,SUM,FOLLOW_SUM_in_aggregate3070);  
                    	        	stream_SUM.Add(op);


                    	        }
                    	        break;
                    	    case 2 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:642:15: op= AVG
                    	        {
                    	        	op=(IToken)Match(input,AVG,FOLLOW_AVG_in_aggregate3076);  
                    	        	stream_AVG.Add(op);


                    	        }
                    	        break;
                    	    case 3 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:642:24: op= MAX
                    	        {
                    	        	op=(IToken)Match(input,MAX,FOLLOW_MAX_in_aggregate3082);  
                    	        	stream_MAX.Add(op);


                    	        }
                    	        break;
                    	    case 4 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:642:33: op= MIN
                    	        {
                    	        	op=(IToken)Match(input,MIN,FOLLOW_MIN_in_aggregate3088);  
                    	        	stream_MIN.Add(op);


                    	        }
                    	        break;

                    	}

                    	OPEN254=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_aggregate3092);  
                    	stream_OPEN.Add(OPEN254);

                    	PushFollow(FOLLOW_additiveExpression_in_aggregate3094);
                    	additiveExpression255 = additiveExpression();
                    	state.followingStackPointer--;

                    	stream_additiveExpression.Add(additiveExpression255.Tree);
                    	CLOSE256=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_aggregate3096);  
                    	stream_CLOSE.Add(CLOSE256);



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
                    	// 643:3: -> ^( AGGREGATE[$op] additiveExpression )
                    	{
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:643:6: ^( AGGREGATE[$op] additiveExpression )
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
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:645:5: COUNT OPEN (s= STAR | p= aggregateDistinctAll ) CLOSE
                    {
                    	COUNT257=(IToken)Match(input,COUNT,FOLLOW_COUNT_in_aggregate3115);  
                    	stream_COUNT.Add(COUNT257);

                    	OPEN258=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_aggregate3117);  
                    	stream_OPEN.Add(OPEN258);

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:645:16: (s= STAR | p= aggregateDistinctAll )
                    	int alt90 = 2;
                    	int LA90_0 = input.LA(1);

                    	if ( (LA90_0 == STAR) )
                    	{
                    	    alt90 = 1;
                    	}
                    	else if ( (LA90_0 == ALL || (LA90_0 >= DISTINCT && LA90_0 <= ELEMENTS) || LA90_0 == INDICES || LA90_0 == IDENT) )
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
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:645:18: s= STAR
                    	        {
                    	        	s=(IToken)Match(input,STAR,FOLLOW_STAR_in_aggregate3123);  
                    	        	stream_STAR.Add(s);


                    	        }
                    	        break;
                    	    case 2 :
                    	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:645:27: p= aggregateDistinctAll
                    	        {
                    	        	PushFollow(FOLLOW_aggregateDistinctAll_in_aggregate3129);
                    	        	p = aggregateDistinctAll();
                    	        	state.followingStackPointer--;

                    	        	stream_aggregateDistinctAll.Add(p.Tree);

                    	        }
                    	        break;

                    	}

                    	CLOSE259=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_aggregate3133);  
                    	stream_CLOSE.Add(CLOSE259);



                    	// AST REWRITE
                    	// elements:          COUNT, p, COUNT
                    	// token labels:      
                    	// rule labels:       p, retval
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_p = new RewriteRuleSubtreeStream(adaptor, "rule p", p!=null ? p.Tree : null);
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

                    	root_0 = (IASTNode)adaptor.GetNilNode();
                    	// 646:3: -> {s == null}? ^( COUNT $p)
                    	if (s == null)
                    	{
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:646:19: ^( COUNT $p)
                    	    {
                    	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
                    	    root_1 = (IASTNode)adaptor.BecomeRoot(stream_COUNT.NextNode(), root_1);

                    	    adaptor.AddChild(root_1, stream_p.NextTree());

                    	    adaptor.AddChild(root_0, root_1);
                    	    }

                    	}
                    	else // 647:3: -> ^( COUNT ^( ROW_STAR[\"*\"] ) )
                    	{
                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:647:6: ^( COUNT ^( ROW_STAR[\"*\"] ) )
                    	    {
                    	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
                    	    root_1 = (IASTNode)adaptor.BecomeRoot(stream_COUNT.NextNode(), root_1);

                    	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:647:14: ^( ROW_STAR[\"*\"] )
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
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:648:5: collectionExpr
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_collectionExpr_in_aggregate3165);
                    	collectionExpr260 = collectionExpr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, collectionExpr260.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:651:1: aggregateDistinctAll : ( ( DISTINCT | ALL )? ( path | collectionExpr ) ) ;
    public HqlParser.aggregateDistinctAll_return aggregateDistinctAll() // throws RecognitionException [1]
    {   
        HqlParser.aggregateDistinctAll_return retval = new HqlParser.aggregateDistinctAll_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken set261 = null;
        HqlParser.path_return path262 = default(HqlParser.path_return);

        HqlParser.collectionExpr_return collectionExpr263 = default(HqlParser.collectionExpr_return);


        IASTNode set261_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:652:2: ( ( ( DISTINCT | ALL )? ( path | collectionExpr ) ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:652:4: ( ( DISTINCT | ALL )? ( path | collectionExpr ) )
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:652:4: ( ( DISTINCT | ALL )? ( path | collectionExpr ) )
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:652:6: ( DISTINCT | ALL )? ( path | collectionExpr )
            	{
            		// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:652:6: ( DISTINCT | ALL )?
            		int alt92 = 2;
            		int LA92_0 = input.LA(1);

            		if ( (LA92_0 == ALL || LA92_0 == DISTINCT) )
            		{
            		    alt92 = 1;
            		}
            		switch (alt92) 
            		{
            		    case 1 :
            		        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:
            		        {
            		        	set261 = (IToken)input.LT(1);
            		        	if ( input.LA(1) == ALL || input.LA(1) == DISTINCT ) 
            		        	{
            		        	    input.Consume();
            		        	    adaptor.AddChild(root_0, (IASTNode)adaptor.Create(set261));
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

            		// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:652:26: ( path | collectionExpr )
            		int alt93 = 2;
            		int LA93_0 = input.LA(1);

            		if ( (LA93_0 == IDENT) )
            		{
            		    alt93 = 1;
            		}
            		else if ( (LA93_0 == ELEMENTS || LA93_0 == INDICES) )
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
            		        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:652:28: path
            		        {
            		        	PushFollow(FOLLOW_path_in_aggregateDistinctAll3191);
            		        	path262 = path();
            		        	state.followingStackPointer--;

            		        	adaptor.AddChild(root_0, path262.Tree);

            		        }
            		        break;
            		    case 2 :
            		        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:652:35: collectionExpr
            		        {
            		        	PushFollow(FOLLOW_collectionExpr_in_aggregateDistinctAll3195);
            		        	collectionExpr263 = collectionExpr();
            		        	state.followingStackPointer--;

            		        	adaptor.AddChild(root_0, collectionExpr263.Tree);

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:657:1: collectionExpr : ( ELEMENTS | INDICES ) OPEN path CLOSE ;
    public HqlParser.collectionExpr_return collectionExpr() // throws RecognitionException [1]
    {   
        HqlParser.collectionExpr_return retval = new HqlParser.collectionExpr_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken ELEMENTS264 = null;
        IToken INDICES265 = null;
        IToken OPEN266 = null;
        IToken CLOSE268 = null;
        HqlParser.path_return path267 = default(HqlParser.path_return);


        IASTNode ELEMENTS264_tree=null;
        IASTNode INDICES265_tree=null;
        IASTNode OPEN266_tree=null;
        IASTNode CLOSE268_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:658:2: ( ( ELEMENTS | INDICES ) OPEN path CLOSE )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:658:4: ( ELEMENTS | INDICES ) OPEN path CLOSE
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:658:4: ( ELEMENTS | INDICES )
            	int alt94 = 2;
            	int LA94_0 = input.LA(1);

            	if ( (LA94_0 == ELEMENTS) )
            	{
            	    alt94 = 1;
            	}
            	else if ( (LA94_0 == INDICES) )
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
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:658:5: ELEMENTS
            	        {
            	        	ELEMENTS264=(IToken)Match(input,ELEMENTS,FOLLOW_ELEMENTS_in_collectionExpr3214); 
            	        		ELEMENTS264_tree = (IASTNode)adaptor.Create(ELEMENTS264);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(ELEMENTS264_tree, root_0);


            	        }
            	        break;
            	    case 2 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:658:17: INDICES
            	        {
            	        	INDICES265=(IToken)Match(input,INDICES,FOLLOW_INDICES_in_collectionExpr3219); 
            	        		INDICES265_tree = (IASTNode)adaptor.Create(INDICES265);
            	        		root_0 = (IASTNode)adaptor.BecomeRoot(INDICES265_tree, root_0);


            	        }
            	        break;

            	}

            	OPEN266=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_collectionExpr3223); 
            	PushFollow(FOLLOW_path_in_collectionExpr3226);
            	path267 = path();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, path267.Tree);
            	CLOSE268=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_collectionExpr3228); 

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:661:1: compoundExpr : ( collectionExpr | path | ( OPEN ( ( expression ( COMMA expression )* ) | subQuery ) CLOSE ) );
    public HqlParser.compoundExpr_return compoundExpr() // throws RecognitionException [1]
    {   
        HqlParser.compoundExpr_return retval = new HqlParser.compoundExpr_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken OPEN271 = null;
        IToken COMMA273 = null;
        IToken CLOSE276 = null;
        HqlParser.collectionExpr_return collectionExpr269 = default(HqlParser.collectionExpr_return);

        HqlParser.path_return path270 = default(HqlParser.path_return);

        HqlParser.expression_return expression272 = default(HqlParser.expression_return);

        HqlParser.expression_return expression274 = default(HqlParser.expression_return);

        HqlParser.subQuery_return subQuery275 = default(HqlParser.subQuery_return);


        IASTNode OPEN271_tree=null;
        IASTNode COMMA273_tree=null;
        IASTNode CLOSE276_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:662:2: ( collectionExpr | path | ( OPEN ( ( expression ( COMMA expression )* ) | subQuery ) CLOSE ) )
            int alt97 = 3;
            switch ( input.LA(1) ) 
            {
            case ELEMENTS:
            case INDICES:
            	{
                alt97 = 1;
                }
                break;
            case IDENT:
            	{
                alt97 = 2;
                }
                break;
            case OPEN:
            	{
                alt97 = 3;
                }
                break;
            	default:
            	    NoViableAltException nvae_d97s0 =
            	        new NoViableAltException("", 97, 0, input);

            	    throw nvae_d97s0;
            }

            switch (alt97) 
            {
                case 1 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:662:4: collectionExpr
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_collectionExpr_in_compoundExpr3283);
                    	collectionExpr269 = collectionExpr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, collectionExpr269.Tree);

                    }
                    break;
                case 2 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:663:4: path
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_path_in_compoundExpr3288);
                    	path270 = path();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, path270.Tree);

                    }
                    break;
                case 3 :
                    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:664:4: ( OPEN ( ( expression ( COMMA expression )* ) | subQuery ) CLOSE )
                    {
                    	root_0 = (IASTNode)adaptor.GetNilNode();

                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:664:4: ( OPEN ( ( expression ( COMMA expression )* ) | subQuery ) CLOSE )
                    	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:664:5: OPEN ( ( expression ( COMMA expression )* ) | subQuery ) CLOSE
                    	{
                    		OPEN271=(IToken)Match(input,OPEN,FOLLOW_OPEN_in_compoundExpr3294); 
                    		// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:664:11: ( ( expression ( COMMA expression )* ) | subQuery )
                    		int alt96 = 2;
                    		int LA96_0 = input.LA(1);

                    		if ( ((LA96_0 >= ALL && LA96_0 <= ANY) || LA96_0 == AVG || LA96_0 == COUNT || LA96_0 == ELEMENTS || (LA96_0 >= EXISTS && LA96_0 <= FALSE) || LA96_0 == INDICES || (LA96_0 >= MAX && LA96_0 <= MIN) || (LA96_0 >= NOT && LA96_0 <= NULL) || (LA96_0 >= SOME && LA96_0 <= TRUE) || LA96_0 == CASE || LA96_0 == EMPTY || (LA96_0 >= NUM_INT && LA96_0 <= NUM_LONG) || LA96_0 == OPEN || LA96_0 == BNOT || (LA96_0 >= PLUS && LA96_0 <= MINUS) || (LA96_0 >= COLON && LA96_0 <= IDENT)) )
                    		{
                    		    alt96 = 1;
                    		}
                    		else if ( (LA96_0 == EOF || LA96_0 == FROM || LA96_0 == GROUP || LA96_0 == ORDER || LA96_0 == SELECT || LA96_0 == UNION || LA96_0 == WHERE || LA96_0 == CLOSE) )
                    		{
                    		    alt96 = 2;
                    		}
                    		else 
                    		{
                    		    NoViableAltException nvae_d96s0 =
                    		        new NoViableAltException("", 96, 0, input);

                    		    throw nvae_d96s0;
                    		}
                    		switch (alt96) 
                    		{
                    		    case 1 :
                    		        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:664:13: ( expression ( COMMA expression )* )
                    		        {
                    		        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:664:13: ( expression ( COMMA expression )* )
                    		        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:664:14: expression ( COMMA expression )*
                    		        	{
                    		        		PushFollow(FOLLOW_expression_in_compoundExpr3300);
                    		        		expression272 = expression();
                    		        		state.followingStackPointer--;

                    		        		adaptor.AddChild(root_0, expression272.Tree);
                    		        		// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:664:25: ( COMMA expression )*
                    		        		do 
                    		        		{
                    		        		    int alt95 = 2;
                    		        		    int LA95_0 = input.LA(1);

                    		        		    if ( (LA95_0 == COMMA) )
                    		        		    {
                    		        		        alt95 = 1;
                    		        		    }


                    		        		    switch (alt95) 
                    		        			{
                    		        				case 1 :
                    		        				    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:664:26: COMMA expression
                    		        				    {
                    		        				    	COMMA273=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_compoundExpr3303); 
                    		        				    	PushFollow(FOLLOW_expression_in_compoundExpr3306);
                    		        				    	expression274 = expression();
                    		        				    	state.followingStackPointer--;

                    		        				    	adaptor.AddChild(root_0, expression274.Tree);

                    		        				    }
                    		        				    break;

                    		        				default:
                    		        				    goto loop95;
                    		        		    }
                    		        		} while (true);

                    		        		loop95:
                    		        			;	// Stops C# compiler whining that label 'loop95' has no statements


                    		        	}


                    		        }
                    		        break;
                    		    case 2 :
                    		        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:664:49: subQuery
                    		        {
                    		        	PushFollow(FOLLOW_subQuery_in_compoundExpr3313);
                    		        	subQuery275 = subQuery();
                    		        	state.followingStackPointer--;

                    		        	adaptor.AddChild(root_0, subQuery275.Tree);

                    		        }
                    		        break;

                    		}

                    		CLOSE276=(IToken)Match(input,CLOSE,FOLLOW_CLOSE_in_compoundExpr3317); 

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:667:1: exprList : ( TRAILING | LEADING | BOTH )? ( expression ( ( COMMA expression )+ | f= FROM expression | AS identifier )? | f2= FROM expression )? ;
    public HqlParser.exprList_return exprList() // throws RecognitionException [1]
    {   
        HqlParser.exprList_return retval = new HqlParser.exprList_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken f = null;
        IToken f2 = null;
        IToken TRAILING277 = null;
        IToken LEADING278 = null;
        IToken BOTH279 = null;
        IToken COMMA281 = null;
        IToken AS284 = null;
        HqlParser.expression_return expression280 = default(HqlParser.expression_return);

        HqlParser.expression_return expression282 = default(HqlParser.expression_return);

        HqlParser.expression_return expression283 = default(HqlParser.expression_return);

        HqlParser.identifier_return identifier285 = default(HqlParser.identifier_return);

        HqlParser.expression_return expression286 = default(HqlParser.expression_return);


        IASTNode f_tree=null;
        IASTNode f2_tree=null;
        IASTNode TRAILING277_tree=null;
        IASTNode LEADING278_tree=null;
        IASTNode BOTH279_tree=null;
        IASTNode COMMA281_tree=null;
        IASTNode AS284_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:673:2: ( ( TRAILING | LEADING | BOTH )? ( expression ( ( COMMA expression )+ | f= FROM expression | AS identifier )? | f2= FROM expression )? )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:673:4: ( TRAILING | LEADING | BOTH )? ( expression ( ( COMMA expression )+ | f= FROM expression | AS identifier )? | f2= FROM expression )?
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:673:4: ( TRAILING | LEADING | BOTH )?
            	int alt98 = 4;
            	switch ( input.LA(1) ) 
            	{
            	    case TRAILING:
            	    	{
            	        alt98 = 1;
            	        }
            	        break;
            	    case LEADING:
            	    	{
            	        alt98 = 2;
            	        }
            	        break;
            	    case BOTH:
            	    	{
            	        alt98 = 3;
            	        }
            	        break;
            	}

            	switch (alt98) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:673:5: TRAILING
            	        {
            	        	TRAILING277=(IToken)Match(input,TRAILING,FOLLOW_TRAILING_in_exprList3336); 
            	        		TRAILING277_tree = (IASTNode)adaptor.Create(TRAILING277);
            	        		adaptor.AddChild(root_0, TRAILING277_tree);

            	        	TRAILING277.Type = IDENT;

            	        }
            	        break;
            	    case 2 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:674:10: LEADING
            	        {
            	        	LEADING278=(IToken)Match(input,LEADING,FOLLOW_LEADING_in_exprList3349); 
            	        		LEADING278_tree = (IASTNode)adaptor.Create(LEADING278);
            	        		adaptor.AddChild(root_0, LEADING278_tree);

            	        	LEADING278.Type = IDENT;

            	        }
            	        break;
            	    case 3 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:675:10: BOTH
            	        {
            	        	BOTH279=(IToken)Match(input,BOTH,FOLLOW_BOTH_in_exprList3362); 
            	        		BOTH279_tree = (IASTNode)adaptor.Create(BOTH279);
            	        		adaptor.AddChild(root_0, BOTH279_tree);

            	        	BOTH279.Type = IDENT;

            	        }
            	        break;

            	}

            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:677:4: ( expression ( ( COMMA expression )+ | f= FROM expression | AS identifier )? | f2= FROM expression )?
            	int alt101 = 3;
            	int LA101_0 = input.LA(1);

            	if ( ((LA101_0 >= ALL && LA101_0 <= ANY) || LA101_0 == AVG || LA101_0 == COUNT || LA101_0 == ELEMENTS || (LA101_0 >= EXISTS && LA101_0 <= FALSE) || LA101_0 == INDICES || (LA101_0 >= MAX && LA101_0 <= MIN) || (LA101_0 >= NOT && LA101_0 <= NULL) || (LA101_0 >= SOME && LA101_0 <= TRUE) || LA101_0 == CASE || LA101_0 == EMPTY || (LA101_0 >= NUM_INT && LA101_0 <= NUM_LONG) || LA101_0 == OPEN || LA101_0 == BNOT || (LA101_0 >= PLUS && LA101_0 <= MINUS) || (LA101_0 >= COLON && LA101_0 <= IDENT)) )
            	{
            	    alt101 = 1;
            	}
            	else if ( (LA101_0 == FROM) )
            	{
            	    alt101 = 2;
            	}
            	switch (alt101) 
            	{
            	    case 1 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:678:5: expression ( ( COMMA expression )+ | f= FROM expression | AS identifier )?
            	        {
            	        	PushFollow(FOLLOW_expression_in_exprList3386);
            	        	expression280 = expression();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, expression280.Tree);
            	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:678:16: ( ( COMMA expression )+ | f= FROM expression | AS identifier )?
            	        	int alt100 = 4;
            	        	switch ( input.LA(1) ) 
            	        	{
            	        	    case COMMA:
            	        	    	{
            	        	        alt100 = 1;
            	        	        }
            	        	        break;
            	        	    case FROM:
            	        	    	{
            	        	        alt100 = 2;
            	        	        }
            	        	        break;
            	        	    case AS:
            	        	    	{
            	        	        alt100 = 3;
            	        	        }
            	        	        break;
            	        	}

            	        	switch (alt100) 
            	        	{
            	        	    case 1 :
            	        	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:678:18: ( COMMA expression )+
            	        	        {
            	        	        	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:678:18: ( COMMA expression )+
            	        	        	int cnt99 = 0;
            	        	        	do 
            	        	        	{
            	        	        	    int alt99 = 2;
            	        	        	    int LA99_0 = input.LA(1);

            	        	        	    if ( (LA99_0 == COMMA) )
            	        	        	    {
            	        	        	        alt99 = 1;
            	        	        	    }


            	        	        	    switch (alt99) 
            	        	        		{
            	        	        			case 1 :
            	        	        			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:678:19: COMMA expression
            	        	        			    {
            	        	        			    	COMMA281=(IToken)Match(input,COMMA,FOLLOW_COMMA_in_exprList3391); 
            	        	        			    	PushFollow(FOLLOW_expression_in_exprList3394);
            	        	        			    	expression282 = expression();
            	        	        			    	state.followingStackPointer--;

            	        	        			    	adaptor.AddChild(root_0, expression282.Tree);

            	        	        			    }
            	        	        			    break;

            	        	        			default:
            	        	        			    if ( cnt99 >= 1 ) goto loop99;
            	        	        		            EarlyExitException eee99 =
            	        	        		                new EarlyExitException(99, input);
            	        	        		            throw eee99;
            	        	        	    }
            	        	        	    cnt99++;
            	        	        	} while (true);

            	        	        	loop99:
            	        	        		;	// Stops C# compiler whinging that label 'loop99' has no statements


            	        	        }
            	        	        break;
            	        	    case 2 :
            	        	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:679:9: f= FROM expression
            	        	        {
            	        	        	f=(IToken)Match(input,FROM,FOLLOW_FROM_in_exprList3409); 
            	        	        		f_tree = (IASTNode)adaptor.Create(f);
            	        	        		adaptor.AddChild(root_0, f_tree);

            	        	        	PushFollow(FOLLOW_expression_in_exprList3411);
            	        	        	expression283 = expression();
            	        	        	state.followingStackPointer--;

            	        	        	adaptor.AddChild(root_0, expression283.Tree);
            	        	        	f.Type = IDENT;

            	        	        }
            	        	        break;
            	        	    case 3 :
            	        	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:680:9: AS identifier
            	        	        {
            	        	        	AS284=(IToken)Match(input,AS,FOLLOW_AS_in_exprList3423); 
            	        	        	PushFollow(FOLLOW_identifier_in_exprList3426);
            	        	        	identifier285 = identifier();
            	        	        	state.followingStackPointer--;

            	        	        	adaptor.AddChild(root_0, identifier285.Tree);

            	        	        }
            	        	        break;

            	        	}


            	        }
            	        break;
            	    case 2 :
            	        // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:681:7: f2= FROM expression
            	        {
            	        	f2=(IToken)Match(input,FROM,FOLLOW_FROM_in_exprList3440); 
            	        		f2_tree = (IASTNode)adaptor.Create(f2);
            	        		adaptor.AddChild(root_0, f2_tree);

            	        	PushFollow(FOLLOW_expression_in_exprList3442);
            	        	expression286 = expression();
            	        	state.followingStackPointer--;

            	        	adaptor.AddChild(root_0, expression286.Tree);
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:684:1: subQuery : union -> ^( QUERY[\"query\"] union ) ;
    public HqlParser.subQuery_return subQuery() // throws RecognitionException [1]
    {   
        HqlParser.subQuery_return retval = new HqlParser.subQuery_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        HqlParser.union_return union287 = default(HqlParser.union_return);


        RewriteRuleSubtreeStream stream_union = new RewriteRuleSubtreeStream(adaptor,"rule union");
        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:685:2: ( union -> ^( QUERY[\"query\"] union ) )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:685:4: union
            {
            	PushFollow(FOLLOW_union_in_subQuery3460);
            	union287 = union();
            	state.followingStackPointer--;

            	stream_union.Add(union287.Tree);


            	// AST REWRITE
            	// elements:          union
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (IASTNode)adaptor.GetNilNode();
            	// 686:2: -> ^( QUERY[\"query\"] union )
            	{
            	    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:686:5: ^( QUERY[\"query\"] union )
            	    {
            	    IASTNode root_1 = (IASTNode)adaptor.GetNilNode();
            	    root_1 = (IASTNode)adaptor.BecomeRoot((IASTNode)adaptor.Create(QUERY, "query"), root_1);

            	    adaptor.AddChild(root_1, stream_union.NextTree());

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
    // $ANTLR end "subQuery"

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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:690:1: constant : ( NUM_INT | NUM_FLOAT | NUM_LONG | NUM_DOUBLE | QUOTED_String | NULL | TRUE | FALSE | EMPTY );
    public HqlParser.constant_return constant() // throws RecognitionException [1]
    {   
        HqlParser.constant_return retval = new HqlParser.constant_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken set288 = null;

        IASTNode set288_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:691:2: ( NUM_INT | NUM_FLOAT | NUM_LONG | NUM_DOUBLE | QUOTED_String | NULL | TRUE | FALSE | EMPTY )
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:708:1: path : identifier ( DOT identifier )* ;
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
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:713:2: ( identifier ( DOT identifier )* )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:713:4: identifier ( DOT identifier )*
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	PushFollow(FOLLOW_identifier_in_path3544);
            	identifier289 = identifier();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, identifier289.Tree);
            	// /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:713:15: ( DOT identifier )*
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
            			    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:713:17: DOT identifier
            			    {
            			    	DOT290=(IToken)Match(input,DOT,FOLLOW_DOT_in_path3548); 
            			    		DOT290_tree = (IASTNode)adaptor.Create(DOT290);
            			    		root_0 = (IASTNode)adaptor.BecomeRoot(DOT290_tree, root_0);

            			    	 WeakKeywords(); 
            			    	PushFollow(FOLLOW_identifier_in_path3553);
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
    // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:718:1: identifier : IDENT ;
    public HqlParser.identifier_return identifier() // throws RecognitionException [1]
    {   
        HqlParser.identifier_return retval = new HqlParser.identifier_return();
        retval.Start = input.LT(1);

        IASTNode root_0 = null;

        IToken IDENT292 = null;

        IASTNode IDENT292_tree=null;

        try 
    	{
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:719:2: ( IDENT )
            // /Users/Steve/Projects/NHibernate/Trunk/nhibernate/src/NHibernate/Hql/Ast/ANTLR/Hql.g:719:4: IDENT
            {
            	root_0 = (IASTNode)adaptor.GetNilNode();

            	IDENT292=(IToken)Match(input,IDENT,FOLLOW_IDENT_in_identifier3569); 
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

 

    public static readonly BitSet FOLLOW_updateStatement_in_statement597 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_deleteStatement_in_statement601 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_selectStatement_in_statement605 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_insertStatement_in_statement609 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_UPDATE_in_updateStatement622 = new BitSet(new ulong[]{0x0010000000400000UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_VERSIONED_in_updateStatement626 = new BitSet(new ulong[]{0x0010000000400000UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_optionalFromTokenFromClause_in_updateStatement632 = new BitSet(new ulong[]{0x0000400000000000UL});
    public static readonly BitSet FOLLOW_setClause_in_updateStatement636 = new BitSet(new ulong[]{0x0020000000000002UL});
    public static readonly BitSet FOLLOW_whereClause_in_updateStatement641 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_SET_in_setClause655 = new BitSet(new ulong[]{0x0010000000400000UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_assignment_in_setClause658 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000400000000UL});
    public static readonly BitSet FOLLOW_COMMA_in_setClause661 = new BitSet(new ulong[]{0x0010000000400000UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_assignment_in_setClause664 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000400000000UL});
    public static readonly BitSet FOLLOW_stateField_in_assignment678 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000800000000UL});
    public static readonly BitSet FOLLOW_EQ_in_assignment680 = new BitSet(new ulong[]{0x80938098085A1230UL,0x07860011E0000000UL});
    public static readonly BitSet FOLLOW_newValue_in_assignment683 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_path_in_stateField696 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_concatenation_in_newValue709 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_DELETE_in_deleteStatement720 = new BitSet(new ulong[]{0x0010000000400000UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_optionalFromTokenFromClause_in_deleteStatement726 = new BitSet(new ulong[]{0x0020000000000002UL});
    public static readonly BitSet FOLLOW_whereClause_in_deleteStatement732 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_optionalFromTokenFromClause2_in_optionalFromTokenFromClause747 = new BitSet(new ulong[]{0x0010000000400000UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_path_in_optionalFromTokenFromClause749 = new BitSet(new ulong[]{0x0010000000400082UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_asAlias_in_optionalFromTokenFromClause752 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_FROM_in_optionalFromTokenFromClause2783 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_queryRule_in_selectStatement797 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_INSERT_in_insertStatement826 = new BitSet(new ulong[]{0x0000000040000000UL});
    public static readonly BitSet FOLLOW_intoClause_in_insertStatement829 = new BitSet(new ulong[]{0x0020220001400000UL});
    public static readonly BitSet FOLLOW_selectStatement_in_insertStatement831 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_INTO_in_intoClause842 = new BitSet(new ulong[]{0x0010000000400000UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_path_in_intoClause845 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000001000000000UL});
    public static readonly BitSet FOLLOW_insertablePropertySpec_in_intoClause849 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_OPEN_in_insertablePropertySpec860 = new BitSet(new ulong[]{0x80938098085A1230UL,0x07860011E0000000UL});
    public static readonly BitSet FOLLOW_primaryExpression_in_insertablePropertySpec862 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000002400000000UL});
    public static readonly BitSet FOLLOW_COMMA_in_insertablePropertySpec866 = new BitSet(new ulong[]{0x80938098085A1230UL,0x07860011E0000000UL});
    public static readonly BitSet FOLLOW_primaryExpression_in_insertablePropertySpec868 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000002400000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_insertablePropertySpec873 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_queryRule_in_union896 = new BitSet(new ulong[]{0x0004000000000002UL});
    public static readonly BitSet FOLLOW_UNION_in_union899 = new BitSet(new ulong[]{0x0020220001400000UL});
    public static readonly BitSet FOLLOW_queryRule_in_union901 = new BitSet(new ulong[]{0x0004000000000002UL});
    public static readonly BitSet FOLLOW_selectFrom_in_queryRule917 = new BitSet(new ulong[]{0x0020020001000002UL});
    public static readonly BitSet FOLLOW_whereClause_in_queryRule922 = new BitSet(new ulong[]{0x0000020001000002UL});
    public static readonly BitSet FOLLOW_groupByClause_in_queryRule929 = new BitSet(new ulong[]{0x0000020000000002UL});
    public static readonly BitSet FOLLOW_orderByClause_in_queryRule936 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_selectClause_in_selectFrom957 = new BitSet(new ulong[]{0x0000000000400002UL});
    public static readonly BitSet FOLLOW_fromClause_in_selectFrom964 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_SELECT_in_selectClause1017 = new BitSet(new ulong[]{0x809380F8085B1230UL,0x07860051E0000004UL});
    public static readonly BitSet FOLLOW_DISTINCT_in_selectClause1029 = new BitSet(new ulong[]{0x809380F8085B1230UL,0x07860051E0000004UL});
    public static readonly BitSet FOLLOW_selectedPropertiesList_in_selectClause1035 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_newExpression_in_selectClause1039 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_selectObject_in_selectClause1043 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_NEW_in_newExpression1057 = new BitSet(new ulong[]{0x0010000000400000UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_path_in_newExpression1059 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000001000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_newExpression1064 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x07860051E0000000UL});
    public static readonly BitSet FOLLOW_selectedPropertiesList_in_newExpression1066 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_newExpression1068 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_OBJECT_in_selectObject1094 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000001000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_selectObject1097 = new BitSet(new ulong[]{0x0010000000400000UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_identifier_in_selectObject1100 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_selectObject1102 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_FROM_in_fromClause1123 = new BitSet(new ulong[]{0x0010000004420080UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_fromRange_in_fromClause1128 = new BitSet(new ulong[]{0x0000100310800002UL,0x0000000400000000UL});
    public static readonly BitSet FOLLOW_fromJoin_in_fromClause1132 = new BitSet(new ulong[]{0x0000100310800002UL,0x0000000400000000UL});
    public static readonly BitSet FOLLOW_COMMA_in_fromClause1136 = new BitSet(new ulong[]{0x0010000004420080UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_fromRange_in_fromClause1141 = new BitSet(new ulong[]{0x0000100310800002UL,0x0000000400000000UL});
    public static readonly BitSet FOLLOW_set_in_fromJoin1162 = new BitSet(new ulong[]{0x0000040100000000UL});
    public static readonly BitSet FOLLOW_OUTER_in_fromJoin1173 = new BitSet(new ulong[]{0x0000000100000000UL});
    public static readonly BitSet FOLLOW_FULL_in_fromJoin1181 = new BitSet(new ulong[]{0x0000000100000000UL});
    public static readonly BitSet FOLLOW_INNER_in_fromJoin1185 = new BitSet(new ulong[]{0x0000000100000000UL});
    public static readonly BitSet FOLLOW_JOIN_in_fromJoin1190 = new BitSet(new ulong[]{0x0010000000600000UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_FETCH_in_fromJoin1194 = new BitSet(new ulong[]{0x0010000000400000UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_path_in_fromJoin1198 = new BitSet(new ulong[]{0x2010000000600082UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_asAlias_in_fromJoin1201 = new BitSet(new ulong[]{0x2000000000200002UL});
    public static readonly BitSet FOLLOW_propertyFetch_in_fromJoin1206 = new BitSet(new ulong[]{0x2000000000000002UL});
    public static readonly BitSet FOLLOW_withClause_in_fromJoin1211 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_set_in_fromJoin1222 = new BitSet(new ulong[]{0x0000040100000000UL});
    public static readonly BitSet FOLLOW_OUTER_in_fromJoin1233 = new BitSet(new ulong[]{0x0000000100000000UL});
    public static readonly BitSet FOLLOW_FULL_in_fromJoin1241 = new BitSet(new ulong[]{0x0000000100000000UL});
    public static readonly BitSet FOLLOW_INNER_in_fromJoin1245 = new BitSet(new ulong[]{0x0000000100000000UL});
    public static readonly BitSet FOLLOW_JOIN_in_fromJoin1250 = new BitSet(new ulong[]{0x0000000000220000UL});
    public static readonly BitSet FOLLOW_FETCH_in_fromJoin1254 = new BitSet(new ulong[]{0x0000000000020000UL});
    public static readonly BitSet FOLLOW_ELEMENTS_in_fromJoin1258 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000001000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_fromJoin1261 = new BitSet(new ulong[]{0x0010000000400000UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_path_in_fromJoin1264 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_fromJoin1266 = new BitSet(new ulong[]{0x2010000000600082UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_asAlias_in_fromJoin1270 = new BitSet(new ulong[]{0x2000000000200002UL});
    public static readonly BitSet FOLLOW_propertyFetch_in_fromJoin1275 = new BitSet(new ulong[]{0x2000000000000002UL});
    public static readonly BitSet FOLLOW_withClause_in_fromJoin1280 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WITH_in_withClause1293 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x07860051E0000000UL});
    public static readonly BitSet FOLLOW_logicalExpression_in_withClause1296 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_fromClassOrOuterQueryPath_in_fromRange1307 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_inClassDeclaration_in_fromRange1312 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_inCollectionDeclaration_in_fromRange1317 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_inCollectionElementsDeclaration_in_fromRange1322 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_path_in_fromClassOrOuterQueryPath1334 = new BitSet(new ulong[]{0x0010000000600082UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_asAlias_in_fromClassOrOuterQueryPath1339 = new BitSet(new ulong[]{0x0000000000200002UL});
    public static readonly BitSet FOLLOW_propertyFetch_in_fromClassOrOuterQueryPath1344 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_alias_in_inClassDeclaration1374 = new BitSet(new ulong[]{0x0000000004000000UL});
    public static readonly BitSet FOLLOW_IN_in_inClassDeclaration1376 = new BitSet(new ulong[]{0x0010000000400800UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_CLASS_in_inClassDeclaration1378 = new BitSet(new ulong[]{0x0010000000400000UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_path_in_inClassDeclaration1381 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_IN_in_inCollectionDeclaration1409 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000001000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_inCollectionDeclaration1411 = new BitSet(new ulong[]{0x0010000000400000UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_path_in_inCollectionDeclaration1413 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_inCollectionDeclaration1415 = new BitSet(new ulong[]{0x0010000000400080UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_alias_in_inCollectionDeclaration1417 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_alias_in_inCollectionElementsDeclaration1451 = new BitSet(new ulong[]{0x0000000004000000UL});
    public static readonly BitSet FOLLOW_IN_in_inCollectionElementsDeclaration1453 = new BitSet(new ulong[]{0x0000000000020000UL});
    public static readonly BitSet FOLLOW_ELEMENTS_in_inCollectionElementsDeclaration1455 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000001000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_inCollectionElementsDeclaration1457 = new BitSet(new ulong[]{0x0010000000400000UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_path_in_inCollectionElementsDeclaration1459 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_inCollectionElementsDeclaration1461 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ELEMENTS_in_inCollectionElementsDeclaration1483 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000001000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_inCollectionElementsDeclaration1485 = new BitSet(new ulong[]{0x0010000000400000UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_path_in_inCollectionElementsDeclaration1487 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_inCollectionElementsDeclaration1489 = new BitSet(new ulong[]{0x0000000000000080UL});
    public static readonly BitSet FOLLOW_AS_in_inCollectionElementsDeclaration1491 = new BitSet(new ulong[]{0x0010000000400080UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_alias_in_inCollectionElementsDeclaration1493 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_AS_in_asAlias1525 = new BitSet(new ulong[]{0x0010000000400080UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_alias_in_asAlias1530 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_identifier_in_alias1542 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_FETCH_in_propertyFetch1561 = new BitSet(new ulong[]{0x0000000000000010UL});
    public static readonly BitSet FOLLOW_ALL_in_propertyFetch1563 = new BitSet(new ulong[]{0x0000080000000000UL});
    public static readonly BitSet FOLLOW_PROPERTIES_in_propertyFetch1566 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_GROUP_in_groupByClause1581 = new BitSet(new ulong[]{0x0040000000000000UL});
    public static readonly BitSet FOLLOW_LITERAL_by_in_groupByClause1587 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x07860051E0000000UL});
    public static readonly BitSet FOLLOW_expression_in_groupByClause1590 = new BitSet(new ulong[]{0x0000000002000002UL,0x0000000400000000UL});
    public static readonly BitSet FOLLOW_COMMA_in_groupByClause1594 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x07860051E0000000UL});
    public static readonly BitSet FOLLOW_expression_in_groupByClause1597 = new BitSet(new ulong[]{0x0000000002000002UL,0x0000000400000000UL});
    public static readonly BitSet FOLLOW_havingClause_in_groupByClause1605 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ORDER_in_orderByClause1621 = new BitSet(new ulong[]{0x0040000000000000UL});
    public static readonly BitSet FOLLOW_LITERAL_by_in_orderByClause1624 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x07860051E0000000UL});
    public static readonly BitSet FOLLOW_orderElement_in_orderByClause1627 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000400000000UL});
    public static readonly BitSet FOLLOW_COMMA_in_orderByClause1631 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x07860051E0000000UL});
    public static readonly BitSet FOLLOW_orderElement_in_orderByClause1634 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000400000000UL});
    public static readonly BitSet FOLLOW_expression_in_orderElement1648 = new BitSet(new ulong[]{0x0000000000004102UL,0x0000000000000000UL,0x000000000000000CUL});
    public static readonly BitSet FOLLOW_ascendingOrDescending_in_orderElement1652 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ASCENDING_in_ascendingOrDescending1670 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_130_in_ascendingOrDescending1676 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_DESCENDING_in_ascendingOrDescending1696 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_131_in_ascendingOrDescending1702 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_HAVING_in_havingClause1726 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x07860051E0000000UL});
    public static readonly BitSet FOLLOW_logicalExpression_in_havingClause1729 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WHERE_in_whereClause1743 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x07860051E0000000UL});
    public static readonly BitSet FOLLOW_logicalExpression_in_whereClause1746 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_aliasedExpression_in_selectedPropertiesList1760 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000400000000UL});
    public static readonly BitSet FOLLOW_COMMA_in_selectedPropertiesList1764 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x07860051E0000000UL});
    public static readonly BitSet FOLLOW_aliasedExpression_in_selectedPropertiesList1767 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000400000000UL});
    public static readonly BitSet FOLLOW_expression_in_aliasedExpression1782 = new BitSet(new ulong[]{0x0000000000000082UL});
    public static readonly BitSet FOLLOW_AS_in_aliasedExpression1786 = new BitSet(new ulong[]{0x0010000000400000UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_identifier_in_aliasedExpression1789 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_expression_in_logicalExpression1828 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_logicalOrExpression_in_expression1840 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_logicalAndExpression_in_logicalOrExpression1852 = new BitSet(new ulong[]{0x0000010000000002UL});
    public static readonly BitSet FOLLOW_OR_in_logicalOrExpression1856 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x07860051E0000000UL});
    public static readonly BitSet FOLLOW_logicalAndExpression_in_logicalOrExpression1859 = new BitSet(new ulong[]{0x0000010000000002UL});
    public static readonly BitSet FOLLOW_negatedExpression_in_logicalAndExpression1874 = new BitSet(new ulong[]{0x0000000000000042UL});
    public static readonly BitSet FOLLOW_AND_in_logicalAndExpression1878 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x07860051E0000000UL});
    public static readonly BitSet FOLLOW_negatedExpression_in_logicalAndExpression1881 = new BitSet(new ulong[]{0x0000000000000042UL});
    public static readonly BitSet FOLLOW_NOT_in_negatedExpression1902 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x07860051E0000000UL});
    public static readonly BitSet FOLLOW_negatedExpression_in_negatedExpression1906 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_bitwiseNotExpression_in_negatedExpression1919 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_BNOT_in_bitwiseNotExpression1943 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x07860051E0000000UL});
    public static readonly BitSet FOLLOW_bitwiseOrExpression_in_bitwiseNotExpression1946 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_bitwiseOrExpression_in_bitwiseNotExpression1952 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_bitwiseXOrExpression_in_bitwiseOrExpression1964 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000008000000000UL});
    public static readonly BitSet FOLLOW_BOR_in_bitwiseOrExpression1967 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x07860051E0000000UL});
    public static readonly BitSet FOLLOW_bitwiseXOrExpression_in_bitwiseOrExpression1970 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000008000000000UL});
    public static readonly BitSet FOLLOW_bitwiseAndExpression_in_bitwiseXOrExpression1984 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000010000000000UL});
    public static readonly BitSet FOLLOW_BXOR_in_bitwiseXOrExpression1987 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x07860051E0000000UL});
    public static readonly BitSet FOLLOW_bitwiseAndExpression_in_bitwiseXOrExpression1990 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000010000000000UL});
    public static readonly BitSet FOLLOW_equalityExpression_in_bitwiseAndExpression2004 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000020000000000UL});
    public static readonly BitSet FOLLOW_BAND_in_bitwiseAndExpression2007 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x07860051E0000000UL});
    public static readonly BitSet FOLLOW_equalityExpression_in_bitwiseAndExpression2010 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000020000000000UL});
    public static readonly BitSet FOLLOW_relationalExpression_in_equalityExpression2032 = new BitSet(new ulong[]{0x0000000080000002UL,0x00000C0800000000UL});
    public static readonly BitSet FOLLOW_EQ_in_equalityExpression2040 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x07860051E0000000UL});
    public static readonly BitSet FOLLOW_IS_in_equalityExpression2049 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x07860051E0000000UL});
    public static readonly BitSet FOLLOW_NOT_in_equalityExpression2055 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x07860051E0000000UL});
    public static readonly BitSet FOLLOW_NE_in_equalityExpression2067 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x07860051E0000000UL});
    public static readonly BitSet FOLLOW_SQL_NE_in_equalityExpression2076 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x07860051E0000000UL});
    public static readonly BitSet FOLLOW_relationalExpression_in_equalityExpression2087 = new BitSet(new ulong[]{0x0000000080000002UL,0x00000C0800000000UL});
    public static readonly BitSet FOLLOW_concatenation_in_relationalExpression2104 = new BitSet(new ulong[]{0x0000004404000402UL,0x0000F00000000002UL});
    public static readonly BitSet FOLLOW_LT_in_relationalExpression2116 = new BitSet(new ulong[]{0x80938098085A1230UL,0x07860011E0000000UL});
    public static readonly BitSet FOLLOW_GT_in_relationalExpression2121 = new BitSet(new ulong[]{0x80938098085A1230UL,0x07860011E0000000UL});
    public static readonly BitSet FOLLOW_LE_in_relationalExpression2126 = new BitSet(new ulong[]{0x80938098085A1230UL,0x07860011E0000000UL});
    public static readonly BitSet FOLLOW_GE_in_relationalExpression2131 = new BitSet(new ulong[]{0x80938098085A1230UL,0x07860011E0000000UL});
    public static readonly BitSet FOLLOW_additiveExpression_in_relationalExpression2136 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000F00000000000UL});
    public static readonly BitSet FOLLOW_NOT_in_relationalExpression2153 = new BitSet(new ulong[]{0x0000000404000400UL,0x0000000000000002UL});
    public static readonly BitSet FOLLOW_IN_in_relationalExpression2174 = new BitSet(new ulong[]{0x0011001808421200UL,0x0400001000000000UL});
    public static readonly BitSet FOLLOW_inList_in_relationalExpression2183 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_BETWEEN_in_relationalExpression2194 = new BitSet(new ulong[]{0x80938098085A1230UL,0x07860011E0000000UL});
    public static readonly BitSet FOLLOW_betweenList_in_relationalExpression2203 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_LIKE_in_relationalExpression2215 = new BitSet(new ulong[]{0x80938098085A1230UL,0x07860011E0000000UL});
    public static readonly BitSet FOLLOW_concatenation_in_relationalExpression2224 = new BitSet(new ulong[]{0x0000000000040000UL});
    public static readonly BitSet FOLLOW_likeEscape_in_relationalExpression2226 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_MEMBER_in_relationalExpression2235 = new BitSet(new ulong[]{0x0010000000400000UL,0x0400000000000008UL});
    public static readonly BitSet FOLLOW_OF_in_relationalExpression2239 = new BitSet(new ulong[]{0x0010000000400000UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_path_in_relationalExpression2246 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ESCAPE_in_likeEscape2273 = new BitSet(new ulong[]{0x80938098085A1230UL,0x07860011E0000000UL});
    public static readonly BitSet FOLLOW_concatenation_in_likeEscape2276 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_compoundExpr_in_inList2289 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_concatenation_in_betweenList2310 = new BitSet(new ulong[]{0x0000000000000040UL});
    public static readonly BitSet FOLLOW_AND_in_betweenList2312 = new BitSet(new ulong[]{0x80938098085A1230UL,0x07860011E0000000UL});
    public static readonly BitSet FOLLOW_concatenation_in_betweenList2315 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_additiveExpression_in_concatenation2334 = new BitSet(new ulong[]{0x0000000000000002UL,0x0001000000000000UL});
    public static readonly BitSet FOLLOW_CONCAT_in_concatenation2342 = new BitSet(new ulong[]{0x80938098085A1230UL,0x07860011E0000000UL});
    public static readonly BitSet FOLLOW_additiveExpression_in_concatenation2351 = new BitSet(new ulong[]{0x0000000000000002UL,0x0001000000000000UL});
    public static readonly BitSet FOLLOW_CONCAT_in_concatenation2358 = new BitSet(new ulong[]{0x80938098085A1230UL,0x07860011E0000000UL});
    public static readonly BitSet FOLLOW_additiveExpression_in_concatenation2361 = new BitSet(new ulong[]{0x0000000000000002UL,0x0001000000000000UL});
    public static readonly BitSet FOLLOW_multiplyExpression_in_additiveExpression2383 = new BitSet(new ulong[]{0x0000000000000002UL,0x0006000000000000UL});
    public static readonly BitSet FOLLOW_PLUS_in_additiveExpression2389 = new BitSet(new ulong[]{0x80938098085A1230UL,0x07860011E0000000UL});
    public static readonly BitSet FOLLOW_MINUS_in_additiveExpression2394 = new BitSet(new ulong[]{0x80938098085A1230UL,0x07860011E0000000UL});
    public static readonly BitSet FOLLOW_multiplyExpression_in_additiveExpression2399 = new BitSet(new ulong[]{0x0000000000000002UL,0x0006000000000000UL});
    public static readonly BitSet FOLLOW_unaryExpression_in_multiplyExpression2414 = new BitSet(new ulong[]{0x0000000000000002UL,0x0018000000000000UL});
    public static readonly BitSet FOLLOW_STAR_in_multiplyExpression2420 = new BitSet(new ulong[]{0x80938098085A1230UL,0x07860011E0000000UL});
    public static readonly BitSet FOLLOW_DIV_in_multiplyExpression2425 = new BitSet(new ulong[]{0x80938098085A1230UL,0x07860011E0000000UL});
    public static readonly BitSet FOLLOW_unaryExpression_in_multiplyExpression2430 = new BitSet(new ulong[]{0x0000000000000002UL,0x0018000000000000UL});
    public static readonly BitSet FOLLOW_MINUS_in_unaryExpression2448 = new BitSet(new ulong[]{0x80938098085A1230UL,0x07860011E0000000UL});
    public static readonly BitSet FOLLOW_unaryExpression_in_unaryExpression2452 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_PLUS_in_unaryExpression2469 = new BitSet(new ulong[]{0x80938098085A1230UL,0x07860011E0000000UL});
    public static readonly BitSet FOLLOW_unaryExpression_in_unaryExpression2473 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_caseExpression_in_unaryExpression2490 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_quantifiedExpression_in_unaryExpression2504 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_atom_in_unaryExpression2519 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_CASE_in_caseExpression2538 = new BitSet(new ulong[]{0x0800000000000000UL});
    public static readonly BitSet FOLLOW_whenClause_in_caseExpression2541 = new BitSet(new ulong[]{0x0B00000000000000UL});
    public static readonly BitSet FOLLOW_elseClause_in_caseExpression2546 = new BitSet(new ulong[]{0x0100000000000000UL});
    public static readonly BitSet FOLLOW_END_in_caseExpression2550 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_CASE_in_caseExpression2569 = new BitSet(new ulong[]{0x80938098085A1230UL,0x07860011E0000000UL});
    public static readonly BitSet FOLLOW_unaryExpression_in_caseExpression2571 = new BitSet(new ulong[]{0x0800000000000000UL});
    public static readonly BitSet FOLLOW_altWhenClause_in_caseExpression2574 = new BitSet(new ulong[]{0x0B00000000000000UL});
    public static readonly BitSet FOLLOW_elseClause_in_caseExpression2579 = new BitSet(new ulong[]{0x0100000000000000UL});
    public static readonly BitSet FOLLOW_END_in_caseExpression2583 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WHEN_in_whenClause2612 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x07860051E0000000UL});
    public static readonly BitSet FOLLOW_logicalExpression_in_whenClause2615 = new BitSet(new ulong[]{0x0400000000000000UL});
    public static readonly BitSet FOLLOW_THEN_in_whenClause2617 = new BitSet(new ulong[]{0x80938098085A1230UL,0x07860011E0000000UL});
    public static readonly BitSet FOLLOW_unaryExpression_in_whenClause2620 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WHEN_in_altWhenClause2634 = new BitSet(new ulong[]{0x80938098085A1230UL,0x07860011E0000000UL});
    public static readonly BitSet FOLLOW_unaryExpression_in_altWhenClause2637 = new BitSet(new ulong[]{0x0400000000000000UL});
    public static readonly BitSet FOLLOW_THEN_in_altWhenClause2639 = new BitSet(new ulong[]{0x80938098085A1230UL,0x07860011E0000000UL});
    public static readonly BitSet FOLLOW_unaryExpression_in_altWhenClause2642 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ELSE_in_elseClause2656 = new BitSet(new ulong[]{0x80938098085A1230UL,0x07860011E0000000UL});
    public static readonly BitSet FOLLOW_unaryExpression_in_elseClause2659 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_SOME_in_quantifiedExpression2674 = new BitSet(new ulong[]{0x0011001808421200UL,0x0400001000000000UL});
    public static readonly BitSet FOLLOW_EXISTS_in_quantifiedExpression2679 = new BitSet(new ulong[]{0x0011001808421200UL,0x0400001000000000UL});
    public static readonly BitSet FOLLOW_ALL_in_quantifiedExpression2684 = new BitSet(new ulong[]{0x0011001808421200UL,0x0400001000000000UL});
    public static readonly BitSet FOLLOW_ANY_in_quantifiedExpression2689 = new BitSet(new ulong[]{0x0011001808421200UL,0x0400001000000000UL});
    public static readonly BitSet FOLLOW_identifier_in_quantifiedExpression2698 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_collectionExpr_in_quantifiedExpression2702 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_OPEN_in_quantifiedExpression2707 = new BitSet(new ulong[]{0x0020220001400000UL});
    public static readonly BitSet FOLLOW_subQuery_in_quantifiedExpression2712 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_quantifiedExpression2716 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_primaryExpression_in_atom2735 = new BitSet(new ulong[]{0x0000000000008002UL,0x0020000000000000UL});
    public static readonly BitSet FOLLOW_DOT_in_atom2744 = new BitSet(new ulong[]{0x0010000000400000UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_identifier_in_atom2747 = new BitSet(new ulong[]{0x0000000000008002UL,0x0020001000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_atom2775 = new BitSet(new ulong[]{0xC09380D8085A1230UL,0x07860071E0000011UL});
    public static readonly BitSet FOLLOW_exprList_in_atom2780 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_atom2782 = new BitSet(new ulong[]{0x0000000000008002UL,0x0020000000000000UL});
    public static readonly BitSet FOLLOW_OPEN_BRACKET_in_atom2796 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x07860051E0000000UL});
    public static readonly BitSet FOLLOW_expression_in_atom2801 = new BitSet(new ulong[]{0x0000000000000000UL,0x0040000000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_BRACKET_in_atom2803 = new BitSet(new ulong[]{0x0000000000008002UL,0x0020000000000000UL});
    public static readonly BitSet FOLLOW_identPrimary_in_primaryExpression2823 = new BitSet(new ulong[]{0x0000000000008002UL});
    public static readonly BitSet FOLLOW_DOT_in_primaryExpression2836 = new BitSet(new ulong[]{0x0000000000000800UL});
    public static readonly BitSet FOLLOW_CLASS_in_primaryExpression2839 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_constant_in_primaryExpression2849 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_COLON_in_primaryExpression2856 = new BitSet(new ulong[]{0x0010000000400000UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_identifier_in_primaryExpression2859 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_OPEN_in_primaryExpression2868 = new BitSet(new ulong[]{0x80B3A2D8095A1230UL,0x07860051E0000000UL});
    public static readonly BitSet FOLLOW_expressionOrVector_in_primaryExpression2872 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_subQuery_in_primaryExpression2876 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_primaryExpression2879 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_PARAM_in_primaryExpression2887 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000020000000UL});
    public static readonly BitSet FOLLOW_NUM_INT_in_primaryExpression2891 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_expression_in_expressionOrVector2909 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000400000000UL});
    public static readonly BitSet FOLLOW_vectorExpr_in_expressionOrVector2915 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_COMMA_in_vectorExpr2954 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x07860051E0000000UL});
    public static readonly BitSet FOLLOW_expression_in_vectorExpr2957 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000400000000UL});
    public static readonly BitSet FOLLOW_COMMA_in_vectorExpr2960 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x07860051E0000000UL});
    public static readonly BitSet FOLLOW_expression_in_vectorExpr2963 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000400000000UL});
    public static readonly BitSet FOLLOW_identifier_in_identPrimary2979 = new BitSet(new ulong[]{0x0000000000008002UL,0x0000001000000000UL});
    public static readonly BitSet FOLLOW_DOT_in_identPrimary2997 = new BitSet(new ulong[]{0x0010000000400000UL,0x0400000000000004UL});
    public static readonly BitSet FOLLOW_identifier_in_identPrimary3002 = new BitSet(new ulong[]{0x0000000000008002UL,0x0000001000000000UL});
    public static readonly BitSet FOLLOW_OBJECT_in_identPrimary3008 = new BitSet(new ulong[]{0x0000000000008002UL,0x0000001000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_identPrimary3026 = new BitSet(new ulong[]{0xC09380D8085A1230UL,0x07860071E0000011UL});
    public static readonly BitSet FOLLOW_exprList_in_identPrimary3031 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_identPrimary3033 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_aggregate_in_identPrimary3049 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_SUM_in_aggregate3070 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000001000000000UL});
    public static readonly BitSet FOLLOW_AVG_in_aggregate3076 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000001000000000UL});
    public static readonly BitSet FOLLOW_MAX_in_aggregate3082 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000001000000000UL});
    public static readonly BitSet FOLLOW_MIN_in_aggregate3088 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000001000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_aggregate3092 = new BitSet(new ulong[]{0x80938098085A1230UL,0x07860011E0000000UL});
    public static readonly BitSet FOLLOW_additiveExpression_in_aggregate3094 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_aggregate3096 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_COUNT_in_aggregate3115 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000001000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_aggregate3117 = new BitSet(new ulong[]{0x0011001808431210UL,0x0408000000000000UL});
    public static readonly BitSet FOLLOW_STAR_in_aggregate3123 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_aggregateDistinctAll_in_aggregate3129 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_aggregate3133 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_collectionExpr_in_aggregate3165 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_set_in_aggregateDistinctAll3178 = new BitSet(new ulong[]{0x0011001808421200UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_path_in_aggregateDistinctAll3191 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_collectionExpr_in_aggregateDistinctAll3195 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ELEMENTS_in_collectionExpr3214 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000001000000000UL});
    public static readonly BitSet FOLLOW_INDICES_in_collectionExpr3219 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000001000000000UL});
    public static readonly BitSet FOLLOW_OPEN_in_collectionExpr3223 = new BitSet(new ulong[]{0x0010000000400000UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_path_in_collectionExpr3226 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_collectionExpr3228 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_collectionExpr_in_compoundExpr3283 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_path_in_compoundExpr3288 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_OPEN_in_compoundExpr3294 = new BitSet(new ulong[]{0x80B3A2D8095A1230UL,0x07860051E0000000UL});
    public static readonly BitSet FOLLOW_expression_in_compoundExpr3300 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000002400000000UL});
    public static readonly BitSet FOLLOW_COMMA_in_compoundExpr3303 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x07860051E0000000UL});
    public static readonly BitSet FOLLOW_expression_in_compoundExpr3306 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000002400000000UL});
    public static readonly BitSet FOLLOW_subQuery_in_compoundExpr3313 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000002000000000UL});
    public static readonly BitSet FOLLOW_CLOSE_in_compoundExpr3317 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_TRAILING_in_exprList3336 = new BitSet(new ulong[]{0x809380D8085A1232UL,0x07860051E0000000UL});
    public static readonly BitSet FOLLOW_LEADING_in_exprList3349 = new BitSet(new ulong[]{0x809380D8085A1232UL,0x07860051E0000000UL});
    public static readonly BitSet FOLLOW_BOTH_in_exprList3362 = new BitSet(new ulong[]{0x809380D8085A1232UL,0x07860051E0000000UL});
    public static readonly BitSet FOLLOW_expression_in_exprList3386 = new BitSet(new ulong[]{0x0000000000400082UL,0x0000000400000000UL});
    public static readonly BitSet FOLLOW_COMMA_in_exprList3391 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x07860051E0000000UL});
    public static readonly BitSet FOLLOW_expression_in_exprList3394 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000400000000UL});
    public static readonly BitSet FOLLOW_FROM_in_exprList3409 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x07860051E0000000UL});
    public static readonly BitSet FOLLOW_expression_in_exprList3411 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_AS_in_exprList3423 = new BitSet(new ulong[]{0x0010000000400000UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_identifier_in_exprList3426 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_FROM_in_exprList3440 = new BitSet(new ulong[]{0x809380D8085A1230UL,0x07860051E0000000UL});
    public static readonly BitSet FOLLOW_expression_in_exprList3442 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_union_in_subQuery3460 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_set_in_constant0 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_identifier_in_path3544 = new BitSet(new ulong[]{0x0000000000008002UL});
    public static readonly BitSet FOLLOW_DOT_in_path3548 = new BitSet(new ulong[]{0x0010000000400000UL,0x0400000000000000UL});
    public static readonly BitSet FOLLOW_identifier_in_path3553 = new BitSet(new ulong[]{0x0000000000008002UL});
    public static readonly BitSet FOLLOW_IDENT_in_identifier3569 = new BitSet(new ulong[]{0x0000000000000002UL});

}
}