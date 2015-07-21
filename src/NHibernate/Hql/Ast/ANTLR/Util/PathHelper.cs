using System;

using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Util;

namespace NHibernate.Hql.Ast.ANTLR.Util
{
	[CLSCompliant(false)]
	public static class PathHelper
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(PathHelper));

		/// <summary>
		/// Turns a path into an AST.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="factory">The AST factory to use.</param>
		/// <returns>An HQL AST representing the path.</returns>
		public static IASTNode ParsePath(string path, IASTFactory factory)
		{
			string[] identifiers = StringHelper.Split(".", path);
			IASTNode lhs = null;
			for (int i = 0; i < identifiers.Length; i++)
			{
				string identifier = identifiers[i];
				IASTNode child = factory.CreateNode(HqlSqlWalker.IDENT, identifier);
				if (i == 0)
				{
					lhs = child;
				}
				else
				{
					lhs = factory.CreateNode(HqlSqlWalker.DOT, ".", lhs, child);
				}
			}
			if (log.IsDebugEnabled)
			{
				log.Debug("parsePath() : " + path + " -> " + ASTUtil.GetDebugstring(lhs));
			}
			return lhs;
		}

		public static string GetAlias(string path)
		{
			return StringHelper.Root(path);
		}
	}
}
