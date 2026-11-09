using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

/* 项目“NHibernate.Test (net48)”的未合并的更改
在此之前:
using NHibernate.Test.NHSpecificTest;
在此之后:
using NHibernate.Test.NHSpecificTest;
using NHibernate;
using NHibernate.Test;
using NHibernate.Test.MappingTest;
using NHibernate.Test.NHSpecificTest.GH3569;
*/
using NHibernate.Test.NHSpecificTest;

namespace NHibernate.Test.NHSpecificTest.GH3569
{
	


	

	

	[TestFixture]
	internal class ManyToOneFixture : BugTestCase
	{
		[Test]
		public void AccessIdOfManyToOneInEmbeddable()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Parent p = new Parent();
			p.ContainedChildren.Add(new ContainedChild(new Child()));
			s.Persist(p);
			t.Commit();
			var list = s.CreateQuery("from Parent p join p.ContainedChildren c where c.Child.Id is not null").List();
			Assert.AreNotEqual(0, list.Count);
			s.Delete(p);
			t.Commit();
			s.Close();
		}
	}
}
