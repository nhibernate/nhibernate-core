using NUnit.Framework;
using NHibernate.Cfg;

namespace NHibernate.Test.NHSpecificTest.GH1486
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private  OnFlushDirtyInterceptor interceptor = new OnFlushDirtyInterceptor();

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
			configuration.SetInterceptor(interceptor);
		}


		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					var john = new Person(1, "John", new Address());
					session.Save(john);

					var mary = new Person(2, "Mary", null);
					session.Save(mary);

					session.Flush();
					transaction.Commit();
				}
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

		/// <summary>
		/// The test case was imported from Hibernate HHH-11237 and adjusted for NHibernate. 
		/// </summary>
		[Test]
		public void TestSelectBeforeUpdate()
		{

			using (ISession session = OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					var john = session.Get<Person>(1);
					interceptor.Reset();
					john.Address = null;
					session.Flush();
					Assert.AreEqual(0, interceptor.CallCount);

					interceptor.Reset();
					var mary = session.Get<Person>(2);
					mary.Address = new Address();
					session.Flush();
					Assert.AreEqual(0, interceptor.CallCount);
					transaction.Commit();
				}
			}

			Person johnObj;
			Person maryObj;
			using (ISession session = OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					johnObj = session.Get<Person>(1);
				}
			}

			using (ISession session = OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					maryObj = session.Get<Person>(2);
				}
			}

			using (ISession session = OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					interceptor.Reset();				
					johnObj.Address = null;
					session.Update(johnObj);
					session.Flush();
					Assert.AreEqual(0, interceptor.CallCount);

					interceptor.Reset();
					maryObj.Address = new Address();
					session.Update(maryObj);
					session.Flush();
					Assert.AreEqual(0, interceptor.CallCount);
					transaction.Commit();
				}
			}
		}
	}
}
