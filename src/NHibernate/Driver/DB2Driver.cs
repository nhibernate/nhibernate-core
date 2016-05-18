using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using NHibernate.Dialect;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the IBM.Data.DB2 DataProvider.
	/// </summary>
	public class DB2Driver : ReflectionBasedDriver
	{
        private readonly DB2Dialect _db2Dialect = new DB2Dialect();

		/// <summary>
		/// Initializes a new instance of the <see cref="DB2Driver"/> class.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the <c>IBM.Data.DB2</c> assembly can not be loaded.
		/// </exception>
		public DB2Driver() : base(
			"IBM.Data.DB2",
			"IBM.Data.DB2",
			"IBM.Data.DB2.DB2Connection",
			"IBM.Data.DB2.DB2Command")
		{
		}

		public override bool UseNamedPrefixInSql
		{
			get { return false; }
		}

		public override bool UseNamedPrefixInParameter
		{
			get { return false; }
		}

		public override string NamedPrefix
		{
			get { return String.Empty; }
		}

		public override bool SupportsMultipleOpenReaders
		{
			get { return false; }
		}

        protected override void InitializeParameter(IDbDataParameter dbParam, string name, SqlType sqlType)
		{
			var convertedSqlType = sqlType;

            if (convertedSqlType.DbType == DbType.Boolean)
            {
                convertedSqlType = new SqlType(DbType.Int16);   
            }

			base.InitializeParameter(dbParam, name, convertedSqlType);
		}
	}
}