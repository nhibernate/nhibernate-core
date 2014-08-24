using System.Collections;
using NHibernate.Mapping;
using NUnit.Framework;

namespace NHibernate.Test.TypeParameters
{
	[TestFixture]
	public class DefinedTypeForIdFixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new[] { "TypeParameters.EntityCustomId.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}
		
		[Test]
		public void HasParametrizedId()
		{
			var pc = cfg.GetClassMapping(typeof(EntityCustomId));
			var idMap = (SimpleValue)pc.IdentifierProperty.Value;
			Assert.That(idMap.IdentifierGeneratorStrategy, Is.EqualTo("NHibernate.Id.TableHiLoGenerator, NHibernate"));
			Assert.That(idMap.IdentifierGeneratorProperties["max_lo"], Is.EqualTo("99"));
		}

		[Test]
		[Description("Ensure the parametrized generator is working.")]
		public void Save()
		{
			object savedId1;
			object savedId2;

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				savedId1 = s.Save(new EntityCustomId());
				savedId2 = s.Save(new EntityCustomId());
				t.Commit();
			}

			Assert.That(savedId1, Is.LessThan(200), "should be work with custo parameters");
			Assert.That(savedId1, Is.GreaterThan(99));
			Assert.That(savedId2, Is.EqualTo((int)savedId1 + 1));

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.CreateQuery("delete from EntityCustomId").ExecuteUpdate();
				t.Commit();
			}
		}
	}
}