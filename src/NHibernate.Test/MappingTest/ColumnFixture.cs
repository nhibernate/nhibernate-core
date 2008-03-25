using System;
using NHibernate.Dialect;
using NHibernate.Mapping;
using NUnit.Framework;

namespace NHibernate.Test.MappingTest
{
	[TestFixture]
	public class ColumnFixture
	{
		private Dialect.Dialect _dialect;

		[SetUp]
		public void SetUp()
		{
			_dialect = new MsSql2000Dialect();
		}

		[Test]
		public void YesNoSqlType()
		{
			SimpleValue sv = new SimpleValue();
			sv.TypeName = NHibernateUtil.YesNo.Name;
			Column column = new Column();
			column.Value = sv;
			string type = column.GetSqlType(_dialect, null);
			Assert.AreEqual("CHAR(1)", type);
		}

		[Test]
		public void StringSqlType()
		{
			SimpleValue sv = new SimpleValue();
			sv.TypeName = NHibernateUtil.String.Name;
			Column column = new Column();
			column.Value = sv;
			Assert.AreEqual("NVARCHAR(255)", column.GetSqlType(_dialect, null));

			column.Length = 100;
			Assert.AreEqual("NVARCHAR(100)", column.GetSqlType(_dialect, null));
		}
	}
}