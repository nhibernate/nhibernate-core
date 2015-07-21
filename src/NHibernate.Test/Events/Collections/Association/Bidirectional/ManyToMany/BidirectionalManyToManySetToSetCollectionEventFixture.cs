using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.Events.Collections.Association.Bidirectional.ManyToMany
{
	[TestFixture]
	public class BidirectionalManyToManySetToSetCollectionEventFixture : AbstractAssociationCollectionEventFixture
	{
		protected override IList Mappings
		{
			get { return new string[] { "Events.Collections.Association.Bidirectional.ManyToMany.BidirectionalManyToManySetToSetMapping.hbm.xml" }; }
		}

		public override IParentWithCollection CreateParent(string name)
		{
			return new ParentWithBidirectionalManyToMany(name);
		}

		public override ICollection<IChild> CreateCollection()
		{
			return new HashSet<IChild>();
		}
	}
}