using System;

using NHCollection = NHibernate.Collection;
using NHibernate.Type;

namespace NHibernate.Mapping
{
	/// <summary>
	/// An <c>IdentifierBag</c> has a primary key consistenting of just
	/// the identifier column.
	/// </summary>
	public class IdentifierBag : IdentifierCollection
	{
		public IdentifierBag(PersistentClass owner) : base(owner)
		{
		}

		//TODO: need to implement an IdentifierBag in PersistentColleciton.
		public override PersistentCollectionType Type 
		{
			get 
			{ 
				throw new NotImplementedException("need to code IdentifierBag");
				//return TypeFactory.Bag(Role)); 
			}
		}
		
		public override System.Type WrapperClass 
		{
			get 
			{
				throw new NotImplementedException("need to code IdentifierBag");
				//return typeof(NHCollection.IdentifierBag) ; 
			}
		}
		
	}
}
