using System;

using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// An ANSI-style Join.
	/// </summary>
	public class ANSIJoinFragment : JoinFragment
	{
		private SqlStringBuilder buffer = new SqlStringBuilder();
		private SqlStringBuilder conditions = new SqlStringBuilder();

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

			buffer.Add( joinString + tableName + ' ' + alias + " on " );

			for (int j=0; j<fkColumns.Length; j++) 
			{
				if (fkColumns[j].IndexOf('.')<1) throw new AssertionFailure("missing alias");
				buffer.Add( fkColumns[j] + "=" + alias + StringHelper.Dot + pkColumns[j] );
				if (j<fkColumns.Length-1) buffer.Add(" and ");
			}
		}

		public override SqlString ToFromFragmentString 
		{
			get { return buffer.ToSqlString(); }
		}

		public override SqlString ToWhereFragmentString 
		{
			get { return conditions.ToSqlString(); }
		}

		public override void AddJoins(SqlString fromFragment, SqlString whereFragment) 
		{
			buffer.Add(fromFragment);
			//where fragment must be empty!
		}

		public override JoinFragment Copy() 
		{
			ANSIJoinFragment copy = new ANSIJoinFragment();
			copy.buffer = new SqlStringBuilder( buffer.ToSqlString() );
			return copy;
		}

		public override void AddCondition(string alias, string[] columns, string condition) 
		{
			for (int i=0; i<columns.Length; i++) 
			{
				conditions.Add( " and " + alias + StringHelper.Dot + columns[i] + condition );
			}
		}

		public override void AddCondition(string alias, string[] columns, string condition, IType conditionType, ISessionFactoryImplementor factory)
		{
			Parameter[] parameters = Parameter.GenerateParameters(factory, alias, columns, conditionType);
			for( int i=0; i<columns.Length; i++) 
			{
				conditions.Add( " and " + alias + StringHelper.Dot + columns[i] + condition );
				conditions.Add( parameters[i] );
			}
		}


		public override void AddCrossJoin(string tableName, string alias) 
		{
			buffer.Add( StringHelper.CommaSpace + tableName + " " + alias);
		}

		public override void AddCondition(string alias, string[] fkColumns, string[] pkColumns) 
		{
			throw new NotSupportedException();
		}

		public override void AddCondition(string condition) 
		{
			throw new NotSupportedException();
		}

		public override void AddCondition(SqlString condition)
		{
			throw new NotSupportedException();
		}


	}
}
