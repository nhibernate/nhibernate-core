using System;
using NHibernate.Test.Immutable.EntityWithMutableCollection;
using NUnit.Framework;

namespace NHibernate.Test.Immutable.EntityWithMutableCollection.Inverse
{
	[TestFixture]
	public class VersionedEntityWithInverseOneToManyTest : AbstractEntityWithOneToManyTest
	{
		protected override string[] Mappings
		{
			get
			{
				return new string[] { "Immutable.EntityWithMutableCollection.Inverse.ContractVariationVersioned.hbm.xml" };
			}
		}

		protected override bool CheckUpdateCountsAfterAddingExistingElement()
		{
			return false;
		}

		protected override bool CheckUpdateCountsAfterRemovingElementWithoutDelete()
		{
			return false;
		}
	}
}
