using NHibernate.Collection;
using NHibernate.Type;

namespace NHibernate.Mapping
{
	/// <summary>
	/// An array has a primary key consisting of the key columns + index column
	/// </summary>
	public class Array : List
	{
		private System.Type elementClass;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="owner"></param>
		public Array( PersistentClass owner ) : base( owner )
		{
		}

		/// <summary></summary>
		public System.Type ElementClass
		{
			get { return elementClass; }
			set { elementClass = value; }
		}

		/// <summary></summary>
		public override PersistentCollectionType CollectionType
		{
			get { return TypeFactory.Array( Role, ElementClass ); }
		}

		/// <summary></summary>
		public override System.Type WrapperClass
		{
			get { return typeof( ArrayHolder ); }
		}

		/// <summary></summary>
		public override bool IsArray
		{
			get { return true; }
		}


	}
}