using System;

using NHibernate.Dialect;

using NUnit.Framework;

namespace NHibernate.Test.GeneratedTest
{
	[TestFixture]
	public class PartiallyGeneratedComponentTest : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override System.Collections.IList Mappings
		{
			get { return new string[] { "GeneratedTest.ComponentOwner.hbm.xml" }; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2000Dialect || dialect is FirebirdDialect || dialect is Oracle8iDialect;
		}

		[Test]
		public void PartialComponentGeneration()
		{
			ComponentOwner owner = new ComponentOwner("initial");
			ISession s = OpenSession();
			s.BeginTransaction();
			s.Save(owner);
			s.Transaction.Commit();
			s.Close();

			Assert.IsNotNull(owner.Component, "expecting insert value generation");
			int previousValue = owner.Component.Generated;
			Assert.AreNotEqual(0, previousValue, "expecting insert value generation");

			s = OpenSession();
			s.BeginTransaction();
			owner = (ComponentOwner) s.Get(typeof(ComponentOwner), owner.Id);
			Assert.AreEqual(previousValue, owner.Component.Generated, "expecting insert value generation");
			owner.Name = "subsequent";
			s.Transaction.Commit();
			s.Close();

			Assert.IsNotNull(owner.Component);
			previousValue = owner.Component.Generated;

			s = OpenSession();
			s.BeginTransaction();
			owner = (ComponentOwner) s.Get(typeof(ComponentOwner), owner.Id);
			Assert.AreEqual(previousValue, owner.Component.Generated, "expecting update value generation");
			s.Delete(owner);
			s.Transaction.Commit();
			s.Close();
		}
	}
}
