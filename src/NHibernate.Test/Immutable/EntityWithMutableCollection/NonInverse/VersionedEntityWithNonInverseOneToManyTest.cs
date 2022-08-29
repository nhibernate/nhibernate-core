using System;
using NHibernate.Test.Immutable.EntityWithMutableCollection;
using NUnit.Framework;

namespace NHibernate.Test.Immutable.EntityWithMutableCollection.NonInverse
{
	[TestFixture]
	public class VersionedEntityWithNonInverseOneToManyTest : AbstractEntityWithOneToManyTest
	{
		protected override string[] Mappings
		{
			get
			{
				return new string[] { "Immutable.EntityWithMutableCollection.NonInverse.ContractVariationVersioned.hbm.xml" };
			}
		}
	}
}
