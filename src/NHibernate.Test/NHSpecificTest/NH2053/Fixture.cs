using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2053
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(NHibernate.Dialect.Dialect dialect)
		{
			return dialect is MsSql2005Dialect;
		}
		protected override void OnSetUp()
		{
			using (var session = this.OpenSession())
			{
				using (var tran = session.BeginTransaction())
				{
                    Dog snoopy = new Dog()
                    {
                        Name = "Snoopy",
                        Talkable = false
                    };
                    snoopy.Name = "Snoopy";
                    Dog Jake = new Dog()
                    {
                        Name = "Jake the dog",
                        Talkable = true
                    };
                    session.Save(snoopy);
                    session.Save(Jake);
                    Cat kitty = new Cat()
                    {
                        Name = "Kitty"
                    };
                    session.Save(kitty);
					tran.Commit();
				}
			}
		}
		protected override void OnTearDown()
		{
			using (var session = this.OpenSession())
			{
				using (var tran = session.BeginTransaction())
				{
					session.Delete("from Dog");
                    session.Delete("from Animal");
                    tran.Commit();
				}
			}
		}

		[Test]
		public void JoinedSubClass_Filter()
		{
			using (var session = this.OpenSession())
			{
				using (var tran = session.BeginTransaction())
				{
                    session.EnableFilter("talkableFilter").SetParameter("talkable", true);
                    var snoopy = session.QueryOver<Dog>().Where(x => x.Name == "Snoopy").SingleOrDefault();
                    Assert.AreEqual(null, snoopy); // there are no talking dog named Snoopy.

                    var jake = session.QueryOver<Dog>().Where(x => x.Name == "Jake the dog").SingleOrDefault();
                    Assert.AreNotEqual(null, jake);
                    Assert.AreEqual("Jake the dog", jake.Name);

                    var kitty = session.QueryOver<Cat>().Where(x => x.Name == "Kitty").SingleOrDefault();
                    Assert.AreNotEqual(null, kitty);
                    Assert.AreEqual("Kitty", kitty.Name);
				}
			}
		}

	}
}