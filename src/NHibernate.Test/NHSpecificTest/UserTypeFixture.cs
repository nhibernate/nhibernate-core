using System;
using System.Collections;
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
			=> new [] {"NHSpecific.ClassWithNullColumns.hbm.xml"};

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			{
				s.Delete("from ClassWithNullColumns");
				s.Flush();
			}
		}

		/// <summary>
		/// Does a quick test to make sure that a Property specified with a NullInt32UserType 
		/// persist to the db as a null.
		/// </summary>
		[Test]
		public void InsertNull()
		{
			using (var s = OpenSession())
			{
				var userTypeClass = new ClassWithNullColumns
				{
					Id = 5,
					FirstInt32 = 4,
					SecondInt32 = 0
				};
				// with the user type should set 0 value to null

				s.Save(userTypeClass);
				s.Flush();
			}

			// manually read from the db
			using (var provider = ConnectionProviderFactory.NewConnectionProvider(cfg.Properties))
			{
				var conn = provider.GetConnection();
				try
				{
					using (var cmd = conn.CreateCommand())
					{
						cmd.Connection = conn;
						cmd.CommandText = "select * from usertype";

						using (var reader = cmd.ExecuteReader())
						{
							var idOrdinal = reader.GetOrdinal("id");
							var firstOrdinal = reader.GetOrdinal("f_int32");
							var secondOrdinal = reader.GetOrdinal("s_int32");
							while (reader.Read())
							{
								Assert.AreEqual(5, reader[idOrdinal]);
								Assert.AreEqual(4, reader[firstOrdinal]);
								Assert.AreEqual(DBNull.Value, reader[secondOrdinal]);
								break;
							}
						}
					}
				}
				finally
				{
					provider.CloseConnection(conn);
				}
			}
		}
	}
}