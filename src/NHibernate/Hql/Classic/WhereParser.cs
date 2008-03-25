using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Hql.Util;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Hql.Classic
{
	/// <summary> Parses the where clause of a hibernate query and translates it to an
	/// SQL where clause.
	/// </summary>
	// We should reengineer this class so that, rather than the current ad -
	// hoc linear approach to processing a stream of tokens, we instead
	// build up a tree of expressions.
	// We would probably refactor to have LogicParser (builds a tree of simple
	// expressions connected by and, or, not), ExpressionParser (translates
	// from OO terms like foo, foo.Bar, foo.Bar.Baz to SQL terms like
	// FOOS.ID, FOOS.BAR_ID, etc) and PathExpressionParser (which does much
	// the same thing it does now)
	public class WhereParser : IParser
	{
		private readonly PathExpressionParser pathExpressionParser = new PathExpressionParser();

		private static readonly ISet<string> expressionTerminators = new HashedSet<string>(); //tokens that close a sub expression
		private static readonly ISet<string> expressionOpeners = new HashedSet<string>(); //tokens that open a sub expression

		private static readonly ISet<string> booleanOperators = new HashedSet<string>();
		                    //tokens that would indicate a sub expression is a boolean expression

		private static readonly Dictionary<string, string> negations = new Dictionary<string, string>();

		// To parse correctly the functions TRIM and EXTRACT (Note subselect inside TRIM is not supported)
		private static readonly ISet<string> specialFunctions = new HashedSet<string>();

		public WhereParser()
		{
			pathExpressionParser.UseThetaStyleJoin = true;
		}

		static WhereParser()
		{
			expressionTerminators.Add("and");
			expressionTerminators.Add("or");
			expressionTerminators.Add(StringHelper.ClosedParen);
			//expressionTerminators.Add(","); // deliberately excluded

			expressionOpeners.Add("and");
			expressionOpeners.Add("or");
			expressionOpeners.Add(StringHelper.OpenParen);
			//expressionOpeners.Add(","); // deliberately excluded

			booleanOperators.Add("<");
			booleanOperators.Add("=");
			booleanOperators.Add(">");
			booleanOperators.Add("#");
			booleanOperators.Add("~");
			booleanOperators.Add("like");
			booleanOperators.Add("ilike");
			booleanOperators.Add("is");
			booleanOperators.Add("in");
			booleanOperators.Add("any");
			booleanOperators.Add("some");
			booleanOperators.Add("all");
			booleanOperators.Add("exists");
			booleanOperators.Add("between");
			booleanOperators.Add("<=");
			booleanOperators.Add(">=");
			booleanOperators.Add("=>");
			booleanOperators.Add("=<");
			booleanOperators.Add("!=");
			booleanOperators.Add("<>");
			booleanOperators.Add("!#");
			booleanOperators.Add("!~");
			booleanOperators.Add("!<");
			booleanOperators.Add("!>");
			booleanOperators.Add("is not");
			booleanOperators.Add("not like");
			booleanOperators.Add("not ilike");
			booleanOperators.Add("not in");
			booleanOperators.Add("not between");
			booleanOperators.Add("not exists");

			negations.Add("and", "or");
			negations.Add("or", "and");
			negations.Add("<", ">=");
			negations.Add("=", "<>");
			negations.Add(">", "<=");
			negations.Add("#", "!#");
			negations.Add("~", "!~");
			negations.Add("like", "not like");
			negations.Add("ilike", "not ilike");
			negations.Add("is", "is not");
			negations.Add("in", "not in");
			negations.Add("exists", "not exists");
			negations.Add("between", "not between");
			negations.Add("<=", ">");
			negations.Add(">=", "<");
			negations.Add("=>", "<");
			negations.Add("=<", ">");
			negations.Add("!=", "=");
			negations.Add("<>", "=");
			negations.Add("!#", "#");
			negations.Add("!~", "~");
			negations.Add("!<", ">=");
			negations.Add("!>", "<=");
			negations.Add("is not", "is");
			negations.Add("not like", "like");
			negations.Add("not in", "in");
			negations.Add("not between", "between");
			negations.Add("not exists", "exists");

			specialFunctions.Add("trim");
			specialFunctions.Add("extract");

		}

		// Handles things like:
		// a and b or c
		// a and ( b or c )
		// not a and not b
		// not ( a and b )
		// x between y and z            (overloaded "and")
		// x in ( a, b, c )             (overloaded brackets)
		// not not a
		// a is not null                (overloaded "not")
		// etc......
		// and expressions like
		// foo = bar                    (maps to: foo.id = bar.id)
		// foo.Bar = 'foo'              (maps to: foo.bar = 'foo')
		// foo.Bar.Baz = 1.0            (maps to: foo.bar = bar.id and bar.baz = 1.0)
		// 1.0 = foo.Bar.Baz            (maps to: bar.baz = 1.0 and foo.Bar = bar.id)
		// foo.Bar.Baz = a.B.C          (maps to: bar.Baz = b.C and foo.Bar = bar.id and a.B = b.id)
		// foo.Bar.Baz + a.B.C          (maps to: bar.Baz + b.C and foo.Bar = bar.id and a.B = b.id)
		// ( foo.Bar.Baz + 1.0 ) < 2.0  (maps to: ( bar.Baz + 1.0 ) < 2.0 and foo.Bar = bar.id)

		private bool betweenSpecialCase = false; //Inside a BETWEEN ... AND ... expression
		private bool negated = false;

		private bool inSubselect = false;
		private int bracketsSinceSelect = 0;
		private StringBuilder subselect;

		private bool isInSpecialFunctionClause = false;
		private int specialFunctionParenCount = 0;

		private bool expectingPathContinuation = false;
		private int expectingIndex = 0;

		// The following variables are stacks that keep information about each subexpression
		// in the list of nested subexpressions we are currently processing.

		//were an odd or even number of NOTs encountered
		private readonly List<bool> nots = new List<bool>();

		//the join string built up by compound paths inside this expression 
		private readonly List<SqlStringBuilder> joins = new List<SqlStringBuilder>();

		//a flag indicating if the subexpression is known to be boolean		
		private readonly List<bool> booleanTests = new List<bool>();

		private string GetElementName(PathExpressionParser.CollectionElement element, QueryTranslator q)
		{
			string name;
			if (element.IsOneToMany)
			{
				name = element.Alias;
			}
			else
			{
				IType type = element.Type;

				if (type.IsEntityType)
				{
					//ie. a many-to-many
					string clazz = ((EntityType) type).GetAssociatedEntityName();
					name = pathExpressionParser.ContinueFromManyToMany(clazz, element.ElementColumns, q);
				}
				else
				{
					throw new QueryException("illegally dereferenced collection element");
				}
			}
			return name;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="token"></param>
		/// <param name="q"></param>
		public void Token(string token, QueryTranslator q)
		{
			string lcToken = token.ToLowerInvariant();

			//Cope with [,]

			if (token.Equals("[") && !expectingPathContinuation)
			{
				expectingPathContinuation = false;
				if (expectingIndex == 0)
				{
					throw new QueryException("unexpected [");
				}
				return;
			}
			else if (token.Equals("]"))
			{
				expectingIndex--;
				expectingPathContinuation = true;
				return;
			}

			//Cope with a continued path expression (ie. ].baz)
			if (expectingPathContinuation)
			{
				if (ContinuePathExpression(token, q)) return;
			}

			//Cope with a subselect
			if (!inSubselect && (lcToken.Equals("select") || (lcToken.Equals("from") && !isInSpecialFunctionClause)))
			{
				inSubselect = true;
				subselect = new StringBuilder(20);
			}
			if (inSubselect && token.Equals(StringHelper.ClosedParen))
			{
				bracketsSinceSelect--;

				if (bracketsSinceSelect == -1)
				{
					QueryTranslator subq = new QueryTranslator(q.Factory, subselect.ToString(), q.EnabledFilters);
					try
					{
						subq.Compile(q);
					}
					catch (MappingException me)
					{
						throw new QueryException("MappingException occurred compiling subquery", me);
					}

					AppendToken(q, subq.SqlString);
					inSubselect = false;
					bracketsSinceSelect = 0;
				}
			}
			if (inSubselect)
			{
				if (token.Equals(StringHelper.OpenParen))
				{
					bracketsSinceSelect++;
				}
				subselect.Append(token).Append(' ');
				return;
			}

			//Cope with special cases of AND, NOT, ()
			SpecialCasesBefore(lcToken);

			//Close extra brackets we opened
			if (!betweenSpecialCase && expressionTerminators.Contains(lcToken))
			{
				CloseExpression(q, lcToken);
			}

			//take note when this is a boolean expression

			if (booleanOperators.Contains(lcToken))
			{
				booleanTests.RemoveAt(booleanTests.Count - 1);
				booleanTests.Add(true);
			}

			if (lcToken.Equals("not"))
			{
				nots[nots.Count - 1] = !nots[nots.Count - 1];
				negated = !negated;
				return; //NOTE: early return
			}

			if (!isInSpecialFunctionClause && specialFunctions.Contains(lcToken))
			{
				isInSpecialFunctionClause = true;
			}
			if (isInSpecialFunctionClause && token.Equals(StringHelper.OpenParen))
			{
				specialFunctionParenCount++;
			}
			if (isInSpecialFunctionClause && token.Equals(StringHelper.ClosedParen))
			{
				specialFunctionParenCount--;
				isInSpecialFunctionClause = specialFunctionParenCount > 0;
			}
			//process a token, mapping OO path expressions to SQL expressions
			DoToken(token, q);

			//Open any extra brackets we might need.

			if (!betweenSpecialCase && expressionOpeners.Contains(lcToken))
			{
				OpenExpression(q, lcToken);
			}

			//Cope with special cases of AND, NOT, )
			SpecialCasesAfter(lcToken);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="q"></param>
		public void Start(QueryTranslator q)
		{
			Token(StringHelper.OpenParen, q);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="q"></param>
		public void End(QueryTranslator q)
		{
			if (expectingPathContinuation)
			{
				expectingPathContinuation = false;
				PathExpressionParser.CollectionElement element = pathExpressionParser.LastCollectionElement();
				if (element.ElementColumns.Length != 1)
				{
					throw new QueryException("path expression ended in composite collection element");
				}
				AppendToken(q, element.ElementColumns[0]);
				AddToCurrentJoin(element);
			}
			Token(StringHelper.ClosedParen, q);
		}

		private void CloseExpression(QueryTranslator q, string lcToken)
		{
			bool lastBoolTest = booleanTests[booleanTests.Count - 1];
			booleanTests.RemoveAt(booleanTests.Count - 1);
			if (lastBoolTest)
			{
				//it was a boolean expression
				if (booleanTests.Count > 0)
				{
					// the next one up must also be
					booleanTests[booleanTests.Count - 1] = true;
				}

				// Add any joins
				SqlStringBuilder lastJoin = joins[joins.Count - 1];
				joins.RemoveAt(joins.Count - 1);
				AppendToken(q, lastJoin.ToSqlString());
			}
			else
			{
				//unaryCounts.removeLast(); //check that its zero? (As an assertion)
				SqlStringBuilder join = joins[joins.Count - 1];
				joins.RemoveAt(joins.Count - 1);
				joins[joins.Count - 1].Add(join.ToSqlString());
			}

			bool lastNots = nots[nots.Count - 1];
			nots.RemoveAt(nots.Count - 1);
			if (lastNots)
			{
				negated = !negated;
			}

			if (!StringHelper.ClosedParen.Equals(lcToken))
			{
				AppendToken(q, StringHelper.ClosedParen);
			}
		}

		private void OpenExpression(QueryTranslator q, string lcToken)
		{
			nots.Add(false);
			booleanTests.Add(false);
			joins.Add(new SqlStringBuilder());
			if (!StringHelper.OpenParen.Equals(lcToken))
			{
				AppendToken(q, StringHelper.OpenParen);
			}
		}

		private void Preprocess(string token, QueryTranslator q)
		{
			// ugly hack for cases like "foo.bar.collection.elements" 
			// (multi-part path expression ending in elements or indices) 
			string[] tokens = StringHelper.Split(".", token, true);
			if (tokens.Length > 5 &&
			    ("elements".Equals(tokens[tokens.Length - 1]) || "indices".Equals(tokens[tokens.Length - 1])))
			{
				pathExpressionParser.Start(q);
				for (int i = 0; i < tokens.Length - 3; i++)
				{
					pathExpressionParser.Token(tokens[i], q);
				}
				pathExpressionParser.Token(null, q);
				pathExpressionParser.End(q);
				AddJoin(pathExpressionParser.WhereJoin, q);
				pathExpressionParser.IgnoreInitialJoin();
			}
		}

		private void DoPathExpression(string token, QueryTranslator q)
		{
			Preprocess(token, q);

			StringTokenizer tokens = new StringTokenizer(token, ".", true);
			pathExpressionParser.Start(q);
			foreach (string tok in tokens)
			{
				pathExpressionParser.Token(tok, q);
			}
			pathExpressionParser.End(q);

			if (pathExpressionParser.IsCollectionValued)
			{
				OpenExpression(q, string.Empty);
				AppendToken(q, pathExpressionParser.GetCollectionSubquery(q.EnabledFilters));
				CloseExpression(q, string.Empty);
				// this is ugly here, but needed because its a subquery
				q.AddQuerySpaces(q.GetCollectionPersister(pathExpressionParser.CollectionRole).CollectionSpaces);
			}
			else
			{
				if (pathExpressionParser.IsExpectingCollectionIndex)
				{
					expectingIndex++;
				}
				else
				{
					AddJoin(pathExpressionParser.WhereJoin, q);
					AppendToken(q, pathExpressionParser.WhereColumn);
				}
			}
		}

		private void AddJoin(JoinSequence joinSequence, QueryTranslator q)
		{
			q.AddFromJoinOnly(pathExpressionParser.Name, joinSequence);
			try
			{
				AddToCurrentJoin(joinSequence.ToJoinFragment(q.EnabledFilters, true).ToWhereFragmentString);
			}
			catch (MappingException me)
			{
				throw new QueryException(me);
			}
		}

		private void DoToken(string token, QueryTranslator q)
		{
			if (q.IsName(StringHelper.Root(token))) //path expression
			{
				DoPathExpression(q.Unalias(token), q);
			}
			else if (token.StartsWith(ParserHelper.HqlVariablePrefix)) //named query parameter
			{
				q.AddNamedParameter(token.Substring(1));
				// this is only a temporary parameter to help with the parsing of hql - 
				// when the type becomes known then this will be converted to its real
				// parameter type.
				AppendToken(q, SqlString.Parameter);
			}
			else if (token.Equals(StringHelper.SqlParameter))
			{
				//if the token is a "?" then we have a Parameter so convert it to a SqlCommand.Parameter
				// instead of appending a "?" to the WhereTokens
				AppendToken(q, SqlString.Parameter);
			}
			else
			{
				IQueryable persister = q.GetPersisterUsingImports(token);
				if (persister != null) // the name of a class
				{
					string discrim = persister.DiscriminatorSQLValue;
					if (InFragment.Null == discrim || InFragment.NotNull == discrim)
					{
						throw new QueryException("subclass test not allowed for null or not null discriminator");
					}
					AppendToken(q, discrim);
				}
				else
				{
					object constant;
					string fieldName = null;
					System.Type importedType = null;

					int indexOfDot = token.IndexOf(StringHelper.Dot);
					// don't even bother to do the lookups if the indexOfDot is not 
					// greater than -1.  This will save all the string modifications.

					// This allows us to resolve to the full type before obtaining the value e.g. FooStatus.OFF -> NHibernate.Model.FooStatus.OFF
					if (indexOfDot > -1)
					{
						fieldName = StringHelper.Unqualify(token);
						string typeName = StringHelper.Qualifier(token);
						importedType = SessionFactoryHelper.GetImportedClass(q.Factory, typeName);
					}

					if (indexOfDot > -1 && importedType != null &&
							(constant = ReflectHelper.GetConstantValue(importedType, fieldName)) != null)
					{
						// need to get the NHibernate Type so we can convert the Enum or field from 
						// a class into it's string representation for hql.
						IType type;
						try
						{
							type = TypeFactory.HeuristicType(constant.GetType().AssemblyQualifiedName);
						}
						catch (MappingException me)
						{
							throw new QueryException(me);
						}

						if (type == null)
						{
							throw new QueryException(string.Format("Could not determin the type of: {0}", token));
						}

						try
						{
							AppendToken(q, ((ILiteralType)type).ObjectToSQLString(constant, q.Factory.Dialect));
						}
						catch (Exception e)
						{
							throw new QueryException("Could not format constant value to SQL literal: " + token, e);
						}
					}
					else
					{
						//anything else
						string negatedToken = null;
						if (negated)
							negations.TryGetValue(token.ToLowerInvariant(), out negatedToken);
						if (negatedToken != null && (!betweenSpecialCase || !"or".Equals(negatedToken)))
						{
							AppendToken(q, negatedToken);
						}
						else
						{
							AppendToken(q, token);
						}
					}
				}
			}
		}

		private void AddToCurrentJoin(SqlString sql)
		{
			joins[joins.Count - 1].Add(sql);
		}

		private void AddToCurrentJoin(PathExpressionParser.CollectionElement ce)
		{
			try
			{
				AddToCurrentJoin(ce.JoinSequence.ToJoinFragment().ToWhereFragmentString + ce.IndexValue.ToSqlString());
			}
			catch (MappingException me)
			{
				throw new QueryException(me);
			}
		}

		private void SpecialCasesBefore(string lcToken)
		{
			if ("between".Equals(lcToken) || "not between".Equals(lcToken))
			{
				betweenSpecialCase = true;
			}
		}

		private void SpecialCasesAfter(string lcToken)
		{
			if (betweenSpecialCase && "and".Equals(lcToken))
			{
				betweenSpecialCase = false;
			}
		}

		protected virtual void AppendToken(QueryTranslator q, string token)
		{
			if (expectingIndex > 0)
			{
				pathExpressionParser.SetLastCollectionElementIndexValue(new SqlString(token));
			}
			else
			{
				// a String.Empty can get passed in here.  If that occurs
				// then don't create a new SqlString for it - just ignore
				// it since it adds nothing to the sql being generated.
				if (token != null && token.Length > 0)
				{
					q.AppendWhereToken(new SqlString(token));
				}
			}
		}

		protected virtual void AppendToken(QueryTranslator q, SqlString token)
		{
			if (expectingIndex > 0)
			{
				pathExpressionParser.SetLastCollectionElementIndexValue(token);
			}
			else
			{
				q.AppendWhereToken(token);
			}
		}

		private bool ContinuePathExpression(string token, QueryTranslator q)
		{
			expectingPathContinuation = false;

			PathExpressionParser.CollectionElement element = pathExpressionParser.LastCollectionElement();

			if (token.StartsWith("."))
			{
				// the path expression continues after a ]

				DoPathExpression(GetElementName(element, q) + token, q); // careful with this!

				AddToCurrentJoin(element);
				return true; //NOTE: EARLY EXIT!
			}
			else
			{
				// the path expression ends at the ]
				if (element.ElementColumns.Length != 1)
				{
					throw new QueryException("path expression ended in composite collection element");
				}
				AppendToken(q, element.ElementColumns[0]);
				AddToCurrentJoin(element);
				return false;
			}
		}
	}
}