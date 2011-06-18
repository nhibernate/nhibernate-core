using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Hql.Ast.ANTLR.Util;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Impl
{
	internal class ExpressionQueryImpl : AbstractQueryImpl
	{
		private readonly Dictionary<string, LockMode> _lockModes = new Dictionary<string, LockMode>(2);

		public ExpressionQueryImpl(IQueryExpression queryExpression, ISessionImplementor session, ParameterMetadata parameterMetadata)
			: base(queryExpression.Key, FlushMode.Unspecified, session, parameterMetadata)
		{
			QueryExpression = queryExpression;
		}

		public IQueryExpression QueryExpression { get; private set; }

		protected internal override IDictionary<string, LockMode> LockModes
		{
			get { return _lockModes; }
		}

		public override IQuery SetLockMode(string alias, LockMode lockMode)
		{
			_lockModes[alias] = lockMode;
			return this;
		}

		public override int ExecuteUpdate()
		{
			throw new NotImplementedException();
		}

		public override IEnumerable Enumerable()
		{
			throw new NotImplementedException();
		}

		public override IEnumerable<T> Enumerable<T>()
		{
			throw new NotImplementedException();
		}

		public override IList List()
		{
			VerifyParameters();
			IDictionary<string, TypedValue> namedParams = NamedParams;
			Before();
			try
			{
				return Session.List(ExpandParameters(namedParams), GetQueryParameters(namedParams));
			}
			finally
			{
				After();
			}
		}

		/// <summary> 
		/// Warning: adds new parameters to the argument by side-effect, as well as mutating the query expression tree!
		/// </summary>
		protected IQueryExpression ExpandParameters(IDictionary<string, TypedValue> namedParamsCopy)
		{
			if (namedParameterLists.Count == 0)
			{
				// Short circuit straight out
				return QueryExpression;
			}

			// Build a map from single parameters to lists
			var map = new Dictionary<string, List<string>>();

			foreach (var me in namedParameterLists)
			{
				string name = me.Key;
				var vals = (ICollection) me.Value.Value;
				IType type = me.Value.Type;

				if (vals.Count == 1)
				{
					// No expansion needed here
					IEnumerator iter = vals.GetEnumerator();
					iter.MoveNext();
					namedParamsCopy[name] = new TypedValue(type, iter.Current, Session.EntityMode);
					continue;
				}

				var aliases = new List<string>();
				int i = 0;
				bool isJpaPositionalParam = parameterMetadata.GetNamedParameterDescriptor(name).JpaStyle;

				foreach (object obj in vals)
				{
					string alias = (isJpaPositionalParam ? 'x' + name : name + StringHelper.Underscore) + i++ + StringHelper.Underscore;
					namedParamsCopy[alias] = new TypedValue(type, obj, Session.EntityMode);
					aliases.Add(alias);
				}

				map.Add(name, aliases);
			}

			IASTNode newTree = ParameterExpander.Expand(QueryExpression.Translate(Session.Factory), map);
			var key = new StringBuilder(QueryExpression.Key);

			map.Aggregate(key, (sb, kvp) =>
			                   {
			                   	sb.Append(' ');
			                   	sb.Append(kvp.Key);
			                   	sb.Append(':');
			                   	kvp.Value.Aggregate(sb, (sb2, str) => sb2.Append(str));
			                   	return sb;
			                   });

			return new ExpandedQueryExpression(QueryExpression, newTree, key.ToString());
		}

		public override void List(IList results)
		{
			throw new NotImplementedException();
		}

		public override IList<T> List<T>()
		{
			throw new NotImplementedException();
		}
	}

	internal class ExpandedQueryExpression : IQueryExpression
	{
		private readonly IASTNode _tree;

		public ExpandedQueryExpression(IQueryExpression queryExpression, IASTNode tree, string key)
		{
			_tree = tree;
			Key = key;
			Type = queryExpression.Type;
			ParameterDescriptors = queryExpression.ParameterDescriptors;
		}

		#region IQueryExpression Members

		public IASTNode Translate(ISessionFactoryImplementor sessionFactory)
		{
			return _tree;
		}

		public string Key { get; private set; }

		public System.Type Type { get; private set; }

		public IList<NamedParameterDescriptor> ParameterDescriptors { get; private set; }

		#endregion
	}

	internal class ParameterExpander
	{
		private readonly Dictionary<string, List<string>> _map;
		private readonly IASTNode _tree;

		private ParameterExpander(IASTNode tree, Dictionary<string, List<string>> map)
		{
			_tree = tree;
			_map = map;
		}

		public static IASTNode Expand(IASTNode tree, Dictionary<string, List<string>> map)
		{
			var expander = new ParameterExpander(tree, map);

			return expander.Expand();
		}

		private IASTNode Expand()
		{
			IList<IASTNode> parameters = ParameterDetector.LocateParameters(_tree, new HashSet<string>(_map.Keys));
			var nodeMapping = new Dictionary<IASTNode, IEnumerable<IASTNode>>();

			foreach (IASTNode param in parameters)
			{
				IASTNode paramName = param.GetChild(0);
				List<string> aliases = _map[paramName.Text];
				var astAliases = new List<IASTNode>();

				foreach (string alias in aliases)
				{
					IASTNode astAlias = param.DupNode();
					IASTNode astAliasName = paramName.DupNode();
					astAliasName.Text = alias;
					astAlias.AddChild(astAliasName);

					astAliases.Add(astAlias);
				}

				nodeMapping.Add(param, astAliases);
			}

			return DuplicateTree(_tree, nodeMapping);
		}

		private static IASTNode DuplicateTree(IASTNode ast, IDictionary<IASTNode, IEnumerable<IASTNode>> nodeMapping)
		{
			IASTNode thisNode = ast.DupNode();

			foreach (IASTNode child in ast)
			{
				IEnumerable<IASTNode> candidate;

				if (nodeMapping.TryGetValue(child, out candidate))
				{
					foreach (IASTNode replacement in candidate)
					{
						thisNode.AddChild(replacement);
					}
				}
				else
				{
					thisNode.AddChild(DuplicateTree(child, nodeMapping));
				}
			}

			return thisNode;
		}
	}

	internal class ParameterDetector : IVisitationStrategy
	{
		private readonly List<IASTNode> _nodes;
		private readonly HashSet<string> _parameterNames;
		private readonly IASTNode _tree;

		private ParameterDetector(IASTNode tree, HashSet<string> parameterNames)
		{
			_tree = tree;
			_parameterNames = parameterNames;
			_nodes = new List<IASTNode>();
		}

		#region IVisitationStrategy Members

		public void Visit(IASTNode node)
		{
			if ((node.Type == HqlSqlWalker.PARAM) || (node.Type == HqlSqlWalker.COLON))
			{
				string name = node.GetChild(0).Text;

				if (_parameterNames.Contains(name))
				{
					_nodes.Add(node);
				}
			}
		}

		#endregion

		public static IList<IASTNode> LocateParameters(IASTNode tree, HashSet<string> parameterNames)
		{
			var detector = new ParameterDetector(tree, parameterNames);

			return detector.LocateParameters();
		}

		private IList<IASTNode> LocateParameters()
		{
			var nodeTraverser = new NodeTraverser(this);
			nodeTraverser.TraverseDepthFirst(_tree);

			return _nodes;
		}
	}
}