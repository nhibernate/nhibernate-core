namespace NHibernate.Test.DialectTest
{
	using Dialect;
	using Mapping;
	using NUnit.Framework;

	[TestFixture]
	public class SqlCEDialectFixture
	{
		[Test]
		public void BinaryBlob_mapping_to_SqlCe_types()
		{
			Dialect dialect = new MsSqlCeDialect();
			SimpleValue sv = new SimpleValue();
			sv.TypeName = NHibernateUtil.BinaryBlob.Name;
			Column column = new Column();
			column.Value = sv;

			// no length, should produce maximum
			Assert.AreEqual("VARBINARY(8000)", column.GetSqlType(dialect, null));

			// maximum varbinary length is 8000
			column.Length = 8000;
			Assert.AreEqual("VARBINARY(8000)", column.GetSqlType(dialect,null));

			column.Length = 8001;
			Assert.AreEqual("IMAGE", column.GetSqlType(dialect, null));
		}
	}
}