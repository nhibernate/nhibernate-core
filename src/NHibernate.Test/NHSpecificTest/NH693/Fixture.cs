using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH693
{
	[TestFixture]
	public class Fixture 
	{
		[Test]
		public void Bug()
		{
			try
			{
				new Configuration()
					.AddResource(GetType().Namespace + ".EmptyTableName.hbm.xml", GetType().Assembly)
					.BuildSessionFactory();
				Assert.Fail("should throw exception");
			}
			catch (MappingException e)
			{
				MappingException ee = e.InnerException as MappingException;
				if (ee == null)
					throw;
				Assert.IsTrue(ee.Message.StartsWith("Could not determine the name of the table for entity"));
			}
		}

		[Test]
		public void SpaceTableName()
		{
			try
			{
				new Configuration()
					.AddResource(GetType().Namespace + ".SpaceTableName.hbm.xml", GetType().Assembly)
					.BuildSessionFactory();
				Assert.Fail("should throw exception");
			}
			catch (MappingException e)
			{
				MappingException ee = e.InnerException as MappingException;
				if (ee == null)
					throw;
				Assert.IsTrue(ee.Message.StartsWith("Could not determine the name of the table for entity"));
			}
		}
	}
}