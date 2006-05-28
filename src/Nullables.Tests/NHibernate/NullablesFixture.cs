using System;
using System.Collections;

using NHibernate;

using Nullables;
using NUnit.Framework;

namespace Nullables.Tests.NHibernate
{
	/// <summary>
	/// Summary description for NullablesFixture.
	/// </summary>
	[TestFixture]
	public class NullablesFixture : TestCase
	{
		protected override IList Mappings
		{
			get
			{
				return new string[] { "NHibernate.NullablesClass.hbm.xml" };
			}
		}

		[Test]
		public void CRUD() 
		{
			NullablesClass nullNC = InitAllNull( 1 );
			NullablesClass notnullNC = InitAllValues( 2 );

			ISession s = sessions.OpenSession();
			s.Save( nullNC );
			s.Save( notnullNC );
			s.Flush();
			s.Close();

			s = sessions.OpenSession();

			Assert.AreEqual( 2, s.Find( "from NullablesClass" ).Count, "should be 2 in the db" );

			IQuery q = s.CreateQuery( "from NullablesClass as nc where nc.Int32Prop is null" );
			IList results = q.List();
			Assert.AreEqual( 1, results.Count, "only one null int32 in the db" );
			
			nullNC = (NullablesClass)results[0];

			// verify NH did store this fields as null and retrieved them as 
			// the nullable version type.
			Assert.AreEqual( NullableBoolean.Default, nullNC.BooleanProp );
			Assert.AreEqual( NullableByte.Default, nullNC.ByteProp );
			Assert.AreEqual( NullableDateTime.Default, nullNC.DateTimeProp );
			Assert.AreEqual( NullableDecimal.Default, nullNC.DecimalProp );
			Assert.AreEqual( NullableDouble.Default, nullNC.DoubleProp );
			Assert.AreEqual( NullableGuid.Default, nullNC.GuidProp );
			Assert.AreEqual( NullableInt16.Default, nullNC.Int16Prop);
			Assert.AreEqual( NullableInt32.Default, nullNC.Int32Prop );
			Assert.AreEqual( NullableInt64.Default, nullNC.Int64Prop );
			Assert.AreEqual( NullableSByte.Default, nullNC.SByteProp );
			Assert.AreEqual( NullableSingle.Default, nullNC.SingleProp );

			Assert.AreEqual( 1, nullNC.Version );

			// don't change anything but flush it - should not increment
			// the version because there were no changes
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			nullNC = (NullablesClass)s.Find( "from NullablesClass" )[0];
			Assert.AreEqual( 0, nullNC.Version, "no changes to write at last flush - version should not have changed" );

			q = s.CreateQuery( "from NullablesClass as nc where nc.Int32Prop = :int32Prop" );
			q.SetParameter( "int32Prop", new NullableInt32( Int32.MaxValue) , Nullables.NHibernate.NullablesTypes.NullableInt32 );
			results = q.List();

			Assert.AreEqual( 1, results.Count );
			notnullNC = (NullablesClass)results[0];

			// change the Int32 properties
			nullNC.Int32Prop = 5;
			notnullNC.Int32Prop = null;

			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			nullNC = (NullablesClass)s.Load( typeof(NullablesClass), 1 );
			notnullNC = (NullablesClass)s.Load( typeof(NullablesClass), 2 );

			Assert.IsTrue( 5==nullNC.Int32Prop, "should have actual value" );
			Assert.AreEqual( NullableInt32.Default, notnullNC.Int32Prop, "should have 'null' value" );


			// clear the table
			s.Delete( "from NullablesClass" );
			s.Flush();
			s.Close();
		}

		public NullablesClass InitAllNull(int id) 
		{

			NullablesClass nc = new NullablesClass();
			nc.Id = id;

			nc.BooleanProp = NullableBoolean.Default;
			nc.ByteProp = NullableByte.Default;
			nc.DateTimeProp = NullableDateTime.Default;
			nc.DecimalProp = NullableDecimal.Default;
			nc.DoubleProp = NullableDouble.Default;
			nc.GuidProp = NullableGuid.Default;
			nc.Int16Prop = NullableInt16.Default;
			nc.Int32Prop = NullableInt32.Default;
			nc.Int64Prop = NullableInt64.Default;
			nc.SByteProp = NullableSByte.Default;
			nc.SingleProp = NullableSingle.Default;

			return nc;
		}

		public NullablesClass InitAllValues(int id) 
		{
			NullablesClass nc = new NullablesClass();
			nc.Id = id;

			nc.BooleanProp = true;
			nc.ByteProp = (byte)5;
			nc.DateTimeProp = DateTime.Parse( "2004-01-01" );
			nc.DecimalProp = 2.45M;
			nc.DoubleProp = 1.7E+3;
			nc.GuidProp = Guid.NewGuid();
			nc.Int16Prop = Int16.MaxValue;
			nc.Int32Prop = Int32.MaxValue;
			nc.Int64Prop = Int64.MaxValue;
			nc.SByteProp = SByte.MaxValue;
			nc.SingleProp = 4.3f;

			return nc;
		}

	}
}
