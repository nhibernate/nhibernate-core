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
		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		public WhereBuilder( ISessionFactoryImplementor factory ) : base( factory )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="columnNames"></param>
		/// <param name="whereType"></param>
		/// <returns></returns>
		public SqlString WhereClause( string alias, string[ ] columnNames, IType whereType )
		{
			Parameter[ ] parameters = Parameter.GenerateParameters( Factory, alias, columnNames, whereType );

			return ToWhereString( alias, columnNames, parameters );
		}
	}
}
