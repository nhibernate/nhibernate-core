using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Id.Enhanced;

namespace NHibernate.Test.IdGen.Enhanced
{
	public class SourceMock : IAccessCallback
	{
		private long val;
		private long initialValue;
		private int increment;
		private int timesCalled = 0;

		public SourceMock(long initialValue)
			: this(initialValue, 1)
		{
		}

		public SourceMock(long initialValue, int increment)
			: this(initialValue, increment, 0)
		{
		}

		public SourceMock(long initialValue, int increment, int timesCalled)
		{
			this.increment = increment;
			this.timesCalled = timesCalled;
			if (timesCalled != 0)
			{
				this.val = initialValue;
				this.initialValue = 1;
			}
			else
			{
				this.val = -1;
				this.initialValue = initialValue;
			}
		}

		public long GetNextValue()
		{
			try
			{
				if (timesCalled == 0)
				{
					InitValue();
					return val;
				}
				else
				{
					//return value.add( increment ).copy();
					val += increment;
					return val;
				}
			}
			finally
			{
				timesCalled++;
			}
		}

		private void InitValue()
		{
			this.val = initialValue;
		}

		public int TimesCalled { get { return timesCalled; } }

		public long CurrentValue
		{
			get
			{
				//return value== null ? -1 : value.getActualLongValue();
				return val;
			}
		}
	}
}
