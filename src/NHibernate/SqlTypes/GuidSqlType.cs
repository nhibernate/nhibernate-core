
using System;
using System.Data;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// Summary description for GuidSqlType.
	/// </summary>
	[Serializable]
	public class GuidSqlType : SqlType
	{
		public GuidSqlType() : base(DbType.Guid)
		{
		}
	}
}
