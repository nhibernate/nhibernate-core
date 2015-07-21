using System;
using NHibernate.Proxy;
using NUnit.Framework;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1464
{
	[TestFixture]
	public class Fixture
	{
		private readonly IProxyValidator pv = new DynProxyTypeValidator();

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
			ICollection<string> errs = pv.ValidateType(typeof (CPPMimic));
			Assert.That(errs, Is.Null);
			errs = pv.ValidateType(typeof(Another));
			Assert.That(errs, Is.Null);
			errs = pv.ValidateType(typeof(OneMore));
			Assert.That(errs.Count, Is.EqualTo(1));
		}
	}
}
