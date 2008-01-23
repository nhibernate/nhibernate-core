using System;
using System.Threading;
using NHibernate.Shards.Threading;
using NUnit.Framework;

namespace NHibernate.Shards.Test.Threading
{
	[TestFixture]
	public class AtomicIntegerFixture
	{
		private static void ThreadMethod()
		{
			for(int i = 0; i < 100000; i++)
			{
				new Counter();
			}
		}

		public void Run()
		{
			Thread thread1 = new Thread(ThreadMethod);
			Thread thread2 = new Thread(ThreadMethod);
			thread1.Start();
			thread2.Start();
			thread1.Join();
			thread2.Join();

			GC.Collect();
			GC.WaitForPendingFinalizers();

			Assert.AreEqual(0, Counter.Value);
		}

		public class Counter
		{
			private static AtomicInteger safeInteger = new AtomicInteger();

			public Counter()
			{
				safeInteger.IncrementAndGet();
			}

			public static int Value
			{
				get { return safeInteger.Value; }
			}

			~Counter()
			{
				safeInteger.DecrementAndGet();
			}
		}

		[Test]
		public void IntegrationTestCounter()
		{
			Run();
			Run();
			Run();
			Run();
		}

		/// <summary>
		/// Increment and Get
		/// </summary>
		[Test]
		public void test01()
		{
			AtomicInteger a = new AtomicInteger(1);
			Assert.AreEqual(2, a.IncrementAndGet());
			Assert.AreEqual(2, a.Value);
		}

		/// <summary>
		/// Decrement and Get
		/// </summary>
		[Test]
		public void test02()
		{
			AtomicInteger a = new AtomicInteger(1);
			Assert.AreEqual(0, a.DecrementAndGet());
			Assert.AreEqual(0, a.Value);
		}

		/// <summary>
		/// Get and Increment
		/// </summary>
		[Test]
		public void test03()
		{
			AtomicInteger a = new AtomicInteger(1);
			Assert.AreEqual(1, a.GetAndIncrement());
			Assert.AreEqual(2, a.Value);
		}

		/// <summary>
		/// Get and Decrement
		/// </summary>
		[Test]
		public void test04()
		{
			AtomicInteger a = new AtomicInteger(1);
			Assert.AreEqual(1, a.GetAndDecrement());
			Assert.AreEqual(0, a.Value);
		}
	}
}