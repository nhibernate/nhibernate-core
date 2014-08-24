using System;

namespace NHibernate.Mapping
{
	/// <summary>
	/// A primitive array has a primary key consisting 
	/// of the key columns + index column.
	/// </summary>
	[Serializable]
	public class PrimitiveArray : Array
	{
		public PrimitiveArray(PersistentClass owner) : base(owner)
		{
		}

		public override bool IsPrimitiveArray
		{
			get { return true; }
		}
	}
}