using System;
using Antlr.Runtime;

using NHibernate.Hql.Ast.ANTLR.Tree;
using IToken = Antlr.Runtime.IToken;
using RecognitionException = Antlr.Runtime.RecognitionException;

namespace NHibernate.Hql.Ast.ANTLR
{
	public partial class HqlParser
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(HqlParser));

		internal static readonly bool[] possibleIds;

		static HqlParser()
		{
			possibleIds = new bool[tokenNames.Length];

			possibleIds[COUNT] = true;
			possibleIds[ALL] = true;
			possibleIds[ANY] = true;
			possibleIds[AND] = true;
			possibleIds[AS] = true;
			possibleIds[ASCENDING] = true;
			possibleIds[AVG] = true;
			possibleIds[BETWEEN] = true;
			possibleIds[CLASS] = true;
			possibleIds[DELETE] = true;
			possibleIds[DESCENDING] = true;
			possibleIds[DISTINCT] = true;
			possibleIds[ELEMENTS] = true;
			possibleIds[ESCAPE] = true;
			possibleIds[EXISTS] = true;
			possibleIds[FALSE] = true;
			possibleIds[FETCH] = true;
			possibleIds[FROM] = true;
			possibleIds[FULL] = true;
			possibleIds[GROUP] = true;
			possibleIds[HAVING] = true;
			possibleIds[IN] = true;
			possibleIds[INDICES] = true;
			possibleIds[INNER] = true;
			possibleIds[INSERT] = true;
			possibleIds[INTO] = true;
			possibleIds[IS] = true;
			possibleIds[JOIN] = true;
			possibleIds[LEFT] = true;
			possibleIds[LIKE] = true;
			possibleIds[MAX] = true;
			possibleIds[MIN] = true;
			possibleIds[NEW] = true;
			possibleIds[NOT] = true;
			possibleIds[NULL] = true;
			possibleIds[OR] = true;
			possibleIds[ORDER] = true;
			possibleIds[OUTER] = true;
			possibleIds[PROPERTIES] = true;
			possibleIds[RIGHT] = true;
			possibleIds[SELECT] = true;
			possibleIds[SET] = true;
			possibleIds[SOME] = true;
			possibleIds[SUM] = true;
			possibleIds[TRUE] = true;
			possibleIds[UNION] = true;
			possibleIds[UPDATE] = true;
			possibleIds[VERSIONED] = true;
			possibleIds[WHERE] = true;
			possibleIds[LITERAL_by] = true;
			possibleIds[CASE] = true;
			possibleIds[END] = true;
			possibleIds[ELSE] = true;
			possibleIds[THEN] = true;
			possibleIds[WHEN] = true;
			possibleIds[ON] = true;
			possibleIds[WITH] = true;
			possibleIds[BOTH] = true;
			possibleIds[EMPTY] = true;
			possibleIds[LEADING] = true;
			possibleIds[MEMBER] = true;
			possibleIds[OBJECT] = true;
			possibleIds[OF] = true;
			possibleIds[TRAILING] = true;
		}

		/** True if this is a filter query (allow no FROM clause). **/
		private bool filter;
		// Depth of curreny query (root query has depth = 1)
		private int queryDepth = 0;
		private IParseErrorHandler _parseErrorHandler = new ErrorCounter();

		public IParseErrorHandler ParseErrorHandler
		{
			get { return _parseErrorHandler; }
			set { _parseErrorHandler = value; }
		}

		public bool Filter
		{
			get { return filter; }
			set { filter = value; }
		}


		public override void ReportError(RecognitionException e)
		{
			_parseErrorHandler.ReportError(e);
		}

        protected override object RecoverFromMismatchedToken(IIntStream input, int ttype, BitSet follow)
        {
            throw new MismatchedTokenException(ttype, input);
        }

		public void WeakKeywords()
		{
			int t = input.LA(1);

			switch (t)
			{
				case ORDER:
				case GROUP:
					// Case 1: Multi token keywords GROUP BY and ORDER BY
					// The next token ( LT(2) ) should be 'by'... otherwise, this is just an ident.
					if (input.LA(2) != LITERAL_by)
					{
						input.LT(1).Type = IDENT;
						if (log.IsDebugEnabled)
						{
							log.Debug("weakKeywords() : new LT(1) token - " + input.LT(1));
						}
					}
					break;
				default:
					// Case 2: The current token is after FROM and before '.'.
                    if (t != IDENT && input.LA(-1) == FROM && ((input.LA(2) == DOT) || (input.LA(2) == IDENT) || (input.LA(2) == -1)))
					{
						HqlToken hqlToken = input.LT(1) as HqlToken;
						if (hqlToken != null && hqlToken.PossibleId)
						{
							hqlToken.Type = IDENT;
							if (log.IsDebugEnabled)
							{
								log.Debug("weakKeywords() : new LT(1) token - " + input.LT(1));
							}
						}
					}
					break;
			}
		}

        public void WeakKeywords2()
        {
            /*
             * path
             * alias in class? path
             * in open path close
             * alias in elements open path close
             * elements open path close
             * alias in path dot elements
            */
            int t = input.LA(1);

            switch (t)
            {
                case ORDER:
                case GROUP:
                    // Case 1: Multi token keywords GROUP BY and ORDER BY
                    // The next token ( LT(2) ) should be 'by'... otherwise, this is just an ident.
                    if (input.LA(2) != LITERAL_by)
                    {
                        input.LT(1).Type = IDENT;
                        if (log.IsDebugEnabled)
                        {
                            log.Debug("weakKeywords() : new LT(1) token - " + input.LT(1));
                        }
                    }
                    break;
                default:
                    // Case 2: The current token is after FROM and before '.'.
                    if (t != IDENT && input.LA(-1) == FROM && input.LA(2) == DOT)
                    {
                        HqlToken hqlToken = (HqlToken)input.LT(1);
                        if (hqlToken.PossibleId)
                        {
                            hqlToken.Type = IDENT;
                            if (log.IsDebugEnabled)
                            {
                                log.Debug("weakKeywords() : new LT(1) token - " + input.LT(1));
                            }
                        }
                    }
                    break;
            }
        }

		public IASTNode NegateNode(IASTNode node)
		{
			// TODO - copy code from HqlParser.java
			switch (node.Type)
			{
				case OR:
					node.Type = AND;
					node.Text = "{and}";
					NegateNode(node.GetChild(0));
					NegateNode(node.GetChild(1));
					return node;
				case AND:
					node.Type = OR;
					node.Text = "{or}";
					NegateNode(node.GetChild(0));
					NegateNode(node.GetChild(1));
					return node;
				case EQ:
					node.Type = NE;
					node.Text = "{not}" + node.Text;
					return node;	// (NOT (EQ a b) ) => (NE a b)
				case NE:
					node.Type = EQ;
					node.Text = "{not}" + node.Text;
					return node;	// (NOT (NE a b) ) => (EQ a b)
				case GT:
					node.Type = LE;
					node.Text = "{not}" + node.Text;
					return node;	// (NOT (GT a b) ) => (LE a b)
				case LT:
					node.Type = GE;
					node.Text = "{not}" + node.Text;
					return node;	// (NOT (LT a b) ) => (GE a b)
				case GE:
					node.Type = LT;
					node.Text = "{not}" + node.Text;
					return node;	// (NOT (GE a b) ) => (LT a b)
				case LE:
					node.Type = GT;
					node.Text = "{not}" + node.Text;
					return node;	// (NOT (LE a b) ) => (GT a b)
				case LIKE:
					node.Type = NOT_LIKE;
					node.Text = "{not}" + node.Text;
					return node;	// (NOT (LIKE a b) ) => (NOT_LIKE a b)
				case NOT_LIKE:
					node.Type = LIKE;
					node.Text = "{not}" + node.Text;
					return node;	// (NOT (NOT_LIKE a b) ) => (LIKE a b)
				case IN:
					node.Type = NOT_IN;
					node.Text = "{not}" + node.Text;
					return node;
				case NOT_IN:
					node.Type = IN;
					node.Text = "{not}" + node.Text;
					return node;
				case IS_NULL:
					node.Type = IS_NOT_NULL;
					node.Text = "{not}" + node.Text;
					return node;	// (NOT (IS_NULL a b) ) => (IS_NOT_NULL a b)
				case IS_NOT_NULL:
					node.Type = IS_NULL;
					node.Text = "{not}" + node.Text;
					return node;	// (NOT (IS_NOT_NULL a b) ) => (IS_NULL a b)
				case BETWEEN:
					node.Type = NOT_BETWEEN;
					node.Text = "{not}" + node.Text;
					return node;	// (NOT (BETWEEN a b) ) => (NOT_BETWEEN a b)
				case NOT_BETWEEN:
					node.Type = BETWEEN;
					node.Text = "{not}" + node.Text;
					return node;	// (NOT (NOT_BETWEEN a b) ) => (BETWEEN a b)
				/* This can never happen because this rule will always eliminate the child NOT.
							case NOT:
								return x.getFirstChild();			// (NOT (NOT x) ) => (x)
				*/
				default:
					IASTNode not = (IASTNode) TreeAdaptor.Create(NOT, "not");
					not.AddChild(node);
					return not;
			}
		}

		public IASTNode ProcessEqualityExpression(object o)
		{
			IASTNode x = o as IASTNode;

			if (x == null)
			{
				log.Warn("processEqualityExpression() : No expression to process!");
				return null;
			}

			int type = x.Type;
			if (type == EQ || type == NE)
			{
				bool negated = type == NE;

				if (x.ChildCount == 2)
				{
					IASTNode a = x.GetChild(0);
					IASTNode b = x.GetChild(1);
					// (EQ NULL b) => (IS_NULL b)
					if (a.Type == NULL && b.Type != NULL)
					{
						return CreateIsNullParent(b, negated);
					}
					// (EQ a NULL) => (IS_NULL a)
					if (b.Type == NULL && a.Type != NULL)
					{
						return CreateIsNullParent(a, negated);
					}
					if (b.Type == EMPTY)
					{
						return ProcessIsEmpty(a, negated);
					}
				}
			}

			return x;
		}

		public void HandleDotIdent()
		{
			// This handles HHH-354, where there is a strange property name in a where clause.
			// If the lookahead contains a DOT then something that isn't an IDENT...
			if (input.LA(1) == DOT && input.LA(2) != IDENT)
			{
				// See if the second lookahed token can be an identifier.
				HqlToken t = input.LT(2) as HqlToken;
				if (t != null && t.PossibleId)
				{
					// Set it!
					input.LT(2).Type = IDENT;
					if (log.IsDebugEnabled)
					{
						log.Debug("handleDotIdent() : new LT(2) token - " + input.LT(1));
					}
				}
			}
		}

		private IASTNode CreateIsNullParent(IASTNode node, bool negated)
		{
			int type = negated ? IS_NOT_NULL : IS_NULL;
			string text = negated ? "is not null" : "is null";

			return (IASTNode)adaptor.BecomeRoot(adaptor.Create(type, text), node);
		}

		private IASTNode ProcessIsEmpty(IASTNode node, bool negated)
		{
			// NOTE: Because we're using ASTUtil.createParent(), the tree must be created from the bottom up.
			// IS EMPTY x => (EXISTS (QUERY (SELECT_FROM (FROM x) ) ) )

			IASTNode ast = CreateSubquery(node);

			ast = (IASTNode)adaptor.BecomeRoot(adaptor.Create(EXISTS, "exists"), ast);

			// Add NOT if it's negated.
			if (!negated)
			{
				ast = (IASTNode)adaptor.BecomeRoot(adaptor.Create(NOT, "not"), ast);
			}
			return ast;
		}

		private IASTNode CreateSubquery(IASTNode node)
		{
			return (IASTNode)adaptor.BecomeRoot(
							   adaptor.Create(QUERY, "QUERY"),
							   adaptor.BecomeRoot(
								   adaptor.Create(SELECT_FROM, "SELECT_FROM"),
								   adaptor.BecomeRoot(
									   adaptor.Create(FROM, "from"),
									   adaptor.BecomeRoot(
										adaptor.Create(RANGE, "RANGE"),
										node)))
							   );
		}

		public IASTNode ProcessMemberOf(IToken n, IASTNode p, IASTNode root)
		{
			ASTFactory factory = new ASTFactory(adaptor);

			return factory.CreateNode(n == null ? IN : NOT_IN,
			                          n == null ? "in" : "not in",
			                          root.IsNil && root.ChildCount == 1 ? root.GetChild(0) : root,
			                          factory.CreateNode(IN_LIST, "inList",
			                                             CreateSubquery(p)));
		}

		public IASTNode HandleIdentifierError(IToken token, RecognitionException ex)
		{
			if (token is HqlToken)
			{
				HqlToken hqlToken = (HqlToken)token;

				// ... and the token could be an identifer and the error is
				// a mismatched token error ...
				if (hqlToken.PossibleId && (ex is MismatchedTokenException))
				{
					MismatchedTokenException mte = (MismatchedTokenException)ex;

					// ... and the expected token type was an identifier, then:
					if (mte.Expecting == IDENT)
					{
						// Use the token as an identifier.
						_parseErrorHandler.ReportWarning("Keyword  '"
								+ token.Text
								+ "' is being interpreted as an identifier due to: " + mte.Message);

						// Add the token to the AST.

						token.Type = WEIRD_IDENT;

						input.Consume();
						return (IASTNode) adaptor.Create(token);
					}
				} 
			}
			
			// Otherwise, handle the error normally.
			throw ex;
		}
	}
}
