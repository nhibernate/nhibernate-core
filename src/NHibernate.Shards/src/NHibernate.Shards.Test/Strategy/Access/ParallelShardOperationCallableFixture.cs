using System.Collections.Generic;
using Iesi.Collections.Generic;
using NHibernate.Shards.Strategy.Access;
using NHibernate.Shards.Strategy.Exit;
using NHibernate.Shards.Test.Mock;
using NHibernate.Shards.Threading;
using NUnit.Framework;
using Rhino.Mocks;

namespace NHibernate.Shards.Test.Strategy.Access
{
	[TestFixture]
	public class ParallelShardOperationCallableFixture : TestFixtureBaseWithMock
	{
		public class StartAwareFutureTask_CancelReturnTrue : StartAwareFutureTask<object>
		{
			public StartAwareFutureTask_CancelReturnTrue(ICallable<object> callable, int id) : base(callable, id)
			{
			}

			public override bool Cancel(bool mayInterruptIfRunning)
			{
				return true;
			}
		}

		public class StartAwareFutureTask_CancelReturnFalse : StartAwareFutureTask<object>
		{
			public StartAwareFutureTask_CancelReturnFalse(ICallable<object> callable, int id) : base(callable, id)
			{
			}

			public override bool Cancel(bool mayInterruptIfRunning)
			{
				return false;
			}
		}

		[Test]
		public void CountDown()
		{
			MockRepository mr = Mocks;

			int latchSize = 5;
			CountDownLatch startLatch = new CountDownLatch(0);
			CountDownLatch latch = new CountDownLatch(latchSize);

			IShard shard = mr.DynamicMock<IShard>();

			IExitStrategy<object> strat = mr.CreateMock<IExitStrategy<object>>();

			ICallable<object> anotherCallable = mr.DynamicMock<ICallable<object>>();

			IShardOperation<object> operation = mr.DynamicMock<IShardOperation<object>>();
			
			//Expectations and results
			Expect.Call(strat.AddResult(null, shard)).Return(false);
			Expect.Call(strat.AddResult(null, shard)).Return(false);
			Expect.Call(strat.AddResult(null, shard)).Return(true);
			Expect.Call(strat.AddResult(null, shard)).Return(true);
			SetupResult.For(shard.ShardIds).Return(new HashedSet<ShardId>(new ShardId[] {(new ShardId(0))}));

			mr.ReplayAll();

			IList<StartAwareFutureTask<object>> futureTasks = new List<StartAwareFutureTask<object>>();

			ParallelShardOperationCallable<object> callable = new ParallelShardOperationCallable<object>(
				startLatch,
				latch,
				strat,
				operation,
				shard,
				futureTasks);

			callable.Call();
			// addResult returns false so latch is only decremented by 1
			Assert.AreEqual(latchSize - 1, latch.Count);
			// addResult returns false so latch is only decremented by 1
			callable.Call();
			Assert.AreEqual(latchSize - 2, latch.Count);


			StartAwareFutureTask<object> ft = new StartAwareFutureTask_CancelReturnFalse(anotherCallable, 0);

			futureTasks.Add(ft);
			callable.Call();
			// cancelling the 1 task returns false, so latch is only decremented by 1
			Assert.AreEqual(latchSize - 3, latch.Count);

			ft = new StartAwareFutureTask_CancelReturnTrue(anotherCallable, 0);

			futureTasks.Add(ft);
			callable.Call();
			// 1 decrement for myself and 1 for the task that returned true when cancelled
			Assert.AreEqual(latchSize - 5, latch.Count);
		}
	}
}