using System;

using NHibernate.Type;

using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// Summary description for TimestampTypeFixture.
	/// </summary>
	[TestFixture]
	public class TimestampTypeFixture
	{
		[Test]
		public void Next() 
		{
			TimestampType type = (TimestampType)NHibernateUtil.Timestamp;
			object current = DateTime.Parse( "2004-01-01" );
			object next = type.Next( current );
			
			Assert.IsTrue( next is DateTime, "Next should be DateTime" );
			Assert.IsTrue( (DateTime)next > (DateTime)current, "next should be greater than current (could be equal depending on how quickly this occurs)" );
			
		}

		[Test]
		public void Seed() 
		{
			TimestampType type = (TimestampType)NHibernateUtil.Timestamp;
			Assert.IsTrue( type.Seed is DateTime, "seed should be DateTime" );
		}
	}
}
