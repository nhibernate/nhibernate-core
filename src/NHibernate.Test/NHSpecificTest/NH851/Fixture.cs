using System;
using NHibernate.Type;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH851
{
	[TestFixture]
	public class Fixture
	{
		public class SomeClass
		{
			public SomeClass(int x)
			{
			}
		}

		[Test]
		public void ConstructorNotFound()
		{
			try
			{
				ReflectHelper.GetConstructor(typeof(SomeClass), new IType[] {NHibernateUtil.String});
				Assert.Fail("Should have thrown an exception");
			}
			catch (InstantiationException e)
			{
				Assert.IsTrue(e.Message.IndexOf("String") >= 0);
				Assert.IsTrue(e.Message.IndexOf(typeof(SomeClass).ToString()) >= 0);
			}
		}
	}
}