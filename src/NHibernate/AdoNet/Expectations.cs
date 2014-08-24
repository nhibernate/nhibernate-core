using System;
using System.Data;
using NHibernate.Engine;

namespace NHibernate.AdoNet
{
	public class Expectations
	{
		private const int UsualExpectedCount = 1;

		public class BasicExpectation : IExpectation
		{
			private readonly int expectedRowCount;

			public BasicExpectation(int expectedRowCount)
			{
				this.expectedRowCount = expectedRowCount;
				if (expectedRowCount < 0)
				{
					throw new ArgumentException("Expected row count must be greater than zero");
				}
			}

			public void VerifyOutcomeNonBatched(int rowCount, IDbCommand statement)
			{
				rowCount = DetermineRowCount(rowCount, statement);
				if (expectedRowCount > rowCount)
				{
					throw new StaleStateException(
						"Unexpected row count: " + rowCount + "; expected: " + expectedRowCount
						);
				}
				if (expectedRowCount < rowCount)
				{
					String msg = "Unexpected row count: " + rowCount + "; expected: " + expectedRowCount;
					throw new TooManyRowsAffectedException(msg, expectedRowCount, rowCount);
				}
			}

			public virtual bool CanBeBatched
			{
				get { return true; }
			}

			public virtual int ExpectedRowCount
			{
				get { return expectedRowCount; }
			}

			protected virtual int DetermineRowCount(int reportedRowCount, IDbCommand statement)
			{
				return reportedRowCount;
			}
		}

		public class NoneExpectation : IExpectation
		{
			public void VerifyOutcomeNonBatched(int rowCount, IDbCommand statement)
			{
				// explicitly perform no checking...
			}

			public bool CanBeBatched
			{
				// Change from H3! We cannot check this expectation when batching because we currently
				// can only check the total row count of a batch.
				get { return false; }
			}

			public int ExpectedRowCount
			{
				//get { throw new InvalidOperationException("Cannot get ExpectedRowCount of a non-batchable expectation"); }
				get { return 1; }
			}
		} ;

		// Various Expectation instances ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

		public static readonly IExpectation None = new NoneExpectation();
		public static readonly IExpectation Basic = new BasicExpectation(UsualExpectedCount);

		public static IExpectation AppropriateExpectation(ExecuteUpdateResultCheckStyle style)
		{
			if (style.Equals(ExecuteUpdateResultCheckStyle.None))
			{
				return None;
			}
			else if (style.Equals(ExecuteUpdateResultCheckStyle.Count))
			{
				return Basic;
			}
			else
			{
				throw new HibernateException("unknown check style : " + style);
			}
		}

		private Expectations()
		{
		}

		public static void VerifyOutcomeBatched(int expectedRowCount, int rowCount)
		{
			if (expectedRowCount > rowCount)
			{
				throw new StaleStateException(
					"Batch update returned unexpected row count from update; actual row count: " + rowCount +
					"; expected: " + expectedRowCount
					);
			}
			if (expectedRowCount < rowCount)
			{
				string msg = "Batch update returned unexpected row count from update; actual row count: " + rowCount +
				             "; expected: " + expectedRowCount;
				throw new TooManyRowsAffectedException(msg, expectedRowCount, rowCount);
			}
		}
	}
}