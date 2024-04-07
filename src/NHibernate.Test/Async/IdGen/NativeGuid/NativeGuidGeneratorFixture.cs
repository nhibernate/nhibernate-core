﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Id;
using NUnit.Framework;

namespace NHibernate.Test.IdGen.NativeGuid
{
	using System.Threading.Tasks;
	using System.Threading;
	[TestFixture]
	public class NativeGuidGeneratorFixtureAsync
	{
		protected Configuration cfg;
		protected ISessionFactoryImplementor sessions;

		[OneTimeSetUp]
		public void TestFixtureSetUp()
		{
			cfg = new Configuration();
			if (TestConfigurationHelper.hibernateConfigFile != null)
				cfg.Configure(TestConfigurationHelper.hibernateConfigFile);

			sessions = (ISessionFactoryImplementor) cfg.BuildSessionFactory();
		}

		[Test]
		public async Task ReturnedValueIsGuidAsync()
		{
			try
			{
				var str = Dialect.Dialect.GetDialect().SelectGUIDString;	
			}
			catch (NotSupportedException)
			{
				Assert.Ignore($"This test does not apply to {Dialect.Dialect.GetDialect()}");
			}
			 
			var gen = new NativeGuidGenerator();
			using (ISession s = sessions.OpenSession())
			{
				object result = await (gen.GenerateAsync((ISessionImplementor)s, null, CancellationToken.None));
				Assert.That(result, Is.TypeOf(typeof (Guid)));
				Assert.That(result, Is.Not.EqualTo(Guid.Empty));
			}
		}
	}
}
