using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using NHibernate.Dialect.Function;
using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Param;
using NHibernate.SqlCommand;

namespace NHibernate.Hql.Ast.ANTLR
{
	/// <summary>
	/// Generates SQL by overriding callback methods in the base class, which does
	/// the actual SQL AST walking.
	/// Author: Joshua Davis, Steve Ebersole
	/// Ported By: Steve Strong
	/// </summary>
	[CLSCompliant(false)]
	public partial class SqlGenerator : IErrorReporter
	{
		private readonly List<IParameterSpecification> collectedParameters = new List<IParameterSpecification>();

		/**
		 * all append invocations on the buf should go through this Output instance variable.
		 * The value of this variable may be temporarily substitued by sql function processing code
		 * to catch generated arguments.
		 * This is because sql function templates need arguments as separate string chunks
		 * that will be assembled into the target dialect-specific function call.
		 */
		private readonly List<ISqlWriter> outputStack = new List<ISqlWriter>();

		/// <summary>
		/// Handles parser errors.
		/// </summary>
		private readonly IParseErrorHandler parseErrorHandler;

		private readonly ISessionFactoryImplementor sessionFactory;

		private readonly SqlStringBuilder sqlStringBuilder = new SqlStringBuilder();
		private ISqlWriter writer;

		public SqlGenerator(ISessionFactoryImplementor sfi, ITreeNodeStream input) : this(input)
		{
			parseErrorHandler = new ErrorCounter();
			sessionFactory = sfi;
			writer = new DefaultWriter(this);
		}

		public IParseErrorHandler ParseErrorHandler
		{
			get { return parseErrorHandler; }
		}

		#region IErrorReporter Members

		public override void ReportError(RecognitionException e)
		{
			parseErrorHandler.ReportError(e); // Use the delegate.
		}

		public void ReportError(String s)
		{
			parseErrorHandler.ReportError(s); // Use the delegate.
		}

		public void ReportWarning(String s)
		{
			parseErrorHandler.ReportWarning(s);
		}

		#endregion

		public SqlString GetSQL()
		{
			return sqlStringBuilder.ToSqlString();
		}

		public IList<IParameterSpecification> GetCollectedParameters()
		{
			return collectedParameters;
		}

		private void Out(string s)
		{
			writer.Clause(s);
		}

		private void Out(SqlString s)
		{
			writer.Clause(s);
		}

		/// <summary>
		/// Add a space if the previous token was not a space or a parenthesis.
		/// </summary>
		private void OptionalSpace()
		{
			int len = sqlStringBuilder.Count;
			if (len == 0)
			{
				return;
			}
			else
			{
				string lastPart = sqlStringBuilder[len - 1].ToString();
				if (lastPart.Length == 0)
				{
					return;
				}
				switch (lastPart[lastPart.Length - 1])
				{
					case ' ':
						return;
					case ')':
						return;
					case '(':
						return;
				}
			}
			Out(" ");
		}

		private void Out(IASTNode n)
		{
			var parameterNode = n as ParameterNode;
			if (parameterNode != null)
			{
				var parameter = Parameter.Placeholder;
				// supposed to be simplevalue
				parameter.BackTrack = parameterNode.HqlParameterSpecification.GetIdsForBackTrack(sessionFactory).Single();
				writer.PushParameter(parameter);
			}
			else if (n is SqlNode)
			{
				Out(((SqlNode) n).RenderText(sessionFactory));
			}
			else
			{
				Out(n.Text);
			}

			if (parameterNode != null)
			{
				collectedParameters.Add(parameterNode.HqlParameterSpecification);
			}
			else if (n is IParameterContainer)
			{
				var parameterContainer = (IParameterContainer) n;
				if (parameterContainer.HasEmbeddedParameters)
				{
					IParameterSpecification[] specifications = parameterContainer.GetEmbeddedParameters();
					collectedParameters.AddRange(specifications);
				}
			}
		}

		private void Separator(IASTNode n, String sep)
		{
			if (n.NextSibling != null)
			{
				Out(sep);
			}
		}

		private static bool HasText(IASTNode a)
		{
			return !string.IsNullOrEmpty(a.Text);
		}

		protected virtual void FromFragmentSeparator(IASTNode a)
		{
			// check two "adjacent" nodes at the top of the from-clause tree
			IASTNode next = a.NextSibling;
			if (next == null || !HasText(a))
			{
				return;
			}

			var left = (FromElement) a;
			var right = (FromElement) next;

			///////////////////////////////////////////////////////////////////////
			// HACK ALERT !!!!!!!!!!!!!!!!!!!!!!!!!!!!
			// Attempt to work around "ghost" ImpliedFromElements that occasionally
			// show up between the actual things being joined.  This consistently
			// occurs from index nodes (at least against many-to-many).  Not sure
			// if there are other conditions
			//
			// Essentially, look-ahead to the next FromElement that actually
			// writes something to the SQL
			while (right != null && !HasText(right))
			{
				right = (FromElement) right.NextSibling;
			}

			if (right == null)
			{
				return;
			}
			///////////////////////////////////////////////////////////////////////

			if (!HasText(right))
			{
				return;
			}

			if (right.RealOrigin == left || (right.RealOrigin != null && right.RealOrigin == left.RealOrigin))
			{
				// right represents a joins originating from left; or
				// both right and left reprersent joins originating from the same FromElement
				if (right.JoinSequence != null && right.JoinSequence.IsThetaStyle)
				{
					Out(", ");
				}
				else
				{
					Out(" ");
				}
			}
			else
			{
				// these are just two unrelated table references
				Out(", ");
			}
		}

		protected virtual void NestedFromFragment(IASTNode d, IASTNode parent)
		{
			// check a set of parent/child nodes in the from-clause tree
			// to determine if a comma is required between them
			if (d != null && HasText(d))
			{
				if (parent != null && HasText(parent))
				{
					// again, both should be FromElements
					var left = (FromElement) parent;
					var right = (FromElement) d;
					if (right.RealOrigin == left)
					{
						// right represents a joins originating from left...
						if (right.JoinSequence != null && right.JoinSequence.IsThetaStyle)
						{
							Out(", ");
						}
						else
						{
							Out(" ");
						}
					}
					else
					{
						// not so sure this is even valid subtree.  but if it was, it'd
						// represent two unrelated table references...
						Out(", ");
					}
				}

				Out(d);
			}
		}

		private SqlStringBuilder GetStringBuilder()
		{
			return sqlStringBuilder;
		}

		private void BeginFunctionTemplate(IASTNode m, IASTNode i)
		{
			var methodNode = (MethodNode) m;
			ISQLFunction template = methodNode.SQLFunction;
			if (template == null)
			{
				// if template is null we just write the function out as it appears in the hql statement
				Out(i);
				Out("(");
			}
			else
			{
				// this function has a template -> redirect output and catch the arguments
				outputStack.Insert(0, writer);
				writer = new FunctionArguments();
			}
		}

		private void EndFunctionTemplate(IASTNode m)
		{
			var methodNode = (MethodNode) m;
			ISQLFunction template = methodNode.SQLFunction;
			if (template == null)
			{
				Out(")");
			}
			else
			{
				// this function has a template -> restore output, apply the template and write the result out
				var functionArguments = (FunctionArguments) writer; // TODO: Downcast to avoid using an interface?  Yuck.
				writer = outputStack[0];
				outputStack.RemoveAt(0);
				Out(template.Render(functionArguments.Args, sessionFactory));
			}
		}

		private void CommaBetweenParameters(String comma)
		{
			writer.CommaBetweenParameters(comma);
		}

		private void StartQuery()
		{
			outputStack.Insert(0, writer);
			writer = new QueryWriter();
		}

		private void EndQuery()
		{
			SqlString sqlString = GetSqlStringWithLimitsIfNeeded((QueryWriter) writer);

			writer = outputStack[0];
			outputStack.RemoveAt(0);
			Out(sqlString);
		}

		private SqlString GetSqlStringWithLimitsIfNeeded(QueryWriter queryWriter)
		{
			Parameter skipParameter = null;
			Parameter takeParameter = null;
			if (queryWriter.SkipParameter != null)
			{
				queryWriter.SkipParameter.ExpectedType = NHibernateUtil.Int32;
				queryWriter.SkipParameter.IsSkipParameter();
				skipParameter = Parameter.Placeholder;
				skipParameter.BackTrack = queryWriter.SkipParameter.GetIdsForBackTrack(sessionFactory).First();
			}
			if (queryWriter.TakeParameter != null)
			{
				queryWriter.TakeParameter.ExpectedType = NHibernateUtil.Int32;
				queryWriter.TakeParameter.IsTakeParameterWithSkipParameter(queryWriter.SkipParameter);
				takeParameter = Parameter.Placeholder;
				takeParameter.BackTrack = queryWriter.TakeParameter.GetIdsForBackTrack(sessionFactory).First();
			}

			// We allow the user to specify either constants or parameters for their limits.
			// The dialect can move the given parameters where he need, what it can't do is generates new parameters, losing the BackTrack.
			var dialect = sessionFactory.Dialect;
			return dialect.GetLimitString(queryWriter.ToSqlString(),
			                              queryWriter.Skip.HasValue
				                              ? (int?) dialect.GetOffsetValue(queryWriter.Skip.Value)
				                              : null,
			                              queryWriter.Take.HasValue
				                              ? (int?) dialect.GetLimitValue(queryWriter.Skip ?? 0, queryWriter.Take.Value)
				                              : null,
			                              skipParameter,
			                              takeParameter);
		}

		private void Skip(IASTNode node)
		{
			var queryWriter = (QueryWriter) writer;
			var pnode = node as ParameterNode;
			if (pnode != null)
			{
				queryWriter.SkipParameter = (IPageableParameterSpecification) pnode.HqlParameterSpecification;
				collectedParameters.Add(pnode.HqlParameterSpecification);
				return;
			}
			queryWriter.Skip = Convert.ToInt32(node.Text);
		}

		private void Take(IASTNode node)
		{
			var queryWriter = (QueryWriter) writer;
			var pnode = node as ParameterNode;
			if (pnode != null)
			{
				queryWriter.TakeParameter = (IPageableParameterSpecification) pnode.HqlParameterSpecification;
				collectedParameters.Add(pnode.HqlParameterSpecification);
				return;
			}
			queryWriter.Take = Convert.ToInt32(node.Text);
		}

		#region Nested type: DefaultWriter

		/// <summary>
		/// The default SQL writer.
		/// </summary>
		private class DefaultWriter : ISqlWriter
		{
			private readonly SqlGenerator generator;

			internal DefaultWriter(SqlGenerator generator)
			{
				this.generator = generator;
			}

			#region ISqlWriter Members

			public void Clause(String clause)
			{
				generator.GetStringBuilder().Add(clause);
			}

			public void Clause(SqlString clause)
			{
				generator.GetStringBuilder().Add(clause);
			}

			public void PushParameter(Parameter parameter)
			{
				generator.GetStringBuilder().Add(parameter);
			}

			public void CommaBetweenParameters(String comma)
			{
				generator.GetStringBuilder().Add(comma);
			}

			#endregion
		}

		#endregion

		#region Nested type: QueryWriter

		/// <summary>
		/// The default SQL writer.
		/// </summary>
		private class QueryWriter : ISqlWriter
		{
			private readonly SqlStringBuilder builder = new SqlStringBuilder();

			public IPageableParameterSpecification TakeParameter { get; set; }
			public IPageableParameterSpecification SkipParameter { get; set; }
			public int? Skip { get; set; }
			public int? Take { get; set; }

			#region ISqlWriter Members

			public void Clause(String clause)
			{
				builder.Add(clause);
			}

			public void Clause(SqlString clause)
			{
				builder.Add(clause);
			}

			public void PushParameter(Parameter parameter)
			{
				builder.Add(parameter);
			}

			public void CommaBetweenParameters(String comma)
			{
				builder.Add(comma);
			}

			public SqlString ToSqlString()
			{
				return builder.ToSqlString();
			}


			#endregion
		}

		#endregion

		#region Nested type: FunctionArguments

		private class FunctionArguments : ISqlWriter
		{
			//private readonly List<object> args = new List<object>();
			private readonly List<SqlString> args = new List<SqlString>();
			private int argInd;

			public IList Args
			{
				get { return args; }
			}

			#region ISqlWriter Members

			public void Clause(string clause)
			{
				Clause(SqlString.Parse(clause));
				/*
				if (argInd == args.Count)
				{
					args.Add(clause);
				}
				else
				{
					args[argInd] = args[argInd] + clause;
				}
				 */
			}

			public void Clause(SqlString clause)
			{
				//Clause(clause.ToString());
				if (argInd == args.Count)
				{
					args.Add(clause);
				}
				else
				{
					args[argInd] = args[argInd].Append(clause);
				}
			}

			public void PushParameter(Parameter parameter)
			{
				if (argInd == args.Count)
				{
					args.Add(new SqlString(parameter));
				}
				else
				{
					args[argInd] = args[argInd].Append(new SqlString(parameter));
				}
			}

			public void CommaBetweenParameters(string comma)
			{
				++argInd;
			}

			#endregion
		}

		#endregion

		#region Nested type: ISqlWriter

		/// <summary>
		/// Writes SQL fragments.
		/// </summary>
		private interface ISqlWriter
		{
			void Clause(string clause);
			void Clause(SqlString clause);
			void PushParameter(Parameter parameter);
			/**
			 * todo remove this hack
			 * The parameter is either ", " or " , ". This is needed to pass sql generating tests as the old
			 * sql generator uses " , " in the WHERE and ", " in SELECT.
			 *
			 * @param comma either " , " or ", "
			 */
			void CommaBetweenParameters(string comma);
		}

		#endregion
	}
}