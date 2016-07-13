using System;
using System.Collections;
using NHibernate.Criterion;
using NHibernate.Dialect;
using NHibernate.Driver;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// Summary description for StringTypeWithLengthFixture.
	/// </summary>
	[TestFixture]
	public class StringTypeWithLengthFixture : TypeFixtureBase
	{
		protected override string TypeName
		{
			get { return "String"; }
		}


		protected override IList Mappings
		{
			get
			{
				return new string[]
					{
						String.Format("TypesTest.{0}ClassWithLength.hbm.xml", TypeName)
					};
			}
		}


		[Test]
		public void NhThrowsOnTooLong()
		{
			if (!(Dialect is MsSql2008Dialect))
				Assert.Ignore(
					"This test only works (and is only relevant) " +
					"where the driver has set an explicit length " +
					"on the IDbDataParameter.");

			int maxStringLength = 4000;
			PropertyValueException ex = Assert.Throws<PropertyValueException>(
				() =>
				{
					using (ISession s = OpenSession())
					{
						StringClass b = new StringClass {LongStringValue = new string('x', maxStringLength + 1)};
						s.Save(b);
						s.Flush();
					}
				});

			Assert.That(ex.Message, Is.EqualTo("Error dehydrating property value for NHibernate.Test.TypesTest.StringClass.LongStringValue"));
			Assert.That(ex.InnerException, Is.TypeOf<HibernateException>());
			Assert.That(ex.InnerException.Message, Is.EqualTo("The length of the string value exceeds the length configured in the mapping/parameter."));
		}

		[Test]
		public void DbThrowsOnTooLong()
		{
			bool dbThrewError = false;

			try
			{
				using (ISession s = OpenSession())
				{
					StringClass b = new StringClass {StringValue = "0123456789a"};
					s.Save(b);
					s.Flush();
				}
			}
			catch
			{
				dbThrewError = true;
			}

			Assert.That(dbThrewError, "Database did not throw an error when trying to put too large a value into a column");
		}

		[Test]
		public void CriteriaLikeParameterCanExceedColumnSize()
		{
			if (sessions.ConnectionProvider.Driver is OdbcDriver)
				Assert.Ignore("This test fails against the ODBC driver.  The driver would need to be override to allow longer parameter sizes than the column.");

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new StringClass() { Id = 1, StringValue = "AAAAAAAAAB" });
				s.Save(new StringClass() { Id = 2, StringValue = "BAAAAAAAAA" });

				var aaItems =
					s.CreateCriteria<StringClass>()
						.Add(Restrictions.Like("StringValue", "%AAAAAAAAA%"))
						.List();

				Assert.That(aaItems.Count, Is.EqualTo(2));
			}
		}

		[Test]
		public void HqlLikeParameterCanExceedColumnSize()
		{
			if (sessions.ConnectionProvider.Driver is OdbcDriver)
				Assert.Ignore("This test fails against the ODBC driver.  The driver would need to be override to allow longer parameter sizes than the column.");

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new StringClass() { Id = 1, StringValue = "AAAAAAAAAB" });
				s.Save(new StringClass() { Id = 2, StringValue = "BAAAAAAAAA" });

				var aaItems =
					s.CreateQuery("from StringClass s where s.StringValue like :likeValue")
						.SetParameter("likeValue", "%AAAAAAAAA%")
						.List();

				Assert.That(aaItems.Count, Is.EqualTo(2));
			}
		}


		[Test]
		public void CriteriaEqualityParameterCanExceedColumnSize()
		{
			if (sessions.ConnectionProvider.Driver is OdbcDriver)
				Assert.Ignore("This test fails against the ODBC driver.  The driver would need to be override to allow longer parameter sizes than the column.");

			// We should be able to query a column with a value longer than
			// the specified column size, to avoid tedious exceptions.

			using (ISession s = OpenSession())
			using (s.BeginTransaction())
			{
				s.Save(new StringClass { Id = 1, StringValue = "AAAAAAAAAB" });
				s.Save(new StringClass { Id = 2, StringValue = "BAAAAAAAAA" });

				var aaItems =
					s.CreateCriteria<StringClass>()
					 .Add(Restrictions.Eq("StringValue", "AAAAAAAAABx"))
					 .List();

				Assert.That(aaItems.Count, Is.EqualTo(0));
			}
		}

		[Test]
		public void HqlEqualityParameterCanExceedColumnSize()
		{
			if (sessions.ConnectionProvider.Driver is OdbcDriver)
				Assert.Ignore("This test fails against the ODBC driver.  The driver would need to be override to allow longer parameter sizes than the column.");

			// We should be able to query a column with a value longer than
			// the specified column size, to avoid tedious exceptions.

			using (ISession s = OpenSession())
			using (s.BeginTransaction())
			{
				s.Save(new StringClass { Id = 1, StringValue = "AAAAAAAAAB" });
				s.Save(new StringClass { Id = 2, StringValue = "BAAAAAAAAA" });

				var aaItems =
					s.CreateQuery("from StringClass s where s.StringValue = :likeValue")
					 .SetParameter("likeValue", "AAAAAAAAABx")
					 .List();

				Assert.That(aaItems.Count, Is.EqualTo(0));
			}
		}
	}
}
