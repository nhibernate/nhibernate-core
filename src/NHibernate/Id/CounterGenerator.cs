using System;
using System.Runtime.CompilerServices;
using NHibernate.Engine;

namespace NHibernate.Id
{
	/// <summary>
	/// An <see cref="IIdentifierGenerator" /> that returns a <c>Int64</c> constructed from the system
	/// time and a counter value. Not safe for use in a clustser!
	/// </summary>
	public partial class CounterGenerator : IIdentifierGenerator
	{
		// (short)0 by default
		private static short counter;

		protected short Count
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			get
			{
				if (counter < 0)
				{
					counter = 0;
				}
				return counter++;
			}
		}

		public object Generate(ISessionImplementor cache, object obj)
		{
			return unchecked ((DateTime.Now.Ticks << 16) + Count);
		}
	}
}