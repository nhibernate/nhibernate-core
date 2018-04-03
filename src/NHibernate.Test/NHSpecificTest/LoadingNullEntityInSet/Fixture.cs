using NHibernate.Dialect;

namespace NHibernate.Test.NHSpecificTest.LoadingNullEntityInSet
{
	using System.Collections;
	using Mapping;
	using NUnit.Framework;
	using SqlCommand;
	using Type;
	using TestCase=NHibernate.Test.TestCase;

	[TestFixture]
    public class Fixture : TestCase
    {

        protected override IList Mappings
        {
            get { return new string[] { "NHSpecificTest.LoadingNullEntityInSet.Mappings.hbm.xml" }; }
        }

        protected override string MappingsAssembly
        {
            get { return "NHibernate.Test"; }
        }

	    protected override bool AppliesTo(Dialect.Dialect dialect)
	    {
		    return !(dialect is AbstractHanaDialect); // HANA does not support inserting a row without specifying any column values
	    }

		protected override DebugSessionFactory BuildSessionFactory()
		{
			cfg.GetCollectionMapping(typeof (Employee).FullName + ".Primaries")
				.CollectionTable.Name = "WantedProfessions";
			cfg.GetCollectionMapping(typeof (Employee).FullName + ".Secondaries")
				.CollectionTable.Name = "WantedProfessions";
			try
			{
				return base.BuildSessionFactory();
			}
			finally
			{
				// Restore configuration.
				Configure();
			}
		}

        [Test]
        public void CanHandleNullEntityInList()
        {
            using (ISession sess = OpenSession())
            using (ITransaction tx = sess.BeginTransaction())
            {
                Employee e = new Employee();
                PrimaryProfession ppc = new PrimaryProfession();
				sess.Save(e);
            	sess.Save(ppc);
            	sess.Flush();

				WantedProfession wanted = new WantedProfession();
            	wanted.Id = 15;
				wanted.Employee = e;
				wanted.PrimaryProfession = ppc;

            	sess.Save(wanted);

				tx.Commit();
            }

            using (ISession sess = OpenSession())
            {
            	ICriteria criteria = sess.CreateCriteria(typeof(Employee));
            	criteria.CreateCriteria("Primaries", JoinType.LeftOuterJoin);
				criteria.CreateCriteria("Secondaries", JoinType.LeftOuterJoin);
            	criteria.List();
            }


        	using (ISession sess = OpenSession())
            {
				sess.Delete("from WantedProfession");
				sess.Flush();
				sess.Delete("from PrimaryProfession");
                sess.Flush();
				sess.Delete("from Employee");
                sess.Flush();
            }
        }
    }
}
