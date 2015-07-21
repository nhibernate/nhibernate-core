using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.Events.Collections.Association.Unidirectional.OneToMany
{
	[TestFixture]
	public class UnidirectionalOneToManyBagCollectionEventFixture : AbstractAssociationCollectionEventFixture
	{
		protected override IList Mappings
		{
			get { return new string[] { "Events.Collections.Association.Unidirectional.OneToMany.UnidirectionalOneToManyBagMapping.hbm.xml" }; }
		}

		public override IParentWithCollection CreateParent(string name)
		{
			return new ParentWithCollectionOfEntities(name);
		}

		public override ICollection<IChild> CreateCollection()
		{
			return new List<IChild>();
		}
	}
}