using System;

namespace NHibernate.Shards.Threading
{
	/// <summary>
	/// A synchronization aid that allows one or more threads to wait until 
	/// a set of operations being performed in other threads completes.
	/// </summary>
	public class CountDownLatch
	{
		private int count;

		/// <summary>
		/// Constructs a <c>CountDownLatch</c> initialized with the given count.
		/// </summary>
		/// <param name="count">the number of times CountDown() must be invoked before threads can pass through Await()</param>
		public CountDownLatch(int count)
		{
			this.count = count;
		}

		/// <summary>
		/// Returns the current count.
		/// </summary>
		public int Count
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Causes the current thread to wait until the latch has counted down to zero, 
		/// unless the thread is interrupted.
		/// </summary>
		public void Await()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Causes the current thread to wait until the latch has counted down to zero, 
		/// unless the thread is interrupted, or the specified waiting time elapses.
		/// </summary>
		/// <param name="timeout"></param>
		/// <returns></returns>
		public bool AWait(TimeSpan timeout)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Decrements the count of the latch, releasing all waiting threads if the count 
		/// reaches zero.
		/// </summary>
		public void CountDown()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns a string identifying this latch, as well as its state.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			throw new NotImplementedException();
		}
	}
}