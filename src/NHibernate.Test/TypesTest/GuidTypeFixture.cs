using System;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// The Unit Tests for the GuidType.
	/// </summary>
	[TestFixture]
	public class GuidTypeFixture : TypeFixtureBase
	{
		protected override string TypeName
		{
			get { return "Guid"; }
		}

		/// <summary>
		/// Verify Equals will correctly determine when the property
		/// is dirty.
		/// </summary>
		[Test]
		public void Equals()
		{
			GuidType type = (GuidType)NHibernateUtil.Guid;

			Guid lhs = new Guid("{01234567-abcd-abcd-abcd-0123456789ab}");
			Guid rhs = new Guid("{01234567-abcd-abcd-abcd-0123456789ab}");
			Assert.IsTrue(type.Equals(lhs, rhs));

			rhs = new Guid("{11234567-abcd-abcd-abcd-0123456789ab}");

			Assert.IsFalse(type.Equals(lhs, rhs));
		}

		[Test]
		public void ReadWrite()
		{
			Guid val = new Guid("{01234567-abcd-abcd-abcd-0123456789ab}");

			GuidClass basic = new GuidClass();
			basic.Id = 1;
			basic.GuidValue = val;

			ISession s = OpenSession();
			s.Save(basic);
			s.Flush();
			s.Close();

			s = OpenSession();
			basic = (GuidClass)s.Load(typeof(GuidClass), 1);

			Assert.AreEqual(val, basic.GuidValue);

			s.Delete(basic);
			s.Flush();
			s.Close();
		}

		[Test]
		public void GuidInWhereClause()
		{
			Guid val = new Guid("{01234567-abcd-abcd-abcd-0123456789ab}");
			GuidClass basic = new GuidClass();

			using (ISession s = OpenSession())
			{
				basic.Id = 1;
				basic.GuidValue = val;

				s.Save(basic);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				basic = (GuidClass)s.CreateCriteria(typeof(GuidClass))
														.Add(Expression.Expression.Eq("GuidValue", val))
														.UniqueResult();

				Assert.IsNotNull(basic);
				Assert.AreEqual(1, basic.Id);
				Assert.AreEqual(val, basic.GuidValue);

				s.Delete(basic);
				s.Flush();
			}
		}
	}
}