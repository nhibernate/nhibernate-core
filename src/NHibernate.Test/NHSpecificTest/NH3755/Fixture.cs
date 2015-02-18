using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3755
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var c = new Circle { Property1 = "Circle1", Property2 = "Circle2", Property3 = "Circle3"};
				session.Save(c);

                var s = new Square { Property1 = "Square1", Property2 = "Square2", Property3 = "Square3" };
                session.Save(c); session.Save(s);

			    var sc1 = new ShapeContainer {Name = "Circle", Shape = c};
                var sc2 = new ShapeContainer { Name = "Square", Shape = s };

			    session.Save(sc1);
			    session.Save(sc2);

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

        [Test, KnownBug("NH-3755")]
		public void TestCompositeProxy()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result1 = (from e in session.Query<ShapeContainer>()
							 where e.Name == "Circle"
							 select e).ToList();

                var result2 = (from e in session.Query<ShapeContainer>()
                               where e.Name == "Square"
                               select e).ToList();

			    var circle = (ICircle) result1[0].Shape;
			    var square = (ISquare) result2[0].Shape;

                Assert.IsNotNull(circle.Property1);
                Assert.IsNotNull(circle.Property2);
                Assert.IsNotNull(circle.Property3);

                Assert.IsNotNull(square.Property1);
                Assert.IsNotNull(square.Property2);
                Assert.IsNotNull(square.Property3);

			}
		}
	}
}