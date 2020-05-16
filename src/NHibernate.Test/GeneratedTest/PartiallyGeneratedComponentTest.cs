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

		protected override string[] Mappings
		{
			get { return new [] { "GeneratedTest.ComponentOwner.hbm.xml" }; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2000Dialect || dialect is FirebirdDialect || dialect is Oracle8iDialect;
		}

		[Test]
		public void PartialComponentGeneration()
		{
			ComponentOwner owner = new ComponentOwner("initial");
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Save(owner);
				t.Commit();
				s.Close();
			}

			Assert.IsNotNull(owner.Component, "expecting insert value generation");
			int previousValue = owner.Component.Generated;
			Assert.AreNotEqual(0, previousValue, "expecting insert value generation");

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				owner = (ComponentOwner) s.Get(typeof(ComponentOwner), owner.Id);
				Assert.AreEqual(previousValue, owner.Component.Generated, "expecting insert value generation");
				owner.Name = "subsequent";
				t.Commit();
				s.Close();
			}

			Assert.IsNotNull(owner.Component);
			previousValue = owner.Component.Generated;

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				owner = (ComponentOwner) s.Get(typeof(ComponentOwner), owner.Id);
				Assert.AreEqual(previousValue, owner.Component.Generated, "expecting update value generation");
				s.Delete(owner);
				t.Commit();
				s.Close();
			}
		}
	}
}
