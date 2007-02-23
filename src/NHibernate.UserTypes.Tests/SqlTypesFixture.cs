using System;
using System.Collections;
using System.Data.SqlTypes;

using NHibernate.UserTypes.SqlTypes;

using NUnit.Framework;

namespace NHibernate.UserTypes.Tests
{
	[TestFixture]
	public class SqlTypesFixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[] {"SqlTypesClass.hbm.xml"}; }
		}

		[Test]
		public void CRUD()
		{
			SqlTypesClass nullNC = InitAllNull(1);
			SqlTypesClass notnullNC = InitAllValues(2);

			ISession s = sessions.OpenSession();
			s.Save(nullNC);
			s.Save(notnullNC);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();

			Assert.AreEqual(2, s.Find("from SqlTypesClass").Count, "should be 2 in the db");

			IQuery q = s.CreateQuery("from SqlTypesClass as nc where nc.Int32Prop is null");
			IList results = q.List();
			Assert.AreEqual(1, results.Count, "only one null int32 in the db");

			nullNC = (SqlTypesClass) results[0];

			// verify NH did store this fields as null and retrieved them as 
			// the nullable version type.
			Assert.AreEqual(SqlBoolean.Null, nullNC.BooleanProp);
			Assert.AreEqual(SqlByte.Null, nullNC.ByteProp);
			Assert.AreEqual(SqlDateTime.Null, nullNC.DateTimeProp);
			Assert.AreEqual(SqlDecimal.Null, nullNC.DecimalProp);
			Assert.AreEqual(SqlDouble.Null, nullNC.DoubleProp);
			Assert.AreEqual(SqlGuid.Null, nullNC.GuidProp);
			Assert.AreEqual(SqlInt16.Null, nullNC.Int16Prop);
			Assert.AreEqual(SqlInt32.Null, nullNC.Int32Prop);
			Assert.AreEqual(SqlInt64.Null, nullNC.Int64Prop);
			Assert.AreEqual(SqlSingle.Null, nullNC.SingleProp);

			Assert.AreEqual(1, nullNC.Version);

			// don't change anything but flush it - should not increment
			// the version because there were no changes
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			nullNC = (SqlTypesClass) s.Find("from SqlTypesClass")[0];
			Assert.AreEqual(1, nullNC.Version, "no changes to write at last flush - version should not have changed");

			q = s.CreateQuery("from SqlTypesClass as nc where nc.Int32Prop = :int32Prop");
			q.SetParameter("int32Prop", new SqlInt32(Int32.MaxValue), new SqlInt32Type());
			results = q.List();

			Assert.AreEqual(1, results.Count);
			notnullNC = (SqlTypesClass) results[0];

			// change the Int32 properties
			nullNC.Int32Prop = 5;
			notnullNC.Int32Prop = SqlInt32.Null;

			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			nullNC = (SqlTypesClass) s.Load(typeof(SqlTypesClass), 1);
			notnullNC = (SqlTypesClass) s.Load(typeof(SqlTypesClass), 2);

			Assert.IsTrue(5 == nullNC.Int32Prop.Value, "should have actual value");
			Assert.AreEqual(SqlInt32.Null, notnullNC.Int32Prop, "should have 'null' value");


			// clear the table
			s.Delete("from SqlTypesClass");
			s.Flush();
			s.Close();
		}

		public SqlTypesClass InitAllNull(int id)
		{
			SqlTypesClass nc = new SqlTypesClass();
			nc.Id = id;

			nc.BooleanProp = SqlBoolean.Null;
			nc.ByteProp = SqlByte.Null;
			nc.DateTimeProp = SqlDateTime.Null;
			nc.DecimalProp = SqlDecimal.Null;
			nc.DoubleProp = SqlDouble.Null;
			nc.GuidProp = SqlGuid.Null;
			nc.Int16Prop = SqlInt16.Null;
			nc.Int32Prop = SqlInt32.Null;
			nc.Int64Prop = SqlInt64.Null;
			nc.SingleProp = SqlSingle.Null;

			return nc;
		}

		public SqlTypesClass InitAllValues(int id)
		{
			SqlTypesClass nc = new SqlTypesClass();
			nc.Id = id;

			nc.BooleanProp = true;
			nc.ByteProp = (byte) 5;
			nc.DateTimeProp = DateTime.Parse("2004-01-01");
			nc.DecimalProp = 2.45M;
			nc.DoubleProp = 1.7E+3;
			nc.GuidProp = Guid.NewGuid();
			nc.Int16Prop = Int16.MaxValue;
			nc.Int32Prop = Int32.MaxValue;
			nc.Int64Prop = Int64.MaxValue;
			nc.SingleProp = 4.3f;

			return nc;
		}
	}
}