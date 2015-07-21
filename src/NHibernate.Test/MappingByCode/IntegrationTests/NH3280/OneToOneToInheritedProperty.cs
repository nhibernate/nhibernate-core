using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.IntegrationTests.NH3280
{
	[TestFixture]
	public class OneToOneToInheritedProperty : TestCaseMappingByCode
	{
		private int _person1Id;
		private int _person2Id;
		private int _personDetailId;

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var person1 = new Person { FirstName = "Jack" };
				_person1Id = (int)session.Save(person1);

				var person2 = new Person { FirstName = "Robert" };
				_person2Id = (int)session.Save(person2);

				var personDetail = new PersonDetail { LastName = "Smith", Person = person1 };
				_personDetailId = (int)session.Save(personDetail);

				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.Delete("from System.Object");
				tx.Commit();
			}
		}

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<PersonDetail>(m =>
				{
					m.Id(t => t.PersonDetailId, a => a.Generator(Generators.Identity));
					m.Property(t => t.LastName,
							   c =>
								   {
									   c.NotNullable(true);
									   c.Length(32);
								   });
					m.ManyToOne(t => t.Person,
								c =>
									{
										c.Column("PersonId");
										c.Unique(true);
										c.NotNullable(false);
										c.NotFound(NotFoundMode.Ignore);
									});
				});

			mapper.Class<Person>(m =>
				{
					m.Id(t => t.PersonId, a => a.Generator(Generators.Identity));
					m.Property(t => t.FirstName,
							   c =>
								   {
									   c.NotNullable(true);
									   c.Length(32);
								   });
					m.OneToOne(t => t.PersonDetail,
							   oo =>
								   {
									   oo.PropertyReference(typeof (PersonDetail).GetProperty("Person"));
									   oo.Cascade(Mapping.ByCode.Cascade.All);
								   });
				});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		[Test]
		public void ShouldConfigureSessionCorrectly()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var person1 = session.Get<Person>(_person1Id);
				var person2 = session.Get<Person>(_person2Id);
				var personDetail = session.Get<PersonDetail>(_personDetailId);

				Assert.IsNull(person2.PersonDetail);
				Assert.IsNotNull(person1.PersonDetail);
				Assert.AreEqual(person1.PersonDetail.LastName, personDetail.LastName);
				Assert.AreEqual(person1.FirstName, personDetail.Person.FirstName);
			}
		}
	}
}
