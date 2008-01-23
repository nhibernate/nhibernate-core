namespace NHibernate.Shards.Threading
{
	/// <summary>
	/// An <c>int</c> value that may be updated atomically
	/// </summary>
	public class AtomicInteger
	{
		private volatile int _value;

		/// <summary>
		/// Construct a atomic integer
		/// </summary>
		public AtomicInteger() : this(0)
		{
		}

		/// <summary>
		/// Construct a atomic integer
		/// </summary>
		/// <param name="value">initial value</param>
		public AtomicInteger(int value)
		{
			_value = value;
		}

		/// <summary>
		/// Get the current integer
		/// </summary>
		public int Value
		{
			get { return _value; }
			set
			{
				lock(this)
				{
					_value = value;
				}
			}
		}

		/// <summary>
		/// Atomically increments by one the current value.
		/// </summary>
		/// <returns>Return the incremented value.</returns>
		public int IncrementAndGet()
		{
			lock(this)
			{
				return ++_value;
			}
		}

		/// <summary>
		/// Atomically decrements by one the current value.
		/// </summary>
		/// <returns>Return the decremented value.</returns>
		public int DecrementAndGet()
		{
			lock(this)
			{
				return --_value;
			}
		}

		/// <summary>
		/// Atomically increments by one the current value.
		/// </summary>
		/// <returns>Return the past value.</returns>
		public int GetAndIncrement()
		{
			lock(this)
			{
				int old = _value;
				_value++;
				return old;
			}
		}

		/// <summary>
		/// Atomically decrements by one the current value.
		/// </summary>
		/// <returns>Return the past value.</returns>
		public int GetAndDecrement()
		{
			lock(this)
			{
				int old = _value;
				_value--;
				return old;
			}
		}

		/// <summary>
		/// Atomically sets to the given value and returns the old value.
		/// </summary>
		/// <param name="newValue">the new value</param>
		/// <returns>the past value</returns>
		public int GetAndSet(int newValue)
		{
			lock(this)
			{
				int old = newValue;
				_value = newValue;
				return old;
			}
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}
}