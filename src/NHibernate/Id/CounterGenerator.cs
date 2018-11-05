using System;
using System.Runtime.CompilerServices;
using NHibernate.Engine;

namespace NHibernate.Id
{
	/// <summary>
	/// An <see cref="IIdentifierGenerator" /> that returns a <c>Int64</c> constructed from the system
	/// time and a counter value. Not safe for use in a clustser! May generate colliding identifiers in
	/// a bit less than one year.
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
			// This causes the most significant digits to be shifted out, causing hi-part to cycle in a bit less than
			// one year Count only serves to avoid collision for entities persisted in the same 100ns. Maybe it should
			// have been (DateTime.Now.Ticks && 0xffff) instead, with count serving to avoid collision for up to 37767
			// entities in the same 6.5535ms. But changing this would be a breaking change for existing values.
			return unchecked ((DateTime.Now.Ticks << 16) + Count);
		}
	}
}
