using System;
using System.Collections.Generic;
using System.Reflection;
using Antlr.Runtime;
using NHibernate.Hql.Ast.ANTLR.Util;
using NHibernate.Type;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Represents the list of expressions in a SELECT clause.
	/// Author: josh
	/// Ported by: Steve Strong
	/// </summary>
	public class SelectClause : SelectExpressionList
	{
		private const string JoinFetchWithoutOwnerExceptionMsg = "Query specified join fetching, but the owner of the fetched association was not present in the select list [{0}]";
		private bool _prepared;
		private bool _scalarSelect;
		private List<FromElement> _collectionFromElements;
		private IType[] _queryReturnTypes;
		private string[][] _columnNames;
		private readonly List<FromElement> _fromElementsForLoad = new List<FromElement>();
		private ConstructorNode _constructorNode;
		private string[] _aliases;

		public static bool VERSION2_SQL = false;

		public SelectClause(IToken token) : base(token)
		{
		}

		/**
		 * Prepares a derived (i.e., not explicitly defined in the query) select clause.
		 *
		 * @param fromClause The from clause to which this select clause is linked.
		 */
		public void InitializeDerivedSelectClause(FromClause fromClause)
		{
			if ( _prepared ) 
			{
				throw new InvalidOperationException( "SelectClause was already prepared!" );
			}

			//Used to be tested by the TCK but the test is no longer here
	//		if ( getSessionFactoryHelper().isStrictJPAQLComplianceEnabled() && !getWalker().isSubQuery() ) {
	//			// NOTE : the isSubQuery() bit is a temporary hack...
	//			throw new QuerySyntaxException( "JPA-QL compliance requires select clause" );
	//		}
			IList<IASTNode> fromElements = fromClause.GetProjectionList();

			ASTAppender appender = new ASTAppender( ASTFactory, this );	// Get ready to start adding nodes.
			int size = fromElements.Count;
			List<IType> sqlResultTypeList = new List<IType>(size);
			List<IType> queryReturnTypeList = new List<IType>(size);

			int k = 0;
			foreach (FromElement fromElement in fromElements)
			{
				IType type = fromElement.SelectType;

				AddCollectionFromElement( fromElement );

				if ( type != null ) 
				{
					bool collectionOfElements = fromElement.IsCollectionOfValuesOrComponents;
					if ( !collectionOfElements ) 
					{
						if ( !fromElement.IsFetch )
						{
							// Add the type to the list of returned sqlResultTypes.
							queryReturnTypeList.Add( type );
						}

						_fromElementsForLoad.Add( fromElement );
						sqlResultTypeList.Add( type );

						// Generate the select expression.
						string text = fromElement.RenderIdentifierSelect( size, k );

						SelectExpressionImpl generatedExpr = ( SelectExpressionImpl ) appender.Append( HqlSqlWalker.SELECT_EXPR, text, false );
						if ( generatedExpr != null ) 
						{
							generatedExpr.FromElement =  fromElement;
						}
					}
				}
				k++;
			}

			// Get all the select expressions (that we just generated) and render the select.
			ISelectExpression[] selectExpressions = CollectSelectExpressions();

			if ( Walker.IsShallowQuery ) 
			{
				RenderScalarSelects( selectExpressions, fromClause );
			}
			else 
			{
				RenderNonScalarSelects( selectExpressions, fromClause );
			}

			FinishInitialization( /*sqlResultTypeList,*/ queryReturnTypeList );
		}

				/**
		 * Prepares an explicitly defined select clause.
		 *
		 * @param fromClause The from clause linked to this select clause.
		 * @throws SemanticException
		 */
		public void InitializeExplicitSelectClause(FromClause fromClause)
		{
			if ( _prepared ) {
				throw new InvalidOperationException( "SelectClause was already prepared!" );
			}

			//explicit = true;	// This is an explict Select.
			//ArrayList sqlResultTypeList = new ArrayList();
			List<IType> queryReturnTypeList = new List<IType>();

			// First, collect all of the select expressions.
			// NOTE: This must be done *before* invoking setScalarColumnText() because setScalarColumnText()
			// changes the AST!!!
			ISelectExpression[] selectExpressions = CollectSelectExpressions();
			
			for ( int i = 0; i < selectExpressions.Length; i++ ) 
			{
				ISelectExpression expr = selectExpressions[i];

				if ( expr.IsConstructor )
				{
					_constructorNode = ( ConstructorNode ) expr;
					IList<IType> constructorArgumentTypeList = _constructorNode.ConstructorArgumentTypeList;
					//sqlResultTypeList.addAll( constructorArgumentTypeList );
					queryReturnTypeList.AddRange(constructorArgumentTypeList );
					_scalarSelect = true;
				}
				else 
				{
					IType type = expr.DataType;
					if ( type == null ) 
					{
						throw new QueryException("No data type for node: " + expr.GetType().Name + " " + new ASTPrinter().ShowAsString((IASTNode)expr, "" ) );
					}
					//sqlResultTypeList.add( type );

					// If the data type is not an association type, it could not have been in the FROM clause.
					if ( expr.IsScalar ) 
					{
						_scalarSelect = true;
					}

					if ( IsReturnableEntity( expr ) ) 
					{
						_fromElementsForLoad.Add( expr.FromElement );
					}

					// Always add the type to the return type list.
					queryReturnTypeList.Add( type );
				}
			}

			//init the aliases, after initing the constructornode
			InitAliases(selectExpressions);

			if ( !Walker.IsShallowQuery )
			{
				// add the fetched entities
				IList<IASTNode> fromElements = fromClause.GetProjectionList();
		
				ASTAppender appender = new ASTAppender( ASTFactory, this );	// Get ready to start adding nodes.
				int size = fromElements.Count;
				int k = 0;
				
				foreach (FromElement fromElement in fromElements)
				{		
					if ( fromElement.IsFetch ) 
					{
						FromElement origin;
						if ( fromElement.RealOrigin == null ) 
						{
							// work around that crazy issue where the tree contains
							// "empty" FromElements (no text); afaict, this is caused
							// by FromElementFactory.createCollectionJoin()
							if ( fromElement.Origin == null ) 
							{
								throw new QueryException( "Unable to determine origin of join fetch [" + fromElement.GetDisplayText() + "]" );
							}
							else 
							{
								origin = fromElement.Origin;
							}
						}
						else
						{
							origin = fromElement.RealOrigin;
						}

						if (!_fromElementsForLoad.Contains(origin))
						{
							throw new QueryException(string.Format(JoinFetchWithoutOwnerExceptionMsg, fromElement.GetDisplayText()));
						}

						IType type = fromElement.SelectType;
						AddCollectionFromElement( fromElement );

						if ( type != null ) {
							bool collectionOfElements = fromElement.IsCollectionOfValuesOrComponents;
							if ( !collectionOfElements ) 
							{
								// Add the type to the list of returned sqlResultTypes.
								fromElement.IncludeSubclasses = true;
								_fromElementsForLoad.Add( fromElement );
								//sqlResultTypeList.add( type );
								// Generate the select expression.
								String text = fromElement.RenderIdentifierSelect( size, k );
								SelectExpressionImpl generatedExpr = ( SelectExpressionImpl ) appender.Append( HqlSqlWalker.SELECT_EXPR, text, false );
								if ( generatedExpr != null ) {
									generatedExpr.FromElement = fromElement;
								}
							}
						}
					}
				}
		
				// generate id select fragment and then property select fragment for
				// each expression, just like generateSelectFragments().
				RenderNonScalarSelects( CollectSelectExpressions(), fromClause );
			}

			if ( _scalarSelect || Walker.IsShallowQuery ) 
			{
				// If there are any scalars (non-entities) selected, render the select column aliases.
				RenderScalarSelects( selectExpressions, fromClause );
			}

			FinishInitialization( /*sqlResultTypeList,*/ queryReturnTypeList );
		}

		/// <summary>
		/// FromElements which need to be accounted for in the load phase (either for return or for fetch).
		/// </summary>
		public IList<FromElement> FromElementsForLoad
		{
			get { return _fromElementsForLoad; }
		}

		public bool IsScalarSelect
		{
			get { return _scalarSelect; }
		}

		public bool IsDistinct
		{
			get { return ChildCount > 0 && GetChild(0).Type == HqlSqlWalker.DISTINCT; }
		}

		/// <summary>
		/// The column alias names being used in the generated SQL.
		/// </summary>
		public string[][] ColumnNames
		{
			get { return _columnNames; }
		}

		/// <summary>
		/// The constructor to use for dynamic instantiation queries.
		/// </summary>
		public ConstructorInfo Constructor
		{
			get { return _constructorNode == null ? null : _constructorNode.Constructor; }
		}

		public bool IsMap
		{
			get { return _constructorNode == null ? false : _constructorNode.IsMap; }
		}

		public bool IsList
		{
			get { return _constructorNode == null ? false : _constructorNode.IsList; }
		}

		/// <summary>
		/// The HQL aliases, or generated aliases
		/// </summary>
		public string[] QueryReturnAliases
		{
			get { return _aliases; }
		}

		public IList<FromElement> CollectionFromElements
		{
			get { return _collectionFromElements; }
		}

		/// <summary>
		/// The types actually being returned from this query at the "object level".
		/// </summary>
		public IType[] QueryReturnTypes
		{
			get { return _queryReturnTypes; }
		}

		protected internal override IASTNode GetFirstSelectExpression()
		{
			foreach (IASTNode child in this)
			{
				if (!(child.Type == HqlSqlWalker.DISTINCT || child.Type == HqlSqlWalker.ALL))
				{
					return child;
				}
			}

		    return null;
		}

		private static bool IsReturnableEntity(ISelectExpression selectExpression)
		{
			FromElement fromElement = selectExpression.FromElement;
			bool isFetchOrValueCollection = fromElement != null && 
					( fromElement.IsFetch || fromElement.IsCollectionOfValuesOrComponents );
 
			if ( isFetchOrValueCollection ) 
			{
				return false;
			}
			else 
			{
				return selectExpression.IsReturnableEntity;
			}
		}

		private void InitAliases(ISelectExpression[] selectExpressions)
		{
			if (_constructorNode == null)
			{
				_aliases = new String[selectExpressions.Length];
				for (int i = 0; i < selectExpressions.Length; i++)
				{
					string alias = selectExpressions[i].Alias;
					_aliases[i] = alias == null ? i.ToString() : alias;
				}
			}
			else
			{
				_aliases = _constructorNode.GetAliases();
			}
		}

		private void RenderNonScalarSelects(ISelectExpression[] selectExpressions, FromClause currentFromClause) 
		{
			ASTAppender appender = new ASTAppender( ASTFactory, this );
			int size = selectExpressions.Length;
			int nonscalarSize = 0;

			for ( int i = 0; i < size; i++ ) 
			{
				if ( !selectExpressions[i].IsScalar)
				{
					nonscalarSize++;
				}
			}

			int j = 0;
			for ( int i = 0; i < size; i++ ) 
			{
				if ( !selectExpressions[i].IsScalar )
				{
					ISelectExpression expr = selectExpressions[i];
					FromElement fromElement = expr.FromElement;
					if ( fromElement != null ) 
					{
						RenderNonScalarIdentifiers( fromElement, nonscalarSize, j, expr, appender );
						j++;
					}
				}
			}

			if ( !currentFromClause.IsSubQuery) 
			{
				// Generate the property select tokens.
				int k = 0;
				for ( int i = 0; i < size; i++ ) 
				{
					if ( !selectExpressions[i].IsScalar) 
					{
						FromElement fromElement = selectExpressions[i].FromElement;
						if ( fromElement != null ) 
						{
							RenderNonScalarProperties( appender, fromElement, nonscalarSize, k );
							k++;
						}
					}
				}
			}
		}

		private void RenderNonScalarIdentifiers(FromElement fromElement, int nonscalarSize, int j, ISelectExpression expr, ASTAppender appender)
		{
			string text = fromElement.RenderIdentifierSelect(nonscalarSize, j);

			if (!fromElement.FromClause.IsSubQuery)
			{
				if (!_scalarSelect && !Walker.IsShallowQuery)
				{
					//TODO: is this a bit ugly?
					expr.Text = text;
				}
				else
				{
					appender.Append(HqlSqlWalker.SQL_TOKEN, text, false);
				}
			}
		}

		private static void RenderNonScalarProperties(ASTAppender appender, FromElement fromElement, int nonscalarSize, int k)
		{
			string text = fromElement.RenderPropertySelect(nonscalarSize, k);
			appender.Append(HqlSqlWalker.SQL_TOKEN, text, false);

			if (fromElement.QueryableCollection != null && fromElement.IsFetch)
			{
				text = fromElement.RenderCollectionSelectFragment(nonscalarSize, k);
				appender.Append(HqlSqlWalker.SQL_TOKEN, text, false);
			}

			// Look through the FromElement's children to find any collections of values that should be fetched...
			ASTIterator iter = new ASTIterator(fromElement);
			foreach (FromElement child in iter)
			{
				if (child.IsCollectionOfValuesOrComponents && child.IsFetch)
				{
					// Need a better way to define the suffixes here...
					text = child.RenderValueCollectionSelectFragment(nonscalarSize, nonscalarSize + k);
					appender.Append(HqlSqlWalker.SQL_TOKEN, text, false);
				}
			}
		}

		private static void RenderScalarSelects(ISelectExpression[] se, FromClause currentFromClause)
		{
			if ( !currentFromClause.IsSubQuery) 
			{
				for ( int i = 0; i < se.Length; i++ ) 
				{
					ISelectExpression expr = se[i];
					expr.SetScalarColumnText( i );	// Create SQL_TOKEN nodes for the columns.
				}
			}
		}

		private void AddCollectionFromElement(FromElement fromElement)
		{
			if (fromElement.IsFetch)
			{
				if (fromElement.CollectionJoin || fromElement.QueryableCollection != null)
				{
					String suffix;
					if (_collectionFromElements == null)
					{
						_collectionFromElements = new List<FromElement>();
						suffix = VERSION2_SQL ? "__" : "0__";
					}
					else
					{
						suffix = _collectionFromElements.Count.ToString() + "__";
					}
					_collectionFromElements.Add(fromElement);
					fromElement.CollectionSuffix = suffix;
				}
			}
		}

		private void FinishInitialization(/*ArrayList sqlResultTypeList,*/ List<IType> queryReturnTypeList)
		{
			//sqlResultTypes = ( Type[] ) sqlResultTypeList.toArray( new Type[sqlResultTypeList.size()] );
			_queryReturnTypes = queryReturnTypeList.ToArray();
			InitializeColumnNames();
			_prepared = true;
		}

		private void InitializeColumnNames()
		{
			// Generate an 2d array of column names, the first dimension is parallel with the
			// return types array.  The second dimension is the list of column names for each
			// type.

			// todo: we should really just collect these from the various SelectExpressions, rather than regenerating here
			_columnNames = SessionFactoryHelper.GenerateColumnNames(_queryReturnTypes);
		}

	}
}
