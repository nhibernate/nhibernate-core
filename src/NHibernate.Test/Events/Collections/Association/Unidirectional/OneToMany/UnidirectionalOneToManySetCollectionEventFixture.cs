using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.Events.Collections.Association.Unidirectional.OneToMany
{
	[TestFixture]
	public class UnidirectionalOneToManySetCollectionEventFixture : AbstractAssociationCollectionEventFixture
	{
		protected override IList Mappings
		{
			get { return new string[] { "Events.Collections.Association.Unidirectional.OneToMany.UnidirectionalOneToManySetMapping.hbm.xml" }; }
		}

		public override IParentWithCollection CreateParent(string name)
		{
			return new ParentWithCollectionOfEntities(name);
		}

		public override ICollection<IChild> CreateCollection()
		{
			return new HashSet<IChild>();
		}
	}
}