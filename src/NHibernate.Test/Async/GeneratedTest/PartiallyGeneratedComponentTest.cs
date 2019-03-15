﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;

using NHibernate.Dialect;

using NUnit.Framework;

namespace NHibernate.Test.GeneratedTest
{
	using System.Threading.Tasks;
	[TestFixture]
	public class PartiallyGeneratedComponentTestAsync : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override string[] Mappings
		{
			get { return new string[] { "GeneratedTest.ComponentOwner.hbm.xml" }; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2000Dialect || dialect is FirebirdDialect || dialect is Oracle8iDialect;
		}

		[Test]
		public async Task PartialComponentGenerationAsync()
		{
			ComponentOwner owner = new ComponentOwner("initial");
			ISession s = OpenSession();
			s.BeginTransaction();
			await (s.SaveAsync(owner));
			await (s.Transaction.CommitAsync());
			s.Close();

			Assert.IsNotNull(owner.Component, "expecting insert value generation");
			int previousValue = owner.Component.Generated;
			Assert.AreNotEqual(0, previousValue, "expecting insert value generation");

			s = OpenSession();
			s.BeginTransaction();
			owner = (ComponentOwner) await (s.GetAsync(typeof(ComponentOwner), owner.Id));
			Assert.AreEqual(previousValue, owner.Component.Generated, "expecting insert value generation");
			owner.Name = "subsequent";
			await (s.Transaction.CommitAsync());
			s.Close();

			Assert.IsNotNull(owner.Component);
			previousValue = owner.Component.Generated;

			s = OpenSession();
			s.BeginTransaction();
			owner = (ComponentOwner) await (s.GetAsync(typeof(ComponentOwner), owner.Id));
			Assert.AreEqual(previousValue, owner.Component.Generated, "expecting update value generation");
			await (s.DeleteAsync(owner));
			await (s.Transaction.CommitAsync());
			s.Close();
		}
	}
}
