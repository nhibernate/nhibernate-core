using System;
using System.Collections;
using System.Data;

using NHibernate.Driver;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;

using NUnit.Framework;

namespace NHibernate.Test.TypeParameters
{
	/// <summary>
	/// Test for parameterizable types.
	/// </summary>
	[TestFixture]
	public class TypeParameterTest : TestCase
	{
		protected override IList Mappings
		{
			get
			{
				return new String[]
					{
						"TypeParameters.Typedef.hbm.xml",
						"TypeParameters.Widget.hbm.xml"
					};
			}
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		[Test]
		public void Save()
		{
			DeleteData();

			ISession s = OpenSession();

			ITransaction t = s.BeginTransaction();

			Widget obj = new Widget();
			obj.ValueThree = 5;

			int id = (int) s.Save(obj);

			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();

			IDriver driver = sessions.ConnectionProvider.Driver;

			IDbConnection connection = s.Connection;
			IDbCommand statement = driver.GenerateCommand(
				CommandType.Text,
				SqlString.Parse("SELECT * FROM STRANGE_TYPED_OBJECT WHERE ID=?"),
				new SqlType[] {SqlTypeFactory.Int32});
			statement.Connection = connection;
			t.Enlist(statement);
			((IDataParameter) statement.Parameters[0]).Value = id;
			IDataReader reader = statement.ExecuteReader();

			Assert.IsTrue(reader.Read(), "A row should have been returned");
			Assert.IsTrue(reader.GetValue(reader.GetOrdinal("VALUE_ONE")) == DBNull.Value,
			              "Default value should have been mapped to null");
			Assert.IsTrue(reader.GetValue(reader.GetOrdinal("VALUE_TWO")) == DBNull.Value,
			              "Default value should have been mapped to null");
			Assert.AreEqual(Convert.ToInt32(reader.GetValue(reader.GetOrdinal("VALUE_THREE"))), 5,
			                "Non-Default value should not be changed");
			Assert.IsTrue(reader.GetValue(reader.GetOrdinal("VALUE_FOUR")) == DBNull.Value,
			              "Default value should have been mapped to null");
			reader.Close();

			t.Commit();
			s.Close();

            DeleteData();
        }

		[Test]
		public void Loading()
		{
			InitData();

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			Widget obj = (Widget) s.CreateQuery("from Widget o where o.Str = :string")
			                      	.SetString("string", "all-normal").UniqueResult();
			Assert.AreEqual(obj.ValueOne, 7, "Non-Default value incorrectly loaded");
			Assert.AreEqual(obj.ValueTwo, 8, "Non-Default value incorrectly loaded");
			Assert.AreEqual(obj.ValueThree, 9, "Non-Default value incorrectly loaded");
			Assert.AreEqual(obj.ValueFour, 10, "Non-Default value incorrectly loaded");

			obj = (Widget) s.CreateQuery("from Widget o where o.Str = :string")
			               	.SetString("string", "all-default").UniqueResult();
			Assert.AreEqual(obj.ValueOne, 1, "Default value incorrectly loaded");
			Assert.AreEqual(obj.ValueTwo, 2, "Default value incorrectly loaded");
			Assert.AreEqual(obj.ValueThree, -1, "Default value incorrectly loaded");
			Assert.AreEqual(obj.ValueFour, -5, "Default value incorrectly loaded");

			t.Commit();
			s.Close();

            DeleteData();
        }

		private void InitData()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			Widget obj = new Widget();
			obj.ValueOne = (7);
			obj.ValueTwo = (8);
			obj.ValueThree = (9);
			obj.ValueFour = (10);
			obj.Str = "all-normal";
			s.Save(obj);

			obj = new Widget();
			obj.ValueOne = (1);
			obj.ValueTwo = (2);
			obj.ValueThree = (-1);
			obj.ValueFour = (-5);
			obj.Str = ("all-default");
			s.Save(obj);

			t.Commit();
			s.Close();
		}

		private void DeleteData()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Delete("from Widget");
			t.Commit();
			s.Close();
		}
	}
}