using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1290
{
	[TestFixture]
	public class Fixture
	{
		[Test]
		public void ExposeBug()
		{
	
			string hbm =
@"<?xml version='1.0' encoding='utf-8' ?> 
<hibernate-mapping xmlns='urn:nhibernate-mapping-2.2'
								namespace='NHibernate.Test.NHSpecificTest.NH1290'
								assembly='NHibernate.Test'>
	<database-object>
		<definition class='AuxType'/>
	</database-object>
</hibernate-mapping>";

			Configuration cfg = new Configuration();
			cfg.AddXmlString(hbm);
			// the mapping is added without problem
		}

	}
}