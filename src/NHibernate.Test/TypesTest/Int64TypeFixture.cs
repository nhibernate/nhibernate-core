using System;

using NHibernate.Type;

using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// Summary description for Int64TypeFixture.
	/// </summary>
	[TestFixture]
	public class Int64TypeFixture
	{
		[Test]
		public void Next() 
		{
			Int64Type type = (Int64Type)NHibernate.Int64;
			object current = (long)1;
			object next = type.Next( current );
			
			Assert.IsTrue( next is Int64, "Next should be Int64" );
			Assert.AreEqual( (long)2, (long)next, "current should have been incremented to 2" );
			
		}

		[Test]
		public void Seed() 
		{
			Int64Type type = (Int64Type)NHibernate.Int64;
			Assert.IsTrue( type.Seed is Int64, "seed should be int64" );
		}
	}
}
