using System;
using System.Text;
using NHibernate.Util;

namespace NHibernate.Sql 
{
	/// <summary>
	/// An ANSI-style Join.
	/// </summary>
	public class ANSIJoinFragment : JoinFragment 
	{
		private StringBuilder buffer = new StringBuilder();
		private StringBuilder conditions = new StringBuilder();

		public override void AddJoin(string tableName, string alias, string[] fkColumns, string[] pkColumns, JoinType joinType) 
		{
			string joinString = null;
			switch (joinType) 
			{
				case JoinType.InnerJoin:
					joinString = " inner join ";
					break;
				case JoinType.LeftOuterJoin:
					joinString = " left outer join ";
					break;
				case JoinType.RightOuterJoin:
					joinString = " right outer join ";
					break;
				case JoinType.FullJoin:
					joinString = " full outer join ";
					break;
				default:
					throw new AssertionFailure("undefind join type");
			}

			buffer.Append(joinString)
				.Append(tableName)
				.Append(' ')
				.Append(alias)
				.Append(" on ");

			for (int j=0; j<fkColumns.Length; j++) 
			{
				if (fkColumns[j].IndexOf('.')<1) throw new AssertionFailure("missing alias");
				buffer.Append( fkColumns[j] )
					.Append('=')
					.Append(alias)
					.Append(StringHelper.Dot)
					.Append( pkColumns[j] );
				if (j<fkColumns.Length-1) buffer.Append(" and ");
			}
		}

		public override string ToFromFragmentString 
		{
			get { return buffer.ToString(); }
		}

		public override string ToWhereFragmentString 
		{
			get { return conditions.ToString(); }
		}

		public override void AddJoins(string fromFragment, string whereFragment) 
		{
			buffer.Append(fromFragment);
			//where fragment must be empty!
		}

		public override JoinFragment Copy() 
		{
			ANSIJoinFragment copy = new ANSIJoinFragment();
			copy.buffer = new StringBuilder( buffer.ToString() );
			return copy;
		}

		public override void AddCondition(string alias, string[] columns, string condition) 
		{
			for (int i=0; i<columns.Length; i++) 
			{
				conditions.Append(" and ")
					.Append(alias)
					.Append(StringHelper.Dot)
					.Append(columns[i])
					.Append(conditions);
			}
		}

		public override void AddCrossJoin(string tableName, string alias) 
		{
			buffer.Append(StringHelper.CommaSpace)
				.Append(tableName)
				.Append(' ')
				.Append(alias);
		}

		public override void AddCondition(string alias, string[] fkColumns, string[] pkColumns) 
		{
			throw new NotSupportedException();
		}

		public override void AddCondition(string condition) 
		{
			throw new NotSupportedException();
		}

		
	}
}
