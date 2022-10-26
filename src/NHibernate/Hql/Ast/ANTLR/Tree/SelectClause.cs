using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Antlr.Runtime;
using NHibernate.Hql.Ast.ANTLR.Util;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Represents the list of expressions in a SELECT clause.
	/// Author: josh
	/// Ported by: Steve Strong
	/// </summary>
	[CLSCompliant(false)]
	public class SelectClause : SelectExpressionList
	{
		private const string JoinFetchWithoutOwnerExceptionMsg = "Query specified join fetching, but the owner of the fetched association was not present in the select list [{0}]";
		private bool _prepared;
		private bool _scalarSelect;
		private List<FromElement> _collectionFromElements;
		private IType[] _queryReturnTypes;
		// An 2d array of column names, the first dimension is parallel with the
		// return types array. The second dimension is the list of column names for each
		// type.
		private string[][] _columnNames;
		private readonly List<FromElement> _fromElementsForLoad = new List<FromElement>();
		private readonly Dictionary<int, int> _entityByResultTypeDic = new Dictionary<int, int>();

		private ConstructorNode _constructorNode;
		private string[] _aliases;
		private int[] _columnNamesStartPositions;
		private HashSet<ISelectExpression> _derivedSelectExpressions;
		private Dictionary<ISelectExpression, List<int>> _replacedExpressions = new Dictionary<ISelectExpression, List<int>>();

		internal List<ISelectExpression> SelectExpressions;
		internal List<ISelectExpression> OriginalSelectExpressions;
		internal List<ISelectExpression> NonScalarExpressions;

		public static bool VERSION2_SQL;

		public SelectClause(IToken token)
			: base(token)
		{
		}

		/// <summary>
		/// Prepares a derived (i.e., not explicitly defined in the query) select clause.
		/// </summary>
		/// <param name="fromClause">The from clause to which this select clause is linked.</param>
		public void InitializeDerivedSelectClause(FromClause fromClause)
		{
			if (_prepared)
			{
				throw new InvalidOperationException("SelectClause was already prepared!");
			}

			//Used to be tested by the TCK but the test is no longer here
			//		if ( getSessionFactoryHelper().isStrictJPAQLComplianceEnabled() && !getWalker().isSubQuery() ) {
			//			// NOTE : the isSubQuery() bit is a temporary hack...
			//			throw new QuerySyntaxException( "JPA-QL compliance requires select clause" );
			//		}
			var appender = new ASTAppender(ASTFactory, this);
			var fromElements = fromClause.GetProjectionListTyped();
			_derivedSelectExpressions = new HashSet<ISelectExpression>();
			foreach (FromElement fromElement in fromElements)
			{
				IType type;
				if (fromElement.IsFetch || fromElement.IsCollectionOfValuesOrComponents || ((type = fromElement.SelectType) == null))
				{
					continue;
				}

				var node = (IdentNode)appender.Append(HqlSqlWalker.IDENT, fromElement.ClassAlias ?? "", true);
				node.FromElement = fromElement;
				node.DataType = type;
				_derivedSelectExpressions.Add(node);
			}

			InitializeExplicitSelectClause(fromClause);
		}

		/// <summary>
		/// Prepares an explicitly defined select clause.
		/// </summary>
		/// <param name="fromClause">The from clause linked to this select clause.</param>
		/// <exception cref="SemanticException"></exception>
		public void InitializeExplicitSelectClause(FromClause fromClause)
		{
			if (_prepared)
			{
				throw new InvalidOperationException("SelectClause was already prepared!");
			}

			//explicit = true;	// This is an explict Select.
			//ArrayList sqlResultTypeList = new ArrayList();
			List<IType> queryReturnTypeList = new List<IType>();

			// First, collect all of the select expressions.
			// NOTE: This must be done *before* invoking setScalarColumnText() because setScalarColumnText()
			// changes the AST!!!
			var inheritedExpressions = new Dictionary<ISelectExpression, SelectClause>();
			var selExprs = GetSelectExpressions();
			OriginalSelectExpressions = selExprs;
			SelectExpressions = new List<ISelectExpression>(selExprs.Count);
			NonScalarExpressions = new List<ISelectExpression>();
			foreach (var expr in selExprs)
			{
				if (expr.IsConstructor)
				{
					_constructorNode = (ConstructorNode) expr;
					_scalarSelect = true;
					NonScalarExpressions.AddRange(_constructorNode.GetSelectExpressions(true, o => !o.IsScalar));
					foreach (var argumentExpression in _constructorNode.GetSelectExpressions())
					{
						SelectExpressions.Add(argumentExpression);
						AddExpression(argumentExpression, queryReturnTypeList);
					}
				}
				else if (expr.FromElement is JoinSubqueryFromElement joinSubquery &&
				         TryProcessSubqueryExpressions(expr, joinSubquery, out var selectClause, out var subqueryExpressions))
				{
					var indexes = new List<int>(subqueryExpressions.Count);
					foreach (var expression in subqueryExpressions)
					{
						inheritedExpressions[expression] = selectClause;
						indexes.Add(SelectExpressions.Count);
						SelectExpressions.Add(expression);
						AddExpression(expression, queryReturnTypeList);
					}

					_replacedExpressions.Add(expr, indexes);
				}
				else
				{
					if (!expr.IsScalar)
					{
						NonScalarExpressions.Add(expr);
					}

					SelectExpressions.Add(expr);
					AddExpression(expr, queryReturnTypeList);
				}
			}

			_queryReturnTypes = queryReturnTypeList.ToArray();

			// Init the aliases for explicit select, after initing the constructornode
			if (_derivedSelectExpressions == null)
			{
				InitAliases(SelectExpressions);
			}

			Render(fromClause, inheritedExpressions);

			FinishInitialization();
		}

		private bool TryProcessSubqueryExpressions(
			ISelectExpression selectExpression,
			JoinSubqueryFromElement joinSubquery,
			out SelectClause selectClause,
			out List<ISelectExpression> subqueryExpressions)
		{
			if (selectExpression is IdentNode)
			{
				selectClause = joinSubquery.QueryNode.GetSelectClause();
				subqueryExpressions = selectClause.SelectExpressions;
				NonScalarExpressions.Add(selectExpression);
			}
			else if (selectExpression is DotNode dotNode)
			{
				subqueryExpressions = joinSubquery.GetRelatedSelectExpressions(dotNode, out selectClause);
				if (subqueryExpressions == null)
				{
					return false;
				}

				if (!selectClause.IsScalarSelect)
				{
					RemoveChildAndUnsetParent((IASTNode) selectExpression);
				}

				foreach (var expression in subqueryExpressions)
				{
					if (!expression.IsScalar)
					{
						NonScalarExpressions.Add(expression);
					}
				}
			}
			else
			{
				selectClause = null;
				subqueryExpressions = null;
				return false;
			}

			return true;
		}

		private void Render(
			FromClause fromClause,
			Dictionary<ISelectExpression, SelectClause> inheritedExpressions)
		{
			if (_scalarSelect || Walker.IsShallowQuery)
			{
				// If there are any scalars (non-entities) selected, render the select column aliases.
				RenderScalarSelects(fromClause, inheritedExpressions);
				InitializeScalarColumnNames();
			}

			// generate id select fragment and then property select fragment for
			// each expression, just like generateSelectFragments().
			RenderNonScalarSelects(fromClause, inheritedExpressions, GetFetchedFromElements(fromClause));
		}

		private List<FromElement> GetFetchedFromElements(FromClause fromClause)
		{
			var fetchedFromElements = new List<FromElement>();
			if (Walker.IsShallowQuery)
			{
				return fetchedFromElements;
			}

			// add the fetched entities
			foreach (FromElement fromElement in fromClause.GetAllProjectionListTyped())
			{
				if (!fromElement.IsFetch)
				{
					continue;
				}

				var origin = GetOrigin(fromElement);

				// Only perform the fetch if its owner is included in the select 
				if (!_fromElementsForLoad.Contains(origin))
				{
					// NH-2846: Before 2012-01-18, we threw this exception. However, some
					// components using LINQ (e.g. paging) like to automatically append e.g. Count(). It
					// can then be difficult to avoid having a bogus fetch statement, so just ignore those.
					// An alternative solution may be to have the linq provider filter out the fetch instead.
					// throw new QueryException(string.Format(JoinFetchWithoutOwnerExceptionMsg, fromElement.GetDisplayText()));

					//throw away the fromElement. It's clearly redundant.
					if (fromElement.FromClause == fromClause)
					{
						fromElement.Parent.RemoveChild(fromElement);
					}
				}
				else
				{
					IType type = fromElement.SelectType;
					AddCollectionFromElement(fromElement);

					if (type != null && !fromElement.IsCollectionOfValuesOrComponents)
					{
						// Add the type to the list of returned sqlResultTypes.
						fromElement.IncludeSubclasses = true;
						_fromElementsForLoad.Add(fromElement);
						//sqlResultTypeList.add( type );
						// We will generate the select expression later in order to avoid having different
						// columns order when _scalarSelect is true
						fetchedFromElements.Add(fromElement);
					}
				}
			}

			return fetchedFromElements;
		}

		private void AddExpression(ISelectExpression expr, List<IType> queryReturnTypeList)
		{
			IType type = expr.DataType;
			if (type == null)
			{
				if (expr is ParameterNode param)
				{
					type = param.GuessedType;
				}
				else
					throw new QueryException("No data type for node: " + expr.GetType().Name + " " + new ASTPrinter().ShowAsString((IASTNode)expr, ""));
			}
			//sqlResultTypeList.add( type );

			// If the data type is not an association type, it could not have been in the FROM clause.
			if (expr.IsScalar)
			{
				_scalarSelect = true;
			}
			else if (IsReturnableEntity(expr))
			{
				AddEntityToProjection(queryReturnTypeList.Count, expr);
			}

			// Always add the type to the return type list.
			queryReturnTypeList.Add(type);
		}

		private void AddEntityToProjection(int resultIndex, ISelectExpression se)
		{
			_entityByResultTypeDic[resultIndex] = _fromElementsForLoad.Count;
			_fromElementsForLoad.Add(se.FromElement);
		}

		private static FromElement GetOrigin(FromElement fromElement)
		{
			var realOrigin = fromElement.RealOrigin;
			if (realOrigin != null)
				return realOrigin;

			// work around that crazy issue where the tree contains
			// "empty" FromElements (no text); afaict, this is caused
			// by FromElementFactory.createCollectionJoin()
			var origin = fromElement.Origin;
			if (origin == null)
				throw new QueryException("Unable to determine origin of join fetch [" + fromElement.GetDisplayText() + "]");

			return origin;
		}

		/// <summary>
		/// FromElements which need to be accounted for in the load phase (either for return or for fetch).
		/// </summary>
		public IList<FromElement> FromElementsForLoad
		{
			get { return _fromElementsForLoad; }
		}

		/// <summary>
		/// Maps QueryReturnTypes[key] to entities from FromElementsForLoad[value]
		/// </summary>
		internal IReadOnlyDictionary<int, int> EntityByResultTypeDic => _entityByResultTypeDic;

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
			get { return _constructorNode != null && _constructorNode.IsMap; }
		}

		public bool IsList
		{
			get { return _constructorNode != null && _constructorNode.IsList; }
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
					(fromElement.IsFetch || fromElement.IsCollectionOfValuesOrComponents);

			if (isFetchOrValueCollection)
			{
				return false;
			}
			else
			{
				return selectExpression.IsReturnableEntity;
			}
		}

		private void InitAliases(List<ISelectExpression> selectExpressions)
		{
			if (_constructorNode == null)
			{
				_aliases = new String[selectExpressions.Count];
				for (int i = 0; i < selectExpressions.Count; i++)
				{
					string alias = selectExpressions[i].Alias;
					_aliases[i] = alias ?? i.ToString();
				}
			}
			else
			{
				_aliases = _constructorNode.GetAliases();
			}
		}

		private void RenderNonScalarSelects(
			FromClause currentFromClause,
			Dictionary<ISelectExpression, SelectClause> inheritedExpressions,
			IList<FromElement> fetchedFromElements)
		{
			var appender = new ASTAppender(ASTFactory, this);
			var combinedFromElements = new List<FromElement>();
			var processedElements = new HashSet<FromElement>();
			RenderNonScalarIdentifiers(appender, processedElements, combinedFromElements, inheritedExpressions);
			if (Walker.IsShallowQuery)
			{
				return;
			}

			// Append fetched elements
			RenderFetchedNonScalarIdentifiers(appender, fetchedFromElements, processedElements, combinedFromElements);
			if (currentFromClause.IsScalarSubQuery)
			{
				return;
			}
			
			// Generate the property select tokens.
			foreach (var fromElement in combinedFromElements)
			{
				RenderNonScalarProperties(appender, fromElement);
			}

			// Generate properties for fetched collections of components or values
			var fromElements = currentFromClause.GetAllProjectionListTyped();
			foreach (var fromElement in fromElements)
			{
				if (fromElement.IsCollectionOfValuesOrComponents &&
					fromElement.IsFetch &&
					processedElements.Add(fromElement))
				{
					var suffix = Walker.GetSuffix(fromElement);
					var fragment = fromElement.GetValueCollectionSelectFragment(suffix);
					Append(appender, HqlSqlWalker.SQL_TOKEN, fragment);
				}
			}
		}

		private void RenderNonScalarIdentifiers(
			ASTAppender appender,
			HashSet<FromElement> processedElements,
			List<FromElement> combinedFromElements,
			Dictionary<ISelectExpression, SelectClause> inheritedExpressions)
		{
			foreach (var e in NonScalarExpressions)
			{
				var fromElement = e.FromElement;
				if (fromElement == null)
				{
					continue;
				}

				var node = (IASTNode) e;
				if (processedElements.Add(fromElement))
				{
					combinedFromElements.Add(fromElement);
					RenderNonScalarIdentifiers(fromElement, inheritedExpressions.ContainsKey(e) ? null : e, appender);
				}
				else if (!inheritedExpressions.ContainsKey(e) && node.Parent != null)
				{
					RemoveChildAndUnsetParent(node);
				}
			}
		}

		private void RenderFetchedNonScalarIdentifiers(
			ASTAppender appender,
			IList<FromElement> fetchedFromElements,
			HashSet<FromElement> processedElements,
			List<FromElement> combinedFromElements)
		{
			foreach (var fetchedFromElement in fetchedFromElements)
			{
				if (!processedElements.Add(fetchedFromElement))
				{
					continue;
				}

				fetchedFromElement.EntitySuffix = Walker.GetEntitySuffix(fetchedFromElement);
				combinedFromElements.Add(fetchedFromElement);
				var fragment = fetchedFromElement.GetIdentifierSelectFragment(fetchedFromElement.EntitySuffix);
				if (fragment == null)
				{
					// When a subquery join has a scalar select only
					continue;
				}

				var generatedExpr = (SelectExpressionImpl) Append(appender, HqlSqlWalker.SELECT_EXPR, fragment);
				generatedExpr.FromElement = fetchedFromElement;
				generatedExpr.DataType = fetchedFromElement.DataType;
				NonScalarExpressions.Add(generatedExpr);
			}
		}

		private IASTNode Append(ASTAppender appender, int type, SelectFragment fragment)
		{
			if (fragment == null)
			{
				return null;
			}

			return appender.Append(type, fragment.ToSqlStringFragment(false), false);
		}

		private void RenderNonScalarIdentifiers(FromElement fromElement, ISelectExpression expr, ASTAppender appender)
		{
			if (fromElement.FromClause.IsScalarSubQuery && _derivedSelectExpressions?.Contains(expr) != true)
			{
				return;
			}

			if (Walker.IsShallowQuery && !fromElement.FromClause.IsScalarSubQuery && SelectExpressions.Contains(expr))
			{
				// A scalar column was generated
				return;
			}

			fromElement.EntitySuffix = Walker.GetEntitySuffix(fromElement);
			var fragment = fromElement.GetIdentifierSelectFragment(fromElement.EntitySuffix);
			if (fragment == null)
			{
				// When a subquery join has a scalar select only
				RemoveChildAndUnsetParent((IASTNode) expr);
				return;
			}

			if ((!_scalarSelect || fromElement.Type == HqlSqlWalker.JOIN_SUBQUERY) && expr != null)
			{
				//TODO: is this a bit ugly?
				expr.Text = fragment.ToSqlStringFragment(false);
			}
			else
			{
				Append(appender, HqlSqlWalker.SQL_TOKEN, fragment);
			}
		}

		private void RenderNonScalarProperties(ASTAppender appender, FromElement fromElement)
		{
			var suffix = fromElement.EntitySuffix;
			var fragment = fromElement.GetPropertiesSelectFragment(suffix);
			Append(appender, HqlSqlWalker.SQL_TOKEN, fragment);

			if (fromElement.QueryableCollection != null && fromElement.IsFetch)
			{
				fragment = fromElement.GetCollectionSelectFragment(suffix);
				Append(appender, HqlSqlWalker.SQL_TOKEN, fragment);
			}
		}

		internal List<ISelectExpression> GetReplacedExpressions(ISelectExpression expression)
		{
			if (!_replacedExpressions.TryGetValue(expression, out var indexes))
			{
				return null;
			}

			return indexes.Select(o => SelectExpressions[o]).ToList();
		}

		internal string[] GetScalarColumns(ISelectExpression expression)
		{
			if (_replacedExpressions.TryGetValue(expression, out var indexes))
			{
				var columns = new List<string>();
				foreach (var index in indexes)
				{
					columns.AddRange(ColumnNames[index]);
				}

				return columns.ToArray();
			}
			else
			{
				if (!IsScalarSelect)
				{
					return expression.FromElement
					                 .GetIdentifierSelectFragment(expression.FromElement.EntitySuffix)
					                 .GetColumnAliases()
					                 .ToArray();
				}

				var index = SelectExpressions.IndexOf(expression);
				if (index < 0)
				{
					throw new InvalidOperationException($"Unable to get scalar columns for expression: {expression}");
				}

				return ColumnNames[index];
			}
		}

		private void RenderScalarSelects(
			FromClause currentFromClause,
			Dictionary<ISelectExpression, SelectClause> inheritedExpressions)
		{
			if (currentFromClause.IsScalarSubQuery)
			{
				return;
			}

			List<int> deprecateExpressions = null; // 6.0 TODO: Remove 
			_columnNames = new string[SelectExpressions.Count][];
			for (var i = 0; i < SelectExpressions.Count; i++)
			{
				var expr = SelectExpressions[i];
				if (inheritedExpressions.TryGetValue(expr, out var selectClause))
				{
					_columnNames[i] = selectClause.GetScalarColumns(expr);

					continue;
				}

				_columnNames[i] = expr.SetScalarColumn(Walker.CurrentScalarIndex++, NameGenerator.ScalarName); // Create SQL_TOKEN nodes for the columns.
				// 6.0 TODO: Remove 
				if (_columnNames[i] == null)
				{
					if (deprecateExpressions == null)
					{
						deprecateExpressions = new List<int>();
					}

					deprecateExpressions.Add(i);
				}
			}

			// 6.0 TODO: Remove 
			if (deprecateExpressions != null)
			{
#pragma warning disable 618
				var columnNames = SessionFactoryHelper.GenerateColumnNames(_queryReturnTypes);
#pragma warning restore 618
				foreach (var index in deprecateExpressions)
				{
					_columnNames[index] = columnNames[index];
				}
			}
		}

		private void AddCollectionFromElement(FromElement fromElement)
		{
			if (!fromElement.IsFetch)
			{
				return;
			}

			var suffix = Walker.GetCollectionSuffix(fromElement);
			if (suffix == null)
			{
				return;
			}

			if (_collectionFromElements == null)
			{
				_collectionFromElements = new List<FromElement>();
			}

			_collectionFromElements.Add(fromElement);
			fromElement.CollectionSuffix = suffix;
		}

		private void FinishInitialization()
		{
			_prepared = true;
		}

		private void InitializeScalarColumnNames()
		{
			if (_columnNames == null)
			{
				return;
			}

			_columnNamesStartPositions = new int[_columnNames.Length];
			var startPosition = 1;
			for (var i = 0; i < _columnNames.Length; i++)
			{
				_columnNamesStartPositions[i] = startPosition;
				startPosition += _columnNames[i].Length;
			}
		}

		public int GetColumnNamesStartPosition(int i)
		{
			return _columnNamesStartPositions[i];
		}

		private static void RemoveChildAndUnsetParent(IASTNode node)
		{
			if (node?.Parent != null)
			{
				node.Parent.RemoveChild(node);
				node.Parent = null;
			}
		}
	}
}
