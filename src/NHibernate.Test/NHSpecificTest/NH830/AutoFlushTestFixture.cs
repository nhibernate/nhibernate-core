using System.Collections;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH830
{
	[TestFixture]
	public class AutoFlushTestFixture : BugTestCase
	{
		[Test]
		public void AutoFlushTest()
		{
			ISession sess = OpenSession();
			ITransaction t = sess.BeginTransaction();
			//Setup the test data
			Cat mum = new Cat();
			Cat son = new Cat();
			sess.Save(mum);
			sess.Save(son);
			sess.Flush();

			//reload the data and then setup the many-to-many association
			mum = (Cat) sess.Get(typeof (Cat), mum.Id);
			son = (Cat) sess.Get(typeof (Cat), son.Id);
			mum.Children.Add(son);
			son.Parents.Add(mum);

			//Use criteria API to search first 
			IList result = sess.CreateCriteria(typeof (Cat))
				.CreateAlias("Children", "child")
				.Add(Expression.Eq("child.Id", son.Id))
				.List();
			//the criteria failed to find the mum cat with the child
			Assert.AreEqual(1, result.Count);

			sess.Delete(mum);
			sess.Delete(son);
			t.Commit();
			sess.Close();
		}
	}
}