using System;
using NUnit.Framework;
using NHibernate.Test.Immutable.EntityWithMutableCollection;

namespace NHibernate.Test.Immutable.EntityWithMutableCollection.Inverse
{
	[TestFixture]
	public class VersionedEntityWithInverseOneToManyFailureExpectedTest : AbstractEntityWithOneToManyTest
	{
		protected override System.Collections.IList Mappings
		{
			get
			{
				return new string[] { "Immutable.EntityWithMutableCollection.Inverse.ContractVariationVersioned.hbm.xml" };
			}
		}
		
		[Test]
		[Ignore("known to fail with versioned entity with inverse collection")]
		public override void AddExistingOneToManyElementToPersistentEntity()
		{
		}

		[Test]
		[Ignore("known to fail with versioned entity with inverse collection")]
		public override void CreateWithEmptyOneToManyCollectionMergeWithExistingElement()
		{
		}

		[Test]
		[Ignore("known to fail with versioned entity with inverse collection")]
		public override void CreateWithEmptyOneToManyCollectionUpdateWithExistingElement()
		{
		}
		
		[Test]
		[Ignore("known to fail with versioned entity with inverse collection")]
		public override void RemoveOneToManyElementUsingUpdate()
		{
		}

		[Test]
		[Ignore("known to fail with versioned entity with inverse collection")]
		public override void RemoveOneToManyElementUsingMerge()
		{
		}
	}
}
