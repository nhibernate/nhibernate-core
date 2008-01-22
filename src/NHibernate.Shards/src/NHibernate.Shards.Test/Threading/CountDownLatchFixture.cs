using System;
using System.Diagnostics;
using System.Threading;
using NHibernate.Shards.Threading;
using NUnit.Framework;

namespace NHibernate.Shards.Test.Threading
{
	[TestFixture]
	public class CountDownLatchFixture
	{
		public class Task
		{
			private CountDownLatch cdl;
			private long elapsed;

			public Task(CountDownLatch cdl)
			{
				this.cdl = cdl;
			}

			public long Elapsed
			{
				get { return elapsed; }
			}

			public void DoSomething()
			{
				Stopwatch sw = new Stopwatch();
				sw.Start();

				cdl.Await();

				sw.Stop();

				elapsed = sw.ElapsedMilliseconds;
			}
		}

		/// <summary>
		/// The counter could not be initialized in minor then zero.
		/// </summary>
		[Test, ExpectedException(typeof(ArgumentException))]
		public void MinorZeroException()
		{
			CountDownLatch cdl = new CountDownLatch(-1);
		}

		[Test]
		public void Result()
		{
			CountDownLatch cdl;

			cdl = new CountDownLatch(0);
			Assert.AreEqual(0, cdl.Count);

			cdl = new CountDownLatch(13);
			Assert.AreEqual(13, cdl.Count);

			cdl = new CountDownLatch(25);
			Assert.AreEqual(25, cdl.Count);
		}

		/// <summary>
		/// Simple count down
		/// </summary>
		[Test]
		public void SimpleCountDown()
		{
			CountDownLatch cdl = new CountDownLatch(3);

			cdl.CountDown();
			Assert.AreEqual(2, cdl.Count);

			cdl.CountDown();
			Assert.AreEqual(1, cdl.Count);

			cdl.CountDown();
			Assert.AreEqual(0, cdl.Count);

			cdl.CountDown();
			Assert.AreEqual(0, cdl.Count);
		}

		[Test]
		public void WaitingTask2()
		{
			CountDownLatch cdl = new CountDownLatch(2);
			Task task = new Task(cdl);
			Thread runner = new Thread(task.DoSomething);

			runner.Start();
			Thread.Sleep(600);
			cdl.CountDown();
			Thread.Sleep(600);
			cdl.CountDown();
			runner.Join();

			Assert.IsTrue(task.Elapsed >= 1100,"The task should cost more than 1100 millis");
			Assert.IsTrue(task.Elapsed <= 1300, "The task should cost less than 1300 millis"); 
			
		}

		[Test]
		public void WaitingTask1()
		{
			CountDownLatch cdl = new CountDownLatch(1);
			Task task = new Task(cdl);
			Thread runner = new Thread(task.DoSomething);

			runner.Start();
			Thread.Sleep(600);
			cdl.CountDown();
			runner.Join();

			Assert.IsTrue(task.Elapsed >= 500, "The task should cost more than 500 millis");
			Assert.IsTrue(task.Elapsed <= 700, "The task should cost less than 700 millis"); 
		}


		[Test]
		public void ZeroArgument()
		{
			CountDownLatch cdl = new CountDownLatch(0);
		}
	}
}