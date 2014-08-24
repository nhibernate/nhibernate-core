using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.Events.Collections.Association.Bidirectional.OneToMany
{
	[TestFixture]
	public class BidirectionalOneToManyBagCollectionEventFixture : AbstractAssociationCollectionEventFixture
	{
		protected override IList Mappings
		{
			get
			{
				return new string[] { "Events.Collections.Association.Bidirectional.OneToMany.BidirectionalOneToManyBagMapping.hbm.xml" };
			}
		}

		public override IParentWithCollection CreateParent(string name)
		{
			return new ParentWithBidirectionalOneToMany(name);
		}

		public override ICollection<IChild> CreateCollection()
		{
			return new List<IChild>();
		}
	}
}