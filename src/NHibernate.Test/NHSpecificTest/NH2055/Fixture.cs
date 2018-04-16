using System;
using System.Collections.Generic;
using System.Data;
using NUnit.Framework;
using NHibernate.Tool.hbm2ddl;
using System.Text;
using NHibernate.Cfg;

namespace NHibernate.Test.NHSpecificTest.NH2055
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(NHibernate.Dialect.Dialect dialect)
		{
			return (dialect is Dialect.MsSql2000Dialect);
		}

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
			cfg = configuration;
		}

		[Test] 
		public void CanCreateAndDropSchema() 
		{
            using(var s = Sfi.OpenSession())
            {
                using(var cmd = s.Connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.CommandText = "test_proc1";

                    Assert.AreEqual(1, cmd.ExecuteScalar());

                    cmd.CommandText = "test_proc2";

                    Assert.AreEqual(2, cmd.ExecuteScalar());
                }
            }
		} 

	}
}
