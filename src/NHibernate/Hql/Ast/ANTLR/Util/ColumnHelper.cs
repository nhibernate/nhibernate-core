using System;
using NHibernate.Hql.Ast.ANTLR.Tree;

namespace NHibernate.Hql.Ast.ANTLR.Util
{
	[CLSCompliant(false)]
	public class ColumnHelper
	{
		// Since v5.4
		[Obsolete("Use overload with aliasCreator parameter instead.")]
		public static void GenerateSingleScalarColumn(IASTFactory factory, IASTNode node, int i)
		{
			GenerateSingleScalarColumn(factory, node, i, NameGenerator.ScalarName);
		}

		public static string GenerateSingleScalarColumn(IASTFactory factory, IASTNode node, int i, Func<int, int, string> aliasCreator)
		{
			var alias = aliasCreator(i, 0);
			node.AddSibling(factory.CreateNode(HqlSqlWalker.SELECT_COLUMNS, " as " + alias));
			return alias;
		}

		/// <summary>
		/// Generates the scalar column AST nodes for a given array of SQL columns
		/// </summary>
		// Since v5.4
		[Obsolete("Use overload with aliasCreator parameter instead.")]
		public static void GenerateScalarColumns(IASTFactory factory, IASTNode node, string[] sqlColumns, int i)
		{
			GenerateScalarColumns(factory, node, sqlColumns, i, NameGenerator.ScalarName);
		}

		/// <summary>
		/// Generates the scalar column AST nodes for a given array of SQL columns
		/// </summary>
		public static string[] GenerateScalarColumns(IASTFactory factory, IASTNode node, string[] sqlColumns, int i,
		                                             Func<int, int, string> aliasCreator)
		{
			if (sqlColumns.Length == 1)
			{
				return new[] {GenerateSingleScalarColumn(factory, node, i, aliasCreator)};
			}

			var aliases = new string[sqlColumns.Length];
			node.Text = sqlColumns[0]; // Use the DOT node to emit the first column name.

			// Create the column names, folled by the column aliases.
			for (var j = 0; j < sqlColumns.Length; j++)
			{
				if (j > 0)
				{
					node = node.AddSibling(factory.CreateNode(HqlSqlWalker.SQL_TOKEN, sqlColumns[j]));
				}

				aliases[j] = aliasCreator(i, j);
				node = node.AddSibling(factory.CreateNode(HqlSqlWalker.SELECT_COLUMNS, " as " + aliases[j]));
			}

			return aliases;
		}
	}
}
