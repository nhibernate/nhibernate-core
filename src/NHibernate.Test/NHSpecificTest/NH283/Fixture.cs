using System;
using System.Reflection;
using System.Collections;

using NHibernate.Cfg;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH283
{
	[TestFixture]
	public class Fixture
	{
		[Test]
		public void ForeignKeyNames()
		{
			Configuration cfg = new Configuration();
			Assembly assembly = Assembly.GetExecutingAssembly();
			cfg.AddResource( "NHibernate.DomainModel.MasterDetail.hbm.xml",
				Assembly.GetAssembly( typeof( NHibernate.DomainModel.Master ) )
				);
			

			string script = string.Join( "\n",
					cfg.GenerateSchemaCreationScript( new Dialect.MsSql2000Dialect() ) );

			Assert.IsTrue( script.IndexOf( "add constraint AA" ) >= 0 );
			Assert.IsTrue( script.IndexOf( "add constraint BB" ) >= 0 );
			Assert.IsTrue( script.IndexOf( "add constraint CC" ) >= 0 );
		}
	}
}
