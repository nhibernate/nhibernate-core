using System;
using System.Collections;
using System.Data;
using NHibernate.Connection;
using NHibernate.DomainModel.NHSpecific;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest
{
	/// <summary>
	/// Summary description for UserTypeFixture.
	/// </summary>
	[TestFixture]
	public class UserTypeFixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[] {"NHSpecific.ClassWithNullColumns.hbm.xml"}; }
		}

		/// <summary>
		/// Does a quick test to make sure that a Property specified with a NullInt32UserType 
		/// persist to the db as a null.
		/// </summary>
		[Test]
		public void InsertNull()
		{
			using (ISession s = OpenSession())
			{
				ClassWithNullColumns userTypeClass = new ClassWithNullColumns();
				userTypeClass.Id = 5;
				userTypeClass.FirstInt32 = 4;
				userTypeClass.SecondInt32 = 0; // with the user type should set value to null

				s.Save(userTypeClass);
				s.Flush();
			}

			// manually read from the db
			IConnectionProvider provider = ConnectionProviderFactory.NewConnectionProvider(cfg.Properties);
			IDbConnection conn = provider.GetConnection();
			IDbCommand cmd = conn.CreateCommand();
			cmd.Connection = conn;
			cmd.CommandText = "select * from usertype";

			IDataReader reader = cmd.ExecuteReader();

			while (reader.Read())
			{
				Assert.AreEqual(5, reader[0]);
				Assert.AreEqual(4, reader[1]);
				Assert.AreEqual(DBNull.Value, reader[2]);
				break;
			}

			conn.Close();

			using (ISession s = OpenSession())
			{
				s.Delete("from ClassWithNullColumns");
				s.Flush();
			}
		}
	}
}