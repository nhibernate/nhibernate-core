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
			var result1 = _appender.AppendLockHint(new SqlString("select * from Person person"));
			Assert.That(result1.ToString(), Is.EqualTo("select * from Person person with (updlock, rowlock)"));

			var result2 = _appender.AppendLockHint(new SqlString("select * from Person as person"));
			Assert.That(result2.ToString(), Is.EqualTo("select * from Person as person with (updlock, rowlock)"));
		}

		[Test]
		public void AppendHintToJoinedTableAlias()
		{
			var result = _appender.AppendLockHint(new SqlString("select * from Person person inner join Country country on person.Id = country.Id"));
			Assert.That(result.ToString(), Is.EqualTo("select * from Person person with (updlock, rowlock) inner join Country country on person.Id = country.Id"));
		}

		[Test]
		public void AppendHintToUnionTableAlias()
		{
			var result = _appender.AppendLockHint(new SqlString("select Id, Name from (select Id, CONCAT(FirstName, LastName) from Employee union all select Id, CONCAT(FirstName, LastName) from Manager) as person"));
			Assert.That(result.ToString(), Is.EqualTo("select Id, Name from (select Id, CONCAT(FirstName, LastName) from Employee with (updlock, rowlock) union all select Id, CONCAT(FirstName, LastName) from Manager with (updlock, rowlock)) as person"));
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
		public void ShouldHandleEscapingInSubselect()
		{
			const string expectedQuery =
				"select Id, Name from (select Id, Name from [Employee] with (updlock, rowlock) union all select Id, Name from [Manager] with (updlock, rowlock)) as person";

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
