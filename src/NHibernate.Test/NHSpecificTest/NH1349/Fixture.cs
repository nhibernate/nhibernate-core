using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1349
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using(var session=this.OpenSession())
			{
				using(var tran=session.BeginTransaction())
				{
					string name = "fabio";
					string accNum = DateTime.Now.Ticks.ToString(); ;
					Services newServ = new Services();
					newServ.AccountNumber = accNum;
					newServ.Name = name + " person";
					newServ.Type = (new Random()).Next(0, 9).ToString();

					session.Save(newServ);
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
					session.Delete("from Services");
					tran.Commit();
				}
			}
		}

		[Test]
		public void Can_page_with_formula_property()
		{
			using (var session = this.OpenSession())
			{
				using(var tran=session.BeginTransaction())
				{
					IList ret = session.CreateCriteria(typeof(Services)).SetMaxResults(5).List(); //this breaks
					Assert.That(ret.Count,Is.EqualTo(1));
				}
			}
		}
	}
}
