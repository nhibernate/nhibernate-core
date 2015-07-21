using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2328
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			base.OnSetUp();

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				var circle = new Circle();
				var square = new Square();

				s.Save(circle);
				s.Save(square);

				s.Save(new ToyBox() { Name = "Box1", Shape = circle });
				s.Save(new ToyBox() { Name = "Box2", Shape = square });
				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
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
			using (ISession s = OpenSession())
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
			using (ISession s = OpenSession())
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
			using (ISession s = OpenSession())
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
			using (ISession s = OpenSession())
			{
				var boxes =
					s.CreateQuery("from ToyBox t where Square = t.Shape.class")
						.List<ToyBox>();

				Assert.That(boxes.Count, Is.EqualTo(1));
				Assert.That(boxes[0].Name, Is.EqualTo("Box2"));
			}
		}
	}
}