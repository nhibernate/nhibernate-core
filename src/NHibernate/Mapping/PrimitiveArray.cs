namespace NHibernate.Mapping
{
	/// <summary></summary>
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