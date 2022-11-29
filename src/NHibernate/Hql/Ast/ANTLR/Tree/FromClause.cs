using System;
using System.Collections.Generic;
using System.Linq;

using Antlr.Runtime;

using NHibernate.Hql.Ast.ANTLR.Util;
using NHibernate.Util;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	// 6.0 TODO: consider retyping methods yielding IList<IASTNode> as IList<FromElement>
	// They all do actually yield FromElement, and most of their callers end up recasting
	// them.
	/// <summary>
	/// Represents the 'FROM' part of a query or subquery, containing all mapped class references.
	/// Author: josh
	/// Ported by: Steve Strong
	/// </summary>
	[CLSCompliant(false)]
	public class FromClause : HqlSqlWalkerNode, IDisplayableNode
	{
		private static readonly INHibernateLogger Log = NHibernateLogger.For(typeof(FromClause));
		private const int RootLevel = 1;

		private int _level = RootLevel;

		/// <summary>
		/// Counts the from elements as they are added.
		/// </summary>
#pragma warning disable 649
		private int _fromElementCounter;
#pragma warning restore 649

		private readonly NullableDictionary<string, FromElement> _fromElementByClassAlias = new NullableDictionary<string, FromElement>();
		private readonly Dictionary<string, FromElement> _fromElementByTableAlias = new Dictionary<string, FromElement>();
		private readonly NullableDictionary<string, FromElement> _fromElementsByPath = new NullableDictionary<string, FromElement>();
		private readonly List<FromElement> _fromElements = new List<FromElement>();
		private readonly List<FromElement> _appendFromElements = new List<FromElement>(); // Used for entity and subquery joins

		/// <summary>
		/// All of the implicit FROM xxx JOIN yyy elements that are the destination of a collection.  These are created from
		/// index operators on collection property references.
		/// </summary>
		private readonly NullableDictionary<string, FromElement> _collectionJoinFromElementsByPath = new NullableDictionary<string, FromElement>();

		/// <summary>
		/// Pointer to the parent FROM clause, if there is one.
		/// </summary>
		private FromClause _parentFromClause;

		/// <summary>
		/// Collection of FROM clauses of which this is the parent.
		/// </summary>
		private ISet<FromClause> _childFromClauses;

		public FromClause(IToken token) : base(token)
		{
		}

		public void SetParentFromClause(FromClause parentFromClause)
		{
			_parentFromClause = parentFromClause;
			if (parentFromClause != null)
			{
				_level = parentFromClause.Level + 1;
				parentFromClause.AddChild(this);
			}
		}

		public FromClause ParentFromClause
		{
			get { return _parentFromClause; }
		}
		
		//6.0 TODO: Replace with Typed version below
		public IList<IASTNode> GetExplicitFromElements()
		{
			return ASTUtil.CollectChildren<IASTNode>(this, ExplicitFromPredicate);
		}
		
		internal IList<FromElement> GetExplicitFromElementsTyped()
		{
			return ASTUtil.CollectChildren<FromElement>(this, ExplicitFromPredicate);
		}
		
		//6.0 TODO: Replace with Typed version below
		public IList<IASTNode> GetCollectionFetches()
		{
			return ASTUtil.CollectChildren<IASTNode>(this, CollectionFetchPredicate);
		}

		internal IList<FromElement> GetCollectionFetchesTyped()
		{
			return ASTUtil.CollectChildren<FromElement>(this, CollectionFetchPredicate);
		}

		public FromElement FindCollectionJoin(String path)
		{
			return _collectionJoinFromElementsByPath[path];
		}

		/// <summary>
		/// Convenience method to check whether a given token represents a from-element alias.
		/// </summary>
		/// <param name="possibleAlias">The potential from-element alias to check.</param>
		/// <returns>True if the possibleAlias is an alias to a from-element visible from this point in the query graph.</returns>
		public bool IsFromElementAlias(string possibleAlias)
		{
			bool isAlias = ContainsClassAlias(possibleAlias);
			if (!isAlias && _parentFromClause != null)
			{
				// try the parent FromClause...
				isAlias = _parentFromClause.IsFromElementAlias(possibleAlias);
			}
			return isAlias;
		}

		/// <summary>
		/// Returns true if the from node contains the class alias name.
		/// </summary>
		/// <param name="alias">The HQL class alias name.</param>
		/// <returns>true if the from node contains the class alias name.</returns>
		public bool ContainsClassAlias(string alias)
		{
			if (_fromElementByClassAlias.ContainsKey(alias))
			{
				return true;
			}
			if (SessionFactoryHelper.IsStrictJPAQLComplianceEnabled)
			{
				return FindIntendedAliasedFromElementBasedOnCrazyJPARequirements(alias) != null;
			}
			return false;
		}

		/// <summary>
		/// Returns true if the from node contains the table alias name.
		/// </summary>
		/// <param name="alias">The SQL table alias name.</param>
		/// <returns>true if the from node contains the table alias name.</returns>
		public bool ContainsTableAlias(String alias)
		{
			return _fromElementByTableAlias.ContainsKey(alias);
		}

		public void AddJoinByPathMap(string path, FromElement destination)
		{
			if (Log.IsDebugEnabled())
			{
				Log.Debug("addJoinByPathMap() : {0} -> {1}", path, destination);
			}

			_fromElementsByPath.Add(path, destination);
		}

		public void AddCollectionJoinFromElementByPath(string path, FromElement destination)
		{
			if (Log.IsDebugEnabled())
			{
				Log.Debug("addCollectionJoinFromElementByPath() : {0} -> {1}", path, destination);
			}
			_collectionJoinFromElementsByPath.Add(path, destination);	// Add the new node to the map so that we don't create it twice.
		}

		private void AddChild(FromClause fromClause)
		{
			if (_childFromClauses == null)
			{
				_childFromClauses = new HashSet<FromClause>();
			}
			_childFromClauses.Add(fromClause);
		}

		/// <summary>
		/// Adds a new from element to the from node.
		/// </summary>
		/// <param name="path">The reference to the class.</param>
		/// <param name="alias">The alias AST.</param>
		/// <returns>The new FROM element.</returns>
		public FromElement AddFromElement(string path, IASTNode alias)
		{
			// The path may be a reference to an alias defined in the parent query.
			string classAlias = ( alias == null ) ? null : alias.Text;
			CheckForDuplicateClassAlias( classAlias );
			var factory = new FromElementFactory(this, null, path, classAlias, null, false);
			return factory.AddFromElement();
		}

		/// <summary>
		/// Retrieves the from-element represented by the given alias.
		/// </summary>
		/// <param name="aliasOrClassName">The alias by which to locate the from-element.</param>
		/// <returns>The from-element assigned the given alias, or null if none.</returns>
		public FromElement GetFromElement(string aliasOrClassName)
		{
			FromElement fromElement;
			
			_fromElementByClassAlias.TryGetValue(aliasOrClassName, out fromElement);

			if (fromElement == null && SessionFactoryHelper.IsStrictJPAQLComplianceEnabled)
			{
				fromElement = FindIntendedAliasedFromElementBasedOnCrazyJPARequirements(aliasOrClassName);
			}
			if (fromElement == null && _parentFromClause != null)
			{
				fromElement = _parentFromClause.GetFromElement(aliasOrClassName);
			}
			return fromElement;
		}

		//6.0 TODO: Replace with Typed version below
		/// <summary>
		/// Returns the list of from elements in order.
		/// </summary>
		/// <returns>The list of from elements (instances of FromElement).</returns>
		public IList<IASTNode> GetFromElements()
		{
			return ASTUtil.CollectChildren<IASTNode>(this, node => FromElementPredicate(node, this));
		}

		internal IList<FromElement> GetFromElementsTyped()
		{
			return ASTUtil.CollectChildren<FromElement>(this, node => FromElementPredicate(node, this));
		}

		//6.0 TODO: Replace with Typed version below
		/// <summary>
		/// Returns the list of from elements that will be part of the result set.
		/// </summary>
		/// <returns>the list of from elements that will be part of the result set.</returns>
		public IList<IASTNode> GetProjectionList()
		{
			return ASTUtil.CollectChildren<IASTNode>(this, node => ProjectionListPredicate(node, this));
		}

		internal IList<FromElement> GetProjectionListTyped()
		{
			return ASTUtil.CollectChildren<FromElement>(this, node => ProjectionListPredicate(node, this));
		}

		internal IList<FromElement> GetAllProjectionListTyped()
		{
			return ASTUtil.CollectChildren<FromElement>(this, node => AllProjectionListPredicate(node));
		}

		public FromElement GetFromElement()
		{
			return GetFromElementsTyped()[0];
		}

		public void AddDuplicateAlias(string alias, FromElement element)
		{
			if (alias != null)
			{
				_fromElementByClassAlias.Add(alias, element);
			}
		}

		/// <summary>
		/// Look for an existing implicit or explicit join by the given path.
		/// </summary>
		public FromElement FindJoinByPath(string path)
		{
			FromElement elem = FindJoinByPathLocal(path);
			if (elem == null && _parentFromClause != null)
			{
				elem = _parentFromClause.FindJoinByPath(path);
			}
			return elem;
		}

		int Level
		{
		   get { return _level; }	
		}

		public bool IsSubQuery
		{
			get 
			{
				// TODO : this is broke for subqueries in statements other than selects...
				return _parentFromClause != null;
			}
		}

		internal bool IsScalarSubQuery => IsSubQuery && !IsJoinSubQuery;

		internal bool IsJoinSubQuery { get; set; }

		public string GetDisplayText()
		{
			return "FromClause{" +
				   "level=" + _level +
				   ", fromElementCounter=" + _fromElementCounter +
				   ", fromElements=" + _fromElements.Count +
				   ", fromElementByClassAlias=" + _fromElementByClassAlias.Keys +
				   ", fromElementByTableAlias=" + _fromElementByTableAlias.Keys +
				   ", fromElementsByPath=" + _fromElementsByPath.Keys +
				   ", collectionJoinFromElementsByPath=" + _collectionJoinFromElementsByPath.Keys +
				   "}";
		}

		private void CheckForDuplicateClassAlias(string classAlias)
		{
			if ( classAlias != null && _fromElementByClassAlias.ContainsKey( classAlias ) ) 
			{
				throw new QueryException( "Duplicate definition of alias '" + classAlias + "'" );
			}
		}

		private static bool ProjectionListPredicate(IASTNode node, FromClause fromClause)
		{
			return node is FromElement fromElement &&
			       fromElement.InProjectionList &&
			       // Skip in case node is within a join subquery
			       fromElement.FromClause == fromClause;
		}

		private static bool AllProjectionListPredicate(IASTNode node)
		{
			return node is FromElement fromElement && fromElement.InProjectionList;
		}

		private static bool FromElementPredicate(IASTNode node, FromClause fromClause) 
		{
			var fromElement = node as FromElement;

			if (fromElement != null)
			{
				return fromElement.IsFromOrJoinFragment &&
					// Skip in case node is within a join subquery
					fromElement.FromClause == fromClause;
			}

			return false;
		}

		static bool ExplicitFromPredicate(IASTNode node)
		{
			var fromElement = node as FromElement;

			if (fromElement != null)
			{
				return !fromElement.IsImplied;
			}

			return false;
		}

		private static bool CollectionFetchPredicate(IASTNode node)
		{
			var fromElement = node as FromElement;

			if (fromElement != null)
			{
				return fromElement.IsFetch && (fromElement.QueryableCollection != null);
			}

			return false;
		}

		private FromElement FindIntendedAliasedFromElementBasedOnCrazyJPARequirements(string specifiedAlias)
		{
			foreach (var entry in _fromElementByClassAlias)
			{
				string alias = entry.Key;
				if (string.Equals(alias, specifiedAlias, StringComparison.InvariantCultureIgnoreCase))
				{
					return entry.Value;
				}
			}
			return null;
		}

		public void RegisterFromElement(FromElement element)
		{
			_fromElements.Add(element);
			string classAlias = element.ClassAlias;
			if (classAlias != null)
			{
				// The HQL class alias refers to the class name.
				_fromElementByClassAlias.Add(classAlias, element);
			}
			// Associate the table alias with the element.
			string tableAlias = element.TableAlias;
			if (tableAlias != null)
			{
				_fromElementByTableAlias[tableAlias] = element;
			}

			if (element.IsEntityJoin())
			{
				_appendFromElements.Add((EntityJoinFromElement) element);
			}
		}

		internal void AppendFromElement(FromElement element)
		{
			_appendFromElements.Add(element);
		}

		private FromElement FindJoinByPathLocal(string path)
		{
			return _fromElementsByPath[path];
		}

		public override string ToString()
		{
			return "FromClause{" + "level=" + _level + "}";
		}

		public virtual void Resolve()
		{
			// Make sure that all from elements registered with this FROM clause are actually in the AST.
			var childrenInTree = new HashSet<IASTNode>();
			foreach (var ast in new ASTIterator(GetFirstChild()))
			{
				childrenInTree.Add(ast);
			}
			foreach (var fromElement in _fromElements)
			{
				if (!childrenInTree.Contains(fromElement))
				{
					throw new SemanticException("Element not in AST: " + fromElement);
				}
			}
		}

		public FromElement GetFromElementByClassName(string className)
		{
			return _fromElementByClassAlias.Values.FirstOrDefault(variable => variable.ClassName == className);
		}

		internal void FinishInit()
		{
			foreach (var item in _appendFromElements)
			{
				var dependentElement = GetFirstDependentFromElement(item);
				if (dependentElement == null)
				{
					AddChild(item);
				}
				else
				{
					var index = dependentElement.ChildIndex;
					dependentElement.Parent.InsertChild(index, item);
				}
			}
			_appendFromElements.Clear();
		}

		private FromElement GetFirstDependentFromElement(FromElement element)
		{
			foreach (var fromElement in _fromElements)
			{
				if (fromElement == element ||
					fromElement.WithClauseFromElements?.Contains(element) != true ||
				    // Parent will be null for entity and subquery joins
				    fromElement.Parent == null)
				{
					continue;
				}

				return fromElement;
			}

			return null;
		}
	}
}
