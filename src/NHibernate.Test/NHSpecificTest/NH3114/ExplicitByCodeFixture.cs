using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3114
{
	[TestFixture]
	public class ExplicitByCodeFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.Id(i => i.Id, m => m.Generator(Generators.GuidComb));
				rc.Component(p => p.FirstComponent,
					m =>
					{
						m.Set(c => c.ComponentCollection,
							c => c.Table("FirstTable"),
							c => c.Element());
						m.Property(p => p.ComponentProperty, p => p.Column("FirstProperty"));
					});
				rc.Component(p => p.SecondComponent,
					m =>
					{
						m.Set(c => c.ComponentCollection,
							c => c.Table("SecondTable"),
							c => c.Element());
						m.Property(p => p.ComponentProperty, p => p.Column("SecondProperty"));
					});
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from Entity");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void Component_WithSameType_ButDifferentTables_ShouldBeMappedAccordingly()
		{
			var mappings = GetMappings();
			var modelMapping = mappings.Items.OfType<HbmClass>().FirstOrDefault();
			Assert.IsNotNull(modelMapping);
			var lists = modelMapping.Items.OfType<HbmComponent>();
			Assert.AreEqual(2, lists.Count());
			var firstMapping = lists.FirstOrDefault(l => l.Name == nameof(Entity.FirstComponent));
			Assert.IsNotNull(firstMapping);
			var firstMember = firstMapping.Properties.OfType<HbmProperty>().FirstOrDefault(p => p.Name == nameof(Component.ComponentProperty));
			Assert.IsNotNull(firstMember);
			Assert.AreEqual("FirstProperty", firstMember.column);
			var firstCollection = firstMapping.Items.OfType<HbmSet>().FirstOrDefault();
			Assert.IsNotNull(firstCollection);
			Assert.AreEqual("FirstTable", firstCollection.Table);
			var secondMapping = lists.FirstOrDefault(l => l.Name == nameof(Entity.SecondComponent));
			Assert.IsNotNull(secondMapping);
			var secondMember = secondMapping.Properties.OfType<HbmProperty>().FirstOrDefault(p => p.Name == nameof(Component.ComponentProperty));
			Assert.IsNotNull(secondMember);
			Assert.AreEqual("SecondProperty", secondMember.column);
			var secondCollection = secondMapping.Items.OfType<HbmSet>().FirstOrDefault();
			Assert.IsNotNull(secondCollection);
			Assert.AreEqual("SecondTable", secondCollection.Table);
		}

		[Test]
		public void Component_WithSameType_ButDifferentTables_IsStoredInTheCorrectTableAndCollection()
		{
			Guid previouslySavedId;
			using (var session = OpenSession())
			{
				var entity = new Entity();
				entity.FirstComponent = new Component();
				entity.FirstComponent.ComponentProperty = "First";
				entity.FirstComponent.ComponentCollection = new List<string> { "FirstOne", "FirstTwo", "FirstThree" };
				// not setting entity.SecondComponent; it must not contain the contents of entity.FirstComponent later
				session.SaveOrUpdate(entity);
				session.Flush();
				previouslySavedId = entity.Id;
			}

			using (var session = OpenSession())
			{
				var entity = session.Get<Entity>(previouslySavedId);
				Assert.IsNotNull(entity);
				Assert.IsNotNull(entity.FirstComponent);
				Assert.AreEqual("First", entity.FirstComponent.ComponentProperty);
				CollectionAssert.AreEquivalent(new[] { "FirstOne", "FirstTwo", "FirstThree" }, entity.FirstComponent.ComponentCollection);
				//Assert.IsNull(entity.SecondComponent); // cannot check SecondComponent for null, since components are apparently always initialized
				Assert.AreNotEqual("First", entity.SecondComponent.ComponentProperty);
				CollectionAssert.IsEmpty(entity.SecondComponent.ComponentCollection);
			}
		}
	}
}
