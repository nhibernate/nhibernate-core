using System.Data.Common;
using NHibernate.SqlTypes;

namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the IBM.Data.DB2.Core DataProvider.
	/// </summary>
	public class DB2CoreDriver : DB2DriverBase
	{		
		public DB2CoreDriver() : base("IBM.Data.DB2.Core")
		{
		}
		
		public override bool UseNamedPrefixInSql => true;

		public override bool UseNamedPrefixInParameter => true;

		public override string NamedPrefix => "@";

		protected override void InitializeParameter(DbParameter dbParam, string name, SqlType sqlType)
		{
			dbParam.ParameterName = FormatNameForParameter(name);
			base.InitializeParameter(dbParam, name, sqlType);
		}
	}
}
