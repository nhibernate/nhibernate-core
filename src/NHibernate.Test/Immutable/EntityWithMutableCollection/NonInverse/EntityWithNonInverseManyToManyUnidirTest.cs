using System;
using NUnit.Framework;
using NHibernate.Test.Immutable.EntityWithMutableCollection;

namespace NHibernate.Test.Immutable.EntityWithMutableCollection.NonInverse
{
	[TestFixture]
	public class EntityWithNonInverseManyToManyUnidirTest : AbstractEntityWithManyToManyTest
	{
		protected override System.Collections.IList Mappings
		{
			get
			{
				return new string[] { "Immutable.EntityWithMutableCollection.NonInverse.ContractVariationUnidir.hbm.xml" };
			}
		}
	}
}
