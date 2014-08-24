using System;
using NHibernate.Hql.Ast.ANTLR.Tree;

namespace NHibernate.Hql.Ast.ANTLR.Util
{
	[CLSCompliant(false)]
	public class ColumnHelper
	{
		public static void GenerateSingleScalarColumn(IASTFactory factory, IASTNode node, int i)
		{
			node.AddSibling(factory.CreateNode(HqlSqlWalker.SELECT_COLUMNS, " as " + NameGenerator.ScalarName(i, 0)));
		}

		/// <summary>
		/// Generates the scalar column AST nodes for a given array of SQL columns
		/// </summary>
		public static void GenerateScalarColumns(IASTFactory factory, IASTNode node, string[] sqlColumns, int i)
		{
			if (sqlColumns.Length == 1)
			{
				GenerateSingleScalarColumn(factory, node, i);
			}
			else
			{
				node.Text = sqlColumns[0]; // Use the DOT node to emit the first column name.

				// Create the column names, folled by the column aliases.
				for (int j = 0; j < sqlColumns.Length; j++)
				{
					if (j > 0)
					{
						node = node.AddSibling(factory.CreateNode(HqlSqlWalker.SQL_TOKEN, sqlColumns[j]));
					}

					node = node.AddSibling(factory.CreateNode(HqlSqlWalker.SELECT_COLUMNS, " as " + NameGenerator.ScalarName(i, j)));
				}
			}
		}
	}
}