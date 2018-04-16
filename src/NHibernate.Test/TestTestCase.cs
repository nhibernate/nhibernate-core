using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test
{
	/// <summary>
	/// Summary description for TestTestCase.
	/// </summary>
	[TestFixture]
	public class TestTestCase : TestCase
	{
		protected override IList Mappings
		{
			get { return Array.Empty<string>(); }
		}

		private bool _failOnTearDown;

		protected override void OnTearDown()
		{
			if (!_failOnTearDown)
				return;

			_failOnTearDown = false;
			throw new InvalidOperationException("Tear-down failure");
		}

		[Test(Description = "This test will fail with a \"Hard coded test failure.\" exception message.")]
		[Explicit("Testing test failure")]
		public void TearDownFailure()
		{
			throw new InvalidOperationException("Hard coded test failure.");
		}

		[Test(Description = "This test will fail with a \"Hard coded test failure.\" exception message, and additionally will cause a tear-down failure. The tear-down failure should not hide original failure.")]
		[Explicit("Testing test failure")]
		public void TearDownFailureShouldNotHideTestFailure()
		{
			_failOnTearDown = true;

			throw new InvalidOperationException("Hard coded test failure.");
		}

		private ISession _nonClosedSession;

		[Test(Description = "This test will fail with a \"Hard coded test failure.\" exception message, and additionally will cause a tear-down not clean failure. The tear-down failure should not hide original failure.")]
		[Explicit("Testing test failure")]
		public void TearDownNotCleanFailureShouldNotHideTestFailure()
		{
			_nonClosedSession = OpenSession();

			throw new InvalidOperationException("Hard coded test failure.");
		}

		[Test(Description = "This test tear-down should fail due to unclosed session. If test is successful, it has indeed failed.")]
		[Explicit("Testing test failure")]
		public void TearDownShouldFailDueToUncloseSession()
		{
			_nonClosedSession = OpenSession();
			using (var otherSession = OpenSession())
			{
				// Dummy code.
				otherSession.BeginTransaction().Rollback();
			}
		}

		[Test]
		public void TestExecuteStatement()
		{
			ExecuteStatement("create table yyyy (x int)");
			ExecuteStatement("drop table yyyy");
		}
	}
}