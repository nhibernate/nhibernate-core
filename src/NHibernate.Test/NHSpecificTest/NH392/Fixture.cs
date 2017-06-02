using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH392
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH392"; }
		}

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
				s.Delete("from UnsavedValueMinusOne");
				s.Flush();
			}
        }
	}
}