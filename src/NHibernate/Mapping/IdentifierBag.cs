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

		public override PersistentCollectionType Type 
		{
			get 
			{ 
				return TypeFactory.IdBag(Role); 
			}
		}
		
		public override System.Type WrapperClass 
		{
			get 
			{
				return typeof(NHCollection.IdentifierBag) ; 
			}
		}
		
	}
}
