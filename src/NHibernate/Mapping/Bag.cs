using System;
using NHibernate.Type;
using NHibernateBag=NHibernate.Collection.Bag;

namespace NHibernate.Mapping
{
	/// <summary>
	///  bag permits duplicates, so it has no primary key
	/// </summary>
	public class Bag : Collection
	{
		public Bag(PersistentClass owner) : base(owner) {
		}

		public override PersistentCollectionType Type {
			get {
				return null; //TODO: return TypeFactory.Bag( Role );
			}
		}
	
		public override System.Type WrapperClass {
			get {
				return typeof(NHibernateBag);
			}
		}

	}
}
