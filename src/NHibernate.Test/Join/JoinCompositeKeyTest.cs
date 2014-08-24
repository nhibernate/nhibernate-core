using log4net;
using NUnit.Framework;
using System;
using System.Collections;
using System.Data;

namespace NHibernate.Test.Join
{
	[TestFixture]
	public class JoinCompositeKeyTest : TestCase
	{
		private static ILog log = LogManager.GetLogger(typeof(JoinCompositeKeyTest));

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get
			{
				return new string[] { 
					"Join.CompositeKey.hbm.xml"
				};
			}
		}

		ISession s;

		protected override void OnSetUp()
		{
			s = OpenSession();

			objectsNeedDeleting.Clear();
		}

		protected override void OnTearDown()
		{
			s.Flush();
			s.Clear();
			try
			{
				// Delete the objects in reverse order because it is
				// less likely to violate foreign key constraints.
				for (int i = objectsNeedDeleting.Count - 1; i >= 0; i--)
				{
					s.Delete(objectsNeedDeleting[i]);
				}
				s.Flush();
			}
			finally
			{
				//t.Commit();
				s.Close();
			}

			s = null;
		}

		private IList objectsNeedDeleting = new ArrayList();

		[Test]
		public void SimpleSaveAndRetrieve()
		{
			EmployeeWithCompositeKey emp = new EmployeeWithCompositeKey(1, 100);
			emp.StartDate = DateTime.Today;
			emp.FirstName = "Karl";
			emp.Surname = "Chu";
			emp.OtherNames = "The Yellow Dart";
			emp.Title = "Rock Star";
			objectsNeedDeleting.Add(emp);

			s.Save(emp);
			s.Flush();
			s.Clear();

			EmployeePk pk = new EmployeePk(1, 100);
			EmployeeWithCompositeKey retrieved = s.Get<EmployeeWithCompositeKey>(pk);

			Assert.IsNotNull(retrieved);
			Assert.AreEqual(emp.StartDate, retrieved.StartDate);
			Assert.AreEqual(emp.FirstName, retrieved.FirstName);
			Assert.AreEqual(emp.Surname, retrieved.Surname);
			Assert.AreEqual(emp.OtherNames, retrieved.OtherNames);
			Assert.AreEqual(emp.Title, retrieved.Title);
		}
	}
}
