using System;

namespace NHibernate.Cache
{
	/// <summary>
	/// Generates increasing identifiers (in a single application domain only).
	/// </summary>
	/// <remarks>
	/// Not valid across multiple application domains. Identifiers are not necessarily
	/// strictly increasing, but usually are.
	///	</remarks>
	public static class Timestamper
	{
		private static object lockObject = new object();

		// hibernate is using System.currentMilliSeconds which is calculated
		// from jan 1, 1970
		private static long baseDateMs = (new DateTime(1970, 1, 1).Ticks) / 10000;

		private static short counter = 0;
		private static long time;
		private const int BinDigits = 12;

		/// <summary></summary>
		public const short OneMs = 1 << BinDigits; //(4096 is the value)

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static long Next()
		{
			lock (lockObject)
			{
				// Ticks is accurate down to 100 nanoseconds - hibernate uses milliseconds
				// to help calculate next time so drop the nanoseconds portion.(1ms==1000000ns)
				long newTime = ((DateTime.Now.Ticks / 10000) - baseDateMs) << BinDigits;
				if (time < newTime)
				{
					time = newTime;
					counter = 0;
				}
				else if (counter < OneMs - 1)
				{
					counter++;
				}
				return time + counter;
			}
		}
	}
}