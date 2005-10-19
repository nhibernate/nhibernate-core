using System;
using System.Data;

using NHibernate.DomainModel.NHSpecific;
using NUnit.Framework;


namespace NHibernate.Test.NHSpecificTest 
{
    /// <summary>
    /// Test the ability of GetSetHelperFactory to generate code that can set
    /// a value type from a null.
    /// </summary>
	[TestFixture]
    public class GetSetHelperFixture : TestCase 
	{
		protected override System.Collections.IList Mappings
		{
			get
			{
                return new string[] { "NHSpecific.GetSetHelper.hbm.xml" };
			}
		}

		[Test]
		public void TestDefaultValue() 
		{
			using( ISession s1 = OpenSession() )
			{
                IDbCommand cmd = s1.Connection.CreateCommand();
                cmd.CommandText =
                    "insert into GetSetHelper(ID) values(1)";
                cmd.ExecuteNonQuery();
			}

			try
			{
				// load the object and check default values
				using( ISession s2 = OpenSession() )
				{
					GetSetHelper gs = (GetSetHelper)s2.Load( typeof(GetSetHelper), 1 );

                    Assert.AreEqual(new int(), gs.A);
                    Assert.AreEqual(new TimeSpan(), gs.B);
                    Assert.AreEqual(new bool(), gs.C);
                    Assert.AreEqual(new DateTime(), gs.D);
                    Assert.AreEqual(new short(), gs.E);
                    Assert.AreEqual(new byte(), gs.F);
                    Assert.AreEqual(new float(), gs.G);
                    Assert.AreEqual(new double(), gs.H);
                    Assert.AreEqual(new decimal(), gs.I);
                    Assert.AreEqual(new GetSetHelper.TestEnum(), gs.L);
                    Assert.IsNull(gs.M);
                }
			}
			finally
			{
                ExecuteStatement("delete from GetSetHelper");
			}
		}

	}
}
