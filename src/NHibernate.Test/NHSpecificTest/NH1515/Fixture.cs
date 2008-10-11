using System.Collections.Generic;
using NHibernate.Proxy;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace NHibernate.Test.NHSpecificTest.NH1515
{
	[TestFixture]
	public class Fixture
	{
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
			ICollection<string> errs = ProxyTypeValidator.ValidateType(typeof(ClassWithInternal));
			Assert.That(errs, Is.Not.Null);
			Assert.That(errs.Count, Is.EqualTo(1));
		}

		[Test]
		public void NoExceptionForProperty()
		{
			ICollection<string> errs = ProxyTypeValidator.ValidateType(typeof(ClassWithInternalProperty));
			Assert.That(errs, Is.Not.Null);
			Assert.That(errs.Count, Is.EqualTo(2));
		}
	}
}