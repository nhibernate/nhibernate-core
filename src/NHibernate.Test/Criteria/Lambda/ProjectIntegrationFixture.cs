using System.Collections;
using System.Linq;
using NHibernate.Criterion;
using NHibernate.Transform;
using NUnit.Framework;

namespace NHibernate.Test.Criteria.Lambda
{
	[TestFixture]
	public class ProjectIntegrationFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] { "Criteria.Lambda.Mappings.hbm.xml" }; }
		}

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Save(new Person { Name = "test person 1", Age = 20 });
				s.Save(new Person { Name = "test person 1", Age = 30 });
				s.Save(new Person { Name = "test person 2", Age = 40 });
				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.CreateQuery("delete from Person").ExecuteUpdate();
				t.Commit();
			}
		}

		[Test]
		public void MultipleProperties()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				Person personAlias = null;
				var actual = s.QueryOver<Person>(() => personAlias)
					.Select(p => p.Name, p => personAlias.Age)
					.OrderBy(p => p.Age).Asc
					.List<object[]>()
					.Select(props => new
					{
						TestName = (string) props[0],
						TestAge = (int) props[1],
					});

				Assert.That(actual.ElementAt(0).TestName, Is.EqualTo("test person 1"));
				Assert.That(actual.ElementAt(1).TestAge, Is.EqualTo(30));
			}
		}

		[Test]
		public void SingleProperty()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var actual = s.QueryOver<Person>()
						.Select(p => p.Age)
						.OrderBy(p => p.Age).Asc
						.List<int>();

				Assert.That(actual[0], Is.EqualTo(20));
			}
		}

		[Test]
		public void ProjectTransformToDto()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				PersonSummary summary = null;
				var actual = s.QueryOver<Person>()
					.SelectList(list => list
						.SelectGroup(p => p.Name).WithAlias(() => summary.Name)
						.Select(Projections.RowCount()).WithAlias(() => summary.Count))
					.OrderByAlias(() => summary.Name).Asc
					.TransformUsing(Transformers.AliasToBean<PersonSummary>())
					.List<PersonSummary>();

				Assert.That(actual.Count, Is.EqualTo(2));
				Assert.That(actual[0].Name, Is.EqualTo("test person 1"));
				Assert.That(actual[0].Count, Is.EqualTo(2));
				Assert.That(actual[1].Name, Is.EqualTo("test person 2"));
				Assert.That(actual[1].Count, Is.EqualTo(1));
			}
		}
	}
}
