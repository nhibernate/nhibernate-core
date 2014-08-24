using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Hql.Util;

namespace NHibernate.Hql.Ast.ANTLR
{
    internal class PolymorphicQuerySourceDetector
    {
        private readonly ISessionFactoryImplementor _sfi;
        private readonly Dictionary<IASTNode, IASTNode[]> _map = new Dictionary<IASTNode, IASTNode[]>();
        private readonly SessionFactoryHelper _sessionFactoryHelper;

        public PolymorphicQuerySourceDetector(ISessionFactoryImplementor sfi)
        {
            _sfi = sfi;
            _sessionFactoryHelper = new SessionFactoryHelper(sfi);
        }

				public Dictionary<IASTNode, IASTNode[]> Process(IASTNode tree)
				{
					foreach (var querySource in new QuerySourceDetector(tree).LocateQuerySources())
					{
						var className = GetClassName(querySource);
						string[] implementors = _sfi.GetImplementors(className);
						AddImplementorsToMap(querySource, className, implementors);
					}

					return _map;
				}

				private void AddImplementorsToMap(IASTNode querySource, string className, string[] implementors)
				{
					if (implementors.Length == 1 && implementors[0] == className)
					{
						// No need to change things
						return;
					}

					_map.Add(querySource,
					         implementors.Select(implementor => MakeIdent(querySource, implementor)).ToArray());
				}

    	private static string GetClassName(IASTNode querySource)
        {
            switch (querySource.Type)
            {
                case HqlSqlWalker.IDENT:
                    return querySource.Text;
                case HqlSqlWalker.DOT:
                    return BuildPath(querySource);
            }

            // TODO
            throw new NotSupportedException();
        }

        private static IASTNode MakeIdent(IASTNode source, string text)
        {
            var ident = source.DupNode();
            ident.Type = HqlSqlWalker.IDENT;
            ident.Text = text;
            return ident;
        }

        private static string BuildPath(IASTNode node)
        {
            var sb = new StringBuilder();
            BuildPath(node, sb);
            return sb.ToString();
        }

        private static void BuildPath(IASTNode node, StringBuilder sb)
        {
            if (node.Type == HqlSqlWalker.DOT)
            {
                BuildPath(node.GetChild(0), sb);

                sb.Append('.');
                sb.Append(node.GetChild(1).Text);
            }
            else
            {
                sb.Append(node.Text);
            }
        }

    }
}