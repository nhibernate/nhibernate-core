using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Type;
using NUnit.Framework;
using NHibernate.Engine;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// Summary description for Int64TypeFixture.
	/// </summary>
	[TestFixture]
	public class Int64TypeFixture: TestCase
	{
		[Test]
		public void Next()
		{
			Int64Type type = (Int64Type)NHibernateUtil.Int64;
			object current = (long)1;
			object next = type.Next(current, null);

			Assert.IsTrue(next is Int64, "Next should be Int64");
			Assert.AreEqual((long)2, (long)next, "current should have been incremented to 2");
		}

		[Test]
		public void Seed()
		{
			Int64Type type = (Int64Type)NHibernateUtil.Int64;
			Assert.IsTrue(type.Seed(null) is Int64, "seed should be int64");
		}

		[Test]
		public void NullableWrapperDirty()
		{
			Int64Type type = (Int64Type)NHibernateUtil.Int64;

			Nullable<Int64> nullLong = null;
			Nullable<Int64> valueLong = new Nullable<long>(5);
			Nullable<long> fiveAgain = (long)5;
			using (ISession s = OpenSession())
			{
				Assert.IsTrue(type.IsDirty(nullLong, valueLong, (ISessionImplementor)s), "should be dirty - null to '5'");
				Assert.IsFalse(type.IsDirty(valueLong, fiveAgain, (ISessionImplementor)s), "should not be dirty - 5 to 5");
			}
		}

		protected override IList Mappings
		{
			get { return new List<string>(); }
		}
	}
}
