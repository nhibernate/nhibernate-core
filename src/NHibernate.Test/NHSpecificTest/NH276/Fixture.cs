using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH276
{
	[TestFixture]
	public class Fixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] {"NHSpecificTest.NH276.Mappings.hbm.xml"}; }
		}

		/// <summary>
		/// Testing that the syntax of "manytoone.Id" works inside
		/// of an ICriteria.  This was broken in the upgrade to 0.8
		/// </summary>
		[Test]
		public void ManyToOneId()
		{
			Building madison = new Building();
			madison.Id = 1;
			madison.Number = "4800";

			Building college = new Building();
			college.Id = 2;
			college.Number = "6363";

			Office acctg = new Office();
			acctg.Id = 3;
			acctg.Worker = "Bean Counter";
			acctg.Location = college;

			Office hr = new Office();
			hr.Id = 4;
			hr.Worker = "benefits";
			hr.Location = madison;

			Office it = new Office();
			hr.Id = 5;
			it.Worker = "servers";
			it.Location = madison;

			ISession s = OpenSession();
			s.Save(madison);
			s.Save(college);
			s.Save(acctg);
			s.Save(hr);
			s.Save(it);
			s.Flush();
			s.Close();

			s = OpenSession();

			ICriteria c = s.CreateCriteria(typeof(Office));
			c.Add(Expression.Expression.Eq("Location.Id", madison.Id));
			IList results = c.List();

			Assert.AreEqual(2, results.Count, "2 objects");
			foreach (Office office in results)
			{
				Assert.AreEqual(madison.Id, office.Location.Id, "same location as criteria specified");
			}

			c = s.CreateCriteria(typeof(Office));
			c.Add(Expression.Expression.Eq("Location.Id", college.Id));
			results = c.List();

			Assert.AreEqual(1, results.Count, "1 objects");
			foreach (Office office in results)
			{
				Assert.AreEqual(college.Id, office.Location.Id, "same location as criteria specified");
			}

			s.Delete("from Office ");
			s.Delete("from Building");
			s.Flush();
			s.Close();
		}
	}
}