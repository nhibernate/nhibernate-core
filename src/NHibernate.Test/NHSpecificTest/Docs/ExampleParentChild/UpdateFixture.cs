using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.Docs.ExampleParentChild
{
	[TestFixture]
	public class UpdateFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] {"NHSpecificTest.Docs.ExampleParentChild.Mappings.hbm.xml"}; }
		}

		[Test]
		public void Update()
		{
			ISession session1 = OpenSession();

			Parent parent1 = new Parent();
			Child child1 = new Child();
			parent1.AddChild(child1);

			long pId = (long) session1.Save(parent1);
			long cId = (long) session1.Save(child1);
			session1.Flush();
			session1.Close();

			ISession session2 = OpenSession();
			Parent parent = session2.Load(typeof(Parent), pId) as Parent;
			Child child = session2.Load(typeof(Child), cId) as Child;
			session2.Close();

			parent.AddChild(child);
			Child newChild = new Child();
			parent.AddChild(newChild);

			ISession session = OpenSession();
			session.Update(parent);
			session.Flush();
			session.Close();

			// Clean up
			using (ISession s = OpenSession())
			{
				s.Delete("from Parent");
				s.Flush();
			}
		}
	}
}