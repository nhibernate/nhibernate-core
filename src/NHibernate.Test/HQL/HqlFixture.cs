using System.Collections;
using NHibernate.Engine.Query;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.Hql
{
	[TestFixture, Ignore("Not supported yet.")]
	public class HqlFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] {"HQL.Animal.hbm.xml"}; }
		}

		protected HQLQueryPlan CreateQueryPlan(string hql, bool scalar)
		{
			return new HQLQueryPlan(hql, scalar, new CollectionHelper.EmptyMapClass<string, IFilter>(), sessions);
		}

		protected HQLQueryPlan CreateQueryPlan(string hql)
		{
			return CreateQueryPlan(hql, false);
		}

		private static void Check(ReturnMetadata returnMetadata, bool expectingEmptyTypes, bool expectingEmptyAliases)
		{
			Assert.IsNotNull(returnMetadata, "null return metadata");
			Assert.IsNotNull(returnMetadata, "null return metadata - types");
			Assert.AreEqual(1, returnMetadata.ReturnTypes.Length, "unexpected return size");

			if (expectingEmptyTypes)
			{
				Assert.IsNull(returnMetadata.ReturnTypes[0], "non-empty types");
			}
			else
			{
				Assert.IsNotNull(returnMetadata.ReturnTypes[0], "empty types");
			}

			if (expectingEmptyAliases)
			{
				Assert.IsNull(returnMetadata.ReturnAliases, "non-empty aliases");
			}
			else
			{
				Assert.IsNotNull(returnMetadata.ReturnAliases, "empty aliases");
				Assert.IsNotNull(returnMetadata.ReturnAliases[0], "empty aliases");
			}
		}

		[Test]
		public void ReturnMetadata()
		{
			HQLQueryPlan plan;
			plan = CreateQueryPlan("from Animal a");
			Check(plan.ReturnMetadata, false, true);

			plan = CreateQueryPlan("select a as animal from Animal a");
			Check(plan.ReturnMetadata, false, false);

			plan = CreateQueryPlan("from java.lang.Object");
			Check(plan.ReturnMetadata, true, true);

			plan = CreateQueryPlan("select o as entity from java.lang.Object o");
			Check(plan.ReturnMetadata, true, false);
		}
	}
}