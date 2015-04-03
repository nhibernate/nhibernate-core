using System.Diagnostics;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3772 {
	public class Fixture : TestCaseMappingByCode {
		protected override HbmMapping GetMappings() {
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc => {
				rc.Id(x => x.Id, m => m.Generator(Generators.Identity));
				rc.Property(x => x.Name);

				rc.Set(x => x.SubEntities,
					   x => {
						   x.Type<CustomGenericCollection<SubEntity>>();
					   },
					   x => {
						   x.ManyToMany();
					   });
			});

			mapper.Class<SubEntity>(rc => {
				rc.Id(x => x.Id, m => m.Generator(Generators.Identity));
				rc.Property(x => x.Name);
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp() {
			using (var session = this.OpenSession()) {
				using (var transaction = session.BeginTransaction()) {
					var e1 = new Entity {Name = "Bob"};
					session.Save(e1);

					var e2 = new Entity {Name = "Sally"};
					session.Save(e2);

					var s1 = new SubEntity {Name = "Bob"};
					session.Save(s1);

					var s2 = new SubEntity {Name = "Sally"};
					session.Save(s2);

					session.Flush();
					transaction.Commit();
				}

			}
			CustomGenericCollection<SubEntity>.TestBehavior = true;
		}

		protected override void OnTearDown() {
			CustomGenericCollection<SubEntity>.TestBehavior = false;

			using (var session = this.OpenSession()) {
				using (var transaction = session.BeginTransaction()) {
					session.Delete("from NHibernate.Test.NHSpecificTest.NH3772.Entity");
					session.Delete("from NHibernate.Test.NHSpecificTest.NH3772.SubEntity");

					session.Flush();

					transaction.Commit();
				}
			}
		}

		[Test]
		public void CustomCollectionType_ThrowsHibernateException_WhenUserCollectionTypeReturnsInitializedPersistentCollection() {
			using (var session = this.OpenSession()) {
				using (session.BeginTransaction()) {
					TestDelegate action = () => (from e in session.Query<Entity>() where e.Name == "Bob" select e).First();

					Assert.That(action, Throws.InstanceOf<HibernateException>().And.Message.EqualTo("UserCollectionType.Instantiate should return a non-initialized persistent collection. Implement UserCollectionType.Instantiate(int anticipatedSize) to actually create the collection that needs to be wrapped by the persistent collection."));
				}
			}
		}
	}
}