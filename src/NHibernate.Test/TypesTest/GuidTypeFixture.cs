using System;
using System.Data;
using System.Data.Common;
using NHibernate.Criterion;
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
			Assert.IsTrue(type.IsEqual(lhs, rhs));

			rhs = new Guid("{11234567-abcd-abcd-abcd-0123456789ab}");

			Assert.IsFalse(type.IsEqual(lhs, rhs));
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
														.Add(Expression.Eq("GuidValue", val))
														.UniqueResult();

				Assert.IsNotNull(basic);
				Assert.AreEqual(1, basic.Id);
				Assert.AreEqual(val, basic.GuidValue);

				s.Delete(basic);
				s.Flush();
			}
		}

		[Test]
		public void GetGuidWorksWhenUnderlyingTypeIsRepresentedByString()
		{
			GuidType type = (GuidType)NHibernateUtil.Guid;

			Guid value = Guid.NewGuid();
			DataTable data = new DataTable("test");
			data.Columns.Add("guid", typeof(Guid));
			data.Columns.Add("varchar", typeof(string));
			DataRow row = data.NewRow();
			row["guid"] = value;
			row["varchar"] = value.ToString();
			data.Rows.Add(row);
			var reader = data.CreateDataReader();
			reader.Read();

			using (var s = OpenSession())
			{
				var si = s.GetSessionImplementation();
				Assert.AreEqual(value, type.Get(reader, "guid", si));
				Assert.AreEqual(value, type.Get(reader, 0, si));
				Assert.AreEqual(value, type.Get(reader, "varchar", si));
				Assert.AreEqual(value, type.Get(reader, 1, si));
			}
		}
	}
}