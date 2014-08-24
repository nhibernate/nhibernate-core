using NHibernate.Id.Enhanced;

namespace NHibernate.Test.IdGen.Enhanced
{
	public class SourceMock : IAccessCallback
	{
		private long _val;
		private readonly long _initialValue;
		private readonly int _increment;
		private int _timesCalled;

		public SourceMock(long initialValue) : this(initialValue, 1) { }

		public SourceMock(long initialValue, int increment) : this(initialValue, increment, 0) { }

		public SourceMock(long initialValue, int increment, int timesCalled)
		{
			_increment = increment;
			_timesCalled = timesCalled;

			if (timesCalled != 0)
			{
				_val = initialValue;
				_initialValue = 1;
			}
			else
			{
				_val = -1;
				_initialValue = initialValue;
			}
		}

		public long GetNextValue()
		{
			try
			{
				if (_timesCalled == 0)
				{
					InitValue();
					return _val;
				}
				else
				{
					//return value.add( increment ).copy();
					_val += _increment;
					return _val;
				}
			}
			finally
			{
				_timesCalled++;
			}
		}

		private void InitValue()
		{
			_val = _initialValue;
		}

		public int TimesCalled
		{
			get { return _timesCalled; }
		}

		public long CurrentValue
		{
			get
			{
				//return value== null ? -1 : value.getActualLongValue();
				return _val;
			}
		}
	}
}