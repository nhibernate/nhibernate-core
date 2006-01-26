using System;
using NHibernate.SqlCommand;

namespace NHibernate.JetDriver
{
	/// <summary>
	/// Jet engine does not support FULL JOINS.
	/// 
	/// <p>
	/// Author: <a href="mailto:lukask@welldatatech.com">Lukas Krejci</a>
	/// </p>
	/// </summary>
	public class JetJoinFragment : ANSIJoinFragment
	{
		/// <summary>
		/// Jet engine does not support full joins.
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="alias"></param>
		/// <param name="fkColumns"></param>
		/// <param name="pkColumns"></param>
		/// <param name="joinType"></param>
		public override void AddJoin( string tableName, string alias, string[] fkColumns, string[] pkColumns, JoinType joinType )
		{
			if( joinType == JoinType.FullJoin ) throw new NotSupportedException( "The FULL JOIN is not supported by Jet database engine." );

			base.AddJoin( tableName, alias, fkColumns, pkColumns, joinType );
		}
	}
}