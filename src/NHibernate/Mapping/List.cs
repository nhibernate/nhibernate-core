using System;

using NHibernate.Type;
using NHCollection = NHibernate.Collection;

namespace NHibernate.Mapping 
{
	/// <summary>
	/// A list has a primary key consisting of the key columns + index column
	/// </summary>
	public class List : IndexedCollection 
	{
		public List(PersistentClass owner) : base(owner) 
		{ 
		}

		public override PersistentCollectionType Type 
		{
			get { return TypeFactory.List( Role ); }
		}

		public override System.Type WrapperClass 
		{
			get { return typeof(NHCollection.List); }
		}
		
	}
}
