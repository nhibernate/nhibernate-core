using System;
using System.Runtime.CompilerServices;

namespace NHibernate.Cache 
{
	/// <summary>
	/// Geterates increasing identifiers. 
	/// </summary>
	/// <remarks>
	/// Not valid across multiple application domains. Identifiers are not necessarily
	/// strickly increasing, but usually are.
	///	</remarks>
	public class Timestamper 
	{
		private static short counter = 0;
		private static long time;
		private const int BinDigits = 12;
		public const short OneMs = 1<<BinDigits;

		[MethodImpl(MethodImplOptions.Synchronized)]
		public static long Next() 
		{
			long newTime = System.DateTime.Now.Ticks << BinDigits; //is this right?
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

		private Timestamper()
		{
		}
	}
}
