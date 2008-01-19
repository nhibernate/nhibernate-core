using System;
using System.Threading;

namespace NHibernate.Shards.Threading
{
	/// <summary>
	/// A synchronization aid that allows one or more threads to wait until 
	/// a set of operations being performed in other threads completes.
	/// </summary>
	public class CountDownLatch
	{
		private volatile int count;

		/// <summary>
		/// Constructs a <c>CountDownLatch</c> initialized with the given count.
		/// </summary>
		/// <param name="count">the number of times CountDown() must be invoked before threads can pass through Await()</param>
		public CountDownLatch(int count)
		{
			if (count < 0) throw new ArgumentException("Must be greater than zero", "count");

			this.count = count;
		}

		/// <summary>
		/// Returns the current count.
		/// </summary>
		public int Count
		{
			get { return count; }
		}

		/// <summary>
		/// Causes the current thread to wait until the latch has counted down to zero, 
		/// unless the thread is interrupted.
		/// </summary>
		public void Await()
		{
			lock(this)
			{
				while(count > 0)
				{
					Monitor.Wait(this);
				}
			}
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
			lock(this)
			{
				if(count != 0)
				{
					count--;
	
					if(count == 0)
						Monitor.PulseAll(this);
				}
			}
		}

		/// <summary>
		/// Returns a string identifying this latch, as well as its state.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Concat("Count = ", count);
		}
	}
}