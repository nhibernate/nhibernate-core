using System;
using System.Collections;
using NHibernate.Proxy;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace NHibernate.Test.NHSpecificTest.NH1464
{
	[TestFixture]
	public class Fixture
	{
		public class CPPMimicBase
		{
			public virtual void Dispose()
			{
				
			}
		}
		public class CPPMimic : CPPMimicBase
		{
			public sealed override void Dispose()
			{

			}
		}

		public class Another: IDisposable
		{
			protected void Dispose(bool disposing)
			{

			}

			public void Dispose()
			{
			}

			~Another()
			{
				
			}
		}

		public class OneMore : IDisposable
		{
			public void Dispose(bool disposing)
			{

			}

			public void Dispose()
			{
			}

			~OneMore()
			{

			}
		}

		[Test]
		public void NoExceptionForDispose()
		{
			ICollection errs = ProxyTypeValidator.ValidateType(typeof (CPPMimic));
			Assert.That(errs, Is.Null);
			errs = ProxyTypeValidator.ValidateType(typeof(Another));
			Assert.That(errs, Is.Null);
			errs = ProxyTypeValidator.ValidateType(typeof(OneMore));
			Assert.That(errs.Count, Is.EqualTo(1));
		}
	}
}
