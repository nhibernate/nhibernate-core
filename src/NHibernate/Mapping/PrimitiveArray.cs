namespace NHibernate.Mapping
{
	/// <summary>
	/// A primitive array has a primary key consisting 
	/// of the key columns + index column.
	/// </summary>
	public class PrimitiveArray : Array
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="owner"></param>
		public PrimitiveArray( PersistentClass owner ) : base( owner )
		{
		}

		/// <summary></summary>
		public override bool IsPrimitiveArray
		{
			get { return true; }
		}
	}
}