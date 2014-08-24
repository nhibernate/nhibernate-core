using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.Events.Collections.Association.Bidirectional.OneToMany
{
	[TestFixture]
	public class BidirectionalOneToManyBagSubclassCollectionEventFixture : BidirectionalOneToManyBagCollectionEventFixture
	{
		protected override IList Mappings
		{
			get
			{
				return new string[] { "Events.Collections.Association.Bidirectional.OneToMany.BidirectionalOneToManyBagSubclassMapping.hbm.xml" };
			}
		}

		public override IParentWithCollection CreateParent(string name)
		{
			return new ParentWithBidirectionalOneToManySubclass(name);
		}
	}
}