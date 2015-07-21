using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.Events.Collections.Association.Bidirectional.ManyToMany
{
	[TestFixture]
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

		public override void UpdateParentOneToTwoSameChildren()
		{
			Assert.Ignore("Not supported");
			// This test need some more deep study if it really work in H3.2
			// because <bag> allow duplication.
		}
	}
}