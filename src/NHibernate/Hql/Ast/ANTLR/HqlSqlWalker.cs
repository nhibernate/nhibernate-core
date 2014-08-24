using System;
using System.Collections.Generic;
using Antlr.Runtime;
using Antlr.Runtime.Tree;

using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Hql.Ast.ANTLR.Util;
using NHibernate.Id;
using NHibernate.Param;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.UserTypes;
using NHibernate.Util;

namespace NHibernate.Hql.Ast.ANTLR
{
	public partial class HqlSqlWalker
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(HqlSqlWalker));

		// Fields
		private readonly string _collectionFilterRole;
		private readonly SessionFactoryHelperExtensions _sessionFactoryHelper;
		private readonly QueryTranslatorImpl _qti;
		private int _currentClauseType;
		private int _level;
		private bool _inSelect;
		private bool _inFrom;
		private bool _inFunctionCall;
		private bool _inCase;
		private int _statementType;
		private int _currentStatementType;
		private string _statementTypeName;
		private int _positionalParameterCount;
		private int _parameterCount;
		private readonly NullableDictionary<string, object> _namedParameters = new NullableDictionary<string, object>();
		private readonly List<IParameterSpecification> _parameters = new List<IParameterSpecification>();
		private FromClause _currentFromClause;
		private SelectClause _selectClause;
		private readonly AliasGenerator _aliasGenerator = new AliasGenerator();
		private readonly ASTPrinter _printer = new ASTPrinter();

		private readonly ISet<string> _querySpaces = new HashSet<string>();

		private readonly LiteralProcessor _literalProcessor;

		private readonly IDictionary<string, string> _tokenReplacements;

		private JoinType _impliedJoinType;

		private IParseErrorHandler _parseErrorHandler = new ErrorCounter();

		private IASTFactory _nodeFactory;
		private readonly List<AssignmentSpecification> assignmentSpecifications = new List<AssignmentSpecification>();
		private int numberOfParametersInSetClause;

		public HqlSqlWalker(QueryTranslatorImpl qti,
					  ISessionFactoryImplementor sfi,
					  ITreeNodeStream input, 
					  IDictionary<string, string> tokenReplacements,
					  string collectionRole)
			: this(input)
		{
			_sessionFactoryHelper = new SessionFactoryHelperExtensions(sfi);
			_qti = qti;
			_literalProcessor = new LiteralProcessor(this);
			_tokenReplacements = tokenReplacements;
			_collectionFilterRole = collectionRole;
		}

		public override void ReportError(RecognitionException e)
		{
			_parseErrorHandler.ReportError(e);
		}

		/*
		protected override void Mismatch(IIntStream input, int ttype, BitSet follow)
		{
		   throw new MismatchedTokenException(ttype, input);
		}

		public override object RecoverFromMismatchedSet(IIntStream input, RecognitionException e, BitSet follow)
		{
			throw e;
		}
		*/
		public IList<AssignmentSpecification> AssignmentSpecifications
		{
			get { return assignmentSpecifications; }
		}

		public int NumberOfParametersInSetClause
		{
			get { return numberOfParametersInSetClause; }
		}

		public IParseErrorHandler ParseErrorHandler
		{
			get { return _parseErrorHandler; }
			set { _parseErrorHandler = value; }
		}

		public AliasGenerator AliasGenerator
		{
			get { return _aliasGenerator; }
		}

		public ISet<string> QuerySpaces
		{
			get { return _querySpaces; }
		}

	    public IDictionary<string, object> NamedParameters
	    {
            get { return _namedParameters; }
	    }

		internal SessionFactoryHelperExtensions SessionFactoryHelper
		{
			get { return _sessionFactoryHelper; }
		}
		
		public int CurrentStatementType
		{
			get { return _currentStatementType; }
		}

		public JoinType ImpliedJoinType
		{
			get { return _impliedJoinType; }
		}

		public String[] ReturnAliases
		{
			get { return _selectClause.QueryReturnAliases; }
		}

		public IType[] ReturnTypes
		{
			get { return _selectClause.QueryReturnTypes; }	
		}

		public string CollectionFilterRole
		{
			get { return _collectionFilterRole; }	
		}

		public SelectClause SelectClause
		{
			get { return _selectClause; }
		}

		public IList<IParameterSpecification> Parameters
		{
			get { return _parameters; }
		}

		void BeforeStatement(string statementName, int statementType)
		{
			_inFunctionCall = false;
			_level++;
			if (_level == 1)
			{
				_statementTypeName = statementName;
				_statementType = statementType;
			}
			_currentStatementType = statementType;
			if (log.IsDebugEnabled)
			{
				log.Debug(statementName + " << begin [level=" + _level + ", statement=" + _statementTypeName + "]");
			}
		}

		void BeforeStatementCompletion(string statementName)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug(statementName + " : finishing up [level=" + _level + ", statement=" + _statementTypeName + "]");
			}
		}

		void PrepareVersioned(IASTNode updateNode, IASTNode versioned)
		{
			var updateStatement = (UpdateStatement)updateNode;
			FromClause fromClause = updateStatement.FromClause;
			if (versioned != null)
			{
				// Make sure that the persister is versioned
				IQueryable persister = fromClause.GetFromElement().Queryable;
				if (!persister.IsVersioned)
				{
					throw new SemanticException("increment option specified for update of non-versioned entity");
				}

				IVersionType versionType = persister.VersionType;
				if (versionType is IUserVersionType)
				{
					throw new SemanticException("user-defined version types not supported for increment option");
				}

				IASTNode eq = ASTFactory.CreateNode(EQ, "=");
				IASTNode versionPropertyNode = GenerateVersionPropertyNode(persister);

				eq.SetFirstChild(versionPropertyNode);

				IASTNode versionIncrementNode;
				if (typeof(DateTime).IsAssignableFrom(versionType.ReturnedClass))
				{
					versionIncrementNode = ASTFactory.CreateNode(PARAM, "?");
					IParameterSpecification paramSpec = new VersionTypeSeedParameterSpecification(versionType);
					((ParameterNode)versionIncrementNode).HqlParameterSpecification = paramSpec;
					Parameters.Insert(0, paramSpec);
				}
				else
				{
					// Not possible to simply re-use the versionPropertyNode here as it causes
					// OOM errors due to circularity :(
					versionIncrementNode = ASTFactory.CreateNode(PLUS, "+");
					versionIncrementNode.SetFirstChild(GenerateVersionPropertyNode(persister));
					versionIncrementNode.AddChild(ASTFactory.CreateNode(IDENT, "1"));
				}

				eq.AddChild(versionIncrementNode);

				EvaluateAssignment(eq, persister, 0);

				IASTNode setClause = updateStatement.SetClause;
				setClause.InsertChild(0, eq);
			}
		}

		private IASTNode GenerateVersionPropertyNode(IQueryable persister)
		{
			string versionPropertyName = persister.PropertyNames[persister.VersionProperty];
			var versionPropertyRef = ASTFactory.CreateNode(IDENT, versionPropertyName);
			var versionPropertyNode = LookupNonQualifiedProperty(versionPropertyRef);
			Resolve(versionPropertyNode);
			return versionPropertyNode;
		}

		void PostProcessUpdate(IASTNode update)
		{
			var updateStatement = (UpdateStatement)update;
			PostProcessDML(updateStatement);
		}

		void PostProcessDelete(IASTNode delete)
		{
			PostProcessDML((DeleteStatement)delete);
		}

		void PostProcessInsert(IASTNode insert)
		{
			var insertStatement = (InsertStatement)insert;
			insertStatement.Validate();

			SelectClause selectClause = insertStatement.SelectClause;
			var persister = insertStatement.IntoClause.Queryable;

			if (!insertStatement.IntoClause.IsExplicitIdInsertion)
			{
				// We need to generate ids as part of this bulk insert.
				//
				// Note that this is only supported for sequence-style generators and
				// post-insert-style generators; basically, only in-db generators
				IIdentifierGenerator generator = persister.IdentifierGenerator;
				if (!SupportsIdGenWithBulkInsertion(generator))
				{
					throw new QueryException("can only generate ids as part of bulk insert with either sequence or post-insert style generators");
				}

				IASTNode idSelectExprNode = null;

				var seqGenerator = generator as SequenceGenerator;
				if (seqGenerator != null)
				{
					string seqName = seqGenerator.GeneratorKey();
					string nextval = SessionFactoryHelper.Factory.Dialect.GetSelectSequenceNextValString(seqName);
					idSelectExprNode = ASTFactory.CreateNode(SQL_TOKEN, nextval);
				}
				else
				{
					//Don't need this, because we should never ever be selecting no columns in an insert ... select...
					//and because it causes a bug on DB2
					/*String idInsertString = sessionFactoryHelper.getFactory().getDialect().getIdentityInsertString();
					if ( idInsertString != null ) {
					idSelectExprNode = getASTFactory().create( HqlSqlTokenTypes.SQL_TOKEN, idInsertString );
					}*/
				}

				if (idSelectExprNode != null)
				{
					selectClause.InsertChild(0, idSelectExprNode);

					insertStatement.IntoClause.PrependIdColumnSpec();
				}
			}

			bool includeVersionProperty = persister.IsVersioned && !insertStatement.IntoClause.IsExplicitVersionInsertion && persister.VersionPropertyInsertable;
			if (includeVersionProperty)
			{
				// We need to seed the version value as part of this bulk insert
				IVersionType versionType = persister.VersionType;
				IASTNode versionValueNode;

				if (SessionFactoryHelper.Factory.Dialect.SupportsParametersInInsertSelect)
				{
					versionValueNode = ASTFactory.CreateNode(PARAM, "?");
					IParameterSpecification paramSpec = new VersionTypeSeedParameterSpecification(versionType);
					((ParameterNode)versionValueNode).HqlParameterSpecification = paramSpec;
					_parameters.Insert(0, paramSpec);
				}
				else
				{
					if (IsIntegral(versionType))
					{
						try
						{
							object seedValue = versionType.Seed(null);
							versionValueNode = ASTFactory.CreateNode(SQL_TOKEN, seedValue.ToString());
						}
						catch (Exception t)
						{
							throw new QueryException("could not determine seed value for version on bulk insert [" + versionType + "]", t);
						}
					}
					else if (IsDatabaseGeneratedTimestamp(versionType))
					{
						string functionName = SessionFactoryHelper.Factory.Dialect.CurrentTimestampSQLFunctionName;
						versionValueNode = ASTFactory.CreateNode(SQL_TOKEN, functionName);
					}
					else
					{
						throw new QueryException("cannot handle version type [" + versionType + "] on bulk inserts with dialects not supporting parameters in insert-select statements");
					}
				}

				selectClause.InsertChild(0, versionValueNode);

				insertStatement.IntoClause.PrependVersionColumnSpec();
			}

			if (insertStatement.IntoClause.IsDiscriminated)
			{
				string sqlValue = insertStatement.IntoClause.Queryable.DiscriminatorSQLValue;
				IASTNode discrimValue = ASTFactory.CreateNode(SQL_TOKEN, sqlValue);
				insertStatement.SelectClause.AddChild(discrimValue);
			}
		}

		private static bool IsDatabaseGeneratedTimestamp(IType type)
		{
			// TODO NH: we should check the "generated" property
			// currently only the Hibernate-supplied DbTimestampType is supported here
			return type is TimestampType;
		}

		private static bool IsIntegral(IType type)
		{
			return
				typeof(long).IsAssignableFrom(type.ReturnedClass) ||
				typeof(int).IsAssignableFrom(type.ReturnedClass) ||
				typeof(short).IsAssignableFrom(type.ReturnedClass);
		}

		public static bool SupportsIdGenWithBulkInsertion(IIdentifierGenerator generator)
		{
			return generator is SequenceGenerator 
				|| generator is IPostInsertIdentifierGenerator;
		}

		private void PostProcessDML(IRestrictableStatement statement)
		{
			statement.FromClause.Resolve();

			var fromElement = (FromElement)statement.FromClause.GetFromElements()[0];
			IQueryable persister = fromElement.Queryable;
			// Make #@%$^#^&# sure no alias is applied to the table name
			fromElement.Text = persister.TableName;

			// append any filter fragments; the EMPTY_MAP is used under the assumption that
			// currently enabled filters should not affect this process
			if (persister.DiscriminatorType != null)
			{
				new SyntheticAndFactory(this)
					.AddDiscriminatorWhereFragment(statement, persister, new CollectionHelper.EmptyMapClass<string, IFilter>(), fromElement.TableAlias);
			}
		}

		void AfterStatementCompletion(string statementName)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug(statementName + " >> end [level=" + _level + ", statement=" + _statementTypeName + "]");
			}
			_level--;
		}

		void HandleClauseStart(int clauseType)
		{
			_currentClauseType = clauseType;
		}

		IASTNode CreateIntoClause(string path, IASTNode propertySpec)
		{
			var persister = (IQueryable) SessionFactoryHelper.RequireClassPersister(path);

			var intoClause = (IntoClause) adaptor.Create(INTO, "into");
			intoClause.SetFirstChild(propertySpec);
			intoClause.Initialize(persister);

			AddQuerySpaces(persister.QuerySpaces);

			return intoClause;
		}

		IASTNode Resolve(IASTNode node)
		{
			if (node != null)
			{
				// This is called when it's time to fully resolve a path expression.
				IResolvableNode r = (IResolvableNode)node;

				if (_inFunctionCall)
				{
					r.ResolveInFunctionCall(false, true);
				}
				else
				{
					r.Resolve(false, true);	// Generate implicit joins, only if necessary.
				}
			}

			return node;
		}

		void ProcessQuery(IASTNode select, IASTNode query)
		{
			if ( log.IsDebugEnabled ) {
				log.Debug( "processQuery() : " + query.ToStringTree() );
			}

			try {
				QueryNode qn = ( QueryNode ) query;

				// Was there an explicit select expression?
				bool explicitSelect = select != null && select.ChildCount > 0;

				if ( !explicitSelect ) {
					// No explicit select expression; render the id and properties
					// projection lists for every persister in the from clause into
					// a single 'token node'.
					//TODO: the only reason we need this stuff now is collection filters,
					//      we should get rid of derived select clause completely!
					CreateSelectClauseFromFromClause( qn );
				}
				else {
					// Use the explicitly declared select expression; determine the
					// return types indicated by each select token
					UseSelectClause( select );
				}

				// After that, process the JOINs.
				// Invoke a delegate to do the work, as this is farily complex.
				JoinProcessor joinProcessor = new JoinProcessor( this );
				joinProcessor.ProcessJoins( qn );

				// Attach any mapping-defined "ORDER BY" fragments
				foreach (FromElement fromElement in qn.FromClause.GetProjectionList())
				{
					if ( fromElement.IsFetch && fromElement.QueryableCollection != null ) 
					{
						// Does the collection referenced by this FromElement
						// specify an order-by attribute?  If so, attach it to
						// the query's order-by
						if ( fromElement.QueryableCollection.HasOrdering) 
						{
							string orderByFragment = fromElement
									.QueryableCollection
									.GetSQLOrderByString( fromElement.TableAlias );
							qn.GetOrderByClause().AddOrderFragment( orderByFragment );
						}
						if ( fromElement.QueryableCollection.HasManyToManyOrdering ) 
						{
							string orderByFragment = fromElement.QueryableCollection
									.GetManyToManyOrderByString( fromElement.TableAlias );
							qn.GetOrderByClause().AddOrderFragment( orderByFragment );
						}
					}
				}
			}
			finally
			{
				PopFromClause();
			}
		}

		private void UseSelectClause(IASTNode select)
		{
			_selectClause = (SelectClause) select;
			_selectClause.InitializeExplicitSelectClause(_currentFromClause);
		}

		private void CreateSelectClauseFromFromClause(IASTNode qn)
		{
			// TODO - check this.  Not *exactly* the same logic as the Java original
			qn.InsertChild(0, (IASTNode)adaptor.Create(SELECT_CLAUSE, "{derived select clause}"));

			_selectClause = ( SelectClause ) qn.GetChild(0);
			_selectClause.InitializeDerivedSelectClause( _currentFromClause );

			if ( log.IsDebugEnabled ) 
			{
				log.Debug( "Derived SELECT clause created." );
			}
		}

		/// <summary>
		/// Returns to the previous 'FROM' context.
		/// </summary>
		private void PopFromClause()
		{
			_currentFromClause = _currentFromClause.ParentFromClause;
		}

		static void ProcessConstructor(IASTNode constructor)
		{
			ConstructorNode constructorNode = (ConstructorNode)constructor;
			constructorNode.Prepare();
		}

		protected void EvaluateAssignment(IASTNode eq)
		{
			PrepareLogicOperator(eq);
			IQueryable persister = CurrentFromClause.GetFromElement().Queryable;
			EvaluateAssignment(eq, persister, -1);
		}

		private void EvaluateAssignment(IASTNode eq, IQueryable persister, int targetIndex)
		{
			if (persister.IsMultiTable)
			{
				// no need to even collect this information if the persister is considered multi-table
				var specification = new AssignmentSpecification(eq, persister);
				if (targetIndex >= 0)
				{
					assignmentSpecifications.Insert(targetIndex, specification);
				}
				else
				{
					assignmentSpecifications.Add(specification);
				}
				numberOfParametersInSetClause += specification.Parameters.Length;
			}
		}

		void BeforeSelectClause()
		{
			// Turn off includeSubclasses on all FromElements.
			FromClause from = CurrentFromClause;

			foreach (FromElement fromElement in from.GetFromElements())
			{
				fromElement.IncludeSubclasses = false;
			}
		}

		static void SetAlias(IASTNode selectExpr, IASTNode ident)
		{
			((ISelectExpression)selectExpr).Alias = ident.Text;
		}

		static void ResolveSelectExpression(IASTNode node)
		{
			// This is called when it's time to fully resolve a path expression.
			int type = node.Type;
			switch (type)
			{
				case DOT:
					DotNode dot = (DotNode)node;
					dot.ResolveSelectExpression();
					break;
				case ALIAS_REF:
					// Notify the FROM element that it is being referenced by the select.
					FromReferenceNode aliasRefNode = (FromReferenceNode)node;

					aliasRefNode.Resolve(false, false); //TODO: is it kosher to do it here?
					FromElement fromElement = aliasRefNode.FromElement;
					if (fromElement != null)
					{
						fromElement.IncludeSubclasses = true;
					}
					break;
				default:
					break;
			}
		}

		void PrepareFromClauseInputTree(IASTNode fromClauseInput, ITreeNodeStream input)
		{
			if (IsFilter())
			{
				// Handle collection-fiter compilation.
				// IMPORTANT NOTE: This is modifying the INPUT (HQL) tree, not the output tree!
				IQueryableCollection persister = _sessionFactoryHelper.GetCollectionPersister(_collectionFilterRole);
				IType collectionElementType = persister.ElementType;
				if (!collectionElementType.IsEntityType)
				{
					throw new QueryException("collection of values in filter: this");
				}

				string collectionElementEntityName = persister.ElementPersister.EntityName;

				IASTNode fromElement = (IASTNode)adaptor.Create(FILTER_ENTITY, collectionElementEntityName);
				IASTNode alias = (IASTNode)adaptor.Create(ALIAS, "this");

                ((HqlSqlWalkerTreeNodeStream)input).InsertChild(fromClauseInput, fromElement);
                ((HqlSqlWalkerTreeNodeStream)input).InsertChild(fromClauseInput, alias);

//				fromClauseInput.AddChild(fromElement);
//				fromClauseInput.AddChild(alias);

				// Show the modified AST.
				if (log.IsDebugEnabled)
				{
					log.Debug("prepareFromClauseInputTree() : Filter - Added 'this' as a from element...");
				}
				
				// Create a parameter specification for the collection filter...
				IType collectionFilterKeyType = _sessionFactoryHelper.RequireQueryableCollection(_collectionFilterRole).KeyType;
				ParameterNode collectionFilterKeyParameter = (ParameterNode)adaptor.Create(PARAM, "?");
				CollectionFilterKeyParameterSpecification collectionFilterKeyParameterSpec = new CollectionFilterKeyParameterSpecification(
						_collectionFilterRole, collectionFilterKeyType, _positionalParameterCount++
				);
				collectionFilterKeyParameter.HqlParameterSpecification = collectionFilterKeyParameterSpec;
				_parameters.Add(collectionFilterKeyParameterSpec);
			}
		}

		void CreateFromJoinElement(
				IASTNode path,
				IASTNode alias,
				int joinType,
				IASTNode fetchNode,
				IASTNode propertyFetch,
				IASTNode with)
		{
			bool fetch = fetchNode != null;
			if ( fetch && IsSubQuery ) 
			{
				throw new QueryException( "fetch not allowed in subquery from-elements" );
			}
			// The path AST should be a DotNode, and it should have been evaluated already.
			if ( path.Type != DOT ) 
			{
				throw new SemanticException( "Path expected for join!" );
			}

			DotNode dot = ( DotNode ) path;
			//JoinType hibernateJoinType = JoinProcessor.ToHibernateJoinType( joinType );
			JoinType hibernateJoinType = _impliedJoinType;

			dot.JoinType = hibernateJoinType;	// Tell the dot node about the join type.
			dot.Fetch = fetch;

			// Generate an explicit join for the root dot node.   The implied joins will be collected and passed up
			// to the root dot node.
			dot.Resolve( true, false, alias == null ? null : alias.Text );

			FromElement fromElement;
			if (dot.DataType != null && dot.DataType.IsComponentType)
			{
				var factory = new FromElementFactory(CurrentFromClause, dot.GetLhs().FromElement, dot.PropertyPath, alias == null ? null : alias.Text, null, false);
				fromElement = factory.CreateComponentJoin((ComponentType) dot.DataType);
			}
			else
			{
				fromElement = dot.GetImpliedJoin();
				if (fromElement == null)
				{
					throw new InvalidPathException("Invalid join: " + dot.Path);
				}
				fromElement.SetAllPropertyFetch(propertyFetch != null);

				if (with != null)
				{
					if (fetch)
					{
						throw new SemanticException("with-clause not allowed on fetched associations; use filters");
					}

					HandleWithFragment(fromElement, with);
				}
			}

			if ( log.IsDebugEnabled )
			{
				log.Debug( "createFromJoinElement() : " + _printer.ShowAsString( fromElement, "-- join tree --" ) );
			}
		}

		IASTNode CreateFromElement(string path, IASTNode pathNode, IASTNode alias, IASTNode propertyFetch)
		{
            FromElement fromElement = _currentFromClause.AddFromElement(path, alias);
            fromElement.SetAllPropertyFetch(propertyFetch != null);
            return fromElement;
		}

	    IASTNode CreateFromFilterElement(IASTNode filterEntity, IASTNode alias)
		{
			FromElement fromElement = _currentFromClause.AddFromElement(filterEntity.Text, alias);
			FromClause fromClause = fromElement.FromClause;
			IQueryableCollection persister = _sessionFactoryHelper.GetCollectionPersister(_collectionFilterRole);

			// Get the names of the columns used to link between the collection
			// owner and the collection elements.
			String[] keyColumnNames = persister.KeyColumnNames;
			String fkTableAlias = persister.IsOneToMany
					? fromElement.TableAlias
					: fromClause.AliasGenerator.CreateName(_collectionFilterRole);

			JoinSequence join = _sessionFactoryHelper.CreateJoinSequence();
			join.SetRoot(persister, fkTableAlias);

			if (!persister.IsOneToMany)
			{
				join.AddJoin((IAssociationType)persister.ElementType,
						fromElement.TableAlias,
					 	JoinType.InnerJoin,
						persister.GetElementColumnNames(fkTableAlias));
			}

			join.AddCondition(fkTableAlias, keyColumnNames, " = ", true);
			fromElement.JoinSequence = join;
			fromElement.Filter = true;

			if (log.IsDebugEnabled)
			{
				log.Debug("createFromFilterElement() : processed filter FROM element.");
			}
	
			return fromElement;
		}

		void SetImpliedJoinType(int joinType)
		{
			_impliedJoinType = JoinProcessor.ToHibernateJoinType(joinType);
		}

		void PushFromClause(IASTNode fromNode)
		{
			FromClause newFromClause = (FromClause)fromNode;
			newFromClause.SetParentFromClause(_currentFromClause);
			_currentFromClause = newFromClause;
		}

		static void PrepareArithmeticOperator(IASTNode op)
		{
			((IOperatorNode)op).Initialize();
		}

		static void ProcessFunction(IASTNode functionCall, bool inSelect)
		{
			MethodNode methodNode = (MethodNode)functionCall;
			methodNode.Resolve(inSelect);
		}

		void ProcessBool(IASTNode constant)
		{
			_literalProcessor.ProcessBoolean(constant);  // Use the delegate.
		}

		static void PrepareLogicOperator(IASTNode operatorNode)
		{
			( ( IOperatorNode ) operatorNode ).Initialize();
		}

		void ProcessNumericLiteral(IASTNode literal)
		{
			_literalProcessor.ProcessNumericLiteral((SqlNode) literal);
		}

		protected IASTNode LookupProperty(IASTNode dot, bool root, bool inSelect)
		{
			DotNode dotNode = (DotNode) dot;

			// Resolve everything up to this dot, but don't resolve the placeholders yet.
			dotNode.ResolveFirstChild();
			return dotNode;
		}

		static void ProcessIndex(IASTNode indexOp)
		{
			IndexNode indexNode = (IndexNode)indexOp;
			indexNode.Resolve(true, true);
		}

		bool IsNonQualifiedPropertyRef(IASTNode ident)
		{
			string identText = ident.Text;

			if ( _currentFromClause.IsFromElementAlias( identText ) ) 
			{
				return false;
			}

			IList<IASTNode> fromElements = _currentFromClause.GetExplicitFromElements();
			if ( fromElements.Count == 1 ) 
			{
				FromElement fromElement = (FromElement) fromElements[0];

				log.Info( "attempting to resolve property [" + identText + "] as a non-qualified ref" );

				IType type;
				return fromElement.GetPropertyMapping(identText).TryToType(identText, out type);
			}

			return false;
		}

		IASTNode LookupNonQualifiedProperty(IASTNode property)
		{
			FromElement fromElement = (FromElement) _currentFromClause.GetExplicitFromElements()[0];
			IASTNode syntheticDotNode = GenerateSyntheticDotNodeForNonQualifiedPropertyRef( property, fromElement );
			return LookupProperty( syntheticDotNode, false, _currentClauseType == SELECT );
		}

		IASTNode GenerateSyntheticDotNodeForNonQualifiedPropertyRef(IASTNode property, FromElement fromElement)
		{
			IASTNode dot = (IASTNode) adaptor.Create(DOT, "{non-qualified-property-ref}");

			// TODO : better way?!?
			((DotNode)dot).PropertyPath = ((FromReferenceNode)property).Path;

			IdentNode syntheticAlias = (IdentNode)adaptor.Create(IDENT, "{synthetic-alias}");
			syntheticAlias.FromElement = fromElement;
			syntheticAlias.IsResolved = true;

			dot.SetFirstChild(syntheticAlias);
			dot.AddChild(property);

			return dot;
		}

		void LookupAlias(IASTNode aliasRef)
		{
			FromElement alias = _currentFromClause.GetFromElement(aliasRef.Text);
			FromReferenceNode aliasRefNode = (FromReferenceNode)aliasRef;
			aliasRefNode.FromElement = alias;
		}

		IASTNode GenerateNamedParameter(IASTNode delimiterNode, IASTNode nameNode)
		{
			string name = nameNode.Text;
			TrackNamedParameterPositions(name);

			// create the node initially with the param name so that it shows
			// appropriately in the "original text" attribute
			ParameterNode parameter = (ParameterNode) adaptor.Create(NAMED_PARAM, name);
			parameter.Text = "?";

			NamedParameterSpecification paramSpec = new NamedParameterSpecification(
					delimiterNode.Line,
					delimiterNode.CharPositionInLine,
					name
			);

			parameter.HqlParameterSpecification = paramSpec;
			_parameters.Add(paramSpec);
			return parameter;
		}

		IASTNode GeneratePositionalParameter(IASTNode inputNode)
		{
			if (_namedParameters.Count > 0)
			{
				// NH TODO: remove this limitation
				throw new SemanticException("cannot define positional parameter after any named parameters have been defined");
			}
			ParameterNode parameter = (ParameterNode)adaptor.Create(PARAM, "?");
			PositionalParameterSpecification paramSpec = new PositionalParameterSpecification(
					inputNode.Line,
					inputNode.CharPositionInLine,
					_positionalParameterCount++
			);
			parameter.HqlParameterSpecification = paramSpec;
			_parameters.Add(paramSpec);
			return parameter;
		}

		public FromClause CurrentFromClause
		{
			get { return _currentFromClause; }
		}

		public int StatementType 
		{
			get { return _statementType; }
		}

		public LiteralProcessor LiteralProcessor
		{
			get { return _literalProcessor; }
		}

		public int CurrentClauseType
		{
				get { return _currentClauseType; }
		}

		public IDictionary<string, IFilter> EnabledFilters
		{
			get { return _qti.EnabledFilters; }
		}

		public bool IsSubQuery
		{
			get { return _level > 1; }
		}

		public bool IsSelectStatement
		{
			get { return _statementType == SELECT; }
		}

		public bool IsInFrom
		{
			get { return _inFrom; }
		}

		public bool IsInSelect
		{
				get { return _inSelect; }
		}

		public IDictionary<string, string> TokenReplacements
		{
			get { return _tokenReplacements; }
		}

		public bool IsComparativeExpressionClause
		{
			get
			{
				// Note: once we add support for "JOIN ... ON ...",
				// the ON clause needs to get included here
				return CurrentClauseType == WHERE ||
					   CurrentClauseType == WITH ||
					   IsInCase;
			}
		}

		public bool IsInCase
		{
			get { return _inCase; }
		}

		public bool IsShallowQuery
		{
			get 
			{
				// select clauses for insert statements should alwasy be treated as shallow
				return StatementType == INSERT ||  _qti.IsShallowQuery;
			}
		}

		public FromClause GetFinalFromClause()
		{
			FromClause top = _currentFromClause;
			while (top.ParentFromClause != null)
			{
				top = top.ParentFromClause;
			}
			return top;
		}


		// Helper methods

		public bool IsFilter()
		{
			return _collectionFilterRole != null;
		}

		public IASTFactory ASTFactory
		{
			get
			{
				if (_nodeFactory == null)
				{
					_nodeFactory = new ASTFactory(adaptor);
				}

				return _nodeFactory;
			}
		}

		public void AddQuerySpaces(string[] spaces)
		{
			for (int i = 0; i < spaces.Length; i++)
			{
				_querySpaces.Add(spaces[i]);
			}
		}

		private void TrackNamedParameterPositions(string name) 
		{
			int loc = _parameterCount++;
			object o = _namedParameters[name];
			if ( o == null ) 
			{
				_namedParameters.Add(name, loc);
			}
			else if (o is int)
			{
				List<int> list = new List<int>(4) {(int) o, loc};
				_namedParameters[name] = list;
			}
			else
			{
				((List<int>) o).Add(loc);
			}
		}

		private void HandleWithFragment(FromElement fromElement, IASTNode hqlWithNode)
		{
			try
			{
				ITreeNodeStream old = input;
				input = new CommonTreeNodeStream(adaptor, hqlWithNode);

				IASTNode hqlSqlWithNode = (IASTNode) withClause().Tree;
				input = old;

				if (log.IsDebugEnabled)
				{
					log.Debug("handleWithFragment() : " + _printer.ShowAsString(hqlSqlWithNode, "-- with clause --"));
				}
				WithClauseVisitor visitor = new WithClauseVisitor(fromElement);
				NodeTraverser traverser = new NodeTraverser(visitor);
				traverser.TraverseDepthFirst(hqlSqlWithNode);
				FromElement referencedFromElement = visitor.GetReferencedFromElement();
				if (referencedFromElement != fromElement)
				{
					throw new InvalidWithClauseException(
						"with-clause expressions did not reference from-clause element to which the with-clause was associated");
				}
				SqlGenerator sql = new SqlGenerator(_sessionFactoryHelper.Factory, new CommonTreeNodeStream(adaptor, hqlSqlWithNode.GetChild(0)));

				sql.whereExpr();

				var withClauseFragment = new SqlString("(", sql.GetSQL(), ")");
				fromElement.SetWithClauseFragment(visitor.GetJoinAlias(), withClauseFragment);
			}
			catch (SemanticException)
			{
				throw;
			}
			catch (InvalidWithClauseException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new SemanticException(e.Message);
			}
		}
	}

	class WithClauseVisitor : IVisitationStrategy 
	{
		private readonly FromElement _joinFragment;
		private FromElement _referencedFromElement;
		private String _joinAlias;

		public WithClauseVisitor(FromElement fromElement) 
		{
			_joinFragment = fromElement;
		}

		public void Visit(IASTNode node) 
		{
			// todo : currently expects that the individual with expressions apply to the same sql table join.
			//      This may not be the case for joined-subclass where the property values
			//      might be coming from different tables in the joined hierarchy.  At some
			//      point we should expand this to support that capability.  However, that has
			//      some difficulties:
			//          1) the biggest is how to handle ORs when the individual comparisons are
			//              linked to different sql joins.
			//          2) here we would need to track each comparison individually, along with
			//              the join alias to which it applies and then pass that information
			//              back to the FromElement so it can pass it along to the JoinSequence
			if ( node is DotNode ) 
			{
				DotNode dotNode = ( DotNode ) node;
				FromElement fromElement = dotNode.FromElement;
				if ( _referencedFromElement != null )
				{
					if ( fromElement != _referencedFromElement ) 
					{
						throw new HibernateException( "with-clause referenced two different from-clause elements" );
					}
				}
				else
				{
					_referencedFromElement = fromElement;
					_joinAlias = ExtractAppliedAlias( dotNode );

					// todo : temporary
					//      needed because currently persister is the one that
					//      creates and renders the join fragments for inheritence
					//      hierarchies...
					if ( _joinAlias != _referencedFromElement.TableAlias) 
					{
						throw new InvalidWithClauseException( "with clause can only reference columns in the driving table" );
					}
				}
			}
			else if ( node is ParameterNode ) 
			{
				ApplyParameterSpecification(((ParameterNode) node).HqlParameterSpecification);
			}
			else if ( node is IParameterContainer ) 
			{
				ApplyParameterSpecifications( ( IParameterContainer ) node );
			}
		}

		private void ApplyParameterSpecifications(IParameterContainer parameterContainer) 
		{
			if ( parameterContainer.HasEmbeddedParameters) 
			{
				IParameterSpecification[] specs = parameterContainer.GetEmbeddedParameters();
				for ( int i = 0; i < specs.Length; i++ ) 
				{
					ApplyParameterSpecification( specs[i] );
				}
			}
		}

		private void ApplyParameterSpecification(IParameterSpecification paramSpec) 
		{
			_joinFragment.AddEmbeddedParameter(paramSpec);
		}

		private static String ExtractAppliedAlias(IASTNode dotNode) 
		{
			return dotNode.Text.Substring( 0, dotNode.Text.IndexOf( '.' ) );
		}

		public FromElement GetReferencedFromElement() 
		{
			return _referencedFromElement;
		}

		public String GetJoinAlias() 
		{
			return _joinAlias;
		}
	}

}
