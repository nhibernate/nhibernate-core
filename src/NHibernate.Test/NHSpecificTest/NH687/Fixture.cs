using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH687
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return TestDialect.SupportsEmptyInsertsOrHasNonIdentityNativeGenerator;
		}

		[Test]
		public void GetQueryTest()
		{
			//Instantiate and setup associations (all needed to generate the error);
			Bar bar1 = new Bar();
			Bar bar2 = new Bar();
			Foo foo = new Foo();
			bar1.Children.Add(bar2);
			foo.FooBar = new FooBar();
			foo.FooBar.Children.Add(new FooBar());
			foo.FooBar.Children.Add(new FooBar());
			foo.FooBar.Bar = bar1;
			foo.FooBar.Children[0].Bar = bar2;
			foo.FooBar.Children[1].Bar = bar2;

			foo.Children.Add(new Foo());
			foo.Children.Add(new Foo());

			try
			{
				int child1Id, child2Id;
				using (ISession session = Sfi.OpenSession())
				{
					session.Save(foo);

					child1Id = foo.Children[0].Id;
					child2Id = foo.Children[1].Id;
					session.Flush();
				}

				using (ISession session = Sfi.OpenSession())
				{
					Foo r = session.Get<Foo>(foo.Id);
					Assert.IsNotNull(r);

					Foo child1a = session.Get<Foo>(child1Id);
					Assert.IsNotNull(child1a);

					Foo child2a = session.Get<Foo>(child2Id);
					Assert.IsNotNull(child2a);
				}
			}
			finally
			{
				using (ISession session = OpenSession())
				{
					session.Delete(foo);
					session.Delete(foo.FooBar);
					session.Delete(bar1);
					session.Delete(bar2);
					session.Flush();
				}
			}
		}
	}
}
