using System;
using System.Collections;

using NUnit.Framework;
using NHibernate.Mapping;

namespace NHibernate.Test.MappingTest
{
	[TestFixture]
	public class ColumnFixture
	{
		private Dialect.Dialect _dialect;

		[SetUp]
		public void SetUp()
		{
			_dialect = new Dialect.MsSql2000Dialect();
		}

		[Test]
		public void YesNoSqlType()
		{
			Column column = new Column(NHibernateUtil.YesNo, 0);
			string type = column.GetSqlType(_dialect, null);
			Assert.AreEqual("CHAR(1)", type);
		}

		[Test]
		public void StringSqlType()
		{
			Column column = new Column(NHibernateUtil.String, 0);
			Assert.AreEqual("NVARCHAR(255)", column.GetSqlType(_dialect, null));

			column.Length = 100;
			Assert.AreEqual("NVARCHAR(100)", column.GetSqlType(_dialect, null));

		}
	}
}
