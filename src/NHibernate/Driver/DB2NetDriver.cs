using System.Data.Common;
using NHibernate.SqlTypes;

namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the Net5.IBM.Data.Db2/Net.IBM.Data.Db2 DataProvider.
	/// </summary>
	public class DB2NetDriver : DB2DriverBase
	{
		private protected DB2NetDriver(string assemblyName) : base(assemblyName)
		{
		}

		public DB2NetDriver() : base("IBM.Data.Db2")
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
