using System.Data.Common;
using NHibernate.Engine;
using NHibernate.Type;
using NSubstitute;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// The Unit Tests for the BooleanType.
	/// </summary>
	[TestFixture]
	public class BooleanTypeFixture : TypeFixtureBase
	{
		protected override string TypeName
		{
			get { return "Boolean"; }
		}

		/// <summary>
		/// Verify Equals will correctly determine when the property
		/// is dirty.
		/// </summary>
		[Test]
		public void Equals()
		{
			BooleanType type = NHibernateUtil.Boolean;

			Assert.IsTrue(type.IsEqual(true, true));
			Assert.IsTrue(type.IsEqual(false, false));
			Assert.IsFalse(type.IsEqual(true, false));
		}

		[Test]
		public void ReadWrite()
		{
			BooleanClass basic = new BooleanClass();
			basic.Id = 1;
			basic.BooleanValue = true;

			ISession s = OpenSession();
			s.Save(basic);
			s.Flush();
			s.Close();

			s = OpenSession();
			basic = (BooleanClass) s.Load(typeof(BooleanClass), 1);

			Assert.AreEqual(true, basic.BooleanValue);

			s.Delete(basic);
			s.Flush();
			s.Close();
		}

		[Theory]
		public void GetByIndex(bool expected)
		{
			const int index0 = 0;
			const int index1 = 1;
			BooleanType type = NHibernateUtil.Boolean;
			var session = Substitute.For<ISessionImplementor>();
			var reader = Substitute.For<DbDataReader>();
			reader[index0].Returns(expected);
			reader[index1].Returns(expected);

			var result0 = type.Get(reader, index0, session);
			var result1 = type.Get(reader, index1, session);

			Assert.AreEqual(expected, (bool) result0);
			Assert.AreSame(result0, result1);
		}

		[Theory]
		public void GetByName(bool expected)
		{
			const string name0 = "name0";
			const string name1 = "name1";
			BooleanType type = NHibernateUtil.Boolean;
			var session = Substitute.For<ISessionImplementor>();
			var reader = Substitute.For<DbDataReader>();
			reader[name0].Returns(expected);
			reader[name1].Returns(expected);

			var result0 = type.Get(reader, name0, session);
			var result1 = type.Get(reader, name1, session);

			Assert.AreEqual(expected, (bool) result0);
			Assert.AreSame(result0, result1);
		}

		[Test]
		public void DefaultValue()
		{
			BooleanType type = NHibernateUtil.Boolean;

			var result0 = type.DefaultValue;
			var result1 = type.DefaultValue;

			Assert.IsFalse((bool) result0);
			Assert.AreSame(result0, result1);
		}

		[Theory]
		public void StringToObject(bool expected)
		{
			BooleanType type = NHibernateUtil.Boolean;

			var result0 = type.StringToObject(expected.ToString());
			var result1 = type.StringToObject(expected.ToString());

			Assert.AreEqual(expected, result0);
			Assert.AreSame(result0, result1);
		}
	}
}
