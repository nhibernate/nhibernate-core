using System;

using Iesi.Collections;

namespace NHibernate.Type
{
	/// <summary>
	/// A <see cref="SetType" /> implemented using a collection that maintains
	/// the order in which elements are inserted into it.
	/// </summary>
	[Serializable]
	public class OrderedSetType : SetType
	{
		/// <summary>
		/// Initializes a new instance of a <see cref="OrderedSetType"/> class
		/// </summary>
		/// <param name="role">The role the persistent collection is in.</param>
		/// <param name="propertyRef"></param>
		public OrderedSetType( string role, string propertyRef )
			: base( role, propertyRef )
		{
		}

		public override object Instantiate()
		{
			return new ListSet();
		}
	}
}