using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1192
{
	[TestFixture]
	public class Fixture:BugTestCase
	{
		protected override void OnSetUp()
		{
			using(var session=this.OpenSession())
			using(var tran=session.BeginTransaction())
			{
				session.Save(new ObjectA {FontType = Status.Bold|Status.Italic, Name = "Object1"});
				session.Save(new ObjectA { FontType = Status.Italic, Name = "Object2" });
				session.Save(new ObjectA { FontType = Status.Underlined, Name = "Object2" });
				tran.Commit();
			}
		}
		protected override void OnTearDown()
		{
			using (var session = this.OpenSession())
			using (var tran = session.BeginTransaction())
			{
				session.Delete("from ObjectA");
				tran.Commit();
			}
		}

		[Test]
		public void BitwiseAndWorksCorrectly()
		{
			using (var session = this.OpenSession())
			using (var tran = session.BeginTransaction())
			{
				var query = session.CreateQuery("from ObjectA o where (o.FontType & 1) > 0");
				var result = query.List();
				Assert.That(result,Has.Count.EqualTo(1));
				query = session.CreateQuery("from ObjectA o where (o.FontType & 2) > 0");
				result = query.List();
				Assert.That(result, Has.Count.EqualTo(2));

				query = session.CreateQuery("from ObjectA o where (o.FontType & 4) > 0");
				result = query.List();
				Assert.That(result, Has.Count.EqualTo(1));
			}
		}

		[Test]
		public void BitwiseOrWorksCorrectly()
		{
			using (var session = this.OpenSession())
			using (var tran = session.BeginTransaction())
			{
				var query = session.CreateQuery("from ObjectA o where (o.FontType | 2)  = (1|2) ");
				var result = query.List();
				Assert.That(result, Has.Count.EqualTo(1));
			}
		}
	}
}
