using System;

using NHibernate.SqlCommand;

using NUnit.Framework;

namespace NHibernate.Test.SqlCommandTest
{
	/// <summary>
	/// Summary description for SqlStringBuilderFixture.
	/// </summary>
	[TestFixture]
	public class SqlStringBuilderFixture
	{
		[Test]
		public void InsertAndAdd() 
		{
			SqlStringBuilder builder = new SqlStringBuilder();

			builder.Add("col1, col2 ");
			builder.Insert( 0, "select ");
			builder.Add("from table ");

			Assert.AreEqual( "select col1, col2 from table ", builder.ToSqlString().ToString() );

		}
	}
}
