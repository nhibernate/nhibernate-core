﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH681
{
	using System.Threading.Tasks;
	[TestFixture]
	public class FixtureAsync : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH681"; }
		}

		protected override void Configure(NHibernate.Cfg.Configuration cfg)
		{
			
		}

		[Test]
		public async Task BugAsync()
		{
			Foo parent = new Foo();
			parent.Children.Add(new Foo());
			parent.Children.Add(new Foo());

			using (ISession s = OpenSession())
			{
				await (s.SaveAsync(parent));
				await (s.FlushAsync());
			}

			using (ISession s = OpenSession())
			{
				Foo parentReloaded = await (s.GetAsync<Foo>(parent.Id));
				parentReloaded.Children.RemoveAt(0);
				await (s.FlushAsync());
			}
			
			using (ISession s = OpenSession())
			{
				await (s.DeleteAsync(await (s.GetAsync<Foo>(parent.Id))));
				await (s.FlushAsync());
			}
		}
	}
}
