using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH392
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void UnsavedMinusOneNoNullReferenceException()
		{
			UnsavedValueMinusOne uvmo = new UnsavedValueMinusOne();
			uvmo.Name = "TEST";
			uvmo.UpdateTimestamp = DateTime.Now;

			Assert.AreEqual(-1, uvmo.Id);

			using (ISession s = OpenSession())
			{
				ITransaction tran = s.BeginTransaction();
				try
				{
					s.SaveOrUpdate(uvmo);
					tran.Commit();
				}
				catch
				{
					tran.Rollback();
				}
			}
		}

		protected override void OnTearDown()
		{
			using (ISession s = Sfi.OpenSession())
			{
				// s.Delete("from UnsavedValueMinusOne") loads then delete entities one by one, checking the version.
				// This fails with ODBC & Sql Server 2008+, see NH-1756 test case for more details.
				// Use an in-db query instead.
				s.CreateQuery("delete from UnsavedValueMinusOne").ExecuteUpdate();
			}
		}
	}
}
