using System.Collections.Generic;
using NHibernate.Dialect;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.DialectTest
{
	[TestFixture]
	public class LockHintAppenderFixture
	{
		private const string MsSql2000LockHint = " with (updlock, rowlock)";
		private MsSql2000Dialect.LockHintAppender _appender;

		[SetUp]
		public void SetUp()
		{
			_appender = new MsSql2000Dialect.LockHintAppender(new MsSql2000Dialect(), new Dictionary<string, LockMode> { {"person", LockMode.Upgrade} });
		}

		[Test]
		public void AppendHintToSingleTableAlias()
		{
			const string expectedQuery1 = "select * from Person person with (updlock, rowlock)";
			const string expectedQuery2 = "select * from Person as person with (updlock, rowlock)";

			var result1 = _appender.AppendLockHint(new SqlString(expectedQuery1.Replace(MsSql2000LockHint, string.Empty)));
			Assert.That(result1.ToString(), Is.EqualTo(expectedQuery1));

			var result2 = _appender.AppendLockHint(new SqlString(expectedQuery2.Replace(MsSql2000LockHint, string.Empty)));
			Assert.That(result2.ToString(), Is.EqualTo(expectedQuery2));
		}

		[Test]
		public void AppendHintToJoinedTableAlias()
		{
			const string expectedQuery =
				"select * from Person person with (updlock, rowlock) inner join Country country on person.Id = country.Id";

			var result = _appender.AppendLockHint(new SqlString(expectedQuery.Replace(MsSql2000LockHint, string.Empty)));
			Assert.That(result.ToString(), Is.EqualTo(expectedQuery));
		}

		[Test]
		public void AppendHintToUnionTableAlias()
		{
			const string expectedQuery =
				"select Id, Name from (select Id, CONCAT(FirstName, LastName) from Employee with (updlock, rowlock) union all select Id, CONCAT(FirstName, LastName) from Manager with (updlock, rowlock)) as person";

			var result = _appender.AppendLockHint(new SqlString(expectedQuery.Replace(MsSql2000LockHint, string.Empty)));
			Assert.That(result.ToString(), Is.EqualTo(expectedQuery));
		}

		[Test]
		public void ShouldIgnoreCasing()
		{
			const string expectedQuery =
				"select Id, Name FROM (select Id, Name FROM Employee with (updlock, rowlock) union all select Id, Name from Manager with (updlock, rowlock)) as person";

			var result = _appender.AppendLockHint(new SqlString(expectedQuery.Replace(MsSql2000LockHint, string.Empty)));
			Assert.That(result.ToString(), Is.EqualTo(expectedQuery));
		}

		[Test]
		public void ShouldHandleExplicitSchemas()
		{
			const string expectedQuery =
				"select Id, Name from (select Id, Name FROM dbo.Employee with (updlock, rowlock) union all select Id, Name from Manager with (updlock, rowlock)) as person";

			var result = _appender.AppendLockHint(new SqlString(expectedQuery.Replace(MsSql2000LockHint, string.Empty)));
			Assert.That(result.ToString(), Is.EqualTo(expectedQuery));
		}

		[Test]
		public void ShouldHandleExplicitSchemasAndDbNames()
		{
			const string expectedQuery =
				"select Id, Name from (select Id, Name FROM nhibernate.dbo.Employee with (updlock, rowlock) union all select Id, Name from Manager with (updlock, rowlock)) as person";

			var result = _appender.AppendLockHint(new SqlString(expectedQuery.Replace(MsSql2000LockHint, string.Empty)));
			Assert.That(result.ToString(), Is.EqualTo(expectedQuery));
		}

		[Test]
		public void ShouldHandleExplicitDbNameWithoutSchemaName()
		{
			const string expectedQuery =
				"select Id, Name from (select Id, Name FROM nhibernate..Employee with (updlock, rowlock) union all select Id, Name from Manager with (updlock, rowlock)) as person";

			var result = _appender.AppendLockHint(new SqlString(expectedQuery.Replace(MsSql2000LockHint, string.Empty)));
			Assert.That(result.ToString(), Is.EqualTo(expectedQuery));
		}

		[Test]
		public void ShouldHandleExplicitSchemasAndDbNamesWithSpacesBetweenNameParts()
		{
			const string expectedQuery =
				"select Id, Name from (select Id, Name FROM nhibernate .dbo. Employee with (updlock, rowlock) union all select Id, Name from Manager with (updlock, rowlock)) as person";

			var result = _appender.AppendLockHint(new SqlString(expectedQuery.Replace(MsSql2000LockHint, string.Empty)));
			Assert.That(result.ToString(), Is.EqualTo(expectedQuery));
		}

		[Test]
		public void ShouldHandleEscapingInSubselect()
		{
			const string expectedQuery =
				"select Id, Name from (select Id, Name from [Employee] with (updlock, rowlock) union all select Id, Name from [Manager] with (updlock, rowlock)) as person";

			var result = _appender.AppendLockHint(new SqlString(expectedQuery.Replace(MsSql2000LockHint, string.Empty)));
			Assert.That(result.ToString(), Is.EqualTo(expectedQuery));
		}

		[Test]
		public void ShouldHandleEscapingWithWhitespacesInSubselect()
		{
			const string expectedQuery =
				"select Id, Name from (select Id, Name from [Empl oyee] with (updlock, rowlock) union all select Id, Name from [Man ager] with (updlock, rowlock)) as person";

			var result = _appender.AppendLockHint(new SqlString(expectedQuery.Replace(MsSql2000LockHint, string.Empty)));
			Assert.That(result.ToString(), Is.EqualTo(expectedQuery));
		}

		[Test]
		public void ShouldHandleEscapingWithSquareBracketsInSubselect()
		{
			const string expectedQuery =
				"select Id, Name from (select Id, Name from [Empl ]]oyee] with (updlock, rowlock) union all select Id, Name from [Manager] with (updlock, rowlock)) as person";

			var result = _appender.AppendLockHint(new SqlString(expectedQuery.Replace(MsSql2000LockHint, string.Empty)));
			Assert.That(result.ToString(), Is.EqualTo(expectedQuery));
		}

		[Test]
		public void ShouldHandleMultilineQuery()
		{
			const string expectedQuery = @"
select Id, Name from
	(select Id, Name from Employee with (updlock, rowlock) union all
	select Id, Name from Manager with (updlock, rowlock))
as person";

			var result = _appender.AppendLockHint(new SqlString(expectedQuery.Replace(MsSql2000LockHint, string.Empty)));
			Assert.That(result.ToString(), Is.EqualTo(expectedQuery));
		}
	}
}
