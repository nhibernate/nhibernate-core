using NHibernate.Criterion;
using NHibernate.Transform;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3455
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new Person { Name = "Bob", Age = 31, Weight = 185, Address = new Address
					{
						City = "Abington",
						State = "VA",
						Street = "Avenue",
						Zip = "11121"
					}};
				session.Save(e1);

				var e2 = new Person
				{
					Name = "Sally",
					Age = 22,
					Weight = 122,
					Address = new Address
					{
						City = "Olympia",
						State = "WA",
						Street = "Broad",
						Zip = "99989"
					}
				};
				session.Save(e2);

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void OrderBySpecifiedPropertyWithQueryOver()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				PersonDto dto = null;
				var people = session.QueryOver<Person>()
									.SelectList(b => b.Select(p => p.Id).WithAlias(() => dto.Id)
													  .Select(p => p.Name).WithAlias(() => dto.Name)
													  .Select(p => p.Address).WithAlias(() => dto.Address)
													  .Select(p => p.Age).WithAlias(() => dto.Age))
									.OrderBy(p => p.Age)
									.Desc
									.TransformUsing(Transformers.AliasToBean<PersonDto>())
									.List<PersonDto>();

				Assert.That(people.Count, Is.EqualTo(2));
				Assert.That(people, Is.Ordered.By("Age").Descending);
			}
		}

		[Test]
		public void OrderBySpecifiedPropertyWithCriteria()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var selectList = Projections.ProjectionList()
				                            .Add(Projections.Property("Id"), "Id")
				                            .Add(Projections.Property("Name"), "Name")
				                            .Add(Projections.Property("Address"), "Address")
											.Add(Projections.Property("Age"), "Age");
				var order = new Order("Age", false);
				var people = session.CreateCriteria<Person>()
									.SetProjection(selectList)
									.AddOrder(order)
									.SetResultTransformer(Transformers.AliasToBean<PersonDto>())
									.List<PersonDto>();

				Assert.That(people.Count, Is.EqualTo(2));
				Assert.That(people, Is.Ordered.By("Age").Descending);
			}
		}
	}
}