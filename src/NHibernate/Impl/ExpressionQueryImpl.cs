using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Engine.Query.Sql;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Hql.Ast.ANTLR.Util;
using NHibernate.Hql.Classic;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Impl
{
    class ExpressionQueryImpl : AbstractQueryImpl
    {
        private readonly Dictionary<string, LockMode> _lockModes = new Dictionary<string, LockMode>(2);

        public IQueryExpression QueryExpression { get; private set; }

        public ExpressionQueryImpl(IQueryExpression queryExpression, ISessionImplementor session, ParameterMetadata parameterMetadata)
            : base(queryExpression.Key, FlushMode.Unspecified, session, parameterMetadata)
        {
            QueryExpression = queryExpression;
        }

        public override IQuery SetLockMode(string alias, LockMode lockMode)
        {
            _lockModes[alias] = lockMode;
            return this;
        }

        protected internal override IDictionary<string, LockMode> LockModes
        {
            get { return _lockModes; }
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
				var type = me.Value.Type;
				
				if (vals.Count == 1)
				{
					// No expansion needed here
					var iter = vals.GetEnumerator();
					iter.MoveNext();
                    namedParamsCopy[name] = new TypedValue(type, iter.Current, Session.EntityMode);
					continue;
				}
				
				var aliases = new List<string>();
                var i = 0;
                var isJpaPositionalParam = parameterMetadata.GetNamedParameterDescriptor(name).JpaStyle;

                foreach (var obj in vals)
	            {
	                var alias = (isJpaPositionalParam ? 'x' + name : name + StringHelper.Underscore) + i++ + StringHelper.Underscore;
	                namedParamsCopy[alias] = new TypedValue(type, obj, Session.EntityMode);
					aliases.Add(alias);
	            }
				
				map.Add(name, aliases);
				
			}

            var newTree = ParameterExpander.Expand(QueryExpression.Translate(Session.Factory), map);
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

        public IASTNode Translate(ISessionFactory sessionFactory)
        {
            return _tree;
        }

        public string Key { get; private set; }

        public System.Type Type { get; private set; }

        public IList<NamedParameterDescriptor> ParameterDescriptors { get; private set; }
    }

    internal class ParameterExpander
	{
	    private readonly IASTNode _tree;
	    private readonly Dictionary<string, List<string>> _map;

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
	        var parameters = ParameterDetector.LocateParameters(_tree, new HashSet<string>(_map.Keys));
	        var nodeMapping = new Dictionary<IASTNode, IEnumerable<IASTNode>>();

            foreach (var param in parameters)
            {
                var paramName = param.GetChild(0);
                var aliases = _map[paramName.Text];
                var astAliases = new List<IASTNode>();

                foreach (var alias in aliases)
                {
                    var astAlias = param.DupNode();
                    var astAliasName = paramName.DupNode();
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
            var thisNode = ast.DupNode();

            foreach (var child in ast)
            {
                IEnumerable<IASTNode> candidate;

                if (nodeMapping.TryGetValue(child, out candidate))
                {
                   foreach (var replacement in candidate)
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
        private readonly IASTNode _tree;
        private readonly HashSet<string> _parameterNames;
        private readonly List<IASTNode> _nodes;

        private ParameterDetector(IASTNode tree, HashSet<string> parameterNames)
        {
            _tree = tree;
            _parameterNames = parameterNames;
            _nodes = new List<IASTNode>();
        }

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

        public void Visit(IASTNode node)
        {
            if ((node.Type == HqlSqlWalker.PARAM) || (node.Type == HqlSqlWalker.COLON))
            {
                var name = node.GetChild(0).Text;

                if (_parameterNames.Contains(name))
                {
                    _nodes.Add(node);
                }
            }
        }

    }
}