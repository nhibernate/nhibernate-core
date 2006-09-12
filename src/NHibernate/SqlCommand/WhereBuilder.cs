using System;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// Allows us to construct SQL WHERE fragments
	/// </summary>
	public class WhereBuilder : SqlBaseBuilder
	{
		public WhereBuilder( ISessionFactoryImplementor factory ) : base( factory )
		{
		}

		public SqlString WhereClause( string alias, string[ ] columnNames, IType whereType )
		{
			return ToWhereString( alias, columnNames );
		}
	}
}
