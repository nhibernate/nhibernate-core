using NUnit.Framework;
using NHibernate.Id.Enhanced;

namespace NHibernate.Test.IdGen.Enhanced
{
	[TestFixture]
	public class OptimizerTests
	{
		[Test]
		public void TestBasicNoOptimizerUsage()
		{
			// test historic sequence behavior, where the initial values start at 1...
			var sequence = new SourceMock(1);
			var optimizer = OptimizerFactory.BuildOptimizer(OptimizerFactory.None, typeof(long), 1, -1);
			for (int i = 1; i < 11; i++)
			{
				long next = (long)optimizer.Generate(sequence);
				Assert.That(next, Is.EqualTo(i));
			}
			Assert.That(sequence.TimesCalled, Is.EqualTo(10));
			Assert.That(sequence.CurrentValue, Is.EqualTo(10));

			// test historic table behavior, where the initial values started at 0 (we now force 1 to be the first used id value)
			sequence = new SourceMock(0);
			optimizer = OptimizerFactory.BuildOptimizer(OptimizerFactory.None, typeof(long), 1, -1);
			for (int i = 1; i < 11; i++)
			{
				long next = (long)optimizer.Generate(sequence);
				Assert.That(next, Is.EqualTo(i));
			}
			Assert.That(sequence.TimesCalled, Is.EqualTo(11));  // an extra time to get to 1 initially
			Assert.That(sequence.CurrentValue, Is.EqualTo(10));
		}

		[Test]
		public void TestBasicHiLoOptimizerUsage()
		{
			const int increment = 10;
			long next;

			// test historic sequence behavior, where the initial values start at 1...
			var sequence = new SourceMock(1);
			var optimizer = OptimizerFactory.BuildOptimizer(OptimizerFactory.HiLo, typeof(long), increment, -1);
			for (int i = 1; i <= increment; i++)
			{
				next = (long)optimizer.Generate(sequence);
				Assert.That(next, Is.EqualTo(i));
			}
			Assert.That(sequence.TimesCalled, Is.EqualTo(1));   // once to initialize state
			Assert.That(sequence.CurrentValue, Is.EqualTo(1));

			// force a "clock over"
			next = (long)optimizer.Generate(sequence);
			Assert.That(next, Is.EqualTo(11));
			Assert.That(sequence.TimesCalled, Is.EqualTo(2));
			Assert.That(sequence.CurrentValue, Is.EqualTo(2));


			// test historic table behavior, where the initial values started at 0 (we now force 1 to be the first used id value)
			sequence = new SourceMock(0);
			optimizer = OptimizerFactory.BuildOptimizer(OptimizerFactory.HiLo, typeof(long), increment, -1);
			for (int i = 1; i <= increment; i++)
			{
				next = (long)optimizer.Generate(sequence);
				Assert.That(next, Is.EqualTo(i));
			}
			Assert.That(sequence.TimesCalled, Is.EqualTo(2)); // here have have an extra call to get to 1 initially
			Assert.That(sequence.CurrentValue, Is.EqualTo(1));

			// force a "clock over"
			next = (long)optimizer.Generate(sequence);
			Assert.That(next, Is.EqualTo(11));
			Assert.That(sequence.TimesCalled, Is.EqualTo(3));
			Assert.That(sequence.CurrentValue, Is.EqualTo(2));
		}

		[Test]
		public void TestBasicPooledOptimizerUsage()
		{
			long next;
			// test historic sequence behavior, where the initial values start at 1...
			var sequence = new SourceMock(1, 10);
			var optimizer = OptimizerFactory.BuildOptimizer(OptimizerFactory.Pool, typeof(long), 10, -1);
			for (int i = 1; i < 11; i++)
			{
				next = (long)optimizer.Generate(sequence);
				Assert.That(next, Is.EqualTo(i));
			}
			Assert.That(sequence.TimesCalled, Is.EqualTo(2)); // twice to initialize state
			Assert.That(sequence.CurrentValue, Is.EqualTo(11));

			// force a "clock over"
			next = (long)optimizer.Generate(sequence);
			Assert.That(next, Is.EqualTo(11));
			Assert.That(sequence.TimesCalled, Is.EqualTo(3));
			Assert.That(sequence.CurrentValue, Is.EqualTo(21));
		}

		[Test]
		public void TestSubsequentPooledOptimizerUsage()
		{
			// test the pooled optimizer in situation where the sequence is already beyond its initial value on init.
			// cheat by telling the sequence to start with 1000
			var sequence = new SourceMock(1001, 3, 5);
			// but tell the optimizer the start-with is 1
			var optimizer = OptimizerFactory.BuildOptimizer(OptimizerFactory.Pool, typeof(long), 3, 1);

			Assert.That(sequence.TimesCalled, Is.EqualTo(5));
			Assert.That(sequence.CurrentValue, Is.EqualTo(1001));

			long next = (long)optimizer.Generate(sequence);
			Assert.That(next, Is.EqualTo(1001));
			Assert.That(sequence.TimesCalled, Is.EqualTo(5 + 1));
			Assert.That(sequence.CurrentValue, Is.EqualTo(1001 + 3));

			next = (long)optimizer.Generate(sequence);
			Assert.That(next, Is.EqualTo(1001 + 1));
			Assert.That(sequence.TimesCalled, Is.EqualTo(5 + 1));
			Assert.That(sequence.CurrentValue, Is.EqualTo(1001 + 3));

			next = (long)optimizer.Generate(sequence);
			Assert.That(next, Is.EqualTo(1001 + 2));
			Assert.That(sequence.TimesCalled, Is.EqualTo(5 + 1));
			Assert.That(sequence.CurrentValue, Is.EqualTo(1001 + 3));

			// force a "clock over"
			next = (long)optimizer.Generate(sequence);
			Assert.That(next, Is.EqualTo(1001 + 3));
			Assert.That(sequence.TimesCalled, Is.EqualTo(5 + 2));
			Assert.That(sequence.CurrentValue, Is.EqualTo(1001 + 6));
		}

		[Test]
		public void TestBasicPooledLoOptimizerUsage()
		{
			var sequence = new SourceMock(1, 3);
			var optimizer = OptimizerFactory.BuildOptimizer(OptimizerFactory.PoolLo, typeof(long), 3, -1);

			Assert.That(sequence.TimesCalled, Is.EqualTo(0));
			Assert.That(sequence.CurrentValue, Is.EqualTo(-1));

			var next = (long)optimizer.Generate(sequence);
			Assert.That(next, Is.EqualTo(1));
			Assert.That(sequence.TimesCalled, Is.EqualTo(1));
			Assert.That(sequence.CurrentValue, Is.EqualTo(1));

			next = (long)optimizer.Generate(sequence);
			Assert.That(next, Is.EqualTo(2));
			Assert.That(sequence.TimesCalled, Is.EqualTo(1));
			Assert.That(sequence.CurrentValue, Is.EqualTo(1));

			next = (long)optimizer.Generate(sequence);
			Assert.That(next, Is.EqualTo(3));
			Assert.That(sequence.TimesCalled, Is.EqualTo(1));
			Assert.That(sequence.CurrentValue, Is.EqualTo(1));

			// force a "clock over"
			next = (long)optimizer.Generate(sequence);
			Assert.That(next, Is.EqualTo(4));
			Assert.That(sequence.TimesCalled, Is.EqualTo(2));
			Assert.That(sequence.CurrentValue, Is.EqualTo(1 + 3));
		}

		[Test]
		public void TestSubsequentPooledLoOptimizerUsage()
		{
			// test the pooled optimizer in situation where the sequence is already beyond its initial value on init.
			// cheat by telling the sequence to start with 1000
			var sequence = new SourceMock(1001, 3, 5);
			// but tell the optimizer the start-with is 1
			var optimizer = OptimizerFactory.BuildOptimizer(OptimizerFactory.PoolLo, typeof(long), 3, 1);

			Assert.That(sequence.TimesCalled, Is.EqualTo(5));
			Assert.That(sequence.CurrentValue, Is.EqualTo(1001));

			var next = (long)optimizer.Generate(sequence);
			Assert.That(next, Is.EqualTo(1001 + 3));
			Assert.That(sequence.TimesCalled, Is.EqualTo(5 + 1));
			Assert.That(sequence.CurrentValue, Is.EqualTo(1001 + 3));

			next = (long)optimizer.Generate(sequence);
			Assert.That(next, Is.EqualTo(1001 + 3 + 1));
			Assert.That(sequence.TimesCalled, Is.EqualTo(5 + 1));
			Assert.That(sequence.CurrentValue, Is.EqualTo(1001 + 3));

			next = (long)optimizer.Generate(sequence);
			Assert.That(next, Is.EqualTo(1001 + 3 + 2));
			Assert.That(sequence.TimesCalled, Is.EqualTo(5 + 1));
			Assert.That(sequence.CurrentValue, Is.EqualTo(1001 + 3));

			// force a "clock over"
			next = (long)optimizer.Generate(sequence);
			Assert.That(next, Is.EqualTo(1001 + 3 + 3));
			Assert.That(sequence.TimesCalled, Is.EqualTo(5 + 2));
			Assert.That(sequence.CurrentValue, Is.EqualTo(1001 + 6));
		}

		[Test]
		public void TestRecoveredPooledOptimizerUsage()
		{
			var sequence = new SourceMock(1, 3);
			var optimizer = OptimizerFactory.BuildOptimizer(OptimizerFactory.Pool, typeof(long), 3, 1);

			Assert.That(sequence.TimesCalled, Is.EqualTo(0));
			Assert.That(sequence.CurrentValue, Is.EqualTo(-1));

			var next = (long)optimizer.Generate(sequence);
			Assert.That(next, Is.EqualTo(1));
			Assert.That(sequence.TimesCalled, Is.EqualTo(2));
			Assert.That(sequence.CurrentValue, Is.EqualTo(4));

			// app ends, and starts back up (we should "lose" only 2 and 3 as id values)
			var optimizer2 = OptimizerFactory.BuildOptimizer(OptimizerFactory.Pool, typeof(long), 3, 1);
			next = (long)optimizer2.Generate(sequence);
			Assert.That(next, Is.EqualTo(4));
			Assert.That(sequence.TimesCalled, Is.EqualTo(3));
			Assert.That(sequence.CurrentValue, Is.EqualTo(7));
		}

		[Test]
		public void TestRecoveredPooledLoOptimizerUsage()
		{
			var sequence = new SourceMock(1, 3);
			var optimizer = OptimizerFactory.BuildOptimizer(OptimizerFactory.PoolLo, typeof(long), 3, 1);

			Assert.That(sequence.TimesCalled, Is.EqualTo(0));
			Assert.That(sequence.CurrentValue, Is.EqualTo(-1));

			long next = (long)optimizer.Generate(sequence);
			Assert.That(next, Is.EqualTo(1));
			Assert.That(sequence.TimesCalled, Is.EqualTo(1));
			Assert.That(sequence.CurrentValue, Is.EqualTo(1));

			// app ends, and starts back up (we should "lose" only 2 and 3 as id values)
			var optimizer2 = OptimizerFactory.BuildOptimizer(OptimizerFactory.PoolLo, typeof(long), 3, 1);
			next = (long)optimizer2.Generate(sequence);
			Assert.That(next, Is.EqualTo(4));
			Assert.That(sequence.TimesCalled, Is.EqualTo(2));
			Assert.That(sequence.CurrentValue, Is.EqualTo(4));
		}
	}
}