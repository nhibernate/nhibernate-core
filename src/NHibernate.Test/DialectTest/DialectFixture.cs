using System;
using System.Collections.Generic;
using System.Data;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.DialectTest
{
	/// <summary>
	/// Summary description for DialectFixture.
	/// </summary>
	[TestFixture]
	public class DialectFixture
	{
		protected Dialect.Dialect d = null;

		private const int BeforeQuoteIndex = 0;
		private const int AfterQuoteIndex = 1;
		private const int AfterUnquoteIndex = 2;

		protected string[] tableWithNothingToBeQuoted;

		// simulating a string already enclosed in the Dialects quotes of Quote"d[Na$` 
		// being passed in that should be returned as Quote""d[Na$` - notice the "" before d
		protected string[] tableAlreadyQuoted;

		// simulating a string that has NOT been enclosed in the Dialects quotes and needs to 
		// be.
		protected string[] tableThatNeedsToBeQuoted;


		[SetUp]
		public virtual void SetUp()
		{
			// Generic Dialect inherits all of the Quoting functions from
			// Dialect (which is abstract)
			d = new GenericDialect();
			tableWithNothingToBeQuoted = new string[] { "plainname", "\"plainname\"" };
			tableAlreadyQuoted = new string[] { "\"Quote\"\"d[Na$`\"", "\"Quote\"\"d[Na$`\"", "Quote\"d[Na$`" };
			tableThatNeedsToBeQuoted = new string[] { "Quote\"d[Na$`", "\"Quote\"\"d[Na$`\"", "Quote\"d[Na$`" };
		}

		[Test]
		public void IsQuotedTrue()
		{
			Assert.IsTrue(d.IsQuoted(tableAlreadyQuoted[BeforeQuoteIndex]));
		}

		/// <summary>
		/// Test that only the first char identifies that the Identifier
		/// is Quoted - regardless of what chars are contained in it.
		/// </summary>
		[Test]
		public void IsQuotedFalse()
		{
			Assert.IsFalse(d.IsQuoted(tableThatNeedsToBeQuoted[BeforeQuoteIndex]));
		}

		[Test]
		public void WhenNullOrEmptyIsQuotedFalse()
		{
			Assert.That(d.IsQuoted(null), Is.False);
			Assert.That(d.IsQuoted(""), Is.False);
		}

		[Test]
		public void QuoteTableNameNeeded()
		{
			Assert.AreEqual(
				tableThatNeedsToBeQuoted[AfterQuoteIndex],
				d.QuoteForTableName(tableThatNeedsToBeQuoted[BeforeQuoteIndex]));
		}

		[Test]
		public void QuoteTableNameNotNeeded()
		{
			Assert.AreEqual(
				tableWithNothingToBeQuoted[AfterQuoteIndex],
				d.QuoteForTableName(tableWithNothingToBeQuoted[BeforeQuoteIndex]));
		}

		[Test]
		public void QuoteTableNameAlreadyQuoted()
		{
			Assert.AreEqual(
				tableAlreadyQuoted[BeforeQuoteIndex],
				d.QuoteForTableName(tableAlreadyQuoted[BeforeQuoteIndex]));
		}


		/// <summary>
		/// Test that it does not matter if the name passed in has been quoted or not
		/// already.  The UnQuote should take care of it and return the same result.
		/// </summary>
		[Test]
		public void UnQuoteAlreadyQuoted()
		{
			Assert.AreEqual(
				tableAlreadyQuoted[AfterUnquoteIndex],
				d.UnQuote(tableAlreadyQuoted[BeforeQuoteIndex]));

			Assert.AreEqual(
				tableAlreadyQuoted[AfterUnquoteIndex],
				d.UnQuote(tableAlreadyQuoted[AfterQuoteIndex]));
		}

		[Test]
		public void UnQuoteNeedingQuote()
		{
			Assert.AreEqual(
				tableThatNeedsToBeQuoted[AfterUnquoteIndex],
				d.UnQuote(tableThatNeedsToBeQuoted[BeforeQuoteIndex]));

			Assert.AreEqual(
				tableThatNeedsToBeQuoted[AfterUnquoteIndex],
				d.UnQuote(tableThatNeedsToBeQuoted[AfterQuoteIndex]));
		}

		[Test]
		public void UnQuoteArray()
		{
			string[] actualUnquoted = new string[2];
			string[] expectedUnquoted =
				new string[] { tableThatNeedsToBeQuoted[AfterUnquoteIndex], tableAlreadyQuoted[AfterUnquoteIndex] };

			actualUnquoted =
				d.UnQuote(new string[] { tableThatNeedsToBeQuoted[BeforeQuoteIndex], tableAlreadyQuoted[BeforeQuoteIndex] });

			ObjectAssert.AreEqual(expectedUnquoted, actualUnquoted, true);
		}

		[Test]
		public void GetDialectUntrimmedName()
		{
			Dictionary<string, string> props = new Dictionary<string, string>();
			props[Environment.Dialect] = "\r\n\t "
										 + typeof(MsSql2000Dialect).AssemblyQualifiedName
										 + " \t\r\n  ";

			Dialect.Dialect dialect = Dialect.Dialect.GetDialect(props);
			Assert.IsTrue(dialect is MsSql2000Dialect);
		}

		[Test]
		public void CurrentTimestampSelection()
		{
			var conf = TestConfigurationHelper.GetDefaultConfiguration();
			Dialect.Dialect dialect = Dialect.Dialect.GetDialect(conf.Properties);
			if (!dialect.SupportsCurrentTimestampSelection)
			{
				Assert.Ignore("This test does not apply to " + dialect.GetType().FullName);
			}
			var sessions = (ISessionFactoryImplementor)conf.BuildSessionFactory();
			sessions.ConnectionProvider.Configure(conf.Properties);
			IDriver driver = sessions.ConnectionProvider.Driver;

			using (IDbConnection connection = sessions.ConnectionProvider.GetConnection())
			{
				IDbCommand statement = driver.GenerateCommand(CommandType.Text, new SqlString(dialect.CurrentTimestampSelectString),
															  new SqlType[0]);
				statement.Connection = connection;
				using (IDataReader reader = statement.ExecuteReader())
				{
					Assert.That(reader.Read(), "should return one record");
					Assert.That(reader[0], Is.InstanceOf<DateTime>());
				}
			}
		}

	}
}