using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;
using NHibernate.Mapping.ByCode.Impl;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Test.NHSpecificTest.NH2875
{
	[TestFixture]
	public class Fixture 
	{
		
        readonly string fxName = "FXXXX";
		[Test]
		public void MappingByCodeAcceptForeignKeyNameOnOneToOne()
		{
            var hbm = new HbmOneToOne();
            var mi = typeof(Entity).GetProperty("Id");
            OneToOneMapper o2o = new OneToOneMapper(mi,hbm);
            o2o.ForeignKey(fxName);
            Assert.AreEqual(fxName, hbm.foreignkey);
		}
	}
}