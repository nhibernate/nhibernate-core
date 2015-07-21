using System.Collections.Generic;
using NHibernate.Proxy;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1515
{
	[TestFixture]
	public class Fixture
	{
		private readonly IProxyValidator pv = new DynProxyTypeValidator();

		public class ClassWithInternal
		{
			internal virtual void DoSomething() {}
		}

		public class ClassWithInternalProperty
		{
			internal virtual string DoSomething { get; set; }
		}

		[Test]
		public void NoExceptionForMethod()
		{
			ICollection<string> errs = pv.ValidateType(typeof(ClassWithInternal));
			Assert.That(errs, Is.Not.Null);
			Assert.That(errs.Count, Is.EqualTo(1));
		}

		[Test]
		public void NoExceptionForProperty()
		{
			ICollection<string> errs = pv.ValidateType(typeof(ClassWithInternalProperty));
			Assert.That(errs, Is.Not.Null);
			Assert.That(errs.Count, Is.EqualTo(2));
		}
	}
}