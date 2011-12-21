using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH2974
{
	public class ByCodeFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
		    mapper.Class<Entity>(rc =>
		                             {
		                                 rc.Id(x => x.Id, ClassMappings);
		                                 rc.Property(x => x.Name);
		                             });

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

        private void ClassMappings(IIdMapper mapper)
        {
            mapper.Generator(Generators.Identity);
            mapper.UnsavedValue(-1);
        }

		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var e1 = new Entity { Name = "Bob" };
				session.Save(e1);

				var e2 = new Entity { Name = "Sally" };
				session.Save(e2);

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

        [Test]
        public void CanSetUnsavedValue()
        {
            var hbmId = new HbmId();
            var mapper = new IdMapper(null, hbmId);
            mapper.UnsavedValue(-1);
            hbmId.unsavedvalue.Should().Be("-1");
        }

		[Test]
		public void CanSavePersistedEntityAsNewEntityBySettingUnsavedValue()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = from e in session.Query<Entity>()
							 where e.Name == "Bob"
							 select e;

				Assert.AreEqual(1, result.ToList().Count);

			    var entity = result.First();

                //Remove from session
                session.Evict(entity);

                //Find Id for already persisted entity
			    var oldEntityId = entity.Id;

                //Mark entity as unsaved/transient
			    entity.Id = -1;

                //Save as a new object
                session.SaveOrUpdate(entity);

                Assert.That(oldEntityId, Is.Not.EqualTo(entity.Id));
			}
		}
	}
}