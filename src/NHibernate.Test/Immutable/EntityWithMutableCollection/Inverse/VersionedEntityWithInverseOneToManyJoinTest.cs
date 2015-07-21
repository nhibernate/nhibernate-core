using System;
using NUnit.Framework;
using NHibernate.Test.Immutable.EntityWithMutableCollection;

namespace NHibernate.Test.Immutable.EntityWithMutableCollection.Inverse
{
	[TestFixture]
	public class VersionedEntityWithInverseOneToManyJoinTest : AbstractEntityWithOneToManyTest
	{
		protected override System.Collections.IList Mappings
		{
			get
			{
				return new string[] { "Immutable.EntityWithMutableCollection.Inverse.ContractVariationVersionedOneToManyJoin.hbm.xml" };
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
		
		[Test]
		[Ignore("Fails. Passes in Hibernate because nullability check on Contract.Party (with JOIN mapping) is skipped due to 'check_nullability' setting not implemented by NH.")]
		public override void AddExistingOneToManyElementToPersistentEntity()
		{
		}
		
		[Test]
		[Ignore("Fails. Passes in Hibernate because nullability check on Contract.Party (with JOIN mapping) is skipped due to 'check_nullability' setting not implemented by NH.")]
		public override void CreateWithEmptyOneToManyCollectionUpdateWithExistingElement()
		{
		}

		[Test]
		[Ignore("Fails. Passes in Hibernate because nullability check on Contract.Party (with JOIN mapping) is skipped due to 'check_nullability' setting not implemented by NH.")]
		public override void CreateWithEmptyOneToManyCollectionMergeWithExistingElement()
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
