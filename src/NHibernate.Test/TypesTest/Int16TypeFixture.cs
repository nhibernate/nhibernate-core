using System;
using System.Data;

using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// Summary description for Int16TypeFixture.
	/// </summary>
	[TestFixture]
	public class Int16TypeFixture
	{
		[Test]
		public void Next() 
		{
			Int16Type type = (Int16Type)NHibernateUtil.Int16;
			object current = (short)1;
			object next = type.Next( current );
			
			Assert.IsTrue( next is Int16, "Next should be Int16" );
			Assert.AreEqual( (short)2, (short)next, "current should have been incremented to 2" );
			
		}

		[Test]
		public void Seed() 
		{
			Int16Type type = (Int16Type)NHibernateUtil.Int16;
			Assert.IsTrue( type.Seed is Int16, "seed should be int16" );
		}
	}
}
