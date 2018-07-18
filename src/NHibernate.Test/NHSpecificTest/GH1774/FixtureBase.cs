using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1774
{
	public abstract class FixtureBase : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return TestDialect.SupportsEmptyInsertsOrHasNonIdentityNativeGenerator;
		}

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var circle = new Circle();
				var square = new Square();

				s.Save(circle);
				s.Save(square);

				s.Save(new ToyBox { Name = "Box1", Shape = circle });
				s.Save(new ToyBox { Name = "Box2", Shape = square });
				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.CreateQuery("delete from ToyBox").ExecuteUpdate();
				s.CreateQuery("delete from Circle").ExecuteUpdate();
				s.CreateQuery("delete from Square").ExecuteUpdate();
				t.Commit();
			}
		}

		[Test]
		public void AnyIs_QueryOver()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var boxes =
					s.QueryOver<ToyBox>()
					 .Where(t => t.Shape is Square)
					 .List();

				Assert.That(boxes.Count, Is.EqualTo(1));
				Assert.That(boxes[0].Name, Is.EqualTo("Box2"));
			}
		}

		[Test]
		public void AnyIs_Linq()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var boxes =
					(from t in s.Query<ToyBox>()
					 where t.Shape is Square
					 select t).ToList();

				Assert.That(boxes.Count, Is.EqualTo(1));
				Assert.That(boxes[0].Name, Is.EqualTo("Box2"));
			}
		}

		[Test]
		public void AnyIs_HqlWorksWithClassNameInTheRight()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var boxes =
					s.CreateQuery("from ToyBox t where t.Shape.class = Square")
					 .List<ToyBox>();

				Assert.That(boxes.Count, Is.EqualTo(1));
				Assert.That(boxes[0].Name, Is.EqualTo("Box2"));
			}
		}

		[Test]
		public void AnyIs_HqlWorksWithClassNameInTheLeft()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var boxes =
					s.CreateQuery("from ToyBox t where Square = t.Shape.class")
					 .List<ToyBox>();

				Assert.That(boxes.Count, Is.EqualTo(1));
				Assert.That(boxes[0].Name, Is.EqualTo("Box2"));
			}
		}

		[Test]
		public void AnyIs_HqlWorksWithParameterInTheRight()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var boxes =
					s.CreateQuery("from ToyBox t where t.Shape.class = :c")
					 .SetParameter("c", typeof(Square).FullName)
					 .List<ToyBox>();

				Assert.That(boxes.Count, Is.EqualTo(1));
				Assert.That(boxes[0].Name, Is.EqualTo("Box2"));
			}
		}

		[Test]
		public void AnyIs_HqlWorksWithParameterInTheLeft()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var boxes =
					s.CreateQuery("from ToyBox t where :c = t.Shape.class")
					 .SetParameter("c", typeof(Square).FullName)
					 .List<ToyBox>();

				Assert.That(boxes.Count, Is.EqualTo(1));
				Assert.That(boxes[0].Name, Is.EqualTo("Box2"));
			}
		}
	}
}
