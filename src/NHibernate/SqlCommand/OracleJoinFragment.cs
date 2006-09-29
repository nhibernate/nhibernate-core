using System;
using System.Text;
using Iesi.Collections;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// An Oracle-style (theta) Join
	/// </summary>
	public class OracleJoinFragment : JoinFragment
	{
		private SqlStringBuilder afterFrom = new SqlStringBuilder();
		private SqlStringBuilder afterWhere = new SqlStringBuilder();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="alias"></param>
		/// <param name="fkColumns"></param>
		/// <param name="pkColumns"></param>
		/// <param name="joinType"></param>
		public override void AddJoin( string tableName, string alias, string[ ] fkColumns, string[ ] pkColumns, JoinType joinType )
		{
			AddCrossJoin( tableName, alias );

			for( int j = 0; j < fkColumns.Length; j++ )
			{
				//HasThetaJoins = true;
				afterWhere.Add( " and " + fkColumns[ j ] );
				if( joinType == JoinType.RightOuterJoin || joinType == JoinType.FullJoin )
				{
					afterWhere.Add( "(+)" );
				}

				afterWhere.Add( "=" + alias + StringHelper.Dot + pkColumns[ j ] );

				if( joinType == JoinType.LeftOuterJoin || joinType == JoinType.FullJoin )
				{
					afterWhere.Add( "(+)" );
				}
			}
		}

		public override void AddJoin(string tableName, string alias, string[] fkColumns, string[] pkColumns, JoinType joinType, string on)
		{
			//arbitrary on clause ignored!!
			AddJoin(tableName, alias, fkColumns, pkColumns, joinType);
			if (joinType == JoinType.InnerJoin)
			{
				AddCondition(on);
			}
			else if (joinType == JoinType.LeftOuterJoin)
			{
				AddLeftOuterJoinCondition(on);
			}
			else
			{
				throw new NotSupportedException("join type not supported by OracleJoinFragment (use Oracle9Dialect)");
			}
		}


		
		/// <summary>
		/// This method is a bit of a hack, and assumes
		/// that the column on the "right" side of the
		/// join appears on the "left" side of the
		/// operator, which is extremely wierd if this
		/// was a normal join condition, but is natural
		/// for a filter.
		/// </summary>
		private void AddLeftOuterJoinCondition(string on)
		{
			StringBuilder buf = new StringBuilder(on);
			for (int i = 0; i < buf.Length; i++)
			{
				char character = buf[i];
				bool isInsertPoint = Operators.Contains(character) ||
				                     (character == ' ' && buf.Length > i + 3 && "is ".Equals(buf.ToString(i + 1, 3)));
				if (isInsertPoint)
				{
					buf.Insert(i, "(+)");
					i += 3;
				}
			}
			AddCondition(buf.ToString());
		}

		private static readonly ISet Operators = new HashedSet();

		static OracleJoinFragment()
		{
			Operators.Add('=');
			Operators.Add('<');
			Operators.Add('>');
		}



		/// <summary></summary>
		public override SqlString ToFromFragmentString
		{
			get { return afterFrom.ToSqlString(); }
		}

		/// <summary></summary>
		public override SqlString ToWhereFragmentString
		{
			get { return afterWhere.ToSqlString(); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fromFragment"></param>
		/// <param name="whereFragment"></param>
		public override void AddJoins( SqlString fromFragment, SqlString whereFragment )
		{
			afterFrom.Add( fromFragment );
			afterWhere.Add( whereFragment );
		}

		public override JoinFragment Copy()
		{
			OracleJoinFragment copy = new OracleJoinFragment();
			copy.afterFrom = new SqlStringBuilder( afterFrom.ToSqlString() );
			copy.afterWhere = new SqlStringBuilder( afterWhere.ToSqlString() );
			return copy;
		}

		public override void AddCondition( string alias, string[ ] columns, string condition )
		{
			for( int i = 0; i < columns.Length; i++ )
			{
				afterWhere.Add( " and " + alias + StringHelper.Dot + columns[ i ] + condition );
			}
		}

		public override void AddCondition( string alias, string[ ] columns, string condition, IType conditionType, ISessionFactoryImplementor factory )
		{
			for( int i = 0; i < columns.Length; i++ )
			{
				afterWhere.Add( " and " + alias + StringHelper.Dot + columns[ i ] + condition );
				afterWhere.AddParameter();
			}
		}

		public override void AddCrossJoin( string tableName, string alias )
		{
			afterFrom.Add( StringHelper.CommaSpace + tableName + " " + alias );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="fkColumns"></param>
		/// <param name="pkColumns"></param>
		public override void AddCondition( string alias, string[ ] fkColumns, string[ ] pkColumns )
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="condition"></param>
		public override bool AddCondition( string condition )
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="condition"></param>
		public override bool AddCondition( SqlString condition )
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fromFragmentString"></param>
		public override void AddFromFragmentString( SqlString fromFragmentString )
		{
			afterFrom.Add( fromFragmentString );
		}
	}
}