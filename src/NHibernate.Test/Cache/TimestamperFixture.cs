using System;

using NHibernate.Cache;

using NUnit.Framework;

namespace NHibernate.Test.Cache
{
	/// <summary>
	/// Summary description for TimestamperFixture.
	/// </summary>
	[TestFixture]
	public class TimestamperFixture
	{
		[Test]
		public void VerifyIncrease() 
		{
			long currentTicks = 0;
			long newTicks = 0;

			// the Timestampper will only generate 4095 increasing identifiers per millisecond.
			for( int i=0; i<4095; i++ ) 
			{
				newTicks = Timestamper.Next();
				if( (newTicks - currentTicks) == 0 ) 
				{
					Assert.Fail( "diff was " + (newTicks - currentTicks) + ".  It should always increase.  Loop i=" + i + " with currentTicks = " + currentTicks + " and newTicks = " + newTicks );
				}
				currentTicks = newTicks;
			}
		}
	}
}
