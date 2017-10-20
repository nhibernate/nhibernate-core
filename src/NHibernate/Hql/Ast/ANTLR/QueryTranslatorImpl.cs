using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Antlr.Runtime;
using Antlr.Runtime.Tree;

using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Event;
using NHibernate.Hql.Ast.ANTLR.Exec;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Hql.Ast.ANTLR.Util;
using NHibernate.Loader.Hql;
using NHibernate.Param;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;
using IQueryable = NHibernate.Persister.Entity.IQueryable;

namespace NHibernate.Hql.Ast.ANTLR
{
	[CLSCompliant(false)]
	public partial class QueryTranslatorImpl : IFilterTranslator
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(QueryTranslatorImpl));

		private readonly string _queryIdentifier;
		private readonly IASTNode _stageOneAst;
		private readonly ISessionFactoryImplementor _factory;
		
		private bool _shallowQuery;
		private bool _compiled;
		private IDictionary<string, IFilter> _enabledFilters;
		private QueryLoader _queryLoader;
		private IStatementExecutor _statementExecutor;
		private IStatement _sqlAst;
		private IDictionary<string, string> _tokenReplacements;
		private HqlSqlGenerator _generator;

		/// <summary>
		/// Creates a new AST-based query translator.
		/// </summary>
		/// <param name="queryIdentifier">The query-identifier (used in stats collection)</param>
		/// <param name="parsedQuery">The hql query to translate</param>
		/// <param name="enabledFilters">Currently enabled filters</param>
		/// <param name="factory">The session factory constructing this translator instance.</param>
		public QueryTranslatorImpl(
				string queryIdentifier,
				IASTNode parsedQuery,
				IDictionary<string, IFilter> enabledFilters,
				ISessionFactoryImplementor factory)
		{
			_queryIdentifier = queryIdentifier;
			_stageOneAst = parsedQuery;
			_compiled = false;
			_shallowQuery = false;
			_enabledFilters = enabledFilters;
			_factory = factory;
		}

		/// <summary>
		/// Compile a "normal" query. This method may be called multiple
		/// times. Subsequent invocations are no-ops.
		/// </summary>
		/// <param name="replacements">Defined query substitutions.</param>
		/// <param name="shallow">Does this represent a shallow (scalar or entity-id) select?</param>
		public void Compile(IDictionary<string, string> replacements, bool shallow)
		{
			DoCompile( replacements, shallow, null );
		}

		/// <summary>
		/// Compile a filter. This method may be called multiple
		/// times. Subsequent invocations are no-ops.
		/// </summary>
		/// <param name="collectionRole">the role name of the collection used as the basis for the filter.</param>
		/// <param name="replacements">Defined query substitutions.</param>
		/// <param name="shallow">Does this represent a shallow (scalar or entity-id) select?</param>
		public void Compile(string collectionRole, IDictionary<string, string> replacements, bool shallow)
		{
			DoCompile(replacements, shallow, collectionRole);
		}

		public IList List(ISessionImplementor session, QueryParameters queryParameters)
		{
			// Delegate to the QueryLoader...
			ErrorIfDML();
			var query = ( QueryNode ) _sqlAst;
			bool hasLimit = queryParameters.RowSelection != null && queryParameters.RowSelection.DefinesLimits;
			bool needsDistincting = ( query.GetSelectClause().IsDistinct || hasLimit ) && ContainsCollectionFetches;

			QueryParameters queryParametersToUse;

			if ( hasLimit && ContainsCollectionFetches ) 
			{
				log.Warn( "firstResult/maxResults specified with collection fetch; applying in memory!" );
				var selection = new RowSelection
											{
												FetchSize = queryParameters.RowSelection.FetchSize,
												Timeout = queryParameters.RowSelection.Timeout
											};
				queryParametersToUse = queryParameters.CreateCopyUsing( selection );
			}
			else 
			{
				queryParametersToUse = queryParameters;
			}

			IList results = _queryLoader.List(session, queryParametersToUse);

			if ( needsDistincting ) 
			{
				int includedCount = -1;
				// NOTE : firstRow is zero-based
				int first = !hasLimit || queryParameters.RowSelection.FirstRow == RowSelection.NoValue
							? 0
							: queryParameters.RowSelection.FirstRow;
				int max = !hasLimit || queryParameters.RowSelection.MaxRows == RowSelection.NoValue
							? -1
							: queryParameters.RowSelection.MaxRows;

				int size = results.Count;
				var tmp = new List<object>();
				var distinction = new IdentitySet();

				for ( int i = 0; i < size; i++ ) 
				{
					object result = results[i];
					if ( !distinction.Add(result ) ) 
					{
						continue;
					}
					includedCount++;
					if ( includedCount < first ) 
					{
						continue;
					}
					tmp.Add( result );
					// NOTE : ( max - 1 ) because first is zero-based while max is not...
					if ( max >= 0 && ( includedCount - first ) >= ( max - 1 ) ) 
					{
						break;
					}
				}

				results = tmp;
			}

			return results;
		}

		public IEnumerable GetEnumerable(QueryParameters queryParameters, IEventSource session)
		{
			ErrorIfDML();
			return _queryLoader.GetEnumerable(queryParameters, session);
		}

		public int ExecuteUpdate(QueryParameters queryParameters, ISessionImplementor session)
		{
			ErrorIfSelect();
			return _statementExecutor.Execute(queryParameters, session);
		}

		private void ErrorIfSelect()
		{
			if (!_sqlAst.NeedsExecutor)
			{
				throw new QueryExecutionRequestException("Not supported for select queries:", _queryIdentifier);
			}
		}

		public NHibernate.Loader.Loader Loader
		{
			get { return _queryLoader; }
		}

		public virtual IType[] ActualReturnTypes
		{
			get { return _queryLoader.ReturnTypes; }
		}

		public ParameterMetadata BuildParameterMetadata()
		{
			IList<IParameterSpecification> specifications = _sqlAst.Walker.Parameters;
			IEnumerable<OrdinalParameterDescriptor> ordinals =
				specifications.OfType<PositionalParameterSpecification>().Select(op => new OrdinalParameterDescriptor(op.HqlPosition, op.ExpectedType));
			Dictionary<string, NamedParameterDescriptor> nameds = specifications.OfType<NamedParameterSpecification>()
				.Distinct()
				.Select(np => new {np.Name, Descriptor = new NamedParameterDescriptor(np.Name, np.ExpectedType, false)})
				.ToDictionary(ep => ep.Name, ep => ep.Descriptor);
			return new ParameterMetadata(ordinals, nameds);
		}

		public string[][] GetColumnNames()
		{
			ErrorIfDML();
			return _sqlAst.Walker.SelectClause.ColumnNames;
		}

		public ISet<string> QuerySpaces
		{
			get { return _sqlAst.Walker.QuerySpaces; }
		}

		public string SQLString
		{
			get { return _generator.Sql.ToString(); }
		}

		public IStatement SqlAST
		{
			get { return _sqlAst; }
		}

		public IList<IParameterSpecification> CollectedParameterSpecifications
		{
			get { return _generator.CollectionParameters; }
		}

		public SqlString SqlString
		{
			get { return _generator.Sql; }
		}

		public string QueryIdentifier
		{
			get { return _queryIdentifier; }
		}

		public IList<string> CollectSqlStrings
		{
			get
			{
				var list = new List<string>();
				if (IsManipulationStatement)
				{
					foreach (var sqlStatement in _statementExecutor.SqlStatements)
					{
						if (sqlStatement != null)
						{
							list.Add(sqlStatement.ToString());
						}
					}
				}
				else
				{
					list.Add(_generator.Sql.ToString());
				}
				return list;
			}
		}

		public string QueryString
		{
			get { return _queryIdentifier; }
		}

		public IDictionary<string, IFilter> EnabledFilters
		{
			get { return _enabledFilters; }
		}

		public IType[] ReturnTypes
		{
			get
			{
				ErrorIfDML();
				return _sqlAst.Walker.ReturnTypes;
			}
		}

		public string[] ReturnAliases
		{
			get
			{
				ErrorIfDML();
				return _sqlAst.Walker.ReturnAliases;
			}
		}

		public bool ContainsCollectionFetches
		{
			get
			{
				ErrorIfDML();
				IList<IASTNode> collectionFetches = ((QueryNode)_sqlAst).FromClause.GetCollectionFetches();
				return collectionFetches != null && collectionFetches.Count > 0;
			}
		}

		public bool IsManipulationStatement
		{
			get { return _sqlAst.NeedsExecutor; }
		}

		public bool IsShallowQuery
		{
			get { return _shallowQuery; }
		}

		/// <summary>
		/// Performs both filter and non-filter compiling.
		/// </summary>
		/// <param name="replacements">Defined query substitutions.</param>
		/// <param name="shallow">Does this represent a shallow (scalar or entity-id) select?</param>
		/// <param name="collectionRole">the role name of the collection used as the basis for the filter, NULL if this is not a filter.</param>
		private void DoCompile(IDictionary<string, string> replacements, bool shallow, String collectionRole) 
		{
			// If the query is already compiled, skip the compilation.
			if ( _compiled ) 
			{
				if ( log.IsDebugEnabled ) 
				{
					log.Debug( "compile() : The query is already compiled, skipping..." );
				}
				return;
			}

			// Remember the parameters for the compilation.
			_tokenReplacements = replacements ?? new Dictionary<string, string>(1);

			_shallowQuery = shallow;

			try 
			{
				// PHASE 1 : Analyze the HQL AST, and produce an SQL AST.
				var translator = Analyze(collectionRole);

				_sqlAst = translator.SqlStatement;

				// at some point the generate phase needs to be moved out of here,
				// because a single object-level DML might spawn multiple SQL DML
				// command executions.
				//
				// Possible to just move the sql generation for dml stuff, but for
				// consistency-sake probably best to just move responsiblity for
				// the generation phase completely into the delegates
				// (QueryLoader/StatementExecutor) themselves.  Also, not sure why
				// QueryLoader currently even has a dependency on this at all; does
				// it need it?  Ideally like to see the walker itself given to the delegates directly...

				if (_sqlAst.NeedsExecutor) 
				{
					_statementExecutor = BuildAppropriateStatementExecutor(_sqlAst);
				}
				else 
				{
					// PHASE 2 : Generate the SQL.
					_generator = new HqlSqlGenerator(_sqlAst, _factory);
					_generator.Generate();

					_queryLoader = new QueryLoader(this, _factory, _sqlAst.Walker.SelectClause);
				}

				_compiled = true;
			}
			catch ( QueryException qe ) 
			{
				qe.QueryString = _queryIdentifier;
				throw;
			}
			catch ( RecognitionException e ) 
			{
				// we do not actually propogate ANTLRExceptions as a cause, so
				// log it here for diagnostic purposes
				if ( log.IsInfoEnabled ) 
				{
					log.Info( "converted antlr.RecognitionException", e );
				}
				throw QuerySyntaxException.Convert(e, _queryIdentifier);
			}

			_enabledFilters = null; //only needed during compilation phase...
		}

		private static IStatementExecutor BuildAppropriateStatementExecutor(IStatement statement)
		{
			HqlSqlWalker walker = statement.Walker;
			if (walker.StatementType == HqlSqlWalker.DELETE)
			{
				FromElement fromElement = walker.GetFinalFromClause().GetFromElement();
				IQueryable persister = fromElement.Queryable;
				if (persister.IsMultiTable)
				{
					return new MultiTableDeleteExecutor(statement);
				}
				else
				{
					return new BasicExecutor(statement, persister);
				}
			}
			else if (walker.StatementType == HqlSqlWalker.UPDATE)
			{
				FromElement fromElement = walker.GetFinalFromClause().GetFromElement();
				IQueryable persister = fromElement.Queryable;
				if (persister.IsMultiTable)
				{
					// even here, if only properties mapped to the "base table" are referenced
					// in the set and where clauses, this could be handled by the BasicDelegate.
					// TODO : decide if it is better performance-wise to perform that check, or to simply use the MultiTableUpdateDelegate
					return new MultiTableUpdateExecutor(statement);
				}
				else
				{
					return new BasicExecutor(statement, persister);
				}
			}
			else if (walker.StatementType == HqlSqlWalker.INSERT)
			{
				return new BasicExecutor(statement, ((InsertStatement)statement).IntoClause.Queryable);
			}
			else
			{
				throw new QueryException("Unexpected statement type");
			}
		}

		private HqlSqlTranslator Analyze(string collectionRole)
		{
			var translator = new HqlSqlTranslator(_stageOneAst, this, _factory, _tokenReplacements, collectionRole);

			translator.Translate();

			return translator;
		}

		private void ErrorIfDML()
		{
			if (_sqlAst.NeedsExecutor)
			{
				throw new QueryExecutionRequestException("Not supported for DML operations", _queryIdentifier);
			}
		}
	}

	public class HqlParseEngine
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(HqlParseEngine));

		private readonly string _hql;
		private CommonTokenStream _tokens;
		private readonly bool _filter;
		private readonly ISessionFactoryImplementor _sfi;

		public HqlParseEngine(string hql, bool filter, ISessionFactoryImplementor sfi)
		{
			_hql = hql;
			_filter = filter;
			_sfi = sfi;
		}

		public IASTNode Parse()
		{
			// Parse the query string into an HQL AST.
			var lex = new HqlLexer(new CaseInsensitiveStringStream(_hql));
			_tokens = new CommonTokenStream(lex);

			var parser = new HqlParser(_tokens) {TreeAdaptor = new ASTTreeAdaptor(), Filter = _filter};

			if (log.IsDebugEnabled)
			{
				log.Debug("parse() - HQL: " + _hql);
			}

			try
			{
				var ast = (IASTNode) parser.statement().Tree;

				var walker = new NodeTraverser(new ConstantConverter(_sfi));
				walker.TraverseDepthFirst(ast);

				return ast;
			}
			finally
			{
				parser.ParseErrorHandler.ThrowQueryException();
			}
		}

		class ConstantConverter : IVisitationStrategy
		{
			private IASTNode _dotRoot;
			private readonly ISessionFactoryImplementor _sfi;

			public ConstantConverter(ISessionFactoryImplementor sfi)
			{
				_sfi = sfi;
			}

			public void Visit(IASTNode node)
			{
				if (_dotRoot != null)
				{
					// we are already processing a dot-structure
					if (ASTUtil.IsSubtreeChild(_dotRoot, node))
					{
						// ignore it...
						return;
					}

					// we are now at a new tree level
					_dotRoot = null;
				}

				if (_dotRoot == null && node.Type == HqlSqlWalker.DOT)
				{
					_dotRoot = node;
					HandleDotStructure(_dotRoot);
				}
			}

			private void HandleDotStructure(IASTNode dotStructureRoot)
			{
				var expression = ASTUtil.GetPathText(dotStructureRoot);

				var constant = ReflectHelper.GetConstantValue(expression, _sfi);

				if (constant != null)
				{
					dotStructureRoot.ClearChildren();
					dotStructureRoot.Type = HqlSqlWalker.JAVA_CONSTANT;
					dotStructureRoot.Text = expression;
				}
			}
		}
	}

	internal class HqlSqlTranslator
	{
		private readonly IASTNode _inputAst;
		private readonly QueryTranslatorImpl _qti;
		private readonly ISessionFactoryImplementor _sfi;
		private readonly IDictionary<string, string> _tokenReplacements;
		private readonly string _collectionRole;
		private IStatement _resultAst;

		public HqlSqlTranslator(IASTNode ast, QueryTranslatorImpl qti, ISessionFactoryImplementor sfi, IDictionary<string, string> tokenReplacements, string collectionRole)
		{
			_inputAst = ast;
			_qti = qti;
			_sfi = sfi;
			_tokenReplacements = tokenReplacements;
			_collectionRole = collectionRole;
		}

		public IStatement SqlStatement
		{
			get { return _resultAst; }
		}

		public IStatement Translate()
		{
			if (_resultAst == null)
			{
				if (_collectionRole != null)
				{
					HqlFilterPreprocessor.AddImpliedFromToQuery(_inputAst, _collectionRole, _sfi);
				}

				var nodes = new BufferedTreeNodeStream(_inputAst);

				var hqlSqlWalker = new HqlSqlWalker(_qti, _sfi, nodes, _tokenReplacements, _collectionRole);
				hqlSqlWalker.TreeAdaptor = new HqlSqlWalkerTreeAdaptor(hqlSqlWalker);

				try
				{
					// Transform the tree.
					_resultAst = (IStatement) hqlSqlWalker.statement().Tree;
				}
				finally
				{
					hqlSqlWalker.ParseErrorHandler.ThrowQueryException();
				}
			}

			return _resultAst;
		}
	}

	internal class HqlSqlGenerator
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(HqlSqlGenerator));

		private readonly IASTNode _ast;
		private readonly ISessionFactoryImplementor _sfi;
		private SqlString _sql;
		private IList<IParameterSpecification> _parameters;

		public HqlSqlGenerator(IStatement ast, ISessionFactoryImplementor sfi)
		{
			_ast = (IASTNode)ast;
			_sfi = sfi;
		}

		public SqlString Sql
		{
			get { return _sql; }
		}

		public IList<IParameterSpecification> CollectionParameters
		{
			get { return _parameters; }
		}

		public SqlString Generate()
		{
			if (_sql == null)
			{
				var gen = new SqlGenerator(_sfi, new CommonTreeNodeStream(_ast));

				try
				{
					gen.statement();

					_sql = gen.GetSQL();

					if (log.IsDebugEnabled)
					{
						log.Debug("SQL: " + _sql);
					}
				}
				finally
				{
					gen.ParseErrorHandler.ThrowQueryException();
				}

				_parameters = gen.GetCollectedParameters();
			}

			return _sql;
		}
	}
}
