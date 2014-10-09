using System;
using System.Globalization;
using System.Text;
using Antlr.Runtime;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Represents a reference to a result_variable as defined in the JPA 2 spec.
	/// </summary>
	/// <example>
	/// <code>select v as value from tab1 order by value</code>
	/// "value" used in the order by clause is a reference to the result_variable, "value", defined in the select clause.
	/// </example>
	/// Author: Gail Badner
	public class ResultVariableRefNode : HqlSqlWalkerNode
	{
		private ISelectExpression _selectExpression;
		
		public ResultVariableRefNode(IToken token)
			: base(token)
		{
		}

		public void SetSelectExpression(ISelectExpression selectExpression)
		{
			if (selectExpression == null || selectExpression.Alias == null)
			{
				throw new SemanticException("A ResultVariableRefNode must refer to a non-null alias.");
			}
			_selectExpression = selectExpression;
		}


		public override SqlString RenderText(ISessionFactoryImplementor sessionFactory)
		{
			int scalarColumnIndex = _selectExpression.ScalarColumnIndex;
			if (scalarColumnIndex < 0)
			{
				throw new QueryException("selectExpression.ScalarColumnIndex must be >= 0; actual = " + scalarColumnIndex);
			}

			return sessionFactory.Dialect.ReplaceResultVariableInOrderByClauseWithPosition
				? GetColumnPositionsString(scalarColumnIndex)
				: GetColumnNamesString(scalarColumnIndex);
		}

		private SqlString GetColumnPositionsString(int scalarColumnIndex)
		{
			int startPosition = Walker.SelectClause.GetColumnNamesStartPosition(scalarColumnIndex);
			SqlStringBuilder buf = new SqlStringBuilder();
			int nColumns = Walker.SelectClause.ColumnNames[scalarColumnIndex].Length;
			for (int i = startPosition; i < startPosition + nColumns; i++)
			{
				if (i > startPosition)
				{
					buf.Add(", ");
				}
				buf.Add(i.ToString(CultureInfo.InvariantCulture));
			}
			return buf.ToSqlString();
		}

		private SqlString GetColumnNamesString(int scalarColumnIndex)
		{
			return SqlStringHelper.Join(new SqlString(", "), Walker.SelectClause.ColumnNames[scalarColumnIndex]);
		}
	}
}
