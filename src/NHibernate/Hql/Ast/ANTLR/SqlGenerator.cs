using System;
using System.Collections;
using System.Collections.Generic;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using NHibernate.Dialect.Function;
using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR.Parameters;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.SqlCommand;

namespace NHibernate.Hql.Ast.ANTLR
{
	/// <summary>
	/// Generates SQL by overriding callback methods in the base class, which does
	/// the actual SQL AST walking.
	/// Author: Joshua Davis, Steve Ebersole
	/// Ported By: Steve Strong
	/// </summary>
	public partial class SqlGenerator : IErrorReporter
	{
		/// <summary>
		/// Handles parser errors.
		/// </summary>
		private readonly IParseErrorHandler _parseErrorHandler;

		private readonly ISessionFactoryImplementor _sessionFactory;

		private readonly List<IParameterSpecification> _collectedParameters = new List<IParameterSpecification>();

		/**
		 * all append invocations on the buf should go through this Output instance variable.
		 * The value of this variable may be temporarily substitued by sql function processing code
		 * to catch generated arguments.
		 * This is because sql function templates need arguments as seperate string chunks
		 * that will be assembled into the target dialect-specific function call.
		 */
		private ISqlWriter _writer;

		private readonly List<ISqlWriter> _outputStack = new List<ISqlWriter>();

		private readonly SqlStringBuilder _sqlStringBuilder = new SqlStringBuilder();

		public SqlGenerator(ISessionFactoryImplementor sfi, ITreeNodeStream input) : this(input)
		{
			_parseErrorHandler = new ErrorCounter();
			_sessionFactory = sfi;
			_writer = new DefaultWriter(this);
		}

        public override void ReportError(RecognitionException e)
		{
			_parseErrorHandler.ReportError(e); // Use the delegate.
		}

		public void ReportError(String s)
		{
			_parseErrorHandler.ReportError(s); // Use the delegate.
		}

		public void ReportWarning(String s)
		{
			_parseErrorHandler.ReportWarning(s);
		}

		public SqlString GetSQL()
		{
			return _sqlStringBuilder.ToSqlString();
		}

		public IList<IParameterSpecification> GetCollectedParameters()
		{
			return _collectedParameters;
		}

		public IParseErrorHandler ParseErrorHandler
		{
			get { return _parseErrorHandler; }
		}

		private void Out(string s) 
		{
			_writer.Clause(s);
		}

        private void Out(SqlString s)
        {
            _writer.Clause(s);
        }

		private void ParameterOut()
		{
			_writer.Parameter();
		}

		/**
		 * Returns the last character written to the output, or -1 if there isn't one.
		 *//*
		private int GetLastChar() 
		{
			int len = _buf.Length;
			if ( len == 0 )
				return -1;
			else
				return _buf[len - 1];
		}*/

		/**
		 * Add a aspace if the previous token was not a space or a parenthesis.
		 */
		void OptionalSpace() 
		{/*
			int c = GetLastChar();
			switch ( c ) {
				case -1:
					return;
				case ' ':
					return;
				case ')':
					return;
				case '(':
					return;
				default:
					Out( " " );
					break;
			}*/
			Out(" ");
		}

		private void Out(IASTNode n) 
		{
			if (n is ParameterNode)
			{
				ParameterOut();
			}
			else if ( n is SqlNode ) 
			{
				Out(((SqlNode) n).RenderText(_sessionFactory));
			}
			else 
			{
				Out(n.Text);
			}

			if ( n is ParameterNode ) 
			{
				_collectedParameters.Add( ( ( ParameterNode ) n ).HqlParameterSpecification );
			}
			else if ( n is IParameterContainer ) 
			{
				if ( ( ( IParameterContainer ) n ).HasEmbeddedParameters ) 
				{
					IParameterSpecification[] specifications = ( ( IParameterContainer ) n ).GetEmbeddedParameters();
					if ( specifications != null ) 
					{
						_collectedParameters.AddRange(specifications);
					}
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
			// check two "adjecent" nodes at the top of the from-clause tree
			IASTNode next = a.NextSibling;
			if ( next == null || !HasText( a ) ) 
			{
				return;
			}

			FromElement left = ( FromElement ) a;
			FromElement right = ( FromElement ) next;

			///////////////////////////////////////////////////////////////////////
			// HACK ALERT !!!!!!!!!!!!!!!!!!!!!!!!!!!!
			// Attempt to work around "ghost" ImpliedFromElements that occasionally
			// show up between the actual things being joined.  This consistently
			// occurs from index nodes (at least against many-to-many).  Not sure
			// if there are other conditions
			//
			// Essentially, look-ahead to the next FromElement that actually
			// writes something to the SQL
			while ( right != null && !HasText( right ) ) 
			{
				right = ( FromElement ) right.NextSibling;
			}

			if ( right == null ) 
			{
				return;
			}
			///////////////////////////////////////////////////////////////////////

			if ( !HasText( right ) ) 
			{
				return;
			}

			if ( right.RealOrigin == left ||
				 ( right.RealOrigin != null && right.RealOrigin == left.RealOrigin ) ) 
			{
				// right represents a joins originating from left; or
				// both right and left reprersent joins originating from the same FromElement
				if ( right.JoinSequence != null && right.JoinSequence.IsThetaStyle) 
				{
					Out( ", " );
				}
				else 
				{
					Out( " " );
				}
			}
			else 
			{
				// these are just two unrelated table references
				Out( ", " );
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
					FromElement left = (FromElement) parent;
					FromElement right = (FromElement) d;
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
			return _sqlStringBuilder;
		}

		private void BeginFunctionTemplate(IASTNode m, IASTNode i) 
		{
			MethodNode methodNode = (MethodNode)m;
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
				_outputStack.Insert(0, _writer);
				_writer = new FunctionArguments();
			}
		}

		private void EndFunctionTemplate(IASTNode m) 
		{
			MethodNode methodNode = ( MethodNode ) m;
			ISQLFunction template = methodNode.SQLFunction;
			if ( template == null ) 
			{
				  Out(")");
			}
			else 
			{
				// this function has a template -> restore output, apply the template and write the result out
				FunctionArguments functionArguments = ( FunctionArguments ) _writer;   // TODO: Downcast to avoid using an interface?  Yuck.
				_writer = _outputStack[0];
				_outputStack.RemoveAt(0);
				Out( template.Render( functionArguments.Args, _sessionFactory) );
			}
		}

		private void CommaBetweenParameters(String comma) 
		{
			_writer.CommaBetweenParameters(comma);
		}

		/**
		 * Writes SQL fragments.
		 */
		interface ISqlWriter 
		{
			void Clause(String clause);
            void Clause(SqlString clause);
            void Parameter();
			/**
			 * todo remove this hack
			 * The parameter is either ", " or " , ". This is needed to pass sql generating tests as the old
			 * sql generator uses " , " in the WHERE and ", " in SELECT.
			 *
			 * @param comma either " , " or ", "
			 */
			void CommaBetweenParameters(String comma);
		}

		/**
		 * SQL function processing code redirects generated SQL output to an instance of this class
		 * which catches function arguments.
		 */
		class FunctionArguments : ISqlWriter 
		{
			private int argInd;
			private readonly List<object> args = new List<object>();

			public void Clause(String clause) 
			{
                if (argInd == args.Count)
                {
                    args.Add(clause);
                }
                else
                {
                    args[argInd] = args[argInd] + clause;
                }
			}

            public void Clause(SqlString clause)
            {
                this.Clause(clause.ToString());
            }

			public void Parameter()
			{
                args.Add(SqlCommand.Parameter.Placeholder);
			}

			public void CommaBetweenParameters(String comma) 
			{
				++argInd;
			}

			public IList Args
			{
				get { return args; }
			}
		}

		/**
		 * The default SQL writer.
		 */
		class DefaultWriter : ISqlWriter 
		{
			private readonly SqlGenerator _generator;

			internal DefaultWriter(SqlGenerator generator)
			{
				_generator = generator;
			}

			public void Clause(String clause) 
			{
				_generator.GetStringBuilder().Add( clause );
			}

            public void Clause(SqlString clause)
            {
                _generator.GetStringBuilder().Add(clause);
            }

			public void Parameter()
			{
				_generator.GetStringBuilder().AddParameter();
			}


			public void CommaBetweenParameters(String comma) 
			{
				_generator.GetStringBuilder().Add(comma);
			}
		}
	}
}
