using System;
using NUnit.Framework;
using NHibernate.Test.Immutable.EntityWithMutableCollection;

namespace NHibernate.Test.Immutable.EntityWithMutableCollection.Inverse
{
	[TestFixture]
	public class VersionedEntityWithInverseOneToManyJoinFailureExpectedTest : AbstractEntityWithOneToManyTest
	{
		protected override string[] Mappings
		{
			get
			{
				return new string[] { "Immutable.EntityWithMutableCollection.Inverse.ContractVariationVersionedOneToManyJoin.hbm.xml" };
			}
		}
		
		[Test]
		[Ignore("known to fail with inverse collection")]
		// Also [Ignore("Fails. Passes in Hibernate because nullability check on Contract.Party (with JOIN mapping) is skipped due to 'check_nullability' setting not implemented by NH.")]
		public override void AddExistingOneToManyElementToPersistentEntity()
		{
		}

		[Test]
		[Ignore("known to fail with inverse collection")]
		// Also [Ignore("Fails. Passes in Hibernate because nullability check on Contract.Party (with JOIN mapping) is skipped due to 'check_nullability' setting not implemented by NH.")]
		public override void CreateWithEmptyOneToManyCollectionUpdateWithExistingElement()
		{
		}

		[Test]
		[Ignore("known to fail with inverse collection")]
		// Also [Ignore("Fails. Passes in Hibernate because nullability check on Contract.Party (with JOIN mapping) is skipped due to 'check_nullability' setting not implemented by NH.")]
		public override void CreateWithEmptyOneToManyCollectionMergeWithExistingElement()
		{
		}
		
		[Test]
		[Ignore("known to fail with inverse collection")]
		public override void RemoveOneToManyElementUsingUpdate()
		{
		}

		[Test]
		[Ignore("known to fail with inverse collection")]
		public override void RemoveOneToManyElementUsingMerge()
		{
		}
		
		[Test]
		[Ignore("Fails. Passes in Hibernate because nullability check on Contract.Party (with JOIN mapping) is skipped due to 'check_nullability' setting not implemented by NH.")]
		public override void CreateWithNonEmptyOneToManyCollectionOfExisting()
		{
		}
			
		[Test]
		[Ignore("Fails. Passes in Hibernate because nullability check on Contract.Party (with JOIN mapping) is skipped due to 'check_nullability' setting not implemented by NH.")]
		public override void DeleteOneToManyElement()
		{
		}

		[Test]
		[Ignore("Fails. Passes in Hibernate because nullability check on Contract.Party (with JOIN mapping) is skipped due to 'check_nullability' setting not implemented by NH.")]
		public override void RemoveOneToManyElementByDelete()
		{
		}
	}
}
