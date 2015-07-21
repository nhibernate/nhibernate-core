using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH681
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH681"; }
		}

		protected override void Configure(NHibernate.Cfg.Configuration cfg)
		{
			
		}

		[Test]
		public void Bug()
		{
			Foo parent = new Foo();
			parent.Children.Add(new Foo());
			parent.Children.Add(new Foo());

			using (ISession s = OpenSession())
			{
				s.Save(parent);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				Foo parentReloaded = s.Get<Foo>(parent.Id);
				parentReloaded.Children.RemoveAt(0);
				s.Flush();
			}
			
			using (ISession s = OpenSession())
			{
				s.Delete(s.Get<Foo>(parent.Id));
				s.Flush();
			}
		}
	}
}
