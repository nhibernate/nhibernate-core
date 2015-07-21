using System;
using NUnit.Framework;
using NHibernate.Test.Immutable.EntityWithMutableCollection;

namespace NHibernate.Test.Immutable.EntityWithMutableCollection.Inverse
{
	[TestFixture]
	public class VersionedEntityWithInverseOneToManyTest : AbstractEntityWithOneToManyTest
	{
		protected override System.Collections.IList Mappings
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
