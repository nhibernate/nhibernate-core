using System;

using NHibernate.Type;

using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// Summary description for TimeSpanTypeFixture.
	/// </summary>
	[TestFixture]
	public class TimeSpanTypeFixture
	{
		[Test]
		public void Next() 
		{
			TimeSpanType type = (TimeSpanType)NHibernate.TimeSpan;
			object current = new TimeSpan( DateTime.Now.Ticks - 5 );
			object next = type.Next( current );
			
			Assert.IsTrue( next is TimeSpan, "Next should be TimeSpan" );
			Assert.IsTrue( (TimeSpan)next > (TimeSpan)current, "next should be greater than current (could be equal depending on how quickly this occurs)" );
			
		}

		[Test]
		public void Seed() 
		{
			TimeSpanType type = (TimeSpanType)NHibernate.TimeSpan;
			Assert.IsTrue( type.Seed is TimeSpan, "seed should be TimeSpan" );
		}
	}
}
