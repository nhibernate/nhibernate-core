using System;

using NHibernate.Type;

using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// Summary description for Int32TypeFixture.
	/// </summary>
	[TestFixture]
	public class Int32TypeFixture
	{
		[Test]
		public void Next() 
		{
			Int32Type type = (Int32Type)NHibernate.Int32;
			object current = (int)1;
			object next = type.Next( current );
			
			Assert.IsTrue( next is Int32, "Next should be Int32" );
			Assert.AreEqual( (int)2, (int)next, "current should have been incremented to 2" );
			
		}

		[Test]
		public void Seed() 
		{
			Int32Type type = (Int32Type)NHibernate.Int32;
			Assert.IsTrue( type.Seed is Int32, "seed should be Int32" );
		}
	}
}
