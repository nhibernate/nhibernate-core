using System.Threading;
using NHibernate.Shards.Threading;
using NUnit.Framework;

namespace NHibernate.Shards.Test.Threading
{
	[TestFixture]
	public class FutureFixture
	{
		/// <summary>
		/// IsDone, IsCancelled and Get
		/// </summary>
		[Test]
		public void IsDoneIsCanceledAndGet()
		{
			FutureTask<int> ft = new FutureTask<int>(new InmediatlyCallable(13));

			Assert.AreEqual(false, ft.IsDone);
			Assert.AreEqual(false, ft.IsCancelled);

			ft.Run();

			Assert.AreEqual(13, ft.Get()); //The result

			Assert.AreEqual(true, ft.IsDone);
			Assert.AreEqual(false, ft.IsCancelled);
		}

		/// <summary>
		/// Cancel Before Run
		/// </summary>
		[Test]
		public void CancelAndInterruptBeforeRun()
		{
			FutureTask<string> ft = new FutureTask<string>(new FooCallable1());
			ft.Cancel(true);
			ft.Run();
			Assert.IsTrue(ft.IsDone);
			Assert.IsTrue(ft.IsCancelled);
		}

		/// <summary>
		/// Cancel but not interrupt before run
		/// </summary>
		[Test]
		public void CancelButNotInterruptBeforeRun()
		{
			FutureTask<string> ft = new FutureTask<string>(new FooCallable1());
			Assert.IsTrue(ft.Cancel(false));
			ft.Run();
			Assert.IsTrue(ft.IsDone);
			Assert.IsTrue(ft.IsCancelled);
		}

		/// <summary>
		/// Cancel and interrupt the running task
		/// </summary>
		[Test]
		public void CancelAndInterruptTheRunningTask()
		{
			FutureTask<int> ft = new FutureTask<int>(new InmediatlyCallable(13));
			ft.Run();
			Assert.IsFalse(ft.Cancel(true));
			Assert.IsTrue(ft.IsDone);
			Assert.IsFalse(ft.IsCancelled);
		}

		/// <summary>
		/// Cancel but not interrupt because the Task is running
		/// </summary>
		[Test]
		public void CancelButNotInterruptTheRunningTask()
		{	//The Task takes 5 seconds.
			FutureTask<string> ft = new FutureTask<string>(new FooCallable1()); 
			Thread t = new Thread(ft.Run);
			t.Start();
			Thread.Sleep(100);
			Assert.IsTrue(ft.Cancel(false)); //Not interrupt because the task running
			t.Join();
			Assert.IsTrue(ft.IsDone);
			Assert.IsTrue(ft.IsCancelled);
		}
	}
}