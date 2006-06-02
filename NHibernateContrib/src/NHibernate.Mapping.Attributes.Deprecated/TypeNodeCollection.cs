using System;
using System.Collections;

namespace NHibernate.Mapping.Attributes
{
	/// <summary>
	/// Summary description for TypeNodeCollection.
	/// </summary>
	internal class TypeNodeCollection : CollectionBase
	{
		#region Member Variables

		#endregion

		/// <summary>
		/// Class constructor.
		/// </summary>
		public TypeNodeCollection()
		{
		}

		/// <summary>
		/// Adds the node to the collection.
		/// </summary>
		/// <param name="node"></param>
		public void Add( TypeNode node )
		{
			base.InnerList.Add( node );
		}
		
		/// <summary>
		/// Removes the node from the collection.
		/// </summary>
		/// <param name="node"></param>
		public void Remove( TypeNode node )
		{
			base.InnerList.Remove( node );
		}
	}
}
