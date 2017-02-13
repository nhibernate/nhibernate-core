using System;
using System.Collections.Generic;
using Antlr.Runtime;

using NHibernate.Hql.Ast.ANTLR.Util;
using NHibernate.Util;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Represents the 'FROM' part of a query or subquery, containing all mapped class references.
	/// Author: josh
	/// Ported by: Steve Strong
	/// </summary>
	[CLSCompliant(false)]
	public class FromClause : HqlSqlWalkerNode, IDisplayableNode
	{
		private static readonly IInternalLogger Log = LoggerProvider.LoggerFor(typeof(FromClause));
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

		public IList<IASTNode> GetExplicitFromElements()
		{
			return ASTUtil.CollectChildren(this, ExplicitFromPredicate);
		}

		public IList<IASTNode> GetCollectionFetches()
		{
			return ASTUtil.CollectChildren(this, CollectionFetchPredicate);
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
			if (Log.IsDebugEnabled)
			{
				Log.Debug("addJoinByPathMap() : " + path + " -> " + destination);
			}

			_fromElementsByPath.Add(path, destination);
		}

		public void AddCollectionJoinFromElementByPath(string path, FromElement destination)
		{
			if (Log.IsDebugEnabled)
			{
				Log.Debug("addCollectionJoinFromElementByPath() : " + path + " -> " + destination);
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

		/// <summary>
		/// Returns the list of from elements in order.
		/// </summary>
		/// <returns>The list of from elements (instances of FromElement).</returns>
		public IList<IASTNode> GetFromElements()
		{
			return ASTUtil.CollectChildren(this, FromElementPredicate);
		}

		/// <summary>
		/// Returns the list of from elements that will be part of the result set.
		/// </summary>
		/// <returns>the list of from elements that will be part of the result set.</returns>
		public IList<IASTNode> GetProjectionList()
		{
			return ASTUtil.CollectChildren(this, ProjectionListPredicate);
		}

		public FromElement GetFromElement()
		{
			return (FromElement)GetFromElements()[0];
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

		private static bool ProjectionListPredicate(IASTNode node)
		{
			var fromElement = node as FromElement;

			if (fromElement != null)
			{
				return fromElement.InProjectionList;
			}

			return false;
		}

		private static bool FromElementPredicate(IASTNode node) 
		{
			var fromElement = node as FromElement;

			if (fromElement != null)
			{
				return fromElement.IsFromOrJoinFragment;
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
			var iter = (new ASTIterator(GetFirstChild())).GetEnumerator();
			var childrenInTree = new HashSet<IASTNode>();
			while (iter.MoveNext())
			{
				childrenInTree.Add(iter.Current);
			}
			foreach (var fromElement in _fromElements)
			{
				if (!childrenInTree.Contains(fromElement))
				{
					throw new SemanticException("Element not in AST: " + fromElement);
				}
			}
		}
	}
}
