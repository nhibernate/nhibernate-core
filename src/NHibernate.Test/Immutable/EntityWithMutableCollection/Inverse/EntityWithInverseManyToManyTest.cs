using System;
using NUnit.Framework;
using NHibernate.Test.Immutable.EntityWithMutableCollection;

namespace NHibernate.Test.Immutable.EntityWithMutableCollection.Inverse
{
	[TestFixture]
	public class EntityWithInverseManyToManyTest : AbstractEntityWithManyToManyTest
	{
		protected override System.Collections.IList Mappings
		{
			get
			{
				return new string[] { "Immutable.EntityWithMutableCollection.Inverse.ContractVariation.hbm.xml" };
			}
		}
	}
}
