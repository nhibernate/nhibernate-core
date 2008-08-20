using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.Events.Collections.Association.Bidirectional.ManyToMany
{
	[TestFixture, Ignore("Need some more check for timeouts.")]
	public class BidirectionalManyToManyBagToSetCollectionEventFixture : AbstractAssociationCollectionEventFixture
	{
		protected override IList Mappings
		{
			get { return new string[] { "Events.Collections.Association.Bidirectional.ManyToMany.BidirectionalManyToManyBagToSetMapping.hbm.xml" }; }
		}

		public override IParentWithCollection CreateParent(string name)
		{
			return new ParentWithBidirectionalManyToMany(name);
		}

		public override ICollection<IChild> CreateCollection()
		{
			return new List<IChild>();
		}
	}
}